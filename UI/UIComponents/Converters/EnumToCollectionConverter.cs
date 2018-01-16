using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Trinity.UI.UIComponents.Converters
{
    public static class EnumHelper
    {
        public static string Description(this Enum eValue)
        {
            var nAttributes = eValue.GetType().GetField(eValue.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (nAttributes.Any())
                return (nAttributes.First() as DescriptionAttribute).Description;

            // If no description is found, the least we can do is replace underscores with spaces
            TextInfo oTI = CultureInfo.CurrentCulture.TextInfo;
            return oTI.ToTitleCase(oTI.ToLower(eValue.ToString().Replace("_", " ")));
        }

        public static IEnumerable<ValueDescription> GetAllValuesAndDescriptions<T>() where T : struct, IConvertible, IComparable, IFormattable
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum type");

            return Enum.GetValues(typeof(T)).Cast<Enum>().Select((e) => new ValueDescription() { Value = e, Description = e.Description() }).ToList();
        }
    }

    public class ValueDescription
    {
        public Enum Value { get; set; }
        public string Description { get; set; }
        public override string ToString() => Description;
    }

    [ValueConversion(typeof(Enum), typeof(IEnumerable<KeyValuePair<Enum, string>>))]
    public class EnumToCollectionConverter : MarkupExtension, IValueConverter
    {
        public static IEnumerable<ValueDescription> GetAllValuesAndDescriptions(Type t, object parameter)
        {
            if (!t.IsEnum)
                throw new ArgumentException("t must be an enum type");

            var values = Enum.GetValues(t).Cast<Enum>().Select(e => new ValueDescription
            {
                Value = e,
                Description = e.Description()

            });

            var paramString = parameter as string;
            if (!string.IsNullOrEmpty(paramString))
            {
                var validOptions = parameter.ToString();
                return values.Where(e => validOptions.Contains(e.Value.ToString())).ToList();              
            }

            return values.ToList();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetAllValuesAndDescriptions(value.GetType(), parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    [ValueConversion(typeof(Enum), typeof(KeyValuePair<Enum, string>))]
    public class EnumToCollectionItemConverter : MarkupExtension, IValueConverter
    {
        public static ValueDescription GetValueAndDescription(Type t, object parameter)
        {
            if (!t.IsEnum)
                throw new ArgumentException($"type {t.Name} must be an enum type");

            if (!Enum.IsDefined(t, parameter.ToString()))
                throw new ArgumentException($"parameter {parameter} is not defined in enum of type {t.Name}");

            var e = (Enum)Enum.Parse(t, parameter.ToString());

            return new ValueDescription
            {
                Value = e,
                Description = e.Description()
            };
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ValueDescription)
                return value;

            return GetValueAndDescription(value.GetType(), parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

}

