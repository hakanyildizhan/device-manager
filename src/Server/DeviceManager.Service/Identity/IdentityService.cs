using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using DeviceManager.Service.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Service.Identity
{
    public class IdentityService : IIdentityService
    {
        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        private readonly ApplicationSignInManager _securityManager;
        private readonly ApplicationUserManager _userManager;
        private readonly IAuthenticationManager _authenticationManager;

        public IdentityService(
            ApplicationSignInManager securityManager, 
            ApplicationUserManager userManager,
            IAuthenticationManager authenticationManager)
        {
            _securityManager = securityManager;
            _userManager = userManager;
            _authenticationManager = authenticationManager;
        }

        public async Task<User> FindUserByNameAsync(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            return new User { Name = user.UserName };
        }

        public async Task<bool> LoginAsync(LoginModel model)
        {
            var result = await _securityManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return true;
                case SignInStatus.LockedOut:
                case SignInStatus.RequiresVerification:
                case SignInStatus.Failure:
                default:
                    return false;
            }
        }

        public async Task<bool> IsAdminAccountPresentAsync()
        {
            IList<UserAccount> accounts = await DbContext.Users.ToListAsync();
            if (accounts == null || !accounts.Any())
            {
                return false;
            }

            foreach (var account in accounts)
            {
                if (_userManager.IsInRole(account.Id, "Admin"))
                {
                    return true;
                }
            }

            return false;
        }

        public void Logout()
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        public async Task<RegisterAccountResult> RegisterAsync(RegisterModel model)
        {
            var user = new UserAccount { UserName = model.UserName };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _securityManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                var userAccount = await _userManager.FindByNameAsync(model.UserName);
                return new RegisterAccountResult { Succeeded = true, UserId = userAccount.Id };
            }
            else
            {
                return new RegisterAccountResult { Succeeded = false };
            }
        }

        public async Task<RegisterAccountResult> RegisterWithRoleAsync(RegisterModel userInfo, string role)
        {
            var user = new UserAccount { UserName = userInfo.UserName };
            var result = await _userManager.CreateAsync(user, userInfo.Password);

            if (result.Succeeded)
            {
                var userAccount = await _userManager.FindByNameAsync(userInfo.UserName);
                bool addRoleSuccess = await AddUserToRoleAsync(userAccount.Id, role);

                if (!addRoleSuccess)
                {
                    await RemoveUserAsync(userAccount.Id);
                    return new RegisterAccountResult { Succeeded = false };
                }

                await _securityManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return new RegisterAccountResult { Succeeded = true, UserId = userAccount.Id };
            }
            else
            {
                return new RegisterAccountResult { Succeeded = false };
            }
        }

        public bool IsUserInRole(string userId, string role)
        {
            return _userManager.IsInRole(userId, role);
        }

        public async Task<bool> AddUserToRoleAsync(string userId, string role)
        {
            var result = await _userManager.AddToRoleAsync(userId, role);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserAsync(string userId)
        {
            var user = DbContext.Users.Find(userId);

            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
