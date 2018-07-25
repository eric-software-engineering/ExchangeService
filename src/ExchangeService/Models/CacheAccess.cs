using System;
using System.Linq;
using System.Collections.Generic;
using ExchangeService.Interfaces;
using ExchangeService.Models.Database;
using ExchangeService.Models.RedisCache;
using System.Threading.Tasks;

namespace ExchangeService.Models
{
  public class CacheAccess : IRepository<AzureRedisControllerCache>
  {
    public async Task<ExchangeRate> GetData(string baseCurrency, string targetCurrency)
    {
      var cacheRep = new AzureRedisControllerCache();
      ExchangeRate result = null;

      // C#7 pattern matching
      if (await cacheRep.Get("ExchangeRate").ConfigureAwait(false) is List<ExchangeRate> cache)
        result = cache.Where(x => x.baseCurrency == baseCurrency && x.targetCurrency == targetCurrency && x.timestamp > DateTime.Now.AddMinutes(-10)).FirstOrDefault();

      return result;
    }

    public async Task InsertData(List<ExchangeRate> rates)
    {
      var cacheRep = new AzureRedisControllerCache();
      await cacheRep.Put(rates, "ExchangeRate").ConfigureAwait(false);
    }

    public Task<List<ExchangeRate>> GetAllData()
    {
      throw new NotImplementedException();
    }
  }
}
