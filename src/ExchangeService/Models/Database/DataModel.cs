using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeService.Models.Database
{
  public partial class DataModel : DbContext
  {
    public DataModel() : base("name=DataModel") { }

    public virtual DbSet<ExchangeRate> ExchangeRates { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ExchangeRate>()
       .Property(c => c.Id)
       .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

      modelBuilder.Entity<ExchangeRate>().Property(p => p.exchangeRate).HasPrecision(10, 5);
    }
  }
}
