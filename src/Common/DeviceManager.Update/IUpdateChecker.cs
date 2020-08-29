// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.IO;
using System.Threading.Tasks;

namespace DeviceManager.Update
{
    /// <summary>
    /// Interface for checking available updates.
    /// </summary>
    public interface IUpdateChecker
    {
        EventHandler<int> DownloadProgressChanged { get; set; }
        Task<AccessToken> GetAccessToken();
        Task<Stream> GetManifest();
        Task<UpdateCheckResult> CheckForUpdate(ServerUpdateRequest request);
        Task<UpdateCheckResult> CheckForUpdate(ClientUpdateRequest request);
        Task<UpdateDownloadResult> DownloadUpdate(UpdateDownloadRequest request);
    }
}
