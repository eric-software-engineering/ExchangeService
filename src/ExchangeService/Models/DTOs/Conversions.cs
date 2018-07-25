namespace ExchangeService.Models.DTOs
{
  public class Conversions
  {
    public bool success { get; set; }
    public int timestamp { get; set; }
    public string @base { get; set; }
    public string date { get; set; }
    public Rates rates { get; set; }
  }

  public class Rates
  {
    public decimal AUD { get; set; }
    public decimal SEK { get; set; }
    public decimal GBP { get; set; }
    public decimal EUR { get; set; }
    public decimal USD { get; set; }
  }
}
