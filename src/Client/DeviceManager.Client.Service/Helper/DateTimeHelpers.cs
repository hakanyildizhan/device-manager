using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
