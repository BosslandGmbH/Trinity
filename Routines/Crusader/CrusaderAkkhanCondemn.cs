using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Common.Helpers;
using Zeta.Game.Internals.Actors;

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
            TrinityActor target;

            Vector3 buffPosition;
            if (ShouldWalkToGroundBuff(out buffPosition))
                return Walk(buffPosition);

            if (ShouldShieldGlare(out target))
                return ShieldGlare(target);

            if (ShouldPunish(out target))
                return Punish(target);

            if (ShouldShieldGlare(out target))
                return ShieldGlare(target);

            if (ShouldSlash(out target))
                return Slash(target);

            return Walk(CurrentTarget);
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

            if (ShouldCondemn())
                return Condemn();

            if (ShouldIronSkin())
                return IronSkin();

            if (ShouldProvoke())
                return Provoke();

            if (ShouldJudgement())
                return Judgement();

            TrinityPower power;
            if (TryLaw(out power))
                return power;

            return null;
        }

        private Vector3 _lastBuffPosition;
        readonly WaitTimer _groundBuffWalkTimer = WaitTimer.FiveSeconds;
        private bool ShouldWalkToGroundBuff(out Vector3 buffPosition)
        {
            buffPosition = Vector3.Zero;
            if (!Settings.MoveToGroundBuffs || CurrentTarget == null)
                return false;

            if (_lastBuffPosition != Vector3.Zero && _lastBuffPosition.Distance2D(CurrentTarget.Position) > 20)
                return false;

            if (_lastBuffPosition != Vector3.Zero && Player.Position.Distance2D(_lastBuffPosition) > 9f && !_groundBuffWalkTimer.IsFinished)
            {
                Core.Logger.Log($"Moving to buff: {_lastBuffPosition} - Distance: {Player.Position.Distance2D(_lastBuffPosition)}");
                return true;
            }

            _lastBuffPosition = Vector3.Zero;
            
            Vector3 bestBuffedPosition;
            var bestClusterPoint = TargetUtil.GetBestClusterPoint();

            if (TargetUtil.BestBuffPosition(40f, bestClusterPoint, false, out bestBuffedPosition) &&
                bestBuffedPosition != Vector3.Zero)
            {
                Core.Logger.Log($"Found buff: {bestBuffedPosition} - Distance: {Player.Position.Distance2D(bestBuffedPosition)}");
                buffPosition = bestBuffedPosition;
                if (bestBuffedPosition != Vector3.Zero)
                {
                    _lastBuffPosition = bestBuffedPosition;
                    _groundBuffWalkTimer.Reset();
                    return true;
                }
            }

            return false;
        }

        #region Overrides

        protected override bool ShouldJudgement()
        {
            if (!Skills.Crusader.Judgment.CanCast())
                return false;

            return true;
        }

        protected override bool ShouldIronSkin()
        {
            if (!Skills.Crusader.IronSkin.CanCast())
                return false;

            if (Settings.SpamIronSkin)
                return true;

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

            if (Settings.SpamLawsOfValor)
                return true;

            if (!TargetUtil.AnyMobsInRange(25f))
                return false;

            return true;
        }

        protected override bool ShouldProvoke()
        {
            if (!Skills.Crusader.Provoke.CanCast())
                return false;

            if (Settings.SpamProvoke)
                return true;

            if (Player.HasBuff(SNOPower.X1_Crusader_Provoke))
                return false;

            if (!TargetUtil.AnyMobsInRange(15f))
                return false;

            return true;
        }

        protected override bool ShouldCondemn()
        {
            if (!Skills.Crusader.Condemn.CanCast())
                return false;

            if (Legendary.FrydehrsWrath.IsEquipped && Player.PrimaryResource < 40)
                return false;

            if (Settings.SpamCondemn)
                return true;

            if (!TargetUtil.AnyMobsInRange(30f))
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

        #endregion

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
            if (AllowedToUse(Settings.SteedCharge, Skills.Crusader.SteedCharge) && ShouldSteedCharge())
                return SteedCharge();

            return Walk(destination);
        }

        #region Settings


        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public CrusaderAkkhanCondemnSettings Settings { get; } = new CrusaderAkkhanCondemnSettings();

        public sealed class CrusaderAkkhanCondemnSettings : NotifyBase, IDynamicSetting
        {
            private bool _moveToGroundBuffs;
            [DefaultValue(false)]
            public bool MoveToGroundBuffs
            {
                get { return _moveToGroundBuffs; }
                set { SetField(ref _moveToGroundBuffs, value); }
            }

            private bool _spamCondemn;
            [DefaultValue(true)]
            public bool SpamCondemn
            {
                get { return _spamCondemn; }
                set { SetField(ref _spamCondemn, value); }
            }

            private bool _spamLawsOfValor;
            [DefaultValue(true)]
            public bool SpamLawsOfValor
            {
                get { return _spamLawsOfValor; }
                set { SetField(ref _spamLawsOfValor, value); }
            }

            private bool _spamProvoke;
            [DefaultValue(true)]
            public bool SpamProvoke
            {
                get { return _spamProvoke; }
                set { SetField(ref _spamProvoke, value); }
            }

            private bool _spamIronSkin;
            [DefaultValue(true)]
            public bool SpamIronSkin
            {
                get { return _spamIronSkin; }
                set { SetField(ref _spamIronSkin, value); }
            }

            private int _clusterSize;
            [DefaultValue(1)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
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

            private SkillSettings _steedCharge;
            public SkillSettings SteedCharge
            {
                get { return _steedCharge; }
                set { SetField(ref _steedCharge, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings AkaratsDefaults = new SkillSettings
            {
                UseMode = UseTime.Always,
                Reasons = UseReasons.Elites | UseReasons.HealthEmergency,
            };

            private static readonly SkillSettings SteedChargeDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
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
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this, true);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}
