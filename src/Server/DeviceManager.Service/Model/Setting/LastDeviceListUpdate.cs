using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeviceManager.Service.Model.Setting
{
    public class LastDeviceListUpdate
    {
        [DisplayName("Last hardware list update")]
        public string Value { get; set; }
    }
}