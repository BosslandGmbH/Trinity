using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Abilities;
using Trinity.Components.Combat.Abilities.PhelonsPlayground;
using Trinity.Config;
using Trinity.Config.Combat;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Modules;
using Trinity.Framework.Objects.Attributes;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Reference;
using Trinity.Technicals;
using Trinity.UIComponents;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Zeta.Common.Logger;

namespace Trinity.Routines.DemonHunter
{
    public class DemonHunterDefault : RoutineBase, IRoutine
    {
        #region Definition

        public ActorClass Class => ActorClass.DemonHunter;
        public string Author => "XZJV";
        public string DisplayName => "DemonHunter Generic Routine";
        public string Description => "Generic class support, casts all spells whenever possible";
        public Build RequiredBuild => null;
        public bool IsEquipped => true;
		public KiteMode KiteMode => KiteMode.Always;
        public float KiteDistance => 15f;

        #endregion

        public double EnergyReserve { get; set; }

        public virtual TrinityPower GetCombatPower()
        {
            TrinityActor target;
            Vector3 position;

            if (ShouldRainOfVengeance(out target))
                return RainOfVengeance(target);

            if (ShouldSentry(out position))
                return Sentry(position);

            if (ShouldCaltrops())
                return Caltrops();

            if (ShouldMarkedForDeath(out target))
                return MarkedForDeath(target);

            if (ShouldVault(out position))
                return Vault(position);

            if (ShouldFanOfKnives())
                return FanOfKnives();

            if (ShouldMultishot(out target))
                return Multishot(target);

            if (ShouldStrafe(out position))
                return Strafe(position);

            if (ShouldSpikeTrap())
                return SpikeTrap();

            if (ShouldElementalArrow(out target))
                return ElementalArrow(target);

            if (ShouldClusterArrow(out target))
                return ClusterArrow(target);

            if (ShouldRapidFire(out target))
                return RapidFire(target);

            if (ShouldImpale(out target))
                return Impale(target);

            if (ShouldChakram(out target))
                return Chakram(target);

            if (ShouldEvasiveFire(out target))
                return EvasiveFire(target);

            if (ShouldHungeringArrow(out target))
                return HungeringArrow(target);

            if (ShouldEntanglingShot(out target))
                return EntanglingShot(target);

            if (ShouldBolas(out target))
                return Bolas(target);

            if (ShouldGrenade(out target))
                return Grenade(target);

            return null;
        }


        public virtual TrinityPower GetDefensivePower()
        {
            if (ShouldCaltrops())
                return Caltrops();

            if (ShouldShadowPower())
                return ShadowPower();

            return null;
        }

        public virtual TrinityPower GetBuffPower()
        {
            if (ShouldVengeance())
                return Vengeance();

            if (ShouldShadowPower())
                return ShadowPower();

            if (ShouldSmokeScreen())
                return SmokeScreen();

            if (ShouldPreparation())
                return Preparation();

            if (ShouldCompanion())
                return Companion();

            return null;
        }

        public virtual TrinityPower GetDestructiblePower()
        {			
            if (Skills.DemonHunter.HungeringArrow.CanCast())
                return Skills.DemonHunter.HungeringArrow.ToPower();

            if (Skills.DemonHunter.EntanglingShot.CanCast())
                return Skills.DemonHunter.EntanglingShot.ToPower();

            if (Skills.DemonHunter.Bolas.CanCast())
                return Skills.DemonHunter.Bolas.ToPower();

            if (Skills.DemonHunter.Grenade.CanCast())
                return Skills.DemonHunter.Grenade.ToPower();

            return CombatBase.DefaultPower;
        }

        public virtual TrinityPower GetMovementPower(Vector3 destination)
        {
            Vector3 position;

            if (ShouldVault(out position))
                Vault(position);

            return null;
        }

        public virtual async Task<bool> OnHandleTarget()
        {
            return false;
        }

        #region Conditions

        protected bool ShouldRainOfVengeance(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.RainOfVengeance.CanCast())
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool ShouldSentry(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.DemonHunter.Sentry.CanCast())
                return false;

            if (Skills.DemonHunter.Sentry.Charges == 0)
                return false;

            if (!TargetUtil.AnyMobsInRange(65f))
                return false;

            if (Player.Summons.DHSentryCount >= MaxSentryCount)
                return false;

            position = TargetUtil.BestSentryPosition();
            return position != Vector3.Zero;
        }

        protected bool ShouldCaltrops()
        {
            if (!Skills.DemonHunter.Caltrops.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(40f))
                return false;

            if (!Player.HasBuff(SNOPower.DemonHunter_Caltrops))
                return false;

            return true;
        }

        protected bool ShouldMarkedForDeath(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.MarkedForDeath.CanCast())
                return false;

            if (CurrentTarget.HasDebuff(SNOPower.DemonHunter_MarkedForDeath))
                return false;

            if (!SpellTracker.IsUnitTracked(CurrentTarget, SNOPower.DemonHunter_MarkedForDeath))
                return true;

            target = CurrentTarget;
            return target != null;
        }

        protected bool ShouldVault(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.DemonHunter.Vault.CanCast())
                return false;

            if (CombatBase.KiteMode == KiteMode.Never)
                return false;

            if (TargetUtil.AnyMobsInRange(Core.Avoidance.Settings.KiteDistance))
                return false;

            if (Skills.DemonHunter.Vault.TimeSinceUse < 1000)
                return false;

            if (CurrentTarget?.Distance <= CombatBase.KiteDistance && Player.CurrentHealthPct > 0.85 && !CombatBase.IsCurrentlyAvoiding)
                return false;

            position = Core.Avoidance.Avoider.SafeSpot;
            return position != Vector3.Zero;
        }

        protected bool ShouldFanOfKnives()
        {
            if (!Skills.DemonHunter.FanOfKnives.CanCast())
                return false;

            if (TargetUtil.NumMobsInRange() > 3 || TargetUtil.AnyElitesInRange(15f))
                return false;

            return true;
        }

        protected bool ShouldMultishot(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.Multishot.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(60f))
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool ShouldStrafe(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.DemonHunter.Strafe.CanCast())
                return false;

            position = TargetUtil.GetZigZagTarget(CurrentTarget.Position, 20f, true);
            return position != Vector3.Zero;
        }

        protected bool ShouldVengeance()
        {
            if (!Skills.DemonHunter.Vengeance.CanCast())
                return false;

            if (Legendary.Dawn.IsEquipped)
                return true;

            if (!TargetUtil.IsEliteTargetInRange(70f) && Player.CurrentHealthPct > 0.5f)
                return false;

            return true;
        }

        protected bool ShouldShadowPower()
        {
            if (!Skills.DemonHunter.ShadowPower.CanCast())
                return false;

            if (Player.HasBuff(SNOPower.DemonHunter_ShadowPower))
                return false;

            if (Skills.DemonHunter.ShadowPower.TimeSinceUse < 4500)
                return false;

            if (TargetUtil.AnyElitesInRange(40f))
                return true;

            if (Player.CurrentHealthPct < 0.5f)
                return true;

            if (TargetUtil.NumMobsInRange() > 5)
                return true;

            return false;
        }

        protected bool ShouldSpikeTrap()
        {
            if (!Skills.DemonHunter.Vengeance.CanCast())
                return false;

            if (CombatBase.LastPowerUsed == SNOPower.DemonHunter_SpikeTrap)
                return false;

            return true;
        }

        protected bool ShouldSmokeScreen()
        {
            if (!Skills.DemonHunter.SmokeScreen.CanCast())
                return false;

            if (Player.HasBuff(SNOPower.DemonHunter_ShadowPower))
                return false;

            if ((Player.CurrentHealthPct <= 0.50 || Player.IsRooted || Player.IsIncapacitated))
                return true;

            if (TargetUtil.AnyMobsInRange(15))
                return true;

            return true;
        }

        protected bool ShouldPreparation()
        {
            if (!Skills.DemonHunter.Preparation.CanCast())
                return false;

            if (!Runes.DemonHunter.Punishment.IsActive && Player.SecondaryResourcePct <= 0.5f)
                return true;

            if (Runes.DemonHunter.Punishment.IsActive && Player.PrimaryResourcePct <= 0.3f)
                return true;

            return false;
        }

        protected bool ShouldCompanion()
        {
            if (!Skills.DemonHunter.Preparation.CanCast())
                return false;

            // Use Spider Slow on 4 or more trash mobs in an area or on Unique/Elite/Champion
            if (Runes.DemonHunter.SpiderCompanion.IsActive && TargetUtil.ClusterExists(25f, 4) && TargetUtil.EliteOrTrashInRange(25f))
                return true;

            // Use Bat when Hatred is Needed
            if (Runes.DemonHunter.BatCompanion.IsActive && Player.PrimaryResourceMissing >= 60)
                return true;

            // Use Boar Taunt on 3 or more trash mobs in an area or on Unique/Elite/Champion
            if (Runes.DemonHunter.BoarCompanion.IsActive && ((TargetUtil.ClusterExists(20f, 4) && TargetUtil.EliteOrTrashInRange(20f)) || (CurrentTarget.IsElite && CurrentTarget.Distance <= 20f)))
                return true;

            // Ferrets used for picking up Health Globes when low on Health
            if (Runes.DemonHunter.FerretCompanion.IsActive && Core.Targets.Items.Any(o => o.Type == TrinityObjectType.HealthGlobe && o.Distance < 60f) && Player.CurrentHealthPct < CombatBase.EmergencyHealthPotionLimit)
                return true;

            // Use Wolf Howl on Unique/Elite/Champion
            if (Runes.DemonHunter.WolfCompanion.IsActive && (TargetUtil.AnyElitesInRange(80f) || TargetUtil.AnyMobsInRange(40, 5)))
                return true;

            return false;
        }

        protected bool ShouldElementalArrow(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.ElementalArrow.CanCast())
                return false;

            if (Player.PrimaryResource < EnergyReserve && !Legendary.Kridershot.IsEquipped)
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool ShouldClusterArrow(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.ClusterArrow.CanCast())
                return false;

            if (Sets.NatalyasVengeance.IsFullyEquipped && Player.PrimaryResource < 100 && !Player.HasBuff(SNOPower.P2_ItemPassive_Unique_Ring_053))
                return false;

            if (Player.PrimaryResource < EnergyReserve)
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool ShouldChakram(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.Chakram.CanCast())
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool ShouldImpale(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.Impale.CanCast())
                return false;

            if (Player.PrimaryResource <= EnergyReserve || Player.PrimaryResource <= 20)
                return false;

            if (CurrentTarget.RadiusDistance < 50f)
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool ShouldRapidFire(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.RapidFire.CanCast())
                return false;

            if (Player.PrimaryResource < EnergyReserve)
                return false;

            if (SpellHistory.LastPowerUsed == SNOPower.DemonHunter_RapidFire)
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool ShouldEvasiveFire(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.EvasiveFire.CanCast())
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool ShouldHungeringArrow(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.HungeringArrow.CanCast())
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool ShouldEntanglingShot(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.EntanglingShot.CanCast())
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool ShouldBolas(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.Bolas.CanCast())
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool ShouldGrenade(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.Grenade.CanCast())
                return false;

            target = PhelonUtils.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        #endregion

        #region Expressions

        protected static TrinityActor CurrentTarget => CombatBase.CurrentTarget;

        protected static PlayerCache Player => Core.Player;

        protected static bool IsVaultFree
            => Legendary.ChainOfShadows.IsEquipped && Core.Buffs.HasBuff(445266);

        protected static int MaxSentryCount 
			=> 2 + (Legendary.BombardiersRucksack.IsEquipped ? 2 : 0) + (Passives.DemonHunter.CustomEngineering.IsActive ? 1 : 0);

        protected static TrinityPower RainOfVengeance(TrinityActor target)
            => new TrinityPower(SNOPower.DemonHunter_RainOfVengeance, 60f, target.Position);

        protected static TrinityPower Caltrops()
            => new TrinityPower(SNOPower.DemonHunter_Caltrops);

        protected static TrinityPower Sentry(Vector3 position)
            => new TrinityPower(SNOPower.DemonHunter_Sentry, 60f, position);

        private static TrinityPower MarkedForDeath(TrinityActor target)
            => new TrinityPower(SNOPower.DemonHunter_MarkedForDeath, 60f, target.Position);

        protected static TrinityPower Vault(Vector3 position)
            => new TrinityPower(SNOPower.DemonHunter_Vault, 60f, position);

        protected static TrinityPower FanOfKnives()
            => new TrinityPower(SNOPower.DemonHunter_FanOfKnives);

        protected static TrinityPower Multishot(TrinityActor target)
            => new TrinityPower(SNOPower.DemonHunter_Multishot, 70f, target.Position);

        protected static TrinityPower Strafe(Vector3 position)
            => new TrinityPower(SNOPower.DemonHunter_Strafe, 60f, position);

        protected static TrinityPower SpikeTrap()
            => new TrinityPower(SNOPower.DemonHunter_SpikeTrap);

        protected static TrinityPower ElementalArrow(TrinityActor target)
            => new TrinityPower(SNOPower.DemonHunter_ElementalArrow, 70f, target.Position);

        protected static TrinityPower ClusterArrow(TrinityActor target)
            => new TrinityPower(SNOPower.DemonHunter_Multishot, 60f, target.Position);

        protected static TrinityPower Chakram(TrinityActor target)
            => new TrinityPower(SNOPower.DemonHunter_Chakram, 60f, target.Position);

        protected static TrinityPower RapidFire(TrinityActor target)
            => new TrinityPower(SNOPower.DemonHunter_RapidFire, 60f, target.Position);

        protected static TrinityPower Impale(TrinityActor target)
            => new TrinityPower(SNOPower.DemonHunter_Impale, 60f, target.Position);

        protected static TrinityPower EvasiveFire(TrinityActor target)
            => new TrinityPower(SNOPower.X1_DemonHunter_EvasiveFire, 60f, target.Position);

        protected static TrinityPower HungeringArrow(TrinityActor target)
            => new TrinityPower(SNOPower.DemonHunter_HungeringArrow, 60f, target.Position);

        protected static TrinityPower EntanglingShot(TrinityActor target)
            => new TrinityPower(SNOPower.X1_DemonHunter_EntanglingShot, 60f, target.Position);

        protected static TrinityPower Bolas(TrinityActor target)
            => new TrinityPower(SNOPower.DemonHunter_Bolas, 60f, target.Position);

        protected static TrinityPower Grenade(TrinityActor target)
            => new TrinityPower(SNOPower.DemonHunter_Grenades, 60f, target.Position);

        protected static TrinityPower Vengeance()
            => new TrinityPower(SNOPower.X1_DemonHunter_Vengeance);

        protected static TrinityPower ShadowPower()
            => new TrinityPower(SNOPower.DemonHunter_ShadowPower);

        protected static TrinityPower SmokeScreen()
            => new TrinityPower(SNOPower.DemonHunter_SmokeScreen);

        protected static TrinityPower Preparation()
            => new TrinityPower(SNOPower.DemonHunter_Preparation);

        protected static TrinityPower Companion()
            => new TrinityPower(SNOPower.X1_DemonHunter_Companion);

        #endregion

        #region Settings

        //public enum SkillUseTime
        //{
        //    Default,
        //    Always,
        //    Elites,
        //    Emergency
        //}

        //private SkillUseTime _healthThresholdPct;

        //[DataMember]
        //[Setting, DefaultValue(90)]
        //[UIControl(UIControlType.ComboBox), Limit(1, 100)]
        //[Description("When to use this skill")]
        //public SkillUseTime VengeanceUse
        //{
        //    get { return _healthThresholdPct; }
        //    set { SetField(ref _healthThresholdPct, value); }
        //}

        #endregion
    }
}
