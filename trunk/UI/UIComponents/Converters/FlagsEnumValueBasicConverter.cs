using System;
using System.Globalization;
using System.Windows.Data;

namespace Trinity.UIComponents
{
    public class FlagsEnumValueConverterBasic : IValueConverter
    {
        private int targetValue;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int mask = (int)Enum.Parse(value.GetType(), (string)parameter);
            this.targetValue = (int)value;
            return ((mask & this.targetValue) != 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            this.targetValue ^= (int)Enum.Parse(targetType, (string)parameter);
            return Enum.Parse(targetType, this.targetValue.ToString());
        }
    }

}
