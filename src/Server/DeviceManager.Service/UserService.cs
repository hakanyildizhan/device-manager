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
        private readonly ILogService _logService;

        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public UserService(ILogService<UserService> logService)
        {
            _logService = logService;
        }

        public IList<UserInfo> GetUserInfo()
        {
            try
            {
                IList<UserInfo> userList = new List<UserInfo>();
                DbContext.Users.ToList().ForEach(u => userList.Add(new UserInfo
                {
                    UserName = u.DomainUsername,
                    FriendlyName = u.FriendlyName
                }));
                return userList;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while getting users");
                throw new ServiceException(ex);
            }
        }

        public async Task<RegisterResult> RegisterUserAsync(string domainUserName)
        {
            try
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
            catch (System.Data.DataException ex)
            {
                _logService.LogException(ex, $"DataException occured while registering user {domainUserName}");
                return new RegisterResult
                {
                    Result = RegisterUserResult.Unknown
                };
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, $"Unknown error occured while registering user {domainUserName}");
                throw new ServiceException(ex);
            }
        }

        public async Task<bool> SetFriendlyNameAsync(string domainUserName, string name)
        {
            try
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
            catch (System.Data.DataException ex)
            {
                _logService.LogException(ex, $"DataException occured while setting name for user {domainUserName}");
                return false;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, $"DataException occured while setting name for user {domainUserName}");
                throw new ServiceException(ex);
            }
        }
    }
}
