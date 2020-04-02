using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Service
{
    public class SettingsService : ISettingsService
    {
        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public Dictionary<string, string> Get()
        {
            //Settings settings = new Settings();
            Dictionary<string, string> settings = new Dictionary<string, string>();

            DbContext.Settings.ToList().ForEach(s => settings.Add(s.Name, s.Value));

            //Setting refreshIntervalSetting = DbContext.Settings.Where(s => s.Name == "refreshInterval").FirstOrDefault();
            //if (refreshIntervalSetting != null)
            //{
            //    settings.RefreshInterval = int.Parse(refreshIntervalSetting.Value);
            //}

            //Setting serverVersionSetting = DbContext.Settings.Where(s => s.Name == "serverVersion").FirstOrDefault();
            //if (serverVersionSetting != null)
            //{
            //    settings.ServerVersion = serverVersionSetting.Value;
            //}

            //Setting lastDeviceListUpdateSetting = DbContext.Settings.Where(s => s.Name == "lastDeviceListUpdate").FirstOrDefault();
            //if (lastDeviceListUpdateSetting != null)
            //{
            //    settings.LastDeviceListUpdate = DateTimeHelpers.Parse(lastDeviceListUpdateSetting.Value);
            //}

            return settings;
        }

        public async Task<bool> AddOrUpdate(string name, string value)
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
            catch (Exception) //TODO: log
            {
                return false;
            }
            
        }

        public string Get(string keyName)
        {
            return DbContext.Settings.FirstOrDefault(s => s.Name == keyName)?.Value;
        }
    }
}
