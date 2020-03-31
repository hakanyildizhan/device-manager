using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Entity.Context.Entity
{
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Group { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public string HardwareInfo { get; set; }

        [Required]
        public string Address { get; set; }
        
        public string Address2 { get; set; }

        public string ConnectedModuleInfo { get; set; }
        public bool IsActive { get; set; }
    }
}
