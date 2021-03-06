// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Service.Model
{
    /// <summary>
    /// Used for importing hardware data into the database.
    /// </summary>
    public class DeviceImport
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public string Info { get; set; }
        public string PrimaryAddress { get; set; }
        public string SecondaryAddress { get; set; }
        public string ConnectedModuleInfo { get; set; }
    }
}
