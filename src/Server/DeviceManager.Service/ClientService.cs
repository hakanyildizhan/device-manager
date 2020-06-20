// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Service
{
    public class ClientService : IClientService
    {
        private readonly ILogService _logService;

        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public ClientService(ILogService<ClientService> logService)
        {
            _logService = logService;
        }

        public IList<ClientInfo> GetClientInfo()
        {
            try
            {
                IList<ClientInfo> clientList = new List<ClientInfo>();
                DbContext.Clients.ToList().ForEach(u => clientList.Add(new ClientInfo
                {
                    UserName = u.DomainUsername,
                    FriendlyName = u.FriendlyName
                }));
                return clientList;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while getting clients");
                throw new ServiceException(ex);
            }
        }

        public async Task<RegisterResult> RegisterClientAsync(string domainUserName)
        {
            try
            {
                Client client = DbContext.Clients.Where(u => u.DomainUsername.Equals(domainUserName)).FirstOrDefault();
                RegisterUserResult result = RegisterUserResult.Unknown;

                if (client == null)
                {
                    client = DbContext.Clients.Add(new Client { DomainUsername = domainUserName });
                    await DbContext.SaveChangesAsync();
                    result = RegisterUserResult.Created;
                }
                else
                {
                    result = RegisterUserResult.AlreadyExists;
                }
                return new RegisterResult
                {
                    User = new ClientInfo
                    {
                        UserName = client.DomainUsername,
                        FriendlyName = client.FriendlyName
                    },

                    Result = result
                };
            }
            catch (System.Data.DataException ex)
            {
                _logService.LogException(ex, $"DataException occured while registering client {domainUserName}");
                return new RegisterResult
                {
                    Result = RegisterUserResult.Unknown
                };
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, $"Unknown error occured while registering client {domainUserName}");
                throw new ServiceException(ex);
            }
        }

        public async Task<bool> SetFriendlyNameAsync(string domainUserName, string name)
        {
            try
            {
                bool clientExists = DbContext.Clients.Any(u => u.DomainUsername.Equals(domainUserName));

                if (!clientExists)
                {
                    return false;
                }
                else
                {
                    Client client = await DbContext.Clients.FindAsync(domainUserName);
                    client.FriendlyName = name;
                    await DbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (System.Data.DataException ex)
            {
                _logService.LogException(ex, $"DataException occured while setting name for client {domainUserName}");
                return false;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, $"DataException occured while setting name for client {domainUserName}");
                throw new ServiceException(ex);
            }
        }
    }
}
