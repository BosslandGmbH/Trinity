using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Components.Adventurer.Game.Stats;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Common.Helpers;
using Zeta.Common.Plugins;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Coroutines.RiftCoroutines
{
    using Framework;

    public class RiftCoroutine : IDisposable, ICoroutine
    {
        public class RiftOptions
        {
            public int RiftCount = 0;
            public bool IsEmpowered = false;
            public States EndState = States.Finished;
            public bool NormalRiftForXPShrine;
        }

        private RiftType _RiftType;
        private bool _runningNephalemInsteadOfGreaterRift;
        private int _level;
        private States _state;
        private bool _townRunInitiated;
        private int _portalScanRange = 150;

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
        private ExperienceTracker _experienceTracker = new ExperienceTracker();

        private readonly MoveToPositionCoroutine _moveToRiftStoneCoroutine = new MoveToPositionCoroutine(ExplorationData.ActHubWorldIds[Act.A1], RiftData.Act1RiftStonePosition);
        private readonly InteractionCoroutine _interactWithRiftStoneInteractionCoroutine = new InteractionCoroutine(RiftData.RiftStoneSNO, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1));
        private readonly InteractionCoroutine _interactWithUrshiCoroutine = new InteractionCoroutine(RiftData.UrshiSNO, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), 3);

        private readonly MoveToPositionCoroutine _moveToOrekCoroutine = new MoveToPositionCoroutine(ExplorationData.ActHubWorldIds[Act.A1], RiftData.Act1OrekPosition);
        private readonly InteractionCoroutine _talkToOrekCoroutine = new InteractionCoroutine(RiftData.OrekSNO, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(400), 3);
        private readonly InteractionCoroutine _talkToHolyCowCoroutine = new InteractionCoroutine(RiftData.HolyCowSNO, TimeSpan.FromSeconds(2), TimeSpan.FromMilliseconds(1000), 3);

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
            get { return _state; }
            set
            {
                if (_state == value) return;
                if (value != States.NotStarted)
                {
                    Util.Logger.Debug("[Rift] " + value);
                }
                _state = value;
            }
        }

        #endregion

        public RiftCoroutine(RiftType RiftType, RiftOptions options = null)
        {
            _RiftType = RiftType;
            if (RiftType == RiftType.Nephalem)
            {
                _level = -1;
            }
            else
            {
                _level = RiftData.GetGreaterRiftLevel();
            }

            _id = Guid.NewGuid();

            _options = options ?? new RiftOptions();
        }

        private readonly Guid _id;
        public Guid Id
        {
            get { return _id; }
        }

        public void Reset()
        {
            _riftCounter = 0;
            State = States.NotStarted;
        }

        public virtual async Task<bool> GetCoroutine()
        {
            if (_isPulsing)
            {
                PulseChecks();
            }

            if (State == _options.EndState)
            {
                Util.Logger.Debug("[Rift] Someone told us to stop rifting, so we will do what we're told like a good boy and/or girl.");
                State = States.Finished;
                DisablePulse();
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
                    return TownRun();
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
		
		private long minimumRiftKeys = 50; // todo make accessible through UI and save in settings etc

        private long CurrentRiftKeyCount
        {
            get
            {
				long keyCount = AdvDia.StashAndBackpackItems.Where(i => i.IsValid && i.ActorSnoId == RiftData.GreaterRiftKeySNO).Sum(c => c.ItemStackQuantity);
				Logger.Info("I have {0} rift keys.", keyCount);
                return keyCount;
            }
        }

        private bool NotStarted()
        {
            if(!_experienceTracker.IsStarted) _experienceTracker.Start();
            SafeZerg.Instance.DisableZerg();

            if (_RiftType == RiftType.Greater)
            {
                _level = RiftData.GetGreaterRiftLevel();
            }
            if (_runningNephalemInsteadOfGreaterRift && CurrentRiftKeyCount > minimumRiftKeys)
            {
                _level = RiftData.GetGreaterRiftLevel();
                _RiftType = RiftType.Greater;
                _runningNephalemInsteadOfGreaterRift = false;
                return false;
            }
            if (AdvDia.RiftQuest.State == QuestState.NotStarted && _RiftType == RiftType.Greater && CurrentRiftKeyCount <= minimumRiftKeys)
            {
                if (PluginSettings.Current.GreaterRiftRunNephalem)
                {
                    _level = -1;
                    _RiftType = RiftType.Nephalem;
                    _runningNephalemInsteadOfGreaterRift = true;
                    return false;
                }
                else
                {
                    Util.Logger.Error("You have no Greater Rift Keys. Stopping the bot.");
                    BotMain.Stop();
                    return true;
                }
            }
            _currentWorldDynamicId = AdvDia.CurrentWorldDynamicId;
            if (AdvDia.RiftQuest.State == QuestState.InProgress && RiftData.RiftWorldIds.Contains(AdvDia.CurrentWorldId))
            {
                State = States.SearchingForExitPortal;
                return false;
            }
            State = AdvDia.CurrentWorldId == ExplorationData.ActHubWorldIds[Act.A1] ? States.InTown : States.GoingToAct1Hub;
            if (AdvDia.RiftQuest.State == QuestState.NotStarted)
            {
                ScenesStorage.Reset();
                RiftData.EntryPortals.Clear();
                _currentWorldDynamicId = 0;
                _previusWorldDynamicId = 0;
                _bossLocation = Vector3.Zero;
                _nextLevelPortalLocation = Vector3.Zero;
                _holyCowLocation = Vector3.Zero;
                _holyCowEventCompleted = false;
                _possiblyCowLevel = false;
            }
            return false;
        }

        private WaitTimer _goingToAct1HubWaitTimer;
        private async Task<bool> GoingToAct1Hub()
        {
            DisablePulse();
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
            DisablePulse();
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
            DisablePulse();
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
                    Util.Logger.Info("[Rift] I think I should go and brag to Orek about my success.");
                    State = States.MoveToOrek;
                    return false;
                case RiftStep.BossSpawned:
                    Util.Logger.Info("[Rift] I wonder why am I in town while the boss is spawned in the rift, I'm taking my chances with this portal.");
                    State = States.MoveToRiftStone;
                    return false;
                case RiftStep.UrshiSpawned:
                    Util.Logger.Info("[Rift] I wonder why am I in town while Urshi spawned in the rift, I'm taking my chances with this portal.");
                    State = States.MoveToRiftStone;
                    return false;
                case RiftStep.KillingMobs:
                    Util.Logger.Info("[Rift] I wonder why am I in town while there are many mobs to kill in the rift, I'm taking my chances with this portal.");
                    State = States.MoveToRiftStone;
                    return false;
                case RiftStep.NotStarted:
                    State = States.MoveToRiftStone;
                    _moveToRiftStoneCoroutine.Reset();
                    Util.Logger.Info("[Rift] Time to kill some scary monsters. Chop chop!");
                    return false;
                default:
                    Util.Logger.Info("[Rift] I really don't know what to do now.");
                    State = States.Failed;
                    return false;
            }
        }

        private async Task<bool> MoveToOrek()
        {
            DisablePulse();
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

            DisablePulse();
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
                await Coroutine.Sleep(1200);
                await Coroutine.Wait(TimeSpan.FromSeconds(5), () => !ZetaDia.Me.IsParticipatingInTieredLootRun);
                if (ZetaDia.Me.IsParticipatingInTieredLootRun)
                {
                    Util.Logger.Info("[Rift] Oh well, I seem to think that the rift is still active, that means I'll not be able to clear out my packs properly, sorry in advance.");
                }
            }

            _experienceTracker.StopAndReport("Rift");
            _experienceTracker.Start();

            State = States.TownRun;
            return false;
        }

        private bool TownRun()
        {
            Util.Logger.Debug("[TownRun] BrainBehavior.IsVendoring is {0}", BrainBehavior.IsVendoring);
            Util.Logger.Debug("[TownRun] ZetaDia.Me.IsParticipatingInTieredLootRun is {0}", ZetaDia.Me.IsParticipatingInTieredLootRun);
            Util.Logger.Debug("[TownRun] AdvDia.RiftQuest.State is {0}", AdvDia.RiftQuest.State);
            Util.Logger.Debug("[TownRun] AdvDia.RiftQuest.Step is {0}", AdvDia.RiftQuest.Step);
            DisablePulse();
            if (BrainBehavior.IsVendoring)
            {
                return false;
            }
            if (!_townRunInitiated)
            {
                _townRunInitiated = true;
                BrainBehavior.ForceTownrun(" We need it.", true);
                return false;
            }

            _riftCounter++;
            Util.Logger.Info("Rifts Completed = {0}", _riftCounter);

            if (_options.RiftCount > 0 && _riftCounter >= _options.RiftCount)
            {
                Util.Logger.Info("[Rift] Rift limit set on profile tag reached. ({0})", _options.RiftCount);
                State = States.Completed;
                return Finished();
            }

            _townRunInitiated = false;
            if (AdvDia.RiftQuest.Step == RiftStep.Completed)
            {
                State = States.WaitForRiftCountdown;
                Util.Logger.Info("[Rift] Tick tock, tick tock...");
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
            DisablePulse();
            if (AdvDia.RiftQuest.State != QuestState.NotStarted)
            {
                return false;
            }
            State = States.NotStarted;
            return false;
        }

        private async Task<bool> MoveToRiftStone()
        {
            DisablePulse();
            if (!await _moveToRiftStoneCoroutine.GetCoroutine()) return false;
            _moveToRiftStoneCoroutine.Reset();

            if (AdvDia.IsInTown && ZetaDia.CurrentRift != null && !ZetaDia.CurrentRift.IsCompleted)
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
		
		private long minimumCoinageKept = 1000000000; // todo add to GUI and settings, keeping gold for reforges and empowering self-run rifts

        private async Task<bool> OpeningRift()
        {
            if (RiftData.RiftWorldIds.Contains(AdvDia.CurrentWorldId))
            {
                State = States.OnNewRiftLevel;
                return false;
            }

            DisablePulse();
            if (!await _interactWithRiftStoneInteractionCoroutine.GetCoroutine()) return false;
            await Coroutine.Wait(2500, () => UIElements.RiftDialog.IsVisible);
            _interactWithRiftStoneInteractionCoroutine.Reset();
            if (!UIElements.RiftDialog.IsVisible)
            {
                return false;
            }

            long empoweredCost = 0;
            bool shouldEmpower = _options.IsEmpowered;
			bool haveMoneyForEmpower = RiftData.EmpoweredRiftCost.TryGetValue(_level, out empoweredCost) && ZetaDia.PlayerData.Coinage >= (empoweredCost + minimumCoinageKept);
            bool canEmpower = (_RiftType == RiftType.Greater && haveMoneyForEmpower);
            var settings = global::Trinity.Components.Adventurer.Settings.PluginSettings.Current;

            _riftStartTime = DateTime.UtcNow;
            const int waittime = 45;
            const int partysize = 3; // ToDo: Add slider for party size under beta playground checkbox

            if (TrinityPluginSettings.Settings.Advanced.BetaPlayground)
            {
                if (Core.Player.IsInParty &&
                    ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true).Count() < ZetaDia.Service.Party.NumPartyMembers)
                {
                    Logger.Info("Waiting until all party is present.");
                    await Coroutine.Wait(TimeSpan.FromMinutes(60),
                            () =>
                                ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true).Count() >=
                                ZetaDia.Service.Party.NumPartyMembers);
                }

                if (Core.Player.IsInParty && ZetaDia.Service.Party.NumPartyMembers < partysize)
                {
                    Logger.Info("Waiting until we have a party of " + partysize + ".");
                    await Coroutine.Wait(TimeSpan.FromMinutes(60),
                            () => ZetaDia.Service.Party.NumPartyMembers >= partysize || !ZetaDia.IsInGame);
                }

                if (ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true).Count(u => u.Distance >= 5f) <=
                    ZetaDia.Service.Party.NumPartyMembers)
                {
                    Logger.Info("Party member(s) father than 5 yards away. Waiting " + waittime +
                                " seconds before opening rift. If party stacks, Starting rift.");
                    await Coroutine.Wait(TimeSpan.FromSeconds(waittime),
                            () =>
                                ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true).Count(u => u.Distance <= 5) >=
                                ZetaDia.Service.Party.NumPartyMembers);
                }
            }

            var maximizeXp = _RiftType == RiftType.Greater && _options.NormalRiftForXPShrine && (ZetaDia.Me.RestExperience < 5000000000 && ZetaDia.Me.RestExperience > -1);
            if (maximizeXp)
            {
                Util.Logger.Info("Opening Normal Rift for XP Shrine", _RiftType);
                ZetaDia.Me.OpenRift(-1);
            }
            else
            {
                if (settings.UseGemAutoLevel && _RiftType == RiftType.Greater)
                {
                    int maxLevel = _level;
                    int minLevel = Math.Min(maxLevel, Math.Max(13, maxLevel - settings.GemAutoLevelReductionLimit));

                    var gems = PluginSettings.Current.Gems;
                    if (gems.Gems.Count < 21)
                    {
                        Util.Logger.Info("We're getting a new gem on that run, running it at minimum level (GR " + minLevel + ")!");
                        _level = minLevel;
                    }
                    else
                    {
                        for (_level = minLevel; _level < maxLevel; _level++)
                        {
                            //Logger.Debug($"Starting Auto-Gem test for level: {_level}");        
                            canEmpower = (RiftData.EmpoweredRiftCost.TryGetValue(_level, out empoweredCost) && ZetaDia.PlayerData.Coinage >= empoweredCost);
                            var upgradeAttempts = (canEmpower && (shouldEmpower || _level <= settings.EmpoweredRiftLevelLimit) ? 4 : 3);
                            var possibleUpgrades = gems.Gems.Sum(g => g.GetUpgrades(_level, upgradeAttempts, 100));
                            if (possibleUpgrades >= upgradeAttempts)
                            {
                                Util.Logger.Info($"Setting GR level to {_level}, RequiredChance={PluginSettings.Current.GreaterRiftGemUpgradeChance} Upgrades={possibleUpgrades} / {upgradeAttempts}");
                                break;
                            }
                        }

                        // if upgrade chance at max level is 60%, check if we can still downgrade a few levels for the same upgrade chance
                        if (_level == maxLevel && gems.Gems.Max(g => g.GetUpgradeChance(_level)) == 60)
                        {
                            Util.Logger.Info("Update chance at max level is 60%, checking if we can take a few levels off still!");
                            for (; _level > minLevel; _level--)
                            {
                                var couldEmpower = (RiftData.EmpoweredRiftCost.TryGetValue(_level - 1, out empoweredCost) && ZetaDia.PlayerData.Coinage >= (empoweredCost + minimumCoinageKept));
                                var upgradeAttempts = (couldEmpower && (shouldEmpower || _level - 1 <= settings.EmpoweredRiftLevelLimit) ? 4 : 3);
                                var possibleUpgrades = gems.Gems.Sum(g => g.GetUpgrades(_level - 1, upgradeAttempts, 60));


                                if (possibleUpgrades < upgradeAttempts)
                                    break;
                                else
                                    canEmpower = couldEmpower;
                            }
                        }
                    }

                    if (_level <= settings.EmpoweredRiftLevelLimit)
                        shouldEmpower = true;
                }


                if (_RiftType == RiftType.Greater && shouldEmpower && canEmpower && global::Trinity.Components.Adventurer.Settings.PluginSettings.Current.UseEmpoweredRifts)
                {
                    Util.Logger.Info("Opening Empowered Greater Rift (Cost={0})", empoweredCost);
                    ZetaDia.Me.OpenRift(Math.Min(_level, ZetaDia.Me.CommonData.HighestUnlockedRiftLevel), true);
                }
                else
                {
                    Util.Logger.Info("Opening {0} Rift", _RiftType);
                    ZetaDia.Me.OpenRift(Math.Min(_level, ZetaDia.Me.CommonData.HighestUnlockedRiftLevel));
                }
            }

            if (_level == -1 || maximizeXp)
            {
                await Coroutine.Sleep(5000);
                if (IsRiftPortalOpen)
                {
                    _prePortalWorldDynamicId = AdvDia.CurrentWorldDynamicId;
                    State = States.EnteringRift;
                }
            }
            else
            {
                await Coroutine.Wait(30000, () => ZetaDia.CurrentRift.IsStarted && RiftData.RiftWorldIds.Contains(AdvDia.CurrentWorldId) && !ZetaDia.IsLoadingWorld);
                DisablePulse();
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
            EnablePulse();

            var portal = ZetaDia.Actors.GetActorsOfType<DiaGizmo>(true).FirstOrDefault(g => g.ActorSnoId == RiftData.RiftEntryPortalSNO || g.ActorSnoId == RiftData.GreaterRiftEntryPortalSNO);
            if (portal != null)
            {
                if (!await UsePortalCoroutine.UsePortal(portal.ActorSnoId, _prePortalWorldDynamicId))
                    return false;
            }
            else
            {
                var inTown = ZetaDia.IsInTown;
                Util.Logger.Debug("Expecting to find a portal here but didnt find one. InTown={0}", inTown);
                if(inTown)
                    State = States.InTown;
                else
                    State = States.Failed;
            }

            State = States.OnNewRiftLevel;
            return false;
        }

        private DateTime? _lastEnteringGreaterRiftTime;

        private bool EnteringGreaterRift()
        {
            if (ZetaDia.IsLoadingWorld || AdvDia.CurrentWorldId == ExplorationData.ActHubWorldIds[Act.A1])
            {
                if (!_lastEnteringGreaterRiftTime.HasValue)
                {
                    _lastEnteringGreaterRiftTime = DateTime.UtcNow;
                }
                else if (DateTime.UtcNow.Subtract(_lastEnteringGreaterRiftTime.Value).TotalSeconds > 10)
                {
                    Util.Logger.Debug("Stuck detected entering portal, maybe interaction coroutine has failed to do its job");
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
            EnablePulse();
            RiftData.AddEntryPortal();

            if (ZetaDia.IsInTown)
            {
                State = States.InTown;
                return false;
            }

            if (AdvDia.CurrentLevelAreaId == 276150 && !_holyCowEventCompleted)
            {
                _possiblyCowLevel = true;
            }
            if (_RiftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore && AdvDia.RiftQuest.Step == RiftStep.Cleared)
            {
                State = States.SearchingForTownstoneOrExitPortal;
            }
            else
            {
                State = States.SearchingForExitPortal;
            }
            if (Randomizer.GetRandomNumber(1, 10) > 5)
            {
                Logger.Info("[Rift] Let the massacre continue!");
            }
            else
            {
                Logger.Info("[Rift] Crom, Count the Dead!");
            }
            return false;
        }

        private string lastError = null;
        private async Task<bool> SearchingForExitPortal()
        {
            if (_RiftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore && AdvDia.RiftQuest.Step == RiftStep.Cleared)
            {
                State = States.SearchingForTownstoneOrExitPortal;
                return false;
            }
            EnablePulse();
            if (_nextLevelPortalLocation != Vector3.Zero && (_nextLevelPortalZRequirement <= 0 || AdvDia.MyZDiff(_nextLevelPortalLocation) < _nextLevelPortalZRequirement))
            {
                State = States.MovingToExitPortal;
                return false;
            }

            if (Core.Player.IsInParty && RiftData.GetGreaterRiftLevel() > 55 && TrinityPluginSettings.Settings.Advanced.BetaPlayground)
            {
                var deadPlayer =
                    ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true)
                        .FirstOrDefault(u => u.IsValid && u.CommonData != null && u.CommonData.IsValid && !u.IsAlive);

                if (deadPlayer != null && deadPlayer.Distance > 15)
                {
                    if (!await NavigationCoroutine.MoveTo(deadPlayer.Position, 15)) return false;
                }

                var players =
                        ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true)
                            .Where(
                                u =>
                                    u.IsValid && u.CommonData != null && u.CommonData.IsValid &&
                                    u.WorldId == ZetaDia.Me.WorldId && u.HitpointsMaxTotal < 999999999);

                var diaPlayers = players as DiaPlayer[] ?? players.ToArray();
                var maxhpplayer = diaPlayers.OrderByDescending(x => x.HitpointsMaxTotal).FirstOrDefault();

                if (!PluginManager.GetEnabledPlugins().Any(u => u.Contains("AutoFollow")) && maxhpplayer != null && maxhpplayer.HitpointsMaxTotal > ZetaDia.Me.HitpointsMaxTotal)
                {
                    if (maxhpplayer.Distance > 20 && ZetaDia.Me.IsInCombat || !ZetaDia.Me.IsInCombat)
                    {
                        var say = "[Follower] Got to far away.  Trying to follow the Tank with HPs: " +
                                  maxhpplayer.HitpointsMaxTotal + "!";
                        if (lastError == null || say != lastError)
                        {
                            Logger.Info(say);
                            lastError = say;
                            Logger.Info(lastError);
                        }
                        if (!await NavigationCoroutine.MoveTo(maxhpplayer.Position, 5)) return false;
                    }
                }
            }

            if (!await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId })) return false;
            ScenesStorage.Reset();
            return false;
        }


        private async Task<bool> SearchingForTownstoneOrExitPortal()
        {
            EnablePulse();
            if (_nextLevelPortalLocation != Vector3.Zero)
            {
                State = States.MovingToExitPortal;
                return false;
            }

            if (Core.Player.IsInParty && RiftData.GetGreaterRiftLevel() > 55 && TrinityPluginSettings.Settings.Advanced.BetaPlayground)
            {
                var deadPlayer =
                    ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true)
                        .FirstOrDefault(u => u.IsValid && u.CommonData != null && u.CommonData.IsValid && !u.IsAlive);

                if (deadPlayer != null && deadPlayer.Distance > 15)
                {
                    if (!await NavigationCoroutine.MoveTo(deadPlayer.Position, 15)) return false;
                }

                var players =
                        ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true)
                            .Where(
                                u =>
                                    u.IsValid && u.CommonData != null && u.CommonData.IsValid &&
                                    u.WorldId == ZetaDia.Me.WorldId && u.HitpointsMaxTotal < 999999999);

                var diaPlayers = players as DiaPlayer[] ?? players.ToArray();
                var maxhpplayer = diaPlayers.OrderByDescending(x => x.HitpointsMaxTotal).FirstOrDefault();

                if (!PluginManager.GetEnabledPlugins().Any(u => u.Contains("AutoFollow")) && maxhpplayer != null && maxhpplayer.HitpointsMaxTotal > ZetaDia.Me.HitpointsMaxTotal)
                {
                    if (maxhpplayer.Distance > 20 && ZetaDia.Me.IsInCombat || !ZetaDia.Me.IsInCombat)
                    {
                        var say = "[Follower] Got to far away.  Trying to follow the Tank with HPs: " +
                                  maxhpplayer.HitpointsMaxTotal + "!";
                        if (lastError == null || say != lastError)
                        {
                            Logger.Info(say);
                            lastError = say;
                            Logger.Info(lastError);
                        }
                        if (!await NavigationCoroutine.MoveTo(maxhpplayer.Position, 5)) return false;
                    }
                }
            }

            if (!await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId })) return false;
            ScenesStorage.Reset();
            return false;
        }


        private bool TownstoneFound()
        {
            Logger.Info("[Rift] That's it folks, returning to town.");
            State = States.ReturningToTown;
            return false;
        }


        private async Task<bool> MovingToExitPortal()
        {
            EnablePulse();
            if (!await NavigationCoroutine.MoveTo(_nextLevelPortalLocation, 15))
            {
                return false;
            }

            _nextLevelPortalLocation = Vector3.Zero;
            if (NavigationCoroutine.LastResult == CoroutineResult.Failure)
            {
                var canPathTo = await AdvDia.DefaultNavigationProvider.CanFullyClientPathTo(_nextLevelPortalLocation);
                if (!canPathTo)
                {
                    // Handle the situation where the bot is standing directly above the exit portal
                    // but the scenes required to path to the portal have not yet been discovered/loaded.
                    var zDiff = AdvDia.MyZDiff(_nextLevelPortalLocation);
                    if (zDiff > 5f)
                    {
                        _nextLevelPortalZRequirement = _nextLevelPortalZRequirement == 0 ? 15 : Math.Max(5, _nextLevelPortalZRequirement - 5);
                    }
                    Logger.Debug($"Cannot fully client path to destination, ZDiffReq={_nextLevelPortalZRequirement} CurrentZDiff={zDiff}");
                }
                _portalScanRange = ActorFinder.LowerSearchRadius(_portalScanRange);
                if (_portalScanRange <= 100)
                {
                    _portalScanRange = 100;
                }
                if (_RiftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore &&
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
                portal = BountyHelpers.GetPortalNearMarkerPosition(_nextLevelPortalLocation);
                if (portal == null)
                {
                    if (_RiftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore &&
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
            EnablePulse();
            if (await UsePortalCoroutine.UsePortal(_nextLevelPortalSNO, _prePortalWorldDynamicId))
            {
                State = States.OnNewRiftLevel;
                return false;
            }
            return false;
        }


        private bool BossSpawned()
        {
            EnablePulse();
            if (AdvDia.RiftQuest.Step != RiftStep.Cleared)
            {
                State = States.SearchingForBoss;
            }
            else
            {
                if (_RiftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore)
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
            EnablePulse();
            if (_bossLocation != Vector3.Zero)
            {
                State = States.MovingToBoss;
                return false;
            }
            if (!await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId })) return false;
            Logger.Info("[Rift] The Boss must be scared, but we will find him!");
            ScenesStorage.Reset();
            return false;
        }

        private async Task<bool> MovingToBoss()
        {
            if (AdvDia.RiftQuest.Step == RiftStep.Cleared)
            {
                if (_RiftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore)
                {
                    State = States.SearchingForTownstoneOrExitPortal;
                }
                else
                {
                    State = States.Completed;
                }
            }
            EnablePulse();
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
                Logger.Info("[Rift] You will suffer and die, ugly creature!");
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
            EnablePulse();
            State = States.SearchingForUrshi;
            return false;
        }

        private async Task<bool> SearchingForUrshi()
        {
            EnablePulse();
            if (_urshiLocation != Vector3.Zero)
            {
                State = States.MovingToUrshi;
                return false;
            }
            if (!await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId })) return false;
            Logger.Info("[Rift] Where are you, my dear Urshi!");
            ScenesStorage.Reset();
            return false;
        }

        private async Task<bool> MovingToUrshi()
        {
            EnablePulse();
            if (!await NavigationCoroutine.MoveTo(_urshiLocation, 5)) return false;
            _urshiLocation = Vector3.Zero;
            State = States.InteractingWithUrshi;
            return false;
        }

        private async Task<bool> InteractingWithUrshi()
        {
            DisablePulse();
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
            _enableGemUpgradeLogs = false;
            State = States.UpgradingGems;
            return false;
        }

        private int _gemUpgradesLeft;
        private bool _enableGemUpgradeLogs;
        private async Task<bool> UpgradingGems()
        {

            if (RiftData.VendorDialog.IsVisible && RiftData.ContinueButton.IsVisible && RiftData.ContinueButton.IsEnabled)
            {
                Logger.Debug("[Rift] Clicking to Continue button.");
                RiftData.ContinueButton.Click();
                RiftData.VendorCloseButton.Click();
                await Coroutine.Sleep(250);
                return false;
            }

            var gemToUpgrade = GetUpgradeTarget(_enableGemUpgradeLogs);
            if (gemToUpgrade == null)
            {
                Logger.Info("[Rift] I couldn't find any gems to upgrade, failing.");
                State = States.Failed;
                return false;

            }
            _enableGemUpgradeLogs = false;
            if (AdvDia.RiftQuest.Step == RiftStep.Cleared)
            {
                Logger.Debug("[Rift] Rift Quest is completed, returning to town");
                State = States.Completed;
                return false;
            }

            Logger.Debug("[Rift] Gem upgrades left before the attempt: {0}", ZetaDia.Me.JewelUpgradesLeft);
            if (!await CommonCoroutines.AttemptUpgradeGem(gemToUpgrade))
            {
                Logger.Debug("[Rift] Gem upgrades left after the attempt: {0}", ZetaDia.Me.JewelUpgradesLeft);
                return false;
            }
            var gemUpgradesLeft = ZetaDia.Me.JewelUpgradesLeft;
            if (_gemUpgradesLeft != gemUpgradesLeft)
            {
                _gemUpgradesLeft = gemUpgradesLeft;
                _enableGemUpgradeLogs = true;
            }
            if (AdvDia.RiftQuest.State == QuestState.Completed && AdvDia.RiftQuest.Step != RiftStep.UrshiSpawned)//gemUpgradesLeft == 0)
            {
                Logger.Debug("[Rift] Finished all upgrades, returning to town.");
                State = States.Completed;
                return false;
            }


            return false;
        }

        private ACDItem GetUpgradeTarget(bool enableLog)
        {
            ZetaDia.Actors.Update();
            var gems = PluginSettings.Current.Gems;
            gems.UpdateGems(ZetaDia.Me.InTieredLootRunLevel + 1, PluginSettings.Current.GreaterRiftPrioritizeEquipedGems);

            var minChance = PluginSettings.Current.GreaterRiftGemUpgradeChance;
            var upgradeableGems =
                gems.Gems.Where(g => g.UpgradeChance >= minChance && !g.IsMaxRank).ToList();
            if (upgradeableGems.Count == 0)
            {
                if (enableLog) Logger.Info("[Rift] Couldn't find any gems which is over the minimum upgrade change, upgrading the gem with highest upgrade chance");
                upgradeableGems = gems.Gems.Where(g => !g.IsMaxRank).OrderByDescending(g => g.UpgradeChance).ToList();
            }
            if (upgradeableGems.Count == 0)
            {
                if (enableLog) Logger.Info("[Rift] Looks like you have no legendary gems, failing.");
                State = States.Failed;
                return null;
            }
            var gemToUpgrade = upgradeableGems.First();
            if (enableLog) Logger.Info("[Rift] Attempting to upgrade {0}", gemToUpgrade.DisplayName);
            var acdGem =
                ZetaDia.Actors.GetActorsOfType<ACDItem>()
                    .FirstOrDefault(
                        i =>
                            i.ItemType == ItemType.LegendaryGem && i.ActorSnoId == gemToUpgrade.SNO &&
                            i.JewelRank == gemToUpgrade.Rank);
            return acdGem;
        }

        private async Task<bool> SearchingForHolyCow()
        {
            EnablePulse();
            if (_holyCowLocation != Vector3.Zero)
            {
                Logger.Info("[Rift] Mooooo!");
                State = States.MovingToHolyCow;
                return false;
            }
            if (!await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId })) return false;
            Logger.Info("[Rift] I am no butcher, where is this cow?");
            ScenesStorage.Reset();
            return false;
        }

        private async Task<bool> MovingToHolyCow()
        {
            EnablePulse();
            if (!await NavigationCoroutine.MoveTo(_holyCowLocation, 5)) return false;
            _holyCowLocation = Vector3.Zero;
            Logger.Info("[Rift] Mooooo?");
            State = States.InteractingWithHolyCow;
            return false;
        }

        private async Task<bool> InteractingWithHolyCow()
        {
            EnablePulse();
            if (!await _talkToHolyCowCoroutine.GetCoroutine()) return false;
            _talkToHolyCowCoroutine.Reset();
            Logger.Info("[Rift] Mooo moooo....");
            State = States.SearchingForExitPortal;
            return false;
        }


        private bool Completed()
        {
            State = States.ReturningToTown;
            return false;
        }

        private bool Failed()
        {
            Logger.Error("[Rift] Failed to complete the rift.");
            return true;
        }

        private bool Finished()
        {
            return true;
        }

        #region Pulse Checks & Scans

        private static readonly HashSet<States> BossSpawnedStates = new HashSet<States> { States.BossSpawned, States.SearchingForBoss, States.KillingBoss, States.MovingToBoss };
        private static readonly HashSet<States> UrshiSpawnedStates = new HashSet<States> { States.UrshiSpawned, States.SearchingForUrshi, States.InteractingWithUrshi, States.MovingToUrshi };
        private static readonly HashSet<States> ClearedStates = new HashSet<States> { States.InTown, States.GoingToAct1Hub, States.ReturningToTown, States.MoveToOrek, States.TalkToOrek, States.TownstoneFound };
        private static readonly HashSet<States> EnteringRiftStates = new HashSet<States> { States.MoveToRiftStone, States.EnteringRift, States.OpeningRift };

        private void Scans()
        {
            switch (State)
            {
                case States.SearchingForExitPortal:
                    if (_possiblyCowLevel)
                    {
                        ScanForHolyCow();
                    }
                    ScanForExitPortal();
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
                    ScanForExitPortal();
                    ScanForTownstone();
                    return;

            }
        }


        private void PulseChecks()
        {
            if (BrainBehavior.IsVendoring || !ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || ZetaDia.IsPlayingCutscene)
            {
                DisablePulse();
                return;
            }

            //AdvDia.Update(true);

            if (AdvDia.CurrentWorldId == ExplorationData.ActHubWorldIds[Act.A1] && (AdvDia.RiftQuest.Step == RiftStep.KillingMobs || AdvDia.RiftQuest.Step == RiftStep.BossSpawned || AdvDia.RiftQuest.Step == RiftStep.UrshiSpawned))
            {
                if (!EnteringRiftStates.Contains(State))
                {
                    Logger.Info(
                        "[Rift] Oh darn, I managed to return to town, I better go back in the rift before anyone notices.");
                    State = States.MoveToRiftStone;
                    return;
                }
            }

            if (AdvDia.CurrentWorldId == ExplorationData.ActHubWorldIds[Act.A1] && State != States.EnteringRift)
            {
                DisablePulse();
                return;
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
                    if (!BossSpawnedStates.Contains(State))
                    {
                        Logger.Info("[Rift] Behold the Rift Boss!");
                        State = States.BossSpawned;
                    }
                    break;
                case RiftStep.UrshiSpawned:
                    if (!UrshiSpawnedStates.Contains(State))
                    {
                        _riftEndTime = DateTime.UtcNow;
                        var totalTime = _riftEndTime - _riftStartTime;
                        if (totalTime.TotalSeconds < 3600 && totalTime.TotalSeconds > 0)
                        {
                            Logger.Info("[Rift] All done. (Total Time: {0} mins {1} seconds)", totalTime.Minutes, totalTime.Seconds);
                            Logger.Info("[Rift] Level: {0}", ZetaDia.Me.InTieredLootRunLevel + 1);
                        }
                        else
                        {
                            Logger.Info("[Rift] All done. (Partial rift, no stats available)");
                        }
                        Logger.Info("[Rift] My dear Urshi, I have some gems for you.");
                        State = States.UrshiSpawned;
                    }
                    break;
                case RiftStep.Cleared:
                    if (_RiftType == RiftType.Nephalem && PluginSettings.Current.NephalemRiftFullExplore && State != States.TownstoneFound)
                    {
                        break;
                    }
                    if (!ClearedStates.Contains(State))
                    {
                        _riftEndTime = DateTime.UtcNow;
                        var totalTime = _riftEndTime - _riftStartTime;
                        if (totalTime.TotalSeconds < 3600 && totalTime.TotalSeconds > 0)
                        {
                            Logger.Info("[Rift] All done. (Total Time: {0} mins {1} seconds)", totalTime.Minutes, totalTime.Seconds);
                        }
                        else
                        {
                            Logger.Info("[Rift] All done. (Partial rift, no stats available)");
                        }
                        State = States.ReturningToTown;
                    }
                    break;
            }

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
                Logger.Info("[Rift] Urshi is near.");
                State = States.MovingToUrshi;
                Logger.Debug("[Rift] Found Urshi at distance {0}", AdvDia.MyPosition.Distance(_urshiLocation));
            }
        }

        private void ScanForBoss()
        {
            var portalMarker = AdvDia.CurrentWorldMarkers.FirstOrDefault(m => m.Id >= 0 && m.Id <= 200);
            if (portalMarker != null)
            {
                _bossLocation = portalMarker.Position;
            }
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
                Logger.Info("[Rift] The Boss is near.");
                State = States.MovingToBoss;
                Logger.Debug("[Rift] Found the boss at distance {0}", AdvDia.MyPosition.Distance(_bossLocation));
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
            if (_nextLevelPortalLocation != Vector3.Zero)
            {
                Logger.Info("[Rift] Oh look! There is a portal over there, let's see what's on the other side.");
                Logger.Debug("[Rift] Found the objective at distance {0}",
                    AdvDia.MyPosition.Distance(_nextLevelPortalLocation));
            }
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
        #endregion



        #region OnPulse Implementation
        private readonly WaitTimer _pulseTimer = new WaitTimer(TimeSpan.FromMilliseconds(250));
        private bool _isPulsing;
        private States _startingState;
        private RiftOptions _options;
        private int _riftCounter;
        private int _nextLevelPortalZRequirement;

        private void EnablePulse()
        {
            if (!_isPulsing)
            {
                Logger.Debug("[Rift] Registered to pulsator.");
                Pulsator.OnPulse += OnPulse;
                _isPulsing = true;
            }
        }

        private void DisablePulse()
        {
            if (_isPulsing)
            {
                Logger.Debug("[Rift] Unregistered from pulsator.");
                Pulsator.OnPulse -= OnPulse;
                _isPulsing = false;
            }
        }

        private void OnPulse(object sender, EventArgs e)
        {
            if (_pulseTimer.IsFinished)
            {
                _pulseTimer.Stop();
                Scans();
                _pulseTimer.Reset();
            }
        }

        #endregion

        public void Dispose()
        {
            DisablePulse();
        }

    }
}
