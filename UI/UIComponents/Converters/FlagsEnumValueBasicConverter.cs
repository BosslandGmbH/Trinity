using System;
using System.Globalization;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class FlagsEnumValueConverterBasic : IValueConverter
    {
        private int _targetValue;
        public bool Reverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var resultValue = Reverse ? 1 : 0;
            int mask = (int)Enum.Parse(value.GetType(), (string)parameter);
            this._targetValue = (int)value;
            return ((mask & this._targetValue) != resultValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            this._targetValue ^= (int)Enum.Parse(targetType, (string)parameter);
            return Enum.Parse(targetType, this._targetValue.ToString());
        }
    }

}
