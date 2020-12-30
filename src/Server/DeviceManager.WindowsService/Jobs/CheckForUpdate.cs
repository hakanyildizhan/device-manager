// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Common;
using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using DeviceManager.Service;
using DeviceManager.Update;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Version = DeviceManager.Update.Version;

namespace DeviceManager.WindowsService.Jobs
{
    public class CheckForUpdate : IJob
    {
        private readonly IUpdateChecker _updater;
        private readonly ILogService<CheckForUpdate> _logService;
        private readonly ISettingsService _settings;

        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public JobType Type => JobType.CheckUpdate;
        public string Schedule { get; set; }
        public bool IsEnabled { get; set; }
        public bool ExecuteNow { get; set; }
        public bool IsAlreadyRunning { get; set; }

        public CheckForUpdate(
            IUpdateChecker updater,
            ILogService<CheckForUpdate> logService,
            ISettingsService settings)
        {
            _updater = updater;
            _logService = logService;
            _settings = settings;
        }

        public async Task Execute()
        {
            try
            {
                var updateJob = DbContext.Jobs.Where(j => j.Type == this.Type).FirstOrDefault();
                UpdateJob updateJobDetail = null;

                if (ExecuteNow)
                {
                    updateJobDetail = DbContext.UpdateJobs.Where(j => j.Job == updateJob && j.Status == JobStatus.UserTriggered).OrderByDescending(j => j.LastUpdate).FirstOrDefault();
                    updateJobDetail.Status = JobStatus.Started;
                    updateJobDetail.Info = "Starting job via manual trigger";
                    updateJobDetail.LastUpdate = DateTime.UtcNow;
                }
                else
                {
                    updateJobDetail = new UpdateJob()
                    {
                        Job = updateJob,
                        Status = JobStatus.Started,
                        Info = "Starting job",
                        LastUpdate = DateTime.UtcNow
                    };

                    DbContext.UpdateJobs.Add(updateJobDetail);
                }

                await DbContext.SaveChangesAsync();
                string serverVersion = _settings.Get(ServiceConstants.Settings.VERSION);
                
                if (string.IsNullOrEmpty(serverVersion))
                {
                    _logService.LogError("Cannot get server version. Aborting CheckUpdate");
                    updateJobDetail.Status = JobStatus.Completed | JobStatus.Failed;
                    updateJobDetail.Info = "Job failed. Could not get server version";
                    updateJobDetail.LastUpdate = DateTime.UtcNow;
                    await DbContext.SaveChangesAsync();
                    return;
                }

                var updateCheckResult = await _updater.CheckForUpdate(new ServerUpdateRequest()
                {
                    ServerVersion = new Version(serverVersion)
                });

                if (!updateCheckResult.Success)
                {
                    _logService.LogError("Update check failed");
                    updateJobDetail.Status = JobStatus.Completed | JobStatus.Failed;
                    updateJobDetail.Info = "Job failed. Error during update check";
                    updateJobDetail.LastUpdate = DateTime.UtcNow;
                    await DbContext.SaveChangesAsync();
                    return;
                }

                if (!updateCheckResult.UpdateIsAvailable)
                {
                    _logService.LogInformation("No update available");
                    updateJobDetail.Status = JobStatus.Completed | JobStatus.Succeeded;
                    updateJobDetail.Info = "No update available";
                    updateJobDetail.LastUpdate = DateTime.UtcNow;
                    await DbContext.SaveChangesAsync();
                    return;
                }

                // an update is available. finish up the updateCheck job
                updateJobDetail.Status = JobStatus.Completed | JobStatus.Succeeded;
                updateJobDetail.Info = "Update available";
                updateJobDetail.LastUpdate = DateTime.UtcNow;
                updateJobDetail.NewVersion = updateCheckResult.Package.Version.ToString();
                updateJobDetail.Uri = updateCheckResult.Package.Url;
                await DbContext.SaveChangesAsync();

                // register an InstallUpdate job
                // are there already existing incomplete installUpdate jobs?

                var installUpdateJob = DbContext.Jobs.Where(j => j.Type == JobType.InstallUpdate).FirstOrDefault();
                var installUpdateJobListDetail = DbContext.UpdateJobs
                                                .Where(j => j.Job == installUpdateJob && 
                                                    !j.Status.HasFlag(JobStatus.Completed))
                                                .ToList();

                if (installUpdateJobListDetail.Any())
                {
                    // finish up previous registrations regarding a previous update
                    var previousRegistrations = installUpdateJobListDetail
                                                .Where(r => new Version(r.NewVersion).Compare(updateCheckResult.Package.Version) == VersionCompareResult.Older)
                                                .ToList();

                    if (previousRegistrations.Any())
                    {
                        foreach (var previousJob in previousRegistrations)
                        {
                            previousJob.LastUpdate = DateTime.UtcNow;
                            previousJob.Status = JobStatus.Completed;
                            previousJob.Info = $"New version ({updateCheckResult.Package.Version}) found, closing this job";
                        }

                        await DbContext.SaveChangesAsync();
                    }

                    // is there a registration for this specific update?
                    var registrationsForSameVersion = installUpdateJobListDetail
                                                .Where(r => new Version(r.NewVersion).Compare(updateCheckResult.Package.Version) == VersionCompareResult.Equal)
                                                .ToList();

                    if (registrationsForSameVersion.Any())
                    {
                        if (registrationsForSameVersion.Count > 1) // if there is more than one, finish up all except one
                        {
                            var registrationsToDelete = registrationsForSameVersion
                                .OrderByDescending(r => r.LastUpdate).Skip(1).ToList();

                            registrationsToDelete.ForEach(d => registrationsForSameVersion.Remove(d));

                            foreach (var previousJob in previousRegistrations)
                            {
                                previousJob.LastUpdate = DateTime.UtcNow;
                                previousJob.Status = JobStatus.Completed;
                                previousJob.Info = $"More than one registration for the newest update ({updateCheckResult.Package.Version}) found, closing this job";
                            }

                            await DbContext.SaveChangesAsync();
                        }

                        var installUpdateJobDetail = registrationsForSameVersion.First();
                        _logService.LogInformation($"An update with version {updateCheckResult.Package.Version} found, previous registration of update version {installUpdateJobDetail.NewVersion} is updated");
                        installUpdateJobDetail.Status = JobStatus.Pending;
                        installUpdateJobDetail.NewVersion = updateCheckResult.Package.Version.ToString();
                        installUpdateJobDetail.Uri = updateCheckResult.Package.Url;
                        installUpdateJobDetail.LastUpdate = DateTime.UtcNow;
                        installUpdateJobDetail.Info = $"Job is updated with version ({updateCheckResult.Package.Version})";
                        await DbContext.SaveChangesAsync();
                    }
                }
                else // no previous registration
                {
                    _logService.LogInformation($"An update with version {updateCheckResult.Package.Version} found, registering job");
                    var installUpdateJobDetail = new UpdateJob()
                    {
                        Job = installUpdateJob,
                        Status = JobStatus.Pending,
                        NewVersion = updateCheckResult.Package.Version.ToString(),
                        Uri = updateCheckResult.Package.Url,
                        LastUpdate = DateTime.UtcNow,
                        Info = "Registered job"
                    };

                    await DbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Error occurred while checking for update");
            }
        }
    }
}
