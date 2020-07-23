// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.Globalization;

namespace DeviceManager.Client.TrayApp
{
    /// <summary>
    /// This converter is used to decide whether sub-menu items that represent device items should be in "Enabled" state.
    /// <para></para>
    /// This converter takes 3 parameters in this exact order: 
    /// 1) "IsOffline" - whether the application is currently offline; 
    /// 2) "IsAvailable" - whether this device item is currently available; and
    /// 3) "UsedByMe" - whether this device item is already checked out by the current user.
    /// </summary>
    public class DeviceItemIsEnabledConverter : BaseMultiValueConverter<DeviceItemIsEnabledConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3 || !(values[0] is bool) || !(values[1] is bool) || !(values[2] is bool))
            {
                return false;
            }

            return !(bool)values[0] && ((bool)values[1] || (bool)values[2]);
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
