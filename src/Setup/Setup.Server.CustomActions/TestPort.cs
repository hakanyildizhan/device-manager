using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Setup.Server.CustomActions
{
    public partial class CustomActions
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
    }
}
