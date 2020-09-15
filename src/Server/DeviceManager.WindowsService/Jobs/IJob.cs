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
        JobType Type { get; }
        string Schedule { get; set; }
        bool IsEnabled { get; set; }
        Task Execute();
    }
}
