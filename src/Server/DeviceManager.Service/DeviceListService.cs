using DeviceManager.Entity.Context;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Service
{
    public class DeviceListService : IDeviceListService
    {
        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public IEnumerable<Device> GetDevices()
        {
            foreach (var d in DbContext.Devices.AsQueryable())
            {
                yield return new Device
                {
                    Id = d.Id,
                    HardwareInfo = d.HardwareInfo,
                    Address = $"{d.Address}{(string.IsNullOrEmpty(d.Address2) ? "" : $", {d.Address2}")}",
                    ConnectedModuleInfo = d.ConnectedModuleInfo,
                    IsAvailable = !DbContext.Sessions.Any(s => s.IsActive && s.Device.Id == d.Id),
                    Name = d.Name,
                    DeviceGroup = d.Group,
                    UsedBy = DbContext.Sessions.Any(s => s.IsActive && s.Device.Id == d.Id) ?
                                DbContext.Sessions.Where(s => s.IsActive && s.Device.Id == d.Id).FirstOrDefault().User.DomainUsername : null
                };
            }
        }
    }
}
