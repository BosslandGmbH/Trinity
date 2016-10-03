using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Framework.Modules
{
    /// <summary>
    /// Stops the bot when certain conditions are met
    /// </summary>
    public class GameStopper : Module
    {
        protected override int UpdateIntervalMs => 1000;

        protected override void OnPulse()
        {
            if (!BotMain.IsRunning)
                return;

            //if (Core.Settings.Advanced.StopOnGoblins)
            //{
            //    var goblin = Core.Actors.AllRActors.FirstOrDefault(g => g.IsTreasureGoblin);
            //    if (goblin != null)
            //    {
            //        Logger.Warn($"Stopping Bot because: Goblin Found! {goblin}");
            //        BotMain.Stop();
            //    }
            //}
        }

    }
}

