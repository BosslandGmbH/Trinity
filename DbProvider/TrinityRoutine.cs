// VERSION 1.2.0

using log4net;
using System;
using System.Windows;
using Zeta.Bot;
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
        private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        public override void Initialize()
        {
            //foreach (PluginContainer plugin in PluginManager.Plugins)
            //{
            //    if (plugin.Plugin.Name == "Trinity" && !plugin.Enabled)
            //        if (plugin.Plugin.Name == "TrinityPlugin" && !plugin.Enabled)
            //        {
            //            plugin.Enabled = true;
            //        }
            //}
        }

        public override void Dispose() { }
        public override string Name => "Trinity";

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
                    Log.Error($"[Trinity] Error Opening Plugin Config window! {ex}");
                }
                return null;
            }
        }

        public override ActorClass Class
        {
            get
            {
                if (!ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld)
                {
                    // Return none if we are oog to make sure we can start the bot anytime.
                    return ActorClass.Invalid;
                }

                return ZetaDia.Me.ActorClass;
            }
        }

        public override SNOPower DestroyObjectPower => SNOPower.None;
        public override float DestroyObjectDistance => 0;
        public override Composite Combat => NoAction;
        public override Composite Buff => NoAction;

        private Action NoAction = new Action();
    }
}