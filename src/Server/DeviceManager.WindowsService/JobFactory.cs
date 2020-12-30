// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Common;
using DeviceManager.Entity.Context;
using DeviceManager.Entity.Context.Entity;
using DeviceManager.WindowsService.Jobs;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace DeviceManager.WindowsService
{
    public class JobFactory : IJobFactory
    {
        private readonly List<IJob> _jobs;
        private readonly ILogService<JobFactory> _logService;

        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public JobFactory(List<IJob> jobs, ILogService<JobFactory> logService)
        {
            _logService = logService;

            foreach (var job in jobs)
            {
                var jobInDB = DbContext.Jobs.Where(j => j.Type == job.Type).FirstOrDefault();

                if (jobInDB == null)
                {
                    _logService.LogError($"Job of type {job.Type} does not exist in the database.");
                    continue;
                }

                job.Schedule = jobInDB.Schedule;
                job.IsEnabled = jobInDB.IsEnabled;

                if (job.Type == JobType.CheckUpdate || job.Type == JobType.InstallUpdate)
                {
                    var jobDetail = DbContext.UpdateJobs.Where(u => u.Job == job)
                        .OrderByDescending(u => u.LastUpdate).FirstOrDefault();

                    if (jobDetail != null)
                    {
                        if (jobDetail.Status == JobStatus.UserTriggered)
                        {
                            job.ExecuteNow = true;
                        }
                        else if (jobDetail.Status == JobStatus.Started)
                        {
                            job.IsAlreadyRunning = true;
                        }
                    }
                }
            }

            _jobs = jobs;
        }

        public IJob Get(JobType jobType)
        {
            return _jobs.SingleOrDefault(j => j.Type == jobType);
        }

        public IList<IJob> GetAll()
        {
            return _jobs;
        }
    }
}
