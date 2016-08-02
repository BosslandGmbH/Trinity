using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Abilities;
using Trinity.Helpers;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.SNO;

namespace Trinity.DbProvider
{
    public class KillAllBountyDetector
    {
        public static DateTime LastCheckTime = DateTime.MinValue;

        private static CombatMode _previousCombatMode = CombatMode.On;
        private static bool _isCombatModeChanged;
        private static int _currentQuestId;
        private static int _currentQuestStep;
        private static BountyInfo _currentBounty;
        private static int _currentWorldId;
        private static QuestInfo _info;
        private static QuestStep[] _steps;

        public static void Check()
        {
            //todo test, seems to be causing performance issues

            //if (DateTime.UtcNow.Subtract(LastCheckTime).TotalSeconds < 2)
            //    return;

            //LastCheckTime = DateTime.UtcNow;

            //var worldId = ZetaDia.CurrentWorldSnoId;
            //var isSameQuestAndStep = ConditionParser.IsActiveQuestAndStep(_currentQuestId, _currentQuestStep);
            //var activeBounty = GetUpdatedActiveBounty(isSameQuestAndStep, worldId);

            //if (activeBounty == null || worldId != _currentWorldId || !isSameQuestAndStep || activeBounty.State != QuestState.InProgress)
            //{
            //    if (_isCombatModeChanged)
            //    {
            //        Logger.Log("No Longer on Kill-All Bounty, reverting Combat Mode");
            //        RevertCombatMode();
            //    }
            //}

            //if (activeBounty != null && activeBounty.State == QuestState.InProgress)
            //{             
            //    foreach (var step in _steps)
            //    {
            //        var killAllBountyStep = step.QuestStepObjectiveSet.QuestStepObjectives.FirstOrDefault(o => o.ObjectiveType == QuestStepObjectiveType.KillAllMonstersInWorld);
            //        if (killAllBountyStep != null)
            //        {
            //            if (activeBounty.Info.QuestStep == step.StepId)
            //            {
            //                Logger.Log("Current Bounty ({0}) Step: {1} is Kill All Objective", _info.DisplayName, step.Name);
            //                SetCombatMode();
            //            }
            //            else
            //            {
            //                Logger.Log("This Bounty ({0}) has a Kill All Objective", _info.DisplayName);
            //                RevertCombatMode();
            //            }

            //            return;                          
            //        }
            //    }
            //}
        }

        private static BountyInfo GetUpdatedActiveBounty(bool isSameQuestAndStep, int worldId)
        {
            BountyInfo activeBounty;
            if (isSameQuestAndStep && worldId == _currentWorldId)
            {
                activeBounty = _currentBounty;
            }
            else
            {
                _currentWorldId = worldId;
                _currentBounty = ZetaDia.ActInfo.ActiveBounty;
                activeBounty = _currentBounty;

                if (activeBounty != null)
                {
                    _info = activeBounty.Info;
                    _steps = _info.QuestRecord.Steps;
                    _currentQuestId = _info.QuestSNO;
                    _currentQuestStep = _info.QuestStep;
                }
            }
            return activeBounty;
        }

        private static void SetCombatMode()
        {
            if (CombatBase.CombatMode != CombatMode.KillAll && !_isCombatModeChanged)
            {
                _previousCombatMode = CombatBase.CombatMode;
                CombatBase.CombatMode = CombatMode.KillAll;
            }
        }

        private static void RevertCombatMode()
        {
            if (_isCombatModeChanged)
            {
                if (CombatBase.CombatMode != _previousCombatMode)
                {
                    CombatBase.CombatMode = _previousCombatMode;
                }
                _isCombatModeChanged = false;
            }
        }
    }
}
