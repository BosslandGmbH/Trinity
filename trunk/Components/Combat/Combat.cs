using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Combat.Abilities;
using Trinity.Coroutines;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Behaviors;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Objects;
using Trinity.Reference;
using Trinity.Routines;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Components.Combat
{
    public class Combat : Component
    {
        /// <summary>
        /// Handles fighting a target.
        /// </summary>
        public static ITargetHandler Targeting { get; } = new TargetHandler();

        /// <summary>
        /// Handles casting for spells
        /// </summary>
        public static ISpellHandler Spells { get; } = new SpellHandler();

        /// <summary>
        /// Access point for the current routine.
        /// </summary>
        public static IRoutineProvider Routines { get; } = new RoutineProvider();

        /// <summary>
        /// Prioritizes targets
        /// </summary>
        public static IWeightingProvider Weighting { get; } = new WeightingProvider();

        /// <summary>
        /// 
        /// </summary>
        public static CombatMode CombatMode { get; set; }

        /// <summary>
        /// Combat Hook entry-point, manages when lower-level hooks can run and executes trinity features.
        /// </summary>
        public static async Task<bool> MainCombatTask()
        {
            Instance.Stats.Start();
            try
            {
                if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld || !ZetaDia.Me.IsValid)
                    return false;

                if (Core.Player.IsDead)
                    return false;

                await UsePotion.Execute();
                await OpenTreasureBags.Execute();
    
                VacuumItems.Execute();           

                if (!IsCombatAllowed && (Targeting.CurrentTarget == null || Targeting.CurrentTarget.IsUnit))
                {
                    return false;
                }

                var target = Weighting.WeightActors(Core.Targets);

                if (await CastBuffs())
                    return true;

                if (target != null)
                {
                    return await Targeting.HandleTarget(target);
                }

                if (!Core.Player.IsCasting)
                {
                    if (await Behaviors.MoveToMarker.While(m => m.MarkerType == WorldMarkerType.LegendaryItem || m.MarkerType == WorldMarkerType.SetItem))
                        return true;

                    await EmergencyRepair.Execute();
                    await AutoEquipSkills.Instance.Execute();
                    await AutoEquipItems.Instance.Execute();
                }
                    
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception in MainCombatTask {ex}");
            }
            finally
            {
                Instance.Stats.Stop();
            }

            return false; // Allow Profile to Run.   

        }

        private static async Task<bool> CastBuffs()
        {
            var power = Routines.Current.GetBuffPower();
            if (power != null && power.SNOPower != SpellHistory.LastPowerUsed)
            {
                return await Spells.CastTrinityPower(power, "Buff");
            }
            return false;
        }

        /// <summary>
        /// Allows for completely disabling combat. Settable through API only.
        /// </summary>
        public static bool IsCombatAllowed
        {
            get
            {
                if (CombatMode == CombatMode.KillAll)
                    return true;

                if (!CombatTargeting.Instance.AllowedToKillMonsters)
                    return false;
 
                if (CombatMode == CombatMode.Normal || CombatMode == CombatMode.Questing)
                    return true;

                if (CombatMode == CombatMode.Off || CombatMode == CombatMode.SafeZerg && TargetUtil.NumMobsInRangeOfPosition(Core.Player.Position, 10f) > 4)
                    return true;

                return true;
            }
        }

        public static bool IsInCombat
            => IsCombatAllowed && Targeting.CurrentTarget != null && Targeting.CurrentTarget.ActorType == ActorType.Monster;

        public static bool IsCurrentlyAvoiding
            => Targeting.CurrentTarget != null && Targeting.CurrentTarget.IsSafeSpot && Core.Avoidance.Avoider.ShouldAvoid;

        public static bool IsCurrentlyKiting
            => Targeting.CurrentTarget != null && Targeting.CurrentTarget.IsSafeSpot && Core.Avoidance.Avoider.ShouldKite;
    }


}
