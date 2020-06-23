// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

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