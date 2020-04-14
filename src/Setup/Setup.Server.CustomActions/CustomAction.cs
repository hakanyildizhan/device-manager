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

            string sqlCommand = GetEmbeddedSQLFileContents();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(sqlCommand, connection);
                    cmd.ExecuteNonQuery();
                    session.Log("Granted database permissions successfully");
                    return ActionResult.Success;
                }
                catch (SqlException)
                {
                    session.Log("Could not grant database permissions");
                    return ActionResult.Success;
                }
            }
        }

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

        private static string GetEmbeddedSQLFileContents()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DeviceManager.Setup.Server.CustomActions.Resources.Script_v1.0.0.0_User.sql";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }
    }
}
