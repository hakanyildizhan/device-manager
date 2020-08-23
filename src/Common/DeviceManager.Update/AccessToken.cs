// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;

namespace DeviceManager.Update
{
    /// <summary>
    /// Represents the bearer token obtained for accessing Office resources.
    /// </summary>
    public class AccessToken
    {
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string not_before { get; set; }

        /// <summary>
        /// Epoch time representing the date and time that this token will expire.
        /// </summary>
        public string expires_on { get; set; }
        public string resource { get; set; }

        /// <summary>
        /// The access token.
        /// </summary>
        public string access_token { get; set; }

        public bool IsValid => DateTime.UtcNow.CompareTo(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(int.Parse(expires_on))) == -1;
    }
}
