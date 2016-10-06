using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace Trinity.UI.UIComponents.MarkupExtensions
{
    public class ValueExtension : MarkupExtension
    {
        public object DesignValue { get; set; } = DependencyProperty.UnsetValue;

        [ConstructorArgument("value")]
        public object Value { get; set; } = DependencyProperty.UnsetValue;

        public ValueExtension()
        {
        }

        public ValueExtension(object value)
        {
            Value = value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var provideValueTarget = serviceProvider.GetService(typeof (IProvideValueTarget)) as IProvideValueTarget;
            var target = provideValueTarget.TargetObject as FrameworkElement;

            var value = DesignerProperties.GetIsInDesignMode(target) && DesignValue != DependencyProperty.UnsetValue ? DesignValue : Value;

            if (value == DependencyProperty.UnsetValue || value == null)
                return value;

            if (value is MarkupExtension)
                return ((MarkupExtension) value).ProvideValue(serviceProvider);

            var property = provideValueTarget.TargetProperty as DependencyProperty;

            if (property.PropertyType.IsInstanceOfType(value))
                return value;

            return TypeDescriptor.GetConverter(property.PropertyType).ConvertFrom(value);
        }
    }
}