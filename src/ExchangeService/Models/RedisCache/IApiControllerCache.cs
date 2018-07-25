using System.Threading.Tasks;

namespace ExchangeService.Models.RedisCache
{
  public interface IApiControllerCache
  {
    Task<object> Get(string CacheKey);
    Task Put(object value, string CacheKey);
  }
}