using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceManager.Api.Model
{
    public class SetFriendlyNameRequest
    {
        public string DomainUserName { get; set; }
        public string FriendlyName { get; set; }
    }
}