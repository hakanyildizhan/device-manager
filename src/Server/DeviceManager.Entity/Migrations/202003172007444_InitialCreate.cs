namespace DeviceManager.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Device",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 128),
                        FirmwareVersion = c.String(),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.DeviceType", t => t.Name)
                .Index(t => t.Name);
            
            CreateTable(
                "dbo.DeviceType",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Session",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartedAt = c.DateTime(nullable: false),
                        FinishedAt = c.DateTime(nullable: false),
                        Device_Id = c.Int(),
                        User_DomainUsername = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Device", t => t.Device_Id)
                .ForeignKey("dbo.User", t => t.User_DomainUsername)
                .Index(t => t.Device_Id)
                .Index(t => t.User_DomainUsername);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        DomainUsername = c.String(nullable: false, maxLength: 128),
                        FriendlyName = c.String(),
                    })
                .PrimaryKey(t => t.DomainUsername);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Session", "User_DomainUsername", "dbo.User");
            DropForeignKey("dbo.Session", "Device_Id", "dbo.Device");
            DropForeignKey("dbo.Device", "Name", "dbo.DeviceType");
            DropIndex("dbo.Session", new[] { "User_DomainUsername" });
            DropIndex("dbo.Session", new[] { "Device_Id" });
            DropIndex("dbo.Device", new[] { "Name" });
            DropTable("dbo.User");
            DropTable("dbo.Session");
            DropTable("dbo.DeviceType");
            DropTable("dbo.Device");
        }
    }
}
