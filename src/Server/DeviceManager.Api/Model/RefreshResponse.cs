using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeviceManager.Api.Model
{
    public class RefreshResponse
    {
        //public Settings Settings { get; set; }
        public Dictionary<string,string> Settings { get; set; }

        /// <summary>
        /// If true, client should get a full list of hardware.
        /// </summary>
        public bool FullUpdateRequired { get; set; }

        public IEnumerable<DeviceSessionInfo> DeviceSessionInfo { get; set; }
    }
}