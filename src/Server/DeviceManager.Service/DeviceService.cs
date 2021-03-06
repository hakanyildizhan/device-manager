// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Entity.Context;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Service
{
    public class DeviceService : IDeviceService
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogService _logService;
        private readonly ISessionService _sessionService;

        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public DeviceService(
            ISettingsService settingsService,
            ISessionService sessionService,
            ILogService<DeviceService> logService)
        {
            _settingsService = settingsService;
            _sessionService = sessionService;
            _logService = logService;
        }

        public IEnumerable<Device> GetDevices()
        {
            try
            {
                IList<Device> devices = new List<Device>();

                foreach (var d in DbContext.Devices.Where(d => d.IsActive).AsQueryable())
                {
                    Entity.Context.Entity.Session activeSession =
                        DbContext.Sessions.Any(s => s.IsActive && s.Device.Id == d.Id) ?
                           DbContext.Sessions.Where(s => s.IsActive && s.Device.Id == d.Id).FirstOrDefault() :
                           null;

                    devices.Add(new Device
                    {
                        Id = d.Id,
                        HardwareInfo = d.HardwareInfo,
                        Address = $"{d.Address}{(string.IsNullOrEmpty(d.Address2) ? "" : $", {d.Address2}")}",
                        ConnectedModuleInfo = d.ConnectedModuleInfo,
                        IsAvailable = !DbContext.Sessions.Any(s => s.IsActive && s.Device.Id == d.Id),
                        Name = d.Name,
                        DeviceGroup = d.Group,
                        UsedBy = activeSession?.Client?.DomainUsername,
                        UsedByFriendly = activeSession?.Client?.FriendlyName,
                        CheckoutDate = activeSession?.StartedAt
                    });
                }
                return devices;
            }
            catch (System.Data.DataException ex)
            {
                _logService.LogException(ex, "DataException occured while getting devices");
                return null;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while getting devices");
                throw new ServiceException(ex);
            }
        }

        public async Task<bool> Import(IEnumerable<DeviceImport> deviceData)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    // end all active sessions
                    _logService.LogInformation("Ending all active sessions");
                    bool success = await _sessionService.EndActiveSessionsAsync();

                    if (!success)
                    {
                        _logService.LogError("Error occured during hardware import. Could not end active sessions");
                        transaction.Rollback();
                        return false;
                    }

                    _logService.LogInformation("Deactivating all existing devices");
                    DbContext.Devices.ToList().ForEach(d => { d.IsActive = false; });
                    await DbContext.SaveChangesAsync();

                    _logService.LogInformation("Adding imported devices");
                    foreach (Entity.Context.Entity.Device item in deviceData.ToDevice())
                    {
                        try
                        {
                            DbContext.Devices.Add(item);
                            await DbContext.SaveChangesAsync();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException)
                        {
                            // validation error, i.e. a required property is empty
                            // discard entire item
                            DbContext.Devices.Remove(item);
                        }
                    }

                    _logService.LogInformation("Updating device list update date");
                    await UpdateDeviceListUpdateDate();
                    _logService.LogInformation("Finishing up device import");
                    transaction.Commit();
                    return true;
                }
                catch (System.Data.DataException ex)
                {
                    _logService.LogException(ex, "DataException occured during hardware import");
                    transaction.Rollback();
                    return false;
                }
                catch (Exception ex)
                {
                    _logService.LogException(ex, "Unknown error occured during hardware import");
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public DeviceDetail GetDevice(int id)
        {
            Entity.Context.Entity.Device device = DbContext.Devices.Find(id);

            if (device == null)
            {
                _logService.LogError($"Device with Id {id} was not found. Could not fetch device");
                return null;
            }

            return new DeviceDetail
            {
                Id = device.Id,
                Info = device.HardwareInfo,
                Address = device.Address,
                Address2 = device.Address2,
                ConnectedModuleInfo = device.ConnectedModuleInfo,
                Name = device.Name,
                Group = device.Group
            };
        }

        public async Task<bool> AddDevice(DeviceDetail device)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    DbContext.Devices.Add(new Entity.Context.Entity.Device()
                    {
                        Name = device.Name,
                        Group = device.Group,
                        Address = device.Address,
                        Address2 = device.Address2,
                        ConnectedModuleInfo = device.ConnectedModuleInfo,
                        HardwareInfo = device.Info,
                        IsActive = true
                    });

                    await DbContext.SaveChangesAsync();
                    await UpdateDeviceListUpdateDate();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    _logService.LogException(ex, "Error occured during adding new device item");
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> UpdateDevice(DeviceDetail device)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                var existingDevice = await DbContext.Devices.FindAsync(device.Id);

                if (existingDevice == null)
                {
                    _logService.LogError($"Device with Id {device.Id} was not found. Could not update device");
                    return false;
                }

                try
                {
                    existingDevice.Name = device.Name;
                    existingDevice.Group = device.Group;
                    existingDevice.HardwareInfo = device.Info;
                    existingDevice.ConnectedModuleInfo = device.ConnectedModuleInfo;
                    existingDevice.Address = device.Address;
                    existingDevice.Address2 = device.Address2;

                    await DbContext.SaveChangesAsync();
                    await UpdateDeviceListUpdateDate();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    _logService.LogException(ex, "Error occured during device item update");
                    transaction.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> DeactivateDevice(int deviceId)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                var existingDevice = await DbContext.Devices.FindAsync(deviceId);

                if (existingDevice == null)
                {
                    _logService.LogError($"Device with Id {deviceId} was not found. Could not deactivate device");
                    return false;
                }

                try
                {
                    existingDevice.IsActive = false;
                    await DbContext.SaveChangesAsync();
                    await UpdateDeviceListUpdateDate();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    _logService.LogException(ex, "Error occured during deleting device item");
                    transaction.Rollback();
                    return false;
                }
            }
        }

        private async Task<bool> UpdateDeviceListUpdateDate()
        {
            return await _settingsService.AddOrUpdateAsync(ServiceConstants.Settings.LAST_DEVICE_LIST_UPDATE, DateTime.UtcNow.ToStringTurkish());
        }
    }
}
