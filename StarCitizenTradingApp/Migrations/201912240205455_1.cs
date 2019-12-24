namespace StarCitizenTradingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Commodities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PurchaseCost = c.Double(nullable: false),
                        SellPrice = c.Double(nullable: false),
                        LocationId = c.Int(nullable: false),
                        Location_Id = c.Int(),
                        Location_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.Location_Id)
                .ForeignKey("dbo.Locations", t => t.Location_Id1)
                .ForeignKey("dbo.Locations", t => t.LocationId, cascadeDelete: true)
                .Index(t => t.LocationId)
                .Index(t => t.Location_Id)
                .Index(t => t.Location_Id1);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InputVMs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShipId = c.Int(nullable: false),
                        Capital = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RouteVMs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Double(nullable: false),
                        Profit = c.Double(nullable: false),
                        Commodity_Id = c.Int(),
                        PurchaseLocation_Id = c.Int(),
                        SellLocation_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Commodities", t => t.Commodity_Id)
                .ForeignKey("dbo.Locations", t => t.PurchaseLocation_Id)
                .ForeignKey("dbo.Locations", t => t.SellLocation_Id)
                .Index(t => t.Commodity_Id)
                .Index(t => t.PurchaseLocation_Id)
                .Index(t => t.SellLocation_Id);
            
            CreateTable(
                "dbo.Ships",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Capacity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RouteVMs", "SellLocation_Id", "dbo.Locations");
            DropForeignKey("dbo.RouteVMs", "PurchaseLocation_Id", "dbo.Locations");
            DropForeignKey("dbo.RouteVMs", "Commodity_Id", "dbo.Commodities");
            DropForeignKey("dbo.Commodities", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.Commodities", "Location_Id1", "dbo.Locations");
            DropForeignKey("dbo.Commodities", "Location_Id", "dbo.Locations");
            DropIndex("dbo.RouteVMs", new[] { "SellLocation_Id" });
            DropIndex("dbo.RouteVMs", new[] { "PurchaseLocation_Id" });
            DropIndex("dbo.RouteVMs", new[] { "Commodity_Id" });
            DropIndex("dbo.Commodities", new[] { "Location_Id1" });
            DropIndex("dbo.Commodities", new[] { "Location_Id" });
            DropIndex("dbo.Commodities", new[] { "LocationId" });
            DropTable("dbo.Ships");
            DropTable("dbo.RouteVMs");
            DropTable("dbo.InputVMs");
            DropTable("dbo.Locations");
            DropTable("dbo.Commodities");
        }
    }
}
