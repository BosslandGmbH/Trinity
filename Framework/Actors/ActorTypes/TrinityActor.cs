using System;
using Trinity.Cache;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Actors.Properties;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory.Misc;
using Trinity.Objects.Native;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using GizmoType = Zeta.Game.Internals.SNO.GizmoType;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Actors.ActorTypes
{
    public class TrinityActor : ActorBase
    {
        public virtual ActorAttributes Attributes { get; set; }
        public bool IsAllowedClientEffect { get; set; }
        public bool IsExcludedId { get; set; }
        public bool IsExcludedType { get; set; }
        public string InternalNameLowerCase { get; set; }
        public GizmoType GizmoType { get; set; } = GizmoType.None;
        public bool IsObstacle { get; set; }
        public int WorldSnoId { get; set; }
        public float Radius { get; set; }
        public string ObjectHash { get; set; }
        public TrinityObjectType Type { get; set; }
        public float CollisionRadius { get; set; }
        public float AxialRadius { get; set; }
        public int GameBalanceId { get; set; }
        public GameBalanceType GameBalanceType { get; set; }
        public bool IsMe { get; set; }
        public bool IsUnit { get; set; }
        public bool IsItem { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsGizmo { get; set; }
        public bool IsMonster { get; set; }
        public SNOAnim Animation { get; set; }
        public string AnimationNameLowerCase { get; set; }
        public AnimationState AnimationState { get; set; }
        public MonsterRace MonsterRace { get; set; } = MonsterRace.Unknown;
        public MonsterType MonsterType { get; set; } = MonsterType.None;
        public MonsterSize MonsterSize { get; set; } = MonsterSize.Unknown;
        public MonsterAffixes MonsterAffixes { get; set; }
        public bool IsPlayerHeadstone { get; set; }
        public bool IsRareChest { get; set; }
        public bool IsChest { get; set; }
        public bool IsCorpse { get; set; }
        public bool IsWeaponRack { get; set; }
        public bool IsGroundClicky { get; set; }
        public bool IsContainer { get; set; }
        public bool IsCursedChest { get; set; }
        public bool IsCursedShrine { get; set; }
        public bool IsDestroyable { get; set; }
        public bool IsEventObject { get; set; }
        public bool IsUntargetable { get; set; }
        public bool IsInvulnerable { get; set; }
        public bool IsInteractableType { get; set; }
        public bool IsUsed { get; set; }
        public bool IsBountyObjective { get; set; }
        public bool IsMinimapActive { get; set; }
        public DateTime LastSeenTime { get; set; }
        public int AnnId { get; set; }
        public bool IsInLineOfSight { get; set; }
        public bool IsWalkable { get; set; }
        public bool HasBeenWalkable { get; set; }
        public bool HasBeenInLoS { get; set; }
        public InventorySlot InventorySlot { get; set; }
        public bool IsGroundItem { get; set; }
        public int TeamId { get; set; }
        public bool IsQuestGiver { get; set; }
        public bool IsSameTeam { get; set; }
        public bool IsHidden { get; set; }
        public bool IsSpawningBoss { get; set; }
        public bool IsReflectingDamage { get; set; }
        public float HitPointsMax { get; set; }
        public float HitPointsPct { get; set; }
        public float HitPoints { get; set; }
        public bool IsDead { get; set; }
        public bool IsSummoner { get; set; }
        public int SummonerId { get; set; }
        public bool IsSummoned { get; set; }
        public bool IsSummonedByPlayer { get; set; }
        public bool IsTrashMob { get; set; }
        public bool IsElite { get; set; }
        public bool IsMinion { get; set; }
        public bool IsUnique { get; set; }
        public bool IsChampion { get; set; }
        public bool IsRare { get; set; }
        public bool IsIllusion { get; set; }
        public bool IsTreasureGoblin { get; set; }
        public double RiftValuePct { get; set; }
        public bool IsHostile { get; set; }
        public bool IsBoss { get; set; }
        public MonsterQuality MonsterQuality { get; set; }
        public bool IsFriendly { get; set; }
        public MarkerType MarkerType { get; set; }
        public bool IsNpc { get; set; }
        public bool NpcIsOperable { get; set; }
        public bool HasDotDps { get; set; }
        public int SummonedByAnnId { get; set; }
        public string CacheInfo { get; set; }
        public float Distance { get; set; }
        public float RadiusDistance { get; set; }
        public TargetCategory TargetCategory { get; set; }
        public double Weight { get; set; }
        public ItemQuality ItemQualityLevel { get; set; } = ItemQuality.Invalid;
        public ItemType ItemType { get; set; } = ItemType.Unknown;
        public ItemBaseType ItemBaseType { get; set; }
        public TrinityItemType TrinityItemType { get; set; }
        public TrinityItemBaseType TrinityItemBaseType { get; set; }
        public bool HasBuffVisualEffect { get; set; }
        public bool IsMarker { get; set; }
        public bool IsSafeSpot { get; set; }
        public bool IsWaitSpot { get; set; }
        public string Name { get; set; }
        public float RotationDegrees { get; set; }
        public bool IsQuestMonster { get; set; }
        public PetType PetType { get; set; } = PetType.None;
        public bool IsNoDamage { get; set; }
        public TeamType Team { get; set; }

        /// <summary>
        /// Rotation in radians
        /// </summary>
        public float Rotation { get; set; }

        public Vector2 DirectionVector { get; set; }
        public float MovementSpeed { get; set; }
        public bool IsMoving { get; set; }
        public string WeightInfo { get; set; }
        public bool HasBeenPrimaryTarget { get; set; }
        public double RiftValueInRadius { get; set; }
        public float RequiredRange { get; set; }

        public override void OnCreated()
        {
            Attributes = new ActorAttributes(FastAttributeGroupId);
            UpdateProperties();
        }

        public override void OnUpdated()
        {
            Attributes.Update();
            UpdateProperties();
        }

        public bool CanWalkTo(Vector3 destination = default(Vector3))
        {
            return Core.Avoidance.Grid.CanRayWalk(Position == default(Vector3) ? TrinityPlugin.Player.Position : destination, Position);
        }

        public bool CanCastTo(Vector3 destination = default(Vector3))
        {
            return Core.Avoidance.Grid.CanRayCast(Position == default(Vector3) ? TrinityPlugin.Player.Position : destination, Position);
        }

        public bool IsFacing(Vector3 targetPosition, float arcDegrees = 70f) => TargetUtil.IsFacing(this, targetPosition, arcDegrees);

        public bool IsPlayerFacing(float arc) => TrinityPlugin.Player.IsFacing(Position, arc);

        public int NearbyUnitsWithinDistance(float range = 5f) => TargetUtil.NearbyUnitsWithinDistance(this, range);

        public int CountUnitsBehind(float range) => TargetUtil.CountUnitsBehind(this, range);

        public int CountUnitsInFront() => TargetUtil.CountUnitsInFront(this);

        /// <summary>
        /// Includes temporary avoidances like monsters with telgraphing charge animations.
        /// </summary>
        public bool IsActiveAvoidance => Type == TrinityObjectType.Avoidance || Core.Avoidance.ActiveAvoidanceSnoIds.Contains(ActorSnoId);

        /// <summary>
        /// Is blacklisted by GenericBlacklist.
        /// </summary>
        public bool IsBlacklisted => GenericBlacklist.ContainsKey(ObjectHash);

        /// <summary>
        /// Difference in position Z
        /// </summary>
        public float ZDiff => Math.Abs(Position.Z - TrinityPlugin.Player.Position.Z);

        public bool IsLastTarget => RActorId == TrinityPlugin.LastTargetRactorGUID;
        public string DebugAvoidanceFlags => Core.Avoidance.Grid.GetNearestNode(Position)?.AvoidanceFlags.ToString();
        public string DebugNavCellFlags => Core.Avoidance.Grid.GetNearestNode(Position)?.NodeFlags.ToString();
        public bool IsStandingInAvoidance => Core.Avoidance.Grid.IsLocationInFlags(Position, AvoidanceFlags.Avoidance);
        public bool IsFacingPlayer => TargetUtil.IsFacing(this, Core.Player.Position, 30f);
        public double CacheTime => Math.Abs(UpdateTime) < double.Epsilon ? CreateTime : UpdateTime;
        public bool IsIgnored => TargetCategory == TargetCategory.Ignore;
        public bool NpcHasInteractOptions { get; set; }


        public void AddCacheInfo(string reason)
        {
            if (string.IsNullOrEmpty(reason)) return;
            var seperator = CacheInfo?.Length > 0 ? ", " : string.Empty;
            CacheInfo += seperator + reason;
        }

        private void UpdateProperties()
        {
            CommonProperties.Populate(this);
            UnitProperties.Populate(this);
            GizmoProperties.Populate(this);   
        }

        public override string ToString() => $"{InternalName} ({ActorSnoId}) Type={Type} Dist={Distance}";

        public bool Interact()
        {
            Navigator.PlayerMover.MoveStop();

            switch (ActorType)
            {
                case ActorType.Monster:
                    return ZetaDia.Me.UsePower(SNOPower.Axe_Operate_NPC, Vector3.Zero, 0, AcdId);
                case ActorType.Gizmo:
                case ActorType.Item:
                    return ZetaDia.Me.UsePower(SNOPower.Axe_Operate_Gizmo, Vector3.Zero, 0, AcdId);
                default:
                    return false;
            }
        }
        
        public DiaObject ToDiaObject() => RActor.IsValid ? RActor.BaseAddress.UnsafeCreate<DiaObject>() : null;

        public DiaObject ToDiaUnit() => RActor.IsValid ? RActor.BaseAddress.UnsafeCreate<DiaUnit>() : null;

    }

}

