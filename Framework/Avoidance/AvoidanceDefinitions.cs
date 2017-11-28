using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using Trinity.Framework.Avoidance.Handlers;
using Trinity.Framework.Avoidance.Settings;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Objects;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Avoidance
{
    public class AvoidanceDefinitions : FieldCollection<AvoidanceDefinitions, AvoidanceDefinition>
    {
        public static AvoidanceDefinition DiseaseCloud = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.DiseaseCloud,
            Type = AvoidanceType.DiseaseCloud,
            Name = "Disease Cloud",
            Element = Element.Poison,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                // Generic Proxy is used for many different things but it's attributes may contain powers that identify it
                new AvoidancePart
                {
                    Name = "Disease Cloud Effect",
                    Filter = o => o.Attributes.GetAttribute<bool>(ActorAttributeType.PowerBuff0VisualEffectNone, (int) SNOPower.FastMummy_Disease_Cloud),
                    ActorSnoId = (int) SNOActor.Generic_Proxy,
                    Type = PartType.Main,
                    Radius = 13f
                }
            }
        };

        public static AvoidanceDefinition Urzael = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.Urzael,
            Type = AvoidanceType.Urzael,
            Name = "Urzael",
            Element = Element.Fire,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Falling wood",
                    ActorSnoId = (int) SNOActor.x1_westm_Falling_Wood_Urzael,
                    Type = PartType.Telegraph,
                    Radius = 14f
                },
                new AvoidancePart
                {
                    Name = "Falling wood 2",
                    ActorSnoId = (int) SNOActor.x1_westm_Falling_Wood_02_Urzael,
                    Type = PartType.Telegraph,
                    Radius = 14f
                },
                new AvoidancePart
                {
                    Name = "Cannonball",
                    ActorSnoId = (int) SNOActor.x1_Urzael_Cannonball,
                    Type = PartType.Projectile,
                    Radius = 14f
                },
                new AvoidancePart
                {
                    Name = "Cannonball Burning",
                    ActorSnoId = (int) SNOActor.x1_Urzael_Cannonball_burning,
                    Type = PartType.Projectile,
                    Radius = 14f
                },
                new AvoidancePart
                {
                    Name = "Flame Sweep",
                    ActorSnoId = (int) SNOActor.x1_Urzael_FlameSweep,
                    Type = PartType.Projectile,
                    Radius = 14f
                },
                new AvoidancePart
                {
                    Name = "Ceiling Debri Impact Emitter",
                    ActorSnoId = (int) SNOActor.x1_Urzael_ceilingDebris_beam_impact_emitter,
                    Type = PartType.Telegraph,
                    Radius = 14f
                },
                new AvoidancePart
                {
                    Name = "Bad Circle",
                    ActorSnoId = (int) SNOActor.x1_Urzael_ceilingDebris_beam_groundGlow,
                    Type = PartType.Telegraph,
                    Radius = 14f
                },

                new AvoidancePart
                {
                    Name = "Falling Debri Circle",
                    ActorSnoId = (int) SNOActor.x1_Urzael_ceilingDebris_Impact_Beam,
                    Type = PartType.Main,
                    Radius = 14f
                },
            }
        };

        public static AvoidanceDefinition ZoltunKulleTornado = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.ZoltunKulleTornado,
            Type = AvoidanceType.ZoltunKulleTornado,
            Name = "Zoltun Kulle Tornado",
            Element = Element.Physical,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Tornado",
                    ActorSnoId = (int)SNOActor.ZoltunKulle_EnergyTwister,
                    Type = PartType.Main,
                    Radius = 12f
                }
            }
        };

        public static AvoidanceDefinition OrlashBoss = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.OrlashBoss,
            Type = AvoidanceType.OrlashBoss,
            Name = "Orlash Boss",
            Element = Element.Lightning,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Breathe Lightning",
                    ActorSnoId = (int) SNOActor.x1_LR_boss_terrorDemon_A_projectile,
                    Type = PartType.Projectile,
                    Radius = 9f
                }
            }
        };

        public static AvoidanceDefinition Thunderstorm = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.Thunderstorm,
            Type = AvoidanceType.Thunderstorm,
            Name = "Thunderstorm",
            Element = Element.Lightning,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Pulse",
                    ActorSnoId = (int) SNOActor.x1_MonsterAffix_Thunderstorm_Impact,
                    Radius = 22f,
                    Delay = TimeSpan.FromSeconds(1),
                    Duration = TimeSpan.FromSeconds(4),
                    Type = PartType.Main
                }
            }
        };

        public static AvoidanceDefinition HeraldOfPestilence = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.HeraldOfPestilence,
            Type = AvoidanceType.HeraldOfPestilence,
            Name = "Herald Of Pestilence",
            Element = Element.Poison,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Hands",
                    ActorSnoId = (int) SNOActor.creepMobArm,
                    Radius = 15f,
                    Type = PartType.Main
                }
            }
        };

        // monsterAffix_Desecrator_damage_AOE
        // monsterAffix_Desecrator_telegraph

        public static AvoidanceDefinition Desecrator = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.Desecrator,
            Type = AvoidanceType.Desecrator,
            Name = "Desecrator",
            Element = Element.Fire,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Desecrator AOE",
                    ActorSnoId = (int) SNOActor.monsterAffix_Desecrator_damage_AOE,
                    Radius = 8f,
                    Type = PartType.Main
                }
            }
        };

        //ActorId: 159367, Type: ServerProp, Name: MorluSpellcaster_Meteor_afterBurn
        //ActorId: 159368, Type: ServerProp, Name: MorluSpellcaster_Meteor_Impact
        //ActorId: 159369, Type: ServerProp, Name: MorluSpellcaster_Meteor_Pending

        public static AvoidanceDefinition MorluMeteor = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.MorluMeteor,
            Type = AvoidanceType.MorluMeteor,
            Name = "Morlu Meteor",
            Element = Element.Fire,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Morlu Meteor Pending",
                    ActorSnoId = (int) SNOActor.MorluSpellcaster_Meteor_Pending,
                    Radius = 20f,
                    Type = PartType.Main
                }
            }
        };

        //[22017F4C] Type: ClientEffect Name: x1_sniperAngel_shardBolt_orb-3273 ActorSnoId: 333688, Distance: 20.05622
        //

        public static AvoidanceDefinition ExarchLightningStorm = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.ExarchLightningStorm,
            Type = AvoidanceType.ExarchLightningStorm,
            Name = "Exarch Lightning Storm",
            Element = Element.Lightning,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Lightning Storm",
                    ActorSnoId = (int) SNOActor.x1_sniperAngel_shardBolt_orb,
                    Radius = 22f,
                    Type = PartType.Main
                }
            }
        };

        // Raizeil GR Boss
        //[206427B0] Type: Monster Name: Generic_Proxy-34948 ActorSnoId: 4176, Distance: 0
        //[206725C4] Type: ClientEffect Name: � ActorSnoId: 255720, Distance: 6.180007
        //[2063E270] Type: Monster Name: Generic_Proxy-34955 ActorSnoId: 4176, Distance: 7.551051
        //[2064B684] Type: Monster Name: Generic_Proxy-34967 ActorSnoId: 4176, Distance: 7.551051
        //[20654558] Type: ServerProp Name: g_ChargedBolt_Projectile-34975 ActorSnoId: 4394, Distance: 14.51281
        // Generic proxy probably used for a lot of things, need to find unique id or check only in the presence of this actor.

        //AvoidanceData.Add(new AvoidanceData
        //{
        //    Name = "Raizeil Thunderstorm",
        //    IsEnabledByDefault = true,
        //    Element = Element.Poison,
        //    Handler = new CircularAvoidanceHandler(),
        //    Parts = new List<AvoidancePart>
        //    {
        //        new AvoidancePart
        //        {
        //            Name = "Thunderstorm",
        //            ActorSnoId = (int)SNOActor.Generic_Proxy,
        //            Radius = 20f,
        //            Type = PartType.Main,
        //        },
        //    }
        //});

        //[1FACC478] Type: ServerProp Name: Gluttony_gasCloud_proxy-73081 ActorSnoId: 93837, Distance: 42.44696

        public static AvoidanceDefinition GhomBoss = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.GhomBoss,
            Type = AvoidanceType.GhomBoss,
            Name = "Ghom Boss",
            Description = "Gas cloud scattered around by this rift boss",
            Element = Element.Poison,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Gas Cloud",
                    ActorSnoId = (int) SNOActor.Gluttony_gasCloud_proxy,
                    Radius = 20f,
                    Type = PartType.Main
                }
            }
        };

        /*
        InternalNameLowerCase => monsteraffix_molten_deathstart_proxy-395571
        Attributes => Attributes (4) Id=136/1543897224
         105: Invulnerable (-3991) i:1 f:0 Value=1
         103: TeamId (-3993) i:10 f:0 Value=10
         877: PowerBuff4VisualEffectNone (-3219) [ PowerSnoId: MonsterAffix_Molten: 90314 ] i:1 f:0 Value=1
         585: BuffVisualEffect (-3511) i:1 f:0 Value=1
         CollisionRadius => 1
        AxialRadius => 5.587935E-09
        ActorType => ServerProp
        ActorType:ServerProp ApperanceSNO:14322 PhysMeshSNO:-1 Cylinder:Position:<0.007729018, -0.006169997, -0.007890721>
        Ax1:5.587935E-09 Ax2:9 Sphere:Center:<0, 0, 0> Radius:9 AABB:Min:<0, 0, 0> Max:<0.1593657, 0.1593657, 3.53863E-17>
        AnimSetSNO:-1 SNOMonster:-1
        */

        public static AvoidanceDefinition MoltenCore = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.MoltenCore,
            Type = AvoidanceType.MoltenCore,
            Name = "Molten Core",
            Description = "Explosion on the ground after killing a 'molten' elite",
            Affix = MonsterAffixes.Molten,
            Element = Element.Fire,
            InfoUrl = "http://www.diablowiki.net/Molten",
            Handler = new CircularAvoidanceHandler(),
            Defaults = new AvoidanceSettingsEntry
            {
                DistanceMultiplier = 1,
                HealthPct = 100,
                Prioritize = true,
                IsEnabled = true,
            },
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Telegraph",
                    InternalName = "monsterAffix_Molten_deathStart_Proxy",
                    ActorSnoId = 4803,
                    Radius = 24f,
                    Duration = TimeSpan.FromSeconds(3),
                    Type = PartType.Telegraph
                },
                new AvoidancePart
                {
                    Name = "Explosion",
                    InternalName = "monsterAffix_Molten_deathExplosion_Proxy",
                    ActorSnoId = 4804,
                    Radius = 24f,
                    Delay = TimeSpan.FromSeconds(3),
                    Duration = TimeSpan.FromSeconds(1),
                    Type = PartType.Main
                },
                new AvoidancePart
                {
                    Name = "Explosion",
                    InternalName = "monsterAffix_molten_fireRing",
                    ActorSnoId = 224225,
                    Radius = 24f,
                    Delay = TimeSpan.FromSeconds(3),
                    Duration = TimeSpan.FromSeconds(1),
                    Type = PartType.Main
                }
            }
        };

        public static AvoidanceDefinition FireChains = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.FireChains,
            Type = AvoidanceType.FireChains,
            Name = "Fire Chains",
            Affix = MonsterAffixes.FireChains,
            Handler = new FireChainsAvoidanceHandler(),
            InfoUrl = "http://www.diablowiki.net/Fire_Chains",
            Element = Element.Fire,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Fire Chain Actor",
                    Affix = MonsterAffixes.FireChains,
                    Duration = TimeSpan.FromSeconds(3),
                    Type = PartType.ActorAffix
                }
            }
        };

        public static AvoidanceDefinition TethrysBoss = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.TethrysBoss,
            Type = AvoidanceType.TethrysBoss,
            Name = "Tethrys Boss",
            Handler = new CircularAvoidanceHandler(),
            Element = Element.Cold,
            InfoUrl = "http://www.diablowiki.net/Rift_Guardian",
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Satanic Symbol 1",
                    ActorSnoId = 315366,
                    InternalName = "x1_Adria_Geyser",
                    Radius = 12f,
                    Type = PartType.Main
                },
                new AvoidancePart
                {
                    Name = "Satanic Symbol 2",
                    ActorSnoId = 315362,
                    InternalName = "x1_Adria_Geyser_Pending",
                    Type = PartType.Main
                }
            }
        };

        public static AvoidanceDefinition Frozen = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.Frozen,
            Type = AvoidanceType.Frozen,
            Name = "Frozen",
            Affix = MonsterAffixes.Frozen,
            Handler = new CircularAvoidanceHandler(),
            InfoUrl = "http://www.diablowiki.net/Frozen",
            Element = Element.Cold,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Ice Cluster",
                    ActorSnoId = (int) SNOActor.monsterAffix_frozen_iceClusters, //223675,
                    Radius = 17f,
                    Duration = TimeSpan.FromSeconds(4),
                    Type = PartType.Main
                }
            }
        };

        public static AvoidanceDefinition Furnace = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.Furnace,
            Type = AvoidanceType.Furnace,
            Name = "Furnace",
            Handler = new FurnaceAvoidanceHandler(),
            Element = Element.Fire,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Forge Fire Breath 1",
                    ActorSnoId = (int) SNOActor.a3_Battlefield_demonic_forge, //174900,
                    Attribute = ActorAttributeType.PowerBuff0VisualEffectNone,
                    Power = SNOPower.a3_battlefield_demonic_forge,
                    Type = PartType.VisualEffect
                },
                new AvoidancePart
                {
                    Name = "Forge Fire Breath 2",
                    ActorSnoId = (int) SNOActor.a3_crater_st_demonic_forge, //185391,
                    Attribute = ActorAttributeType.PowerBuff0VisualEffectNone,
                    Power = SNOPower.a3_battlefield_demonic_forge,
                    Type = PartType.VisualEffect
                }
            }
        };

        public static AvoidanceDefinition ButcherBoss = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.ButcherBoss,
            Type = AvoidanceType.ButcherBoss,
            Name = "Butcher Boss",
            Description = "A Rift and Campaign Boss with a big cleaver. Fresh Meat!",
            Handler = new CircularAvoidanceHandler(),
            Element = Element.Fire,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Butcher Pentagram",
                    ActorSnoId = (int) SNOActor.X1_Unique_Monster_Generic_AOE_DOT_Fire_10foot, //359693,
                    Radius = 12f,
                    Type = PartType.Main
                },
                new AvoidancePart
                {
                    Name = "Butcher Pentagram Telegraph", // ClientEffect
                    ActorSnoId = (int) SNOActor.X1_Unique_Monster_Generic_AOE_Sphere_Distortion, //359693,
                    Radius = 12f,
                    Type = PartType.Telegraph
                }
            }
        };

        public static AvoidanceDefinition Grotesque = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.Grotesque,
            Type = AvoidanceType.Grotesque,
            Name = "Grotesque",
            Description = "Fat guys that explode into worms",
            Handler = new AnimationCircularAvoidanceHandler(),
            Element = Element.Physical,

            Defaults = new AvoidanceSettingsEntry
            {
                DistanceMultiplier = 1,
                HealthPct = 100,
                Prioritize = true,
                IsEnabled = true,
            },

            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Death Explosion",
                    Animation = SNOAnim.Stitch_Suicide_Bomb,
                    Animations = new List<SNOAnim>()
                    {
                        SNOAnim.Stitch_Suicide_Bomb,
                        SNOAnim.Stitch_Suicide_Bomb_Frost,
                        SNOAnim.Stitch_Suicide_Bomb_Imps,
                        SNOAnim.Stitch_Suicide_Bomb_spiders,
                    },
                    Type = PartType.ActorAnimation,
                    Radius = 26f
                }
            }
        };

        public static AvoidanceDefinition ChargeAttacks = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.ChargeAttacks,
            Type = AvoidanceType.ChargeAttacks,
            Name = "Charge Attacks",
            Description = "All types of charge based attacks by monsters",
            Handler = new AnimationBeamAvoidanceHandler(),
            Element = Element.Physical,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Assault Beast Charge",
                    Animation = SNOAnim.AssaultBeast_Land_attack_BullCharge_in,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Assault Beast Charge",
                    Animation = SNOAnim.AssaultBeast_Land_attack_BullCharge_middle,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Assault Beast Charge",
                    Animation = SNOAnim.AssaultBeast_Land_attack_BullCharge_out,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Beast Charge",
                    Animation = SNOAnim.Beast_start_charge_02,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Beast Charge",
                    Animation = SNOAnim.Beast_charge_04,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Beast Charge",
                    Animation = SNOAnim.Beast_charge_02,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Diablo Charge",
                    Animation = SNOAnim.Diablo_charge_attack,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Diablo Charge",
                    Animation = SNOAnim.Beast_start_charge_02,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Diablo Charge",
                    Animation = SNOAnim.Diablo_charge_attack,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Butcher Charge",
                    Animation = SNOAnim.Butcher_Attack_Charge_01_in,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Butcher Charge",
                    Animation = SNOAnim.Butcher_Attack_Charge_01_in_knockback,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Tentacle Horse",
                    Animation = SNOAnim.TentacleHorse_Charge_Loop_01,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Tentacle Horse",
                    Animation = SNOAnim.TentacleHorse_Charge_Loop_Outro_01,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Tentacle Horse",
                    Animation = SNOAnim.TentacleHorse_Charge_Intro_01,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Wraith",
                    Animation = SNOAnim.x1_Wraith_attack_05_charge_in,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Wraith",
                    Animation = SNOAnim.x1_Wraith_attack_05_charge_mid,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Wraith",
                    Animation = SNOAnim.x1_Wraith_attack_05_charge_out,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Floater Demon",
                    Animation = SNOAnim.FloaterDemon_attack_charge_01,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Floater Demon",
                    Animation = SNOAnim.FloaterDemon_attack_charge_in_01,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Floater Demon",
                    Animation = SNOAnim.FloaterDemon_attack_charge_in_01,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Big Red Charge",
                    Animation = SNOAnim.X1_BigRed_charge_01,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Big Red Charge",
                    Animation = SNOAnim.BigRed_charge_01,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Greed Boss Charge",
                    Animation = SNOAnim.p1_Greed_Attack_Charge_01_in,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Greed Boss Charge",
                    Animation = SNOAnim.p1_Greed_BreakFree_charge_01,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                }
            }
        };

        public static AvoidanceDefinition PerendiDefaultAttack = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.PerendiDefaultAttack,
            Type = AvoidanceType.PerendiDefaultAttack,
            Name = "Perendi & Mallet Lord Default Attack",
            Description = "Rift boss Perendi and Mallet Lord default attack",
            Handler = new AnimationBeamAvoidanceHandler(),
            Element = Element.Physical,
            Defaults = new AvoidanceSettingsEntry
            {
                DistanceMultiplier = 1,
                HealthPct = 100,
                Prioritize = true,
                IsEnabled = true,
            },
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Perendi/Mallet Lord Melee Attack",
                    Animation = SNOAnim.malletDemon_attack_01,
                    Type = PartType.ActorAnimation,
                    Radius = 30f
                },
            }
        };

        public static AvoidanceDefinition RatKing = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.RatKing,
            Type = AvoidanceType.RatKing,
            Name = "Rat King",
            Description = "Rift Boss who dispatches balls of rats",
            Handler = new CircularAvoidanceHandler(),
            Element = Element.Poison,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Rat Ball",
                    ActorSnoId = (int) SNOActor.p4_RatKing_RatBallMonster,
                    Radius = 10f,
                    Type = PartType.Main
                }
            }
        };

        public static AvoidanceDefinition Plagued = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.Plagued,
            Type = AvoidanceType.Plagued,
            Name = "Plagued",
            Description = "Swirly puddles of poisonous goo",
            Affix = MonsterAffixes.Plagued,
            Handler = new CircularAvoidanceHandler(),
            InfoUrl = "http://www.diablowiki.net/Plagued",
            Element = Element.Poison,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Swirly Poison Pool A",
                    ActorSnoId = 108869,
                    Radius = 12f,
                    Type = PartType.Main
                },
                new AvoidancePart
                {
                    Name = "Swirly Poison Pool B",
                    ActorSnoId = 223933,
                    Radius = 12f,
                    Type = PartType.Main
                }
            }
        };

        public static AvoidanceDefinition FrozenPulse = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.FrozenPulse,
            Type = AvoidanceType.FrozenPulse,
            Name = "Frozen Pulse",
            Description = "Spikey ice balls that chase you, then explode",
            InfoUrl = "http://www.diablowiki.net/Frozen_Pulse",
            Affix = MonsterAffixes.FrozenPulse,
            Element = Element.Cold,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Swirly Poison Pool A",
                    ActorSnoId = 349774,
                    Radius = 15f,
                    Type = PartType.Main
                }
            }
        };

        public static AvoidanceDefinition MoltenTrail = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.MoltenTrail,
            Type = AvoidanceType.MoltenTrail,
            Name = "Molten Trail",
            Description = "Fire trails behind molten elites",
            Affix = MonsterAffixes.Molten,
            InfoUrl = "http://www.diablowiki.net/Molten",
            Handler = new CircularAvoidanceHandler(),
            Element = Element.Fire,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Trail",
                    ActorSnoId = 95868,
                    Duration = TimeSpan.FromSeconds(1),
                    Type = PartType.Main
                }
            }
        };

        public static AvoidanceDefinition ArcaneSentry = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.ArcaneSentry,
            Type = AvoidanceType.ArcaneSentry,
            Name = "Arcane Sentry",
            Description = "Beams of hurt that spin around",
            Affix = MonsterAffixes.ArcaneEnchanted,
            Element = Element.Arcane,
            InfoUrl = "http://www.diablowiki.net/Arcane_Enchanted",
            Handler = new ArcaneAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Telegraph",
                    ActorSnoId = 257306,
                    Duration = TimeSpan.FromSeconds(2),
                    Type = PartType.Telegraph
                },
                new AvoidancePart
                {
                    Name = "Beam",
                    ActorSnoId = 219702,
                    //Delay = TimeSpan.FromSeconds(2),
                    Duration = TimeSpan.FromSeconds(10),
                    MovementType = MovementType.Rotation,
                    Type = PartType.Main
                },
                new AvoidancePart
                {
                    Name = "Beam Reverse Beam",
                    ActorSnoId = 221225,
                    //Delay = TimeSpan.FromSeconds(2),
                    Duration = TimeSpan.FromSeconds(10),
                    MovementType = MovementType.Rotation,
                    Type = PartType.Main
                }
            }
        };

        public static AvoidanceDefinition MaghdaBoss = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.MaghdaBoss,
            Type = AvoidanceType.MaghdaBoss,
            Name = "Maghda Boss",
            Description = "Handling for the insect projectile she shoots",
            Element = Element.Physical,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Projectile",
                    ActorSnoId = (int) SNOActor.UberMaghda_Punish_projectile, //278340,
                    InternalName = "UberMaghda_Punish_projectile",
                    Duration = TimeSpan.FromSeconds(10),
                    Type = PartType.Projectile,
                    MovementType = MovementType.Follow
                }
            }
        };

        public static AvoidanceDefinition Orbiter = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.Orbiter,
            Type = AvoidanceType.Orbiter,
            Name = "Orbiter",
            Description = "Balls of lighting that enlarge when u get close",
            InfoUrl = "http://www.diablowiki.net/Orbiter",
            Element = Element.Lightning,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Orb1",
                    Radius = 7f,
                    ActorSnoId = (int) SNOActor.X1_MonsterAffix_Orbiter_Projectile, //343539,
                    InternalName = "X1_MonsterAffix_Orbiter_Projectile",
                    Duration = TimeSpan.FromSeconds(10),
                    Type = PartType.Projectile
                },
                new AvoidancePart
                {
                    Name = "Orb2",
                    Radius = 7f,
                    InternalName = "x1_MonsterAffix_orbiter_projectile_orb",
                    ActorSnoId = (int) SNOActor.x1_MonsterAffix_orbiter_projectile_orb, //346805,
                    Duration = TimeSpan.FromSeconds(10),
                    Type = PartType.Projectile
                }
            }
        };

        public static AvoidanceDefinition MageFire = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.MageFire,
            Type = AvoidanceType.MageFire,
            Name = "Mage Fire",
            Description = "A single small puddle of fire",
            Handler = new CircularAvoidanceHandler(),
            Element = Element.Fire,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Projectile",
                    Radius = 6f,
                    ActorSnoId = (int) SNOActor.skeletonMage_Fire_projectile, //5374,
                    InternalName = "skeletonMage_Fire_projectile",
                    Type = PartType.Projectile
                },
                new AvoidancePart
                {
                    Name = "Ground Effect",
                    Radius = 8f,
                    ActorSnoId = (int) SNOActor.skeletonMage_fire_groundPool, //432,
                    InternalName = "skeletonMage_fire_groundPool",
                    Duration = TimeSpan.FromSeconds(3),
                    Type = PartType.Main
                }
            }
        };

        public static AvoidanceDefinition PoisonEnchanted = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.PoisonEnchanted,
            Type = AvoidanceType.PoisonEnchanted,
            Name = "Poison Enchanted",
            Description = "Poison affix that makes long vertical or horizontal trails from an explosion point.",
            InfoUrl = "http://www.diablowiki.net/Poison_Enchanted",
            Affix = MonsterAffixes.PoisonEnchanted,
            Handler = new PoisonEnchantedAvoidanceHandler(),
            Element = Element.Poison,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Slime Bomb Telegraph",
                    InternalName = "x1_MonsterAffix_CorpseBomber_bomb_start",
                    ActorSnoId = (int) SNOActor.x1_MonsterAffix_CorpseBomber_bomb_start, //340319,
                    Duration = TimeSpan.FromSeconds(2),
                    Type = PartType.Telegraph
                },
                new AvoidancePart
                {
                    Name = "Slime Bomb",
                    ActorSnoId = (int) SNOActor.X1_MonsterAffix_corpseBomber_bomb, //325761,
                    InternalName = "X1_MonsterAffix_corpseBomber_bomb",
                    Type = PartType.Main
                },
                new AvoidancePart
                {
                    Name = "Slime Bomb Projectile",
                    ActorSnoId = (int) SNOActor.x1_MonsterAffix_CorpseBomber_projectile, //316389,
                    InternalName = "x1_MonsterAffix_CorpseBomber_projectile",
                    Type = PartType.Projectile
                }
            }
        };

        public static AvoidanceDefinition Mortar = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.Mortar,
            Type = AvoidanceType.Mortar,
            Name = "Mortar",
            Affix = MonsterAffixes.Mortar,
            Handler = new CircularAvoidanceHandler(),
            Element = Element.Fire,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Mortar Impact",
                    InternalName = "x1_MonsterAffix_CorpseBomber_bomb_start",
                    ActorSnoId = (int) SNOActor.Grenadier_Proj_mortar_inpact,
                    Type = PartType.Main,
                    Radius = 8f
                }
            }
        };

        public static AvoidanceDefinition MalthaelDeathFog = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.MalthaelDeathFog,
            Group = "Malthael",
            Type = AvoidanceType.MalthaelDeathFog,
            Name = "Malthael DeathFog",
            Description = "Fog that hurts",
            Element = Element.Physical,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Death Fog Actor",
                    ActorSnoId = (int)SNOActor.x1_Malthael_DeathFogMonster,
                    Type = PartType.Main,
                    Radius = 18f
                },
            }
        };

        public static AvoidanceDefinition MalthaelHellGates = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.MalthaelHellGates,
            Group = "Malthael",
            Type = AvoidanceType.MalthaelHellGates,
            Name = "Malthael HellGates",
            Description = "Fire gates arrainged in outer ring",
            Element = Element.Fire,
            Handler = new CircularAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Fire Gate Actor",
                    ActorSnoId = (int)SNOActor.x1_malthael_gratesOfHell_darkBall_glowOuter,
                    Type = PartType.Main,
                    Radius = 20f
                },
            }
        };

        public static AvoidanceDefinition MalthealTeleport = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.MalthaelTeleport,
            Type = AvoidanceType.MalthaelTeleport,
            Group = "Malthael",
            Name = "Malthael Teleport",
            Description = "Maltheal teleports to you and owns you with his sickle",
            Element = Element.Physical,
            Handler = new AnimationBeamAvoidanceHandler(),
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Sickle Throw",
                    Animation = SNOAnim.x1_Malthael_sickle_throw,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Sickle Throw In",
                    Animation = SNOAnim.x1_Malthael_throw_sickle_intro,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
                new AvoidancePart
                {
                    Name = "Sickle Throw Out",
                    Animation = SNOAnim.x1_Malthael_throw_sickle_outtro,
                    Type = PartType.ActorAnimation,
                    Radius = 40f
                },
            }
        };

        public static AvoidanceDefinition CriticalProjectile = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.CriticalProjectile,
            Type = AvoidanceType.CriticalProjectile,
            Name = "Critical Projectile",
            Description = "A single small puddle of fire",
            Handler = new CircularAvoidanceHandler(),
            Element = Element.Physical,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Fire ball",
                    Radius = 6f,
                    ActorSnoId = 4103,
                    InternalName = "GoatMutant_Shaman_blast_projectile",
                    Type = PartType.Projectile
                },
                new AvoidancePart
                {
                    Name = "Blood star",
                    Radius = 6f,
                    ActorSnoId =  (int) SNOActor.succubus_bloodStar_projectile,
                    InternalName = "succubus_bloodStar_projectile",
                    Type = PartType.Projectile
                },
            }
        };

        public static AvoidanceDefinition Projectile = new AvoidanceDefinition
        {
            Id = (int)AvoidanceType.Projectile,
            Type = AvoidanceType.Projectile,
            Name = "Projectile",
            Description = "A single small puddle of fire",
            Handler = new CircularAvoidanceHandler(),
            Element = Element.Physical,
            Parts = new List<AvoidancePart>
            {
                new AvoidancePart
                {
                    Name = "Skeleton bow",
                    Radius = 6f,
                    ActorSnoId = 5347,
                    InternalName = "SkeletonArcher_B",
                    Type = PartType.Projectile
                },
                new AvoidancePart
                {
                    Name = "Bees wasps",
                    Radius = 6f,
                    ActorSnoId = 5212,
                    InternalName = "Bees_Wasps",
                    Type = PartType.Projectile
                },
            }
        };
    }
}