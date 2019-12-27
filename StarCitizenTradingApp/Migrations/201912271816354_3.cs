namespace StarCitizenTradingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InputVMs", "Stops", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InputVMs", "Stops");
        }
    }
}
