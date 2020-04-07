using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.WindowsInstaller;

namespace DeviceManager.Setup.CustomActions
{
    public class CustomActions
    {
        static HttpClient _client = new HttpClient();
        static object _lockObject = new object();
        static string _configFileName = "appsettings.json";

        [CustomAction]
        public static ActionResult TestServer(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            session.Log("TestServer CustomAction started");

            string address = session["SERVERADDRESS"];

            // if address does not start with http or https, add it
            if (!address.StartsWith("http") && !address.StartsWith("https"))
            {
                address = "http://" + address;
            }

            // if address does not end with '/', add it
            if (!address.EndsWith("/"))
            {
                address = address + '/';
            }

            // send a request to <address>/api/user/register
            UriBuilder builder = new UriBuilder($"{address}api/");
            Uri uri = new Uri(builder.Uri, "user/register");
            string success = "0";
             
            try
            {
                string domainUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                HttpResponseMessage response = Task.Run(async () => await _client.PostAsJsonAsync(uri, domainUserName)).Result;
                if (response.IsSuccessStatusCode)
                {
                    success = "1";

                    lock(_lockObject)
                    {
                        string configFile = Path.Combine(GetAppRoamingFolder(), _configFileName);
                        if (!File.Exists(configFile))
                        {
                            File.WriteAllText(configFile, $"{{ serverAddress: \"{address}\" }}");
                            session.Log("server address written to config file");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                session.Log($"Error occured while writing server address to config file. Error type {ex.GetType().Name}, message: {ex.Message}");
            }

            session["TESTRESULT"] = success;
            return ActionResult.Success;
        }

        /// <summary>
        /// Gets the full path of the app folder under AppData\Roaming that belongs to the current user. If the directory does not exist, it is created.
        /// </summary>
        /// <returns></returns>
        static string GetAppRoamingFolder()
        {
            string appRoamingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeviceManager");
            Directory.CreateDirectory(appRoamingFolder);
            return appRoamingFolder;
        }
    }
}
