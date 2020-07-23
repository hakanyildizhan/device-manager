// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.Service
{
    public interface ISessionService
    {
        /// <summary>
        /// Returns true if the device is available.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        bool IsDeviceAvailable(int deviceId);

        /// <summary>
        /// Creates a session for given user on given device.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<bool> CreateSessionAsync(string userName, int deviceId);

        /// <summary>
        /// Ends an existing session for given user on given device.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<bool> EndSessionAsync(string userName, int deviceId);

        /// <summary>
        /// Ends all existing sessions for given user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<bool> EndAllSessionsOfUserAsync(string userName);

        /// <summary>
        /// Ends all active sessions for all users.
        /// </summary>
        /// <returns></returns>
        Task<bool> EndActiveSessionsAsync();

        /// <summary>
        /// Gets availability status of all devices.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DeviceSessionInfo> GetDeviceSessions();
    }
}
