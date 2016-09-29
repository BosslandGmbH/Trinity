using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class ValueToCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var type = value.GetType();

            if (type.IsEnum)
            {
                // Note: ComboBox will not select the SelectedItem from its binding unless 
                // it's the exact same object reference in memory as it's ItemSource.

                var enumValues = Enum.GetValues(type);
                var newEnumValues = new List<object>();

                foreach (var enumValue in enumValues)
                {
                    var description = GetDescriptionAttribute(type, enumValue);
                    if (description != null)
                    {
                        newEnumValues.Add(description);
                    }
                    else
                    {
                        //var actualValue = System.Convert.ChangeType(enumValue, Enum.GetUnderlyingType(type));
                        newEnumValues.Add(enumValue);
                    }
                }

                if(newEnumValues.Any())
                    return newEnumValues;

                return enumValues;
            }

            if (value is IEnumerable)
            {
                return value;
            }

            return null;
        }

        private static string GetDescriptionAttribute(Type type, object enumValue)
        {
            var memInfo = type.GetMember(enumValue.ToString());
            if (!memInfo.Any())
                return null;

            var attributes = memInfo[0].GetCustomAttributes(typeof (DescriptionAttribute), false);
            if (!attributes.Any())
                return null;

            return ((DescriptionAttribute)attributes[0]).Description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ConvertBack doesn't get called for Collections as ItemSource
            return Binding.DoNothing;
        }
    }
}

