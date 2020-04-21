using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Identity
{
    public class RegisterAccountResult
    {
        public bool Succeeded { get; set; }
        public string UserId { get; set; }
    }
}
