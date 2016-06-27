//#region

//using System;
//using System.Collections.Generic;
//using Trinity.Cache;
//using Trinity.Framework;
//using Trinity.Framework.ActorCache.Attributes;
//using Trinity.Framework.ActorCache.Properties;
//using Trinity.Framework.Avoidance.Structures;
//using Trinity.Framework.Objects.Memory.Attributes;
//using Zeta.Bot.Navigation;
//using Zeta.Common;
//using Zeta.Game;
//using Zeta.Game.Internals.Actors;
//using Zeta.Game.Internals.SNO;

//#endregion

//namespace Trinity
//{
//    public class TrinityActor
//    {
//        public TrinityActor()
//        {
//        }

//        public TrinityActor(DiaObject diaObject)
//        {
//            LoadFrom(diaObject);
//        }

//        public void LoadFrom(DiaObject diaObject)
//        {
//            //Object = diaObject;
//            //CommonData = diaObject.CommonData;

//            //CommonProperties.Load(this, Object);

//            //switch (ActorType)
//            //{
//            //    case ActorType.Monster:
//            //        Unit = diaObject as DiaUnit;
//            //        UnitProperties.UpdateUnitProperties(this, Unit);
//            //        break;
//            //    case ActorType.Gizmo:
//            //        Gizmo = diaObject as DiaGizmo;
//            //        GizmoProperties.UpdateGizmoProperties(this, Gizmo);
//            //        break;
//            //    case ActorType.Item:
//            //        Item = diaObject as DiaItem;
//            //        ItemProperties.UpdateItemProperties(this, Item);
//            //        break;
//            //    case ActorType.Player:
//            //        Unit = diaObject as DiaUnit;
//            //        Player = diaObject as DiaPlayer;
//            //        UnitProperties.UpdateUnitProperties(this, Unit);
//            //        PlayerProperties.UpdatePlayerProperties(this, Player);
//            //        break;
//            //}
//        }

//        #region Data Sources

//        public DiaObject Object { get; set; }
//        public DiaUnit Unit { get; set; }
//        public DiaGizmo Gizmo { get; set; }
//        public DiaPlayer Player { get; set; }
//        public DiaItem Item { get; set; }
//        public ACD CommonData { get; set; }

//        #endregion

//        #region Properties

//        public ActorClass ActorClass { get; set; } = ActorClass.Invalid;
//        public ActorType ActorType { get; set; } = ActorType.Invalid;
//        public GizmoType GizmoType { get; set; } = GizmoType.None;
//        public MonsterType MonsterType { get; set; } = MonsterType.None;
//        public MonsterRace MonsterRace { get; set; } = MonsterRace.Unknown;
//        public MonsterSize MonsterSize { get; set; } = MonsterSize.Unknown;
//        public int AnnId { get; set; }
//        public bool IsWaitSpot { get; set; }
//        public double RiftValueInRadius { get; set; }
//        public bool IsIllusion { get; set; }
//        public AnimationState AnimationState { get; internal set; }
//        public bool IsHostile { get; internal set; }
//        public bool IsSummoned { get; internal set; }
//        public int AcdId { get; set; }
//        public int RActorGuid { get; set; }
//        public int ActorSNO { get; set; }
//        public TrinityObjectType Type { get; set; }
//        public double RiftValuePct { get; set; }
//        public double Weight { get; set; }
//        public string WeightInfo { get; set; }
//        public Vector3 Position { get; set; }
//        public float Distance { get; set; }
//        public string InternalName { get; set; } = string.Empty;
//        public SNOAnim Animation { get; set; }
//        public int GameBalanceID { get; set; }
//        public int ItemLevel { get; set; }
//        public string ItemLink { get; set; }
//        public int GoldAmount { get; set; }
//        public bool IsTwoSlotItem { get; set; }
//        public ItemQuality ItemQuality { get; set; }
//        public DBItemBaseType DBItemBaseType { get; set; }
//        public ItemType DBItemType { get; set; }
//        public FollowerType FollowerType { get; set; }
//        public TrinityItemType ItemType { get; set; }
//        public DateTime LastSeenTime { get; set; }
//        public bool IsElite { get; set; }
//        public bool IsInvulnerable { get; set; }
//        public bool IsRare { get; set; }
//        public bool IsUnique { get; set; }
//        public bool IsMinion { get; set; }
//        public MonsterAffixes MonsterAffixes { get; set; }
//        public bool IsTreasureGoblin { get; set; }
//        public bool IsBoss { get; set; }
//        public bool HasDotDPS { get; set; }
//        public double HitPointsPct { get; set; }
//        public double HitPoints { get; set; }
//        public float Radius { get; set; }
//        public float Rotation { get; set; }
//        public Vector2 DirectionVector { get; set; }
//        public bool IsFacingPlayer { get; set; }
//        public bool HasBeenPrimaryTarget { get; set; }
//        public string ObjectHash { get; set; }
//        public bool IsUsed { get; set; }
//        public bool IsSummonedByPlayer { get; set; }
//        public bool IsSummoner { get; set; }
//        public int SummonedByACDId { get; set; }
//        public bool IsNPC { get; set; }
//        public bool NPCIsOperable { get; set; }
//        public bool IsQuestMonster { get; set; }
//        public bool IsMinimapActive { get; set; }
//        public bool IsBountyObjective { get; set; }
//        public bool IsQuestGiver { get; set; }
//        public string CacheInfo { get; set; }
//        public float CollisionRadius { get; set; }
//        public bool IsMarker { get; set; }
//        public bool IsSafeSpot { get; set; }
//        public bool IsSpawningBoss { get; set; }
//        public string AnimationNameLowerCase { get; set; }
//        public string InternalNameLowerCase { get; set; }
//        public int TeamId { get; set; }
//        public MonsterQuality MonsterQuality { get; set; }
//        public bool IsMyDroppedItem { get; set; }
//        public bool IsDead { get; set; }
//        public bool IsObstacle { get; set; }
//        public bool IsExcludedId { get; set; }
//        public GameBalanceType GameBalanceType { get; set; }
//        public float HitPointsMax { get; set; }
//        public bool IsAlly { get; set; }
//        public bool IsSameTeam { get; set; }
//        public bool IsUntargetable { get; set; }
//        public int SummonerId { get; set; }
//        public bool IsReflectingDamage { get; set; }
//        public TrinityItemBaseType BaseType { get; set; }
//        public bool IsEquipment { get; set; }
//        public int WorldSnoId { get; set; }
//        public bool IsGroundClicky { get; set; }
//        public bool IsCorpse { get; set; }
//        public bool IsWeaponRack { get; set; }
//        public bool IsChest { get; set; }
//        public bool IsRareChest { get; set; }
//        public bool IsGizmoDisabledByScript { get; set; }
//        public bool IsPlayerHeadstone { get; set; }
//        public bool IsChampion { get; set; }
//        public bool IsInLineOfSight { get; set; }
//        public bool HasBeenInLoS { get; set; }
//        public bool IsWalkable { get; set; }
//        public bool HasBeenWalkable { get; set; }
//        public double CacheTime { get; set; }
//        public bool IsFrozen { get; set; }
//        public int FastAttributeGroupId { get; set; }
//        public bool IsAllowedClientEffect { get; set; }
//        public bool IsExcludedType { get; set; }
//        public bool IsContainer { get; set; }
//        public ActorAttributes ActorAttributes { get; set; }
//        public bool IsPickupNoClick { get; set; }
//        public bool IsCursedChest { get; set; }
//        public bool IsCursedShrine { get; set; }
//        public bool IsDestroyable { get; set; }
//        public bool IsMe { get; set; }
//        public bool IsUnit { get; set; }
//        public bool IsItem { get; set; }
//        public bool IsPlayer { get; set; }
//        public bool IsGizmo { get; set; }
//        public bool IsMonster { get; set; }
//        public bool IsEventObject { get; set; }
//        public bool IsInteractableType { get; set; }
//        public bool IsHidden { get; set; }
//        public bool IsTrashMob { get; set; }
//        public bool IsIgnored { get; set; }
//        public float AxialRadius { get; set; }

//        #endregion

//        #region Expressions

//        /// <summary>
//        /// Includes temporary avoidances like monsters with telgraphing charge animations.
//        /// </summary>
//        public bool IsActiveAvoidance => Type == TrinityObjectType.Avoidance || Core.Avoidance.ActiveAvoidanceSnoIds.Contains(ActorSNO);

//        /// <summary>
//        /// Is blacklisted by GenericBlacklist.
//        /// </summary>
//        public bool IsBlacklisted => GenericBlacklist.ContainsKey(ObjectHash);

//        /// <summary>
//        /// Difference in position Z
//        /// </summary>
//        public float ZDiff => Math.Abs(Position.Z - TrinityPlugin.Player.Position.Z);

//        /// <summary>
//        /// Distance to closest point at radius of actor
//        /// </summary>
//        public float RadiusDistance => Math.Max(Distance - AxialRadius, 0f);

//        public bool IsLastTarget => RActorGuid == TrinityPlugin.LastTargetRactorGUID;
//        public string DebugAvoidanceFlags => Core.Avoidance.Grid.GetNearestNode(Position)?.AvoidanceFlags.ToString();
//        public string DebugNavCellFlags => Core.Avoidance.Grid.GetNearestNode(Position)?.NodeFlags.ToString();
//        public bool IsStandingInAvoidance => Core.Avoidance.Grid.IsLocationInFlags(Position, AvoidanceFlags.Avoidance);                
//        public bool IsValid => Object != null && Object.IsValid && (ActorType == ActorType.ClientEffect || CommonData != null && CommonData.IsValid && !CommonData.IsDisposed);

//        #endregion

//        #region Helper Methods

//        public bool CanWalkTo(Vector3 destination = default(Vector3))
//        {
//            return Core.Avoidance.Grid.CanRayWalk(Position == default(Vector3) ? TrinityPlugin.Player.Position : destination, Position);
//        }

//        public bool CanCastTo(Vector3 destination = default(Vector3))
//        {
//            return Core.Avoidance.Grid.CanRayCast(Position == default(Vector3) ? TrinityPlugin.Player.Position : destination, Position);
//        }

//        public bool IsFacing(Vector3 targetPosition, float arcDegrees = 70f)
//        {
//            return TargetUtil.IsFacing(this, targetPosition, arcDegrees);
//        }

//        public bool IsPlayerFacing(float arc)
//        {
//            return TrinityPlugin.Player.IsFacing(Position, arc);
//        }

//        public int NearbyUnitsWithinDistance(float range = 5f)
//        {
//            return TargetUtil.NearbyUnitsWithinDistance(this, range);
//        }

//        public int CountUnitsBehind(float range)
//        {
//            return TargetUtil.CountUnitsBehind(this, range);
//        }

//        public int CountUnitsInFront()
//        {
//            return TargetUtil.CountUnitsInFront(this);
//        }

//        public void AddCacheInfo(string reason)
//        {
//            if (string.IsNullOrEmpty(reason)) return;
//            var seperator = CacheInfo?.Length > 0 ? ", " : string.Empty;
//            CacheInfo += seperator + reason;
//        }

//        public override string ToString()
//        {
//            return $"{InternalName} ({ActorSNO}) Type={Type} Dist={Distance} Weight={Weight} WeightInfo={WeightInfo}";
//        }

//        #endregion
//    }
//}