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
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;

namespace Trinity.Routines.Wizard
{
    public class WizardParalysisArchonBlizzard : WizardBase, IRoutine
    {

        #region Definition
        public string DisplayName => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "塔维 5+3 暴风雪" : "Pralysis 5+3 Greater Rift Blizzard";
        public string Description => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "配装看下边链接" : "you can find the build with this url";
        public string Author => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "晚风清徐" : "Night Breeze";
        public string Version => "1.0.1";
        public string Url => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "http://db.178.com/d3/s/636467146" : "https://www.d3planner.com/122375086";

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
            // 锁定奥拉什
            TrinityActor target;
            Vector3 position = Vector3.Zero;

            if (IsArchonActive)
            {
                target = TargetUtil.BestRangedAoeUnit(75, 75) ?? CurrentTarget;


               if (Skills.Wizard.ArchonTeleport.CanCast())
                {
                    if (!Core.Buffs.HasInvulnerableShrine && Skills.Wizard.Archon.TimeSinceUse > 19000)
                        Core.Avoidance.Avoider.TryGetSafeSpot(out position, 30, 50, Player.Position);

                    if (!Core.Buffs.HasInvulnerableShrine && Core.Avoidance.InAvoidance(Core.Player.Position))
                    {
                        Core.Avoidance.Avoider.TryGetSafeSpot(out position, 25, 45, target.Position);

                        if (position == Vector3.Zero)
                            Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15, 40, Player.Position);
                    }
                        
                    if ((!Core.Buffs.HasInvulnerableShrine && (Player.CurrentHealthPct >= 1 && Player.ShieldHitpoints < Player.CurrentHealth) && Skills.Wizard.ArchonTeleport.TimeSinceUse > 3000) ||
                        (!Core.Buffs.HasInvulnerableShrine && Player.CurrentHealthPct < 1 && Skills.Wizard.ArchonTeleport.TimeSinceUse > 1500) ||
                        TargetUtil.AnyElitesInRange(10))
                        Core.Avoidance.Avoider.TryGetSafeSpot(out position, 30, 45, target.Position);

                    if (target.IsBoss && target.Distance < 30)
                        Core.Avoidance.Avoider.TryGetSafeSpot(out position, 35, 50, target.Position);

                    if (position != Vector3.Zero)
                        return Teleport(position);
                }
               
                if (Skills.Wizard.ArchonDisintegrationWave.CanCast())
                    return ArchonDisintegrationWave(target);

                return Walk(Avoider.SafeSpot);
            }


            target = TargetUtil.BestRangedAoeUnit(50) ?? CurrentTarget;


            if (Skills.Wizard.Archon.CanCast())
            {
                if (!HasTalRashaStacks)
                {
                    if (TalRashaStacks == 2 && Skills.Wizard.ArcaneTorrent.TimeSinceUse < 6000)
                    {
                        if (Skills.Wizard.Blizzard.TimeSinceUse < 6000)
                            return BlackHole(target);
                        if (Skills.Wizard.BlackHole.TimeSinceUse < 6000)
                            return Blizzard(target);
                    }
                }
                else
                {
                    return Archon();
                }
            }

            var blizzard = Skills.Wizard.Blizzard;
            if (blizzard.CanCast() && blizzard.TimeSinceUse > 6000)
                return Blizzard(target);

            var blackhole = Skills.Wizard.BlackHole;
            if ((blackhole.CanCast() && blackhole.TimeSinceUse > 6000))
                return BlackHole(target);

            if (TalRashaStacks >= 2)
            {
                return ArcaneTorrent(target);
            }

            return Walk(Avoider.SafeSpot);
        }
		
        public TrinityPower GetDefensivePower()
        {
            return null;
        }

        public TrinityPower GetBuffPower() 
        {
            if (!IsArchonActive)
            {
                if (!Skills.Wizard.EnergyArmor.IsBuffActive && Skills.Wizard.EnergyArmor.CanCast())
                    return EnergyArmor();

                if (!Skills.Wizard.MagicWeapon.IsBuffActive && Skills.Wizard.MagicWeapon.CanCast())
                    return MagicWeapon();

                if (Skills.Wizard.Archon.CanCast() && !Core.Player.IsInTown && HasTalRashaStacks)
                    return Archon();
            }
            else
            {
                if (!IsArchonSlowTimeActive && Skills.Wizard.ArchonSlowTime.CanCast())
                    return ArchonSlowTime();
                
                if (Core.Player.IsMoving && Skills.Wizard.ArchonBlast.CanCast())
                    return ArchonBlast();

            }

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
        public static WizardParalysisArchonBlizzardSettings Settings { get; } = new WizardParalysisArchonBlizzardSettings();

        public sealed class WizardParalysisArchonBlizzardSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _teleport;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(10)]
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


