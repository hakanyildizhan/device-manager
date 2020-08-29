// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Update
{
    public class UpdateDownloadRequest
    {
        public UpdatePackage Package { get; set; }
        public string TargetDirectory { get; set; }

        /// <summary>
        /// Unique ID for this request. This ID will be used to identify and update resources associated with this request, i.e. download progress bar.
        /// </summary>
        public string RequestId { get; set; }
    }
}
