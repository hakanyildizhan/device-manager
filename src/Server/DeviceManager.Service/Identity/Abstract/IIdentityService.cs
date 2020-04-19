using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Identity
{
    public interface IIdentityService
    {
        Task<bool> RegisterAsync(RegisterModel model);
        Task<bool> LoginAsync(LoginModel model);
        void Logout();
    }
}
