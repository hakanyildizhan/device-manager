namespace DeviceManager.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeviceIsActive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Device", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Device", "IsActive");
        }
    }
}
