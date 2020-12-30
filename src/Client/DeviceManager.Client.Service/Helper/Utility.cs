// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.IO;
using System.Reflection;

namespace DeviceManager.Client.Service
{
    public static class Utility
    {
        /// <summary>
        /// Gets the full name of the current Windows user.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUserName()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        }

        /// <summary>
        /// Compares two user names and returns true if both point to the same user.
        /// </summary>
        /// <param name="userName1"></param>
        /// <param name="userName2"></param>
        /// <returns></returns>
        public static bool IsSameUser(string userName1, string userName2)
        {
            return string.Compare(userName1, userName2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Gets the full path of the app folder under AppData\Roaming that belongs to the current user. If the directory does not exist, it is created.
        /// </summary>
        /// <returns></returns>
        public static string GetAppRoamingFolder()
        {
            string appRoamingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeviceManager");
            Directory.CreateDirectory(appRoamingFolder);
            return appRoamingFolder;
        }

        /// <summary>
        /// Gets the application version in the format of Major.Minor.Build.
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}
