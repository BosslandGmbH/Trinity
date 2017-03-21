using System;
using Trinity.Framework;
using System.Globalization;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class FlagsEnumValueConverter : IValueConverter
    {
        private ulong _targetValueUlong;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var type = value?.GetType();
                if (type != null && type.IsEnum)
                {
                    var mask = GetEnumValue(type, parameter);
                    _targetValueUlong = (ulong)System.Convert.ChangeType(value, typeof(ulong));
                    return ((mask & _targetValueUlong) != 0);
                }
                return value;
            }
            catch (Exception ex)
            {
                Core.Logger.Log("FlagsEnumValueConverter: Invalid Cast(to) Value={0} Type={1} Param={2} Exception{3}", value, targetType, parameter, ex);
            }
            return value;
        }

        public ulong GetEnumValue(Type targetType, object parameter)
        {
            return (ulong)System.Convert.ChangeType(Enum.Parse(targetType, parameter.ToString()), typeof(ulong));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (targetType.IsEnum && parameter != null)
                {
                    _targetValueUlong ^= GetEnumValue(targetType, (string)parameter);
                    return Enum.Parse(targetType, _targetValueUlong.ToString());
                }

                return value;
            }
            catch (Exception ex)
            {
                Core.Logger.Log("FlagsEnumValueConverter: Invalid Cast(from) Value={0} Type={1} Param={2} Exception{3}", value, targetType, parameter, ex);
            }
            return value;
        }

    }

}
