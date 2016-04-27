using System;
using System.Configuration;
using System.Globalization;
using System.Windows.Data;
using Trinity.UI.UIComponents;

namespace Trinity.UIComponents
{
    public class PropertyToBindingMemberConverter : IValueConverter
    {
        /// <summary>
        /// Wraps a property as a binding member instance
        /// </summary>
        /// <param name="value">a property</param>
        /// <param name="parameter">the parent class of value property</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            var parentType = parameter.GetType();
            if (!parentType.IsClass)                          
                return null;

            var propertyType = value.GetType();

            var param  = value as string;
            if (param != null)
            {
                var paramBinding = GetProperty(parentType, propertyType.Name);
                if (paramBinding != null)
                    return paramBinding;
            }

            return null;
        }

        private static BindingMember GetProperty(object value, string propertyName, BindingMember parent = null, bool requireSettingAttribute = false)
        {
            var property = value.GetType().GetProperty(propertyName);
            if (property != null && (!requireSettingAttribute || property.IsDefined(typeof(SettingAttribute), false)))
            {
                return new BindingMember(property, value, parent);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}


