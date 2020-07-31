namespace DeviceManager.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeviceGroupAndNameUniqueConstraint : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Device", "Group", c => c.String(maxLength: 50));
            AlterColumn("dbo.Device", "Name", c => c.String(nullable: false, maxLength: 50));
            Sql(@"CREATE UNIQUE NONCLUSTERED INDEX
             [IX_DeviceGroupAndName] ON [dbo].[Device]
             (
                [IsActive] ASC,
                [Group] ASC,
                [Name] ASC 
             )
             WHERE ([IsActive]=(1))");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Device", "IX_DeviceGroupAndName");
            AlterColumn("dbo.Device", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Device", "Group", c => c.String());
        }
    }
}
