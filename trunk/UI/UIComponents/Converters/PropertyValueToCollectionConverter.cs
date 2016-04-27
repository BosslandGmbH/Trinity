using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using Trinity.UI.UIComponents;

namespace Trinity.UIComponents
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

                var isFlags = type.GetCustomAttribute(typeof (FlagsAttribute), false) != null;
                if (!isFlags)
                    return enumValues;

                return enumValues.Where(ev => (int)ev != 0).Select(ev => new PropertyValueFlagBindingItem
                {
                    Name = ev.ToString(), 
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
    }
}



//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Globalization;
//using System.Linq;
//using System.Reflection;
//using System.Windows.Data;
//using Trinity.Framework.Attributes;
//using Zeta.Common;
//using Trinity.Framework.Helpers;
//

//namespace Trinity.UI.UIComponents
//{
//    /// <summary>
//    /// Converts a collection of items into useful objects that UI controls can bind to.
//    /// e.g. ItemsSource="{Binding Converter={StaticResource PropertyValueToCollectionConverter}}"
//    /// </summary>
//    public class PropertyValueToCollectionConverter : IValueConverter
//    {
//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            var pv = value as BindingMember;
//            if (pv == null)
//                return null;

//            value = pv.Value;
//            if (value == null)
//                return null;

//            var type = value.GetType();

//            if (type.IsEnum)
//            {
//                var enumValues = Enum.GetValues(type).Cast<object>().ToList();
//                var enumNames = enumValues.Select(e => e.ToString());

//                if (pv.IsUseDescription)
//                {
//                    var newEnumValues = new List<object>();
//                    foreach (var enumValue in enumValues)
//                    {
//                        var memInfo = type.GetMember(enumValue.ToString());
//                        var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
//                        var description = ((DescriptionAttribute)attributes[0]).Description;
//                        newEnumValues.Add(description);
//                    }
//                    enumValues = newEnumValues;
//                }

//                var isFlags = type.GetCustomAttribute(typeof(FlagsAttribute), false) != null;
//                if (!isFlags)
//                    return enumValues;

//                return enumValues.Where(ev => (int)ev != 0).Select(ev => new PropertyValueFlagBindingItem
//                {
//                    Name = ev.ToString(),
//                    Type = type,
//                    Source = pv,
//                    Flag = ev,
//                });

//            }

//            if (value is IEnumerable)
//            {
//                return value;
//            }

//            return null;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            return Binding.DoNothing;
//        }
//    }
//}

