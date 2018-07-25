using ExchangeService.Models.Database;
using System.Data.Entity.Migrations;

namespace ExchangeService.Migrations
{

  internal sealed class Configuration : DbMigrationsConfiguration<DataModel>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DataModel context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
