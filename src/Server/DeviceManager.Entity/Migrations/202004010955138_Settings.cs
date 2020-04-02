namespace DeviceManager.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Settings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Setting",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        Value = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Setting");
        }
    }
}
