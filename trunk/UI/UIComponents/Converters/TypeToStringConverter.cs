﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Trinity.UIComponents
{
    public class ObjectToTypeStringConverter : IValueConverter
    {
        public object Convert(
         object value, Type targetType,
         object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(
         object value, Type targetType,
         object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
