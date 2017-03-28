using System;
using System.Windows.Controls;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.UI;
using Trinity.Components.Adventurer.Util;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Bot;
using Zeta.Game;

namespace Trinity.Components.Adventurer
{
    public sealed class Adventurer : Module, IDynamicSetting
    {
        private static readonly Lazy<Adventurer> _instance = new Lazy<Adventurer>(() => new Adventurer());
        public static Adventurer Instance => _instance.Value;

        private Adventurer()
        {
        }

        protected override int UpdateIntervalMs => 50;

        protected override void OnGameJoined()
        {
            if (Core.Scenes.CurrentScene?.LevelAreaId != ZetaDia.CurrentLevelAreaSnoId)
            {
                Core.Scenes.Reset();
            }
        }

        protected override void OnWorldChanged(ChangeEventArgs<int> args)
        {
            PluginEvents.WorldChangeTime = PluginTime.CurrentMillisecond;
            Core.Logger.Debug("[BotEvents] World has changed to WorldId: {0} LevelAreaSnoIdId: {1}", ZetaDia.Globals.WorldSnoId, ZetaDia.CurrentLevelAreaSnoId);
            EntryPortals.AddEntryPortal();
        }

        public static long TimeSinceWorldChange
        {
            get
            {
                if (PluginEvents.WorldChangeTime == 0)
                {
                    return Int32.MaxValue;
                }
                return PluginTime.CurrentMillisecond - PluginEvents.WorldChangeTime;
            }
        }

        protected override void OnBotStop()
        {
            BountyStatistics.Report();
        }

        protected override void OnPulse()
        {
            ExplorationGrid.PulseSetVisited();
            BountyStatistics.Pulse();
        }

        protected override void OnPluginEnabled()
        {
            BotMain.OnStart += PluginEvents.OnBotStart;
            BotMain.OnStop += PluginEvents.OnBotStop;
            DeveloperUI.InstallTab();      
            SafeZerg.Instance.DisableZerg();
        }

        protected override void OnPluginDisabled()
        {
            DeveloperUI.RemoveTab();
        }

        public static bool IsAdventurerTagRunning()
        {
            return true;
        }

        public static string GetCurrentTag()
        {
            if (ProfileManager.OrderManager == null || ProfileManager.OrderManager.CurrentBehavior == null)
            {
                return String.Empty;
            }
            return ProfileManager.OrderManager.CurrentBehavior.GetType().Name;
        }

        public static ISubroutine GetCurrentCoroutine()
        {
            return GenericBountyCoroutine.LastBountySubroutine;
        }

        #region Explicit IDynamicSetting Implementation

        string IDynamicSetting.GetName() => Name;

        UserControl IDynamicSetting.GetControl() => ConfigWindow.Instance.Content as UserControl;

        object IDynamicSetting.GetDataContext() => PluginSettings.Current.GetDataContext();

        string IDynamicSetting.GetCode() => PluginSettings.Current.GenerateCode();

        void IDynamicSetting.ApplyCode(string code) => PluginSettings.Current.ApplySettingsCode(code);

        void IDynamicSetting.Reset() => PluginSettings.Current.LoadDefaults();

        void IDynamicSetting.Save()
        {
        }

        #endregion Explicit IDynamicSetting Implementation
    }
}