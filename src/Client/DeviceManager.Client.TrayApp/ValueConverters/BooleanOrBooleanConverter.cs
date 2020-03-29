using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
