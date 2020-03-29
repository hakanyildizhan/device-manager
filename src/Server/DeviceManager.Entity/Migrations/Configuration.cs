namespace DeviceManager.Entity.Migrations
{
    using DeviceManager.Entity.Context.Entity;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
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

            context.HMIs.AddOrUpdate(h => new { h.Type, h.FirmwareVersion },
                new HMI { Type = HMIModuleType.HMI_HF, FirmwareVersion = "V1.1" },
                new HMI { Type = HMIModuleType.HMI_HF, FirmwareVersion = "V2.0" },
                new HMI { Type = HMIModuleType.HMI_HF, FirmwareVersion = "V3.0" },
                new HMI { Type = HMIModuleType.HMI_ST, FirmwareVersion = "V1.0" }
                );

            context.CommModules.AddOrUpdate(h => h.Type,
                new CommunicationModule { Type = CommunicationModuleType.PN_ST },
                new CommunicationModule { Type = CommunicationModuleType.PN_HF },
                new CommunicationModule { Type = CommunicationModuleType.DP }
                );

            context.Devices.AddOrUpdate(d => d.Name,
            #region Rack 1
                new Device 
                { 
                    Name = "starter_pn_st_1_3rw55v2-1",
                    OrderNumber = "3RW5 513-*HA**",
                    Address = "192.168.90.40", 
                    DeviceType = DeviceType.Starter3RW55,
                    FirmwareVersion = "V2.1",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "V3.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_ST),
                    AccessType = AccessType.CommunicationModule | AccessType.HMI
                },
                new Device
                {
                    Name = "starter_pn_st_6_3rw50",
                    OrderNumber = "3RW5 0**-**B**",
                    Address = "192.168.100.42",
                    DeviceType = DeviceType.Starter3RW50,
                    FirmwareVersion = "V1.0",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "V3.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_ST),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_pn_st_7",
                    OrderNumber = "3RW5 513-*HA**",
                    Address = "192.168.90.45",
                    DeviceType = DeviceType.Starter3RW55,
                    FirmwareVersion = "V2.0",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "V3.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_ST),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_pn_hf_61",
                    OrderNumber = "3RW5 513-*HF**",
                    Address = "192.168.100.61",
                    DeviceType = DeviceType.Starter3RW55F,
                    FirmwareVersion = "V1.0",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "V3.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_HF),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_pn_hf_10",
                    OrderNumber = "3RW5 513-*HA**",
                    Address = "192.168.90.99",
                    DeviceType = DeviceType.Starter3RW55,
                    FirmwareVersion = "T2.1",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "V3.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_HF),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_pn_hf_3",
                    OrderNumber = "3RW5 513-*HA**",
                    Address = "192.168.90.52",
                    DeviceType = DeviceType.Starter3RW55,
                    FirmwareVersion = "V2.0",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "V2.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_HF),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_pn_3rw44_1",
                    OrderNumber = "3RW4 425-*BC**",
                    Address = "192.168.90.31",
                    DeviceType = DeviceType.Starter3RW44,
                    FirmwareVersion = "V1.11",
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "3RW5 513-1HF14 [PB=22]",
                    OrderNumber = "3RW5 513-*HF**",
                    Address = "22",
                    DeviceType = DeviceType.Starter3RW55F,
                    FirmwareVersion = "V2.0",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "V3.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.DP),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "3RW5 513-1HA04 [PB=55]",
                    OrderNumber = "3RW5 513-*HA**",
                    Address = "55",
                    DeviceType = DeviceType.Starter3RW55,
                    FirmwareVersion = "V2.1",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "V3.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.DP),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_plc_1500_1",
                    OrderNumber = "6ES7 516-3AN00-0AB0",
                    Address = "192.168.90.10",
                    Address2 = "10",
                    DeviceType = DeviceType.PLC1500,
                    FirmwareVersion = "V1.7",
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_plc_300_1",
                    OrderNumber = "6ES7 317-2EK14-0AB0",
                    Address = "192.168.90.11",
                    Address2 = "3",
                    DeviceType = DeviceType.PLC300,
                    FirmwareVersion = "V3.2",
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_plc_1200_1",
                    OrderNumber = "6ES7 217-1AG40-0XB0",
                    Address = "192.168.90.12",
                    DeviceType = DeviceType.PLC1200,
                    FirmwareVersion = "R4.1",
                    AccessType = AccessType.CommunicationModule
                },
            #endregion
            #region Rack 2
                new Device
                {
                    Name = "starter_pn_st_5",
                    OrderNumber = "3RW5 0**-**B**",
                    Address = "192.168.100.41",
                    DeviceType = DeviceType.Starter3RW50,
                    FirmwareVersion = "V1.0",
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_ST),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_pn_hf_4",
                    OrderNumber = "3RW5 513-*HA**",
                    Address = "192.168.100.50",
                    DeviceType = DeviceType.Starter3RW55,
                    FirmwareVersion = "T2.1",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "V3.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_HF),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_pn_hf_5",
                    OrderNumber = "3RW5 513-*HA**",
                    Address = "192.168.100.51",
                    DeviceType = DeviceType.Starter3RW55,
                    FirmwareVersion = "V2.0",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "T3.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_HF),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_pn_hf_6",
                    OrderNumber = "3RW5 513-*HA**",
                    Address = "192.168.100.52",
                    DeviceType = DeviceType.Starter3RW55,
                    FirmwareVersion = "T2.1",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "V3.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_HF),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_pn_hf_7",
                    OrderNumber = "3RW5 543-*HF**",
                    Address = "192.168.100.53",
                    DeviceType = DeviceType.Starter3RW55F,
                    FirmwareVersion = "T1.0",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_HF && h.FirmwareVersion == "T3.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_HF),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_pn_hf_8",
                    OrderNumber = "3RW5 548-*HF**",
                    Address = "192.168.100.54",
                    DeviceType = DeviceType.Starter3RW55F,
                    FirmwareVersion = "V1.0",
                    CommModule = context.CommModules.Find(CommunicationModuleType.PN_HF),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "3RW5 213-1AC04 [PB=14]",
                    OrderNumber = "3RW5 2**-**C**",
                    Address = "14",
                    DeviceType = DeviceType.Starter3RW52,
                    FirmwareVersion = "T2.0.2_1",
                    HMI = context.HMIs.Where(h => h.Type == HMIModuleType.HMI_ST && h.FirmwareVersion == "V1.0").FirstOrDefault(),
                    CommModule = context.CommModules.Find(CommunicationModuleType.DP),
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_pn_3rw44_2",
                    OrderNumber = "3RW4 422-*BC**",
                    Address = "192.168.100.30",
                    DeviceType = DeviceType.Starter3RW44,
                    FirmwareVersion = "V1.11",
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_plc_1500_2",
                    OrderNumber = "6ES7 516-3AN01-0AB0",
                    Address = "192.168.100.10",
                    Address2 = "12",
                    DeviceType = DeviceType.PLC1500,
                    FirmwareVersion = "V2.5",
                    AccessType = AccessType.CommunicationModule
                },
                new Device
                {
                    Name = "starter_plc_300_2",
                    OrderNumber = "6ES7 317-2FK14-0AB0",
                    Address = "192.168.100.11",
                    Address2 = "13",
                    DeviceType = DeviceType.PLC300,
                    FirmwareVersion = "V3.2",
                    AccessType = AccessType.CommunicationModule
                }
                #endregion
                );
        }
    }
}
