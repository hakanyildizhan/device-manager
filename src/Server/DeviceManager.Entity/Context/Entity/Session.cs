// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeviceManager.Entity.Context.Entity
{
    public class Session
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public virtual Client Client { get; set; }

        [Required]
        public virtual Device Device { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
