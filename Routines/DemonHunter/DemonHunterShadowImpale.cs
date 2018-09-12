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
using Trinity.UI;
using Trinity.Settings;
using Zeta.Common;
using Zeta.Game;
using Trinity.Routines.DemonHunter;
using Trinity.Routines;
using Zeta.Game.Internals.Actors;

namespace Trinity750.Routines.DemonHunter
{
    public sealed class DemonHunterShadowImpale : DemonHunterBase, IRoutine
    {
        #region Definition

        public string DisplayName => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "暗影三刀" : "DemonHunterShadowImpale";
        public string Description => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "电或者冰都行，技能可以改符文，装备也可以动。 配装看下边链接" : "you can change impale rune and gears or cube, but dont change the skills.you can find the build with this url";
        public string Author => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "晚风清徐" : "Night Breeze";
        public string Version => "1.0.1";
        public string Url => System.Globalization.CultureInfo.InstalledUICulture.Name.ToLower().StartsWith("zh") ? "http://db.178.com/d3/s/974240690" : "http://www.d3planner.com/356198488";



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

        public override Func<bool> ShouldIgnoreAvoidance => () => true;

        public TrinityPower GetOffensivePower()
        {

            TrinityActor target = CurrentTarget;
            Vector3 position = Vector3.Zero;

            if (CurrentTarget != null && (!CurrentTarget.IsBoss || (CurrentTarget.IsElite && CurrentTarget.EliteType == EliteTypes.Minion)))
                target = TargetUtil.BestRangedAoeUnit(50) ?? CurrentTarget;

            if (!Core.Buffs.HasBuff(445266) && Player.PrimaryResource > 36)
                return new TrinityPower(Skills.DemonHunter.Impale);

            position = MathEx.CalculatePointFrom(target.Position, ZetaDia.Me.Position, 15);
            if ((target.Distance > 15 || ((Legendary.ElusiveRing.IsEquipped || Legendary.ElusiveRing.IsEquippedInCube) && Skills.DemonHunter.Vault.TimeSinceUse > 6000)) && Core.Grids.CanRayWalk(Player.Position, position))
                return Vault(position);


            if (Skills.DemonHunter.Impale.CanCast())
                return new TrinityPower(Skills.DemonHunter.Impale, Player.CurrentHealthPct < 0.45 ? 30f : 15f, target.AcdId);

            return null;
        }

        public TrinityPower GetBuffPower()
        {
            if (Player.IsInTown)
                return null;

            if (Skills.DemonHunter.ShadowPower.CanCast() && !Player.HasBuff(SNOPower.DemonHunter_ShadowPower))
                return ShadowPower();

            if (Skills.DemonHunter.Vengeance.CanCast())
                return Vengeance();

            if (Skills.DemonHunter.FanOfKnives.CanCast())
                return FanOfKnives();

            if (Skills.DemonHunter.Companion.CanCast() && TargetUtil.NumMobsInRange(15) > 5)
                return Companion();

            return null;
        }


        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetDefensivePower() => null;

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (!Player.IsInTown)
            {
                if ((IsBlocked || IsStuck) && (Core.Buffs.HasBuff(445266) || Player.PrimaryResource > 36))
                    return Vault(MathEx.CalculatePointFrom(destination, ZetaDia.Me.Position, 25));

                if (!Core.Buffs.HasBuff(445266) && Player.PrimaryResource > 36)
                    return new TrinityPower(Skills.DemonHunter.Impale);

                if ((Core.Buffs.HasBuff(445266) || Player.PrimaryResource > 36) && CanVaultTo(destination))
                    return Vault(destination);
            }

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public DemonHunterShadowImpaleSettings Settings { get; } = new DemonHunterShadowImpaleSettings();

        public sealed class DemonHunterShadowImpaleSettings : NotifyBase, IDynamicSetting
        {
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(8)]
            public int ClusterSize
            {
                get => _clusterSize;
                set => SetField(ref _clusterSize, value);
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get => _emergencyHealthPct;
                set => SetField(ref _emergencyHealthPct, value);
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
