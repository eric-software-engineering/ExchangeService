using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace ExchangeService.Models.RedisCache
{
  public class AzureRedisControllerCache : IApiControllerCache
  {
    private IDatabase cache;

    private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
      return ConnectionMultiplexer.Connect("exchange-rate.redis.cache.windows.net:6380,password=8uTut7qhDeYUaOyu5p9yV+Ilboje4W0WGfodAXyxo8A=,ssl=True,abortConnect=False");
    });

    // C#6 Body expression
    public static ConnectionMultiplexer Connection => lazyConnection.Value;

    public async Task<object> Get(string CacheKey)
    {
      if (cache == null)
        cache = Connection.GetDatabase();
      return await Task.Run(() => cache.Get(CacheKey)).ConfigureAwait(false);
    }

    public async Task Put(object value, string CacheKey)
    {
      if (cache == null)
        cache = Connection.GetDatabase();
      await Task.Run(() => cache.Set(CacheKey, value, new TimeSpan(1, 0, 0, 0))).ConfigureAwait(false);
    }
  }
}