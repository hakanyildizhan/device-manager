using DeviceManager.Entity.Context;
using DeviceManager.Service.Model;
using AccessTypeEnum = DeviceManager.Entity.Context.Entity.AccessType;
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
                    MlfbAndVersion = $"{d.OrderNumber} [{d.FirmwareVersion}]",
                    Address = $"{d.Address}{(string.IsNullOrEmpty(d.Address2) ? "" : $" + {d.Address2}")}",
                    AvailableVia = (d.AccessType.HasFlag(AccessTypeEnum.CommunicationModule) &&
                                    d.AccessType.HasFlag(AccessTypeEnum.HMI) ? "Im + HMI" :
                                        (d.AccessType.HasFlag(AccessTypeEnum.CommunicationModule) ? "Im" : "HMI")),
                    ConnectedModuleInfo = $"{d.CommModule?.ToString()} {d.HMI?.ToString()}",
                    IsAvailable = !DbContext.Sessions.Any(s => s.IsActive && s.Device.Id == d.Id),
                    Name = d.Name,
                    ProductFamily = d.DeviceType.GetDescription(),
                    UsedBy = DbContext.Sessions.Any(s => s.IsActive && s.Device.Id == d.Id) ?
                                DbContext.Sessions.Where(s => s.IsActive && s.Device.Id == d.Id).FirstOrDefault().User.DomainUsername : null
                };
            }
        }
    }
}
