// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManager.Entity.Context.Entity
{
    public class Job
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, Index(IsUnique = true)]
        public JobType Type { get; set; }

        [Required, DefaultValue(true)]
        public bool IsIndependent { get; set; }

        [Required, DefaultValue("")]
        public string Schedule { get; set; }

        [Required, DefaultValue(true)]
        public bool IsEnabled { get; set; }
    }
}
