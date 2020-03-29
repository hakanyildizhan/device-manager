using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
