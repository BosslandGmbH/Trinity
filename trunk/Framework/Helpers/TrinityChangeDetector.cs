using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Zeta.Game;

namespace Trinity.Framework.Helpers
{
    public interface IChangeDetector
    {
        bool IsEnabled { get; set; }
        bool HasSubscribers { get; }
        string Name { get; }
        bool CheckForChanges();
    }

    public class ChangeDetectorEventArgs<T> : EventArgs
    {
        public T OldValue { get; set; }
        public T NewValue { get; set; }
        public string Name { get; set; }
    }

    public class ChangeDetector<T> : IChangeDetector
    {
        public delegate void ChangeEvent(ChangeDetectorEventArgs<T> args);

        public ChangeDetector(Func<T> valueProducer, TimeSpan interval, string name = "")
        {
            if (valueProducer == null)
                throw new ArgumentNullException(nameof(valueProducer));

            if (interval.Ticks < 0)
                interval = TimeSpan.FromMilliseconds(250);

            Name = name;
            Interval = interval;
            ValueProducer = valueProducer;
            IsEnumerable = typeof(IEnumerable).IsAssignableFrom(typeof(T));
            IsEnabled = true;
        }

        private Func<T> ValueProducer { get; }
        public T Value { get; private set; }
        public DateTime LastCheck { get; private set; }
        public TimeSpan Interval { get; set; }
        public int HashValue { get; private set; }
        public bool IsEnumerable { get; }
        public bool IsEnabled { get; set; }
        public DateTime LastChanged { get; private set; }
        public string Name { get; }
        public bool HasSubscribers => Changed != null;

        public bool CheckForChanges()
        {
            if (!IsEnabled)
                return false;

            if (DateTime.UtcNow - LastCheck < Interval)
                return false;

            LastCheck = DateTime.UtcNow;
            var newVal = ValueProducer();
            var oldVal = Value;

            if (IsEnumerable)
            {
                var oldHash = HashValue;
                HashValue = GetEnumerableHashCode((IEnumerable)newVal);
                if (oldHash != HashValue)
                {
                    SetValue(oldVal, newVal);
                    return true;
                }

                return false;
            }

            if (!EqualityComparer<T>.Default.Equals(oldVal, newVal))
            {
                SetValue(oldVal, newVal);
                return true;
            }

            return false;
        }

        private void SetValue(T oldVal, T newVal)
        {
            LastChanged = DateTime.UtcNow;
            Value = newVal;
            Changed?.Invoke(new ChangeDetectorEventArgs<T>
            {
                Name = Name,
                OldValue = oldVal,
                NewValue = newVal
            });
        }

        public event ChangeEvent Changed;

        public int GetEnumerableHashCode(IEnumerable enumerable)
        {
            if (enumerable == null)
                return 0;

            unchecked
            {
                var hash = 19;
                foreach (var item in enumerable)
                {
                    hash = hash * 31 + item.GetHashCode();
                }
                return hash;
            }
        }
    }

    public delegate Task<bool> ChangeTask(Func<List<IChangeDetector>> detectors);

    public class ChangeDetectorMonitor : ChangeDetectorStore
    {
        private readonly ChangeTask _task;

        public ChangeDetectorMonitor(ChangeTask task)
        {
            _task = task;
        }

        public Task Worker { get; set; }

        public override void Add(IChangeDetector detector)
        {
            base.Add(detector);

            if (Worker == null)
            {
                Worker = Task.Run(async () => await _task(GetDetectors));
            }
        }
    }

    public class ChangeDetectorStore
    {
        private readonly List<WeakReference<IChangeDetector>> _detectors = new List<WeakReference<IChangeDetector>>();

        public List<IChangeDetector> GetDetectors()
        {
            var detectors = new List<IChangeDetector>();
            foreach (var utilReference in _detectors.ToList())
            {
                IChangeDetector detector;
                if (utilReference.TryGetTarget(out detector))
                {
                    detectors.Add(detector);
                }
                else
                {
                    _detectors.Remove(utilReference);
                }
            }
            return detectors;
        }

        public virtual void Add(IChangeDetector detector)
        {
            _detectors.Add(new WeakReference<IChangeDetector>(detector));
        }
    }

}