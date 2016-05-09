using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity;
using Trinity.DbProvider;
using Trinity.Helpers;
using TrinityCoroutines.Resources;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.TreeSharp;
using Logger = Trinity.Technicals.Logger;

namespace TrinityCoroutines
{
    public class MoveTo
    {
        private static int _startingWorldId;

        /// <summary>
        /// Moves to somewhere.
        /// </summary>
        /// <param name="location">where to move to</param>
        /// <param name="destinationName">name of location for debugging purposes</param>
        /// <param name="range">how close it should get</param>
        public static async Task<bool> Execute(Vector3 location, string destinationName = "", float range = 10f, Func<bool> stopCondition = null)
        {
            if (string.IsNullOrEmpty(destinationName))
                destinationName = location.ToString();

            _startingWorldId = ZetaDia.CurrentWorldSnoId;

            if (Trinity.TrinityPlugin.Player.IsInTown)
            {
                GameUI.CloseVendorWindow();
            }

            while(ZetaDia.Me.LoopingAnimationEndTime > 0)
            {
                await Coroutine.Sleep(50);
            }

            Navigator.PlayerMover.MoveTowards(location);

            while (ZetaDia.IsInGame && location.Distance2D(ZetaDia.Me.Position) >= range && !ZetaDia.Me.IsDead)
            {
                if (Navigator.StuckHandler.IsStuck)
                {
                    await Navigator.StuckHandler.DoUnstick();
                    Logger.LogVerbose("MoveTo Finished. (Stuck)", _startingWorldId, ZetaDia.CurrentWorldSnoId);
                    break;
                }

                if (stopCondition != null && stopCondition())
                {
                    Logger.LogVerbose("MoveTo Finished. (Stop Condition)", _startingWorldId, ZetaDia.CurrentWorldSnoId);
                    return false;
                }

                if (_startingWorldId != ZetaDia.CurrentWorldSnoId)
                {
                    Logger.LogVerbose("MoveTo Finished. World Changed from {0} to {1}", _startingWorldId, ZetaDia.CurrentWorldSnoId);
                    return false;
                }

                Logger.LogVerbose("Moving to " + destinationName);
                PlayerMover.NavigateTo(location, destinationName);
                await Coroutine.Yield();
            }

            var distance = location.Distance(ZetaDia.Me.Position);
            if (distance <= range)
                Navigator.PlayerMover.MoveStop();

            Logger.LogVerbose("MoveTo Finished. Distance={0}", distance);
            return true;
        }

    }
}
