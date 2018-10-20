using Trinity.Framework;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.QuestTools;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("TrinityTownPortal")]
    [XmlElement("TownPortal")]
    public class TownPortalTag : BaseProfileBehavior
    {
        private ISubroutine _clearAreaTask;

        public override async Task<bool> StartTask()
        {
            CreateClearAreaTask();
            return false;
        }

        private void CreateClearAreaTask()
        {
            _clearAreaTask = new ClearAreaForNSecondsCoroutine(QuestId, 5, 0, 0, 10);
        }

        public override async Task<bool> MainTask()
        {
            if (!UIElements.BackgroundScreenPCButtonRecall.IsEnabled)
            {
                Core.Logger.Log("Not portaling because its not possibel right now");
                return true;
            }

            if (!_clearAreaTask.IsDone && !await _clearAreaTask.GetCoroutine())
                return false;

            if (Core.Player.IsTakingDamage)
            {
                Core.Logger.Log("Taking damage, reverting to clear area.");
                CreateClearAreaTask();
                return false;
            }

            if (ZetaDia.IsInTown)
            {
                Core.Logger.Log("Not portaling because we're already in town.");
                return true;
            }

            return await GoToTown();
        }

        public static async Task<bool> GoToTown()
        {
            Navigator.PlayerMover.MoveStop();
            await Coroutine.Wait(2000, () => !ZetaDia.Me.Movement.IsMoving);

            Core.Logger.Warn("Casting town portal");

            if (!ZetaDia.IsInTown && !ZetaDia.Globals.IsLoadingWorld)
            {
                ZetaDia.Me.UseTownPortal();
            }

            await Coroutine.Wait(5000, () => !Core.CastStatus.StoneOfRecall.IsCasting && !ZetaDia.IsInTown);
            return true;
        }
    }
}
