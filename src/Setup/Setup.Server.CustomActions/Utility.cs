// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using Microsoft.Deployment.WindowsInstaller;

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
