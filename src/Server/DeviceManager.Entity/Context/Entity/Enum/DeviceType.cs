using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Entity.Context.Entity
{
    public enum DeviceType
    {
        [Description("3RW44")]
        Starter3RW44,

        [Description("3RW50")]
        Starter3RW50,

        [Description("3RW52")]
        Starter3RW52,

        [Description("3RW55")]
        Starter3RW55,

        [Description("3RW55-F")]
        Starter3RW55F,

        [Description("PLC 300")]
        PLC300,

        [Description("PLC 1200")]
        PLC1200,

        [Description("PLC 1500")]
        PLC1500
    }
}
