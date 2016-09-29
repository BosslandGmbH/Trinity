using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trinity.Framework.Helpers
{
    /// <summary>
    /// Exposes compile-time value of fields as collection.    
    /// </summary>
    /// <typeparam name="TBase">Class to Build enumeration of TValue properties</typeparam>
    /// <typeparam name="TItem">Type of Fields to include</typeparam>
    public class FieldCollection<TBase, TItem> : IEnumerable<TItem>
    {
        private static List<TItem> _list;
        public static List<TItem> ToList(bool clone = false)
        {
            return _list ?? (_list = Load(clone).ToList());
        }

        public static List<TItem> Items => _list ?? Load().ToList();

        public static IEnumerable<TItem> Load(bool clone = false)
        {
            var source = typeof(TBase).GetFields(BindingFlags.Public | BindingFlags.Static).Select(f => f.GetValue(null)).OfType<TItem>();
            
            if (clone && IsCloneableType(typeof (TItem)))
            {
                return source.Select(o => (TItem)((ICloneable)o).Clone());
            }
            return source;
        }

        public static IEnumerable<TItem> Where(Func<TItem, bool> predicate)
        {
            return Items.Where(predicate);
        }

        public static bool Any(Func<TItem, bool> predicate)
        {
            return Items.Any(predicate);
        }

        public static IOrderedEnumerable<TItem> OrderBy(Func<TItem, bool> predicate)
        {
            return Items.OrderBy(predicate);
        }

        public static IOrderedEnumerable<TItem> OrderByDescending(Func<TItem, bool> predicate)
        {
            return Items.OrderByDescending(predicate);
        }

        public int Count
        {
            get { return Items.Count; }
        }

        public static bool IsCloneableType(Type type)
        {
            if (typeof(ICloneable).IsAssignableFrom(type))
                return true;

            return type.IsValueType;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    
}
