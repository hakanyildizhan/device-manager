using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Service.Model.Setting
{
    public class UsagePromptInterval
    {
        [Required]
        [Range(1800, 28800, ErrorMessage = "{0} must be between {1} (30 minutes) and {2} (8 hours)")]
        [DisplayName("Hardware usage prompt interval (seconds)")]
        public int Value { get; set; }

        public string Description { get; set; }
    }
}