using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Model
{
    public class Settings
    {
        public int RefreshInterval { get; set; }
        public string ServerVersion { get; set; }
        public DateTime LastDeviceListUpdate { get; set; }
    }
}
