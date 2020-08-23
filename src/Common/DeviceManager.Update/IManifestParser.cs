// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.IO;

namespace DeviceManager.Update
{
    /// <summary>
    /// Interface to be used for reading the installer manifest.
    /// </summary>
    public interface IManifestParser
    {
        UpdatePackage GetUpdateInfoForServer(Stream fileStream);
        UpdatePackage GetUpdateInfoForClient(Stream fileStream, Version serverVersion);
    }
}
