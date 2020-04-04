using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service.Model
{
    /// <summary>
    /// Data retrieved from server on each refresh.
    /// </summary>
    public class RefreshResponse
    {
        public Dictionary<string,string> Settings { get; set; }

        /// <summary>
        /// If true, client should get a full list of hardware.
        /// </summary>
        public bool FullUpdateRequired { get; set; }

        public IEnumerable<DeviceSessionInfo> DeviceSessionInfo { get; set; }
    }
}
