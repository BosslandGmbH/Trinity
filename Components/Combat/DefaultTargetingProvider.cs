using Serilog;
using System;
using System.Threading.Tasks;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Grid;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Zeta.Bot.Coroutines;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.Actors.Gizmos;

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
        private static readonly ILogger s_logger = Logger.GetLoggerInstanceForType();

        public TrinityActor CurrentTarget { get; private set; }

        public TrinityPower CurrentPower { get; private set; }

        public TrinityActor LastTarget { get; private set; }

        public TrinityPower LastPower { get; private set; }

        public float MaxTargetDistance { get; } = 200f;

        public void SetCurrentTarget(TrinityActor target)
        {
            if (CurrentTarget != null &&
                CurrentTarget.Targeting.TotalTargetedTime > TimeSpan.FromSeconds(30))
            {
                s_logger.Information($"[{nameof(SetCurrentTarget)}] Long target time detected: {CurrentTarget} duration: {CurrentTarget.Targeting.TotalTargetedTime.TotalSeconds:N2}s");
            }

            if (target != null &&
                target.IsMe)
            {
                return;
            }

            if (CurrentTarget == target)
            {
                target?.Targeting.UpdateTargetInfo(true);
                return;
            }

            if (target == null &&
                CurrentTarget != null)
            {
                s_logger.Information($"[{nameof(SetCurrentTarget)}] Clearing Target. Was: {CurrentTarget}");
            }

            if (CurrentTarget != null)
            {
                target?.Targeting.UpdateTargetInfo(false);
                LastTarget = CurrentTarget;
            }

            if (target != null)
            {
                target?.Targeting.UpdateTargetInfo(true);
                s_logger.Information($"[{nameof(SetCurrentTarget)}] New Target: {target.Name} {target.Targeting} WeightInfo={target.WeightInfo} Targeting={target.Targeting}");
            }

            CurrentTarget = target;
        }

        public void SetCurrentPower(TrinityPower power)
        {
            if (CurrentPower != null)
            {
                LastPower = CurrentPower;
            }
            CurrentPower = power;
        }

        public async Task<bool> HandleTarget(TrinityActor target)
        {
            if (await TrinityCombat.Routines.Current.HandleAvoiding())
                return true;

            if (await TrinityCombat.Routines.Current.HandleKiting())
                return true;

            if (target == null ||
                !target.IsValid)
            {
                Clear();
                return false;
            }
            
            if (TryBlacklist(target))
            {
                Clear();
                return false;
            }

            // Gizmos should always use the MoveAndInteract coroutine.
            // Don't try to outsmart the game with custom shit down the line.
            if (target.IsGizmo &&
                target.ToDiaObject() is DiaGizmo obj &&
                !obj.IsDestructibleObject)
            {
                // TODO: Fix the interaction condition here.
                if (await CommonCoroutines.MoveAndInteract(
                        obj,
                        () => obj is GizmoLootContainer lc ? lc.IsOpen : obj.HasBeenOperated) == CoroutineResult.Running)
                {
                    return true;
                }

                Clear();
                return false;
            }

            SetCurrentTarget(target);

            var power = TrinityCombat.Routines.Current.GetPowerForTarget(target);

            SetCurrentPower(power);

            return await TrinityCombat.Routines.Current.HandleTarget(target);
        }

        private void Clear()
        {
            SetCurrentTarget(null);
            SetCurrentPower(null);
        }

        public bool TryBlacklist(TrinityActor target)
        {
            if (target == null)
                return false;

            if (target.IsBlacklisted)
                return false;

            switch (target.Type)
            {
                case TrinityObjectType.Door when target.ActorSnoId == SNOActor.p43_AD_Catacombs_Door_A &&
                                                 target.Targeting.TargetedTimes > 3:
                    // Special case 'p43_AD_Catacombs_Door_A' no way to tell it's locked, blacklist quickly to explore
                    GenericBlacklist.Blacklist(
                        target, TimeSpan.FromSeconds(15),
                        $"Probably locked door p43_AD_Catacombs_Door_A at {target.Position}");
                    return true;
                case TrinityObjectType.Door when !target.IsUsed:
                    return false;
            }

            if (LastPower != null &&
                LastPower.SNOPower == SNOPower.Axe_Operate_Gizmo &&
                target.IsGizmo &&
                target.IsLastTarget &&
                target.Targeting.TargetedTimes > 25 &&
                target.IsItem &&
                target is TrinityItem item &&
                item.ToAcdItem().IsLowQuality)
            {
                // There's a weird stuck where bot is unable to interact with an item, possibly move/interact range related.
                GenericBlacklist.Blacklist(
                    target,
                    TimeSpan.FromSeconds(120),
                    $"Failed too many times to pickup low quality item. {target.Name} Distance={target.Distance}");
                return true;
            }

            switch (target.Type)
            {
                case TrinityObjectType.ProgressionGlobe:
                case TrinityObjectType.Shrine:
                    return false;
            }

            if (target.IsElite)
                return false;

            var times = target.Targeting.TargetedTimes;
            var duration = target.Targeting.TotalTargetedTime;

            if (duration > TimeSpan.FromSeconds(10) &&
                times > 5)
            {
                GenericBlacklist.Blacklist(
                    target,
                    TimeSpan.FromSeconds(5),
                    $"Micro-Blacklist for reposition / anti-flipflop (Times={times} Duration={duration})");
                return true;
            }

            if (target.IsCorruptGrowth &&
                duration > TimeSpan.FromSeconds(10) &&
                Core.Player.MovementSpeed <= 2)
            {
                GenericBlacklist.Blacklist(
                    target,
                    TimeSpan.FromSeconds(3),
                    $"Micro-Blacklist for corrupt growth reposition (Times={times} Duration={duration})");
                return true;
            }

            if (duration > TimeSpan.FromSeconds(30) &&
                !target.IsElite)
            {
                GenericBlacklist.Blacklist(
                    target,
                    TimeSpan.FromSeconds(60),
                    $"Targetted for too long ({duration})");
                return true;
            }

            if (duration <= TimeSpan.FromSeconds(30) ||
                target.Targeting.TargetedTimes <= 50 ||
                target.IsBoss)
            {
                return false;
            }

            GenericBlacklist.Blacklist(
                target,
                TimeSpan.FromSeconds(60),
                $"Targetted too many times ({times})");
            return true;

        }

        public bool IsInRange(TrinityActor target, TrinityPower power)
        {
            if (target == null ||
                target.IsSafeSpot)
            {
                return false;
            }

            if (CurrentPower != null &&
                CurrentPower.IsCastOnSelf)
            {
                return true;
            }

            if (GameData.ForceSameZDiffSceneSnoIds.Contains(Core.Player.CurrentSceneSnoId) &&
                Math.Abs(target.Position.Z - Core.Player.Position.Z) > 2)
            {
                return false;
            }

            var objectRange = Math.Max(2f, target.RequiredRadiusDistance);

            var spellRange = CurrentPower != null ? Math.Max(2f, CurrentPower.MinimumRange) : objectRange;

            var targetRangeRequired = target.IsHostile || target.IsDestroyable ? spellRange : objectRange;

            s_logger.Verbose($"[{nameof(IsInRange)}] >> CurrentPower={TrinityCombat.Targeting.CurrentPower} CurrentTarget={target} RangeReq:{targetRangeRequired} RadDist:{target.RadiusDistance}");
            
            // Handle Belial differently, he's never in LineOfSight.
            if (Core.Player.IsInBossEncounter &&
                target.ActorSnoId == SNOActor.Belial)
            {
                return target.RadiusDistance <= targetRangeRequired;
            }

            return target.RadiusDistance <= targetRangeRequired && IsInLineOfSight(target);
        }

        public bool IsInRange(Vector3 position, TrinityPower power)
        {
            if (position == Vector3.Zero)
                return false;

            if (power == null ||
                power.SNOPower == SNOPower.None)
            {
                return false;
            }

            if (power.IsCastOnSelf)
                return true;

            if (power.CastWhenBlocked &&
                PlayerMover.IsBlocked)
            {
                return true;
            }

            if (GameData.ForceSameZDiffSceneSnoIds.Contains(Core.Player.CurrentSceneSnoId) &&
                Math.Abs(position.Z - Core.Player.Position.Z) > 2)
            {
                return false;
            }
            
            var rangeRequired = Math.Max(1f, power.MinimumRange);
            var distance = position.Distance(Core.Player.Position);

            TrinityActor currentTarget = TrinityCombat.Targeting.CurrentTarget;
            if (Core.Player.IsInBossEncounter &&
                currentTarget != null)
            {
                var positionIsBoss = currentTarget.IsBoss &&
                                     currentTarget.Position.Distance(position) < 10f;
                if (positionIsBoss)
                {
                    rangeRequired += currentTarget.CollisionRadius;
                }
            }

            s_logger.Verbose($"[{nameof(IsInRange)}] >> CurrentPower={power} CurrentTarget={position} RangeReq:{rangeRequired} Dist:{distance}");

            // Handle Belial differently, he's never in LineOfSight.
            if (Core.Player.IsInBossEncounter &&
                currentTarget != null &&
                currentTarget.ActorSnoId == SNOActor.Belial)
            {
                return distance <= rangeRequired;
            }

            return distance <= rangeRequired && IsInLineOfSight(position);
        }

        public bool IsInLineOfSight(TrinityActor currentTarget)
        {
            if (GameData.LineOfSightWhitelist.Contains(currentTarget.ActorSnoId))
                return true;

            if (currentTarget.RadiusDistance <= 2f)
                return true;

            var requiresRayWalk = Core.ProfileSettings.Options.CurrentSceneOptions.AlwaysRayWalk;
            if (!requiresRayWalk &&
                currentTarget.Targeting.TotalTargetedTime < TimeSpan.FromSeconds(5) &&
                currentTarget.IsInLineOfSight)
            {
                return true;
            }

            return TrinityGrid.Instance.CanRayWalk(currentTarget, 5f);
        }

        public bool IsInLineOfSight(Vector3 position)
        {
            var longTargetTime = CurrentTarget?.Targeting.TotalTargetedTime < TimeSpan.FromSeconds(10);

            if (longTargetTime ||
                Core.BlockedCheck.IsBlocked ||
                Core.StuckHandler.IsStuck ||
                Core.ProfileSettings.Options.CurrentSceneOptions.AlwaysRayWalk)
            {
                return TrinityGrid.Instance.CanRayWalk(Core.Player.Position, position, 5f);
            }

            return TrinityGrid.Instance.CanRayCast(Core.Player.Position, position);
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
