// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Common;
using DeviceManager.Service;
using DeviceManager.WindowsService.Jobs;
using System;
using System.Collections.Generic;
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
            _timer.Enabled = true;
            _timer.Start();
        }

        private async void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IList<IJob> jobs = JobFactory.GetAll();

            foreach (var job in jobs)
            {
                if (!job.IsEnabled)
                {
                    continue;
                }

                bool scheduleIsValid = ScheduleParser.Check(job.Schedule).IsValid;

                if (!scheduleIsValid)
                {
                    continue;
                }

                DateTime? nextOccurence = ScheduleParser.GetNextOccurence(job.Schedule);

                if (nextOccurence == null)
                {
                    return;
                }

                bool timeToRun = (DateTime.UtcNow - nextOccurence.Value).TotalMinutes == 0;

                if (timeToRun)
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

        protected override void OnStop()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
