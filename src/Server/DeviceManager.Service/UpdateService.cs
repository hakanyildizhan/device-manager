// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Entity.Context;
using Unity;

namespace DeviceManager.Service
{
    public class UpdateService : IUpdateService
    {
        [Dependency]
        public DeviceManagerContext DbContext { get; set; }

        public bool DownloadUpdate()
        {
            throw new System.NotImplementedException();
        }

        public bool InstallUpdate()
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateIsAvailable()
        {
            throw new System.NotImplementedException();
        }
    }
}
