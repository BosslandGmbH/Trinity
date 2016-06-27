using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Coroutines.Town;
using Trinity.Framework.Helpers;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;

namespace Trinity.Framework
{
    public partial class Module
    {
        public string Name { get; }

        public UpdateStats Stats { get; }

        public DateTime LastUpdated { get;  set; } = DateTime.MinValue;

        public bool IsEnabled { get; set; }

        protected virtual int UpdateIntervalMs { get; } = 200;

        protected virtual void OnPulse() { }

        protected virtual void OnWorldChanged() { }

        public Module()
        {
            Name = this.GetType().Name;
            Stats = new UpdateStats { Name = Name };
            Instances.Add(new WeakReference<Module>(this));
        }

        private void FireEvent(ModuleEventType moduleEventType)
        {
            if (!IsEnabled)
                return;

            switch (moduleEventType)
            {
                case ModuleEventType.ForcedPulse:
                    FirePulse(true);
                    break;
                case ModuleEventType.Pulse:
                    FirePulse();
                    break;
                case ModuleEventType.WorldChanged:
                    OnWorldChanged();
                    break;
            }
        }

        private void FirePulse(bool force = false)
        {
            if (!force && DateTime.UtcNow.Subtract(LastUpdated).TotalMilliseconds < UpdateIntervalMs)
                return;

            if (!ZetaDia.IsInGame)
                return;

            Stats.Start();
            OnPulse();
            LastUpdated = DateTime.UtcNow;
            Stats.Stop();
        }

    }

    public partial class Module
    {
        public static List<WeakReference<Module>> Instances = new List<WeakReference<Module>>();    

        public static void EnableAll()
        {
            ExecuteOnInstances(util => util.IsEnabled = true);
        }

        public static void DisableAll()
        {
            ExecuteOnInstances(util => util.IsEnabled = false);
        }

        public static void FireEventAll(ModuleEventType moduleEventType)
        {
            ExecuteOnInstances(util => util.FireEvent(moduleEventType));
        }

        private static void ExecuteOnInstances(Action<Module> action)
        {
            foreach (var utilReference in Instances.ToList())
            {
                Module util;
                if (utilReference.TryGetTarget(out util))
                {
                    action?.Invoke(util);
                }
                else
                {
                    Instances.Remove(utilReference);
                }
            }
        }
    }
}

