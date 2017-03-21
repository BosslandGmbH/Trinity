using System;
using Trinity.Framework.Helpers;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class BoolMultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var valuesList = values.ToList();
            if (!valuesList.Any())
                return false;

            return values.All(v => v is bool && (bool)v);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
