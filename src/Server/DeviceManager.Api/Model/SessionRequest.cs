using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceManager.Api.Model
{
    public class SessionRequest
    {
        public string UserName { get; set; }
        public int DeviceId { get; set; }
    }
}