﻿using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Service
{
    public class SessionService : ISessionService
    {
        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public async Task<bool> CreateSessionAsync(string userName, int deviceId)
        {
            Session activeSession = DbContext.Sessions
                .Where(s => s.Device.Id == deviceId &&
                    s.User.DomainUsername == userName &&
                    s.IsActive).FirstOrDefault();

            if (activeSession != null)
            {
                return false;
            }

            DbContext.Sessions.Add(new Session
            {
                Device = await DbContext.Devices.FindAsync(deviceId),
                User = await DbContext.Users.FindAsync(userName),
                StartedAt = DateTime.UtcNow,
                IsActive = true
            });
            await DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EndSessionAsync(string userName, int deviceId)
        {
            Session activeSession = DbContext.Sessions
                .Where(s => s.Device.Id == deviceId && 
                    s.User.DomainUsername == userName &&
                    s.IsActive).FirstOrDefault();

            if (activeSession == null)
                return true;

            activeSession.FinishedAt = DateTime.UtcNow;
            activeSession.IsActive = false;
            await DbContext.SaveChangesAsync();
            return true;
        }

        public bool IsDeviceAvailable(int deviceId)
        {
            Device device = DbContext.Devices.Find(deviceId);

            if (device == null)
            {
                return false;
            }

            return !DbContext.Sessions.Any(s => s.Device.Id == deviceId && s.IsActive);
        }
    }
}
