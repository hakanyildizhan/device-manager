// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.Globalization;
using System.Windows;

namespace DeviceManager.Client.TrayApp
{
	public class BooleanToVisibilityConverter : BaseValueConverter<BooleanToVisibilityConverter>
	{
		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null)
				return (bool)value ? Visibility.Hidden : Visibility.Visible;
			else
				return (bool)value ? Visibility.Visible : Visibility.Hidden;
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
