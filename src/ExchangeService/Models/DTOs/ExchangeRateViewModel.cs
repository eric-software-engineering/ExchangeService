using ExchangeService.Models.Database;
using System;

namespace ExchangeService.Models.DTOs
{
  public class ExchangeRateViewModel
  {
    public ExchangeRateViewModel(ExchangeRate rate)
    {
      baseCurrency = rate.baseCurrency;
      targetCurrency = rate.targetCurrency;
      exchangeRate = rate.exchangeRate;
      timestamp = rate.timestamp;
    }

    public string baseCurrency { get; set; }
    public string targetCurrency { get; set; }
    public decimal exchangeRate { get; set; }
    public DateTime timestamp { get; set; }
  }
}
