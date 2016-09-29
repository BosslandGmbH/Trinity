using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Trinity.Framework.Helpers;

namespace Trinity.UI.UIComponents.Converters
{
    public class PropertyValueConverter : IValueConverter
    {
        public string PropertyName = string.Empty;
        public BindingMember BindingMember { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var param  = parameter as string;
            if (param != null)
            {
                PropertyName = param;
                var paramBinding = GetProperty(value, param);
                if (paramBinding != null)
                {
                    BindingMember = paramBinding;
                    return paramBinding;
                }
            }

            var settingProperties = GetProperties(value).ToList();

            //// Flatten Embedded Settings Objects.
            //foreach (var pv in settingProperties.ToList())
            //{
            //    if (pv.Value is IDynamicSettings)
            //    {
            //        Log.Info(LogCategory.Configuration, "Detected Embedded Settings Object {0}", pv.Type);
            //        var childProps = GetProperties(pv.Value, pv).ToList();
            //        //pv.Children.AddRange(childProps);
            //        settingProperties.AddRange(childProps);
            //        settingProperties.Remove(pv);
            //    }                    
            //}

            var cvs = new CollectionViewSource();
            cvs.Source = settingProperties;
            cvs.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            return cvs.View;
        }

        private void NotifyOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (PropertyName == propertyChangedEventArgs.PropertyName)
            {
                Logger.LogNormal($"Property Changed {propertyChangedEventArgs.PropertyName}");
                BindingMember.OnPropertyChanged(nameof(BindingMember.Value));
            }
        }

        /// <summary>
        /// Gets a "BindingMember" object for every valid property in an object
        /// </summary>
        /// <param name="value">an object containing properties</param>
        /// <param name="parent">property value for which this property is a child</param>
        /// <returns></returns>
        private static IEnumerable<BindingMember> GetProperties(object value, BindingMember parent = null)
        {
            var settingProperties = 
                from p in value.GetType().GetProperties()
                where p != null && p.IsDefined(typeof (SettingAttribute), false)
                select new BindingMember(p, value, parent);

            return settingProperties;
        }

        private static BindingMember GetProperty(object value, string propertyName, BindingMember parent = null)
        {
            var property = value.GetType().GetProperty(propertyName);
            if (property != null && property.IsDefined(typeof(SettingAttribute), false))
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

