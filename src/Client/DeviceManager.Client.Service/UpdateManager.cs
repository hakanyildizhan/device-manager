// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Update;
using System;
using System.Threading.Tasks;
using Version = DeviceManager.Update.Version;

namespace DeviceManager.Client.Service
{
    public class UpdateManager : IUpdateManager
    {
        private readonly IUpdateChecker _updater;
        private readonly IConfigurationService _configService;
        private readonly IFeedbackService _feedbackService;

        public UpdateManager(
            IUpdateChecker updater, 
            IConfigurationService configService,
            IFeedbackService feedbackService)
        {
            _updater = updater;
            _configService = configService;
        }

        public bool UpdateIsAvailable { get; set; }
        public string InstallerPath { get; set; }
        public UpdatePackage Update { get; set; }

        public async Task CheckUpdate()
        {
            Reset();
            string serverVersion = _configService.Get(ServiceConstants.Settings.SERVER_VERSION);

            if (string.IsNullOrEmpty(serverVersion))
            {
                return;
            }

            string appVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            var updateCheckResult = await _updater.CheckForUpdate(new ClientUpdateRequest()
            {
                ClientVersion = new Version(appVersion),
                ServerVersion = new Version(serverVersion)
            });

            if (updateCheckResult.Success && updateCheckResult.UpdateIsAvailable)
            {
                Update = updateCheckResult.Package;
                UpdateIsAvailable = true;
            }
        }

        public Task DownloadAndInstall()
        {
            throw new NotImplementedException();
            Reset();
        }

        public void Reset()
        {
            Update = null;
            UpdateIsAvailable = false;
        }
    }
}
