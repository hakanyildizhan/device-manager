// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DefaultValues : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Session", "StartedAt", c => c.DateTime(nullable: false, defaultValue: DateTime.UtcNow));
            AlterColumn("dbo.Device", "AccessType", c => c.Int(nullable: false, defaultValue: 1));

        }
        
        public override void Down()
        {
            AlterColumn("dbo.Session", "StartedAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Device", "AccessType", c => c.Int(nullable: false));
        }
    }
}
