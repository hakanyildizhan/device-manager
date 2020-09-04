// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using Microsoft.Deployment.WindowsInstaller;
using System;
using System.IO;

namespace DeviceManager.Setup.CustomActions
{
    /// <summary>
    /// Closes the installation location (AppData\Local\Device Manager) before install, if open.
    /// If this folder is open prior to an upgrade, installation fails with an "access denied" message.
    /// </summary>
    public partial class CustomActions
    {
        [CustomAction]
        public static ActionResult CloseInstallFolder(Session session)
        {
            session.Log("CloseInstallFolder CustomAction started");
            string localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            if (string.IsNullOrEmpty(localAppDataFolder))
            {
                session.Log("CloseInstallFolder CustomAction failed, cannot get local app data folder");
                return ActionResult.NotExecuted;
            }

            string installationPath = Path.Combine(localAppDataFolder, "Device Manager");

            if (!Directory.Exists(installationPath))
            {
                session.Log("CloseInstallFolder CustomAction completed, installation folder does not yet exist");
                return ActionResult.NotExecuted;
            }

            try
            {
                foreach (SHDocVw.InternetExplorer window in new SHDocVw.ShellWindows())
                {
                    if (Path.GetFileNameWithoutExtension(window.FullName).ToLowerInvariant() == "explorer")
                    {
                        if (Uri.IsWellFormedUriString(window.LocationURL, UriKind.Absolute))
                        {
                            string location = new Uri(window.LocationURL).LocalPath;

                            if (string.Equals(location, installationPath, StringComparison.OrdinalIgnoreCase))
                            {
                                window.Quit();
                            } 
                        }
                    }
                }
            }
            catch (Exception)
            {
                session.Log("CloseInstallFolder CustomAction failed");
                return ActionResult.NotExecuted;
            }

            return ActionResult.Success;
        }
    }
}
