using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trinity.Helpers
{
    public class LookupList<TKey, TElement> : ILookup<TKey, TElement>
    {
        public LookupList()
        {
            UpdateLookup();
        }

        public class Kvp
        {
            public object Key { get; set; }
            public object Value { get; set; }
        }

        private readonly List<KeyValuePair<TKey, TElement>> _storage = new List<KeyValuePair<TKey, TElement>>();

        private Lookup<TKey, TElement> _lookup;

        public void Add(TKey key, TElement value)
        {
            _storage.Add(new KeyValuePair<TKey, TElement>(key, value));
            UpdateLookup();
        }

        public void Remove(TKey key)
        {
            _storage.RemoveAll(o => Compare(o.Key, key));
            UpdateLookup();
        }

        static bool Compare<T>(T object1, T object2)
        {
            //Get the type of the object
            var type = typeof(T);

            //return false if any of the object is false
            if (type.IsValueType || object1 == null || object2 == null)
                return false;

            //Loop through each properties inside class and get values for the property from both the objects and compare
            foreach (System.Reflection.PropertyInfo property in type.GetProperties())
            {
                if (property.Name == "ExtensionData") continue;
                var object1Value = string.Empty;
                var object2Value = string.Empty;
                if (type.GetProperty(property.Name).GetValue(object1, null) != null)
                    object1Value = type.GetProperty(property.Name).GetValue(object1, null).ToString();
                if (type.GetProperty(property.Name).GetValue(object2, null) != null)
                    object2Value = type.GetProperty(property.Name).GetValue(object2, null).ToString();
                if (object1Value.Trim() != object2Value.Trim())
                    return false;
            }
            return true;
        }

        private void UpdateLookup()
        {
            _lookup = (Lookup<TKey, TElement>)_storage.ToLookup(k => k.Key, v => v.Value);
        }

        IEnumerator<IGrouping<TKey, TElement>> IEnumerable<IGrouping<TKey, TElement>>.GetEnumerator()
        {
            return (IEnumerator<IGrouping<TKey, TElement>>)((IEnumerable)_lookup).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_lookup).GetEnumerator();
        }

        public bool Contains(TKey key)
        {
            return _lookup.Contains(key);
        }

        public int Count
        {
            get { return _lookup.Count; }
        }

        public IEnumerable<TElement> this[TKey key]
        {
            get { return _lookup[key]; }
        }

        public object SyncRoot { get; private set; }
        public bool IsSynchronized { get; private set; }

        public virtual void Clear()
        {
            _storage.Clear();
            UpdateLookup();
        }
    }
}
