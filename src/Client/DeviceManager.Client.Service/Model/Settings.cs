using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service.Model
{
    /// <summary>
    /// Server settings retrieved on each refresh.
    /// </summary>
    public class Settings
    {
        public int RefreshInterval { get; set; }
        public string ServerVersion { get; set; }
        public DateTime LastDeviceListUpdate { get; set; }
    }
}
