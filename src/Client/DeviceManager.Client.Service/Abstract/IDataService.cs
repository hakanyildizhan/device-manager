// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service.Model;
using System.Collections.Generic;
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
