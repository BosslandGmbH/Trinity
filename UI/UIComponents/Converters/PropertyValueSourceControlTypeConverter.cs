using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class PropertyValueSourceControlTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            var controlType = (UIControlType)parameter;

            var propVal = value as BindingMember;
            if (propVal == null)
                return false;

            if (propVal.UIControl == controlType && propVal.BoundSources.Any())
                return true;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }   
}

