using System;
using Trinity.Framework.Helpers;
using System.Linq;
using System.Runtime.CompilerServices;
using Trinity.Framework.Objects;

namespace Trinity.Modules
{
    public class TrinityChangeDetector<T> : ChangeDetector<T>
    {
        public TrinityChangeDetector(Func<T> valueProducer, TimeSpan interval = default(TimeSpan), [CallerMemberName] string name = "") : base(valueProducer, interval, name)
        {
            ChangeMonitor.Store.Add(this);
        }
    }

    public class ChangeMonitor : Module
    {
        public static readonly ChangeDetectorStore Store = new ChangeDetectorStore();

        public void Add(IChangeDetector detector) => Store.Add(detector);

        protected override int UpdateIntervalMs => 100;

        protected override void OnPulse()
        {
            Update();
        }

        public void Update()
        {
            foreach (var detector in Store.GetDetectors().Where(d => d.IsEnabled && d.HasSubscribers))
            {
                detector.CheckForChanges();
            }
        }
    }
}
