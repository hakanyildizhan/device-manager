// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DeviceManager.Update
{
    public class XmlManifestParser : IManifestParser
    {
        public UpdatePackage GetUpdateInfoForClient(Stream fileStream, Version serverVersion)
        {
            XElement manifest = XElement.Load(fileStream);

            if (manifest == null)
            {
                return null;
            }

            XElement existingServer = manifest.Descendants("ServerList").Elements("Server").FirstOrDefault(s => new Version(s.Attribute("version").Value).Compare(serverVersion) == VersionCompareResult.Equal);
            XElement latestClientForExistingServer = existingServer?.Descendants("CompatibleClients").Elements("CompatibleClient").FirstOrDefault(c => c.Attribute("latest") != null && c.Attribute("latest").Value == "true");
            string version = latestClientForExistingServer?.Attribute("version")?.Value;

            if (string.IsNullOrEmpty(version))
            {
                return null;
            }

            XElement clientNode = manifest.Descendants("ClientList").Elements("Client").FirstOrDefault(c => c.Attribute("version") != null && new Version(c.Attribute("version").Value).Compare(new Version(version)) == VersionCompareResult.Equal);
            string installer = clientNode?.Element("Installer")?.Value;

            if (string.IsNullOrEmpty(installer))
            {
                return null;
            }

            return new UpdatePackage() { Url = installer, Version = new Version(version) };
        }

        public UpdatePackage GetUpdateInfoForServer(Stream fileStream)
        {
            XElement manifest = XElement.Load(fileStream);

            if (manifest == null)
            {
                return null;
            }

            XElement latestServer = manifest.Descendants("ServerList").Elements("Server").FirstOrDefault(s => s.Attribute("latest") != null && s.Attribute("latest").Value == "true");

            string installer = latestServer?.Element("Installer")?.Value;
            string version = latestServer?.Attribute("version")?.Value;

            if (string.IsNullOrEmpty(installer) || string.IsNullOrEmpty(version))
            {
                return null;
            }

            return new UpdatePackage() { Url = installer, Version = new Version(version) };
        }
    }
}
