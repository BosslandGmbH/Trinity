using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Trinity.Technicals;

namespace Trinity.UIComponents
{
    public class ValueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return string.Empty;

                if (IsNumericType(value))
                {
                    var number = double.Parse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
                    return $"{number:0.####}";
                }
                return value.ToString();
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception in ValueToStringConverter {ex}");
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public static bool IsNumericType(object o)
        {
            if (o == null) return false;
            var type = o.GetType();
            if (type.IsEnum) return false;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

    }
}
