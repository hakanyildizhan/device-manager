using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.TrayApp.ViewModel
{
    public class DeviceListViewModel
    {
        public string Type { get; set; }
        public IEnumerable<DeviceItemViewModel> DeviceList { get; set; }
    }
}
