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
        Task<bool> SetUsernameAsync(string domainUserName, string friendlyName);
        Task<bool> CheckDeviceAvailabilityAsync(int deviceId);
        Task<bool> CheckoutDeviceAsync(string userName, int deviceId);
        Task<bool> CheckinDeviceAsync(string userName, int deviceId);
        Task<RefreshData> Refresh();
    }
}
