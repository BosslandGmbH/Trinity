using System;
using System.Windows;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class EnumToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var param = parameter as string;
            if (param == null)
                return Binding.DoNothing;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return Binding.DoNothing;

            var parameterValue = Enum.Parse(value.GetType(), param);
            return parameterValue.Equals(value) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }  
    }

    public class EnumToVisibilityConverter : EnumToValueConverter<Visibility> { }


}
