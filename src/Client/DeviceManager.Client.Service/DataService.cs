// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service.Model;
using DeviceManager.Client.Service.Model.Api;
using DeviceManager.Common;
using Flurl;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service
{
    public class DataService : IDataService
    {
        static HttpClient client = new HttpClient();
        private readonly string _baseApiAddress;
        private readonly ILogService _logService;
        private readonly IConfigurationService _configService;

        public DataService(ILogService<DataService> logService, IConfigurationService configService)
        {
            _logService = logService;
            _configService = configService;
            
            Task<string> getServerAddressTask = Task.Run(async () => await GetApiAddress());
            _baseApiAddress = Url.Combine(getServerAddressTask.Result, "api");
        }

        private async Task<string> GetApiAddress()
        {
#if DEBUG
            string serverAddress = "http://localhost/DeviceManager";
#else
            string serverAddress = await _configService.GetServerAddressAsync();
#endif
            return serverAddress;
        }

        public async Task<ApiCallResult> CheckDeviceAvailabilityAsync(int deviceId)
        {
            string uri = Url.Combine(_baseApiAddress, "session/isdeviceavailable");
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, deviceId);
                if (response.IsSuccessStatusCode)
                {
                    bool result = await response.Content.ReadAsAsync<bool>();
                    return result ? ApiCallResult.Success : ApiCallResult.Failure;
                }
                else
                {
                    return ApiCallResult.Failure;
                }
            }
            catch (HttpRequestException ex)
            {
                _logService.LogException(ex, "Cannot contact server. Could not check device availability");
                return ApiCallResult.NotReachable;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while checking device availability");
                return ApiCallResult.Unknown;
            }
        }

        public async Task<ApiCallResult> CheckinDeviceAsync(string userName, int deviceId)
        {
            string uri = Url.Combine(_baseApiAddress, "session/end");
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, new SessionRequest
                {
                    UserName = userName,
                    DeviceId = deviceId
                });

                if (response.IsSuccessStatusCode)
                {
                    bool result = await response.Content.ReadAsAsync<bool>();
                    return result ? ApiCallResult.Success : ApiCallResult.Failure;
                }
                else
                {
                    return ApiCallResult.Failure;
                }
            }
            catch (HttpRequestException ex)
            {
                _logService.LogException(ex, "Cannot contact server. Check in failed");
                return ApiCallResult.NotReachable;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while checking in");
                return ApiCallResult.Unknown;
            }
        }

        public async Task<ApiCallResult> CheckinAllDevicesAsync(string userName)
        {
            string uri = Url.Combine(_baseApiAddress, "session/endall");
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, userName);

                if (response.IsSuccessStatusCode)
                {
                    bool result = await response.Content.ReadAsAsync<bool>();
                    return result ? ApiCallResult.Success : ApiCallResult.Failure;
                }
                else
                {
                    return ApiCallResult.Failure;
                }
            }
            catch (HttpRequestException ex)
            {
                _logService.LogException(ex, "Cannot contact server. Failed to check in all devices");
                return ApiCallResult.NotReachable;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while checking in all devices");
                return ApiCallResult.Unknown;
            }
        }

        public async Task<ApiCallResult> CheckoutDeviceAsync(string userName, int deviceId)
        {
            string uri = Url.Combine(_baseApiAddress, "session/create");
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, new SessionRequest
                {
                    UserName = userName,
                    DeviceId = deviceId
                });

                if (response.IsSuccessStatusCode)
                {
                    bool result = await response.Content.ReadAsAsync<bool>();
                    return result ? ApiCallResult.Success : ApiCallResult.Failure;
                }
                else
                {
                    return ApiCallResult.Failure;
                }
            }
            catch (HttpRequestException ex)
            {
                _logService.LogException(ex, "Cannot contact server. Check out failed");
                return ApiCallResult.NotReachable;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while checking out");
                return ApiCallResult.Unknown;
            }
        }

        public async Task<RefreshResponse> Refresh(string lastSuccessfulRefreshTime)
        {
            string uri = Url.Combine(_baseApiAddress, "refresh");
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, new RefreshRequest
                {
                    LastSuccessfulRefresh = lastSuccessfulRefreshTime
                });

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<RefreshResponse>();
                }
                else
                {
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logService.LogException(ex, "Cannot contact server. Refresh failed");
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while refreshing");
            }

            return null;
        }

        public async Task<IEnumerable<Device>> GetDevicesAsync()
        {
            string uri = Url.Combine(_baseApiAddress, "device");
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<IEnumerable<Device>>();
                }
                else
                {
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logService.LogException(ex, "Cannot contact server. Could not get devices");
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while getting devices");
            }

            return null;
        }

        public async Task<UserInfo> RegisterUserAsync(string domainUserName)
        {
            string uri = Url.Combine(_baseApiAddress, "user/register");
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, domainUserName);
                if (response.IsSuccessStatusCode)
                {
                    UserInfo user = await response.Content.ReadAsAsync<UserInfo>();
                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logService.LogException(ex, "Cannot contact server. Could not register user");
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while registering user");
            }

            return null;
        }

        public async Task<ApiCallResult> SetUsernameAsync(string domainUserName, string friendlyName)
        {
            string uri = Url.Combine(_baseApiAddress, "user/setfriendlyname");
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, new SetFriendlyNameRequest
                {
                    DomainUserName = domainUserName,
                    FriendlyName = friendlyName
                });

                return response.IsSuccessStatusCode ? ApiCallResult.Success : ApiCallResult.Failure;
            }
            catch (HttpRequestException ex)
            {
                _logService.LogException(ex, "Cannot contact server. Could not set user name");
                return ApiCallResult.NotReachable;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while setting user name");
                return ApiCallResult.Unknown;
            }
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            string uri = Url.Combine(_baseApiAddress, "settings");
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<Dictionary<string, string>>();
                }
                else
                {
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logService.LogException(ex, "Cannot contact server. Could not get settings");
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while getting settings");
            }

            return null;
        }
    }
}
