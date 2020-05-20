using System.ComponentModel;

namespace DeviceManager.Service.Model.Setting
{
    public class ServerVersion
    {
        [DisplayName("Server version")]
        public string Value { get; set; }
    }
}