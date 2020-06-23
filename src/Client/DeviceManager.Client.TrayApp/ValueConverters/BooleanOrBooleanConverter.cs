// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.Globalization;

namespace DeviceManager.Client.TrayApp
{
    public class BooleanOrBooleanConverter : BaseMultiValueConverter<BooleanOrBooleanConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is bool) || !(values[1] is bool))
            {
                return false;
            }

            return (bool)values[0] || (bool)values[1];
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
