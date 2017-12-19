// VERSION 1.2.1

using System;
using System.Windows;
using Zeta.Bot;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Action = Zeta.TreeSharp.Action;

namespace Trinity.DbProvider
{
    public class TrinityRoutine : CombatRoutine
    {
        public override void Initialize() { }
        public override void Dispose() { }
        public override string Name => "Trinity";
        public override ActorClass Class => !ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld ? ActorClass.Invalid : ZetaDia.Me.ActorClass;
        public override SNOPower DestroyObjectPower => SNOPower.None;
        public override float DestroyObjectDistance => 0;
        public override Composite Combat => new Action();
        public override Composite Buff => new Action();

        public override Window ConfigWindow
        {
            get
            {
                try
                {
                    foreach (PluginContainer plugin in PluginManager.Plugins)
                    {
                        if (plugin.Plugin.Name == "Trinity")
                        {
                            return plugin.Plugin.DisplayWindow;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                return null;
            }
        }
    }
}