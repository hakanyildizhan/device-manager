// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Service.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.Service
{
    public interface IUpdateService
    {
        /// <summary>
        /// Schedules an update job previously registered by the updater service.
        /// </summary>
        /// <returns></returns>
        Task<bool> ScheduleUpdate();

        /// <summary>
        /// Checks if an update job is registered by the updater service. If so, returns information about an available update. In case no update is available, <see cref="UpdateInfo.Info"/> property will contain some error information.
        /// </summary>
        /// <returns></returns>
        UpdateInfo GetAvailableUpdateInformation();

        /// <summary>
        /// Checks if the update Windows service is currently running.
        /// </summary>
        /// <returns></returns>
        bool IsServiceRunning();

        /// <summary>
        /// Gets a list of jobs currently defined in the database.
        /// </summary>
        /// <returns></returns>
        IList<ScheduledTask> GetTaskList();

        /// <summary>
        /// Updates the state of available jobs.
        /// </summary>
        /// <param name="tasks"></param>
        /// <returns></returns>
        Task<bool> UpdateTaskList(IList<ScheduledTask> tasks);
    }
}
