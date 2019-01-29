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
using Trinity.Framework.Grid;

namespace Trinity.Routines.Wizard
{
    public sealed class WizardParalysisArchonHydra : WizardBase, IRoutine
    {

        #region Definition
        public string DisplayName => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "塔维 5+3 电蛇" : "Pralysis 5+3 Greater Rift Hydra";
        public string Description => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "配装看下边链接" : "you can find the build with this url";
        public string Author => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "晚风清徐" : "Night Breeze";
        public string Version => "1.0.1";
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
            TrinityActor target;

            if (IsArchonActive)
            {
                target = TargetUtil.SafeList(true)
                             .FirstOrDefault(a => a.IsBoss && !a.IsShadowClone && a.Distance < 125f) ??
                         TargetUtil.SafeList(true)
                             .Where(a => a.IsElite &&
                                         a.EliteType != EliteTypes.Minion &&
                                         !a.IsIllusion &&
                                         a.Distance < 125f)
                             .OrderBy(a => a.NearbyUnitsWithinDistance(6f))
                             .FirstOrDefault() ??
                         TargetUtil.SafeList(true)
                             .Where(a => a.IsTrashMob &&
                                         a.IsInLineOfSight &&
                                         !a.IsSummoner &&
                                         !a.IsSummoned &&
                                         a.Distance < 50f)
                             .OrderByDescending(a => a.NearbyUnitsWithinDistance(6f))
                             .FirstOrDefault() ??
                         CurrentTarget;
            }
            else
            {
                target = TargetUtil.SafeList(true)
                             .FirstOrDefault(a => a.IsBoss &&
                                                  !a.IsShadowClone &&
                                                  a.Distance < 125f) ??
                         TargetUtil.SafeList(true)
                             .Where(a => a.IsMonster &&
                                         a.Distance < 50f &&
                                         a.IsInLineOfSight)
                             .OrderBy(a => a.Distance)
                             .FirstOrDefault(a => TrinityGrid.Instance.CanRayWalk(Player.Position, a.Position)) ??
                         CurrentTarget;
            }

            if (target == null)
                return null;

            if (position != Vector3.Zero)
                return Teleport(position);

            Skill teleport = Skills.Wizard.Teleport;
            if (Skills.Wizard.ArchonTeleport.CanCast() ||
                Skills.Wizard.Teleport.CanCast())
            {
                teleport = IsArchonActive ? Skills.Wizard.ArchonTeleport : Skills.Wizard.Teleport;
                var needTeleport = !Core.Buffs.HasInvulnerableShrine &&
                                   Core.Avoidance.InAvoidance(Player.Position) ||
                                   !Core.Buffs.HasInvulnerableShrine &&
                                   Player.CurrentHealthPct < 1 &&
                                   teleport.TimeSinceUse > 1500 ||
                                   TargetUtil.AnyElitesInRange(15) &&
                                   teleport.TimeSinceUse > 2000 ||
                                   TargetUtil.AnyBossesInRange(30) ||
                                   TargetUtil.NumMobsInRange() > 3 ||
                                   teleport.TimeSinceUse > 3000;

                if (needTeleport)
                {
                    if (position == Vector3.Zero)
                        Core.Avoidance.Avoider.TryGetSafeSpot(out position, 30, 45);

                    return Teleport(position);
                }
            }

            if (IsArchonActive)
                return ArchonDisintegrationWave(target);

            if (Skills.Wizard.Archon.CanCast() &&
                TalRashaStacks >= 3 &&
                Player.Summons.HydraCount >= 2)
            {
                return Archon();
            }

            var hydra = Skills.Wizard.Hydra;
            var rayOfFrost = Skills.Wizard.RayOfFrost;

            var elite = TargetUtil.SafeList(true)
                .Where(a => a.IsElite &&
                            a.IsInLineOfSight &&
                            a.EliteType != EliteTypes.Minion &&
                            !a.IsIllusion &&
                            a.Distance < 50f)
                .OrderByDescending(a => a.Distance)
                .FirstOrDefault();

            if (Skills.Wizard.Archon.CanCast() &&
                TalRashaStacks < 3)
            {
                if (Player.Summons.HydraCount < 2 ||
                    hydra.TimeSinceUse > 6000)
                {
                    return Hydra(elite?.Position ?? target.Position);
                }

                if (rayOfFrost.TimeSinceUse > 6000)
                    return RayOfFrost(target);

                if (teleport.TimeSinceUse > 6000 &&
                    Core.Avoidance.Avoider.TryGetSafeSpot(out position, 20, 50))
                {
                    return Teleport(position);
                }
            }

            if (Skills.Wizard.Hydra.CanCast() &&
                Player.Summons.HydraCount < 2 &&
                IsInCombat)
            {
                return new TrinityPower(Skills.Wizard.Hydra, 50, elite ?? target);
            }

            if (Skills.Wizard.RayOfFrost.CanCast() &&
                Player.PrimaryResource > 15)
            {
                return RayOfFrost(target);
            }

            return Walk(Player.Position);
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

                if (Skills.Wizard.Archon.CanCast() && !Core.Player.IsInTown)
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
        public static WizardParalysisArchonHydraSettings Settings { get; } = new WizardParalysisArchonHydraSettings();

        public sealed class WizardParalysisArchonHydraSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _teleport;
            private int _clusterSize;
            private float _emergencyHealthPct;
            private bool _getStacksBeforeArchon;

            [DefaultValue(true)]
            public bool GetStacksBeforeArchon
            {
                get => _getStacksBeforeArchon;
                set => SetField(ref _getStacksBeforeArchon, value);
            }

            [DefaultValue(10)]
            public int ClusterSize
            {
                get => _clusterSize;
                set => SetField(ref _clusterSize, value);
            }

            [DefaultValue(0.6f)]
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

            private static readonly SkillSettings s_teleportDefaults = new SkillSettings
            {
                UseMode = UseTime.Default,
                RecastDelayMs = 200,
                Reasons = UseReasons.Blocked
            };

            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                Teleport = s_teleportDefaults.Clone();
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


