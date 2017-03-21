using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Events;
using Trinity.Framework.Objects;
using Zeta.Bot;
using Zeta.Game;

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
            ExecuteOnInstances(util => util.IsEnabled = true);
        }

        public static void DisableModules()
        {
            ExecuteOnInstances(util => util.IsEnabled = false);
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
                ProfileManager.OnProfileLoaded += OnProfileLoaded;
                ChangeEvents.WorldId.Changed += WorldIdOnChanged;
                ChangeEvents.IsInGame.Changed += IsInGameOnChanged;
                ChangeEvents.HeroId.Changed += HeroIdOnChanged;
                BotMain.OnShutdownRequested += BotMainOnShutdownRequested;
                BotMain.OnStop += BotMainOnStop;
                BotMain.OnStart += BotMainOnStart;
                ExecuteOnInstances(m => m.PluginEnabled());
                EnableModules();
                IsEnabled = true;
            }
        }

        private static void GameEventsOnGameJoined(object sender, EventArgs e)
        {
            IsGameJoined = true;
            ExecuteOnInstances(m => m.GameJoined());
        }

        private static void GameEventsOnGameLeft(object sender, EventArgs e)
        {
            IsGameJoined = false;
            ExecuteOnInstances(m => m.GameLeft());
        }

        private static void GameEventsOnPlayerDied(object sender, EventArgs e)
            => ExecuteOnInstances(m => m.PlayerDied());

        private static void PulsatorOnPulse(object sender, EventArgs e)
        {
            if ((IsGameJoined || !BotMain.IsRunning) && !ZetaDia.Globals.IsLoadingWorld)
            {
                ExecuteOnInstances(m => m.Pulse());
            }
        }

        private static void OnProfileLoaded(object sender, EventArgs eventArgs) 
            => ExecuteOnInstances(m => m.ProfileLoaded());
 
        private static void BotMainOnStart(IBot bot) 
            => ExecuteOnInstances(m => m.BotStart());

        private static void BotMainOnStop(IBot bot) 
            => ExecuteOnInstances(m => m.BotStop());

        private static void BotMainOnShutdownRequested(object sender, ShutdownRequestedEventArgs args)
            => ExecuteOnInstances(m => m.ShutDown());
 
        private static void HeroIdOnChanged(ChangeEventArgs<int> args)
            => ExecuteOnInstances(m => m.HeroIdChanged(args));

        private static void IsInGameOnChanged(ChangeEventArgs<bool> args)
            => ExecuteOnInstances(m => m.IsInGameChanged(args));

        private static void WorldIdOnChanged(ChangeEventArgs<int> args)
            => ExecuteOnInstances(m => m.WorldChanged(args));

        private static void InBossEncounterChanged(ChangeEventArgs<bool> args)
            => ExecuteOnInstances(m => m.BossEncounterChanged(args));

        public static void Disable()
        {
            Pulsator.OnPulse -= PulsatorOnPulse;
            GameEvents.OnPlayerDied -= GameEventsOnPlayerDied;
            GameEvents.OnGameJoined -= GameEventsOnGameJoined;
            ProfileManager.OnProfileLoaded -= OnProfileLoaded;
            ChangeEvents.WorldId.Changed -= WorldIdOnChanged;
            ChangeEvents.IsInGame.Changed -= IsInGameOnChanged;
            ChangeEvents.HeroId.Changed -= HeroIdOnChanged;
            BotMain.OnShutdownRequested -= BotMainOnShutdownRequested;
            BotMain.OnStop -= BotMainOnStop;
            BotMain.OnStart -= BotMainOnStart;
            DisableModules();
            ExecuteOnInstances(m => m.PluginDisabled());
            IsEnabled = false;
        }

        internal static void Pulse()
        {
            ExecuteOnInstances(m => m.Pulse());
        }

        private static void ExecuteOnInstances(Action<Module> action)
        {
            foreach (var utilReference in Instances.ToList())
            {
                Module util;
                if (utilReference.TryGetTarget(out util))
                {
                    Core.Logger.Verbose(LogCategory.CrashDebug, "ModuleManager.ExecuteOnInstances " + util.Name);

                    action?.Invoke(util);
                }
                else
                {
                    Instances.Remove(utilReference);
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

        public static bool IsGameJoined { get; private set; }
    }
}