using System;
using System.Collections.Generic;
using Trinity.Framework;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Components.QuestTools;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("Bounties")]
    [XmlElement("RunActBounties")]
    public class RunActBountiesTag : RunActBountiesProfileBehavior { }

    public enum ActSelectionMode
    {
        None = 0,
        Balance,
        All,
        A1,
        A2,
        A3,
        A4,
        A5
    }

    public class RunActBountiesProfileBehavior : BaseProfileBehavior
    {
        private List<Act> _acts;
        private List<Act> _completedActs;
        private Act _currentAct;
        private ActBountiesCoroutine _currentActBountiesCoroutine;

        #region XmlAttributes

        [XmlAttribute("act")]
        [XmlAttribute("acts")]
        [Description("Which acts are completed, else user preferences used.")]
        public ActSelectionMode ActsMode { get; set; }

        #endregion

        public override async Task<bool> StartTask()
        {
            PluginEvents.CurrentProfileType = ProfileType.Bounty;

            _currentAct = Act.Invalid;
            _currentActBountiesCoroutine = null;
            _completedActs = new List<Act>();

            switch (ActsMode)
            {
                case ActSelectionMode.None:
                    var isSelectedActMode = PluginSettings.Current.BountyMode3.HasValue && PluginSettings.Current.BountyMode3.Value;
                    _acts = isSelectedActMode ? GetUserSelectedActs() : GetBalanceMaterialActs();
                    break;
                case ActSelectionMode.Balance:
                    _acts = GetBalanceMaterialActs();
                    break;
                case ActSelectionMode.All:
                    _acts = GetAllActs();
                    break;
                case ActSelectionMode.A1:
                    _acts = GetSpecificAct(Act.A1);
                    break;
                case ActSelectionMode.A2:
                    _acts = GetSpecificAct(Act.A2);
                    break;
                case ActSelectionMode.A3:
                    _acts = GetSpecificAct(Act.A3);
                    break;
                case ActSelectionMode.A4:
                    _acts = GetSpecificAct(Act.A4);
                    break;
                case ActSelectionMode.A5:
                    _acts = GetSpecificAct(Act.A5);
                    break;
                default:
                    _acts = new List<Act>();
                    break;
            }

            foreach (var act in _acts.ToList())
            {
                if (BountyHelpers.AreAllActBountiesCompleted(act))
                    _acts.Remove(act);
                else
                    _completedActs.Add(act);
            }

            return TrySelectActCoroutine();
        }

        public override async Task<bool> MainTask()
        {
            if (Adventurer.TimeSinceWorldChange < 1000)
            {
                Core.Logger.Debug("[Bounties] Sleeping 1 second due to world change");
                await Coroutine.Sleep(1000);
            }

            if (_currentActBountiesCoroutine == null)
            {
                if (_currentAct != Act.Invalid)
                {
                    if (!BountyHelpers.AreAllActBountiesCompleted(_currentAct))
                    {
                        _currentActBountiesCoroutine = new ActBountiesCoroutine(_currentAct);
                    }
                    else
                    {
                        if (!TrySelectActCoroutine())
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            if (!await _currentActBountiesCoroutine.GetCoroutine())
                return false;

            _completedActs.Add(_currentAct);

            if (_acts.Any())
            {
                _acts.Remove(_currentAct);

                if (_acts.Count == 0)
                {
                    Core.Logger.Log("All Acts Complete");
                    return true;
                }
            }

            return TrySelectActCoroutine();
        }

        private bool TrySelectActCoroutine()
        {
            _currentActBountiesCoroutine = GetNextAct();
            _currentAct = _currentActBountiesCoroutine?.Act ?? Act.Invalid;

            if (_currentActBountiesCoroutine != null)
            {
                Core.Logger.Log($"[Bounties] Picked {_currentActBountiesCoroutine.Act} as new target.");
                return false;
            }

            Core.Logger.Log("[Bounties] Unable to select an act for bounties, Finished.");
            return true;
        }

        private ActBountiesCoroutine GetNextAct()
        {
            if (_acts.Any())
            {
                return new ActBountiesCoroutine(_acts.First());
            }
            return null;
        }

        private List<Act> GetUserSelectedActs()
        {
            var acts = new List<Act>();
            if (PluginSettings.Current.BountyAct1) acts.Add(Act.A1);
            if (PluginSettings.Current.BountyAct2) acts.Add(Act.A2);
            if (PluginSettings.Current.BountyAct3) acts.Add(Act.A3);
            if (PluginSettings.Current.BountyAct4) acts.Add(Act.A4);
            if (PluginSettings.Current.BountyAct5) acts.Add(Act.A5);
            Core.Logger.Log($"User Selected Acts: {acts.Aggregate("", (s, item) => s + $"{item}, ")}");
            return acts;
        }

        private List<Act> GetAllActs()
        {
            Core.Logger.Log($"All Acts");
            return new List<Act>
            {
                Act.A1,
                Act.A2,
                Act.A3,
                Act.A4,
                Act.A5
            };
        }

        private List<Act> GetSpecificAct(Act act)
        {
            Core.Logger.Log($"Specific Act {act}");
            return new List<Act> { act };
        }

        private List<Act> GetBalanceMaterialActs()
        {
            var matCounts = new Dictionary<Act, long>
            {
                {Act.A1, BountyHelpers.GetActMatsCount(Act.A1)},
                {Act.A2, BountyHelpers.GetActMatsCount(Act.A2)},
                {Act.A3, BountyHelpers.GetActMatsCount(Act.A3)},
                {Act.A4, BountyHelpers.GetActMatsCount(Act.A4)},
                {Act.A5, BountyHelpers.GetActMatsCount(Act.A5)},
            };

            var averageMatsCount = matCounts.Values.Average(m => m);
            var minMatsCount = matCounts.Values.Min(m => m);
            var maxMatsCount = matCounts.Values.Max(m => m);

            LogMaterialStats(matCounts, averageMatsCount, minMatsCount, maxMatsCount);

            var eligibleActs = matCounts.Where(kv =>
                        !_completedActs.Contains(kv.Key) && BountyHelpers.AreAllActBountiesSupported(kv.Key) &&
                        kv.Value <= averageMatsCount + 1).ToDictionary(kv => kv.Key, kv => kv.Value);

            var orderedActs = eligibleActs.OrderByDescending(p => p.Value).Select(v => v.Key).ToList();

            Core.Logger.Log($"Balance Material Acts: {eligibleActs.Aggregate("", (s, item) => s + $"{item.Key}({item.Value}), ")}");
            return orderedActs;
        }

        private static void LogMaterialStats(Dictionary<Act, long> matCounts, double averageMatsCount, long minMatsCount, long maxMatsCount)
        {
            Core.Logger.Log("[Bounties] Current Bounty Mats Counts");
            Core.Logger.Log("[Bounties] Act 1: {0}", matCounts[Act.A1]);
            Core.Logger.Log("[Bounties] Act 2: {0}", matCounts[Act.A2]);
            Core.Logger.Log("[Bounties] Act 3: {0}", matCounts[Act.A3]);
            Core.Logger.Log("[Bounties] Act 4: {0}", matCounts[Act.A4]);
            Core.Logger.Log("[Bounties] Act 5: {0}", matCounts[Act.A5]);
            Core.Logger.Debug("[Bounties] Average Mats Count: {0}", averageMatsCount);

            const int diff = 8;
            if (averageMatsCount - minMatsCount > diff)
            {
                Core.Logger.Debug("[Bounties] Average Mats Count - Min Mats Count > {0}", diff);
            }
            else if (maxMatsCount - minMatsCount <= diff)
            {
                Core.Logger.Debug("[Bounties] Max Mats Count - Min Mats Count <= {0}", diff);
            }
        }



    }
}

//using System;
//using Trinity.Framework;
//using Trinity.Framework.Helpers;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Buddy.Coroutines;
//using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
//using Trinity.Components.Adventurer.Game.Events;
//using Trinity.Components.Adventurer.Game.Quests;
//using Trinity.Components.Adventurer.Settings;
//using Trinity.Components.Combat;
//using Trinity.Components.Combat.Resources;
//using Zeta.Bot;
//using Zeta.Bot.Profile;
//using Zeta.Game;
//using Zeta.TreeSharp;
//using Zeta.XmlEngine;

//namespace Trinity.ProfileTags
//{
//    [XmlElement("Bounties")]
//    public class BountiesTag : ProfileBehavior
//    {

//        private List<Act> _acts;
//        private List<Act> _completedActs;
//        public Act _currentAct;
//        private ActBountiesCoroutine _currentActBountiesCoroutine;

//        private bool _isDone;

//        public override bool IsDone
//        {
//            get
//            {
//                return _isDone;
//            }
//        }

//        public static BountiesTag Current { get; private set; }

//        public override void OnStart()
//        {
//            Current = this;

//            if (!Core.Adventurer.IsEnabled)
//            {
//                Core.Logger.Error("Plugin is not enabled. Please enable Adventurer and try again.");
//                return;
//            }

//            Combat.CombatMode = CombatMode.Questing;
//            PluginEvents.CurrentProfileType = ProfileType.Bounty;

//            ResetAll();

//            _currentActBountiesCoroutine = GetNextAct();
//            if (_currentActBountiesCoroutine != null)
//            {
//                Core.Logger.Log("[Bounties] Picked {0} as new target.", _currentActBountiesCoroutine.Act);
//            }
//        }

//        public override void OnDone()
//        {
//            Current = null;
//        }

//        private void ResetAll()
//        {
//            _isDone = false;
//            _currentAct = Act.Invalid;
//            _currentActBountiesCoroutine = null;
//            _acts = new List<Act>();
//            _completedActs = new List<Act>();
//            if (PluginSettings.Current.BountyAct1) _acts.Add(Act.A1);
//            if (PluginSettings.Current.BountyAct2) _acts.Add(Act.A2);
//            if (PluginSettings.Current.BountyAct3) _acts.Add(Act.A3);
//            if (PluginSettings.Current.BountyAct4) _acts.Add(Act.A4);
//            if (PluginSettings.Current.BountyAct5) _acts.Add(Act.A5);
//            //_acts = new List<Act> { Act.A1, Act.A2, Act.A3, Act.A4, Act.A5 };

//        }

//        protected override Composite CreateBehavior()
//        {
//            return new ActionRunCoroutine(ctx => GetCoroutine());
//        }

//        private DateTime _delayUntilTime = DateTime.MinValue;

//        private async Task<bool> GetCoroutine()
//        {
//            if (DateTime.UtcNow < _delayUntilTime)
//            {
//                Core.Logger.Debug("[Bounties] Waiting...");
//                await Coroutine.Sleep(250);
//                return false;
//            }

//            if (_isDone)
//            {
//                return true;
//            }
//            PluginEvents.PulseUpdates();
//            if (_currentActBountiesCoroutine == null)
//            {
//                _isDone = true;
//                return true;
//            }

//            if (PluginEvents.TimeSinceWorldChange < 1000)
//            {
//                Core.Logger.Debug("[Bounties] Sleeping 1 second due to world change");
//                await Coroutine.Sleep(1000);
//            }

//            if (!await _currentActBountiesCoroutine.GetCoroutine())
//                return false;

//            _completedActs.Add(_currentAct);

//            _delayUntilTime = DateTime.UtcNow.AddSeconds(5);

//            if (PluginSettings.Current.BountyMode3.HasValue && PluginSettings.Current.BountyMode3.Value)
//            {
//                _acts.Remove(_currentAct);
//                if (_acts.Count == 0)
//                {
//                    _isDone = true;
//                    return true;
//                }
//            }
//            _currentActBountiesCoroutine = GetNextAct();
//            if (_currentActBountiesCoroutine != null)
//            {
//                Core.Logger.Log("[Bounties] Picked {0} as new target.)", _currentActBountiesCoroutine.Act);
//            }
//            return true;
//        }


//        private ActBountiesCoroutine GetNextAct()
//        {
//            //var bonusAct = ZetaDia.CurrentBonusAct;

//            //if (PluginSettings.Current.BountyMode1.HasValue && PluginSettings.Current.BountyMode1.Value)
//            //{
//            //    Core.Logger.Log("[Bounties] Skip Mode activated. Trying to pick the best act.");
//            //    if (BountyHelpers.AreAllActBountiesSupported(bonusAct) && !_completedActs.Contains(bonusAct))
//            //    {
//            //        _currentAct = bonusAct;
//            //        return new ActBountiesCoroutine(_currentAct);
//            //    }
//            //    for (var i = Act.A1; i <= Act.A5; i++)
//            //    {
//            //        if (_completedActs.Contains(i)) continue;
//            //        if (!BountyHelpers.AreAllActBountiesSupported(bonusAct)) continue;
//            //        _currentAct = i;
//            //        return new ActBountiesCoroutine(_currentAct);
//            //    }
//            //}

//            if (PluginSettings.Current.BountyMode3.HasValue && PluginSettings.Current.BountyMode3.Value)
//            {
//                Core.Logger.Log("[Bounties] Act Selection Mode activated. Trying to pick the best act.");
//                if (_acts.Count > 0)
//                {
//                    if(_acts.Contains(Act.A4))
//                        return new ActBountiesCoroutine(Act.A4);

//                    return new ActBountiesCoroutine(_acts[0]);
//                }
//            }
//            else
//            {
//                Core.Logger.Log("[Bounties] Balance Mats Mode activated. Trying to pick the best act.");
//                var matCounts = new Dictionary<Act, long>
//                {
//                    {Act.A1,  BountyHelpers.GetActMatsCount(Act.A1)},
//                    {Act.A2,  BountyHelpers.GetActMatsCount(Act.A2)},
//                    {Act.A3,  BountyHelpers.GetActMatsCount(Act.A3)},
//                    {Act.A4,  BountyHelpers.GetActMatsCount(Act.A4)},
//                    {Act.A5,  BountyHelpers.GetActMatsCount(Act.A5)},
//                };

//                Core.Logger.Log("[Bounties] Current Bounty Mats Counts");
//                Core.Logger.Log("[Bounties] Act 1: {0}", matCounts[Act.A1]);
//                Core.Logger.Log("[Bounties] Act 2: {0}", matCounts[Act.A2]);
//                Core.Logger.Log("[Bounties] Act 3: {0}", matCounts[Act.A3]);
//                Core.Logger.Log("[Bounties] Act 4: {0}", matCounts[Act.A4]);
//                Core.Logger.Log("[Bounties] Act 5: {0}", matCounts[Act.A5]);

//                var averageMatsCount = matCounts.Values.Average(m => m);
//                var minMatsCount = matCounts.Values.Min(m => m);
//                var maxMatsCount = matCounts.Values.Max(m => m);
//                Core.Logger.Debug("[Bounties] Average Mats Count: {0}", averageMatsCount);
//                const int diff = 8;
//                if (averageMatsCount - minMatsCount > diff)
//                {
//                    averageMatsCount = minMatsCount;
//                    Core.Logger.Debug("[Bounties] Average Mats Count - Min Mats Count > {0}", diff);
//                }
//                else if (maxMatsCount - minMatsCount <= diff)
//                {
//                    Core.Logger.Debug("[Bounties] Max Mats Count - Min Mats Count <= {0}", diff);
//                    averageMatsCount = maxMatsCount;
//                }
//                Core.Logger.Debug("[Bounties] Will try to run acts with less than or equal to {0} mats.", averageMatsCount + 1);

//                var eligibleActs =
//                    matCounts.Where(
//                        kv =>
//                            !_completedActs.Contains(kv.Key) && BountyHelpers.AreAllActBountiesSupported(kv.Key) &&
//                            kv.Value <= averageMatsCount + 1).ToDictionary(kv => kv.Key, kv => kv.Value);

//                if (eligibleActs.Count == 0)
//                {
//                    Core.Logger.Log("[Bounties] It seems like we are done with this game, restarting.");
//                    _isDone = true;
//                    return null;
//                }
//                _currentAct = eligibleActs.OrderBy(kv => kv.Value).First().Key;
//                return new ActBountiesCoroutine(_currentAct);

//                //Core.Logger.Log("[Bounties] Force Bonus Act Mode activated. Attempting to run {0}.", bonusAct);
//                //if (!BountyHelpers.AreAllActBountiesSupported(bonusAct))
//                //{
//                //    Core.Logger.Log("[Bounties] One or more unsupported bounties are detected in the bonus act, restarting the game.");
//                //    _isDone = true;
//                //    return null;
//                //}
//                //if (_completedActs.Contains(bonusAct))
//                //{
//                //    Core.Logger.Log("[Bounties] It seems like the bonus act is completed or contains an incomplete bounty, restarting the game.");
//                //    _isDone = true;
//                //    return null;
//                //}
//                //_currentAct = bonusAct;
//                //return new ActBountiesCoroutine(_currentAct);
//            }
//            return null;
//        }


//        public override void ResetCachedDone(bool force = false)
//        {
//            ResetAll();
//        }
//    }
//}
