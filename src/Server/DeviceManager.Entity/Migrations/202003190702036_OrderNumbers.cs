namespace DeviceManager.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderNumbers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Device", "OrderNumber", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Device", "OrderNumber");
        }
    }
}
