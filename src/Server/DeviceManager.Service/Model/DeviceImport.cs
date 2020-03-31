using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Model
{
    /// <summary>
    /// Used for importing hardware data into the database.
    /// </summary>
    public class DeviceImport
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public string HardwareInfo { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string ConnectedModuleInfo { get; set; }
    }
}
