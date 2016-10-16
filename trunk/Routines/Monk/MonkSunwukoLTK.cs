
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Monk
{
    public sealed class MonkSunwukoLTK : MonkBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Sunwuko Lashing Tail Kick";
        public string Description => "Following several set reworks and the addition of a handful synergistic items over the course of the Patch 2.4 lifetime, Lashing Tail Kick Lashing Tail Kick finally comes into form for Patch 2.4.2. This is a high speed, visceral melee spec.";
        public string Author => "xzjv";
        public string Version => "0.1";

        // Also try linked build with:
        // => Laws of Seph instead of cindercoat in cube.
        // => Blinding flash - faith in the light instead of epiphany
        // => mantra of conviction instead of salvation.

        // Speed farming variation is with in-geom and vengeful wind in cube.      

        public string Url => "http://www.diablofans.com/builds/82587-sunwuko-ltk-solo-gr90-now-with-less-stack";
        
        // this was the original build variation, but its tough to sustain with no primary.
        // http://www.icy-veins.com/d3/monk-lashing-tail-kick-build-with-the-sunwuko-set-patch-2-4-2-season-7

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

            if ((ShouldRefreshBastiansGenerator || ShouldRefreshSpiritGuardsBuff) && TryPrimaryPower(out power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            if (IsNoPrimary)
            {
                // Stay away from units for belt to build stacks.
                if (Legendary.KyoshirosSoul.IsEquipped)
                {
                    var needStacks = Skills.Monk.SweepingWind.BuffStacks < 3 && Player.PrimaryResourcePct <= 0.75f;
                    var needResource = Player.PrimaryResource < PrimaryEnergyReserve;
                    if ((needStacks || needResource) && AllUnits.Any(u => u.Distance <= 12f))
                    {
                        Logger.Log(LogCategory.Routine, "Moving away to build stacks");
                        var pos = TargetUtil.GetLoiterPosition(CurrentTarget, 30f);

                        if (pos.Distance(Player.Position) < 10f)
                        {
                            Logger.Log(LogCategory.Routine, ".. Using Avoider Position");
                            Avoider.TryGetSafeSpot(out pos, 15f, 100f, Player.Position);
                        }

                        return Walk(pos);
                    }
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

            return Walk(TargetUtil.GetLoiterPosition(CurrentTarget, AttackRange + 10f));
        }

        public List<SNOAnim> LashingTailkickAnimations = new List<SNOAnim>
        {
            SNOAnim.Monk_Male_HTH_LashingTail,
            SNOAnim.Monk_Female_HTH_LashingTail
        };

        public bool IsBeltBuildingStacks = !AllUnits.Any(u => u.Position.Distance(Player.Position) < 12f) && Skills.Monk.SweepingWind.BuffStacks > 1;

        public float AttackRange 
            => (CurrentTarget.IsBoss || CurrentTarget.IsElite) && IsBeltBuildingStacks ? 50f
            : (Runes.Monk.SweepingArmada.IsActive ? 14f : 10f);

        protected override TrinityPower LashingTailKick(TrinityActor target)
        {
            // Teleport with Epiphany
            if (Skills.Monk.Epiphany.IsBuffActive && (IsStuck || IsBlocked || target.Distance > 30f))
                return new TrinityPower(SNOPower.Monk_LashingTailKick, AttackRange, target.AcdId, 0, 0);

            // Attack at position (Shift+Click) to send fireball from range.
            return new TrinityPower(SNOPower.Monk_LashingTailKick, AttackRange, target.Position, 0, 0);            
        }
            
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

            if (!TargetUtil.AnyMobsInRange(20f))
                return false;

            if (Player.CurrentHealthPct < 0.5f)
                return true;
                    
            if (Legendary.TheLawsOfSeph.IsEquipped)
                return Player.PrimaryResource < Player.PrimaryResourceMax - 165;

            return true;
        }

        protected override bool ShouldMantraOfConviction()
        {
            // Only use to dump resource.
            if (Player.PrimaryResourcePct < 0.65f)
                return false;

            return base.ShouldMantraOfConviction();
        }

        protected override bool ShouldMantraOfSalvation()
        {
            if (Player.PrimaryResourcePct < 0.3f && Player.CurrentHealthPct > 0.3f)
                return false;

            return base.ShouldMantraOfSalvation();
        }

        protected override bool ShouldMantraOfHealing()
        {
            if (Player.PrimaryResourcePct < 0.3f && Player.CurrentHealthPct > 0.3f)
                return false;

            return base.ShouldMantraOfHealing();
        }

        protected override bool ShouldMantraOfRetribution()
        {
            if (Player.PrimaryResourcePct < 0.3f && Player.CurrentHealthPct > 0.3f)
                return false;

            return base.ShouldMantraOfRetribution();
        }

        private int MinSweepingWindStacks => Legendary.VengefulWind.IsEquipped ? 3 : 1;

        private bool IsLTKAnimating => LashingTailkickAnimations.Contains(ZetaDia.Me.CommonData.CurrentAnimation);

        protected override bool ShouldLashingTailKick(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.LashingTailKick.CanCast())
            {
                Logger.Log(LogCategory.Routine, "Skipping LTK - Cant Cast.");
                return false;
            }

            if (Skills.Monk.SweepingWind.BuffStacks < 1)
                return false;

            if (!TargetUtil.AnyMobsInRange(50f))
            {
                Logger.Log(LogCategory.Routine, "Skipping LTK - No Units in 50yd Range.");
                return false;
            }

            // Sometimes ZetaDia UsePower says it was cast but it wasn't
            var timeSinceUse = Skills.Monk.LashingTailKick.TimeSinceUse;
            if (timeSinceUse < 25 || IsLTKAnimating && timeSinceUse < 250)
            {
                Logger.Log(LogCategory.Routine, $"Skipping LTK - Time Since Use ({timeSinceUse})");
                return false;
            }

            //// disabled - reports are that its not efficient to let it drop and recast.
            //var stacks = Skills.Monk.SweepingWind.BuffStacks;
            //if (stacks <= MinSweepingWindStacks)
            //{
            //    Logger.Log(LogCategory.Routine, $"Skipping LTK - Not Enough SW Stacks ({stacks})");
            //    return false;
            //}

            if (IsBlocked || IsStuck)
            {
                target = TargetUtil.GetClosestUnit(50f) ?? CurrentTarget;
            }
            else
            {
                target = TargetUtil.GetBestClusterUnit(50f) ?? CurrentTarget;
            }
            return true;
        }

        protected override bool ShouldSweepingWind()
        {
            if (!Skills.Monk.SweepingWind.CanCast())
                return false;

            if (Skills.Monk.SweepingWind.BuffStacks < 1)
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
            public void ApplyCode(string code) => JsonSerializer.Deserialize(code, this);
            public void Reset() => LoadDefaults();
            public void Save() { }

            #endregion
        }

        #endregion
    }
}

