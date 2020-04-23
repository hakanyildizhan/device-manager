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
        Task<RegisterAccountResult> RegisterAsync(RegisterModel model);
        Task<bool> LoginAsync(LoginModel model);
        void Logout();
        Task<User> FindUserByNameAsync(string name);
        Task<bool> IsAdminAccountPresentAsync();
        bool IsUserInRole(string userId, string role);
        Task<bool> AddUserToRoleAsync(string userId, string role);
        Task<bool> RemoveUserAsync(string userId);
    }
}
