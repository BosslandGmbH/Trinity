using System;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
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

            var attributes = actor.Attributes;
            var commonData = actor.CommonData;
            var rActor = actor.RActor;

            actor.IsPlayerHeadstone = actor.ActorSnoId == GameData.PLAYER_HEADSTONE_SNO;
            actor.IsRareChest = actor.InternalNameLowerCase.Contains("chest_rare") || GameData.ResplendentChestIds.Contains(actor.ActorSnoId);
            actor.IsCorpse = actor.InternalNameLowerCase.Contains("corpse");
            actor.IsWeaponRack = actor.InternalNameLowerCase.Contains("rack");
            actor.IsGroundClicky = actor.InternalNameLowerCase.Contains("ground_clicky");
            actor.IsContainer = actor.IsRareChest || actor.IsChest || actor.IsCorpse || actor.IsWeaponRack || actor.IsGroundClicky;
            actor.IsCursedChest = actor.Type == TrinityObjectType.CursedChest;
            actor.IsCursedShrine = actor.Type == TrinityObjectType.CursedShrine;
            actor.IsChest = actor.IsCursedChest || actor.IsRareChest || actor.InternalNameLowerCase.Contains("chest") || GameData.ContainerWhiteListIds.Contains(actor.ActorSnoId);
            actor.IsDestroyable = actor.Type == TrinityObjectType.Barricade || actor.Type == TrinityObjectType.Destructible;
            actor.IsEventObject = actor.IsCursedChest || actor.IsCursedShrine;
            actor.IsInteractableType = GameData.InteractableTypes.Contains(actor.Type);
            actor.IsUntargetable = actor.Attributes.IsUntargetable && !GameData.IgnoreUntargettableAttribute.Contains(actor.ActorSnoId);
            actor.IsInvulnerable = actor.Attributes.IsInvulnerable;
            actor.IsUsed = GetIsGizmoUsed(actor);
            actor.IsLockedDoor = actor.Attributes.IsDoorLocked || actor.Attributes.IsDoorTimed;
            actor.ShrineType = GetShrineType(actor);
            actor.ContainerType = GetContainerType(actor);

            var movement = rActor.Movement;
            if (movement != null && movement.IsValid)
            {
                actor.Rotation = movement.Rotation;
                actor.RotationDegrees = MathEx.ToDegrees(actor.Rotation);
                actor.DirectionVector = movement.DirectionVector;
                actor.IsMoving = movement.IsMoving;
                actor.MovementSpeed = movement.SpeedXY;
            }
        }

        public static ShrineTypes GetShrineType(TrinityActor cacheObject)
        {
            switch (cacheObject.ActorSnoId)
            {
                case (int)SNOActor.a4_Heaven_Shrine_Global_Fortune:
                case (int)SNOActor.Shrine_Global_Fortune:
                    return ShrineTypes.Fortune;

                case (int)SNOActor.a4_Heaven_Shrine_Global_Frenzied:
                case (int)SNOActor.Shrine_Global_Frenzied:
                    return ShrineTypes.Frenzied;

                case (int)SNOActor.a4_Heaven_Shrine_Global_Reloaded:
                case (int)SNOActor.Shrine_Global_Reloaded:
                    return ShrineTypes.RunSpeed;

                case (int)SNOActor.a4_Heaven_Shrine_Global_Enlightened:
                case (int)SNOActor.Shrine_Global_Enlightened:
                    return ShrineTypes.Enlightened;

                case (int)SNOActor.Shrine_Global_Glow:
                    return ShrineTypes.Glow;

                case (int)SNOActor.a4_Heaven_Shrine_Global_Hoarder:
                case (int)SNOActor.Shrine_Global_Hoarder:
                    return ShrineTypes.Hoarder;

                case (int)SNOActor.x1_LR_Shrine_Infinite_Casting:
                    return ShrineTypes.Casting;

                case (int)SNOActor.x1_LR_Shrine_Electrified_TieredRift:
                case (int)SNOActor.x1_LR_Shrine_Electrified:
                    return ShrineTypes.Conduit;

                case (int)SNOActor.x1_LR_Shrine_Invulnerable:
                    return ShrineTypes.Shield;

                case (int)SNOActor.x1_LR_Shrine_Run_Speed:
                    return ShrineTypes.Shield;

                case (int)SNOActor.x1_LR_Shrine_Damage:
                    return ShrineTypes.Damage;

                case (int)SNOActor.Shrine_TreasureGoblin:
                    return ShrineTypes.Goblin;

                default:
                    return ShrineTypes.None;
            }
        }

        public static ContainerTypes GetContainerType(TrinityActor cacheObject)
        {
            if (cacheObject.IsRareChest || cacheObject.IsChest)
                return ContainerTypes.NormalChest;

            if (cacheObject.IsWeaponRack)
                return ContainerTypes.WeaponRack;

            if (cacheObject.IsGroundClicky)
                return ContainerTypes.GroundClicky;

            if (cacheObject.IsCorpse)
                return ContainerTypes.Corpse;

            if (cacheObject.IsContainer)
                return ContainerTypes.Other;

            return ContainerTypes.None;
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

                if (actor.IsUnit && attributes.GizmoState == 1 && !attributes.IsQuestMonster)
                    return true;

                if (attributes.GizmoOperatorACDId > 0)
                    return true;

                if (attributes.IsGizmoBeenOperated)
                    return true;

                if (attributes.IsChestOpen)
                    return true;

                if (actor.IsChest && attributes.GizmoState == 1)
                    return false;

                if (actor.GizmoType == GizmoType.PowerUp && attributes.GizmoState == 1)
                    return true;

                //if (actor.GizmoType == GizmoType.Door && attributes.GizmoState == 1)
                //    return false;

                if (attributes.GizmoState == 1)
                    return true;

                if (actor.Type == TrinityObjectType.Barricade && attributes.IsNoDamage)
                    return true;

                if (actor.Type == TrinityObjectType.Destructible || actor.Type == TrinityObjectType.Barricade)
                {
                    if (actor.IsUntargetable || actor.IsInvulnerable || Math.Abs(actor.HitPointsMax - actor.HitPoints) > 0.0001)
                         return true;

                    //if (attributes.IsDeletedOnServer)
                    //    return true;
                }
            }

            int endAnimation;
            if (actor.IsInteractableType && GameData.InteractEndAnimations.TryGetValue(actor.ActorSnoId, out endAnimation)
                && endAnimation == (int)actor.Animation)
                return true;

            switch (actor.Type)
            {
                case TrinityObjectType.Door:
                case TrinityObjectType.Container:
                case TrinityObjectType.Interactable:
                case TrinityObjectType.Destructible:
                    var currentAnimation = actor.AnimationNameLowerCase;

                    if (currentAnimation.Contains("irongate") && currentAnimation.Contains("open"))
                        return false;

                    if (currentAnimation.Contains("_dead"))
                        return true;

                    if (currentAnimation.Contains("irongate") && currentAnimation.Contains("idle"))
                        return true;

                    if (currentAnimation.EndsWith("open") || currentAnimation.EndsWith("opening"))
                        return true;

                    break;
            }

            return false;
        }

    }
}




