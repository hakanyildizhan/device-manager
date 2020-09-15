// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Entity.Context.Entity;
using DeviceManager.WindowsService.Jobs;
using System.Collections.Generic;

namespace DeviceManager.WindowsService
{
    public interface IJobFactory
    {
        /// <summary>
        /// Returns a job of requested type.
        /// </summary>
        /// <param name="jobType"></param>
        /// <returns></returns>
        IJob Get(JobType jobType);

        /// <summary>
        /// Returns all registered jobs.
        /// </summary>
        /// <returns></returns>
        IList<IJob> GetAll();
    }
}
