using System;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeviceManager.Setup.CustomActions
{
    public partial class CustomActions
    {
        static object _lockObject = new object();
        private static string _configFile = Path.Combine(GetAppRoamingFolder(), "appsettings.json");
        private static string _serverAddressSetting = "serverAddress";

        /// <summary>
        /// Checks if SERVERADDRESS property contains a valid server URL.
        /// If it is valid, sets TESTRESULT to "1".
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
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

            // if address points to Default Web Site (port 80)
            // and does not include "/DeviceManager", handle it
            Uri uri = new Uri(address);
            if (uri.Port == 80 && !address.Contains("DeviceManager"))
            {
                address = uri.Scheme + Uri.SchemeDelimiter + uri.Host + "/" + "DeviceManager";
                session["SERVERADDRESS"] = address;
            }

            // if address does not end with '/', add it
            if (!address.EndsWith("/"))
            {
                address = address + '/';
            }

            var serverValidityResult = Utility.TestServerValidity(address);
            session.Log(serverValidityResult.Message);

            if (serverValidityResult.Success)
            {
                var result = WriteServerSetting(address);
                session.Log(result.Message);
                session["TESTRESULT"] = result.Success ? "1" : "0";
            }
            else
            {
                session["TESTRESULT"] = "0";
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// Gets the full path of the app folder under AppData\Roaming that belongs to the current user. If the directory does not exist, it is created.
        /// </summary>
        /// <returns></returns>
        private static string GetAppRoamingFolder()
        {
            string appRoamingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeviceManager");
            Directory.CreateDirectory(appRoamingFolder);
            return appRoamingFolder;
        }

        private static GetServerSettingResult GetServerSetting()
        {
            GetServerSettingResult result = new GetServerSettingResult();

            try
            {
                lock (_lockObject)
                {
                    string json = File.ReadAllText(_configFile);
                    if (string.IsNullOrEmpty(json))
                    {
                        result.Result = string.Empty;
                        result.Message = "Setting file is not present, cannot get setting";
                    }

                    dynamic jsonObj = JsonConvert.DeserializeObject(json);
                    result.Result = (string)(jsonObj[_serverAddressSetting]);
                    result.Message = "Got server setting successfully";
                }
            }
            catch (Exception ex)
            {
                result.Result = string.Empty;
                result.Message = $"Error occured while getting server setting. Error type {ex.GetType().Name}, message: {ex.Message}";
            }

            return result;
        }

        private static WriteServerSettingResult WriteServerSetting(string address)
        {
            WriteServerSettingResult result = new WriteServerSettingResult();

            try
            {
                lock (_lockObject)
                {
                    bool fileExistsOrCreated = CreateConfigFileIfNotExists();

                    if (!fileExistsOrCreated)
                    {
                        result.Success = false;
                        result.Message = "Setting file could not be created";
                    }
                    else
                    {
                        string json = File.ReadAllText(_configFile);
                        dynamic jsonObj;
                        if (!string.IsNullOrEmpty(json))
                        {
                            jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                            if (jsonObj[_serverAddressSetting] == address)
                            {
                                result.Success = true;
                                result.Message = "Server setting already exists";
                            }
                        }
                        else
                        {
                            jsonObj = new JObject();
                        }
                        jsonObj[_serverAddressSetting] = address;
                        string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                        File.WriteAllText(_configFile, output);
                        result.Success = true;
                        result.Message = "Server setting written successfully";
                    }
                }
            }
            catch (Exception ex )
            {
                result.Message = $"Error occured while setting server address. Error type {ex.GetType().Name}, message: {ex.Message}";
            }

            return result;
        }

        private static bool CreateConfigFileIfNotExists()
        {
            bool success = false;

            try
            {
                lock (_lockObject)
                {
                    if (!File.Exists(_configFile))
                    {
                        File.WriteAllText(_configFile, string.Empty);
                    }
                    success = true;
                }
            }
            catch (Exception)
            {
            }
            return success;
        }
    }
}
