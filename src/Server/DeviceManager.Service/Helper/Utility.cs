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
            string appRoamingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeviceManager");
            Directory.CreateDirectory(appRoamingFolder);
            return appRoamingFolder;
        }
    }
}
