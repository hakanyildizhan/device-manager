// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Common;
using Microsoft.Win32;
using System;

namespace DeviceManager.Client.Service
{
    /// <summary>
    /// Registry based implementation of <see cref="IRedundantConfigService"/> for Windows operating systems.
    /// </summary>
    public class RegistryService : IRedundantConfigService
    {
        private const string REGISTRY_ADDRESS = "SOFTWARE\\Hakan Yildizhan\\DeviceManager";
        private const string REGISTRY_KEY = "ServerAddress";
        private readonly ILogService _logService;

        public RegistryService(ILogService<RegistryService> logService)
        {
            _logService = logService;
        }

        public bool CanUseToastNotifications()
        {
            int OSMajorVersion = (int)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", 0);
            return OSMajorVersion >= 10;
        }

        public bool CanUseToastProgressBars()
        {
            int OSMajorVersion = (int)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", 0);

            if (OSMajorVersion < 10)
            {
                return false;
            }

            int win10ReleaseId = (int)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", 0);
            return win10ReleaseId >= 1703;
        }

        public string FetchServerURL()
        {
            RegistryKey regKey = Registry.CurrentUser;
            string url = string.Empty;

            try
            {
                regKey = regKey.OpenSubKey(REGISTRY_ADDRESS, false);

                if (regKey != null)
                {
                    url = regKey.GetValue(REGISTRY_KEY, string.Empty).ToString();
                    regKey.Close();

                    if (string.IsNullOrEmpty(url))
                    {
                        _logService.LogError("Key value is empty");
                    }
                }
                else
                {
                    _logService.LogError("Key does not exist");
                }
            }
            catch (Exception e)
            {
                _logService.LogException(e, "Could not read server URL from registry");
            }

            return url;
        }

        public bool StoreServerURL(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                _logService.LogError("Server URL is empty, aborting write");
                return false;
            }

            RegistryKey regKey = Registry.CurrentUser;
            try
            {
                regKey = regKey.CreateSubKey(REGISTRY_ADDRESS, true);
                regKey.SetValue(REGISTRY_KEY, url, RegistryValueKind.String);
                regKey.Close();
                return true;
            }
            catch (Exception e)
            {
                _logService.LogException(e, "Could not write server URL to registry");
                return false;
            }
        }
    }
}
