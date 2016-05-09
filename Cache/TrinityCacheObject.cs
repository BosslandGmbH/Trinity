using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web.UI;
using Adventurer.Game.Exploration;
using Trinity.Cache;
using Trinity.Config.Combat;
using Trinity.Framework;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Helpers;
using Trinity.Objects;
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
    // TrinityCacheObject type used to cache all data
    // Let's me create an object list with ONLY the data I need read from D3 memory, and then read from this while
    // Handling movement and interaction with the target - whether the target is a shrine, an item or a monster
    // Completely minimizing the D3 memory reads to the bare minimum
    [DataContract]
    public class TrinityCacheObject : IActor
    {
        [NoCopy]
        private DiaObject _object;
        [NoCopy]
        public DiaObject Object
        {
            get
            {
                if (_object == null)
                {
                    _object = ZetaDia.Actors.GetActorsOfType<DiaObject>(true, true).FirstOrDefault(o => o.RActorId == RActorGuid);
                }
                return _object;
            }
            set { _object = value; }
        }

        [NoCopy]
        private DiaUnit _unit;
        [NoCopy]
        public DiaUnit Unit
        {
            get
            {
                if (_unit != null && _unit.IsFullyValid())
                    return _unit;
                if (Object != null && Object.IsFullyValid() && Object is DiaUnit)
                {
                    _unit = Object as DiaUnit;
                    return _unit;
                }
                return default(DiaUnit);
            }
        }

        [NoCopy]
        private DiaGizmo _gizmo;
        [NoCopy]
        public DiaGizmo Gizmo
        {
            get
            {
                if (_gizmo != null && _unit.IsFullyValid())
                    return _gizmo;
                if (Object != null && Object.IsFullyValid() && Object is DiaGizmo)
                {
                    _gizmo = Object as DiaGizmo;
                    return _gizmo;
                }
                return default(DiaGizmo);
            }
        }

        [NoCopy]
        private DiaItem _item;
        [NoCopy]
        public DiaItem Item
        {
            get
            {
                if (_item != null && _unit.IsFullyValid())
                    return _item;
                if (Object != null && Object.IsFullyValid() && Object is DiaItem)
                {
                    _item = Object as DiaItem;
                    return _item;
                }
                return default(DiaItem);
            }
        }

        [NoCopy]
        private ACD _commonData;
        [NoCopy]
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

        [NoCopy]
        public bool CommonDataIsValid
        {
            get
            {
                return IsFullyValid();
            }
        }

        private double? _riftValuePct;
        public double RiftValuePct
        {
            get
            {
                if (_riftValuePct != null) return _riftValuePct.Value;

                if (IsIllusion)
                {
                    _riftValuePct = 0;
                    return 0;
                }

                if (IsBoss && !IsSummoned)
                {
                    _riftValuePct = 10d;
                    return _riftValuePct.Value;
                }

                if (IsMinion)
                {
                    _riftValuePct = 0.25d;
                    return _riftValuePct.Value;
                }

                if (IsBossOrEliteRareUnique)
                {
                    _riftValuePct = 1d;
                    return _riftValuePct.Value;
                }

                var baseValue = RiftProgression.Values.ContainsKey(ActorSNO) ? RiftProgression.Values[ActorSNO] : -1d;
                var finalValue = IsBossOrEliteRareUnique ? baseValue * 4 : baseValue;
                if (finalValue < 0)
                    finalValue = 0;

                _riftValuePct = finalValue;
                return finalValue;
            }
        }

        [DataMember]
        public int AnnId { get; set; }

        [DataMember]
        public bool IsWaitSpot { get; set; }

        [DataMember]
        public double RiftValueInRadius { get; set; }

        [DataMember]
        public bool IsIllusion { get; set; }

        [DataMember]
        public HashSet<TrinityMonsterAffix> Affixes { get; set; }

        [DataMember]
        public AnimationState AnimationState { get; internal set; }

        [DataMember]
        public bool IsHostile { get; internal set; }

        [DataMember]
        public ObjectType ObjectType { get; set; }

        [DataMember]
        public bool IsSummoned { get; internal set; }

        [DataMember]
        public int ACDGuid { get; set; }

        [DataMember]
        public int RActorGuid { get; set; }

        [DataMember]
        public int ActorSNO { get; set; }

        [DataMember]
        public TrinityObjectType Type { get; set; }

        [DataMember]
        public ActorType ActorType { get; set; }

        [DataMember]
        public GizmoType GizmoType { get; set; }

        [DataMember]
        public double Weight { get; set; }

        [DataMember]
        public string WeightInfo { get; set; }

        [DataMember]
        public int BoundToACD { get; set; }


        [DataMember]
        public Vector3 Position { get; set; }

        public int WorldSnoId { get; }

        [DataMember]
        public AABB AABBBounds { get; set; }

        [DataMember]
        public float Distance { get; set; }

        [NoCopy]
        public float RadiusDistance { get { return Math.Max(Distance - Radius, 0f); } }

        [DataMember]
        public string InternalName { get; set; } = string.Empty;

        public SNOAnim Animation { get; set; }

        [DataMember]
        public int DynamicID { get; set; }

        [DataMember]
        public int GameBalanceID { get; set; }

        [DataMember]
        public int ItemLevel { get; set; }

        [DataMember]
        public string ItemLink { get; set; }

        [DataMember]
        public int GoldAmount { get; set; }

        [DataMember]
        public bool OneHanded { get; set; }

        [DataMember]
        public bool TwoHanded { get; set; }

        [DataMember]
        public ItemQuality ItemQuality { get; set; }

        [DataMember]
        public ItemBaseType DBItemBaseType { get; set; }

        [DataMember]
        public ItemType DBItemType { get; set; }

        [DataMember]
        public FollowerType FollowerType { get; set; }

        [DataMember]
        public TrinityItemType TrinityItemType { get; set; }

        [DataMember]
        public DateTime LastSeenTime { get; set; }

        [DataMember]
        public bool IsElite { get; set; }

        [DataMember]
        public bool IsInvulnerable { get; set; }

        [DataMember]
        public bool IsRare { get; set; }

        [DataMember]
        public bool IsUnique { get; set; }

        [DataMember]
        public bool IsMinion { get; set; }

        [DataMember]
        public MonsterAffixes MonsterAffixes { get; set; }

        [DataMember]
        public bool IsTreasureGoblin { get; set; }

        [DataMember]
        public bool IsEliteRareUnique { get; set; }

        [DataMember]
        public bool IsBoss { get; set; }

        [DataMember]
        public bool IsAncient { get; set; }

        [DataMember]
        public bool HasAffixShielded { get; set; }

        [DataMember]
        public bool IsAttackable { get; set; }

        [DataMember]
        public bool HasDotDPS { get; set; }

        [DataMember]
        public double HitPointsPct { get; set; }

        [DataMember]
        public double HitPoints { get; set; }

        [DataMember]
        public float Radius { get; set; }

        [DataMember]
        public float Rotation { get; set; }

        [DataMember]
        public Vector2 DirectionVector { get; set; }

        /// <summary>
        /// If unit is facing player
        /// </summary>
        [DataMember]
        public bool IsFacingPlayer { get; set; }

        /// <summary>
        /// If Player is facing unit
        /// </summary>
        [DataMember]
        public bool ForceLeapAgainst { get; set; }

        [DataMember]
        public bool HasBeenPrimaryTarget { get; set; }

        [DataMember]
        public int TimesBeenPrimaryTarget { get; set; }

        [DataMember]
        public DateTime FirstTargetAssignmentTime { get; set; }

        [DataMember]
        public string ObjectHash { get; set; }

        [DataMember]
        public double KillRange { get; set; }

        public MonsterSize MonsterSize { get; set; }

        public bool IsUsed
        {
            get { return IActor.IsUsed; }
        }

        [DataMember]
        public bool HasBeenNavigable { get; set; }

        [DataMember]
        public bool HasBeenRaycastable { get; set; }

        [DataMember]
        public bool HasBeenInLoS { get; set; }

        [NoCopy]
        public bool IsBossOrEliteRareUnique { get { return (IsUnit && (IsEliteRareUnique || IsBoss)); } }

        [NoCopy]
        public bool IsTrashMob { get { return (IsUnit && !(IsEliteRareUnique || IsBoss || IsTreasureGoblin || IsMinion)); } }

        [NoCopy]
        public bool IsMe { get { return RActorGuid == TrinityPlugin.Player.RActorGuid; } }

        [DataMember]
        public bool IsSummonedByPlayer { get; set; }

        [DataMember]
        public bool IsSummoner { get; set; }

        [DataMember]
        public int SummonedByACDId { get; set; }

        [NoCopy]
        public bool IsUnit { get { return this.Type == TrinityObjectType.Unit || this.ObjectType == ObjectType.Unit; } }

        [DataMember]
        public bool IsNPC { get; set; }

        [DataMember]
        public bool IsPlayer { get { return this.Type == TrinityObjectType.Player || this.ObjectType == ObjectType.Player; } }

        [DataMember]
        public bool NPCIsOperable { get; set; }

        [DataMember]
        public bool IsQuestMonster { get; set; }

        [DataMember]
        public bool IsMinimapActive { get; set; }

        [DataMember]
        public bool IsBountyObjective { get; set; }

        [DataMember]
        public bool IsQuestGiver { get; set; }

        [DataMember]
        public string IgnoreReason { get; set; }

        [DataMember]
        public string ExtraInfo { get; set; }

        [NoCopy]
        public bool IsCursedChest { get { return Type == TrinityObjectType.CursedChest; } }

        [NoCopy]
        public bool IsCursedShrine { get { return Type == TrinityObjectType.CursedShrine; } }

        [NoCopy]
        public bool IsEventObject { get { return IsCursedChest || IsCursedShrine; } }

        [NoCopy]
        public bool IsDestroyable
        {
            get
            {
                return Type == TrinityObjectType.Barricade || Type == TrinityObjectType.Destructible;
            }
        }

        [NoCopy]
        public bool IsInteractable
        {
            get
            {
                return Type == TrinityObjectType.Item || Type == TrinityObjectType.Container || Type == TrinityObjectType.CursedChest || Type == TrinityObjectType.CursedChest || Type == TrinityObjectType.CursedShrine ||
                    Type == TrinityObjectType.Door || Type == TrinityObjectType.HealthWell || Type == TrinityObjectType.Interactable || Type == TrinityObjectType.Shrine;
            }
        }

        public bool IsPickupNoClick
        {
            get
            {
                return Type == TrinityObjectType.Gold || Type == TrinityObjectType.PowerGlobe || Type == TrinityObjectType.HealthGlobe || Type == TrinityObjectType.ProgressionGlobe;
            }
        }

        [NoCopy]
        public bool IsFacing(Vector3 targetPosition, float arcDegrees = 70f)
        {
            if (DirectionVector != Vector2.Zero)
            {
                Vector3 u = targetPosition - this.Position;
                u.Z = 0f;
                Vector3 v = new Vector3(DirectionVector.X, DirectionVector.Y, 0f);
                bool result = ((MathEx.ToDegrees(Vector3.AngleBetween(u, v)) <= arcDegrees) ? 1 : 0) != 0;
                return result;
            }
            else
                return false;
        }

        [NoCopy]
        public bool IsPlayerFacing(float arc)
        {
            return TrinityPlugin.Player.IsFacing(this.Position, arc);
        }

        [NoCopy]
        public bool IsStandingInAvoidance
        {
            get
            {
                return CacheData.TimeBoundAvoidance.Any(a => a.Position.Distance(this.Position) <= a.Radius);
            }
        }


        public DateTime CreationTime = DateTime.MinValue;
        public bool IsACDBased;

        public TrinityCacheObject()
        {
            CreationTime = DateTime.UtcNow;
        }

        public TrinityCacheObject(DiaObject diaObject)
        {
            CreationTime = DateTime.UtcNow;

            if (diaObject != null && diaObject.IsValid && (diaObject.CommonData == null || diaObject.CommonData.IsValid))
            {
                Object = diaObject;

                this.InternalName = diaObject.Name;
                this.ActorType = diaObject.ActorType;
                this.Position = diaObject.Position;
                this.ActorSNO = diaObject.ActorSnoId;
                this.Radius = diaObject.CollisionSphere.Radius;
                this.CollisionRadius = diaObject.ActorInfo.AxialCylinder.Ax1 * 0.2f;
                this.RActorGuid = diaObject.RActorId;
                this.ACDGuid = diaObject.ACDId;

                CommonData = diaObject.CommonData;
                if (CommonData != null && CommonData.IsValid && !CommonData.IsDisposed)
                {
                    var unit = diaObject as DiaUnit;
                    if (unit != null)
                    {
                        this.HitPoints = unit.HitpointsCurrent;
                    }

                    this.IsACDBased = true;
                    this.IsElite = CommonData.IsElite;
                    this.Animation = CommonData.CurrentAnimation;
                    this.GizmoType = CommonData.GizmoType;

                    if (this.IsElite)
                    {
                        this.Affixes = GetMonsterAffixes(CommonData.Affixes);
                    }
                    else
                    {
                        this.Affixes = new HashSet<TrinityMonsterAffix>();
                    }
                }

                this.ObjectType = TypeMapper.GetObjectType(this);
            }
        }

        public float CollisionRadius { get; set; }

        public int NearbyUnitsWithinDistance(float range = 5f)
        {
            using (new PerformanceLogger("CacheObject.UnitsNear"))
            {
                if (this.Type != TrinityObjectType.Unit)
                    return 0;

                return TrinityPlugin.ObjectCache
                    .Count(u => u.RActorGuid != this.RActorGuid && u.IsUnit && u.Position.Distance(this.Position) <= range && u.HasBeenInLoS);
            }
        }

        public int CountUnitsBehind(float range)
        {
            return
                (from u in TrinityPlugin.ObjectCache
                 where u.RActorGuid != this.RActorGuid &&
                 u.IsUnit &&
                 MathUtil.IntersectsPath(this.Position, this.Radius, TrinityPlugin.Player.Position, u.Position)
                 select u).Count();
        }

        public int CountUnitsInFront()
        {
            return
                (from u in TrinityPlugin.ObjectCache
                 where u.RActorGuid != RActorGuid &&
                 u.IsUnit &&
                 MathUtil.IntersectsPath(u.Position, u.Radius, TrinityPlugin.Player.Position, Position)
                 select u).Count();
        }

        public bool HasDebuff(SNOPower debuffSNO)
        {
            try
            {
                //These are the debuffs we've seen so far
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffect & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectD & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff4VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff4VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff4VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff5VisualEffectC & 0xFFF)) == 1)
                    return true;

                //These are here just in case
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectD & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectE & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectD & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectA & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectB & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectC & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)debuffSNO << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectD & 0xFFF)) == 1)
                    return true;



            }
            catch (Exception) { }
            return false;
        }

        public bool HasReflectDamage()
        {
            try
            {
                if (CommonData.GetAttribute<int>(((int)SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int)ActorAttributeType.PowerBuff0VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int)ActorAttributeType.PowerBuff1VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int)ActorAttributeType.PowerBuff2VisualEffectNone & 0xFFF)) == 1)
                    return true;

                // Probably only this one is needed
                if (CommonData.GetAttribute<int>(((int)SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int)ActorAttributeType.PowerBuff3VisualEffectNone & 0xFFF)) == 1)
                    return true;

                if (CommonData.GetAttribute<int>(((int)SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int)ActorAttributeType.PowerBuff4VisualEffectNone & 0xFFF)) == 1)
                    return true;
                if (CommonData.GetAttribute<int>(((int)SNOPower.MonsterAffix_ReflectsDamageCast << 12) + ((int)ActorAttributeType.PowerBuff5VisualEffectNone & 0xFFF)) == 1)
                    return true;
            }
            catch (Exception) { }
            return false;
        }

        public override string ToString()
        {
            return string.Format("{0}, Type={1} Dist={2} IsBossOrEliteRareUnique={3} IsAttackable={4}", InternalName, Type, RadiusDistance, IsBossOrEliteRareUnique, IsAttackable);
        }

        public bool IsMarker { get; set; }

        /// <summary>
        /// Determines whether [is in line of sight].
        /// </summary>
        /// <returns><c>true</c> if [is in line of sight]; otherwise, <c>false</c>.</returns>
        public bool IsInLineOfSight()
        {
            return !Navigator.Raycast(TrinityPlugin.Player.Position, Position);
        }

        public bool IsFullyValid()
        {
            return _object != null && _object.CommonData != null && _object.IsValid && _object.CommonData.IsValid;
        }

        public bool IsSafeSpot { get; set; }

        public double AvoidanceHealth { get; set; }

        public float AvoidanceRadius { get; set; }

        public bool IsSpawning { get; set; }
        public string AnimationNameLowerCase { get; set; }
        public string InternalNameLowerCase { get; set; }
        public MonsterType MonsterType { get; set; }


        #region Implementation of IActor

        public IActor IActor
        {
            get { return this; }
        }

        DiaObject IActor.DiaObject
        {
            get { return Object; }
            set { Object = value; }
        }

        ACDItem IActor.ACDItem
        {
            get { return CommonData as ACDItem; }
            set { CommonData = value; }
        }

        DiaGizmo IActor.DiaGizmo
        {
            get { return Object as DiaGizmo; }
            set { Object = value; }
        }

        DiaItem IActor.DiaItem
        {
            get { return Object as DiaItem; }
            set { Object = value; }
        }

        DiaUnit IActor.DiaUnit
        {
            get { return Object as DiaUnit; }
            set { Object = value; }
        }

        float IActor.RadiusDistance
        {
            get { return RadiusDistance; }
            set { }
        }

        bool IActor.IsMonster
        {
            get { return ActorType == ActorType.Monster; }
            set { }
        }

        bool IActor.IsGizmo
        {
            get { return ActorType == ActorType.Gizmo; }
            set { }
        }

        bool IActor.IsItem
        {
            get { return ActorType == ActorType.Item; }
            set { }
        }

        bool IActor.IsUsed
        {
            get { return CommonDataIsValid && GetIsGizmoUsed(this); }
            set { }
        }

        bool IActor.IsUsable
        {
            get { return DataDictionary.InteractableTypes.Contains(Type) && !IActor.IsUsed; }
            set { }
        }

        bool IActor.IsDestroyed
        {
            get { return IActor.IsUsed; }
            set { }
        }

        bool IActor.IsDestroyable
        {
            get { return DataDictionary.DestroyableTypes.Contains(Type) && !IActor.IsDestroyed; }
            set { }
        }

        bool IActor.IsPickupNoClick
        {
            get { return DataDictionary.NoPickupClickTypes.Contains(TrinityItemType); }
            set { }
        }

        int IActor.GameBalanceId
        {
            get { return GameBalanceID; }
            set { }
        }

        bool IActor.IsGoblin
        {
            get { return IsTreasureGoblin; }
            set { }
        }

        bool IActor.IsSummoned
        {
            get { return IsSummoned; }
            set { }
        }

        bool IActor.IsDead
        {
            get { return IsUnit && HitPoints <= 0; }
            set { }
        }

        bool IActor.IsInCombat
        {
            get { return TrinityPlugin.Player.IsInCombat; }
            set { }
        }

        bool IActor.IsUnit
        {
            get { return Object is DiaUnit; }
            set { }
        }

        bool IActor.IsValid
        {
            get { return CommonDataIsValid; }
        }

        double IActor.HitpointsPct
        {
            get { return HitPointsPct; }
            set { }
        }

        bool IActor.IsBossOrEliteRareUnique
        {
            get { return IsBossOrEliteRareUnique; }
            set { }
        }

        bool IActor.IsTrashMob
        {
            get { return IsTrashMob; }
            set { }
        }

        int IActor.UnitsInFront
        {
            get { return CountUnitsInFront(); }
            set { }
        }

        bool IActor.IsInLineOfSight
        {
            get { return HasBeenInLoS; }
            set { }
        }

        float IActor.HitpointsCurrent
        {
            get { return (float)HitPoints; }
            set { }
        }

        string IActor.Name
        {
            get { return InternalName; }
            set { }
        }

        bool IActor.IsHidden
        {
            get { return CommonDataIsValid && CommonData.Hidden > 0; }
            set { }
        }

        bool IActor.IsMoving
        {
            get { return CommonDataIsValid && Object.Movement.IsMoving; }
            set { }
        }

        bool IActor.IsFriendly
        {
            get { return !IActor.IsHostile; }
            set { }
        }

        bool IActor.IsUntargetable
        {
            get { return CommonDataIsValid && Object.CommonData.Untargetable > 0; }
            set { }
        }

        float IActor.ZDiff
        {
            get { return Math.Abs(Position.Z - TrinityPlugin.Player.Position.Z); }
            set { }
        }

        bool IActor.IsHostile
        {
            get { return IsHostile; }
            set { }
        }

        bool IActor.IsDoor
        {
            get { return ActorType == ActorType.Gizmo && Object is GizmoDoor; }
            set { }
        }

        bool IActor.IsBlacklisted
        {
            get { return GenericBlacklist.ContainsKey(ObjectHash); }
            set { }
        }

        bool IActor.IsMe
        {
            get { return RActorGuid == TrinityPlugin.Player.RActorGuid; }
            set { }
        }

        bool IActor.IsPlayer
        {
            get { return Object is DiaPlayer; }
            set { }
        }

        bool IActor.IsAvoidance
        {
            get { return Core.Avoidance.ActiveAvoidanceIds.Contains(ActorSNO); }
            set { }
        }

        float IActor.RotationRadians
        {
            get { return Rotation; }
            set { }
        }

        DateTime IActor.CreationTime
        {
            get { return CreationTime; }
            set { }
        }

        float IActor.MovementSpeed
        {
            get { return IActor.IsUnit && CommonDataIsValid ? IActor.DiaUnit.Movement.SpeedXY : 0; }
            set { }
        }

        bool IActor.IsStunned
        {
            get { return IActor.IsUnit && CommonDataIsValid && IActor.DiaUnit.IsStunned; }
            set { }
        }

        bool IActor.InGreaterRift
        {
            get { return DataDictionary.RiftWorldIds.Contains(TrinityPlugin.Player.WorldID); }
            set { }
        }

        long IActor.Coinage
        {
            get { return TrinityPlugin.Player.Coinage; }
            set { }
        }

        Rotator IActor.Rotator
        {
            get { return null; }
            set { }
        }

        ICollection<TrinityMonsterAffix> IActor.MonsterAffixes
        {
            get { return Affixes; }
            set { }
        }

        ObjectType IActor.Type
        {
            get { return ObjectType; }
            set { }
        }

        AnimationState IActor.AnimationState
        {
            get { return AnimationState; }
            set { }
        }

        SNOAnim IActor.CurrentAnimation
        {
            get { return Animation; }
            set { }
        }

        TargetingType IActor.TargetingType
        {
            get { return TargetingType.Unknown; }
            set { }
        }

        MonsterQuality IActor.MonsterQualityLevel
        {
            get { return MonsterQuality.Normal; }
            set { }
        }

        double IActor.SecondaryResource
        {
            get { return TrinityPlugin.Player.SecondaryResource; }
            set { }
        }

        double IActor.PrimaryResource
        {
            get { return TrinityPlugin.Player.PrimaryResource; }
            set { }
        }

        public bool IsChampion { get; set; }

        bool IActor.HasBuff(SNOPower snoPower)
        {
            return CacheData.Buffs.HasBuff(snoPower);
        }

        int IActor.BuffStacks(SNOPower snoPower)
        {
            return CacheData.Buffs.GetBuffStacks(snoPower);
        }

        bool IActor.Interact()
        {
            return Object.Interact();
        }

        int IFindable.ActorId
        {
            get { return ActorSNO; }
        }

        public static bool GetIsGizmoUsed(IActor actor)
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

                    return false;
                }

                int endAnimation;
                if (actor.Type == ObjectType.Interactable &&
                    DataDictionary.InteractEndAnimations.TryGetValue(actor.ActorSNO, out endAnimation) &&
                    endAnimation == (int)actor.CurrentAnimation)
                    return true;

                if (actor.GizmoType == GizmoType.None)
                    return true;

                var commonData = actor.CommonData;
                if (commonData != null && commonData.IsValid && !commonData.IsDisposed)
                {
                    if (commonData.GizmoState == 1)
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

                var lootContainer = actor.DiaGizmo as GizmoLootContainer;
                if (lootContainer != null && lootContainer.IsOpen)
                    return true;

                var gizmoDestructible = actor.DiaGizmo as GizmoDestructible;
                if (gizmoDestructible != null && gizmoDestructible.HitpointsCurrent <= 0)
                    return true;

                var destructibleContainer = actor.DiaGizmo as GizmoDestructibleLootContainer;
                if (destructibleContainer != null && destructibleContainer.HitpointsCurrent <= 0)
                    return true;

                if (actor.Type == ObjectType.Door || actor.Type == ObjectType.Container || actor.Type == ObjectType.Interactable)
                {
                    var currentAnimation = actor.CurrentAnimation.ToString().ToLower();

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

        #endregion

        /// <summary>
        /// Map affix GameBalanceIds to enum, excluding Bad Axe Data / -1
        /// </summary>
        public static HashSet<TrinityMonsterAffix> GetMonsterAffixes(int[] sourceAffixes)
        {
            var affixes = new HashSet<TrinityMonsterAffix>();

            if (!sourceAffixes.Any())
                return affixes;

            foreach (var sourceAffix in sourceAffixes)
            {
                if (sourceAffix != -1)
                    affixes.Add((TrinityMonsterAffix)sourceAffix);
            }

            return affixes;
        }


        public string Flags => string.Join(", ", Core.Avoidance.Grid.GetAvoidanceFlags(Position));
        public int TeamId { get; set; }
    }
}
