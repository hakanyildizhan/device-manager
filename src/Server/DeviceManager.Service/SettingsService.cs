// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using DeviceManager.Service.Model;
using DeviceManager.Service.Model.Setting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Service
{
    public class SettingsService : ISettingsService
    {
        private readonly ILogService _logService;

        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public SettingsService(ILogService<SettingsService> logService)
        {
            _logService = logService;
        }

        public Dictionary<string, string> Get()
        {
            try
            {
                Dictionary<string, string> settings = new Dictionary<string, string>();
                DbContext.Settings.ToList().ForEach(s => settings.Add(s.Name, s.Value));
                return settings;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while getting settings");
                throw new ServiceException(ex);
            }
        }

        public async Task<bool> AddOrUpdateAsync(string name, string value)
        {
            try
            {
                if (DbContext.Settings.Any(s => s.Name == name))
                {
                    Setting setting = DbContext.Settings.Find(name);
                    setting.Value = value;
                }
                else
                {
                    DbContext.Settings.Add(new Setting { Name = name, Value = value });
                }
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (System.Data.DataException ex)
            {
                _logService.LogException(ex, $"DataException occured while adding/updating setting {name}");
                return false;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, $"Unknown error occured while adding/updating setting {name}");
                throw new ServiceException(ex);
            }
        }

        public string Get(string keyName)
        {
            try
            {
                return DbContext.Settings.FirstOrDefault(s => s.Name == keyName)?.Value;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, $"Unknown error occured while getting setting {keyName}");
                throw new ServiceException(ex);
            }
        }

        public async Task<SettingsDetail> GetDetailedAsync()
        {
            try
            {
                var settings = await DbContext.Settings.ToListAsync();
                var serverVersionSetting = settings.Where(s => s.Name == ServiceConstants.Settings.VERSION).FirstOrDefault();
                var refreshIntervalSetting = settings.Where(s => s.Name == ServiceConstants.Settings.REFRESH_INTERVAL).FirstOrDefault();
                var lastDeviceUpdateSetting = settings.Where(s => s.Name == ServiceConstants.Settings.LAST_DEVICE_LIST_UPDATE).FirstOrDefault();
                var promptDurationSetting = settings.Where(s => s.Name == ServiceConstants.Settings.USAGE_PROMPT_DURATION).FirstOrDefault();
                var promptIntervalSetting = settings.Where(s => s.Name == ServiceConstants.Settings.USAGE_PROMPT_INTERVAL).FirstOrDefault();

                ServerVersion serverVersion = null;
                if (serverVersionSetting != null)
                {
                    serverVersion = new ServerVersion { Value = serverVersionSetting.Value };
                }

                RefreshInterval refreshInterval = null;
                if (refreshIntervalSetting != null)
                {
                    refreshInterval = new RefreshInterval { Value = int.Parse(refreshIntervalSetting.Value), Description = refreshIntervalSetting.Description };
                }

                LastDeviceListUpdate lastDeviceListUpdate = null;
                if (lastDeviceUpdateSetting != null)
                {
                    lastDeviceListUpdate = new LastDeviceListUpdate { Value = lastDeviceUpdateSetting.Value };
                }

                UsagePromptDuration usagePromptDuration = null;
                if (promptDurationSetting != null)
                {
                    usagePromptDuration = new UsagePromptDuration { Value = int.Parse(promptDurationSetting.Value), Description = promptDurationSetting.Description };
                }

                UsagePromptInterval usagePromptInterval = null;
                if (promptIntervalSetting != null)
                {
                    usagePromptInterval = new UsagePromptInterval { Value = int.Parse(promptIntervalSetting.Value), Description = promptIntervalSetting.Description };
                }

                return new SettingsDetail
                {
                    ServerVersion = serverVersion,
                    RefreshInterval = refreshInterval,
                    LastDeviceListUpdate = lastDeviceListUpdate,
                    UsagePromptDuration = usagePromptDuration,
                    UsagePromptInterval = usagePromptInterval
                };
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, $"Error occured while getting detailed settings");
                throw new ServiceException(ex);
            }
        }

        public async Task UpdateAsync(SettingsDetail settings)
        {
            if (settings.RefreshInterval != null)
            {
                await AddOrUpdateAsync(ServiceConstants.Settings.REFRESH_INTERVAL, settings.RefreshInterval.Value.ToString());
            }
            if (settings.UsagePromptInterval != null)
            {
                await AddOrUpdateAsync(ServiceConstants.Settings.USAGE_PROMPT_INTERVAL, settings.UsagePromptInterval.Value.ToString());
            }
            if (settings.UsagePromptDuration != null)
            {
                await AddOrUpdateAsync(ServiceConstants.Settings.USAGE_PROMPT_DURATION, settings.UsagePromptDuration.Value.ToString());
            }
        }
    }
}
