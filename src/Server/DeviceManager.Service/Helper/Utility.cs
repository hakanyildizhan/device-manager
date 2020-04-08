using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // SpecialFolder.ApplicationData can be empty if the user profile environment is not set
            // it should be set in applicationHost.config, i.e. setProfileEnvironment="true"
            // TODO: should be tested more extensively
            if (string.IsNullOrEmpty(roamingFolder))
            {
                roamingFolder = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "AppData", "Roaming");
            }

            string appRoamingFolder = Path.Combine(roamingFolder, "DeviceManager");
            Directory.CreateDirectory(appRoamingFolder);
            return appRoamingFolder;
        }
    }
}
