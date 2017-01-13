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
        public override float KiteDistance => 15f;

        public override Func<bool> ShouldIgnoreAvoidance => () => IsArchonActive;
        #endregion
        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;

			var bestCluster = TargetUtil.GetBestClusterPoint();
            var energyLevelReady = !Legendary.AquilaCuirass.IsEquipped || Player.PrimaryResourcePct > 0.95;
            
            if (IsArchonActive)
            {                
                if (CanTeleport)
                    Teleport(TargetUtil.GetBestClusterPoint());

                if (Skills.Wizard.ArchonDisintegrationWave.CanCast())
                {
                    // Use Wave to pull and ignite monsters that are lined up nicely and are not burning.
                    var pierceUnits = WeightedUnits.Where(u => u.Distance < 50f && !u.Attributes.HasFirebirdPermanent && !u.Attributes.HasFirebirdTemporary && (u.CountUnitsInFront() + u.CountUnitsBehind(15f)) > 5).ToList();
                    var bestPierceUnit = pierceUnits.OrderBy(u => u.Distance).FirstOrDefault(u => u.Distance <= 15f);
                    if (bestPierceUnit != null)
                        return ArchonDisintegrationWave(bestPierceUnit);
                }

                if (ShouldArchonStrike(out target))
                    return ArchonStrike(target);

                return null;
            }
			//frost nova to proc tal rasha
			
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
		//cast frost nova frequently to proc Tal's
		protected override bool ShouldFrostNova()
		{
			if (!Skills.Wizard.FrostNova.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(15f))
                return false;

            return true;
        }
		protected override bool ShouldTeleport(out Vector3 position)
		{
			//Teleport into clusters if calamity is equipped
			
			position = Vector3.Zero;
			
			if (!Skills.Wizard.Teleport.CanCast())
				return false;
			
			if (Skills.Wizard.Teleport.TimeSinceUse < 2000)
				return false;
			
			var closeEnoughToBestCluster = TargetUtil.GetBestClusterUnit(15f, 50f)?.Distance < 15f;
			if (!IsInCombat && closeEnoughToBestCluster)
				return false;
			
			position = TargetUtil.GetBestClusterPoint();
			return position != Vector3.Zero;
		}
		
        public TrinityPower GetDefensivePower() => null;

        public TrinityPower GetBuffPower() 
        {
			return DefaultBuffPower();
			
        }

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (AllowedToUse(Settings.Teleport, Skills.Wizard.Teleport) && CanTeleportTo(destination))
                return Teleport(destination);

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


