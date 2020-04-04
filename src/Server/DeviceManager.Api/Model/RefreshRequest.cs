using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceManager.Api.Model
{
    public class RefreshRequest
    {
        public string LastSuccessfulRefresh { get; set; }
    }
}