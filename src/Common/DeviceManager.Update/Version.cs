// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System.Text.RegularExpressions;

namespace DeviceManager.Update
{
    /// <summary>
    /// Class that represents application versions.
    /// </summary>
    public class Version
    {
        public int Major { get; set; } = 1;
        public int Minor { get; set; } = 0;
        public int Patch { get; set; } = 0;

        public Version(string versionString)
        {
            if (string.IsNullOrEmpty(versionString))
            {
                return;
            }

            var regex = Regex.Match(versionString, @"(\d+)[.]?(\d+)?[.]?(\d+)?");

            if (!regex.Success)
            {
                return;
            }

            Major = int.Parse(regex.Groups[1].Value);
            Minor = regex.Groups[2].Success ? int.Parse(regex.Groups[2].Value) : 0;
            Patch = regex.Groups[3].Success ? int.Parse(regex.Groups[3].Value) : 0;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }
    }

    public enum VersionCompareResult
    {
        Equal,
        Newer,
        Older,
        Unknown
    }

    public static class VersionHelper
    {
        /// <summary>
        /// Compares this version to another version.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="otherVersion"></param>
        /// <returns></returns>
        public static VersionCompareResult Compare(this Version version, Version otherVersion)
        {
            if (version == null || otherVersion == null)
            {
                return VersionCompareResult.Unknown;
            }
            else if (version.Major < otherVersion.Major)
            {
                return VersionCompareResult.Older;
            }
            else if (version.Major > otherVersion.Major)
            {
                return VersionCompareResult.Newer;
            }
            else if (version.Major == otherVersion.Major && version.Minor < version.Major)
            {
                return VersionCompareResult.Older;
            }
            else if (version.Major == otherVersion.Major && version.Minor > version.Major)
            {
                return VersionCompareResult.Newer;
            }
            else if (version.Major == otherVersion.Major && version.Minor == version.Major && version.Patch == otherVersion.Patch)
            {
                return VersionCompareResult.Equal;
            }
            else if (version.Major == otherVersion.Major && version.Minor == version.Major && version.Patch < otherVersion.Patch)
            {
                return VersionCompareResult.Older;
            }
            else if (version.Major == otherVersion.Major && version.Minor == version.Major && version.Patch > otherVersion.Patch)
            {
                return VersionCompareResult.Newer;
            }
            else
            {
                return VersionCompareResult.Unknown;
            }
        }
    }
}
