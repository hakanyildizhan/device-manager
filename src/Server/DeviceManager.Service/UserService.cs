using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Service
{
    public class UserService : IUserService
    {
        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public async Task<RegisterResult> RegisterUserAsync(string domainUserName)
        {
            User user = DbContext.Users.Where(u => u.DomainUsername.Equals(domainUserName)).FirstOrDefault();
            RegisterUserResult result = RegisterUserResult.Unknown;

            if (user == null)
            {
                user = DbContext.Users.Add(new User { DomainUsername = domainUserName });
                await DbContext.SaveChangesAsync();
                result = RegisterUserResult.Created;
            }
            else
            {
                result = RegisterUserResult.AlreadyExists;
            }
            return new RegisterResult 
            { 
                User = new UserInfo 
                { 
                    UserName = user.DomainUsername, 
                    FriendlyName = user.FriendlyName 
                }, 
                
                Result = result 
            };
        }

        public async Task<bool> SetFriendlyNameAsync(string domainUserName, string name)
        {
            bool userExists = DbContext.Users.Any(u => u.DomainUsername.Equals(domainUserName));
            
            if (!userExists)
            {
                return false;
            }
            else
            {
                User user = await DbContext.Users.FindAsync(domainUserName);
                user.FriendlyName = name;
                await DbContext.SaveChangesAsync();
                return true;
            }
        }
    }
}
