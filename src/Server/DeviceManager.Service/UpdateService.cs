// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Common;
using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using DeviceManager.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Unity;

namespace DeviceManager.Service
{
    public class UpdateService : IUpdateService
    {
        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        private readonly ILogService _logService;

        public UpdateService(ILogService<UpdateService> logService)
        {
            _logService = logService;
        }

        public async Task<bool> ScheduleUpdate()
        {
            try
            {
                var installUpdateJob = DbContext.Jobs.Where(j => j.Type == JobType.InstallUpdate).FirstOrDefault();
                var installUpdateJobDetail = DbContext.UpdateJobs.Where(j => j.Job == installUpdateJob && j.Status == JobStatus.Registered).FirstOrDefault();

                if (installUpdateJobDetail == null)
                {
                    _logService.LogError("ScheduleUpdate failed. No job is registered yet");
                    return false;
                }

                installUpdateJobDetail.LastUpdate = DateTime.UtcNow;
                installUpdateJobDetail.Status = JobStatus.Scheduled;
                installUpdateJobDetail.Info = "Update scheduled by user";
                await DbContext.SaveChangesAsync();
                _logService.LogInformation($"Update ({installUpdateJobDetail.NewVersion}) is scheduled");
                return true;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "Update could not be scheduled");
                return false;
            }
        }

        public UpdateInfo GetAvailableUpdateInformation()
        {
            Job installUpdateJob = DbContext.Jobs.Where(j => j.Type == JobType.InstallUpdate).FirstOrDefault();

            if (installUpdateJob == null)
            {
                return new UpdateInfo() 
                { 
                    UpdateAvailable = false, 
                    Info = @"Error: ""InstallUpdate"" job was not found in the database." 
                };
            }

            if (!installUpdateJob.IsEnabled)
            {
                return new UpdateInfo()
                {
                    UpdateAvailable = false,
                    Info = @"Error: ""InstallUpdate"" job is not enabled. Please enable it in order to check for updates."
                };
            }

            UpdateJob updateJobDetail = DbContext.UpdateJobs.Where(j => j.Job == installUpdateJob).FirstOrDefault();

            if (updateJobDetail == null)
            {
                return new UpdateInfo() 
                { 
                    UpdateAvailable = false,
                    Info = "No update available."
                };
            }

            return new UpdateInfo() { UpdateAvailable = true, UpdateVersion = updateJobDetail.NewVersion };
        }

        public bool IsServiceRunning()
        {
            ServiceController sc = new ServiceController("DeviceManagerUpdateService");
            return sc.Status == ServiceControllerStatus.Running;
        }

        public IList<ScheduledTask> GetTaskList()
        {
            var jobs = DbContext.Jobs.ToList();
            var taskList = new List<ScheduledTask>();

            foreach (var job in jobs)
            {
                taskList.Add(new ScheduledTask()
                {
                    Id = job.Id,
                    IsEnabled = job.IsEnabled,
                    Schedule = job.Schedule,
                    Title = job.Type.GetDescription()
                });
            }

            return taskList;
        }

        public async Task<bool> UpdateTaskList(IList<ScheduledTask> tasks)
        {
            try
            {
                foreach (var task in tasks)
                {
                    var job = DbContext.Jobs.Where(j => j.Id == task.Id).FirstOrDefault();

                    if (job != null)
                    {
                        job.IsEnabled = task.IsEnabled;
                        job.Schedule = task.Schedule;
                    }
                }

                await DbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex, "An error occurred while updating task list");
                return false;
            }
        }
    }
}
