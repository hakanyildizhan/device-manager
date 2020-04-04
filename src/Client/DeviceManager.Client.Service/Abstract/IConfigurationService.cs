using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service
{
    public interface IConfigurationService
    {
        /// <summary>
        /// Gets a specific setting value.
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        string Get(string keyName);

        /// <summary>
        /// Gets the duration between each data refresh.
        /// </summary>
        /// <returns></returns>
        int GetRefreshInterval();

        /// <summary>
        /// Upserts a setting value.
        /// </summary>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task SetAsync(string keyName, string value);

        /// <summary>
        /// Logs the refresh attempt with the attempt time into the configuration file.
        /// </summary>
        /// <returns></returns>
        Task LogRefresh();

        /// <summary>
        /// Logs a successful refresh with the refresh time into the configuration file.
        /// </summary>
        /// <returns></returns>
        Task LogSuccessfulRefresh();

        /// <summary>
        /// Gets the time of the last successful refresh from the configuration file.
        /// </summary>
        /// <returns></returns>
        string GetLastSuccessfulRefreshTime();
    }
}
