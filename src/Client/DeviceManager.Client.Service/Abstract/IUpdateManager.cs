// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Update;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service
{
    /// <summary>
    /// Interface for managing web requests as well as installation regarding update installers.
    /// </summary>
    public interface IUpdateManager
    {
        /// <summary>
        /// Check to see if an update is available. <see cref="CheckUpdate"/> method should be called first.
        /// </summary>
        bool UpdateIsAvailable { get; set; }

        /// <summary>
        /// Check to see if the installer is already downloaded & ready to install.
        /// </summary>
        //bool UpdateIsReadyToInstall { get; set; }

        /// <summary>
        /// Contains information about the update (if there is any).
        /// </summary>
        UpdatePackage Update { get; set; }

        /// <summary>
        /// The location of the downloaded installer.
        /// </summary>
        string InstallerPath { get; set; }

        /// <summary>
        /// Checks for an update. If an update is available, <see cref="UpdateIsAvailable"/> flag is set to true and the <see cref="Update"/> property will include the update information.
        /// </summary>
        /// <returns></returns>
        Task CheckUpdate();

        /// <summary>
        /// Handles download and installation of the update.
        /// </summary>
        /// <returns></returns>
        Task DownloadAndInstall();

        /// <summary>
        /// Resets all state.
        /// </summary>
        void Reset();
    }
}
