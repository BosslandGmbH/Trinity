#region

using System;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

#endregion

namespace Trinity.Coroutines
{
    public class ClearArea
    {
        public static Vector3 StartPosition;
        public static DateTime ClearStarted = DateTime.MinValue;
        public static int ClearTimeSeconds = 10;
        public static bool IsClearing;
        public static int StartWorld;

        static ClearArea()
        {
            Core.CastStatus.CastFailure += info => Start();
            Core.CastStatus.CastSuccess += info => Stop();
        }

        public static void Start()
        {
            if (!IsClearing)
            {
                Logger.Log("Started Clearing Area");
                IsClearing = true;
                TrinityTownRun.IsVendoring = false;
                Combat.CombatMode = CombatMode.KillAll;
                StartWorld = ZetaDia.CurrentWorldSnoId;
                StartPosition = ZetaDia.Me.Position;
            }
        }

        public static void Stop()
        {
            if (IsClearing)
            {
                Logger.Log("Stopped Clearing Area");
                IsClearing = false;
                Combat.CombatMode = CombatMode.Normal;
            }
        }

        public static async Task<bool> Execute()
        {
            if (!IsClearing)
                return false;

            Logger.LogDebug("Clear task executed");

            var noMonsters = !ZetaDia.Actors.GetActorsOfType<DiaUnit>().Any(u => u?.CommonData != null && u.CommonData.IsValid && u.IsAlive && u.IsHostile && u.Distance < 70f);
            if (noMonsters)
            {
                Logger.LogDebug($"No Monsters nearby, go back to portal position. Distance={StartPosition.Distance(ZetaDia.Me.Position)}");
                await MoveTo.Execute(StartPosition, "Town Portal Position", 15f, () => ZetaDia.CurrentWorldSnoId != StartWorld || Navigator.StuckHandler.IsStuck || PlayerMover.IsBlocked);
                Stop();
                return false;
            }

            var clearFinished = DateTime.UtcNow.Subtract(ClearStarted).TotalSeconds > ClearTimeSeconds;
            if (clearFinished)
            {
                Logger.LogDebug("Clear timer finished, go back to portal position. Distance={StartPosition.Distance(ZetaDia.Me.Position)}");
                await MoveTo.Execute(StartPosition, "Town Portal Position", 15f, () => ZetaDia.CurrentWorldSnoId != StartWorld || Navigator.StuckHandler.IsStuck || PlayerMover.IsBlocked);
                Stop();
                return false;
            }

            var worldChanged = ZetaDia.CurrentWorldSnoId != StartWorld;
            if (worldChanged)
            {
                Logger.LogDebug("World Changed, Stop Clearing");
                Stop();
                return false;
            }
              
            return true;
        }
    }
}