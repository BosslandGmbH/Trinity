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
    public sealed class CrusaderInvoker : CrusaderBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Crusader Invoker Routine";
        public string Description => "The Thorns of the Invoker set unlocks a powerful, retaliation-based playstyle with what used to be the laughing stock of stats, Thorns. This tanky, utility and cooldown-oriented playstyle is available in both solo progression and speedfarming variations, explained in that order.";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/crusader-endgame-thorns-build-with-the-invoker-set-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.ThornsoftheInvoker, SetBonus.Third }
            },
            Items = new List<Item>
            {
                Legendary.Hack,
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
            if (AllowedToUse(Settings.SteedCharge, Skills.Crusader.SteedCharge) && ShouldSteedCharge())
                return SteedCharge();

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public CrusaderInvokerSettings Settings { get; } = new CrusaderInvokerSettings();

        public sealed class CrusaderInvokerSettings : NotifyBase, IDynamicSetting        
        {
            private SkillSettings _akarats;
            private SkillSettings _steedCharge;
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

            public SkillSettings SteedCharge
            {
                get { return _steedCharge; }
                set { SetField(ref _steedCharge, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings AkaratsDefaults = new SkillSettings
            {
                UseTime = UseTime.Selective,
                Reasons = UseReasons.Elites | UseReasons.HealthEmergency,
            };

            private static readonly SkillSettings SteedChargeDefaults = new SkillSettings
            {
                UseTime = UseTime.AnyTime,
                Reasons = UseReasons.Blocked
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Akarats = AkaratsDefaults.Clone();
                SteedCharge = SteedChargeDefaults.Clone();
            }

            #region IDynamicSetting

            public string GetName() => GetType().Name;
            public UserControl GetControl() => UILoader.LoadXamlByFileName<UserControl>(GetName() + ".xaml");
            public object GetDataContext() => this;
            public string GetCode() => JsonSerializer.Serialize(this);
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}


