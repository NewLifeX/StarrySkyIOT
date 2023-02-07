using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace InterconnectionManagementAPP.Converters
{
    public class BoolToStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? boolValue = (bool?)value;
            if (boolValue == null)
            {
                return Brush.Red;
            }
            return (bool)boolValue ? Brush.Green : Brush.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
