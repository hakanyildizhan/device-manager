// This file is part of Device Manager project released under GNU General Public License v3.0.
// See file LICENSE.md or go to https://www.gnu.org/licenses/gpl-3.0.html for full license details.
// Copyright © Hakan Yildizhan 2020.

using DeviceManager.Client.Service;
using System;
using System.Globalization;
using System.Windows.Input;

namespace DeviceManager.Client.TrayApp
{
	/// <summary>
	/// This converter binds appropriate commands to prompt buttons. 
	/// Parameters should be passed in this order: 
	/// 1. <see cref="ExecuteOnAction"/> flag. 
	/// 2. An <see cref="ICommand"/> that executes given prompt query.
	/// 3. An <see cref="ICommand"/> that dismisses the prompt.
	/// </summary>
	public class PromptCommandBindingConverter : BaseMultiValueConverter<PromptCommandBindingConverter>
	{
		public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
            if (values.Length != 3 || !(values[0] is ExecuteOnAction) || !(values[1] is ICommand) || !(values[2] is ICommand))
            {
				return null;
            }

            if ((ExecuteOnAction)values[0] == ExecuteOnAction.Yes)
            {
				return parameter != null ? (ICommand)values[1] : (ICommand)values[2];
			}
            else
            {
				return parameter != null ? (ICommand)values[2] : (ICommand)values[1];
			}
		}

		public override object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
