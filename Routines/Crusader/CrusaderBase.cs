using System;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Reference;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Crusader
{
    public class CrusaderBase : RoutineBase
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

        public virtual ActorClass Class { get; } = ActorClass.Crusader;
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

        protected virtual bool ShouldPunish(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.Punish.CanCast())
                return false;

            if (Skills.Crusader.Punish.IsLastUsed && IsMultiPrimary)
                return false;

            target = CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldSlash(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.Slash.CanCast())
                return false;

            if (Skills.Crusader.Slash.IsLastUsed && IsMultiPrimary)
                return false;

            target = CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldSmite(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.Smite.CanCast())
                return false;

            if (Skills.Crusader.Smite.IsLastUsed && IsMultiPrimary)
                return false;

            target = CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldJustice(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.Justice.CanCast())
                return false;

            if (Skills.Crusader.Justice.IsLastUsed && IsMultiPrimary)
                return false;

            target = CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldJudgement()
        {
            if (!Skills.Crusader.Judgment.CanCast())
                return false;

            if (!TargetUtil.EliteOrTrashInRange(16f))
                return false;

            return true;
        }

        protected virtual bool ShouldCondemn()
        {
            if (!Skills.Crusader.Condemn.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(14f))
                return false;

            if (Legendary.FrydehrsWrath.IsEquipped && Player.PrimaryResource < 40)
                return false;

            return true;
        }

        protected virtual bool ShouldSteedCharge()
        {
            if (!Skills.Crusader.SteedCharge.CanCast())
                return false;

            return true;
        }

        protected virtual bool ShouldIronSkin()
        {
            if (!Skills.Crusader.IronSkin.CanCast())
                return false;

            if (Player.HasBuff(SNOPower.X1_Crusader_IronSkin))
                return false;

            if (!TargetUtil.AnyMobsInRange(20f))
                return false;

            return true;
        }

        protected virtual bool ShouldProvoke()
        {
            if (!Skills.Crusader.Provoke.CanCast())
                return false;

            if (Player.HasBuff(SNOPower.X1_Crusader_Provoke))
                return false;

            if (!TargetUtil.AnyMobsInRange(15f))
                return false;

            return true;
        }

        protected virtual bool ShouldConsecration()
        {
            if (!Skills.Crusader.Consecration.CanCast())
                return false;

            if (Player.HasBuff(SNOPower.X1_Crusader_Consecration))
                return false;

            if (!TargetUtil.AnyMobsInRange(20f))
                return false;

            return true;
        }

        protected virtual bool ShouldLawsOfValor()
        {
            if (!Skills.Crusader.LawsOfValor.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(25f))
                return false;

            return true;
        }

        protected virtual bool ShouldLawsOfJustice()
        {
            if (!Skills.Crusader.LawsOfJustice.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(25f))
                return false;

            return true;
        }

        protected virtual bool ShouldLawsOfHope()
        {
            if (!Skills.Crusader.LawsOfHope.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(25f))
                return false;

            return true;
        }

        protected virtual bool ShouldAkaratsChampion()
        {
            if (!Skills.Crusader.AkaratsChampion.CanCast())
                return false;

            return true;
        }

        protected virtual bool ShouldPhalanx()
        {
            if (!Skills.Crusader.Phalanx.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(30f))
                return false;

            return true;
        }

        protected virtual bool ShouldShieldBash(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.ShieldBash.CanCast())
                return false;

            if (Player.PrimaryResource <= PrimaryEnergyReserve)
                return false;

            if (!TargetUtil.AnyMobsInRange(30f))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldShieldGlare(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.ShieldGlare.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(20f))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldSweepAttack(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.SweepAttack.CanCast())
                return false;

            if (Player.PrimaryResource <= PrimaryEnergyReserve)
                return false;

            if (!TargetUtil.AnyMobsInRange(16f))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldBlessedHammer(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.BlessedHammer.CanCast())
                return false;

            if (Player.PrimaryResource <= PrimaryEnergyReserve)
                return false;

            if (!TargetUtil.AnyMobsInRange(10f))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldFallingSword(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Crusader.FallingSword.CanCast())
                return false;

            var target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            if (target != null)
            {
                position = target.Position;
                return true;
            }

            return false;
        }

        protected virtual bool ShouldFistOfTheHeavens(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.FistOfTheHeavens.CanCast())
                return false;

            if (Player.PrimaryResource <= PrimaryEnergyReserve)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldHeavensFury(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.HeavensFury.CanCast())
                return false;

            if (Runes.Crusader.FiresOfHeaven.IsActive)
            {
                if (Player.PrimaryResource <= PrimaryEnergyReserve)
                    return false;

                target = TargetUtil.GetBestPierceTarget(35f) ?? CurrentTarget;
            }
            else
            {
                target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            }

            return target != null;
        }

        protected virtual bool ShouldBombardment(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.Bombardment.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldBlessedShield(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.BlessedShield.CanCast())
                return false;

            if (Player.PrimaryResource <= PrimaryEnergyReserve)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        #endregion

        #region Expressions 

        public bool IsSteedCharging
            => GameData.SteedChargeAnimations.Contains(Player.CurrentAnimation);

        // Generator

        protected static TrinityPower Punish(TrinityActor target)
            => new TrinityPower(Skills.Crusader.Punish.SNOPower, 7f, target.AcdId);

        protected static TrinityPower Slash(TrinityActor target)
            => new TrinityPower(Skills.Crusader.Slash.SNOPower, 7f, target.AcdId);

        protected static TrinityPower Smite(TrinityActor target)
            => new TrinityPower(Skills.Crusader.Smite.SNOPower, 7f, target.AcdId);

        protected static TrinityPower Justice(TrinityActor target)
            => new TrinityPower(Skills.Crusader.Justice.SNOPower, 7f, target.AcdId);

        // Targetted

        protected static TrinityPower ShieldBash(TrinityActor target)
            => new TrinityPower(Skills.Crusader.ShieldBash.SNOPower, 30f, target.AcdId);

        protected static TrinityPower ShieldGlare(TrinityActor target)
            => new TrinityPower(Skills.Crusader.ShieldGlare.SNOPower, 30f, target.AcdId);

        protected static TrinityPower SweepAttack(TrinityActor target)
            => new TrinityPower(Skills.Crusader.SweepAttack.SNOPower, 18f, target.AcdId);

        protected static TrinityPower BlessedHammer(TrinityActor target)
            => new TrinityPower(Skills.Crusader.BlessedHammer.SNOPower, 12f, target.AcdId);

        protected static TrinityPower FallingSword(Vector3 position)
            => new TrinityPower(Skills.Crusader.FallingSword.SNOPower, 60f, position);

        protected static TrinityPower FistOfTheHeavens(TrinityActor target)
            => new TrinityPower(Skills.Crusader.FistOfTheHeavens.SNOPower, 70f, target.AcdId);

        protected static TrinityPower HeavensFury(TrinityActor target)
            => new TrinityPower(Skills.Crusader.HeavensFury.SNOPower, 70f, target.AcdId);

        protected static TrinityPower Bombardment(TrinityActor target)
            => new TrinityPower(Skills.Crusader.Bombardment.SNOPower, 70f, target.AcdId);

        protected static TrinityPower BlessedShield(TrinityActor target)
            => new TrinityPower(Skills.Crusader.BlessedShield.SNOPower, 70f, target.AcdId);

        // Self

        protected static TrinityPower Judgement()
            => new TrinityPower(Skills.Crusader.Judgment);

        protected static TrinityPower Condemn()
            => new TrinityPower(Skills.Crusader.Condemn);

        protected static TrinityPower SteedCharge()
            => new TrinityPower(Skills.Crusader.SteedCharge);

        protected static TrinityPower IronSkin()
            => new TrinityPower(Skills.Crusader.IronSkin);

        protected static TrinityPower Provoke()
            => new TrinityPower(Skills.Crusader.Provoke);

        protected static TrinityPower Consecration()
            => new TrinityPower(Skills.Crusader.Consecration);

        protected static TrinityPower LawsOfValor()
            => new TrinityPower(Skills.Crusader.LawsOfValor);

        protected static TrinityPower LawsOfJustice()
            => new TrinityPower(Skills.Crusader.LawsOfJustice);

        protected static TrinityPower LawsOfHope()
            => new TrinityPower(Skills.Crusader.LawsOfHope);

        protected static TrinityPower AkaratsChampion()
            => new TrinityPower(Skills.Crusader.AkaratsChampion);

        protected static TrinityPower Phalanx()
            => new TrinityPower(Skills.Crusader.Phalanx);

        #endregion

        #region Helpers

        protected bool TryPrimaryPower(out TrinityPower power)
        {
            TrinityActor target;
            power = null;

            if (ShouldPunish(out target))
                power = Punish(target);

            else if (ShouldSmite(out target))
                power = Smite(target);

            else if (ShouldSlash(out target))
                power = Slash(target);

            else if (ShouldJustice(out target))
                power = Justice(target);

            return power != null;
        }

        protected bool TrySecondaryPower(out TrinityPower power)
        {
            TrinityActor target;
            power = null;

            if (ShouldShieldBash(out target))
                power = ShieldBash(target);

            else if (ShouldSweepAttack(out target))
                power = SweepAttack(target);

            else if (ShouldFistOfTheHeavens(out target))
                power = FistOfTheHeavens(target);

            else if (ShouldBlessedShield(out target))
                power = BlessedShield(target);

            else if (ShouldBlessedHammer(out target))
                power = BlessedHammer(target);

            return power != null;
        }

        protected bool TrySpecialPower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldShieldGlare(out target))
                power = ShieldGlare(target);

            else if (ShouldBombardment(out target))
                power = Bombardment(target);

            else if (ShouldHeavensFury(out target))
                power = HeavensFury(target);

            else if (ShouldJudgement())
                power = Judgement();

            else if (ShouldPhalanx())
                power = Phalanx();

            else if (ShouldFallingSword(out position))
                power = FallingSword(position);

            else if (ShouldCondemn())
                power = Condemn();

            return power != null;
        }

        protected bool TryLaw(out TrinityPower power)
        {
            power = null;

            if (ShouldLawsOfJustice())
                power = LawsOfJustice();

            else if (ShouldLawsOfHope())
                power = LawsOfHope();

            else if (ShouldLawsOfValor())
                power = LawsOfValor();

            return power != null;
        }

        public TrinityPower DefaultDestructiblePower()
        {
            if (Skills.Crusader.Smite.CanCast())
                return Smite(CurrentTarget);

            if (Skills.Crusader.Punish.CanCast())
                return Punish(CurrentTarget);

            if (Skills.Crusader.Slash.CanCast())
                return Slash(CurrentTarget);

            if (Skills.Crusader.Justice.CanCast())
                return Justice(CurrentTarget);

            if (Skills.Crusader.ShieldBash.CanCast())
                return ShieldBash(CurrentTarget);

            if (Skills.Crusader.BlessedHammer.CanCast())
                return BlessedHammer(CurrentTarget);

            if (Skills.Crusader.BlessedShield.CanCast())
                return BlessedShield(CurrentTarget);

            if (Skills.Crusader.FistOfTheHeavens.CanCast())
                return FistOfTheHeavens(CurrentTarget);

            return DefaultPower;
        }

        #endregion

    }

}
