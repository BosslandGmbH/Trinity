using System;
using Trinity.Framework.Helpers;
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

        public Module()
        {
            Name = GetType().Name;
            Stats = new UpdateStats { Name = Name };
            ModuleManager.Add(this);
        }

        public void Pulse(bool force = false)
        {
            if (DateTime.UtcNow.Subtract(LastUpdated).TotalMilliseconds < UpdateIntervalMs)
                return;

            if (!ZetaDia.IsInGame)
                return;

            Stats.Start();
            OnPulse();
            LastUpdated = DateTime.UtcNow;
            Stats.Stop();
        }

        protected virtual void OnPulse() { }
        public void WorldChanged(ChangeEventArgs<int> args) => OnWorldChanged(args);
        protected virtual void OnWorldChanged(ChangeEventArgs<int> args) { }
        public void GameJoined() => OnGameJoined();
        protected virtual void OnGameJoined() { }
        public void GameLeft() => OnGameLeft();
        protected virtual void OnGameLeft() { }
        public void PluginEnabled() => OnPluginEnabled();
        protected virtual void OnPluginEnabled() { }
        public void PluginDisabled() => OnPluginDisabled();
        protected virtual void OnPluginDisabled() { }
        public void ProfileLoaded() => OnProfileLoaded();
        protected virtual void OnProfileLoaded() { }
        public void ShutDown() => OnShutdown();
        protected virtual void OnShutdown() { }
        public void HeroIdChanged(ChangeEventArgs<int> args) => OnHeroIdChanged(args);
        protected virtual void OnHeroIdChanged(ChangeEventArgs<int> args) { }
        public void PlayerDied() => OnPlayerDied();
        protected virtual void OnPlayerDied() { }
        public void BotStop() => OnBotStop();
        protected virtual void OnBotStop() { }
        public void BotStart() => OnBotStart();
        protected virtual void OnBotStart() { }
        public void IsInGameChanged(ChangeEventArgs<bool> args) => OnIsInGameChanged(args);
        protected virtual void OnIsInGameChanged(ChangeEventArgs<bool> args) { }
        public void BossEncounterChanged(ChangeEventArgs<bool> args) => OnBossEncounterChanged(args);
        protected virtual void OnBossEncounterChanged(ChangeEventArgs<bool> args) { }
    }
}


