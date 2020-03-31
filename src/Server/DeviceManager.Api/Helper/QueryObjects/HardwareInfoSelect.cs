using DeviceManager.Api.Model;
using DeviceManager.FileParsing;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceManager.Api
{
    public static class HardwareInfoSelect
    {
        public static IList<HardwareInfo> ToHardwareInfo(this IList<Hardware> hardwareList)
        {
            return hardwareList.Select((h, x) => new HardwareInfo
            {
                Name = h.Name,
                PrimaryAddress = h.PrimaryAddress,
                SecondaryAddress = h.SecondaryAddress,
                Group = h.Group,
                Info = h.HardwareInfo,
                ConnectedModuleInfo = h.ConnectedModuleInfo
            }).ToList();
        }

        public static IEnumerable<DeviceImport> ToDeviceImport(this IEnumerable<HardwareInfo> hardwareList)
        {
            return hardwareList.Select((h, x) => new DeviceImport
            {
                Name = h.Name,
                Address = h.PrimaryAddress,
                Address2 = h.SecondaryAddress,
                Group = h.Group,
                ConnectedModuleInfo = h.ConnectedModuleInfo,
                HardwareInfo = h.Info
            });
        }
    }
}