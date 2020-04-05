using DeviceManager.Client.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service
{
    public interface IDataService
    {
        Task<IEnumerable<Device>> GetDevicesAsync();
        Task<Dictionary<string,string>> GetSettingsAsync();
        Task<UserInfo> RegisterUserAsync(string domainUserName);
        Task<ApiCallResult> SetUsernameAsync(string domainUserName, string friendlyName);
        Task<ApiCallResult> CheckDeviceAvailabilityAsync(int deviceId);
        Task<ApiCallResult> CheckoutDeviceAsync(string userName, int deviceId);
        Task<ApiCallResult> CheckinDeviceAsync(string userName, int deviceId);
        Task<RefreshResponse> Refresh(string lastSuccessfulRefreshTime);
    }
}
