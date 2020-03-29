using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Entity.Context.Entity
{
    public class CommunicationModule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public CommunicationModuleType Type { get; set; }

        public override string ToString()
        {
            return Enum.GetName(typeof(CommunicationModuleType), Type);
        }
    }
}
