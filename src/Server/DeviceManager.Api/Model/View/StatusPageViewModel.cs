using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceManager.Api.Model
{
    public class StatusPageViewModel
    {
        public IList<ClientInfo> UserList { get; set; }
        public IList<Device> HardwareList { get; set; }
    }
}