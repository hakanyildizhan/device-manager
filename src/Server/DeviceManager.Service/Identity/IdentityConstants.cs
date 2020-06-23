// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

namespace DeviceManager.Service.Identity
{
    /// <summary>
    /// Constants for identity stuff.
    /// </summary>
    public static class IdentityConstants
    {
        /// <summary>
        /// This is set in <see cref="RegisterModel"/> and <see cref="ApplicationUserManager"/>.
        /// </summary>
        public const int MIN_PASSWORD_LENGTH = 5;

        /// <summary>
        /// This is set in <see cref="RegisterModel"/>.
        /// </summary>
        public const int MIN_USERNAME_LENGTH = 3;
    }
}
