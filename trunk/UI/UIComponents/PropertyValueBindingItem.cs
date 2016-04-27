using System;
using System.Reflection;

namespace Trinity.UI.UIComponents
{
    public class PropertyValueBindingItem
    {
        private object _value;

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

        private MemberInfo _member;

    }

}




