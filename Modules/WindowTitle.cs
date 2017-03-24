using System;
using Trinity.Framework;
using System.Windows;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;

namespace Trinity.Modules
{
    public class WindowTitle : Module
    {
        protected override int UpdateIntervalMs => 1000;

        protected override void OnPulse()
        {
            var title = string.Empty;

            if (Core.Settings.Advanced.ShowBattleTag)
                title += $"{Core.Player.BattleTag} ";

            if (Core.Settings.Advanced.ShowHeroName)
                title += $"{Core.Player.Name} ";

            if (Core.Settings.Advanced.ShowHeroClass)
                title += $"{Core.Player.ActorClass} ";

            if (!string.IsNullOrEmpty(title))
                Application.Current.Dispatcher.Invoke((Action)(()
                    => Application.Current.MainWindow.Title = $"DemonBuddy ({DemonbuddyUI.Version}) - {title}"));
        }
    }
}
