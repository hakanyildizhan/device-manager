using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.FileParsing
{
    public class Hardware
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string PrimaryAddress { get; set; }
        public string SecondaryAddress { get; set; }
        public string HardwareInfo { get; set; }
        public string ConnectedModuleInfo { get; set; }
    }
}
