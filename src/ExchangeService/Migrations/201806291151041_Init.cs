namespace ExchangeService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExchangeRate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        baseCurrency = c.String(nullable: false, maxLength: 3),
                        targetCurrency = c.String(nullable: false, maxLength: 3),
                        exchangeRate = c.Decimal(nullable: false, precision: 10, scale: 5),
                        timestamp = c.DateTime(nullable: false, precision: 7, storeType: "datetime2", defaultValueSql: "GetDate()"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExchangeRate");
        }
    }
}
