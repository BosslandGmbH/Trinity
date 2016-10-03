using System;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Barbarian
{
    public class BarbarianBase : RoutineBase
    {
        // Important Notes!

        // Keep UI settings out of here! override in derived class with your settings
        // then call base.ShouldWhatever() if you dont wan't to duplicate the basic checks.

        // The bot will try to move into the range specified BEFORE casting a power.
        // => new TrinityPower(Skills.Barbarian.Leap, 15f, position)
        // means GetMovementPower() until within 15f of position, then cast Leap.

        // Wrap buff SNOPowers as expressions, because nobody is going to 
        // remember what P3_ItemPassive_Unique_Ring_010 means.

        // Use the Routine LogCategory for logging.
        // Logger.Log(LogCategory.Routine, $"My Current Target is {CurrentTarget}");

        #region IRoutine Defaults

        public virtual ActorClass Class { get; } = ActorClass.Barbarian;

        public virtual int PrimaryEnergyReserve => 25;
        public virtual int SecondaryEnergyReserve => 0;
        public virtual KiteMode KiteMode => KiteMode.Never;
        public virtual float KiteDistance => 15f;
        public virtual int KiteStutterDuration => 800;
        public virtual int KiteStutterDelay => 1400;
        public virtual int KiteHealthPct => 100;
        public virtual float TrashRange => 80f;
        public virtual float EliteRange => 80f;
        public virtual float HealthGlobeRange => 60f;
        public virtual float ShrineRange => 80f;
        public virtual Func<bool> ShouldIgnoreNonUnits { get; } = () => false;
        public virtual Func<bool> ShouldIgnorePackSize { get; } = () => false;
        public virtual Func<bool> ShouldIgnoreAvoidance { get; } = () => false;
        public virtual Func<bool> ShouldIgnoreKiting { get; } = () => false;
        public virtual Func<bool> ShouldIgnoreFollowing { get; } = () => false;

        #endregion

        #region Conditions

        protected virtual bool ShouldBash(out TrinityActor target)
        {
            target = null;

            if (!Skills.Barbarian.Bash.CanCast())
                return false;

            if (Skills.Barbarian.Bash.IsLastUsed && IsMultiPrimary)
                return false;

            target = CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldCleave(out TrinityActor target)
        {
            target = null;

            if (!Skills.Barbarian.Cleave.CanCast())
                return false;

            if (Skills.Barbarian.Cleave.IsLastUsed && IsMultiPrimary)
                return false;

            target = CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldFrenzy(out TrinityActor target)
        {
            target = null;

            if (!Skills.Barbarian.Frenzy.CanCast())
                return false;

            if (Skills.Barbarian.Frenzy.IsLastUsed && IsMultiPrimary)
                return false;

            target = CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldWeaponThrow(out TrinityActor target)
        {
            target = null;

            if (!Skills.Barbarian.WeaponThrow.CanCast())
                return false;

            if (Skills.Barbarian.WeaponThrow.IsLastUsed && IsMultiPrimary)
                return false;

            target = CurrentTarget;
            return target != null;
        }

        // Secondary

        protected virtual bool ShouldHammerOfTheAncients(out TrinityActor target)
        {
            target = null;

            if (!Skills.Barbarian.HammerOfTheAncients.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (Skills.Barbarian.HammerOfTheAncients.IsLastUsed && IsMultiSpender)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldSeismicSlam(out TrinityActor target)
        {
            target = null;

            if (!Skills.Barbarian.SeismicSlam.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (Skills.Barbarian.SeismicSlam.IsLastUsed && IsMultiSpender)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldRend(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Barbarian.Rend.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (Skills.Barbarian.Rend.IsLastUsed && IsMultiSpender)
                return false;

            if (Skills.Barbarian.Rend.TimeSinceUse < 500) 
                return false;

            if (TargetUtil.IsPercentOfMobsDebuffed(SNOPower.Barbarian_Rend, 10f, 0.75f))
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldWhirlwind(out Vector3 position)
        {
            // Note: This is for casting while not moving.  
            // Routine GetMovermentPower() may cast for movement
            // (Its called directly by our IPlayerMover)

            position = Vector3.Zero;

            if (!Skills.Barbarian.Whirlwind.CanCast())
                return false;

            if (!Sets.BulKathossOath.IsFullyEquipped)
            {
                if (Player.PrimaryResource < PrimaryEnergyReserve)
                    return false;
            }

            if (Skills.Barbarian.Whirlwind.IsLastUsed && IsMultiSpender)
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldAncientSpear(out TrinityActor target)
        {
            target = null;

            if (!Skills.Barbarian.AncientSpear.CanCast())
                return false;

            if (Skills.Barbarian.AncientSpear.IsLastUsed && IsMultiSpender)
                return false;

            target = TargetUtil.BestAoeUnit(60, true).IsInLineOfSight
                ? TargetUtil.BestAoeUnit(60, true)
                : TargetUtil.GetBestClusterUnit(10, 60, false, true, false, true);

            if (target == null)
                return false;

            return target.Distance <= 60 && Player.PrimaryResourcePct > 0.95;        }

        // Defensive

        protected virtual bool ShouldGroundStomp(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Barbarian.GroundStomp.CanCast())
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldIgnorePain()
        {
            if (!Skills.Barbarian.IgnorePain.CanCast())
                return false;

            if (Player.CurrentHealthPct < 0.90)
                return true;

            if (Player.IsIncapacitated)
                return true;

            if (Runes.Barbarian.MobRule.IsActive && TargetUtil.AnyPlayer(p => p.HitPointsPct < 0.50 && p.Distance < 50f))
                return true;

            if (PlayerMover.IsBlocked && Runes.Barbarian.Bravado.IsActive)
                return true;

            return false;
        }

        protected virtual bool ShouldLeap(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Barbarian.Leap.CanCast())
                return false;

            position = TargetUtil.GetBestClusterPoint();            

            if (Legendary.LutSocks.IsEquipped && Skills.Barbarian.Leap.TimeSinceUse < 2000)
                return true;

            if (Sets.MightOfTheEarth.IsFullyEquipped)
                return true;

            if (!TargetUtil.ClusterExists(15f, 50f, 5) && !TargetUtil.AnyElitesInRange(50f))
                return false;

            return position != Vector3.Zero;
        }

        protected virtual bool ShouldSprint()
        {
            if (!Skills.Barbarian.Sprint.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (!Core.Player.IsMoving && !Runes.Barbarian.Rush.IsActive)
                return false;

            if (Skills.Barbarian.Sprint.IsBuffActive)
                return false;

            return true;
        }

        // Might

        protected virtual bool ShouldOverpower(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Barbarian.Overpower.CanCast())
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldRevenge(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Barbarian.Revenge.CanCast())
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldFuriousCharge(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Barbarian.FuriousCharge.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(60f) && !IsNoPrimary)
                return false;            

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        // Tactics

        protected virtual bool ShouldBattleRage()
        {
            if (!Skills.Barbarian.BattleRage.CanCast())
                return false;

            if (Skills.Barbarian.BattleRage.IsBuffActive)
                return false;

            return true;
        }

        protected virtual bool ShouldThreateningShout(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Barbarian.ThreateningShout.CanCast())
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldWarCry()
        {
            if (!Skills.Barbarian.WarCry.CanCast())
                return false;

            if (Player.PrimaryResourcePct < 0.5f)
                return true;

            if (Skills.Barbarian.WarCry.IsBuffActive)
                return false;

            return true;
        }

        protected virtual bool ShouldAvalanche(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Barbarian.Avalanche.CanCast())
                return false;

            if (!TargetUtil.ClusterExists(15f, 50f, 5) && !TargetUtil.AnyElitesInRange(50f))
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        // Rage

        protected virtual bool ShouldEarthquake(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Barbarian.Earthquake.CanCast())
                return false;

            if (Sets.MightOfTheEarth.IsFullyEquipped)
                return true;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldCallOfTheAncients()
        {
            if (!Skills.Barbarian.CallOfTheAncients.CanCast())
                return false;

            if (Core.Player.Summons.AncientCount < 3)
            {
                if (Sets.ImmortalKingsCall.IsFirstBonusActive)
                    return true;

                if (TargetUtil.AnyMobsInRange(30f))
                    return true;
            }

            return false;
        }

        protected virtual bool ShouldWrathOfTheBerserker()
        {
            if (!Skills.Barbarian.WrathOfTheBerserker.CanCast())
                return false;

            if (Skills.Barbarian.WrathOfTheBerserker.IsBuffActive)
                return false;

            return true;
        }

        #endregion

        #region Expressions 

        // Primary

        protected static TrinityPower Bash(TrinityActor target)
            => new TrinityPower(Skills.Barbarian.Bash, 7f, target.AcdId);

        protected static TrinityPower Cleave(TrinityActor target)
            => new TrinityPower(Skills.Barbarian.Cleave, 7f, target.AcdId);

        protected static TrinityPower Frenzy(TrinityActor target)
            => new TrinityPower(Skills.Barbarian.Frenzy, 7f, target.AcdId);

        protected static TrinityPower WeaponThrow(TrinityActor target)
             => new TrinityPower(Skills.Barbarian.WeaponThrow, 60f, target.AcdId);

        //Secondary

        protected static TrinityPower HammerOfTheAncients(TrinityActor target)
            => new TrinityPower(Skills.Barbarian.HammerOfTheAncients, 15f, target.AcdId);

        protected static TrinityPower SeismicSlam(TrinityActor target)
            => new TrinityPower(Skills.Barbarian.SeismicSlam, 20f, target.AcdId);

        protected static TrinityPower Rend()
            => new TrinityPower(Skills.Barbarian.Rend);

        protected static TrinityPower Rend(Vector3 position)
            => new TrinityPower(Skills.Barbarian.Rend, 10f, position);

        protected static TrinityPower Whirlwind()
            => new TrinityPower(Skills.Barbarian.Whirlwind);

        protected static TrinityPower Whirlwind(Vector3 position)
            => new TrinityPower(Skills.Barbarian.Whirlwind, 9999f, position);

        protected static TrinityPower AncientSpear(TrinityActor target)
            => new TrinityPower(Skills.Barbarian.AncientSpear, 70f, target.AcdId);

        // Defensive

        protected static TrinityPower GroundStomp()
            => new TrinityPower(Skills.Barbarian.GroundStomp);

        protected static TrinityPower GroundStomp(Vector3 position)
            => new TrinityPower(Skills.Barbarian.GroundStomp, 10f, position);

        protected static TrinityPower IgnorePain()
            => new TrinityPower(Skills.Barbarian.IgnorePain);

        protected static TrinityPower Leap(Vector3 position)
            => new TrinityPower(Skills.Barbarian.Leap, 45f, position);

        protected static TrinityPower Sprint()
            => new TrinityPower(Skills.Barbarian.Sprint);

        // Might

        protected static TrinityPower Overpower()
            => new TrinityPower(Skills.Barbarian.Overpower);

        protected static TrinityPower Overpower(Vector3 position)
            => new TrinityPower(Skills.Barbarian.Overpower, 10f, position);

        protected static TrinityPower FuriousCharge(Vector3 position)
            => new TrinityPower(Skills.Barbarian.FuriousCharge, 60f, position);

        protected static TrinityPower Revenge()
            => new TrinityPower(Skills.Barbarian.Revenge);

        protected static TrinityPower Revenge(Vector3 position)
            => new TrinityPower(Skills.Barbarian.Revenge, 10f, position);

        // Tactics

        protected static TrinityPower BattleRage()
            => new TrinityPower(Skills.Barbarian.BattleRage);

        protected static TrinityPower ThreateningShout()
            => new TrinityPower(Skills.Barbarian.ThreateningShout);

        protected static TrinityPower ThreateningShout(Vector3 position)
            => new TrinityPower(Skills.Barbarian.ThreateningShout, 12f, position);

        protected static TrinityPower WarCry()
            => new TrinityPower(Skills.Barbarian.WarCry);

        protected static TrinityPower Avalanche()
            => new TrinityPower(Skills.Barbarian.Avalanche);

        protected static TrinityPower Avalanche(Vector3 position)
            => new TrinityPower(Skills.Barbarian.Avalanche, 60f, position);

        // Rage

        protected static TrinityPower Earthquake()
            => new TrinityPower(Skills.Barbarian.Earthquake);

        protected static TrinityPower Earthquake(Vector3 position)
            => new TrinityPower(Skills.Barbarian.Earthquake, 12f, position);

        protected static TrinityPower CallOfTheAncients()
            => new TrinityPower(Skills.Barbarian.CallOfTheAncients);

        protected static TrinityPower WrathOfTheBerserker()
            => new TrinityPower(Skills.Barbarian.WrathOfTheBerserker);

        // Misc

        protected static int RaekorDamageStacks 
            => Core.Buffs.GetBuffStacks(SNOPower.P2_ItemPassive_Unique_Ring_026);

       
        #endregion

        #region Helpers

        protected bool TryPrimaryPower(out TrinityPower power)
        {
            TrinityActor target;
            power = null;

            if (ShouldBash(out target))
                power = Bash(target);

            else if (ShouldCleave(out target))
                power = Cleave(target);

            else if (ShouldFrenzy(out target))
                power = Frenzy(target);

            else if (ShouldWeaponThrow(out target))
                power = WeaponThrow(target);

            return power != null;
        }

        protected bool TrySecondaryPower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldRend(out position))
                power = Rend(position);

            else if (ShouldAncientSpear(out target))
                power = AncientSpear(target);

            else if (ShouldWhirlwind(out position))
                power = Whirlwind(position);

            else if(ShouldHammerOfTheAncients(out target))
                power = HammerOfTheAncients(target);

            else if (ShouldSeismicSlam(out target))
                power = SeismicSlam(target);

            else if (ShouldFuriousCharge(out position))
                power = FuriousCharge(position);

            return power != null;
        }

        protected bool TrySpecialPower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldLeap(out position))
                power = Leap(position);

            else if (ShouldGroundStomp(out position))
                power = GroundStomp(position);

            else if (ShouldOverpower(out position))
                power = Overpower(position);

            else if (ShouldRevenge(out position))
                power = Revenge();

            else if (ShouldThreateningShout(out position))
                power = ThreateningShout(position);

            else if (ShouldAvalanche(out position))
                power = Avalanche(position);

            else if (ShouldEarthquake(out position))
                power = Earthquake(position);

            return power != null;
        }

        protected bool TryBuffPower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldIgnorePain())
                power = IgnorePain();

            else if (ShouldSprint())
                power = Sprint();

            else if (ShouldBattleRage())
                power = BattleRage();

            else if (ShouldWarCry())
                power = WarCry();

            else if (ShouldCallOfTheAncients())
                power = CallOfTheAncients();

            else if (ShouldWrathOfTheBerserker())
                power = WrathOfTheBerserker();

            return power != null;
        }

        public TrinityPower DefaultBuffPower()
        {
            TrinityPower power;
            return TryBuffPower(out power) ? power : null;
        }

        public TrinityPower DefaultDestructiblePower()
        {
            if (Skills.Barbarian.Bash.CanCast())
                return Bash(CurrentTarget);

            if (Skills.Barbarian.Cleave.CanCast())
                return Cleave(CurrentTarget);

            if (Skills.Barbarian.Frenzy.CanCast())
                return Frenzy(CurrentTarget);

            if (Skills.Barbarian.WeaponThrow.CanCast())
                return WeaponThrow(CurrentTarget);

            if (Skills.Barbarian.FuriousCharge.CanCast())
                return FuriousCharge(CurrentTarget.Position);

            if (Skills.Barbarian.HammerOfTheAncients.CanCast())
                return HammerOfTheAncients(CurrentTarget);

            if (Skills.Barbarian.SeismicSlam.CanCast())
                return SeismicSlam(CurrentTarget);

            if (Skills.Barbarian.Whirlwind.CanCast())
                return Whirlwind(CurrentTarget.Position);

            return DefaultPower;
        }

        public bool CanChargeTo(Vector3 destination)
        {
            var destinationDistance = Player.Position.Distance(destination);
            if (destinationDistance < 10f)
                return false;

            if (!Skills.Barbarian.FuriousCharge.CanCast())
                return false;

            if (CurrentTarget?.Type == TrinityObjectType.Item && destinationDistance < 20f)
                return false;

            if (Math.Abs(destination.Z - Core.Player.Position.Z) > 5)
                return false;

            return true;
        }

        #endregion

    }

}
