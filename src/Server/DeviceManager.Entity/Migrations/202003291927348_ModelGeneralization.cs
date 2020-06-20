// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModelGeneralization : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Device", "CommModule_Type", "dbo.CommunicationModule");
            DropForeignKey("dbo.Device", new[] { "HMI_Type", "HMI_FirmwareVersion" }, "dbo.HMI");
            DropIndex("dbo.Device", new[] { "CommModule_Type" });
            DropIndex("dbo.Device", new[] { "HMI_Type", "HMI_FirmwareVersion" });
            AddColumn("dbo.Device", "Group", c => c.String());
            AddColumn("dbo.Device", "HardwareInfo", c => c.String(nullable: false));
            AddColumn("dbo.Device", "ConnectedModuleInfo", c => c.String());
            DropColumn("dbo.Device", "DeviceType");
            DropColumn("dbo.Device", "FirmwareVersion");
            DropColumn("dbo.Device", "OrderNumber");
            DropColumn("dbo.Device", "AccessType");
            DropColumn("dbo.Device", "CommModule_Type");
            DropColumn("dbo.Device", "HMI_Type");
            DropColumn("dbo.Device", "HMI_FirmwareVersion");
            DropTable("dbo.CommunicationModule");
            DropTable("dbo.HMI");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.HMI",
                c => new
                    {
                        Type = c.Int(nullable: false),
                        FirmwareVersion = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Type, t.FirmwareVersion });
            
            CreateTable(
                "dbo.CommunicationModule",
                c => new
                    {
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Type);
            
            AddColumn("dbo.Device", "HMI_FirmwareVersion", c => c.String(maxLength: 128));
            AddColumn("dbo.Device", "HMI_Type", c => c.Int());
            AddColumn("dbo.Device", "CommModule_Type", c => c.Int());
            AddColumn("dbo.Device", "AccessType", c => c.Int(nullable: false));
            AddColumn("dbo.Device", "OrderNumber", c => c.String(nullable: false));
            AddColumn("dbo.Device", "FirmwareVersion", c => c.String(nullable: false));
            AddColumn("dbo.Device", "DeviceType", c => c.Int(nullable: false));
            DropColumn("dbo.Device", "ConnectedModuleInfo");
            DropColumn("dbo.Device", "HardwareInfo");
            DropColumn("dbo.Device", "Group");
            CreateIndex("dbo.Device", new[] { "HMI_Type", "HMI_FirmwareVersion" });
            CreateIndex("dbo.Device", "CommModule_Type");
            AddForeignKey("dbo.Device", new[] { "HMI_Type", "HMI_FirmwareVersion" }, "dbo.HMI", new[] { "Type", "FirmwareVersion" });
            AddForeignKey("dbo.Device", "CommModule_Type", "dbo.CommunicationModule", "Type");
        }
    }
}
