using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Service.Model.Setting
{
    public class RefreshInterval
    {
        [Required]
        [Range(30, 600, ErrorMessage = "{0} must be between {1} and {2} seconds.")]
        [DisplayName("Refresh interval (seconds)")]
        public int Value { get; set; }

        public string Description { get; set; }
    }
}