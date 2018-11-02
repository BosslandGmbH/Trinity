using Buddy.Coroutines;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines
{
    public class MoveToAndInteract
    {
        /// <summary>
        /// Moves to something and interacts with it
        /// </summary>
        /// <param name="obj">object to interact with</param>
        /// <param name="interactLimit">maximum number of times to interact</param>
        public static async Task<bool> Execute(DiaObject obj, int interactLimit = 5)
        {
            if (obj == null)
                return false;

            if (!obj.IsFullyValid())
                return false;

            if (interactLimit < 1)
                interactLimit = 5;

            if (Core.Player.IsInTown)
                GameUI.CloseVendorWindow();

            return await CommonCoroutines.MoveAndInteract(obj, () => !IsInteracting &&
                                                                     interactLimit-- > 0) == CoroutineResult.Running;
        }

        public static async Task<bool> Execute(IFindable actor, int interactLimit = 5)
        {
            return await Execute(actor.Position, actor.ActorId, interactLimit);
        }

        /// <summary>
        /// Moves to a position, finds actor by Id and interacts with it
        /// </summary>
        /// <param name="actorId">id of actor to interact with</param>
        /// <param name="position">position from which to interact</param>
        /// <param name="interactLimit">maximum number of times to interact</param>
        public static async Task<bool> Execute(Vector3 position, int actorId, int interactLimit = 5)
        {
            if (position == Vector3.Zero)
                return false;

            if (interactLimit < 1)
                interactLimit = 5;

            if (Core.Player.IsInTown)
                GameUI.CloseVendorWindow();

            DiaObject actor;
            while ((actor = ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                           .FirstOrDefault(a => a.ActorSnoId == actorId)
                   ) == null &&
                   await CommonCoroutines.MoveAndStop(position, 40f, "Close to target") != MoveResult.ReachedDestination)
            {
                await Coroutine.Yield();
            }

            if (actor != null)
                return await Execute(actor, interactLimit);

            Core.Logger.Verbose("Interaction Failed: Actor not found with Id={0}", actorId);
            return false;
        }

        private static bool IsInteracting => ZetaDia.Me.LoopingAnimationEndTime > 0 ||
                                             s_castingAnimationStates.Contains(ZetaDia.Me.CommonData.AnimationState);

        private static readonly HashSet<AnimationState> s_castingAnimationStates = new HashSet<AnimationState>
        {
            AnimationState.Channeling,
            AnimationState.Casting,
        };
    }
}
