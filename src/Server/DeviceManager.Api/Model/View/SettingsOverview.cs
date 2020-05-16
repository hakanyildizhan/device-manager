using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeviceManager.Api.Model
{
    public class SettingsOverview
    {
        [DisplayName("Server version")]
        public string ServerVersion { get; set; }

        [DisplayName("Last hardware list update")]
        public string LastDeviceListUpdate { get; set; }

        [Required]
        [Range(30, 600, ErrorMessage = "{0} must be between {1} and {2} seconds.")]
        [DisplayName("Refresh interval (seconds)")]
        public int RefreshInterval { get; set; }

        [Required]
        [Range(300, 28800, ErrorMessage = "{0} must be between {1} (5 minutes) and {2} (8 hours)")]
        [DisplayName("Hardware usage prompt interval (seconds)")]
        public int UsagePromptInterval { get; set; }
    }
}