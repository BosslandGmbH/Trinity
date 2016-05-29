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

                if (TownInfo.Stash.Distance > 10f)
                {
                    if(!await MoveToAndInteract.Execute(TownInfo.Stash.GetActor()));
                    {
                        return true;
                    }
                }
            }
            return true;
        }

    }
}

