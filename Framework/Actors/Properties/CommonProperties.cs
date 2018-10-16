using System;
using System.Linq;
using System.Text.RegularExpressions;
using Trinity.Components.Combat;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;


namespace Trinity.Framework.Actors.Properties
{
    public class CommonProperties
    {
        internal static void Populate(TrinityActor actor)
        {
            UpdateLineOfSight(actor);
        }

        public static void Update(TrinityActor actor)
        {
            UpdateLineOfSight(actor);
        }

        public static void UpdateLineOfSight(TrinityActor actor)
        {
            if (actor.ActorType == ActorType.Item)
                return;

            var grid = TrinityGrid.GetUnsafeGrid();
            if (grid == null)
                return;

            if (actor.Position != Vector3.Zero && grid.GridBounds != 0)
            {
                var inLineOfSight = grid.CanRayCast(Core.Player.Position, actor.Position);
                actor.IsInLineOfSight = inLineOfSight;

                if (!actor.HasBeenInLoS && inLineOfSight)
                    actor.HasBeenInLoS = true;

                if (inLineOfSight)
                {
                    actor.IsWalkable = grid.CanRayWalk(actor);

                    if (actor.IsWalkable)
                        actor.HasBeenWalkable = true;
                }
                else
                {
                    actor.IsWalkable = false;
                }
            }
        }

        public static TrinityObjectType GetObjectType(ActorType actorType, int actorSno, GizmoType gizmoType, string internalName)
        {
            if (GameData.ObjectTypeOverrides.ContainsKey(actorSno))
                return GameData.ObjectTypeOverrides[actorSno];

            if (GameData.CursedChestSNO.Contains(actorSno))
                return TrinityObjectType.CursedChest;

            if (GameData.CursedShrineSNO.Contains(actorSno))
                return TrinityObjectType.CursedShrine;

            if (GameData.ShrineSNO.Contains(actorSno))
                return TrinityObjectType.Shrine;

            if (GameData.HealthGlobeSNO.Contains(actorSno))
                return TrinityObjectType.HealthGlobe;

            if (GameData.PowerGlobeSNO.Contains(actorSno))
                return TrinityObjectType.PowerGlobe;

            if (GameData.ProgressionGlobeSNO.Contains(actorSno))
                return TrinityObjectType.ProgressionGlobe;

            if (GameData.GoldSNO.Contains(actorSno))
                return TrinityObjectType.Gold;

            if (GameData.BloodShardSNO.Contains(actorSno))
                return TrinityObjectType.BloodShard;

            if (actorType == ActorType.Item || GameData.ForceToItemOverrideIds.Contains(actorSno))
                return TrinityObjectType.Item;

            if (GameData.AvoidanceSNO.Contains(actorSno))
                return TrinityObjectType.Avoidance;

            if (GameData.ForceTypeAsBarricade.Contains(actorSno))
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
                    case GizmoType.LoreChest: // D1 profile Lectern switch to open door
                    case GizmoType.Switch:
                    case GizmoType.Headstone:
                        return TrinityObjectType.Interactable;

                    case GizmoType.Portal:
                        return TrinityObjectType.Portal;

                    case GizmoType.Gate:
                        return TrinityObjectType.Gate;
                }
            }

            if (actorType == ActorType.Environment || actorType == ActorType.Critter || actorType == ActorType.ServerProp)
                return TrinityObjectType.Environment;

            if (actorType == ActorType.Projectile)
                return TrinityObjectType.Projectile;

            if (GameData.BuffedLocationSno.Contains(actorSno))
                return TrinityObjectType.BuffedRegion;

            if (actorType == ActorType.ClientEffect)
                return TrinityObjectType.ClientEffect;

            if (actorType == ActorType.Player)
                return TrinityObjectType.Player;

            if (GameData.PlayerBannerSNO.Contains(actorSno))
                return TrinityObjectType.Banner;

            if (internalName != null && internalName.StartsWith("Waypoint-"))
                return TrinityObjectType.Waypoint;

            return TrinityObjectType.Unknown;
        }

        public static SpecialTypes GetSpecialType(TrinityActor cacheObject)
        {
            if (cacheObject.ActorSnoId == 4860) //SNOActor.PlayerHeadstone
                return SpecialTypes.PlayerTombstone;

            return SpecialTypes.None;
        }

        public static TrinityObjectType GetObjectType(TrinityActor obj)
        {
            return GetObjectType(
                obj.ActorType,
                obj.ActorSnoId,
                obj.GizmoType,
                obj.InternalName
            );
        }

        public static float GetRequiredRange(TrinityActor actor)
        {
            if (GameData.CustomObjectRadius.TryGetValue(actor.ActorSnoId, out var result))
                return result;

            switch (actor.Type)
            {
                // * Unit, we need to pick an ability to use and get within range
                case TrinityObjectType.Unit:
                {
                    if (actor.IsHidden || actor.IsQuestMonster)
                        result = actor.CollisionRadius + 1;

                    else
                    {
                        if (TrinityCombat.Targeting.CurrentPower != null)
                            result = Math.Max(TrinityCombat.Targeting.CurrentPower.MinimumRange, actor.CollisionRadius + 1);
                        else
                            result = actor.CollisionRadius + 1;
                    }
                    break;
                }

                // * Item - need to get within 6 feet and then interact with it
                case TrinityObjectType.Item:
                {
                    result = 5f;
                    break;
                }

                // * Gold - need to get within pickup radius only
                case TrinityObjectType.Gold:
                {
                    result = 2f;
                    break;
                }

                // * Globes - need to get within pickup radius only
                case TrinityObjectType.PowerGlobe:
                case TrinityObjectType.HealthGlobe:
                case TrinityObjectType.ProgressionGlobe:
                {
                    result = 2f;
                    break;
                }

                // * Shrine & Container - need to get within 8 feet and interact
                case TrinityObjectType.HealthWell:
                {
                    result = 4f;
                    break;
                }

                case TrinityObjectType.Shrine:
                case TrinityObjectType.Container:
                {
                    result = 6f;
                    break;
                }

                case TrinityObjectType.Interactable:
                {
                    result = 5f;
                    break;
                }

                // * Destructible - need to pick an ability and attack it
                case TrinityObjectType.Destructible:
                {
                    result = actor.CollisionRadius;
                    break;
                }

                case TrinityObjectType.Barricade:
                {
                    result = actor.AxialRadius * 0.8f;
                    break;
                }

                // * Avoidance - need to pick an avoid location and move there
                case TrinityObjectType.Avoidance:
                {
                    result = 2f;
                    break;
                }
                case TrinityObjectType.Door:
                    result = Math.Max(2f, actor.AxialRadius);
                    break;
                default:
                    result = actor.Radius;
                    break;
            }

            return result;
        }
    }
}
