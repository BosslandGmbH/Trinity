using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Trinity.Cache;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Abilities;
using Trinity.Config;
using Trinity.Config.Combat;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Modules;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Attributes;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Reference;
using Trinity.Settings;
using Trinity.Technicals;
using Trinity.UI;
using Trinity.UIComponents;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Routines.DemonHunter
{
    public sealed class DemonHunterShadowHybrid : DemonHunterBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Shadow-Marauder Hybrid Fan Routine.";
        public string Description => "A group build that focuses on maximising damage with each Fan of Knives.";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/demon-hunter-shadows-mantle-marauder-hybrid-build-with-fan-of-knives-patch-2-4-2-season-7";

        public override Func<bool> ShouldIgnoreAvoidance => ShouldFan;
        public override Func<bool> ShouldIgnoreKiting => ShouldFan;
        public override Func<bool> ShouldIgnoreNonUnits => ShouldFan;

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.TheShadowsMantle, SetBonus.Second },
                { Sets.EmbodimentOfTheMarauder, SetBonus.First }
            },
            Items = new List<Item>
            {
                Legendary.LordGreenstonesFan,
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.DemonHunter.FanOfKnives, null },
            },
        };

        #endregion

        public bool ShouldFan() => IsFanBuffReady && (IsFanTrashCluster || IsFanEliteCluster || IsBossFight) && IsLightningElement;
        public bool IsBossFight => TargetUtil.AnyBossesInRange(60f);
        public bool IsWalkBuffReady => EndlessWalkOffensiveStacks >= 80;
        public bool IsFanBuffReady => LordGreenStoneDamageStacks == 30;
        public bool IsFanTrashCluster => TargetUtil.ClusterExists(20f, 90f, 15, false) && TargetUtil.ElitesInRange(80f) == 0;
        public bool IsFanEliteCluster => TargetUtil.ElitesInRange(20f) == TargetUtil.ElitesInRange(80f);
        public bool IsLightningElement => !ShouldWaitForConventionofElements(Skills.DemonHunter.FanOfKnives, Element.Lightning);
        public double VengeanceCooldownSeconds => 90*(1 - Core.Player.CooldownReductionPct)*(Legendary.Dawn.IsEquipped ? 1 : 0.35);
        public bool IsReallyBlocked => (PlayerMover.IsBlocked && Core.BlockedCheck.BlockedTime.TotalMilliseconds > 1000) || Core.StuckHandler.IsStuck;

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;

            Logger.Log(LogCategory.Routine, $"FanBuff={IsFanBuffReady} TrashCluster={IsFanTrashCluster} EliteCluster={IsFanEliteCluster} Element={IsLightningElement} Crit%={Player.CriticalChancePct}");

            if (Player.CurrentHealthPct < 0.2 && Skills.DemonHunter.FanOfKnives.CanCast())
            {
                Logger.Log(LogCategory.Routine, "About to die...");
                return FanOfKnives();
            }

            if (ShouldFan())
            {
                Logger.Log(LogCategory.Routine, "Its time for the pain");

                // +15% dmg
                var isWolfBuffReady = !Skills.DemonHunter.Companion.IsActive || Skills.DemonHunter.Companion.IsBuffActive;
                if (!isWolfBuffReady && Skills.DemonHunter.Companion.CanCast())
                {
                    return Companion();
                }

                // +40% dmg
                var isVengeanceReady = !Skills.DemonHunter.Vengeance.IsActive || Skills.DemonHunter.Vengeance.IsBuffActive;
                if (!isVengeanceReady && Skills.DemonHunter.Vengeance.CanCast())
                {
                    return Vengeance();
                }

                //if (!Player.IsMaxCriticalChance)
                //{
                //    return Walk(TargetUtil.BestAoeUnit(100f, true));
                //}

                var clusterUnits = TargetUtil.GetBestClusterUnits().Where(u => u.IsElite || u.NearbyUnitsWithinDistance() > ClusterSize).Select(u => u.Position);
                var fanPosition = TargetUtil.GetCentroid(clusterUnits);

                // If we cant get there, dont miss the opportunity.
                if (IsReallyBlocked && TimeFromElementStart(Element.Lightning) > 3000)
                {
                    Logger.Log(LogCategory.Routine, "Deloying Blocked Fan");
                    return FanOfKnives();
                }

                return FanOfKnives(fanPosition);
            }

            if (!IsReallyBlocked)
            {
                var pullingUnit = AllUnits
                    .Where(u => Math.Abs(u.HitPointsPct - 1) < float.Epsilon && (u.IsElite || u.NearbyUnitsWithinDistance(10f) > 3))
                    .OrderByDescending(u => u.Distance).FirstOrDefault();

                if (pullingUnit != null)
                {
                    Logger.Log(LogCategory.Routine, "Pulling units");
                    return Walk(pullingUnit);
                }
            }
          
            if (IsBossFight && LordGreenStoneDamageStacks < 15)
            {
                Logger.Log(LogCategory.Routine, "Attacking while waiting for fan");
                if (TryPrimaryPower(out power))
                    return power;
            }

            if (LordGreenStoneDamageStacks < 28 || !IsLightningElement)
            {
                Logger.Log(LogCategory.Routine, "Hanging out near fan position");
                return Walk(TargetUtil.GetLoiterPosition(TargetUtil.BestAoeUnit(100f, true), 30f));
            }

            Logger.Log(LogCategory.Routine, "Moving to fan position");
            return Walk(TargetUtil.BestAoeUnit(100f, true));
        }        

        public TrinityPower GetBuffPower()
        {
            if (ShouldShadowPower())
                return ShadowPower();

            if (ShouldSmokeScreen())
                return SmokeScreen();

            if (Skills.DemonHunter.Vengeance.CanCast())
            {
                // Only cast vegeance if it will be ready again by the time we need to cast fan of knives.
                if (VengeanceCooldownSeconds < 30 - LordGreenStoneDamageStacks)
                    return Vengeance();

                // Or if health is critical
                if (Player.CurrentHealthPct < EmergencyHealthPct)
                    return Vengeance();
            }

            return null;
        }

        protected override bool ShouldSmokeScreen()
        {
            return Skills.DemonHunter.SmokeScreen.CanCast() && Player.CurrentHealthPct < 0.95f;
        }

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetDefensivePower() => null;

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (CanVaultTo(destination))
                return Vault(destination);

            return Walk(destination);
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;
        public override int KiteHealthPct => 35;
        public override float KiteDistance => 5f;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public DemonHunterMarauderSettings Settings { get; } = new DemonHunterMarauderSettings();

        public sealed class DemonHunterMarauderSettings : NotifyBase, IDynamicSetting
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
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}


