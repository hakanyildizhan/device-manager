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
                    Name = i.GenerateName(),
                    ConnectedModuleInfo = i.ConnectedModuleInfo,
                    IsAvailable = i.IsAvailable,
                    UsedBy = i.UsedBy,
                    UsedByFriendly = i.UsedByFriendly,
                    CheckoutDate = i.CheckoutDate
                }))
            });
        }

        /// <summary>
        /// Generates a name for <see cref="DeviceItemViewModel"/> from this <see cref="Device"/>, i.e.
        /// <para></para>
        /// [Name] [Mlfb] [Address]
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static string GenerateName(this Device device)
        {
            return $"{device.Name}\t{device.HardwareInfo}\t[{device.Address}]";
        }
    }
}