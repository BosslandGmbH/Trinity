using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class BoolToMarginConverter : IValueConverter
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public int FallbackLeft { get; set; }
        public int FallbackTop { get; set; }
        public int FallbackRight { get; set; }
        public int FallbackBottom { get; set; }

        public bool Invert { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fallback = new Thickness(FallbackLeft, FallbackTop, FallbackRight, FallbackBottom);

            if (!(value is bool))
                return fallback;

            var result = Invert ? !(bool) value : (bool) value;

            if(!result)
                return fallback;

            return new Thickness(Left, Top, Right, Bottom);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
