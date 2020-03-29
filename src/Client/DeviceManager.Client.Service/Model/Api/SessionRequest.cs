using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service.Model.Api
{
    public class SessionRequest
    {
        public string UserName { get; set; }
        public int DeviceId { get; set; }
    }
}
