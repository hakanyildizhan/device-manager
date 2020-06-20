// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.ComponentModel;

namespace DeviceManager.Service.Model.Setting
{
    public class ServerVersion
    {
        [DisplayName("Server version")]
        public string Value { get; set; }
    }
}