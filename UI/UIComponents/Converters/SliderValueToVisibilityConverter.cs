using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Trinity.UIComponents
{
    public class SliderValueToVisibilityConverter : IValueConverter
    {
        public enum SliderEnd
        {
            LessThanValue,
            GreaterThanValue
        }

        public SliderEnd Direction { get; set; }        

        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            int paramLimit;
            if (!int.TryParse(parameter.ToString(), out paramLimit))
                return Visibility.Collapsed;

            int result;
            if (!int.TryParse(value.ToString(), out result))
                return Visibility.Collapsed;

            if (Direction == SliderEnd.LessThanValue)
            {
                return result < paramLimit ? Visibility.Visible : Visibility.Collapsed;
            }

            return result > paramLimit ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
