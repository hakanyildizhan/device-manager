﻿using DeviceManager.Client.Service.Model;
using DeviceManager.Client.Service.Model.Api;
using Flurl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
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

//#if DEBUG
//            string serverAddress = "http://localhost:8060/";
//#else
            string serverAddress = _configService.GetServerAddress();
//#endif
            _baseApiAddress = Url.Combine(serverAddress, "api");
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
