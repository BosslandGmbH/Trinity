using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    /// <summary>
    /// Helper for debugging DataBinding
    /// http://spin.atomicobject.com/2013/12/11/wpf-data-binding-debug/
    /// </summary>
    public class DebugDataBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}
