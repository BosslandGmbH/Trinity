﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
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
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Monk
{
    public sealed class MonkLoNWaveOfLight : MonkBase, IRoutine
    {
        #region Definition

        public string DisplayName => "LoN Wave Of Light";
        public string Description => "Fun build that doesnt really compete with Sunwuko for DPS and clear times, but is enjoyable to play with.";
        public string Author => " TwoCigars";
        public string Version => "Beta 1.3";
        public string Url => "http://www.d3planner.com/120704732";
        
        // Build lacks Damage and Spirit regen for many builds. Still need to find the best combination of gear/skills to use. 

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.LegacyOfNightmares, SetBonus.First },
            },
			Items = new List<Item>
            {
                Legendary.PintosPride,
				Legendary.TzoKrinsGaze,
				Legendary.KyoshirosBlade,
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Monk.WaveOfLight, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            Vector3 position;

            if (TrySpecialPower(out power))
                return power;            

            if (Core.Buffs.HasCastingShrine)
            {
                if (Skills.Monk.BlindingFlash.CanCast() && Legendary.TheLawsOfSeph.IsEquipped && Player.PrimaryResource < Player.PrimaryResourceMax - 165)
                    return BlindingFlash();

                if (Skills.Monk.MysticAlly.CanCast() && Runes.Monk.AirAlly.IsActive && Player.PrimaryResource < Player.PrimaryResourceMax - 200)
                    return MysticAlly();

                if (Skills.Monk.DashingStrike.CanCast() && !Skills.Monk.DashingStrike.IsLastUsed)
                    return DashingStrike(CurrentTarget.Position);

                if (Skills.Monk.WaveOfLight.CanCast())
                    return WaveOfLight(CurrentTarget);
            }

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            if (IsNoPrimary)
            {
                // Stay away from hostile units when Regen skills are on cooldown and under [50] Spirit
                if (Skills.Monk.BlindingFlash.IsActive && Legendary.TheLawsOfSeph.IsEquipped || Skills.Monk.MysticAlly.IsActive && Runes.Monk.AirAlly.IsActive)
                {
                    var regenOnCooldown = !Skills.Monk.BlindingFlash.CanCast() && !Skills.Monk.MysticAlly.CanCast();
                    var needResource = Player.PrimaryResource < PrimaryEnergyReserve;
                    if ((regenOnCooldown || needResource) && HostileMonsters.Any(u => u.Distance <= 12f))
                    {
                        Logger.Log(LogCategory.Routine, "Moving away - Low Spirit - Regen on Cooldown");
                        return Walk(TargetUtil.GetLoiterPosition(CurrentTarget, 30f));
                    }
                }
            }

            return Walk(TargetUtil.GetLoiterPosition(CurrentTarget, 20f));
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();
        public TrinityPower GetBuffPower() => DefaultBuffPower();
        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (HasInstantCooldowns && Skills.Monk.DashingStrike.CanCast() && Skills.Monk.DashingStrike.TimeSinceUse > 200 && destination.Distance(Player.Position) > 18f)
				return DashingStrike(destination);
			
            if (AllowedToUse(Settings.DashingStrike, Skills.Monk.DashingStrike) && CanDashTo(destination))
            {
                var distance = destination.Distance(Player.Position);

                // Dont dash towards a unit
                var isDestinationUnitTarget = IsInCombat && CurrentTarget.IsUnit && destination.Distance(CurrentTarget.Position) < 15f;

                if (distance > 35f && !isDestinationUnitTarget || IsBlocked)
                {
                    return DashingStrike(destination);
                }
            }

            return Walk(destination);
        }

        protected override bool ShouldBlindingFlash()
        {
            if (!Skills.Monk.BlindingFlash.CanCast())
                return false;

            //Prioritize using Blinding Flash for Spirit Regeneration
            if (Legendary.TheLawsOfSeph.IsEquipped)
                return Player.PrimaryResource < Player.PrimaryResourceMax - 165;

            if (!TargetUtil.AnyMobsInRange(20f))
                return false;

            if (Player.CurrentHealthPct < 0.5f)
                return true;

            return true;
        }

        protected override bool ShouldMysticAlly()
        {
            if (!Skills.Monk.MysticAlly.CanCast())
                return false;

            //Prioritize using Mystic Ally for Spirit Regeneration when missing 200 Spirit
            if (Runes.Monk.AirAlly.IsActive)
                return Player.PrimaryResource < Player.PrimaryResourceMax - 200;

            if (!TargetUtil.AnyMobsInRange(50f))
                return false;

            //Still check for buff for when not using Air Ally Rune
            if (Core.Buffs.HasBuff(SNOPower.X1_Monk_MysticAlly_v2))
                return false;

            return true;
        }

        protected override bool ShouldWaveOfLight(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.WaveOfLight.CanCast())
            {
                return false;
            }  
			
            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;

            var isBigCluster = TargetUtil.ClusterExists(Settings.WoLRange, Settings.ClusterSize);
            var isEliteInRange = TargetUtil.AnyElitesInRange(Settings.WoLEliteRange);
            var isFarTooMuchResource = Player.PrimaryResourcePct > 0.8f;

            if (isBigCluster || isEliteInRange || isFarTooMuchResource)
            {
                target = TargetUtil.GetBestClusterUnit();
                return target != null;
            }
			
            if (IsBlocked || IsStuck)
            {
                target = TargetUtil.GetClosestUnit(Settings.WoLRange) ?? CurrentTarget;
            }
            else
            {
                target = TargetUtil.GetBestClusterUnit(Settings.WoLRange) ?? CurrentTarget;
            }
            return true;
        }			
		
        protected override bool ShouldDashingStrike(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Monk.DashingStrike.CanCast())
                return false;

            var skill = Skills.Monk.DashingStrike;
            if (skill.TimeSinceUse < 3000 && skill.Charges < MaxDashingStrikeCharges && !Core.Buffs.HasCastingShrine)
                return false;

            if (!AllowedToUse(Settings.DashingStrike, Skills.Monk.DashingStrike))
                return false;

            if (Skills.Monk.DashingStrike.TimeSinceUse < 500)
                return false;

            var unit = TargetUtil.GetBestClusterUnit(50f);
            if (unit == null || unit.Distance < 15f)
                return false;

            return unit.Position != Vector3.Zero;
        }

        protected override bool ShouldEpiphany()
        {
            if (!Skills.Monk.Epiphany.CanCast())
                return false;

            if (HasInstantCooldowns && !Skills.Monk.Epiphany.IsLastUsed)
                return true;

            if (Core.Buffs.HasBuff(SNOPower.X1_Monk_Epiphany))
                return false;

            if (!AllowedToUse(Settings.Epiphany, Skills.Monk.Epiphany))
                return false;

            return true;
        }	
		
        #region Settings
	
        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;
		public override float TrashRange => Settings.WoLRange;
		public override float EliteRange => Settings.WoLEliteRange;		

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public MonkLoNWaveOfLightSettings Settings { get; } = new MonkLoNWaveOfLightSettings();

        public sealed class MonkLoNWaveOfLightSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _epiphany;
            private SkillSettings _dashingStrike;
            private int _clusterSize;
			private float _woLRange;
			private float _woLEliteRange;			
            private float _emergencyHealthPct;

            [DefaultValue(50)]
            public float WoLRange
            {
                get { return _woLRange; }
                set { SetField(ref _woLRange, value); }
            }			
			
            [DefaultValue(65)]
            public float WoLEliteRange
            {
                get { return _woLEliteRange; }
                set { SetField(ref _woLEliteRange, value); }
            }				
			
            [DefaultValue(6)]
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

            public SkillSettings Epiphany
            {
                get { return _epiphany; }
                set { SetField(ref _epiphany, value); }
            }

            public SkillSettings DashingStrike
            {
                get { return _dashingStrike; }
                set { SetField(ref _dashingStrike, value); }
            }

            #region Skill Defaults

            private static readonly SkillSettings EpiphanyDefaults = new SkillSettings
            {
                UseMode = UseTime.Selective,
                Reasons = UseReasons.Elites | UseReasons.HealthEmergency
            };

            private static readonly SkillSettings DashingStrikeDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 2000,
                Reasons = UseReasons.Blocked
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Epiphany = EpiphanyDefaults.Clone();
                DashingStrike = DashingStrikeDefaults.Clone();
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

