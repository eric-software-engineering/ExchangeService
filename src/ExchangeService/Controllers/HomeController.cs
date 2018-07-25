using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using ExchangeService.Interfaces;
using ExchangeService.Models.Database;
using ExchangeService.Models.RedisCache;
using ExchangeService.ExtensionMethods;
using System.Threading;

namespace ExchangeService.Controllers
{
  public class HomeController : ApiController
  {
    private readonly IRepository<IHTTPClientAdapter> _apiClient;
    private readonly IRepository<DataModel> _dbRepository;
    private readonly IRepository<AzureRedisControllerCache> _cacheRepository;

    // Cannot use "lock" with awaitable tasks. Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
    static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    // SOLID PRINCIPLE: [D]ependency Inversion
    public HomeController(IRepository<IHTTPClientAdapter> apiClient,
                      IRepository<DataModel> dbRepository,
                      IRepository<AzureRedisControllerCache> cacheRepository)
    // C#7 Expression Bodied Constructor
    => (_apiClient, _dbRepository, _cacheRepository) = (apiClient, dbRepository, cacheRepository);

    [HttpGet, Route("api/GetExchangeRate/{baseCurrency}/{targetCurrency}")]
    public async Task<IHttpActionResult> GetExchangeRate(string baseCurrency, string targetCurrency)
    {
      try
      {
        // Load-balancing issue with cache: if the cache is on each server, then they all have their own independent lifecycle
        // Solution: use the Azure Redis, a shared cache with high performance and low cost
        var result = await _cacheRepository.GetData(baseCurrency, targetCurrency).ConfigureAwait(false);

        // The cache is empty or outdated: refreshing it
        if (result == null)
        {
          // Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the semaphore is released.
          await semaphoreSlim.WaitAsync();

          // Maybe another thread fetched the cache in the meantime. Returning it.
          result = await _cacheRepository.GetData(baseCurrency, targetCurrency).ConfigureAwait(false);
          if (result != null)
            return Ok(result.ToViewModel());

          // This is the first thread to find an empty cache. Refilling it.
          var value = await _apiClient.GetAllData();
          result = value.Where(x => x.baseCurrency == baseCurrency && x.targetCurrency == targetCurrency).FirstOrDefault();

          var t1 = _cacheRepository.InsertData(value);
          var t2 = _dbRepository.InsertData(value);

          await Task.WhenAll(t1, t2).ConfigureAwait(false);
        }

        return Ok(result.ToViewModel());
      }
      catch (Exception e)
      {
        e.TraceException();
        // Something didn't work, either the call to the fixer api or the Redis cache. Getting the latest result from the db
        var result = await _dbRepository.GetData(baseCurrency, targetCurrency);
        return Ok(result.ToViewModel());
      }
      finally
      {
        // When the task is ready, release the semaphore. 
        if (semaphoreSlim.CurrentCount == 0)
          semaphoreSlim.Release();
      }
    }
  }
}
