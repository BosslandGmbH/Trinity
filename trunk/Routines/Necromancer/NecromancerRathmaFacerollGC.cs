using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Bot.Navigation;


namespace Trinity.Routines.Necromancer
{
    public sealed class NecromancerRathmaFacerollGC : NecroMancerBase, IRoutine
    {
        #region Definition

        public string DisplayName => "Necromancer Rathma Faceroll GC";
        public string Description => "Necromancer Rathma Faceroll Glass Cannon - Stay away I'm very fragile";
        public string Author => "Bantou";
        public string Version => "1.3.1";
        public string Url => "http://www.diablofans.com/builds/91398-rathma-faceroll-summoner-gr100-new-video-gr100";
        public Build BuildRequirements => new Build
        {
            // Set requirements
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.BonesOfRathma, SetBonus.Third },
            },
            // Skills Requirement
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Necromancer.BoneSpikes, null },
                { Skills.Necromancer.Simulacrum, Runes.Necromancer.Reservoir},
                { Skills.Necromancer.SkeletalMage, null },
                { Skills.Necromancer.LandOfTheDead, Runes.Necromancer.FrozenLands },
                { Skills.Necromancer.Devour, null },
                { Skills.Necromancer.CommandSkeletons, null },
            },
            // Items Requirements
            Items = new List<Item>
            {
                Legendary.TheTravelersPledge,
                Legendary.TheCompassRose,
                Legendary.CircleOfNailujsEvol,
                Legendary.JessethSkullscythe,
                Legendary.JessethSkullshield,
            }
        };

        #endregion

        private const float CombatRange = 60f;
        private const float TargetRange = 65f;
        private const float GetGlobesPct = 0.7f;

        public override float TrashRange => CombatRange;
        public override float EliteRange => CombatRange;
        public override float HealthGlobeRange => 30f;
        public override float ShrineRange => 40f;
        public override float ClusterRadius => 15f;

        private DateTime CombatBlockedTime = DateTime.UtcNow;
        private DateTime CombatProgressBlockedTime = DateTime.UtcNow;
        private DateTime CombatHealthBlockedTime = DateTime.UtcNow;
        private DateTime NonCombatBlockedTime = DateTime.UtcNow;

        private TrinityPower MySkeletalMage(TrinityActor target)
            => new TrinityPower(Skills.Necromancer.SkeletalMage, 60f, target.AcdId);

        public override Func<bool> ShouldIgnoreNonUnits => InCombatState;

        private bool InCombatState()
        {
            return IsInCombat;
        }

        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;

            TryMovementPower();

            if (TryCursePower(out power))
                return power;

            if (TryBloodPower(out power))
                return power;

            if (ShouldApplyCurseWithScythe() && ShouldGrimScythe(out target))
                return GrimScythe(target);

            if (TryCorpsePower(out power))
                return power;

            if (TryReanimationPower(out power))
                return power;

            if (MyTrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            return null;
        }

        public TrinityPower MyGetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;

            if (!IsInCombat)
                return null;

            if (TryCursePower(out power))
                return power;

            if (TryBloodPower(out power))
                return power;

            if (ShouldApplyCurseWithScythe() && ShouldGrimScythe(out target))
                return GrimScythe(target);

            if (TryCorpsePower(out power))
                return power;

            if (TryReanimationPower(out power))
                return power;

            if (MyTrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            return null;
        }

        private bool ShouldApplyCurseWithScythe()
        {
            // Inarius build with Shadowhook should rarely run out of resource to force primary,
            // so it needs to be occasionally prioritized to apply curses.
            return Runes.Necromancer.CursedScythe.IsActive && Skills.Necromancer.GrimScythe.TimeSinceUse > 2000;
        }

        public TrinityPower GetDefensivePower()
        {
            return GetBuffPower();
        }

        public TrinityPower GetBuffPower()
        {
            if (Skills.Necromancer.BloodRush.CanCast())
            {
                if (CurrentTarget?.Type == TrinityObjectType.ProgressionGlobe)
                {
                    return BloodRush(CurrentTarget.Position);
                }
                if (Player.CurrentHealthPct < 0.25)
                {
                    return BloodRush(Avoider.SafeSpot);
                }
            }
            if (ShouldDevour())
                return Devour();
            // Put up bone armor when running around with high cluster size setting and not yet fighting
            if (Skills.Necromancer.BoneArmor.CanCast() && !Skills.Necromancer.BoneArmor.IsBuffActive && TargetUtil.AnyMobsInRange(15f, 3))
            {
                return BoneArmor();
            }
            return null;
        }

        public TrinityPower GetDestructiblePower()
        {
            if (CurrentTarget == null)
                return null;

            if (CurrentTarget.IsCorruptGrowth && Skills.Necromancer.SkeletalMage.CanCast())
                return MySkeletalMage(CurrentTarget);

            if (Skills.Necromancer.BoneSpikes.CanCast())
                return BoneSpikes(CurrentTarget);

            if (Skills.Necromancer.GrimScythe.CanCast())
                return GrimScythe(CurrentTarget);

            if (Skills.Necromancer.SiphonBlood.CanCast())
                return SiphonBlood(CurrentTarget);

            if (Skills.Necromancer.DeathNova.CanCast())
                return DeathNova(CurrentTarget);

            if (Skills.Necromancer.SkeletalMage.CanCast())
                return SkeletalMage(CurrentTarget.Position);

            if (Skills.Necromancer.DeathNova.CanCast())
                return DeathNova(CurrentTarget);

            if (Skills.Necromancer.BoneSpear.CanCast())
                return BoneSpear(CurrentTarget);

            return DefaultPower;
        }

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            TrinityPower power;
            TrinityActor target;

            if (!IsInCombat || CurrentTarget != null && CurrentTarget.IsElite && CurrentTarget.Position.Distance(destination) <= 10f)
            {
                if (TryBloodrushMovement(destination, out power))
                    return power;
            }
            target = FindClosestTarget();
            if (Player.IsGhosted)
            {
                Core.Avoidance.Avoider.TryGetSafeSpot(out destination);
                return Walk(destination);
            }
            if ((IsValidTarget(target) && target.Position.Distance(Player.Position) > MinimumRange) || (Core.Rift.IsNephalemRift && Core.Rift.IsGaurdianSpawned && IgnoreRange) || (!IsInCombat && !Core.Rift.IsGaurdianSpawned) || Core.Avoidance.InAvoidance(Player.Position))
            {
                target = FindHealthGlobe();
                if ((((Legendary.ReapersWraps.IsEquipped || Legendary.ReapersWraps.IsEquippedInCube) && Player.PrimaryResourcePct < GetGlobesPct) || (Player.CurrentHealthPct < EmergencyHealthPct && CollectForHealth)) && !IsInCombat && target != null && !Core.Avoidance.InAvoidance(Player.Position) && DateTime.Compare(DateTime.UtcNow, NonCombatBlockedTime) > 0)
                {
                    if (IsBlocked)
                        NonCombatBlockedTime = DateTime.UtcNow.AddMilliseconds(10000);
                    else
                        return Walk(target);
                }
                return Walk(destination);
            }
            if (TryOffensivePower(out power))
                return power;
            // Try to walk to myself so as not to return null.
            return Walk(Player.Position);
        }

        #region DecisionHelpers

        private TrinityActor FindBestTarget(float range = TargetRange)
        {
            var units = Core.Targets.Where(u => !u.IsPlayer && u.IsUnit && u.Weight > 0 && u.IsHostile && u.HitPoints > 0 && u.Distance < range).OrderBy(u => u.Distance);
            TrinityActor goblin = null;
            TrinityActor boss = null;
            TrinityActor elite = null;
            TrinityActor minion = null;
            TrinityActor trash = null;

            foreach (var unit in units)
            {
                if (unit.IsTreasureGoblin)
                {
                    if (goblin == null)
                        goblin = unit;
                    if (unit.Distance < goblin.Distance)
                        goblin = unit;
                }
                else if (unit.IsBoss)
                {
                    if (boss == null)
                        boss = unit;
                    if (unit.ActorSnoId == 360636)
                        boss = unit;
                    if (unit.ActorSnoId != 360636 && unit.ActorSnoId != boss.ActorSnoId)
                        boss = unit;
                }
                else if (unit.IsElite && !unit.IsIllusion && unit.EliteType != EliteTypes.Minion)
                {
                    if (elite == null)
                        elite = unit;
                    if (unit.Distance < elite.Distance)
                        elite = unit;
                }
                else if (unit.IsElite && !unit.IsIllusion && unit.EliteType == EliteTypes.Minion)
                {
                    if (minion == null)
                        minion = unit;
                    if (unit.Distance < minion.Distance)
                        minion = unit;
                }
                else
                {
                    if (trash == null)
                        trash = unit;
                    if (unit.Distance < trash.Distance)
                        trash = unit;
                }
            }
            return goblin ?? boss ?? elite ?? minion ?? trash ?? CurrentTarget;
        }

        private TrinityActor FindClusterTarget(float range = TargetRange)
        {
            return Core.Targets.Where(u => !u.IsPlayer && u.IsUnit && u.Weight > 0 && u.IsHostile && u.HitPoints > 0 && u.Distance < range).OrderBy(u => u.NearbyUnitsWithinDistance(6f)).FirstOrDefault() ?? CurrentTarget;
        }

        private TrinityActor FindClosestTarget (float range = TargetRange)
        {
            return Core.Targets.Where(u => !u.IsPlayer && u.IsUnit && u.Weight > 0 && u.IsHostile && u.HitPoints > 0 && u.Distance < range).OrderBy(u => u.Distance).FirstOrDefault() ?? CurrentTarget;
        }

        private TrinityActor FindProgressOrPowerGlobe (float range = 80f)
        {
            var units = Core.Targets.Where(u => (u?.Type == TrinityObjectType.ProgressionGlobe || u?.Type == TrinityObjectType.PowerGlobe) && u.Distance < range && u.IsInLineOfSight).OrderBy(u => u.Distance);
            TrinityActor progressGlobe = null;
            TrinityActor powerGlobe = null;

            foreach (var unit in units)
            {
                if (unit?.Type == TrinityObjectType.ProgressionGlobe)
                {
                    if (progressGlobe == null)
                        progressGlobe = unit;
                    if (unit.Distance < progressGlobe.Distance)
                        progressGlobe = unit;
                }
                else if (unit?.Type == TrinityObjectType.PowerGlobe)
                {
                    if (powerGlobe == null)
                        powerGlobe = unit;
                    if (unit.Distance < powerGlobe.Distance)
                        powerGlobe = unit;
                }
            }
            return progressGlobe ?? powerGlobe;
        }

        private TrinityActor FindHealthGlobe(float range = 40f)
        {
            return Core.Targets.Where(u => u?.Type == TrinityObjectType.HealthGlobe && u.Distance < range && u.IsInLineOfSight).OrderBy(u => u.Distance).FirstOrDefault();
        }

        private bool IsValidTarget(TrinityActor target)
        {
            if (target == null)
                return false;
            return target.IsBoss || target.IsElite || target.IsTrashMob || target.IsTreasureGoblin;
        }

        private bool MyTrySecondaryPower(out TrinityPower power)
        {
            TrinityActor target;
            power = null;

            if (ShouldDeathNova())
                power = DeathNova();

            else if (ShouldSkeletalMage(out target))
                power = MySkeletalMage(target);

            else if (ShouldBoneSpear(out target))
                power = BoneSpear(target);

            return power != null;
        }

        protected override bool ShouldSimulacrum(out Vector3 position)
        {
            position = Vector3.Zero;
            TrinityActor target;

            if (!Skills.Necromancer.Simulacrum.CanCast())
                return false;

            if (!TargetUtil.AnyElitesInRange(LotDRange) && !AlwaysSimulacrum && !HasInstantCooldowns)
                return false;

            target = FindBestTarget();

            if (!IsValidTarget(target))
                return false;
            position = target.Position;
            return true;
        }

        protected override bool ShouldLandOfTheDead(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.LandOfTheDead.CanCast())
                return false;

            if (!TargetUtil.AnyElitesInRange(LotDRange) && !HasInstantCooldowns)
                return false;

            target = FindBestTarget();
            return IsValidTarget(target);
        }

        protected override bool ShouldDevour()
        {
            if (!Skills.Necromancer.Devour.CanCast())
                return false;

            if (Skills.Necromancer.LandOfTheDead.TimeSinceUse <= 10000 && Skills.Necromancer.Devour.TimeSinceUse >= 1800)
                return true;

            if (TargetUtil.CorpseCount(80f) > 0 && Skills.Necromancer.Devour.TimeSinceUse >= 2000)
                return true;

            return false;
        }

        protected override bool ShouldBoneSpikes(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.BoneSpikes.CanCast())
                return false;

            target = FindClusterTarget();
            return IsValidTarget(target);
        }

        protected override bool ShouldSiphonBlood(out TrinityActor target)
        {
            target = null;
            if (!Skills.Necromancer.SiphonBlood.CanCast())
                return false;

            target = FindBestTarget(50f);
            return IsValidTarget(target);
        }

        protected override bool ShouldCommandSkeletons(out TrinityActor target)
        {
            target = null;

            if (!Skills.Necromancer.CommandSkeletons.CanCast())
                return false;

            target = FindBestTarget();
            if (IsValidTarget(target) && Skills.Necromancer.CommandSkeletons.TimeSinceUse >= 3000)
                return true;
            return false;
        }

        private bool ShouldSkeletalMage(out TrinityActor target)
        {
            target = null;
            int skeletalMageCount;

            if (!Skills.Necromancer.SkeletalMage.CanCast())
                return false;

            skeletalMageCount = Core.Actors.Actors.Count(a => a.ActorSnoId == 472606);//counting SkelletonMage

            if ((skeletalMageCount >= 10 && Player.PrimaryResourcePct < CastMagesPct) || (Player.PrimaryResourcePct < CastMagesPct && !QuickCastMages))
                return false;

            target = FindBestTarget();

            return IsValidTarget(target);
        }

        private bool ShouldMove(out TrinityActor target)
        {
            // TrinityActor shrineTarget;
            TrinityActor progressGlobeTarget;
            TrinityActor healthGlobeTarget;
            TrinityActor closestTarget;

            target = FindBestTarget();

            if (!IsValidTarget(target))
                return false;

            closestTarget = FindClosestTarget();
            
            if (!IsValidTarget(closestTarget))
                closestTarget = target;

            if (!IsInCombat && !Core.Rift.IsGaurdianSpawned)
                return false;

            if (Core.Avoidance.InAvoidance(Player.Position))
                return false;

            progressGlobeTarget = FindProgressOrPowerGlobe();
            if (progressGlobeTarget != null)
            {
                if (progressGlobeTarget.Position.Distance(closestTarget.Position) >= closestTarget.Distance && closestTarget.Distance >= 10f && !progressGlobeTarget.IsAvoidanceOnPath && DateTime.Compare(DateTime.UtcNow, CombatProgressBlockedTime) < 0)
                {
                    if (IsBlocked)
                        CombatProgressBlockedTime = DateTime.UtcNow.AddMilliseconds(10000);
                    else
                    {
                        target = progressGlobeTarget;
                        return true;
                    }
                }
            }
            healthGlobeTarget = FindHealthGlobe();
            if (healthGlobeTarget != null)
            {
                if (((Legendary.ReapersWraps.IsEquipped || Legendary.ReapersWraps.IsEquippedInCube) && Player.PrimaryResourcePct < GetGlobesPct) || (Player.CurrentHealthPct <= Settings.EmergencyHealthPct && CollectForHealth))
                {
                    if (healthGlobeTarget.Position.Distance(closestTarget.Position) >= closestTarget.Distance && closestTarget.Distance >= 10f && healthGlobeTarget.Position.Distance(target.Position) <= CombatRange && !healthGlobeTarget.IsAvoidanceOnPath && DateTime.Compare(DateTime.UtcNow, CombatHealthBlockedTime) < 0)
                    {
                        if (IsBlocked)
                            CombatHealthBlockedTime = DateTime.UtcNow.AddMilliseconds(10000);
                        else
                        {
                            target = healthGlobeTarget;
                            return true;
                        }
                    }
                }
            }

            if (DateTime.Compare(DateTime.UtcNow, CombatBlockedTime) < 0)
                return false;

            if (Core.Rift.IsNephalemRift && Core.Rift.IsGaurdianSpawned && IgnoreRange)
                return false;

            if (target.IsAvoidanceOnPath)
                return false;

            if (IsValidTarget(closestTarget) && closestTarget.Position.Distance(Player.Position) <= MaximumRange)
                return false;

            if ((Core.Rift.IsNephalemRift && !Core.Rift.RiftComplete) || !Core.Rift.IsNephalemRift)
            {
                if (IsBlocked)
                {
                    CombatBlockedTime = DateTime.UtcNow.AddMilliseconds(10000);
                    return false;
                }
                return true;
            }
            return false;
        }

        private void TryMovementPower()
        {
            TrinityActor target;

            if (ShouldMove(out target))
                Navigator.PlayerMover.MoveTowards(target.Position);
        }

        public bool TryOffensivePower(out TrinityPower power)
        {
            power = MyGetOffensivePower();
            return power != null;
        }

        #endregion

        #region Settings      

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;
        public float CastMagesPct => Settings.CastMagesPct;
        public bool QuickCastMages => Settings.QuickCastMages;
        public float MinimumRange => Settings.MinimumRange;
        public float MaximumRange => Settings.MaximumRange;
        public float LotDRange => Settings.LotDRange;
        public bool AlwaysSimulacrum => Settings.AlwaysSimulacrum;
        public bool CollectForHealth => Settings.CollectForHealth;
        public bool IgnoreRange => Settings.IgnoreRange;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public NecromancerRathmaFacerollGCSettings Settings { get; } = new NecromancerRathmaFacerollGCSettings();

        public sealed class NecromancerRathmaFacerollGCSettings : NotifyBase, IDynamicSetting
        {
            //private SkillSettings _wrathOfTheBerserker;
            //private SkillSettings _furiousCharge;

            private int _clusterSize;
            private float _emergencyHealthPct;
            private float _castMagesPct;
            private bool _quickCastMages;
            private float _minimumRange;
            private float _maximumRange;
            private float _lotDRange;
            private bool _alwaysSimulacrum;
            private bool _collectForHealth;
            private bool _ignoreRange;

            [DefaultValue(1)]
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

            [DefaultValue(true)]
            public bool CollectForHealth
            {
                get { return _collectForHealth; }
                set { SetField(ref _collectForHealth, value); }
            }

            [DefaultValue(0.80f)]
            public float CastMagesPct
            {
                get { return _castMagesPct; }
                set { SetField(ref _castMagesPct, value); }
            }

            [DefaultValue(true)]
            public bool QuickCastMages
            {
                get { return _quickCastMages; }
                set { SetField(ref _quickCastMages, value); }
            }

            [DefaultValue(25f)]
            public float MinimumRange
            {
                get { return _minimumRange; }
                set { SetField(ref _minimumRange, value); }
            }

            [DefaultValue(40f)]
            public float MaximumRange
            {
                get { return _maximumRange; }
                set { SetField(ref _maximumRange, value); }
            }

            [DefaultValue(40f)]
            public float LotDRange
            {
                get { return _lotDRange; }
                set { SetField(ref _lotDRange, value); }
            }

            [DefaultValue(false)]
            public bool AlwaysSimulacrum
            {
                get { return _alwaysSimulacrum; }
                set { SetField(ref _alwaysSimulacrum, value); }
            }

            [DefaultValue(true)]
            public bool IgnoreRange
            {
                get { return _ignoreRange; }
                set { SetField(ref _ignoreRange, value); }
            }

            //public SkillSettings WrathOfTheBerserker
            //{
            //    get { return _wrathOfTheBerserker; }
            //    set { SetField(ref _wrathOfTheBerserker, value); }
            //}

            //public SkillSettings FuriousCharge
            //{
            //    get { return _furiousCharge; }
            //    set { SetField(ref _furiousCharge, value); }
            //}

            //#region Skill Defaults

            //private static readonly SkillSettings WrathOfTheBerserkerDefaults = new SkillSettings
            //{
            //    UseMode = UseTime.Selective,
            //    Reasons = UseReasons.Elites | UseReasons.HealthEmergency
            //};

            //private static readonly SkillSettings FuriousChargeDefaults = new SkillSettings
            //{
            //    UseMode = UseTime.Default,
            //    RecastDelayMs = 200,
            //    Reasons = UseReasons.Blocked
            //};

            //#endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
                //WrathOfTheBerserker = WrathOfTheBerserkerDefaults.Clone();
                //FuriousCharge = FuriousChargeDefaults.Clone();
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


