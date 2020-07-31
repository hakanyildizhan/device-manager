// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.Service
{
    public interface IDeviceService
    {
        /// <summary>
        /// Gets all existing devices.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Device> GetDevices();

        /// <summary>
        /// Gets all devices checked out to given user.
        /// </summary>
        /// <returns></returns>
        IEnumerable<DeviceDetail> GetUserDevices(string userName);

        /// <summary>
        /// Gets requested device by Id.
        /// </summary>
        /// <returns></returns>
        DeviceDetail GetDevice(int id);

        /// <summary>
        /// Adds a new device item.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        Task<bool> AddDevice(DeviceDetail device);

        /// <summary>
        /// Updates a device item.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        Task<bool> UpdateDevice(DeviceDetail device);

        /// <summary>
        /// Deactivates a device item.
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        Task<bool> DeactivateDevice(int deviceId);

        /// <summary>
        /// Imports given hardware data into the device database.
        /// </summary>
        /// <param name="deviceData"></param>
        /// <returns></returns>
        Task<bool> Import(IEnumerable<DeviceImport> deviceData);
    }
}
