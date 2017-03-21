using System;
using Trinity.Framework;
using System.Linq;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;


namespace Trinity.Routines.DemonHunter
{
    public class DemonHunterBase : RoutineBase
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

        public virtual ActorClass Class => ActorClass.DemonHunter;
        public virtual int PrimaryEnergyReserve => 25;
        public virtual int SecondaryEnergyReserve => 30;
        public virtual KiteMode KiteMode => KiteMode.Always;
        public virtual float KiteDistance => 15f;

        /// <summary>
        /// The amount of time spent pausing between kite movements
        /// </summary>
        public virtual int KiteStutterDuration => 800;

        /// <summary>
        /// The amount of time spent kiting before pausing to 'stutter'
        /// </summary>
        public virtual int KiteStutterDelay => 1400;

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

        protected virtual bool ShouldRainOfVengeance(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.RainOfVengeance.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldSentry(out Vector3 position)
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

        protected virtual bool ShouldCaltrops()
        {
            if (!Skills.DemonHunter.Caltrops.CanCast())
                return false;

            if (Player.SecondaryResource < SecondaryEnergyReserve)
                return false;

            if (Runes.DemonHunter.BaitTheTrap.IsActive)
            {
                if (Player.HasBuff(SNOPower.DemonHunter_Caltrops, 3))
                    return false;

                if (Skills.DemonHunter.Caltrops.TimeSinceUse < 2000)
                    return false;
            }
            else
            {
                if (!TargetUtil.AnyMobsInRange(16f))
                    return false;

                if (Skills.DemonHunter.Caltrops.TimeSinceUse < 6000)
                    return false;
            }

            return true;
        }

        protected virtual bool ShouldMarkedForDeath(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.MarkedForDeath.CanCast())
                return false;

            if (CurrentTarget.HasDebuff(SNOPower.DemonHunter_MarkedForDeath))
                return false;

            if (SpellTracker.IsUnitTracked(CurrentTarget, SNOPower.DemonHunter_MarkedForDeath))
                return false;

            target = CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldVault(out Vector3 destination)
        {
            // Note: This is for casting while not moving.  
            // Routine GetMovermentPower() may cast for movement
            // (Its called directly by our IPlayerMover)

            destination = Vector3.Zero;

            if (!Skills.DemonHunter.Vault.CanCast())
                return false;

            // Find a safespot with no monsters within kite range.
            Core.Avoidance.Avoider.TryGetSafeSpot(out destination, 23f, 27f, Player.Position, 
                node => !TargetUtil.AnyMobsInRangeOfPosition(node.NavigableCenter, KiteDistance));

            // Vault is a fixed distance spell, predict our actual landing position
            destination = MathEx.CalculatePointFrom(destination, ZetaDia.Me.Position, 25);

            // Prevent vaulting away from stuff that needs to be interacted with.
            if (ZetaDia.Actors.GetActorsOfType<DiaGizmo>().Any(g => g.Distance < 10f && g.ActorInfo.GizmoType != GizmoType.DestroyableObject))
                return false;

            // Don't vault into molten core/arcane.
            if (!Core.Avoidance.InCriticalAvoidance(ZetaDia.Me.Position) && Core.Avoidance.Grid.IsIntersectedByFlags(ZetaDia.Me.Position, destination, AvoidanceFlags.CriticalAvoidance))
                return false;

            // Prevent the bot from vaulting back and forth over and item without being able to pick it up.
            if (CurrentTarget?.Type == TrinityObjectType.Item && CurrentTarget.Distance < 35f)
                return false;

            // Prevent trying to vault up walls; spider man he is not.
            if (Math.Abs(destination.Z - Core.Player.Position.Z) > 5)
                return false;

            return true;
        }

        protected virtual bool ShouldFanOfKnives()
        {
            if (!Skills.DemonHunter.FanOfKnives.CanCast())
                return false;

            if (Runes.DemonHunter.FanOfDaggers.IsActive && Player.CurrentHealthPct < 0.65 && TargetUtil.AnyMobsInRange(16f))
                return true;

            if (TargetUtil.NumMobsInRange(20f) >= 4 || TargetUtil.AnyElitesInRange(15f))
                return true;

            return false;
        }

        protected virtual bool ShouldMultishot(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.Multishot.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(60f))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldStrafe(out Vector3 position)
        {
            // Note: This is for casting while not moving.  
            // Routine GetMovermentPower() may cast for movement
            // (Its called directly by our IPlayerMover)   

            position = Vector3.Zero;

            if (!Skills.DemonHunter.Strafe.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(50f))
                return false;

            position = TargetUtil.GetZigZagTarget(CurrentTarget.Position, 20f, true);
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldVengeance()
        {
            if (!Skills.DemonHunter.Vengeance.CanCast())
                return false;

            return true;
        }

        protected virtual bool ShouldShadowPower()
        {
            if (!Skills.DemonHunter.ShadowPower.CanCast())
                return false;

            if (Player.HasBuff(SNOPower.DemonHunter_ShadowPower))
                return false;

            if (Skills.DemonHunter.ShadowPower.TimeSinceUse < 4500)
                return false;

            if (TargetUtil.AnyElitesInRange(40f))
                return true;

            if (Player.CurrentHealthPct < 0.7f)
                return true;

            if (TargetUtil.NumMobsInRange() > 5)
                return true;

            return false;
        }

        protected virtual bool ShouldSpikeTrap()
        {
            if (!Skills.DemonHunter.SpikeTrap.CanCast())
                return false;

            if (SpellHistory.LastPowerUsed == SNOPower.DemonHunter_SpikeTrap)
                return false;

            if (!TargetUtil.AnyMobsInRange(15f))
                return false;

            return true;
        }

        protected virtual bool ShouldSmokeScreen()
        {
            if (!Skills.DemonHunter.SmokeScreen.CanCast())
                return false;

            if (Player.HasBuff(SNOPower.DemonHunter_SmokeScreen))
                return false;

            if (Player.CurrentHealthPct <= 0.75 || Player.IsRooted || Player.IsIncapacitated)
                return true;

            if (TargetUtil.AnyMobsInRange(15))
                return true;

            return false;
        }

        protected virtual bool ShouldPreparation()
        {
            if (!Skills.DemonHunter.Preparation.CanCast())
                return false;

            if (!Runes.DemonHunter.Punishment.IsActive && Player.SecondaryResourcePct <= 0.5f)
                return true;

            if (Runes.DemonHunter.Punishment.IsActive && Player.PrimaryResourcePct <= 0.3f)
                return true;

            return false;
        }

        protected virtual bool ShouldCompanion()
        {
            if (!Skills.DemonHunter.Companion.CanCast())
                return false;

            // Use Spider Slow on 4 or more trash mobs in an area or on Unique/Elite/Champion
            if (Runes.DemonHunter.SpiderCompanion.IsActive && TargetUtil.ClusterExists(25f, 4) && TargetUtil.EliteOrTrashInRange(25f))
                return true;

            // Use Bat when Hatred is Needed
            if (Runes.DemonHunter.BatCompanion.IsActive && Player.PrimaryResourceMissing >= 60)
                return true;

            // Use Boar Taunt on 3 or more trash mobs in an area or on Unique/Elite/Champion
            if (Runes.DemonHunter.BoarCompanion.IsActive && (TargetUtil.ClusterExists(20f, 4) && TargetUtil.EliteOrTrashInRange(20f) || CurrentTarget != null && CurrentTarget.IsElite && CurrentTarget.Distance <= 20f))
                return true;

            // Ferrets used for picking up Health Globes when low on Health
            if (Runes.DemonHunter.FerretCompanion.IsActive && Core.Targets.Entries.Any(o => o.Type == TrinityObjectType.HealthGlobe && o.Distance < 60f) && Player.CurrentHealthPct < EmergencyHealthPct)
                return true;

            // Use Wolf Howl on Unique/Elite/Champion
            if (Runes.DemonHunter.WolfCompanion.IsActive && (TargetUtil.AnyElitesInRange(80f) || TargetUtil.AnyMobsInRange(40, 5)))
                return true;

            return false;
        }

        protected virtual bool ShouldElementalArrow(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.ElementalArrow.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve && !Legendary.Kridershot.IsEquipped)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldClusterArrow(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.ClusterArrow.CanCast())
                return false;

            if (Sets.NatalyasVengeance.IsFullyEquipped && Player.PrimaryResource < 100 && !Player.HasBuff(SNOPower.P2_ItemPassive_Unique_Ring_053))
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldChakram(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.Chakram.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldImpale(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.Impale.CanCast())
                return false;

            if (Player.PrimaryResource <= PrimaryEnergyReserve || Player.PrimaryResource <= 20)
                return false;

            if (CurrentTarget.RadiusDistance > 50f)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldRapidFire(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.RapidFire.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldEvasiveFire(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.EvasiveFire.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldHungeringArrow(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.HungeringArrow.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldEntanglingShot(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.EntanglingShot.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldBolas(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.Bolas.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldGrenade(out TrinityActor target)
        {
            target = null;

            if (!Skills.DemonHunter.Grenade.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        #endregion

        #region Expressions 

        protected virtual TrinityPower RainOfVengeance(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.RainOfVengeance, 60f, target.AcdId);

        protected virtual TrinityPower Caltrops()
            => new TrinityPower(Skills.DemonHunter.Caltrops);

        protected virtual TrinityPower Sentry(Vector3 position)
            => new TrinityPower(Skills.DemonHunter.Sentry, 60f, position);

        protected virtual TrinityPower MarkedForDeath(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.MarkedForDeath, 60f, target.AcdId);

        protected virtual TrinityPower Vault(Vector3 position)
            => new TrinityPower(Skills.DemonHunter.Vault, 60f, position);

        protected virtual TrinityPower FanOfKnives()
            => new TrinityPower(Skills.DemonHunter.FanOfKnives);

        protected virtual TrinityPower FanOfKnives(Vector3 position)
            => new TrinityPower(Skills.DemonHunter.FanOfKnives, 5f, position);

        protected virtual TrinityPower Multishot(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.Multishot, 80f, target.AcdId);

        protected virtual TrinityPower Strafe(Vector3 position)
            => new TrinityPower(Skills.DemonHunter.Strafe, 60f, position, 25, 25);

        protected virtual TrinityPower SpikeTrap()
            => new TrinityPower(Skills.DemonHunter.SpikeTrap);

        protected virtual TrinityPower ElementalArrow(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.ElementalArrow, 70f, target.AcdId);

        protected virtual TrinityPower ClusterArrow(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.ClusterArrow, 70f, target.AcdId);

        protected virtual TrinityPower Chakram(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.Chakram, 60f, target.AcdId);

        protected virtual TrinityPower RapidFire(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.RapidFire, 75f, target.AcdId, 25, 25);

        protected virtual TrinityPower Impale(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.Impale, 60f, target.AcdId);

        protected virtual TrinityPower EvasiveFire(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.EvasiveFire, 60f, target.AcdId);

        protected virtual TrinityPower HungeringArrow(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.HungeringArrow, 60f, target.AcdId);

        protected virtual TrinityPower EntanglingShot(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.EntanglingShot, 60f, target.AcdId);

        protected virtual TrinityPower Bolas(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.Bolas, 60f, target.AcdId);

        protected virtual TrinityPower Grenade(TrinityActor target)
            => new TrinityPower(Skills.DemonHunter.Grenade, 60f, target.AcdId);

        protected virtual TrinityPower Vengeance()
            => new TrinityPower(Skills.DemonHunter.Vengeance);

        protected virtual TrinityPower ShadowPower()
            => new TrinityPower(Skills.DemonHunter.ShadowPower);

        protected virtual TrinityPower SmokeScreen()
            => new TrinityPower(Skills.DemonHunter.SmokeScreen);

        protected virtual TrinityPower Preparation()
            => new TrinityPower(Skills.DemonHunter.Preparation);

        protected virtual TrinityPower Companion()
            => new TrinityPower(Skills.DemonHunter.Companion);

        protected static bool IsVaultFree
            => Legendary.ChainOfShadows.IsEquipped && Core.Buffs.HasBuff(445266);

        protected static int MaxSentryCount
            => 2 + (Legendary.BombardiersRucksack.IsEquipped ? 2 : 0) + (Passives.DemonHunter.CustomEngineering.IsActive ? 1 : 0);

        public static bool CanAcquireFreeVaultBuff
            => Legendary.ChainOfShadows.IsEquipped && !Core.Buffs.HasBuff(445266) && Skills.DemonHunter.Impale.CanCast();

        public static int LordGreenStoneDamageStacks
            => Core.Buffs.GetBuffStacks((SNOPower)445274); //P4_ItemPassive_Unique_Ring_007


        #endregion

        #region Helpers

        protected bool TryPrimaryPower(out TrinityPower power)
        {
            TrinityActor target;
            power = null;

            if (ShouldHungeringArrow(out target))
                power = HungeringArrow(target);

            else if (ShouldEntanglingShot(out target))
                power = EntanglingShot(target);

            else if (ShouldBolas(out target))
                power = Bolas(target);

            else if (ShouldGrenade(out target))
                power = Grenade(target);

            else if (ShouldEvasiveFire(out target))
                power = EvasiveFire(target);

            return power != null;
        }

        protected bool TrySecondaryPower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldElementalArrow(out target))
                power = ElementalArrow(target);

            else if (ShouldClusterArrow(out target))
                power = ClusterArrow(target);

            else if (ShouldMultishot(out target))
                power = Multishot(target);

            else if (ShouldRapidFire(out target))
                power = RapidFire(target);

            else if (ShouldImpale(out target))
                power = Impale(target);

            else if (ShouldStrafe(out position))
                power = Strafe(position);

            else if (ShouldChakram(out target))
                power = Chakram(target);

            return power != null;
        }

        protected bool TrySpecialPower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldFanOfKnives())
                power = FanOfKnives();

            else if (ShouldSpikeTrap())
                power = SpikeTrap();

            else if (ShouldCaltrops())
                power = Caltrops();

            else if (ShouldMarkedForDeath(out target))
                power = MarkedForDeath(target);

            else if (ShouldRainOfVengeance(out target))
                power = RainOfVengeance(target);

            else if (ShouldSentry(out position))
                power = Sentry(position);

            return power != null;
        }

        public TrinityPower DefaultDestructiblePower()
        {
            TrinityPower power;
            if (CurrentTarget.IsCorruptGrowth && TrySecondaryPower(out power))
                return power;

            if (Skills.DemonHunter.HungeringArrow.CanCast())
                return HungeringArrow(CurrentTarget);

            if (Skills.DemonHunter.EntanglingShot.CanCast())
                return EntanglingShot(CurrentTarget);

            if (Skills.DemonHunter.EvasiveFire.CanCast())
                return EvasiveFire(CurrentTarget);

            if (Skills.DemonHunter.Bolas.CanCast())
                return Bolas(CurrentTarget);

            if (Skills.DemonHunter.Grenade.CanCast())
                return Grenade(CurrentTarget);

            if (Skills.DemonHunter.Chakram.CanCast())
                return Multishot(CurrentTarget);

            if (Skills.DemonHunter.ClusterArrow.CanCast())
                return Multishot(CurrentTarget);

            if (Skills.DemonHunter.ElementalArrow.CanCast())
                return Multishot(CurrentTarget);

            return DefaultPower;          
        }

        public bool CanVaultTo(Vector3 destination)
        {
            if (destination == Vector3.Zero)
                return false;

            if (!Skills.DemonHunter.Vault.CanCast())
                return false;

            var destinationDistance = destination.Distance(Core.Player.Position);
            if (destinationDistance < 10f)
                return false;

            // Vault is a fixed distance spell 
            if (destinationDistance < 25f)
                destination = MathEx.CalculatePointFrom(destination, ZetaDia.Me.Position, 25);

            // Prevent vaulting away from stuff that needs to be interacted with.
            if (ZetaDia.Actors.GetActorsOfType<DiaGizmo>().Any(g => g.Distance < 10f && g.ActorInfo.GizmoType != GizmoType.DestroyableObject))
                return false;

            // Prevent vaulting somewhere we'll just kite away from.
            if (TargetUtil.AnyMobsInRangeOfPosition(destination, KiteDistance) && Player.CurrentHealthPct > KiteHealthPct)
                return false;

            // Don't vault into a target who's just out of range
            if (CurrentTarget?.Distance < 90f && ShouldRefreshBastiansGenerator && TargetUtil.UnitsInRangeOfPosition(Player.Position, 90f).Any(u => u.IsInLineOfSight))
                return false;

            // Don't vault into molten core/arcane.
            if (!Core.Avoidance.InCriticalAvoidance(ZetaDia.Me.Position) && Core.Avoidance.Grid.IsIntersectedByFlags(ZetaDia.Me.Position, destination, AvoidanceFlags.CriticalAvoidance))
                return false;

            // Prevent the bot from vaulting back and forth over and item without being able to pick it up.
            if (CurrentTarget?.Type == TrinityObjectType.Item && destinationDistance < 20f)
                return false;

            // Prevent trying to vault up walls; spider man he is not.
            if (Math.Abs(destination.Z - Core.Player.Position.Z) > 5)
                return false;

            return true;
        }

        public virtual bool CanStrafeTo(Vector3 destination)
        {
            if (!Skills.DemonHunter.Strafe.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(50f))
                return false;

            return true;
        }

        #endregion

    }
}
