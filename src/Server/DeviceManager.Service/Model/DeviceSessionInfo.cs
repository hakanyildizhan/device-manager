using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Model
{
    public class DeviceSessionInfo
    {
        public int DeviceId { get; set; }
        public bool IsAvailable { get; set; }
        public string UsedBy { get; set; }
        public string UsedByFriendly { get; set; }
    }
}
