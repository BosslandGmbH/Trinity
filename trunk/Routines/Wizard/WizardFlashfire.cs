using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;


namespace Trinity.Routines.Wizard
{
    public sealed class WizardFlashfire : WizardBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Flash Fire Speed Farm";
        public string Description => "Wizard routine intended for non-grift speed farming materials";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/flashfire-wizard-wand-of-woh-build-with-the-tal-rasha-set-patch-2-4-2-season-7";

        public override KiteMode KiteMode => KiteMode.Never;       

        // Ignore everything except molten core.
        public override Func<bool> ShouldIgnoreAvoidance => () 
            => !Core.Avoidance.InCriticalAvoidance(Core.Player.Position);

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.TalRashasElements, SetBonus.Third },
                //{ Sets.FirebirdsFinery, SetBonus.Second }
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Wizard.Teleport, null },
                { Skills.Wizard.ExplosiveBlast, null },
            },
            Items = new List<Item>
            {
                Legendary.WandOfWoh
            },
        };

        #endregion
        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;

            if (TrySpecialPower(out power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            // Force moving close to units before casting.
            if (CurrentTarget.Distance > 15f && Player.PrimaryResourcePct > 0.3f)
                return Walk(CurrentTarget);

            if (TryPrimaryPower(out power))
                return power;

            return null;
        }

        public TrinityPower GetDefensivePower() => null;

        public TrinityPower GetBuffPower() 
        {
            // ty RobbenHoschi

            if (Skills.Wizard.ExplosiveBlast.CanCast())
                return ExplosiveBlast();
            if (Skills.Wizard.DiamondSkin.CanCast())
                return DiamondSkin();
            if (Skills.Wizard.FrostNova.CanCast() && (TargetUtil.ClusterExists(25f, ClusterSize) || TargetUtil.AnyElitesInRange(40f)))
                return FrostNova();
            return DefaultBuffPower();
        }

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            // todo: add some predicted points at longer distance on navigator path
            // to prevent tiny teleports to next small segment.

            if (AllowedToUse(Settings.Teleport, Skills.Wizard.Teleport) && CanTeleportTo(destination))
                return Teleport(destination);

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WizardFlashfireSettings Settings { get; } = new WizardFlashfireSettings();

        public sealed class WizardFlashfireSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _teleport;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(8)]
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

            public SkillSettings Teleport
            {
                get { return _teleport; }
                set { SetField(ref _teleport, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings TeleportDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 200,
                Reasons = UseReasons.Blocked
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Teleport = TeleportDefaults.Clone();
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


