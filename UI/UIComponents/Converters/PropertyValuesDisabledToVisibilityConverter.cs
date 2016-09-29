using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    /// <summary>
    /// Used to check against children in a GroupStyle
    /// http://stackoverflow.com/questions/2212014/display-sum-of-grouped-items-in-listview
    /// </summary>
    public class PropertyValuesDisabledToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value)
                return "null";

            var items = (ReadOnlyObservableCollection<object>)value;

            if (items.Count == items.Cast<BindingMember>().Count(pv => !pv.IsEnabled))
            {
                //Log.Info(LogCategory.Configuration, "All not enabled, returning collapsed");
                return Visibility.Collapsed;                
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
