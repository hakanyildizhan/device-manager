﻿using DeviceManager.Entity.Context;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Service
{
    public class DeviceService : IDeviceService
    {
        private readonly ISettingsService _settingsService;

        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public DeviceService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public IEnumerable<Device> GetDevices()
        {
            foreach (var d in DbContext.Devices.Where(d => d.IsActive).AsQueryable())
            {
                Entity.Context.Entity.Session activeSession =
                    DbContext.Sessions.Any(s => s.IsActive && s.Device.Id == d.Id) ?
                       DbContext.Sessions.Where(s => s.IsActive && s.Device.Id == d.Id).FirstOrDefault() :
                       null;

                yield return new Device
                {
                    Id = d.Id,
                    HardwareInfo = d.HardwareInfo,
                    Address = $"{d.Address}{(string.IsNullOrEmpty(d.Address2) ? "" : $", {d.Address2}")}",
                    ConnectedModuleInfo = d.ConnectedModuleInfo,
                    IsAvailable = !DbContext.Sessions.Any(s => s.IsActive && s.Device.Id == d.Id),
                    Name = d.Name,
                    DeviceGroup = d.Group,
                    UsedBy = activeSession?.User?.DomainUsername,
                    UsedByFriendly = activeSession?.User?.FriendlyName
                };
            }
        }

        public async Task<bool> Import(IEnumerable<DeviceImport> deviceData)
        {
            try
            {
                using (var transaction = DbContext.Database.BeginTransaction())
                {
                    DbContext.Devices.ToList().ForEach(d => { d.IsActive = false; });
                    await DbContext.SaveChangesAsync();
                    foreach (Entity.Context.Entity.Device item in deviceData.ToDevice())
                    {
                        try
                        {
                            DbContext.Devices.Add(item);
                            await DbContext.SaveChangesAsync();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException)
                        {
                            // validation error
                            DbContext.Devices.Remove(item);
                        }
                    }

                    bool lastUpdateSaved = await _settingsService.AddOrUpdate(ServiceConstants.Settings.LAST_DEVICE_LIST_UPDATE, DateTime.UtcNow.ToStringTurkish());

                    if (lastUpdateSaved)
                    {
                        transaction.Commit();
                    }
                }
                return true;
            }
            catch (Exception) //TODO: log
            {
                return false;
            }
        }
    }
}