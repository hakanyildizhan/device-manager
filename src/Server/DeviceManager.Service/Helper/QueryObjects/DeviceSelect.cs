using DeviceManager.Entity.Context.Entity;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service
{
    public static class DeviceSelect
    {
        public static IEnumerable<Entity.Context.Entity.Device> ToDevice(this IEnumerable<DeviceImport> hardwareList)
        {
            return hardwareList.Select((d, x) => new Entity.Context.Entity.Device
            {
                Name = d.Name,
                Address = d.Address,
                Address2 = d.Address2,
                ConnectedModuleInfo = d.ConnectedModuleInfo,
                Group = d.Group,
                HardwareInfo = d.HardwareInfo,
                IsActive = true
            });
        }
    }
}
