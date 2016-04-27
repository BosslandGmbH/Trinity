using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Trinity.Helpers
{
    [DataContract]
    [KnownType("DerivedTypes")]
    public class NotifyBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Called by KnownType Attribute, avoids having to specify all known types for DataContract in derived types.
        /// </summary>
        private static Type[] DerivedTypes()
        {
            return GetDerivedTypes(typeof(NotifyBase), Assembly.GetExecutingAssembly()).ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool ThrottleChangeNotifications { get; set; }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            if(!ThrottleChangeNotifications)
                OnPropertyChanged(propertyName);
            return true;
        }

        public static IEnumerable<Type> GetDerivedTypes(Type baseType, Assembly assembly)
        {
            var types = from t in assembly.GetTypes()
                        where t.IsSubclassOf(baseType)
                        select t;

            return types;
        }

        public virtual void LoadDefaults()
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(this))
            {
                var myAttribute = (DefaultValueAttribute)property.Attributes[typeof(DefaultValueAttribute)];
                if (myAttribute != null)
                {
                    property.SetValue(this, myAttribute.Value);
                }
            }
        }
    }
}
