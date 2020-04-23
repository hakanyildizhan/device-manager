using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace DeviceManager.Setup.Server.CustomActions
{
    public class CustomActions
    {
        /// <summary>
        /// Tests if the given port is available to bind for the new IIS website. Sets "IIS_PORT_AVAILABLE" to 1 if it is.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult TestPort(Session session)
        {
            session.Log("Begin TestPort");
            string portParam = session["IIS_PORT"];

            // Empty parameter
            if (string.IsNullOrEmpty(portParam))
            {
                session.Log("No port provided");
                session["IIS_PORT_AVAILABLE"] = "0";
                return ActionResult.Success;
            }

            int port;
            bool convertResult = int.TryParse(portParam, out port);

            // Parameter is not a valid integer
            if (!convertResult)
            {
                session.Log("Provided port is not a valid integer");
                session["IIS_PORT_AVAILABLE"] = "0";
                return ActionResult.Success;
            }

            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
            try
            {
                TcpListener tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();
            }
            catch (SocketException) // port is not available!
            {
                session.Log("Provided port is not available");
                session["IIS_PORT_AVAILABLE"] = "0";
                return ActionResult.Success;
            }

            // if we got this far, port is available
            session.Log("Provided port is available");
            session["IIS_PORT_AVAILABLE"] = "1";
            return ActionResult.Success;
        }

        /// <summary>
        /// Checks if database connection can be established. Sets "DB_SERVER_OK" to 1 if the connection was successfully established.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult TestConnection(Session session)
        {
            session.Log("Begin TestConnection");

            string connString = GenerateConnectionString(session);

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

        /// <summary>
        /// After install completes, runs an SQL file to give the IIS App Pool necessary permissions on the application database.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult GrantDatabasePermissions(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            session.Log("Begin GrantDatabasePermissions");
            string connString = GenerateConnectionString(session);

            if (string.IsNullOrEmpty(connString))
            {
                session.Log("Invalid input for GrantDatabasePermissions");
                return ActionResult.Success;
            }

            string sqlCommand = GetEmbeddedSQLFileContents("UserPermissions.sql");
            if (string.IsNullOrEmpty(sqlCommand))
            {
                session.Log("Could not get SQL file contents for GrantDatabasePermissions");
                return ActionResult.Success;
            }

            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction("AfterInstallUserPermissions");
                SqlCommand cmd = new SqlCommand(sqlCommand, connection);
                cmd.Transaction = transaction;
                try
                {
                    cmd.ExecuteNonQuery();
                    transaction.Commit();
                    session.Log("Granted database permissions successfully");
                    return ActionResult.Success;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    session.Log("Error occured. Could not grant database permissions");
                    return ActionResult.Success;
                }
            }
        }

        /// <summary>
        /// Generates a connection string from the Authentication mode, DB Server-Instance name and login information in the session.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private static string GenerateConnectionString(Session session)
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

        /// <summary>
        /// Gets the contents of the embedded file.
        /// </summary>
        /// <returns></returns>
        private static string GetEmbeddedSQLFileContents(string resourceFileName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "DeviceManager.Setup.Server.CustomActions.Resources." + resourceFileName;

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
