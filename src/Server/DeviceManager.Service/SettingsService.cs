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
    }
}
