// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DeviceManager.Service
{
    public static class Utility
    {
        /// <summary>
        /// Gets the full path of the app folder under AppData\Roaming that belongs to the current App Pool user. If the directory does not exist, it is created.
        /// </summary>
        /// <returns></returns>
        public static string GetAppRoamingFolder()
        {
            string roamingFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            /* SpecialFolder.ApplicationData can be empty if 
             *  - user profile is not loaded
             *  - profile environment is not set
             * 
             * For logging to work correctly, 
             * this tag should exist for "DeviceManager" App Pool on applicationHost.config:
             *    <processModel setProfileEnvironment="true" loadUserProfile="true" />
             * 
             * This is normally done by the installer.
            */ 

            if (string.IsNullOrEmpty(roamingFolder))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(@"DeviceManager App Pool does not seem to have ""setProfileEnvironment"" and ""loadUserProfile"" settings set to true. This prevents the web server from initializing.");
                sb.AppendLine(@"Make sure that the part below exists for ""DeviceManager"" App Pool on file C:\Windows\System32\inetsrv\config\applicationHost.config.");
                sb.AppendLine();
                sb.AppendLine(@"<add name=""DeviceManager"" autoStart=""true"" enable32BitAppOnWin64=""true"" managedRuntimeVersion=""v4.0"" managedPipelineMode=""Integrated"">");
                sb.AppendLine(@"    <processModel identityType=""ApplicationPoolIdentity"" setProfileEnvironment=""true"" loadUserProfile=""true"" />");
                sb.AppendLine("</add>");

                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "DeviceManager";
                    eventLog.WriteEntry(sb.ToString(), EventLogEntryType.Error, 4022, 1);
                }

                throw new Exception("DeviceManager App Pool does not have setProfileEnvironment and loadUserProfile settings set to true!");
            }

            string appRoamingFolder = Path.Combine(roamingFolder, "DeviceManager");
            Directory.CreateDirectory(appRoamingFolder);
            return appRoamingFolder;
        }
    }
}
