﻿using Buddy.Coroutines;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Components.Adventurer.Game.Stats;
using Trinity.Components.Adventurer.Settings;
using Trinity.Framework;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using GizmoType = Zeta.Game.Internals.SNO.GizmoType;

namespace Trinity.Components.Adventurer.Coroutines.RiftCoroutines
{
    using CoroutineResult = Zeta.Bot.Coroutines.CoroutineResult;

    public static class RiftCoroutine
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();
        private static readonly ExperienceTracker s_experienceTracker = new ExperienceTracker();

        public static long CurrentRiftKeyCount => Core.Actors.Inventory
            .Where(i => i.GetRawItemType() == RawItemType.TieredRiftKey)
            .Sum(k => k.ItemStackQuantity);

        //ActorId: 364715, Type: Gizmo, Name: x1_OpenWorld_LootRunObelisk_B - 27053, Distance2d: 9.72007, CollisionRadius: 9.874258, MinimapActive: 1, MinimapIconOverride: 327066, MinimapDisableArrow: 0
        //ActorId: 345935, Type: Gizmo, Name: X1_OpenWorld_LootRunPortal - 27292, Distance2d: 9.72007, CollisionRadius: 8.316568, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0
        public static DiaGizmo RiftPortal => ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
            .FirstOrDefault(g => g.IsFullyValid() &&
                                 g.ActorSnoId == RiftData.RiftEntryPortalSNO ||
                                 g.ActorSnoId == RiftData.GreaterRiftEntryPortalSNO);

        public static DiaGizmo LootRunSwitch => ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
            .FirstOrDefault(g => g.IsFullyValid() &&
                                 g.CommonData.GizmoType == GizmoType.LootRunSwitch);

        public static Vector3 EntryLocation => BountyHelpers.ScanForRiftEntryMarkerLocation();

        // TODO: Make sure we detect the Exit portal properly. Might lead to portal cycles and stuff like that when wrong!
        public static DiaGizmo ExitPortal => ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
            .Where(g => g.IsFullyValid() &&
                        g.IsPortal &&
                        g.Position.Distance2DSqr(EntryLocation) > 400f &&
                        !RiftData.PossibleDungeonStoneSNO.Contains(g.ActorSnoId) &&
                        g.CommonData.GizmoType != GizmoType.HearthPortal)
            .OrderBy(g => g.Position.Distance2DSqr(AdvDia.MyPosition))
            .FirstOrDefault();

        public static DiaUnit Urshi => ZetaDia.Actors.GetActorsOfType<DiaUnit>(true)
            .FirstOrDefault(u => u.ActorSnoId == RiftData.UrshiSNO);

        public static DiaUnit Orek => ZetaDia.Actors.GetActorsOfType<DiaUnit>(true)
            .FirstOrDefault(u => u.ActorSnoId == RiftData.OrekSNO);

        public static DiaUnit HolyCow => ZetaDia.Actors.GetActorsOfType<DiaUnit>()
            .FirstOrDefault(a => a.IsFullyValid() &&
                                 a.ActorSnoId == RiftData.HolyCowSNO &&
                                 a.IsInteractableQuestObject());

        public static bool IsRiftPortalOpen => AdvDia.RiftQuest.State > QuestState.NotStarted;

        // TODO: Add a way to open the cow portal.
        public static async Task<bool> ClearCowLevel()
        {
            if (ZetaDia.IsInTown)
                return true;

            if (!RiftData.PossibleHolyCowLevelID.Contains(AdvDia.CurrentLevelAreaId))
                return true;

            if (HolyCow != null)
                Debugger.Break();

            return await CommonCoroutines.MoveAndInteract(
                HolyCow,
                () => false) == CoroutineResult.Running;
        }

        public static async Task<CoroutineResult> EnsureIsInTown()
        {
            if (!ZetaDia.IsInTown &&
                await WaypointCoroutine.UseWaypoint(WaypointFactory.ActHubs[Act.A1]))
            {
                return CoroutineResult.Running;
            }
            return await Coroutine.Wait(TimeSpan.FromSeconds(2), () => ZetaDia.IsInTown) ? CoroutineResult.Done : CoroutineResult.Running;
        }

        public static async Task<bool> OpenRift(RiftType riftType,
                                                int maxLevel,
                                                int maxEmpowerLevel,
                                                bool shouldEmpower,
                                                bool runNormalUntilXP)
        {
            if (IsRiftPortalOpen)
                return IsRiftPortalOpen;

            var riftKeys = CurrentRiftKeyCount;
            if (riftType == RiftType.Greater &&
                riftKeys <= PluginSettings.Current.MinimumKeys &&
                !PluginSettings.Current.GreaterRiftRunNephalem)
            {
                s_logger.Error($"[{nameof(OpenRift)}] You have no Greater Rift Keys. Stopping the bot.");
                BotMain.Stop();
                return false;
            }

            s_logger.Debug($"[{nameof(OpenRift)}] I have {riftKeys} rift keys.");

            if (await EnsureIsInTown() == CoroutineResult.Running)
                return false;

            if (riftKeys <= PluginSettings.Current.MinimumKeys)
                riftType = RiftType.Nephalem;

            // TODO: Figure out why there is that check against that magic.
            var maximizeXp = runNormalUntilXP &&
                             riftType == RiftType.Greater &&
                             ZetaDia.Me.RestExperience < 5000000000 &&
                             ZetaDia.Me.RestExperience > -1;

            if (maximizeXp)
                riftType = RiftType.Nephalem;

            var level = Math.Min(riftType == RiftType.Greater ? RiftData.GetGreaterRiftLevel() : -1, maxLevel);
            var isEmpowered = riftType == RiftType.Greater &&
                              shouldEmpower &&
                              level <= maxEmpowerLevel &&
                              RiftData.EmpoweredRiftCost.TryGetValue(level, out var empoweredCost) &&
                              ZetaDia.Storage.PlayerDataManager.ActivePlayerData.Coinage >=
                              (empoweredCost + PluginSettings.Current.MinimumGold);

            var lrs = LootRunSwitch;
            if (lrs == null)
            {
                await CommonCoroutines.MoveTo(ZetaDia.Actors.GetActorsOfType<DiaGizmo>()
                    .Where(g => g.Distance > 10f)
                    .OrderByDescending(g => g.Distance)
                    .FirstOrDefault());
                return false;
            }

            if (!s_experienceTracker.IsStarted)
                s_experienceTracker.Start();

            if (await CommonCoroutines.MoveAndInteract(
                    lrs,
                    () => UIElements.RiftDialog.IsVisible) == CoroutineResult.Running)
            {
                return false;
            }

            ZetaDia.Me.OpenRift(level, isEmpowered);
            return await Coroutine.Wait(2000, () => IsRiftPortalOpen);
        }

        public static async Task<CoroutineResult> EnsureInRift()
        {
            if (!ZetaDia.IsInTown)
                return CoroutineResult.NoAction;

            if (!IsRiftPortalOpen)
                return CoroutineResult.NoAction;

            if (RiftPortal != null)
                return await CommonCoroutines.MoveAndInteract(
                    RiftPortal,
                    () => ZetaDia.IsInGame &&
                          !ZetaDia.Globals.IsLoadingWorld &&
                          !ZetaDia.Globals.IsPlayingCutscene &&
                          !ZetaDia.IsInTown);

            // TODO: Make sure we move somewhere we expect the portal to show up.
            await CommonCoroutines.MoveTo(ZetaDia.Actors.GetActorsOfType<DiaGizmo>()
                .Where(g => g.Distance > 10f)
                .OrderBy(g => g.Distance)
                .FirstOrDefault());
            return CoroutineResult.Running;

        }

        public static async Task<bool> ClearRift()
        {
            if (AdvDia.RiftQuest.Step >= RiftStep.UrshiSpawned)
                return true;

            if (await EnsureInRift() == CoroutineResult.Running)
                return false;

            // TODO: Handle Cow level
            // TODO: Fix Portal detection
            if (ExitPortal == null || ExitPortal.ZDiff > 5f)
            {
                await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId });
                return false;
            }

            await CommonCoroutines.MoveAndInteract(ExitPortal, () => ZetaDia.Globals.IsLoadingWorld ||
                                                                     ZetaDia.Globals.IsPlayingCutscene);
            return false;
        }

        public static async Task<CoroutineResult> UpgradeGems()
        {
            if (AdvDia.RiftQuest.Step != RiftStep.UrshiSpawned)
                return CoroutineResult.NoAction;

            CoroutineResult previousResult;
            if ((previousResult = await EnsureInRift()) == CoroutineResult.Running)
                return CoroutineResult.Running;

            if (previousResult == CoroutineResult.Failed)
                return CoroutineResult.Failed;

            var gemToUpgrade = PluginSettings.Current.Gems.GetUpgradeTarget();
            if (gemToUpgrade == null)
                return CoroutineResult.NoAction;

            return await CommonCoroutines.AttemptUpgradeGem(gemToUpgrade);
        }

        public static async Task<CoroutineResult> TurnInQuest()
        {
            if (AdvDia.RiftQuest.Step != RiftStep.Cleared)
                return CoroutineResult.NoAction;

            if (await EnsureIsInTown() == CoroutineResult.Running)
                return CoroutineResult.Running;

            if (Orek == null)
            {
                await CommonCoroutines.MoveTo(ZetaDia.Actors.GetActorsOfType<DiaGizmo>()
                    .Where(g => g.Distance > 10f)
                    .OrderByDescending(g => g.Distance)
                    .FirstOrDefault());
                return CoroutineResult.Running;
            }

            if (!(Orek.IsValid &&
                  await CommonCoroutines.MoveAndInteract(
                      Orek,
                      () => !Orek.IsQuestGiver) != CoroutineResult.Running))
            {
                return CoroutineResult.Running;
            }

            if (s_experienceTracker.IsStarted)
                s_experienceTracker.StopAndReport(nameof(RiftCoroutine));

            return CoroutineResult.Done;
        }

        public static async Task<bool> RunRift(RiftType riftType,
                                               int maxLevel,
                                               int maxEmpowerLevel,
                                               bool shouldEmpower,
                                               bool runNormalUntilXP)
        {
            if (!ZetaDia.IsInGame ||
                ZetaDia.Globals.IsLoadingWorld ||
                ZetaDia.Globals.IsPlayingCutscene)
            {
                return false;
            }

            CoroutineResult previousResult;
            if ((previousResult = await TurnInQuest()) == CoroutineResult.Running)
            {
                return false;
            }

            if (BrainBehavior.IsVendoring &&
                previousResult != CoroutineResult.NoAction)
            {
                return false;
            }

            // TODO: Decide if we want to run the cow level
            //if (!await ClearCowLevel())
            //    return false;

            if (!await OpenRift(
                riftType,
                maxLevel,
                maxEmpowerLevel,
                shouldEmpower,
                runNormalUntilXP))
            {
                return false;
            }

            if (!await ClearRift())
                return false;

            if (await UpgradeGems() == CoroutineResult.Running)
                return false;

            s_logger.Info("Rift done, let's force a town run...");
            BrainBehavior.ForceTownrun(nameof(RiftCoroutine));

            return true;
        }
    }
}
