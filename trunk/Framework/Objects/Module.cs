using System;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Zeta.Game;

namespace Trinity.Framework.Objects
{
    public class Module
    {
        public string Name { get; set; }

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

        public void FireEvent(ModuleEvent moduleEvent)
        {
            switch (moduleEvent)
            {
                case ModuleEvent.ForcedPulse:
                    FirePulse(true);
                    break;
                case ModuleEvent.Pulse:
                    FirePulse();
                    break;
                case ModuleEvent.WorldChanged:
                    OnWorldChanged();
                    break;
                case ModuleEvent.GameJoined:
                    OnGameJoined();
                    break;
                case ModuleEvent.PluginDisabled:
                    OnPluginDisabled();
                    break;
                case ModuleEvent.PluginEnabled:
                    OnPluginEnabled();
                    break;
                case ModuleEvent.Shutdown:
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


