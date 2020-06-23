// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;

namespace DeviceManager.Client.Service.Model
{
    /// <summary>
    /// Server settings retrieved on each refresh.
    /// </summary>
    public class Settings
    {
        public int RefreshInterval { get; set; }
        public string ServerVersion { get; set; }
        public DateTime LastDeviceListUpdate { get; set; }
    }
}
