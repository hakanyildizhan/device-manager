// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Data.SqlClient;
using Microsoft.Deployment.WindowsInstaller;

namespace DeviceManager.Setup.Server.CustomActions
{
    public partial class CustomActions
    {
        /// <summary>
        /// Checks if database connection can be established. Sets "DB_SERVER_OK" to 1 if the connection was successfully established.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult TestConnection(Session session)
        {
            session.Log("Begin TestConnection");

            string connString = Utility.GenerateConnectionString(session);

            if (string.IsNullOrEmpty(connString))
            {
                session.Log("Invalid input for TestConnection");
                session["DB_SERVER_OK"] = "0";
                return ActionResult.Success;
            }

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    session.Log("Established SQL connection successfully");
                    session["DB_SERVER_OK"] = "1";
                    return ActionResult.Success;
                }
                catch (SqlException)
                {
                    session.Log("Could not establish connection to SQL");
                    session["DB_SERVER_OK"] = "0";
                    return ActionResult.Success;
                }
            }
        }
    }
}
