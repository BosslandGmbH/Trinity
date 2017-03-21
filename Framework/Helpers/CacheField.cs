using System;
using Trinity.Framework.Objects.Enums;

namespace Trinity.Framework.Helpers
{
    public class CacheField<T>
    {
        private T _cachedValue;

        public CacheField(UpdateSpeed speed, T defaultValue = default(T))
        {
            Delay = (int)speed;
            _cachedValue = defaultValue;
        }

        public CacheField(int delay = -1)
        {
            Delay = delay;
        }

        public T CachedValue
        {
            get { return _cachedValue; }
            set
            {
                if (!IsValueCreated)
                    IsValueCreated = true;

                LastUpdate = DateTime.UtcNow;
                _cachedValue = value;
            }
        }

        public bool IsValueCreated { get; set; }

        public bool IsFrozen { get; set; }

        public bool IsCacheValid
        {
            get
            {
                if (IsValueOverride || IsFrozen) return true;
                if (!IsValueCreated) return false;
                if (Delay < 0) return true;
                if (Delay == 0) return false;
                return !(DateTime.UtcNow.Subtract(LastUpdate).TotalMilliseconds >= Delay);
            }
        }

        public int Delay { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsValueOverride { get; private set; }

        public T GetValue(Func<T> retriever)
        {
            if (IsCacheValid)
                return CachedValue;

            return CachedValue = retriever();
        }

        internal void SetValueOverride(T value)
        {
            IsValueOverride = true;
            IsValueCreated = true;
            CachedValue = value;
        }

        public void Clear()
        {
            IsValueOverride = false;
            IsValueCreated = false;
            CachedValue = default(T);
        }

        internal void Invalidate()
        {
            Clear();
        }
    }
}