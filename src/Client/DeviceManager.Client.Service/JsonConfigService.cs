// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DeviceManager.Client.Service
{
    public class JsonConfigService : IConfigurationService
    {
        private readonly ILogService _logService;

        private static string _configFile = Path.Combine(Utility.GetAppRoamingFolder(), "appsettings.json");
        static object lockObj = new object();

        public JsonConfigService(ILogService<JsonConfigService> logService)
        {
            _logService = logService;
            Init();
        }

        private void Init()
        {
            try
            {
                lock (lockObj)
                {
                    if (!File.Exists(_configFile))
                    {
                        File.WriteAllText(_configFile, string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Could not create config file");
            }
        }

        public string Get(string keyName)
        {
            try
            {
                lock (lockObj)
                {
                    string json = File.ReadAllText(_configFile);
                    if (string.IsNullOrEmpty(json))
                    {
                        return string.Empty;
                    }

                    dynamic jsonObj = JsonConvert.DeserializeObject(json);
                    return (string)(jsonObj[keyName]);
                }
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, $"Could not get setting {keyName}, returning empty string");
                return string.Empty;
            }
        }

        public async Task SetAsync(string keyName, string value)
        {
            try
            {
                var block = new ActionBlock<Tuple<string, string>>(tuple =>
                {
                    var key = tuple.Item1;
                    var val = tuple.Item2;

                    lock (lockObj)
                    {
                        string json = File.ReadAllText(_configFile);
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
                        File.WriteAllText(_configFile, output);
                    }
                });

                block.Post(new Tuple<string, string>(keyName, value));
                block.Complete();
                await block.Completion;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, $"Could not set setting {keyName}");
            }
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

        public string GetLastSuccessfulRefreshTime()
        {
            return Get(ServiceConstants.Settings.LAST_SUCCESSFUL_REFRESH);
        }

        public int GetUsagePromptInterval()
        {
            string value = Get(ServiceConstants.Settings.USAGE_PROMPT_INTERVAL);
            return !string.IsNullOrEmpty(value) ? int.Parse(value) : ServiceConstants.Settings.USAGE_PROMPT_INTERVAL_DEFAULT;
        }

        public int GetUsagePromptDuration()
        {
            string value = Get(ServiceConstants.Settings.USAGE_PROMPT_DURATION);
            return !string.IsNullOrEmpty(value) ? int.Parse(value) : ServiceConstants.Settings.USAGE_PROMPT_DURATION_DEFAULT;
        }

        public string GetServerAddress()
        {
            return Get(ServiceConstants.Settings.SERVER_ADDRESS);
        }
    }
}
