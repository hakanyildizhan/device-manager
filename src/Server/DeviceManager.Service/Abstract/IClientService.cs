using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
