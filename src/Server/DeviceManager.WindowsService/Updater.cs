// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Common;
using DeviceManager.Service;
using DeviceManager.WindowsService.Jobs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace DeviceManager.WindowsService
{
    public partial class Updater : UpdateServiceBase
    {
        static Timer _timer = new Timer();
        private readonly IJobFactory JobFactory = (IJobFactory) ServiceProvider.GetService<IJobFactory>();
        private readonly IScheduleParser ScheduleParser = (IScheduleParser) ServiceProvider.GetService<IScheduleParser>();
        private readonly ILogService<Updater> _logger = (ILogService<Updater>) ServiceProvider.GetService<ILogService<Updater>>();

        public Updater()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            _timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            _timer.Start();
        }

        private async void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IList<IJob> jobs = JobFactory.GetAll();

            foreach (var job in jobs)
            {
                // job is not enabled, therefore will never run
                if (!job.IsEnabled)
                {
                    continue;
                }

                // job is already running
                if (job.IsAlreadyRunning)
                {
                    continue;
                }

                // job is triggered manually. execute immediately
                if (job.ExecuteNow)
                {
                    await ExecuteJob(job);
                    continue;
                }

                // job is enabled and is set to run automatically
                bool scheduleIsValid = ScheduleParser.Check(job.Schedule).IsValid;

                // invalid schedule. abort
                if (!scheduleIsValid)
                {
                    continue;
                }

                DateTime? nextOccurence = ScheduleParser.GetNextOccurence(job.Schedule);

                // schedule shows no incoming executing time. abort
                if (nextOccurence == null)
                {
                    continue;
                }

                // if it is time to run, execute the job
                bool timeToRun = (DateTime.UtcNow - nextOccurence.Value).TotalMinutes == 0;

                if (timeToRun)
                {
                    await ExecuteJob(job);
                    continue;
                }
            }
        }

        protected override void OnStop()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private async Task ExecuteJob(IJob job)
        {
            try
            {
                await job.Execute();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"Error occurred while executing job (type: {job.Type.ToString()} / {job.Type.GetDescription()})");
            }
        }
    }
}
