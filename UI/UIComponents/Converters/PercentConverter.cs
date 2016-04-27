using System;
using System.Windows.Data;

namespace Trinity.UIComponents
{
    public class PercentConverter : IValueConverter
    {
        #region IValueConverter Members
        /// <summary>Converts a value.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is float)
            {
                return (float)Math.Round((float)value * 100,0);
            }
            else if (value is double)
            {
                return (double)Math.Round((double)value * 100,0);
            }
            else
                return 0f;
        }

        /// <summary>Converts a value.</summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is float)
            {
                return (float)Math.Round((float)value / 100,2);
            }
            else if (value is double)
            {
                return Math.Round((double)value / 100,2);
            }
            else
                return 0f;
        }
        #endregion
    }
}
