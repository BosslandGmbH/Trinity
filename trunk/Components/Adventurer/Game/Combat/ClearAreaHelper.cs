using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Combat.Resources;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Game.Combat
{
    public class ClearAreaHelper
    {
        private static DateTime _lastCheckedObjectivesTime = DateTime.MinValue;

        public static void CheckClearArea(Func<bool> condition)
        {
            if (DateTime.UtcNow.Subtract(_lastCheckedObjectivesTime).TotalSeconds < 1)
                return;

            _lastCheckedObjectivesTime = DateTime.UtcNow;

            var shouldEnable = condition.Invoke();

            if (!shouldEnable && IsCombatModeModified)
            {
                RevertClearArea();
                return;
            }

            if (shouldEnable && !IsCombatModeModified)
            {
                Logger.DebugSetting("Enabling kill-all mode to clear area");
                GameEvents.OnWorldChanged += OnWorldChanged;
                _previousCombatMode = Components.Combat.Combat.CombatMode;
                Components.Combat.Combat.CombatMode = CombatMode.KillAll;
                IsCombatModeModified = true;
            }
        }

        public static void CheckClearArea(Vector3 center, float radius)
        {
            CheckClearArea(() => ActorFinder.FindNearestHostileUnitInRadius(center, radius) != Vector3.Zero);
        }

        public static void CheckClearArea(BountyData bountyData)
        {
            CheckClearArea(() => bountyData.QuestData?.Steps?.Any(
                q => q.IsActive && q.Objectives.Any(
                    o => o.IsActive && ClearMonsterObjectiveTypes.Contains(o.ObjectiveType))) ?? false);
        }

        public static bool IsCombatModeModified;

        private static HashSet<QuestStepObjectiveType> ClearMonsterObjectiveTypes { get; } = new HashSet<QuestStepObjectiveType>
        {
            QuestStepObjectiveType.KillAnyMonsterInLevelArea
        };

        private static CombatMode _previousCombatMode;

        private static void OnWorldChanged(object sender, EventArgs eventArgs)
        {
            RevertClearArea();
        }

        private static void RevertClearArea()
        {
            if (!IsCombatModeModified)
                return;

            if (Components.Combat.Combat.CombatMode == CombatMode.KillAll)
            {
                Components.Combat.Combat.CombatMode = _previousCombatMode;
                Logger.DebugSetting($"Reverting combat mode to '{_previousCombatMode}' after clearing area");
            }

            GameEvents.OnWorldChanged -= OnWorldChanged;
            IsCombatModeModified = false;
        }
    }
}
