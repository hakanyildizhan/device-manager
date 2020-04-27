using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Setup.Server.CustomActions
{
    public static class Utility
    {
        /// <summary>
        /// Generates a connection string from the Authentication mode, DB Server-Instance name and login information in the session.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string GenerateConnectionString(Session session)
        {
            string authMode = session["DB_AUTHENTICATIONMODE"];
            string server = session["DB_SERVER"];
            string user = session["DB_USER"];
            string password = session["DB_PASSWORD"];

            bool authModeIsWindows = authMode == "0";

            // validate inputs
            if (string.IsNullOrEmpty(server) ||
                (!authModeIsWindows &&
                  (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))))
            {
                return string.Empty;
            }

            string connString = $"Server={server};database=master;";

            if (authModeIsWindows)
            {
                connString += "Integrated Security=SSPI;";
            }
            else
            {
                connString += $"User Id={user};Password={password};";
            }

            return connString;
        }
    }
}
