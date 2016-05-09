using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Avoidance.Handlers;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Objects;
using Trinity.Technicals;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Avoidance
{
    /// <summary>
    /// Contains the definition of all types of avoidance.
    /// </summary>
    public class AvoidanceDataFactory
    {
        internal static List<AvoidanceData> AvoidanceData = new List<AvoidanceData>();

        internal static ILookup<SNOAnim, AvoidancePart> LookupPartByAnimation { get; set; }

        public static readonly Dictionary<int, AvoidancePart> AvoidanceDataDictionary = new Dictionary<int, AvoidancePart>();

        public static bool TryCreateAvoidance(List<IActor> actors, IActor actor, out Structures.Avoidance avoidance)
        {
            avoidance = null;

            var data = GetAvoidanceData(actor);
            if (data == null)
                return false;

            avoidance = new Structures.Avoidance
            {
                Data = data,
                CreationTime = DateTime.UtcNow,
                StartPosition = actor.Position,
                Actors = new List<IActor> { actor },
                IsImmune = TrinityPlugin.Player.ElementImmunity.Contains(data.Element)
            };

            return true;
        }

        static AvoidanceDataFactory()
        {
            CreateData();
            CreateUtils();
        }

        private static void CreateData()
        {
            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Molten Core",
                IsEnabledByDefault = true,
                AffixGbId = (int) TrinityMonsterAffix.Molten,
                Element = Element.Fire,
                Handler = new CircularAvoidanceHandler(),
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Telegraph",
                        InternalName = "monsterAffix_Molten_deathStart_Proxy",
                        ActorSnoId = 4803,
                        Radius = 20f,
                        Duration = TimeSpan.FromSeconds(3),
                        Severity = Severity.Extreme,
                        Type = PartType.Telegraph
                    },
                    new AvoidancePart
                    {
                        Name = "Explosion",
                        InternalName = "monsterAffix_Molten_deathExplosion_Proxy",
                        ActorSnoId = 4804,
                        Radius = 20f,
                        Delay = TimeSpan.FromSeconds(3),
                        Duration = TimeSpan.FromSeconds(1),
                        Severity = Severity.Extreme,
                        Type = PartType.Main,
                    },
                    new AvoidancePart
                    {
                        Name = "Explosion",
                        InternalName = "monsterAffix_molten_fireRing",
                        ActorSnoId = 224225,
                        Radius = 20f,
                        Delay = TimeSpan.FromSeconds(3),
                        Duration = TimeSpan.FromSeconds(1),
                        Severity = Severity.Extreme,
                        Type = PartType.Main,
                    }
                }
            });

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Fire Chains",
                AffixGbId = (int) TrinityMonsterAffix.FireChains,
                Handler = new FireChainsAvoidanceHandler(),
                Element = Element.Fire,
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Fire Chain Actor",
                        Affix = TrinityMonsterAffix.FireChains,
                        Duration = TimeSpan.FromSeconds(3),
                        Type = PartType.ActorAffix,
                    },
                }
            });

            //Tethrys ActorId: 359688, Type: Monster, Name: X1_LR_Boss_Succubus_A-17034, 
            //X1_Unique_Monster_Generic_Projectile_Fire - 17346
            //ActorId: 315366, Type: ServerProp, Name: x1_Adria_Geyser - 17939, Distance2d: 6.132203, CollisionRadius: 0, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0
            //ActorId: 4176, Type: Monster, Name: Generic_Proxy - 17932, Distance2d: 4.620997, CollisionRadius: 0, MinimapActive: 0, 
            //ActorId: 315362, Type: ServerProp, Name: x1_Adria_Geyser_Pending - 17931, Distance2d: 48.03704, CollisionRadius: 0, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Rift Boss: Tethrys",
                AffixGbId = (int) TrinityMonsterAffix.Frozen,
                Handler = new CircularAvoidanceHandler(),
                Element = Element.Cold,
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Projectile",
                        ActorSnoId = 4176,
                        Radius = 5f,                        
                        Severity = Severity.Minor,
                        Type = PartType.Projectile,
                    },
                    new AvoidancePart
                    {
                        Name = "Satanic Symbol 1",
                        ActorSnoId = 315366,
                        InternalName = "x1_Adria_Geyser",
                        Radius = 12f,
                        Type = PartType.Main,
                    },
                    new AvoidancePart
                    {
                        Name = "Satanic Symbol 2",
                        ActorSnoId = 315362,
                        InternalName = "x1_Adria_Geyser_Pending",
                        Type = PartType.Main,
                    },
                }
            });


            //ActorId: 223675, Type: ServerProp, Name: monsterAffix_frozen_iceClusters - 4487, 

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Frozen",
                AffixGbId = (int) TrinityMonsterAffix.Frozen,
                Handler = new CircularAvoidanceHandler(),
                Element = Element.Cold,
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Ice Cluster",
                        ActorSnoId = (int) SNOActor.monsterAffix_frozen_iceClusters, //223675,
                        Radius = 16f,
                        Duration = TimeSpan.FromSeconds(4),
                        Type = PartType.Main,
                    },
                }
            });

            // Unit a3_Battlefield_demonic_forge-29378 (174900) has gained BuffVisualEffect (i:1 f:00.00000)
            // Unit a3_Battlefield_demonic_forge-29378 has a3_battlefield_demonic_forge(PowerBuff0VisualEffectNone)
            // ActorId: 185391, Type: Monster, Name: a3_crater_st_demonic_forge-875, 

            AvoidanceData.Add(new AvoidanceData
            {
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
                        Type = PartType.VisualEffect,
                    },
                    new AvoidancePart
                    {
                        Name = "Forge Fire Breath 2",
                        ActorSnoId = (int) SNOActor.a3_crater_st_demonic_forge, //185391,
                        Attribute = ActorAttributeType.PowerBuff0VisualEffectNone,
                        Power = SNOPower.a3_battlefield_demonic_forge,
                        Type = PartType.VisualEffect,
                    },
                }
            });

            //Line 8: [1F490598] Type: ServerProp Name: X1_Unique_Monster_Generic_AOE_DOT_Fire_10foot-211250 ActorSnoId: 359693, Distance: 26.54131
            //Line 259: [1F4A52DC] Type: ClientEffect Name: X1_Unique_Monster_Generic_AOE_Sphere_Distortion-211338 ActorSnoId: 358917, Distance: 2.384186E-07
            //Line 260: [1F472980] Type: ServerProp Name: X1_Unique_Monster_Generic_AOE_DOT_Fire_10foot-211337 ActorSnoId: 359693, Distance: 2.384186E-07
            //Line 271: [1F490598] Type: ServerProp Name: X1_Unique_Monster_Generic_AOE_DOT_Fire_10foot-211250 ActorSnoId: 359693, Distance: 29.45315

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Pentagram",
                Handler = new CircularAvoidanceHandler(),
                Element = Element.Fire,
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Butcher Pentagram",
                        ActorSnoId = (int)SNOActor.X1_Unique_Monster_Generic_AOE_DOT_Fire_10foot, //359693,    
                        Radius = 12f,
                        Type = PartType.Main,
                    },
                    new AvoidancePart
                    {
                        Name = "Butcher Pentagram Telegraph", // ClientEffect
                        ActorSnoId = (int)SNOActor.X1_Unique_Monster_Generic_AOE_Sphere_Distortion, //359693,    
                        Radius = 12f,
                        Type = PartType.Telegraph,
                    },
                }
            });

            //// Fat guys that explode into worms
            //// Stitch_Suicide_Bomb State=Transform By: Corpulent_C (3849)
            //new DoubleInt((int)SNOActor.Corpulent_A, (int)SNOAnim.Stitch_Suicide_Bomb),
            //new DoubleInt((int)SNOActor.Corpulent_A_Unique_01, (int)SNOAnim.Stitch_Suicide_Bomb),
            //new DoubleInt((int)SNOActor.Corpulent_A_Unique_02, (int)SNOAnim.Stitch_Suicide_Bomb),
            //new DoubleInt((int)SNOActor.Corpulent_A_Unique_03, (int)SNOAnim.Stitch_Suicide_Bomb),
            //new DoubleInt((int)SNOActor.Corpulent_B, (int)SNOAnim.Stitch_Suicide_Bomb),
            //new DoubleInt((int)SNOActor.Corpulent_B_Unique_01, (int)SNOAnim.Stitch_Suicide_Bomb),
            //new DoubleInt((int)SNOActor.Corpulent_C, (int)SNOAnim.Stitch_Suicide_Bomb),
            //new DoubleInt((int)SNOActor.Corpulent_D_CultistSurvivor_Unique, (int)SNOAnim.Stitch_Suicide_Bomb),
            //new DoubleInt((int)SNOActor.Corpulent_C_OasisAmbush_Unique, (int)SNOAnim.Stitch_Suicide_Bomb),
            //new DoubleInt((int)SNOActor.Corpulent_D_Unique_Spec_01, (int)SNOAnim.Stitch_Suicide_Bomb),

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Corpulent / Grotesque",
                Handler = new AnimationAvoidanceHandler(),
                Element = Element.Physical,
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Death Explosion",
                        Animation = SNOAnim.Stitch_Suicide_Bomb,
                        Type = PartType.ActorAnimation,      
                        Radius = 26f
                    },
                }
            });

            //[19147660] Type: ClientEffect Name: p4_ratKing_ratBall_model-47703 ActorSnoId: 427100, Distance: 19.72662
            //p4_RatKing_RatBallMonster, Type=Unit Dist=24.44967 IsBossOrEliteRareUnique=False IsAttackable=True

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Rat King",
                Handler = new CircularAvoidanceHandler(),
                Element = Element.Poison,
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Rat Ball",
                        ActorSnoId = (int)SNOActor.p4_RatKing_RatBallMonster,
                        Radius = 10f,
                        Type = PartType.Main,
                    },
                }
            });

            // monsterAffix_Plagued_endCloud = 108869,
            // Zombie_Plagued_C_Unique = 111321,
            // monsterAffix_plagued_groundGeo = 223933,
            // X1_Plagued_LacuniMale_A = 349601,
            // X1_Plagued_LacuniMale_Unique_A = 361088,
            // X1_Plagued_LacuniMale_Unique_B = 361099,
            // x1_Lacuni_male_plagued_swipeRight = 365732,
            // x1_Lacuni_male_plagued_swipeLeft = 365733,
            // x1_Lacuni_male_plagued_comboSwipe4 = 365745,
            // X1_Plagued_LacuniMale_Event_RatAlley = 367485,
            // x1_lacuniMale_plagued_summon_castRat = 374347,

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Plagued",
                AffixGbId = (int) TrinityMonsterAffix.Plagued,
                Handler = new CircularAvoidanceHandler(),
                Element = Element.Poison,
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Swirly Poison Pool A",
                        ActorSnoId = 108869,
                        Radius = 12f,
                        Type = PartType.Main,
                    },
                    new AvoidancePart
                    {
                        Name = "Swirly Poison Pool B",
                        ActorSnoId = 223933,
                        Radius = 12f,
                        Type = PartType.Main,
                    },
                }
            });

            //ActorId: 349774, Type: Monster, Name: x1_MonsterAffix_frozenPulse_monster-17780, Distance2d: 41.06812
            //Unit x1_MonsterAffix_frozenPulse_monster-5812 has X1_MonsterAffix_LightningStormCast(PowerBuff1VisualEffectNone)
            //Unit x1_MonsterAffix_frozenPulse_monster-5812 has X1_MonsterAffix_LightningStorm_Pulse(PowerBuff0VisualEffectNone)

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "FrozenPulse",
                AffixGbId = (int) TrinityMonsterAffix.FrozenPulse,
                Element = Element.Cold,
                Handler = new CircularAvoidanceHandler(),
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Swirly Poison Pool A",
                        ActorSnoId = 349774,
                        Radius = 12f,
                        Type = PartType.Main,
                    },
                }
            });

            //if (monster.MonsterAffixes.Contains(TrinityMonsterAffix.FireChains) && monster.MonsterQualityLevel == MonsterQuality.Minion)
            //    Telegraph.TelegraphFireChains(monster);

            //_avoidanceData.Add(new AvoidanceData
            //{
            //    Name = "Thunderstorm",
            //    AffixGbId = 0,
            //    HandlerProducer = () => new CircularAvoidanceHandler(),
            //    Parts = new List<AvoidancePart>
            //    {
            //        new AvoidancePart
            //        {
            //            Name = "Telegraph",
            //            ActorSnoId = 0,
            //            Duration = TimeSpan.FromSeconds(1),
            //            Type = PartType.Telegraph
            //        },
            //        new AvoidancePart
            //        {
            //            Name = "Pulse",
            //            ActorSnoId = 0,
            //            Radius = 20f,
            //            Delay = TimeSpan.FromSeconds(1),
            //            Duration = TimeSpan.FromSeconds(4),
            //            Type = PartType.Main,
            //        }
            //    }
            //});

            //ActorId: 95868, Type: ServerProp, Name: monsterAffix_Molten_trail-21985

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Molten Trail",
                AffixGbId = 95868,
                Handler = new CircularAvoidanceHandler(),
                Element = Element.Fire,
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Trail",
                        ActorSnoId = 95868,
                        Duration = TimeSpan.FromSeconds(1),
                        Type = PartType.Telegraph
                    },
                }
            });

            //ActorId: 257306, Type: ServerProp, Name: arcaneEnchantedDummy_spawn - 15126,
            //ActorId: 221225, Type: Monster, Name: MonsterAffix_ArcaneEnchanted_PetSweep_reverse - 15129,
            //ActorId: 219702, Type: Monster, Name: MonsterAffix_ArcaneEnchanted_PetSweep - 15166,

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Arcane Sentry",
                IsEnabledByDefault = true,
                AffixGbId = (int) TrinityMonsterAffix.ArcaneEnchanted,
                Element = Element.Arcane,
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
                        Type = PartType.Main,
                    },
                    new AvoidancePart
                    {
                        Name = "Beam Reverse Beam",
                        ActorSnoId = 221225,
                        //Delay = TimeSpan.FromSeconds(2),
                        Duration = TimeSpan.FromSeconds(10),
                        MovementType = MovementType.Rotation,
                        Type = PartType.Main,
                    }
                }
            });

            //Type: Projectile Name: UberMaghda_Punish_projectile-1409 ActorSnoId: 278340, Distance: 5.54583

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Maghda Projectile",
                AffixGbId = 0,
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
                        MovementType = MovementType.Follow,
                    },
                }
            });

            //Projectile Name: X1_MonsterAffix_Orbiter_Projectile - 3943 ActorSnoId: 343539, Distance: 2.998138
            //Type: ClientEffect Name: x1_MonsterAffix_orbiter_projectile_orb-3944 ActorSnoId: 346805, Distance: 5.775279
            //ClientEffect Name: x1_MonsterAffix_orbiter_projectile_orb - 3948 ActorSnoId: 346805, Distance: 14.95078
            //Unit x1_MonsterAffix_orbiter_projectile_orb-10831 has X1_MonsterAffix_OrbiterChampionCast(PowerBuff0VisualEffectNone)
            //Unit x1_MonsterAffix_orbiter_projectile_orb-10831 has None(ProjectileDetonateTime) Value=44339

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Orbiter",
                AffixGbId = 0,
                IsEnabledByDefault = true,
                Element = Element.Lightning,
                Handler = new CircularAvoidanceHandler(),
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Orb1",
                        Radius = 10f,
                        ActorSnoId = (int) SNOActor.X1_MonsterAffix_Orbiter_Projectile, //343539,
                        InternalName = "X1_MonsterAffix_Orbiter_Projectile",
                        Duration = TimeSpan.FromSeconds(10),
                        Type = PartType.Projectile,
                    },
                    new AvoidancePart
                    {
                        Name = "Orb2",
                        Radius = 6f,
                        InternalName = "x1_MonsterAffix_orbiter_projectile_orb",
                        ActorSnoId = (int) SNOActor.x1_MonsterAffix_orbiter_projectile_orb, //346805,
                        Duration = TimeSpan.FromSeconds(10),
                        Type = PartType.Projectile,
                    },
                }
            });

            //ActorId: 432, Type: ServerProp, Name: skeletonMage_fire_groundPool-1029, Distance2d: 0.240312, 
            //ActorId: 5374, Type: Projectile, Name: skeletonMage_Fire_projectile-1435, 

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Mage Fire",
                AffixGbId = 0,
                Handler = new CircularAvoidanceHandler(),
                Element = Element.Fire,
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Projectile",
                        Radius = 4f,
                        ActorSnoId = (int) SNOActor.skeletonMage_Fire_projectile, //5374,
                        InternalName = "skeletonMage_Fire_projectile",
                        Type = PartType.Projectile,
                    },
                    new AvoidancePart
                    {
                        Name = "Ground Effect",
                        Radius = 8f,
                        ActorSnoId = (int) SNOActor.skeletonMage_fire_groundPool, //432,
                        InternalName = "skeletonMage_fire_groundPool",
                        Duration = TimeSpan.FromSeconds(3),
                        Type = PartType.Main,
                    },
                }
            });

            //AvoidanceData.Add(new AvoidanceData
            //{
            //    Name = "Skeleton Archer Test",
            //    AffixGbId = 0,
            //    Handler = new CircularAvoidanceHandler(),
            //    Parts = new List<AvoidancePart>
            //    {
            //        new AvoidancePart
            //        {
            //            Name = "Actor",
            //            Radius = 14f,
            //            ActorSnoId = 5347,
            //            InternalName = "SkeletonArcher_B",
            //            Duration = TimeSpan.FromSeconds(2),
            //            Type = PartType.ActorAnimation,
            //        },
            //    }
            //});


            //Line 10390:     x1_MonsterAffix_CorpseBomber_projectile = 316389,
            //Line 10634:     X1_MonsterAffix_corpseBomber_bomb = 325761,
            //Line 11233:     x1_MonsterAffix_CorpseBomber_bomb_start = 340319,
            //Line 13788:     x1_MonsterAffix_Avenger_CorpseBomber_bomb_start = 384614,
            //Line 13789:     x1_MonsterAffix_Avenger_CorpseBomber_projectile = 384617,
            //Line 13844:     x1_MonsterAffix_Avenger_corpseBomber_slime = 389483,

            //Unit x1_MonsterAffix_CorpseBomber_projectile-15784 has X1_MonsterAffix_CorpseBomberCast(PowerBuff2VisualEffectNone)
            //Unit x1_MonsterAffix_CorpseBomber_projectile-15784 has X1_MonsterAffix_CorpseBomberCast(PowerBuff4VisualEffectNone)
            //Unit x1_MonsterAffix_CorpseBomber_projectile-12172 (316389) has gained ProjectileDetonateTime (i:46557 f:00.00000)
            //Unit x1_MonsterAffix_CorpseBomber_bomb_start-11590 (340319) has gained BuffVisualEffect (i:1 f:00.00000)
            //Unit x1_MonsterAffix_CorpseBomber_projectile-9797 (316389) has gained Hidden (i:1 f:00.00000)
            //Unit x1_MonsterAffix_CorpseBomber_projectile-9797 (316389) has gained BuffVisualEffect (i:1 f:00.00000)

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Poison Enchanted",
                AffixGbId = (int) TrinityMonsterAffix.PoisonEnchanted,
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
                    },
                }
            });

            //[TrinityPlugin 2.14.34] Unit Grenadier_Proj_mortar_inpact-17615 has None (ProjectileDetonateTime) Value=134387

            AvoidanceData.Add(new AvoidanceData
            {
                Name = "Mortar",
                AffixGbId = (int) TrinityMonsterAffix.Mortar,
                Handler = new CircularAvoidanceHandler(),
                Element = Element.Fire,
                Parts = new List<AvoidancePart>
                {
                    new AvoidancePart
                    {
                        Name = "Mortar Impact",
                        InternalName = "x1_MonsterAffix_CorpseBomber_bomb_start",
                        ActorSnoId = (int) SNOActor.Grenadier_Proj_mortar_inpact,
                        Type = PartType.Main
                    },
                }

            });
        }

        private static void CreateUtils()
        {
            var allParts = new List<AvoidancePart>();
                                
            foreach (var avoidanceDatum in AvoidanceData)
            {
                foreach (var part in avoidanceDatum.Parts)
                {
                    part.Parent = avoidanceDatum;
                    allParts.Add(part);
                    
                    try
                    {
                        if (part.ActorSnoId > 0)
                        {
                            AvoidanceDataDictionary.Add(part.ActorSnoId, part);
                        }
                    }
                    catch(Exception ex)                   
                    {
                        Logger.LogError("Failed to add AvoidanceData for {0} > {1}. Probably a duplicate ActorSnoId ({2})", avoidanceDatum.Name, part.Name, part.ActorSnoId);
                    }
                }
            }

            LookupPartByAnimation = allParts.Where(o => o.Animation != default(SNOAnim)).ToLookup(k => k.Animation, v => v);
        }

        public static AvoidanceData GetAvoidanceData(IActor actor)
        {
            if (actor == null)
                return null;

            AvoidanceData data;

            if (TryFindPartByActorId(actor, out data))
                return data;

            if (TryFindPartByAffix(actor, out data))
                return data;

            if (TryFindPartByAnimation(actor, out data))
                return data;

            return null;
        }

        private static bool TryFindPartByActorId(IActor actor, out AvoidanceData data)
        {
            data = null;

            if (actor == null || actor.ActorSNO <= 0)
                return false;

            var part = GetAvoidancePart(actor.ActorSNO);
            if (part != null)
            {
                {
                    data = part.Parent;
                    return true;
                }
            }
            return false;
        }

        private static bool TryFindPartByAffix(IActor actor, out AvoidanceData data)
        {
            data = null;

            if (actor?.MonsterAffixes == null || !actor.MonsterAffixes.Any())
                return false;

            foreach (var affix in actor.MonsterAffixes)
            {
                var part = GetAvoidancePart(affix);
                if (part != null)
                {
                    data = part.Parent;
                    return true;
                }
            }            
            return false;
        }

        private static bool TryFindPartByAnimation(IActor actor, out AvoidanceData data)
        {
            data = null;

            if (actor.CurrentAnimation == default(SNOAnim))
                return false;

            var part = GetAvoidancePart(actor.CurrentAnimation);
            if (part != null)
            {
                data = part.Parent;
                return true;
            }

            return false;
        }

        public static AvoidancePart GetAvoidancePart(int actorId)
        {
            return AvoidanceDataDictionary.ContainsKey(actorId) ? AvoidanceDataDictionary[actorId] : null;
        }

        public static AvoidancePart GetAvoidancePart(TrinityMonsterAffix affix)
        {
            return AvoidanceDataDictionary.Values.FirstOrDefault(a => a.Affix == affix);
        }

        public static AvoidancePart GetAvoidancePart(SNOAnim actorAnimation)
        {
            return LookupPartByAnimation.Contains(actorAnimation) ? LookupPartByAnimation[actorAnimation].FirstOrDefault() : null;
        }

    }
}


