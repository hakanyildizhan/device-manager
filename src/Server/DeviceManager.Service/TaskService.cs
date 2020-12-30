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
    public class TaskService : ITaskService
    {
        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        private readonly ILogService _logService;

        public TaskService(ILogService<TaskService> logService)
        {
            _logService = logService;
        }

        public async Task<bool> TriggerManually(int taskId)
        {
            try
            {
                if (!CanBeTriggeredManually(taskId))
                {
                    return false;
                }

                var job = DbContext.Jobs.Where(j => j.Id == taskId).FirstOrDefault();

                if (job == null)
                {
                    _logService.LogError($"Job with ID {taskId} was not found.");
                    return false;
                }

                UpdateJob jobDetail = null;

                if (job.IsIndependent)
                {
                    jobDetail = new UpdateJob();
                    jobDetail.Job = job;
                    jobDetail.LastUpdate = DateTime.UtcNow;
                    jobDetail.Status = JobStatus.UserTriggered;
                    jobDetail.Info = "Job triggered by user";
                    DbContext.UpdateJobs.Add(jobDetail);
                }
                else
                {
                    jobDetail = DbContext.UpdateJobs.Where(j => j.Job == job && j.Status == JobStatus.Pending)
                    .OrderByDescending(j => j.LastUpdate).FirstOrDefault();

                    if (jobDetail == null)
                    {
                        _logService.LogError("Job could not be triggered manually. No job is registered yet");
                        return false;
                    }

                    jobDetail.LastUpdate = DateTime.UtcNow;
                    jobDetail.Status = JobStatus.UserTriggered;
                    jobDetail.Info = "Job triggered by user";
                }

                await DbContext.SaveChangesAsync();
                _logService.LogInformation($"Update ({jobDetail.NewVersion}) is scheduled");
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
                UpdateJob jobDetail = null;
                if (job.Type == JobType.CheckUpdate || job.Type == JobType.InstallUpdate)
                {
                    jobDetail = DbContext.UpdateJobs.Where(u => u.Job == job).OrderByDescending(u => u.LastUpdate).FirstOrDefault();
                }

                taskList.Add(new ScheduledTask()
                {
                    Id = job.Id,
                    IsEnabled = job.IsEnabled,
                    CanBeTriggeredManually = CanBeTriggeredManually(job.Id),
                    Schedule = job.Schedule,
                    Title = job.Type.GetDescription(),
                    LastRun = jobDetail?.LastUpdate,
                    LastRunInformation = jobDetail?.Info,
                    LastRunStatus = jobDetail == null ? "Not Run" : jobDetail.Status.ToString()
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

        public bool CanBeTriggeredManually(int taskId)
        {
            var job = DbContext.Jobs.SingleOrDefault(j => j.Id == taskId);

            if (job == null)
            {
                return false;
            }

            var jobDetail = DbContext.UpdateJobs.Where(u => u.Job == job)
                    .OrderByDescending(u => u.LastUpdate)
                    .FirstOrDefault();

            // Independent jobs do not require a prep-job to be completed beforehand.
            // Dependent jobs need to be in Pending state; other states will indicate that the job is already running,
            // or the prep-job was not run in the first place.
            // For both cases, no other job of the same type should be already running.
            if (job.IsIndependent)
            {
                return jobDetail == null || (jobDetail != null && jobDetail.Status.HasFlag(JobStatus.Completed));
            }
            else
            {
                return jobDetail != null && jobDetail.Status == JobStatus.Pending;
            }
        }
    }
}
