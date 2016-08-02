using System;
using Trinity.Coroutines.Town;
using Trinity.Framework.Helpers;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;

namespace Trinity.Framework
{
    public class Module
    {
        public string Name { get; }

        public UpdateStats Stats { get; }

        public DateTime LastUpdated { get;  set; } = DateTime.MinValue;

        public bool IsEnabled { get; set; }

        protected virtual int UpdateIntervalMs { get; } = 200;

        protected virtual void OnPulse() { }

        protected virtual void OnWorldChanged() { }

        protected virtual void OnGameJoined() { }

        protected virtual void OnPluginEnabled() { }

        protected virtual void OnPluginDisabled() { }

        protected virtual void OnShutdown() { }

        public Module()
        {
            Name = GetType().Name;
            Stats = new UpdateStats { Name = Name };
            ModuleManager.Instances.Add(new WeakReference<Module>(this));
        }

        public void FireEvent(ModuleEventType moduleEventType)
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
                case ModuleEventType.GameJoined:
                    OnGameJoined();
                    break;
                case ModuleEventType.PluginDisabled:
                    OnPluginDisabled();
                    break;
                case ModuleEventType.PluginEnabled:
                    OnPluginEnabled();
                    break;
                case ModuleEventType.Shutdown:
                    OnShutdown();
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
}


