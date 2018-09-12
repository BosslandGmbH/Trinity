using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Trinity.UI.UIComponents.Converters
{
    public class FlagListParentUpdateMultiConverter : IMultiValueConverter
    {
        private readonly FlagsEnumValueConverter _flagsConverter = new FlagsEnumValueConverter();
        private Enum _itemsSource;
        private PropertyValueBindingItem _itemBinding;
        private string _name;
        private readonly List<string> _names = new List<string>();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var valuesList = values.ToList();
            if (!valuesList.Any())
                return null;

            _itemsSource = values.FirstOrDefault(i => i is Enum) as Enum;
            if (_itemsSource == null)
            {
                //Log.Verbose("ItemsControl not found!");
                return null;
            }
                
            _itemBinding = values.FirstOrDefault(i => i is PropertyValueBindingItem) as PropertyValueBindingItem;
            if (_itemBinding == null)
            {
                //Log.Verbose("ItemBinding not found!");
                return null;
            }

            var name = values.FirstOrDefault(i => i is string) as string;
            if (name == null)
            {
                //Log.Verbose("Name not found!");
                return null;
            }
            else
            {
                _name = name;
                _names.Add(name);
            }

            return _flagsConverter.Convert(_itemsSource, _itemBinding.Type, _itemBinding.Name.ToString(), culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            //Log.Info("ConvertBack Fired");
            return null;

            //if (_itemBinding == null || _itemsSource == null)
            //{
            //    Log.Verbose("ItemBinding not found!");
            //    return null;
            //}

            //var itemsSource = _flagsConverter.ConvertBack(_itemsSource, _itemBinding.Type, _itemBinding.Value, culture);

            //return new[]
            //{
            //    itemsSource,
            //    _itemBinding,
            //};
        }
    }
}


