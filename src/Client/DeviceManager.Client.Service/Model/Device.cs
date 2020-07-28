// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;

namespace DeviceManager.Client.Service.Model
{
    /// <summary>
    /// This class should be in sync with the server-side model <see cref="DeviceManager.Service.Model.Device"/>.
    /// </summary>
    public class Device
    {
        public int Id { get; set; }
        public string DeviceGroup { get; set; }
        public string Name { get; set; }
        public string HardwareInfo { get; set; }
        public string Address { get; set; }
        public string ConnectedModuleInfo { get; set; }
        public bool IsAvailable { get; set; }
        public string UsedBy { get; set; }
        public string UsedByFriendly { get; set; }
        public DateTime? CheckoutDate { get; set; }
    }
}
