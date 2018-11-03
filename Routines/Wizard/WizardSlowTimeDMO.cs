using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Routines.Wizard
{
    public sealed class WizardSlowTimeDMO : WizardBase, IRoutine
    {
        #region RoutineID
        public string DisplayName => "S10 Wizard SlowTime DMO";
        public string Description => "Delsere's Magnum Opus (DMO) set focuses heavily on Slow Time, and the Arcane Orbit \n melee spellcaster capitalizes well on this area-based playstyle.";
        public string Author => "Sequence";
        public string Version => "0.1.8b";
        public string Url => "https://www.icy-veins.com/d3/wizard-arcane-orb-build-with-the-dmo-set-patch-2-5-season-10";
        #endregion

        #region Build Notes
        /* Delsere's Magnum Opus
            * (2) Set: 
            * Casting <ARCANE ORB>, <EXPLOSIVE BLAST>, <SPECTRAL BLADE> reduces the cooldown of Slow Time 
            * by 3 seconds.
            * (4) Set: 
            * You take 60% reduced damage while you have a Slow Time active. Allies 
            * inside your Slow Time gain half benefit.
            * (6) Set: 
            * Enemies affected by your Slow Time and for 5 seconds after exiting take 
            * 3500% increased damage from your <ARCANE ORB>, <EXPLOSIVE BLAST>, <SPECTRAL BLADE>
            * abilities.*/

        /* Slow Time:
            * Invoke a bubble of warped time and space at your <BEST ELITE> || <BESTCLUSTER> location up to 60 
            * yards away for 15 seconds, reducing enemy attack speed by 20% and 
            * movement speed by 60%. This bubble also slows the speed of enemy 
            * projectiles by 90%. 
            * Exhaustion Rune:
            * Enemies caught by Slow Time deal 25% less damage. 
            * SpellHistory.GetSpellLastTargetPosition(Zeta.Game.Internals.Actors.SNOPower.Wizard_SlowTime); */

        /* Arcane Dynamo <Passive>
            * When you cast a <SPECTRAL BLADE>, you gain a Flash of Insight. After 5 
            * Flashes of Insight, your next <ARCANE ORB> spell deals 60% additional 
            * damage. */

        /* DMO routine:
            * - if target is singular => slow time on target, so it will spend the most time in it.
            * - if target is aoe => center of the group should be calculated as densest point and slowtime there.
            * - upon casting, bot should stay as much as possible in the slow time radius for reduce received attack & amplify damage.
            * - bot will try not to stand out of ring as much as possible.
            * - every 5 spectral blade should be followed by an Arcane Orb for maximum damage output
            * - keep ice armor on
            * - bot should always stay in meele range & within slow time bubble when fighting
            * - when taking fatal damage, use teleport <safe passage> as defense mechanism
            * - at 4~5 seconds frequency cast Explosive Blast to increase attack% & dmg reduction%. Reach maximum stack 
            * (16% dmg & 60% dmg reduction as soon as possible) */
        #endregion

        #region Build
        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                {Sets.DelseresMagnumOpus,SetBonus.Third}
            },
            Skills = new Dictionary<Skill, Rune>
            {
                {Skills.Wizard.SpectralBlade,Runes.Wizard.BarrierBlades},
                {Skills.Wizard.Teleport,Runes.Wizard.SafePassage},
                {Skills.Wizard.ArcaneOrb,Runes.Wizard.ArcaneOrbit},
                {Skills.Wizard.SlowTime,Runes.Wizard.Exhaustion},
                {Skills.Wizard.ExplosiveBlast,Runes.Wizard.ChainReaction},
                {Skills.Wizard.IceArmor,Runes.Wizard.Crystallize}
            },
            Passives = new List<Passive>
            {
                {Passives.Wizard.ArcaneDynamo},
                {Passives.Wizard.Illusionist},
                {Passives.Wizard.Audacity},
                {Passives.Wizard.UnstableAnomaly}
            },
            Items = new List<Item>
            {
                {Legendary.TheShameOfDelsere},
            },
        };
        #endregion

        #region FineTuning
        public override KiteMode KiteMode
        => Core.Player.CurrentHealthPct <= Settings.EmergencyHealthPct ? KiteMode.Always : KiteMode.Never;
        #endregion

        #region Buff
        public TrinityPower GetBuffPower()
        {
            if (!Skills.Wizard.IceArmor.IsBuffActive && Skills.Wizard.IceArmor.CanCast())
                return IceArmor();
            return null;
        }
        #endregion

        #region Defensive
        public TrinityPower GetDefensivePower() => null;
        #endregion

        #region DestructiblePower
        public TrinityPower GetDestructiblePower()
        {
            if (CurrentTarget.IsCorruptGrowth)
            {
                Core.Logger.Log(LogCategory.Targetting, $"It's a {CurrentTarget.ActorSnoId}!");
                return GetOffensivePower();
            }
            return DefaultDestructiblePower();
        }
        #endregion

        #region Movement
        public TrinityPower GetMovementPower(Vector3 destination)
        {
            if (CanTeleportTo(destination) && destination.Distance(Core.Player.Position) > 15)
            {
                Core.Logger.Log(LogCategory.Movement, $"Moving method <Coord:{destination}>, Reason: Movement");
                return Teleport(destination);
            }

            return Walk(destination);
        }
        #endregion

        #region Offensive
        public TrinityPower GetOffensivePower()
        {
            #region Skill Descriptions
            /* ExplosiveBlast Cost: 20 Arcane PowerCooldown: 6 secondsGather an infusion of energy around you 
                * that explodes after 1.5 seconds for 945% weapon damage as Arcane to all enemies within 12 yards.
                * 
                * Spectral Blade: This is a Signature spell. Signature spells are free to cast.Summon a spectral 
                * blade that strikes all enemies up to 15 yards in front of you for 168% weapon damage as Arcane. 
                * 
                * Arcane Orb: Cost: 30 Arcane PowerHurl an orb of pure energy that explodes on contact, dealing 435% 
                * weapon damage as Arcane to all enemies within 15 yards.*/
            #endregion

            #region Shorteners
            var targetSelector = TargetUtil.BestEliteInRange(50f) ?? TargetUtil.GetBestClusterUnit(0f, 50f);
            var target = targetSelector ?? CurrentTarget;
            Vector3 position = Vector3.Zero;
            #endregion
            
            if (TrySpecialPower(out var power))
                return power;

            if (TrySecondaryPower(out power))
                return power;

            if (TryPrimaryPower(out power))
                return power;

            if (!Core.Buffs.HasInvulnerableShrine)
            {
                if (Player.CurrentHealthPct < 1 && Skills.Wizard.Teleport.TimeSinceUse > 1200 && CanTeleportTo(position))
                {
                    Core.Avoidance.Avoider.TryGetSafeSpot(out position, 30, 50, Player.Position);

                    if (position == Vector3.Zero)
                        Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15, 50, Player.Position);
                }

                if (Core.Avoidance.InAvoidance(ZetaDia.Me.Position) &&
                    CanTeleportTo(position))
                {
                    Core.Avoidance.Avoider.TryGetSafeSpot(out position, 15, 50, Player.Position);
                }

                if (position != Vector3.Zero &&
                    !TrinityGrid.Instance.IsIntersectedByFlags(
                        ZetaDia.Me.Position,
                        position,
                        AvoidanceFlags.Combat,
                        AvoidanceFlags.CriticalAvoidance))
                {
                    return Teleport(position);
                }
            }

            if (Core.Buffs.HasInvulnerableShrine)
            {
                if (target != null &&
                    TrinityGrid.Instance.CanRayCast(position) &&
                    CanTeleportTo(position) &&
                    Skills.Wizard.Teleport.TimeSinceUse > 1200)
                {
                    if (TargetUtil.UnitsBetweenLocations(ZetaDia.Me.Position, CurrentTarget.Position).Count > 3 && CurrentTarget.Distance < 50)
                        return Teleport(position);
                }
            }

            if (CanTeleportTo(CurrentTarget.Position) &&
                TargetUtil.UnitsBetweenLocations(ZetaDia.Me.Position, CurrentTarget.Position).Count > 3)
            {
                return Teleport(CurrentTarget.Position);
            }

            return Walk(CurrentTarget.Position);
        }
        #endregion

        #region SlowTime
        protected override bool ShouldSlowTime(out Vector3 position)
        {
            position = Vector3.Zero;

            var skill = Skills.Wizard.SlowTime;
            if (!skill.CanCast())
                return false;

            var myPosition = ZetaDia.Me.Position;
            var clusterPosition = TargetUtil.GetBestClusterPoint(0f, 50f);
            var bubbles = SpellHistory.FindSpells(skill.SNOPower, 12).ToList();
            var bubblePositions = new List<Vector3>(bubbles.Select(b => b.TargetPosition));
            var isDefensiveRune = Runes.Wizard.PointOfNoReturn.IsActive || Runes.Wizard.StretchTime.IsActive || Runes.Wizard.Exhaustion.IsActive;

            Func<Vector3, bool> isBubbleAtPosition = pos => bubblePositions.Any(b => b.Distance(pos) <= 14f && pos.Distance(myPosition) < SlowTimeRange);

            // On Self            
            if (TargetUtil.ClusterExists(15f, 60f, 8) && isDefensiveRune && !isBubbleAtPosition(myPosition))
            {
                position = MathEx.GetPointAt(myPosition, 10f, Player.Rotation);
                return true;
            }

            // On Elites
            if (CurrentTarget.IsElite && CurrentTarget.Distance < SlowTimeRange && !isBubbleAtPosition(CurrentTarget.Position))
            {
                position = CurrentTarget.Position;
                return true;
            }

            // On Clusters            
            if (TargetUtil.ClusterExists(50f, 5) && clusterPosition.Distance(myPosition) < SlowTimeRange && !isBubbleAtPosition(clusterPosition))
            {
                position = clusterPosition;
                return true;
            }

            // Delseres Magnum Opus Set
            if (Sets.DelseresMagnumOpus.IsEquipped)
            {
                var isLargeCluster = Core.Clusters.LargeCluster.Exists && Core.Clusters.LargeCluster.Position.Distance(myPosition) < SlowTimeRange;
                if (isLargeCluster && !isBubbleAtPosition(Core.Clusters.LargeCluster.Position))
                {
                    position = Core.Clusters.LargeCluster.Position;
                    return true;
                }

                var isAnyCluster = Core.Clusters.BestCluster.Exists && Core.Clusters.BestCluster.Position.Distance(myPosition) < SlowTimeRange;
                if (isAnyCluster && !isBubbleAtPosition(Core.Clusters.BestCluster.Position))
                {
                    position = Core.Clusters.BestCluster.Position;
                    return true;
                }

                if (!isBubbleAtPosition(myPosition))
                {
                    position = MathEx.GetPointAt(myPosition, 10f, Player.Rotation);
                    return true;
                }
            }

            return false;
        }
        #endregion SlowTime

        #region ExplosiveBast
        protected override bool ShouldExplosiveBlast(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Wizard.ExplosiveBlast.CanCast())
                return false;
            if (Player.CurrentHealthPct < 0.8 && CurrentTarget.HasDebuff(SNOPower.Wizard_SlowTime))
                return true;
            if ((Core.Buffs.ConventionElement == Element.Fire || Core.Buffs.ConventionElement == Element.Lightning || Core.Buffs.ConventionElement == Element.Arcane) && CurrentTarget.HasDebuff(SNOPower.Wizard_SlowTime))
                return true;
            return false;
        }
        #endregion ExlpsiveBlast

        #region SpectralBlade
        protected override bool ShouldSpectralBlade(out TrinityActor target)
        {
            target = TargetUtil.GetBestClusterUnit(0f, 50f);
            var arcaneDynamo = Core.Buffs.GetBuffStacks(SNOPower.Wizard_Passive_ArcaneDynamo); /* Wizard_Passive_ArcaneDynamo = 208823, */

            if (!Skills.Wizard.SpectralBlade.CanCast())
                return false;
            if (arcaneDynamo <= 5 && (Skills.Wizard.ArcaneOrb.TimeSinceUse >= 500 || Skills.Wizard.ArcaneOrb.TimeSinceUse <= 4500) && target != null)
                return true;
            return false;
        }
        #endregion SpectralBlade

        #region ArcaneOrb
        protected override bool ShouldArcaneOrb(out TrinityActor target)
        {
            target = TargetUtil.GetBestClusterUnit(0f, 50f) ?? CurrentTarget;
            var arcaneDynamo = Core.Buffs.GetBuffStacks(SNOPower.Wizard_Passive_ArcaneDynamo); /* Wizard_Passive_ArcaneDynamo = 208823, */

            if (!Skills.Wizard.ArcaneOrb.CanCast())
                return false;

            if (target == null)
                return false;

            if (arcaneDynamo < 5)
                return false;

            if (Skills.Wizard.ArcaneOrb.IsLastUsed || Skills.Wizard.ArcaneOrb.TimeSinceUse < 1000)
                return false;

            if (Player.PrimaryResource < PrimaryEnergyReserve)
                return false;
            if (Player.CurrentHealthPct < 0.8 && arcaneDynamo == 5 && CurrentTarget.HasDebuff(SNOPower.Wizard_SlowTime))
                return true;
            if (arcaneDynamo == 5 && CurrentTarget.HasDebuff(SNOPower.Wizard_SlowTime) && target != null && (Core.Buffs.ConventionElement == Element.Arcane || Core.Buffs.ConventionElement == Element.Cold || Core.Buffs.ConventionElement == Element.Fire))
                return true;

            return false;
        }
        #endregion ArcaneOrb

        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;

        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WizardSlowTimeDMOSettings Settings { get; } = new WizardSlowTimeDMOSettings();

        public sealed class WizardSlowTimeDMOSettings : NotifyBase, IDynamicSetting
        {
            private SkillSettings _teleport;
            private int _clusterSize;
            private float _emergencyHealthPct;

            [DefaultValue(8)]
            public int ClusterSize
            {
                get => _clusterSize;
                set => SetField(ref _clusterSize, value);
            }

            [DefaultValue(0.7f)]
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

            #region Skill Defaults

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

            #region IDynamicSetting

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
 