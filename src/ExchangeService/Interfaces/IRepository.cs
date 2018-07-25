using ExchangeService.Models.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeService.Interfaces
{
  // SOLID Principle: [I]nterface Segregation
  public interface IRepository<T>
  {
    Task InsertData(List<ExchangeRate> rates);
    Task<ExchangeRate> GetData(string baseCurrency, string targetCurrency);
    Task<List<ExchangeRate>> GetAllData();
  }
}
