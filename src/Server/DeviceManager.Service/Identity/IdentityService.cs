using DeviceManager.Entity.Context.Entity;
using DeviceManager.Service.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Identity
{
    public class IdentityService : IIdentityService
    {
        
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
            var result = await _securityManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
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

        public void Logout()
        {
            _authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }

        public async Task<bool> RegisterAsync(RegisterModel model)
        {
            var user = new UserAccount { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _securityManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
