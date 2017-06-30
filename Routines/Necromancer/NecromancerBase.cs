using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Routines.Necromancer
{
    public class NecroMancerBase : RoutineBase
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

        public virtual ActorClass Class => ActorClass.Necromancer;
        public virtual int PrimaryEnergyReserve => 80;
        public virtual int SecondaryEnergyReserve => 0;
        public virtual KiteMode KiteMode => KiteMode.Never;
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

        #region Expressions 

        protected virtual TrinityPower ArmyOfTheDead(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.ArmyOfTheDead, 60f, target.AcdId);

        protected virtual TrinityPower BloodRush(Vector3 position)
            => new TrinityPower(Skills.Necromancer.BloodRush, 50f, position);

        protected virtual TrinityPower BoneArmor()
            => new TrinityPower(Skills.Necromancer.BoneArmor);

        protected virtual TrinityPower BoneSpear(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.BoneSpear, 60f, target.AcdId);

        protected virtual TrinityPower BoneSpikes(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.BoneSpikes, 60f, target.AcdId);

        protected virtual TrinityPower BoneSpirit(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.BoneSpirit, 60f, target.AcdId);

        protected virtual TrinityPower CommandGolem(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.CommandGolem, 60f, target.AcdId);

        protected virtual TrinityPower CommandSkeletons(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.CommandSkeletons, 60f, target.AcdId);

        protected virtual TrinityPower CorpseExplosion(Vector3 position)
            => new TrinityPower(Skills.Necromancer.CorpseExplosion, 60f, position);

        protected virtual TrinityPower DeathNova(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.DeathNova, 20f, target.AcdId);

        protected virtual TrinityPower DeathNova()
            => new TrinityPower(Skills.Necromancer.DeathNova);

        protected virtual TrinityPower Devour()
            => new TrinityPower(Skills.Necromancer.Devour);

        protected virtual TrinityPower Frailty(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.Frailty, 70f, target.AcdId);

        protected virtual TrinityPower GrimScythe(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.GrimScythe, 15f, target.AcdId);

        protected virtual TrinityPower LandOfTheDead()
            => new TrinityPower(Skills.Necromancer.LandOfTheDead);

        protected virtual TrinityPower Leech(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.Leech, 60f, target.AcdId);

        protected virtual TrinityPower Decrepify(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.Decrepify, 60f, target.AcdId);

        protected virtual TrinityPower Revive(Vector3 position)
            => new TrinityPower(Skills.Necromancer.Revive, 60f, position);

        protected virtual TrinityPower Simulacrum(Vector3 position)
            => new TrinityPower(Skills.Necromancer.Simulacrum, 60f, position);

        protected virtual TrinityPower SiphonBlood(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.SiphonBlood, 60f, target.AcdId);

        protected virtual TrinityPower SkeletalMage(Vector3 position)
            => new TrinityPower(Skills.Necromancer.SkeletalMage, 60f, position);

        protected virtual TrinityPower CorpseLance(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.CorpseLance, 60f, target.AcdId);

    
        #endregion

        #region Helpers / Conditions

        protected bool TryPrimaryPower(out TrinityPower power)
        {
            TrinityActor target;
            power = null;

            if (ShouldBoneSpikes(out target))
                power = BoneSpikes(target);

            else if (ShouldGrimScythe(out target))
                power = GrimScythe(target);

            else if (ShouldSiphonBlood(out target))
                power = SiphonBlood(target);

            return power != null;
        }

        protected virtual bool ShouldBoneSpikes(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.BoneSpikes.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldGrimScythe(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.GrimScythe.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldSiphonBlood(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.SiphonBlood.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool TrySecondaryPower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldDeathNova())
                power = DeathNova();

            else if (ShouldSkeletalMage(out position))
                power = SkeletalMage(position);

            else if (ShouldBoneSpear(out target))
                power = BoneSpear(target);

            return power != null;
        }

        protected virtual bool ShouldDeathNova()
        {
            if (!Skills.Necromancer.DeathNova.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            return TargetUtil.AnyMobsInRange(15f);
        }

        protected virtual bool ShouldBoneSpear(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.BoneSpear.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldSkeletalMage(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Necromancer.SkeletalMage.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected bool TryCorpsePower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldCorpseExplosion(out position))
                power = CorpseExplosion(position);

            else if (ShouldCorpseLance(out target))
                power = CorpseLance(target);

            else if (ShouldDevour())
                power = Devour();

            else if (ShouldRevive(out position))
                power = Revive(position);

            return power != null;
        }

        protected virtual bool ShouldRevive(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Necromancer.Revive.CanCast())
                return false;

            position = TargetUtil.GetBestCorpsePoint(80f,20f);
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldCorpseExplosion(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Necromancer.CorpseExplosion.CanCast())
                return false;

            if (IsStuck || IsBlocked)
                return false;

            // Grasps of Essense gloves increase damage 75-100 % 5x stacks max

            position = TargetUtil.GetBestCorpsePoint(80f,15f);
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldCorpseLance(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.CorpseLance.CanCast())
                return false;

            if (TargetUtil.CorpseCount(80f) <= 0)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldDevour()
        {
            if (!Skills.Necromancer.Devour.CanCast())
                return false;

            if (Player.PrimaryResourcePct > 0.3f)
                return false;

            if (TargetUtil.CorpseCount(60f) <= 0)
                return false;

            return true;
        }

        protected bool TryReanimationPower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldCommandSkeletons(out target))
                power = CommandSkeletons(target);

            else if (ShouldCommandGolem(out target))
                power = CommandGolem(target);

            else if (ShouldArmyOfTheDead(out target))
                power = ArmyOfTheDead(target);

            else if (ShouldLandOfTheDead(out target))
                power = LandOfTheDead();

            return power != null;
        }


        protected virtual bool ShouldLandOfTheDead(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.LandOfTheDead.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldArmyOfTheDead(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.ArmyOfTheDead.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldCommandGolem(out TrinityActor target)
        {
            target = null;

            // note: this is returning PowerManager.CanCast true even when active is on cooldown.

            if (!Skills.Necromancer.CommandGolem.CanCast())
                return false;

            var rune = Skills.Necromancer.CommandGolem.CurrentRune;
            if (rune == Runes.Necromancer.FleshGolem)
            {
                //
            }
            if (rune == Runes.Necromancer.IceGolem)
            {
                //
            }
            if (rune == Runes.Necromancer.BoneGolem)
            {
                //
            }
            if (rune == Runes.Necromancer.DecayGolem)
            {
                if (TargetUtil.CorpseCount(20f) == 0)
                    return false;
            }
            if (rune == Runes.Necromancer.BloodGolem)
            {
                //
            }

            if (Core.Cooldowns.GetSkillCooldownRemaining(SNOPower.P6_Necro_RaiseGolem).TotalMilliseconds > 0)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected int LastSkeletonCommandTargetAcdId { get; set; }

        protected virtual bool ShouldCommandSkeletons(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.CommandSkeletons.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            if (target != null)
            {
                // assuming skeletons only get the focus damage bonus on one acd at a time.
                // prevent spamming on the same target over and over, maybe there is an attribute on skeletons 
                // for their current target or the skeleton power on the target itself.

                var lastCast = SpellHistory.GetLastUseHistoryItem(SNOPower.P6_Necro_CommandSkeletons);
                if (lastCast == null)
                    return true;
               
                LastSkeletonCommandTargetAcdId = lastCast.TargetAcdId;
                return target.AcdId != lastCast.TargetAcdId;
            }
            return false;
        }        

        protected bool TryCursePower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldLeech(out target))
                power = Leech(target);

            else if (ShouldDecrepify(out target))
                power = Decrepify(target);

            else if (ShouldFrailty(out target))
                power = Frailty(target);

            return power != null;
        }

        protected virtual bool ShouldFrailty(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.Frailty.CanCast())
                return false;


            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            // todo: investigate why cant find the power on monsters.         

            // anti-spam workaround
            if (Skills.Necromancer.Frailty.TimeSinceUse < 4000)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldDecrepify(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.Decrepify.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            // todo: investigate why cant find the power on monsters.         

            // anti-spam workaround
            if (Skills.Necromancer.Decrepify.TimeSinceUse < 4000) 
                return false;

            target = TargetUtil.BestTargetWithoutDebuff(60f, SNOPower.P6_Necro_Decrepify);
            return target != null;
        }

        protected virtual bool ShouldLeech(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.Leech.CanCast())
                return false;


            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            // todo: investigate why cant find the debuff on monsters.         

            // anti-spam workaround
            if (Skills.Necromancer.Leech.TimeSinceUse < 8000)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected bool TryBloodPower(out TrinityPower power)
        {
            TrinityActor target;
            Vector3 position;
            power = null;

            if (ShouldBoneArmor())
                power = BoneArmor();

            else if (ShouldBloodRush(out position))
                power = BloodRush(position);

            else if (ShouldSimulacrum(out position))
                power = Simulacrum(position);

            else if (ShouldBoneSpirit(out target))
                power = BoneSpirit(target);

            return power != null;
        }

        protected virtual bool ShouldBoneArmor()
        {
            if (!Skills.Necromancer.BoneArmor.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(10f))
                return false;

            // todo Need a way to check the cooldown properly, PowerManager/CanCast doesnt seem to be catching it.
            if (Skills.Necromancer.BoneArmor.TimeSinceUse < 10000)
                return false;

            // Inarius Set inceases damage making it more useful than just for defence
            // Wisdom of Kalan ring increases stacks by 5

            //if (Core.Buffs.GetBuff(SNOPower.P6_Necro_BoneArmor)?.Remaining.TotalSeconds > 0)
            //    return false;

            return true;
        }

        protected virtual bool ShouldBloodRush(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Necromancer.BloodRush.CanCast())
                return false;

            if (Skills.Necromancer.BloodRush.CooldownRemaining > 0)
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldSimulacrum(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Necromancer.Simulacrum.CanCast())
                return false;

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldBoneSpirit(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.BoneSpirit.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        public TrinityPower DefaultDestructiblePower()
        {
            if (CurrentTarget == null)
                return null;

            if (Skills.Necromancer.BoneSpikes.CanCast())
                return BoneSpikes(CurrentTarget);

            if (Skills.Necromancer.GrimScythe.CanCast())
                return GrimScythe(CurrentTarget);

            if (Skills.Necromancer.SiphonBlood.CanCast())
                return SiphonBlood(CurrentTarget);

            if (Skills.Necromancer.DeathNova.CanCast())
                return DeathNova(CurrentTarget);

            if (Skills.Necromancer.SkeletalMage.CanCast())
                return SkeletalMage(CurrentTarget.Position);

            if (Skills.Necromancer.DeathNova.CanCast())
                return DeathNova(CurrentTarget);

            if (Skills.Necromancer.BoneSpear.CanCast())
                return BoneSpear(CurrentTarget);

            return DefaultPower;
        }

        #endregion

    }
}
