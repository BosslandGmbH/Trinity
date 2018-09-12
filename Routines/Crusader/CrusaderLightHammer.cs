using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game.Internals.Actors;


namespace Trinity.Routines.Crusader
{
    public sealed class CrusaderLightHammer : CrusaderBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Seeker of The Light Hammerdin";
        public string Description => "The Seeker of the Light set marks the return of the Hammerdin build, decidedly focusing on bringing Blessed Hammer Blessed Hammer to competitive DPS in Greater Rifts — aided and protected by Falling Sword Falling Sword. This mobile, sustain damage playstyle is available in both solo progression and speedfarming.";
        public string Author => "xzjv, TwoCigars";
        public string Version => "0.5";
        public string Url => "http://www.icy-veins.com/d3/crusader-holy-hammerdin-build-with-blessed-hammer-and-seeker-of-the-light-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.SeekerOfTheLight, SetBonus.Third }
            },
            Items = new List<Item>
            {
                Legendary.JohannasArgument,
                Legendary.GuardOfJohanna,
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            if (TrySpecialPower(out var power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            if (IsNoPrimary)
                return Walk(CurrentTarget);

            return null;
        }

        public TrinityPower GetDefensivePower()
        {
            return GetBuffPower();
        }

        public TrinityPower GetBuffPower()
        {
            TrinityPower power;

            if (IsSteedCharging)
                return null;

            if (AllowedToUse(Settings.Akarats, Skills.Crusader.AkaratsChampion) && ShouldAkaratsChampion())
                return AkaratsChampion();

            if (ShouldProvoke())
                return Provoke();

            if (ShouldIronSkin())
                return IronSkin();

            if (TryLaw(out power))
                return power;

            return null;
        }

        public TrinityPower GetDestructiblePower()
        {
            return DefaultDestructiblePower();
        }

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            Vector3 position;

            //Delay settings in ShouldFallingSword will help keep this from being spammed on a Cooldown Pylon
            //Default range of 20 yards increased to 50 yards. It will search for any 5 mob cluster within 50 yards
            if (TargetUtil.AnyMobsInRangeOfPosition(destination, 50f, 5) && ShouldFallingSword(out position))
                return FallingSword(destination);

            if (ShouldSteedCharge())
                return SteedCharge();

            return Walk(destination);
        }

protected override bool ShouldFallingSword(out Vector3 position)
{
    position = Vector3.Zero;

    if (!Skills.Crusader.FallingSword.CanCast())
        return false;

    //If your health falls below the Emergency Health Percentage in Trinity > Routine settings, cast falling sword again regardless of delay setting.
    if (Player.CurrentHealthPct < Settings.EmergencyHealthPct)
        return true;

            //Uses the delay [in milliseconds] defined in Trinity > Routines to keep falling sword from being recast too quickly - Added check for mobs being in Range
            if (Skills.Crusader.FallingSword.TimeSinceUse < Settings.FallingSwordDelay && TargetUtil.AnyMobsInRange(Settings.FallingSwordMobsRange))
        return false;

    var target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
    if (target != null)
    {
        position = target.Position;
        return true;
    }

    return false;
}

        protected override bool ShouldBlessedHammer(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.BlessedHammer.CanCast())
                return false;

            //Do not cast Blessed Hammer if Wrath is less than Primary Energy Reserve [default = 25]
            //This will conserve enough energy for Falling Sword to be cast again
            if (Player.PrimaryResource <= PrimaryEnergyReserve)
                return false;

            if (!TargetUtil.AnyMobsInRange(10f))
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }


        protected override bool ShouldProvoke()
        {
            if (!Skills.Crusader.Provoke.CanCast())
                return false;

            if (Player.HasBuff(SNOPower.X1_Crusader_Provoke))
                return false;

            if (!TargetUtil.AnyMobsInRange(15f))
                return false;

            return true;
        }

        protected override bool ShouldIronSkin()
        {
            if (!Skills.Crusader.IronSkin.CanCast())
                return false;

            if (Player.HasBuff(SNOPower.X1_Crusader_IronSkin))
                return false;

            if (!TargetUtil.AnyMobsInRange(20f))
                return false;

            return true;
        }

        protected override bool ShouldLawsOfValor()
        {
            if (!Skills.Crusader.LawsOfValor.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(25f))
                return false;

            return true;
        }

        protected override bool ShouldLawsOfJustice()
        {
            if (!Skills.Crusader.LawsOfJustice.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(25f))
                return false;

            return true;
        }

        protected override bool ShouldLawsOfHope()
        {
            if (!Skills.Crusader.LawsOfHope.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(25f))
                return false;

            return true;
        }

        protected override bool ShouldAkaratsChampion()
        {
            if (!Skills.Crusader.AkaratsChampion.CanCast())
                return false;

            return true;
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public CrusaderLightHammerSettings Settings { get; } = new CrusaderLightHammerSettings();

        public sealed class CrusaderLightHammerSettings : NotifyBase, IDynamicSetting        
        {
            private SkillSettings _akarats;
            private int _fallingSwordDelay;
            private int _fallingSwordMobsRange;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(10)]
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

            public SkillSettings Akarats
            {
                get => _akarats;
                set => SetField(ref _akarats, value);
            }

            [DefaultValue(8000)]
            public int FallingSwordDelay
            {
                get => _fallingSwordDelay;
                set => SetField(ref _fallingSwordDelay, value);
            }
            
            [DefaultValue(15)]
            public int FallingSwordMobsRange
            {
                get => _fallingSwordMobsRange;
                set => SetField(ref _fallingSwordMobsRange, value);
            }


            #region Skill Defaults

            private static readonly SkillSettings AkaratsDefaults = new SkillSettings
            {
                UseMode = UseTime.Selective,
                Reasons = UseReasons.Elites | UseReasons.HealthEmergency,
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Akarats = AkaratsDefaults.Clone();
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


