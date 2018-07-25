using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeService.Models.Database
{
  [Table("ExchangeRate")]
  [Serializable]
  public class ExchangeRate
  {
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(3)]
    public string baseCurrency { get; set; }
    [Required]
    [StringLength(3)]
    public string targetCurrency { get; set; }
    [Required]
    public decimal exchangeRate { get; set; }
    [Column(TypeName = "datetime2")]
    [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime timestamp { get; set; }
  }
}
