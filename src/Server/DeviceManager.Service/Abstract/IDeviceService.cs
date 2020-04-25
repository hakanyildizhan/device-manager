using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
