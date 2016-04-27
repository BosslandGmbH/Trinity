using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Trinity.UIComponents
{
    public class EnumBooleanToGridLengthConverter : IValueConverter
    {
        public GridLength TrueValue { get; set; }
        public GridLength FalseValue { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }
   
    }
}

