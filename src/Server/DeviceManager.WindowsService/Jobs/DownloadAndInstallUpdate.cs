// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Common;
using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using DeviceManager.Update;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Version = DeviceManager.Update.Version;

namespace DeviceManager.WindowsService.Jobs
{
    public class DownloadAndInstallUpdate : IJob
    {
        private readonly IUpdateChecker _updater;
        private readonly ILogService<CheckForUpdate> _logService;

        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public JobType Type => JobType.InstallUpdate;
        public string Schedule { get; set; }
        public bool IsEnabled { get; set; }
        public bool ExecuteNow { get; set; }
        public bool IsAlreadyRunning { get; set; }

        public DownloadAndInstallUpdate(
            IUpdateChecker updater,
            ILogService<CheckForUpdate> logService)
        {
            _updater = updater;
            _logService = logService;
        }

        public async Task Execute()
        {
            if (!ExecuteNow)
            {
                return;
            }

            var installUpdateJob = DbContext.Jobs.Where(j => j.Type == JobType.InstallUpdate).FirstOrDefault();
            var installUpdateJobDetail = DbContext.UpdateJobs.Where(j => j.Job == installUpdateJob && j.Status == JobStatus.UserTriggered).OrderByDescending(j => j.LastUpdate).FirstOrDefault();
            installUpdateJobDetail.Status = JobStatus.Started;
            installUpdateJobDetail.LastUpdate = DateTime.UtcNow;
            installUpdateJobDetail.Info = "Starting job";
            await DbContext.SaveChangesAsync();

            var result = await _updater.DownloadUpdate(new UpdateDownloadRequest()
            {
                Package = new UpdatePackage() { Url = installUpdateJobDetail.Uri, Version = new Version(installUpdateJobDetail.NewVersion.ToString()) },
                TargetDirectory = Path.GetTempPath()
            });

            if (!result.Success || string.IsNullOrEmpty(result.File))
            {
                _logService.LogError("An error occurred while downloading the update.");
                installUpdateJobDetail.Status = JobStatus.Completed | JobStatus.Failed;
                installUpdateJobDetail.LastUpdate = DateTime.UtcNow;
                installUpdateJobDetail.Info = "Error occured while downloading update";
                await DbContext.SaveChangesAsync();
                return;
            }

            try
            {
                await Task.Run(() =>
                {
                    Process process = new Process();
                    process.StartInfo = new ProcessStartInfo("msiexec", $@"/i ""{result.File}"" /qn");
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                });

                _logService.LogInformation("Update installation is completed successfully");
                installUpdateJobDetail.Status = JobStatus.Completed | JobStatus.Succeeded;
                installUpdateJobDetail.LastUpdate = DateTime.UtcNow;
                installUpdateJobDetail.Info = "Update installation is completed successfully";
                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Error occured while installing update");
                installUpdateJobDetail.Status = JobStatus.Completed | JobStatus.Failed;
                installUpdateJobDetail.LastUpdate = DateTime.UtcNow;
                installUpdateJobDetail.Info = "Error occured while installing update";
                await DbContext.SaveChangesAsync();
            }
        }
    }
}
