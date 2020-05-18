using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Service.Model.Setting
{
    public class UsagePromptDuration
    {
        [Required]
        [Range(10, 300, ErrorMessage = "{0} must be between {1} and {2} (5 minutes)")]
        [DisplayName("Hardware usage prompt duration (seconds)")]
        public int Value { get; set; }

        public string Description { get; set; }
    }
}