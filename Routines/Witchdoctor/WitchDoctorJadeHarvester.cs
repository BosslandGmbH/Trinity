using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game.Internals.Actors;


namespace Trinity.Routines.Witchdoctor
{
    public sealed class WitchDoctorJadeHarvester : WitchDoctorBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Jade Harvester";
        public string Description => "Patch 2.4 marks the return of Raiment of the Jade Harvester as one of the top tier Witch Doctor playstyles. This bursty, melee range spellcaster playstyle works well for solo progression and speedfarming.";
        public string Author => "phelon, xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/witch-doctor-build-with-the-jade-harvester-set-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.RaimentOfTheJadeHarvester, SetBonus.Third },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            // Ported from Phelon's Jade Harvester Routine

            TrinityPower power;
            TrinityActor target;
            Vector3 position;

            if (Settings.HarvestModeIC == HarvestMode.NonStop && Skills.WitchDoctor.SoulHarvest.CanCast() && TargetUtil.AnyMobsInRange(14f))
                return SoulHarvest();
                
            if (ShouldPiranhas(out target))
                return Piranhas(target);

            if (ShouldSoulHarvest(out position))
            {
                if (Skills.WitchDoctor.SpiritWalk.CanCast())
                    return SpiritWalk();

                return SoulHarvest(position);
            }

            if (ShouldWallOfDeath(out target))
                return WallOfDeath(target);

            if (ShouldLocustSwarm(out target))
                return LocustSwarm(target);

            if (ShouldHaunt(out target))
                return Haunt(target);

            if (TrySpecialPower(out power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            if (Player.HasBuff(SNOPower.Witchdoctor_SpiritWalk))
            {
                if (Player.CurrentHealthPct < EmergencyHealthPct &&
                    TargetUtil.BestWalkLocation(35f, true).Distance(Player.Position) > 5)
                    return new TrinityPower(SNOPower.Walk, 7f, TargetUtil.BestWalkLocation(45f, true));

                if (TargetUtil.BestDpsPosition(TargetUtil.BestAoeUnit(45, true).Position, 45f, true).Distance(Player.Position) > 5)
                    return new TrinityPower(SNOPower.Walk, 3f,
                        TargetUtil.BestDpsPosition(TargetUtil.BestAoeUnit(45, true).Position, 45f, true));
            }

            return new TrinityPower(SNOPower.Walk, 3f,
                TargetUtil.BestDpsPosition(TargetUtil.BestAoeUnit(45, true).Position, 45f, true));
        }


        protected override bool ShouldSoulHarvest(out Vector3 position)
        {
            // We don't need to restrict harvest too much the CD with set reductions is very quick.

            position = Vector3.Zero;

            if (!Skills.WitchDoctor.SoulHarvest.CanCast())
                return false;

            var needStacks = Skills.WitchDoctor.SoulHarvest.BuffStacks <= 2;
            if (!needStacks)
            {
                var primaryTargetHaunted = CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Haunt);
                var primaryTargetSwarmed = CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Locust_Swarm);

                if (!primaryTargetHaunted || !primaryTargetSwarmed)
                    return false;
            }

            position = TargetUtil.GetBestClusterPoint();
            return position != Vector3.Zero;
        }

        protected override bool ShouldWallOfDeath(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.WallOfDeath.CanCast())
                return false;

            if (Skills.WitchDoctor.WallOfDeath.TimeSinceUse < 2500)
                return false;

            target = TargetUtil.BestAoeUnit(35, true);
            return target != null;
        }


        protected override bool ShouldHaunt(out TrinityActor target)
        {
            target = null;

            if (!Skills.WitchDoctor.Haunt.CanCast())
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            if (!CurrentTarget.HasDebuff(SNOPower.Witchdoctor_Haunt) || CurrentTarget.IsBoss)
            {
                target = CurrentTarget;
                return target != null;
            }

            List<TrinityActor> unitsNotHaunted;
            var percentHaunted = TargetUtil.DebuffedPercent(SNOPower.Witchdoctor_Haunt, 20f, out unitsNotHaunted);
            if (Player.PrimaryResourcePct < 0.4f && percentHaunted >= 0.60f)
                 return false;

            if (!unitsNotHaunted.Any() && Player.PrimaryResourcePct > 0.75 && TargetUtil.GetBestClusterPoint().Distance(Player.Position) < 10f)
                target = CurrentTarget;
            else
                target = unitsNotHaunted.FirstOrDefault();

            return target != null;
        }

        protected override bool ShouldHorrify()
        {
            if (!Skills.WitchDoctor.Horrify.CanCast())
                return false;

            if (Skills.WitchDoctor.Horrify.IsBuffActive)
                return false;

            var safeOutOfCombat = !IsInCombat && Player.CurrentHealthPct > 0.5f;
            if (safeOutOfCombat)
                return false;

            return true;
        }

        protected override TrinityPower Piranhas(TrinityActor target)
            => new TrinityPower(SNOPower.Witchdoctor_Piranhas, 45, target.Position);

        protected override TrinityPower WallOfDeath(TrinityActor target)
            => new TrinityPower(SNOPower.Witchdoctor_WallOfZombies, 45, target.Position);

        protected override TrinityPower Haunt(TrinityActor target)
            => new TrinityPower(SNOPower.Witchdoctor_Haunt, 35f, target.AcdId);

        public TrinityPower GetBuffPower()
        {
            if (ShouldHorrify())
                return Horrify();

            if (Settings.SpiritWalk.UseMode == UseTime.Always && !Player.IsInTown && Skills.WitchDoctor.SpiritWalk.CanCast())
                return SpiritWalk();

            var closeUnit = HostileMonsters.FirstOrDefault(u => u.Distance < 16f);
            if (closeUnit != null && !Player.IsInTown && !IsInCombat && Skills.WitchDoctor.SoulHarvest.CanCast())
            {                
                if (Settings.HarvestModeOOC == HarvestMode.NonStop)
                {                                
                    if (Skills.WitchDoctor.Haunt.CanCast() && Skills.WitchDoctor.Haunt.TimeSinceUse > 1000)
                        return Haunt(closeUnit);

                    if(HostileMonsters.Any(u => u.Distance < 16f && u.HasDebuff(SNOPower.Witchdoctor_Haunt)))
                        return SoulHarvest();                                         
                }

                // Refresh the buff time to avoid losing 10 stacks.
                if (Skills.WitchDoctor.SoulHarvest.TimeSinceUse > 4500)
                    return SoulHarvest();

                // Build some stacks
                if (Skills.WitchDoctor.SoulHarvest.BuffStacks < 10)
                    return SoulHarvest();                                   
            }

            return DefaultBuffPower();
        }

        public TrinityPower GetDefensivePower() => null;
        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();
        public TrinityPower GetMovementPower(Vector3 destination)
        {
            Vector3 position;

            if (!Player.IsInTown)
            {
                if (Runes.WitchDoctor.AngryChicken.IsActive && ShouldHex(out position))
                    return Hex(position);
            }

            return Walk(destination);
        }

        #region Settings

        public override KiteMode KiteMode => KiteMode.Never;
        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WitchDoctorJadeHarvesterSettings Settings { get; } = new WitchDoctorJadeHarvesterSettings();

        public enum HarvestMode
        {
            None = 0,
            Default,
            NonStop,
        }

        public sealed class WitchDoctorJadeHarvesterSettings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private float _emergencyHealthPct;
            private HarvestMode _harvestModeOoc;
            private HarvestMode _harvestModeIc;
            private SkillSettings _spiritWalk;

            [DefaultValue(HarvestMode.Default)]
            public HarvestMode HarvestModeOOC
            {
                get => _harvestModeOoc;
                set => SetField(ref _harvestModeOoc, value);
            }

            [DefaultValue(HarvestMode.Default)]
            public HarvestMode HarvestModeIC
            {
                get => _harvestModeIc;
                set => SetField(ref _harvestModeIc, value);
            }

            [DefaultValue(8)]
            public int ClusterSize
            {
                get => _clusterSize;
                set => SetField(ref _clusterSize, value);
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get => _emergencyHealthPct;
                set => SetField(ref _emergencyHealthPct, value);
            }

            public SkillSettings SpiritWalk
            {
                get => _spiritWalk;
                set => SetField(ref _spiritWalk, value);
            }

            private static readonly SkillSettings DefaultSpiritWalkSettings = new SkillSettings
            {
                UseMode = UseTime.Default,
            };

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                SpiritWalk = DefaultSpiritWalkSettings.Clone();
            }

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this, true);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}


