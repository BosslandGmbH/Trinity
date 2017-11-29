using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;

namespace Trinity.Routines.Crusader
{

    public sealed class CrusaderAkkhanCondemn : CrusaderBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Crusader Akkhan Condemn Routine";
        public string Description => "";
        public string Author => "Nesox";
        public string Version => "0.1";
        public string Url => "https://www.icy-veins.com/d3/crusader-akkhan-condemn-build-patch-2-6-1-season-12";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.ArmorOfAkkhan, SetBonus.Third }
            },
            Items = new List<Item>
            {
                Legendary.FrydehrsWrath,
            },
        };

        #endregion

        /// <summary> Only cast in combat and the target is a unit </summary>
        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;

            if (!Skills.Crusader.Punish.IsBuffActive && ShouldPunish(out target))
                return Punish(target);

            if (ShouldCondemn())
                return Condemn();

            if (ShouldSlash(out target))
                return Slash(target);

            if (IsNoPrimary)
                return Walk(CurrentTarget);

            return null;
        }

        protected override bool ShouldCondemn()
        {
            if (!Skills.Crusader.Condemn.CanCast())
                return false;

            if (!Settings.SpamCondemn)
                if (!TargetUtil.AnyMobsInRange(14f))
                    return false;

            if (Legendary.FrydehrsWrath.IsEquipped && Player.PrimaryResource < 40)
                return false;

            return true;
        }

        protected override bool ShouldSlash(out TrinityActor target)
        {
            target = null;

            if (!Skills.Crusader.Slash.CanCast())
                return false;

            if (Skills.Crusader.Slash.IsLastUsed && IsMultiPrimary)
                return false;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }

        /// <summary> Only cast when avoiding. </summary>
        public TrinityPower GetDefensivePower()
        {
            return GetBuffPower();
        }

        /// <summary>
        /// Cast always, in and out of combat.
        /// </summary>
        public TrinityPower GetBuffPower()
        {
            if (Player.IsInTown)
                return null;

            if (AllowedToUse(Settings.Akarats, Skills.Crusader.AkaratsChampion) && ShouldAkaratsChampion())
                return AkaratsChampion();

            if (ShouldIronSkin())
                return IronSkin();

            if (ShouldProvoke())
                return Provoke();

            TrinityPower power;
            if (TryLaw(out power))
                return power;

            return null;
        }

        /// <summary>
        /// Only cast on destructibles/barricades
        /// </summary>
        public TrinityPower GetDestructiblePower()
        {
            return DefaultDestructiblePower();
        }

        /// <summary>
        /// Cast by all plugins for all movement.        
        /// </summary>
        public TrinityPower GetMovementPower(Vector3 destination)
        {
            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public int UnitsInRangeCondemn => Settings.UnitsInRangeCondemn;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public CrusaderAkkhanCondemnSettings Settings { get; } = new CrusaderAkkhanCondemnSettings();

        public sealed class CrusaderAkkhanCondemnSettings : NotifyBase, IDynamicSetting
        {
            private bool _spamCondemn;
            [DefaultValue(true)]
            public bool SpamCondemn
            {
                get { return _spamCondemn; }
                set { SetField(ref _spamCondemn, value); }
            }

            private int _clusterSize;
            [DefaultValue(1)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
            }

            private int _unitsInRangeCondemn;
            [DefaultValue(1)]
            public int UnitsInRangeCondemn
            {
                get { return _unitsInRangeCondemn; }
                set { SetField(ref _unitsInRangeCondemn, value); }
            }

            private float _emergencyHealthPct;
            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get { return _emergencyHealthPct; }
                set { SetField(ref _emergencyHealthPct, value); }
            }

            private SkillSettings _akarats;
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
