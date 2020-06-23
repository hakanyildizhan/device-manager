// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using System;
using System.Globalization;
using System.Windows;

namespace DeviceManager.Client.TrayApp
{
	public class UserNameOrFriendlyNameConverter : BaseMultiValueConverter<UserNameOrFriendlyNameConverter>
	{
		public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Length != 2 || values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
			{
				return string.Empty;
			}

			return !string.IsNullOrEmpty((string)values[1]) ? (string)values[1] : (string)values[0];
		}

		public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
