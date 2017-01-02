using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.UI;
using Trinity.Framework.Objects;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using JsonSerializer = Trinity.Components.Adventurer.Util.JsonSerializer;

namespace Trinity.Components.Adventurer
{
    public sealed class Adventurer : Component, IDynamicSetting
    {
        private static readonly Lazy<Adventurer> _instance = new Lazy<Adventurer>(() => new Adventurer());
        public static Adventurer Instance => _instance.Value;

        private Adventurer()
        {
    
        }

        protected override int UpdateIntervalMs => 50;

        protected override void OnPulse()
        {
            PluginEvents.PulseUpdates();
        }

        protected override void OnPluginEnabled()
        {
            BotMain.OnStart += PluginEvents.OnBotStart;
            BotMain.OnStop += PluginEvents.OnBotStop;
            DeveloperUI.InstallTab();
            BotEvents.WireUp();
            SafeZerg.Instance.DisableZerg();
        }

        protected override void OnPluginDisabled()
        {
            DeveloperUI.RemoveTab();
            BotEvents.UnWire();
        }

        public static bool IsAdventurerTagRunning()
        {            
            //const string tagsNameSpace = "Trinity.Components.Adventurer.Tags";
            //if (ProfileManager.OrderManager == null || ProfileManager.OrderManager.CurrentBehavior == null)
            //{
            //    return false;
            //}
            //return ProfileManager.OrderManager.CurrentBehavior.GetType().Namespace == tagsNameSpace;
            return true;
        }

        public static string GetCurrentTag()
        {
            if (ProfileManager.OrderManager == null || ProfileManager.OrderManager.CurrentBehavior == null)
            {
                return string.Empty;
            }
            return ProfileManager.OrderManager.CurrentBehavior.GetType().Name;
        }

        #region Explicit IDynamicSetting Implementation

        string IDynamicSetting.GetName() => Name;
        UserControl IDynamicSetting.GetControl() => ConfigWindow.Instance.Content as UserControl;
        object IDynamicSetting.GetDataContext() => PluginSettings.Current.GetDataContext();
        string IDynamicSetting.GetCode() => PluginSettings.Current.GenerateCode();
        void IDynamicSetting.ApplyCode(string code) => PluginSettings.Current.ApplySettingsCode(code);
        void IDynamicSetting.Reset() => PluginSettings.Current.LoadDefaults();
        void IDynamicSetting.Save() { }

        #endregion
    }
}
