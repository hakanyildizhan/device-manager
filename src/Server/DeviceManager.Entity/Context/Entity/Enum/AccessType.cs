using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Entity.Context.Entity
{
    [Flags]
    public enum AccessType
    {
        CommunicationModule = 1,
        HMI = 2
    }
}
