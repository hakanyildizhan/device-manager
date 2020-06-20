// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.Service
{
    public interface ISettingsService
    {
        /// <summary>
        /// Gets server settings in key-value pairs.
        /// </summary>
        /// <returns></returns>
        Dictionary<string,string> Get();

        /// <summary>
        /// Gets server settings in full detail, including descriptions.
        /// </summary>
        /// <returns></returns>
        Task<SettingsDetail> GetDetailedAsync();

        /// <summary>
        /// Updates server settings as entered on administration page.
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync(SettingsDetail settings);

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
        Task<bool> AddOrUpdateAsync(string name, string value);
    }
}
