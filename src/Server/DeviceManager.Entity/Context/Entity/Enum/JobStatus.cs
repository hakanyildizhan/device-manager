// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;

namespace DeviceManager.Entity.Context.Entity
{
    [Flags]
    public enum JobStatus : int
    {
        /// <summary>
        /// Job is awaiting user confirmation to execute.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Job is triggered by the user to be executed immediately.
        /// </summary>
        UserTriggered = 2,

        /// <summary>
        /// Job is being executed.
        /// </summary>
        Started = 4,

        /// <summary>
        /// Job is completed.
        /// </summary>
        Completed = 8,

        /// <summary>
        /// Job is completed and is successful.
        /// </summary>
        Succeeded = 16,

        /// <summary>
        /// Job is completed and it failed.
        /// </summary>
        Failed = 32
    }
}
