namespace StarCitizenTradingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Commodities", "RestockTimer", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Commodities", "RestockTimer");
        }
    }
}
