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
