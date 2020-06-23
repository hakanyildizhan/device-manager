// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service.Model;
using System.Collections.Generic;

namespace DeviceManager.Api.Model
{
    public class StatusPageViewModel
    {
        public IList<ClientInfo> UserList { get; set; }
        public IList<Device> HardwareList { get; set; }
    }
}