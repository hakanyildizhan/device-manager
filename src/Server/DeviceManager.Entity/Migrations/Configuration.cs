namespace DeviceManager.Entity.Migrations
{
    using DeviceManager.Entity.Context.Entity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Globalization;
    using System.Linq;

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
                new Setting { Name = "lastDeviceListUpdate", Value = DateTime.UtcNow.ToString(CultureInfo.GetCultureInfo("tr-TR")) }
                );

            /*
            context.Devices.AddOrUpdate(d => d.Name,
            #region Rack 1
                new Device 
                { 
                    Name = "starter_pn_st_1_3rw55v2-1",
                    HardwareInfo = "3RW5 513-*HA** V2.1",
                    Address = "192.168.90.40", 
                    Address2 = "192.168.42.91",
                    Group = "3RW5 PN ST",
                    ConnectedModuleInfo = "PN ST, HMI HF V3.0"
                },
                new Device
                {
                    Name = "starter_pn_st_6_3rw50",
                    HardwareInfo = "3RW5 0**-**B** V1.0",
                    Address = "192.168.100.42",
                    Group = "3RW5 PN ST",
                    ConnectedModuleInfo = "PN ST, HMI HF V3.0"
                },
                new Device
                {
                    Name = "starter_pn_st_7",
                    HardwareInfo = "3RW5 513-*HA** V2.0",
                    Address = "192.168.90.45",
                    Group = "3RW5 PN ST",
                    ConnectedModuleInfo = "PN ST, HMI HF V3.0"
                },
                new Device
                {
                    Name = "starter_pn_hf_61",
                    HardwareInfo = "3RW5 513-*HF** V1.0",
                    Address = "192.168.100.61",
                    Group = "3RW5 PN HF",
                    ConnectedModuleInfo = "PN HF, HMI HF V3.0"
                },
                new Device
                {
                    Name = "starter_pn_hf_10",
                    HardwareInfo = "3RW5 513-*HA** T2.1",
                    Address = "192.168.90.99",
                    Group = "3RW5 PN HF",
                    ConnectedModuleInfo = "PN HF, HMI HF V3.0"
                },
                new Device
                {
                    Name = "starter_pn_hf_3",
                    HardwareInfo = "3RW5 513-*HA** V2.0",
                    Address = "192.168.90.52",
                    Group = "3RW5 PN HF",
                    ConnectedModuleInfo = "PN HF, HMI HF V2.0"
                },
                new Device
                {
                    Name = "starter_pn_3rw44_1",
                    HardwareInfo = "3RW4 425-*BC** V1.11",
                    Address = "192.168.90.31",
                    Group = "3RW44"
                },
                new Device
                {
                    Name = "3RW5 513-1HF14 [PB=22]",
                    HardwareInfo = "3RW5 513-*HF** V2.0",
                    Address = "22",
                    Group = "3RW5 DP",
                    ConnectedModuleInfo = "DP, HMI HF V2.0"
                },
                new Device
                {
                    Name = "3RW5 513-1HA04 [PB=55]",
                    HardwareInfo = "3RW5 513-*HA** V2.1",
                    Address = "55",
                    Group = "3RW5 DP",
                    ConnectedModuleInfo = "DP, HMI HF V3.0"
                },
                new Device
                {
                    Name = "starter_plc_1500_1",
                    HardwareInfo = "6ES7 516-3AN00-0AB0 V1.7",
                    Address = "192.168.90.10",
                    Address2 = "10",
                    Group = "PLC",
                },
                new Device
                {
                    Name = "starter_plc_300_1",
                    HardwareInfo = "6ES7 317-2EK14-0AB0 V3.2",
                    Address = "192.168.90.11",
                    Address2 = "3",
                    Group = "PLC"
                },
                new Device
                {
                    Name = "starter_plc_1200_1",
                    HardwareInfo = "6ES7 217-1AG40-0XB0 R4.1",
                    Address = "192.168.90.12",
                    Group = "PLC"
                },
            #endregion
            #region Rack 2
                new Device
                {
                    Name = "starter_pn_st_5",
                    HardwareInfo = "3RW5 0**-**B** V1.0",
                    Address = "192.168.100.41",
                    Group = "3RW5 PN ST",
                    ConnectedModuleInfo = "PN ST"
                },
                new Device
                {
                    Name = "starter_pn_hf_4",
                    HardwareInfo = "3RW5 513-*HA** T2.1",
                    Address = "192.168.100.50",
                    Group = "3RW5 PN HF",
                    ConnectedModuleInfo = "PN HF, HMI HF V3.0"
                },
                new Device
                {
                    Name = "starter_pn_hf_5",
                    HardwareInfo = "3RW5 513-*HA** V2.0",
                    Address = "192.168.100.51",
                    Group = "3RW5 PN HF",
                    ConnectedModuleInfo = "PN HF, HMI HF T3.0"
                },
                new Device
                {
                    Name = "starter_pn_hf_6",
                    HardwareInfo = "3RW5 513-*HA** T2.1",
                    Address = "192.168.100.52",
                    Group = "3RW5 PN HF",
                    ConnectedModuleInfo = "PN HF, HMI HF V3.0"
                },
                new Device
                {
                    Name = "starter_pn_hf_7",
                    HardwareInfo = "3RW5 543-*HF** T1.0",
                    Address = "192.168.100.53",
                    Group = "3RW5 PN HF",
                    ConnectedModuleInfo = "PN HF, HMI HF T3.0"
                },
                new Device
                {
                    Name = "starter_pn_hf_8",
                    HardwareInfo = "3RW5 548-*HF** V1.0",
                    Address = "192.168.100.54",
                    Group = "3RW5 PN HF",
                    ConnectedModuleInfo = "PN HF"
                },
                new Device
                {
                    Name = "3RW5 213-1AC04 [PB=14]",
                    HardwareInfo = "3RW5 2**-**C** T2.0.2_1",
                    Address = "14",
                    Group = "3RW5 DP",
                    ConnectedModuleInfo = "DP, HMI ST"
                },
                new Device
                {
                    Name = "starter_pn_3rw44_2",
                    HardwareInfo = "3RW4 422-*BC** V1.11",
                    Address = "192.168.100.30",
                    Group = "3RW44"
                },
                new Device
                {
                    Name = "starter_plc_1500_2",
                    HardwareInfo = "6ES7 516-3AN01-0AB0 V2.5",
                    Address = "192.168.100.10",
                    Address2 = "12",
                    Group = "PLC"
                },
                new Device
                {
                    Name = "starter_plc_300_2",
                    HardwareInfo = "6ES7 317-2FK14-0AB0 V3.2",
                    Address = "192.168.100.11",
                    Address2 = "13",
                    Group = "PLC"
                }
                #endregion
                );
            */
        }
    }
}
