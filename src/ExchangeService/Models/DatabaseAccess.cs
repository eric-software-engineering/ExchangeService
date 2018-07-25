using System.Linq;
using System.Collections.Generic;
using ExchangeService.Interfaces;
using ExchangeService.Models.Database;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ExchangeService.Models
{
  public class DatabaseAccess : IRepository<DataModel>
  {
    public async Task<ExchangeRate> GetData(string baseCurrency, string targetCurrency)
    {
      using (var dm = new DataModel())
      {
        return await (from b in dm.ExchangeRates
                      where b.baseCurrency == baseCurrency && b.targetCurrency == targetCurrency
                      orderby b.baseCurrency descending
                      select b).FirstOrDefaultAsync().ConfigureAwait(false);
      }
    }

    public async Task InsertData(List<ExchangeRate> rates)
    {
      using (var dm = new DataModel())
      {
        dm.ExchangeRates.AddRange(rates);
        await dm.SaveChangesAsync().ConfigureAwait(false);
      }
    }

    public Task<List<ExchangeRate>> GetAllData()
    {
      throw new System.NotImplementedException();
    }
  }
}
