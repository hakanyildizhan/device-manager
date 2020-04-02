using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace DeviceManager.Service
{
    public static class DateTimeHelpers
    {
        /// <summary>
        /// Parses a datetime string into <see cref="DateTime"/> according to the tr-TR <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="dateTimeString"></param>
        /// <returns></returns>
        public static DateTime Parse(string dateTimeString)
        {
            DateTime result;
            bool success = DateTime.TryParse(dateTimeString, CultureInfo.GetCultureInfo("tr-TR"), DateTimeStyles.AssumeUniversal, out result);
            return success ? result.ToUniversalTime() : DateTime.MinValue;
        }

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
