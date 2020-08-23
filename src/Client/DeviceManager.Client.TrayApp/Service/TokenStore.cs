// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using DeviceManager.Update;
using System.Threading.Tasks;

namespace DeviceManager.Client.TrayApp.Service
{
    public class TokenStore : ITokenStore
    {
        private readonly IConfigurationService _configService;

        public TokenStore(IConfigurationService configService)
        {
            _configService = configService;
        }

        public AccessToken GetAccessToken()
        {
            string accessToken = _configService.Get(ServiceConstants.Settings.ACCESS_TOKEN);
            string accessTokenExpiry = _configService.Get(ServiceConstants.Settings.ACCESS_TOKEN_EXPIRY);

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(accessTokenExpiry))
            {
                return null;
            }

            return new AccessToken() { access_token = accessToken, expires_on = accessTokenExpiry };
        }

        public async Task StoreAccessToken(AccessToken token)
        {
            await _configService.SetAsync(ServiceConstants.Settings.ACCESS_TOKEN, token.access_token);
            await _configService.SetAsync(ServiceConstants.Settings.ACCESS_TOKEN_EXPIRY, token.expires_on);
        }
    }
}
