using System;
using Trinity.Framework.Objects.Attributes;
using Trinity.Framework.Objects.Memory.Attributes;
using Trinity.Technicals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.Game.Internals.SNO;

namespace Trinity.Cache.Properties
{
    /// <summary>
    /// PropertyLoader that are specific to gizmos
    /// </summary>
    public class GizmoProperties : PropertyLoader.IPropertyCollection
    {
        private DateTime _lastUpdated = DateTime.MinValue;
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(100);

        public void ApplyTo(TrinityCacheObject target)
        {
            if (DateTime.UtcNow.Subtract(_lastUpdated).TotalMilliseconds > UpdateInterval.TotalMilliseconds)
                Update(target);

            target.IsUsed = this.IsUsed;
            target.IsGroundClicky = this.IsGroundClicky;
            target.IsWeaponRack = this.IsWeaponRack;
            target.IsCorpse = this.IsCorpse;
            target.IsChest = this.IsChest;
            target.IsUntargetable = this.IsUntargetable;
            target.IsRareChest = this.IsRareChest;
            target.IsGizmoDisabledByScript = this.IsGizmoDisabledByScript;
            target.IsPlayerHeadstone = this.IsPlayerHeadstone;
        }

        public void OnCreate(TrinityCacheObject source)
        {
            if (source.ActorType != ActorType.Gizmo || !source.IsValid)
                return;

            var gizmo = source.Gizmo;
            if (gizmo == null)
                return;

            this.IsPlayerHeadstone = source.ActorSNO == DataDictionary.PLAYER_HEADSTONE_SNO;
            this.IsRareChest = source.InternalNameLowerCase.Contains("chest_rare") || DataDictionary.ResplendentChestIds.Contains(source.ActorSNO);
            this.IsChest = (!this.IsRareChest && source.InternalNameLowerCase.Contains("chest")) || DataDictionary.ContainerWhiteListIds.Contains(source.ActorSNO);
            this.IsCorpse = source.InternalNameLowerCase.Contains("corpse");
            this.IsWeaponRack = source.InternalNameLowerCase.Contains("rack");
            this.IsGroundClicky = source.InternalNameLowerCase.Contains("ground_clicky");

            var commonData = source.CommonData;
            if (commonData == null)
                return;

            this.IsUntargetable = source.ActorAttributes.IsUntargetable && !DataDictionary.IgnoreUntargettableAttribute.Contains(source.ActorSNO);
            this.IsInvulnerable = source.ActorAttributes.IsInvulnerable;
        }

        public void Update(TrinityCacheObject source)
        {
            _lastUpdated = DateTime.UtcNow;

            if (source.ActorType != ActorType.Gizmo || !source.IsValid)
                return;

            var gizmo = source.Gizmo;
            if (gizmo == null)
                return;

            this.IsUsed = GizmoPropertyUtils.IsGizmoUsed(source);
            this.IsGizmoDisabledByScript = gizmo.IsGizmoDisabledByScript;

        }

        public bool IsGroundClicky { get; set; }
        public bool IsWeaponRack { get; set; }
        public bool IsCorpse { get; set; }
        public bool IsChest { get; set; }
        public bool IsUntargetable { get; set; }
        public object IsInvulnerable { get; set; }
        public bool IsRareChest { get; set; }
        public bool IsGizmoDisabledByScript { get; set; }
        public bool IsUsed { get; set; }
        public bool IsPlayerHeadstone { get; set; }
    }



    public class GizmoPropertyUtils
    {
        public static bool IsGizmoUsed(TrinityCacheObject actor)
        {
            if (actor.ActorType != ActorType.Gizmo || !actor.IsValid)
                return false;

            try
            {
                // Treat all NPCIsOperatable inside rift as used? (cow Lord event only quest scenario inside rift)
                if (RiftProgression.IsInRift && actor.IsNPC && actor.CommonData != null && actor.CommonData.GetAttribute<int>(ActorAttributeType.NPCIsOperatable) > 0)
                {
                    /* Cow lord
                     480: NPCIsOperatable (-3616) i:1 f:0 Value=1 IsValid=True 
                     479: IsNPC (-3617) i:1 f:0 Value=1 IsValid=True 
                     124: HitpointsMaxTotal (-3972) i:0 f:2.053896E+09 Value=2053896000 IsValid=True 
                     122: HitpointsMax (-3974) i:0 f:2.053896E+09 Value=2053896000 IsValid=True 
                     119: HitpointsTotalFromLevel (-3977) i:0 f:0 Value=0 IsValid=True 
                     115: HitpointsCur (-3981) i:0 f:2.053896E+09 Value=2053896000 IsValid=True 
                     103: TeamId (-3993) i:1 f:0 Value=1 IsValid=True 
                     57: Level (-4039) i:70 f:0 Value=70 IsValid=True 
                     482: NPCHasInteractOptions (-3614) i:1 f:0 Value=1 IsValid=True 
                     483: ConversationIcon (-3613) i:1 f:0 Value=1 IsValid=True 
                    */

                    /* Locked gate for cow lord event. closed state.
                    [Trinity 2.41.95] -- Dumping Attribtues for trDun_Cath_Gate_D_UdderChaos-261211 (Sno=375568 Ann=-1366490265) at <1057.776, 898.3652, 0.3411865> ----
                    [Trinity 2.41.95] Attributes (9): 
                     467: GizmoHasBeenOperated (-3629) i:1 f:0 Value=1 IsValid=True 
                     460: DoorTimer (-3636) i:520283 f:0 Value=520283 IsValid=True 
                     456: GizmoState (-3640) i:0 f:0 Value=0 IsValid=True 
                     124: HitpointsMaxTotal (-3972) i:0 f:1 Value=1 IsValid=True 
                     122: HitpointsMax (-3974) i:0 f:0.0009994507 Value=0 IsValid=True 
                     119: HitpointsTotalFromLevel (-3977) i:0 f:0 Value=0 IsValid=True 
                     115: HitpointsCur (-3981) i:0 f:0.001 Value=0 IsValid=True 
                     103: TeamId (-3993) i:1 f:0 Value=1 IsValid=True 
                     57: Level (-4039) i:70 f:0 Value=70 IsValid=True */

                    return false;
                }

                /* A5 gate Closed State
                [Trinity 2.41.92] -- Dumping Attribtues for X1_Westm_Door_Giant_Lowering_Wolf-128134 (Sno=308241 Ann=1826030664) at <299.6067, 971.1008, 0.3713183> ----
                [Trinity 2.41.92] Attributes (7): 
                 456: GizmoState (-3640) i:0 f:0 Value=0 IsValid=True 
                 124: HitpointsMaxTotal (-3972) i:0 f:1 Value=1 IsValid=True 
                 122: HitpointsMax (-3974) i:0 f:0.0009994507 Value=0 IsValid=True 
                 119: HitpointsTotalFromLevel (-3977) i:0 f:0 Value=0 IsValid=True 
                 115: HitpointsCur (-3981) i:0 f:0.001 Value=0 IsValid=True 
                 103: TeamId (-3993) i:1 f:0 Value=1 IsValid=True 
                 57: Level (-4039) i:70 f:0 Value=70 IsValid=True                     

                [Trinity 2.41.92] -- Dumping Attribtues for X1_Westm_Door_Giant_Lowering_Wolf-128134 (Sno=308241 Ann=1826030664) at <299.6067, 971.1008, 0.3713183> ----
                [Trinity 2.41.92] Attributes (10): 
                 467: GizmoHasBeenOperated (-3629) i:1 f:0 Value=1 IsValid=True 
                 462: GizmoOperatorACDId (-3634) i:2019950762 f:0 Value=2019951000 IsValid=True 
                 460: DoorTimer (-3636) i:452227 f:0 Value=452227 IsValid=True 
                 456: GizmoState (-3640) i:1 f:0 Value=1 IsValid=True 
                 124: HitpointsMaxTotal (-3972) i:0 f:1 Value=1 IsValid=True 
                 122: HitpointsMax (-3974) i:0 f:0.0009994507 Value=0 IsValid=True 
                 119: HitpointsTotalFromLevel (-3977) i:0 f:0 Value=0 IsValid=True 
                 115: HitpointsCur (-3981) i:0 f:0.001 Value=0 IsValid=True 
                 103: TeamId (-3993) i:1 f:0 Value=1 IsValid=True 
                 57: Level (-4039) i:70 f:0 Value=70 IsValid=True 
                */

                int endAnimation;
                if (actor.Type == TrinityObjectType.Interactable &&
                    DataDictionary.InteractEndAnimations.TryGetValue(actor.ActorSNO, out endAnimation) &&
                    endAnimation == (int)actor.Animation)
                    return true;

                if (actor.GizmoType == GizmoType.None)
                    return true;

                var commonData = actor.CommonData;
                if (commonData != null && commonData.IsValid && !commonData.IsDisposed)
                {
                    if (commonData.GizmoState == 1)
                        return true;

                    if (actor.ActorAttributes.GizmoState == 1)
                        return true;

                    if (commonData.GizmoOperatorACDId > 0)
                        return true;

                    if (commonData.GizmoHasBeenOperated > 0)
                        return true;
                }
                else
                {
                    return true;
                }

                switch (actor.GizmoType)
                {
                    case GizmoType.BreakableChest:
                    case GizmoType.LoreChest:
                    case GizmoType.Chest:

                        if (actor.GizmoType == GizmoType.Chest && actor.CommonData.ChestOpen > 0)
                            return true;

                        break;
                    case GizmoType.HealingWell:

                        if (commonData.GizmoCharges > 0)
                            return false;
                        break;                    
                }

                if (actor.Type == TrinityObjectType.Barricade)
                {
                    if (commonData.NoDamage > 0)
                    {
                        return true;
                    }
                }

                var lootContainer = actor.Gizmo as GizmoLootContainer;
                if (lootContainer != null && lootContainer.IsOpen)
                    return true;

                var untargetable = commonData.Untargetable > 0;
                var invulnerable = commonData.Invulnerable > 0;

                var gizmoDestructible = actor.Gizmo as GizmoDestructible;
                if (gizmoDestructible != null && (gizmoDestructible.HitpointsCurrent <= 0 || invulnerable || untargetable))
                    return true;

                var gizmoDoor = actor.Gizmo as GizmoDoor;
                if (gizmoDoor != null && gizmoDoor.IsLocked)
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
            }
            catch (Exception)
            {
                Logger.LogVerbose("Exception in GetIsGizmoUsed");
            }
            return false;
        }

    }
}




