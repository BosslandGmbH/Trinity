using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using Trinity.Framework.Helpers;

namespace Trinity.UI.UIComponents.Converters
{
    [ValueConversion(typeof(object), typeof(string))]    
    public class StringFormatConverter : IValueConverter    
    {        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)        
        {
            try
            {
                string format = parameter as string;
                if (!string.IsNullOrEmpty(format))
                {
                    if (_validForamts.Contains(format))
                        return string.Format(culture, format, value);

                    Logger.Log($"Invalid string Format Param={parameter} Value={value} ValueType={value.GetType()} TargetType={targetType}");
                    return value.ToString();
                }
                return value.ToString();
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception in StringFormatConverter Param={parameter} Value={value} ValueType={value.GetType()} TargetType={targetType} {ex}");
            }
            return value;
        }

        readonly HashSet<string> _validForamts = new HashSet<string>
        {
            "f","F","X","x","g","G","d","D"
        };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
