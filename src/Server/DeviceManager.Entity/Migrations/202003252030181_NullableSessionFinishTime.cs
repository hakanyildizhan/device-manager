// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

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
