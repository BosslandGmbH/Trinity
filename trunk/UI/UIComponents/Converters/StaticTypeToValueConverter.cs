using System;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class StaticTypeToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter) ? TrueValue : FalseValue;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(TrueValue) ? parameter : Binding.DoNothing;
        }
    }

    public class StaticTypeToBoolConverter : StaticTypeToValueConverter<bool> { }
}