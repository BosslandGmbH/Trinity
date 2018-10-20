using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Components.Coroutines;
using Trinity.Framework.Objects;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Trinity.Framework.Reference;

namespace Trinity.Routines.Wizard
{
    public sealed class WohExplosiveBlast : WizardBase, IRoutine
    {
        #region Definition
        public string DisplayName => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "沃尔贤者气息速刷专用" : "WohExplosiveBlast Nephalem Rift Beta";
        public string Description => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "平均一小时死亡之息可以获得600-800个，如果装备足够好的话，可以速刷T12" : "You can got every hour 600-800 death breath.Godly gears can farm T12.";
        public string Author => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "晚风清徐" : "Night Breeze";
        public string Version => "1.0.0";
        public string Url => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "http://db.178.com/d3/s/632908684" : "http://www.d3planner.com/822977979";

        #region Build Definition
        public Build BuildRequirements => new Build
        {
            // 套装检查
            Sets = new Dictionary<Set, SetBonus>
            {
                // 塔拉夏的法理 3件套效果
                { Sets.TalRashasElements, SetBonus.Third },
                // 意志壁垒 2件套效果
                { Sets.BastionsOfWill, SetBonus.First },

            },
            // 技能检查
            Skills = new Dictionary<Skill, Rune>
            {
                // 冰霜新星-冰冻迷雾
                { Skills.Wizard.FrostNova, Runes.Wizard.FrozenMist },
                // 魔法武器-原力武器
                { Skills.Wizard.MagicWeapon, Runes.Wizard.ForceWeapon },
                // 幽魂之刃-飞掷之刃
                { Skills.Wizard.SpectralBlade, Runes.Wizard.ThrownBlade },
                // 聚能爆破-连锁效应
                { Skills.Wizard.ExplosiveBlast, Runes.Wizard.ChainReaction },
                // 钻石体肤-节能棱镜
                { Skills.Wizard.DiamondSkin, Runes.Wizard.Prism},
                // 传送-灾厄降临
                { Skills.Wizard.Teleport, Runes.Wizard.Calamity}
            },
            // 物品检查
            Items = new List<Item>
            {
                // 沃尔魔杖
                Legendary.WandOfWoh,
                // 无尽深渊法珠
                Legendary.GlovesOfWorship,
                // 归赋肩垫
                Legendary.HomingPads,
                // 复仇者护腕
                Legendary.NemesisBracers,
                // 寅剑
                Legendary.Ingeom,
                // 金织带
                Legendary.Goldwrap,
                // 皇家华戒
                Legendary.RingOfRoyalGrandeur,

            }
        };
        #endregion
        public override KiteMode KiteMode => KiteMode.Never;
        public override float KiteDistance => 0f;

        public override Func<bool> ShouldIgnoreAvoidance => () => true;
        #endregion

        public override async Task<bool> HandleAvoiding()
        {
            if (Core.Player.Actor == null || !IsAvoidanceRequired) return false;

            var safe = (!Core.Player.IsTakingDamage || Core.Player.CurrentHealthPct > 0.5f) && Core.Player.Actor != null && !Core.Player.Actor.IsInCriticalAvoidance;
            if (!TrinityCombat.IsInCombat && Core.Player.Actor.IsAvoidanceOnPath && safe)
            {
                Core.Logger.Log(LogCategory.Avoidance, "Waiting for avoidance to clear (out of combat)");
                return await MoveTo.Execute(Core.Avoidance.Avoider.SafeSpot, "Safe Spot", 5f, () => !IsAvoidanceRequired);
            }

            Core.Logger.Log(LogCategory.Avoidance, "Avoiding");
            return await MoveTo.Execute(Core.Avoidance.Avoider.SafeSpot, "Safe Spot", 5f, () => !IsAvoidanceRequired);
        }

        public TrinityPower GetOffensivePower()
        {
            if (Player.IsCastingPortal)
                return null;

            if (Player.IsInRift && ZetaDia.Storage.RiftCompleted && !Player.IsCastingPortal && !TargetUtil.AnyMobsInRange(12) && InventoryManager.Backpack.Any(i => i.ActorSnoId == 408416))
            {
                ZetaDia.Me.UseTownPortal();
                return null;
            }

            // 锁定奥拉什
            var target = Core.Actors.Actors.FirstOrDefault(u => u.ActorSnoId == 360636) ?? (TargetUtil.BestEliteInRange(50) ?? TrinityCombat.Targeting.CurrentTarget);

            if (target.Distance > 15 && Skills.Wizard.Teleport.CanCast())
                return Teleport(target.Position);

            if (target.Distance > 10)
                return Walk(target.Position);

            if (target.IsUnit && target.IsValid && !target.IsDead)
                return SpectralBlade(target);

            return null;
        }

        public TrinityPower GetDefensivePower()
        {
            return null;
        }

        public TrinityPower GetBuffPower()
        {
            if (Player.IsInTown)
                return null;

            if (Player.IsCastingPortal)
                return null;

            if (Player.IsInRift && ZetaDia.Storage.RiftCompleted && !Player.IsCastingPortal && !TargetUtil.AnyMobsInRange(12) && InventoryManager.Backpack.Any(i => i.ActorSnoId == 408416))
            {
                ZetaDia.Me.UseTownPortal();
                return null;
            }

            if (!Skills.Wizard.MagicWeapon.IsBuffActive && Skills.Wizard.MagicWeapon.CanCast())
                return MagicWeapon();
            if (Skills.Wizard.ExplosiveBlast.CanCast())
                return ExplosiveBlast();
            if (Skills.Wizard.DiamondSkin.CanCast())
                return DiamondSkin();
            return Skills.Wizard.FrostNova.CanCast() ? FrostNova() : null;
        }

        public TrinityPower GetDestructiblePower()
        {
            return null;
        }

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (Player.IsCastingPortal)
                return null;

            if (Player.IsInRift && ZetaDia.Storage.RiftCompleted && !Player.IsCastingPortal && !TargetUtil.AnyMobsInRange(12) && InventoryManager.Backpack.Any(i => i.ActorSnoId == 408416))
            {
                ZetaDia.Me.UseTownPortal();
                return null;
            }

            if (Skills.Wizard.Teleport.CanCast() && destination.Distance(Player.Position) > 15)
                return Teleport(destination);

            return Walk(destination);
        }


        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WohExplosiveBlastSettings Settings { get; } = new WohExplosiveBlastSettings();

        public sealed class WohExplosiveBlastSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _teleport;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(6)]
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


