using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using Trinity.Framework.Helpers;

namespace Trinity.UIComponents
{
    public class IsEnumConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           // Log.Info("Fired IsEnumConverter");

            if (value == null)
                return null;

            var type = value.GetType();

            return type.IsEnum;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}

