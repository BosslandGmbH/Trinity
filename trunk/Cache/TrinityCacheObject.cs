using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Cache;
using Trinity.Cache.Properties;
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
        private ACD _commonData;
        private DiaGizmo _gizmo;
        private DiaItem _item;
        private DiaObject _object;        
        private DiaUnit _unit;
        public DateTime CreationTime = DateTime.UtcNow;

        public TrinityCacheObject() { }

        public TrinityCacheObject(DiaObject diaObject)
        {
            LoadFrom(diaObject);
        }

        public void LoadFrom(DiaObject diaObject)
        {            
            Object = diaObject;
            ActorType = diaObject.ActorType;
            CommonData = diaObject.CommonData;
            RActorGuid = diaObject.RActorId;

            if (!IsValid) return;
            
            var cachedProperties = PropertyLoader.GetOrCreate(RActorGuid);
            PropertyLoader.Load<CommmonProperties>(this, cachedProperties);

            if (CommonData == null)
                return;

            switch (ActorType)
            {
                case ActorType.Monster:
                    PropertyLoader.Load<MonsterProperties>(this, cachedProperties);
                    break;
                case ActorType.Gizmo:
                    PropertyLoader.Load<GizmoProperties>(this, cachedProperties);
                    break;
                case ActorType.Item:
                    PropertyLoader.Load<ItemProperties>(this, cachedProperties);
                    break;
            }
        }

        #region Data Sources

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

        #endregion

        #region Properties
        public int AnnId { get; set; }
        public bool IsWaitSpot { get; set; }
        public double RiftValueInRadius { get; set; }
        public bool IsIllusion { get; set; }
        public AnimationState AnimationState { get; internal set; }        
        public bool IsHostile { get; internal set; }
        public bool IsSummoned { get; internal set; }
        public int ACDGuid { get; set; }
        public int RActorGuid { get; set; }
        public int ActorSNO { get; set; }
        public TrinityObjectType Type { get; set; }
        public ActorType ActorType { get; set; }
        public GizmoType GizmoType { get; set; }
        public double RiftValuePct { get; set; }
        public double Weight { get; set; }        
        public string WeightInfo { get; set; }        
        public int BoundToACD { get; set; }        
        public Vector3 Position { get; set; }        
        public float Distance { get; set; }        
        public string InternalName { get; set; } = string.Empty;
        public SNOAnim Animation { get; set; }              
        public int GameBalanceID { get; set; }       
        public int ItemLevel { get; set; }        
        public string ItemLink { get; set; }       
        public int GoldAmount { get; set; }        
        public bool OneHanded { get; set; }       
        public bool IsTwoSlotItem { get; set; }        
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
        public bool IsBoss { get; set; }        
        public bool IsAncient { get; set; }              
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
        public bool IsUsed { get; set; }
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
        public bool IsMyDroppedItem { get; set; }
        public bool IsShrine { get; set; }
        public bool IsDead { get; set; }
        public bool IsObstacle { get; set; }
        public bool IsIgnoreName { get; set; }
        public GameBalanceType GameBalanceType { get; set; }
        public MonsterRace MonsterRace { get; set; }
        public float HitPointsMax { get; set; }
        public bool IsAlly { get; set; }
        public bool IsSameTeam { get; set; }
        public bool IsUntargetable { get; set; }
        public int SummonerId { get; set; }
        public bool IsReflectingDamage { get; set; }
        public TrinityItemBaseType BaseType { get; set; }
        public bool IsEquipment { get; set; }
        public int WorldSnoId { get; set; }
        public bool IsGroundClicky { get; set; }
        public bool IsCorpse { get; set; }
        public bool IsWeaponRack { get; set; }
        public bool IsChest { get; set; }
        public bool IsRareChest { get; set; }
        public bool IsGizmoDisabledByScript { get; set; }
        public bool IsPlayerHeadstone { get; set; }
        public bool IsChampion { get; set; }

        #endregion

        #region Calculated PropertyLoader

        [Obsolete("Use IsElite for includes all special monster types and specific properties for special cases IsBoss IsUnique etc")]

        public bool IsEliteRareUnique => IsElite;

        [Obsolete("Use IsElite for includes all special monster types and specific properties for special cases IsBoss IsUnique etc")]
        public bool IsBossOrEliteRareUnique => IsElite;

        public bool IsTrashMob => (IsUnit && !(IsEliteRareUnique || IsBoss || IsTreasureGoblin || IsMinion));
        public bool IsMe => RActorGuid == TrinityPlugin.Player.RActorGuid;
        public bool IsUnit => Type == TrinityObjectType.Unit || ActorType == ActorType.Monster;
        public bool IsItem => Type == TrinityObjectType.Item || ActorType == ActorType.Item;
        public bool IsPlayer => Type == TrinityObjectType.Player || ActorType == ActorType.Player;
        public bool IsGizmo => ActorType == ActorType.Gizmo;
        public bool IsMonster => ActorType == ActorType.Monster;
        public bool IsBlacklisted => GenericBlacklist.ContainsKey(ObjectHash);
        public bool IsAvoidance => Core.Avoidance.ActiveAvoidanceSnoIds.Contains(ActorSNO);
        public bool IsCursedChest => Type == TrinityObjectType.CursedChest;
        public bool IsCursedShrine => Type == TrinityObjectType.CursedShrine;
        public bool IsEventObject => IsCursedChest || IsCursedShrine;
        public bool IsDestroyable => Type == TrinityObjectType.Barricade || Type == TrinityObjectType.Destructible;
        public bool IsHidden => IsValid && CommonData != null && (CommonData.Hidden > 0 || ActorAttributes.IsBurrowed);
        public bool IsInteractableType => DataDictionary.InteractableTypes.Contains(Type);
        public bool IsValid => ActorType == ActorType.ClientEffect || CommonData != null && CommonData.IsValid && !CommonData.IsDisposed;
        public bool IsPickupNoClick => DataDictionary.NoPickupClickItemTypes.Contains(ItemType) || DataDictionary.NoPickupClickTypes.Contains(Type);
        public bool IsStandingInAvoidance => CacheData.TimeBoundAvoidance.Any(a => a.Position.Distance(Position) <= a.Radius);
        public bool CanWalkTo => Core.Avoidance.Grid.CanRayWalk(TrinityPlugin.Player.Position, Position);
        public bool CanCastTo => Core.Avoidance.Grid.CanRayCast(TrinityPlugin.Player.Position, Position);
        public float ZDiff => Math.Abs(Position.Z - TrinityPlugin.Player.Position.Z);
        public float RadiusDistance => Math.Max(Distance - Radius, 0f);

        public ActorAttributes ActorAttributes { get; set; }

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

        public override string ToString()
        {
            return $"{InternalName} ({ActorSNO}), Type={Type} Dist={Distance}";
        }
    
        #endregion
    }
}