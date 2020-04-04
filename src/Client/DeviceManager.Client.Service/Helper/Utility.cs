using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service
{
    public static class Utility
    {
        public static string GetCurrentUserName()
        {
            return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
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
    }
}
