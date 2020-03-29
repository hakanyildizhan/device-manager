using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace DeviceManager.Client.TrayApp
{
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter // to access it in curly bracket like StaticResource
        where T : class, new()
    {
        private static T Converter = null; // xaml will be calling this

        public override object ProvideValue(IServiceProvider serviceProvider) // get the value in xaml.
        {
            return Converter ?? (Converter = new T());
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}
