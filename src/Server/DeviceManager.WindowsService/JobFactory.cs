// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

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

        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public JobFactory(List<IJob> jobs)
        {
            foreach (var job in jobs)
            {
                var jobInDB = DbContext.Jobs.Where(j => j.Type == job.Type).FirstOrDefault();
                job.Schedule = jobInDB.Schedule;
                job.IsEnabled = jobInDB.IsEnabled;
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
