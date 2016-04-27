using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace Trinity.UIComponents
{
    public class IntToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public string Format { get; set; }

        /// <summary>Converts a value.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = Format == null ? "{0:0.0}" : "{" + Format + "}";
            return string.Format(format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TConverter.ChangeType(value, targetType);
        }

        public static class TConverter
        {
            public static T ChangeType<T>(object value)
            {
                return (T)ChangeType(typeof(T), value);
            }
            public static object ChangeType(object value, Type type)
            {
                return ChangeType(type, value);
            }
            public static object ChangeType(Type t, object value)
            {
                TypeConverter tc = TypeDescriptor.GetConverter(t);
                return tc.ConvertFrom(value);
            }
            public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
            {

                TypeDescriptor.AddAttributes(typeof(T), new TypeConverterAttribute(typeof(TC)));
            }
        }

        #endregion
    }
}
