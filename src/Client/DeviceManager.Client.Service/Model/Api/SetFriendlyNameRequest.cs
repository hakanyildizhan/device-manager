using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service.Model.Api
{
    public class SetFriendlyNameRequest
    {
        public string DomainUserName { get; set; }
        public string FriendlyName { get; set; }
    }
}
