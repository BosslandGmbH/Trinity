using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;


namespace Trinity.Routines.Wizard
{
    public class WizardBase : RoutineBase
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

        public virtual ActorClass Class => ActorClass.Wizard;
        public virtual int PrimaryEnergyReserve => 25;
        public virtual int SecondaryEnergyReserve => 0;
        public virtual KiteMode KiteMode => KiteMode.Always;
        public virtual float KiteDistance => 10f;
        public virtual int KiteStutterDuration => 800;
        public virtual int KiteStutterDelay => 1400;
        public virtual int KiteHealthPct => 100;
        public virtual float TrashRange => 80f;
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

        protected virtual bool ShouldMagicMissile(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.MagicMissile.CanCast())
                return false;

            if (Skills.Wizard.MagicMissile.IsLastUsed && IsMultiPrimary)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldShockPulse(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.ShockPulse.CanCast())
                return false;

            if (Skills.Wizard.ShockPulse.IsLastUsed && IsMultiPrimary)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldSpectralBlade(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.SpectralBlade.CanCast())
                return false;

            if (Skills.Wizard.SpectralBlade.IsLastUsed && IsMultiPrimary)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldElectrocute(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.Electrocute.CanCast())
                return false;

            if (Skills.Wizard.Electrocute.IsLastUsed && IsMultiPrimary)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        // Defensive

        protected virtual bool ShouldFrostNova()
        {
            if (!Skills.Wizard.FrostNova.CanCast())
                return false;

            if (Legendary.HaloOfArlyse.IsEquipped && Skills.Wizard.IceArmor.IsActive)
                return false;

            if (Player.CurrentHealthPct > 0.8)
                return false;

            if (!TargetUtil.AnyMobsInRange(25f))
                return false;

            return true;
        }

        protected virtual bool ShouldDiamondSkin()
        {
            if (!Skills.Wizard.DiamondSkin.CanCast())
                return false;

            if (Player.CurrentHealthPct > 0.8)
                return false;

            if (!TargetUtil.AnyMobsInRange(25f))
                return false;

            return true;
        }

        // Secondary

        protected virtual bool ShouldArcaneOrb(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.ArcaneOrb.CanCast())
                return false;

            if (Skills.Wizard.ArcaneOrb.IsLastUsed || Skills.Wizard.ArcaneOrb.TimeSinceUse < 1000)
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldRayOfFrost(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.RayOfFrost.CanCast())
                return false;

            var isChannelling = Player.IsChannelling && Skills.Wizard.RayOfFrost.IsLastUsed;
            if (!isChannelling)
            {
                if (Player.PrimaryResource < PrimaryEnergyReserve)
                    return false;
            }

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldArcaneTorrent(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.ArcaneTorrent.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldDisintegrate(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.Disintegrate.CanCast())
                return false;

            if (!IsChannellingDisintegrate)
            {
                if (Player.PrimaryResource < PrimaryEnergyReserve)
                    return false;
            }

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        // Force

        protected virtual bool ShouldWaveOfForce(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.WaveOfForce.CanCast())
                return false;

            if (!IsNoPrimary && Skills.Wizard.WaveOfForce.TimeSinceUse < 1000)
                return false;

            if (!Player.IsChannelling)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldHydra(out Vector3 position)
        {
            TrinityActor target;
            position = Vector3.Zero;

            var skill = Skills.Wizard.Hydra;
            if (!skill.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            if (target == null)
                return false;

            var isHydraAtLocation = SpellHistory.FindSpells(skill.SNOPower, target.Position, 15f, 6).Any();
            if (isHydraAtLocation)
                return false;

            if (Player.Summons.HydraCount < MaxHydras && ZetaDia.Me.IsInCombat)
            {
                Core.Logger.Log(LogCategory.Routine, $"Casting Hydra, Less than max Hydras! ({Player.Summons.HydraCount}/{MaxHydras})");
                position = target.Position;
                return true;
            }

            return false;
        }

        protected virtual bool ShouldEnergyTwister(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.EnergyTwister.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldMeteor(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.Meteor.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldBlizzard(out TrinityActor target)
        {
            target = null;

            var skill = Skills.Wizard.Blizzard;
            if (!skill.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit(BlizzardRadius, 70f) ?? CurrentTarget;
            if (target == null)
                return false;

            var isExistingBlizzard = SpellHistory.FindSpells(skill.SNOPower, target.Position, BlizzardRadius, 6).Any();
            return !isExistingBlizzard;
        }

        // Conjuration

        protected virtual bool ShouldIceArmor()
        {
            if (!Skills.Wizard.IceArmor.CanCast())
                return false;

            if (Skills.Wizard.IceArmor.IsBuffActive)
                return false;

            return true;
        }

        protected virtual bool ShouldStormArmor()
        {
            if (!Skills.Wizard.StormArmor.CanCast())
                return false;

            if (Skills.Wizard.StormArmor.IsBuffActive)
                return false;

            return true;
        }

        protected virtual bool ShouldEnergyArmor()
        {
            if (!Skills.Wizard.EnergyArmor.CanCast())
                return false;

            if (Skills.Wizard.EnergyArmor.IsBuffActive)
                return false;

            return true;
        }

        protected virtual bool ShouldMagicWeapon()
        {
            if (!Skills.Wizard.MagicWeapon.CanCast())
                return false;

            if (Skills.Wizard.MagicWeapon.IsBuffActive)
                return false;

            return true;
        }

        protected virtual bool ShouldFamiliar()
        {
            if (!Skills.Wizard.Familiar.CanCast())
                return false;

            if (Skills.Wizard.Familiar.IsBuffActive)
                return false;

            return true;
        }

        // Defensive

        protected virtual bool ShouldSlowTime(out Vector3 position)
        {
            position = Vector3.Zero;

            var skill = Skills.Wizard.SlowTime;
            if (!skill.CanCast())
                return false;

            var myPosition = ZetaDia.Me.Position;
            var clusterPosition = TargetUtil.GetBestClusterPoint(0f, 50f);
            var bubbles = SpellHistory.FindSpells(skill.SNOPower, 12).ToList();
            var bubblePositions = new List<Vector3>(bubbles.Select(b => b.TargetPosition));
            var isDefensiveRune = Runes.Wizard.PointOfNoReturn.IsActive || Runes.Wizard.StretchTime.IsActive || Runes.Wizard.Exhaustion.IsActive;

            bool IsBubbleAtPosition(Vector3 pos) => bubblePositions
                .Any(b => b.Distance(pos) <= 14f &&
                          pos.Distance(myPosition) < SlowTimeRange);

            // On Self            
            if (TargetUtil.ClusterExists(15f, 60f, 8) && isDefensiveRune && !IsBubbleAtPosition(myPosition))
            {
                position = MathEx.GetPointAt(myPosition, 10f, Player.Rotation);
                return true;
            }

            // On Elites
            if (CurrentTarget.IsElite && CurrentTarget.Distance < SlowTimeRange && !IsBubbleAtPosition(CurrentTarget.Position))
            {
                position = CurrentTarget.Position;
                return true;
            }

            // On Clusters            
            if (TargetUtil.ClusterExists(50f, 5) && clusterPosition.Distance(myPosition) < SlowTimeRange && !IsBubbleAtPosition(clusterPosition))
            {
                position = clusterPosition;
                return true;
            }

            // Delseres Magnum Opus Set
            if (Sets.DelseresMagnumOpus.IsEquipped)
            {
                var isLargeCluster = Core.Clusters.LargeCluster.Exists && Core.Clusters.LargeCluster.Position.Distance(myPosition) < SlowTimeRange;
                if (isLargeCluster && !IsBubbleAtPosition(Core.Clusters.LargeCluster.Position))
                {
                    position = Core.Clusters.LargeCluster.Position;
                    return true;
                }

                var isAnyCluster = Core.Clusters.BestCluster.Exists && Core.Clusters.BestCluster.Position.Distance(myPosition) < SlowTimeRange;
                if (isAnyCluster && !IsBubbleAtPosition(Core.Clusters.BestCluster.Position))
                {
                    position = Core.Clusters.BestCluster.Position;
                    return true;
                }

                if (!IsBubbleAtPosition(myPosition))
                {
                    position = MathEx.GetPointAt(myPosition, 10f, Player.Rotation);
                    return true;
                }
            }

            return false;
        }

        protected virtual bool ShouldTeleport(out Vector3 position)
        {
            // Note: This is for casting while not moving.  
            // Routine GetMovermentPower() may cast for movement
            // (Its called directly by our IPlayerMover)

            position = Vector3.Zero;

            if (!Skills.Wizard.Teleport.CanCast())
                return false;

            if (Sets.DelseresMagnumOpus.IsFullyEquipped && Legendary.Triumvirate.IsEquipped &&
                Passives.Wizard.ArcaneDynamo.IsActive && Core.Buffs.GetBuffStacks(SNOPower.Wizard_Passive_ArcaneDynamo) > 1)
                return false;

            if (IsInCombat && Runes.Wizard.SafePassage.IsActive && !Skills.Wizard.Teleport.IsBuffActive)
            {
                if (CanTeleportTo(CurrentTarget.Position) && !ZetaDia.Me.IsCasting)
                {
                    position = CanTeleportTo(CurrentTarget.Position) ? CurrentTarget.Position : Player.Position;
                    return true;
                }
            }

            return true;
        }

        // Mastery

        protected virtual bool ShouldExplosiveBlast(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Wizard.ExplosiveBlast.CanCast())
                return false;

            position = CurrentTarget.Position;
            return position != Vector3.Zero;
        }

        protected virtual bool ShouldMirrorImage()
        {
            if (!Skills.Wizard.MirrorImage.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(30f))
                return false;

            return true;
        }

        protected virtual bool ShouldArchon()
        {
            if (!Skills.Wizard.Archon.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(30f))
                return false;

            if (IsFirebirdsMeteorReviveUsed && Player.CurrentHealthPct < EmergencyHealthPct)
                return true;

            if (Sets.ChantodosResolve.IsFullyEquipped && ChantodosStacks < 20)
                return false;

            return true;
        }

        protected virtual bool ShouldBlackHole(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.BlackHole.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        // Archon

        protected virtual bool ShouldArchonBlast()
        {
            if (!Skills.Wizard.ArchonBlast.CanCast())
                return false;

            return true;
        }

        protected virtual bool ShouldArchonStrike(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.ArchonStrike.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldArchonDisintegrationWave(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.ArchonDisintegrationWave.CanCast())
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        protected virtual bool ShouldArchonTeleport(out Vector3 position)
        {
            // Note: This is for casting while not moving.  
            // Routine GetMovermentPower() may cast for movement
            // (Its called directly by our IPlayerMover)

            position = Vector3.Zero;

            if (!Skills.Wizard.ArchonTeleport.CanCast())
                return false;

            var target = TargetUtil.GetBestClusterUnit();
            if (target?.Distance > 10f && CurrentTarget.IsUnit)
                position = target.Position;

            return position != Vector3.Zero;
        }

        protected virtual bool ShouldArchonSlowTime()
        {
            if (!Skills.Wizard.ArchonSlowTime.CanCast())
                return false;

            if (!IsArchonSlowTimeActive)
                return true;

            return false;
        }

        #endregion

        #region Expressions 

        // Primary

        protected virtual TrinityPower MagicMissile(TrinityActor target)
            => new TrinityPower(Skills.Wizard.MagicMissile, 70f, target.AcdId);

        protected virtual TrinityPower ShockPulse(TrinityActor target)
            => new TrinityPower(Skills.Wizard.ShockPulse, 30f, target.AcdId);

        protected virtual TrinityPower SpectralBlade(TrinityActor target)
            => new TrinityPower(Skills.Wizard.SpectralBlade, 30f, target.AcdId);

        protected virtual TrinityPower Electrocute(TrinityActor target)
            => new TrinityPower(Skills.Wizard.Electrocute, 70f, target.AcdId);

        // Defensive

        protected virtual TrinityPower FrostNova()
            => new TrinityPower(Skills.Wizard.FrostNova);

        protected virtual TrinityPower DiamondSkin()
            => new TrinityPower(Skills.Wizard.DiamondSkin);

        // Secondary

        protected virtual TrinityPower ArcaneOrb(TrinityActor target)
            => new TrinityPower(Skills.Wizard.ArcaneOrb, 70f, target.AcdId);

        protected virtual TrinityPower RayOfFrost(TrinityActor target)
            => new TrinityPower(Skills.Wizard.RayOfFrost, 50f, target.AcdId, 25, 25);

        protected virtual TrinityPower ArcaneTorrent(TrinityActor target) // Prone to disconnects.
            => new TrinityPower(Skills.Wizard.ArcaneTorrent, 70f, target.AcdId, 25, 50);

        protected virtual TrinityPower Disintegrate(TrinityActor target)
            => new TrinityPower(Skills.Wizard.Disintegrate, 50f, target.AcdId, 75, 75);

        // Force

        protected virtual TrinityPower WaveOfForce()
            => new TrinityPower(Skills.Wizard.WaveOfForce);

        protected virtual TrinityPower WaveOfForce(TrinityActor actor)
            => new TrinityPower(Skills.Wizard.WaveOfForce, 14f, actor.AcdId);

        protected virtual TrinityPower EnergyTwister(TrinityActor target)
            => new TrinityPower(Skills.Wizard.EnergyTwister, 70f, target.AcdId);

        protected virtual TrinityPower Hydra(Vector3 position)
            => new TrinityPower(Skills.Wizard.Hydra, 60f, position);

        protected virtual TrinityPower Meteor(TrinityActor target)
            => new TrinityPower(Skills.Wizard.Meteor, 70f, target.AcdId);

        protected virtual TrinityPower Blizzard(TrinityActor target)
            => new TrinityPower(Skills.Wizard.Blizzard, 70f, target.AcdId);

        // Conjuration

        protected virtual TrinityPower IceArmor()
            => new TrinityPower(Skills.Wizard.IceArmor);

        protected virtual TrinityPower StormArmor()
            => new TrinityPower(Skills.Wizard.StormArmor);

        protected virtual TrinityPower EnergyArmor()
            => new TrinityPower(Skills.Wizard.EnergyArmor);

        protected virtual TrinityPower MagicWeapon()
            => new TrinityPower(Skills.Wizard.MagicWeapon);

        protected virtual TrinityPower Familiar()
            => new TrinityPower(Skills.Wizard.Familiar);

        // Defensive

        protected virtual TrinityPower SlowTime(Vector3 position)
            => new TrinityPower(Skills.Wizard.SlowTime, SlowTimeRange, position);

        protected virtual TrinityPower Teleport(Vector3 position)
            => IsArchonActive ? new TrinityPower(Skills.Wizard.ArchonTeleport, 50f, position)
                              : new TrinityPower(Skills.Wizard.Teleport, 50f, position);

        // Mastery

        protected virtual TrinityPower ExplosiveBlast()
            => new TrinityPower(Skills.Wizard.ExplosiveBlast);

        protected virtual TrinityPower ExplosiveBlast(Vector3 position)
            => new TrinityPower(Skills.Wizard.ExplosiveBlast, 10f, position);

        protected virtual TrinityPower MirrorImage()
            => new TrinityPower(Skills.Wizard.MirrorImage);

        protected virtual TrinityPower Archon()
            => new TrinityPower(Skills.Wizard.Archon);

        protected virtual TrinityPower BlackHole(TrinityActor target)
            => new TrinityPower(Skills.Wizard.BlackHole, 70f, target.AcdId);

        // Archon

        protected virtual TrinityPower ArchonDisintegrationWave(TrinityActor target)
            => new TrinityPower(Skills.Wizard.ArchonDisintegrationWave, 47f, target.AcdId, 25, 25);

        protected virtual TrinityPower ArchonBlast()
            => new TrinityPower(Skills.Wizard.ArchonBlast);

        protected virtual TrinityPower ArchonStrike(TrinityActor target)
            => new TrinityPower(Skills.Wizard.ArchonStrike, 14f, target.AcdId);

        protected virtual TrinityPower ArchoArchonTeleport(Vector3 position)
            => new TrinityPower(Skills.Wizard.ArchonTeleport, 50f, position);

        protected virtual TrinityPower ArchonSlowTime()
            => new TrinityPower(Skills.Wizard.ArchonSlowTime.SNOPower);

        // Misc

        protected static bool IsArchonActive
            => Core.Hotbar.ActivePowers.Any(p => GameData.ArchonSkillIds.Contains((int)p));

        protected bool IsSlowTimeActive
            => ZetaDia.Actors.GetActorsOfType<DiaObject>().Any(a => GameData.SlowTimeSNO.Contains(a.ActorSnoId));

        protected bool IsArchonSlowTimeActive
            => ZetaDia.Actors.GetActorsOfType<DiaObject>().Any(a => GameData.SlowTimeSNO.Contains(a.ActorSnoId) && a.Distance <= 5f);

        protected static float BlizzardRadius
            => Runes.Wizard.Apocalypse.IsActive ? 30f : 16f;

        public static float SlowTimeRange
            => Sets.DelseresMagnumOpus.IsFullyEquipped ? 15f : 57f;

        public static float RayOfFrostRange
            => Runes.Wizard.SleetStorm.IsActive ? 20f : 60f;

        public static float DisintegrateRange
            => Runes.Wizard.Entropy.IsActive ? 15f : 50f;

        public static bool CanTeleport
            => Skills.Wizard.Teleport.CanCast() || Skills.Wizard.ArchonTeleport.CanCast();

        public static int MaxHydras
            => Legendary.SerpentsSparker.IsEquipped ? 2 : 1;

        public static int ChantodosStacks
            => Core.Buffs.GetBuffStacks(SNOPower.P3_ItemPassive_Unique_Ring_021);

        public static bool IsFirebirdsMeteorReviveUsed
            => Core.Buffs.HasBuff(SNOPower.ItemPassive_Unique_Ring_732_x1);

        public static bool HasTalRashaStacks
            => TalRashaStacks >= 3;

        public static int TalRashaStacks
            => Core.Buffs.GetBuffStacks(SNOPower.P2_ItemPassive_Unique_Ring_028);

        public static bool IsChannellingDisintegrate
            => Player.IsChannelling && Skills.Wizard.Disintegrate.IsLastUsed;

        public static bool IsChannellingRayOfFrost
            => Player.IsChannelling && Skills.Wizard.RayOfFrost.IsLastUsed;

        public static void CancelArchon() =>
            Core.Buffs.GetBuff(SNOPower.Wizard_Archon).Cancel();

        #endregion

        #region Helpers

        protected bool TryPrimaryPower(out TrinityPower power)
        {
            power = null;

            if (ShouldElectrocute(out var target))
                power = Electrocute(target);

            else if (ShouldShockPulse(out target))
                power = ShockPulse(target);

            else if (ShouldMagicMissile(out target))
                power = MagicMissile(target);

            else if (ShouldSpectralBlade(out target))
                power = SpectralBlade(target);

            return power != null;
        }

        protected bool TrySecondaryPower(out TrinityPower power)
        {
            power = null;

            if (ShouldArcaneOrb(out var target))
                power = ArcaneOrb(target);

            else if (ShouldRayOfFrost(out target))
                power = RayOfFrost(target);

            else if (ShouldArcaneTorrent(out target))
                power = ArcaneTorrent(target);

            else if (ShouldDisintegrate(out target))
                power = Disintegrate(target);

            return power != null;
        }

        protected bool TrySpecialPower(out TrinityPower power)
        {
            power = null;

            if (ShouldTeleport(out var position))
                power = Teleport(position);

            else if (ShouldArchon())
                power = Archon();

            else if (ShouldDiamondSkin())
                power = DiamondSkin();

            else if (ShouldFrostNova())
                power = FrostNova();

            else if (ShouldSlowTime(out position))
                power = SlowTime(position);

            else if (ShouldMirrorImage())
                power = MirrorImage();

            else if (ShouldHydra(out position))
                power = Hydra(position);

            else if (ShouldBlizzard(out var target))
                power = Blizzard(target);

            else if (ShouldMeteor(out target))
                power = Meteor(target);

            else if (ShouldExplosiveBlast(out position))
                power = ExplosiveBlast(position);

            else if (ShouldBlackHole(out target))
                power = BlackHole(target);

            else if (ShouldEnergyTwister(out target))
                power = EnergyTwister(target);

            else if (ShouldWaveOfForce(out target))
                power = WaveOfForce(target);

            return power != null;
        }

        protected bool TryArmor(out TrinityPower power)
        {
            power = null;

            if (ShouldEnergyArmor())
                power = EnergyArmor();

            else if (ShouldStormArmor())
                power = StormArmor();

            else if (ShouldIceArmor())
                power = IceArmor();

            return power != null;
        }

        public bool TryBuffPower(out TrinityPower power)
        {
            power = null;

            if (IsArchonActive)
            {
                if (ShouldArchonSlowTime())
                    power = ArchonSlowTime();

                if (ShouldArchonBlast())
                    power = ArchonBlast();

                return power != null;
            }

            if (TryArmor(out power))
                return power != null;

            if (ShouldMagicWeapon())
                power = MagicWeapon();

            if (ShouldFamiliar())
                power = Familiar();

            return power != null;
        }

        public TrinityPower DefaultBuffPower()
        {
            return TryBuffPower(out var power) ? power : null;
        }

        public TrinityPower DefaultDestructiblePower()
        {
            if (IsArchonActive && Skills.Wizard.ArchonDisintegrationWave.CanCast())
            {
                return ArchonDisintegrationWave(CurrentTarget);
            }

            if (Skills.Wizard.MagicMissile.CanCast())
                return MagicMissile(CurrentTarget);

            if (Skills.Wizard.ShockPulse.CanCast())
                return ShockPulse(CurrentTarget);

            if (Skills.Wizard.SpectralBlade.CanCast())
                return SpectralBlade(CurrentTarget);

            if (Skills.Wizard.Electrocute.CanCast())
                return Electrocute(CurrentTarget);

            if (Skills.Wizard.ArcaneOrb.CanCast())
                return ArcaneOrb(CurrentTarget);

            if (Skills.Wizard.RayOfFrost.CanCast())
                return RayOfFrost(CurrentTarget);

            if (Skills.Wizard.ArcaneTorrent.CanCast())
                return ArcaneTorrent(CurrentTarget);

            if (Skills.Wizard.Disintegrate.CanCast())
                return Disintegrate(CurrentTarget);

            return DefaultPower;
        }

        protected bool TryPredictiveTeleport(Vector3 destination, out TrinityPower trinityPower)
        {
            trinityPower = null;

            var path = Core.DBNavProvider.CurrentPath;
            if (path != null && path.Contains(destination) && CanTeleport)
            {
                // The destination is often the next point along a jagged navigation path,
                // which could be very close. Instead of going to the given destination,
                // the idea here is to look through the path and find a better (further away) position.
                // It also advances the current path to skip the points we'll miss by teleporting to
                // prevent the bot from backtracking (DB's navigator doesn't update itself to reflect 
                // having past a point and clearing the path appears to be delayed and/or not working)

                var projectedPosition = IsBlocked
                    ? TrinityGrid.Instance.GetPathCastPosition(50f, true)
                    : TrinityGrid.Instance.GetPathWalkPosition(50f, true);

                if (projectedPosition != Vector3.Zero)
                {
                    var distance = projectedPosition.Distance(Player.Position);
                    var inFacingDirection = TrinityGrid.Instance.IsInPlayerFacingDirection(projectedPosition, 90);
                    if ((distance > 15f || IsBlocked && distance > 5f) && inFacingDirection)
                    {
                        trinityPower = Teleport(projectedPosition);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanTeleportTo(Vector3 destination)
        {
            if (destination == Vector3.Zero)
                return false;

            if (!CanTeleport)
                return false;

            var destinationDistance = destination.Distance(Core.Player.Position);
            if (destinationDistance < 15f && !PlayerMover.IsBlocked)
                return false;

            // Prevent moving away from stuff that needs to be interacted with.
            if (ZetaDia.Actors.GetActorsOfType<DiaGizmo>().Any(g => g.Distance < 10f && g.ActorInfo.GizmoType != GizmoType.DestroyableObject))
                return false;

            if (!IsArchonActive)
            {
                // Prevent moving somewhere we'll just kite away from.
                if (KiteMode != KiteMode.Never && KiteDistance > 0 && TargetUtil.AnyMobsInRangeOfPosition(destination, KiteDistance) && Player.CurrentHealthPct > KiteHealthPct)
                    return false;

                // Dont move from outside avoidance into avoidance.
                if (!Core.Avoidance.InAvoidance(ZetaDia.Me.Position) && Core.Avoidance.Grid.IsLocationInFlags(destination, AvoidanceFlags.Avoidance))
                    return false;
            }

            // Don't move into molten core/arcane.
            if (!Core.Avoidance.InCriticalAvoidance(ZetaDia.Me.Position) && Core.Avoidance.Grid.IsIntersectedByFlags(ZetaDia.Me.Position, destination, AvoidanceFlags.CriticalAvoidance))
                return false;

            if (Skills.Wizard.Teleport.TimeSinceUse < 200)
                return false;

            return true;
        }


        #endregion

    }

}
