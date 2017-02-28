using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Adventurer.UI.UIComponents.RadarCanvas;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Memory.Symbols.Types;
using Trinity.Reference;
using Trinity.Routines.Crusader;
using Trinity.Settings;
using Trinity.UI;
using Trinity.UI.Visualizer.RadarCanvas;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Routines.Wizard
{
    public sealed class WizardVyRashaManaldArchon : WizardBase, IRoutine
    {
        #region Definition

        public string DisplayName => "VyRasha Manald Archon GR Build";
        public string Description => "This RANGED build uses Tal Rasha's damage buff along with Manald Heal and Archon to produce massive DPS. It is designed to stay at range to benefit from ranged gear/gems/passives. \n     Cooldown Reduction:  Required in every slot except amulet. Diamond in Helm. \n     Attack Speed:  Get as many 7% rolls as you can, up to 4. CDR > Attack Speed!  \n     Lightning Damage:  Only get it on Bracers, damage percent does not matter. \n         It changes Archon to lightning, Manald Heal does not benefit!";
        public string Author => "TwoCigars";
        public string Version => "1.9";
        public string Url => "Guide http://www.icy-veins.com/d3/wizard-manald-archon-build-patch-2-4-3-season-9 \n\n D3Planner http://www.d3planner.com/599182396 \n\n Manald Info: https://us.battle.net/forums/en/d3/topic/20752649109";
        public Build BuildRequirements => new Build
        {
            Sets = new Dictionary<Set, SetBonus>
            {
                { Sets.TalRashasElements, SetBonus.Third },
                { Sets.VyrsAmazingArcana, SetBonus.Second }
            },
            Skills = new Dictionary<Skill, Rune>
            {
                { Skills.Wizard.Archon, null },
                { Skills.Wizard.ArcaneTorrent, null }
            },
            Items = new List<Item>
            {
                Legendary.ManaldHeal,
                Legendary.Starfire
            },
        };

        public override Func<bool> ShouldIgnoreAvoidance => () => IsArchonActive;
        public override Func<bool> ShouldIgnoreKiting => () => IsArchonActive;
        public static int BlizzardAPCost => Runes.Wizard.Snowbound.IsActive ? 10 : 40;

        #endregion


        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;
            Vector3 position = Vector3.Zero;

            if (ShouldArchon())
                return Archon();

            if (TryKitingTeleport(out position))
                return Teleport(position);

            if (IsArchonActive)
            {
                if (ShouldArchonDisintegrationWave(out target))
                    return ArchonDisintegrationWave(target);

                return null;
            }


            if (!IsArchonActive)
            {

                if (ShouldTeleport(out position))
                    return Teleport(position);

                if (ShouldBlackHole(out target))
                    return BlackHole(target);

                if (ShouldExplosiveBlast(out position))
                    return ExplosiveBlast(position);

                if (ShouldBlizzard(out target))
                    return Blizzard(target);

                if (ShouldEnergyTwister(out target))
                    return EnergyTwister(target);

                if (ShouldArcaneTorrent(out target))
                    return ArcaneTorrent(target);
            }
            return null;
        }

        public TrinityPower GetDefensivePower() => GetBuffPower();

        public TrinityPower GetBuffPower()
        {
            TrinityPower power = null;

            if (!Player.IsInTown)
            {
                if (TryBuffPower(out power))
                    return power;
            }

            var avoidanceTest = Core.Avoidance.GridEnricher.AvoidanceCentroid;
            RadarDebug.Draw(new List<Vector3> { avoidanceTest }, 100, RadarDebug.DrawType.Elipse, RadarDebug.DrawColor.Blue);

            var avoidanceActors = Core.Avoidance.CurrentAvoidances.SelectMany(av => av.Actors);
            var avoidanceActorPositions = avoidanceActors.Select(a => a.Position);
            RadarDebug.Draw(new List<Vector3>(avoidanceActorPositions), 100, RadarDebug.DrawType.Elipse, RadarDebug.DrawColor.White);

            var customCentroid = TargetUtil.GetCentroid(avoidanceActorPositions);
            RadarDebug.Draw(new List<Vector3> { customCentroid }, 100, RadarDebug.DrawType.Elipse, RadarDebug.DrawColor.Yellow);

            return null;
        }

        public TrinityPower GetDestructiblePower() => DefaultDestructiblePower();

        private readonly List<TrinityObjectType> _priorityTarget = new List<TrinityObjectType>
        {
            TrinityObjectType.ProgressionGlobe,
            TrinityObjectType.HealthGlobe,
            TrinityObjectType.Shrine,
        };

        public TrinityPower GetMovementPower(Vector3 destination)
        {
            var distance = destination.Distance(Player.Position);
            var isDestinationUnitTarget = IsInCombat && CurrentTarget.IsUnit && destination.Distance(CurrentTarget.Position) < 30f;
            var isPriorityTarget = CurrentTarget != null && _priorityTarget.Contains(CurrentTarget.Type);
            var teleportCanBeCast = Skills.Wizard.Teleport.CanCast() || Skills.Wizard.ArchonTeleport.CanCast();

            //if (teleportCanBeCast && IsArchonActive && isPriorityTarget && CurrentTarget.Distance < 50f && !Core.Avoidance.InAvoidance(CurrentTarget.Position))
            //{
            //    destination = CurrentTarget.Position;
            //    return Teleport(destination);
            //}

            if (CanTeleportTo(destination))
            {
                if (distance > 35f && !isDestinationUnitTarget || IsBlocked)
                {
                    return Teleport(destination);
                }
            }

            return Walk(destination);


        }

        public bool TryKitingTeleport(out Vector3 position)
        {
            position = Vector3.Zero;
            var teleportCanBeCast = Skills.Wizard.Teleport.CanCast() || Skills.Wizard.ArchonTeleport.CanCast();
            var affixOnPlayer = Core.Avoidance.InAvoidance(Player.Position);

            //Teleport Activations
            var healthIsLow = Player.CurrentHealthPct < Settings.TeleportHealthEmergency;
            var archonHealthIsLow = Player.CurrentHealthPct < Settings.ArchonTeleportHealthEmergency;
            var anyBossInRange = TargetUtil.AnyBossesInRange(Settings.TeleportEliteKiteRange);
            var anyElitesinRange = TargetUtil.AnyElitesInRange(Settings.TeleportEliteKiteRange);
            var anyMobsInRange = TargetUtil.AnyMobsInRangeOfPosition(Player.Position, Settings.TeleportTrashKiteRange, Settings.TeleportTrashInRangeCount);

            //Teleport Delays
            var archonHealthIsLowDelay = Skills.Wizard.Teleport.TimeSinceUse > Settings.ArchonTeleportHealthDelay;
            var archonCannotMoveDelay = Skills.Wizard.Teleport.TimeSinceUse > Settings.ArchonStuckOrBlockedDelay;
            var archonTeleportDelay = (TalRashaStacks == 4 && Skills.Wizard.Teleport.TimeSinceUse > Settings.Archon4StackDelay) || (TalRashaStacks < 4 && Skills.Wizard.Teleport.TimeSinceUse > Settings.Archon1StackDelay);

            if (IsArchonActive && teleportCanBeCast)
            {
                if (Player.IsChannelling)
                {
                    if (archonCannotMoveDelay && IsStuck || IsBlocked)
                    {
                        //Logger.Log($"Teleport! Stuck:{IsStuck}, Blocked {IsBlocked}");
                        Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
                    }

                    if (archonHealthIsLowDelay && archonHealthIsLow)
                    {
                        //Logger.Log($"Break Channel for Teleport! Health {Player.CurrentHealthPct} < Setting {archonHealthIsLow}");
                        Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
                    }

                    if (anyBossInRange && affixOnPlayer)
                    {
                        //Logger.Log($"Boss Affix Teleport! Elite:{anyBossInRange}, Affix On Player: {affixOnPlayer}");
                        Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
                    }

                    if (archonTeleportDelay && anyBossInRange || anyElitesinRange || anyMobsInRange)
                    {
                        //Logger.Log($"Proximity Teleport! Elite:{anyElitesinRange}, Close Mobs: {anyMobsInRange}");
                        Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
                    }
                }

                if (archonCannotMoveDelay && IsStuck || IsBlocked)
                {
                    //Logger.Log($"Teleport! Stuck:{IsStuck}, Blocked {IsBlocked}");
                    Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
                }

                if (archonHealthIsLowDelay && archonHealthIsLow)
                {
                    //Logger.Log($"Break Channel for Teleport! Health {Player.CurrentHealthPct} < Setting {archonHealthIsLow}");
                    Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
                }

                if (anyBossInRange && affixOnPlayer)
                {
                    //Logger.Log($"Boss Affix Teleport! Elite:{anyBossInRange}, Affix On Player: {affixOnPlayer}");
                    Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
                }

                if (archonTeleportDelay && anyBossInRange || anyElitesinRange || anyMobsInRange)
                {
                    //Logger.Log($"Proximity Teleport! Elite:{anyElitesinRange}, Close Mobs: {anyMobsInRange}");
                    Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
                }
            }

            if (!IsArchonActive && teleportCanBeCast)
            {
                if (IsStuck || IsBlocked)
                {
                    //Logger.Log($"Teleport! Stuck:{IsStuck}, Blocked {IsBlocked}");
                    Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
                }

                if (Player.IsChannelling)
                {
                    if (anyBossInRange || anyElitesinRange || anyMobsInRange || healthIsLow)
                    {
                        //Logger.Log($"Break Channel for Teleport! Elite:{anyElitesinRange}, Close Mobs: {anyMobsInRange}, Health: {healthIsLow}");
                        Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
                    }
                }

                if (anyBossInRange || anyElitesinRange || anyMobsInRange || healthIsLow)
                {
                    //Logger.Log($"Proximity Teleport! Elite:{anyElitesinRange}, Close Mobs: {anyMobsInRange}, Health: {healthIsLow}");
                    Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
                }
            }

            return position != Vector3.Zero;
        }


        /* Non-Archon skills below: Used for DPS and Building Tal Rasha Stacks
         * 
         * Stacks are earned by casting an elemental skill: Lightning, Fire, Arcane and Cold.
         * Each stack lasts for 8 seconds, and they do not refresh each other.  
         * Each stack adds: 750% damage to attacks, 25% All resistance, Cause a matching elemental meteor to fall [8 second cooldown]
         * Note that these skills do not need to actually deal damage to any enemies to proc a stack. 
          */

        protected override bool ShouldArcaneTorrent(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.ArcaneTorrent.CanCast())
                return false;

            if (!(TalRashaStacks >= 2))
                return false;

            if (Player.PrimaryResource < Settings.ArcaneResourceReserve)
                return false;


            target = TargetUtil.BestAoeUnit(Settings.ArcaneTorrentRange, true);
            if (TargetUtil.BestAoeUnit(Settings.ArcaneTorrentRange, true) != null && TalRashaStacks >= 3 && Skills.Wizard.Archon.CooldownRemaining < 6000)
                return true;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            if (TargetUtil.BestAoeUnit(Settings.ArcaneTorrentRange, true) == null && TalRashaStacks >= 3 && Skills.Wizard.Archon.CooldownRemaining < 6000)
                return true;

            return target != null;
        }

        protected override bool ShouldBlackHole(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.BlackHole.CanCast())
                return false;

            if (Skills.Wizard.BlackHole.TimeSinceUse < Settings.BlackHoleDelay)
                return false;

            target = TargetUtil.BestEliteInRange(50, true);
            if (target != null)
                return true;

            target = TargetUtil.BestAoeUnit(50, true);
            if (TargetUtil.BestAoeUnit(50, true) != null)
                return true;

            //Make sure that we get all of our Tal Stacks before Archon!
            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            if (Skills.Wizard.Archon.CooldownRemaining < 2000 && TalRashaStacks < 3)
                return true;

            return target != null;
        }

        public static float ExplosiveBlastRange => Runes.Wizard.Obliterate.IsActive ? 18f : 12f;

        protected override bool ShouldExplosiveBlast(out Vector3 position)
        {
            position = Vector3.Zero;

            if (!Skills.Wizard.ExplosiveBlast.CanCast())
                return false;

            if (Skills.Wizard.ExplosiveBlast.TimeSinceUse < Settings.ExplosiveBlastDelay)
                return false;

            if (!TargetUtil.AnyMobsInRange(ExplosiveBlastRange, false))
                return false;

            //When Archon is almost ready, make sure we get our stacks!
            if ((Skills.Wizard.Archon.CooldownRemaining < 2000) && (TalRashaStacks < 3) && Skills.Wizard.ExplosiveBlast.TimeSinceUse > 6000)
                return true;

            position = Player.Position;
            return position != Vector3.Zero;
        }

        protected override bool ShouldBlizzard(out TrinityActor target)
        {

            target = null;
            var blizzardDelay = Runes.Wizard.Snowbound.IsActive ? Settings.BlizzardSnowboundDelay : Settings.BlizzardFrozenSolidDelay;
            var skill = Skills.Wizard.Blizzard;

            if (!skill.CanCast())
                return false;

            if (skill.TimeSinceUse < blizzardDelay)
                return false;

            target = TargetUtil.GetBestClusterUnit(BlizzardRadius, 70f) ?? CurrentTarget;
            if (target == null)
                return false;

            var isExistingBlizzard = SpellHistory.FindSpells(skill.SNOPower, target.Position, BlizzardRadius, 6).Any();
            return !isExistingBlizzard;

        }

        protected override bool ShouldTeleport(out Vector3 position)
        {
            position = Vector3.Zero;

            var healthIsLow = Player.CurrentHealthPct < Settings.TeleportHealthEmergency;
            var archonHealthIsLow = Player.CurrentHealthPct < Settings.ArchonTeleportHealthEmergency;
            var anyBossInRange = TargetUtil.AnyBossesInRange(Settings.TeleportEliteKiteRange);
            var anyElitesinRange = TargetUtil.AnyElitesInRange(Settings.TeleportEliteKiteRange);
            var anyMobsInRange = TargetUtil.AnyMobsInRangeOfPosition(Player.Position, Settings.TeleportTrashKiteRange, Settings.TeleportTrashInRangeCount);

            if (!Skills.Wizard.Teleport.CanCast())
                return false;

            if (IsStuck || IsBlocked)
                return true;

            if (Player.IsChannelling && anyBossInRange || anyElitesinRange || anyMobsInRange || healthIsLow)
                return true;

            if (anyBossInRange || anyElitesinRange || anyMobsInRange || healthIsLow)
                return true;

            Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
            return position != Vector3.Zero;
        }

        protected override bool ShouldEnergyTwister(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.EnergyTwister.CanCast())
                return false;

            if (Skills.Wizard.EnergyTwister.TimeSinceUse < Settings.EnergyTwisterDelay)
                return false;

            target = TargetUtil.BestEliteInRange(50, true);
            if (target != null)
                return true;

            target = TargetUtil.BestAoeUnit(50, true);
            if (TargetUtil.BestAoeUnit(50, true) != null)
                return true;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            if (Skills.Wizard.Archon.CooldownRemaining < 2000 && TalRashaStacks < 3)
                return true;

            target = TargetUtil.GetBestClusterUnit() ?? CurrentTarget;
            return target != null;
        }




        /*Achon and associated Skills below. 
         *  This is a ranged build therefore Disintegration Wave is the only DPS skill used. 
         *  With StarFire, Power Hungry and Zei's Stone, damage is too great at distance to fight melee.
        */

        protected override bool ShouldArchon()
        {
            if (!Skills.Wizard.Archon.CanCast())
                return false;

            if (Player.CurrentHealthPct < Settings.ShouldArchonHealthPct)
            {
                Logger.Log($"Emergency Archon: Health at {Player.CurrentHealthPct}%! (Setting: {Settings.ShouldArchonHealthPct}%)");
                return true;
            }

            if (Player.PrimaryResource < Settings.ShouldArchonLowResource)
            {
                Logger.Log($"Emergency Archon: Arcane power at {Player.PrimaryResource}! (Setting: {Settings.ShouldArchonLowResource})");
                return true;
            }

            if (TalRashaStacks < 3)
                return false;

            if (!TargetUtil.AnyMobsInRange(50f))
                return false;

            return true;
        }

        protected override bool ShouldArchonDisintegrationWave(out TrinityActor target)
        {
            target = null;

            if (!Skills.Wizard.ArchonDisintegrationWave.CanCast())
                return false;

            target = TargetUtil.BestEliteInRange(Settings.DisintegrationWaveRange, true);
            if (target != null && target.Distance > Settings.DisintegrationWaveMinDistance)
                return true;

            target = TargetUtil.BestPierceOrClusterUnit(15, Settings.DisintegrationWaveRange, true);
            if (target != null && target.Distance > Settings.DisintegrationWaveMinDistance)
                return true;

            target = TargetUtil.BestAoeUnit(Settings.DisintegrationWaveRange, true);
            if (target != null && target.Distance > Settings.DisintegrationWaveMinDistance)
                return true;

            return target != null;
        }

        protected override bool ShouldArchonTeleport(out Vector3 position)
        {
            position = Vector3.Zero;
            var affixOnPlayer = Core.Avoidance.InAvoidance(Player.Position);

            //Teleport Activations
            var archonHealthIsLow = Player.CurrentHealthPct < Settings.ArchonTeleportHealthEmergency;
            var anyBossInRange = TargetUtil.AnyBossesInRange(Settings.TeleportEliteKiteRange);
            var anyElitesinRange = TargetUtil.AnyElitesInRange(Settings.TeleportEliteKiteRange);
            var anyMobsInRange = TargetUtil.AnyMobsInRangeOfPosition(Player.Position, Settings.TeleportTrashKiteRange, Settings.TeleportTrashInRangeCount);

            //Teleport Delays
            var archonHealthIsLowDelay = Skills.Wizard.Teleport.TimeSinceUse > Settings.ArchonTeleportHealthDelay;
            var archonCannotMoveDelay = Skills.Wizard.Teleport.TimeSinceUse > Settings.ArchonStuckOrBlockedDelay;
            var archonTeleportDelay = (TalRashaStacks == 4 && Skills.Wizard.Teleport.TimeSinceUse > Settings.Archon4StackDelay) || (TalRashaStacks < 4 && Skills.Wizard.Teleport.TimeSinceUse > Settings.Archon1StackDelay);

            if (!Skills.Wizard.Teleport.CanCast())
                return false;

            if (Player.IsChannelling)
            {
                if (archonCannotMoveDelay && IsStuck || IsBlocked)
                    return true;

                if (archonHealthIsLowDelay && archonHealthIsLow)
                    return true;

                if (anyBossInRange && affixOnPlayer)
                    return true;

                if (archonTeleportDelay && anyBossInRange || anyElitesinRange || anyMobsInRange)
                    return true;
            }

            if (archonCannotMoveDelay && IsStuck || IsBlocked)
                return true;

            if (archonHealthIsLowDelay && archonHealthIsLow)
                return true;

            if (anyBossInRange && affixOnPlayer)
                return true;

            if (archonTeleportDelay && anyBossInRange || anyElitesinRange || anyMobsInRange)
                return true;


            Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance);
            return position != Vector3.Zero;
        }




        /*Buff Skills Below. 
         *  All Armor lasts 10 minutes, all are set to cast when not in town and when buff is not active.
         *  Magic Weapon lasts 10 minutes. Activation same as Armor.
         *  Archon SlowTime is basically on or off, therefore it will be kept on and is used as a buff.
         *  Archon Blast does not interrupt any animations and can cast while moving, so it's spammed as a buff.  
        */

        protected override bool ShouldArchonSlowTime()
        {
            //This should always be on, leave in Buff Power
            if (!Skills.Wizard.ArchonSlowTime.CanCast())
                return false;

            if (!IsArchonSlowTimeActive)
                return true;

            return false;
        }

        protected override bool ShouldArchonBlast()
        {
            //Should be constantly spammed: Does not interrupt other skills: Keep in Buff Power
            if (!Skills.Wizard.ArchonBlast.CanCast())
                return false;

            return true;
        }

        protected override bool ShouldIceArmor()
        {
            if (!Skills.Wizard.IceArmor.CanCast())
                return false;

            if (Skills.Wizard.IceArmor.IsBuffActive)
                return false;

            return true;
        }

        protected override bool ShouldStormArmor()
        {
            if (!Skills.Wizard.StormArmor.CanCast())
                return false;

            if (Skills.Wizard.StormArmor.IsBuffActive)
                return false;

            return true;
        }

        protected override bool ShouldEnergyArmor()
        {
            if (!Skills.Wizard.EnergyArmor.CanCast())
                return false;

            if (Skills.Wizard.EnergyArmor.IsBuffActive)
                return false;

            return true;
        }

        protected override bool ShouldMagicWeapon()
        {
            if (!Skills.Wizard.MagicWeapon.CanCast())
                return false;

            if (Skills.Wizard.MagicWeapon.IsBuffActive)
                return false;

            return true;
        }


        #region Settings

        public override int ClusterSize => Settings.ClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;


        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WizardVyRashaManaldArchonSettings Settings { get; } = new WizardVyRashaManaldArchonSettings();

        public sealed class WizardVyRashaManaldArchonSettings : NotifyBase, IDynamicSetting
        {
            //General
            private int _clusterSize;
            private float _emergencyHealthPct;

            //Teleport: Where to look for Safe Spot
            private float _teleportKiteMinDistance;
            private float _teleportKiteMaxDistance;

            //Teleport Activation
            private float _teleportHealthEmergency;
            private float _archonTeleportHealthEmergency;
            private float _teleportEliteKiteRange;
            private float _teleportTrashKiteRange;
            private int _teleportTrashInRangeCount;

            // Teleport Delay                   Note: No delay for Non-Archon as we always want to move from enemies
            private int _archonTeleportHealthDelay;
            private int _archon4StackDelay;
            private int _archon1StackDelay;
            private int _archonStuckOrBlockedDelay;

            //Archon Activation and Skills
            private float _shouldArchonHealthPct;
            private int _shouldArchonLowResource;
            private float _disintegrationWaveMinDistance;
            private float _disintegrationWaveRange;

            //Arcane Torrent
            private float _arcaneTorrentRange;
            private int _arcaneResourceReserve;

            //Tal Rasha Stacks - Elemental Skill Delays
            private int _blackHoleDelay;
            private int _blizzardSnowboundDelay;
            private int _blizzardFrozenSolidDelay;
            private int _energyTwisterDelay;
            private int _explosiveBlastDelay;




            //General
            [DefaultValue(5)]
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


            //Teleport: Where to look for Safe Spot
            [DefaultValue(30f)]
            public float TeleportKiteMinDistance
            {
                get { return _teleportKiteMinDistance; }
                set { SetField(ref _teleportKiteMinDistance, value); }
            }

            [DefaultValue(50)]
            public float TeleportKiteMaxDistance
            {
                get { return _teleportKiteMaxDistance; }
                set { SetField(ref _teleportKiteMaxDistance, value); }
            }


            //Teleport: Activations
            [DefaultValue(0.4f)]
            public float TeleportHealthEmergency
            {
                get { return _teleportHealthEmergency; }
                set { SetField(ref _teleportHealthEmergency, value); }
            }

            [DefaultValue(0.3f)]
            public float ArchonTeleportHealthEmergency
            {
                get { return _archonTeleportHealthEmergency; }
                set { SetField(ref _archonTeleportHealthEmergency, value); }
            }

            [DefaultValue(30)]
            public float TeleportEliteKiteRange
            {
                get { return _teleportEliteKiteRange; }
                set { SetField(ref _teleportEliteKiteRange, value); }
            }

            [DefaultValue(20)]
            public float TeleportTrashKiteRange
            {
                get { return _teleportTrashKiteRange; }
                set { SetField(ref _teleportTrashKiteRange, value); }
            }

            [DefaultValue(3)]
            public int TeleportTrashInRangeCount
            {
                get { return _teleportTrashInRangeCount; }
                set { SetField(ref _teleportTrashInRangeCount, value); }
            }


            // Teleport Delay                   Note: No delay for Non-Archon as we always want to move from enemies
            [DefaultValue(1000)]
            public int ArchonTeleportHealthDelay
            {
                get { return _archonTeleportHealthDelay; }
                set { SetField(ref _archonTeleportHealthDelay, value); }
            }

            [DefaultValue(2000)]
            public int Archon4StackDelay
            {
                get { return _archon4StackDelay; }
                set { SetField(ref _archon4StackDelay, value); }
            }

            [DefaultValue(1000)]
            public int Archon1StackDelay
            {
                get { return _archon1StackDelay; }
                set { SetField(ref _archon1StackDelay, value); }
            }

            [DefaultValue(1000)]
            public int ArchonStuckOrBlockedDelay
            {
                get { return _archonStuckOrBlockedDelay; }
                set { SetField(ref _archonStuckOrBlockedDelay, value); }
            }


            //Archon Activation and Skills
            [DefaultValue(0.4f)]
            public float ShouldArchonHealthPct
            {
                get { return _shouldArchonHealthPct; }
                set { SetField(ref _shouldArchonHealthPct, value); }
            }

            [DefaultValue(20)]
            public int ShouldArchonLowResource
            {
                get { return _shouldArchonLowResource; }
                set { SetField(ref _shouldArchonLowResource, value); }
            }

            [DefaultValue(30)]
            public float DisintegrationWaveMinDistance
            {
                get { return _disintegrationWaveMinDistance; }
                set { SetField(ref _disintegrationWaveMinDistance, value); }
            }

            [DefaultValue(50)]
            public float DisintegrationWaveRange
            {
                get { return _disintegrationWaveRange; }
                set { SetField(ref _disintegrationWaveRange, value); }
            }


            //Arcane Torrent
            [DefaultValue(55)]
            public float ArcaneTorrentRange
            {
                get { return _arcaneTorrentRange; }
                set { SetField(ref _arcaneTorrentRange, value); }
            }

            [DefaultValue(25)]
            public int ArcaneResourceReserve
            {
                get { return _arcaneResourceReserve; }
                set { SetField(ref _arcaneResourceReserve, value); }
            }


            //Tal Rasha Stacks - Elemental Skill Delays
            [DefaultValue(6000)]
            public int BlackHoleDelay
            {
                get { return _blackHoleDelay; }
                set { SetField(ref _blackHoleDelay, value); }
            }

            [DefaultValue(2000)]
            public int BlizzardSnowboundDelay
            {
                get { return _blizzardSnowboundDelay; }
                set { SetField(ref _blizzardSnowboundDelay, value); }
            }

            [DefaultValue(6000)]
            public int BlizzardFrozenSolidDelay
            {
                get { return _blizzardFrozenSolidDelay; }
                set { SetField(ref _blizzardFrozenSolidDelay, value); }
            }

            [DefaultValue(4000)]
            public int EnergyTwisterDelay
            {
                get { return _energyTwisterDelay; }
                set { SetField(ref _energyTwisterDelay, value); }
            }

            [DefaultValue(4000)]
            public int ExplosiveBlastDelay
            {
                get { return _explosiveBlastDelay; }
                set { SetField(ref _explosiveBlastDelay, value); }
            }


            #region Skill Defaults



            #endregion

            public override void LoadDefaults()
            {
                base.LoadDefaults();
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


