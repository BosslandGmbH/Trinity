using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Trinity.Framework.Helpers
{
    public class EasyCache
    {
        public interface ICacheValue
        {
            T Cast<T>();
        }

        public class CacheValue<T> : ICacheValue
        {
            public DateTime LastRefreshed;
            public T CachedValue;

            public TResult Cast<TResult>() => (TResult)Convert.ChangeType(CachedValue, typeof(T));
        }

        public TimeSpan Duration;

        public Dictionary<string, ICacheValue> CachedValues = new Dictionary<string, ICacheValue>();

        public EasyCache(TimeSpan duration)
        {
            Duration = duration;
        }

        public T GetValue<T>(Func<T> valueProducer, [CallerMemberName] string caller = "")
        {
            return GetValue(caller, valueProducer);
        }

        public T GetValue<T>(string propertyName, Func<T> valueProducer)
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