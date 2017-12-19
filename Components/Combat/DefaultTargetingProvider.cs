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

        public void SetCurrentTarget(TrinityActor target)
        {
            if (CurrentTarget != null && CurrentTarget.Targeting.TotalTargetedTime > TimeSpan.FromSeconds(30))
            {
                Core.Logger.Log(LogCategory.Targetting, $"长时间检测的目标: {CurrentTarget} 持续时间: {CurrentTarget.Targeting.TotalTargetedTime.TotalSeconds:N2}秒");
            }

            if (target != null && target.IsMe)
            {
                return;
            }

            if (CurrentTarget == target)
            {
                target?.Targeting.UpdateTargetInfo(true);
                return;
            }

            if (target == null && CurrentTarget != null)
            {
                Core.Logger.Log(LogCategory.Targetting, $"清除目标. 因为: {CurrentTarget}");
            }

            if (CurrentTarget != null)
            {
                target?.Targeting.UpdateTargetInfo(false);
                LastTarget = CurrentTarget;
            }

            if (target != null)
            {
                target?.Targeting.UpdateTargetInfo(true);
                Core.Logger.Log(LogCategory.Targetting, $"新的目标: {target.Name} {target.Targeting} 权重信息={target.WeightInfo} 定位={target.Targeting}");
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

            if (target == null || !target.IsValid)
            {
                Clear();
                return false;
            }

            if (TryBlacklist(target))
            {
                Clear();
                return false;
            }

            SetCurrentTarget(target);
            // SENY
            switch (target.Type)
            {
                case TrinityObjectType.Door:
                case TrinityObjectType.HealthWell:
                case TrinityObjectType.Shrine:
                case TrinityObjectType.Interactable:
                case TrinityObjectType.CursedShrine:
                    CompensateMove(target.Position, 10);
                    break;
            }

            var power = TrinityCombat.Routines.Current.GetPowerForTarget(target);

            SetCurrentPower(power);

            return await TrinityCombat.Routines.Current.HandleTarget(target);
        }

        /// <summary>
        /// 移动补偿
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="minDest"></param>
        /// <returns></returns>
        public virtual bool CompensateMove(Vector3 destination, float minDest)
        {
            if ((!Core.Player.IsInTown && (!ZetaDia.Storage.RiftGuardianSpawned || TrinityCombat.Targeting.CurrentTarget.IsBoss)) && (destination.Distance(Core.Player.Position) >= minDest))
            {
                float diffZ = Math.Abs((float)(destination.Z - Core.Player.Position.Z));
                float distance = destination.Distance(Core.Player.Position);
                if ((((ZetaDia.Storage.CurrentRiftType == Zeta.Game.Internals.RiftType.Greater || ZetaDia.Storage.CurrentRiftType == Zeta.Game.Internals.RiftType.Nephalem) && (ZetaDia.Storage.RiftStarted && (distance < 50f))) && (diffZ < 3f)) && Core.Grids.CanRayWalk(Core.Player.Position, destination))
                {
                    Navigator.PlayerMover.MoveTowards(destination);
                }
                else
                {
                    PlayerMover.NavigateTo(destination, "");
                }
                return true;
            }
            return false;
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

            if (target.IsCorruptGrowth && duration > TimeSpan.FromSeconds(10) && Core.Player.MovementSpeed <= 2)
            {
                GenericBlacklist.Blacklist(target, TimeSpan.FromSeconds(3), $"Micro-Blacklist for corrupt growth reposition (Times={times} Duration={duration})");
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


        public bool IsInRange(TrinityActor target, TrinityPower power)
        {
            if (target == null || target.IsSafeSpot)
                return false;

            if (CurrentPower != null && CurrentPower.IsCastOnSelf)
                return true;

            if (GameData.ForceSameZDiffSceneSnoIds.Contains(Core.Player.CurrentSceneSnoId) && Math.Abs(target.Position.Z - Core.Player.Position.Z) > 2)
                return false;

            var objectRange = Math.Max(2f, target.RequiredRadiusDistance);

            var spellRange = CurrentPower != null ? Math.Max(2f, CurrentPower.MinimumRange) : objectRange;

            var targetRangeRequired = target.IsHostile || target.IsDestroyable ? spellRange : objectRange;

            Core.Logger.Verbose(LogCategory.Targetting, $">> CurrentPower={TrinityCombat.Targeting.CurrentPower} CurrentTarget={target} RangeReq:{targetRangeRequired} RadDist:{target.RadiusDistance}");

            return target.RadiusDistance <= targetRangeRequired && IsInLineOfSight(target);
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

            if (GameData.ForceSameZDiffSceneSnoIds.Contains(Core.Player.CurrentSceneSnoId) && Math.Abs(position.Z - Core.Player.Position.Z) > 2)
                return false;

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

        public bool IsInLineOfSight(TrinityActor currentTarget)
        {
            if (GameData.LineOfSightWhitelist.Contains(currentTarget.ActorSnoId))
                return true;

            if (currentTarget.RadiusDistance <= 2f)
                return true;

            var requiresRayWalk = Core.ProfileSettings.Options.CurrentSceneOptions.AlwaysRayWalk;
            if (!requiresRayWalk && currentTarget.Targeting.TotalTargetedTime < TimeSpan.FromSeconds(5) && currentTarget.IsInLineOfSight)
                return true;

            return Core.Grids.Avoidance.CanRayWalk(currentTarget, 5f);
        }

        public bool IsInLineOfSight(Vector3 position)
        {
            var longTargetTime = CurrentTarget?.Targeting.TotalTargetedTime < TimeSpan.FromSeconds(10);

            if (longTargetTime || Core.BlockedCheck.IsBlocked || Core.StuckHandler.IsStuck || Core.ProfileSettings.Options.CurrentSceneOptions.AlwaysRayWalk)
            {
                return Core.Grids.Avoidance.CanRayWalk(Core.Player.Position, position, 5f);
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