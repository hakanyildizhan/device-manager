﻿using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service.Identity
{
    public class ApplicationUserManager : UserManager<UserAccount>
    {
        public ApplicationUserManager(IUserStore<UserAccount> store, IdentityFactoryOptions<ApplicationUserManager> options)
            : base(store)
        {
            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<UserAccount>(this)
            {
                AllowOnlyAlphanumericUserNames = false
                //RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = IdentityConstants.MIN_PASSWORD_LENGTH,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            // Configure user lockout defaults
            this.UserLockoutEnabledByDefault = false;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            this.MaxFailedAccessAttemptsBeforeLockout = 5;

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                this.UserTokenProvider =
                    new DataProtectorTokenProvider<UserAccount>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserAccount userAccount)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await this.CreateIdentityAsync(userAccount, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
