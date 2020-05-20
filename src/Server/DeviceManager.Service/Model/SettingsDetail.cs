using DeviceManager.Service.Model.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
