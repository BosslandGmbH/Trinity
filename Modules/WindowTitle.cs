using System;
using Trinity.Framework;
using System.Windows;
using Trinity.Components.Combat;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Bot;

namespace Trinity.Modules
{
    public class WindowTitle : Module
    {
        protected override int UpdateIntervalMs => 1000;

        protected override void OnProfileLoaded()
        {
            ProfileName = ProfileManager.CurrentProfile?.Name ?? "";
        }

        public string ProfileName = "";

        protected override void OnPulse()
        {
            if (!BotMain.IsRunning)
                return;

            var title = string.Empty;

            if (Core.Settings.Advanced.ShowBattleTag)
                title += $"{Core.Player.BattleTag} ";

            if (Core.Settings.Advanced.ShowHeroName)
                title += $"{Core.Player.Name} ";

            if (Core.Settings.Advanced.ShowHeroClass)
                title += $"{Core.Player.ActorClass} ";

            title += $@" - ""{ProfileName}"" - {TrinityCombat.CombatMode} Mode";

            if (!string.IsNullOrEmpty(title))
                Application.Current.Dispatcher.Invoke((Action)(()
                    => Application.Current.MainWindow.Title = $"DemonBuddy ({DemonbuddyUI.Version}) - {title}"));
        }
    }
}
