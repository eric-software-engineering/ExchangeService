using ExchangeService.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeService.Models
{
  // SOLID Principle: [S]ingle Responsibility
  public class HTTPClientAdapter : IHTTPClientAdapter
  {
    private HttpClient _httpClient = new HttpClient();

    public async Task<HttpResponseMessage> GetAsync(string apiKey, string baseCurrency, string targetCurrencies)
    {
      // C# 6: String interpolation
      return await _httpClient.GetAsync($"http://data.fixer.io/api/latest?access_key={apiKey}&base={baseCurrency}&symbols={targetCurrencies}").ConfigureAwait(false);
    }

    public void Dispose()
    {
      _httpClient.Dispose();
    }
  }
}