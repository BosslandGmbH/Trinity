using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Trinity.UI.UIComponents;
using Zeta.Bot;
using Trinity.Technicals;

namespace Trinity.UIComponents
{
    public class OffsetWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int paramNum;
            if (!int.TryParse(parameter.ToString(), out paramNum))
                return new GridLength(1, GridUnitType.Star);

            int inputNum;
            if (!int.TryParse(value.ToString(), out inputNum))
                return new GridLength(1, GridUnitType.Star);

            return new GridLength(inputNum - paramNum);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}

