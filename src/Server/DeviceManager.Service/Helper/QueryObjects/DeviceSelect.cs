// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service.Model;
using System.Collections.Generic;
using System.Linq;

namespace DeviceManager.Service
{
    public static class DeviceSelect
    {
        public static IEnumerable<Entity.Context.Entity.Device> ToDevice(this IEnumerable<DeviceImport> importedDeviceList)
        {
            return importedDeviceList.Select((d, x) => new Entity.Context.Entity.Device
            {
                Name = d.Name,
                Address = d.PrimaryAddress,
                Address2 = d.SecondaryAddress,
                ConnectedModuleInfo = d.ConnectedModuleInfo,
                Group = d.Group,
                HardwareInfo = d.Info,
                IsActive = true
            });
        }
    }
}
