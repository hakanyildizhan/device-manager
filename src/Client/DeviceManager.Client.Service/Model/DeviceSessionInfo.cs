using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service.Model
{
    /// <summary>
    /// A availability status summary for each device, retrieved from server on each refresh.
    /// </summary>
    public class DeviceSessionInfo
    {
        public int DeviceId { get; set; }
        public bool IsAvailable { get; set; }
        public string UsedBy { get; set; }
        public string UsedByFriendly { get; set; }
        public DateTime? CheckoutDate { get; set; }
    }
}
