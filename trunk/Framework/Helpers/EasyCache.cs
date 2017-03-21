using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Trinity.Framework.Helpers
{
    public class EasyCache
    {
        public interface ICacheValue
        {
            T Value<T>();
        }

        public class CacheValue<T> : ICacheValue
        {
            public DateTime LastRefreshed;
            public T CachedValue;

            public TV Value<TV>() => (TV)Convert.ChangeType(CachedValue, typeof(T));
        }

        public TimeSpan Duration;

        public Dictionary<string, ICacheValue> CachedValues = new Dictionary<string, ICacheValue>();

        public T Value<T>(Func<T> valueProducer, [CallerMemberName] string caller = "")
        {
            return Value(caller, valueProducer);
        }

        public T Value<T>(string propertyName, Func<T> valueProducer)
        {
            if (!CachedValues.ContainsKey(propertyName))
                return CreateValue(propertyName, valueProducer);

            var cached = CachedValues[propertyName] as CacheValue<T>;
            if (cached != null && DateTime.UtcNow < cached.LastRefreshed + Duration)
                return cached.CachedValue;

            return CreateValue(propertyName, valueProducer);
        }

        private T CreateValue<T>(string propertyName, Func<T> valueProducer)
        {
            var value = valueProducer();
            CachedValues[propertyName] = new CacheValue<T>
            {
                LastRefreshed = DateTime.UtcNow,
                CachedValue = value
            };
            return value;
        }
    }
}