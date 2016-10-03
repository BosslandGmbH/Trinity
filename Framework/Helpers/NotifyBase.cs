using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Trinity.Framework.Helpers
{
    [DataContract]
    [KnownType("DerivedTypes")]
    public class NotifyBase : INotifyPropertyChanged
    {
        private static Type[] DerivedTypes()
        {
            return GetDerivedTypes(typeof(NotifyBase), Assembly.GetExecutingAssembly()).ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SupressChangeNotifications { get; set; }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            if(!SupressChangeNotifications)
                OnPropertyChanged(propertyName);
            return true;
        }

        public static IEnumerable<Type> GetDerivedTypes(Type baseType, Assembly assembly) => 
            from t in assembly.GetTypes()
            where t.IsSubclassOf(baseType)
            select t;

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
