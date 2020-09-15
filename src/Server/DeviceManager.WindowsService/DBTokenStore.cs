// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using DeviceManager.Service;
using DeviceManager.Update;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.WindowsService
{
    /// <summary>
    /// Stores access tokens in / retrieves access tokens from a database.
    /// </summary>
    public class DBTokenStore : ITokenStore
    {
        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public AccessToken GetAccessToken()
        {
            // check if an access token already exists
            var existingToken = DbContext.Settings.Where(s => s.Name == ServiceConstants.Settings.ACCESS_TOKEN).FirstOrDefault();
            var tokenExpiry = DbContext.Settings.Where(s => s.Name == ServiceConstants.Settings.ACCESS_TOKEN_EXPIRY).FirstOrDefault();

            if (existingToken != null && tokenExpiry != null)
            {
                return new AccessToken() { access_token = existingToken.Value, expires_on = tokenExpiry.Value };
            }
            else
            {
                return null;
            }
        }

        public async Task StoreAccessToken(AccessToken token)
        {
            // check if an access token already exists
            var existingToken = DbContext.Settings.Where(s => s.Name == ServiceConstants.Settings.ACCESS_TOKEN).FirstOrDefault();

            if (existingToken != null) // update existing token
            {
                existingToken.Value = token.access_token;

                var tokenExpiry = DbContext.Settings.Where(s => s.Name == ServiceConstants.Settings.ACCESS_TOKEN_EXPIRY).FirstOrDefault();

                if (tokenExpiry != null)
                {
                    tokenExpiry.Value = token.expires_on;
                }
                else
                {
                    DbContext.Settings.Add(new Setting
                    {
                        Name = ServiceConstants.Settings.ACCESS_TOKEN_EXPIRY,
                        Value = token.expires_on
                    });
                }
            }
            else // add new token
            {
                DbContext.Settings.Add(new Setting
                {
                    Name = ServiceConstants.Settings.ACCESS_TOKEN,
                    Value = token.access_token
                });

                DbContext.Settings.Add(new Setting
                {
                    Name = ServiceConstants.Settings.ACCESS_TOKEN_EXPIRY,
                    Value = token.expires_on
                });
            }

            await DbContext.SaveChangesAsync();
        }
    }
}
