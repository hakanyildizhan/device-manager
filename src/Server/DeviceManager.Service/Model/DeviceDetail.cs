// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Service.Model
{
    /// <summary>
    /// Entity representation for a device item for Add / Update scenarios.
    /// This model is also used to return device items checked out to a particular user - see <see cref="IDeviceService.GetUserDevices(string)"/>.
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
