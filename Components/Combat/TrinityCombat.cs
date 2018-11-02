using System;
using Trinity.Framework;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Components.Combat.Resources;
using Trinity.Components.Coroutines;
using Trinity.Components.Coroutines.Town;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Zeta.Bot;
using Zeta.Game;
using Zeta.Game.Internals.SNO;

namespace Trinity.Components.Combat
{
    public static class TrinityCombat
    {
        static TrinityCombat()
        {
            BotMain.OnStart += (b) => ResetCombatMode();
            GameEvents.OnGameChanged += (s, e) => ResetCombatMode();
            GameEvents.OnWorldChanged += (s, e) => ResetCombatMode();
        }

        /// <summary>
        /// Handles selecting targets and if we should engage in combat.
        /// </summary>
        public static ITargetingProvider Targeting { get; } = DefaultProviders.Targeting;

        /// <summary>
        /// Handles casting spells
        /// </summary>
        public static ISpellProvider Spells { get; } = DefaultProviders.Spells;

        /// <summary>
        /// Access point for the current routine.
        /// </summary>
        public static IRoutineProvider Routines { get; } = DefaultProviders.Routines;

        /// <summary>
        /// Prioritizes targets
        /// </summary>
        public static IWeightingProvider Weighting { get; } = DefaultProviders.Weighting;

        /// <summary>
        /// Access point for information on other bots in the party, and coordination.
        /// </summary>
        public static IPartyProvider Party { get; set; } = DefaultProviders.Party;

        /// <summary>
        /// Loot information, if items should be picked up, stashed etc.
        /// </summary>
        public static ILootProvider Loot { get; set; } = DefaultProviders.Loot;

        /// <summary>
        /// Combat Hook entry-point, manages when lower-level hooks can run and executes trinity features.
        /// </summary>
        public static async Task<bool> MainCombatTask()
        {
            if (!ZetaDia.IsInGame ||
                ZetaDia.Globals.IsLoadingWorld ||
                !ZetaDia.Me.IsValid ||
                ZetaDia.Me.IsDead)
            {
                return false;
            }

            if (ZetaDia.IsInTown &&
                TrinityTownRun.IsVendoring)
            {
                return false;
            }

            if (!Core.Scenes.CurrentWorldScenes.Any())
                return false;

            // Allow a 'false' return in routine (to continue with profile) and skip default behavior.
            var startHookResult = await Routines.Current.HandleStart();
            if (Routines.Current.ShouldReturnStartResult)
                return startHookResult;

            // We don't really care about the result of DrinkPotion as it will always return
            // either CoroutineStatus.NoAction or CoroutineStatus.Done.
            await UsePotion.DrinkPotion();

            // TODO: Why is OpenTreasureBags called during combat? Move to Townrun.
            await OpenTreasureBags.Execute();

            if (!await VacuumItems.Execute())
                return true;

            var target = Weighting.WeightActors(Core.Targets);

            if (await CastBuffs())
                return true;

            if (await Routines.Current.HandleBeforeCombat())
                return true;

            // When combat is disabled, we're still allowing trinity to handle non-unit targets.
            if (!IsCombatAllowed && IsUnitOrInvalid(target))
                return false;

            if (await Targeting.HandleTarget(target))
                return true;

            // We're not in combat at this point.

            if (await Routines.Current.HandleOutsideCombat())
                return true;

            if (!Core.Player.IsCasting && (!TargetUtil.AnyMobsInRange(20f) || !Core.Player.IsTakingDamage))
            {
                await AutoEquipSkills.Instance.Execute();
                await AutoEquipItems.Instance.Execute();
                return false;
            }

            // Allow Profile to Run.
            return false;
        }

        private static bool IsUnitOrInvalid(TrinityActor target)
        {
            if (target == null || target.IsUnit) return true;
            if (target.Weight < 0 || Math.Abs(target.Weight) < float.Epsilon) return true;
            return !target.IsValid || target.IsUsed;
        }

        private static async Task<bool> CastBuffs()
        {
            var power = Routines.Current?.GetBuffPower();
            if (power != null && power.TimeSinceUseMs > 500 && !Core.Player.IsInteractingWithGizmo && !Core.Player.IsCastingTownPortalOrTeleport())
            {
                return await Spells.CastTrinityPower(power, "Buff");
            }
            return false;
        }

        public static bool IsCombatAllowed
        {
            get
            {
                if (!CombatTargeting.Instance.AllowedToKillMonsters)
                    return false;

                switch (CombatMode)
                {
                    case CombatMode.KillAll:
                        return true;
                    case CombatMode.Normal:
                        return true;
                    case CombatMode.Off:
                        Core.Logger.Verbose(LogCategory.Targetting, "CombatMode is set to Off");
                        return false;
                    case CombatMode.SafeZerg:
                        var result = Core.BlockedCheck.IsBlocked;
                        Core.Logger.Verbose(LogCategory.Targetting, result ? "Safe Zerg suspended due to being blocked" : "Safe Zerg is active");
                        return result;
                }

                return true;
            }
        }

        public static void ResetCombatMode()
        {
            CombatMode = CombatMode.Normal;
            KillAllRadius = 150f;
        }

        public static void SetKillMode(float radius = 80f)
        {
            CombatMode = CombatMode.KillAll;
            KillAllRadius = radius;
        }

        public static CombatMode CombatMode { get; set; }
        public static float KillAllRadius { get; set; }

        public static bool IsInCombat
            => IsCombatAllowed && Targeting.CurrentTarget != null && Targeting.CurrentTarget.ActorType == ActorType.Monster;

        public static bool IsCurrentlyAvoiding
            => Targeting.CurrentTarget != null && Targeting.CurrentTarget.IsSafeSpot && Core.Avoidance.Avoider.ShouldAvoid;

        public static bool IsCurrentlyKiting
            => Targeting.CurrentTarget != null && Targeting.CurrentTarget.IsSafeSpot && Core.Avoidance.Avoider.ShouldKite;
    }
}