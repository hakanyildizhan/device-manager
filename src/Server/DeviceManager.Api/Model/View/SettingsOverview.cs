using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DeviceManager.Service.Model.Setting;

namespace DeviceManager.Api.Model
{
    public class SettingsOverview
    {
        public ServerVersion ServerVersion { get; set; }

        public LastDeviceListUpdate LastDeviceListUpdate { get; set; }

        public RefreshInterval RefreshInterval { get; set; }

        public UsagePromptInterval UsagePromptInterval { get; set; }

        public UsagePromptDuration UsagePromptDuration { get; set; }
    }
}