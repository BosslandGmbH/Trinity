#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Buddy.Coroutines;
using Trinity.Components.Combat.Resources;
using Trinity.Coroutines;
using Trinity.Coroutines.Town;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Items;
using Trinity.Reference;
using Trinity.Routines;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Zeta.TreeSharp;
using Logger = Trinity.Framework.Helpers.Logger;

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
    }

    public class DefaultTargetingProvider : ITargetingProvider
    {
        public TrinityActor CurrentTarget { get; private set; }

        public TrinityPower CurrentPower { get; private set; }

        public TrinityActor LastTarget { get; private set; }

        public TrinityPower LastPower { get; private set; }

        private void SetCurrentTarget(TrinityActor target)
        {
            if (CurrentTarget != null && CurrentTarget.Targeting.TotalTargetedTime > TimeSpan.FromSeconds(30))
            {
                Logger.Log(LogCategory.Targetting, $"Long target time detected: {CurrentTarget} duration: {CurrentTarget.Targeting.TotalTargetedTime.TotalSeconds:N2}s");
            }

            if (CurrentTarget == target)
                return;

            if (target == null && CurrentTarget != null)
            {
                Logger.Log(LogCategory.Targetting, $"Clearing Target. Was: {CurrentTarget}");
            }
         
            if (CurrentTarget != null)
            {
                CurrentTarget.Targeting.IsTargetted = false;
                LastTarget = CurrentTarget;
            }

            if (target != null)
            {
                target.Targeting.IsTargetted = true;
                Logger.Log(LogCategory.Targetting, $"New Target: {target.Name} {target.Targeting} WeightInfo={target.WeightInfo}");
            }

            CurrentTarget = target;                 
        }

        private void SetCurrentPower(TrinityPower power)
        {
            if (CurrentPower != null)
                LastPower = CurrentPower;

            CurrentPower = power;
        }

        public async Task<bool> HandleTarget(TrinityActor target)
        {
            if (await HandleAvoidance(target))
                return true;

            if (target == null || !target.IsValid)
            {
                SetCurrentTarget(null);
                SetCurrentPower(null);
                return false;
            }

            if (TryBlacklist(target))
            {
                SetCurrentTarget(null);
                SetCurrentPower(null);
                return false;
            }

            SetCurrentTarget(target);
            SetCurrentPower(GetPowerForTarget(target));            

            if (await HandleKiting())
                return true;

            if (CurrentPower == null)
            {
                if (!Core.Player.IsPowerUseDisabled)
                {
                    Logger.LogError(LogCategory.Targetting, $"No valid power was selected for target: {CurrentTarget}");                    
                }
                return false;
            }

            if (CurrentPower.SNOPower != SNOPower.None)
            {
                await Combat.Spells.CastTrinityPower(CurrentPower);
            }
            
            return true;
        }

        private bool TryBlacklist(TrinityActor target)
        {
            if (target == null)
                return false;

            if (target.Type == TrinityObjectType.Door && !target.IsUsed)
                return false;

            if (target.Type == TrinityObjectType.ProgressionGlobe)
                return false;

            if (target.Type == TrinityObjectType.Shrine)
                return false;

            if (target.IsElite)
                return false;

            var times = target.Targeting.TargetedTimes;
            var duration = target.Targeting.TotalTargetedTime;

            if (duration > TimeSpan.FromSeconds(10) && times > 10)
            {
                GenericBlacklist.Blacklist(target, TimeSpan.FromSeconds(5), $"Micro-Blacklist for reposition / anti-flipflop (Times={times} Duration={duration})");
                return true;
            }

            if (duration > TimeSpan.FromMinutes(2))
            {
               GenericBlacklist.Blacklist(target, TimeSpan.FromSeconds(60), $"Targetted for too long ({duration})");
                return true;
            }

            if (duration > TimeSpan.FromSeconds(30) && target.Targeting.TargetedTimes > 50)
            {
                GenericBlacklist.Blacklist(target, TimeSpan.FromSeconds(60), $"Targetted too many times ({times})");
                return true;
            }

            return false;
        }

        private TrinityPower GetPowerForTarget(TrinityActor target)
        {
            var routine = Combat.Routines.Current;
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

            if (Combat.IsInCombat)
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
            if (target.IsTreasureGoblin && Core.Settings.Weighting.GoblinPriority == GoblinPriority.Kamikaze)
            {
                Logger.Log(LogCategory.Targetting, $"Forcing Kamakazi Target on {target}, routineProvided={routinePower}");

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
            var power = Combat.Routines.Current.GetDefensivePower();
            if (power != null && power.SNOPower != SpellHistory.LastPowerUsed)
            {
                return await Combat.Spells.CastTrinityPower(power, "Defensive");
            }
            return false;
        }

        private async Task<bool> HandleKiting()
        {
            if (Core.Avoidance.Avoider.ShouldKite)
            {
                if (await Combat.Routines.Current.HandleKiting())
                {
                    return true;
                }

                Vector3 safespot;
                if (Core.Avoidance.Avoider.TryGetSafeSpot(out safespot) && safespot.Distance(ZetaDia.Me.Position) > 3f)
                {
                    Logger.Log(LogCategory.Avoidance, $"Kiting");
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
                if (await Combat.Routines.Current.HandleAvoiding())
                {
                    return true;
                }
          
                Logger.Log(LogCategory.Avoidance, $"Avoiding");
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

            Logger.LogVerbose(LogCategory.Targetting, $">> CurrentPower={Combat.Targeting.CurrentPower} CurrentTarget={target} RangeReq:{targetRangeRequired} RadDist:{targetRadiusDistance}");
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

            var rangeRequired = Math.Max(1f, power.MinimumRange);
            var distance = position.Distance(Core.Player.Position);

            Logger.LogVerbose(LogCategory.Targetting, $">> CurrentPower={power} CurrentTarget={position} RangeReq:{rangeRequired} Dist:{distance}");
            return distance <= Math.Max(1f, power.MinimumRange) && IsInLineOfSight(position);
        }

        private bool IsInLineOfSight(TrinityActor currentTarget)
        {            
            if (GameData.LineOfSightWhitelist.Contains(currentTarget.ActorSnoId))
                return true;

            if (currentTarget.RadiusDistance <= 2f)
                return true;

            if (currentTarget.Targeting.TotalTargetedTime < TimeSpan.FromSeconds(15) && currentTarget.IsInLineOfSight)
                return true;

            return currentTarget.IsWalkable;
        }

        private bool IsInLineOfSight(Vector3 position)
        {
            return Core.Grids.Avoidance.CanRayCast(Core.Player.Position, position);
        }


    }
}