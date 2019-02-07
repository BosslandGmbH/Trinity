using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Zeta.Bot.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;
using Zeta.Game.Internals.SNO;

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
            else if (currentSearchRadius >= 500)
            {
                radius = currentSearchRadius - 100;
            }
            else if (currentSearchRadius >= 100)
            {
                radius = currentSearchRadius - 50;
            }
            else if (currentSearchRadius > 20)
            {
                radius = currentSearchRadius - 10;
            }

            if (_radiusChangeCount >= 2)
            {
                Core.Logger.Log("[ActorFinder] It looks like we couldn't path to the destination, lowering the search radius (New Radius = {0})", radius);
                _radiusChangeCount = 0;
            }
            _radiusChangeCount++;

            return radius;
        }

        public static Vector3 FindNearestHostileUnitInRadius(Vector3 center, float radius)
        {
            using (new PerformanceLogger("FindNearestHostileUnitInRadius"))
            {
                try
                {
                    DiaUnit actor = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Where(
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

        public static TrinityActor FindActor(SNOActor actorId, int marker = 0, float maxRange = 500, string internalName = "", Func<TrinityActor, bool> condition = null)
        {
            TrinityActor actor = null;
            if (actorId != 0)
            {
                if (marker != 0)
                {
                    actor = BountyHelpers.ScanForActor(actorId, marker, (int)maxRange, condition);
                }
                else
                {
                    actor = BountyHelpers.ScanForActor(actorId, (int)maxRange, condition);
                }
            }
            else if (!string.IsNullOrEmpty(internalName))
            {
                actor = BountyHelpers.ScanForActor(internalName, (int)maxRange, condition);
            }
            else if (actorId == 0 && marker != 0)
            {
                actor = BountyHelpers.GetActorNearMarker(marker, 10f, condition);
            }
            return actor;
        }

        public static DiaObject FindObject(SNOActor actorId)
        {
            return ZetaDia.Actors.GetActorsOfType<DiaObject>(true).Where(
            u =>
                u.IsValid &&
                u.CommonData != null &&
                u.CommonData.IsValid &&
                u.ActorSnoId == actorId
            ).OrderBy(u => u.Distance).FirstOrDefault();
        }

        public static DiaUnit FindUnit(SNOActor actorId)
        {
            return ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Where(
                u =>
                    u.IsValid &&
                    u.CommonData != null &&
                    u.CommonData.IsValid &&
                    u.ActorSnoId == actorId
                ).OrderBy(u => u.Distance).FirstOrDefault();
        }

        public static DiaGizmo FindGizmo(SNOActor actorId, Func<DiaGizmo, bool> func = null)
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

        public static bool IsDeathGate(DiaObject o)
        {
            return o != null && o.IsValid && o.ActorSnoId == SNOActor.x1_Fortress_Portal_Switch;
        }

        public static DiaGizmo FindNearestDeathGate()
        {
            ////328830
            //if (!ExplorationData.FortressLevelAreaIds.Contains(AdvDia.CurrentLevelAreaId) &&
            //    !ExplorationData.FortressWorldIds.Contains(AdvDia.CurrentWorldId))
            //    return null;

            DiaObject gizmo = ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                    .Where(u => u.IsValid && u.ActorSnoId == SNOActor.x1_Fortress_Portal_Switch && u.Distance < 200)
                    .OrderBy(u => u.Distance)
                    .FirstOrDefault();

            return gizmo as DiaGizmo;
        }

        public static DiaGizmo FindNearestDeathGateToPosition(Vector3 position, Dictionary<Vector3, DateTime> ignoreList)
        {
            if (ignoreList == null)
            {
                ignoreList = new Dictionary<Vector3, DateTime>();
            }
            //328830
            if (ExplorationData.FortressLevelAreaIds.Contains(AdvDia.CurrentLevelAreaId) || ExplorationData.FortressWorldIds.Contains(AdvDia.CurrentWorldId))
            {
                DiaObject gizmo =
                    ZetaDia.Actors.GetActorsOfType<DiaObject>(true)
                        .Where(u => u.IsValid && u.ActorSnoId == SNOActor.x1_Fortress_Portal_Switch && !NavigationCoroutine.IsDeathGateIgnored(u.Position, ignoreList))
                        .OrderBy(u => u.Position.Distance(position))
                        .FirstOrDefault();
                return gizmo as DiaGizmo;
            }
            return null;
        }

        //public static Func<DiaObject, bool> CanPathTo = actor
        //    => actor.Distance < 10f || ((AdvDia.Navigator as Navigator)?.CanPathWithinDistance(actor.Position, 10f).Result ?? false);

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

        public static bool IsInteractableQuestObject(this TrinityActor actor)
        {
            DiaObject a = ZetaDia.Actors.GetActorByACDId(actor.AcdId);
            return a?.IsInteractableQuestObject() ?? false;
        }

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

        private static readonly HashSet<SNOActor> GizmoInteractBlacklist = new HashSet<SNOActor>
        {
            SNOActor.caOut_Boneyards_Dervish_Alter - 10883,
            SNOActor.caOut_Oasis_Mine_Entrance_A - 26687,
        };

        private static readonly HashSet<SNOActor> GuardedGizmos = new HashSet<SNOActor>
        {
            SNOActor.px_Leorics_Camp_WorthamMilitia_Ex,
            SNOActor.px_Wilderness_Camp_TemplarPrisoners,
            SNOActor.px_Highlands_Camp_ResurgentCult_Totem,
            SNOActor.px_SpiderCaves_Camp_Cocoon,
            SNOActor.px_caOut_Cage_BountyCamp,
            SNOActor.px_Bridge_Camp_LostPatrol,
            SNOActor.px_Bounty_Ramparts_Camp_Switch,
            SNOActor.px_Bounty_Camp_TrappedAngels,
            SNOActor.px_Bounty_Camp_Hellportals_Frame,
            SNOActor.X1_westm_Necro_Jar_of_Souls_Camp_graveyard,
            SNOActor.px_Bounty_Death_Orb_Little,
            SNOActor.px_Bounty_Camp_azmodan_fight_spawner
        };

        private static readonly HashSet<SNOActor> CursedGizmos = new HashSet<SNOActor>
                                                            {
                                                                (SNOActor)364601,
                                                                SNOActor.x1_Global_Chest_CursedChest_B,
                                                                (SNOActor)368169
                                                            };

        public static readonly HashSet<SNOActor> InteractWhitelist = new HashSet<SNOActor>
        {
            RiftData.UrshiSNO,
            SNOActor.x1_PandExt_Time_Activator,
            SNOActor.X1_LR_Nephalem,
            SNOActor.a1dun_Crypts_Jar_of_Souls,
            SNOActor.x1_westm_Graveyard_Floor_Sarcophagus_Undead_Husband_Event,
            SNOActor.caldeumTortured_Poor_Male_A_ZakarwaPrisoner,
            SNOActor.x1_Westm_Corpse_A_01,
            // A5 - Bounty: Finding the Forgotten (368611)
            SNOActor.x1_Westm_Corpse_C_01,
            SNOActor.x1_Westm_Corpse_D_02,
            SNOActor.x1_Westm_Corpse_C_06,
            SNOActor.x1_Westm_Corpse_A_05,
            SNOActor.x1_Westm_Corpse_D_05,
            SNOActor.x1_Westm_Corpse_B_04,
            SNOActor.x1_Westm_Corpse_E_06,
            SNOActor.x1_Westm_Corpse_B_01,
            // A5 - Bounty: A Shameful Death (368445)
            SNOActor.x1_westmarchGuard_CaptainStokely_Event,
            // A2 - Bounty: Lost Treasure of Khan Dakab (346067)
            SNOActor.a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle,
            SNOActor.x1_Fortress_Portal_Switch, // Death Gate
            SNOActor.Tyrael_Heaven,
            // Guarded Gizmos
            SNOActor.px_Leorics_Camp_WorthamMilitia_Ex,
            SNOActor.px_Wilderness_Camp_TemplarPrisoners,
            SNOActor.px_Highlands_Camp_ResurgentCult_Totem,
            SNOActor.px_SpiderCaves_Camp_Cocoon,
            SNOActor.px_caOut_Cage_BountyCamp,
            SNOActor.px_Bridge_Camp_LostPatrol,
            SNOActor.px_Bounty_Ramparts_Camp_Switch,
            SNOActor.px_Bounty_Camp_TrappedAngels,
            SNOActor.px_Bounty_Camp_Hellportals_Frame,
            SNOActor.X1_westm_Necro_Jar_of_Souls_Camp_graveyard,
            SNOActor.px_Bounty_Death_Orb_Little,

            SNOActor.P4_BountyGrounds_CursedShrine_A5 // cursed shrine bounty
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
            if (GuardedGizmos.Contains(gizmo.ActorSnoId) && !gizmo.HasBeenOperated &&
                gizmo.CommonData.GetAttribute<int>(ActorAttributeType.Untargetable) != 1)
            {
                return true;
            }
            if (gizmo is GizmoShrine && gizmo.CommonData.GizmoHasBeenOperated != 1 && gizmo.CommonData.GizmoState != 1)
            // Buggy Cursed Shrine
            {
                return true;
            }
            switch (gizmo.CommonData.GizmoType)
            {
                case GizmoType.Chest:
                    return gizmo.CommonData.ChestOpen == 0;

                case GizmoType.Portal:
                case GizmoType.DungeonPortal:
                    return
                        !AdvDia.CurrentWorldMarkers.Any(
                            m =>
                                m.Position.Distance(gizmo.Position) < 10 &&
                                EntryPortals.IsEntryPortal(AdvDia.CurrentWorldDynamicId, m.NameHash));
                case GizmoType.PortalDestination:
                case GizmoType.PoolOfReflection:
                case GizmoType.Headstone:
                case GizmoType.HealingWell:
                case GizmoType.PowerUp:
                    return false;

                case GizmoType.LootRunSwitch:
                case GizmoType.MultiClick:
                    return true;

                case GizmoType.Switch:
                    if (gizmo.ActorSnoId == SNOActor.x1_Fortress_Portal_Switch) return true;
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
            if (gizmo.IsGizmoDisabledByScript ||
                gizmo.CommonData.GetAttribute<int>(ActorAttributeType.Untargetable) == 1)
            {
                return false;
            }
            if (gizmo.ActorSnoId == SNOActor.p43_AD_Valor_Pedestal && gizmo.CommonData?.MinimapActive == 1)
            {
                return true; //p43_AD_Valor_Pedestal-19848 Special Valor Event in Diablo1 Area
            }

            if (gizmo.HasBeenOperated)
            {
                return false;
            }

            //if (gizmo is GizmoLootContainer || gizmo is GizmoShrine || gizmo.CommonData.GizmoType == GizmoType.Switch)
            //{
            //    return true;
            //}
            if

            ((gizmo.CommonData.MinimapVisibilityFlags & 0x80) != 0)
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