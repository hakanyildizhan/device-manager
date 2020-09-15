// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Update;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceManager.Client.Service.Command
{
    /// <summary>
    /// Command to download and install an available update.
    /// </summary>
    public class InstallUpdateCommand : IAppCommand
    {
        private readonly IUpdateManager _updater;
        public string Name { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public InstallUpdateCommand(IUpdateManager updater)
        {
            _updater = updater;
        }

        public async Task<bool> Execute()
        {
            await _updater.DownloadAndInstall();
            return true;
        }
    }
}
