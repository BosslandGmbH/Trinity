using System;
using Trinity.Framework.Helpers;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    /// <summary>
    /// Converts a collection of items into useful objects that UI controls can bind to.
    /// e.g. ItemsSource="{Binding Converter={StaticResource PropertyValueToCollectionConverter}}"
    /// </summary>
    public class PropertyValueToCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pv = value as BindingMember;
            if (pv == null)
                return null;

            value = pv.Value;
            if (value == null)
                return null;

            var type = value.GetType();

            if (type.IsEnum)
            {
                var enumValues = Enum.GetValues(type).Cast<object>().ToList();

                var isFlags = type.GetCustomAttribute(typeof(FlagsAttribute), false) != null;
                if (!isFlags)
                    return enumValues;

                return enumValues.Where(ev =>
                {
                    var num = (long)System.Convert.ChangeType(ev, typeof(long));

                    // If this flag is not 0(default/None) or in the exclusion list
                    return num != 0 && (pv.ExcludeMask & num) != num;

                }).Select(ev => new PropertyValueFlagBindingItem
                {
                    Name = ev.ToString(),
                    Description = GetDescription(type, ev),
                    Type = type,
                    Source = pv,
                    Flag = ev,
                });

            }

            if (value is IEnumerable)
            {
                return value;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        private static string GetDescription(Type type, object enumValue)
        {
            var memInfo = type.GetMember(enumValue.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute),false);
            return (attributes.FirstOrDefault() as DescriptionAttribute)?.Description;
        }
    }
}