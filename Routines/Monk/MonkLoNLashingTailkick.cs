using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


namespace Trinity.Routines.Monk
{
    public sealed class MonkLoNLashingTailkick : MonkBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Legacy of Nightmares Lashing Tail Kick";
        public string Description => "Lashing Tailkick build that uses Legacy of Nightmare rings and no generator.";
        public string Author => "TwoCigars";
        public string Version => "Beta 0.1";      

        public string Url => "http://www.d3planner.com/295554430";

        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.LegacyOfNightmares, SetBonus.First },
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Monk.LashingTailKick, null },
            },
        };

        #endregion

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            Vector3 position;

            if (TrySpecialPower(out power))
                return power;            

            if (Core.Buffs.HasCastingShrine)
            {
                if (Skills.Monk.BlindingFlash.CanCast() && Legendary.TheLawsOfSeph.IsEquipped && Player.PrimaryResource < Player.PrimaryResourceMax - 165)
                    return BlindingFlash();

                if (Skills.Monk.MysticAlly.CanCast() && Runes.Monk.AirAlly.IsActive && Player.PrimaryResource < Player.PrimaryResourceMax - 200)
                    return MysticAlly();

                if (Skills.Monk.DashingStrike.CanCast() && !Skills.Monk.DashingStrike.IsLastUsed)
                    return DashingStrike(CurrentTarget.Position);

                if (Skills.Monk.LashingTailKick.CanCast())
                    return LashingTailKick(CurrentTarget);
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
                        Core.Logger.Log(LogCategory.Routine, $"Adjusting Distance for Sweeping Armarda RDist={CurrentTarget.RadiusDistance} Dist={ZetaDia.Me.Position.Distance(CurrentTarget.Position)}");
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
                // Stay away from hostile units when Regen skills are on cooldown and under [50] Spirit
                if (Skills.Monk.BlindingFlash.IsActive && Legendary.TheLawsOfSeph.IsEquipped || Skills.Monk.MysticAlly.IsActive && Runes.Monk.AirAlly.IsActive)
                {
                    var regenOnCooldown = !Skills.Monk.BlindingFlash.CanCast() && !Skills.Monk.MysticAlly.CanCast();
                    var needResource = Player.PrimaryResource < PrimaryEnergyReserve;
                    if ((regenOnCooldown || needResource) && HostileMonsters.Any(u => u.Distance <= 12f))
                    {
                        Core.Logger.Log(LogCategory.Routine, "Moving away - Low Spirit - Regen on Cooldown");
                        return Walk(TargetUtil.GetLoiterPosition(CurrentTarget, 30f));
                    }
                }
            }

            return Walk(TargetUtil.GetLoiterPosition(CurrentTarget, 25f));
        }

        public List<SNOAnim> LashingTailkickAnimations = new List<SNOAnim>
        {
            SNOAnim.Monk_Male_HTH_LashingTail,
            SNOAnim.Monk_Female_HTH_LashingTail
        };

        public bool AnyUnitsInRange(float range)
            => HostileMonsters.Any(u => u.Position.Distance(Player.Position) < range);

        public bool ShootFromDistance
            => (CurrentTarget.IsBoss || CurrentTarget.IsElite) && Settings.LTKFromRange;

        public float AttackRange 
            => ShootFromDistance ? 50f
            : (Runes.Monk.SweepingArmada.IsActive ? 18f : 12f);

        protected override TrinityPower LashingTailKick(TrinityActor target)
        {
            // Teleport with Epiphany
            if (target.IsDestroyable || Skills.Monk.Epiphany.IsBuffActive && (IsStuck || IsBlocked || target.Distance > 30f))
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

            //Prioritize using Blinding Flash for Spirit Regeneration
            if (Legendary.TheLawsOfSeph.IsEquipped)
                return Player.PrimaryResource < Player.PrimaryResourceMax - 165;

            if (!TargetUtil.AnyMobsInRange(20f))
                return false;

            if (Player.CurrentHealthPct < 0.5f)
                return true;

            return true;
        }

        protected override bool ShouldMysticAlly()
        {
            if (!Skills.Monk.MysticAlly.CanCast())
                return false;
             
            //Prioritize using Mystic Ally for Spirit Regeneration when missing 200 Spirit
            if (Runes.Monk.AirAlly.IsActive)
                return Player.PrimaryResource < Player.PrimaryResourceMax - 200;

            if (!TargetUtil.AnyMobsInRange(50f))
                return false;

            //Still check for buff for when not using Air Ally Rune
            if (Core.Buffs.HasBuff(SNOPower.X1_Monk_MysticAlly_v2))
                return false;

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

        private bool IsLTKAnimating => LashingTailkickAnimations.Contains(ZetaDia.Me.CommonData.CurrentAnimation);

        protected override bool ShouldLashingTailKick(out TrinityActor target)
        {
            target = null;

            if (!Skills.Monk.LashingTailKick.CanCast())
            {
                return false;
            }
  
            if (IsLTKAnimating || Player.IsCastingOrLoading)
            {
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

        protected override bool ShouldCycloneStrike()
        {
            var skill = Skills.Monk.CycloneStrike;
            if (!skill.CanCast())
                return false;

            //Overriding the default Primary Energy Reserve [50] to 100. Designed to prevent Cyconle Strike 
            if (Player.PrimaryResource < 100)
                return false;

            if (skill.TimeSinceUse < (skill.DistanceFromLastUsePosition < 20f ? 4000 : 2000))
                return false;

            var targetIsCloseElite = CurrentTarget.IsElite && CurrentTarget.Distance < CycloneStrikeRange;
            var plentyOfTargetsToPull = TargetUtil.IsPercentUnitsWithinBand(15f, CycloneStrikeRange, 0.25);

            return targetIsCloseElite || plentyOfTargetsToPull;
        }

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;


        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public MonkLoNLashingTailkickSettings Settings { get; } = new MonkLoNLashingTailkickSettings();

        public sealed class MonkLoNLashingTailkickSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _epiphany;
            private SkillSettings _dashingStrike;
            private int _clusterSize;
            private float _emergencyHealthPct;
            private bool _LTKFromRange;

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

            public SkillSettings Epiphany
            {
                get => _epiphany;
                set => SetField(ref _epiphany, value);
            }

            public SkillSettings DashingStrike
            {
                get => _dashingStrike;
                set => SetField(ref _dashingStrike, value);
            }
 
            public bool LTKFromRange
            {
                get => _LTKFromRange;
                set => SetField(ref _LTKFromRange, value);
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

