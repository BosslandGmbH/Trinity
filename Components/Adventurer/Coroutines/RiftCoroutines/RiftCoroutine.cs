using Buddy.Coroutines;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Components.Adventurer.Game.Stats;
using Trinity.Components.Adventurer.Settings;
using Trinity.Framework.Helpers;
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
        private static readonly ILogger s_logger = Logger.GetLoggerInstanceForType();
        private static readonly ExperienceTracker s_experienceTracker = new ExperienceTracker();

        public static long CurrentRiftKeyCount =>
            ZetaDia.Storage?.PlayerDataManager?.ActivePlayerData
                ?.GetCurrencyAmount(CurrencyType.GreaterRiftKeys) ?? 0;
        
        public static DiaGizmo RiftPortal => ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
            .FirstOrDefault(g => g.IsFullyValid() &&
                                 g.ActorSnoId == SNOActor.X1_OpenWorld_LootRunPortal ||
                                 g.ActorSnoId == SNOActor.X1_OpenWorld_Tiered_Rifts_Portal);

        public static DiaGizmo LootRunSwitch => ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
            .FirstOrDefault(g => g.IsFullyValid() &&
                                 g.ActorSnoId == SNOActor.x1_OpenWorld_LootRunObelisk_B &&
                                 g.CommonData.GizmoType == GizmoType.LootRunSwitch);

        public static SNOWorld PreviousWorld { get; set; } = SNOWorld.Invalid;
        public static SNOLevelArea PreviousLevel { get; set; } = SNOLevelArea.Invalid;

        public static HashSet<SNOWorld> TownWorlds { get; } = new HashSet<SNOWorld>()
        {
            SNOWorld.X1_Tristram_Adventure_Mode_Hub,
            SNOWorld.caOUT_RefugeeCamp,
            SNOWorld.a3dun_hub_keep,
            SNOWorld.X1_Westmarch_Hub,
        };

        public static HashSet<SNOLevelArea> TownLevelAreas { get; } = new HashSet<SNOLevelArea>()
        {
            SNOLevelArea.A1_Tristram_Adventure_Mode_Hub,
            SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub,
            SNOLevelArea.A3_Dun_Keep_Hub,
            SNOLevelArea.x1_Westm_Hub
        };

        // TODO: Make sure we detect the Exit portal properly. Might lead to portal cycles and stuff like that when wrong!
        public static DiaGizmo ExitPortal => ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
            .Where(g => g.IsFullyValid() &&
                        g.IsPortal &&
                        g.CommonData.PortalDestination?.WorldSNO != PreviousWorld &&
                        g.CommonData.PortalDestination?.DestLevelAreaSNO != PreviousLevel &&
                        !TownWorlds.Contains(g.CommonData.PortalDestination?.WorldSNO ?? SNOWorld.Invalid) &&
                        !TownLevelAreas.Contains(g.CommonData.PortalDestination?.DestLevelAreaSNO ?? SNOLevelArea.Invalid) &&
                        !RiftData.PossibleDungeonStoneSNO.Contains(g.ActorSnoId) &&
                        g.CommonData.GizmoType != GizmoType.HearthPortal)
            .OrderBy(g => g.DistanceSqr)
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
            if (!(ZetaDia.IsInTown &&
                  ZetaDia.Globals.WorldSnoId == SNOWorld.X1_Tristram_Adventure_Mode_Hub) &&
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

            DiaGizmo lrs = LootRunSwitch;
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
            return await Coroutine.Wait(TimeSpan.FromSeconds(2), () => IsRiftPortalOpen);
        }

        public static async Task<CoroutineResult> EnsureInRift()
        {
            if (!ZetaDia.IsInTown)
                return CoroutineResult.NoAction;

            if (!IsRiftPortalOpen)
                return CoroutineResult.NoAction;

            var rp = RiftPortal;
            if (rp != null)
                return await CommonCoroutines.MoveAndInteract(
                    rp,
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
            if (ExitPortal == null)
            {
                await ExplorationCoroutine.Explore(new HashSet<SNOLevelArea> { AdvDia.CurrentLevelAreaId });
                return false;
            }

            var tmpWorld = ZetaDia.Globals.WorldSnoId;
            var tmpLevelArea = ZetaDia.CurrentLevelAreaSnoId;
            if (await CommonCoroutines.MoveAndInteract(
                    ExitPortal,
                    () => ZetaDia.Globals.IsLoadingWorld ||
                          ZetaDia.Globals.IsPlayingCutscene) ==
                CoroutineResult.Done)
            {
                PreviousWorld = tmpWorld;
                PreviousLevel = tmpLevelArea;
            }

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

            ACDItem gemToUpgrade = PluginSettings.Current.Gems.GetUpgradeTarget();
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
                return false;

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

            s_logger.Information("Rift done, let's force a town run...");
            BrainBehavior.ForceTownrun(nameof(RiftCoroutine));
            PreviousWorld = SNOWorld.Invalid;
            PreviousLevel = SNOLevelArea.Invalid;
            return true;
        }
    }
}
