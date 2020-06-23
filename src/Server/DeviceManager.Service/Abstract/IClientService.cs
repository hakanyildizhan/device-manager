// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.Service
{
    public interface IClientService
    {
        /// <summary>
        /// Registers a new client if it does not already exist.
        /// </summary>
        /// <param name="domainUserName"></param>
        /// <returns></returns>
        Task<RegisterResult> RegisterClientAsync(string domainUserName);

        /// <summary>
        /// Sets a friendly name for given client. Returns false if the user is not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> SetFriendlyNameAsync(string domainUserName, string name);

        /// <summary>
        /// Gets a list of clients connected to the server.
        /// </summary>
        /// <returns></returns>
        IList<ClientInfo> GetClientInfo();
    }
}
