// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Common;
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
        private readonly ILogService<UpdateManager> _logService;

        public UpdateManager(
            IUpdateChecker updater, 
            IConfigurationService configService,
            IFeedbackService feedbackService,
            IProgressBarService progressBar,
            ILogService<UpdateManager> logService)
        {
            _updater = updater;
            _configService = configService;
            _feedbackService = feedbackService;
            _progressBar = progressBar;
            _logService = logService;
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

            string appVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();

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

            string installerExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceManager.Installer.exe");

            if (!File.Exists(installerExePath))
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "An error occurred. Cannot install update at this time.");
                _logService.LogError("Installer was not found, could not install update");
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

            await _feedbackService.ShowMessageAsync(MessageType.Information, "Installing the update...");
            string tempInstaller = Path.Combine(Utility.GetAppRoamingFolder(), "DeviceManager.Installer.exe");

            try
            {
                File.Copy(installerExePath, tempInstaller, true);
            }
            catch (Exception ex)
            {
                await _feedbackService.ShowMessageAsync(MessageType.Error, "An error occurred. Cannot install update at this time.");
                _logService.LogException(ex, "Installer could not be copied. Could not install update");
                return;
            }

            await Task.Run(() =>
            {
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo(tempInstaller, $@"{Update.Version} ""{result.File}""");
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            });
            Reset();
        }

        public void Reset()
        {
            Update = null;
            UpdateIsAvailable = false;
            _updater.DownloadProgressChanged -= HandleDownloadProgressChanged;

            string tempInstaller = Path.Combine(Utility.GetAppRoamingFolder(), "DeviceManager.Installer.exe");

            if (File.Exists(tempInstaller))
            {
                try
                {
                    File.Delete(tempInstaller);
                }
                catch (Exception ex)
                {
                    _logService.LogException(ex, "Could not delete temp installer after update");
                    return;
                }
            }
        }

        private void HandleDownloadProgressChanged(object sender, int newProgress)
        {
            string requestId = (string)sender;
            _progressBar?.IncreaseProgress(requestId, newProgress);
        }
    }
}
