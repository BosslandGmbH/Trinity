using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.DbProvider;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.UI;
using Zeta.Common;
using Zeta.Game;


namespace Trinity.Routines.Wizard
{
    public sealed class WizardVyRashaManaldArchon : WizardBase, IRoutine
    {
        #region Definition

        public string DisplayName => "VyRasha Manald Archon GR Build";
        public string Description => "This RANGED build uses Tal Rasha's damage buff along with Manald Heal and Archon to produce massive DPS. It is designed to stay at range to benefit from ranged gear/gems/passives. \n     Cooldown Reduction:  Required in every slot except amulet. Diamond in Helm. \n     Attack Speed:  Get as many 7% rolls as you can, up to 4. CDR > Attack Speed!  \n     Lightning Damage:  Only get it on Bracers, damage percent does not matter. \n         It changes Archon to lightning, Manald Heal does not benefit!";
        public string Author => "TwoCigars";
        public string Version => "2.4";
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
                Legendary.Starfire,
                Legendary.TheSwami,
                Legendary.FazulasImprobableChain
            },
        };

       
        public override Func<bool> ShouldIgnoreAvoidance => () => true;
        public override Func<bool> ShouldIgnoreKiting => () => true;
        //public static int BlizzardAPCost => Runes.Wizard.Snowbound.IsActive ? 10 : 40;

        #endregion


        public TrinityPower GetOffensivePower()
        {
            TrinityPower power;
            TrinityActor target;
            Vector3 position = Vector3.Zero;

            if (ShouldArchon())
                return Archon();

            if (ShouldCancelArchon())
            {
                Core.Logger.Log(LogCategory.Routine, $"Canceling Archon: Tal's Stacks {TalRashaStacks} Has Cooldown Pylon: {HasInfiniteCasting}");
                CancelArchon();
            }

            if (IsArchonActive)
            {
                if (ShouldArchonTeleport(out position))
                    return Teleport(position);

                if (ShouldArchonDisintegrationWave(out target))
                    return ArchonDisintegrationWave(target);

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

            return null;
        }

        public TrinityPower GetDestructiblePower()
        {
            var isTargetAccessible = CurrentTarget.IsInLineOfSight;

            if (isTargetAccessible && CurrentTarget.Distance < 20f)
            {
                if (IsArchonActive && Skills.Wizard.ArchonDisintegrationWave.CanCast())
                    return ArchonDisintegrationWave(CurrentTarget);

                if (!IsArchonActive && Skills.Wizard.ArcaneTorrent.CanCast())
                    return ArcaneTorrent(CurrentTarget);
            }

            return null;
        }
        public TrinityPower GetMovementPower(Vector3 destination)
        {

            if (IsInCombat && CurrentTarget.IsElite && destination.Distance(CurrentTarget.Position) < 50f)
            {
                return null;
            }

            if (IsArchonActive)
            {
                if (ShouldMovementArchonTeleport(destination))
                    return ArchoArchonTeleport(destination);
            }

            if (!IsArchonActive && Player.CurrentHealthPct > Settings.TeleportHealthEmergency)
            {
                if (IsInCombat && CurrentTarget.IsUnit)
                {
                    return null;
                }
                else if (ShouldMovementTeleport(destination))
                {
                    return Teleport(destination);
                }
            }

            if (IsStuck || IsBlocked)
            {
                    return Teleport(destination);
            }


            return Walk(destination);

        }




        /* Teleport skills below: Required for DPS and Survival
         */

        private bool ShouldMovementTeleport(Vector3 destination)
        {

            if (!Skills.Wizard.Teleport.CanCast())
                return false;

            if (IsStuck || IsBlocked)
                return true;

            if (!CanTeleportTo(destination))
                return false;

            return true;
        }

        private bool ShouldMovementArchonTeleport(Vector3 destination)
        {

            if (!Skills.Wizard.ArchonTeleport.CanCast())
                return false;

            if (destination == Vector3.Zero)
                return false;

            if (IsStuck || IsBlocked)
                return true;

            var destinationDistance = destination.Distance(Core.Player.Position);
            if (destinationDistance < 15f && !PlayerMover.IsBlocked)
                return false;

            // Don't move into molten core/arcane.
            if (!Core.Avoidance.InCriticalAvoidance(ZetaDia.Me.Position) && Core.Avoidance.Grid.IsIntersectedByFlags(ZetaDia.Me.Position, destination, AvoidanceFlags.CriticalAvoidance))
                return false;

            if (Skills.Wizard.Teleport.TimeSinceUse < 200)
                return false;

            return true;
        }

        protected override bool ShouldTeleport(out Vector3 position)
        {
            position = Vector3.Zero;
            var skill = Skills.Wizard.Teleport;
            var affixOnPlayer = Core.Avoidance.InAvoidance(ZetaDia.Me.Position);
            var healthIsLow = Player.CurrentHealthPct < Settings.TeleportHealthEmergency;
            var archonHealthIsLow = Player.CurrentHealthPct < Settings.ArchonTeleportHealthEmergency;
            var anyElitesinRange = TargetUtil.AnyElitesInRange(Settings.TeleportEliteKiteRange);
            var anyMobsInRange = TargetUtil.AnyMobsInRangeOfPosition(Player.Position, Settings.TeleportTrashKiteRange, Settings.TeleportTrashInRangeCount);

            if (!skill.CanCast())
                return false;

            if (Player.IsChannelling || !Player.IsChannelling)
            {
                if (anyElitesinRange || anyMobsInRange || healthIsLow || affixOnPlayer)
                    Core.Logger.Log(LogCategory.Routine, $"Close Elites: {anyElitesinRange}, Mobs: {anyMobsInRange}, Health: {healthIsLow}, Affix: {affixOnPlayer}");
                    return true;
            }

            Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance, ZetaDia.Me.Position, node => !HostileMonsters.Any(m => m.Position.Distance(node.NavigableCenter) < 15f));

            return position != Vector3.Zero;
        }

        protected override bool ShouldArchonTeleport(out Vector3 position)
        {
            position = Vector3.Zero;
            var skill = Skills.Wizard.ArchonTeleport;
            var affixOnPlayer = Core.Avoidance.InAvoidance(ZetaDia.Me.Position);
            var isShrine = Combat.Targeting.CurrentTarget.Type == TrinityObjectType.Shrine;
            var isProgressionGlobe = Combat.Targeting.CurrentTarget.Type == TrinityObjectType.ProgressionGlobe;
            var isHealthGlobe = Combat.Targeting.CurrentTarget.Type == TrinityObjectType.HealthGlobe;

            //Teleport Activations
            var archonHealthIsLow = IsArchonActive && Player.CurrentHealthPct < Settings.ArchonTeleportHealthEmergency;
            var anyElitesinRange = TargetUtil.AnyElitesInRange(Settings.TeleportEliteKiteRange);
            var anyMobsInRange = TargetUtil.AnyMobsInRangeOfPosition(Player.Position, Settings.TeleportTrashKiteRange, Settings.TeleportTrashInRangeCount);

            //Teleport Delays
            var archonHealthIsLowDelay = Skills.Wizard.Teleport.TimeSinceUse > Settings.ArchonTeleportHealthDelay;
            var archonTeleportDelay = (TalRashaStacks == 4 && Skills.Wizard.Teleport.TimeSinceUse > Settings.Archon4StackDelay) || (TalRashaStacks < 4 && Skills.Wizard.Teleport.TimeSinceUse > Settings.Archon1StackDelay);

            if (!skill.CanCast())
                return false;

            if (CurrentTarget.Distance < 50 && isShrine || isProgressionGlobe || (isHealthGlobe && archonHealthIsLow))
            {
                //Core.Logger.Log($"Teleporting to Priority Target");
                position = CurrentTarget.Position;
                return true;
            }

            if ((Player.IsChannelling || !Player.IsChannelling))
            {
                if (affixOnPlayer || (archonHealthIsLow && archonHealthIsLowDelay && anyMobsInRange))
                {
                    //Core.Logger.Log($"Teleport for Survival! Affix: {affixOnPlayer}, Health: {archonHealthIsLow}");
                    Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance, ZetaDia.Me.Position, node => !HostileMonsters.Any(m => m.Position.Distance(node.NavigableCenter) < 15f));
                    return true;
                }

                if(Skills.Wizard.Archon.TimeSinceUse > 19500)
                {
                    //Core.Logger.Log($"Teleport! Archon is about to drop!!!");
                    Avoider.TryGetSafeSpot(out position, Settings.TeleportKiteMinDistance, Settings.TeleportKiteMaxDistance, ZetaDia.Me.Position, node => !HostileMonsters.Any(m => m.Position.Distance(node.NavigableCenter) < 15f));
                    return true;
                }

                if (CurrentTarget.IsElite && anyElitesinRange && archonTeleportDelay)
                {
                    //Core.Logger.Log($"Teleport! Elite too close: {CurrentTarget.Distance} Setting: {Settings.TeleportEliteKiteRange}");
                    Avoider.TryGetSafeSpot(out position, 40, Settings.TeleportKiteMaxDistance, Combat.Targeting.CurrentTarget.Position, node => !HostileMonsters.Any(m => m.Position.Distance(node.NavigableCenter) < 15f));
                    return true;
                }

                var target = TargetUtil.BestRangedAoeUnit(10, 50, ClusterSize);
                if (target != null && target.Distance < 30f)
                {
                    //Core.Logger.Log($"Teleport! Trash Target too close: {CurrentTarget.Distance}");
                    Avoider.TryGetSafeSpot(out position, 40, Settings.TeleportKiteMaxDistance, target.Position, node => !HostileMonsters.Any(m => m.Position.Distance(node.NavigableCenter) < 15f));
                    return true;
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

            if (Player.PrimaryResource < Settings.ArcaneResourceReserve)
                return false;

            target = CurrentTarget;
            if (target.IsTreasureGoblin && target.Distance < Settings.ArcaneTorrentRange && target.IsInLineOfSight)
                return true;

            target = TargetUtil.GetClosestUnit(Settings.ArcaneTorrentRange);
            if (target != null && (Player.CurrentHealthPct < Settings.EmergencyHealthPct) || IsBlocked || IsStuck)
                return true;

            target = TargetUtil.BestAoeUnit(Settings.ArcaneTorrentRange, true);
            if (target != null && TalRashaStacks >= 2)
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
                Core.Logger.Log(LogCategory.Routine, $"Emergency Archon: Health at {Player.CurrentHealthPct}%! (Setting: {Settings.ShouldArchonHealthPct}%)");
                return true;
            }

            if (Player.PrimaryResource < Settings.ShouldArchonLowResource)
            {
                Core.Logger.Log(LogCategory.Routine, $"Emergency Archon: Arcane power at {Player.PrimaryResource}! (Setting: {Settings.ShouldArchonLowResource})");
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

            target = CurrentTarget;
            if (target.IsTreasureGoblin && target.Distance < Settings.DisintegrationWaveRange && target.IsInLineOfSight)
                return true;

            target = TargetUtil.BestRangedAoeUnit(Settings.DisintegrationWaveMinClusterRadius, Settings.DisintegrationWaveRange, ClusterSize);
            if (target != null && target.IsInLineOfSight)
                return true;

            return target != null;
        }

        public bool ShouldCancelArchon()
        {
            if (!IsArchonActive)
                return false;

            if (!HasInfiniteCasting)
                return false;

            if (TalRashaStacks == 4)
                return false;

            if (CurrentTarget == null)
                return false;

            return true;
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

        public override int ClusterSize => IsArchonActive ? Settings.ArchonClusterSize : Settings.NonArchonClusterSize;
        public override float EmergencyHealthPct => Settings.EmergencyHealthPct;


        IDynamicSetting IRoutine.RoutineSettings => Settings;
        public WizardVyRashaManaldArchonSettings Settings { get; } = new WizardVyRashaManaldArchonSettings();

        public sealed class WizardVyRashaManaldArchonSettings : NotifyBase, IDynamicSetting
        {
            //General
            private int _archonClusterSize;
            private int _nonArchonClusterSize;
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

            //Archon Activation and Skills
            private float _shouldArchonHealthPct;
            private int _shouldArchonLowResource;
            private float _disintegrationWaveRange;
            private float _disintegrationWaveMinClusterRadius;

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
            [DefaultValue(4)]
            public int ArchonClusterSize
            {
                get { return _archonClusterSize; }
                set { SetField(ref _archonClusterSize, value); }
            }

            [DefaultValue(1)]
            public int NonArchonClusterSize
            {
                get { return _nonArchonClusterSize; }
                set { SetField(ref _nonArchonClusterSize, value); }
            }

            [DefaultValue(0.4f)]
            public float EmergencyHealthPct
            {
                get { return _emergencyHealthPct; }
                set { SetField(ref _emergencyHealthPct, value); }
            }


            //Teleport: Where to look for Safe Spot
            [DefaultValue(40f)]
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

            [DefaultValue(50f)]
            public float DisintegrationWaveRange
            {
                get { return _disintegrationWaveRange; }
                set { SetField(ref _disintegrationWaveRange, value); }
            }

            [DefaultValue(10f)]
            public float DisintegrationWaveMinClusterRadius
            {
                get { return _disintegrationWaveMinClusterRadius; }
                set { SetField(ref _disintegrationWaveMinClusterRadius, value); }
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


