using System;
using Trinity.Framework.Helpers;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class DisplayNameAttributeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            try
            {
                var type = value.GetType();

                // Parameter is a proeprty to use for display name
                if (parameter is string propertyName)
                {
                    var prop = type.GetProperty(propertyName);
                    var propName = prop.PropertyType.GetAttributeValue((DisplayNameAttribute dna) => dna.DisplayName);
                    if (!string.IsNullOrEmpty(propName))
                        return propName;

                    return ToFriendlyName(propertyName);
                }

                // Look at class name
                var name = type.GetAttributeValue((DisplayNameAttribute dna) => dna.DisplayName);
                if (!string.IsNullOrEmpty(name))
                    return name;

                return ToFriendlyName(type.Name);
            }
            catch (Exception)
            {
                //Log.Info("Exception {0}", ex);
            }
            return value;
        }

        public static string ToFriendlyName(string input)
        {
            StringBuilder builder = new StringBuilder();
            var theString = input;
            foreach (char c in theString)
            {
                if (Char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }
            return builder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

    }
}

