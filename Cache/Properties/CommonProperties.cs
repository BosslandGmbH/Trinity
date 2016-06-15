using System;
using System.Linq;
using System.Text.RegularExpressions;
using Trinity.Framework;
using Trinity.Framework.Objects.Memory.Attributes;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Vector3 = Zeta.Common.Vector3;

namespace Trinity.Cache.Properties
{
    /// <summary>
    /// PropertyLoader shared by many types
    /// </summary>
    public class CommmonProperties : PropertyLoader.IPropertyCollection
    {
        private DateTime _lastUpdated = DateTime.MinValue;
        private static TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(100);
        private static readonly Regex NameNumberTrimRegex = new Regex(@"-\d+$", RegexOptions.Compiled);

        public void ApplyTo(TrinityCacheObject target)
        {
            if (DateTime.UtcNow.Subtract(_lastUpdated).TotalMilliseconds > UpdateInterval.TotalMilliseconds && target.IsValid)
                Update(target);

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
            target.InternalNameLowerCase = this.InternalName.ToLower();
            target.ActorType = this.ActorType;
            target.GizmoType = this.GizmoType;
            target.IsObstacle = this.IsObstacle;
            target.IsIgnoreName = this.IsIgnoreName;
            target.GameBalanceID = this.GameBalanceId;
            target.GameBalanceType = this.GameBalanceType;
            target.Type = this.Type;
            target.AnnId = this.AnnId;
            target.ACDGuid = this.AcdId;
            target.WorldSnoId = this.WorldSnoId;
        }

        public void OnCreate(TrinityCacheObject source)
        {
            var diaObject = source.Object;
            if (diaObject == null)
                return;

            this.ActorSnoId = diaObject.ActorSnoId;
            this.ActorType = diaObject.ActorType;
            this.InternalName = NameNumberTrimRegex.Replace(diaObject.Name, "");
            this.InternalNameLowerCase = InternalName.ToLower();
            this.GizmoType = diaObject.ActorInfo.GizmoType;
            this.IsObstacle = DataDictionary.NavigationObstacleIds.Contains(this.ActorSnoId) || DataDictionary.PathFindingObstacles.ContainsKey(this.ActorSnoId);

            this.Type = CommonPropertyUtils.GetObjectType(
                source.IsValid,
                this.ActorType,
                this.ActorSnoId,
                this.GizmoType,
                this.InternalName
            );

            this.IsIgnoreName = DataDictionary.ActorIgnoreNames.Any(n => this.InternalNameLowerCase.StartsWith(n));

            if (source.CommonData == null)
                return;

            this.AnnId = source.CommonData.AnnId;
            this.GameBalanceId = source.CommonData.GameBalanceId;
            this.GameBalanceType = source.CommonData.GameBalanceType;
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
            this.Radius = diaObject.CollisionSphere.Radius;
            this.CollisionRadius = diaObject.ActorInfo.AxialCylinder.Ax1*0.6f;
            this.ObjectHash = HashGenerator.GenerateObjecthash(source);
            this.AcdId = diaObject.ACDId;
            this.WorldSnoId = TrinityPlugin.Player.WorldSnoId;

            var commonData = source.CommonData;
            if (commonData == null)
                return;
            
            this.ActorAttributes = new ActorAttributes(commonData.FastAttribGroupId);
            this.Animation = commonData.CurrentAnimation;
            this.AnimationNameLowerCase = DataDictionary.GetAnimationNameLowerCase(this.Animation);
            this.AnimationState = commonData.AnimationState;
            //this.IsBountyObjective = source.CommonData.BountyObjective > 0; // 0.200ms
            this.IsMiniMapActive = commonData.MinimapActive > 0;

            if (!this.HasBeenWalkable)
                this.HasBeenWalkable = Core.Avoidance.Grid.CanRayWalk(TrinityPlugin.Player.Position, this.Position);

            if (!this.HasBeenCastable)
                this.HasBeenCastable = Core.Avoidance.Grid.CanRayCast(TrinityPlugin.Player.Position, this.Position);            
        }

        public int WorldSnoId { get; set; }
        public string AnimationNameLowerCase { get; set; }
        public int AnnId { get; set; }
        public bool HasBeenCastable { get; set; }
        public bool HasBeenWalkable { get; set; }
        public bool IsBlacklisted { get; set; }
        public string ObjectHash { get; set; }
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
        public bool IsIgnoreName { get; set; }
        public bool IsObstacle { get; set; }
        public TrinityObjectType Type { get; set; }
        public int ActorSnoId { get; set; }
        public ActorType ActorType { get; set; }
        public GizmoType GizmoType { get; set; }
        public string InternalNameLowerCase { get; set; }
        public string InternalName { get; set; }
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

            if (Core.Avoidance.ActiveAvoidanceSnoIds.Contains(actorSNO) || DataDictionary.AvoidanceSNO.Contains(actorSNO))
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

            if (actorType == ActorType.ClientEffect)
                return TrinityObjectType.Effect;

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
