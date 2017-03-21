using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Reference;
using Zeta.Bot.Coroutines;
using Zeta.Game;
using Zeta.Game.Internals;


namespace Trinity.Components.Coroutines.Town
{
    public class ReturnToStash
    {
        public static bool StartedOutOfTown { get; set; }

        public static async Task<bool> Execute()
        {
            if (ZetaDia.Me.IsInCombat)
            {
                Core.Logger.Debug("[ReturnToStash] Cannot return to stash while in combat");
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
                    if (!await MoveToAndInteract.Execute(TownInfo.Stash.GetActor()))
                    {
                        return true;
                    }
                }
            }
            return true;
        }
    }
}