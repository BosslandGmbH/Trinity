
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
using Trinity.Framework.Modules;
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

        /* Speed Farm Solo Variant tested @ GR75
        [Trinity 2.55.668] ------ Equipped Non-Set Legendaries: Items=8, Sets=1 ------
        [Trinity 2.55.668] Item: SpiritStone: Gyana Na Kashu (222169) is Equipped
        [Trinity 2.55.668] Item: Belt: Kyoshiro's Soul (298136) is Equipped
        [Trinity 2.55.668] Item: Boots: Rivera Dancers (197224) is Equipped
        [Trinity 2.55.668] Item: Ring: Obsidian Ring of the Zodiac (212588) is Equipped
        [Trinity 2.55.668] Item: Ring: Unity (212581) is Equipped
        [Trinity 2.55.668] Item: Bracer: Nemesis Bracers (298121) is Equipped
        [Trinity 2.55.668] Item: FistWeapon: Scarbringer (130557) is Equipped
        [Trinity 2.55.668] Item: FistWeapon: Vengeful Wind (403775) is Equipped
        [Trinity 2.55.668] ------ Equipped in Kanai's Cube: Items=3 ------
        [Trinity 2.55.668] Item: SpiritStone: The Laws of Seph (299454) is Equipped
        [Trinity 2.55.668] Item: Ring: Ring of Royal Grandeur (298094) is Equipped
        [Trinity 2.55.668] Item: Sword: In-geom (410946) is Equipped
        [Trinity 2.55.668] ------ Set: Monkey King's Garb : 5/6 Equipped. ActiveBonuses=3/3 ------
        [Trinity 2.55.668] Item: Shoulder: Sunwuko's Balance (336175) is Equipped
        [Trinity 2.55.668] Item: Gloves: Sunwuko's Paws (336172) is Equipped
        [Trinity 2.55.668] Item: Amulet: Sunwuko's Shines (336174) is Equipped
        [Trinity 2.55.668] Item: Legs: Sunwuko's Leggings (429075) is Equipped
        [Trinity 2.55.668] Item: Chest: Sunwuko's Soul (429167) is Equipped
        [Trinity 2.55.668] ------ Active Skills / Runes ------
        [Trinity 2.55.668] Skill: Lashing Tail Kick Rune=Sweeping Armada  Type=Spender
        [Trinity 2.55.668] Skill: Blinding Flash Rune=Faith in the Light Type=Other
        [Trinity 2.55.668] Skill: Dashing Strike Rune=Way of the Falling Star Type=Other
        [Trinity 2.55.668] Skill: Mantra of Salvation Rune=Agility Type=Other
        [Trinity 2.55.668] Skill: Sweeping Wind Rune=Inner Storm Type=Spender
        [Trinity 2.55.668] Skill: Epiphany Rune=Desert Shroud Type=Other
        [Trinity 2.55.668] ------ Passives ------
        [Trinity 2.55.668] Passive: Exalted Soul
        [Trinity 2.55.668] Passive: Beacon of Ytar
        [Trinity 2.55.668] Passive: Harmony
        [Trinity 2.55.668] Passive: Momentum
        
        [Trinity 2.55.668] ------ Equipped Non-Set Legendaries: Items=6, Sets=2 ------
        [Trinity 2.55.668] Item: SpiritStone: Gyana Na Kashu (222169) is Equipped
        [Trinity 2.55.668] Item: Belt: The Witching Hour (193670) is Equipped
        [Trinity 2.55.668] Item: Boots: Rivera Dancers (197224) is Equipped
        [Trinity 2.55.668] Item: Bracer: Spirit Guards (430290) is Equipped
        [Trinity 2.55.668] Item: FistWeapon: Crystal Fist (175939) is Equipped
        [Trinity 2.55.668] Item: FistWeapon: Scarbringer (130557) is Equipped
        [Trinity 2.55.668] ------ Equipped in Kanai's Cube: Items=3 ------
        [Trinity 2.55.668] Item: SpiritStone: The Laws of Seph (299454) is Equipped
        [Trinity 2.55.668] Item: Ring: Ring of Royal Grandeur (298094) is Equipped
        [Trinity 2.55.668] Item: Daibo: Flying Dragon (197065) is Equipped
        [Trinity 2.55.668] ------ Set: Bastions of Will : 2/2 Equipped. ActiveBonuses=1/1 ------
        [Trinity 2.55.668] Item: Ring: Focus (332209) is Equipped
        [Trinity 2.55.668] Item: Ring: Restraint (332210) is Equipped
        [Trinity 2.55.668] ------ Set: Monkey King's Garb : 5/6 Equipped. ActiveBonuses=3/3 ------
        [Trinity 2.55.668] Item: Shoulder: Sunwuko's Balance (336175) is Equipped
        [Trinity 2.55.668] Item: Gloves: Sunwuko's Paws (336172) is Equipped
        [Trinity 2.55.668] Item: Amulet: Sunwuko's Shines (336174) is Equipped
        [Trinity 2.55.668] Item: Legs: Sunwuko's Leggings (429075) is Equipped
        [Trinity 2.55.668] Item: Chest: Sunwuko's Soul (429167) is Equipped
        [Trinity 2.55.668] ------ Active Skills / Runes ------
        [Trinity 2.55.668] Skill: Fists of Thunder Rune=Quickening Type=Generator
        [Trinity 2.55.668] Skill: Lashing Tail Kick Rune=Sweeping Armada  Type=Spender
        [Trinity 2.55.668] Skill: Blinding Flash Rune=Faith in the Light Type=Other
        [Trinity 2.55.668] Skill: Dashing Strike Rune=Blinding Speed Type=Other
        [Trinity 2.55.668] Skill: Mantra of Salvation Rune=Agility Type=Other
        [Trinity 2.55.668] Skill: Sweeping Wind Rune=Inner Storm Type=Spender
        [Trinity 2.55.668] ------ Passives ------
        [Trinity 2.55.668] Passive: Exalted Soul
        [Trinity 2.55.668] Passive: Seize the Initiative
        [Trinity 2.55.668] Passive: The Guardian's Path
        [Trinity 2.55.668] Passive: Beacon of Ytar
        [Trinity 2.55.668][Routine] Need Spirit Gaurds Buff            
        
        */

        public TrinityPower GetOffensivePower()
        {
            // 853: PowerBuff0VisualEffectNone (-3243) [ PowerSnoId: ItemPassive_Unique_Ring_903_x1: 402411 ] i:1 f:0 Value=1 
            // 865: PowerBuff2VisualEffectNone (-3231) [ PowerSnoId: ItemPassive_Unique_Ring_922_x1: 402461 ] i:0 f:0 Value=0 
            // 588: BuffIconStartTick2(-3508)[PowerSnoId: ItemPassive_Unique_Gem_018_x1: 428348] i: 79817 f: 0 Value = 79817
            // 863: PowerBuff1VisualEffectD (-3233) [ PowerSnoId: Monk_LashingTailKick: 111676 ] i:1 f:0 Value=1 

            TrinityPower power;
            Vector3 position;

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

            if (ShouldRefreshSpiritGuardsBuff)
            {
                Logger.Log(LogCategory.Routine, "Need Spirit Gaurds Buff");
                if (TryPrimaryClosestTarget(out power))
                    return power;
            }

            if (ShouldRefreshBastiansGenerator)
            {
                Logger.Log(LogCategory.Routine, "Need Bastians Buff");
                if (TryPrimaryClosestTarget(out power))
                    return power;
            }

            // With sweeping armada try to keep distance in the sweet spot between 10-15yd
            if (Runes.Monk.SweepingArmada.IsActive)
            {
                var enoughTimePassed = SpellHistory.TimeSinceUse(SNOPower.Walk).TotalMilliseconds > 500;
                var isSoloElite = TargetUtil.ElitesInRange(25f) == 1 && !AnyUnitsInRange(25f);
                if (enoughTimePassed && isSoloElite && CurrentTarget.RadiusDistance <= 10f && !IsStuck)
                {
                    if (Avoider.TryGetSafeSpot(out position, 12f + CurrentTarget.CollisionRadius, 30f, CurrentTarget.Position))
                    {
                        Logger.Log(LogCategory.Routine, $"Adjusting Distance for Sweeping Armarda RDist={CurrentTarget.RadiusDistance} Dist={ZetaDia.Me.Position.Distance(CurrentTarget.Position)}");
                        return Walk(position,2f);
                    }
                }
            }

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
                        return Walk(TargetUtil.GetLoiterPosition(CurrentTarget, 30f));
                    }
                }
            }

            // SW Stacks but no mana, or skills cooldown maybe, hang out just outside range of target.
            Logger.Log(LogCategory.Routine, "Can't cast anything.");
            return Walk(TargetUtil.GetLoiterPosition(CurrentTarget, 25f));
        }

        private bool TryPrimaryClosestTarget(out TrinityPower power)
        {
            TrinityActor closestTarget = TargetUtil.ClosestUnit(30f);
            TrinityActor bestTarget;
            power = null;

            if (ShouldFistsOfThunder(out bestTarget))
                power = FistsOfThunder(closestTarget ?? bestTarget);

            else if (ShouldDeadlyReach(out bestTarget))
                power = DeadlyReach(closestTarget ?? bestTarget);

            else if (ShouldCripplingWave(out bestTarget))
                power = CripplingWave(closestTarget ?? bestTarget);

            else if (ShouldWayOfTheHundredFists(out bestTarget))
                power = WayOfTheHundredFists(closestTarget ?? bestTarget);

            return power != null;
        }

        public List<SNOAnim> LashingTailkickAnimations = new List<SNOAnim>
        {
            SNOAnim.Monk_Male_HTH_LashingTail,
            SNOAnim.Monk_Female_HTH_LashingTail
        };

        public bool AnyUnitsInRange(float range)
            => AllUnits.Any(u => u.Position.Distance(Player.Position) < range);

        public bool IsBeltBuildingStacks 
            => !AnyUnitsInRange(12f) && Skills.Monk.SweepingWind.UncachedBuffStacks >= 1;

        public bool ShootFromDistance
            => (CurrentTarget.IsBoss || CurrentTarget.IsElite) && IsBeltBuildingStacks && Settings.LTKFromRange;

        public float AttackRange 
            => ShootFromDistance ? 50f
            : (Runes.Monk.SweepingArmada.IsActive ? 18f : 12f);

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
            {
                return DashingStrike(CalculateAttackPosition(destination));
            }

            if (AllowedToUse(Settings.DashingStrike, Skills.Monk.DashingStrike) && CanDashTo(destination) && (destination.Distance(Player.Position) > 35f || IsBlocked))
            {
                return DashingStrike(CalculateAttackPosition(destination));
            }

            return Walk(destination);
        }

        private Vector3 CalculateAttackPosition(Vector3 destination)
        {
            if (Runes.Monk.SweepingArmada.IsActive && CurrentTarget?.Position.Distance(destination) <= 4f && CurrentTarget.IsUnit)
                return MathEx.CalculatePointFrom(CurrentTarget.Position, destination, AttackRange);

            return destination;
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
            var swStacks = Skills.Monk.SweepingWind.UncachedBuffStacks;

            if (!Skills.Monk.LashingTailKick.CanCast())
            {
                Logger.Log(LogCategory.Routine, $"Skipping LTK - Cant Cast. Charges={swStacks}");
                return false;
            }

            if (swStacks <= 1)
                return false;     

            //if (!TargetUtil.AnyMobsInRange(50f))
            //{
            //    Logger.Log(LogCategory.Routine, $"Skipping LTK - No Units in 50yd Range. Charges={swStacks}");
            //    return false;
            //}

            // Sometimes ZetaDia UsePower says it was cast but it wasn't
            //var timeSinceUse = Skills.Monk.LashingTailKick.TimeSinceUse;
            if (IsLTKAnimating || Player.IsCastingOrLoading)
            {
                //Logger.Log(LogCategory.Routine, $"Skipping LTK - Time Since Use ({timeSinceUse}) Charges={swStacks}");
                Logger.Log(LogCategory.Routine, $"Skipping LTK - casting/animating");//l;Time Since Use ({timeSinceUse}) Charges={swStacks}");
                return false;
            }

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

            if (Skills.Monk.SweepingWind.UncachedBuffStacks < 1)
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
            private bool _LTKFromRange;

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
 
            public bool LTKFromRange
            {
                get { return _LTKFromRange; }
                set { SetField(ref _LTKFromRange, value); }
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

