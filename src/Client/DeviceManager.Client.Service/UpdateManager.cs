// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Update;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Version = DeviceManager.Update.Version;

namespace DeviceManager.Client.Service
{
    public class UpdateManager : IUpdateManager
    {
        private readonly IUpdateChecker _updater;
        private readonly IConfigurationService _configService;
        private readonly IFeedbackService _feedbackService;
        private readonly IProgressBarService _progressBar;

        public UpdateManager(
            IUpdateChecker updater, 
            IConfigurationService configService,
            IFeedbackService feedbackService,
            IProgressBarService progressBar)
        {
            _updater = updater;
            _configService = configService;
            _feedbackService = feedbackService;
            _progressBar = progressBar;
        }

        public bool UpdateIsAvailable { get; set; }
        //public bool UpdateIsReadyToInstall { get; set; }
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

        public async Task DownloadAndInstall()
        {
            if (!UpdateIsAvailable || Update == null)
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "An error occurred. Please try again later.");
                return;
            }

            string requestId = Guid.NewGuid().ToString();
            _updater.DownloadProgressChanged += HandleDownloadProgressChanged;
            _progressBar?.Show("Downloading update", $"Update v{Update.Version}", "Downloading...", requestId);
            var result = await _updater.DownloadUpdate(new UpdateDownloadRequest()
            {
                Package = Update,
                TargetDirectory = Path.GetTempPath(),
                RequestId = requestId
            });

            _progressBar?.Close(requestId);

            if (!result.Success || string.IsNullOrEmpty(result.File))
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "An error occurred while downloading the update.");
                return;
            }

            Reset();
            await _feedbackService.ShowMessageAsync(MessageType.Information, "Installing the update...");

            string installerExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceManager.Installer.exe");
            await Task.Run(() =>
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo(installerExePath, $@"{Update.Version} ""{result.File}""");
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            });
        }

        public void Reset()
        {
            Update = null;
            UpdateIsAvailable = false;
            _updater.DownloadProgressChanged -= HandleDownloadProgressChanged;
            //UpdateIsReadyToInstall = false;
        }

        private void HandleDownloadProgressChanged(object sender, int newProgress)
        {
            string requestId = (string)sender;
            _progressBar?.IncreaseProgress(requestId, newProgress);
        }
    }
}
