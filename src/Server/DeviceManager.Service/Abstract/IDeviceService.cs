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
        /// Imports given hardware data into the device database.
        /// </summary>
        /// <param name="deviceData"></param>
        /// <returns></returns>
        Task<bool> Import(IEnumerable<DeviceImport> deviceData);
    }
}
