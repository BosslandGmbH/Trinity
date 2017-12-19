using Buddy.Coroutines;
using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Game.Rift;
using Trinity.Components.Adventurer.Game.Stats;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.UI;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Bot.Coroutines;
using Zeta.Bot.Logic;
using Zeta.Common;
using Zeta.Common.Helpers;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;
using GizmoType = Zeta.Game.Internals.SNO.GizmoType;


namespace Trinity.Components.Adventurer.Coroutines.RiftCoroutines
{
    public class RiftCoroutine : IDisposable, ICoroutine
    {
        public class RiftOptions
        {
            public int RiftCount = 0;
            public bool IsEmpowered = false;
            public States EndState = States.Finished;
            public bool NormalRiftForXPShrine;
        }

        // 经验池是否已满
        private bool IsRestExperienceFull = false;

        private RiftType _RiftType;
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
        private ExperienceTracker _experienceTracker = new ExperienceTracker();

        private readonly MoveToPositionCoroutine _moveToRiftStoneCoroutine = new MoveToPositionCoroutine(ExplorationData.ActHubWorldIds[Act.A1], RiftData.Act1RiftStonePosition, 1);
        private readonly InteractionCoroutine _interactWithRiftStoneInteractionCoroutine = new InteractionCoroutine(RiftData.RiftStoneSNO, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), 1);
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
                    Core.Logger.Debug("[秘境] " + value);
                    StatusText = "[秘境] " + value;
                }
                _state = value;
            }
        }

        #endregion State

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
            _currentExitScene = null;
            _entranceSceneNames.Clear();
            State = States.NotStarted;
        }

        public string StatusText { get; set; }

        public virtual async Task<bool> GetCoroutine()
        {
            if (_isPulsing)
            {
                PulseChecks();
            }

            if (State == _options.EndState)
            {
                Core.Logger.Debug("[秘境] 接到停止秘境通知，所以去执行能通知的事情.");
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

        private long CurrentRiftKeyCount
        {
            get
            {
                ZetaDia.Actors.Update();
                Core.Update();
                var keys = AdvDia.StashAndBackpackItems.Where(i => i.RawItemType == RawItemType.TieredRiftKey).Sum(k => k.ItemStackQuantity);
                Core.Logger.Log("我有 {0} 秘境钥匙.", keys);
                return keys;
            }
        }

        private bool NotStarted()
        {
			ExpStatistics.UpdateStartRiftInfo();
            if (!_experienceTracker.IsStarted) _experienceTracker.Start();
            SafeZerg.Instance.DisableZerg();

            if (_RiftType == RiftType.Greater)
            {
                _level = RiftData.GetGreaterRiftLevel();
            }
            if (_runningNephalemInsteadOfGreaterRift && CurrentRiftKeyCount > PluginSettings.Current.MinimumKeys)
            {
                _level = RiftData.GetGreaterRiftLevel();
                _RiftType = RiftType.Greater;
                _runningNephalemInsteadOfGreaterRift = false;
                return false;
            }
            if (AdvDia.RiftQuest.State == QuestState.NotStarted && _RiftType == RiftType.Greater && CurrentRiftKeyCount <= PluginSettings.Current.MinimumKeys)
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
                    Core.Logger.Error("你没有秘境钥匙,停止辅助.");
                    BotMain.Stop();
                    return true;
                }
            }
            _currentWorldDynamicId = AdvDia.CurrentWorldDynamicId;
            if (AdvDia.RiftQuest.State == QuestState.InProgress && RiftData.RiftWorldIds.Contains(AdvDia.CurrentWorldId))
            {
                //State = States.SearchingForExitPortal;
                State = States.OnNewRiftLevel;
                return false;
            }
            State = AdvDia.CurrentWorldId == ExplorationData.ActHubWorldIds[Act.A1] ? States.InTown : States.GoingToAct1Hub;
            if (AdvDia.RiftQuest.State == QuestState.NotStarted)
            {
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
                    Core.Logger.Log("[秘境] 我想我应该给欧雷克 吹吹我的成功事迹.");
                    State = States.MoveToOrek;
                    return false;

                case RiftStep.BossSpawned:
                    Core.Logger.Log("[秘境] 我不知道为什么我在城里而秘境中产生了boss.");
                    State = States.MoveToRiftStone;
                    return false;

                case RiftStep.UrshiSpawned:
                    Core.Logger.Log("[秘境] 我不知道为什么我在城镇里, 而秘境中乌尔什出现了.");
                    State = States.MoveToRiftStone;
                    return false;

                case RiftStep.KillingMobs:
                    Core.Logger.Log("[秘境] 我不知道为什么我在城镇的同时有很多小怪在秘境内厮杀.");
                    State = States.MoveToRiftStone;
                    return false;

                case RiftStep.NotStarted:
                    State = States.MoveToRiftStone;
                    _moveToRiftStoneCoroutine.Reset();
                    Core.Logger.Log("[秘境] 需要花点时间来杀死这些吓人的怪物了, 砍~砍!");
                    return false;

                default:
                    Core.Logger.Log("[秘境] 我真的不知道现在该做什么.");
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
                    Core.Logger.Log("[秘境] 噢,我似乎觉得秘境效果仍然有效,这意味这我无法正常清理我的背包.");
                }
            }
            ExpStatistics.UpdateEndRiftInfo();
            _experienceTracker.StopAndReport("Rift");
            _experienceTracker.Start();

            State = States.TownRun;
            return false;
        }

        private bool TownRun()
        {
            Core.Logger.Debug("[城镇] BrainBehavior.IsVendoring is {0}", BrainBehavior.IsVendoring);
            Core.Logger.Debug("[城镇] ZetaDia.Me.IsParticipatingInTieredLootRun is {0}", ZetaDia.Me.IsParticipatingInTieredLootRun);
            Core.Logger.Debug("[城镇] AdvDia.RiftQuest.State is {0}", AdvDia.RiftQuest.State);
            Core.Logger.Debug("[城镇] AdvDia.RiftQuest.Step is {0}", AdvDia.RiftQuest.Step);
            DisablePulse();
            if (BrainBehavior.IsVendoring)
            {
                return false;
            }
            if (!_townRunInitiated)
            {
                _townRunInitiated = true;
                if (!Core.Settings.SenExtend.EnableIntelligentFinishing)
                {
                    BrainBehavior.ForceTownrun(" We need it.", true);
                    return false;
                }
                else if (Core.Settings.SenExtend.EnableIntelligentFinishing && ZetaDia.Storage?.CurrentRiftType == RiftType.Nephalem)
                {
                    BrainBehavior.ForceTownrun(" We need it.", true);
                    return false;
                }
            }

            _riftCounter++;
            Core.Logger.Log("秘境完成 = {0}", _riftCounter);

            if (_options.RiftCount > 0 && _riftCounter >= _options.RiftCount)
            {
                Core.Logger.Log("[秘境] 已达到配置文件上设定的密境次数. ({0})", _options.RiftCount);
                State = States.Completed;
                return Finished();
            }

            _townRunInitiated = false;
            if (AdvDia.RiftQuest.Step == RiftStep.Completed)
            {
                State = States.WaitForRiftCountdown;
                Core.Logger.Log("[秘境] 滴答,滴答...");
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

            DisablePulse();
            if (!await _interactWithRiftStoneInteractionCoroutine.GetCoroutine()) return false;
            await Coroutine.Wait(2500, () => UIElements.RiftDialog.IsVisible);
            _interactWithRiftStoneInteractionCoroutine.Reset();
            if (!UIElements.RiftDialog.IsVisible)
            {
                return false;
            }

            _entranceSceneNames.Clear();
            long empoweredCost = 0;
            bool shouldEmpower = _options.IsEmpowered;
            bool haveMoneyForEmpower = RiftData.EmpoweredRiftCost.TryGetValue(_level, out empoweredCost) && ZetaDia.Storage.PlayerDataManager.ActivePlayerData.Coinage >= (empoweredCost + PluginSettings.Current.MinimumGold);
            bool canEmpower = (_RiftType == RiftType.Greater && haveMoneyForEmpower);
            var settings = PluginSettings.Current;

            _riftStartTime = DateTime.UtcNow;
            const int waittime = 45;
            const int partysize = 3; // ToDo: Add slider for party size under beta playground checkbox

            //if (TrinityPluginSettings.Settings.Advanced.BetaPlayground)
            //{
            //    if (Core.Player.IsInParty &&
            //        ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true).Count() < ZetaDia.Service.Party.NumPartyMembers)
            //    {
            //        Core.Logger.Log("等待所有人都聚齐.");
            //        await Coroutine.Wait(TimeSpan.FromMinutes(60),
            //                () =>
            //                    ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true).Count() >=
            //                    ZetaDia.Service.Party.NumPartyMembers);
            //    }

            //    if (Core.Player.IsInParty && ZetaDia.Service.Party.NumPartyMembers < partysize)
            //    {
            //        Core.Logger.Log("Waiting until we have a party of " + partysize + ".");
            //        await Coroutine.Wait(TimeSpan.FromMinutes(60),
            //                () => ZetaDia.Service.Party.NumPartyMembers >= partysize || !ZetaDia.IsInGame);
            //    }

            //    if (ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true).Count(u => u.Distance >= 5f) <=
            //        ZetaDia.Service.Party.NumPartyMembers)
            //    {
            //        Core.Logger.Log("Party member(s) father than 5 yards away. Waiting " + waittime +
            //                    " 开始秘境前几秒. 如果队员聚齐, 开始秘境.");
            //        await Coroutine.Wait(TimeSpan.FromSeconds(waittime),
            //                () =>
            //                    ZetaDia.Actors.GetActorsOfType<DiaPlayer>(true).Count(u => u.Distance <= 5) >=
            //                    ZetaDia.Service.Party.NumPartyMembers);
            //    }
            //}

            bool _IsFullRestExperience = (bool)Core.Settings.SenExtend.IsFullRestExperience;
            

            if (_IsFullRestExperience && _options.NormalRiftForXPShrine)
            {
                long TotalExperience = ZetaDia.Me.ParagonCurrentExperience + ZetaDia.Me.ParagonExperienceNextLevel;
                // 池子数量
                int _restExperienceNum = ZetaDia.Me.RestExperience < 1 ?  0 : (int)Math.Ceiling((float)ZetaDia.Me.RestExperience / (float)TotalExperience * 10);
                string strRestExperienceNum = string.Format("当前粪池经验 : {0:0,0}", ZetaDia.Me.RestExperience);
                // 如果没有经验池
                if (ZetaDia.Me.RestExperience == 0)
                {
                    Core.Logger.Warn($"擦~擦~擦~ 没有粪池经验了, {strRestExperienceNum}");
                    IsRestExperienceFull = false;
                    
                } else {                   
                    // 经验池未满
                    if (!IsRestExperienceFull) {
                        // 经验池已满
                        if (_restExperienceNum >= (int)Core.Settings.SenExtend.RestExperienceNum)
                        {
                            Core.Logger.Warn($"嘢~嘢~粪池已满,{strRestExperienceNum}");
                            IsRestExperienceFull = true;
                        }
                        else
                        {
                            // 经验池未来满
                            Core.Logger.Warn($"粪池没吃饱, {strRestExperienceNum}, 已吃了 {_restExperienceNum} 个粪池, 必须吃满 {(int)Core.Settings.SenExtend.RestExperienceNum} 个粪池!");
                            IsRestExperienceFull = false; 
                        }
                    }
                }
            }


            //var maximizeXp = _RiftType == RiftType.Greater && _options.NormalRiftForXPShrine && (ZetaDia.Me.RestExperience < 5000000000 && ZetaDia.Me.RestExperience > -1);
            long _miniNormalRiftForXPShrine = Core.Settings.SenExtend.EnableNephalemRestExperienceCheck ? (long)Core.Settings.SenExtend.MiniNormalRiftForXPShrine * 100000000 : 5000000000;

            var maximizeXp = _RiftType == RiftType.Greater && _options.NormalRiftForXPShrine && (ZetaDia.Me.RestExperience < _miniNormalRiftForXPShrine && ZetaDia.Me.RestExperience > -1);

            if (_options.NormalRiftForXPShrine)
            {
                if (_IsFullRestExperience)
                {
                    // 经验池没满,去小米
                    maximizeXp = _RiftType == RiftType.Greater && _options.NormalRiftForXPShrine && !IsRestExperienceFull;
                }
            }

            if (maximizeXp)
            {
                Core.Logger.Log("开启小秘境寻找经验池", _RiftType);
                ZetaDia.Me.OpenRift(-1);
            }
            else
            {
                if (settings.UseGemAutoLevel && _RiftType == RiftType.Greater)
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
                        Core.Logger.Error("Adventurer设置中的宝石收藏未正确填充.");
                        State = States.Failed;
                        return false;
                    }
                    if (gems.Gems != null && gems.Gems.Count < 21)
                    {
                        Core.Logger.Log("我们正在获得一个新的宝石, 在最低级别运行它 (GR " + minLevel + ")!");
                        _level = minLevel;
                    }
                    else
                    {
                        for (_level = minLevel; _level < maxLevel; _level++)
                        {
                            //Core.Logger.Debug($"Starting Auto-Gem test for level: {_level}");
                            canEmpower = (RiftData.EmpoweredRiftCost.TryGetValue(_level, out empoweredCost) && ZetaDia.Storage.PlayerDataManager.ActivePlayerData.Coinage >= empoweredCost);
                            var upgradeAttempts = (canEmpower && (shouldEmpower || _level <= settings.EmpoweredRiftLevelLimit) ? 4 : 3);
                            var possibleUpgrades = gems.Gems.Sum(g => g.GetUpgrades(_level, upgradeAttempts, 100));
                            if (possibleUpgrades >= upgradeAttempts)
                            {
                                Core.Logger.Log($"设置大秘境等级 {_level}, 宝石升级几率={PluginSettings.Current.GreaterRiftGemUpgradeChance} 升级={possibleUpgrades} / {upgradeAttempts}");
                                break;
                            }
                        }

                        // if upgrade chance at max level is 60%, check if we can still downgrade a few levels for the same upgrade chance
                        if (_level == maxLevel && gems.Gems.Max(g => g.GetUpgradeChance(_level)) == 60)
                        {
                            Core.Logger.Log("现在最高升级几率是 60%, 如果我们可以采取一些水平仍然!");
                            for (; _level > minLevel; _level--)
                            {
                                var couldEmpower = (RiftData.EmpoweredRiftCost.TryGetValue(_level - 1, out empoweredCost) && ZetaDia.Storage.PlayerDataManager.ActivePlayerData.Coinage >= (empoweredCost + PluginSettings.Current.MinimumGold));
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

                if (_RiftType == RiftType.Greater && shouldEmpower && canEmpower && PluginSettings.Current.UseEmpoweredRifts)
                {
                    Core.Logger.Log("开启强化秘境 (花费={0})", empoweredCost);
                    ZetaDia.Me.OpenRift(Math.Min(_level, ZetaDia.Me.CommonData.HighestUnlockedRiftLevel), true);
                }
                else
                {
                    Core.Logger.Log("开启 {0} 秘境", _RiftType);
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
                await Coroutine.Wait(30000, () => ZetaDia.Storage.RiftStarted && RiftData.RiftWorldIds.Contains(AdvDia.CurrentWorldId) && !ZetaDia.Globals.IsLoadingWorld);
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
                Core.Logger.Debug("期待在这里找到一个入口，但没有找到。在城镇中=={0}", inTown);
                if (inTown)
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
            if (ZetaDia.Globals.IsLoadingWorld || AdvDia.CurrentWorldId == ExplorationData.ActHubWorldIds[Act.A1])
            {
                if (!_lastEnteringGreaterRiftTime.HasValue)
                {
                    _lastEnteringGreaterRiftTime = DateTime.UtcNow;
                }
                else if (DateTime.UtcNow.Subtract(_lastEnteringGreaterRiftTime.Value).TotalSeconds > 10)
                {
                    Core.Logger.Debug("在检测到入口，也许未能完成互动过程");
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

            _currentExitScene = null;
            _currentEntranceScene = null;

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
            if (Randomizer.Random(1, 10) > 5)
            {
                Core.Logger.Log("[秘境] 让大屠杀继续!");
            }
            else
            {
                Core.Logger.Log("[秘境] 死亡统计!");
            }
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
            EnablePulse();
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
            Core.Logger.Log("[秘境] 就这样，乡亲们，返回到小镇.");
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
                    Core.Logger.Debug($"不能完全到达目的地的路径, Z不相同={_nextLevelPortalZRequirement} CurrentZDiff={zDiff}");
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
                portal = BountyHelpers.GetPortalNearPosition(_nextLevelPortalLocation);
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
            Core.Logger.Log("[秘境] Boss一定是害怕躲起来了.但是我们会找到它的!");
            Core.Scenes.Reset();
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
                Core.Logger.Log("[秘境] 你将受苦并死去,丑陋的怪物!");
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
            Core.Logger.Log("[秘境] 我亲爱的乌尔什你在那里!");
            Core.Scenes.Reset();
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
                Core.Logger.Debug("[秘境] 单击继续按钮.");
                RiftData.ContinueButton.Click();
                RiftData.VendorCloseButton.Click();
                await Coroutine.Sleep(250);
                return false;
            }

            var gemToUpgrade = PluginSettings.Current.Gems.GetUpgradeTarget();
            if (gemToUpgrade == null)
            {
                Core.Logger.Log("[秘境] 我没有发现任务可升级的宝石.失败.");
                State = States.Failed;
                return false;
            }
            _enableGemUpgradeLogs = false;
            if (AdvDia.RiftQuest.Step == RiftStep.Cleared)
            {
                Core.Logger.Debug("[秘境] 秘境完成，返回城镇");
                State = States.Completed;
                return false;
            }

            Core.Logger.Debug("[秘境] 尝试升级宝石: {0}", ZetaDia.Me.JewelUpgradesLeft);
            if (!await CommonCoroutines.AttemptUpgradeGem(gemToUpgrade))
            {
                Core.Logger.Debug("[秘境] 尝试升级宝石: {0}", ZetaDia.Me.JewelUpgradesLeft);
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
                Core.Logger.Debug("[秘境] 升级完成, 返回城镇.");
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
        //        if (enableLog) Core.Logger.Log("[秘境] 找不到达到最小成功几率内的宝石,升级具有最高成功率的宝石");
        //        upgradeableGems = gems.Gems.Where(g => !g.IsMaxRank).OrderByDescending(g => g.UpgradeChance).ToList();
        //    }
        //    if (upgradeableGems.Count == 0)
        //    {
        //        if (enableLog) Core.Logger.Log("[秘境] 看起来你有没有传奇宝石,失败.");
        //        State = States.Failed;
        //        return null;
        //    }
        //    var gemToUpgrade = upgradeableGems.First();
        //    if (enableLog) Core.Logger.Log("[秘境] 尝试升级 {0}", gemToUpgrade.DisplayName);
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
            EnablePulse();
            if (_holyCowLocation != Vector3.Zero)
            {
                Core.Logger.Log("[秘境] 哞~~~~!");
                State = States.MovingToHolyCow;
                return false;
            }
            if (!await ExplorationCoroutine.Explore(new HashSet<int> { AdvDia.CurrentLevelAreaId })) return false;
            Core.Logger.Log("[秘境] 我不是屠夫,这是哪里的牛?");
            Core.Scenes.Reset();
            return false;
        }

        private async Task<bool> MovingToHolyCow()
        {
            EnablePulse();
            if (!await NavigationCoroutine.MoveTo(_holyCowLocation, 5)) return false;
            _holyCowLocation = Vector3.Zero;
            Core.Logger.Log("[秘境] 哞~~~~?");
            State = States.InteractingWithHolyCow;
            return false;
        }

        private async Task<bool> InteractingWithHolyCow()
        {
            EnablePulse();
            if (!await _talkToHolyCowCoroutine.GetCoroutine()) return false;
            _talkToHolyCowCoroutine.Reset();
            Core.Logger.Log("[秘境] 哞~哞~~");
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
            Core.Logger.Error("[秘境] 无法完成秘境.");
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
                    Core.Logger.Warn($"找到入口 '{entranceScene.Name}' ({entranceScene.SnoId}) 距离: {entranceScene.Center.Distance(Core.Player.Position.ToVector2())} 码!");

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
                    Core.Logger.Warn($"找到出口-1 '{markerExitScene.Name}' ({markerExitScene.SnoId}) 距离: {exitSceneConnection.Value.Distance(Core.Player.Position)} 码!");
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
                Core.Logger.Warn($"找到出口-2 '{exitScene.Name}' ({exitScene.SnoId}) 距离: {exitSceneConnection.Value.Distance(Core.Player.Position)} 码! CanRayWalk: {Core.Grids.Avoidance.CanRayWalk(Core.Player.Position, exitSceneConnection.Value, 5f)} CanRayCast: {Core.Grids.Avoidance.CanRayCast(Core.Player.Position, exitSceneConnection.Value)}");


                //if (exitSceneConnection.Value.Distance(Core.Player.Position) <= _portalScanRange)
                //{
                //    ExplorationHelpers.SetExplorationPriority(exitSceneConnection.Value);
                //    Core.Logger.Warn($"找到出口-2 '{exitScene.Name}' ({exitScene.SnoId}) 距离: {exitSceneConnection.Value.Distance(Core.Player.Position)} 码!");
                //}

            }
        }



        private void PulseChecks()
        {
            if (BrainBehavior.IsVendoring || !ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld || ZetaDia.Globals.IsPlayingCutscene)
            {
                DisablePulse();
                return;
            }

            //AdvDia.Update(true);

            if (AdvDia.CurrentWorldId == ExplorationData.ActHubWorldIds[Act.A1] && (AdvDia.RiftQuest.Step == RiftStep.KillingMobs || AdvDia.RiftQuest.Step == RiftStep.BossSpawned || AdvDia.RiftQuest.Step == RiftStep.UrshiSpawned))
            {
                if (!EnteringRiftStates.Contains(State))
                {
                    Core.Logger.Log(
                        "[秘境] Oh,该死.我要回城,我最好在别人没有注意到我之前回去.");
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
                        Core.Logger.Log("[秘境] 看!秘境Boss!");
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
                            Core.Logger.Log("[秘境] 全部完成. (总时间: {0} 分 {1} 秒)", totalTime.Minutes, totalTime.Seconds);
                            Core.Logger.Log("[秘境] 等级: {0}", ZetaDia.Me.InTieredLootRunLevel + 1);
                        }
                        else
                        {
                            Core.Logger.Log("[秘境] 全部完成.(部分秘境没有统计数据)");
                        }
                        Core.Logger.Log("[秘境] 我亲爱的乌尔什.我有很多宝石给你.");
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
                            Core.Logger.Log("[秘境] 全部完成. (总时间: {0} 分 {1} 秒)", totalTime.Minutes, totalTime.Seconds);
                        }
                        else
                        {
                            Core.Logger.Log("[秘境] 全部完成.(部分秘境没有统计数据)");
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
                Core.Logger.Log("[秘境] 乌尔什在附近.");
                State = States.MovingToUrshi;
                Core.Logger.Debug("[秘境] 找到乌尔什, 距离: {0}", AdvDia.MyPosition.Distance(_urshiLocation));
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
                Core.Logger.Log("[秘境] Boss在附近.");
                State = States.MovingToBoss;
                Core.Logger.Debug("[秘境] 找到Boss, 距离 : {0}", AdvDia.MyPosition.Distance(_bossLocation));
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
                Core.Logger.Log("[秘境] 哦,看!有一个入口在那边,让我们来看看另一边有什么.");
                Core.Logger.Debug("[秘境] 在远处找到目标, 距离 : {0}",
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
        private bool _isPulsing;
        private States _startingState;
        private RiftOptions _options;
        private int _riftCounter;
        private int _nextLevelPortalZRequirement;

        private List<string> _entranceSceneNames = new List<string>();
        private WorldScene _currentEntranceScene;

        private void EnablePulse()
        {
            if (!_isPulsing)
            {
                Core.Logger.Debug("[秘境] 注册Pulsator.");
                Pulsator.OnPulse += OnPulse;
                _isPulsing = true;
            }
        }

        private void DisablePulse()
        {
            if (_isPulsing)
            {
                Core.Logger.Debug("[秘境] 注销Pulsator.");
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

        #endregion OnPulse Implementation

        public void Dispose()
        {
            DisablePulse();
        }
    }
}