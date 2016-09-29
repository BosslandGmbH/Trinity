using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Trinity.UI.UIComponents.Converters
{
    public class FlagsToBoolConverter : MarkupExtension, IValueConverter
    {
        private FlagsToBoolConverter _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            _converter = new FlagsToBoolConverter();
            return _converter;
        }

        private ulong _sourceValue;

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {   
            var type = value.GetType();
            if (type.IsEnum)
            {
                ulong mask = (ulong)System.Convert.ChangeType(Enum.Parse(type, (string)parameter), typeof(ulong));
                _sourceValue = (ulong)System.Convert.ChangeType(Enum.Parse(type, value.ToString()), typeof(ulong));
                return ((mask & _sourceValue) != 0);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {    
            if (targetType.IsEnum)
            {
                var original = this._sourceValue;
                var originalEnum = Enum.Parse(targetType, original.ToString());
                var maskEnum = Enum.Parse(targetType, (string)parameter);
                var mask = (ulong)System.Convert.ChangeType(maskEnum, typeof(ulong));
                _sourceValue ^= mask;
                var sourceEnum = Enum.Parse(targetType, _sourceValue.ToString());
                return sourceEnum;
            }
            return value;
        }
    }
}
