// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service.Model;
using System.Collections.Generic;
using System.Linq;
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
                DeviceList = new ObservableCollection<DeviceItemViewModel>(d.AsEnumerable().OrderBy(di => di.Name).Select(i => new DeviceItemViewModel
                {
                    Id = i.Id,
                    DeviceName = i.Name,
                    Header = i.GenerateHeader(),
                    ConnectedModuleInfo = i.ConnectedModuleInfo,
                    IsAvailable = i.IsAvailable,
                    UsedBy = i.UsedBy,
                    UsedByFriendly = i.UsedByFriendly,
                    CheckoutDate = i.CheckoutDate
                }))
            });
        }

        /// <summary>
        /// Generates a header for <see cref="DeviceItemViewModel"/> from this <see cref="Device"/>, i.e.
        /// <para></para>
        /// [Name] [Mlfb] [Address]
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static string GenerateHeader(this Device device)
        {
            return $"{device.Name}\t{device.HardwareInfo}\t[{device.Address}]";
        }
    }
}