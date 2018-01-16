using System;
using Trinity.Framework;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Zeta.Bot;
using Zeta.Common;


namespace Trinity.Components.Adventurer.Game.Combat
{
    public class ClearAreaHelper
    {
        private static DateTime _lastCheckedObjectivesTime = DateTime.MinValue;

        public static void CheckClearArea(float radius, Func<bool> condition)
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
                Core.Logger.Debug("Enabling kill-all mode to clear area");
                GameEvents.OnWorldChanged += OnWorldChanged;

                //_previousCombatMode = Components.Combat.TrinityCombat.CombatMode;
                //Components.Combat.TrinityCombat.CombatMode = CombatMode.KillAll;

                TrinityCombat.CombatMode = CombatMode.KillAll;
                TrinityCombat.KillAllRadius = radius;

                IsCombatModeModified = true;
            }
        }

        public static void CheckClearArea(Vector3 center, float radius)
        {
            CheckClearArea(radius, () => ActorFinder.FindNearestHostileUnitInRadius(center, radius) != Vector3.Zero);
        }

        //public static void CheckClearArea(BountyData bountyData)
        //{
        //    CheckClearArea(80, () => bountyData?.QuestData?.Steps?.Any(
        //        q => q.IsActive && q.Objectives.Any(
        //            o => o.IsActive && ClearMonsterObjectiveTypes.Contains(o.ObjectiveType))) ?? false);
        //}

        public static bool IsCombatModeModified;

        //private static HashSet<QuestStepObjectiveType> ClearMonsterObjectiveTypes { get; } = new HashSet<QuestStepObjectiveType>
        //{
        //    QuestStepObjectiveType.KillAnyMonsterInLevelArea
        //};

        //private static CombatMode _previousCombatMode;

        private static void OnWorldChanged(object sender, EventArgs eventArgs)
        {
            RevertClearArea();
        }

        private static void RevertClearArea()
        {
            if (!IsCombatModeModified)
                return;

            TrinityCombat.ResetCombatMode();

            //if (Components.Combat.TrinityCombat.CombatMode == CombatMode.KillAll)
            //{
            //    Components.Combat.TrinityCombat.CombatMode = _previousCombatMode;
            //    Core.Logger.Debug($"Reverting combat mode to '{_previousCombatMode}' after clearing area");
            //}

            GameEvents.OnWorldChanged -= OnWorldChanged;
            IsCombatModeModified = false;
        }
    }
}