// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Entity.Migrations
{
    using DeviceManager.Entity.Context.Entity;
    using System;
    using System.Data.Entity.Migrations;
    using System.Globalization;

    internal sealed class Configuration : DbMigrationsConfiguration<DeviceManager.Entity.Context.DeviceManagerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DeviceManager.Entity.Context.DeviceManagerContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            context.Settings.AddOrUpdate(s => s.Name,
                new Setting { Name = "serverVersion", Value = "1.0" },
                new Setting { Name = "refreshInterval", Value = "60", Description = "Refresh interval for clients (seconds)" },
                new Setting { Name = "usagePromptInterval", Value = "3600", Description = "Interval (seconds) at which clients will be prompted to check devices back in" },
                new Setting { Name = "lastDeviceListUpdate", Value = DateTime.UtcNow.ToString(CultureInfo.GetCultureInfo("tr-TR")) },
                new Setting { Name = "usagePromptDuration", Value = "60", Description = "Duration for the reminder prompt to remain active on the client screen" }
                );

            context.Roles.AddOrUpdate(r => r.Name,
                new Microsoft.AspNet.Identity.EntityFramework.IdentityRole { Name = "Admin" });
        }
    }
}
