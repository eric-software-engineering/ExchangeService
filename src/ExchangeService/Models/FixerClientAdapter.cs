using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using ExchangeService.Interfaces;
using ExchangeService.Models.Database;
using System;
using ExchangeService.Models.DTOs;
using ExchangeService.ExtensionMethods;
using System.Configuration;

namespace ExchangeService.Models
{
  // SOLID Principle: [S]ingle Responsibility
  public class FixerClientAdapter : IRepository<IHTTPClientAdapter>
  {
    // Adapter Design Pattern
    private IHTTPClientAdapter _httpClient;

    // C#7 Expression Bodied Constructor
    public FixerClientAdapter(IHTTPClientAdapter httpClient) => (_httpClient) = (httpClient);

    public async Task<List<ExchangeRate>> GetAllData()
    {
      // Using a ConcurrentBag instead of a list as the object will be accessed by multiple threads simultaneously
      var result = new ConcurrentBag<ExchangeRate>();
      var currencies = ConfigurationManager.AppSettings["Currencies"].Split(',');
      var apiKey = ConfigurationManager.AppSettings["ApiKey"];
      var date = DateTime.Now;

      // Parallel API calls for faster performance using an async select
      var tasks = currencies.Select(async item =>
      {
        var targetCurrencies = string.Join(",", currencies.Where(x => x != item).ToList());
        var response = await _httpClient.GetAsync(apiKey, item, targetCurrencies).ConfigureAwait(false);

        //Defensive programming
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Conversions>(responseBody);
        var rates = data.rates;

        // Using reflection to convert the properties of the DTO to values. Scalable: same number of lines of code for 100 currencies
        var ConvertedRates = rates.GetType().GetProperties()
          .Where(pi => pi.PropertyType == typeof(decimal) && pi.GetGetMethod() != null)
          .Select(pi => new ExchangeRate()
          {
            baseCurrency = data.@base,
            targetCurrency = pi.Name,
            exchangeRate = (decimal)pi.GetGetMethod().Invoke(rates, null),
            timestamp = date
          }).Where(x => x.exchangeRate > 0).ToList();

        result.AddRange(ConvertedRates);
      });

      await Task.WhenAll(tasks).ConfigureAwait(false);
      return result.ToList();
    }

    public Task<ExchangeRate> GetData(string baseCurrency, string targetCurrency)
    {
      throw new NotImplementedException();
    }

    public Task InsertData(List<ExchangeRate> rates)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      _httpClient.Dispose();
    }
  }
}