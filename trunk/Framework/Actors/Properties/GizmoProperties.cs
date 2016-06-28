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
            actor.IsUsed = IsUsed(actor);
        }

        //public static void UpdateGizmoProperties(TrinityActor target, DiaGizmo diaGizmo)
        //{
        //    // properties that were already updated by common properties
        //    var actorSno = target.ActorSnoId;
        //    var type = target.Type;
        //    var annId = target.AnnId;
        //    var rActorGuid = target.RActorId;
        //    var attributes = target.ActorAttributes;
        //    var internalName = target.InternalName;
        //    var internalNameLowerCase = target.InternalNameLowerCase;

        //    if (diaGizmo == null || !diaGizmo.IsValid || attributes == null)
        //        return;

        //    var commonData = diaGizmo.CommonData;
        //    if (commonData == null || !commonData.IsValid || commonData.IsDisposed)
        //        return;

        //    target.IsPlayerHeadstone = actorSno == DataDictionary.PLAYER_HEADSTONE_SNO;
        //    target.IsRareChest = internalNameLowerCase.Contains("chest_rare") || DataDictionary.ResplendentChestIds.Contains(actorSno);
        //    target.IsChest = (!target.IsRareChest && internalNameLowerCase.Contains("chest")) || DataDictionary.ContainerWhiteListIds.Contains(actorSno);
        //    target.IsCorpse = internalNameLowerCase.Contains("corpse");
        //    target.IsWeaponRack = internalNameLowerCase.Contains("rack");
        //    target.IsGroundClicky = internalNameLowerCase.Contains("ground_clicky");
        //    target.IsContainer = target.IsRareChest || target.IsChest || target.IsCorpse || target.IsWeaponRack || target.IsGroundClicky;
        //    target.IsCursedChest = type == TrinityObjectType.CursedChest;
        //    target.IsCursedShrine = type == TrinityObjectType.CursedShrine;
        //    target.IsDestroyable = type == TrinityObjectType.Barricade || type == TrinityObjectType.Destructible;
        //    target.IsEventObject = target.IsCursedChest || target.IsCursedShrine;
        //    target.IsUntargetable = attributes.IsUntargetable && !DataDictionary.IgnoreUntargettableAttribute.Contains(actorSno);
        //    target.IsInvulnerable = attributes.IsInvulnerable;
        //    target.IsInteractableType = DataDictionary.InteractableTypes.Contains(type);
        //    target.IsUsed = IsUsed(target);
        //}

        public static bool IsUsed(TrinityActor actor)
        {
            var attributes = actor.Attributes;
            if (attributes != null)
            {
                if (attributes.IsDoorLocked)
                    return true;

                if (attributes.GizmoState == 1)
                    return true;

                if (attributes.GizmoOperatorACDId > 0)
                    return true;

                if (attributes.IsGizmoBeenOperated)
                    return true;

                if (attributes.IsGizmoDisabledByScript)
                    return true;

                if (attributes.GizmoCharges > 0)
                    return true;

                if (attributes.IsChestOpen)
                    return true;

                if (attributes.GizmoCharges > 0)
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

        //public static bool IsGizmoUsed(TrinityActor actor)
        //{
        //    // todo: this method neds to be refactored

        //    if (actor.ActorType != ActorType.Gizmo || !actor.IsValid)
        //        return false;

        //    try
        //    {
        //        // Treat all NPCIsOperatable inside rift as used? (cow Lord event only quest scenario inside rift)
        //        if (RiftProgression.IsInRift && actor.IsNPC && actor.CommonData != null && actor.CommonData.GetAttribute<int>(ActorAttributeType.NPCIsOperatable) > 0)
        //        {
        //            /* Cow lord
        //             480: NPCIsOperatable (-3616) i:1 f:0 Value=1 IsValid=True 
        //             479: IsNPC (-3617) i:1 f:0 Value=1 IsValid=True 
        //             124: HitpointsMaxTotal (-3972) i:0 f:2.053896E+09 Value=2053896000 IsValid=True 
        //             122: HitpointsMax (-3974) i:0 f:2.053896E+09 Value=2053896000 IsValid=True 
        //             119: HitpointsTotalFromLevel (-3977) i:0 f:0 Value=0 IsValid=True 
        //             115: HitpointsCur (-3981) i:0 f:2.053896E+09 Value=2053896000 IsValid=True 
        //             103: TeamId (-3993) i:1 f:0 Value=1 IsValid=True 
        //             57: Level (-4039) i:70 f:0 Value=70 IsValid=True 
        //             482: NPCHasInteractOptions (-3614) i:1 f:0 Value=1 IsValid=True 
        //             483: ConversationIcon (-3613) i:1 f:0 Value=1 IsValid=True 
        //            */

        //            /* Locked gate for cow lord event. closed state.
        //            [Trinity 2.41.95] -- Dumping Attribtues for trDun_Cath_Gate_D_UdderChaos-261211 (Sno=375568 Ann=-1366490265) at <1057.776, 898.3652, 0.3411865> ----
        //            [Trinity 2.41.95] Attributes (9): 
        //             467: GizmoHasBeenOperated (-3629) i:1 f:0 Value=1 IsValid=True 
        //             460: DoorTimer (-3636) i:520283 f:0 Value=520283 IsValid=True 
        //             456: GizmoState (-3640) i:0 f:0 Value=0 IsValid=True 
        //             124: HitpointsMaxTotal (-3972) i:0 f:1 Value=1 IsValid=True 
        //             122: HitpointsMax (-3974) i:0 f:0.0009994507 Value=0 IsValid=True 
        //             119: HitpointsTotalFromLevel (-3977) i:0 f:0 Value=0 IsValid=True 
        //             115: HitpointsCur (-3981) i:0 f:0.001 Value=0 IsValid=True 
        //             103: TeamId (-3993) i:1 f:0 Value=1 IsValid=True 
        //             57: Level (-4039) i:70 f:0 Value=70 IsValid=True */

        //            return false;
        //        }

        //        /* A5 gate Closed State
        //        [Trinity 2.41.92] -- Dumping Attribtues for X1_Westm_Door_Giant_Lowering_Wolf-128134 (Sno=308241 Ann=1826030664) at <299.6067, 971.1008, 0.3713183> ----
        //        [Trinity 2.41.92] Attributes (7): 
        //         456: GizmoState (-3640) i:0 f:0 Value=0 IsValid=True 
        //         124: HitpointsMaxTotal (-3972) i:0 f:1 Value=1 IsValid=True 
        //         122: HitpointsMax (-3974) i:0 f:0.0009994507 Value=0 IsValid=True 
        //         119: HitpointsTotalFromLevel (-3977) i:0 f:0 Value=0 IsValid=True 
        //         115: HitpointsCur (-3981) i:0 f:0.001 Value=0 IsValid=True 
        //         103: TeamId (-3993) i:1 f:0 Value=1 IsValid=True 
        //         57: Level (-4039) i:70 f:0 Value=70 IsValid=True                     

        //        [Trinity 2.41.92] -- Dumping Attribtues for X1_Westm_Door_Giant_Lowering_Wolf-128134 (Sno=308241 Ann=1826030664) at <299.6067, 971.1008, 0.3713183> ----
        //        [Trinity 2.41.92] Attributes (10): 
        //         467: GizmoHasBeenOperated (-3629) i:1 f:0 Value=1 IsValid=True 
        //         462: GizmoOperatorACDId (-3634) i:2019950762 f:0 Value=2019951000 IsValid=True 
        //         460: DoorTimer (-3636) i:452227 f:0 Value=452227 IsValid=True 
        //         456: GizmoState (-3640) i:1 f:0 Value=1 IsValid=True 
        //         124: HitpointsMaxTotal (-3972) i:0 f:1 Value=1 IsValid=True 
        //         122: HitpointsMax (-3974) i:0 f:0.0009994507 Value=0 IsValid=True 
        //         119: HitpointsTotalFromLevel (-3977) i:0 f:0 Value=0 IsValid=True 
        //         115: HitpointsCur (-3981) i:0 f:0.001 Value=0 IsValid=True 
        //         103: TeamId (-3993) i:1 f:0 Value=1 IsValid=True 
        //         57: Level (-4039) i:70 f:0 Value=70 IsValid=True 
        //        */

        //        int endAnimation;
        //        if (actor.Type == TrinityObjectType.Interactable &&
        //            DataDictionary.InteractEndAnimations.TryGetValue(actor.ActorSnoId, out endAnimation) &&
        //            endAnimation == (int) actor.Animation)
        //            return true;

        //        if (actor.GizmoType == GizmoType.None)
        //            return true;

        //        var commonData = actor.CommonData;
        //        if (commonData != null && commonData.IsValid && !commonData.IsDisposed)
        //        {
        //            if (commonData.GizmoState == 1)
        //                return true;

        //            if (actor.ActorAttributes.GizmoState == 1)
        //                return true;

        //            if (commonData.GizmoOperatorACDId > 0)
        //                return true;

        //            if (commonData.GizmoHasBeenOperated > 0)
        //                return true;
        //        }
        //        else
        //        {
        //            return true;
        //        }

        //        switch (actor.GizmoType)
        //        {
        //            case GizmoType.BreakableChest:
        //            case GizmoType.LoreChest:
        //            case GizmoType.Chest:

        //                if (actor.GizmoType == GizmoType.Chest && actor.CommonData.ChestOpen > 0)
        //                    return true;

        //                break;
        //            case GizmoType.HealingWell:

        //                if (commonData.GizmoCharges > 0)
        //                    return false;
        //                break;
        //        }

        //        if (actor.Type == TrinityObjectType.Barricade)
        //        {
        //            if (commonData.NoDamage > 0)
        //            {
        //                return true;
        //            }
        //        }

        //        var lootContainer = actor.Gizmo as GizmoLootContainer;
        //        if (lootContainer != null && lootContainer.IsOpen)
        //            return true;

        //        var untargetable = commonData.Untargetable > 0;
        //        var invulnerable = commonData.Invulnerable > 0;

        //        var gizmoDestructible = actor.Gizmo as GizmoDestructible;
        //        if (gizmoDestructible != null && (gizmoDestructible.HitpointsCurrent <= 0 || invulnerable || untargetable))
        //            return true;

        //        var gizmoDoor = actor.Gizmo as GizmoDoor;
        //        if (gizmoDoor != null && gizmoDoor.IsLocked)
        //            return true;

        //        if (actor.Type == TrinityObjectType.Door || actor.Type == TrinityObjectType.Container || actor.Type == TrinityObjectType.Interactable)
        //        {
        //            var currentAnimation = actor.AnimationNameLowerCase;

        //            if (currentAnimation.Contains("irongate") && currentAnimation.Contains("open"))
        //                return false;

        //            if (currentAnimation.Contains("_dead"))
        //                return true;

        //            if (currentAnimation.Contains("irongate") && currentAnimation.Contains("idle"))
        //                return true;

        //            if (currentAnimation.EndsWith("open") || currentAnimation.EndsWith("opening"))
        //                return true;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        Logger.LogVerbose("Exception in GetIsGizmoUsed");
        //    }
        //    return false;
        //}


    }
}




