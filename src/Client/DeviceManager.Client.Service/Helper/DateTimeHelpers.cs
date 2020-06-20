// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.Globalization;

namespace DeviceManager.Client.Service
{
    public static class DateTimeHelpers
    {
        /// <summary>
        /// Stringifies a <see cref="DateTime"/> value using Turkish <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToStringTurkish(this DateTime value)
        {
            return value.ToString(CultureInfo.GetCultureInfo("tr-TR"));
        }
    }
}
