using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DeviceManager.Client.Service
{
    public class JsonConfigService : IConfigurationService
    {
        private string _settingsFilePath;
        static object lockObj = new object();

        public JsonConfigService()
        {
            string settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DeviceManager");
            Directory.CreateDirectory(settingsFolder);
            _settingsFilePath = Path.Combine(settingsFolder, "appsettings.json");

            if (!File.Exists(_settingsFilePath))
            {
                File.Create(_settingsFilePath);
            }
        }

        public string Get(string keyName)
        {
            string json = File.ReadAllText(_settingsFilePath);
            if (string.IsNullOrEmpty(json))
            {
                return string.Empty;
            }

            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            return (string)(jsonObj[keyName]);
        }

        public async Task SetAsync(string keyName, string value)
        {
            var block = new ActionBlock<Tuple<string, string>>(tuple =>
            {
                var key = tuple.Item1;
                var val = tuple.Item2;

                lock (lockObj)
                {
                    string json = File.ReadAllText(_settingsFilePath);
                    dynamic jsonObj;
                    if (!string.IsNullOrEmpty(json))
                    {
                        jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                        if (jsonObj[key] == val)
                        {
                            return;
                        }
                    }
                    else
                    {
                        jsonObj = new JObject();
                    }
                    jsonObj[key] = val;
                    string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
                    File.WriteAllText(_settingsFilePath, output);
                }
            });

            block.Post(new Tuple<string, string>(keyName, value));
            block.Complete();
            await block.Completion;
        }

        public int GetRefreshInterval()
        {
            string value = Get(ServiceConstants.Settings.REFRESH_INTERVAL);
            return !string.IsNullOrEmpty(value) ? int.Parse(value) : ServiceConstants.Settings.REFRESH_INTERVAL_DEFAULT;
        }

        public async Task LogRefresh()
        {
            await this.SetAsync(ServiceConstants.Settings.LAST_REFRESH, DateTime.UtcNow.ToStringTurkish());
        }

        public async Task LogSuccessfulRefresh()
        {
            await this.SetAsync(ServiceConstants.Settings.LAST_SUCCESSFUL_REFRESH, DateTime.UtcNow.ToStringTurkish());
        }
    }
}
