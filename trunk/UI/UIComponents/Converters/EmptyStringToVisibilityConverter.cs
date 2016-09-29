﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class EmptyStringToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
                              CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string)
                       ? Visibility.Collapsed
                       : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}
