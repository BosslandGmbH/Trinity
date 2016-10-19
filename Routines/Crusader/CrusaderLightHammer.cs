using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.UI;
using Zeta.Common;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Crusader
{
    public sealed class CrusaderLightHammer : CrusaderBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Holy Light Hammerdin";
        public string Description => "The Seeker of the Light set marks the return of the Hammerdin build, decidedly focusing on bringing Blessed Hammer Blessed Hammer to competitive DPS in Greater Rifts — aided and protected by Falling Sword Falling Sword. This mobile, sustain damage playstyle is available in both solo progression and speedfarming.";
        public string Author => "xzjv";
        public string Version => "0.1";
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
            TrinityPower power;
            TrinityActor target;

            if (!Skills.Crusader.Punish.IsBuffActive && ShouldPunish(out target))
                return Punish(target);
                
            if (TrySpecialPower(out power))
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

            if (ShouldIronSkin())
                return IronSkin();

            if (ShouldJudgement())
                return Judgement();

            if (ShouldConsecration())
                return Consecration();

            if (ShouldProvoke())
                return Provoke();

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

            // Its not efficient really to falling sword for movement.
            if (TargetUtil.AnyMobsInRangeOfPosition(destination, 20f, 5) && ShouldFallingSword(out position))
                return FallingSword(destination);

            if (ShouldSteedCharge())
                return SteedCharge();

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public CrusaderLightHammerSettings Settings { get; } = new CrusaderLightHammerSettings();

        public sealed class CrusaderLightHammerSettings : NotifyBase, IDynamicSetting        
        {
            private SkillSettings _akarats;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(10)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get { return _emergencyHealthPct; }
                set { SetField(ref _emergencyHealthPct, value); }
            }

            public SkillSettings Akarats
            {
                get { return _akarats; }
                set { SetField(ref _akarats, value); }
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


