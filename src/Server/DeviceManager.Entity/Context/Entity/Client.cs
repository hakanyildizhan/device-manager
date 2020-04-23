using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Entity.Context.Entity
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string DomainUsername { get; set; }

        public string FriendlyName { get; set; }
    }
}
