// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;

namespace DeviceManager.Client.Service.Model
{
    /// <summary>
    /// A availability status summary for each device, retrieved from server on each refresh.
    /// </summary>
    public class DeviceSessionInfo
    {
        public int DeviceId { get; set; }
        public bool IsAvailable { get; set; }
        public string UsedBy { get; set; }
        public string UsedByFriendly { get; set; }
        public DateTime? CheckoutDate { get; set; }
    }
}
