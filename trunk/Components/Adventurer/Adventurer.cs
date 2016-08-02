using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.UI;
using Trinity.Framework.Objects;
using Trinity.Helpers;
using Trinity.UI;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using JsonSerializer = Trinity.Components.Adventurer.Util.JsonSerializer;

namespace Trinity.Components.Adventurer
{
    public class Adventurer : Component, IDynamicSetting
    {
        public static Adventurer Instance;

        public Adventurer()
        {
            Instance = this;
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
            const string tagsNameSpace = "Trinity.Components.Adventurer.Tags";
            if (ProfileManager.OrderManager == null || ProfileManager.OrderManager.CurrentBehavior == null)
            {
                return false;
            }
            return ProfileManager.OrderManager.CurrentBehavior.GetType().Namespace == tagsNameSpace;
        }

        public static string GetCurrentTag()
        {
            const string tagsNameSpace = "Trinity.Components.Adventurer.Tags";
            if (ProfileManager.OrderManager == null || ProfileManager.OrderManager.CurrentBehavior == null)
            {
                return string.Empty;
            }
            var type = ProfileManager.OrderManager.CurrentBehavior.GetType();
            if (type.Namespace == tagsNameSpace)
            {
                return type.Name;
            }
            return string.Empty;
        }

        #region Explicit IDynamicSetting Implementation

        UserControl IDynamicSetting.Control => ConfigWindow.Instance.Content as UserControl;
        object IDynamicSetting.DataContext => ConfigWindow.Instance.DataContext;
        string IDynamicSetting.GetCode() => PluginSettings.Current.GenerateCode();
        void IDynamicSetting.ApplyCode(string code) => PluginSettings.Current.ApplySettingsCode(code);
        void IDynamicSetting.Reset() => PluginSettings.Current.LoadDefaults();
        void IDynamicSetting.Save() => PluginSettings.Current.Save();

        #endregion
    }
}
