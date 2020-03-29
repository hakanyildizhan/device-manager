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

        [Required]
        public DeviceType DeviceType { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string FirmwareVersion { get; set; }

        [Required]
        public string OrderNumber { get; set; }

        [Required]
        public string Address { get; set; }
        
        public string Address2 { get; set; }

        public virtual HMI HMI { get; set; }

        public virtual CommunicationModule CommModule { get; set; }

        [DefaultValue(1)]
        public AccessType AccessType { get; set; }
    }
}
