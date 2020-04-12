using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
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
                session.Log("Invalid input for TestConnection");
                session["DB_SERVER_OK"] = "0";
                return ActionResult.Success;
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
