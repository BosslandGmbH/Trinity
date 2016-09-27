
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Abilities;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.Reference;
using Trinity.Technicals;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Routines.Monk
{
    public sealed class MonkSunwukoLTK : MonkBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Sunwuko Lashing Tail Kick";
        public string Description => "Following several set reworks and the addition of a handful synergistic items over the course of the Patch 2.4 lifetime, Lashing Tail Kick Lashing Tail Kick finally comes into form for Patch 2.4.2. This is a high speed, visceral melee spec.";
        public string Author => "xzjv";
        public string Version => "0.1";
        public string Url => "http://www.icy-veins.com/d3/monk-lashing-tail-kick-build-with-the-sunwuko-set-patch-2-4-2-season-7";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.MonkeyKingsGarb, SetBonus.Third },
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Monk.LashingTailKick, null },
            },
        };

        #endregion

        //public static class FireballTracker
        //{
        //    public static Dictionary<int,TrinityActor> Fireballs = new Dictionary<int, TrinityActor>();
        //    private static DateTime _lastUseTime;

        //    public static bool IsFireballPending { get; private set; }

        //    public const int FireballSnoId = (int) SNOActor.zoltunKulle_fieryBoulder_model;
        //    public const int PendingTimeoutMs = 1000;

        //    public static void Update()
        //    {
        //        var lastUseTime = SpellHistory.PowerLastUsedTime(SNOPower.Monk_LashingTailKick);
        //        var msSinceUse = DateTime.UtcNow.Subtract(lastUseTime).TotalMilliseconds;
        //        if (lastUseTime != _lastUseTime)
        //        {
        //            Logger.Log($"New LTK cast, waiting for fireball");
        //            IsFireballPending = msSinceUse < PendingTimeoutMs;
        //            _lastUseTime = lastUseTime;
        //        }

        //        if (msSinceUse > PendingTimeoutMs && IsFireballPending)
        //        {
        //            Logger.Log($"Fireball wait timed out");
        //            IsFireballPending = false;
        //        }

        //        foreach (var actor in Core.Actors.AllRActors.Where(a => a.Type == TrinityObjectType.ClientEffect))
        //        {
        //            if (actor.ActorSnoId == FireballSnoId && !Fireballs.ContainsKey(actor.AnnId))
        //            {
        //                Logger.Log($"New Fireball Found Delay:{msSinceUse}ms");
        //                IsFireballPending = false;
        //                Fireballs.Add(actor.AnnId, actor);                  
        //            }                    
        //        }

        //        foreach (var fireball in Fireballs.ToList().Where(fireball => !fireball.Value.IsValid))
        //        {
        //            Fireballs.Remove(fireball.Key);
        //        }
        //    }
        //}

        public TrinityPower GetOffensivePower()
        {
            // It's worth trying other variations of this build that produce more spirit. 
            // Speed farming variation is with in-geom and vengeful wind in cube.       
            // Sweeping armada for LTK == more monsters hit == more stacks.

            // 853: PowerBuff0VisualEffectNone (-3243) [ PowerSnoId: ItemPassive_Unique_Ring_903_x1: 402411 ] i:1 f:0 Value=1 
            // 865: PowerBuff2VisualEffectNone (-3231) [ PowerSnoId: ItemPassive_Unique_Ring_922_x1: 402461 ] i:0 f:0 Value=0 
            // 588: BuffIconStartTick2(-3508)[PowerSnoId: ItemPassive_Unique_Gem_018_x1: 428348] i: 79817 f: 0 Value = 79817
            // 863: PowerBuff1VisualEffectD (-3233) [ PowerSnoId: Monk_LashingTailKick: 111676 ] i:1 f:0 Value=1 
            
            TrinityPower power;

            //FireballTracker.Update();
            //if (FireballTracker.IsFireballPending)
            //{
            //    Logger.Log("Waiting for fireball");
            //    return new TrinityPower(SNOPower.None);
            //}

            if (TrySpecialPower(out power))
                return power;            

            if (Core.Buffs.HasCastingShrine)
            {
                if (Skills.Monk.DashingStrike.CanCast() && !Skills.Monk.DashingStrike.IsLastUsed)
                    return DashingStrike(CurrentTarget.Position);

                if (Skills.Monk.LashingTailKick.CanCast())
                    return LashingTailKick(CurrentTarget);
            }

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            if (IsNoPrimary)
            {
                // Stay away from units for belt to build stacks.
                if (Legendary.KyoshirosSoul.IsEquipped && Skills.Monk.SweepingWind.BuffStacks < 3 && Player.PrimaryResourcePct <= 0.75f)
                {
                    Logger.Log(LogCategory.Routine, "Moving away to build stacks");
                    return Walk(TargetUtil.GetLoiterPosition(CurrentTarget, 40f));
                }

                var clusterUnit = TargetUtil.GetBestClusterUnit(50f) ?? CurrentTarget;
                if (clusterUnit != null)
                {
                    // Allow stacks to drop if resource is high enough.
                    if (Skills.Monk.LashingTailKick.CanCast() && Player.PrimaryResourcePct > 0.75f)
                    {
                        return LashingTailKick(clusterUnit);
                    }
                }
            }

            return null;
        }

        public List<SNOAnim> LashingTailkickAnimations = new List<SNOAnim>
        {
            SNOAnim.Monk_Male_HTH_LashingTail,
            SNOAnim.Monk_Female_HTH_LashingTail
        };

        public bool IsBuildingStacks = !TargetUtil.AnyMobsInRange(20f) && Skills.Monk.SweepingWind.BuffStacks > 1;

        public float AttackRange 
            => (CurrentTarget.IsBoss || CurrentTarget.IsElite) && IsBuildingStacks ? 50f
            : (Runes.Monk.SweepingArmada.IsActive ? 15f : 10f);

        protected override TrinityPower LashingTailKick(TrinityActor target)
            => new TrinityPower(SNOPower.Monk_LashingTailKick, AttackRange, target.AcdId, 0, 0);

        public TrinityPower GetDefensivePower() => GetBuffPower();
        public TrinityPower GetBuffPower() => DefaultBuffPower();
        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (HasInstantCooldowns && Skills.Monk.DashingStrike.CanCast() && Skills.Monk.DashingStrike.TimeSinceUse > 200 && destination.Distance(Player.Position) > 18f)
                return DashingStrike(destination);

            if (AllowedToUse(Settings.DashingStrike, Skills.Monk.DashingStrike) && CanDashTo(destination) && (destination.Distance(Player.Position) > 35f || IsBlocked))
                return DashingStrike(destination);

            return Walk(destination);
        }

        protected override bool ShouldBlindingFlash()
        {
            if (!Skills.Monk.BlindingFlash.CanCast())
                return false;

            if (Player.CurrentHealthPct < 0.3f && TargetUtil.AnyMobsInRange(20f))
                return true;

            return Player.PrimaryResourcePct < 0.2f;
        }

        protected override bool ShouldLashingTailKick(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.LashingTailKick.CanCast())
                return false;

            if (!TargetUtil.AnyMobsInRange(50f))
                return false;

            if (Skills.Monk.LashingTailKick.TimeSinceUse < 500)
                return false;

            if (Skills.Monk.SweepingWind.BuffStacks <= 3)
                return false;

            target = TargetUtil.GetBestClusterUnit(50f);
            return target != null;
        }

        protected override bool ShouldSweepingWind()
        {
            if (!Skills.Monk.SweepingWind.CanCast())
                return false;

            if (!Skills.Monk.SweepingWind.IsBuffActive && AllUnitsInSight.Any(u => u.Distance < 100f))
                return true;

            var buffCooldownRemanining = Core.Cooldowns.GetBuffCooldownRemaining(SNOPower.Monk_SweepingWind);
            if (buffCooldownRemanining.TotalMilliseconds > 750)
                return false;

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
            if (unit == null)
                return false;

            return unit.Position != Vector3.Zero;
        }

        protected override bool ShouldEpiphany()
        {
            if (!AllowedToUse(Settings.Epiphany, Skills.Monk.Epiphany))
                return false;

            return base.ShouldEpiphany();
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;


        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public MonkSunwukoLTKSettings Settings { get; } = new MonkSunwukoLTKSettings();

        public sealed class MonkSunwukoLTKSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _epiphany;
            private SkillSettings _dashingStrike;
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
                UseTime = UseTime.Selective,
                Reasons = UseReasons.Elites | UseReasons.HealthEmergency
            };

            private static readonly SkillSettings DashingStrikeDefaults = new SkillSettings
            {
                UseTime = UseTime.AnyTime,
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
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}

