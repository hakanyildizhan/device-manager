using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Model
{
    public class SettingsUpdate
    {
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
