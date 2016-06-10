using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Cache;
using Trinity.Framework;
using Trinity.Framework.Objects.Memory.Attributes;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Technicals.Logger;

namespace Trinity
{
    public class TrinityCacheObject
    {
        private ActorAttributes _actorAttributes;
        private ACD _commonData;
        private DiaGizmo _gizmo;
        private DiaItem _item;
        private DiaObject _object;        
        private DiaUnit _unit;
        public DateTime CreationTime = DateTime.UtcNow;
        public bool IsACDBased;

        public TrinityCacheObject() { }

        public TrinityCacheObject(DiaObject diaObject)
        {

            if (diaObject != null && diaObject.IsValid && (diaObject.CommonData == null || diaObject.CommonData.IsValid))
            {
                Object = diaObject;

                InternalName = diaObject.Name;
                ActorType = diaObject.ActorType;
                Position = diaObject.Position;
                ActorSNO = diaObject.ActorSnoId;
                Radius = diaObject.CollisionSphere.Radius;
                CollisionRadius = diaObject.ActorInfo.AxialCylinder.Ax1*0.2f;
                RActorGuid = diaObject.RActorId;
                ACDGuid = diaObject.ACDId;

                CommonData = diaObject.CommonData;
                if (CommonData != null && CommonData.IsValid && !CommonData.IsDisposed)
                {
                    var unit = diaObject as DiaUnit;
                    if (unit != null)
                    {
                        HitPoints = unit.HitpointsCurrent;
                        RiftProgression.SetRiftValue(this);
                    }

                    IsACDBased = true;
                    IsElite = CommonData.IsElite;
                    Animation = CommonData.CurrentAnimation;
                    GizmoType = CommonData.GizmoType;

                    if (IsElite)
                    {
                        Affixes = GetMonsterAffixes(CommonData.Affixes);
                    }
                    else
                    {
                        Affixes = new HashSet<TrinityMonsterAffix>();
                    }
                }

                ObjectType = TypeMapper.GetObjectType(this);
            }
        }

        
        public DiaObject Object
        {
            get { return _object ?? (_object = ZetaDia.Actors.GetActorsOfType<DiaObject>(true, true).FirstOrDefault(o => o.RActorId == RActorGuid)); }
            set { _object = value; }
        }
        
        public DiaUnit Unit
        {
            get
            {
                if (_unit != null && _unit.IsFullyValid())
                    return _unit;
                if (Object != null && Object.IsFullyValid() && Object is DiaUnit)
                {
                    _unit = (DiaUnit) Object;
                    return _unit;
                }
                return default(DiaUnit);
            }
        }
        
        public DiaGizmo Gizmo
        {
            get
            {
                if (_gizmo != null && _unit.IsFullyValid())
                    return _gizmo;
                if (Object != null && Object.IsFullyValid() && Object is DiaGizmo)
                {
                    _gizmo = (DiaGizmo) Object;
                    return _gizmo;
                }
                return default(DiaGizmo);
            }
        }
        
        public DiaItem Item
        {
            get
            {
                if (_item != null && _unit.IsFullyValid())
                    return _item;
                if (Object != null && Object.IsFullyValid() && Object is DiaItem)
                {
                    _item = (DiaItem) Object;
                    return _item;
                }
                return default(DiaItem);
            }
        }

        
        public ACD CommonData
        {
            get
            {
                if (_object == null)
                    return null;

                if (_commonData != null && _commonData.IsValid)
                    return _commonData;

                return _commonData = _object.CommonData;
            }
            set { _commonData = value; }
        }
       
        public double RiftValuePct { get; set; }



        #region Cached Properties

        public int AnnId { get; set; }
        public bool IsWaitSpot { get; set; }
        public double RiftValueInRadius { get; set; }
        public bool IsIllusion { get; set; }
        public HashSet<TrinityMonsterAffix> Affixes { get; set; }
        public AnimationState AnimationState { get; internal set; }        
        public bool IsHostile { get; internal set; }
        public ObjectType ObjectType { get; set; }
        public bool IsSummoned { get; internal set; }
        public int ACDGuid { get; set; }
        public int RActorGuid { get; set; }
        public int ActorSNO { get; set; }
        public TrinityObjectType Type { get; set; }
        public ActorType ActorType { get; set; }
        public GizmoType GizmoType { get; set; }        
        public double Weight { get; set; }        
        public string WeightInfo { get; set; }        
        public int BoundToACD { get; set; }        
        public Vector3 Position { get; set; }        
        public float Distance { get; set; }        
        public string InternalName { get; set; } = string.Empty;
        public SNOAnim Animation { get; set; }        
        public int DynamicID { get; set; }        
        public int GameBalanceID { get; set; }       
        public int ItemLevel { get; set; }        
        public string ItemLink { get; set; }       
        public int GoldAmount { get; set; }        
        public bool OneHanded { get; set; }       
        public bool TwoHanded { get; set; }        
        public ItemQuality ItemQuality { get; set; }       
        public ItemBaseType DBItemBaseType { get; set; }       
        public ItemType DBItemType { get; set; }        
        public FollowerType FollowerType { get; set; }        
        public TrinityItemType ItemType { get; set; }        
        public DateTime LastSeenTime { get; set; }        
        public bool IsElite { get; set; }        
        public bool IsInvulnerable { get; set; }        
        public bool IsRare { get; set; }        
        public bool IsUnique { get; set; }        
        public bool IsMinion { get; set; }        
        public MonsterAffixes MonsterAffixes { get; set; }        
        public bool IsTreasureGoblin { get; set; }        
        public bool IsEliteRareUnique { get; set; }        
        public bool IsBoss { get; set; }        
        public bool IsAncient { get; set; }        
        public bool HasAffixShielded { get; set; }        
        public bool IsAttackable { get; set; }        
        public bool HasDotDPS { get; set; }        
        public double HitPointsPct { get; set; }        
        public double HitPoints { get; set; }        
        public float Radius { get; set; }        
        public float Rotation { get; set; }        
        public Vector2 DirectionVector { get; set; }        
        public bool IsFacingPlayer { get; set; }        
        public bool HasBeenPrimaryTarget { get; set; }        
        public int TimesBeenPrimaryTarget { get; set; }        
        public string ObjectHash { get; set; }        
        public double KillRange { get; set; }
        public MonsterSize MonsterSize { get; set; }
        public bool IsUsed => GetIsGizmoUsed(this);        
        public bool HasBeenNavigable { get; set; }        
        public bool HasBeenRaycastable { get; set; }        
        public bool HasBeenInLoS { get; set; }     
        public bool IsSummonedByPlayer { get; set; }        
        public bool IsSummoner { get; set; }        
        public int SummonedByACDId { get; set; }                
        public bool IsNPC { get; set; }
        public bool NPCIsOperable { get; set; }
        public bool IsQuestMonster { get; set; }
        public bool IsMinimapActive { get; set; }
        public bool IsBountyObjective { get; set; }
        public bool IsQuestGiver { get; set; }
        public string IgnoreReason { get; set; }
        public string ExtraInfo { get; set; }
        public float CollisionRadius { get; set; }
        public bool IsMarker { get; set; }
        public bool IsSafeSpot { get; set; }
        public double AvoidanceHealth { get; set; }
        public float AvoidanceRadius { get; set; }
        public bool IsSpawning { get; set; }
        public string AnimationNameLowerCase { get; set; }
        public string InternalNameLowerCase { get; set; }
        public MonsterType MonsterType { get; set; }
        public int TeamId { get; set; }
        public MonsterQuality MonsterQuality { get; set; }

        #endregion

        #region Expressions

        public bool IsBossOrEliteRareUnique => (IsUnit && (IsEliteRareUnique || IsBoss));
        public bool IsTrashMob => (IsUnit && !(IsEliteRareUnique || IsBoss || IsTreasureGoblin || IsMinion));
        public bool IsMe => RActorGuid == TrinityPlugin.Player.RActorGuid;
        public bool IsUnit => Type == TrinityObjectType.Unit || ObjectType == ObjectType.Unit;
        public bool IsPlayer => Type == TrinityObjectType.Player || ObjectType == ObjectType.Player;
        public bool IsGizmo => ActorType == ActorType.Gizmo;
        public bool IsMonster => ActorType == ActorType.Monster;
        public bool IsBlacklisted => GenericBlacklist.ContainsKey(ObjectHash);
        public bool IsAvoidance => Core.Avoidance.ActiveAvoidanceIds.Contains(ActorSNO);
        public bool IsCursedChest => Type == TrinityObjectType.CursedChest;
        public bool IsCursedShrine => Type == TrinityObjectType.CursedShrine;
        public bool IsEventObject => IsCursedChest || IsCursedShrine;
        public bool IsDead => IsUnit && HitPoints <= 0;
        public bool IsDestroyable => Type == TrinityObjectType.Barricade || Type == TrinityObjectType.Destructible;
        public bool IsHidden => IsValid && (CommonData.Hidden > 0 || ActorAttributes.IsBurrowed);
        public bool IsInteractableType => DataDictionary.InteractableTypes.Contains(Type);
        public bool IsValid => ActorType == ActorType.ClientEffect || CommonData != null && CommonData.IsValid;
        public bool IsPickupNoClick => DataDictionary.NoPickupClickItemTypes.Contains(ItemType) || DataDictionary.NoPickupClickTypes.Contains(Type);
        public bool IsStandingInAvoidance => CacheData.TimeBoundAvoidance.Any(a => a.Position.Distance(Position) <= a.Radius);
        public bool CanWalkTo => Core.Avoidance.Grid.CanRayWalk(TrinityPlugin.Player.Position, Position);
        public bool CanCastTo => Core.Avoidance.Grid.CanRayCast(TrinityPlugin.Player.Position, Position);
        public bool IsChampion => MonsterQuality == MonsterQuality.Champion;
        public float ZDiff => Math.Abs(Position.Z - TrinityPlugin.Player.Position.Z);
        public float RadiusDistance => Math.Max(Distance - Radius, 0f);
        public ActorAttributes ActorAttributes => _actorAttributes ?? (_actorAttributes = new ActorAttributes(CommonData.FastAttribGroupId));
        public string Flags => string.Join(", ", Core.Avoidance.Grid.GetAvoidanceFlags(Position));


        #endregion

        #region Helper Methods

        [Obsolete("Use CanCastTo or CanWalkTo instead (Navigator.Raycast == CanWalkTo)")]
        public bool IsInLineOfSight => !Navigator.Raycast(TrinityPlugin.Player.Position, Position);

        public bool IsFacing(Vector3 targetPosition, float arcDegrees = 70f)
        {
            return TargetUtil.IsFacing(this, targetPosition, arcDegrees);
        }
        
        public bool IsPlayerFacing(float arc)
        {
            return TrinityPlugin.Player.IsFacing(Position, arc);
        }

        public int NearbyUnitsWithinDistance(float range = 5f)
        {
            return TargetUtil.NearbyUnitsWithinDistance(this, range);
        }

        public int CountUnitsBehind(float range)
        {
            return TargetUtil.CountUnitsBehind(this, range);
        }

        public int CountUnitsInFront()
        {
            return TargetUtil.CountUnitsInFront(this);
        }

        public bool HasDebuff(SNOPower debuffSNO)
        {
            try
            {
                //These are the debuffs we've seen so far
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff0VisualEffect & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff0VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff0VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff0VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff0VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff0VisualEffectD & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff0VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff1VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff1VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff2VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff2VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff3VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff3VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff4VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff4VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff4VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff5VisualEffectC & 0xFFF)) == 1)
                    return true;

                //These are here just in case
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff1VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff1VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff1VisualEffectD & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff1VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff2VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff2VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff2VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff2VisualEffectD & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff3VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff3VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff3VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) debuffSNO << 12) + ((int) ActorAttributeType.PowerBuff3VisualEffectD & 0xFFF)) == 1)
                    return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public bool HasReflectDamage()
        {
            try
            {
                if (CommonData.GetAttribute<int>(((int) SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int) ActorAttributeType.PowerBuff0VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int) ActorAttributeType.PowerBuff1VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int) ActorAttributeType.PowerBuff2VisualEffectNone & 0xFFF)) == 1)
                    return true;

                // Probably only this one is needed
                if (CommonData.GetAttribute<int>(((int) SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int) ActorAttributeType.PowerBuff3VisualEffectNone & 0xFFF)) == 1)
                    return true;

                if (CommonData.GetAttribute<int>(((int) SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int) ActorAttributeType.PowerBuff4VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int) SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int) ActorAttributeType.PowerBuff5VisualEffectNone & 0xFFF)) == 1)
                    return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        public override string ToString()
        {
            return $"{InternalName}, Type={Type} Dist={RadiusDistance} IsBossOrEliteRareUnique={IsBossOrEliteRareUnique}";
        }

        public static bool GetIsGizmoUsed(TrinityCacheObject actor)
        {
            try
            {
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


                    return false;
                }

                int endAnimation;
                if (actor.ObjectType == ObjectType.Interactable &&
                    DataDictionary.InteractEndAnimations.TryGetValue(actor.ActorSNO, out endAnimation) &&
                    endAnimation == (int) actor.Animation)
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
                }

                var lootContainer = actor.Gizmo as GizmoLootContainer;
                if (lootContainer != null && lootContainer.IsOpen)
                    return true;

                var gizmoDestructible = actor.Gizmo as GizmoDestructible;
                if (gizmoDestructible != null && gizmoDestructible.HitpointsCurrent <= 0)
                    return true;

                var destructibleContainer = actor.Gizmo as GizmoDestructibleLootContainer;
                if (destructibleContainer != null && destructibleContainer.HitpointsCurrent <= 0)
                    return true;

                if (actor.ObjectType == ObjectType.Door || actor.ObjectType == ObjectType.Container || actor.ObjectType == ObjectType.Interactable)
                {
                    var currentAnimation = actor.Animation.ToString().ToLower();

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
                Logger.Log("Exception in GetIsGizmoUsed");
            }
            return false;
        }
    
        public static HashSet<TrinityMonsterAffix> GetMonsterAffixes(int[] sourceAffixes)
        {
            var affixes = new HashSet<TrinityMonsterAffix>();

            if (!sourceAffixes.Any())
                return affixes;

            foreach (var sourceAffix in sourceAffixes)
            {
                if (sourceAffix != -1)
                    affixes.Add((TrinityMonsterAffix) sourceAffix);
            }

            return affixes;
        }

        #endregion
    }
}