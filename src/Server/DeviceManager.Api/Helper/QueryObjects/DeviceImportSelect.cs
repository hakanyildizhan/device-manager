// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.FileParsing;
using DeviceManager.Service.Model;
using System.Collections.Generic;
using System.Linq;

namespace DeviceManager.Api
{
    public static class DeviceImportSelect
    {
        public static IList<DeviceImport> ToDeviceImport(this IList<DeviceItem> deviceItemList)
        {
            return deviceItemList.Select((h, x) => new DeviceImport
            {
                Name = h.Name,
                PrimaryAddress = h.PrimaryAddress,
                SecondaryAddress = h.SecondaryAddress,
                Group = h.Group,
                Info = h.HardwareInfo,
                ConnectedModuleInfo = h.ConnectedModuleInfo
            }).ToList();
        }
    }
}