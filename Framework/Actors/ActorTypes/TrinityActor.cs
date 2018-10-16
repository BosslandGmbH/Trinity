using System;
using Trinity.Framework.Helpers;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Actors.Properties;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Events;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;
using Trinity.Settings;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using GizmoType = Zeta.Game.Internals.SNO.GizmoType;
using Trinity.Framework.Reference;
using Zeta.Bot;
using System.Linq;
using System.Collections.Generic;

namespace Trinity.Framework.Actors.ActorTypes
{
    public class TrinityActor : ActorBase, ITargetable
    {
        public TrinityActor(DiaObject actor) : base(actor)
        {
            Attributes = new AttributesWrapper(CommonData);
            ObjectHash = HashGenerator.GenerateObjectHash(Position, ActorSnoId, InternalName);
            IsProfileBlacklisted = ProfileManager.CurrentProfile?.TargetBlacklists?.Any(b => b.ActorId == ActorSnoId) ?? false;
            MonsterQuality = CommonData?.MonsterQualityLevel ?? MonsterQuality.Normal;
        }

        public TrinityActor(ACD acd, ActorType type) : base(acd, type)
        {
            Attributes = new AttributesWrapper(CommonData);
            ObjectHash = HashGenerator.GenerateObjectHash(Position, ActorSnoId, InternalName);
        }

        public string ObjectHash { get; private set; }
        public bool IsProfileBlacklisted { get; private set; }
        public MonsterQuality MonsterQuality { get; private set; }

        public TargetCategory TargetCategory { get; set; }
        public TargetingInfo Targeting { get; set; } = new TargetingInfo();

        // LOS
        // CommonProperties.UpdateLineOfSight
        public bool IsInLineOfSight { get; set; }
        public bool IsWalkable { get; set; }
        public bool HasBeenWalkable { get; set; }
        public bool HasBeenInLoS { get; set; }
        // UnitProperties.cs => Triggers OnUnitDeath()
        public bool IsDead { get; set; }

        // TODO: Confirm that this was never set in any previous version.
        public bool IsMarker { get; set; }
        public bool IsFrozen { get; set; }

        public string CacheInfo { get; set; }
        public double Weight { get; set; }
        public bool IsSafeSpot { get; set; }
        public string WeightInfo { get; set; }

        public virtual AttributesWrapper Attributes { get; private set; }

        public bool IsAllowedClientEffect => GameData.AllowedClientEffects.Contains(ActorSnoId);

        public bool IsExcludedId => GameData.ExcludedActorIds.Contains(ActorSnoId) ||
                                    GameData.BlackListIds.Contains(ActorSnoId);

        public bool IsExcludedType => GameData.ExcludedActorTypes.Contains(ActorType);

        // TODO: Is this possible null?
        public string InternalNameLowerCase => InternalName.ToLower();

        public GizmoType GizmoType => ActorInfo?.GizmoType ?? GizmoType.None;
        public float AxialRadius => ActorInfo?.AxialCylinder.Ax1 ?? 0.0f;

        public bool IsObstacle => GameData.NavigationObstacleIds.Contains(ActorSnoId) || GameData.PathFindingObstacles.ContainsKey(ActorSnoId);
        public int WorldDynamicId => _actor.WorldId; // TODO: Why doesn't this one work anymore? => _actor.WorldDynamicId
        public float Radius => _actor?.InteractDistance ?? 0.0f;
        public TrinityObjectType Type => CommonProperties.GetObjectType(ActorType, ActorSnoId, GizmoType, InternalName);
        public float CollisionRadius => GameData.CustomObjectRadius.ContainsKey(ActorSnoId) ? GameData.CustomObjectRadius[ActorSnoId] : Math.Max(1f, AxialRadius * 0.60f);
        public int GameBalanceId => CommonData?.GameBalanceId ?? 0;

        // TODO: Make it possible to have some "invalid" GameBalanceTypes.
        public GameBalanceType GameBalanceType => CommonData?.GameBalanceType ?? GameBalanceType.ItemTypes;
        public bool IsMe => _actor.IsMe;
        public bool IsUnit => Type == TrinityObjectType.Unit || ActorType == ActorType.Monster || ActorType == ActorType.Player;
        public bool IsItem => Type == TrinityObjectType.Item || ActorType == ActorType.Item;
        public bool IsPlayer => Type == TrinityObjectType.Player || ActorType == ActorType.Player;
        public bool IsGizmo => ActorType == ActorType.Gizmo;
        public bool IsMonster => ActorType == ActorType.Monster;

        public SNOAnim Animation => CommonData?.AnimationInfo?.Current ?? SNOAnim.Invalid;
        public string AnimationNameLowerCase => GameData.GetAnimationNameLowerCase(Animation);
        public AnimationState AnimationState => CommonData?.AnimationState ?? AnimationState.Invalid;
        public MonsterRace MonsterRace => MonsterInfo?.MonsterRace ?? MonsterRace.Unknown;
        public MonsterType MonsterType => MonsterInfo?.MonsterType ?? MonsterType.None;
        public MonsterSize MonsterSize => MonsterInfo?.MonsterSize ?? MonsterSize.Unknown;
        public MonsterAffixes MonsterAffixes => UnitProperties.GetMonsterAffixes((CommonData?.Affixes.ToList() ?? new List<int>())).Flags;

        public bool IsPlayerHeadstone => ActorSnoId == GameData.PlayerHeadstoneSNO;
        public bool IsRareChest => InternalNameLowerCase.Contains("chest_rare") || GameData.ResplendentChestIds.Contains(ActorSnoId);
        public bool IsChest => IsCursedChest || IsRareChest || InternalNameLowerCase.Contains("chest") || GameData.ContainerWhiteListIds.Contains(ActorSnoId);
        public bool IsCorpse => InternalNameLowerCase.Contains("corpse");
        public bool IsWeaponRack => InternalNameLowerCase.Contains("rack");
        public bool IsGroundClicky => InternalNameLowerCase.Contains("ground_clicky");
        public bool IsContainer => IsRareChest || IsChest || IsCorpse || IsWeaponRack || IsGroundClicky;
        public bool IsCursedChest => Type == TrinityObjectType.CursedChest;
        public bool IsCursedShrine => Type == TrinityObjectType.CursedShrine;
        public bool IsDestroyable => Type == TrinityObjectType.Barricade || Type == TrinityObjectType.Destructible;
        public bool IsEventObject => IsCursedChest || IsCursedShrine;
        public bool IsUntargetable => (Attributes?.IsUntargetable ?? false) && !GameData.IgnoreUntargettableAttribute.Contains(ActorSnoId);
        public bool IsInvulnerable => Attributes?.IsInvulnerable ?? false;
        public bool IsInteractableType => GameData.InteractableTypes.Contains(Type);
        public bool IsUsed => GizmoProperties.GetIsGizmoUsed(this);
        public bool IsBountyObjective => Attributes?.IsBountyObjective ?? false;
        public bool IsMinimapActive => (Attributes?.IsMinimapActive ?? false) || (Core.Minimap.MinimapIconAcdIds.Contains(AcdId));

        public string PositionHash => string.Empty + Core.Player.WorldSnoId + Position.X + Position.Y;
        public int TeamId => UnitProperties.GetTeamId(CommonData);
        public bool IsQuestGiver => (MarkerType == MarkerType.Exclamation || MarkerType == MarkerType.ExclamationBlue);
        public bool IsSameTeam => IsFriendly || TeamId == Core.Player.TeamId || GameData.AllyMonsterTypes.Contains(MonsterType);
        public bool IsHidden => (Attributes?.IsHidden ?? false) || (Attributes?.IsBurrowed ?? false);
        public bool IsSpawningBoss => IsBoss && IsUntargetable;
        public bool IsReflectingDamage => MonsterAffixes.HasFlag(MonsterAffixes.ReflectsDamage) && (Attributes?.IsReflecting ?? false);
        public float HitPointsMax => Attributes?.HitpointsMaxTotal ?? 0f;
        public float HitPointsPct => HitPoints > 0f ? HitPoints / HitPointsMax : 0f;
        public float HitPoints => Attributes?.Hitpoints ?? 0f;

        
        public bool IsSummoner => SummonerId > 0;
        public int SummonerId => Attributes?.SummonerId ?? 0;
        public bool IsSummoned => SummonedByAnnId > 0 && EffectOwnerAnnId > 0;
        public bool IsSummonedByPlayer => IsSummoned && (SummonedByAnnId == Core.Player.MyDynamicID || EffectOwnerAnnId == Core.Player.MyDynamicID);
        public bool IsTrashMob => IsUnit && !(IsElite || IsBoss || IsTreasureGoblin || IsMinion);
        public bool IsElite => IsMinion || IsRare || IsChampion || IsUnique || IsBoss;
        public bool IsMinion => MonsterQuality == MonsterQuality.Minion;
        public bool IsUnique => MonsterQuality == MonsterQuality.Unique;
        public bool IsChampion => MonsterQuality == MonsterQuality.Champion;
        public bool IsRare => MonsterQuality == MonsterQuality.Rare;
        public bool IsIllusion => MonsterAffixes.HasFlag(MonsterAffixes.Illusionist) && (Attributes?.IsIllusion ?? false);
        public bool IsTreasureGoblin => MonsterRace == MonsterRace.TreasureGoblin;
        public double RiftValuePct => Core.Rift.GetRiftValue(this);
        public bool IsHostile => UnitProperties.IsHostile(CommonData, ZetaDia.Me.CommonData) || (Attributes?.LastDamageAnnId ?? 0) == Core.Player.MyDynamicID;
        public bool IsBoss => MonsterQuality == MonsterQuality.Boss;
        public bool IsFriendly => !UnitProperties.IsHostile(CommonData, ZetaDia.Me.CommonData);
        public MarkerType MarkerType => Attributes?.MarkerType ?? MarkerType.Invalid;
        public bool IsNpc => Attributes?.IsNPC ?? false;
        public bool NpcIsOperable => Attributes?.NPCIsOperatable ?? false;
        public bool HasDotDps => Attributes?.HasDotDps ?? false;
        public int SummonedByAnnId => Attributes?.SummonedByAnnId ?? 0;
        public float Distance => Position.Distance(Core.Actors.ActivePlayerPosition);
        public float RadiusDistance => Math.Max(Distance - CollisionRadius, 0f);

        public bool HasBuffVisualEffect => Attributes?.HasBuffVisualEffect ?? false;
        public string Name => InternalName;
        public float RotationDegrees => MathEx.ToDegrees(Rotation);
        public bool IsQuestMonster => (Attributes?.IsQuestMonster ?? false) || (Attributes?.IsShadowClone ?? false);
        public PetType PetType => Attributes?.PetType ?? PetType.None;
        public bool IsNoDamage => Attributes?.IsNoDamage ?? false;
        public TeamType Team => (TeamType)TeamId;
        public EliteTypes EliteType => UnitProperties.GetEliteType(this);
        public ShrineTypes ShrineType => GizmoProperties.GetShrineType(this);
        public ContainerTypes ContainerType => GizmoProperties.GetContainerType(this);

        public SpecialTypes SpecialType => CommonProperties.GetSpecialType(this);

        public ActorMovement Movement
        {
            get
            {
                if (_actor.Movement != null && _actor.Movement.IsValid)
                    return _actor.Movement;

                return null;
            }
        }

        /// <summary>
        /// Rotation in radians
        /// </summary>
        public float Rotation => Movement?.Rotation ?? 0f;
        public Vector2 DirectionVector => Movement?.DirectionVector ?? Vector2.Zero;
        public float MovementSpeed => Movement?.SpeedXY ?? 0f;
        public bool IsMoving => Movement?.IsMoving ?? false;
        public float RequiredRadiusDistance => CommonProperties.GetRequiredRange(this);
        public bool NpcHasInteractOptions => Attributes?.NpcHasInteractOptions ?? false;

        public bool IsLockedDoor => (Attributes?.IsDoorLocked ?? false) || (Attributes?.IsDoorTimed ?? false) && !(Attributes?.IsGizmoBeenOperated ?? false);
        public int MinimapIconOverride => Attributes?.MinimapIconOverride ?? 0;
        public int EffectOwnerAnnId => Attributes?.EffectOwnerAnnId ?? 0;
        public bool IsInteractWhitelisted => GameData.InteractWhiteListIds.Contains(ActorSnoId);
        public bool IsUsingBossbar => IsBoss && (Attributes?.IsUsingBossbar ?? false);
        public bool IsShadowClone => Core.Player.IsInBossEncounter ? (Attributes?.IsShadowClone ?? false) : false;
        public bool IsCorruptGrowth => GameData.CorruptGrowthIds.Contains(ActorSnoId);
        public bool IsPortal => GameData.PortalTypes.Contains(GizmoType);

        public override void OnCreated()
        {
            CommonProperties.Populate(this);
            UnitProperties.Populate(this);
            GizmoProperties.Populate(this);
        }

        public override void OnUpdated()
        {
            CommonProperties.Update(this);
            UnitProperties.Update(this);
            GizmoProperties.Update(this);
        }

        public bool CanWalkTo(Vector3 destination = default(Vector3))
        {
            return Core.Avoidance.Grid.CanRayWalk(Position == default(Vector3) ? Core.Player.Position : destination, Position);
        }

        public bool CanCastTo(Vector3 destination = default(Vector3))
        {
            return Core.Avoidance.Grid.CanRayCast(Position == default(Vector3) ? Core.Player.Position : destination, Position);
        }

        public bool IsFacing(Vector3 targetPosition, float arcDegrees = 70f) => TargetUtil.IsFacing(this, targetPosition, arcDegrees);

        public bool IsPlayerFacing(float arc) => Core.Player.IsFacing(Position, arc);

        public int NearbyUnitsWithinDistance(float range = 5f) => TargetUtil.NearbyUnitsWithinDistance(this, range);

        public int CountUnitsBehind(float range) => TargetUtil.CountUnitsBehind(this, range);

        public int CountUnitsInFront() => TargetUtil.CountUnitsInFront(this);

        public int CountUnitsInFront(float radius) => TargetUtil.CountUnitsInFront(this, radius);

        /// <summary>
        /// Includes temporary avoidances like monsters with telgraphing charge animations.
        /// </summary>
        public bool IsActiveAvoidance => Type == TrinityObjectType.Avoidance || Core.Avoidance.GridEnricher.ActiveAvoidanceSnoIds.Contains(ActorSnoId);

        /// <summary>
        /// Is blacklisted by GenericBlacklist.
        /// </summary>
        public bool IsBlacklisted => GenericBlacklist.ContainsKey(ObjectHash);

        /// <summary>
        /// Difference in position Z
        /// </summary>
        public float ZDiff => Math.Abs(Position.Z - Core.Player.Position.Z);

        public bool IsLastTarget => RActorId == (TrinityCombat.Targeting.LastTarget?.RActorId ?? -1);
        public string DebugAvoidanceFlags => Core.Avoidance.Grid.GetNearestNode(Position)?.AvoidanceFlags.ToString();
        public string DebugNavCellFlags => Core.Avoidance.Grid.GetNearestNode(Position)?.NodeFlags.ToString();
        public bool IsInAvoidance => Core.Avoidance.Grid.IsLocationInFlags(Position, AvoidanceFlags.Avoidance);
        public bool IsInCriticalAvoidance => Core.Avoidance.InCriticalAvoidance(Position);
        public bool IsFacingPlayer => TargetUtil.IsFacing(this, Core.Player.Position, 30f);
        public bool IsIgnored => TargetCategory == TargetCategory.Ignore;
        public bool IsAvoidanceOnPath => Core.Avoidance.Grid.IsIntersectedByFlags(Position, Core.Player.Position, AvoidanceFlags.Avoidance);
        public bool IsCriticalAvoidanceOnPath => Core.Avoidance.Grid.IsIntersectedByFlags(Position, Core.Player.Position, AvoidanceFlags.CriticalAvoidance);

        /// <summary>
        /// [Severely costly method] If a path can be made by DB's Navigator to the actor 
        /// </summary>
        public bool IsNavigable() => Core.DBGridProvider.Height > 0 && Core.DBNavProvider.CanPathWithinDistance(Position, 15f).Result;

        public virtual void OnUnitDeath()
        {
            if (IsExcludedId || IsExcludedType || IsSameTeam)
                return;

            // this is not reliable for trash due to OnDestroyed()
            // disposing objects at max distance who havent really died    
            // should be sufficient for progression globe waiting though.        

            if (IsElite)
            {
                Core.Logger.Log(LogCategory.Targetting, $"Elite Died: {Name} Acd={AcdId} Sno={ActorSnoId} Size={MonsterSize} Race={MonsterRace} Quality={MonsterQuality} Type={MonsterType} Affixes={MonsterAffixes} CollisionRadius={CollisionRadius} AxialRadius={AxialRadius} SphereRadius={Radius} RiftValue={RiftValuePct}");
            }
            //else
            //    Core.Logger.Log($"Unit Died: {Name} Acd={AcdId} Sno={ActorSnoId}");

            ActorEvents.FireUnitKilled(this);
        }

        public void AddCacheInfo(string reason)
        {
            if (string.IsNullOrEmpty(reason)) return;
            var seperator = CacheInfo?.Length > 0 ? ", " : string.Empty;
            CacheInfo += seperator + reason;
        }

        public override string ToString() => $"{InternalName} ({ActorSnoId}) Type={Type} Dist={Distance:N2}";

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

        public virtual ItemQuality GetItemQualityLevel()
        {
            return ItemQuality.Invalid;
        }

    }

}
