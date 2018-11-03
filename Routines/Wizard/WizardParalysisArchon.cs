using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;


namespace Trinity.Routines.Wizard
{
    public sealed class WizardParalysisArchon : WizardBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Vyr/Rasha Paralysis Archon";
        public string Description => "Wizard routine intended for high-level grifts";
        public string Author => "Unexpected/TwoCigars";
        public string Version => "0.4";
        public string Url => "http://www.diablofans.com/builds/85544-s9-2-4-3-paralysis-vyrrasha-t13-speed-rifts";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.TalRashasElements, SetBonus.Third },
                //{ Sets.VyrsAmazingArcana, SetBonus.Second }
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Wizard.Teleport, null },
				{ Skills.Wizard.FrostNova, null }
            },
            Items = new List<Item>
            {
                Legendary.ManaldHeal
            },
        };

		public override KiteMode KiteMode => KiteMode.Always;
        public override float KiteDistance => 25f;
        public override int KiteStutterDuration => 800;
        public override int KiteStutterDelay => 1400;
        public override int KiteHealthPct => 100;

        public override Func<bool> ShouldIgnoreAvoidance => () => IsArchonActive && Player.CurrentHealthPct > 0.7f;

        #endregion
        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;

            if (IsArchonActive)
            {
                if (!IsArchonSlowTimeActive)
                    return ArchonSlowTime();

                if (ShouldArchonDisintegrationWave(out target))
                    return ArchonDisintegrationWave(target);
            }
            else
            {
                if (TrySpecialPower(out power))
                    return power;

                if (TrySecondaryPower(out power))
                    return power;

                if (TryPrimaryPower(out power))
                    return power;     
            }
            return Walk(Avoider.SafeSpot);
        }

		protected override bool ShouldFrostNova()
		{
            //cast frost nova frequently to proc Tal's

            if (!Skills.Wizard.FrostNova.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(15f))
                return false;

            return true;
        }

		protected override bool ShouldTeleport(out Vector3 position)
		{
			position = Vector3.Zero;
			
			if (!CanTeleport)
				return false;

            // Jump into cluster for archon combust
            if (!IsArchonActive && Skills.Wizard.Archon.CanCast() && TalRashaStacks >= 2 && Runes.Wizard.Combustion.IsActive)
		    {
                Core.Logger.Log(LogCategory.Routine, "Teleport to Archon Combust");
                position = TargetUtil.GetBestClusterPoint();
                return position != Vector3.Zero;
            }

            // Jump away from monsters but within cast range
            if (Avoider.TryGetSafeSpot(out position, 15f, 50f, Player.Position, 
                n => n.AvoidanceFlags.HasFlag(AvoidanceFlags.Monster) 
                && TrinityGrid.Instance.CanRayCast(n.NavigableCenter, CurrentTarget.Position)))
            {
                Core.Logger.Log(LogCategory.Routine,"Teleport to Safespot (ShouldTeleport)");
                return true;
            }

            // Try a different approach.
            var target = TargetUtil.GetBestClusterUnit();
            position = TargetUtil.GetLoiterPosition(target, 30f);
            if (position != Vector3.Zero && target != null && TargetUtil.AnyMobsInRange(20f, 3) || TargetUtil.AnyElitesInRange(20f))
		    {
                Core.Logger.Log(LogCategory.Routine, "Teleport to LoiterPosition (ShouldTeleport)");
                return true;
            }

		    return false;
		}
		
        public TrinityPower GetDefensivePower()
        {
            if (ShouldTeleport(out var position))
                    return Teleport(position);

            return null;
        }

        protected override bool ShouldArchon() => false;

        public TrinityPower GetBuffPower()
        {
            if (!IsArchonActive && Skills.Wizard.Archon.CanCast() && !Player.IsInTown)
            {
                if (Settings.GetStacksBeforeArchon && !HasTalRashaStacks)
                {
                    //Core.Logger.Log($"Building Tal'Rasha Set Stacks ({TalRashaStacks})");

                    var target = TargetUtil.GetBestClusterUnit(70f) ?? Player.Actor;
                    if (target == null || !target.IsValid)
                        return DefaultBuffPower();

                    if (Skills.Wizard.FrostNova.CanCast())
                        return FrostNova();

                    if (Skills.Wizard.Blizzard.CanCast())
                        return Blizzard(target);

                    if (Skills.Wizard.BlackHole.CanCast())
                        return BlackHole(target);

                    if (TalRashaStacks == 2 && Skills.Wizard.Teleport.CanCast())
                        return Teleport(target.Position);
                    
                    // AT is never on cooldown so we can use it last.
                    if (TalRashaStacks == 3 && Skills.Wizard.ArcaneTorrent.CanCast())
                        return ArcaneTorrent(target);
                }
                else
                {
                    return Archon();
                }
            }

            return DefaultBuffPower();
        }

        protected override TrinityPower ArchonDisintegrationWave(TrinityActor target)
            => new TrinityPower(Skills.Wizard.ArchonDisintegrationWave, 55f, target.AcdId, 25, 25);

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (AllowedToUse(Settings.Teleport, Skills.Wizard.Teleport))
            {
                TrinityPower trinityPower;
                if (TryPredictiveTeleport(destination, out trinityPower))
                    return trinityPower;
            }

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WizardParalysisArchonSettings Settings { get; } = new WizardParalysisArchonSettings();

        public sealed class WizardParalysisArchonSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _teleport;
            private int _clusterSize;
            private float _emergencyHealthPct;
            private bool _getStacksBeforeArchon;

            [DefaultValue(8)]
            public int ClusterSize
            {
                get => _clusterSize;
                set => SetField(ref _clusterSize, value);
            }

            [DefaultValue(true)]
            public bool GetStacksBeforeArchon
            {
                get => _getStacksBeforeArchon;
                set => SetField(ref _getStacksBeforeArchon, value);
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get => _emergencyHealthPct;
                set => SetField(ref _emergencyHealthPct, value);
            }

            public SkillSettings Teleport
            {
                get => _teleport;
                set => SetField(ref _teleport, value);
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


