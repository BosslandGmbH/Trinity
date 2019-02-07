#region

using System;
using Trinity.Framework;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Combat.Resources;
using Trinity.Components.Coroutines.Town;
using Trinity.DbProvider;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


#endregion

namespace Trinity.Components.Coroutines
{
    public class ClearArea
    {
        public static Vector3 StartPosition;
        public static DateTime ClearStarted = DateTime.MinValue;
        public static int ClearTimeSeconds = 10;
        public static bool IsClearing;
        public static SNOWorld StartWorld;

        static ClearArea()
        {
            Core.CastStatus.CastFailure += info => Start();
            Core.CastStatus.CastSuccess += info => Stop();
        }

        public static void Start()
        {
            if (!IsClearing)
            {
                Core.Logger.Log("Started Clearing Area");
                IsClearing = true;
                TrinityTownRun.IsVendoring = false;
                Combat.TrinityCombat.CombatMode = CombatMode.KillAll;
                StartWorld = ZetaDia.Globals.WorldSnoId;
                StartPosition = ZetaDia.Me.Position;
            }
        }

        public static void Stop()
        {
            if (IsClearing)
            {
                Core.Logger.Log("Stopped Clearing Area");
                IsClearing = false;
                Combat.TrinityCombat.CombatMode = CombatMode.Normal;
            }
        }

        public static async Task<bool> Execute()
        {
            if (!IsClearing)
                return false;

            Core.Logger.Debug("Clear task executed");

            var noMonsters = !ZetaDia.Actors.GetActorsOfType<DiaUnit>().Any(u => u?.CommonData != null && u.CommonData.IsValid && u.IsAlive && u.IsHostile && u.Distance < 70f);
            if (noMonsters)
            {
                Core.Logger.Debug($"No Monsters nearby, go back to portal position. Distance={StartPosition.Distance(ZetaDia.Me.Position)}");
                await CommonCoroutines.MoveAndStop(StartPosition, 15f, "Town Portal Position");
                Stop();
                return false;
            }

            var clearFinished = DateTime.UtcNow.Subtract(ClearStarted).TotalSeconds > ClearTimeSeconds;
            if (clearFinished)
            {
                Core.Logger.Debug("Clear timer finished, go back to portal position. Distance={StartPosition.Distance(ZetaDia.Me.Position)}");
                await CommonCoroutines.MoveAndStop(StartPosition, 15f, "Town Portal Position");
                Stop();
                return false;
            }

            var worldChanged = ZetaDia.Globals.WorldSnoId != StartWorld;
            if (worldChanged)
            {
                Core.Logger.Debug("World Changed, Stop Clearing");
                Stop();
                return false;
            }

            return true;
        }
    }
}