// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Threading.Tasks;

namespace DeviceManager.Service.Identity
{
    public interface IIdentityService
    {
        /// <summary>
        /// Registers a user before signing them in.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<RegisterAccountResult> RegisterAsync(RegisterModel model);

        /// <summary>
        /// Registers a user, adds specified role to them before signing them in.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<RegisterAccountResult> RegisterWithRoleAsync(RegisterModel userInfo, string role);

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> LoginAsync(LoginModel model);

        /// <summary>
        /// Logs out a user.
        /// </summary>
        void Logout();

        /// <summary>
        /// Finds a user by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<User> FindUserByNameAsync(string name);

        /// <summary>
        /// Checks if a user is present with the role "Admin".
        /// </summary>
        /// <returns></returns>
        Task<bool> IsAdminAccountPresentAsync();

        /// <summary>
        /// Checks if a user has the specified role.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        bool IsUserInRole(string userId, string role);

        /// <summary>
        /// Adds a user to the specified role.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<bool> AddUserToRoleAsync(string userId, string role);

        /// <summary>
        /// Removes a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> RemoveUserAsync(string userId);
    }
}
