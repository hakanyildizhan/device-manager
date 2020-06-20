// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.Globalization;

namespace DeviceManager.Client.TrayApp
{
    public class AvailabilityToIconConverter : BaseValueConverter<AvailabilityToIconConverter>
    {
        const string unlockedIconUri = "pack://application:,,,/Resources/Images/unlocked.png";
        const string lockedIconUri = "pack://application:,,,/Resources/Images/locked.png";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? unlockedIconUri : lockedIconUri;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
