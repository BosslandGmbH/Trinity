using log4net;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Bot.Coroutines;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines
{
    /// <summary>
    /// Pickup items within range.
    /// </summary>
    public class VacuumItems
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();

        /* Should be able to wait for finishing this task While doing something.
         * Otherwise we see the bot locks in dilemma between going to Quest/Rift
         * or Vacuuming nearby items -Seq */
        public static async Task<bool> Execute()
        {
            if (!ZetaDia.IsInGame ||
                ZetaDia.Globals.IsLoadingWorld ||
                ZetaDia.Globals.IsPlayingCutscene ||
                ZetaDia.IsInTown)
            {
                return true;
            }

            // A check for is casting.
            if (ZetaDia.Me.LoopingAnimationEndTime > 0)
                return true;

            // Items that shouldn't be picked up are currently excluded from cache.
            // a pickup evaluation should be added here if that changes.
            var currentPickup = Core.Targets.OfType<TrinityItem>().FirstOrDefault(i => i.Distance < 8f && i.ActorSnoId != 0);
            // When no item is inside the vacuum range of 8 just continue.
            if (currentPickup == null)
                return true;

            // Collect information about the item to pickup.
            var logLine =
                $"[{nameof(VacuumItems)}] {currentPickup.Name} ({currentPickup.ActorSnoId}) InternamName={currentPickup.InternalName} GbId={currentPickup.GameBalanceId}";

            // Use the coroutine to pick up the item.
            if (await CommonCoroutines.MoveAndInteract(
                    currentPickup.ToDiaObject(),
                    () => currentPickup.ActorSnoId == 0) == CoroutineResult.Running)
            {
                return false;
            }

            // The item was picked up trigger RecordSpell and log the info collected above.
            SpellHistory.RecordSpell(SNOPower.Axe_Operate_Gizmo);
            s_logger.Info(logLine);

            // We are not done yet so return false.
            return false;
        }
    }
}
