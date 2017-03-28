using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Trinity.Framework.Events;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Service;

namespace Trinity.Framework
{
    public class ModuleManager
    {
        public static void Add(Module module)
        {
            Instances.Add(new WeakReference<Module>(module));
        }

        public static List<WeakReference<Module>> Instances = new List<WeakReference<Module>>();

        public static void EnableModules()
        {
            GetModules().ForEach(m => m.IsEnabled = true);
        }

        public static void DisableModules()
        {
            GetModules().ForEach(m => m.IsEnabled = false);
        }

        public static bool IsEnabled { get; set; }

        public static void Enable()
        {
            if (!IsEnabled)
            {
                Pulsator.OnPulse += PulsatorOnPulse;
                GameEvents.OnPlayerDied += GameEventsOnPlayerDied;
                GameEvents.OnGameJoined += GameEventsOnGameJoined;
                GameEvents.OnGameLeft += GameEventsOnGameLeft;
                GameEvents.OnGameChanged -= GameEventsOnGameChanged;
                ProfileManager.OnProfileLoaded += OnProfileLoaded;
                ChangeEvents.WorldId.Changed += WorldIdOnChanged;
                ChangeEvents.HeroId.Changed += HeroIdOnChanged;
                BotMain.OnShutdownRequested += BotMainOnShutdownRequested;
                BotMain.OnStop += BotMainOnStop;
                BotMain.OnStart += BotMainOnStart;
                SafeExecuteOnInstances(m => m.PluginEnabled());
                EnableModules();
                IsEnabled = true;
            }
        }

        private static void GameEventsOnGameChanged(object sender, EventArgs e)
        {
            Core.Logger.Debug("GameEventsOnGameChanged Fired");
            IsGameJoined = ZetaDia.IsInGame;
            GetModules().ForEach(m => m.GameChanged());
        }

        private static void GameEventsOnGameJoined(object sender, EventArgs e)
        {
            Core.Logger.Debug("GameEventsOnGameJoined Fired");
            IsGameJoined = true;
            GetModules().ForEach(m => m.GameJoined());
        }

        private static void GameEventsOnGameLeft(object sender, EventArgs e)
        {
            Core.Logger.Debug("GameEventsOnGameLeft Fired");
            IsGameJoined = false;
            GetModules().ForEach(m => m.GameLeft());
        }

        private static void GameEventsOnPlayerDied(object sender, EventArgs e)
            => SafeExecuteOnInstances(m => m.PlayerDied());

        private static void PulsatorOnPulse(object sender, EventArgs e)
        {
            if ((IsGameJoined || !BotMain.IsRunning) && !ZetaDia.Globals.IsLoadingWorld)
            {
                SafeExecuteOnInstances(m => m.Pulse());
            }
            else
            {
                Core.Logger.Debug($"Discarding Pulse IsGameJoined={IsGameJoined} BotMain.IsRunning={BotMain.IsRunning} LoadingWorld={ZetaDia.Globals.IsLoadingWorld}");
            }
        }

        private static void OnProfileLoaded(object sender, EventArgs eventArgs) 
            => GetModules().ForEach(m => m.ProfileLoaded());
 
        private static void BotMainOnStart(IBot bot) 
            => GetModules().ForEach(m => m.BotStart());

        private static void BotMainOnStop(IBot bot) 
            => GetModules().ForEach(m => m.BotStop());

        private static void BotMainOnShutdownRequested(object sender, ShutdownRequestedEventArgs args)
            => GetModules().ForEach(m => m.ShutDown());
 
        private static void HeroIdOnChanged(ChangeEventArgs<int> args)
        {
            Core.Logger.Debug("HeroIdOnChanged Fired");
            SafeExecuteOnInstances(m => m.HeroIdChanged(args));
        }

        private static void IsInGameOnChanged(ChangeEventArgs<bool> args)
        {
            Core.Logger.Debug("IsInGameOnChanged Fired");
            SafeExecuteOnInstances(m => m.IsInGameChanged(args));
        }

        private static void WorldIdOnChanged(ChangeEventArgs<int> args)
        {
            Core.Logger.Debug("WorldIdOnChanged Fired");
            SafeExecuteOnInstances(m => m.WorldChanged(args));
        }

        private static void InBossEncounterChanged(ChangeEventArgs<bool> args)
            => SafeExecuteOnInstances(m => m.BossEncounterChanged(args));

        public static void Disable()
        {
            Pulsator.OnPulse -= PulsatorOnPulse;
            GameEvents.OnPlayerDied -= GameEventsOnPlayerDied;
            GameEvents.OnGameJoined -= GameEventsOnGameJoined;
            GameEvents.OnGameChanged -= GameEventsOnGameChanged;
            ProfileManager.OnProfileLoaded -= OnProfileLoaded;
            ChangeEvents.WorldId.Changed -= WorldIdOnChanged;
            ChangeEvents.IsInGame.Changed -= IsInGameOnChanged;
            ChangeEvents.HeroId.Changed -= HeroIdOnChanged;
            BotMain.OnShutdownRequested -= BotMainOnShutdownRequested;
            BotMain.OnStop -= BotMainOnStop;
            BotMain.OnStart -= BotMainOnStart;
            DisableModules();
            SafeExecuteOnInstances(m => m.PluginDisabled());
            IsEnabled = false;
        }

        internal static void Pulse()
        {
            SafeExecuteOnInstances(m => m.Pulse());
        }

        private static void SafeExecuteOnInstances(Action<Module> action, [CallerMemberName] string caller = "")
        {
            foreach (var module in GetModules())
            {
                var notInGame = GameData.MenuWorldSnoIds.Contains(ZetaDia.Globals.WorldSnoId);
                var partyLocked = ZetaDia.Service.Party.CurrentPartyLockReasonFlags != PartyLockReasonFlag.None;
                if (notInGame || partyLocked)
                {
                    Core.Logger.Log("Aborting Updates, not in game or party locked.");
                    return;
                }
                try
                {
                    action?.Invoke(module);
                }
                catch (Exception ex)
                {
                    Core.Logger.Debug($"Action '{caller}' threw exception for module '{module.Name}'.");
                }
            }
        }

        private static IEnumerable<Module> GetModules()
        {
            foreach (var utilReference in Instances.ToList())
            {
                Module util;
                if (utilReference.TryGetTarget(out util))
                {
                    yield return util;
                }
                else
                {
                    Instances.Remove(utilReference);
                }
            }
        }

        public static IEnumerable<IDynamicSetting> DynamicSettings => GetModules().OfType<IDynamicSetting>();

        public static bool IsGameJoined { get; set; }

        public static void OutOfGamePulse()
        {
            if (IsGameJoined)
            {
                IsGameJoined = false;
                GameEventsOnGameLeft(null,null);
            }
        }
    }
}