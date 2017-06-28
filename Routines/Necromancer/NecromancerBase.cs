using System;
using System.Linq;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
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
        public virtual int PrimaryEnergyReserve => 100;
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

        protected virtual TrinityPower BoneSpikes(Vector3 position)
            => new TrinityPower(Skills.Necromancer.BoneSpikes, 60f, position);

        protected virtual TrinityPower BoneSpirit(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.BoneSpirit, 60f, target.AcdId);

        protected virtual TrinityPower CommandGolem(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.CommandGolem, 60f, target.AcdId);

        protected virtual TrinityPower CommandSkeletons(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.CommandSkeletons, 60f, target.AcdId);

        protected virtual TrinityPower CorpseExplosion(Vector3 position)
            => new TrinityPower(Skills.Necromancer.CorpseExplosion, 60f, position);

        protected virtual TrinityPower SpikeTrap(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.CorpseLance, 60f, target.AcdId);

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

        protected virtual TrinityPower Revive(Vector3 position)
            => new TrinityPower(Skills.Necromancer.Revive, 60f, position);

        protected virtual TrinityPower Simulacrum(Vector3 position)
            => new TrinityPower(Skills.Necromancer.Simulacrum, 60f, position);

        protected virtual TrinityPower SiphonBlood(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.SiphonBlood, 60f, target.AcdId);

        protected virtual TrinityPower SkeletalMage(Vector3 position)
            => new TrinityPower(Skills.Necromancer.SkeletalMage, 60f, position);


        #endregion

    }
}
