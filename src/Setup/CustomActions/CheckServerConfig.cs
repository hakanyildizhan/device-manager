// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using Microsoft.Deployment.WindowsInstaller;
using System.IO;

namespace DeviceManager.Setup.CustomActions
{
    public partial class CustomActions
    {
        /// <summary>
        /// Checks if the user already has a config file that contains a valid server URL.
        /// If so, sets TESTRESULT to "1" and sets the SERVERADDRESS property.
        /// Additionally, writes the server URL to the registry.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult CheckServerConfig(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            session.Log("CheckServerConfig CustomAction started");

            // if file does not exist, do nothing
            if (!File.Exists(_configFile))
            {
                return ActionResult.Success;
            }

            var getServerSettingResult = GetServerSetting();
            session.Log(getServerSettingResult.Message);

            string serverAddress = getServerSettingResult.Result;

            // server setting does not exist. return
            if (string.IsNullOrEmpty(serverAddress))
            {
                session["TESTRESULT"] = "0";
                return ActionResult.Success;
            }

            // test the server validity
            var serverValidityResult = Utility.TestServerValidity(serverAddress);
            session.Log(serverValidityResult.Message);
            session["TESTRESULT"] = serverValidityResult.Success ? "1" : "0";
            session["SERVERADDRESS"] = serverValidityResult.Success ? serverAddress : "";

            // if the URL is valid, try saving the value to the registry
            if (serverValidityResult.Success)
            {
                bool success = Utility.SaveServerURLToRegistry(serverAddress);

                if (!success)
                {
                    session.Log("Failed writing server URL to registry");
                }
            }

            return ActionResult.Success;
        }
    }
}
