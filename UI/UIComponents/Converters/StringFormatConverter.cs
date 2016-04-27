using System;
using System.Globalization;
using System.Windows.Data;

namespace Trinity.UIComponents
{
    [ValueConversion(typeof(object), typeof(string))]    
    public class StringFormatConverter : IValueConverter    
    {        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)        
        {            
            string format = parameter as string;
            if (!string.IsNullOrEmpty(format))
            {
                return string.Format(culture, format, value);
            }
            else
            {
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
