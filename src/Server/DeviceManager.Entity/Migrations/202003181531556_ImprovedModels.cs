// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Entity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImprovedModels : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Device", "Name", "dbo.DeviceType");
            DropForeignKey("dbo.Session", "Device_Id", "dbo.Device");
            DropForeignKey("dbo.Session", "User_DomainUsername", "dbo.User");
            DropIndex("dbo.Device", new[] { "Name" });
            DropIndex("dbo.Session", new[] { "Device_Id" });
            DropIndex("dbo.Session", new[] { "User_DomainUsername" });
            CreateTable(
                "dbo.CommunicationModule",
                c => new
                    {
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Type);
            
            CreateTable(
                "dbo.HMI",
                c => new
                    {
                        Type = c.Int(nullable: false),
                        FirmwareVersion = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Type, t.FirmwareVersion });
            
            AddColumn("dbo.Device", "DeviceType", c => c.Int(nullable: false));
            AddColumn("dbo.Device", "Address2", c => c.String());
            AddColumn("dbo.Device", "AccessType", c => c.Int(nullable: false));
            AddColumn("dbo.Device", "CommModule_Type", c => c.Int());
            AddColumn("dbo.Device", "HMI_Type", c => c.Int());
            AddColumn("dbo.Device", "HMI_FirmwareVersion", c => c.String(maxLength: 128));
            AddColumn("dbo.Session", "IsActive", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Device", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Device", "FirmwareVersion", c => c.String(nullable: false));
            AlterColumn("dbo.Device", "Address", c => c.String(nullable: false));
            AlterColumn("dbo.Session", "Device_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Session", "User_DomainUsername", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Device", "CommModule_Type");
            CreateIndex("dbo.Device", new[] { "HMI_Type", "HMI_FirmwareVersion" });
            CreateIndex("dbo.Session", "Device_Id");
            CreateIndex("dbo.Session", "User_DomainUsername");
            AddForeignKey("dbo.Device", "CommModule_Type", "dbo.CommunicationModule", "Type");
            AddForeignKey("dbo.Device", new[] { "HMI_Type", "HMI_FirmwareVersion" }, "dbo.HMI", new[] { "Type", "FirmwareVersion" });
            AddForeignKey("dbo.Session", "Device_Id", "dbo.Device", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Session", "User_DomainUsername", "dbo.User", "DomainUsername", cascadeDelete: true);
            DropTable("dbo.DeviceType");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DeviceType",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Name);
            
            DropForeignKey("dbo.Session", "User_DomainUsername", "dbo.User");
            DropForeignKey("dbo.Session", "Device_Id", "dbo.Device");
            DropForeignKey("dbo.Device", new[] { "HMI_Type", "HMI_FirmwareVersion" }, "dbo.HMI");
            DropForeignKey("dbo.Device", "CommModule_Type", "dbo.CommunicationModule");
            DropIndex("dbo.Session", new[] { "User_DomainUsername" });
            DropIndex("dbo.Session", new[] { "Device_Id" });
            DropIndex("dbo.Device", new[] { "HMI_Type", "HMI_FirmwareVersion" });
            DropIndex("dbo.Device", new[] { "CommModule_Type" });
            AlterColumn("dbo.Session", "User_DomainUsername", c => c.String(maxLength: 128));
            AlterColumn("dbo.Session", "Device_Id", c => c.Int());
            AlterColumn("dbo.Device", "Address", c => c.String());
            AlterColumn("dbo.Device", "FirmwareVersion", c => c.String());
            AlterColumn("dbo.Device", "Name", c => c.String(maxLength: 128));
            DropColumn("dbo.Session", "IsActive");
            DropColumn("dbo.Device", "HMI_FirmwareVersion");
            DropColumn("dbo.Device", "HMI_Type");
            DropColumn("dbo.Device", "CommModule_Type");
            DropColumn("dbo.Device", "AccessType");
            DropColumn("dbo.Device", "Address2");
            DropColumn("dbo.Device", "DeviceType");
            DropTable("dbo.HMI");
            DropTable("dbo.CommunicationModule");
            CreateIndex("dbo.Session", "User_DomainUsername");
            CreateIndex("dbo.Session", "Device_Id");
            CreateIndex("dbo.Device", "Name");
            AddForeignKey("dbo.Session", "User_DomainUsername", "dbo.User", "DomainUsername");
            AddForeignKey("dbo.Session", "Device_Id", "dbo.Device", "Id");
            AddForeignKey("dbo.Device", "Name", "dbo.DeviceType", "Name");
        }
    }
}
