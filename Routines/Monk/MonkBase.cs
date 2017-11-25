using System;
using Trinity.Framework;
using System.Linq;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


namespace Trinity.Routines.Monk
{
    public class MonkBase : RoutineBase
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
        // Core.Logger.Log(LogCategory.Routine, $"My Current Target is {CurrentTarget}");

        #region IRoutine Defaults

        public virtual ActorClass Class => ActorClass.Monk;
        public virtual int PrimaryEnergyReserve => 50;
        public virtual int SecondaryEnergyReserve => 0;
        public virtual KiteMode KiteMode => KiteMode.Never;
        public virtual float KiteDistance => 0;
        public virtual int KiteStutterDuration => 500;
        public virtual int KiteStutterDelay => 1000;
        public virtual int KiteHealthPct => 100;
        public virtual float TrashRange => 75f;
        public virtual float EliteRange => 120f;
        public virtual float HealthGlobeRange => 60f;
        public virtual float ShrineRange => 80f;
        public virtual Func<bool> ShouldIgnoreNonUnits { get; } = () => false;
        public virtual Func<bool> ShouldIgnorePackSize { get; } = () => false;
        public virtual Func<bool> ShouldIgnoreAvoidance { get; } = () => false;
        public virtual Func<bool> ShouldIgnoreKiting { get; } = () => false;
        public virtual Func<bool> ShouldIgnoreFollowing { get; } = () => false;

        #endregion        

        #region Conditions

        protected virtual bool ShouldFistsOfThunder(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.FistsOfThunder.CanCast())
                return false;

            if (IsMultiPrimary && !HasCycled(Skills.Monk.FistsOfThunder))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldDeadlyReach(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.DeadlyReach.CanCast())
                return false;

            if (IsMultiPrimary && !HasCycled(Skills.Monk.DeadlyReach))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldCripplingWave(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.CripplingWave.CanCast())
                return false;

            if (IsMultiPrimary && !HasCycled(Skills.Monk.CripplingWave))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldWayOfTheHundredFists(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.WayOfTheHundredFists.CanCast())
                return false;

            if (IsMultiPrimary && !HasCycled(Skills.Monk.WayOfTheHundredFists))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldSevenSidedStrike(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.SevenSidedStrike.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(45f))
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (Skills.Monk.ExplodingPalm.IsActive && !WeightedUnits.Any(u => u.IsUnit && u.Distance < 35f && u.HasDebuff(SNOPower.Monk_ExplodingPalm)))
                return false;

            var isElitesInRange = TargetUtil.AnyElitesInRange(15, 1);
            var isSpamRelatedItems = Legendary.Madstone.IsEquipped || Sets.UlianasStratagem.IsMaxBonusActive;
            var isLowHealth = Player.CurrentHealthPct < 0.55f;
            var isLargerCluster = TargetUtil.ClusterExists(20f, 20f, LargeClusterSize);

            if (isElitesInRange || isSpamRelatedItems || isLowHealth || isLargerCluster)
            {
                target = TargetUtil.GetBestClusterUnit();
                return target != null;
            }

            return false;
        }

        protected virtual bool ShouldLashingTailKick(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.LashingTailKick.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (!TargetUtil.AnyMobsInRange(60f))
                return false;

            target = TargetUtil.GetBestClusterUnit();
            return target != null;
        }

        protected virtual bool ShouldWaveOfLight(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.WaveOfLight.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            var isBigCluster = TargetUtil.ClusterExists(WaveOfLightRange, 3);
            var isEliteInRange = TargetUtil.AnyElitesInRange(WaveOfLightRange);
            var isFarTooMuchResource = Player.PrimaryResourcePct > 0.8f;

            if (isBigCluster || isEliteInRange || isFarTooMuchResource)
            {
                target = TargetUtil.GetBestClusterUnit();
                return target != null;
            }

            return false;
        }

        protected virtual bool ShouldWaveOfLight(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Monk.WaveOfLight.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            var isBigCluster = TargetUtil.ClusterExists(WaveOfLightRange, 3);
            var isEliteInRange = TargetUtil.AnyElitesInRange(WaveOfLightRange);
            var isFarTooMuchResource = Player.PrimaryResourcePct > 0.8f;

            if (isBigCluster || isEliteInRange || isFarTooMuchResource)
            {
                position = TargetUtil.GetBestClusterUnit().Position;
                return position != null;
            }

            return false;
        }

        protected virtual bool ShouldExplodingPalm(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.ExplodingPalm.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            var isBigCluster = TargetUtil.ClusterExists(WaveOfLightRange, 3);
            var isEliteInRange = TargetUtil.AnyElitesInRange(WaveOfLightRange);
            var isFarTooMuchResource = Player.PrimaryResourcePct > 0.8f;

            if (isBigCluster || isEliteInRange || isFarTooMuchResource)
            {
                target = TargetUtil.BestExplodingPalmTarget(MeleeAttackRange);
                return target != null;
            }

            return false;
        }

        protected virtual bool ShouldTempestRush(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Monk.TempestRush.CanCast())
                return false;

            position = TargetUtil.GetBestPiercePoint(50f);    
                       
            return position != Vector3.Zero;       
        }

        protected virtual bool ShouldDashingStrike(out Vector3 position)
        {
            // Note: This is for casting while not moving.  
            // Routine GetMovermentPower() may cast for movement
            // (Its called directly by our IPlayerMover)

            position = Vector3.Zero;

            if (!Skills.Monk.DashingStrike.CanCast())
                return false;

            if (Skills.Monk.DashingStrike.TimeSinceUse < 750)
                return false;

            var raiment = Sets.ThousandStorms.IsSecondBonusActive && Skills.Monk.DashingStrike.Charges > 1 && Core.Player.PrimaryResource >= 75;
            var nonRaiment = !Sets.ThousandStorms.IsSecondBonusActive && TargetUtil.AnyMobsInRange(60f);

            if (raiment || nonRaiment || Core.Buffs.HasCastingShrine)
            {
                position = TargetUtil.GetBestPiercePoint(40f);
                return position != Vector3.Zero;
            }
            
            return false;
        }

        protected virtual bool ShouldBlindingFlash()
        {
            if (!Skills.Monk.BlindingFlash.CanCast())
                return false;

            if (HasInstantCooldowns && !Skills.Monk.Epiphany.IsLastUsed)
                return true;

            var lowHealth = Player.CurrentHealthPct <= 0.4;
            var enoughStuffToBlind = TargetUtil.AnyElitesInRange(15, 1) || TargetUtil.AnyMobsInRange(15, 3);
            var blindCurrentTarget = CurrentTarget != null && CurrentTarget.IsElite && CurrentTarget.RadiusDistance <= 15f;

            return lowHealth || enoughStuffToBlind || blindCurrentTarget;
        }

        protected virtual bool ShouldBreathOfHeaven()
        {
            if (!Skills.Monk.BreathOfHeaven.CanCast())
                return false;

            if (Core.Buffs.HasBuff(SNOPower.Monk_BreathOfHeaven))
                return false;

            if (!TargetUtil.AnyMobsInRange(20))
                return false;

            var needSpiritBuff = Runes.Monk.InfusedWithLight.IsActive && Player.PrimaryResourcePct < 0.5f;
            var needDamage = Runes.Monk.BlazingWrath.IsActive && TargetUtil.AnyMobsInRange(20);
            var needHealing = Player.CurrentHealthPct <= 0.6f;

            return needSpiritBuff || needHealing || needDamage;
        }

        protected virtual bool ShouldSerenity()
        {
            if (!Skills.Monk.Serenity.CanCast())
                return false;

            if (Core.Buffs.HasBuff(SNOPower.Monk_Serenity))
                return false;

            var needMoveUnhindered = Runes.Monk.InstantKarma.IsActive && PlayerMover.IsBlocked && Core.Player.IsMoving && Player.CurrentHealthPct > 0.8f;
            if (needMoveUnhindered)
                return true;
            
            if (Player.CurrentHealthPct > 0.5f)
                return false;

            return true;
        }

        protected virtual bool ShouldCycloneStrike()
        {
            var skill = Skills.Monk.CycloneStrike;
            if (!skill.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (skill.TimeSinceUse < (skill.DistanceFromLastUsePosition < 20f ? 4000 : 2000))
                return false;

            var targetIsCloseElite = CurrentTarget.IsElite && CurrentTarget.Distance < CycloneStrikeRange;
            var plentyOfTargetsToPull = TargetUtil.IsPercentUnitsWithinBand(15f, CycloneStrikeRange, 0.25);

            return targetIsCloseElite || plentyOfTargetsToPull;
        }

        protected virtual bool ShouldMantraOfSalvation()
        {
            if (!Skills.Monk.MantraOfSalvation.CanCast())
                return false;

            if (Core.Buffs.HasBuff(SNOPower.X1_Monk_MantraOfEvasion_v2_Passive, 1))
                return false;

            if (Player.CurrentHealthPct > 0.7f)
                return false;

            return true;
        }

        protected virtual bool ShouldMantraOfRetribution()
        {
            if (!Skills.Monk.MantraOfRetribution.CanCast())
                return false;

            if (Core.Buffs.HasBuff(SNOPower.X1_Monk_MantraOfRetribution_v2_Passive, 1))
                return false;

            if (Player.PrimaryResourcePct < 0.5f)
                return false;

            return true;
        }

        protected virtual bool ShouldMantraOfHealing()
        {
            if (!Skills.Monk.MantraOfHealing.CanCast())
                return false;

            if (Core.Buffs.HasBuff(SNOPower.X1_Monk_MantraOfHealing_v2_Passive, 1))
                return false;

            if (Player.CurrentHealthPct > 0.6f)
                return false;

            return true;
        }

        protected virtual bool ShouldMantraOfConviction()
        {
            if (!Skills.Monk.MantraOfConviction.CanCast())
                return false;

            if (Core.Buffs.HasBuff(SNOPower.X1_Monk_MantraOfConviction_v2_Passive, 2))
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (CurrentTarget != null && CurrentTarget.IsElite)
                return true;            

            if (TargetUtil.ClusterExists(15f, 30f, 3))
                return true;

            return false;
        }

        protected virtual bool ShouldSweepingWind()
        {
            if (!Skills.Monk.SweepingWind.CanCast())
                return false;

            var buffCooldownRemanining = Core.Cooldowns.GetBuffCooldownRemaining(SNOPower.Monk_SweepingWind);
            if (buffCooldownRemanining.TotalMilliseconds > 750)
                return false;

            if (!WeightedUnits.Any(u => u.Distance < 100f))
                return false;

            return true;
        }

        protected virtual bool ShouldInnerSanctuary()
        {
            if (!Skills.Monk.InnerSanctuary.CanCast())
                return false;

            if (HasInstantCooldowns && !Skills.Monk.InnerSanctuary.IsLastUsed)
                return true;

            if (Core.Buffs.HasBuff(SNOPower.X1_Monk_InnerSanctuary))
                return false;

            if (Player.CurrentHealthPct > 0.9 && !TargetUtil.AnyElitesInRange(10f))
                return false;

            if (TargetUtil.BestAoeUnit(25, Player.IsInParty).Distance > 20)
                return false;

            return true;
        }

        protected virtual bool ShouldEpiphany()
        {
            if (!Skills.Monk.Epiphany.CanCast())
                return false;

            if (HasInstantCooldowns && !Skills.Monk.Epiphany.IsLastUsed)
                return true;

            if (Player.CurrentHealthPct > 0.9 && !TargetUtil.AnyElitesInRange(30f))
                return false;

            if (TargetUtil.BestAoeUnit(25, Player.IsInParty)?.Distance > 30 && !TargetUtil.AnyElitesInRange(20))
                return false;

            return true;
        }

        protected virtual bool ShouldMysticAlly()
        {
            if (!Skills.Monk.MysticAlly.CanCast())
                return false;

            if (Core.Buffs.HasBuff(SNOPower.X1_Monk_MysticAlly_v2))
                return false;

            if (Runes.Monk.AirAlly.IsActive && Player.PrimaryResource > 150)
                return false;

            if (!TargetUtil.AnyMobsInRange(30f))
                return false;

            return true;
        }

        #endregion

        #region Expressions 

        protected static bool ShouldRefreshSpiritGuardsBuff
            => Legendary.SpiritGuards.IsEquipped && (SpellHistory.TimeSinceGeneratorCast >= 2500 || !HasSpiritGuardsBuff);

        protected bool HasCycled(Skill s) 
            => s.TimeSinceUse > 250 && !s.IsLastUsed;

        protected static float WaveOfLightRange 
            => Legendary.TzoKrinsGaze.IsEquipped ? 55f : 16f;

        protected static int MaxDashingStrikeCharges 
            => Runes.Monk.Quicksilver.IsActive ? 3 : 2;

        protected static float MeleeAttackRange 
            => IsEpiphanyActive ? 50f : 8f;

        protected static float CycloneStrikeRange 
            => Runes.Monk.Implosion.IsActive ? 34f : 24f;

        protected static float CycloneStrikeSpirit 
            => Runes.Monk.EyeOfTheStorm.IsActive ? 30 : 50;

        protected static int MaxSweepingWindStacks 
            => Legendary.VengefulWind.IsEquipped ? 10 : 3;

        protected static bool HasShenLongBuff
            => Core.Buffs.HasBuff(SNOPower.P3_ItemPassive_Unique_Ring_026, 1);

        protected static bool HasRaimentDashBuff
            => Core.Buffs.HasBuff(SNOPower.P2_ItemPassive_Unique_Ring_033, 2);

        protected static bool HasSpiritGuardsBuff
            => Core.Buffs.HasBuff(SNOPower.P2_ItemPassive_Unique_Ring_034, 1);

        protected static bool IsEpiphanyActive
            => Core.Buffs.HasBuff(SNOPower.X1_Monk_Epiphany);

        protected virtual TrinityPower FistsOfThunder(TrinityActor target)
            => new TrinityPower(SNOPower.Monk_FistsofThunder, MeleeAttackRange, target.AcdId);

        protected virtual TrinityPower DeadlyReach(TrinityActor target)
            => new TrinityPower(SNOPower.Monk_DeadlyReach, MeleeAttackRange, target.AcdId);

        protected virtual TrinityPower CripplingWave(TrinityActor target)
            => new TrinityPower(SNOPower.Monk_CripplingWave, MeleeAttackRange, target.AcdId);

        protected virtual TrinityPower WayOfTheHundredFists(TrinityActor target)
            => new TrinityPower(SNOPower.Monk_WayOfTheHundredFists, MeleeAttackRange, target.AcdId);

        protected virtual TrinityPower SevenSidedStrike(TrinityActor target)
            => new TrinityPower(SNOPower.Monk_SevenSidedStrike, 16f, target.AcdId) { CastWhenBlocked = true, };

        protected virtual TrinityPower LashingTailKick(TrinityActor target)
            => new TrinityPower(SNOPower.Monk_LashingTailKick, MeleeAttackRange, target.AcdId);

        protected virtual TrinityPower WaveOfLight(TrinityActor target)
            => new TrinityPower(SNOPower.Monk_WaveOfLight, 60f, target);

        protected virtual TrinityPower WaveOfLight(Vector3 position)
            => new TrinityPower(SNOPower.Monk_WaveOfLight, 60f, position);

        protected virtual TrinityPower ExplodingPalm(TrinityActor target)
            => new TrinityPower(SNOPower.Monk_ExplodingPalm, MeleeAttackRange, target.AcdId);

        protected virtual TrinityPower TempestRush(Vector3 position)
            => new TrinityPower(SNOPower.Monk_TempestRush, 60f, position);

        protected virtual TrinityPower DashingStrike(Vector3 position)
            => new TrinityPower(SNOPower.X1_Monk_DashingStrike, 50f, position);

        protected virtual TrinityPower BlindingFlash()
            => new TrinityPower(SNOPower.Monk_BlindingFlash);

        protected virtual TrinityPower BreathOfHeaven()
            => new TrinityPower(SNOPower.Monk_BreathOfHeaven);

        protected virtual TrinityPower Serenity()
            => new TrinityPower(SNOPower.Monk_Serenity);

        protected virtual TrinityPower CycloneStrike()
            => new TrinityPower(SNOPower.Monk_CycloneStrike);

        protected virtual TrinityPower MantraOfSalvation()
            => new TrinityPower(SNOPower.X1_Monk_MantraOfEvasion_v2);

        protected virtual TrinityPower MantraOfRetribution()
            => new TrinityPower(SNOPower.X1_Monk_MantraOfRetribution_v2);

        protected virtual TrinityPower MantraOfHealing()
            => new TrinityPower(SNOPower.X1_Monk_MantraOfHealing_v2);

        protected virtual TrinityPower MantraOfConviction()
            => new TrinityPower(SNOPower.X1_Monk_MantraOfConviction_v2);

        protected virtual TrinityPower SweepingWind()
            => new TrinityPower(SNOPower.Monk_SweepingWind);

        protected virtual TrinityPower InnerSanctuary()
            => new TrinityPower(SNOPower.X1_Monk_InnerSanctuary);

        protected virtual TrinityPower Epiphany()
            => new TrinityPower(SNOPower.X1_Monk_Epiphany);

        protected virtual TrinityPower MysticAlly()
            => new TrinityPower(SNOPower.X1_Monk_MysticAlly_v2);

        
        #endregion

        #region Helpers

        protected bool TryPrimaryPower(out TrinityPower power)
        {
            TrinityActor target;
            power = null;

            if (ShouldFistsOfThunder(out target))
                power = FistsOfThunder(target);

            else if (ShouldDeadlyReach(out target))
                power = DeadlyReach(target);

            else if (ShouldCripplingWave(out target))
                power = CripplingWave(target);

            else if (ShouldWayOfTheHundredFists(out target))
                power = WayOfTheHundredFists(target);

            return power != null;
        }

        protected bool TrySecondaryPower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldTempestRush(out position))
                power = TempestRush(position);

            else if (ShouldDashingStrike(out position))
                power = DashingStrike(position);

            else if (ShouldSevenSidedStrike(out target))
                power = SevenSidedStrike(target);

            else if (ShouldWaveOfLight(out target))
                power = WaveOfLight(target);

            else if (ShouldLashingTailKick(out target))
                power = LashingTailKick(target);

            return power != null;
        }

        protected bool TrySpecialPower(out TrinityPower power)
        {
            TrinityActor target;
            power = null;

            if (ShouldCycloneStrike())
                power = CycloneStrike();

            if (ShouldExplodingPalm(out target))
                power = ExplodingPalm(target);

            return power != null;
        }

        protected bool TryMantra(out TrinityPower power)
        {
            power = null;

            if (ShouldMantraOfConviction())
                power = MantraOfConviction();

            if (ShouldMantraOfHealing())
                power = MantraOfHealing();

            if (ShouldMantraOfRetribution())
                power = MantraOfRetribution();

            if (ShouldMantraOfSalvation())
                power = MantraOfSalvation();

            return power != null;
        }

        public bool TryMovementPower(Vector3 destination, out TrinityPower power)
        {
            power = null;

            var destinationDistance = Player.Position.Distance(destination);
            if (destinationDistance < 10f)
                return false;

            if (CurrentTarget?.Type == TrinityObjectType.Item && destinationDistance < 20f)
                return false;

            if (Math.Abs(destination.Z - Core.Player.Position.Z) > 5)
                return false;

            if (Skills.Monk.DashingStrike.CanCast())
            {
                if (Sets.ThousandStorms.IsFullyEquipped && Skills.Monk.DashingStrike.Charges < 2 && !PlayerMover.IsBlocked)
                    return false;

                power = DashingStrike(destination);
                return true;
            }
         
            return false;
        }

        public bool CanDashTo(Vector3 destination)
        {
            var destinationDistance = Player.Position.Distance(destination);
            if (destinationDistance < 5f)
                return false;

            if (!Skills.Monk.DashingStrike.CanCast())
                return false;

            if (Skills.Monk.DashingStrike.TimeSinceUse < 250)
                return false;

            if (CurrentTarget?.Type == TrinityObjectType.Item && destinationDistance < 20f)
                return false;

            if (Math.Abs(destination.Z - Core.Player.Position.Z) > 5)
                return false;

            if (Sets.ThousandStorms.IsFullyEquipped && Skills.Monk.DashingStrike.Charges < 2 && !PlayerMover.IsBlocked)
                return false;

            return true;
        }

        protected bool TryBuffPower(out TrinityPower power)
        {
            power = null;

            if (ShouldSweepingWind())
                power = SweepingWind();

            if (ShouldMantraOfConviction())
                power = MantraOfConviction();

            if (ShouldMantraOfHealing())
                power = MantraOfConviction();

            if (ShouldMantraOfRetribution())
                power = MantraOfRetribution();

            if (ShouldMantraOfSalvation())
                power = MantraOfSalvation();

            if (ShouldEpiphany())
                power = Epiphany();

            if (ShouldMysticAlly())
                power = MysticAlly();

            if (ShouldBreathOfHeaven())
                power = BreathOfHeaven();

            if (ShouldSerenity())
                power = Serenity();

            if (ShouldBlindingFlash())
                power = BlindingFlash();

            if (ShouldInnerSanctuary())
                power = InnerSanctuary();

            return power != null;
        }

        protected TrinityPower DefaultBuffPower()
        {
            TrinityPower power;
            return TryBuffPower(out power) ? power : null;
        }

        public TrinityPower DefaultDestructiblePower()
        {

            if (CurrentTarget.IsCorruptGrowth)
            {
                if (Skills.Monk.LashingTailKick.CanCast())
                    return LashingTailKick(CurrentTarget);

                if (Skills.Monk.DashingStrike.CanCast())
                    return DashingStrike(CurrentTarget.Position);

                if (Skills.Monk.SevenSidedStrike.CanCast())
                    return SevenSidedStrike(CurrentTarget);
            }


            if (Skills.Monk.FistsOfThunder.CanCast())
                return FistsOfThunder(CurrentTarget);

            if (Skills.Monk.DeadlyReach.CanCast())
                return DeadlyReach(CurrentTarget);

            if (Skills.Monk.CripplingWave.CanCast())
                return CripplingWave(CurrentTarget);

            if (Skills.Monk.WayOfTheHundredFists.CanCast())
                return WayOfTheHundredFists(CurrentTarget);

            if (Skills.Monk.TempestRush.CanCast())
                return TempestRush(CurrentTarget.Position);


            if (Skills.Monk.LashingTailKick.CanCast())
                return LashingTailKick(CurrentTarget);

            if (Skills.Monk.DashingStrike.CanCast())
                return DashingStrike(CurrentTarget.Position);

            if (Skills.Monk.SevenSidedStrike.CanCast())
                return SevenSidedStrike(CurrentTarget);

            return DefaultPower;
        }

        #endregion
    }
}

