// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManager.Entity.Context.Entity
{
    public class UpdateJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual Job Job { get; set; }

        [Required]
        public JobStatus Status { get; set; }

        public string NewVersion { get; set; }
        
        public string Uri { get; set; }
        
        public string Info { get; set; }

        public DateTime LastUpdate { get; set; }
    }
}
