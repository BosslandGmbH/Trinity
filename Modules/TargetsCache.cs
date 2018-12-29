using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Components.Coroutines;
using Trinity.Components.Coroutines.Town;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;


namespace Trinity.Modules
{
    /// <summary>
    /// Creates a list of good target candidates (removing all the rubbish actors) to reduce combat processing time for weighting and target selection.
    /// </summary>
    public class TargetsCache : Module, IEnumerable<TrinityActor>
    {



        public ulong LastUpdatedTick;
        public List<TrinityActor> Entries = new List<TrinityActor>();
        public List<TrinityActor> Ignored = new List<TrinityActor>();
        public ILookup<TrinityObjectType, TrinityActor> ByType = EmptyLookup<TrinityObjectType, TrinityActor>.Instance;
        public ILookup<MonsterQuality, TrinityActor> ByMonsterQuality = EmptyLookup<MonsterQuality, TrinityActor>.Instance;

        protected override void OnPulse()
        {
            Update();
        }

        public void Update()
        {
            using (new PerformanceLogger("ActorCache.Update"))
            {
                if (!ZetaDia.IsInGame || ZetaDia.Service.Hero == null || !ZetaDia.Service.Hero.IsValid)
                    return;

                if (ZetaDia.Me == null || !ZetaDia.Me.IsValid)
                    return;

                var lastUpdatedTick = ZetaDia.Memory.Executor.FrameCount;
                if (LastUpdatedTick == lastUpdatedTick) return;
                LastUpdatedTick = lastUpdatedTick;

                var included = new List<TrinityActor>();
                var ignored = new List<TrinityActor>();

                foreach (var actor in Core.Actors.OfType<TrinityActor>())
                {
                    try
                    {
                        actor.CacheInfo = string.Empty;

                        if (ShouldTargetActor(actor))
                        {
                            actor.TargetCategory = TargetCategory.Normal;
                            included.Add(actor);
                        }
                        else
                        {
                            actor.TargetCategory = TargetCategory.Ignore;
                            ignored.Add(actor);
                        }
                    }
                    catch (Exception ex)
                    {
                        Core.Logger.Error($"Exception updating object cache {actor.Name} {actor.ActorSnoId} {ex}");
                        Clear();
                        return;
                    }
                }

                //if (Core.Settings.Advanced.LogCategories.HasFlag(LogCategory.CacheManagement))
                //{
                //    foreach (var o in included)
                //    {
                //        Core.Logger.Debug($"[Cache][{o.CacheTime:N4}] Added {o}");
                //    }

                //    foreach (var o in ignored)
                //    {
                //        Core.Logger.Debug($"[Cache][{o.CacheTime:N4}] Ignored {o.InternalName} RActorGuid={o.RActorId} ActorSnoId={o.ActorSnoId} Type={o.ActorType} CacheInfo={o.CacheInfo}");
                //    }                   
                //}

                ByType = included.ToLookup(k => k.Type);
                ByMonsterQuality = included.ToLookup(k => k.MonsterQuality);

                Entries = included;
                Ignored = ignored;
            }
        }



        private bool ShouldTargetActor(TrinityActor cacheObject)
        {
            if (cacheObject == null)
            {
                Core.Logger.Error("NullObject");
                return false;
            }

            if (!ShouldCacheCommon(cacheObject))
                return false;

            switch (cacheObject.ActorType)
            {
                case ActorType.ClientEffect:
                    if (cacheObject.IsAllowedClientEffect)
                        return true;

                    cacheObject.AddCacheInfo("IgnoreClientEffect");
                    return false;

                default:
                    if (cacheObject.AcdId == -1)
                    {
                        cacheObject.AddCacheInfo("InvalidCommonData");
                        return false;
                    }
                    break;
            }

            switch (cacheObject.Type)
            {
                case TrinityObjectType.Player:
                case TrinityObjectType.PowerGlobe:
                case TrinityObjectType.HealthGlobe:
                case TrinityObjectType.ProgressionGlobe:
                case TrinityObjectType.BuffedRegion:
                case TrinityObjectType.Gate:
                case TrinityObjectType.BloodShard:
                    return true;

                case TrinityObjectType.Unit:
                    return ShouldCacheUnit(cacheObject);

                case TrinityObjectType.Item:
                    return ShouldCacheItem(cacheObject as TrinityItem);

                case TrinityObjectType.Gold:
                    return ShouldIncludeGold(cacheObject as TrinityItem);

                case TrinityObjectType.Avoidance:
                    cacheObject.AddCacheInfo("Avoidance");
                    return false;

                case TrinityObjectType.Destructible:
                case TrinityObjectType.Door:
                case TrinityObjectType.Barricade:
                case TrinityObjectType.Container:
                case TrinityObjectType.Shrine:
                case TrinityObjectType.Interactable:
                case TrinityObjectType.HealthWell:
                case TrinityObjectType.CursedChest:
                case TrinityObjectType.CursedShrine:
                    return ShouldCacheGizmo(cacheObject);

                default:
                    cacheObject.AddCacheInfo($"Unhandled TrinityObjectType");
                    return false;
            }
        }

        private static bool IsCorpulent(TrinityActor cacheObject)
        {
            switch ((SNOActor)cacheObject.ActorSnoId)
            {
                case SNOActor.Corpulent_A:
                case SNOActor.Corpulent_A_Unique_01:
                case SNOActor.Corpulent_A_Unique_02:
                case SNOActor.Corpulent_A_Unique_03:
                case SNOActor.Corpulent_B:
                case SNOActor.Corpulent_B_Unique_01:
                case SNOActor.Corpulent_C:
                case SNOActor.Corpulent_C_OasisAmbush_Unique:
                case SNOActor.Corpulent_D:
                case SNOActor.Corpulent_D_CultistSurvivor_Unique:
                case SNOActor.Corpulent_D_Unique_Spec_01:
                case SNOActor.Corpulent_Frost_A:
                case SNOActor.Corpulent_suicide_blood:
                case SNOActor.Corpulent_suicide_frost:
                case SNOActor.Corpulent_suicide_imps:
                case SNOActor.Corpulent_suicide_spiders:
                    return true;
            }

            return false;
        }

        private static bool ShouldCacheCommon(TrinityActor cacheObject)
        {
            if (cacheObject.IsExcludedId && !(ClearArea.IsClearing && cacheObject.IsHostile))
            {
                cacheObject.AddCacheInfo("ExcludedId");
                return false;
            }

            if (cacheObject.IsExcludedType)
            {
                cacheObject.AddCacheInfo("ExcludedType");
                return false;
            }

            if (!cacheObject.IsValid)
            {
                cacheObject.AddCacheInfo("Invalid");
                return false;
            }

            if (cacheObject.IsProfileBlacklisted)
            {
                cacheObject.AddCacheInfo("BlacklistedByProfile");

                if (!GameData.IsCursedChestOrShrine.Contains(cacheObject.ActorSnoId))
                    return false;               
            }

            var item = cacheObject as TrinityItem;
            if (item != null && item.IsCosmeticItem)
                return true;

            if (cacheObject.IsUnit && cacheObject.Attributes == null)
            {
                cacheObject.AddCacheInfo("UnitNoAttributes");
                return false;
            }

            if (cacheObject.IsUntargetable)
            {
                // Include corpulents even when they are untargetable otherwise it messes up the avoidance.
                if (IsCorpulent(cacheObject))
                    return true;

                cacheObject.AddCacheInfo("Untargetable");
                return false;
            }

            var isQuestGiverOutsideTown = cacheObject.IsQuestGiver && !Core.Player.IsInTown;
            if (cacheObject.IsNpc && !isQuestGiverOutsideTown && !cacheObject.IsBoss)
            {
                cacheObject.AddCacheInfo("Npc");
                return false;
            }

            if (cacheObject.IsDead)
            {
                cacheObject.AddCacheInfo("Dead");
                return false;
            }

            if (cacheObject.IsObstacle)
            {
                if (cacheObject.IsGizmo && cacheObject.IsUsed)
                {
                    cacheObject.AddCacheInfo("UsedGizmoObstacle");
                    return false;
                }

                if (cacheObject.IsMonster && !GameData.CorruptGrowthIds.Contains(cacheObject.ActorSnoId))
                {
                    cacheObject.AddCacheInfo("MonsterObstacle");
                    return false;
                }
            }

            if (cacheObject.IsBlacklisted)
            {
                cacheObject.AddCacheInfo("Blacklisted " + cacheObject.ObjectHash);
                return false;
            }

            if (cacheObject.ZDiff > ZDiffLimit(cacheObject))
            {
                cacheObject.AddCacheInfo("ZDiff");
                return false;
            }

            if (!cacheObject.HasBeenInLoS)
            {
                if (!ShouldIgnoreLoS(cacheObject))
                {
                    cacheObject.AddCacheInfo("HasntBeenInLoS");
                    return false;
                }
            }

            return true;
        }

        private static bool ShouldIgnoreLoS(TrinityActor cacheObject)
        {
            if (cacheObject.IsMinimapActive)
                return true;

            if (cacheObject.IsNpc && cacheObject.IsQuestGiver)
                return true;

            switch (cacheObject.Type)
            {
                case TrinityObjectType.Shrine:
                    if (cacheObject.RadiusDistance < 40f || cacheObject.IsWalkable)
                        return true;
                    break;

                case TrinityObjectType.ProgressionGlobe:
                case TrinityObjectType.BuffedRegion:
                        return true;
                           
                case TrinityObjectType.Door:
                    if (cacheObject.RadiusDistance < 15f)
                        return true;
                    break;

                case TrinityObjectType.Barricade:
                case TrinityObjectType.Destructible:
                    if (Core.BlockedCheck.MoveSpeed < 5f && cacheObject.Distance < 10f)
                        return true;
                    break;

                case TrinityObjectType.Unit:
                    if (TrinityCombat.CombatMode == CombatMode.KillAll && cacheObject.IsWalkable)
                        return true;
                    if (cacheObject.IsElite && cacheObject.Distance < 40f || cacheObject.IsWalkable)
                        return true;
                    if (cacheObject.IsTreasureGoblin)
                        return true;
                    break;
            }

            if (cacheObject.Distance < 4) return true;
            if (cacheObject.GetItemQualityLevel() >= ItemQuality.Legendary) return true;
            if (GameData.LineOfSightWhitelist.Contains(cacheObject.ActorSnoId)) return true;

            return false;
        }

        private bool ShouldCacheUnit(TrinityActor cacheObject)
        {
            //// Uncomment to ignore juggernauts
            //if (cacheObject.CommonData.AffixIds.Contains(-464468964))
            //    return false;

            if (cacheObject.IsSameTeam && !cacheObject.IsQuestGiver)
            {
                cacheObject.AddCacheInfo("SameTeam");
                return false;
            }

            if (cacheObject.IsNoDamage && !cacheObject.IsQuestGiver)
            {
                cacheObject.AddCacheInfo("UnitNoDamage");
                return false;
            }

            if (cacheObject.IsFriendly && !cacheObject.IsQuestGiver)
            {
                cacheObject.AddCacheInfo("Friendly");
                return false;
            }

            if (cacheObject.MonsterRace == MonsterRace.Unknown)
            {
                cacheObject.AddCacheInfo("InvalidRace");
                return false;
            }

            if (cacheObject.IsInvulnerable && !cacheObject.IsQuestGiver && !cacheObject.IsElite)
            {
                // Include corpulents even when they are invulnerable otherwise it messes up the avoidance.
                if (IsCorpulent(cacheObject))
                    return true;

                cacheObject.AddCacheInfo("Invulnerable");
                return false;
            }

            if (cacheObject.IsSummonedByPlayer)
            {
                cacheObject.AddCacheInfo("SummonedByPlayer");
                return false;
            }

            if (cacheObject.IsDead)
            {
                cacheObject.AddCacheInfo("Dead");
                return false;
            }

            return true;
        }

        private bool ShouldIncludeGold(TrinityItem cacheObject)
        {
            if (!Core.Settings.Items.PickupGold)
            {
                cacheObject.AddCacheInfo("GoldPickupDisabled");
                return false;
            }
            if (cacheObject.GoldAmount < Core.Settings.Items.MinGoldStack)
            {
                cacheObject.AddCacheInfo("NotEnoughGold");
                return false;
            }
            return true;
        }

        private bool ShouldCacheItem(TrinityItem cacheObject)
        {
            if (!cacheObject.IsPickupNoClick && TrinityCombat.Loot.IsBackpackFull)
            {
                cacheObject.AddCacheInfo("BackpackFull");
                return false;
            }

            if (cacheObject.IsMyDroppedItem)
            {
                cacheObject.AddCacheInfo("DroppedItem");
                return false;
            }

            if (cacheObject.IsUntargetable)
            {
                cacheObject.AddCacheInfo("Untargetable");
                return false;
            }

            if (!cacheObject.IsCosmeticItem && cacheObject.ItemQualityLevel <= ItemQuality.Rare4 && cacheObject.Distance > 60f)
            {
                cacheObject.AddCacheInfo($"OutOfRange Limit={CharacterSettings.Instance.LootRadius}");
                return false;
            }

            if (!TrinityCombat.Loot.ShouldPickup(cacheObject))
            {
                cacheObject.AddCacheInfo("LootProvider.ShouldPickup");
                return false;
            }

            return true;
        }

        private bool ShouldCacheGizmo(TrinityActor cacheObject)
        {
            if (cacheObject.IsUsed)
            {
                cacheObject.AddCacheInfo("UsedGizmo");
                return false;
            }

            if (cacheObject.IsInteractWhitelisted)
            {
                cacheObject.AddCacheInfo("Interact Whitelist");
                return true;
            }

            if (GameData.ForceDestructibles.Contains(cacheObject.ActorSnoId))
            {
                cacheObject.AddCacheInfo("ForceDestructibles");
                return true;
            }

            if (TrinityTownRun.IsWantingTownRun && cacheObject.Distance > 10f)
            {
                cacheObject.AddCacheInfo("WantToTownRun");
                return false;
            }

            if (GameData.DoorsToAlwaysIgnore.Contains(cacheObject.ActorSnoId))
            {
                cacheObject.AddCacheInfo("AlwaysIgnoreDoor");
                return false;
            }

            if (GameData.SceneSpecificDoorsIgnore.ContainsKey(Core.Player.CurrentSceneSnoId) &&
                GameData.SceneSpecificDoorsIgnore[Core.Player.CurrentSceneSnoId] == cacheObject.ActorSnoId)
            {
                cacheObject.AddCacheInfo("SceneSpecificIgnoreDoor");
                return false;
            }

            if (cacheObject.IsDestroyable && !cacheObject.HasBeenWalkable && cacheObject.Distance > 5f && cacheObject.GizmoType != GizmoType.BreakableChest )
            {
                cacheObject.AddCacheInfo("CantReachDestructible");
                return false;
            }
            return true;
        }

        public static float ZDiffLimit(TrinityActor cacheObject)
        {
            var defaultValue = 50f;

            if (GameData.LineOfSightWhitelist.Contains(cacheObject.ActorSnoId))
                return defaultValue;

            if (cacheObject.IsLastTarget)
                return defaultValue;

            if (cacheObject.IsBoss)
                return defaultValue;

            if (cacheObject.IsElite)
                return defaultValue;

            switch (cacheObject.Type)
            {
                case TrinityObjectType.ProgressionGlobe:
                    return defaultValue;
                case TrinityObjectType.Avoidance:
                case TrinityObjectType.Door:
                case TrinityObjectType.Unit:
                case TrinityObjectType.Barricade:
                    return 14;
                case TrinityObjectType.Item:
                case TrinityObjectType.HealthWell:
                    return 26;
                case TrinityObjectType.Gold:
                case TrinityObjectType.HealthGlobe:
                case TrinityObjectType.PowerGlobe:
                    return 11;
                case TrinityObjectType.Destructible:
                case TrinityObjectType.Shrine:
                case TrinityObjectType.Container:
                    return 7;
                case TrinityObjectType.Interactable:
                    return 9;
            }

            return defaultValue;
        }

        public void Clear()
        {
            Entries.Clear();
            Ignored.Clear();
        }

        public IEnumerator<TrinityActor> GetEnumerator() => Entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}