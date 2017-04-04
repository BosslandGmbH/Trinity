using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Components.Adventurer.Coroutines.KeywardenCoroutines;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Combat.Resources;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Game.Combat
{
    public class SafeZerg : PulsingObject
    {
        private static SafeZerg _instance;

        public static SafeZerg Instance
        {
            get { return _instance ?? (_instance = new SafeZerg()); }
        }

        private SafeZerg()
        {
            EnablePulse();
        }

        private bool _zergEnabled;

        public void EnableZerg()
        {
            _zergEnabled = true;
        }

        public void DisableZerg()
        {
            if (_zergEnabled || TargetingHelper.CombatState == CombatState.Disabled)
            {
                _zergEnabled = false;
                SetPreviousCombatMode();
                TargetingHelper.TurnCombatOn();
            }
        }

        protected override void OnPulse()
        {
            ZergCheck();
        }

        public static HashSet<int> CorruptGrowthIds = new HashSet<int>()
        {
            210120, //a4dun_Garden_Corruption_Monster
            210268, //a4dun_Garden_Corruption_HellRift_Monster
            360111, //a4dun_Garden_Corruption_Monster_Despair
        };

        private void ZergCheck()
        {
            if (!_zergEnabled || ZetaDia.Me == null || !ZetaDia.Me.IsValid)
            {
                return;
            }

            var corruptGrowthDetectionRadius = ZetaDia.Service.Hero.Class == ActorClass.Barbarian ? 30 : 20;
            var combatState = false;

            if (!combatState && ZetaDia.Me.HitpointsCurrentPct <= 0.8f)
            {
                combatState = true;
            }

            if (!combatState &&

                ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).Any(u => u.IsFullyValid() && u.IsAlive && (
                KeywardenDataFactory.GoblinSNOs.Contains(u.ActorSnoId) || (KeywardenDataFactory.A4CorruptionSNOs.Contains(u.ActorSnoId) && u.IsAlive & u.Position.Distance(AdvDia.MyPosition) <= corruptGrowthDetectionRadius))
                ))

            {
                Core.Logger.Verbose("Turning off zerg because either corrupt growth or keywarden nearby");
                combatState = true;
            }

            var keywarden = KeywardenDataFactory.Items.FirstOrDefault(kw => kw.Value.WorldId == AdvDia.CurrentWorldId);
            if (!combatState && keywarden.Value != null && keywarden.Value.IsAlive)
            {
                var kwActor = ActorFinder.FindUnit(keywarden.Value.KeywardenSNO);
                if (kwActor != null && kwActor.Distance < 80f)
                {
                    Core.Logger.Verbose("Turning off zerg because {0} is nearby. Distance={1}", kwActor.Name, kwActor.Distance);
                    combatState = true;
                }
            }

            var units = ZetaDia.Actors.GetActorsOfType<DiaUnit>(true).ToList();
            if (units.Any(u => CorruptGrowthIds.Contains(u.ActorSnoId) && u.Distance < 30f))
            {
                Core.Logger.Verbose($"Turning off zerg because corrupt growth is nearby");
                combatState = true;
            }

            var health = ZetaDia.Me.HitpointsCurrentPct;
            var closeUnitsCount = units.Count(u => u.IsFullyValid() && u.IsHostile && u.IsAlive && u.Position.Distance(AdvDia.MyPosition) <= 15f);
            if (!combatState && (closeUnitsCount >= 8 || closeUnitsCount >= 3 && health <= 0.6))
            {
                Core.Logger.Verbose($"Turning off zerg because {closeUnitsCount} units nearby and health is {health}. Distance={1}");
                combatState = true;
            }

            if (combatState)
            {
                SetPreviousCombatMode();
            }
            else
            {
                SetSafeZergCombatMode();
            }
        }

        private void SetPreviousCombatMode()
        {
            if (Components.Combat.TrinityCombat.CombatMode == CombatMode.SafeZerg)
            {
                Core.Logger.Debug($"Reverted to {_previousCombatMode} Combat Mode");
                Components.Combat.TrinityCombat.CombatMode = _previousCombatMode;
            }
        }

        private void SetSafeZergCombatMode()
        {
            if (Components.Combat.TrinityCombat.CombatMode != CombatMode.SafeZerg)
            {
                Core.Logger.Debug("Set to Safe Zerg Combat Mode");
                _previousCombatMode = Components.Combat.TrinityCombat.CombatMode;
                Components.Combat.TrinityCombat.CombatMode = CombatMode.SafeZerg;
            }
        }

        private CombatMode _previousCombatMode;
    }
}