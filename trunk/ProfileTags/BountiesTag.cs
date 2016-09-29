using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Settings;
using Trinity.Components.Adventurer.Util;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Zeta.Bot;
using Zeta.Bot.Profile;
using Zeta.Game;
using Zeta.TreeSharp;
using Zeta.XmlEngine;

namespace Trinity.ProfileTags
{
    [XmlElement("Bounties")]
    public class BountiesTag : ProfileBehavior
    {

        private List<Act> _acts;
        private List<Act> _completedActs;
        private Act _currentAct;
        private ActBountiesCoroutine _currentActBountiesCoroutine;

        private bool _isDone;

        public override bool IsDone
        {
            get
            {
                return _isDone;
            }
        }

        public override void OnStart()
        {
            if (!Core.Adventurer.IsEnabled)
            {
                Logger.Error("Plugin is not enabled. Please enable Adventurer and try again.");
                return;
            }

            Combat.CombatMode = CombatMode.Questing;
            PluginEvents.CurrentProfileType = ProfileType.Bounty;

            ResetAll();

            _currentActBountiesCoroutine = GetNextAct();
            if (_currentActBountiesCoroutine != null)
            {
                Logger.Info("[Bounties] Picked {0} as new target. (BonusAct: {1})", _currentActBountiesCoroutine.Act, _currentActBountiesCoroutine.Act == ZetaDia.CurrentBonusAct);
            }
        }

        private void ResetAll()
        {
            _isDone = false;
            _currentAct = Act.Invalid;
            _currentActBountiesCoroutine = null;
            _acts = new List<Act>();
            _completedActs = new List<Act>();
            if (PluginSettings.Current.BountyAct1) _acts.Add(Act.A1);
            if (PluginSettings.Current.BountyAct2) _acts.Add(Act.A2);
            if (PluginSettings.Current.BountyAct3) _acts.Add(Act.A3);
            if (PluginSettings.Current.BountyAct4) _acts.Add(Act.A4);
            if (PluginSettings.Current.BountyAct5) _acts.Add(Act.A5);
            //_acts = new List<Act> { Act.A1, Act.A2, Act.A3, Act.A4, Act.A5 };

        }

        protected override Composite CreateBehavior()
        {
            return new ActionRunCoroutine(ctx => GetCoroutine());
        }

        private DateTime _delayUntilTime = DateTime.MinValue;

        private async Task<bool> GetCoroutine()
        {
            if (DateTime.UtcNow < _delayUntilTime)
            {
                Logger.Debug("[Bounties] Waiting...");
                await Coroutine.Sleep(250);
                return false;
            }

            if (_isDone)
            {
                return true;
            }
            PluginEvents.PulseUpdates();
            if (_currentActBountiesCoroutine == null)
            {
                _isDone = true;
                return true;
            }

            if (PluginEvents.TimeSinceWorldChange < 1000)
            {
                Logger.Debug("[Bounties] Sleeping 1 second due to world change");
                await Coroutine.Sleep(1000);
            }

            if (!await _currentActBountiesCoroutine.GetCoroutine())
                return false;

            _completedActs.Add(_currentAct);

            _delayUntilTime = DateTime.UtcNow.AddSeconds(5);

            if (PluginSettings.Current.BountyMode3.HasValue && PluginSettings.Current.BountyMode3.Value)
            {
                _acts.Remove(_currentAct);
                if (_acts.Count == 0)
                {
                    _isDone = true;
                    return true;
                }
            }
            _currentActBountiesCoroutine = GetNextAct();
            if (_currentActBountiesCoroutine != null)
            {
                Logger.Info("[Bounties] Picked {0} as new target. (BonusAct: {1})", _currentActBountiesCoroutine.Act, _currentActBountiesCoroutine.Act == ZetaDia.CurrentBonusAct);
            }
            return true;
        }


        private ActBountiesCoroutine GetNextAct()
        {
            var bonusAct = ZetaDia.CurrentBonusAct;

            if (PluginSettings.Current.BountyMode1.HasValue && PluginSettings.Current.BountyMode1.Value)
            {
                Logger.Info("[Bounties] Skip Mode activated. Trying to pick the best act.");
                if (BountyHelpers.AreAllActBountiesSupported(bonusAct) && !_completedActs.Contains(bonusAct))
                {
                    _currentAct = bonusAct;
                    return new ActBountiesCoroutine(_currentAct);
                }
                for (var i = Act.A1; i <= Act.A5; i++)
                {
                    if (_completedActs.Contains(i)) continue;
                    if (!BountyHelpers.AreAllActBountiesSupported(bonusAct)) continue;
                    _currentAct = i;
                    return new ActBountiesCoroutine(_currentAct);
                }
            }
            else if (PluginSettings.Current.BountyMode2.HasValue && PluginSettings.Current.BountyMode2.Value)
            {
                Logger.Info("[Bounties] Balance Mats Mode activated. Trying to pick the best act.");
                var matCounts = new Dictionary<Act, long>
                {
                    {Act.A1,  BountyHelpers.GetActMatsCount(Act.A1)},
                    {Act.A2,  BountyHelpers.GetActMatsCount(Act.A2)},
                    {Act.A3,  BountyHelpers.GetActMatsCount(Act.A3)},
                    {Act.A4,  BountyHelpers.GetActMatsCount(Act.A4)},
                    {Act.A5,  BountyHelpers.GetActMatsCount(Act.A5)},
                };

                Logger.Info("[Bounties] Current Bounty Mats Counts");
                Logger.Info("[Bounties] Act 1: {0}", matCounts[Act.A1]);
                Logger.Info("[Bounties] Act 2: {0}", matCounts[Act.A2]);
                Logger.Info("[Bounties] Act 3: {0}", matCounts[Act.A3]);
                Logger.Info("[Bounties] Act 4: {0}", matCounts[Act.A4]);
                Logger.Info("[Bounties] Act 5: {0}", matCounts[Act.A5]);

                var averageMatsCount = matCounts.Values.Average(m => m);
                var minMatsCount = matCounts.Values.Min(m => m);
                var maxMatsCount = matCounts.Values.Max(m => m);
                Logger.Debug("[Bounties] Average Mats Count: {0}", averageMatsCount);
                const int diff = 8;
                if (averageMatsCount - minMatsCount > diff)
                {
                    averageMatsCount = minMatsCount;
                    Logger.Debug("[Bounties] Average Mats Count - Min Mats Count > {0}", diff);
                }
                else if (maxMatsCount - minMatsCount <= diff)
                {
                    Logger.Debug("[Bounties] Max Mats Count - Min Mats Count <= {0}", diff);
                    averageMatsCount = maxMatsCount;
                }
                Logger.Debug("[Bounties] Will try to run acts with less than or equal to {0} mats.", averageMatsCount + 1);

                var eligibleActs =
                    matCounts.Where(
                        kv =>
                            !_completedActs.Contains(kv.Key) && BountyHelpers.AreAllActBountiesSupported(kv.Key) &&
                            kv.Value <= averageMatsCount + 1).ToDictionary(kv => kv.Key, kv => kv.Value);

                if (eligibleActs.Count == 0)
                {
                    Logger.Info("[Bounties] It seems like we are done with this game, restarting.");
                    _isDone = true;
                    return null;
                }
                if (eligibleActs.ContainsKey(bonusAct))
                {
                    _currentAct = bonusAct;
                }
                else
                {
                    _currentAct = eligibleActs.OrderBy(kv => kv.Value).First().Key;
                }
                return new ActBountiesCoroutine(_currentAct);

            }
            else if (PluginSettings.Current.BountyMode3.HasValue && PluginSettings.Current.BountyMode3.Value)
            {
                Logger.Info("[Bounties] Act Selection Mode activated. Trying to pick the best act.");
                if (_acts.Count > 0)
                {
                    _currentAct = _acts.Contains(bonusAct) ? bonusAct : _acts[0];
                    return new ActBountiesCoroutine(_currentAct);
                }
            }
            else
            {
                Logger.Info("[Bounties] Force Bonus Act Mode activated. Attempting to run {0}.", bonusAct);
                if (!BountyHelpers.AreAllActBountiesSupported(bonusAct))
                {
                    Logger.Info("[Bounties] One or more unsupported bounties are detected in the bonus act, restarting the game.");
                    _isDone = true;
                    return null;
                }
                if (_completedActs.Contains(bonusAct))
                {
                    Logger.Info("[Bounties] It seems like the bonus act is completed or contains an incomplete bounty, restarting the game.");
                    _isDone = true;
                    return null;
                }
                _currentAct = bonusAct;
                return new ActBountiesCoroutine(_currentAct);
            }
            return null;
        }


        public override void ResetCachedDone(bool force = false)
        {
            ResetAll();
        }
    }
}
