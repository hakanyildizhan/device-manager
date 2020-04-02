using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
