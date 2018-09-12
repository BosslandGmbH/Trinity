using System;

namespace Trinity.UI.UIComponents
{
    public class PropertyValueBindingItem
    {
        public PropertyValueBindingItem(object name, object value, string altName = "")
        {
            Name = name;
            Value = value;
            AltName = string.IsNullOrEmpty(altName) ? name.ToString() : altName;
        }

        public PropertyValueBindingItem() { }

        public object Name { get; set; }

        public string AltName { get; set; }

        public object Value { get; set; }

        public Type Type { get; set; }
    }

}
