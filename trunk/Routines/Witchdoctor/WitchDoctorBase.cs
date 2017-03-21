using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


namespace Trinity.Routines.Witchdoctor
{
    public class WitchDoctorBase : RoutineBase
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

        public virtual ActorClass Class => ActorClass.Witchdoctor;
        public virtual int PrimaryEnergyReserve => 125;
        public virtual int SecondaryEnergyReserve => 0;
        public virtual KiteMode KiteMode => KiteMode.Always;
        public virtual float KiteDistance => 15f;
        public virtual int KiteStutterDuration => 800;
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

        // Primary

        protected virtual bool ShouldPoisonDart(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.PoisonDart.CanCast())
                return false;

            target = CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldCorpseSpiders(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.CorpseSpiders.CanCast())
                return false;

            if (Runes.WitchDoctor.SpiderQueen.IsActive && Player.Summons.SpiderPetCount == 1)
                return false;
                
            target = CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldPlagueOfToads(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.PlagueOfToads.CanCast())
                return false;

            target = CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldFirebomb(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.Firebomb.CanCast())
                return false;

            target = CurrentTarget;
            return target != null;
        }

        // Secondary

        protected virtual bool ShouldGraspOfTheDead(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.GraspOfTheDead.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldFirebats(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.Firebats.CanCast())
                return false;

            if (!IsChannellingFirebats)
            {
                if (Player.PrimaryResource < PrimaryEnergyReserve)
                    return false;
            }

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldHaunt(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.Haunt.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (!CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Haunt))
            {
                target = CurrentTarget;
                return target != null;
            }
                      
            List<TrinityActor> notHaunted;
            var percentHaunted = TargetUtil.DebuffedPercent(SNOPower.Witchdoctor_Haunt, 20f, out notHaunted);
            if (percentHaunted >= 0.60f)
                return false;

            target = notHaunted.FirstOrDefault();
            return target != null;
        }

        protected virtual bool ShouldLocustSwarm(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.LocustSwarm.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (Skills.WitchDoctor.LocustSwarm.TimeSinceUse < 1000)
                return false;

            if (Runes.WitchDoctor.Pestilence.IsActive)
            {
                if (TargetUtil.IsUnitsWithDebuff(20f, CurrentTarget.Position, SNOPower.Witchdoctor_Locust_Swarm))
                    return false;
            }

            if (TargetUtil.IsPercentOfMobsDebuffed(SNOPower.Witchdoctor_Locust_Swarm, 16f, 0.6f))
                return false;
          
            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }


        // Defensive

        protected virtual bool ShouldSummonZombieDogs(out Vector3 position)
        {
            position = Vector3.Zero;            

            if (!Skills.WitchDoctor.SummonZombieDogs.CanCast())
                return false;

            var resummonThreshold = Skills.WitchDoctor.Sacrifice.IsActive ? 1 : MaxDogs;
            if (Player.Summons.ZombieDogCount >= resummonThreshold)
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return true;
        }

        protected virtual bool ShouldHorrify()
        {
            if (!Skills.WitchDoctor.Horrify.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(15f))
                return false;

            return true;
        }

        protected virtual bool ShouldSpiritWalk()
        {
            if (!Skills.WitchDoctor.SpiritWalk.CanCast())
                return false;

            if (Player.CurrentHealthPct > 0.7)
                return false;
                
            return true;
        }

        protected virtual bool ShouldHex(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.WitchDoctor.Hex.CanCast())
                return false;

            position = Runes.WitchDoctor.AngryChicken.IsActive ? Player.Position : TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldExplodeChicken(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!IsChicken)
                return false;

            if (!PowerManager.CanCast(SNOPower.Witchdoctor_Hex_Explode))
                return false;

            if (!IsInCombat)
                return false;

            var timeAsChicken = Skills.WitchDoctor.Hex.TimeSinceUse;
            var chickenDuration = (Sets.ManajumasWay.IsEquipped ? 15000 : 2000);

            var unit = TargetUtil.GetBestClusterUnit();
            if (unit == null)
                return false;

            position = unit.Position;
            return position != Vector3.Zero;
        }

        // Terror

        protected virtual bool ShouldSoulHarvest(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.WitchDoctor.SoulHarvest.CanCast())
                return false;

            var stacks = Skills.WitchDoctor.SoulHarvest.BuffStacks;
            if (stacks > 1 && Skills.WitchDoctor.SoulHarvest.TimeSinceUse > 11000 && HostileMonsters.Any(u => u.Distance <= 18f))
                return true;

            var isMaxHarvestStacks = stacks == MaxSoulHarvestStacks;
            if (isMaxHarvestStacks && !Sets.RaimentOfTheJadeHarvester.IsEquipped)
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return true;
        }

        protected virtual bool ShouldAcidCloud(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.AcidCloud.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            // Allow opportunity for other spenders to be cast.
            if (Skills.WitchDoctor.SpiritBarrage.TimeSinceUse < 750)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return true;
        }

        protected virtual bool ShouldSacrifice()
        {
            if (!Skills.WitchDoctor.Sacrifice.CanCast())
                return false;

            var unitsNearDogs = TargetUtil.UnitsWithinRangeOfPet(PetType.Pet0, 12f).ToList();
            Core.Logger.Log(LogCategory.Routine, $"Units near dogs = {unitsNearDogs.Count}");
            if (unitsNearDogs.Count < ClusterSize && !unitsNearDogs.Any(u => u.IsElite))
                return false;

            if (Runes.WitchDoctor.ProvokeThePack.IsActive && Skills.WitchDoctor.Sacrifice.TimeSinceUse < 5000)
                return false;

            return true;
        }

        protected virtual bool ShouldMassConfusion()
        {
            if (!Skills.WitchDoctor.MassConfusion.CanCast())
                return false;

            return true;
        }

        // Decay

        protected virtual bool ShouldZombieCharger(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.ZombieCharger.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            // Allow opportunity for other spenders to be cast.
            if (Skills.WitchDoctor.ZombieCharger.TimeSinceUse < 750)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return true;
        }

        protected virtual bool ShouldSpiritBarrage(out TrinityActor target)
        {
            target = null;

            var skill = Skills.WitchDoctor.SpiritBarrage;
            if (!skill.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            // Allow opportunity for other spenders to be cast.
            if (Skills.WitchDoctor.SpiritBarrage.TimeSinceUse < 750)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;

            if (Runes.WitchDoctor.Phantasm.IsActive)
            {
                var numPhantasmsAtTarget = SpellHistory.FindSpells(skill.SNOPower, target.Position, 12f, 5).Count();
                if (numPhantasmsAtTarget >= 3)
                    return false;
            }
            
            return true;
        }

        protected virtual bool ShouldWallOfDeath(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.WallOfDeath.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (Sets.HelltoothHarness.IsFullyEquipped && HasJeramsRevengeBuff)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return true;
        }

        protected virtual bool ShouldPiranhas(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.Piranhas.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            target = Runes.WitchDoctor.WaveOfMutilation.IsActive
                ? TargetUtil.GetBestPierceTarget(25, true)
                : TargetUtil.BestAoeUnit(35, true);

            return target != null;
        }

        // Voodoo

        protected virtual bool ShouldGargantuan(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.WitchDoctor.Gargantuan.CanCast())
                return false;

            if (Player.Summons.GargantuanCount == MaxGargs)
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return true;
        }

        protected virtual bool ShouldBigBadVoodoo(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.WitchDoctor.BigBadVoodoo.CanCast())
                return false;

            position = Player.Position;
            return true;
        }

        protected virtual bool ShouldFetishArmy()
        {
            if (!Skills.WitchDoctor.FetishArmy.CanCast())
                return false;

            if (Skills.WitchDoctor.FetishArmy.IsBuffActive)
                return false;

            return true;
        }

        #endregion

        #region Expressions 

        // Primary

        protected virtual TrinityPower PoisonDart(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.PoisonDart, 65f, target.AcdId);

        protected virtual TrinityPower CorpseSpiders(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.CorpseSpiders, 65f, target.AcdId);

        protected virtual TrinityPower PlagueOfToads(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.PlagueOfToads, 45f, target.AcdId);

        protected virtual TrinityPower Firebomb(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.Firebomb, 65f, target.AcdId);

        // Secondary

        protected virtual TrinityPower GraspOfTheDead(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.GraspOfTheDead, 65f, target.AcdId);

        protected virtual TrinityPower Firebats()
            => new TrinityPower(Skills.WitchDoctor.Firebats, 100, 250);

        protected virtual TrinityPower Firebats(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.Firebats, FireBatsRange, target.Position, 100, 250);

        protected virtual TrinityPower Haunt(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.Haunt, 70f, target.AcdId);

        protected virtual TrinityPower LocustSwarm(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.LocustSwarm, 45f, target.AcdId);

        // Defensive

        protected virtual TrinityPower SummonZombieDogs()
            => new TrinityPower(Skills.WitchDoctor.SummonZombieDogs);

        protected virtual TrinityPower SummonZombieDogs(Vector3 position)
            => new TrinityPower(Skills.WitchDoctor.SummonZombieDogs, 65f, position);

        protected virtual TrinityPower Horrify()
            => new TrinityPower(Skills.WitchDoctor.Horrify);

        protected virtual TrinityPower SpiritWalk()
            => new TrinityPower(Skills.WitchDoctor.SpiritWalk);

        protected virtual TrinityPower Hex(Vector3 position)
            => new TrinityPower(Skills.WitchDoctor.Hex, 65f, position) { CastWhenBlocked = true };

        protected virtual TrinityPower ExplodeChicken(Vector3 position)
            => new TrinityPower(SNOPower.Witchdoctor_Hex_Explode, 10f, position);

        // Terror

        protected virtual TrinityPower SoulHarvest()
            => new TrinityPower(Skills.WitchDoctor.SoulHarvest);

        protected virtual TrinityPower SoulHarvest(Vector3 position)
            => new TrinityPower(Skills.WitchDoctor.SoulHarvest, 12f, position);

        protected virtual TrinityPower AcidCloud(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.AcidCloud, 65f, target.AcdId);

        protected virtual TrinityPower Sacrifice()
            => new TrinityPower(Skills.WitchDoctor.Sacrifice);

        protected virtual TrinityPower MassConfusion()
            => new TrinityPower(Skills.WitchDoctor.MassConfusion);

        // Decay

        protected virtual TrinityPower ZombieCharger(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.ZombieCharger, 45f, target.AcdId);

        protected virtual TrinityPower SpiritBarrage(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.SpiritBarrage, 45f, target.AcdId);

        protected virtual TrinityPower WallOfDeath(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.WallOfDeath, 65f, target.AcdId);

        protected virtual TrinityPower WallOfDeath(Vector3 position)
            => new TrinityPower(SNOPower.Witchdoctor_WallOfZombies, 65f, position);

        protected virtual TrinityPower Piranhas(TrinityActor target)
            => new TrinityPower(Skills.WitchDoctor.Piranhas, 45f, target.AcdId);

        // Voodoo

        protected virtual TrinityPower Gargantuan()
            => new TrinityPower(Skills.WitchDoctor.Gargantuan);

        protected virtual TrinityPower Gargantuan(Vector3 position)
            => new TrinityPower(Skills.WitchDoctor.Gargantuan, 45f, position);

        protected virtual TrinityPower BigBadVoodoo()
            => new TrinityPower(Skills.WitchDoctor.BigBadVoodoo);

        protected virtual TrinityPower BigBadVoodoo(Vector3 position)
            => new TrinityPower(Skills.WitchDoctor.BigBadVoodoo, 65f, position);

        protected virtual TrinityPower FetishArmy()
            => new TrinityPower(Skills.WitchDoctor.FetishArmy);

        // Misc

        public static int MaxDogs
        {
            get
            {
                var totalcount = 3;
                if (Passives.WitchDoctor.MidnightFeast.IsActive) totalcount += 1;
                if (Passives.WitchDoctor.FierceLoyalty.IsActive) totalcount += 1;
                if (Passives.WitchDoctor.ZombieHandler.IsActive) totalcount += 1;
                if (Legendary.TheTallMansFinger.IsEquipped) totalcount = 1;
                return totalcount;
            }
        }

        public static int MaxGargs 
            => Legendary.TheShortMansFinger.IsEquipped ? 3 : 1;

        public static int MaxSoulHarvestStacks 
            => Legendary.SacredHarvester.IsEquipped ? 10 : 5;

        public static float FireBatsRange
            => Runes.WitchDoctor.CloudOfBats.IsActive ? 12f : 35f;

        public static bool IsChannellingFirebats 
            => Player.IsChannelling && Skills.WitchDoctor.Firebats.IsLastUsed;

        public static bool HasJeramsRevengeBuff
            => Player.HasBuff(SNOPower.P3_ItemPassive_Unique_Ring_010);

        public static bool IsChicken
            => Player.HasBuff(SNOPower.Witchdoctor_Hex, 2);

        #endregion

        #region Helpers

        protected bool TryPrimaryPower(out TrinityPower power)
        {
            TrinityActor target;
            power = null;

            if (ShouldPoisonDart(out target))
                power = PoisonDart(target);

            else if (ShouldCorpseSpiders(out target))
                power = CorpseSpiders(target);

            else if (ShouldPlagueOfToads(out target))
                power = PlagueOfToads(target);

            else if (ShouldFirebomb(out target))
                power = Firebomb(target);

            return power != null;
        }

        protected bool TrySecondaryPower(out TrinityPower power)
        {
            TrinityActor target;
            power = null;

            if (ShouldGraspOfTheDead(out target))
                power = GraspOfTheDead(target);

            else if (ShouldLocustSwarm(out target))
                power = LocustSwarm(target);

            else if (ShouldHaunt(out target))
                power = Haunt(target);

            else if (ShouldSpiritBarrage(out target))
                power = SpiritBarrage(target);

            else if (ShouldZombieCharger(out target))
                power = ZombieCharger(target);

            else if (ShouldFirebats(out target))
                power = Firebats(target);

            return power != null;
        }

        protected bool TrySpecialPower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldHorrify())
                power = Horrify();

            else if (ShouldBigBadVoodoo(out position))
                power = BigBadVoodoo(position);

            else if (ShouldWallOfDeath(out target))
                power = WallOfDeath(target);

            else if (ShouldExplodeChicken(out position))
                power = ExplodeChicken(position);

            else if (ShouldHex(out position))
                power = Hex(position);

            else if (ShouldAcidCloud(out target))
                power = AcidCloud(target);

            else if (ShouldSacrifice())
                power = Sacrifice();

            else if (ShouldMassConfusion())
                power = MassConfusion();

            else if (ShouldPiranhas(out target))
                power = Piranhas(target);

            else if (ShouldSoulHarvest(out position))
                power = SoulHarvest(position);

            return power != null;
        }

        protected bool TryBuffPower(out TrinityPower power)
        {
            power = null;
            Vector3 position;

            if (ShouldSummonZombieDogs(out position))
                power = IsInCombat ? SummonZombieDogs(position) : SummonZombieDogs();

            else if (ShouldGargantuan(out position))
                power = IsInCombat ? Gargantuan(position) : Gargantuan();

            else if (ShouldSpiritWalk())
                power = SpiritWalk();

            else if (ShouldFetishArmy())
                power = FetishArmy();

            return power != null;
        }

        protected TrinityPower DefaultBuffPower()
        {
            TrinityPower power;
            return TryBuffPower(out power) ? power : null;
        }

        public TrinityPower DefaultMovementPower(Vector3 destination)
        {
            Vector3 position;

            if (!Player.IsInTown)
            {
                if (Runes.WitchDoctor.Severance.IsActive && Skills.WitchDoctor.SpiritWalk.CanCast())
                    return SpiritWalk();

                if (Runes.WitchDoctor.AngryChicken.IsActive && ShouldHex(out position))
                    return Hex(position);
            }

            return Walk(destination);
        }

        public TrinityPower DefaultDestructiblePower()
        {
            if (Skills.WitchDoctor.PoisonDart.CanCast())
                return PoisonDart(CurrentTarget);

            if (Skills.WitchDoctor.CorpseSpiders.CanCast())
                return CorpseSpiders(CurrentTarget);

            if (Skills.WitchDoctor.PlagueOfToads.CanCast())
                return PlagueOfToads(CurrentTarget);

            if (Skills.WitchDoctor.Firebomb.CanCast())
                return Firebomb(CurrentTarget);

            if (Skills.WitchDoctor.Firebats.CanCast())
                return Firebats(CurrentTarget);

            // Haunt needs a position for non-units targets.
            if (Skills.WitchDoctor.Haunt.CanCast()) 
                return new TrinityPower(Skills.WitchDoctor.Haunt, 70f, CurrentTarget.Position);

            if (Skills.WitchDoctor.GraspOfTheDead.CanCast())
                return GraspOfTheDead(CurrentTarget);

            if (Skills.WitchDoctor.ZombieCharger.CanCast())
                return ZombieCharger(CurrentTarget);

            if (Skills.WitchDoctor.SpiritBarrage.CanCast())
                return SpiritBarrage(CurrentTarget);

            if (Skills.WitchDoctor.SoulHarvest.CanCast() && CurrentTarget.IsUnit)
                return SoulHarvest(CurrentTarget.Position);

            return DefaultPower;          
        }

        #endregion

    }

}
