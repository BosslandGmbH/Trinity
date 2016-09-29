using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Trinity.UI.UIComponents.MarkupExtensions
{
    public class WidthToColumnCountExtension : MarkupExtension, IValueConverter
    {
        private static WidthToColumnCountExtension _instance;

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var widthPerColumn = System.Convert.ToDouble(parameter);
            if (Math.Abs(widthPerColumn) < double.Epsilon)
                return 1;

            return Math.Round(System.Convert.ToDouble(value) / widthPerColumn, 0, MidpointRounding.AwayFromZero);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new WidthToColumnCountExtension());
        }
    }
    
}

