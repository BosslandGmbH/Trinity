using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Adventurer.Util;
using Trinity.Framework;
using Trinity.Framework.Objects.Memory.Attributes;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Vector3 = Zeta.Common.Vector3;

namespace Trinity.Cache.Properties
{
    /// <summary>
    /// Properties shared by many types of actors
    /// This class should update all values that are possible/reasonable/useful.
    /// DO NOT put settings or situational based exclusions in here, do that in weighting etc.
    /// </summary>
    public class CommonProperties : IPropertyCollection
    {
        private DateTime _lastUpdated = DateTime.MinValue;
        private DateTime _attributesLastUpdated = DateTime.MinValue;

        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(100);
        private static readonly Regex NameNumberTrimRegex = new Regex(@"-\d+$", RegexOptions.Compiled);

        public bool IsValid { get; set; } = true;

        public DateTime CreationTime { get; } = DateTime.UtcNow;

        public void ApplyTo(TrinityCacheObject target)
        {
            if (!this.IsFrozen && DateTime.UtcNow.Subtract(_lastUpdated) > UpdateInterval || this.IsMe)
            {
                Update(target);
            }

            target.ActorAttributes = this.ActorAttributes;
            target.Animation = this.Animation;
            target.AnimationNameLowerCase = this.AnimationNameLowerCase;
            target.AnimationState = this.AnimationState;
            target.IsBountyObjective = this.IsBountyObjective;
            target.IsMinimapActive = this.IsMiniMapActive;
            target.Position = this.Position;
            target.Distance = this.Distance;
            target.Radius = this.Radius;
            target.CollisionRadius = this.CollisionRadius;
            target.LastSeenTime = this.LastSeenTime;
            target.ObjectHash = this.ObjectHash;
            target.ActorSNO = this.ActorSnoId;
            target.InternalName = this.InternalName;
            target.InternalNameLowerCase = this.InternalNameLowerCase;
            target.ActorType = this.ActorType;
            target.GizmoType = this.GizmoType;
            target.IsObstacle = this.IsObstacle;
            target.IsExcludedId = this.IsExcludedId;
            target.GameBalanceID = this.GameBalanceId;
            target.GameBalanceType = this.GameBalanceType;
            target.Type = this.Type;
            target.AnnId = this.AnnId;
            target.ACDGuid = this.AcdId;
            target.WorldSnoId = this.WorldSnoId;
            target.HasBeenInLoS = this.HasBeenCastable;
            target.HasBeenWalkable = this.HasBeenWalkable;
            target.IsInLineOfSight = this.IsCastable;
            target.IsWalkable = this.IsWalkable;
            target.FastAttributeGroupId = this.FastAttributeGroupId;
            target.IsFrozen = this.IsFrozen;
            target.IsExcludedId = this.IsExcludedId;
            target.IsExcludedType = this.IsExcludedType;
            target.IsAllowedClientEffect = this.IsAllowedClientEffect;
            target.IsMe = this.IsMe;
            target.IsUnit = this.IsUnit;
            target.IsItem = this.IsItem;
            target.IsPlayer = this.IsPlayer;
            target.IsGizmo = this.IsGizmo;
            target.IsMonster = this.IsMonster;
            target.AxialRadius = this.AxialRadius;
        }

        public void OnCreate(TrinityCacheObject source)
        {
            var diaObject = source.Object;
            if (diaObject == null || !diaObject.IsValid)
                return;

            this.ActorSnoId = diaObject.ActorSnoId;
            this.ActorType = diaObject.ActorType;
            this.InternalName = NameNumberTrimRegex.Replace(diaObject.Name, "");

            this.IsExcludedId = DataDictionary.ExcludedActorIds.Contains(this.ActorSnoId) || DataDictionary.BlackListIds.Contains(this.ActorSnoId);
            if (this.IsExcludedId)
            {
                this.IsFrozen = true;
                return;
            }

            this.IsExcludedType = DataDictionary.ExcludedActorTypes.Contains(this.ActorType);
            if (this.IsExcludedType)
            {
                this.IsFrozen = true;
                return;
            }

            if (this.ActorType == ActorType.ClientEffect)
            {
                this.IsAllowedClientEffect = DataDictionary.AllowedClientEffects.Contains(this.ActorSnoId);
                if (!this.IsAllowedClientEffect)
                {
                    this.IsFrozen = true;
                    return;
                }
            }

            this.InternalNameLowerCase = InternalName.ToLower();
            this.GizmoType = diaObject.ActorInfo.GizmoType;
            this.IsObstacle = DataDictionary.NavigationObstacleIds.Contains(this.ActorSnoId) || DataDictionary.PathFindingObstacles.ContainsKey(this.ActorSnoId);
            this.WorldSnoId = TrinityPlugin.Player.WorldSnoId;
            this.Radius = diaObject.CollisionSphere.Radius;
            this.AxialRadius = diaObject.ActorInfo.AxialCylinder.Ax1;
            this.CollisionRadius = this.AxialRadius * 0.6f;

            var commonData = source.CommonData;
            var isValid = ActorType == ActorType.ClientEffect || commonData != null && commonData.IsValid && !commonData.IsDisposed;
            
            this.Type = CommonPropertyUtils.GetObjectType(
                isValid,
                this.ActorType,
                this.ActorSnoId,
                this.GizmoType,
                this.InternalName
            );

            this.ObjectHash = HashGenerator.GenerateObjecthash(
                this.ActorSnoId,
                this.Position,
                this.InternalName,
                this.Type
            );

            if (commonData == null || !commonData.IsValid)
                return;

            // Trinity attributes incurs an initial cost to cache the attributes structures and all values.
            // GetCachedAttribute<T>() is a straight dictionary lookup, 
            // GetAttribute<T>() updates the value before returning.

            this.AnnId = commonData.AnnId;
            this.GameBalanceId = commonData.GameBalanceId;
            this.GameBalanceType = commonData.GameBalanceType;

            this.IsMe = source.RActorGuid == TrinityPlugin.Player.RActorGuid;
            this.IsUnit = Type == TrinityObjectType.Unit || ActorType == ActorType.Monster || ActorType == ActorType.Player;
            this.IsItem = Type == TrinityObjectType.Item || ActorType == ActorType.Item;
            this.IsPlayer = Type == TrinityObjectType.Player || ActorType == ActorType.Player;
            this.IsGizmo = ActorType == ActorType.Gizmo;
            this.IsMonster = ActorType == ActorType.Monster;

            Update(source);
        }

        public void Update(TrinityCacheObject source)
        {
            _lastUpdated = DateTime.UtcNow;

            var diaObject = source.Object;
            if (diaObject == null)
                return;

            this.LastSeenTime = DateTime.UtcNow;
            this.Position = diaObject.Position;
            this.Distance = TrinityPlugin.Player.Position.Distance(this.Position);
            this.AcdId = diaObject.ACDId;

            // Check for an RActorGuid that was re-used for another object.
            var snoId = diaObject.ActorSnoId;
            if (snoId != this.ActorSnoId)
            {
                Logger.Warn($"SnoIds dont match for actor {diaObject.Name} ({snoId})/ {this.InternalName} ({this.ActorSnoId})");
                this.IsValid = false;
            }

            if (this.Distance > 100f)
                return;

            var commonData = source.CommonData;
            if (commonData == null || !commonData.IsValid || commonData.IsDisposed)
                return;

            var fagId = commonData.FastAttribGroupId;
            if ((fagId != this.FastAttributeGroupId || DateTime.UtcNow.Subtract(_attributesLastUpdated).TotalSeconds > 1) && fagId > 0)
            {
                this.FastAttributeGroupId = fagId;
                this.ActorAttributes = new ActorAttributes(fagId);                
                _attributesLastUpdated = DateTime.UtcNow;
            }

            this.Animation = commonData.CurrentAnimation;
            this.AnimationNameLowerCase = DataDictionary.GetAnimationNameLowerCase(this.Animation);
            this.AnimationState = commonData.AnimationState;

            if (ActorAttributes != null)
            {
                this.IsBountyObjective = this.ActorAttributes.IsBountyObjective;
                this.IsMiniMapActive = this.ActorAttributes.IsMinimapActive;
            }

            this.IsCastable = Core.Avoidance.Grid.CanRayCast(TrinityPlugin.Player.Position, this.Position);
            if (!this.HasBeenCastable && this.IsCastable)
                this.HasBeenCastable = true;

            if (this.IsCastable)
            {
                this.IsWalkable = Core.Avoidance.Grid.CanRayWalk(TrinityPlugin.Player.Position, this.Position);
                if (!this.HasBeenWalkable && this.IsWalkable)
                    this.HasBeenWalkable = true;
            }
            else
            {
                this.IsWalkable = false;
            }

        }

        public bool IsCorrupt { get; set; }

        public float AxialRadius { get; set; }
        public bool IsMe { get; set; }
        public bool IsUnit { get; set; }
        public bool IsItem { get; set; }
        public bool IsPlayer { get; set; }
        public bool IsGizmo { get; set; }
        public bool IsMonster { get; set; }
        public bool IsFrozen { get; set; }
        public int FastAttributeGroupId { get; set; }
        public bool IsExcludedType { get; set; }
        public bool IsAllowedClientEffect { get; set; }
        public bool IsWalkable { get; set; }
        public bool IsCastable { get; set; }
        public int WorldSnoId { get; set; }
        public string AnimationNameLowerCase { get; set; } = string.Empty;
        public int AnnId { get; set; }
        public bool HasBeenCastable { get; set; }
        public bool HasBeenWalkable { get; set; }
        public string ObjectHash { get; set; } = string.Empty;
        public int AcdId { get; set; }
        public float CollisionRadius { get; set; }
        public float Radius { get; set; }
        public float Distance { get; set; }
        public Vector3 Position { get; set; }
        public DateTime LastSeenTime { get; set; }
        public ActorAttributes ActorAttributes { get; set; }
        public SNOAnim Animation { get; set; }
        public AnimationState AnimationState { get; set; }
        public bool IsBountyObjective { get; set; }
        public bool IsMiniMapActive { get; set; }
        public bool IsExcludedId { get; set; }
        public bool IsObstacle { get; set; }
        public TrinityObjectType Type { get; set; }
        public int ActorSnoId { get; set; }
        public ActorType ActorType { get; set; } = ActorType.Invalid;
        public GizmoType GizmoType { get; set; } 
        public string InternalNameLowerCase { get; set; } = string.Empty;
        public string InternalName { get; set; } = string.Empty;
        public int GameBalanceId { get; set; }
        public GameBalanceType GameBalanceType { get; set; }
    }

    public class CommonPropertyUtils
    {
        public static TrinityObjectType GetObjectType(bool isValid, ActorType actorType, int actorSNO, GizmoType gizmoType, string internalName)
        {
            if (!isValid)
                return TrinityObjectType.Unknown;

            if (DataDictionary.ObjectTypeOverrides.ContainsKey(actorSNO))
                return DataDictionary.ObjectTypeOverrides[actorSNO];

            if (DataDictionary.CursedChestSNO.Contains(actorSNO))
                return TrinityObjectType.CursedChest;

            if (DataDictionary.CursedShrineSNO.Contains(actorSNO))
                return TrinityObjectType.CursedShrine;

            if (DataDictionary.ShrineSNO.Contains(actorSNO))
                return TrinityObjectType.Shrine;

            if (DataDictionary.HealthGlobeSNO.Contains(actorSNO))
                return TrinityObjectType.HealthGlobe;

            if (DataDictionary.PowerGlobeSNO.Contains(actorSNO))
                return TrinityObjectType.PowerGlobe;

            if (DataDictionary.ProgressionGlobeSNO.Contains(actorSNO))
                return TrinityObjectType.ProgressionGlobe;

            if (DataDictionary.GoldSNO.Contains(actorSNO))
                return TrinityObjectType.Gold;

            if (DataDictionary.BloodShardSNO.Contains(actorSNO))
                return TrinityObjectType.BloodShard;

            if (actorType == ActorType.Item || DataDictionary.ForceToItemOverrideIds.Contains(actorSNO))
                return TrinityObjectType.Item;

            if (DataDictionary.AvoidanceSNO.Contains(actorSNO))
                return TrinityObjectType.Avoidance;

            if (DataDictionary.ForceTypeAsBarricade.Contains(actorSNO))
                return TrinityObjectType.Barricade;

            if (actorType == ActorType.Monster)
                return TrinityObjectType.Unit;

            if (actorType == ActorType.Gizmo)
            {
                switch (gizmoType)
                {
                    case GizmoType.HealingWell:
                        return TrinityObjectType.HealthWell;

                    case GizmoType.Door:
                        return TrinityObjectType.Door;

                    case GizmoType.BreakableDoor:
                        return TrinityObjectType.Barricade;

                    case GizmoType.PoolOfReflection:
                    case GizmoType.PowerUp:
                        return TrinityObjectType.Shrine;

                    case GizmoType.Chest:
                        return TrinityObjectType.Container;

                    case GizmoType.DestroyableObject:
                    case GizmoType.BreakableChest:
                        return TrinityObjectType.Destructible;

                    case GizmoType.PlacedLoot:
                    case GizmoType.Switch:
                    case GizmoType.Headstone:
                        return TrinityObjectType.Interactable;

                    case GizmoType.Portal:
                        return TrinityObjectType.Portal;
                }
            }

            if (actorType == ActorType.Environment || actorType == ActorType.Critter || actorType == ActorType.ServerProp)
                return TrinityObjectType.Environment;

            if (actorType == ActorType.Projectile)
                return TrinityObjectType.Projectile;

            if (DataDictionary.BuffedLocationSno.Contains(actorSNO))
                return TrinityObjectType.BuffedRegion;

            if (actorType == ActorType.ClientEffect)
                return TrinityObjectType.ClientEffect;

            if (actorType == ActorType.Player)
                return TrinityObjectType.Player;

            if (DataDictionary.PlayerBannerSNO.Contains(actorSNO))
                return TrinityObjectType.Banner;

            if (internalName != null && internalName.StartsWith("Waypoint-"))
                return TrinityObjectType.Waypoint;

            return TrinityObjectType.Unknown;
        }

        public static TrinityObjectType GetObjectType(TrinityCacheObject obj)
        {
            return GetObjectType(
                obj.IsValid,
                obj.ActorType,
                obj.ActorSNO,
                obj.GizmoType,
                obj.InternalName
                );
        }
    }
}
