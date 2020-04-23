using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Api.Model
{
    public class ServerStats
    {
        [DisplayName("Server version")]
        public string ServerVersion { get; set; }

        [DisplayName("Last hardware list update")]
        public string LastDeviceListUpdate { get; set; }
    }
}
