using Trinity.Framework.Actors.ActorTypes;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors.Properties
{
    public class GizmoProperties
    {
        public static void Populate(TrinityActor actor)
        {
            if (actor.ActorType != ActorType.Gizmo)
                return;

            if (!actor.IsAcdBased || !actor.IsAcdValid)
                return;

            actor.IsPlayerHeadstone = actor.ActorSnoId == DataDictionary.PLAYER_HEADSTONE_SNO;
            actor.IsRareChest = actor.InternalNameLowerCase.Contains("chest_rare") || DataDictionary.ResplendentChestIds.Contains(actor.ActorSnoId);
            actor.IsChest = (!actor.IsRareChest && actor.InternalNameLowerCase.Contains("chest")) || DataDictionary.ContainerWhiteListIds.Contains(actor.ActorSnoId);
            actor.IsCorpse = actor.InternalNameLowerCase.Contains("corpse");
            actor.IsWeaponRack = actor.InternalNameLowerCase.Contains("rack");
            actor.IsGroundClicky = actor.InternalNameLowerCase.Contains("ground_clicky");
            actor.IsContainer = actor.IsRareChest || actor.IsChest || actor.IsCorpse || actor.IsWeaponRack || actor.IsGroundClicky;
            actor.IsCursedChest = actor.Type == TrinityObjectType.CursedChest;
            actor.IsCursedShrine = actor.Type == TrinityObjectType.CursedShrine;
            actor.IsDestroyable = actor.Type == TrinityObjectType.Barricade || actor.Type == TrinityObjectType.Destructible;
            actor.IsEventObject = actor.IsCursedChest || actor.IsCursedShrine;
            actor.IsInteractableType = DataDictionary.InteractableTypes.Contains(actor.Type);
            actor.IsUntargetable = actor.Attributes.IsUntargetable && !DataDictionary.IgnoreUntargettableAttribute.Contains(actor.ActorSnoId);
            actor.IsInvulnerable = actor.Attributes.IsInvulnerable;
            actor.IsUsed = GetIsGizmoUsed(actor);
        }

        public static bool GetIsGizmoUsed(TrinityActor actor)
        {
            var attributes = actor.Attributes;
            if (attributes != null)
            {
                if (attributes.IsGizmoDisabledByScript)
                    return true;

                if (attributes.IsDoorLocked)
                    return true;

                if (attributes.GizmoCharges > 0)
                    return false;

                if (attributes.GizmoState == 1 && !attributes.IsQuestMonster)
                    return true;

                if (attributes.GizmoOperatorACDId > 0)
                    return true;

                if (attributes.IsGizmoBeenOperated)
                    return true;

                if (attributes.IsChestOpen)
                    return true;

                if (actor.Type == TrinityObjectType.Barricade && attributes.IsNoDamage)
                    return true;

                if ((actor.Type == TrinityObjectType.Destructible || actor.Type == TrinityObjectType.Barricade) && actor.IsUntargetable || actor.IsInvulnerable)
                    return true;
            }

            int endAnimation;
            if (actor.IsInteractableType && DataDictionary.InteractEndAnimations.TryGetValue(actor.ActorSnoId, out endAnimation)
                && endAnimation == (int)actor.Animation)
                return true;

            if (actor.Type == TrinityObjectType.Door || actor.Type == TrinityObjectType.Container || actor.Type == TrinityObjectType.Interactable)
            {
                var currentAnimation = actor.AnimationNameLowerCase;

                if (currentAnimation.Contains("irongate") && currentAnimation.Contains("open"))
                    return false;

                if (currentAnimation.Contains("_dead"))
                    return true;

                if (currentAnimation.Contains("irongate") && currentAnimation.Contains("idle"))
                    return true;

                if (currentAnimation.EndsWith("open") || currentAnimation.EndsWith("opening"))
                    return true;
            }

            return false;
        }

    }
}




