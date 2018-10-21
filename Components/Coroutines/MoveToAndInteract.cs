using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;


namespace Trinity.Components.Coroutines
{
    public class MoveToAndInteract
    {
        /// <summary>
        /// Moves to something and interacts with it
        /// </summary>
        /// <param name="obj">object to interact with</param>
        /// <param name="range">how close to get</param>
        /// <param name="interactLimit">maximum number of times to interact</param>
        public static async Task<bool> Execute(DiaObject obj, float range = -1f, int interactLimit = 5)
        {
            if (obj == null)
                return false;

            if (!obj.IsFullyValid())
                return false;

            if (interactLimit < 1) interactLimit = 5;
            if (range < 0) range = obj.InteractDistance;

            if (Core.Player.IsInTown)
                GameUI.CloseVendorWindow();

            if (obj.Position.Distance(ZetaDia.Me.Position) > range)
            {
                Navigator.PlayerMover.MoveTowards(obj.Position);

                if (!await MoveTo.Execute(obj.Position, obj.Name))
                {
                    Core.Logger.Log("MoveTo Failed for {0} ({1}) Distance={2}", obj.Name, obj.ActorSnoId, obj.Distance);
                    return false;
                }
            }

            var distance = obj.Position.Distance(ZetaDia.Me.Position);
            if (distance <= range || distance - obj.InteractDistance <= range)
            {
                for (var i = 1; i <= interactLimit; i++)
                {
                    Core.Logger.Verbose("Interacting with {0} ({1}) Attempt={2}", obj.Name, obj.ActorSnoId, i);
                    if (obj.Interact() && i > 1)
                        break;

                    await Coroutine.Yield();
                }
            }

            // Better to be redundant than failing to interact.

            Navigator.PlayerMover.MoveTowards(obj.Position);
            await Coroutine.Yield();
            obj.Interact();

            Navigator.PlayerMover.MoveStop();
            await Coroutine.Yield();
            await Interact(obj);
            return true;
        }

        public static async Task<bool> Execute(IFindable actor, float radiusDistanceRequired = -1f, int interactLimit = 5)
        {
            return await Execute(actor.Position, actor.ActorId, radiusDistanceRequired, interactLimit);
        }

        /// <summary>
        /// Moves to a position, finds actor by Id and interacts with it
        /// </summary>
        /// <param name="actorId">id of actor to interact with</param>
        /// <param name="radiusDistanceRequired">how close to get</param>
        /// <param name="position">position from which to interact</param>
        /// <param name="interactLimit">maximum number of times to interact</param>
        public static async Task<bool> Execute(Vector3 position, int actorId, float radiusDistanceRequired = -1f, int interactLimit = 5)
        {
            if (position == Vector3.Zero)
                return false;

            if (interactLimit < 1) interactLimit = 5;

            if (radiusDistanceRequired < 0) radiusDistanceRequired = 4f;

            if (Core.Player.IsInTown)
                GameUI.CloseVendorWindow();

            Navigator.PlayerMover.MoveTowards(position);

            // First get close enough to the target position that we can be sure of finding the actor if it exists.

            if (position.Distance(ZetaDia.Me.Position) > radiusDistanceRequired)
            {
                if (!await MoveTo.Execute(position, position.ToString(), 12f))
                {
                    Core.Logger.Log("MoveTo Failed for {0} Distance={1}", position, position.Distance(ZetaDia.Me.Position));
                    return false;
                }
            }

            //// Find the actor by Id

            //ZetaDia.Actors.Update();

            var actor = ZetaDia.Actors.GetActorsOfType<DiaObject>(true).FirstOrDefault(a => a.ActorSnoId == actorId);
            if (actor == null)
            {
                Core.Logger.Verbose("Interaction Failed: Actor not found with Id={0}", actorId);
                return false;
            }

            var actorName = actor.Name;
            var realRadius = Math.Max(actor.ActorInfo.Sphere.Radius, 1) * 0.35;
            var playerRealRadius = ZetaDia.Me.ActorInfo.Sphere.Radius * 0.35;
            var distance = position.Distance(ZetaDia.Me.Position);
            var radiusDistance = distance - realRadius - playerRealRadius;
            var distanceToGo = radiusDistance - radiusDistanceRequired;

            Core.Logger.Verbose("Actor found '{0}' ({1}) at {2} distance: {3} realRadius={4} rDistance={5} distanceTo: {6} reqRDistance={7}",
                actor.Name, actor.ActorSnoId, actor.Position, actor.Distance, realRadius, radiusDistance, distanceToGo, radiusDistanceRequired);

            // Now make sure we're close enough to interact.
            if (distanceToGo > 0)
            {
                if (!await MoveTo.Execute(position, position.ToString(), (float)(distance - distanceToGo)))
                {
                    Core.Logger.Log("MoveTo Failed for {0} Distance={1}", position, position.Distance(ZetaDia.Me.Position));
                    return false;
                }
            }

            // Do the interaction
            var startingWorldId = ZetaDia.Globals.WorldSnoId;
            if (distance <= Math.Max(actor.InteractDistance, 10f))
            {
                Navigator.PlayerMover.MoveStop();

                for (var i = 1; i <= interactLimit; i++)
                {
                    if (ZetaDia.Globals.WorldSnoId != startingWorldId)
                        return true;

                    Core.Logger.Log("Interacting with {0} ({1}) Attempt={2}", actorName, actorId, i);
                    if (actor.Interact() && i > 1)
                        break;

                    await Coroutine.Yield();

                    if (IsInteracting)
                        break;
                }
            }

            await Coroutine.Yield();

            // Better to be redundant than failing to interact.

            if (!IsInteracting)
            {
                Navigator.PlayerMover.MoveTowards(actor.Position);
                await Coroutine.Yield();
                actor.Interact();
            }

            if (!IsInteracting)
            {
                Navigator.PlayerMover.MoveStop();
                await Coroutine.Yield();
                await Interact(actor);
            }

            return true;
        }

        private static bool IsInteracting => ZetaDia.Me.LoopingAnimationEndTime > 0 || CastingAnimationStates.Contains(ZetaDia.Me.CommonData.AnimationState);

        private static readonly HashSet<AnimationState> CastingAnimationStates = new HashSet<AnimationState>
        {
            AnimationState.Channeling,
            AnimationState.Casting,
        };

        private static async Task<bool> Interact(DiaObject actor)
        {
            if (!actor.IsFullyValid())
                return false;

            // Doubly-make sure we interact
            var retVal = actor.Interact();
            await Coroutine.Yield();
            return retVal;
        }
    }
}
