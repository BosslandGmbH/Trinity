using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.DbProvider;
using Trinity.Helpers;
using TrinityCoroutines;
using TrinityCoroutines.Resources;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.Game.Internals;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Coroutines.Town
{
    public class ReturnToStash
    {
        public static bool StartedOutOfTown { get; set; }

        public static async Task<bool> Execute()
        {
            if (ZetaDia.Me.IsInCombat)
            {
                Logger.LogDebug("[ReturnToStash] Cannot return to stash while in combat");
                return false;
            }
            if (!ZetaDia.IsInTown && ZetaDia.Me.IsFullyValid() && !ZetaDia.Me.IsInCombat && UIElements.BackgroundScreenPCButtonRecall.IsEnabled)
            {
                StartedOutOfTown = true;
                await CommonCoroutines.UseTownPortal("[ReturnToStash] Returning to stash");
                return true;
            }

            if (!GameUI.IsElementVisible(GameUI.StashDialogMainPage) && ZetaDia.IsInTown)
            {
                GameUI.CloseVendorWindow();

                // Move to Stash
                if (TownRun.StashLocation.Distance(ZetaDia.Me.Position) > 10f)
                {
                    await MoveTo.Execute(TownRun.StashLocation, "Shared Stash");
                    return true;
                }
                if (TownRun.StashLocation.Distance(ZetaDia.Me.Position) <= 10f && TownRun.SharedStash == null)
                {
                    Logger.LogError("Shared Stash actor is null!");
                    return false;
                }

                // Open Stash
                if (TownRun.StashLocation.Distance(ZetaDia.Me.Position) <= 10f && TownRun.SharedStash != null && !GameUI.IsElementVisible(GameUI.StashDialogMainPage))
                {
                    while (ZetaDia.Me.Movement.IsMoving)
                    {
                        Navigator.PlayerMover.MoveStop();
                        await Coroutine.Yield();
                    }
                    Logger.Log("Opening Stash");
                    TownRun.SharedStash.Interact();
                    await Coroutine.Sleep(200);
                    await Coroutine.Yield();
                    if (GameUI.IsElementVisible(GameUI.StashDialogMainPage))
                        return true;
                    return true;
                }
            }
            return true;
        }

    }
}

