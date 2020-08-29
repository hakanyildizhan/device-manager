// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Client.Service
{
    /// <summary>
    /// Interface for a redundancy implementation for the main <see cref="IConfigurationService"/>.
    /// In order to mitigate a possible issue of a crucial application setting cannot be read from <see cref="IConfigurationService"/> (e.g. configuration file becomes corrupt and server address cannot be read, see issue #30), implement this interface to store and read such settings, allowing the application to recover from such cases.
    /// </summary>
    public interface IRedundantConfigService
    {
        /// <summary>
        /// Writes a valid server URL following a successful initialization.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool StoreServerURL(string url);

        /// <summary>
        /// Returns the server URL.
        /// </summary>
        /// <returns></returns>
        string FetchServerURL();

        /// <summary>
        /// Whether current operating system environment supports Windows 10 Toast Notification mechanism.
        /// </summary>
        /// <returns></returns>
        bool CanUseToastNotifications();

        /// <summary>
        /// Whether current operating system environment supports Windows 10 Toast progress bars supported since Creators Update.
        /// </summary>
        /// <returns></returns>
        bool CanUseToastProgressBars();
    }
}
