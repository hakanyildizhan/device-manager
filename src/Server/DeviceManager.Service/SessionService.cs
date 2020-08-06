// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Service
{
    public class SessionService : ISessionService
    {
        private readonly ILogService _logService;

        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public SessionService(ILogService<SessionService> logService)
        {
            _logService = logService;
        }

        public async Task<bool> CreateSessionAsync(string userName, int deviceId)
        {
            try
            {
                Session activeSession = DbContext.Sessions
                .Where(s => s.Device.Id == deviceId &&
                    s.Client.DomainUsername == userName &&
                    s.IsActive).FirstOrDefault();

                if (activeSession != null)
                {
                    return false;
                }

                DbContext.Sessions.Add(new Session
                {
                    Device = await DbContext.Devices.FindAsync(deviceId),
                    Client = await DbContext.Clients.FindAsync(userName),
                    StartedAt = DateTime.UtcNow,
                    IsActive = true
                });
                
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (System.Data.DataException ex)
            {
                _logService.LogException(ex, "DataException occured while creating session");
                return false;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while creating session");
                throw new ServiceException(ex);
            }
        }

        public async Task<bool> EndActiveSessionsAsync()
        {
            try
            {
                DbContext.Sessions.Where(s => s.IsActive).ToList().ForEach(s =>
                {
                    s.IsActive = false;
                    s.FinishedAt = DateTime.UtcNow;
                });
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (System.Data.DataException ex)
            {
                _logService.LogException(ex, "DataException occured while ending all active sessions");
                return false;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while ending all active sessions");
                return false;
            }
        }

        public async Task<bool> EndSessionAsync(string userName, int deviceId)
        {
            try
            {
                Session activeSession = DbContext.Sessions
                .Where(s => s.Device.Id == deviceId &&
                    s.Client.DomainUsername == userName &&
                    s.IsActive).FirstOrDefault();

                // Either device ID is invalid, or the device is already checked in
                if (activeSession == null)
                    return false;

                activeSession.FinishedAt = DateTime.UtcNow;
                activeSession.IsActive = false;
                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (System.Data.DataException ex)
            {
                _logService.LogException(ex, "DataException occured while ending session");
                return false;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while ending session");
                throw new ServiceException(ex);
            }
        }

        public async Task<bool> EndAllSessionsOfUserAsync(string userName)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    IList<Session> activeSessions = DbContext.Sessions
                        .Where(s => s.Client.DomainUsername == userName && s.IsActive)
                        .ToList();

                    // currently there is no active session for the user, succeed anyway
                    if (!activeSessions.Any())
                        return true;

                    foreach (Session activeSession in activeSessions)
                    {
                        activeSession.FinishedAt = DateTime.UtcNow;
                        activeSession.IsActive = false;
                        await DbContext.SaveChangesAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logService.LogException(ex, $"Unknown error occured during ending all sessions for user {userName}");
                    throw new ServiceException(ex);
                }
            }
            return true;
        }

        public bool IsDeviceAvailable(int deviceId)
        {
            try
            {
                Entity.Context.Entity.Device device = DbContext.Devices.Find(deviceId);

                if (device == null)
                {
                    return false;
                }

                return !DbContext.Sessions.Any(s => s.Device.Id == deviceId && s.IsActive);
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while checking device availability");
                throw new ServiceException(ex);
            }
        }

        public IEnumerable<DeviceSessionInfo> GetDeviceSessions()
        {
            try
            {
                IList<DeviceSessionInfo> deviceSessions = new List<DeviceSessionInfo>();

                foreach (var device in DbContext.Devices.AsQueryable())
                {
                    bool isBusy = DbContext.Sessions.Any(s => s.IsActive && s.Device.Id == device.Id);
                    string usedBy = string.Empty;
                    string usedByFriendly = string.Empty;
                    DateTime? checkoutDate = null;

                    if (isBusy)
                    {
                        Session session = DbContext.Sessions.Where(s => s.IsActive && s.Device.Id == device.Id).FirstOrDefault();
                        usedBy = session.Client.DomainUsername;
                        usedByFriendly = session.Client.FriendlyName;
                        checkoutDate = session.StartedAt;
                    }

                    deviceSessions.Add(new DeviceSessionInfo
                    {
                        DeviceId = device.Id,
                        IsAvailable = !isBusy,
                        UsedBy = usedBy,
                        UsedByFriendly = usedByFriendly,
                        CheckoutDate = checkoutDate
                    });
                }
                return deviceSessions;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Unknown error occured while getting active sessions");
                throw new ServiceException(ex);
            }
        }
    }
}
