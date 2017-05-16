using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Routines.DemonHunter
{
    public sealed class DH_ShadowThreeKnifes : DemonHunterBase, IRoutine
    {
        #region Definition

        //English by Google Translate.

        public string DisplayName => "Shadow Three Knives (Impale) 暗影三刀战斗策略";
        public string Description => "暗影套，佳妮匕首，圣力剑袋.";
        public string Author => "太阳帝国";
        public string Version => "0.3";
        public string Url => "见暗黑3各相关论坛--DH板块";

        //  public override Func<bool> ShouldIgnoreAvoidance => ShouldFan;
        //  public override Func<bool> ShouldIgnoreKiting => ShouldFan;

        //public override int KiteStutterDuration => 1500;
        //public override int KiteStutterDelay => 1000;

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.TheShadowsMantle, SetBonus.Third },
            },
            Items = new List<Item>
            {
                Legendary.HolyPointShot,
                Legendary.KarleisPoint,
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.DemonHunter.Impale, null },
                { Skills.DemonHunter.ShadowPower, null }
            },
        };

        #endregion


        public override KiteMode KiteMode => KiteMode.Never;
        //  public override float KiteDistance => 25f;
        public override Func<bool> ShouldIgnoreAvoidance => () => true;

        public bool IsReallyBlocked => (PlayerMover.IsBlocked && Core.BlockedCheck.BlockedTime.TotalMilliseconds > 500) || Core.StuckHandler.IsStuck;

        public TrinityPower GetOffensivePower()
        {
            //TrinityActor target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            IEnumerable<TrinityActor> clusters = TargetUtil.GetBestClusterUnits(15f, 65f, true, true, false, false); // 选择非护盾，非反伤怪。
            TrinityActor target = (from u in clusters
                                   where !u.IsInvulnerable && !u.IsReflectingDamage
                                   select u).FirstOrDefault();
            if (target == null)
                target = CurrentTarget;




            Vector3 position = Vector3.Zero;
            //Core.Logger.Log(LogCategory.Routine, $"在---  战斗  ---中");
            // 先判断需要跑路的条件
            // 1.在致命AOE中
            // 2.血量太低
            // 需要跳到安全点

            // 拉近距离，好打怪
            // 3.有精英且距离精英太远
            // 4.有boss且距离boss太远
            // 5.周围多少码没精英

            // 选取精英，施放技能
            // 假如判断 技能是狼吼的话，在打精英/boss 时开启
            // 增加卡位判断


            //first judge the conditions that need to be run
            //in the fatal AOE
            //2.Blood is too low
            //need to jump to the safety point
            //The following are the same as the "
            //Close the distance, good Daguai
            //3.Elite and too far away from the elite
            //4.boss and too far from the boss
            //5.how many yards around no elite
            //The following are the same as the "
            //Choose elites and cast skills
            // If the judgment skills is wolf roar, then hit the elite / boss open
            //increase the card bit to judge


            if (Player.CurrentHealthPct < 0.4 || Core.Avoidance.InAvoidance(Player.Position) || Skills.DemonHunter.Vault.TimeSinceUse > 8000.0)
            {
                Core.Logger.Log(LogCategory.Routine, "躲避致命怪物技能 Avoid the fatal monster skills");
                if (ShouldVault(out position) && CanVaultTo(position))
                    return Vault(position);
                return Walk(position);
            }

            if (Player.CurrentHealthPct < 0.5 || TargetUtil.NumMobsInRange(15f) > 4 || CurrentTarget.IsBoss || CurrentTarget.IsElite)
            {
                if (Skills.DemonHunter.FanOfKnives.CanCast())
                    return FanOfKnives();

                if (Skills.DemonHunter.Companion.CanCast())
                    return Companion();
            }


            if (target.Distance > 17f && target != null)
            {
                Core.Logger.Log(LogCategory.Routine, $"与目标距离过远，拉近距离打怪 And the target is too far away, closer to Daguai distance");
                return CanVaultTo(target.Position) ? Vault(target.Position) : Walk(target.Position);

            }
            if (target.Distance > 15f && target != null)
                return Walk(target.Position, 7f);

            if (IsStuck || IsBlocked)//TargetUtil.NumMobsInRange(15f)>6)
            {
                Core.Logger.Log(LogCategory.Routine, $"战斗中卡住了，攻击最近怪物 The battle was stuck, attacking the recent monster");
                target = TargetUtil.GetClosestUnit(10f);
                if (target != null)
                    return Impale(target);
                else Vault(Avoider.SafeSpot);
            }

            if (Skills.DemonHunter.Impale.CanCast())
            {
                Core.Logger.Log(LogCategory.Routine, "攻击怪物 Attack monster");
                return Impale(target);
            }

            Core.Logger.Log(LogCategory.Routine, "啥都不做 Nothing to do");
            return null;
        }

        public TrinityPower GetBuffPower()
        {
            //Core.Logger.Log(LogCategory.Routine, $"在  Buff  中");

            // 施放暗影之力
            // 施放复仇
            // 施放护甲刀扇
            // 施放野猪战宠


            // cast the power of the shadow
            // cast revenge
            // cast armor knife fan
            // cast wild boar pet

            if (ShouldShadowPower())
                return ShadowPower();

            if (Skills.DemonHunter.Vengeance.CanCast() && !Player.IsInTown)
                return Vengeance();

            if (Skills.DemonHunter.FanOfKnives.CanCast() && !Player.IsInTown && Player.CurrentHealthPct < 0.3)
                return FanOfKnives();

            if (Skills.DemonHunter.Companion.CanCast() && !Player.IsInTown && Player.CurrentHealthPct < 0.3)
            {
                Core.Logger.Log(LogCategory.Routine, $"施放宠物");
                return Companion();
            }

            return null;
        }


        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetDefensivePower() => null;

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            //Core.Logger.Log(LogCategory.Routine, $"在 =====移动中-----  ");
            //TrinityActor tempTarget = TargetUtil.GetClosestUnit(50f);
            if ((!Player.IsInTown && CanAcquireFreeVaultBuff) || (!Player.IsInTown && Skills.DemonHunter.Impale.TimeSinceUse > 1950.0))
            {
                Core.Logger.Log(LogCategory.Routine, $"-----施放保持翻滚buff的飞刀 cast to keep the roll of the knife");
                return new TrinityPower(Skills.DemonHunter.Impale, 5f);
            }

            if (!Player.IsInTown && CanVaultTo(destination) && destination.Distance(Player.Position) > 25f)
            {
                Core.Logger.Log(LogCategory.Routine, $"在移动中接近怪物 In the move close to the monster");
                return Vault(destination);
            }

            if (IsStuck || IsBlocked)//TargetUtil.NumMobsInRange(15f)>6)
            {
                Core.Logger.Log(LogCategory.Routine, $"移动中卡住了，清怪 Move in the stuck, clear strange");
                var target = TargetUtil.GetClosestUnit(10f);
                if (target != null)
                    return Impale(target);

            }
            return Walk(destination, 5f);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;
        public override int KiteStutterDuration => 800;
        public override int KiteStutterDelay => 800;
        public override int KiteHealthPct => 35;
        public override float KiteDistance => 5f;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public DH_ShadowThreeKnifesSettings Settings { get; } = new DH_ShadowThreeKnifesSettings();

        public sealed class DH_ShadowThreeKnifesSettings : NotifyBase, IDynamicSetting
        {
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


