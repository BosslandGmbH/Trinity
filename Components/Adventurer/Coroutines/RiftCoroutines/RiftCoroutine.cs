using Buddy.Coroutines;
using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Components.Adventurer.Game.Stats;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Coroutines.Town;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Common.Helpers;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using GizmoType = Zeta.Game.Internals.SNO.GizmoType;

namespace Trinity.Components.Adventurer.Coroutines.RiftCoroutines
{
    public static class RiftCoroutine
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();
        //TODO: Add Expirience Tracking
        private static readonly ExperienceTracker s_experienceTracker = new ExperienceTracker();

        public static long CurrentRiftKeyCount
        {
            get
            {
                ZetaDia.Actors.Update();
                Core.Update();
                var keys = AdvDia.StashAndBackpackItems.Where(i => i.RawItemType == RawItemType.TieredRiftKey).Sum(k => k.ItemStackQuantity);
                Core.Logger.Log("I have {0} rift keys.", keys);
                return keys;
            }
        }

        //ActorId: 364715, Type: Gizmo, Name: x1_OpenWorld_LootRunObelisk_B - 27053, Distance2d: 9.72007, CollisionRadius: 9.874258, MinimapActive: 1, MinimapIconOverride: 327066, MinimapDisableArrow: 0
        //ActorId: 345935, Type: Gizmo, Name: X1_OpenWorld_LootRunPortal - 27292, Distance2d: 9.72007, CollisionRadius: 8.316568, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0
        public static DiaGizmo RiftPortal => ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).FirstOrDefault(g => g.IsFullyValid() && g.ActorSnoId == RiftData.RiftEntryPortalSNO || g.ActorSnoId == RiftData.GreaterRiftEntryPortalSNO);

        public static DiaGizmo LootRunSwitch => ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).FirstOrDefault(g => g.IsFullyValid() && g.CommonData.GizmoType == GizmoType.LootRunSwitch);

        public static Vector3 EntryLocation => BountyHelpers.ScanForRiftEntryMarkerLocation();

        // TODO: Make sure we detect the Exit portal properly. Might lead to portal cycles and stuff like that when wrong!
        public static DiaGizmo ExitPortal => ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
            .Where(g => g.IsFullyValid() && g.IsPortal && g.Position.Distance2DSqr(EntryLocation) > 100f && !RiftData.DungeonStoneSNOs.Contains(g.ActorSnoId) && g.CommonData.GizmoType != GizmoType.HearthPortal)
            .OrderBy(g => g.Position.Distance2DSqr(AdvDia.MyPosition))
            .FirstOrDefault();

        public static DiaUnit Urshi => ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).FirstOrDefault(u => u.ActorSnoId == RiftData.UrshiSNO);

        public static DiaUnit Orek => ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).FirstOrDefault(u => u.ActorSnoId == RiftData.OrekSNO);

        public static bool IsRiftPortalOpen => AdvDia.RiftQuest.State > QuestState.NotStarted;

        public static async Task<bool> EnsureIsInTown()
        {
            if (!ZetaDia.IsInTown && await WaypointCoroutine.UseWaypoint(WaypointFactory.ActHubs[Act.A1]))
                return false;
            return await Coroutine.Wait(TimeSpan.FromSeconds(2), () => ZetaDia.IsInTown);
        }

        public static async Task<bool> OpenRift(RiftType riftType, int maxLevel, int maxEmpowerLevel, bool shouldEmpower, bool runNormalUntilXP)
        {
            if (IsRiftPortalOpen) return IsRiftPortalOpen;

            var riftKeys = CurrentRiftKeyCount;
            if (riftType == RiftType.Greater && riftKeys <= PluginSettings.Current.MinimumKeys && !PluginSettings.Current.GreaterRiftRunNephalem)
            {
                Core.Logger.Error("You have no Greater Rift Keys. Stopping the bot.");
                BotMain.Stop();
                return false;
            }

            if (!await EnsureIsInTown())
                return false;

            if (riftKeys <= PluginSettings.Current.MinimumKeys)
                riftType = RiftType.Nephalem;

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
                await CommonCoroutines.MoveTo(ZetaDia.Actors.GetActorsOfType<DiaGizmo>().Where(g => g.Distance > 10f).OrderByDescending(g => g.Distance).FirstOrDefault());
                return false;
            }

            if (!await CommonCoroutines.MoveAndInteract(lrs, () => UIElements.RiftDialog.IsVisible))
                return false;

            ZetaDia.Me.OpenRift(level, isEmpowered);
            return await Coroutine.Wait(2000, () => IsRiftPortalOpen);
        }

        public static async Task<bool> EnsureInRift()
        {
            if (!ZetaDia.IsInTown) return true;
            if (!IsRiftPortalOpen) return true;

            if (RiftPortal == null)
            {
                await CommonCoroutines.MoveTo(ZetaDia.Actors.GetActorsOfType<DiaGizmo>().Where(g => g.Distance > 10f).OrderBy(g => g.Distance).FirstOrDefault());
                return false;
            }

            await CommonCoroutines.MoveAndInteract(RiftPortal, () => ZetaDia.IsInGame && !ZetaDia.Globals.IsLoadingWorld && !ZetaDia.Globals.IsPlayingCutscene && !ZetaDia.IsInTown);
            return false;
        }

        public static async Task<bool> ClearRift()
        {
            if (AdvDia.RiftQuest.Step >= RiftStep.UrshiSpawned) return true;

            if (!await EnsureInRift())
                return false;

            // TODO: Handle Cow level
            // TODO: Fix Portal detection
            if (ExitPortal == null || ExitPortal.ZDiff > 5f)
            {
                await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId });
                return false;
            }

            await CommonCoroutines.MoveAndInteract(ExitPortal, () => ZetaDia.Globals.IsLoadingWorld || ZetaDia.Globals.IsPlayingCutscene);
            return false;
        }

        public static async Task<bool> UpgradeGems()
        {
            if (AdvDia.RiftQuest.Step != RiftStep.UrshiSpawned) return true;

            if (!await EnsureInRift())
                return false;

            var gemToUpgrade = PluginSettings.Current.Gems.GetUpgradeTarget();
            if (gemToUpgrade == null)
                return true;

            return await CommonCoroutines.AttemptUpgradeGem(gemToUpgrade);
        }

        public static async Task<bool> TurnInQuest()
        {
            if (AdvDia.RiftQuest.Step != RiftStep.Cleared) return true;

            if (!await EnsureIsInTown())
                return false;

            if (Orek == null)
            {
                await CommonCoroutines.MoveTo(ZetaDia.Actors.GetActorsOfType<DiaGizmo>().Where(g => g.Distance > 10f).OrderByDescending(g => g.Distance).FirstOrDefault());
                return false;
            }

            if (!(Orek.IsValid && await CommonCoroutines.MoveAndInteract(Orek, () => !Orek.IsQuestGiver)))
                return false;

            if (AdvDia.RiftQuest.State != QuestState.NotStarted)
                return false;

            return true;
        }

        public static async Task<bool> RunRift(RiftType riftType, int maxLevel, int maxEmpowerLevel, bool shouldEmpower, bool runNormalUntilXP)
        {
            if (BrainBehavior.IsVendoring || !ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld || ZetaDia.Globals.IsPlayingCutscene)
            {
                return false;
            }

            if (!await OpenRift(riftType, maxLevel, maxEmpowerLevel, shouldEmpower, runNormalUntilXP))
                return false;

            if (!await ClearRift())
                return false;

            if (!await UpgradeGems())
                return false;

            if (!await TurnInQuest())
                return false;

            s_logger.Error("Rift done what next...");
            // TODO: Update the tracking information here...
            // TODO: Decide if another rift should be run... If so return false else return true...
            return false;
        }
    }
    
    [Obsolete]
    // TODO: Delete once cowlevel and expirence tracker is ported.
    public class RiftCoroutines : IDisposable, ICoroutine
    {
        public class RiftOptions
        {
            public int RiftCount = 0;
            public bool IsEmpowered = false;
            public States EndState = States.Finished;
            public bool NormalRiftForXPShrine;
        }

        private RiftType _riftType;
        private bool _runningNephalemInsteadOfGreaterRift;
        private int _level;
        private States _state;
        private bool _townRunInitiated;
        private int _portalScanRange = 225;

        private int _currentWorldDynamicId;
        private int _previusWorldDynamicId;
        private int _prePortalWorldDynamicId;
        private int _nextLevelPortalSNO;

        private bool _possiblyCowLevel;
        private bool _holyCowEventCompleted;

        private Vector3 _bossLocation = Vector3.Zero;
        private Vector3 _holyCowLocation = Vector3.Zero;
        private Vector3 _urshiLocation = Vector3.Zero;
        private Vector3 _nextLevelPortalLocation = Vector3.Zero;
        private DateTime _riftStartTime;
        private DateTime _riftEndTime;
        private readonly ExperienceTracker _experienceTracker = new ExperienceTracker();

        private readonly MoveToPositionCoroutine _moveToRiftStoneCoroutine = new MoveToPositionCoroutine(ExplorationData.ActHubWorldIds[Act.A1], RiftData.Act1RiftStonePosition);
        private readonly InteractionCoroutine _interactWithRiftStoneInteractionCoroutine = new InteractionCoroutine(RiftData.RiftStoneSNO, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), 1);
        private readonly InteractionCoroutine _interactWithUrshiCoroutine = new InteractionCoroutine(RiftData.UrshiSNO, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1));

        private readonly MoveToPositionCoroutine _moveToOrekCoroutine = new MoveToPositionCoroutine(ExplorationData.ActHubWorldIds[Act.A1], RiftData.Act1OrekPosition);
        private readonly InteractionCoroutine _talkToOrekCoroutine = new InteractionCoroutine(RiftData.OrekSNO, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(400));
        private readonly InteractionCoroutine _talkToHolyCowCoroutine = new InteractionCoroutine(RiftData.HolyCowSNO, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(1000));

        #region State

        public enum States
        {
            NotStarted,
            GoingToAct1Hub,
            ReturningToTown,
            InTown,
            MoveToOrek,
            TalkToOrek,
            TownRun,
            WaitForRiftCountdown,
            MoveToRiftStone,
            OpeningRift,
            EnteringRift,
            EnteringGreaterRift,
            SearchingForExitPortal,
            MovingToExitPortal,
            EnteringExitPortal,
            OnNewRiftLevel,
            BossSpawned,
            SearchingForBoss,
            MovingToBoss,
            KillingBoss,
            UrshiSpawned,
            SearchingForUrshi,
            MovingToUrshi,
            InteractingWithUrshi,
            UpgradingGems,
            SearchingForTownstoneOrExitPortal,
            TownstoneFound,
            SearchingForHolyCow,
            MovingToHolyCow,
            InteractingWithHolyCow,
            Completed,
            Failed,
            Finished
        }

        public States State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Core.Logger.Debug("[Rift] " + value);
                    StatusText = "[Rift] " + value;
                }
                _state = value;
            }
        }

        #endregion State

        public RiftCoroutines(RiftType riftType, RiftOptions options = null)
        {
            _riftType = riftType;
            if (riftType == RiftType.Nephalem)
            {
                _level = -1;
            }
            else
            {
                _level = RiftData.GetGreaterRiftLevel();
            }

            Id = Guid.NewGuid();

            _options = options ?? new RiftOptions();

            Pulsator.OnPulse += OnPulse;
            Core.Logger.Debug("[Rift] Registered from pulsator.");
        }

        public Guid Id { get; }

        public void Reset()
        {
            _riftCounter = 0;
            _currentExitScene = null;
            _entranceSceneNames.Clear();
            State = States.NotStarted;
        }

        public string StatusText { get; set; }

        public virtual async Task<bool> GetCoroutine()
        {

            if (!await PulseChecks())
                return false;

            if (State == _options.EndState)
            {
                Core.Logger.Debug("[Rift] Someone told us to stop rifting, so we will do what we're told like a good boy and/or girl.");
                State = States.Finished;
            }

            switch (State)
            {
                case States.NotStarted:
                    return NotStarted();

                case States.GoingToAct1Hub:
                    return await GoingToAct1Hub();

                case States.ReturningToTown:
                    return await ReturningToTown();

                case States.InTown:
                    return InTown();

                case States.MoveToOrek:
                    return await MoveToOrek();

                case States.TalkToOrek:
                    return await TalkToOrek();

                case States.TownRun:
                    return await TownRun();

                case States.WaitForRiftCountdown:
                    return WaitForRiftCountdown();

                case States.MoveToRiftStone:
                    return await MoveToRiftStone();

                case States.OpeningRift:
                    return await OpeningRift();

                case States.EnteringRift:
                    return await EnteringRift();

                case States.EnteringGreaterRift:
                    return EnteringGreaterRift();

                case States.SearchingForExitPortal:
                    return await SearchingForExitPortal();

                case States.MovingToExitPortal:
                    return await MovingToExitPortal();

                case States.EnteringExitPortal:
                    return await EnteringExitPortal();

                case States.OnNewRiftLevel:
                    return OnNewRiftLevel();

                case States.BossSpawned:
                    return BossSpawned();

                case States.SearchingForBoss:
                    return await SearchingForBoss();

                case States.MovingToBoss:
                    return await MovingToBoss();

                case States.KillingBoss:
                    return KillingBoss();

                case States.UrshiSpawned:
                    return UrshiSpawned();

                case States.SearchingForUrshi:
                    return await SearchingForUrshi();

                case States.MovingToUrshi:
                    return await MovingToUrshi();

                case States.InteractingWithUrshi:
                    return await InteractingWithUrshi();

                case States.UpgradingGems:
                    return await UpgradingGems();

                case States.SearchingForTownstoneOrExitPortal:
                    return await SearchingForTownstoneOrExitPortal();

                case States.TownstoneFound:
                    return TownstoneFound();

                case States.SearchingForHolyCow:
                    return await SearchingForHolyCow();

                case States.MovingToHolyCow:
                    return await MovingToHolyCow();

                case States.InteractingWithHolyCow:
                    return await InteractingWithHolyCow();

                case States.Completed:
                    return Completed();

                case States.Failed:
                    return Failed();

                case States.Finished:
                    return Finished();
            }
            return false;
        }

        private long CurrentRiftKeyCount
        {
            get
            {
                ZetaDia.Actors.Update();
                Core.Update();
                var keys = AdvDia.StashAndBackpackItems.Where(i => i.RawItemType == RawItemType.TieredRiftKey).Sum(k => k.ItemStackQuantity);
                Core.Logger.Log("I have {0} rift keys.", keys);
                return keys;
            }
        }

        private bool NotStarted()
        {
            BotMain.SetCurrentStatusTextProvider(() => StatusText);
            if (!_experienceTracker.IsStarted) _experienceTracker.Start();
            SafeZerg.Instance.DisableZerg();

            _level = RiftData.GetGreaterRiftLevel();
            if (_runningNephalemInsteadOfGreaterRift && CurrentRiftKeyCount > PluginSettings.Current.MinimumKeys)
            {
                _riftType = RiftType.Greater;
                _runningNephalemInsteadOfGreaterRift = false;
                return false;
            }

            if (AdvDia.RiftQuest.State == QuestState.NotStarted && _riftType == RiftType.Greater && CurrentRiftKeyCount <= PluginSettings.Current.MinimumKeys)
            {
                if (PluginSettings.Current.GreaterRiftRunNephalem)
                {
                    _level = -1;
                    _riftType = RiftType.Nephalem;
                    _runningNephalemInsteadOfGreaterRift = true;
                    return false;
                }
                Core.Logger.Error("You have no Greater Rift Keys. Stopping the bot.");
                BotMain.Stop();
                return true;
            }
            _currentWorldDynamicId = AdvDia.CurrentWorldDynamicId;
            if (AdvDia.RiftQuest.State == QuestState.InProgress && RiftData.RiftWorldIds.Contains(AdvDia.CurrentWorldId))
            {
                State = States.OnNewRiftLevel;
                return false;
            }
            State = AdvDia.CurrentWorldId == ExplorationData.ActHubWorldIds[Act.A1] ? States.InTown : States.GoingToAct1Hub;
            if (AdvDia.RiftQuest.State != QuestState.NotStarted) return false;

            if (Core.Scenes.CurrentScene.LevelAreaId != ZetaDia.CurrentLevelAreaSnoId)
            {
                Core.Scenes.Reset();
            }

            RiftData.EntryPortals.Clear();
            _currentWorldDynamicId = 0;
            _previusWorldDynamicId = 0;
            _bossLocation = Vector3.Zero;
            _nextLevelPortalLocation = Vector3.Zero;
            _holyCowLocation = Vector3.Zero;
            _holyCowEventCompleted = false;
            _possiblyCowLevel = false;
            return false;
        }

        private WaitTimer _goingToAct1HubWaitTimer;

        private async Task<bool> GoingToAct1Hub()
        {
            if (_goingToAct1HubWaitTimer == null)
            {
                _goingToAct1HubWaitTimer = new WaitTimer(TimeSpan.FromSeconds(5));
                _goingToAct1HubWaitTimer.Reset();
            }
            if (!_goingToAct1HubWaitTimer.IsFinished) return false;
            if (!await WaypointCoroutine.UseWaypoint(WaypointFactory.ActHubs[Act.A1])) return false;
            _goingToAct1HubWaitTimer = null;
            State = States.InTown;
            return false;
        }

        private WaitTimer _returningToTownbWaitTimer;

        private async Task<bool> ReturningToTown()
        {
            if (_returningToTownbWaitTimer == null)
            {
                _returningToTownbWaitTimer = new WaitTimer(TimeSpan.FromSeconds(5));
                _returningToTownbWaitTimer.Reset();
            }
            if (!_returningToTownbWaitTimer.IsFinished) return false;
            if (!await TownPortalCoroutine.UseWaypoint()) return false;
            _returningToTownbWaitTimer = null;
            State = States.InTown;
            return false;
        }

        private bool InTown()
        {
            if (AdvDia.CurrentWorldId != ExplorationData.ActHubWorldIds[Act.A1])
            {
                State = States.GoingToAct1Hub;
                return false;
            }
            if (AdvDia.RiftQuest.State == QuestState.Completed)
            {
                State = States.TownRun;
                return false;
            }
            switch (AdvDia.RiftQuest.Step)
            {
                case RiftStep.Cleared:
                    Core.Logger.Log("[Rift] I think I should go and brag to Orek about my success.");
                    State = States.MoveToOrek;
                    return false;

                case RiftStep.BossSpawned:
                    Core.Logger.Log("[Rift] I wonder why am I in town while the boss is spawned in the rift, I'm taking my chances with this portal.");
                    State = States.MoveToRiftStone;
                    return false;

                case RiftStep.UrshiSpawned:
                    Core.Logger.Log("[Rift] I wonder why am I in town while Urshi spawned in the rift, I'm taking my chances with this portal.");
                    State = States.MoveToRiftStone;
                    return false;

                case RiftStep.KillingMobs:
                    Core.Logger.Log("[Rift] I wonder why am I in town while there are many mobs to kill in the rift, I'm taking my chances with this portal.");
                    State = States.MoveToRiftStone;
                    return false;

                case RiftStep.NotStarted:
                    State = States.MoveToRiftStone;
                    _moveToRiftStoneCoroutine.Reset();
                    Core.Logger.Log("[Rift] Time to kill some scary monsters. Chop chop!");
                    return false;

                default:
                    Core.Logger.Log("[Rift] I really don't know what to do now.");
                    State = States.Failed;
                    return false;
            }
        }

        private async Task<bool> MoveToOrek()
        {
            if (!await _moveToOrekCoroutine.GetCoroutine()) return false;
            _moveToOrekCoroutine.Reset();
            State = States.TalkToOrek;
            return false;
        }

        private async Task<bool> TalkToOrek()
        {
            if (!AdvDia.IsInTown)
            {
                State = States.ReturningToTown;
                return false;
            }

            if (AdvDia.RiftQuest.State == QuestState.InProgress && AdvDia.RiftQuest.Step == RiftStep.Cleared)
            {
                if (!await _talkToOrekCoroutine.GetCoroutine())
                    return false;

                _talkToOrekCoroutine.Reset();
                if (AdvDia.RiftQuest.Step == RiftStep.Cleared)
                {
                    State = States.MoveToOrek;
                    return false;
                }
                await Coroutine.Wait(TimeSpan.FromSeconds(5), () => !ZetaDia.Me.IsParticipatingInTieredLootRun);
                if (ZetaDia.Me.IsParticipatingInTieredLootRun)
                {
                    Core.Logger.Log("[Rift] Oh well, I seem to think that the rift is still active, that means I'll not be able to clear out my packs properly, sorry in advance.");
                }
            }

            _experienceTracker.StopAndReport("Rift");
            _experienceTracker.Start();

            State = States.TownRun;
            return false;
        }

        private async Task<bool> TownRun()
        {
            Core.Logger.Debug("[TownRun] BrainBehavior.IsVendoring is {0}", BrainBehavior.IsVendoring);
            Core.Logger.Debug("[TownRun] ZetaDia.Me.IsParticipatingInTieredLootRun is {0}", ZetaDia.Me.IsParticipatingInTieredLootRun);
            Core.Logger.Debug("[TownRun] AdvDia.RiftQuest.State is {0}", AdvDia.RiftQuest.State);
            Core.Logger.Debug("[TownRun] AdvDia.RiftQuest.Step is {0}", AdvDia.RiftQuest.Step);

            if (BrainBehavior.IsVendoring)
            {
                return await TrinityTownRun.Execute();
            }
            if (!_townRunInitiated)
            {
                _townRunInitiated = true;
                BrainBehavior.ForceTownrun(" We need it.", true);
                return false;
            }

            _riftCounter++;
            Core.Logger.Log("Rifts Completed = {0}", _riftCounter);

            if (_options.RiftCount > 0 && _riftCounter >= _options.RiftCount)
            {
                Core.Logger.Log("[Rift] Rift limit set on profile tag reached. ({0})", _options.RiftCount);
                State = States.Completed;
                return Finished();
            }

            _townRunInitiated = false;
            if (AdvDia.RiftQuest.Step == RiftStep.Completed)
            {
                State = States.WaitForRiftCountdown;
                Core.Logger.Log("[Rift] Tick tock, tick tock...");
            }
            else
            {
                State = States.NotStarted;
                _moveToRiftStoneCoroutine.Reset();
            }
            return false;
        }

        private bool WaitForRiftCountdown()
        {
            if (AdvDia.RiftQuest.State != QuestState.NotStarted)
            {
                return false;
            }
            State = States.NotStarted;
            return false;
        }

        private async Task<bool> MoveToRiftStone()
        {
            if (!await _moveToRiftStoneCoroutine.GetCoroutine()) return false;
            _moveToRiftStoneCoroutine.Reset();

            if (AdvDia.IsInTown && !ZetaDia.Storage.RiftCompleted)
            {
                State = States.MoveToOrek;
            }

            if (IsRiftPortalOpen)
            {
                _prePortalWorldDynamicId = AdvDia.CurrentWorldDynamicId;
                State = States.EnteringRift;
            }
            else
            {
                State = States.OpeningRift;
            }
            return false;
        }

        private async Task<bool> OpeningRift()
        {
            if (RiftData.RiftWorldIds.Contains(AdvDia.CurrentWorldId))
            {
                State = States.OnNewRiftLevel;
                return false;
            }

            if (!await _interactWithRiftStoneInteractionCoroutine.GetCoroutine()) return false;
            await Coroutine.Wait(2500, () => UIElements.RiftDialog.IsVisible);
            _interactWithRiftStoneInteractionCoroutine.Reset();
            if (!UIElements.RiftDialog.IsVisible)
            {
                return false;
            }

            _entranceSceneNames.Clear();
            bool haveMoneyForEmpower = RiftData.EmpoweredRiftCost.TryGetValue(_level, out var empoweredCost) && ZetaDia.Storage.PlayerDataManager.ActivePlayerData.Coinage >= (empoweredCost + PluginSettings.Current.MinimumGold);
            bool canEmpower = (_riftType == RiftType.Greater && haveMoneyForEmpower);

            var settings = PluginSettings.Current;
            bool shouldEmpower = _options.IsEmpowered && _level <= settings.EmpoweredRiftLevelLimit;

            _riftStartTime = DateTime.UtcNow;
            var maximizeXp = _riftType == RiftType.Greater && _options.NormalRiftForXPShrine && (ZetaDia.Me.RestExperience < 5000000000 && ZetaDia.Me.RestExperience > -1);
            if (maximizeXp)
            {
                Core.Logger.Log("Opening Normal Rift for XP Shrine", _riftType);
                ZetaDia.Me.OpenRift(-1);
            }
            else
            {
                if (settings.UseGemAutoLevel && _riftType == RiftType.Greater)
                {
                    int maxLevel = _level;
                    int minLevel = Math.Min(maxLevel, Math.Max(13, maxLevel - settings.GemAutoLevelReductionLimit));

                    var gems = PluginSettings.Current.Gems;
                    if (gems.Gems == null)
                    {
                        gems.UpdateGems(PluginSettings.Current.GreaterRiftLevel);
                    }
                    if (gems.Gems == null)
                    {
                        Core.Logger.Error("Gems collection in Adventurer settings was not populated properly.");
                        State = States.Failed;
                        return false;
                    }
                    if (gems.Gems != null && gems.Gems.Count < 21)
                    {
                        Core.Logger.Log("We're getting a new gem on that run, running it at minimum level (GR " + minLevel + ")!");
                        _level = minLevel;
                    }
                    else
                    {
                        for (_level = minLevel; _level < maxLevel; _level++)
                        {
                            canEmpower = (RiftData.EmpoweredRiftCost.TryGetValue(_level, out empoweredCost) && ZetaDia.Storage.PlayerDataManager.ActivePlayerData.Coinage >= empoweredCost);
                            var upgradeAttempts = (canEmpower && (shouldEmpower || _level <= settings.EmpoweredRiftLevelLimit) ? 4 : 3);
                            var possibleUpgrades = gems.Gems.Sum(g => g.GetUpgrades(_level, upgradeAttempts, 100));
                            if (possibleUpgrades >= upgradeAttempts)
                            {
                                Core.Logger.Log($"Setting GR level to {_level}, RequiredChance={PluginSettings.Current.GreaterRiftGemUpgradeChance} Upgrades={possibleUpgrades} / {upgradeAttempts}");
                                break;
                            }
                        }

                        // if upgrade chance at max level is 60%, check if we can still downgrade a few levels for the same upgrade chance
                        if (_level == maxLevel && gems.Gems.Max(g => g.GetUpgradeChance(_level)) == 60)
                        {
                            Core.Logger.Log("Update chance at max level is 60%, checking if we can take a few levels off still!");
                            for (; _level > minLevel; _level--)
                            {
                                canEmpower = (RiftData.EmpoweredRiftCost.TryGetValue(_level - 1, out empoweredCost) && ZetaDia.Storage.PlayerDataManager.ActivePlayerData.Coinage >= (empoweredCost + PluginSettings.Current.MinimumGold));
                                var upgradeAttempts = (canEmpower && (shouldEmpower || _level - 1 <= settings.EmpoweredRiftLevelLimit) ? 4 : 3);
                                var possibleUpgrades = gems.Gems.Sum(g => g.GetUpgrades(_level - 1, upgradeAttempts, 60));

                                if (possibleUpgrades < upgradeAttempts)
                                    break;
                            }
                        }
                    }
                }

                if (_riftType == RiftType.Greater && shouldEmpower && canEmpower && PluginSettings.Current.UseEmpoweredRifts)
                {
                    Core.Logger.Log("Opening Empowered Greater Rift (Cost={0})", empoweredCost);
                    ZetaDia.Me.OpenRift(Math.Min(_level, ZetaDia.Me.CommonData.HighestUnlockedRiftLevel), true);
                }
                else
                {
                    Core.Logger.Log("Opening {0} Rift", _riftType);
                    ZetaDia.Me.OpenRift(Math.Min(_level, ZetaDia.Me.CommonData.HighestUnlockedRiftLevel));
                }
            }

            if (_level == -1 || maximizeXp)
            {
                await Coroutine.Yield();
                if (IsRiftPortalOpen)
                {
                    _prePortalWorldDynamicId = AdvDia.CurrentWorldDynamicId;
                    State = States.EnteringRift;
                }
            }
            else
            {
                await Coroutine.Wait(30000, () => ZetaDia.Storage.RiftStarted && RiftData.RiftWorldIds.Contains(AdvDia.CurrentWorldId) && !ZetaDia.Globals.IsLoadingWorld);
                State = States.EnteringGreaterRift;
            }
            return false;
        }

        private bool IsRiftPortalOpen
        {
            get
            {
                //ActorId: 364715, Type: Gizmo, Name: x1_OpenWorld_LootRunObelisk_B - 27053, Distance2d: 9.72007, CollisionRadius: 9.874258, MinimapActive: 1, MinimapIconOverride: 327066, MinimapDisableArrow: 0
                //ActorId: 345935, Type: Gizmo, Name: X1_OpenWorld_LootRunPortal - 27292, Distance2d: 9.72007, CollisionRadius: 8.316568, MinimapActive: 1, MinimapIconOverride: -1, MinimapDisableArrow: 0

                if (AdvDia.RiftQuest.State == QuestState.Completed || AdvDia.RiftQuest.State == QuestState.NotStarted)
                {
                    return false;
                }

                return ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).Any(g => g.ActorSnoId == RiftData.RiftEntryPortalSNO || g.ActorSnoId == RiftData.GreaterRiftEntryPortalSNO);
            }
        }

        private async Task<bool> EnteringRift()
        {
            var portal = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).FirstOrDefault(g => g.ActorSnoId == RiftData.RiftEntryPortalSNO || g.ActorSnoId == RiftData.GreaterRiftEntryPortalSNO);
            if (portal != null)
            {
                if (!await UsePortalCoroutine.UsePortal(portal.ActorSnoId, _prePortalWorldDynamicId))
                    return false;
            }
            else
            {
                var inTown = ZetaDia.IsInTown;
                Core.Logger.Debug("Expecting to find a portal here but didnt find one. InTown={0}", inTown);
                State = inTown ? States.InTown : States.Failed;
            }

            State = States.OnNewRiftLevel;
            return false;
        }

        private DateTime? _lastEnteringGreaterRiftTime;

        private bool EnteringGreaterRift()
        {
            if (ZetaDia.Globals.IsLoadingWorld || AdvDia.CurrentWorldId == ExplorationData.ActHubWorldIds[Act.A1])
            {
                if (!_lastEnteringGreaterRiftTime.HasValue)
                {
                    _lastEnteringGreaterRiftTime = DateTime.UtcNow;
                }
                else if (DateTime.UtcNow.Subtract(_lastEnteringGreaterRiftTime.Value).TotalSeconds > 10)
                {
                    Core.Logger.Debug("Stuck detected entering portal, maybe interaction coroutine has failed to do its job");
                    _lastEnteringGreaterRiftTime = null;
                    State = States.InTown;
                }
                return false;
            }
            _lastEnteringGreaterRiftTime = null;
            State = States.OnNewRiftLevel;
            return false;
        }

        private bool OnNewRiftLevel()
        {
            RiftData.AddEntryPortal();

            if (ZetaDia.IsInTown)
            {
                State = States.InTown;
                return false;
            }

            _currentExitScene = null;
            _currentEntranceScene = null;

            if (AdvDia.CurrentLevelAreaId == 276150 && !_holyCowEventCompleted)
            {
                _possiblyCowLevel = true;
            }
            if (_riftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore && AdvDia.RiftQuest.Step == RiftStep.Cleared)
            {
                State = States.SearchingForTownstoneOrExitPortal;
            }
            else
            {
                State = States.SearchingForExitPortal;
            }

            Core.Logger.Log(Randomizer.Random(1, 10) > 5
                ? "[Rift] Let the massacre continue!"
                : "[Rift] Crom, Count the Dead!");
            return false;
        }

        private static WorldScene FindEntranceScene()
        {
            // Sometimes the rift entry scene will be named 'entrance' and sometimes 'exit', 
            // so we use the marker to identify it instead.

            var entrancePortalPosition = BountyHelpers.ScanForRiftEntryMarkerLocation();
            var entranceScene = Core.Scenes.FirstOrDefault(s => s.IsInScene(entrancePortalPosition));
            return entranceScene;
        }

        private WorldScene _currentExitScene;

        private async Task<bool> SearchingForExitPortal()
        {
            if (_riftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore && AdvDia.RiftQuest.Step == RiftStep.Cleared)
            {
                State = States.SearchingForTownstoneOrExitPortal;
                return false;
            }

            if (_nextLevelPortalLocation != Vector3.Zero && (_nextLevelPortalZRequirement <= 0 || AdvDia.MyZDiff(_nextLevelPortalLocation) < _nextLevelPortalZRequirement))
            {
                State = States.MovingToExitPortal;
                return false;
            }

            if (_nextLevelPortalLocation != Vector3.Zero)
            {
                if (_nextLevelPortalLocation.Distance(Core.Player.Position) < 150)
                {
                    State = States.MovingToExitPortal;
                    return false;
                }
            }

            //if (Core.Player.IsInParty && RiftData.GetGreaterRiftLevel() > 55 && TrinityPluginSettings.Settings.Advanced.BetaPlayground)
            //{
            //    var deadPlayer =
            //        ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true)
            //            .FirstOrDefault(u => u.IsValid && u.CommonData != null && u.CommonData.IsValid && !u.IsAlive);

            //    if (deadPlayer != null && deadPlayer.Distance > 15)
            //    {
            //        if (!await NavigationCoroutine.MoveTo(deadPlayer.Position, 15)) return false;
            //    }

            //    var players =
            //            ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true)
            //                .Where(
            //                    u =>
            //                        u.IsValid && u.CommonData != null && u.CommonData.IsValid &&
            //                        u.WorldId == ZetaDia.Me.WorldId && u.HitpointsMaxTotal < 999999999);

            //    var diaPlayers = players as DiaPlayer[] ?? players.ToArray();
            //    var maxhpplayer = diaPlayers.OrderByDescending(x => x.HitpointsMaxTotal).FirstOrDefault();

            //    if (!PluginManager.GetEnabledPlugins().Any(u => u.Contains("AutoFollow")) && maxhpplayer != null && maxhpplayer.HitpointsMaxTotal > ZetaDia.Me.HitpointsMaxTotal)
            //    {
            //        if (maxhpplayer.Distance > 20 && ZetaDia.Me.IsInCombat || !ZetaDia.Me.IsInCombat)
            //        {
            //            var say = "[Follower] Got to far away.  Trying to follow the Tank with HPs: " +
            //                      maxhpplayer.HitpointsMaxTotal + "!";
            //            if (lastError == null || say != lastError)
            //            {
            //                Core.Logger.Log(say);
            //                lastError = say;
            //                Core.Logger.Log(lastError);
            //            }
            //            if (!await NavigationCoroutine.MoveTo(maxhpplayer.Position, 5)) return false;
            //        }
            //    }
            //}

            if (!await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId })) return false;
            Core.Scenes.Reset();
            return false;
        }

        private async Task<bool> SearchingForTownstoneOrExitPortal()
        {
            if (_nextLevelPortalLocation != Vector3.Zero)
            {
                State = States.MovingToExitPortal;
                return false;
            }

            //if (Core.Player.IsInParty && RiftData.GetGreaterRiftLevel() > 55 && TrinityPluginSettings.Settings.Advanced.BetaPlayground)
            //{
            //    var deadPlayer =
            //        ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true)
            //            .FirstOrDefault(u => u.IsValid && u.CommonData != null && u.CommonData.IsValid && !u.IsAlive);

            //    if (deadPlayer != null && deadPlayer.Distance > 15)
            //    {
            //        if (!await NavigationCoroutine.MoveTo(deadPlayer.Position, 15)) return false;
            //    }

            //    var players =
            //            ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true)
            //                .Where(
            //                    u =>
            //                        u.IsValid && u.CommonData != null && u.CommonData.IsValid &&
            //                        u.WorldId == ZetaDia.Me.WorldId && u.HitpointsMaxTotal < 999999999);

            //    var diaPlayers = players as DiaPlayer[] ?? players.ToArray();
            //    var maxhpplayer = diaPlayers.OrderByDescending(x => x.HitpointsMaxTotal).FirstOrDefault();

            //    if (!PluginManager.GetEnabledPlugins().Any(u => u.Contains("AutoFollow")) && maxhpplayer != null && maxhpplayer.HitpointsMaxTotal > ZetaDia.Me.HitpointsMaxTotal)
            //    {
            //        if (maxhpplayer.Distance > 20 && ZetaDia.Me.IsInCombat || !ZetaDia.Me.IsInCombat)
            //        {
            //            var say = "[Follower] Got to far away.  Trying to follow the Tank with HPs: " +
            //                      maxhpplayer.HitpointsMaxTotal + "!";
            //            if (lastError == null || say != lastError)
            //            {
            //                Core.Logger.Log(say);
            //                lastError = say;
            //                Core.Logger.Log(lastError);
            //            }
            //            if (!await NavigationCoroutine.MoveTo(maxhpplayer.Position, 5)) return false;
            //        }
            //    }
            //}

            if (!await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId })) return false;
            Core.Scenes.Reset();
            return false;
        }

        private bool TownstoneFound()
        {
            Core.Logger.Log("[Rift] That's it folks, returning to town.");
            State = States.ReturningToTown;
            return false;
        }

        private async Task<bool> MovingToExitPortal()
        {
            if (!await NavigationCoroutine.MoveTo(_nextLevelPortalLocation, 15))
            {
                return false;
            }

            _nextLevelPortalLocation = Vector3.Zero;
            if (NavigationCoroutine.LastResult == CoroutineResult.Failure)
            {
                var canPathTo = await AdvDia.Navigator.CanFullyClientPathTo(_nextLevelPortalLocation);
                if (!canPathTo)
                {
                    // Handle the situation where the bot is standing directly above the exit portal
                    // but the scenes required to path to the portal have not yet been discovered/loaded.
                    var zDiff = AdvDia.MyZDiff(_nextLevelPortalLocation);
                    if (zDiff > 5f)
                    {
                        _nextLevelPortalZRequirement = _nextLevelPortalZRequirement == 0 ? 15 : Math.Max(5, _nextLevelPortalZRequirement - 5);
                    }
                    Core.Logger.Debug($"Cannot fully client path to destination, ZDiffReq={_nextLevelPortalZRequirement} CurrentZDiff={zDiff}");
                }
                _portalScanRange = ActorFinder.LowerSearchRadius(_portalScanRange);
                if (_portalScanRange <= 100)
                {
                    _portalScanRange = 100;
                }
                if (_riftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore &&
                    AdvDia.RiftQuest.Step == RiftStep.Cleared)
                {
                    State = States.SearchingForTownstoneOrExitPortal;
                }
                else
                {
                    State = States.SearchingForExitPortal;
                }
                return false;
            }
            var portal =
                ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
                    .Where(g => g.IsFullyValid() && g.IsPortal)
                    .OrderBy(g => g.Distance)
                    .FirstOrDefault();
            if (portal == null)
            {
                portal = BountyHelpers.GetPortalNearPosition(_nextLevelPortalLocation);
                if (portal == null)
                {
                    if (_riftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore &&
                        AdvDia.RiftQuest.Step == RiftStep.Cleared)
                    {
                        State = States.SearchingForTownstoneOrExitPortal;
                    }
                    else
                    {
                        State = States.SearchingForExitPortal;
                    }
                    return false;
                }
            }
            State = States.EnteringExitPortal;
            _nextLevelPortalSNO = portal.ActorSnoId;
            _prePortalWorldDynamicId = AdvDia.CurrentWorldDynamicId;
            return false;
        }

        private async Task<bool> EnteringExitPortal()
        {
            if (!await UsePortalCoroutine.UsePortal(_nextLevelPortalSNO, _prePortalWorldDynamicId)) return false;

            State = States.OnNewRiftLevel;
            return false;
        }

        private bool BossSpawned()
        {
            if (AdvDia.RiftQuest.Step != RiftStep.Cleared)
            {
                State = States.SearchingForBoss;
            }
            else
            {
                if (_riftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore)
                {
                    State = States.SearchingForTownstoneOrExitPortal;
                }
                else
                {
                    State = States.Completed;
                }
            }
            return false;
        }

        private async Task<bool> SearchingForBoss()
        {
            if (_bossLocation != Vector3.Zero)
            {
                State = States.MovingToBoss;
                return false;
            }
            if (!await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId })) return false;
            Core.Logger.Log("[Rift] The Boss must be scared, but we will find him!");
            Core.Scenes.Reset();
            return false;
        }

        private async Task<bool> MovingToBoss()
        {
            if (AdvDia.RiftQuest.Step == RiftStep.Cleared)
            {
                if (_riftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore)
                {
                    State = States.SearchingForTownstoneOrExitPortal;
                }
                else
                {
                    State = States.Completed;
                }
            }

            if (!await NavigationCoroutine.MoveTo(_bossLocation, 5)) return false;
            if (AdvDia.MyPosition.Distance(_bossLocation) > 50)
            {
                _bossLocation = Vector3.Zero;
                State = States.SearchingForBoss;
                return false;
            }
            _bossLocation = Vector3.Zero;
            if (AdvDia.RiftQuest.Step != RiftStep.Cleared)
            {
                Core.Logger.Log("[Rift] You will suffer and die, ugly creature!");
                State = States.KillingBoss;
            }
            return false;
        }

        private bool KillingBoss()
        {
            if (AdvDia.RiftQuest.Step != RiftStep.Cleared)
            {
                return false;
            }
            State = _level == -1 ? States.Completed : States.UrshiSpawned;
            return false;
        }

        private bool UrshiSpawned()
        {
            State = States.SearchingForUrshi;
            return false;
        }

        private async Task<bool> SearchingForUrshi()
        {
            if (_urshiLocation != Vector3.Zero)
            {
                State = States.MovingToUrshi;
                return false;
            }
            if (!await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId })) return false;
            Core.Logger.Log("[Rift] Where are you, my dear Urshi!");
            Core.Scenes.Reset();
            return false;
        }

        private async Task<bool> MovingToUrshi()
        {
            if (!await NavigationCoroutine.MoveTo(_urshiLocation, 5)) return false;
            _urshiLocation = Vector3.Zero;
            State = States.InteractingWithUrshi;
            return false;
        }

        private async Task<bool> InteractingWithUrshi()
        {
            if (RiftData.VendorDialog.IsVisible)
            {
                State = States.UpgradingGems;
                return false;
            }
            if (!await _interactWithUrshiCoroutine.GetCoroutine()) return false;
            await Coroutine.Wait(2500, () => RiftData.VendorDialog.IsVisible);
            _interactWithUrshiCoroutine.Reset();
            if (!RiftData.VendorDialog.IsVisible)
            {
                return false;
            }

            _gemUpgradesLeft = 3;
            State = States.UpgradingGems;
            return false;
        }

        private int _gemUpgradesLeft;

        private async Task<bool> UpgradingGems()
        {
            if (RiftData.VendorDialog.IsVisible && RiftData.ContinueButton.IsVisible && RiftData.ContinueButton.IsEnabled)
            {
                Core.Logger.Debug("[Rift] Clicking to Continue button.");
                RiftData.ContinueButton.Click();
                RiftData.VendorCloseButton.Click();
                await Coroutine.Yield();
                return false;
            }

            var gemToUpgrade = PluginSettings.Current.Gems.GetUpgradeTarget();
            if (gemToUpgrade == null)
            {
                Core.Logger.Log("[Rift] I couldn't find any gems to upgrade, failing.");
                State = States.Failed;
                return false;
            }
            if (AdvDia.RiftQuest.Step == RiftStep.Cleared)
            {
                Core.Logger.Debug("[Rift] Rift Quest is completed, returning to town");
                State = States.Completed;
                return false;
            }

            Core.Logger.Debug("[Rift] Gem upgrades left before the attempt: {0}", ZetaDia.Me.JewelUpgradesLeft);
            if (!await CommonCoroutines.AttemptUpgradeGem(gemToUpgrade))
            {
                Core.Logger.Debug("[Rift] Gem upgrades left after the attempt: {0}", ZetaDia.Me.JewelUpgradesLeft);
                return false;
            }
            var gemUpgradesLeft = ZetaDia.Me.JewelUpgradesLeft;
            if (_gemUpgradesLeft != gemUpgradesLeft)
            {
                _gemUpgradesLeft = gemUpgradesLeft;
            }
            if (AdvDia.RiftQuest.State == QuestState.Completed && AdvDia.RiftQuest.Step != RiftStep.UrshiSpawned)//gemUpgradesLeft == 0)
            {
                Core.Logger.Debug("[Rift] Finished all upgrades, returning to town.");
                State = States.Completed;
                return false;
            }

            return false;
        }

        //private ACDItem GetUpgradeTarget(bool enableLog)
        //{
        //    ZetaDia.Actors.Update();
        //    var gems = PluginSettings.Current.Gems;
        //    gems.UpdateGems(ZetaDia.Me.InTieredLootRunLevel + 1, PluginSettings.Current.GreaterRiftPrioritizeEquipedGems);

        //    var minChance = PluginSettings.Current.GreaterRiftGemUpgradeChance;
        //    var upgradeableGems =
        //        gems.Gems.Where(g => g.UpgradeChance >= minChance && !g.IsMaxRank).ToList();
        //    if (upgradeableGems.Count == 0)
        //    {
        //        if (enableLog) Core.Logger.Log("[Rift] Couldn't find any gems which is over the minimum upgrade change, upgrading the gem with highest upgrade chance");
        //        upgradeableGems = gems.Gems.Where(g => !g.IsMaxRank).OrderByDescending(g => g.UpgradeChance).ToList();
        //    }
        //    if (upgradeableGems.Count == 0)
        //    {
        //        if (enableLog) Core.Logger.Log("[Rift] Looks like you have no legendary gems, failing.");
        //        State = States.Failed;
        //        return null;
        //    }
        //    var gemToUpgrade = upgradeableGems.First();
        //    if (enableLog) Core.Logger.Log("[Rift] Attempting to upgrade {0}", gemToUpgrade.DisplayName);
        //    var acdGem =
        //        ZetaDia.Actors.GetActorsOfType<ACDItem>()
        //            .FirstOrDefault(
        //                i =>
        //                    i.ItemType == ItemType.LegendaryGem && i.ActorSnoId == gemToUpgrade.SNO &&
        //                    i.JewelRank == gemToUpgrade.Rank);
        //    return acdGem;
        //}

        private async Task<bool> SearchingForHolyCow()
        {
            if (_holyCowLocation != Vector3.Zero)
            {
                Core.Logger.Log("[Rift] Mooooo!");
                State = States.MovingToHolyCow;
                return false;
            }
            if (!await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId })) return false;
            Core.Logger.Log("[Rift] I am no butcher, where is this cow?");
            Core.Scenes.Reset();
            return false;
        }

        private async Task<bool> MovingToHolyCow()
        {
            if (!await NavigationCoroutine.MoveTo(_holyCowLocation, 5)) return false;
            _holyCowLocation = Vector3.Zero;
            Core.Logger.Log("[Rift] Mooooo?");
            State = States.InteractingWithHolyCow;
            return false;
        }

        private async Task<bool> InteractingWithHolyCow()
        {
            if (!await _talkToHolyCowCoroutine.GetCoroutine()) return false;
            _talkToHolyCowCoroutine.Reset();
            Core.Logger.Log("[Rift] Mooo moooo....");
            State = States.SearchingForExitPortal;
            return false;
        }

        private bool Completed()
        {
            BotMain.SetCurrentStatusTextProvider(null);
            State = States.ReturningToTown;
            return false;
        }

        private bool Failed()
        {
            BotMain.SetCurrentStatusTextProvider(null);
            Core.Logger.Error("[Rift] Failed to complete the rift.");
            return true;
        }

        private bool Finished()
        {
            BotMain.SetCurrentStatusTextProvider(null);
            return true;
        }

        #region Pulse Checks & Scans

        private static readonly HashSet<States> s_bossSpawnedStates = new HashSet<States> { States.BossSpawned, States.SearchingForBoss, States.KillingBoss, States.MovingToBoss };
        private static readonly HashSet<States> s_urshiSpawnedStates = new HashSet<States> { States.UrshiSpawned, States.SearchingForUrshi, States.InteractingWithUrshi, States.MovingToUrshi };
        private static readonly HashSet<States> s_clearedStates = new HashSet<States> { States.InTown, States.GoingToAct1Hub, States.ReturningToTown, States.MoveToOrek, States.TalkToOrek, States.TownstoneFound };
        private static readonly HashSet<States> s_enteringRiftStates = new HashSet<States> { States.MoveToRiftStone, States.EnteringRift, States.OpeningRift };

        private void Scans()
        {
            switch (State)
            {
                case States.SearchingForExitPortal:
                    if (_possiblyCowLevel)
                    {
                        ScanForHolyCow();
                    }
                    ScanForEntranceScene();
                    ScanForExitPortal();
                    ScanForExitScene();
                    return;

                case States.SearchingForBoss:
                    ScanForBoss();
                    return;

                case States.SearchingForHolyCow:
                    ScanForHolyCow();
                    return;

                case States.SearchingForUrshi:
                    ScanForUrshi();
                    return;

                case States.SearchingForTownstoneOrExitPortal:
                    if (_possiblyCowLevel)
                    {
                        ScanForHolyCow();
                    }
                    ScanForEntranceScene();
                    ScanForExitPortal();
                    ScanForExitScene();
                    ScanForTownstone();
                    return;
            }
        }

        private void ScanForEntranceScene()
        {
            if (_currentEntranceScene == null)
            {
                var entranceScene = FindEntranceScene();
                if (entranceScene != null && !_entranceSceneNames.Contains(entranceScene.HashName))
                {
                    Core.Logger.Warn($"Found the marker entrance scene '{entranceScene.Name}' ({entranceScene.SnoId}) {entranceScene.Center.Distance(Core.Player.Position.ToVector2())} yards away!");

                    _currentEntranceScene = entranceScene;
                    _entranceSceneNames.Add(entranceScene.HashName);
                }
            }
        }

        private void ScanForExitScene()
        {
            if (_currentExitScene != null)
                return;

            var exitMarkerPosition = BountyHelpers.ScanForRiftExitMarkerLocation();
            if (exitMarkerPosition != Vector3.Zero)
            {
                var markerExitScene = Core.Scenes.FirstOrDefault(s => s.IsInScene(exitMarkerPosition));
                if (markerExitScene != null)
                {
                    _currentExitScene = markerExitScene;
                    var exitSceneConnection = markerExitScene.ExitPositions.FirstOrDefault();
                    ExplorationHelpers.SetExplorationPriority(exitSceneConnection.Value);
                    Core.Logger.Warn($"Found the marker exit '{markerExitScene.Name}' ({markerExitScene.SnoId}) {exitSceneConnection.Value.Distance(Core.Player.Position)} yards away!");
                }
                return;
            }

            if (_currentEntranceScene == null)
                return;

            var exitName = _currentEntranceScene.HashName.ToLowerInvariant().Contains("entrance") ? "exit" : "entrance";
            var exitScene = Core.Scenes.FirstOrDefault(s => s.Name.ToLowerInvariant().Contains(exitName) && !_entranceSceneNames.Contains(s.HashName));
            if (exitScene != null)
            {
                _currentExitScene = exitScene;
                var exitSceneConnection = exitScene.ExitPositions.FirstOrDefault();
                ExplorationHelpers.SetExplorationPriority(exitSceneConnection.Value);
                Core.Logger.Warn($"Found the exit '{exitScene.Name}' ({exitScene.SnoId}) {exitSceneConnection.Value.Distance(Core.Player.Position)} yards away!");
            }
        }

        private async Task<bool> PulseChecks()
        {
            if (BrainBehavior.IsVendoring || !ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld || ZetaDia.Globals.IsPlayingCutscene)
            {
                return false;
            }

            if (AdvDia.CurrentWorldId == ExplorationData.ActHubWorldIds[Act.A1] &&
                (AdvDia.RiftQuest.Step == RiftStep.KillingMobs || AdvDia.RiftQuest.Step == RiftStep.BossSpawned || AdvDia.RiftQuest.Step == RiftStep.UrshiSpawned))
            {
                if (!s_enteringRiftStates.Contains(State))
                {
                    Core.Logger.Log("[Rift] Oh darn, I managed to return to town, I better go back in the rift before anyone notices.");
                    State = States.MoveToRiftStone;
                    return false;
                }
            }

            if (AdvDia.CurrentWorldId == ExplorationData.ActHubWorldIds[Act.A1] && State != States.EnteringRift)
            {
                return false;
            }

            if (_currentWorldDynamicId != AdvDia.CurrentWorldDynamicId && _previusWorldDynamicId != AdvDia.CurrentWorldDynamicId && AdvDia.CurrentWorldId != ExplorationData.ActHubWorldIds[Act.A1])
            {
                RiftData.AddEntryPortal();
                _previusWorldDynamicId = _currentWorldDynamicId;
                _currentWorldDynamicId = AdvDia.CurrentWorldDynamicId;
            }

            switch (AdvDia.RiftQuest.Step)
            {
                case RiftStep.BossSpawned:
                    if (!s_bossSpawnedStates.Contains(State))
                    {
                        Core.Logger.Log("[Rift] Behold the Rift Boss!");
                        State = States.BossSpawned;
                    }
                    break;

                case RiftStep.UrshiSpawned:
                    if (!s_urshiSpawnedStates.Contains(State))
                    {
                        _riftEndTime = DateTime.UtcNow;
                        var totalTime = _riftEndTime - _riftStartTime;
                        if (totalTime.TotalSeconds < 3600 && totalTime.TotalSeconds > 0)
                        {
                            Core.Logger.Log("[Rift] All done. (Total Time: {0} mins {1} seconds)", totalTime.Minutes, totalTime.Seconds);
                            Core.Logger.Log("[Rift] Level: {0}", ZetaDia.Me.InTieredLootRunLevel + 1);
                        }
                        else
                        {
                            Core.Logger.Log("[Rift] All done. (Partial rift, no stats available)");
                        }
                        Core.Logger.Log("[Rift] My dear Urshi, I have some gems for you.");
                        State = States.UrshiSpawned;
                    }
                    break;

                case RiftStep.Cleared:
                    if (_riftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore && State != States.TownstoneFound)
                    {
                        break;
                    }
                    if (!s_clearedStates.Contains(State))
                    {
                        _riftEndTime = DateTime.UtcNow;
                        var totalTime = _riftEndTime - _riftStartTime;
                        if (totalTime.TotalSeconds < 3600 && totalTime.TotalSeconds > 0)
                        {
                            Core.Logger.Log("[Rift] All done. (Total Time: {0} mins {1} seconds)", totalTime.Minutes, totalTime.Seconds);
                        }
                        else
                        {
                            Core.Logger.Log("[Rift] All done. (Partial rift, no stats available)");
                        }
                        State = States.ReturningToTown;
                    }
                    break;
            }
            return true;
        }

        private void ScanForUrshi()
        {
            var urshi =
                ZetaDia.Actors.GetActorsOfType<DiaUnit>()
                    .FirstOrDefault(a => a.IsFullyValid() && a.ActorSnoId == RiftData.UrshiSNO);
            if (urshi != null)
            {
                _urshiLocation = urshi.Position;
            }
            if (_urshiLocation != Vector3.Zero)
            {
                Core.Logger.Log("[Rift] Urshi is near.");
                State = States.MovingToUrshi;
                Core.Logger.Debug("[Rift] Found Urshi at distance {0}", AdvDia.MyPosition.Distance(_urshiLocation));
            }
        }

        private void ScanForBoss()
        {
            //// removed due to mistaking shrine markers for boss marker
            //// todo: find boss marker and use that when far away.

            //var portalMarker = AdvDia.CurrentWorldMarkers.FirstOrDefault(m => m.Id >= 0 && m.Id <= 200);
            //if (portalMarker != null)
            //{
            //    _bossLocation = portalMarker.Position;
            //}

            if (_bossLocation == Vector3.Zero)
            {
                var boss =
                    ZetaDia.Actors.GetActorsOfType<DiaUnit>()
                        .FirstOrDefault(a => a.IsFullyValid() && a.CommonData.IsUnique);
                if (boss != null)
                {
                    if (boss.IsAlive)
                    {
                        _bossLocation = boss.Position;
                    }
                }
            }
            if (_bossLocation != Vector3.Zero)
            {
                Core.Logger.Log("[Rift] The Boss is near.");
                State = States.MovingToBoss;
                Core.Logger.Debug("[Rift] Found the boss at distance {0}", AdvDia.MyPosition.Distance(_bossLocation));
            }
        }

        private void ScanForHolyCow()
        {
            var holyCow =
                ZetaDia.Actors.GetActorsOfType<DiaUnit>()
                    .FirstOrDefault(a => a.IsFullyValid() && a.ActorSnoId == RiftData.HolyCowSNO && a.IsInteractableQuestObject());
            if (holyCow != null)
            {
                _holyCowLocation = holyCow.Position;
                switch (State)
                {
                    case States.SearchingForExitPortal:
                        State = States.SearchingForHolyCow;
                        break;
                }
            }
        }

        private void ScanForExitPortal()
        {
            if (_nextLevelPortalLocation != Vector3.Zero) return;
            var portal =
                ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
                    .Where(
                        g =>
                            g.IsFullyValid() && g.IsPortal && !RiftData.DungeonStoneSNOs.Contains(g.ActorSnoId) &&
                            g.Distance < _portalScanRange && g.CommonData.GizmoType != GizmoType.HearthPortal)
                    .OrderBy(g => g.Position.Distance2DSqr(AdvDia.MyPosition))
                    .FirstOrDefault();
            if (portal != null)
            {
                _nextLevelPortalLocation = portal.Position;
                var currentWorldExitPortalHash = RiftData.GetRiftExitPortalHash(AdvDia.CurrentWorldId);
                var marker = AdvDia.CurrentWorldMarkers.FirstOrDefault(m => m.Position.Distance(_nextLevelPortalLocation) < 10);
                if (marker == null)
                {
                    _nextLevelPortalLocation = Vector3.Zero;
                }
                else
                {
                    if (marker.NameHash != currentWorldExitPortalHash)
                    {
                        _nextLevelPortalLocation = Vector3.Zero;
                    }
                }
            }
            if (_nextLevelPortalLocation != Vector3.Zero && !EntranceScenes.Any(s => s.IsInScene(_nextLevelPortalLocation)))
            {
                Core.Logger.Log("[Rift] Oh look! There is a portal over there, let's see what's on the other side.");
                Core.Logger.Debug("[Rift] Found the objective at distance {0}",
                    AdvDia.MyPosition.Distance(_nextLevelPortalLocation));
            }
        }

        private IEnumerable<WorldScene> EntranceScenes
        {
            get { return Core.Scenes.Where(s => _entranceSceneNames.Contains(s.HashName)); }
        }

        private void ScanForTownstone()
        {
            var townStone =
                ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true)
                    .FirstOrDefault(a => a.IsFullyValid() && a.ActorSnoId == RiftData.TownstoneSNO);
            if (townStone != null)
            {
                State = States.TownstoneFound;
            }
        }

        #endregion Pulse Checks & Scans

        #region OnPulse Implementation

        private readonly WaitTimer _pulseTimer = new WaitTimer(TimeSpan.FromMilliseconds(250));
        private readonly RiftOptions _options;
        private int _riftCounter;
        private int _nextLevelPortalZRequirement;

        private readonly List<string> _entranceSceneNames = new List<string>();
        private WorldScene _currentEntranceScene;

        private void OnPulse(object sender, EventArgs e)
        {
            if (!_pulseTimer.IsFinished) return;

            _pulseTimer.Stop();
            Scans();
            _pulseTimer.Reset();
        }

        #endregion OnPulse Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool dispose)
        {
            _experienceTracker.Dispose();

            Pulsator.OnPulse -= OnPulse;
            Core.Logger.Debug("[Rift] Unregistered from pulsator.");
        }
    }
}
