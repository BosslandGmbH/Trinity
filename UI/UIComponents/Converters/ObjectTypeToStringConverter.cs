using System;
using System.Globalization;
using System.Windows.Data;
using Trinity.Framework.Helpers;

namespace Trinity.UIComponents
{
    public class TypeToStringConverter : IValueConverter
    {
        public object Convert(
         object value, Type targetType,
         object parameter, CultureInfo culture)
        {
            var typeName = value.GetType().Name;
            //Log.Verbose("{0}", typeName);
            return typeName;
        }

        public object ConvertBack(
         object value, Type targetType,
         object parameter, CultureInfo culture)
        {
            throw new Exception("Can't convert back");
        }
    }
}
