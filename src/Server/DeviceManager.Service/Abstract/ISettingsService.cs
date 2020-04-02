using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Service
{
    public interface ISettingsService
    {
        /// <summary>
        /// Gets server settings.
        /// </summary>
        /// <returns></returns>
        Dictionary<string,string> Get();

        /// <summary>
        /// Gets a specific setting value.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        string Get(string keyName);

        /// <summary>
        /// Adds/updates a setting value.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> AddOrUpdate(string name, string value);
    }
}
