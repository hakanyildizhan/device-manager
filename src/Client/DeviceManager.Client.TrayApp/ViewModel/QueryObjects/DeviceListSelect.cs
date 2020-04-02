using DeviceManager.Client.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public static class DeviceListSelect
    {
        public static IEnumerable<DeviceListViewModel> MapDeviceToViewModel(this IEnumerable<Device> devices)
        {
            return devices.GroupBy(d => d.DeviceGroup).Select(d => new DeviceListViewModel
            {
                Type = d.Key,
                DeviceList = new ObservableCollection<DeviceItemViewModel>(d.AsEnumerable().Select(i => new DeviceItemViewModel 
                {
                    Id = i.Id,
                    
                    Name = $"{i.Name}\t{i.HardwareInfo}\t[{i.Address}]", 
                    // (<Mlfb> <FWver> [<Addr1>]/[<Addr2>]
                    
                    Tooltip = $"Modules: {i.ConnectedModuleInfo}\r\nChecked out by: {(!string.IsNullOrEmpty(i.UsedByFriendly) ? i.UsedByFriendly : i.UsedBy)}", 
                    // Modules: <Im> <FWver>, <HMI> <FWver> \r\nConnect via: <Im>, <Hmi> (only for starters) \r\nUsed by: <userfriendlyname>
                    
                    IsAvailable = i.IsAvailable,
                    UsedBy = i.UsedBy,
                    UsedByFriendly = i.UsedByFriendly
                }))
            });
        }
    }
}
