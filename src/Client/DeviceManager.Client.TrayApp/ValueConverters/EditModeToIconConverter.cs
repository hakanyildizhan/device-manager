using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DeviceManager.Client.TrayApp
{
    public class EditModeToIconConverter : BaseValueConverter<EditModeToIconConverter>
    {
        const string editIconUri = "pack://application:,,,/Resources/Images/dots.png";
        const string applyIconUri = "pack://application:,,,/Resources/Images/tick.png";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return (bool)value ? editIconUri : applyIconUri;
            else
                return (bool)value ? applyIconUri : editIconUri;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
