using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Model
{
    /// <summary>
    /// Entity representation for a device item for Add / Update scenarios.
    /// </summary>
    public class DeviceDetail
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} must be at least {2}, at most {1} characters long.", MinimumLength = 2)]
        public string Group { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} must be at least {2}, at most {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [DisplayName("Hardware info (Order Number etc.)")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2}, at most {1} characters long.", MinimumLength = 2)]
        public string Info { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} must be at least {2}, at most {1} characters long.", MinimumLength = 2)]
        public string Address { get; set; }

        [DisplayName("Secondary address (if applicable)")]
        [StringLength(50, ErrorMessage = "{0} can be at most {1} characters long.")]
        public string Address2 { get; set; }

        [DisplayName("Connected module info")]
        [StringLength(50, ErrorMessage = "{0} can be at most {1} characters long.")]
        public string ConnectedModuleInfo { get; set; }
    }
}
