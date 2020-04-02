using DeviceManager.Client.Service.Model;
using DeviceManager.Client.Service.Model.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service
{
    public class DataService : IDataService
    {
        static HttpClient client = new HttpClient();
        UriBuilder builder;

        public DataService()
        {
#if DEBUG
            builder = new UriBuilder("http://localhost:8060/api/");
#else
            builder = new UriBuilder("http://EVT01152SNZ:8060/api/");
#endif
        }

        public async Task<bool> CheckDeviceAvailabilityAsync(int deviceId)
        {
            Uri uri = new Uri(builder.Uri, "session/isdeviceavailable");
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, deviceId);
                if (response.IsSuccessStatusCode)
                {
                    bool result = await response.Content.ReadAsAsync<bool>();
                    return result;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception) //TODO: log
            {
                return false;
            }
        }

        public async Task<bool> CheckinDeviceAsync(string userName, int deviceId)
        {
            Uri uri = new Uri(builder.Uri, "session/end");
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
                    return result;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception) //TODO: log
            {
                return false;
            }
        }

        public async Task<bool> CheckoutDeviceAsync(string userName, int deviceId)
        {
            Uri uri = new Uri(builder.Uri, "session/create");
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
                    return result;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception) //TODO: log
            {
                return false;
            }
        }

        public async Task<RefreshData> Refresh()
        {
            Uri uri = new Uri(builder.Uri, "refresh");
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<RefreshData>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception) //TODO: log
            {
                return null;
            }
        }

        public async Task<IEnumerable<Device>> GetDevicesAsync()
        {
            Uri uri = new Uri(builder.Uri, "device");
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
            catch (Exception) //TODO: log
            {
                return null;
            }
        }

        public async Task<UserInfo> RegisterUserAsync(string domainUserName)
        {
            Uri uri = new Uri(builder.Uri, "user/register");
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
            catch (Exception) //TODO: log
            {
                return null;
            }
        }

        public async Task<bool> SetUsernameAsync(string domainUserName, string friendlyName)
        {
            Uri uri = new Uri(builder.Uri, "user/setfriendlyname");
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(uri, new SetFriendlyNameRequest
                { 
                    DomainUserName = domainUserName, 
                    FriendlyName = friendlyName 
                });

                return response.IsSuccessStatusCode;
            }
            catch (Exception) //TODO: log
            {
                return false;
            }
        }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            Uri uri = new Uri(builder.Uri, "settings");
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
            catch (Exception) //TODO: log
            {
                return null;
            }
        }
    }
}
