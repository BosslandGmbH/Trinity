using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;
using Trinity.Framework.Reference;

namespace Trinity.Routines.Wizard
{
    public class WizardParalysisArchonHydra : WizardBase, IRoutine
    {

        #region Definition
        public string DisplayName => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "塔维 5+3 电蛇" : "Pralysis 5+3 Greater Rift Hydra";
        public string Description => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "配装看下边链接" : "you can find the build with this url";
        public string Author => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "晚风清徐" : "Night Breeze";
        public string Version => "1.0.0";
        public string Url => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "http://db.178.com/d3/s/877378168" : "http://www.d3planner.com/812244416";

        #region Build Definition
        public Build BuildRequirements => new Build
        {
            // 套装检查
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.TalRashasElements, SetBonus.Third },
                { Sets.VyrsAmazingArcana, SetBonus.Second }
            },
            // 技能检查
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Wizard.Archon, null },
                { Skills.Wizard.Hydra, null }
            },
            // 物品检查
            Items = new List<Item>
            {
                Legendary.ManaldHeal,
                Legendary.SerpentsSparker,
                Legendary.RingOfRoyalGrandeur,
                Legendary.ObsidianRingOfTheZodiac,
                Legendary.TheSwami

            }
        };
        #endregion
        public override KiteMode KiteMode => IsArchonActive ? KiteMode.Never : (KiteMode.Elites | KiteMode.Bosses);
        public override float KiteDistance => 25;
        public override Func<bool> ShouldIgnoreKiting => () => IsArchonActive;

        public override Func<bool> ShouldIgnoreAvoidance => () => IsArchonActive;

        #endregion

        public TrinityPower GetOffensivePower()
        {
            var position = Vector3.Zero;
            var target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;

            if (!Core.Buffs.HasInvulnerableShrine && (Skills.Wizard.ArchonTeleport.CanCast() || Skills.Wizard.Teleport.CanCast()))
            {
                var ArchonTeleport = Skills.Wizard.ArchonTeleport;
                if (Core.Avoidance.InAvoidance(Player.Position))
                    Core.Avoidance.Avoider.TryGetSafeSpot(out position, 30, 50);

                if (Player.CurrentHealthPct < 1 && ArchonTeleport.TimeSinceUse > 1500)
                    Core.Avoidance.Avoider.TryGetSafeSpot(out position, 30, 50);

                if (TargetUtil.AnyElitesInRange(15) && ArchonTeleport.TimeSinceUse > 2000)
                    Core.Avoidance.Avoider.TryGetSafeSpot(out position, 30, 50);

                if (TargetUtil.AnyBossesInRange(30))
                    Core.Avoidance.Avoider.TryGetSafeSpot(out position, 30, 50);

                if (TargetUtil.NumMobsInRange(15) > 3)
                    Core.Avoidance.Avoider.TryGetSafeSpot(out position, 30, 50);

                if (Skills.Wizard.ArchonTeleport.TimeSinceUse > 3000)
                    Core.Avoidance.Avoider.TryGetSafeSpot(out position, 30, 50);

                if (position != Vector3.Zero)
                    return Teleport(position);
            }


            if (IsArchonActive)
            {
                return ArchonDisintegrationWave(target);
            }

            if (!IsArchonActive)
            {
                if (Skills.Wizard.Archon.CanCast() && TalRashaStacks >= 3 && Player.Summons.HydraCount >= 2)
                    return Archon();


                var archon = Skills.Wizard.Archon;
                var hydra = Skills.Wizard.Hydra;
                var teleport = Skills.Wizard.Teleport;
                var rayOfFrost = Skills.Wizard.RayOfFrost;

                if (Skills.Wizard.Archon.CanCast() && TalRashaStacks < 3)
                {
                    if (Player.Summons.HydraCount < 2 || hydra.TimeSinceUse > 6000)
                        return Hydra(target.Position);

                    if (rayOfFrost.TimeSinceUse > 6000)
                        return RayOfFrost(target);

                    if (teleport.TimeSinceUse > 6000 && Core.Avoidance.Avoider.TryGetSafeSpot(out position, 20, 50))
                        return Teleport(position);
                }


                if (Skills.Wizard.Hydra.CanCast() && Player.Summons.HydraCount < 2 && IsInCombat)
                    return Hydra(target.Position);

                if (Skills.Wizard.RayOfFrost.CanCast() && Player.PrimaryResource > 15)
                    return RayOfFrost(target);
            }

            return Walk(Avoider.SafeSpot);
        }
		
        public TrinityPower GetDefensivePower()
        {
            return null;
        }

        public TrinityPower GetBuffPower() 
        {
            return DefaultBuffPower();
        }

        public TrinityPower GetDestructiblePower()
        {
            return DefaultDestructiblePower();
        }


        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if ((Skills.Wizard.ArchonTeleport.CanCast() || Skills.Wizard.Teleport.CanCast()) && destination.Distance(Core.Player.Position) > 15)
            {
                return Teleport(destination);
            }

            return Walk(destination);
        }


        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public static WizardParalysisArchonHydraSettings Settings { get; } = new WizardParalysisArchonHydraSettings();

        public sealed class WizardParalysisArchonHydraSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _teleport;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(6)]
            public int ClusterSize
            {
                get { return _clusterSize; }
                set { SetField(ref _clusterSize, value); }
            }

            [DefaultValue(0.6f)]
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


