using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class FlagsToGridLengthConverter : IValueConverter
    {
        public GridLength TrueValue { get; set; }
        public GridLength FalseValue { get; set; }
        private int targetValue;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Log.Info("Converting {0} Type={1}", value, targetType);
            int mask = (int)Enum.Parse(value.GetType(), (string)parameter);
            this.targetValue = (int)value;
            var boolResult = ((mask & this.targetValue) != 0);
            if (boolResult)
                return TrueValue;
            
            return FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //this.targetValue ^= (int)Enum.Parse(targetType, (string)parameter);
            //return Enum.Parse(targetType, this.targetValue.ToString());
            return Binding.DoNothing;
        }

    }
}

