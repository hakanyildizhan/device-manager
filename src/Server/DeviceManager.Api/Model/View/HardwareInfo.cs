using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceManager.Api.Model
{
    public class HardwareInfo
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string PrimaryAddress { get; set; }
        public string SecondaryAddress { get; set; }
        public string Info { get; set; }
        public string ConnectedModuleInfo { get; set; }
    }
}