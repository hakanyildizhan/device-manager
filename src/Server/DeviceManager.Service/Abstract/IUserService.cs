using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service
{
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user if it does not already exist.
        /// </summary>
        /// <param name="domainUserName"></param>
        /// <returns></returns>
        Task<RegisterResult> RegisterUserAsync(string domainUserName);

        /// <summary>
        /// Sets a friendly name for given user. Returns false if the user is not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> SetFriendlyNameAsync(string domainUserName, string name);

        /// <summary>
        /// Gets a list of users connected to the server.
        /// </summary>
        /// <returns></returns>
        IList<UserInfo> GetUserInfo();
    }
}
