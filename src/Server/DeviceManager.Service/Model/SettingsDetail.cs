// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service.Model.Setting;

namespace DeviceManager.Service.Model
{
    public class SettingsDetail
    {
        public ServerVersion ServerVersion { get; set; }

        public LastDeviceListUpdate LastDeviceListUpdate { get; set; }

        public RefreshInterval RefreshInterval { get; set; }

        public UsagePromptInterval UsagePromptInterval { get; set; }

        public UsagePromptDuration UsagePromptDuration { get; set; }
    }
}
