// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Entity.Context.Entity;
using System.Threading.Tasks;

namespace DeviceManager.WindowsService.Jobs
{
    /// <summary>
    /// A job scheduled to run in certain intervals.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Type of the job. Each type of job should exist in the <see cref="Job"/> table.
        /// </summary>
        JobType Type { get; }

        /// <summary>
        /// Schedule that this job follows to execute automatically.
        /// </summary>
        string Schedule { get; set; }

        /// <summary>
        /// Only jobs that are in Enabled state are run.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Whether the user triggered the job manually.
        /// </summary>
        bool ExecuteNow { get; set; }

        /// <summary>
        /// Whether this job is already running.
        /// </summary>
        bool IsAlreadyRunning { get; set; }

        /// <summary>
        /// Executes the job.
        /// </summary>
        /// <returns></returns>
        Task Execute();
    }
}
