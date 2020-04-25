using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Model
{
    public class RegisterResult
    {
        public ClientInfo User { get; set; }
        public RegisterUserResult Result { get; set; }
    }
}
