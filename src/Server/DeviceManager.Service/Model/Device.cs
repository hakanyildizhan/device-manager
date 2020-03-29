using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Model
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MlfbAndVersion { get; set; }
        public string ProductFamily { get; set; }
        public string Address { get; set; }
        public string ConnectedModuleInfo { get; set; }
        public string AvailableVia { get; set; }
        public bool IsAvailable { get; set; }
        public string UsedBy { get; set; }
    }
}
