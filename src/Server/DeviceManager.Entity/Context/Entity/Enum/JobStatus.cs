// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;

namespace DeviceManager.Entity.Context.Entity
{
    [Flags]
    public enum JobStatus
    {
        Registered = 0,
        Scheduled = 1,
        Started = 2,
        Completed = 4,
        Succeeded = 8,
        Failed = 16
    }
}
