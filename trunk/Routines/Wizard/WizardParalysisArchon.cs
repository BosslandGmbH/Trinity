using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using IronPython.Modules;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Routines.Crusader;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Wizard
{
    public sealed class WizardParalysisArchon : WizardBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Vyr/Rasha Paralysis Archon";
        public string Description => "Wizard routine intended for high-level grifts";
        public string Author => "Unexpected with help from TwoCigars";
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
                Logger.Log("Teleport to Archon Combust");
                position = TargetUtil.GetBestClusterPoint();
                return position != Vector3.Zero;
            }

            // Jump away from monsters but within cast range
            if (Avoider.TryGetSafeSpot(out position, 15f, 50f, Player.Position, 
                n => n.AvoidanceFlags.HasFlag(AvoidanceFlags.Monster) 
                && Core.Grids.CanRayCast(n.NavigableCenter, CurrentTarget.Position)))
            {
                Logger.Log("Teleport to Safespot (ShouldTeleport)");
                return true;
            }

            // Try a different approach.
            var target = TargetUtil.GetBestClusterUnit();
            position = TargetUtil.GetLoiterPosition(target, 30f);
            if (position != Vector3.Zero)
		    {
                Logger.Log("Teleport to LoiterPosition (ShouldTeleport)");
                return true;
            }

		    return false;
		}
		
        public TrinityPower GetDefensivePower()
        {
            Vector3 position;

            if (ShouldTeleport(out position))
                    return Teleport(position);

            return null;
        }

        protected override bool ShouldArchon() => false;

        public TrinityPower GetBuffPower()
        {
            Vector3 position;

            if (!IsArchonActive && Skills.Wizard.Archon.CanCast() && !Player.IsInTown)
            {
                if (!HasTalRashaStacks)
                {
                    //Logger.Log($"Building Tal'Rasha Set Stacks ({TalRashaStacks})");

                    var target = TargetUtil.GetBestClusterUnit(50f) ?? Player.Actor;

                    if (Skills.Wizard.FrostNova.CanCast())
                        return FrostNova();

                    if (Skills.Wizard.Blizzard.CanCast())
                        return Blizzard(target);

                    if (Skills.Wizard.BlackHole.CanCast())
                        return BlackHole(target);

                    if (ShouldTeleport(out position))
                        return Teleport(position);

                    if (Skills.Wizard.ArcaneTorrent.CanCast())
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


