// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Service.Model
{
    /// <summary>
    /// To be used for listing, enabling/disabling running jobs via server UI.
    /// </summary>
    public class ScheduledTask
    {
        public int Id { get; set; }
        public bool IsEnabled { get; set; }
        public string Title { get; set; }
        public string Schedule { get; set; }
    }
}
