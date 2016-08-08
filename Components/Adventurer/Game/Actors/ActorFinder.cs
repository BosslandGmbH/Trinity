using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Game.Actors
{
    public static class ActorFinder
    {
        private static int _radiusChangeCount;
        public static int LowerSearchRadius(int currentSearchRadius)
        {
            var radius = currentSearchRadius;

            if (currentSearchRadius >= 1500)
            {
                radius = currentSearchRadius / 2;
            }
            else if(currentSearchRadius >= 500)
            {
                radius =  currentSearchRadius - 100;
            }
            else if(currentSearchRadius >= 100)
            {
                radius = currentSearchRadius - 50;
            }
            else if (currentSearchRadius > 20)
            {
                radius = currentSearchRadius - 10;
            }

            if (_radiusChangeCount >= 2)
            {
                Util.Logger.Info("[ActorFinder] It looks like we couldn't path to the destination, lowering the search radius (New Radius = {0})", radius);
                _radiusChangeCount = 0;
            }
            _radiusChangeCount++;

            return radius;
        }

        public static Vector3 FindNearestHostileUnitInRadius(Vector3 center, float radius)
        {
            using (new PerformanceLogger())
            {
                try
                {
                    var actor = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Where(
                        u =>
                            u.IsValid &&
                            u.CommonData != null &&
                            u.CommonData.IsValid &&
                            center.Distance(u.Position) <= radius &&
                            AdvDia.MyPosition.Distance(u.Position) > CharacterSettings.Instance.KillRadius &&
                            u.IsHostile &&
                            !u.IsDead &&
                            u.HitpointsCurrent > 1 &&
                            !u.IsInvulnerable &&
                            u.CommonData.GetAttribute<int>(ActorAttributeType.Invulnerable) <= 0 &&
                            u.CommonData.GetAttribute<int>(ActorAttributeType.Untargetable) <= 0 &&
                            !u.Name.Contains("bloodSpring") &&
                            !u.Name.Contains("firewall") &&
                            !u.Name.Contains("FireGrate")

                            )
                        .OrderBy(u => center.Distance(u.Position))
                        .FirstOrDefault();
                    if (actor != null)
                    {
                        return actor.Position;
                    }

                }
                catch (Exception)
                {
                    return Vector3.Zero;
                }
                return Vector3.Zero;
            }
        }




        public static DiaUnit FindUnitByGuid(int acdGuid)
        {
            return ZetaDia.Actors.GetActorByACDId(acdGuid) as DiaUnit;
        }

        public static DiaObject FindObject(int actorId)
        {
            return ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Where(
            u =>
                u.IsValid &&
                u.CommonData != null &&
                u.CommonData.IsValid &&
                u.ActorSnoId == actorId
            ).OrderBy(u => u.Distance).FirstOrDefault();
        }

        public static DiaUnit FindUnit(int actorId)
        {
            return ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Where(
                u =>
                    u.IsValid &&
                    u.CommonData != null &&
                    u.CommonData.IsValid &&
                    u.ActorSnoId == actorId
                ).OrderBy(u => u.Distance).FirstOrDefault();
        }

        public static DiaGizmo FindGizmo(int actorId, Func<DiaGizmo, bool> func = null)
        {
            return ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).Where(
                u =>
                    u.IsValid &&
                    u.CommonData != null &&
                    u.CommonData.IsValid &&
                    u.ActorSnoId == actorId &&
                    (func == null || func(u))
                ).OrderBy(u => u.Position.DistanceSqr(AdvDia.MyPosition)).FirstOrDefault();
        }

        public static DiaGizmo FindGizmoByName(string actorName)
        {
            return ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).Where(
                u =>
                    u.IsValid &&
                    u.CommonData != null &&
                    u.CommonData.IsValid &&
                    u.Name == actorName
                ).OrderBy(u => u.Distance).FirstOrDefault();
        }

        public static DiaGizmo FindNearestDeathGate(List<int> ignoreList)
        {
            if (ignoreList == null)
            {
                ignoreList = new List<int>();
            }
            //328830
            if (ExplorationData.FortressLevelAreaIds.Contains(AdvDia.CurrentLevelAreaId) || ExplorationData.FortressWorldIds.Contains(AdvDia.CurrentWorldId))
            {
                var gizmo =
                    ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                        //                        .Where(u => u.IsValid && u.ActorSnoId == 328830 && !ignoreList.Contains(u.ACDId))
                        .Where(u => u.IsValid && u.ActorSnoId == 328830 && u.Distance < 200)
                        .OrderBy(u => u.Distance)
                        .FirstOrDefault();
                return gizmo != null ? gizmo as DiaGizmo : null;
            }
            return null;
        }

        public static bool IsFullyValid<T>(this T actor) where T : DiaObject
        {
            return actor.IsValid && actor.CommonData != null && actor.CommonData.IsValid;
        }

        private static readonly HashSet<SNOAnim> UsedGizmoAnimations = new HashSet<SNOAnim>
        {
            SNOAnim.caOut_Cage_Open,
            SNOAnim.X1_Westm_Ex_death, // 'wortham survivers' destructible gizmo
            SNOAnim.a3dun_Keep_Rope_Switch_open, //433385, Type: Gizmo, Name: px_Bounty_Ramparts_Camp_Switch-704, 
            SNOAnim.Prisioner_Stake_open, // templar inquisition bounty px_Wilderness_Camp_TemplarPrisoners-23542
            SNOAnim.a1dun_Crypts_Jar_of_Souls_Dead, //x1_westm_necro_jar_of_souls_camp_graveyard
            SNOAnim.a1dun_Crypts_Jar_of_Souls_Death_Backup, //x1_westm_necro_jar_of_souls_camp_graveyard
            SNOAnim.trOut_Wilderness_Cultist_SummoningMachine_A_death, //px_Highlands_Camp_ResurgentCult_Totem
        };

        public static bool IsInteractableQuestObject<T>(this T actor) where T : DiaObject
        {
            if (actor is DiaUnit)
            {
                return IsUnitInteractable(actor as DiaUnit);
            }
            if (actor is DiaGizmo)
            {
                return IsGizmoInteractable(actor as DiaGizmo);
            }
            return false;
        }

        public static bool IsUnitInteractable(DiaUnit unit)
        {
            if (!unit.IsFullyValid())
            {
                return false;
            }
            if ((unit.CommonData.MinimapVisibilityFlags & 0x80) != 0)
            {
                return true;
            }
            if (unit.CommonData.GetAttribute<int>(ActorAttributeType.MinimapActive) == 1)
            {
                return true;
            }
            if (unit.MarkerType != MarkerType.Invalid && unit.MarkerType != MarkerType.None && unit.MarkerType != MarkerType.Asterisk)
            {
                return true;
            }
            if (InteractWhitelist.Contains(unit.ActorSnoId))
            {
                return true;
            }
            return false;
        }

        private static readonly HashSet<int> GizmoInteractBlacklist = new HashSet<int>
                                                                      {
                                                                          (int)SNOActor.caOut_Boneyards_Dervish_Alter-10883,
                                                                          (int)SNOActor.caOut_Oasis_Mine_Entrance_A-26687,
                                                                      };

        private static readonly HashSet<int> GuardedGizmos = new HashSet<int>
                                                             {
                                                                 434366,
                                                                 430733,
                                                                 432259,
                                                                 432770,
                                                                 433051,
                                                                 433184,
                                                                 433385,
                                                                 433124,
                                                                 433402,
                                                                 433316,
                                                                 433246,
                                                                 433295
                                                             };

        private static readonly HashSet<int> CursedGizmos = new HashSet<int>
                                                            {
                                                                364601,
                                                                365097,
                                                                368169
                                                            };

        public static readonly HashSet<int> InteractWhitelist = new HashSet<int>
                                                            {
                                                                          RiftData.UrshiSNO,
                                                                301177,
                                                                363744,
                                                                93713,
                                                                331397,
                                                                183609,
                                                                289249,
                                                                // A5 - Bounty: Finding the Forgotten (368611)
                                                                309381,
                                                                309400,
                                                                309398,
                                                                309387,
                                                                309403,
                                                                309391,
                                                                309410,
                                                                309380,
                                                                // A5 - Bounty: A Shameful Death (368445)
                                                                321930,
                                                                // A2 - Bounty: Lost Treasure of Khan Dakab (346067)
                                                                175603,
                                                                328830, // Death Gate
                                                                114622,
                                                                // Guarded Gizmos
                                                                 434366,
                                                                 430733,
                                                                 432259,
                                                                 432770,
                                                                 433051,
                                                                 433184,
                                                                 433385,
                                                                 433124,
                                                                 433402,
                                                                 433316,
                                                                 433246,

                                                                 450222 // cursed shrine bounty

                                                            };

        public static bool IsGizmoInteractable(DiaGizmo gizmo)
        {
            if (!gizmo.IsFullyValid())
            {
                return false;
            }
            if (GuardedGizmos.Contains(gizmo.ActorSnoId) && (gizmo.HasBeenOperated || gizmo.CommonData.ChestOpen > 0))
            {
                return false;
            }            
            if (UsedGizmoAnimations.Contains(gizmo.CommonData.CurrentAnimation))
            {
                return false;
            }
            if (GuardedGizmos.Contains(gizmo.ActorSnoId) && !gizmo.HasBeenOperated && gizmo.CommonData.GetAttribute<int>(ActorAttributeType.Untargetable) != 1)
            {
                return true;
            }        
            if (gizmo is GizmoShrine && gizmo.CommonData.GizmoHasBeenOperated != 1 && gizmo.CommonData.GizmoState != 1) // Buggy Cursed Shrine
            {
                return true;
            }
            switch (gizmo.CommonData.GizmoType)
            {
                case GizmoType.Chest:
                    return gizmo.CommonData.ChestOpen == 0;
                case GizmoType.Portal:
                case GizmoType.DungeonPortal:
                    return !AdvDia.CurrentWorldMarkers.Any(m => m.Position.Distance(gizmo.Position) < 10 && EntryPortals.IsEntryPortal(AdvDia.CurrentWorldDynamicId, m.NameHash));
                case GizmoType.PortalDestination:
                case GizmoType.PoolOfReflection:
                case GizmoType.Headstone:
                case GizmoType.HealingWell:
                case GizmoType.PowerUp:                
                    return false;
                case GizmoType.LootRunSwitch:
                    return true;
                case GizmoType.Switch:
                    if (gizmo.ActorSnoId == 328830) return true;
                    break;
            }
            if (GizmoInteractBlacklist.Contains(gizmo.ActorSnoId))
            {
                return false;
            }
            //if (GizmoInteractWhitelist.Contains(gizmo.ActorSnoId))
            //{
            //    return true;
            //}
            if (gizmo.IsGizmoDisabledByScript || gizmo.CommonData.GetAttribute<int>(ActorAttributeType.Untargetable) == 1)
            {
                return false;
            }

            if (gizmo.HasBeenOperated)
            {
                return false;
            }
            //if (gizmo is GizmoLootContainer || gizmo is GizmoShrine || gizmo.CommonData.GizmoType == GizmoType.Switch)
            //{
            //    return true;
            //}
            if ((gizmo.CommonData.MinimapVisibilityFlags & 0x80) != 0)
            {
                return true;
            }
            if (gizmo.CommonData.GetAttribute<int>(ActorAttributeType.MinimapActive) == 1)
            {
                return true;
            }
            if (CursedGizmos.Contains(gizmo.ActorSnoId))
            {
                return true;
            }
            if (InteractWhitelist.Contains(gizmo.ActorSnoId))
            {
                return true;
            }

            return false;
        }


    }
}
