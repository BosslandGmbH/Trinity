using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Framework.Helpers
{

    public class ChangeDetector<T>
    {
        public ChangeDetector(Func<T> valueProducer, Action<T> onChanged = null)
        {
            if (valueProducer == null)
                throw new ArgumentNullException(nameof(valueProducer));

            ValueProducer = valueProducer;
            OnChangedDelegate = onChanged;
        }

        public bool HasChanged
        {
            get
            {
                var newVal = ValueProducer();
                if (!Equals(Value, newVal))
                {
                    Value = newVal;
                    OnChangedDelegate?.Invoke(newVal);
                    return true;
                }
                return false;
            }
        }

        public T Value { get; set; }

        public Func<T> ValueProducer { get; set; }

        public Action<T> OnChangedDelegate { get; set; }    

        public bool CachedHasChanged { get; set; }

        public bool CachedCheck() => CachedHasChanged = HasChanged;

    }
}
