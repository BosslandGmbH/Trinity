using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Trinity.UI.UIComponents.Converters
{
    public class BoolToValueMultiConverter<T> : IMultiValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Any are false, return false;
            if (values.Any(v => ReferenceEquals(v, DependencyProperty.UnsetValue)))
                return FalseValue;

            // All are true, return true.
            return values.All(System.Convert.ToBoolean) ? TrueValue : FalseValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // return value != null && value.Equals(TrueValue);
            throw new NotSupportedException();
        }
    }

    public class BoolToStringMultiConverter : BoolToValueMultiConverter<String> { }
    public class BoolToBrushMultiConverter : BoolToValueMultiConverter<Brush> { }
    public class BoolToVisibilityMultiConverter : BoolToValueMultiConverter<Visibility> { }
    public class BoolToObjectMultiConverter : BoolToValueMultiConverter<Object> { }
    public class BoolToIntMultiConverter : BoolToValueMultiConverter<int> { }


}


