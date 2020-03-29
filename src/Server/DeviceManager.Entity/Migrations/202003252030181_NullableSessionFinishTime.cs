namespace DeviceManager.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableSessionFinishTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Session", "FinishedAt", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Session", "FinishedAt", c => c.DateTime(nullable: false));
        }
    }
}
