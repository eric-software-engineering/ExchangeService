using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeService.Interfaces
{
  // SOLID Principle: [I]nterface Segregation
  public interface IHTTPClientAdapter : IDisposable
  {
    Task<HttpResponseMessage> GetAsync(string apiKey, string baseCurrency, string targetCurrencies);
  }
}
