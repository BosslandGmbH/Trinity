#region

using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Threading.Tasks;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Routines;
using Trinity.Settings;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


#endregion

namespace Trinity.Components.Combat
{
    public interface ITargetingProvider
    {
        Task<bool> HandleTarget(TrinityActor target);

        bool IsInRange(TrinityActor target, TrinityPower power);

        bool IsInRange(Vector3 position, TrinityPower power);

        TrinityActor CurrentTarget { get; }
        TrinityPower CurrentPower { get; }
        TrinityActor LastTarget { get; }
        TrinityPower LastPower { get; }
        float MaxTargetDistance { get; }

        void Clear();
    }

    public class DefaultTargetingProvider : ITargetingProvider
    {
        public TrinityActor CurrentTarget { get; private set; }

        public TrinityPower CurrentPower { get; private set; }

        public TrinityActor LastTarget { get; private set; }

        public TrinityPower LastPower { get; private set; }

        public float MaxTargetDistance { get; private set; } = 200f;

        private void SetCurrentTarget(TrinityActor target)
        {
            if (CurrentTarget != null && CurrentTarget.Targeting.TotalTargetedTime > TimeSpan.FromSeconds(30))
            {
                Core.Logger.Log(LogCategory.Targetting, $"Long target time detected: {CurrentTarget} duration: {CurrentTarget.Targeting.TotalTargetedTime.TotalSeconds:N2}s");
            }

            if (CurrentTarget == target)
                return;

            if (target == null && CurrentTarget != null)
            {
                Core.Logger.Log(LogCategory.Targetting, $"Clearing Target. Was: {CurrentTarget}");
            }

            if (CurrentTarget != null)
            {
                CurrentTarget.Targeting.IsTargetted = false;
                LastTarget = CurrentTarget;
            }

            if (target != null)
            {
                target.Targeting.IsTargetted = true;
                Core.Logger.Log(LogCategory.Targetting, $"New Target: {target.Name} {target.Targeting} WeightInfo={target.WeightInfo} Targeting={target.Targeting}");
            }

            CurrentTarget = target;
        }

        private void SetCurrentPower(TrinityPower power)
        {
            if (CurrentPower != null)
            {
                LastPower = CurrentPower;
            }
            CurrentPower = power;
        }

        public async Task<bool> HandleTarget(TrinityActor target)
        {
            if (await HandleAvoidance(target))
                return true;

            if (target == null || !target.IsValid)
            {
                //Core.Logger.Verbose(LogCategory.Targetting, $"Null or invalid Target. {target?.Name}");
                Clear();
                return false;
            }

            if (TryBlacklist(target))
            {
                Clear();
                return false;
            }

            SetCurrentTarget(target);
            SetCurrentPower(GetPowerForTarget(target));

            if (await WaitForRiftBossSpawn())
                return true;

            if (WaitForInteractionChannelling())
                return true;

            if (await HandleKiting())
                return true;

            if (CurrentPower == null)
            {
                if (!Core.Player.IsPowerUseDisabled)
                {
                    Core.Logger.Log(LogCategory.Targetting, $"No valid power was selected for target: {CurrentTarget}");
                }
                return false;
            }

            if (CurrentPower.SNOPower != SNOPower.None)
            {
                if (!await TrinityCombat.Spells.CastTrinityPower(CurrentPower))
                {
                    if (DateTime.UtcNow.Subtract(SpellHistory.LastSpellUseTime).TotalSeconds > 5)
                    {
                        Core.Logger.Verbose(LogCategory.Targetting, $"Routine power cast failure timeout. Clearing Target: {target?.Name} and Power: {CurrentPower}");
                        Clear();
                        return false;
                    }

                    if (CurrentPower.SNOPower != SNOPower.Walk && CurrentPower.TargetPosition.Distance(Core.Player.Position) > MaxTargetDistance)
                        return false;
                }
            }

            return true;
        }

        private void Clear()
        {
            SetCurrentTarget(null);
            SetCurrentPower(null);
        }

        private async Task<bool> WaitForRiftBossSpawn()
        {
            if (Core.Rift.IsInRift && CurrentTarget.IsBoss)
            {
                if (CurrentTarget.IsSpawningBoss)
                {
                    Core.Logger.Verbose(LogCategory.Targetting, "Waiting while rift boss spawn");

                    Vector3 safeSpot;
                    if (Core.Avoidance.Avoider.TryGetSafeSpot(out safeSpot, 30f, 100f, CurrentTarget.Position))
                    {
                        PlayerMover.MoveTo(safeSpot);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool WaitForInteractionChannelling()
        {
            if (Core.Player.IsCasting && !Core.Player.IsTakingDamage && CurrentTarget != null && CurrentTarget.IsGizmo)
            {
                Core.Logger.Verbose(LogCategory.Targetting, "Waiting while channelling spell");
                return true;
            }
            return false;
        }

        private bool TryBlacklist(TrinityActor target)
        {
            if (target == null)
                return false;

            if (target.IsBlacklisted)
                return false;

            if (target.Type == TrinityObjectType.Door)
            {
                if (target.ActorSnoId == 454346 && target.Targeting.TargetedTimes > 3)
                {
                    // Special case 'p43_AD_Catacombs_Door_A' no way to tell it's locked, blacklist quickly to explore
                    GenericBlacklist.Blacklist(target, TimeSpan.FromSeconds(15), $"Probably locked door p43_AD_Catacombs_Door_A at {target.Position}");
                    return true;
                }

                if (!target.IsUsed)
                    return false;
            }

            if (LastPower != null && LastPower.SNOPower == SNOPower.Axe_Operate_Gizmo && target.IsGizmo && target.IsLastTarget && target.Targeting.TargetedTimes > 25 && target.IsItem && (target as TrinityItem).IsLowQuality)
            {
                // There's a weird stuck where bot is unable to interact with an item, possibly move/interact range related.
                GenericBlacklist.Blacklist(target, TimeSpan.FromSeconds(120), $"Failed too many times to pickup low quality item. {target.Name} Distance={target.Distance}");
                return true;
            }

            if (target.Type == TrinityObjectType.ProgressionGlobe)
                return false;

            if (target.Type == TrinityObjectType.Shrine)
                return false;

            if (target.IsElite)
                return false;

            var times = target.Targeting.TargetedTimes;
            var duration = target.Targeting.TotalTargetedTime;

            if (duration > TimeSpan.FromSeconds(10) && times > 5)
            {
                GenericBlacklist.Blacklist(target, TimeSpan.FromSeconds(5), $"Micro-Blacklist for reposition / anti-flipflop (Times={times} Duration={duration})");
                return true;
            }

            if (duration > TimeSpan.FromSeconds(30) && !target.IsElite)
            {
                GenericBlacklist.Blacklist(target, TimeSpan.FromSeconds(60), $"Targetted for too long ({duration})");
                return true;
            }

            if (duration > TimeSpan.FromSeconds(30) && target.Targeting.TargetedTimes > 50 && !target.IsBoss)
            {
                GenericBlacklist.Blacklist(target, TimeSpan.FromSeconds(60), $"Targetted too many times ({times})");
                return true;
            }

            return false;
        }

        private TrinityPower GetPowerForTarget(TrinityActor target)
        {
            var routine = TrinityCombat.Routines.Current;
            if (target == null)
                return null;

            switch (target.Type)
            {
                case TrinityObjectType.BloodShard:
                case TrinityObjectType.Gold:
                case TrinityObjectType.HealthGlobe:
                case TrinityObjectType.PowerGlobe:
                case TrinityObjectType.ProgressionGlobe:
                    return routine.GetMovementPower(target.Position);

                case TrinityObjectType.Door:
                case TrinityObjectType.HealthWell:
                case TrinityObjectType.Shrine:
                case TrinityObjectType.Interactable:
                case TrinityObjectType.CursedShrine:
                    return InteractPower(target, 100, 250);

                case TrinityObjectType.CursedChest:
                case TrinityObjectType.Container:
                    return InteractPower(target, 100, 1200);

                case TrinityObjectType.Item:
                    return InteractPower(target, 15, 15, 6f);

                case TrinityObjectType.Destructible:
                case TrinityObjectType.Barricade:
                    return routine.GetDestructiblePower();
            }

            if (target.IsQuestGiver)
                return InteractPower(target, 100, 250);

            if (TrinityCombat.IsInCombat)
            {
                var routinePower = routine.GetOffensivePower();

                TrinityPower kamakaziPower;
                if (TryKamakaziPower(target, routinePower, out kamakaziPower))
                    return kamakaziPower;

                return routinePower;
            }

            return null;
        }

        private static bool TryKamakaziPower(TrinityActor target, TrinityPower routinePower, out TrinityPower power)
        {
            // The routine may want us attack something other than current target, like best cluster, whatever.
            // But for goblin kamakazi we need a special exception to force it to always target the goblin.

            power = null;
            if (target.IsTreasureGoblin && Core.Settings.Weighting.GoblinPriority == TargetPriority.Kamikaze)
            {
                Core.Logger.Log(LogCategory.Targetting, $"Forcing Kamakazi Target on {target}, routineProvided={routinePower}");

                var kamaKaziPower = RoutineBase.DefaultPower;
                if (routinePower != null)
                {
                    routinePower.SetTarget(target);
                    kamaKaziPower = routinePower;
                }

                power = kamaKaziPower;
                return true;
            }
            return false;
        }

        public TrinityPower InteractPower(TrinityActor actor, int waitBefore, int waitAfter, float addedRange = 0)
            => new TrinityPower(actor.IsUnit ? SNOPower.Axe_Operate_NPC : SNOPower.Axe_Operate_Gizmo, actor.AxialRadius + addedRange, actor.Position, actor.AcdId, waitBefore, waitAfter);

        public async Task<bool> CastDefensiveSpells()
        {
            var power = TrinityCombat.Routines.Current.GetDefensivePower();
            if (power != null && power.SNOPower != SpellHistory.LastPowerUsed)
            {
                return await TrinityCombat.Spells.CastTrinityPower(power, "Defensive");
            }
            return false;
        }

        private async Task<bool> HandleKiting()
        {
            if (Core.Avoidance.Avoider.ShouldKite)
            {
                if (await TrinityCombat.Routines.Current.HandleKiting())
                {
                    return true;
                }

                Vector3 safespot;
                if (Core.Avoidance.Avoider.TryGetSafeSpot(out safespot) && safespot.Distance(ZetaDia.Me.Position) > 3f)
                {
                    Core.Logger.Log(LogCategory.Avoidance, $"Kiting");
                    await CastDefensiveSpells();
                    PlayerMover.MoveTo(safespot);
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> HandleAvoidance(TrinityActor newTarget)
        {
            if (Core.Avoidance.Avoider.ShouldAvoid)
            {
                if (await TrinityCombat.Routines.Current.HandleAvoiding(newTarget))
                {
                    return true;
                }

                var isCloseToSafeSpot = Core.Player.Position.Distance(Core.Avoidance.Avoider.SafeSpot) < 10f;
                if (CurrentTarget != null && isCloseToSafeSpot)
                {
                    var canReachTarget = CurrentTarget.Distance < CurrentPower?.MinimumRange;
                    if (canReachTarget && CurrentTarget.IsAvoidanceOnPath && !Core.Player.Actor.IsInAvoidance)
                    {
                        Core.Logger.Log(LogCategory.Avoidance, $"Not avoiding due to being safe and target is within range");
                        return false;
                    }
                }

                var safe = (!Core.Player.IsTakingDamage || Core.Player.CurrentHealthPct > 0.5f) && !Core.Player.Actor.IsInCriticalAvoidance;

                if (newTarget?.Position == LastTarget?.Position && newTarget.IsAvoidanceOnPath && safe)
                {
                    Core.Logger.Log(LogCategory.Avoidance, $"Not avoiding due to being safe and waiting for avoidance before handling target {newTarget.Name}");
                    Core.PlayerMover.MoveTowards(Core.Player.Position);
                    return true;
                }

                if (!TrinityCombat.IsInCombat && Core.Player.Actor.IsAvoidanceOnPath && safe)
                {
                    Core.Logger.Log(LogCategory.Avoidance, $"Waiting for avoidance to clear (out of combat)");
                    Core.PlayerMover.MoveTowards(Core.Player.Position);
                    return true;
                }

                Core.Logger.Log(LogCategory.Avoidance, $"Avoiding");
                await CastDefensiveSpells();
                PlayerMover.MoveTo(Core.Avoidance.Avoider.SafeSpot);
                return true;
            }
            return false;
        }

        public bool IsInRange(TrinityActor target, TrinityPower power)
        {
            if (target == null || target.IsSafeSpot)
                return false;

            if (CurrentPower != null && CurrentPower.IsCastOnSelf)
                return true;

            var objectRange = Math.Max(2f, target.RequiredRadiusDistance);
            var spellRange = CurrentPower != null ? Math.Max(2f, CurrentPower.MinimumRange) : objectRange;

            var targetRangeRequired = target.IsHostile || target.IsDestroyable ? Math.Max(spellRange, objectRange) : objectRange;
            var targetRadiusDistance = target.RadiusDistance;

            Core.Logger.Verbose(LogCategory.Targetting, $">> CurrentPower={TrinityCombat.Targeting.CurrentPower} CurrentTarget={target} RangeReq:{targetRangeRequired} RadDist:{targetRadiusDistance}");
            return targetRadiusDistance <= targetRangeRequired && IsInLineOfSight(target);
        }

        public bool IsInRange(Vector3 position, TrinityPower power)
        {
            if (position == Vector3.Zero)
                return false;

            if (power == null || power.SNOPower == SNOPower.None)
                return false;

            if (power.IsCastOnSelf)
                return true;

            if (power.CastWhenBlocked && PlayerMover.IsBlocked)
                return true;

            var rangeRequired = Math.Max(1f, power.MinimumRange);
            var distance = position.Distance(Core.Player.Position);

            if (Core.Player.IsInBossEncounter && TrinityCombat.Targeting.CurrentTarget != null)
            {
                var positionIsBoss = TrinityCombat.Targeting.CurrentTarget.IsBoss && TrinityCombat.Targeting.CurrentTarget.Position.Distance(position) < 10f;
                if (positionIsBoss)
                {
                    rangeRequired += TrinityCombat.Targeting.CurrentTarget.CollisionRadius;
                }
            }

            Core.Logger.Verbose(LogCategory.Targetting, $">> CurrentPower={power} CurrentTarget={position} RangeReq:{rangeRequired} Dist:{distance}");
            return distance <= rangeRequired && IsInLineOfSight(position);
        }

        private bool IsInLineOfSight(TrinityActor currentTarget)
        {
            if (GameData.LineOfSightWhitelist.Contains(currentTarget.ActorSnoId))
                return true;

            if (currentTarget.RadiusDistance <= 2f)
                return true;

            var requiresRayWalk = Core.ProfileSettings.Options.CurrentSceneOptions.AlwaysRayWalk;
            if (!requiresRayWalk && currentTarget.Targeting.TotalTargetedTime < TimeSpan.FromSeconds(5) && currentTarget.IsInLineOfSight)
                return true;

            return currentTarget.IsWalkable;
        }

        private bool IsInLineOfSight(Vector3 position)
        {
            var longTargetTime = CurrentTarget?.Targeting.TotalTargetedTime < TimeSpan.FromSeconds(10);

            if (longTargetTime || Core.BlockedCheck.IsBlocked || Core.StuckHandler.IsStuck || Core.ProfileSettings.Options.CurrentSceneOptions.AlwaysRayWalk)
            {
                return Core.Grids.Avoidance.CanRayWalk(Core.Player.Position, position);
            }

            return Core.Grids.Avoidance.CanRayCast(Core.Player.Position, position);
        }

        void ITargetingProvider.Clear()
        {
            CurrentTarget = null;
            LastTarget = null;
            CurrentPower = null;
            LastPower = null;
        }
    }
}