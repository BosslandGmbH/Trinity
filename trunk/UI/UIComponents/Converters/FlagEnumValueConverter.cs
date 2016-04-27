using System;
using System.Globalization;
using System.Windows.Data;
using Trinity.Technicals;

namespace Trinity.UIComponents
{
    public class FlagsEnumValueConverter : IValueConverter
    {
        private int targetValueInt;
        private ulong targetValueUlong;

        public FlagsEnumValueConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var type = value.GetType();
                if (type.IsEnum)
                {
                    var underlyingType = Enum.GetUnderlyingType(type);

                    if (underlyingType == typeof(int))
                    {
                        int mask = (int)Enum.Parse(type, (string)parameter);
                        this.targetValueInt = (int)value;
                        return ((mask & this.targetValueInt) != 0);
                    }
                    if (underlyingType == typeof (ulong))
                    {
                        ulong mask = (ulong)Enum.Parse(type, (string)parameter);
                        this.targetValueUlong = (ulong)value;
                        return ((mask & this.targetValueUlong) != 0);
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                Logger.Log("FlagsEnumValueConverter: Invalid Cast(to) Value={0} Type={1} Param={2} Exception{3}", value, targetType, parameter, ex);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (targetType.IsEnum)
                {
                    var underlyingType = Enum.GetUnderlyingType(targetType);

                    if (underlyingType == typeof (int))
                    {
                        this.targetValueInt ^= (int)Enum.Parse(targetType, (string)parameter);
                        return Enum.Parse(targetType, this.targetValueInt.ToString());
                    }

                    if (underlyingType == typeof(ulong))
                    {
                        this.targetValueUlong ^= (ulong)Enum.Parse(targetType, (string)parameter);
                        return Enum.Parse(targetType, this.targetValueUlong.ToString());
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                Logger.Log("FlagsEnumValueConverter: Invalid Cast(from) Value={0} Type={1} Param={2} Exception{3}", value, targetType, parameter, ex);
            }
            return value;
        }

    }

}
