using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Helpers;
using Trinity.Objects;
using Zeta.Game;

namespace Trinity.Reference
{
    public static class Runes
    {
        #region Imported Rune Data

            public class Monk : FieldCollection<Monk, Rune>
            {

                /// <summary>
                /// No Rune
                /// </summary>
                public static Rune None = new Rune
                {
                    Index = 0,
                    Name = "None",
                    Description = "No Rune Selected",
                    Tooltip = string.Empty,
                    TypeId = string.Empty,
                    RuneIndex = -1,
                    Class = ActorClass.Monk
                };

                #region Skill: Fists of Thunder

                /// <summary>
                /// Release an electric shockwave with every punch that hits all enemies within 6 yards of your primary enemy for 120% weapon damage as Lightning and causes knockback with every third hit.
                /// </summary>
                public static Rune Thunderclap = new Rune
                {
                    Index = 1,
                    Name = "Thunderclap",
                    Description = "Release an electric shockwave with every punch that hits all enemies within 6 yards of your primary enemy for 120% weapon damage as Lightning and causes knockback with every third hit.",
                    Tooltip = "rune/fists-of-thunder/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 0,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 6f,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Every third hit Freezes enemies for 2 seconds.Fists of Thunder&#39;s damage turns into Cold.
                /// </summary>
                public static Rune WindBlast = new Rune
                {
                    Index = 2,
                    Name = "Wind Blast",
                    Description = "Every third hit Freezes enemies for 2 seconds.Fists of Thunder&#39;s damage turns into Cold.",
                    Tooltip = "rune/fists-of-thunder/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Fists of Thunder applies Static Charge to enemies hit for 6 seconds. Each time an enemy with Static Charge gets hit by you, there is a chance that every other enemy with Static Charge within 40 yards takes 40% weapon damage as Lightning.
                /// </summary>
                public static Rune StaticCharge = new Rune
                {
                    Index = 3,
                    Name = "Static Charge",
                    Description = "Fists of Thunder applies Static Charge to enemies hit for 6 seconds. Each time an enemy with Static Charge gets hit by you, there is a chance that every other enemy with Static Charge within 40 yards takes 40% weapon damage as Lightning.",
                    Tooltip = "rune/fists-of-thunder/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 40f,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Increase Spirit generated to 20.Fists of Thunder&#39;s damage turns into Physical.
                /// </summary>
                public static Rune Quickening = new Rune
                {
                    Index = 4,
                    Name = "Quickening",
                    Description = "Increase Spirit generated to 20.Fists of Thunder&#39;s damage turns into Physical.",
                    Tooltip = "rune/fists-of-thunder/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 0,
                    ModifiedElement = Element.Physical,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Every third hit also releases arcs of holy power, dealing 240% weapon damage as Holy to up to 3 additional enemies.
                /// </summary>
                public static Rune BoundingLight = new Rune
                {
                    Index = 5,
                    Name = "Bounding Light",
                    Description = "Every third hit also releases arcs of holy power, dealing 240% weapon damage as Holy to up to 3 additional enemies.",
                    Tooltip = "rune/fists-of-thunder/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 0,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Lashing Tail Kick

                /// <summary>
                /// Release a torrent of fire that burns enemies within 10 yards for 755% weapon damage as Fire and an additional 230% weapon damage as Fire over 3 seconds.
                /// </summary>
                public static Rune VultureClawKick = new Rune
                {
                    Index = 1,
                    Name = "Vulture Claw Kick",
                    Description = "Release a torrent of fire that burns enemies within 10 yards for 755% weapon damage as Fire and an additional 230% weapon damage as Fire over 3 seconds.",
                    Tooltip = "rune/lashing-tail-kick/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Unleash a large roundhouse kick that deals 825% weapon damage as Physical to enemies within 15 yards.
                /// </summary>
                public static Rune SweepingArmada = new Rune
                {
                    Index = 2,
                    Name = "Sweeping Armada ",
                    Description = "Unleash a large roundhouse kick that deals 825% weapon damage as Physical to enemies within 15 yards.",
                    Tooltip = "rune/lashing-tail-kick/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 1,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 15f,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Hurl a column of fire that burns through enemies, causing 755% weapon damage as Fire to each enemy it strikes.
                /// </summary>
                public static Rune SpinningFlameKick = new Rune
                {
                    Index = 3,
                    Name = "Spinning Flame Kick",
                    Description = "Hurl a column of fire that burns through enemies, causing 755% weapon damage as Fire to each enemy it strikes.",
                    Tooltip = "rune/lashing-tail-kick/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 1,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Enemies hit are stunned for 2 seconds.Lashing Tail Kick&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune ScorpionSting = new Rune
                {
                    Index = 4,
                    Name = "Scorpion Sting",
                    Description = "Enemies hit are stunned for 2 seconds.Lashing Tail Kick&#39;s damage turns into Lightning.",
                    Tooltip = "rune/lashing-tail-kick/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Enemies are chilled at long range, Slowing them by 80% for 3 seconds.Lashing Tail Kick&#39;s damage turns into Cold.
                /// </summary>
                public static Rune HandOfYtar = new Rune
                {
                    Index = 5,
                    Name = "Hand of Ytar",
                    Description = "Enemies are chilled at long range, Slowing them by 80% for 3 seconds.Lashing Tail Kick&#39;s damage turns into Cold.",
                    Tooltip = "rune/lashing-tail-kick/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Deadly Reach

                /// <summary>
                /// Increases chance to knock enemies up into the air to 100% and the second and third hits gain increased area of effect.
                /// </summary>
                public static Rune PiercingTrident = new Rune
                {
                    Index = 1,
                    Name = "Piercing Trident",
                    Description = "Increases chance to knock enemies up into the air to 100% and the second and third hits gain increased area of effect.",
                    Tooltip = "rune/deadly-reach/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 2,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Increase damage to 260% weapon damage as Fire.
                /// </summary>
                public static Rune SearingGrasp = new Rune
                {
                    Index = 2,
                    Name = "Searing Grasp",
                    Description = "Increase damage to 260% weapon damage as Fire.",
                    Tooltip = "rune/deadly-reach/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 2,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Every third hit randomly damages enemies within 25 yards for 215% weapon damage as Lightning.
                /// </summary>
                public static Rune ScatteredBlows = new Rune
                {
                    Index = 3,
                    Name = "Scattered Blows",
                    Description = "Every third hit randomly damages enemies within 25 yards for 215% weapon damage as Lightning.",
                    Tooltip = "rune/deadly-reach/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 2,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 25f,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Each enemy hit with the third hit reduces the Spirit cost of your next Spirit Spender by 8%.Deadly Reach&#39;s damage turns into Cold.
                /// </summary>
                public static Rune StrikeFromBeyond = new Rune
                {
                    Index = 4,
                    Name = "Strike from Beyond",
                    Description = "Each enemy hit with the third hit reduces the Spirit cost of your next Spirit Spender by 8%.Deadly Reach&#39;s damage turns into Cold.",
                    Tooltip = "rune/deadly-reach/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 2,
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Every third hit also increases the damage of all your attacks by 15% for 5 seconds.
                /// </summary>
                public static Rune Foresight = new Rune
                {
                    Index = 5,
                    Name = "Foresight",
                    Description = "Every third hit also increases the damage of all your attacks by 15% for 5 seconds.",
                    Tooltip = "rune/deadly-reach/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 2,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Blinding Flash

                /// <summary>
                /// Increase the duration enemies are blinded to 6 seconds.
                /// </summary>
                public static Rune SelfReflection = new Rune
                {
                    Index = 1,
                    Name = "Self Reflection",
                    Description = "Increase the duration enemies are blinded to 6 seconds.",
                    Tooltip = "rune/blinding-flash/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Blinded enemies are also slowed by 80% for 5 seconds.
                /// </summary>
                public static Rune MystifyingLight = new Rune
                {
                    Index = 2,
                    Name = "Mystifying Light",
                    Description = "Blinded enemies are also slowed by 80% for 5 seconds.",
                    Tooltip = "rune/blinding-flash/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Each enemy you Blind restores 10 Spirit.
                /// </summary>
                public static Rune ReplenishingLight = new Rune
                {
                    Index = 3,
                    Name = "Replenishing Light",
                    Description = "Each enemy you Blind restores 10 Spirit.",
                    Tooltip = "rune/blinding-flash/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 3,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Enemies that are Blinded deal 25% reduced damage for 5 seconds after the Blind wears off.
                /// </summary>
                public static Rune CripplingLight = new Rune
                {
                    Index = 4,
                    Name = "Crippling Light",
                    Description = "Enemies that are Blinded deal 25% reduced damage for 5 seconds after the Blind wears off.",
                    Tooltip = "rune/blinding-flash/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// You deal 29% increased damage for 3 seconds after using Blinding Flash.
                /// </summary>
                public static Rune FaithInTheLight = new Rune
                {
                    Index = 5,
                    Name = "Faith in the Light",
                    Description = "You deal 29% increased damage for 3 seconds after using Blinding Flash.",
                    Tooltip = "rune/blinding-flash/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Tempest Rush

                /// <summary>
                /// Reduce the Spirit cost of Tempest Rush to 25 Spirit and increase its damage to 500% weapon damage as Holy.
                /// </summary>
                public static Rune NorthernBreeze = new Rune
                {
                    Index = 1,
                    Name = "Northern Breeze",
                    Description = "Reduce the Spirit cost of Tempest Rush to 25 Spirit and increase its damage to 500% weapon damage as Holy.",
                    Tooltip = "rune/tempest-rush/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 4,
                    ModifiedCost = 25,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Increases your movement speed while using Tempest Rush by 25%.
                /// </summary>
                public static Rune Tailwind = new Rune
                {
                    Index = 2,
                    Name = "Tailwind",
                    Description = "Increases your movement speed while using Tempest Rush by 25%.",
                    Tooltip = "rune/tempest-rush/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 4,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// After you stop channeling Tempest Rush, you cause an icy blast to all enemies within 15 yards. The damage of the explosion increases by 90% weapon damage as Cold while channeling.Tempest Rush&#39;s damage turns into Cold.
                /// </summary>
                public static Rune Flurry = new Rune
                {
                    Index = 3,
                    Name = "Flurry",
                    Description = "After you stop channeling Tempest Rush, you cause an icy blast to all enemies within 15 yards. The damage of the explosion increases by 90% weapon damage as Cold while channeling.Tempest Rush&#39;s damage turns into Cold.",
                    Tooltip = "rune/tempest-rush/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 4,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 15f,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Enemies within 20 yards take an additional 135% weapon damage as Lightning every second.Tempest Rush&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune ElectricField = new Rune
                {
                    Index = 4,
                    Name = "Electric Field",
                    Description = "Enemies within 20 yards take an additional 135% weapon damage as Lightning every second.Tempest Rush&#39;s damage turns into Lightning.",
                    Tooltip = "rune/tempest-rush/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 4,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 20f,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Enemies hit are knocked back and deal 20% reduced damage for 4 seconds.Tempest Rush&#39;s damage turns into Fire.
                /// </summary>
                public static Rune Bluster = new Rune
                {
                    Index = 5,
                    Name = "Bluster",
                    Description = "Enemies hit are knocked back and deal 20% reduced damage for 4 seconds.Tempest Rush&#39;s damage turns into Fire.",
                    Tooltip = "rune/tempest-rush/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 4,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Breath of Heaven

                /// <summary>
                /// Breath of Heaven also sears enemies for 505% weapon damage as Holy. 
                /// </summary>
                public static Rune CircleOfScorn = new Rune
                {
                    Index = 1,
                    Name = "Circle of Scorn",
                    Description = "Breath of Heaven also sears enemies for 505% weapon damage as Holy. ",
                    Tooltip = "rune/breath-of-heaven/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 5,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Increase the healing power of Breath of Heaven to 139469 - 182383 Life.Heal amount is increased by 30% of your Health Globe Healing Bonus.
                /// </summary>
                public static Rune CircleOfLife = new Rune
                {
                    Index = 2,
                    Name = "Circle of Life",
                    Description = "Increase the healing power of Breath of Heaven to 139469 - 182383 Life.Heal amount is increased by 30% of your Health Globe Healing Bonus.",
                    Tooltip = "rune/breath-of-heaven/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 5,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Breath of Heaven increases the damage of your attacks by 10% for 9 seconds.
                /// </summary>
                public static Rune BlazingWrath = new Rune
                {
                    Index = 3,
                    Name = "Blazing Wrath",
                    Description = "Breath of Heaven increases the damage of your attacks by 10% for 9 seconds.",
                    Tooltip = "rune/breath-of-heaven/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(9),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Gain 14 additional Spirit from Spirit generating attacks for 5 seconds after using Breath of Heaven.
                /// </summary>
                public static Rune InfusedWithLight = new Rune
                {
                    Index = 4,
                    Name = "Infused with Light",
                    Description = "Gain 14 additional Spirit from Spirit generating attacks for 5 seconds after using Breath of Heaven.",
                    Tooltip = "rune/breath-of-heaven/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Allies healed by Breath of Heaven have their movement speed increased by 30% for 3 seconds.
                /// </summary>
                public static Rune Zephyr = new Rune
                {
                    Index = 5,
                    Name = "Zephyr",
                    Description = "Allies healed by Breath of Heaven have their movement speed increased by 30% for 3 seconds.",
                    Tooltip = "rune/breath-of-heaven/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Dashing Strike

                /// <summary>
                /// Gain 20% increased movement speed for 4 seconds after using Dashing Strike.Dashing Strike&#39;s damage turns into Holy.
                /// </summary>
                public static Rune WayOfTheFallingStar = new Rune
                {
                    Index = 1,
                    Name = "Way of the Falling Star",
                    Description = "Gain 20% increased movement speed for 4 seconds after using Dashing Strike.Dashing Strike&#39;s damage turns into Holy.",
                    Tooltip = "rune/dashing-strike/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    ModifiedElement = Element.Holy,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Gain 40% increased chance to Dodge for 4 seconds after using Dashing Strike.Dashing Strike&#39;s damage turns into Cold.
                /// </summary>
                public static Rune BlindingSpeed = new Rune
                {
                    Index = 2,
                    Name = "Blinding Speed",
                    Description = "Gain 40% increased chance to Dodge for 4 seconds after using Dashing Strike.Dashing Strike&#39;s damage turns into Cold.",
                    Tooltip = "rune/dashing-strike/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Increases maximum charges to .Dashing Strike&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune Quicksilver = new Rune
                {
                    Index = 3,
                    Name = "Quicksilver",
                    Description = "Increases maximum charges to .Dashing Strike&#39;s damage turns into Lightning.",
                    Tooltip = "rune/dashing-strike/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 6,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Gain 15% increased attack speed for 4 seconds after using Dashing Strike.Dashing Strike&#39;s damage turns into Fire.
                /// </summary>
                public static Rune Radiance = new Rune
                {
                    Index = 4,
                    Name = "Radiance",
                    Description = "Gain 15% increased attack speed for 4 seconds after using Dashing Strike.Dashing Strike&#39;s damage turns into Fire.",
                    Tooltip = "rune/dashing-strike/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// The last enemy you dash through is obliterated with a barrage of strikes, taking an additional 975% weapon damage as Physical over 2 seconds.
                /// </summary>
                public static Rune Barrage = new Rune
                {
                    Index = 5,
                    Name = "Barrage",
                    Description = "The last enemy you dash through is obliterated with a barrage of strikes, taking an additional 975% weapon damage as Physical over 2 seconds.",
                    Tooltip = "rune/dashing-strike/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Crippling Wave

                /// <summary>
                /// Increase damage to 255% weapon damage as Fire.
                /// </summary>
                public static Rune Mangle = new Rune
                {
                    Index = 1,
                    Name = "Mangle",
                    Description = "Increase damage to 255% weapon damage as Fire.",
                    Tooltip = "rune/crippling-wave/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 7,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Enemies hit by Crippling Wave deal 20% less damage for 3 seconds.
                /// </summary>
                public static Rune Concussion = new Rune
                {
                    Index = 2,
                    Name = "Concussion",
                    Description = "Enemies hit by Crippling Wave deal 20% less damage for 3 seconds.",
                    Tooltip = "rune/crippling-wave/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Each enemy hit generates 2.5 additional Spirit.Crippling Wave&#39;s damage turns into Holy.
                /// </summary>
                public static Rune RisingTide = new Rune
                {
                    Index = 3,
                    Name = "Rising Tide",
                    Description = "Each enemy hit generates 2.5 additional Spirit.Crippling Wave&#39;s damage turns into Holy.",
                    Tooltip = "rune/crippling-wave/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 7,
                    ModifiedElement = Element.Holy,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Crippling Wave&#39;s third attack has its range increased to 17 yards and Freezes enemies for 1 second.Crippling Wave&#39;s damage turns into Cold.
                /// </summary>
                public static Rune Tsunami = new Rune
                {
                    Index = 4,
                    Name = "Tsunami",
                    Description = "Crippling Wave&#39;s third attack has its range increased to 17 yards and Freezes enemies for 1 second.Crippling Wave&#39;s damage turns into Cold.",
                    Tooltip = "rune/crippling-wave/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Enemies hit by Crippling Wave take 10% additional damage from all attacks for 3 seconds.
                /// </summary>
                public static Rune BreakingWave = new Rune
                {
                    Index = 5,
                    Name = "Breaking Wave",
                    Description = "Enemies hit by Crippling Wave take 10% additional damage from all attacks for 3 seconds.",
                    Tooltip = "rune/crippling-wave/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Wave of Light

                /// <summary>
                /// Wave of Light Stuns enemies for 1 second.Wave of Light&#39;s damage turns into Physical.
                /// </summary>
                public static Rune WallOfLight = new Rune
                {
                    Index = 1,
                    Name = "Wall of Light",
                    Description = "Wave of Light Stuns enemies for 1 second.Wave of Light&#39;s damage turns into Physical.",
                    Tooltip = "rune/wave-of-light/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 8,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Physical,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Release bursts of energy that deal 830% weapon damage as Fire to nearby enemies.
                /// </summary>
                public static Rune ExplosiveLight = new Rune
                {
                    Index = 2,
                    Name = "Explosive Light",
                    Description = "Release bursts of energy that deal 830% weapon damage as Fire to nearby enemies.",
                    Tooltip = "rune/wave-of-light/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 8,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Increases the damage of Wave of Light to 1045% weapon damage as Holy.
                /// </summary>
                public static Rune EmpoweredWave = new Rune
                {
                    Index = 3,
                    Name = "Empowered Wave",
                    Description = "Increases the damage of Wave of Light to 1045% weapon damage as Holy.",
                    Tooltip = "rune/wave-of-light/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 8,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Wave of Light deals an additional 820% weapon damage as Cold in a line.
                /// </summary>
                public static Rune ShatteringLight = new Rune
                {
                    Index = 4,
                    Name = "Shattering Light",
                    Description = "Wave of Light deals an additional 820% weapon damage as Cold in a line.",
                    Tooltip = "rune/wave-of-light/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 8,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Summon an ancient pillar that deals 635% weapon damage as Lightning, followed by 785% weapon damage as Lightning over 3 seconds to enemies who remain in the area.
                /// </summary>
                public static Rune PillarOfTheAncients = new Rune
                {
                    Index = 5,
                    Name = "Pillar of the Ancients",
                    Description = "Summon an ancient pillar that deals 635% weapon damage as Lightning, followed by 785% weapon damage as Lightning over 3 seconds to enemies who remain in the area.",
                    Tooltip = "rune/wave-of-light/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 8,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Exploding Palm

                /// <summary>
                /// Enemies hit take 15% additional damage for 9 seconds.
                /// </summary>
                public static Rune TheFleshIsWeak = new Rune
                {
                    Index = 1,
                    Name = "The Flesh is Weak",
                    Description = "Enemies hit take 15% additional damage for 9 seconds.",
                    Tooltip = "rune/exploding-palm/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 9,
                    ModifiedDuration = TimeSpan.FromSeconds(9),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// If the enemy explodes after bleeding, gain 15 Spirit for each enemy caught in the blast.Exploding Palm&#39;s damage turns into Holy.
                /// </summary>
                public static Rune StrongSpirit = new Rune
                {
                    Index = 2,
                    Name = "Strong Spirit",
                    Description = "If the enemy explodes after bleeding, gain 15 Spirit for each enemy caught in the blast.Exploding Palm&#39;s damage turns into Holy.",
                    Tooltip = "rune/exploding-palm/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 9,
                    ModifiedElement = Element.Holy,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Exploding Palm no longer causes the enemy to bleed, but if the enemy dies while affected by Exploding Palm, they explode for 6305% weapon damage as Cold.
                /// </summary>
                public static Rune ImpendingDoom = new Rune
                {
                    Index = 3,
                    Name = "Impending Doom",
                    Description = "Exploding Palm no longer causes the enemy to bleed, but if the enemy dies while affected by Exploding Palm, they explode for 6305% weapon damage as Cold.",
                    Tooltip = "rune/exploding-palm/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 9,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Exploding Palm arcs up to 15 yards to another target.Exploding Palm&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune ShockingGrasp = new Rune
                {
                    Index = 4,
                    Name = "Shocking Grasp",
                    Description = "Exploding Palm arcs up to 15 yards to another target.Exploding Palm&#39;s damage turns into Lightning.",
                    Tooltip = "rune/exploding-palm/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 9,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Instead of bleeding, the enemy will burn for 1875% weapon damage as Fire over 9 seconds. If the enemy dies while burning, it explodes causing all nearby enemies to burn for 3260% weapon damage as Fire over 3 seconds.
                /// </summary>
                public static Rune EssenceBurn = new Rune
                {
                    Index = 5,
                    Name = "Essence Burn",
                    Description = "Instead of bleeding, the enemy will burn for 1875% weapon damage as Fire over 9 seconds. If the enemy dies while burning, it explodes causing all nearby enemies to burn for 3260% weapon damage as Fire over 3 seconds.",
                    Tooltip = "rune/exploding-palm/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 9,
                    ModifiedDuration = TimeSpan.FromSeconds(9),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Cyclone Strike

                /// <summary>
                /// Reduce the Spirit cost of Cyclone Strike to 26 Spirit.Cyclone Strike&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune EyeOfTheStorm = new Rune
                {
                    Index = 1,
                    Name = "Eye of the Storm",
                    Description = "Reduce the Spirit cost of Cyclone Strike to 26 Spirit.Cyclone Strike&#39;s damage turns into Lightning.",
                    Tooltip = "rune/cyclone-strike/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 10,
                    ModifiedCost = 26,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Increase the distance enemies will be pulled towards you to 34 yards.
                /// </summary>
                public static Rune Implosion = new Rune
                {
                    Index = 2,
                    Name = "Implosion",
                    Description = "Increase the distance enemies will be pulled towards you to 34 yards.",
                    Tooltip = "rune/cyclone-strike/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 10,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Blast enemies with an explosion that deals 454% weapon damage as Fire.
                /// </summary>
                public static Rune Sunburst = new Rune
                {
                    Index = 3,
                    Name = "Sunburst",
                    Description = "Blast enemies with an explosion that deals 454% weapon damage as Fire.",
                    Tooltip = "rune/cyclone-strike/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 10,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Enemies are Frozen for 1.5 seconds after being pulled in.Cyclone Strike&#39;s damage turns into Cold.
                /// </summary>
                public static Rune WallOfWind = new Rune
                {
                    Index = 4,
                    Name = "Wall of Wind",
                    Description = "Enemies are Frozen for 1.5 seconds after being pulled in.Cyclone Strike&#39;s damage turns into Cold.",
                    Tooltip = "rune/cyclone-strike/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 10,
                    ModifiedDuration = TimeSpan.FromSeconds(1.5),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Cyclone Strike heals you and all allies within 24 yards for 31036 Life.Heal amount is increased by 17% of your Health Globe Healing Bonus.
                /// </summary>
                public static Rune SoothingBreeze = new Rune
                {
                    Index = 5,
                    Name = "Soothing Breeze",
                    Description = "Cyclone Strike heals you and all allies within 24 yards for 31036 Life.Heal amount is increased by 17% of your Health Globe Healing Bonus.",
                    Tooltip = "rune/cyclone-strike/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 10,
                    ModifiedAreaEffectRadius = 24f,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Way of the Hundred Fists

                /// <summary>
                /// Increase the number of hits in the second strike from 7 to 10 and increasing damage to 423% weapon damage as Lightning.
                /// </summary>
                public static Rune HandsOfLightning = new Rune
                {
                    Index = 1,
                    Name = "Hands of Lightning",
                    Description = "Increase the number of hits in the second strike from 7 to 10 and increasing damage to 423% weapon damage as Lightning.",
                    Tooltip = "rune/way-of-the-hundred-fists/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 11,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Critical Hits increase your attack speed and movement speed by 5% for 5 seconds. This effect can stack up to 3 times.Way of the Hundred Fists&#39;s damage turns into Fire.
                /// </summary>
                public static Rune BlazingFists = new Rune
                {
                    Index = 2,
                    Name = "Blazing Fists",
                    Description = "Critical Hits increase your attack speed and movement speed by 5% for 5 seconds. This effect can stack up to 3 times.Way of the Hundred Fists&#39;s damage turns into Fire.",
                    Tooltip = "rune/way-of-the-hundred-fists/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 11,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Perform a short dash with the first attack and enemies hit take an additional 60% weapon damage as Holy over 3 seconds. Fists of Fury damage can stack multiple times on the same enemy.
                /// </summary>
                public static Rune FistsOfFury = new Rune
                {
                    Index = 3,
                    Name = "Fists of Fury",
                    Description = "Perform a short dash with the first attack and enemies hit take an additional 60% weapon damage as Holy over 3 seconds. Fists of Fury damage can stack multiple times on the same enemy.",
                    Tooltip = "rune/way-of-the-hundred-fists/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 11,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Each enemy hit with the third hit increases your damage by 5% for 5 seconds.
                /// </summary>
                public static Rune Assimilation = new Rune
                {
                    Index = 4,
                    Name = "Assimilation",
                    Description = "Each enemy hit with the third hit increases your damage by 5% for 5 seconds.",
                    Tooltip = "rune/way-of-the-hundred-fists/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 11,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Every third hit also generates a wave of wind that deals 500% weapon damage as Cold to enemies directly ahead of you.Way of the Hundred Fists&#39;s damage turns into Cold.
                /// </summary>
                public static Rune WindforceFlurry = new Rune
                {
                    Index = 5,
                    Name = "Windforce Flurry",
                    Description = "Every third hit also generates a wave of wind that deals 500% weapon damage as Cold to enemies directly ahead of you.Way of the Hundred Fists&#39;s damage turns into Cold.",
                    Tooltip = "rune/way-of-the-hundred-fists/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 11,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Serenity

                /// <summary>
                /// When activated, Serenity heals you for 93874 - 120695 Life.Heal amount is increased by 40% of your Health Globe Healing Bonus.
                /// </summary>
                public static Rune PeacefulRepose = new Rune
                {
                    Index = 1,
                    Name = "Peaceful Repose",
                    Description = "When activated, Serenity heals you for 93874 - 120695 Life.Heal amount is increased by 40% of your Health Globe Healing Bonus.",
                    Tooltip = "rune/serenity/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 12,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// While under the effects of Serenity, enemies within 20 yards take 438% weapon damage as Physical every second.
                /// </summary>
                public static Rune UnwelcomeDisturbance = new Rune
                {
                    Index = 2,
                    Name = "Unwelcome Disturbance",
                    Description = "While under the effects of Serenity, enemies within 20 yards take 438% weapon damage as Physical every second.",
                    Tooltip = "rune/serenity/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 12,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 20f,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Protect allies within 45 yards with a shield that removes control impairing effects and redirects up to 120158 damage to you for 3 seconds.Shield amount is increased by 40% of your Health Globe Healing Bonus.
                /// </summary>
                public static Rune Tranquility = new Rune
                {
                    Index = 3,
                    Name = "Tranquility",
                    Description = "Protect allies within 45 yards with a shield that removes control impairing effects and redirects up to 120158 damage to you for 3 seconds.Shield amount is increased by 40% of your Health Globe Healing Bonus.",
                    Tooltip = "rune/serenity/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedAreaEffectRadius = 45f,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Increase the duration of Serenity to 4 seconds. 
                /// </summary>
                public static Rune Ascension = new Rune
                {
                    Index = 4,
                    Name = "Ascension",
                    Description = "Increase the duration of Serenity to 4 seconds. ",
                    Tooltip = "rune/serenity/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// While under the effects of Serenity, your movement is unhindered.
                /// </summary>
                public static Rune InstantKarma = new Rune
                {
                    Index = 5,
                    Name = "Instant Karma",
                    Description = "While under the effects of Serenity, your movement is unhindered.",
                    Tooltip = "rune/serenity/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 12,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Seven-Sided Strike

                /// <summary>
                /// Teleport to the enemy and increase damage dealt to 8285% weapon damage as Lightning over 7 strikes.
                /// </summary>
                public static Rune SuddenAssault = new Rune
                {
                    Index = 1,
                    Name = "Sudden Assault",
                    Description = "Teleport to the enemy and increase damage dealt to 8285% weapon damage as Lightning over 7 strikes.",
                    Tooltip = "rune/sevensided-strike/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 13,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Seven-Sided Strike causes enemies to burn for 630% weapon damage as Fire over 3 seconds.
                /// </summary>
                public static Rune Incinerate = new Rune
                {
                    Index = 2,
                    Name = "Incinerate",
                    Description = "Seven-Sided Strike causes enemies to burn for 630% weapon damage as Fire over 3 seconds.",
                    Tooltip = "rune/sevensided-strike/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Removes the Spirit cost and enemies hit by Seven-Sided Strike are Frozen for 7 seconds.
                /// </summary>
                public static Rune Pandemonium = new Rune
                {
                    Index = 3,
                    Name = "Pandemonium",
                    Description = "Removes the Spirit cost and enemies hit by Seven-Sided Strike are Frozen for 7 seconds.",
                    Tooltip = "rune/sevensided-strike/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(7),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Reduce the cooldown to 14 seconds.
                /// </summary>
                public static Rune SustainedAttack = new Rune
                {
                    Index = 4,
                    Name = "Sustained Attack",
                    Description = "Reduce the cooldown to 14 seconds.",
                    Tooltip = "rune/sevensided-strike/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(14),
                    ModifiedCooldown = TimeSpan.FromSeconds(14),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Each strike explodes, dealing 877% weapon damage as Holy in a 7 yard radius around the enemy.
                /// </summary>
                public static Rune FulminatingOnslaught = new Rune
                {
                    Index = 5,
                    Name = "Fulminating Onslaught",
                    Description = "Each strike explodes, dealing 877% weapon damage as Holy in a 7 yard radius around the enemy.",
                    Tooltip = "rune/sevensided-strike/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 13,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Mantra of Salvation

                /// <summary>
                /// Passive: Mantra of Salvation also increases Armor by 20%.
                /// </summary>
                public static Rune HardTarget = new Rune
                {
                    Index = 1,
                    Name = "Hard Target",
                    Description = "Passive: Mantra of Salvation also increases Armor by 20%.",
                    Tooltip = "rune/mantra-of-salvation/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 14,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Mantra of Salvation also protects you and your allies when reduced below 25% Life, granting a shield that reduces damage taken by 80% for 3 seconds.Each target may be protected by this effect once every 90 seconds.
                /// </summary>
                public static Rune DivineProtection = new Rune
                {
                    Index = 2,
                    Name = "Divine Protection",
                    Description = "Passive: Mantra of Salvation also protects you and your allies when reduced below 25% Life, granting a shield that reduces damage taken by 80% for 3 seconds.Each target may be protected by this effect once every 90 seconds.",
                    Tooltip = "rune/mantra-of-salvation/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 14,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Mantra of Salvation also increases movement speed by 10%.
                /// </summary>
                public static Rune WindThroughTheReeds = new Rune
                {
                    Index = 3,
                    Name = "Wind through the Reeds",
                    Description = "Passive: Mantra of Salvation also increases movement speed by 10%.",
                    Tooltip = "rune/mantra-of-salvation/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 14,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Increases the resistance to all elements bonus to 40%.
                /// </summary>
                public static Rune Perseverance = new Rune
                {
                    Index = 4,
                    Name = "Perseverance",
                    Description = "Passive: Increases the resistance to all elements bonus to 40%.",
                    Tooltip = "rune/mantra-of-salvation/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 14,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Mantra of Salvation also increases Dodge Chance by 35%.
                /// </summary>
                public static Rune Agility = new Rune
                {
                    Index = 5,
                    Name = "Agility",
                    Description = "Passive: Mantra of Salvation also increases Dodge Chance by 35%.",
                    Tooltip = "rune/mantra-of-salvation/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 14,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Sweeping Wind

                /// <summary>
                /// While your vortex is at 3 or more stacks, enemies damaged by Sweeping Wind for 3 consecutive seconds are Frozen for 2 seconds.Enemies cannot be frozen by Sweeping Wind more than once every 3 seconds.Sweeping Wind&#39;s damage turns into Cold.
                /// </summary>
                public static Rune MasterOfWind = new Rune
                {
                    Index = 1,
                    Name = "Master of Wind",
                    Description = "While your vortex is at 3 or more stacks, enemies damaged by Sweeping Wind for 3 consecutive seconds are Frozen for 2 seconds.Enemies cannot be frozen by Sweeping Wind more than once every 3 seconds.Sweeping Wind&#39;s damage turns into Cold.",
                    Tooltip = "rune/sweeping-wind/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Intensify the vortex, increasing the damage per stack to 145% weapon damage. This increases the damage with 3 stacks to 435% weapon damage.
                /// </summary>
                public static Rune BladeStorm = new Rune
                {
                    Index = 2,
                    Name = "Blade Storm",
                    Description = "Intensify the vortex, increasing the damage per stack to 145% weapon damage. This increases the damage with 3 stacks to 435% weapon damage.",
                    Tooltip = "rune/sweeping-wind/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 15,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Increase the radius of the vortex to 14 yards.Sweeping Wind&#39;s damage turns into Fire.
                /// </summary>
                public static Rune FireStorm = new Rune
                {
                    Index = 3,
                    Name = "Fire Storm",
                    Description = "Increase the radius of the vortex to 14 yards.Sweeping Wind&#39;s damage turns into Fire.",
                    Tooltip = "rune/sweeping-wind/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 15,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// As long as your vortex is at 3 or more stacks, you gain 8 Spirit per second.Sweeping Wind&#39;s damage turns into Holy.
                /// </summary>
                public static Rune InnerStorm = new Rune
                {
                    Index = 4,
                    Name = "Inner Storm",
                    Description = "As long as your vortex is at 3 or more stacks, you gain 8 Spirit per second.Sweeping Wind&#39;s damage turns into Holy.",
                    Tooltip = "rune/sweeping-wind/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 15,
                    ModifiedElement = Element.Holy,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// While your vortex is at 3 or more stacks, Critical Hits have a chance to spawn a lightning tornado that periodically electrocutes nearby enemies for 95% weapon damage as Lightning. Each spawned lightning tornado lasts 3 seconds.Sweeping Wind&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune Cyclone = new Rune
                {
                    Index = 5,
                    Name = "Cyclone",
                    Description = "While your vortex is at 3 or more stacks, Critical Hits have a chance to spawn a lightning tornado that periodically electrocutes nearby enemies for 95% weapon damage as Lightning. Each spawned lightning tornado lasts 3 seconds.Sweeping Wind&#39;s damage turns into Lightning.",
                    Tooltip = "rune/sweeping-wind/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Mantra of Retribution

                /// <summary>
                /// Passive: Increase the amount of damage inflicted by Mantra of Retribution to 202% weapon damage as Fire.
                /// </summary>
                public static Rune Retaliation = new Rune
                {
                    Index = 1,
                    Name = "Retaliation",
                    Description = "Passive: Increase the amount of damage inflicted by Mantra of Retribution to 202% weapon damage as Fire.",
                    Tooltip = "rune/mantra-of-retribution/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 16,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Mantra of Retribution also increases attack speed by 10% for you and your allies.
                /// </summary>
                public static Rune Transgression = new Rune
                {
                    Index = 2,
                    Name = "Transgression",
                    Description = "Passive: Mantra of Retribution also increases attack speed by 10% for you and your allies.",
                    Tooltip = "rune/mantra-of-retribution/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 16,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Enemies damaged by Mantra of Retribution have a 20% chance to be stunned for 3 seconds.
                /// </summary>
                public static Rune Indignation = new Rune
                {
                    Index = 3,
                    Name = "Indignation",
                    Description = "Passive: Enemies damaged by Mantra of Retribution have a 20% chance to be stunned for 3 seconds.",
                    Tooltip = "rune/mantra-of-retribution/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 16,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Mantra of Retribution has a chance to restore 3 Spirit when dealing damage.
                /// </summary>
                public static Rune AgainstAllOdds = new Rune
                {
                    Index = 4,
                    Name = "Against All Odds",
                    Description = "Passive: Mantra of Retribution has a chance to restore 3 Spirit when dealing damage.",
                    Tooltip = "rune/mantra-of-retribution/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 16,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Enemies damaged by Mantra of Retribution have a 75% chance to suffer a feedback blast, dealing 101% weapon damage as Holy to itself and nearby enemies.
                /// </summary>
                public static Rune CollateralDamage = new Rune
                {
                    Index = 5,
                    Name = "Collateral Damage",
                    Description = "Passive: Enemies damaged by Mantra of Retribution have a 75% chance to suffer a feedback blast, dealing 101% weapon damage as Holy to itself and nearby enemies.",
                    Tooltip = "rune/mantra-of-retribution/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 16,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Inner Sanctuary

                /// <summary>
                /// Inner Sanctuary duration is increased to 8 seconds and cannot be passed by enemies.
                /// </summary>
                public static Rune SanctifiedGround = new Rune
                {
                    Index = 1,
                    Name = "Sanctified Ground",
                    Description = "Inner Sanctuary duration is increased to 8 seconds and cannot be passed by enemies.",
                    Tooltip = "rune/inner-sanctuary/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Allies inside Inner Sanctuary are healed for 35779 every second.Heal amount is increased by 7% of your Life per Second.
                /// </summary>
                public static Rune SafeHaven = new Rune
                {
                    Index = 2,
                    Name = "Safe Haven",
                    Description = "Allies inside Inner Sanctuary are healed for 35779 every second.Heal amount is increased by 7% of your Life per Second.",
                    Tooltip = "rune/inner-sanctuary/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 17,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Allies inside Inner Sanctuary are also immune to control impairing effects.
                /// </summary>
                public static Rune TempleOfProtection = new Rune
                {
                    Index = 3,
                    Name = "Temple of Protection",
                    Description = "Allies inside Inner Sanctuary are also immune to control impairing effects.",
                    Tooltip = "rune/inner-sanctuary/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 17,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Dash to the target location, granting a shield that absorbs up to 107284 damage for 3 seconds to allies within 11 yards and then creating Inner Sanctuary.Absorb amount is increased by 28% of your Health Globe Healing Bonus.
                /// </summary>
                public static Rune Intervene = new Rune
                {
                    Index = 4,
                    Name = "Intervene",
                    Description = "Dash to the target location, granting a shield that absorbs up to 107284 damage for 3 seconds to allies within 11 yards and then creating Inner Sanctuary.Absorb amount is increased by 28% of your Health Globe Healing Bonus.",
                    Tooltip = "rune/inner-sanctuary/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedAreaEffectRadius = 11f,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Enemies inside Inner Sanctuary have their movement speed reduced by 80%.
                /// </summary>
                public static Rune ForbiddenPalace = new Rune
                {
                    Index = 5,
                    Name = "Forbidden Palace",
                    Description = "Enemies inside Inner Sanctuary have their movement speed reduced by 80%.",
                    Tooltip = "rune/inner-sanctuary/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 17,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Mystic Ally

                /// <summary>
                /// Active: Your mystic ally performs 7 wave attacks in quick succession, each dealing 625% weapon damage as Cold and Freezing enemies for 3 seconds.Passive: A mystic ally fights by your side that infuses your attacks to Slow enemies by 60% for 3 seconds.
                /// </summary>
                public static Rune WaterAlly = new Rune
                {
                    Index = 1,
                    Name = "Water Ally",
                    Description = "Active: Your mystic ally performs 7 wave attacks in quick succession, each dealing 625% weapon damage as Cold and Freezing enemies for 3 seconds.Passive: A mystic ally fights by your side that infuses your attacks to Slow enemies by 60% for 3 seconds.",
                    Tooltip = "rune/mystic-ally/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 18,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Active: Your mystic ally splits into 5 allies that explode for 480% weapon damage as Fire.Passive: A mystic ally fights by your side that increases your damage by 10%.
                /// </summary>
                public static Rune FireAlly = new Rune
                {
                    Index = 2,
                    Name = "Fire Ally",
                    Description = "Active: Your mystic ally splits into 5 allies that explode for 480% weapon damage as Fire.Passive: A mystic ally fights by your side that increases your damage by 10%.",
                    Tooltip = "rune/mystic-ally/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 18,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Active: You gain 100 Spirit.Passive: A mystic ally fights by your side that increases your Spirit Regeneration by 4.
                /// </summary>
                public static Rune AirAlly = new Rune
                {
                    Index = 3,
                    Name = "Air Ally",
                    Description = "Active: You gain 100 Spirit.Passive: A mystic ally fights by your side that increases your Spirit Regeneration by 4.",
                    Tooltip = "rune/mystic-ally/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 18,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Active: Your mystic ally sacrifices itself to heal you for 100% of your maximum Life. The cooldown on Mystic Ally is increased to 50 seconds.Passive: A mystic ally fights by your side that increases your Life per Second by 10728. The heal amount is increased by 7% of your Life per Second.
                /// </summary>
                public static Rune EnduringAlly = new Rune
                {
                    Index = 4,
                    Name = "Enduring Ally",
                    Description = "Active: Your mystic ally sacrifices itself to heal you for 100% of your maximum Life. The cooldown on Mystic Ally is increased to 50 seconds.Passive: A mystic ally fights by your side that increases your Life per Second by 10728. The heal amount is increased by 7% of your Life per Second.",
                    Tooltip = "rune/mystic-ally/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 18,
                    ModifiedDuration = TimeSpan.FromSeconds(50),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Active: Your mystic ally turns into a boulder for 8 seconds. The boulder deals 380% weapon damage as Physical every second and rolls toward nearby enemies, knocking them up.Passive: A mystic ally fights by your side that increases your Life by 20%.
                /// </summary>
                public static Rune EarthAlly = new Rune
                {
                    Index = 5,
                    Name = "Earth Ally",
                    Description = "Active: Your mystic ally turns into a boulder for 8 seconds. The boulder deals 380% weapon damage as Physical every second and rolls toward nearby enemies, knocking them up.Passive: A mystic ally fights by your side that increases your Life by 20%.",
                    Tooltip = "rune/mystic-ally/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 18,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Mantra of Healing

                /// <summary>
                /// Passive: Increase the Life regeneration granted by Mantra of Healing to 21457 Life per Second.Heal amount is increased by 7% of your Life per Second.
                /// </summary>
                public static Rune Sustenance = new Rune
                {
                    Index = 1,
                    Name = "Sustenance",
                    Description = "Passive: Increase the Life regeneration granted by Mantra of Healing to 21457 Life per Second.Heal amount is increased by 7% of your Life per Second.",
                    Tooltip = "rune/mantra-of-healing/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 19,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Mantra of Healing also regenerates 3 Spirit per second.
                /// </summary>
                public static Rune CircularBreathing = new Rune
                {
                    Index = 2,
                    Name = "Circular Breathing",
                    Description = "Passive: Mantra of Healing also regenerates 3 Spirit per second.",
                    Tooltip = "rune/mantra-of-healing/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 19,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Mantra of Healing also heals 3576 Life when hitting an enemy.Heal amount is increased by 20% of your Life per Hit.
                /// </summary>
                public static Rune BoonOfInspiration = new Rune
                {
                    Index = 3,
                    Name = "Boon of Inspiration",
                    Description = "Passive: Mantra of Healing also heals 3576 Life when hitting an enemy.Heal amount is increased by 20% of your Life per Hit.",
                    Tooltip = "rune/mantra-of-healing/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 19,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Mantra of Healing also increases maximum Life by 20%. 
                /// </summary>
                public static Rune HeavenlyBody = new Rune
                {
                    Index = 4,
                    Name = "Heavenly Body",
                    Description = "Passive: Mantra of Healing also increases maximum Life by 20%. ",
                    Tooltip = "rune/mantra-of-healing/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 19,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Mantra of Healing also reduces damage taken by 30% when below 50% Life.
                /// </summary>
                public static Rune TimeOfNeed = new Rune
                {
                    Index = 5,
                    Name = "Time of Need",
                    Description = "Passive: Mantra of Healing also reduces damage taken by 30% when below 50% Life.",
                    Tooltip = "rune/mantra-of-healing/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 19,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Mantra of Conviction

                /// <summary>
                /// Passive: Increase the strength of Mantra of Conviction so that enemies take 12% increased damage.
                /// </summary>
                public static Rune Overawe = new Rune
                {
                    Index = 1,
                    Name = "Overawe",
                    Description = "Passive: Increase the strength of Mantra of Conviction so that enemies take 12% increased damage.",
                    Tooltip = "rune/mantra-of-conviction/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 20,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Enemies affected by Mantra of Conviction deal 15% less damage.
                /// </summary>
                public static Rune Intimidation = new Rune
                {
                    Index = 2,
                    Name = "Intimidation",
                    Description = "Passive: Enemies affected by Mantra of Conviction deal 15% less damage.",
                    Tooltip = "rune/mantra-of-conviction/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 20,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Mantra of Conviction also slows the movement speed of enemies by 80%.
                /// </summary>
                public static Rune Dishearten = new Rune
                {
                    Index = 3,
                    Name = "Dishearten",
                    Description = "Passive: Mantra of Conviction also slows the movement speed of enemies by 80%.",
                    Tooltip = "rune/mantra-of-conviction/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 20,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Killing an enemy that is affected by Mantra of Conviction grants you and your allies 30% increased movement speed for 3 seconds.
                /// </summary>
                public static Rune Annihilation = new Rune
                {
                    Index = 4,
                    Name = "Annihilation",
                    Description = "Passive: Killing an enemy that is affected by Mantra of Conviction grants you and your allies 30% increased movement speed for 3 seconds.",
                    Tooltip = "rune/mantra-of-conviction/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 20,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Passive: Enemies affected by Mantra of Conviction take 38% weapon damage per second as Holy.
                /// </summary>
                public static Rune Submission = new Rune
                {
                    Index = 5,
                    Name = "Submission",
                    Description = "Passive: Enemies affected by Mantra of Conviction take 38% weapon damage per second as Holy.",
                    Tooltip = "rune/mantra-of-conviction/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 20,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };
                #endregion

                #region Skill: Epiphany

                /// <summary>
                /// Infuse yourself with sand, reducing damage taken by 50%.
                /// </summary>
                public static Rune DesertShroud = new Rune
                {
                    Index = 1,
                    Name = "Desert Shroud",
                    Description = "Infuse yourself with sand, reducing damage taken by 50%.",
                    Tooltip = "rune/epiphany/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 21,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Charge yourself with Lightning, causing your next attack after moving 10 yards to Stun enemies for 1.5 seconds.
                /// </summary>
                public static Rune Ascendance = new Rune
                {
                    Index = 2,
                    Name = "Ascendance",
                    Description = "Charge yourself with Lightning, causing your next attack after moving 10 yards to Stun enemies for 1.5 seconds.",
                    Tooltip = "rune/epiphany/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 21,
                    ModifiedDuration = TimeSpan.FromSeconds(1.5),
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Imbue yourself with water, causing your abilities to heal yourself and allies within 30 yards for 16093 Life.Heal amount is increased by 4% of your Health Globe Healing Bonus.
                /// </summary>
                public static Rune SoothingMist = new Rune
                {
                    Index = 3,
                    Name = "Soothing Mist",
                    Description = "Imbue yourself with water, causing your abilities to heal yourself and allies within 30 yards for 16093 Life.Heal amount is increased by 4% of your Health Globe Healing Bonus.",
                    Tooltip = "rune/epiphany/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 21,
                    ModifiedAreaEffectRadius = 30f,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Increases the bonus Spirit regeneration from Epiphany to 45.
                /// </summary>
                public static Rune Insight = new Rune
                {
                    Index = 4,
                    Name = "Insight",
                    Description = "Increases the bonus Spirit regeneration from Epiphany to 45.",
                    Tooltip = "rune/epiphany/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 21,
                    Class = ActorClass.Monk
                };

                /// <summary>
                /// Engulf yourself in flames, causing your attacks to assault enemies for 353% weapon damage as Fire.
                /// </summary>
                public static Rune InnerFire = new Rune
                {
                    Index = 5,
                    Name = "Inner Fire",
                    Description = "Engulf yourself in flames, causing your attacks to assault enemies for 353% weapon damage as Fire.",
                    Tooltip = "rune/epiphany/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 21,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Monk
                };
                #endregion

            }
            public class Wizard : FieldCollection<Wizard, Rune>
            {

                /// <summary>
                /// No Rune
                /// </summary>
                public static Rune None = new Rune
                {
                    Index = 0,
                    Name = "None",
                    Description = "No Rune Selected",
                    Tooltip = string.Empty,
                    TypeId = string.Empty,
                    RuneIndex = -1,
                    Class = ActorClass.Wizard
                };

                #region Skill: Magic Missile

                /// <summary>
                /// Increase the damage of Magic Missile to 325% weapon damage as Arcane.
                /// </summary>
                public static Rune ChargedBlast = new Rune
                {
                    Index = 1,
                    Name = "Charged Blast",
                    Description = "Increase the damage of Magic Missile to 325% weapon damage as Arcane.",
                    Tooltip = "rune/magic-missile/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 0,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Cast out a shard of ice that explodes on impact, causing enemies within 4.5 yards to take 175% weapon damage as Cold and be Frozen for 1 second.Enemies cannot be Frozen by Glacial Spike more than once every 5 seconds.
                /// </summary>
                public static Rune GlacialSpike = new Rune
                {
                    Index = 2,
                    Name = "Glacial Spike",
                    Description = "Cast out a shard of ice that explodes on impact, causing enemies within 4.5 yards to take 175% weapon damage as Cold and be Frozen for 1 second.Enemies cannot be Frozen by Glacial Spike more than once every 5 seconds.",
                    Tooltip = "rune/magic-missile/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 4.5f,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Fire 3 missiles that each deal 80% weapon damage as Arcane.
                /// </summary>
                public static Rune Split = new Rune
                {
                    Index = 3,
                    Name = "Split",
                    Description = "Fire 3 missiles that each deal 80% weapon damage as Arcane.",
                    Tooltip = "rune/magic-missile/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 0,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Missiles track the nearest enemy. Missile damage is increased to 285% weapon damage as Arcane.
                /// </summary>
                public static Rune Seeker = new Rune
                {
                    Index = 4,
                    Name = "Seeker",
                    Description = "Missiles track the nearest enemy. Missile damage is increased to 285% weapon damage as Arcane.",
                    Tooltip = "rune/magic-missile/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 0,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// The missile pierces through enemies and causes them to burn for 130% weapon damage as Fire over 3 seconds.Burn damage stacks up to 3 times and any Fire damage taken from your other spells refresh all stacks to their maximum duration.
                /// </summary>
                public static Rune Conflagrate = new Rune
                {
                    Index = 5,
                    Name = "Conflagrate",
                    Description = "The missile pierces through enemies and causes them to burn for 130% weapon damage as Fire over 3 seconds.Burn damage stacks up to 3 times and any Fire damage taken from your other spells refresh all stacks to their maximum duration.",
                    Tooltip = "rune/magic-missile/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Ray of Frost

                /// <summary>
                /// Reduce channeling cost to 11 Arcane Power.
                /// </summary>
                public static Rune ColdBlood = new Rune
                {
                    Index = 1,
                    Name = "Cold Blood",
                    Description = "Reduce channeling cost to 11 Arcane Power.",
                    Tooltip = "rune/ray-of-frost/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 1,
                    ModifiedCost = 11,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Ray of Frost has a 10% chance to Freeze enemies for 1 second and increases the Slow amount to 80% for 3 seconds.
                /// </summary>
                public static Rune Numb = new Rune
                {
                    Index = 2,
                    Name = "Numb",
                    Description = "Ray of Frost has a 10% chance to Freeze enemies for 1 second and increases the Slow amount to 80% for 3 seconds.",
                    Tooltip = "rune/ray-of-frost/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies killed by Ray of Frost leave behind a patch of ice that deals 1625% weapon damage as Cold to enemies moving through it over 3 seconds.
                /// </summary>
                public static Rune BlackIce = new Rune
                {
                    Index = 3,
                    Name = "Black Ice",
                    Description = "Enemies killed by Ray of Frost leave behind a patch of ice that deals 1625% weapon damage as Cold to enemies moving through it over 3 seconds.",
                    Tooltip = "rune/ray-of-frost/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Create a swirling storm around you that grows up to a 22 yard radius, dealing 300% weapon damage as Cold to all enemies caught within it.Ray of Frost damage is increased by 220% weapon damage every second, up to a maximum total of 740% weapon damage as Cold.
                /// </summary>
                public static Rune SleetStorm = new Rune
                {
                    Index = 4,
                    Name = "Sleet Storm",
                    Description = "Create a swirling storm around you that grows up to a 22 yard radius, dealing 300% weapon damage as Cold to all enemies caught within it.Ray of Frost damage is increased by 220% weapon damage every second, up to a maximum total of 740% weapon damage as Cold.",
                    Tooltip = "rune/ray-of-frost/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 1,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies hit by Ray of Frost take 15% increased damage from Cold for 4 seconds.
                /// </summary>
                public static Rune SnowBlast = new Rune
                {
                    Index = 5,
                    Name = "Snow Blast",
                    Description = "Enemies hit by Ray of Frost take 15% increased damage from Cold for 4 seconds.",
                    Tooltip = "rune/ray-of-frost/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Shock Pulse

                /// <summary>
                /// Slain enemies explode, dealing 184% weapon damage as Cold to every enemy within 10 yards.Shock Pulse&#39;s damage turns into Cold.
                /// </summary>
                public static Rune ExplosiveBolts = new Rune
                {
                    Index = 1,
                    Name = "Explosive Bolts",
                    Description = "Slain enemies explode, dealing 184% weapon damage as Cold to every enemy within 10 yards.Shock Pulse&#39;s damage turns into Cold.",
                    Tooltip = "rune/shock-pulse/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 2,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Cast 3 bolts of fire that each deal 274% weapon damage as Fire.
                /// </summary>
                public static Rune FireBolts = new Rune
                {
                    Index = 2,
                    Name = "Fire Bolts",
                    Description = "Cast 3 bolts of fire that each deal 274% weapon damage as Fire.",
                    Tooltip = "rune/shock-pulse/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 2,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Merge the bolts in a single giant orb that oscillates forward dealing 214% weapon damage as Lightning to everything it hits.
                /// </summary>
                public static Rune PiercingOrb = new Rune
                {
                    Index = 3,
                    Name = "Piercing Orb",
                    Description = "Merge the bolts in a single giant orb that oscillates forward dealing 214% weapon damage as Lightning to everything it hits.",
                    Tooltip = "rune/shock-pulse/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 2,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Gain 2 Arcane Power for each enemy hit.Shock Pulse&#39;s damage turns into Arcane.
                /// </summary>
                public static Rune PowerAffinity = new Rune
                {
                    Index = 4,
                    Name = "Power Affinity",
                    Description = "Gain 2 Arcane Power for each enemy hit.Shock Pulse&#39;s damage turns into Arcane.",
                    Tooltip = "rune/shock-pulse/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 2,
                    ModifiedElement = Element.Arcane,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Conjure a being of lightning that drifts forward, electrocuting nearby enemies for 165% weapon damage as Lightning.
                /// </summary>
                public static Rune LivingLightning = new Rune
                {
                    Index = 5,
                    Name = "Living Lightning",
                    Description = "Conjure a being of lightning that drifts forward, electrocuting nearby enemies for 165% weapon damage as Lightning.",
                    Tooltip = "rune/shock-pulse/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 2,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Frost Nova

                /// <summary>
                /// A frozen enemy that is killed has a 100% chance of releasing another Frost Nova.
                /// </summary>
                public static Rune Shatter = new Rune
                {
                    Index = 1,
                    Name = "Shatter",
                    Description = "A frozen enemy that is killed has a 100% chance of releasing another Frost Nova.",
                    Tooltip = "rune/frost-nova/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 3,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Reduce the cooldown to 7.5 seconds and increase the Freeze duration to 3 seconds.
                /// </summary>
                public static Rune ColdSnap = new Rune
                {
                    Index = 2,
                    Name = "Cold Snap",
                    Description = "Reduce the cooldown to 7.5 seconds and increase the Freeze duration to 3 seconds.",
                    Tooltip = "rune/frost-nova/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(7.5),
                    ModifiedCooldown = TimeSpan.FromSeconds(7.5),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Frost Nova no longer freezes enemies, but instead leaves behind a mist of frost that deals 915% weapon damage as Cold over 8 seconds.
                /// </summary>
                public static Rune FrozenMist = new Rune
                {
                    Index = 3,
                    Name = "Frozen Mist",
                    Description = "Frost Nova no longer freezes enemies, but instead leaves behind a mist of frost that deals 915% weapon damage as Cold over 8 seconds.",
                    Tooltip = "rune/frost-nova/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Gain a 10% bonus to Critical Hit Chance for 11 seconds if Frost Nova hits 5 or more enemies.
                /// </summary>
                public static Rune DeepFreeze = new Rune
                {
                    Index = 4,
                    Name = "Deep Freeze",
                    Description = "Gain a 10% bonus to Critical Hit Chance for 11 seconds if Frost Nova hits 5 or more enemies.",
                    Tooltip = "rune/frost-nova/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(11),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies take 33% more damage while frozen or chilled by Frost Nova.
                /// </summary>
                public static Rune BoneChill = new Rune
                {
                    Index = 5,
                    Name = "Bone Chill",
                    Description = "Enemies take 33% more damage while frozen or chilled by Frost Nova.",
                    Tooltip = "rune/frost-nova/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 3,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Arcane Orb

                /// <summary>
                /// Increase the speed of the orb and its damage to 700% weapon damage as Arcane, but reduce the area of effect to 8 yards.
                /// </summary>
                public static Rune Obliteration = new Rune
                {
                    Index = 1,
                    Name = "Obliteration",
                    Description = "Increase the speed of the orb and its damage to 700% weapon damage as Arcane, but reduce the area of effect to 8 yards.",
                    Tooltip = "rune/arcane-orb/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 4,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Create 4 Arcane Orbs that orbit you, exploding for 265% weapon damage as Arcane when enemies get close. 
                /// </summary>
                public static Rune ArcaneOrbit = new Rune
                {
                    Index = 2,
                    Name = "Arcane Orbit",
                    Description = "Create 4 Arcane Orbs that orbit you, exploding for 265% weapon damage as Arcane when enemies get close. ",
                    Tooltip = "rune/arcane-orb/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 4,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Lob an electrified orb over enemies that zaps them for 349% weapon damage as Lightning and increases the damage of the next Lightning spell you cast by 2% for every enemy hit up to a maximum of 15.
                /// </summary>
                public static Rune Spark = new Rune
                {
                    Index = 3,
                    Name = "Spark",
                    Description = "Lob an electrified orb over enemies that zaps them for 349% weapon damage as Lightning and increases the damage of the next Lightning spell you cast by 2% for every enemy hit up to a maximum of 15.",
                    Tooltip = "rune/arcane-orb/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 4,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Launch a burning orb that deals 221% weapon damage as Fire. The orb leaves behind a wall of Fire that deals 734% weapon damage as Fire over 5 seconds.
                /// </summary>
                public static Rune Scorch = new Rune
                {
                    Index = 4,
                    Name = "Scorch",
                    Description = "Launch a burning orb that deals 221% weapon damage as Fire. The orb leaves behind a wall of Fire that deals 734% weapon damage as Fire over 5 seconds.",
                    Tooltip = "rune/arcane-orb/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 4,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Create an orb of frozen death that shreds an area with ice bolts, dealing 393% weapon damage as Cold.
                /// </summary>
                public static Rune FrozenOrb = new Rune
                {
                    Index = 5,
                    Name = "Frozen Orb",
                    Description = "Create an orb of frozen death that shreds an area with ice bolts, dealing 393% weapon damage as Cold.",
                    Tooltip = "rune/arcane-orb/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 4,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Diamond Skin

                /// <summary>
                /// Increase the maximum amount of damage absorbed to 80% of your Life.
                /// </summary>
                public static Rune CrystalShell = new Rune
                {
                    Index = 1,
                    Name = "Crystal Shell",
                    Description = "Increase the maximum amount of damage absorbed to 80% of your Life.",
                    Tooltip = "rune/diamond-skin/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 5,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Reduce the Arcane Power cost of all skills by 9 while Diamond Skin is active.
                /// </summary>
                public static Rune Prism = new Rune
                {
                    Index = 2,
                    Name = "Prism",
                    Description = "Reduce the Arcane Power cost of all skills by 9 while Diamond Skin is active.",
                    Tooltip = "rune/diamond-skin/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 5,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Increases your movement speed by 30% while Diamond Skin is active.
                /// </summary>
                public static Rune SleekShell = new Rune
                {
                    Index = 3,
                    Name = "Sleek Shell",
                    Description = "Increases your movement speed by 30% while Diamond Skin is active.",
                    Tooltip = "rune/diamond-skin/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 5,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Increase the duration of Diamond Skin to 6 seconds.
                /// </summary>
                public static Rune EnduringSkin = new Rune
                {
                    Index = 4,
                    Name = "Enduring Skin",
                    Description = "Increase the duration of Diamond Skin to 6 seconds.",
                    Tooltip = "rune/diamond-skin/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// When Diamond Skin fades, diamond shards explode in all directions dealing 863% weapon damage as Arcane to nearby enemies.
                /// </summary>
                public static Rune DiamondShards = new Rune
                {
                    Index = 5,
                    Name = "Diamond Shards",
                    Description = "When Diamond Skin fades, diamond shards explode in all directions dealing 863% weapon damage as Arcane to nearby enemies.",
                    Tooltip = "rune/diamond-skin/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 5,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Wave of Force

                /// <summary>
                /// Wave of Force repels projectiles back toward their shooter, knocks back nearby enemies and Slows them by 60% for 3 seconds.Wave of Force gains a 5 second cooldown.
                /// </summary>
                public static Rune ImpactfulWave = new Rune
                {
                    Index = 1,
                    Name = "Impactful Wave",
                    Description = "Wave of Force repels projectiles back toward their shooter, knocks back nearby enemies and Slows them by 60% for 3 seconds.Wave of Force gains a 5 second cooldown.",
                    Tooltip = "rune/wave-of-force/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedCooldown = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies hit deal 20% reduced damage for 4 seconds.
                /// </summary>
                public static Rune DebilitatingForce = new Rune
                {
                    Index = 2,
                    Name = "Debilitating Force",
                    Description = "Enemies hit deal 20% reduced damage for 4 seconds.",
                    Tooltip = "rune/wave-of-force/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Each enemy hit increases the damage of your next Arcane spell by 4%.
                /// </summary>
                public static Rune ArcaneAttunement = new Rune
                {
                    Index = 3,
                    Name = "Arcane Attunement",
                    Description = "Each enemy hit increases the damage of your next Arcane spell by 4%.",
                    Tooltip = "rune/wave-of-force/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 6,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Each enemy hit restores 1 Arcane Power.Wave of Force&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune StaticPulse = new Rune
                {
                    Index = 4,
                    Name = "Static Pulse",
                    Description = "Each enemy hit restores 1 Arcane Power.Wave of Force&#39;s damage turns into Lightning.",
                    Tooltip = "rune/wave-of-force/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 6,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Increase the damage to 475% weapon damage as Fire.
                /// </summary>
                public static Rune HeatWave = new Rune
                {
                    Index = 5,
                    Name = "Heat Wave",
                    Description = "Increase the damage to 475% weapon damage as Fire.",
                    Tooltip = "rune/wave-of-force/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 6,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Spectral Blade

                /// <summary>
                /// Each enemy hit increases the damage of your Fire spells by 1%, up to a maximum of 30%, for 5 seconds.
                /// </summary>
                public static Rune FlameBlades = new Rune
                {
                    Index = 1,
                    Name = "Flame Blades",
                    Description = "Each enemy hit increases the damage of your Fire spells by 1%, up to a maximum of 30%, for 5 seconds.",
                    Tooltip = "rune/spectral-blade/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Gain 2 Arcane Power for each enemy hit.
                /// </summary>
                public static Rune SiphoningBlade = new Rune
                {
                    Index = 2,
                    Name = "Siphoning Blade",
                    Description = "Gain 2 Arcane Power for each enemy hit.",
                    Tooltip = "rune/spectral-blade/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 7,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Extend the reach of Spectral Blade to 20 yards and increase its damage to 231% weapon damage as Lightning.
                /// </summary>
                public static Rune ThrownBlade = new Rune
                {
                    Index = 3,
                    Name = "Thrown Blade",
                    Description = "Extend the reach of Spectral Blade to 20 yards and increase its damage to 231% weapon damage as Lightning.",
                    Tooltip = "rune/spectral-blade/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 7,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// With each cast, gain a protective shield for 3 seconds that absorbs 4% of your Life in damage.
                /// </summary>
                public static Rune BarrierBlades = new Rune
                {
                    Index = 4,
                    Name = "Barrier Blades",
                    Description = "With each cast, gain a protective shield for 3 seconds that absorbs 4% of your Life in damage.",
                    Tooltip = "rune/spectral-blade/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Chilled enemies have a 5% chance to be Frozen and Frozen enemies have a 5% increased chance to be critically hit by Spectral Blade.
                /// </summary>
                public static Rune IceBlades = new Rune
                {
                    Index = 5,
                    Name = "Ice Blades",
                    Description = "Chilled enemies have a 5% chance to be Frozen and Frozen enemies have a 5% increased chance to be critically hit by Spectral Blade.",
                    Tooltip = "rune/spectral-blade/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 7,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Arcane Torrent

                /// <summary>
                /// You take 15% less damage from attacks while channeling. Every second you channel increases this amount by 5%, up to a maximum total of 25% damage reduction.Arcane Torrent&#39;s damage turns into Fire.
                /// </summary>
                public static Rune FlameWard = new Rune
                {
                    Index = 1,
                    Name = "Flame Ward",
                    Description = "You take 15% less damage from attacks while channeling. Every second you channel increases this amount by 5%, up to a maximum total of 25% damage reduction.Arcane Torrent&#39;s damage turns into Fire.",
                    Tooltip = "rune/arcane-torrent/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 8,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Unleash a torrent of power beyond your control. You no longer direct where the projectiles go, but their damage is greatly increased to 1215% weapon damage as Arcane.Arcane Torrent damage is increased by 640% weapon damage every second, up to a maximum total of 2495% weapon damage as Arcane.
                /// </summary>
                public static Rune DeathBlossom = new Rune
                {
                    Index = 2,
                    Name = "Death Blossom",
                    Description = "Unleash a torrent of power beyond your control. You no longer direct where the projectiles go, but their damage is greatly increased to 1215% weapon damage as Arcane.Arcane Torrent damage is increased by 640% weapon damage every second, up to a maximum total of 2495% weapon damage as Arcane.",
                    Tooltip = "rune/arcane-torrent/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 8,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Lay Arcane mines that arm after 2 seconds. These mines explode when an enemy approaches, dealing 825% weapon damage as Arcane. Enemies caught in the explosion have their movement and attack speeds reduced by 60% for 3 seconds.
                /// </summary>
                public static Rune ArcaneMines = new Rune
                {
                    Index = 3,
                    Name = "Arcane Mines",
                    Description = "Lay Arcane mines that arm after 2 seconds. These mines explode when an enemy approaches, dealing 825% weapon damage as Arcane. Enemies caught in the explosion have their movement and attack speeds reduced by 60% for 3 seconds.",
                    Tooltip = "rune/arcane-torrent/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 8,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Each missile explodes into 2 piercing bolts of electricity that each deal 150% weapon damage as Lightning.
                /// </summary>
                public static Rune StaticDischarge = new Rune
                {
                    Index = 4,
                    Name = "Static Discharge",
                    Description = "Each missile explodes into 2 piercing bolts of electricity that each deal 150% weapon damage as Lightning.",
                    Tooltip = "rune/arcane-torrent/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 8,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies hit by Arcane Torrent have a 12.5% chance to fire a new missile at a nearby enemy dealing 582% weapon damage as Arcane.
                /// </summary>
                public static Rune Cascade = new Rune
                {
                    Index = 5,
                    Name = "Cascade",
                    Description = "Enemies hit by Arcane Torrent have a 12.5% chance to fire a new missile at a nearby enemy dealing 582% weapon damage as Arcane.",
                    Tooltip = "rune/arcane-torrent/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 8,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Energy Twister

                /// <summary>
                /// Reduce the casting cost of Energy Twister to 25 Arcane Power.Energy Twister&#39;s damage turns into Cold.
                /// </summary>
                public static Rune MistralBreeze = new Rune
                {
                    Index = 1,
                    Name = "Mistral Breeze",
                    Description = "Reduce the casting cost of Energy Twister to 25 Arcane Power.Energy Twister&#39;s damage turns into Cold.",
                    Tooltip = "rune/energy-twister/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 9,
                    ModifiedCost = 25,
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies hit by Energy Twister take 15% increased damage from Fire for 4 seconds.
                /// </summary>
                public static Rune GaleForce = new Rune
                {
                    Index = 2,
                    Name = "Gale Force",
                    Description = "Enemies hit by Energy Twister take 15% increased damage from Fire for 4 seconds.",
                    Tooltip = "rune/energy-twister/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 9,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// When two Energy Twisters collide, they merge into a tornado with increased area of effect that causes 3200% weapon damage as Arcane over 6 seconds.
                /// </summary>
                public static Rune RagingStorm = new Rune
                {
                    Index = 3,
                    Name = "Raging Storm",
                    Description = "When two Energy Twisters collide, they merge into a tornado with increased area of effect that causes 3200% weapon damage as Arcane over 6 seconds.",
                    Tooltip = "rune/energy-twister/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 9,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Energy Twister no longer travels but spins in place, dealing 835% weapon damage as Arcane over 6 seconds to everything caught in it.
                /// </summary>
                public static Rune WickedWind = new Rune
                {
                    Index = 4,
                    Name = "Wicked Wind",
                    Description = "Energy Twister no longer travels but spins in place, dealing 835% weapon damage as Arcane over 6 seconds to everything caught in it.",
                    Tooltip = "rune/energy-twister/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 9,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Each cast of Energy Twister grants you a Lightning Charge. You can store up to 3 Lightning Charges at a time. Casting a Signature spell releases all Lightning Charges as a bolt of Lightning that deals 196% weapon damage as Lightning per Lightning Charge.Energy Twister&#39;s damage turns into Lightning.The following skills are Signature spells: Magic Missile Shock Pulse Spectral Blade Electrocute
                /// </summary>
                public static Rune StormChaser = new Rune
                {
                    Index = 5,
                    Name = "Storm Chaser",
                    Description = "Each cast of Energy Twister grants you a Lightning Charge. You can store up to 3 Lightning Charges at a time. Casting a Signature spell releases all Lightning Charges as a bolt of Lightning that deals 196% weapon damage as Lightning per Lightning Charge.Energy Twister&#39;s damage turns into Lightning.The following skills are Signature spells: Magic Missile Shock Pulse Spectral Blade Electrocute",
                    Tooltip = "rune/energy-twister/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 9,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Ice Armor

                /// <summary>
                /// Lower the temperature of the air around you. Nearby enemies are chilled, slowing their movement speed by 80%.
                /// </summary>
                public static Rune ChillingAura = new Rune
                {
                    Index = 1,
                    Name = "Chilling Aura",
                    Description = "Lower the temperature of the air around you. Nearby enemies are chilled, slowing their movement speed by 80%.",
                    Tooltip = "rune/ice-armor/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 10,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// When you are struck by a melee attack, your Armor is increased by 20% for 30 seconds. This effect stacks up to 3 times.
                /// </summary>
                public static Rune Crystallize = new Rune
                {
                    Index = 2,
                    Name = "Crystallize",
                    Description = "When you are struck by a melee attack, your Armor is increased by 20% for 30 seconds. This effect stacks up to 3 times.",
                    Tooltip = "rune/ice-armor/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 10,
                    ModifiedDuration = TimeSpan.FromSeconds(30),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Melee attackers also take 189% weapon damage as Cold.
                /// </summary>
                public static Rune JaggedIce = new Rune
                {
                    Index = 3,
                    Name = "Jagged Ice",
                    Description = "Melee attackers also take 189% weapon damage as Cold.",
                    Tooltip = "rune/ice-armor/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 10,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Melee attacks have a 40% chance to create a Frost Nova centered on the attacker, dealing 75% weapon damage as Cold.
                /// </summary>
                public static Rune IceReflect = new Rune
                {
                    Index = 4,
                    Name = "Ice Reflect",
                    Description = "Melee attacks have a 40% chance to create a Frost Nova centered on the attacker, dealing 75% weapon damage as Cold.",
                    Tooltip = "rune/ice-armor/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 10,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// A whirling storm of ice builds around you, dealing 80% weapon damage as Cold every second.
                /// </summary>
                public static Rune FrozenStorm = new Rune
                {
                    Index = 5,
                    Name = "Frozen Storm",
                    Description = "A whirling storm of ice builds around you, dealing 80% weapon damage as Cold every second.",
                    Tooltip = "rune/ice-armor/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 10,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Electrocute

                /// <summary>
                /// Increase the maximum number of enemies that can be electrocuted to 10.
                /// </summary>
                public static Rune ChainLightning = new Rune
                {
                    Index = 1,
                    Name = "Chain Lightning",
                    Description = "Increase the maximum number of enemies that can be electrocuted to 10.",
                    Tooltip = "rune/electrocute/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 11,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Critical Hits release 4 charged bolts in random directions, dealing 44% weapon damage as Fire to any enemies hit.
                /// </summary>
                public static Rune ForkedLightning = new Rune
                {
                    Index = 2,
                    Name = "Forked Lightning",
                    Description = "Critical Hits release 4 charged bolts in random directions, dealing 44% weapon damage as Fire to any enemies hit.",
                    Tooltip = "rune/electrocute/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 11,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Create streaks of lightning that pierce through enemies for 140% weapon damage as Lightning.
                /// </summary>
                public static Rune LightningBlast = new Rune
                {
                    Index = 3,
                    Name = "Lightning Blast",
                    Description = "Create streaks of lightning that pierce through enemies for 140% weapon damage as Lightning.",
                    Tooltip = "rune/electrocute/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 11,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Gain 1 Arcane Power for each enemy hit.
                /// </summary>
                public static Rune SurgeOfPower = new Rune
                {
                    Index = 4,
                    Name = "Surge of Power",
                    Description = "Gain 1 Arcane Power for each enemy hit.",
                    Tooltip = "rune/electrocute/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 11,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Blast a cone of lightning that deals 310% weapon damage as Lightning to all affected enemies.
                /// </summary>
                public static Rune ArcLightning = new Rune
                {
                    Index = 5,
                    Name = "Arc Lightning",
                    Description = "Blast a cone of lightning that deals 310% weapon damage as Lightning to all affected enemies.",
                    Tooltip = "rune/electrocute/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 11,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Slow Time

                /// <summary>
                /// Increase the potency of the movement speed reduction to 80% and reduces the cooldown to 12 seconds.
                /// </summary>
                public static Rune TimeShell = new Rune
                {
                    Index = 1,
                    Name = "Time Shell",
                    Description = "Increase the potency of the movement speed reduction to 80% and reduces the cooldown to 12 seconds.",
                    Tooltip = "rune/slow-time/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(12),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies caught by Slow Time deal 25% less damage.
                /// </summary>
                public static Rune Exhaustion = new Rune
                {
                    Index = 2,
                    Name = "Exhaustion",
                    Description = "Enemies caught by Slow Time deal 25% less damage.",
                    Tooltip = "rune/slow-time/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 12,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies caught in the bubble of warped time take 15% more damage.
                /// </summary>
                public static Rune TimeWarp = new Rune
                {
                    Index = 3,
                    Name = "Time Warp",
                    Description = "Enemies caught in the bubble of warped time take 15% more damage.",
                    Tooltip = "rune/slow-time/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 12,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies that enter or leave the Slow Time area are stunned for 5 seconds.
                /// </summary>
                public static Rune PointOfNoReturn = new Rune
                {
                    Index = 4,
                    Name = "Point of No Return",
                    Description = "Enemies that enter or leave the Slow Time area are stunned for 5 seconds.",
                    Tooltip = "rune/slow-time/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Time is sped up for any allies standing in the area, increasing their attack speed by 10%.
                /// </summary>
                public static Rune StretchTime = new Rune
                {
                    Index = 5,
                    Name = "Stretch Time",
                    Description = "Time is sped up for any allies standing in the area, increasing their attack speed by 10%.",
                    Tooltip = "rune/slow-time/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 12,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Storm Armor

                /// <summary>
                /// Ranged and melee attackers are shocked for 189% weapon damage as Lightning.
                /// </summary>
                public static Rune ReactiveArmor = new Rune
                {
                    Index = 1,
                    Name = "Reactive Armor",
                    Description = "Ranged and melee attackers are shocked for 189% weapon damage as Lightning.",
                    Tooltip = "rune/storm-armor/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 13,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Reduce the Arcane Power cost of all skills by 3 while Storm Armor is active.
                /// </summary>
                public static Rune PowerOfTheStorm = new Rune
                {
                    Index = 2,
                    Name = "Power of the Storm",
                    Description = "Reduce the Arcane Power cost of all skills by 3 while Storm Armor is active.",
                    Tooltip = "rune/storm-armor/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 13,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Increase the damage of the shock to 315% weapon damage as Lightning. 
                /// </summary>
                public static Rune ThunderStorm = new Rune
                {
                    Index = 3,
                    Name = "Thunder Storm",
                    Description = "Increase the damage of the shock to 315% weapon damage as Lightning. ",
                    Tooltip = "rune/storm-armor/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 13,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Increase your movement speed by 25% for 3 seconds when you are hit by melee or ranged attacks.
                /// </summary>
                public static Rune Scramble = new Rune
                {
                    Index = 4,
                    Name = "Scramble",
                    Description = "Increase your movement speed by 25% for 3 seconds when you are hit by melee or ranged attacks.",
                    Tooltip = "rune/storm-armor/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Critical Hits have a chance to electrocute a nearby enemy for 425% weapon damage as Lightning.
                /// </summary>
                public static Rune ShockingAspect = new Rune
                {
                    Index = 5,
                    Name = "Shocking Aspect",
                    Description = "Critical Hits have a chance to electrocute a nearby enemy for 425% weapon damage as Lightning.",
                    Tooltip = "rune/storm-armor/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 13,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Explosive Blast

                /// <summary>
                /// Increases the damage of Explosive Blast to 1485%.
                /// </summary>
                public static Rune Unleashed = new Rune
                {
                    Index = 1,
                    Name = "Unleashed",
                    Description = "Increases the damage of Explosive Blast to 1485%.",
                    Tooltip = "rune/explosive-blast/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 14,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Reduce the cooldown of Explosive Blast to 3 seconds.Explosive Blast&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune Flash = new Rune
                {
                    Index = 2,
                    Name = "Flash",
                    Description = "Reduce the cooldown of Explosive Blast to 3 seconds.Explosive Blast&#39;s damage turns into Lightning.",
                    Tooltip = "rune/explosive-blast/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 14,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Immediately release the energy of Explosive Blast for 909% weapon damage as Fire.
                /// </summary>
                public static Rune ShortFuse = new Rune
                {
                    Index = 3,
                    Name = "Short Fuse",
                    Description = "Immediately release the energy of Explosive Blast for 909% weapon damage as Fire.",
                    Tooltip = "rune/explosive-blast/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 14,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Release an enormous Explosive Blast that deals 990% weapon damage as Cold to all enemies within 18 yards.
                /// </summary>
                public static Rune Obliterate = new Rune
                {
                    Index = 4,
                    Name = "Obliterate",
                    Description = "Release an enormous Explosive Blast that deals 990% weapon damage as Cold to all enemies within 18 yards.",
                    Tooltip = "rune/explosive-blast/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 14,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 18f,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Instead of a single explosion, release a chain of 3 consecutive explosions, each dealing 520% weapon damage as Fire.
                /// </summary>
                public static Rune ChainReaction = new Rune
                {
                    Index = 5,
                    Name = "Chain Reaction",
                    Description = "Instead of a single explosion, release a chain of 3 consecutive explosions, each dealing 520% weapon damage as Fire.",
                    Tooltip = "rune/explosive-blast/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 14,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Magic Weapon

                /// <summary>
                /// Attacks have a chance to cause lightning to arc to 3 nearby enemies, dealing 61% weapon damage as Lightning.
                /// </summary>
                public static Rune Electrify = new Rune
                {
                    Index = 1,
                    Name = "Electrify",
                    Description = "Attacks have a chance to cause lightning to arc to 3 nearby enemies, dealing 61% weapon damage as Lightning.",
                    Tooltip = "rune/magic-weapon/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 15,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Increase the damage bonus of Magic Weapon to 20% damage.
                /// </summary>
                public static Rune ForceWeapon = new Rune
                {
                    Index = 2,
                    Name = "Force Weapon",
                    Description = "Increase the damage bonus of Magic Weapon to 20% damage.",
                    Tooltip = "rune/magic-weapon/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 15,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies hit by your attacks restore up to 3 Arcane Power.
                /// </summary>
                public static Rune Conduit = new Rune
                {
                    Index = 3,
                    Name = "Conduit",
                    Description = "Enemies hit by your attacks restore up to 3 Arcane Power.",
                    Tooltip = "rune/magic-weapon/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 15,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Attacks have a chance to burn enemies, dealing 300% weapon damage as Fire over 3 seconds.
                /// </summary>
                public static Rune Ignite = new Rune
                {
                    Index = 4,
                    Name = "Ignite",
                    Description = "Attacks have a chance to burn enemies, dealing 300% weapon damage as Fire over 3 seconds.",
                    Tooltip = "rune/magic-weapon/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// When you perform an attack, gain a protective shield for 3 seconds that absorbs 4% of your Life in damage.
                /// </summary>
                public static Rune Deflection = new Rune
                {
                    Index = 5,
                    Name = "Deflection",
                    Description = "When you perform an attack, gain a protective shield for 3 seconds that absorbs 4% of your Life in damage.",
                    Tooltip = "rune/magic-weapon/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Hydra

                /// <summary>
                /// Summon an Arcane Hydra that spits Arcane Orbs that explode on impact, dealing 205% weapon damage as Arcane to enemies near the explosion.
                /// </summary>
                public static Rune ArcaneHydra = new Rune
                {
                    Index = 1,
                    Name = "Arcane Hydra",
                    Description = "Summon an Arcane Hydra that spits Arcane Orbs that explode on impact, dealing 205% weapon damage as Arcane to enemies near the explosion.",
                    Tooltip = "rune/hydra/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 16,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Summon a Lightning Hydra that electrocutes enemies for 255% weapon damage as Lightning.
                /// </summary>
                public static Rune LightningHydra = new Rune
                {
                    Index = 2,
                    Name = "Lightning Hydra",
                    Description = "Summon a Lightning Hydra that electrocutes enemies for 255% weapon damage as Lightning.",
                    Tooltip = "rune/hydra/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 16,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Summon a Blazing Hydra that spits bolts of Fire that burn enemies near the point of impact, dealing 155% weapon damage as Fire over 3 seconds. Burn damage can stack multiple times on the same enemy.
                /// </summary>
                public static Rune BlazingHydra = new Rune
                {
                    Index = 3,
                    Name = "Blazing Hydra",
                    Description = "Summon a Blazing Hydra that spits bolts of Fire that burn enemies near the point of impact, dealing 155% weapon damage as Fire over 3 seconds. Burn damage can stack multiple times on the same enemy.",
                    Tooltip = "rune/hydra/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 16,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Summon a Frost Hydra that breathes a short range cone of frost, causing 255% weapon damage as Cold to all enemies in the cone.
                /// </summary>
                public static Rune FrostHydra = new Rune
                {
                    Index = 4,
                    Name = "Frost Hydra",
                    Description = "Summon a Frost Hydra that breathes a short range cone of frost, causing 255% weapon damage as Cold to all enemies in the cone.",
                    Tooltip = "rune/hydra/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 16,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Summon a Mammoth Hydra that breathes a river of flame at nearby enemies, dealing 330% weapon damage per second as Fire to enemies caught on the burning ground.
                /// </summary>
                public static Rune MammothHydra = new Rune
                {
                    Index = 5,
                    Name = "Mammoth Hydra",
                    Description = "Summon a Mammoth Hydra that breathes a river of flame at nearby enemies, dealing 330% weapon damage per second as Fire to enemies caught on the burning ground.",
                    Tooltip = "rune/hydra/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 16,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Disintegrate

                /// <summary>
                /// Increase the width of the beam allowing it to hit more enemies.Disintegrate&#39;s damage turns into Fire.
                /// </summary>
                public static Rune Convergence = new Rune
                {
                    Index = 1,
                    Name = "Convergence",
                    Description = "Increase the width of the beam allowing it to hit more enemies.Disintegrate&#39;s damage turns into Fire.",
                    Tooltip = "rune/disintegrate/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 17,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies killed by the beam have a 35% chance to explode causing 750% weapon damage as Arcane to all enemies within 8 yards.
                /// </summary>
                public static Rune Volatility = new Rune
                {
                    Index = 2,
                    Name = "Volatility",
                    Description = "Enemies killed by the beam have a 35% chance to explode causing 750% weapon damage as Arcane to all enemies within 8 yards.",
                    Tooltip = "rune/disintegrate/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 17,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 8f,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// The beam fractures into a short-ranged cone that deals 435% weapon damage as Arcane.Disintegrate damage is increased by 340% weapon damage every second, up to a maximum total of 1115% weapon damage as Arcane.
                /// </summary>
                public static Rune Entropy = new Rune
                {
                    Index = 3,
                    Name = "Entropy",
                    Description = "The beam fractures into a short-ranged cone that deals 435% weapon damage as Arcane.Disintegrate damage is increased by 340% weapon damage every second, up to a maximum total of 1115% weapon damage as Arcane.",
                    Tooltip = "rune/disintegrate/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 17,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// While channeling the beam you become charged with energy and discharge at nearby enemies dealing 115% weapon damage as Arcane.
                /// </summary>
                public static Rune ChaosNexus = new Rune
                {
                    Index = 4,
                    Name = "Chaos Nexus",
                    Description = "While channeling the beam you become charged with energy and discharge at nearby enemies dealing 115% weapon damage as Arcane.",
                    Tooltip = "rune/disintegrate/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 17,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies hit by Disintegrate take 15% increased damage from Arcane for 4 seconds.
                /// </summary>
                public static Rune Intensify = new Rune
                {
                    Index = 5,
                    Name = "Intensify",
                    Description = "Enemies hit by Disintegrate take 15% increased damage from Arcane for 4 seconds.",
                    Tooltip = "rune/disintegrate/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Familiar

                /// <summary>
                /// Summon a fiery Familiar that grants you 10% increased damage.
                /// </summary>
                public static Rune Sparkflint = new Rune
                {
                    Index = 1,
                    Name = "Sparkflint",
                    Description = "Summon a fiery Familiar that grants you 10% increased damage.",
                    Tooltip = "rune/familiar/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 18,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// The Familiar&#39;s projectiles have a 35% chance to Freeze the enemy for 1 second.
                /// </summary>
                public static Rune Icicle = new Rune
                {
                    Index = 2,
                    Name = "Icicle",
                    Description = "The Familiar&#39;s projectiles have a 35% chance to Freeze the enemy for 1 second.",
                    Tooltip = "rune/familiar/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 18,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Summon a protective Familiar. When you are below 50% Life the Familiar will absorb damage from 1 attack every 6 seconds.
                /// </summary>
                public static Rune AncientGuardian = new Rune
                {
                    Index = 3,
                    Name = "Ancient Guardian",
                    Description = "Summon a protective Familiar. When you are below 50% Life the Familiar will absorb damage from 1 attack every 6 seconds.",
                    Tooltip = "rune/familiar/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 18,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// While the Familiar is active, you regenerate 4.5 Arcane Power every second.
                /// </summary>
                public static Rune Arcanot = new Rune
                {
                    Index = 4,
                    Name = "Arcanot",
                    Description = "While the Familiar is active, you regenerate 4.5 Arcane Power every second.",
                    Tooltip = "rune/familiar/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 18,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// The Familiar&#39;s projectiles explode on impact, dealing 240% weapon damage as Arcane to all enemies within 6 yards.
                /// </summary>
                public static Rune Cannoneer = new Rune
                {
                    Index = 5,
                    Name = "Cannoneer",
                    Description = "The Familiar&#39;s projectiles explode on impact, dealing 240% weapon damage as Arcane to all enemies within 6 yards.",
                    Tooltip = "rune/familiar/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 18,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 6f,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Teleport

                /// <summary>
                /// For 5 seconds after you Teleport, you will take 25% less damage.
                /// </summary>
                public static Rune SafePassage = new Rune
                {
                    Index = 1,
                    Name = "Safe Passage",
                    Description = "For 5 seconds after you Teleport, you will take 25% less damage.",
                    Tooltip = "rune/teleport/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 19,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// After casting Teleport, you have 3 seconds to Teleport 1 additional time.
                /// </summary>
                public static Rune Wormhole = new Rune
                {
                    Index = 2,
                    Name = "Wormhole",
                    Description = "After casting Teleport, you have 3 seconds to Teleport 1 additional time.",
                    Tooltip = "rune/teleport/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 19,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Casting Teleport again within 5 seconds will instantly return you to your original location and set the remaining cooldown to 1 seconds.
                /// </summary>
                public static Rune Reversal = new Rune
                {
                    Index = 3,
                    Name = "Reversal",
                    Description = "Casting Teleport again within 5 seconds will instantly return you to your original location and set the remaining cooldown to 1 seconds.",
                    Tooltip = "rune/teleport/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 19,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Summon 2 decoys for 6 seconds after teleporting.
                /// </summary>
                public static Rune Fracture = new Rune
                {
                    Index = 4,
                    Name = "Fracture",
                    Description = "Summon 2 decoys for 6 seconds after teleporting.",
                    Tooltip = "rune/teleport/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 19,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Cast a short range Wave of Force upon arrival, dealing 175% weapon damage as Arcane to all nearby enemies and stunning them for 1 second.
                /// </summary>
                public static Rune Calamity = new Rune
                {
                    Index = 5,
                    Name = "Calamity",
                    Description = "Cast a short range Wave of Force upon arrival, dealing 175% weapon damage as Arcane to all nearby enemies and stunning them for 1 second.",
                    Tooltip = "rune/teleport/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 19,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Mirror Image

                /// <summary>
                /// Increase the Life of your Mirror Images to 200% of your own.
                /// </summary>
                public static Rune Simulacrum = new Rune
                {
                    Index = 1,
                    Name = "Simulacrum",
                    Description = "Increase the Life of your Mirror Images to 200% of your own.",
                    Tooltip = "rune/mirror-image/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 20,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Summon 4 Mirror Images that taunt nearby enemies for 1 second and each have 50% of your Life.
                /// </summary>
                public static Rune Duplicates = new Rune
                {
                    Index = 2,
                    Name = "Duplicates",
                    Description = "Summon 4 Mirror Images that taunt nearby enemies for 1 second and each have 50% of your Life.",
                    Tooltip = "rune/mirror-image/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 20,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// When a Mirror Image is destroyed, it explodes, dealing 309% weapon damage as Arcane with a 50% chance to Stun for 2 seconds.
                /// </summary>
                public static Rune MockingDemise = new Rune
                {
                    Index = 3,
                    Name = "Mocking Demise",
                    Description = "When a Mirror Image is destroyed, it explodes, dealing 309% weapon damage as Arcane with a 50% chance to Stun for 2 seconds.",
                    Tooltip = "rune/mirror-image/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 20,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Increase the duration of your Mirror Images to 10 seconds and their Life to 100% of your Life.
                /// </summary>
                public static Rune ExtensionOfWill = new Rune
                {
                    Index = 4,
                    Name = "Extension of Will",
                    Description = "Increase the duration of your Mirror Images to 10 seconds and their Life to 100% of your Life.",
                    Tooltip = "rune/mirror-image/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 20,
                    ModifiedDuration = TimeSpan.FromSeconds(10),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Spells cast by your Mirror Images will deal 20% of the damage of your own spells.
                /// </summary>
                public static Rune MirrorMimics = new Rune
                {
                    Index = 5,
                    Name = "Mirror Mimics",
                    Description = "Spells cast by your Mirror Images will deal 20% of the damage of your own spells.",
                    Tooltip = "rune/mirror-image/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 20,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Meteor

                /// <summary>
                /// Removes the delay before Meteor comes crashing down.Meteor&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune ThunderCrash = new Rune
                {
                    Index = 1,
                    Name = "Thunder Crash",
                    Description = "Removes the delay before Meteor comes crashing down.Meteor&#39;s damage turns into Lightning.",
                    Tooltip = "rune/meteor/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 21,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Expend all remaining Arcane Power. Each point of extra Arcane Power spent increases the impact damage of Meteor by 20% weapon damage as Arcane.
                /// </summary>
                public static Rune StarPact = new Rune
                {
                    Index = 2,
                    Name = "Star Pact",
                    Description = "Expend all remaining Arcane Power. Each point of extra Arcane Power spent increases the impact damage of Meteor by 20% weapon damage as Arcane.",
                    Tooltip = "rune/meteor/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 21,
                    ModifiedElement = Element.Arcane,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Summon a Comet that deals 740% weapon damage as Cold and freezes chilled enemies for 1 second upon impact.The impact site is covered in an icy mist that deals 235% weapon damage as Cold over 3 seconds.
                /// </summary>
                public static Rune Comet = new Rune
                {
                    Index = 3,
                    Name = "Comet",
                    Description = "Summon a Comet that deals 740% weapon damage as Cold and freezes chilled enemies for 1 second upon impact.The impact site is covered in an icy mist that deals 235% weapon damage as Cold over 3 seconds.",
                    Tooltip = "rune/meteor/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 21,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Unleash a volley of 7 small Meteors that each strike for 277% weapon damage as Fire.
                /// </summary>
                public static Rune MeteorShower = new Rune
                {
                    Index = 4,
                    Name = "Meteor Shower",
                    Description = "Unleash a volley of 7 small Meteors that each strike for 277% weapon damage as Fire.",
                    Tooltip = "rune/meteor/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 21,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Greatly increases the size and increases the damage of the Meteor impact to 1648% weapon damage as Fire and the molten fire to 625% weapon damage as Fire over 3 seconds.Adds a 15 second cooldown.
                /// </summary>
                public static Rune MoltenImpact = new Rune
                {
                    Index = 5,
                    Name = "Molten Impact",
                    Description = "Greatly increases the size and increases the damage of the Meteor impact to 1648% weapon damage as Fire and the molten fire to 625% weapon damage as Fire over 3 seconds.Adds a 15 second cooldown.",
                    Tooltip = "rune/meteor/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 21,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedCooldown = TimeSpan.FromSeconds(15),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Blizzard

                /// <summary>
                /// Enemies affected by Blizzard take 15% increased damage from Lightning.
                /// </summary>
                public static Rune LightningStorm = new Rune
                {
                    Index = 1,
                    Name = "Lightning Storm",
                    Description = "Enemies affected by Blizzard take 15% increased damage from Lightning.",
                    Tooltip = "rune/blizzard/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 22,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies caught in the Blizzard have a 100% chance to be Frozen for 2.5 seconds.
                /// </summary>
                public static Rune FrozenSolid = new Rune
                {
                    Index = 2,
                    Name = "Frozen Solid",
                    Description = "Enemies caught in the Blizzard have a 100% chance to be Frozen for 2.5 seconds.",
                    Tooltip = "rune/blizzard/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(2.5),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Reduce the casting cost of Blizzard to 10 Arcane Power.
                /// </summary>
                public static Rune Snowbound = new Rune
                {
                    Index = 3,
                    Name = "Snowbound",
                    Description = "Reduce the casting cost of Blizzard to 10 Arcane Power.",
                    Tooltip = "rune/blizzard/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 22,
                    ModifiedCost = 10,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Increase the area of effect of Blizzard to a 30 yard radius.Blizzard&#39;s damage turns into Fire.
                /// </summary>
                public static Rune Apocalypse = new Rune
                {
                    Index = 4,
                    Name = "Apocalypse",
                    Description = "Increase the area of effect of Blizzard to a 30 yard radius.Blizzard&#39;s damage turns into Fire.",
                    Tooltip = "rune/blizzard/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 22,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Increase the duration and damage of Blizzard to deal 1810% weapon damage as Cold over 8 seconds.
                /// </summary>
                public static Rune UnrelentingStorm = new Rune
                {
                    Index = 5,
                    Name = "Unrelenting Storm",
                    Description = "Increase the duration and damage of Blizzard to deal 1810% weapon damage as Cold over 8 seconds.",
                    Tooltip = "rune/blizzard/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Energy Armor

                /// <summary>
                /// You have a chance to gain 4 Arcane Power when you are hit by a ranged or melee attack.
                /// </summary>
                public static Rune Absorption = new Rune
                {
                    Index = 1,
                    Name = "Absorption",
                    Description = "You have a chance to gain 4 Arcane Power when you are hit by a ranged or melee attack.",
                    Tooltip = "rune/energy-armor/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 23,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Energy Armor also increases your Critical Hit Chance by 5%.
                /// </summary>
                public static Rune PinpointBarrier = new Rune
                {
                    Index = 2,
                    Name = "Pinpoint Barrier",
                    Description = "Energy Armor also increases your Critical Hit Chance by 5%.",
                    Tooltip = "rune/energy-armor/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 23,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Rather than decreasing your maximum Arcane Power, Energy Armor increases it by 20.
                /// </summary>
                public static Rune EnergyTap = new Rune
                {
                    Index = 3,
                    Name = "Energy Tap",
                    Description = "Rather than decreasing your maximum Arcane Power, Energy Armor increases it by 20.",
                    Tooltip = "rune/energy-armor/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 23,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Incoming attacks that would deal more than 35% of your maximum Life are reduced to deal 35% of your maximum Life instead.The amount absorbed cannot exceed 100% of your maximum Life.
                /// </summary>
                public static Rune ForceArmor = new Rune
                {
                    Index = 4,
                    Name = "Force Armor",
                    Description = "Incoming attacks that would deal more than 35% of your maximum Life are reduced to deal 35% of your maximum Life instead.The amount absorbed cannot exceed 100% of your maximum Life.",
                    Tooltip = "rune/energy-armor/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 23,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Energy Armor also increases your resistance to all damage types 25%.
                /// </summary>
                public static Rune PrismaticArmor = new Rune
                {
                    Index = 5,
                    Name = "Prismatic Armor",
                    Description = "Energy Armor also increases your resistance to all damage types 25%.",
                    Tooltip = "rune/energy-armor/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 23,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Archon

                /// <summary>
                /// An explosion erupts around you when you transform, dealing 3680% weapon damage as Fire to all enemies within 15 yards.Archon abilities deal Fire damage instead of Arcane.
                /// </summary>
                public static Rune Combustion = new Rune
                {
                    Index = 1,
                    Name = "Combustion",
                    Description = "An explosion erupts around you when you transform, dealing 3680% weapon damage as Fire to all enemies within 15 yards.Archon abilities deal Fire damage instead of Arcane.",
                    Tooltip = "rune/archon/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 24,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 15f,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Archon form can cast Teleport with a 2 second cooldown.
                /// </summary>
                public static Rune Teleport = new Rune
                {
                    Index = 2,
                    Name = "Teleport",
                    Description = "Archon form can cast Teleport with a 2 second cooldown.",
                    Tooltip = "rune/archon/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 24,
                    ModifiedCooldown = TimeSpan.FromSeconds(2),
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Decrease the cooldown of Archon to 100 seconds.Archon abilities deal Lightning damage instead of Arcane.
                /// </summary>
                public static Rune PurePower = new Rune
                {
                    Index = 3,
                    Name = "Pure Power",
                    Description = "Decrease the cooldown of Archon to 100 seconds.Archon abilities deal Lightning damage instead of Arcane.",
                    Tooltip = "rune/archon/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 24,
                    ModifiedDuration = TimeSpan.FromSeconds(100),
                    ModifiedCooldown = TimeSpan.FromSeconds(100),
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Archon form can cast a Slow Time that follows you and your Arcane Blast and Arcane Strike abilities Freeze enemies for 1 seconds.Archon abilities deal Cold damage instead of Arcane.
                /// </summary>
                public static Rune SlowTime = new Rune
                {
                    Index = 4,
                    Name = "Slow Time",
                    Description = "Archon form can cast a Slow Time that follows you and your Arcane Blast and Arcane Strike abilities Freeze enemies for 1 seconds.Archon abilities deal Cold damage instead of Arcane.",
                    Tooltip = "rune/archon/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 24,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Increase the damage of all Archon abilities by 50%.
                /// </summary>
                public static Rune ImprovedArchon = new Rune
                {
                    Index = 5,
                    Name = "Improved Archon",
                    Description = "Increase the damage of all Archon abilities by 50%.",
                    Tooltip = "rune/archon/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 24,
                    Class = ActorClass.Wizard
                };
                #endregion

                #region Skill: Black Hole

                /// <summary>
                /// Increases the Black Hole radius to 20 yards and damage to 1290% weapon damage as Lightning over 2 seconds.
                /// </summary>
                public static Rune Supermassive = new Rune
                {
                    Index = 1,
                    Name = "Supermassive",
                    Description = "Increases the Black Hole radius to 20 yards and damage to 1290% weapon damage as Lightning over 2 seconds.",
                    Tooltip = "rune/black-hole/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 25,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Each enemy hit increases the damage of your Cold spells by 3% for 10 seconds.Black Hole&#39;s damage turns into Cold.
                /// </summary>
                public static Rune AbsoluteZero = new Rune
                {
                    Index = 2,
                    Name = "Absolute Zero",
                    Description = "Each enemy hit increases the damage of your Cold spells by 3% for 10 seconds.Black Hole&#39;s damage turns into Cold.",
                    Tooltip = "rune/black-hole/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 25,
                    ModifiedDuration = TimeSpan.FromSeconds(10),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// The Black Hole also absorbs enemy projectiles and objects from Elite monster affixes within 15 yards.
                /// </summary>
                public static Rune EventHorizon = new Rune
                {
                    Index = 3,
                    Name = "Event Horizon",
                    Description = "The Black Hole also absorbs enemy projectiles and objects from Elite monster affixes within 15 yards.",
                    Tooltip = "rune/black-hole/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 25,
                    ModifiedAreaEffectRadius = 15f,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Conjure a Black Hole at the target location that draws enemies to it and deals 700% weapon damage as Fire over 2 seconds to all enemies within 15 yards.After the Black Hole disappears, an explosion occurs that deals 725% weapon damage as Fire to enemies within 15 yards.
                /// </summary>
                public static Rune Blazar = new Rune
                {
                    Index = 4,
                    Name = "Blazar",
                    Description = "Conjure a Black Hole at the target location that draws enemies to it and deals 700% weapon damage as Fire over 2 seconds to all enemies within 15 yards.After the Black Hole disappears, an explosion occurs that deals 725% weapon damage as Fire to enemies within 15 yards.",
                    Tooltip = "rune/black-hole/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 25,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 15f,
                    Class = ActorClass.Wizard
                };

                /// <summary>
                /// Enemies hit by Black Hole deal 10% reduced damage for 5 seconds. Each enemy hit by Black Hole grants you 3% increased damage for 5 seconds.
                /// </summary>
                public static Rune Spellsteal = new Rune
                {
                    Index = 5,
                    Name = "Spellsteal",
                    Description = "Enemies hit by Black Hole deal 10% reduced damage for 5 seconds. Each enemy hit by Black Hole grants you 3% increased damage for 5 seconds.",
                    Tooltip = "rune/black-hole/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 25,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Wizard
                };
                #endregion

            }
            public class Barbarian : FieldCollection<Barbarian, Rune>
            {

                /// <summary>
                /// No Rune
                /// </summary>
                public static Rune None = new Rune
                {
                    Index = 0,
                    Name = "None",
                    Description = "No Rune Selected",
                    Tooltip = string.Empty,
                    TypeId = string.Empty,
                    RuneIndex = -1,
                    Class = ActorClass.Barbarian
                };

                #region Skill: Bash

                /// <summary>
                /// Each hit Freezes the enemy for 1.5 seconds.Enemies can be frozen by Bash once every 5 seconds.
                /// </summary>
                public static Rune Frostbite = new Rune
                {
                    Index = 1,
                    Name = "Frostbite",
                    Description = "Each hit Freezes the enemy for 1.5 seconds.Enemies can be frozen by Bash once every 5 seconds.",
                    Tooltip = "rune/bash/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(1.5),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// The enemy has a 10% increased chance to be Critically Hit for 3 seconds.Bash&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune Onslaught = new Rune
                {
                    Index = 2,
                    Name = "Onslaught",
                    Description = "The enemy has a 10% increased chance to be Critically Hit for 3 seconds.Bash&#39;s damage turns into Lightning.",
                    Tooltip = "rune/bash/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase your damage by 4% for 5 seconds after using Bash. This effect stacks up to 3 times.
                /// </summary>
                public static Rune Punish = new Rune
                {
                    Index = 3,
                    Name = "Punish",
                    Description = "Increase your damage by 4% for 5 seconds after using Bash. This effect stacks up to 3 times.",
                    Tooltip = "rune/bash/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase Fury generated to 9.Bash&#39;s damage turns into Fire.
                /// </summary>
                public static Rune Instigation = new Rune
                {
                    Index = 4,
                    Name = "Instigation",
                    Description = "Increase Fury generated to 9.Bash&#39;s damage turns into Fire.",
                    Tooltip = "rune/bash/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 0,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Each hit causes a shockwave that deals 100% weapon damage as Fire to enemies in a 26 yard line behind the primary enemy.
                /// </summary>
                public static Rune Pulverize = new Rune
                {
                    Index = 5,
                    Name = "Pulverize",
                    Description = "Each hit causes a shockwave that deals 100% weapon damage as Fire to enemies in a 26 yard line behind the primary enemy.",
                    Tooltip = "rune/bash/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 0,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Hammer of the Ancients

                /// <summary>
                /// Create a shockwave that deals 505% weapon damage to all enemies within 22 yards in front of you.
                /// </summary>
                public static Rune RollingThunder = new Rune
                {
                    Index = 1,
                    Name = "Rolling Thunder",
                    Description = "Create a shockwave that deals 505% weapon damage to all enemies within 22 yards in front of you.",
                    Tooltip = "rune/hammer-of-the-ancients/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 1,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 22f,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Smash for 640% weapon damage as Fire.
                /// </summary>
                public static Rune Smash = new Rune
                {
                    Index = 2,
                    Name = "Smash",
                    Description = "Smash for 640% weapon damage as Fire.",
                    Tooltip = "rune/hammer-of-the-ancients/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 1,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Each hit creates a tremor at the point of impact for 2 seconds that Chills enemies by 80%.Hammer of the Ancients&#39;s damage turns into Cold.
                /// </summary>
                public static Rune TheDevilsAnvil = new Rune
                {
                    Index = 3,
                    Name = "The Devil's Anvil",
                    Description = "Each hit creates a tremor at the point of impact for 2 seconds that Chills enemies by 80%.Hammer of the Ancients&#39;s damage turns into Cold.",
                    Tooltip = "rune/hammer-of-the-ancients/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// When you kill an enemy with Hammer of the Ancients, other enemies within 10 yards are Stunned for 2 seconds.Hammer of the Ancients turns into Lightning damage.
                /// </summary>
                public static Rune Thunderstrike = new Rune
                {
                    Index = 4,
                    Name = "Thunderstrike",
                    Description = "When you kill an enemy with Hammer of the Ancients, other enemies within 10 yards are Stunned for 2 seconds.Hammer of the Ancients turns into Lightning damage.",
                    Tooltip = "rune/hammer-of-the-ancients/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Lightning,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Critical Hits heal you for 3% of your maximum Life.
                /// </summary>
                public static Rune Birthright = new Rune
                {
                    Index = 5,
                    Name = "Birthright",
                    Description = "Critical Hits heal you for 3% of your maximum Life.",
                    Tooltip = "rune/hammer-of-the-ancients/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 1,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Cleave

                /// <summary>
                /// Enemies slain by Cleave explode, causing 160% weapon damage as Fire to all other enemies within 8 yards.
                /// </summary>
                public static Rune Rupture = new Rune
                {
                    Index = 1,
                    Name = "Rupture",
                    Description = "Enemies slain by Cleave explode, causing 160% weapon damage as Fire to all other enemies within 8 yards.",
                    Tooltip = "rune/cleave/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 2,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 8f,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Generate 1 additional Fury per enemy hit.Cleave&#39;s damage turns into Fire.
                /// </summary>
                public static Rune ReapingSwing = new Rune
                {
                    Index = 2,
                    Name = "Reaping Swing",
                    Description = "Generate 1 additional Fury per enemy hit.Cleave&#39;s damage turns into Fire.",
                    Tooltip = "rune/cleave/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 2,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// On Critical Hits, knock enemies up into the air and deal 80% weapon damage to enemies where they land.
                /// </summary>
                public static Rune ScatteringBlast = new Rune
                {
                    Index = 3,
                    Name = "Scattering Blast",
                    Description = "On Critical Hits, knock enemies up into the air and deal 80% weapon damage to enemies where they land.",
                    Tooltip = "rune/cleave/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 2,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Swing at all enemies around you and increase damage to 235% weapon damage as Lightning.
                /// </summary>
                public static Rune BroadSweep = new Rune
                {
                    Index = 4,
                    Name = "Broad Sweep",
                    Description = "Swing at all enemies around you and increase damage to 235% weapon damage as Lightning.",
                    Tooltip = "rune/cleave/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 2,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Enemies cleaved are Chilled and take 10% increased damage from all sources for 3 seconds.Cleave&#39;s damage turns into Cold.
                /// </summary>
                public static Rune GatheringStorm = new Rune
                {
                    Index = 5,
                    Name = "Gathering Storm",
                    Description = "Enemies cleaved are Chilled and take 10% increased damage from all sources for 3 seconds.Cleave&#39;s damage turns into Cold.",
                    Tooltip = "rune/cleave/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 2,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Ground Stomp

                /// <summary>
                /// Reduce the cooldown of Ground Stomp to 8 seconds.Enemies in the area have their movement speed slowed by 80% for 8 seconds after they recover from being stunned.
                /// </summary>
                public static Rune DeafeningCrash = new Rune
                {
                    Index = 1,
                    Name = "Deafening Crash",
                    Description = "Reduce the cooldown of Ground Stomp to 8 seconds.Enemies in the area have their movement speed slowed by 80% for 8 seconds after they recover from being stunned.",
                    Tooltip = "rune/ground-stomp/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase the area of effect to 24 yards. Enemies are pulled closer before the strike lands.
                /// </summary>
                public static Rune WrenchingSmash = new Rune
                {
                    Index = 2,
                    Name = "Wrenching Smash",
                    Description = "Increase the area of effect to 24 yards. Enemies are pulled closer before the strike lands.",
                    Tooltip = "rune/ground-stomp/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 3,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Enemies in the area also take 575% weapon damage as Fire.
                /// </summary>
                public static Rune TremblingStomp = new Rune
                {
                    Index = 3,
                    Name = "Trembling Stomp",
                    Description = "Enemies in the area also take 575% weapon damage as Fire.",
                    Tooltip = "rune/ground-stomp/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 3,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase Fury generated to 30.
                /// </summary>
                public static Rune FootOfTheMountain = new Rune
                {
                    Index = 4,
                    Name = "Foot of the Mountain",
                    Description = "Increase Fury generated to 30.",
                    Tooltip = "rune/ground-stomp/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 3,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Enemies hit have a 10% chance to drop a health globe.
                /// </summary>
                public static Rune JarringSlam = new Rune
                {
                    Index = 5,
                    Name = "Jarring Slam",
                    Description = "Enemies hit have a 10% chance to drop a health globe.",
                    Tooltip = "rune/ground-stomp/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 3,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Rend

                /// <summary>
                /// Increase the range of Rend to hit all enemies within 18 yards.Rend&#39;s damage turns into Fire.
                /// </summary>
                public static Rune Ravage = new Rune
                {
                    Index = 1,
                    Name = "Ravage",
                    Description = "Increase the range of Rend to hit all enemies within 18 yards.Rend&#39;s damage turns into Fire.",
                    Tooltip = "rune/rend/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 4,
                    ModifiedElement = Element.Fire,
                    ModifiedAreaEffectRadius = 18f,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Heal for 0.5% of your maximum Life per second for each affected enemy.
                /// </summary>
                public static Rune BloodLust = new Rune
                {
                    Index = 2,
                    Name = "Blood Lust",
                    Description = "Heal for 0.5% of your maximum Life per second for each affected enemy.",
                    Tooltip = "rune/rend/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 4,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase damage to 1350% weapon damage as Lightning over 5 seconds.
                /// </summary>
                public static Rune Lacerate = new Rune
                {
                    Index = 3,
                    Name = "Lacerate",
                    Description = "Increase damage to 1350% weapon damage as Lightning over 5 seconds.",
                    Tooltip = "rune/rend/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 4,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Affected enemies are Chilled and take 10% increased damage from all sources.Rend&#39;s damage turns into Cold.
                /// </summary>
                public static Rune Mutilate = new Rune
                {
                    Index = 4,
                    Name = "Mutilate",
                    Description = "Affected enemies are Chilled and take 10% increased damage from all sources.Rend&#39;s damage turns into Cold.",
                    Tooltip = "rune/rend/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 4,
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Enemies killed while bleeding cause all enemies within 10 yards to begin bleeding for 1100% weapon damage as Physical over 5 seconds.
                /// </summary>
                public static Rune Bloodbath = new Rune
                {
                    Index = 5,
                    Name = "Bloodbath",
                    Description = "Enemies killed while bleeding cause all enemies within 10 yards to begin bleeding for 1100% weapon damage as Physical over 5 seconds.",
                    Tooltip = "rune/rend/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 4,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Leap

                /// <summary>
                /// Gain 150% additional Armor for 4 seconds after landing.
                /// </summary>
                public static Rune IronImpact = new Rune
                {
                    Index = 1,
                    Name = "Iron Impact",
                    Description = "Gain 150% additional Armor for 4 seconds after landing.",
                    Tooltip = "rune/leap/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// You leap with such great force that enemies within 10 yards of the takeoff point take 180% weapon damage and are also slowed by 60% for 3 seconds.
                /// </summary>
                public static Rune Launch = new Rune
                {
                    Index = 2,
                    Name = "Launch",
                    Description = "You leap with such great force that enemies within 10 yards of the takeoff point take 180% weapon damage and are also slowed by 60% for 3 seconds.",
                    Tooltip = "rune/leap/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase the damage of Leap to 450% and send enemies hurtling away from where you land.
                /// </summary>
                public static Rune TopplingImpact = new Rune
                {
                    Index = 3,
                    Name = "Toppling Impact",
                    Description = "Increase the damage of Leap to 450% and send enemies hurtling away from where you land.",
                    Tooltip = "rune/leap/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 5,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Shockwaves burst forth from the ground increasing the radius of effect to 16 yards and pulling affected enemies towards you.
                /// </summary>
                public static Rune CallOfArreat = new Rune
                {
                    Index = 4,
                    Name = "Call of Arreat",
                    Description = "Shockwaves burst forth from the ground increasing the radius of effect to 16 yards and pulling affected enemies towards you.",
                    Tooltip = "rune/leap/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 5,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Land with such force that enemies have a 100% chance to be stunned for 3 seconds.
                /// </summary>
                public static Rune DeathFromAbove = new Rune
                {
                    Index = 5,
                    Name = "Death from Above",
                    Description = "Land with such force that enemies have a 100% chance to be stunned for 3 seconds.",
                    Tooltip = "rune/leap/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Overpower

                /// <summary>
                /// Throw up to 3 axes at nearby enemies that each deal 380% weapon damage.
                /// </summary>
                public static Rune StormOfSteel = new Rune
                {
                    Index = 1,
                    Name = "Storm of Steel",
                    Description = "Throw up to 3 axes at nearby enemies that each deal 380% weapon damage.",
                    Tooltip = "rune/overpower/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 6,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Your Critical Hit Chance is increased by 8% for 5 seconds.Overpower&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune KillingSpree = new Rune
                {
                    Index = 2,
                    Name = "Killing Spree",
                    Description = "Your Critical Hit Chance is increased by 8% for 5 seconds.Overpower&#39;s damage turns into Lightning.",
                    Tooltip = "rune/overpower/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Redirect 35% of incoming melee damage back to the attacker for 5 seconds after Overpower is activated.
                /// </summary>
                public static Rune CrushingAdvance = new Rune
                {
                    Index = 3,
                    Name = "Crushing Advance",
                    Description = "Redirect 35% of incoming melee damage back to the attacker for 5 seconds after Overpower is activated.",
                    Tooltip = "rune/overpower/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Generate 5 Fury for each enemy hit by Overpower.
                /// </summary>
                public static Rune Momentum = new Rune
                {
                    Index = 4,
                    Name = "Momentum",
                    Description = "Generate 5 Fury for each enemy hit by Overpower.",
                    Tooltip = "rune/overpower/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 6,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase damage to 760% weapon damage as Fire.
                /// </summary>
                public static Rune Revel = new Rune
                {
                    Index = 5,
                    Name = "Revel",
                    Description = "Increase damage to 760% weapon damage as Fire.",
                    Tooltip = "rune/overpower/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 6,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Frenzy

                /// <summary>
                /// Each strike has a 25% chance to throw a piercing axe at a nearby enemy that deals 300% weapon damage as Cold to all enemies in its path.Frenzy&#39;s damage turns into Cold.
                /// </summary>
                public static Rune Sidearm = new Rune
                {
                    Index = 1,
                    Name = "Sidearm",
                    Description = "Each strike has a 25% chance to throw a piercing axe at a nearby enemy that deals 300% weapon damage as Cold to all enemies in its path.Frenzy&#39;s damage turns into Cold.",
                    Tooltip = "rune/frenzy/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 7,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase Fury generated to 6.Frenzy&#39;s damage turns into Cold.
                /// </summary>
                public static Rune Berserk = new Rune
                {
                    Index = 2,
                    Name = "Berserk",
                    Description = "Increase Fury generated to 6.Frenzy&#39;s damage turns into Cold.",
                    Tooltip = "rune/frenzy/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 7,
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Gain 5% movement speed for each stack of Frenzy.
                /// </summary>
                public static Rune Vanguard = new Rune
                {
                    Index = 3,
                    Name = "Vanguard",
                    Description = "Gain 5% movement speed for each stack of Frenzy.",
                    Tooltip = "rune/frenzy/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 7,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Each hit has a 30% chance to call down a bolt of lightning from above, stunning the enemy for 1.5 seconds.
                /// </summary>
                public static Rune Smite = new Rune
                {
                    Index = 4,
                    Name = "Smite",
                    Description = "Each hit has a 30% chance to call down a bolt of lightning from above, stunning the enemy for 1.5 seconds.",
                    Tooltip = "rune/frenzy/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(1.5),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Each Frenzy effect also increases your damage by 2.5%.Frenzy&#39;s damage turns into Fire.
                /// </summary>
                public static Rune Maniac = new Rune
                {
                    Index = 5,
                    Name = "Maniac",
                    Description = "Each Frenzy effect also increases your damage by 2.5%.Frenzy&#39;s damage turns into Fire.",
                    Tooltip = "rune/frenzy/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 7,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Seismic Slam

                /// <summary>
                /// Reduce the cost to 22 Fury.Seismic Slam&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune Stagger = new Rune
                {
                    Index = 1,
                    Name = "Stagger",
                    Description = "Reduce the cost to 22 Fury.Seismic Slam&#39;s damage turns into Lightning.",
                    Tooltip = "rune/seismic-slam/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 8,
                    ModifiedCost = 22,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase damage to 735% weapon damage as Fire and knocks all enemies hit up into the air.
                /// </summary>
                public static Rune ShatteredGround = new Rune
                {
                    Index = 2,
                    Name = "Shattered Ground",
                    Description = "Increase damage to 735% weapon damage as Fire and knocks all enemies hit up into the air.",
                    Tooltip = "rune/seismic-slam/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 8,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// The ground continues to shudder after the initial strike, damaging enemies in the area for 230% weapon damage as Physical over 2 seconds.
                /// </summary>
                public static Rune Rumble = new Rune
                {
                    Index = 3,
                    Name = "Rumble",
                    Description = "The ground continues to shudder after the initial strike, damaging enemies in the area for 230% weapon damage as Physical over 2 seconds.",
                    Tooltip = "rune/seismic-slam/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 8,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Gain 1% of your maximum Life for every enemy hit.
                /// </summary>
                public static Rune StrengthFromEarth = new Rune
                {
                    Index = 4,
                    Name = "Strength from Earth",
                    Description = "Gain 1% of your maximum Life for every enemy hit.",
                    Tooltip = "rune/seismic-slam/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 8,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Create a sheet of frost that deals 755% weapon damage as Cold and Chills enemies by 60% for 1 seconds.
                /// </summary>
                public static Rune Permafrost = new Rune
                {
                    Index = 5,
                    Name = "Permafrost",
                    Description = "Create a sheet of frost that deals 755% weapon damage as Cold and Chills enemies by 60% for 1 seconds.",
                    Tooltip = "rune/seismic-slam/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 8,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Revenge

                /// <summary>
                /// Increase healing to 6% of maximum Life for each enemy hit.
                /// </summary>
                public static Rune BloodLaw = new Rune
                {
                    Index = 1,
                    Name = "Blood Law",
                    Description = "Increase healing to 6% of maximum Life for each enemy hit.",
                    Tooltip = "rune/revenge/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 9,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase your Critical Hit Chance by 8% for 6 seconds after using Revenge.Revenge&#39;s damage turns into Cold.
                /// </summary>
                public static Rune BestServedCold = new Rune
                {
                    Index = 2,
                    Name = "Best Served Cold",
                    Description = "Increase your Critical Hit Chance by 8% for 6 seconds after using Revenge.Revenge&#39;s damage turns into Cold.",
                    Tooltip = "rune/revenge/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 9,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase damage to 700% weapon damage as Fire.
                /// </summary>
                public static Rune Retribution = new Rune
                {
                    Index = 3,
                    Name = "Retribution",
                    Description = "Increase damage to 700% weapon damage as Fire.",
                    Tooltip = "rune/revenge/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 9,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Knockback enemies 24 yards when using Revenge.Revenge&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune Grudge = new Rune
                {
                    Index = 4,
                    Name = "Grudge",
                    Description = "Knockback enemies 24 yards when using Revenge.Revenge&#39;s damage turns into Lightning.",
                    Tooltip = "rune/revenge/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 9,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase the maximum number of charges to 3.
                /// </summary>
                public static Rune Provocation = new Rune
                {
                    Index = 5,
                    Name = "Provocation",
                    Description = "Increase the maximum number of charges to 3.",
                    Tooltip = "rune/revenge/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 9,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Threatening Shout

                /// <summary>
                /// Affected enemies also have their movement speed reduced by 60%.
                /// </summary>
                public static Rune Intimidate = new Rune
                {
                    Index = 1,
                    Name = "Intimidate",
                    Description = "Affected enemies also have their movement speed reduced by 60%.",
                    Tooltip = "rune/threatening-shout/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 10,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Enemies instead take 25% increased damage for 6 seconds.
                /// </summary>
                public static Rune Falter = new Rune
                {
                    Index = 2,
                    Name = "Falter",
                    Description = "Enemies instead take 25% increased damage for 6 seconds.",
                    Tooltip = "rune/threatening-shout/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 10,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Enemies are badly shaken and have a 15% chance to drop health globes.
                /// </summary>
                public static Rune GrimHarvest = new Rune
                {
                    Index = 3,
                    Name = "Grim Harvest",
                    Description = "Enemies are badly shaken and have a 15% chance to drop health globes.",
                    Tooltip = "rune/threatening-shout/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 10,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Affected enemies are also taunted to attack you for 4 seconds.
                /// </summary>
                public static Rune Demoralize = new Rune
                {
                    Index = 4,
                    Name = "Demoralize",
                    Description = "Affected enemies are also taunted to attack you for 4 seconds.",
                    Tooltip = "rune/threatening-shout/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 10,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Enemies are severely demoralized. Each enemy has a 100% chance to flee in Fear for 3 seconds.
                /// </summary>
                public static Rune Terrify = new Rune
                {
                    Index = 5,
                    Name = "Terrify",
                    Description = "Enemies are severely demoralized. Each enemy has a 100% chance to flee in Fear for 3 seconds.",
                    Tooltip = "rune/threatening-shout/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 10,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Sprint

                /// <summary>
                /// Increase Dodge Chance by 12% while sprinting.
                /// </summary>
                public static Rune Rush = new Rune
                {
                    Index = 1,
                    Name = "Rush",
                    Description = "Increase Dodge Chance by 12% while sprinting.",
                    Tooltip = "rune/sprint/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 11,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Tornadoes rage in your wake, each dealing 60% weapon damage as Physical for 3 seconds to nearby enemies.
                /// </summary>
                public static Rune RunLikeTheWind = new Rune
                {
                    Index = 2,
                    Name = "Run Like the Wind",
                    Description = "Tornadoes rage in your wake, each dealing 60% weapon damage as Physical for 3 seconds to nearby enemies.",
                    Tooltip = "rune/sprint/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 11,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase the movement speed bonus to 40% for 4 seconds.
                /// </summary>
                public static Rune Marathon = new Rune
                {
                    Index = 3,
                    Name = "Marathon",
                    Description = "Increase the movement speed bonus to 40% for 4 seconds.",
                    Tooltip = "rune/sprint/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 11,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Slams through enemies, knocking them back and dealing 25% weapon damage.
                /// </summary>
                public static Rune Gangway = new Rune
                {
                    Index = 4,
                    Name = "Gangway",
                    Description = "Slams through enemies, knocking them back and dealing 25% weapon damage.",
                    Tooltip = "rune/sprint/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 11,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase movement speed of allies within 50 yards by 20% for 3 seconds.
                /// </summary>
                public static Rune ForcedMarch = new Rune
                {
                    Index = 5,
                    Name = "Forced March",
                    Description = "Increase movement speed of allies within 50 yards by 20% for 3 seconds.",
                    Tooltip = "rune/sprint/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 11,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedAreaEffectRadius = 50f,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Weapon Throw

                /// <summary>
                /// Increase thrown weapon damage to 400% weapon damage as Lightning.
                /// </summary>
                public static Rune MightyThrow = new Rune
                {
                    Index = 1,
                    Name = "Mighty Throw",
                    Description = "Increase thrown weapon damage to 400% weapon damage as Lightning.",
                    Tooltip = "rune/weapon-throw/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 12,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// The weapon ricochets to 3 enemies within 20 yards of each other.Weapon Throw&#39;s damage turns into Fire.
                /// </summary>
                public static Rune Ricochet = new Rune
                {
                    Index = 2,
                    Name = "Ricochet",
                    Description = "The weapon ricochets to 3 enemies within 20 yards of each other.Weapon Throw&#39;s damage turns into Fire.",
                    Tooltip = "rune/weapon-throw/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 12,
                    ModifiedElement = Element.Fire,
                    ModifiedAreaEffectRadius = 20f,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Hurl a hammer with a 40% chance to Stun the enemy for 1 second.
                /// </summary>
                public static Rune ThrowingHammer = new Rune
                {
                    Index = 3,
                    Name = "Throwing Hammer",
                    Description = "Hurl a hammer with a 40% chance to Stun the enemy for 1 second.",
                    Tooltip = "rune/weapon-throw/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Aim for the head, gaining a 15% chance of causing your enemy to be Confused and attack other enemies for 3 seconds.
                /// </summary>
                public static Rune Stupefy = new Rune
                {
                    Index = 4,
                    Name = "Stupefy",
                    Description = "Aim for the head, gaining a 15% chance of causing your enemy to be Confused and attack other enemies for 3 seconds.",
                    Tooltip = "rune/weapon-throw/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase Fury generated to 9.Weapon Throw&#39;s damage turns into Fire.
                /// </summary>
                public static Rune BalancedWeapon = new Rune
                {
                    Index = 5,
                    Name = "Balanced Weapon",
                    Description = "Increase Fury generated to 9.Weapon Throw&#39;s damage turns into Fire.",
                    Tooltip = "rune/weapon-throw/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 12,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Earthquake

                /// <summary>
                /// 20 secondary tremors follow your movement and deal 300% weapon damage as Fire each.
                /// </summary>
                public static Rune GiantsStride = new Rune
                {
                    Index = 1,
                    Name = "Giant's Stride",
                    Description = "20 secondary tremors follow your movement and deal 300% weapon damage as Fire each.",
                    Tooltip = "rune/earthquake/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 13,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Create an icy patch, causing Earthquake to Freeze all enemies hit and deal Cold damage.
                /// </summary>
                public static Rune ChillingEarth = new Rune
                {
                    Index = 2,
                    Name = "Chilling Earth",
                    Description = "Create an icy patch, causing Earthquake to Freeze all enemies hit and deal Cold damage.",
                    Tooltip = "rune/earthquake/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 13,
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Remove the Fury cost and reduce the cooldown to 30 seconds.Earthquake&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune TheMountainsCall = new Rune
                {
                    Index = 3,
                    Name = "The Mountain's Call",
                    Description = "Remove the Fury cost and reduce the cooldown to 30 seconds.Earthquake&#39;s damage turns into Lightning.",
                    Tooltip = "rune/earthquake/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(30),
                    ModifiedCooldown = TimeSpan.FromSeconds(30),
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase Earthquake&#39;s damage to 6000% weapon damage as Fire.
                /// </summary>
                public static Rune MoltenFury = new Rune
                {
                    Index = 4,
                    Name = "Molten Fury",
                    Description = "Increase Earthquake&#39;s damage to 6000% weapon damage as Fire.",
                    Tooltip = "rune/earthquake/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 13,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// All enemies within 24 yards are pulled in towards you.Earthquake&#39;s damage turns into Physical.
                /// </summary>
                public static Rune Cavein = new Rune
                {
                    Index = 5,
                    Name = "Cave-In",
                    Description = "All enemies within 24 yards are pulled in towards you.Earthquake&#39;s damage turns into Physical.",
                    Tooltip = "rune/earthquake/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 13,
                    ModifiedElement = Element.Physical,
                    ModifiedAreaEffectRadius = 24f,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Whirlwind

                /// <summary>
                /// Generate harsh tornadoes that deal 120% weapon damage to enemies in their path.
                /// </summary>
                public static Rune DustDevils = new Rune
                {
                    Index = 1,
                    Name = "Dust Devils",
                    Description = "Generate harsh tornadoes that deal 120% weapon damage to enemies in their path.",
                    Tooltip = "rune/whirlwind/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 14,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Pull enemies from up to 35 yards away towards you while whirlwinding.Whirlwind&#39;s damage turns into Cold.
                /// </summary>
                public static Rune Hurricane = new Rune
                {
                    Index = 2,
                    Name = "Hurricane",
                    Description = "Pull enemies from up to 35 yards away towards you while whirlwinding.Whirlwind&#39;s damage turns into Cold.",
                    Tooltip = "rune/whirlwind/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 14,
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Critical Hits restore 1% of your maximum Life.
                /// </summary>
                public static Rune BloodFunnel = new Rune
                {
                    Index = 3,
                    Name = "Blood Funnel",
                    Description = "Critical Hits restore 1% of your maximum Life.",
                    Tooltip = "rune/whirlwind/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 14,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Gain 1 Fury for every enemy struck.Whirlwind&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune WindShear = new Rune
                {
                    Index = 4,
                    Name = "Wind Shear",
                    Description = "Gain 1 Fury for every enemy struck.Whirlwind&#39;s damage turns into Lightning.",
                    Tooltip = "rune/whirlwind/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 14,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Turns Whirlwind into a torrent of magma that deals 400% weapon damage as Fire.
                /// </summary>
                public static Rune VolcanicEruption = new Rune
                {
                    Index = 5,
                    Name = "Volcanic Eruption",
                    Description = "Turns Whirlwind into a torrent of magma that deals 400% weapon damage as Fire.",
                    Tooltip = "rune/whirlwind/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 14,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Furious Charge

                /// <summary>
                /// Increase the damage to 1050% weapon damage as Fire.
                /// </summary>
                public static Rune BatteringRam = new Rune
                {
                    Index = 1,
                    Name = "Battering Ram",
                    Description = "Increase the damage to 1050% weapon damage as Fire.",
                    Tooltip = "rune/furious-charge/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 15,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Recharge time is reduced by 2 seconds for every enemy hit. This effect can reduce the recharge time by up to 10 seconds.
                /// </summary>
                public static Rune MercilessAssault = new Rune
                {
                    Index = 2,
                    Name = "Merciless Assault",
                    Description = "Recharge time is reduced by 2 seconds for every enemy hit. This effect can reduce the recharge time by up to 10 seconds.",
                    Tooltip = "rune/furious-charge/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(10),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Generate 10 additional Fury for each enemy hit while charging.
                /// </summary>
                public static Rune Stamina = new Rune
                {
                    Index = 3,
                    Name = "Stamina",
                    Description = "Generate 10 additional Fury for each enemy hit while charging.",
                    Tooltip = "rune/furious-charge/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 15,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// All enemies hit are Frozen for 2.5 seconds.Furious Charge&#39;s damage turns into Cold.
                /// </summary>
                public static Rune ColdRush = new Rune
                {
                    Index = 4,
                    Name = "Cold Rush",
                    Description = "All enemies hit are Frozen for 2.5 seconds.Furious Charge&#39;s damage turns into Cold.",
                    Tooltip = "rune/furious-charge/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(2.5),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Store up to 3 charges of Furious Charge.Furious Charge&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune Dreadnought = new Rune
                {
                    Index = 5,
                    Name = "Dreadnought",
                    Description = "Store up to 3 charges of Furious Charge.Furious Charge&#39;s damage turns into Lightning.",
                    Tooltip = "rune/furious-charge/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 15,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Ignore Pain

                /// <summary>
                /// While Ignore Pain is active, gain 40% increased movement speed and knock enemies away as you run.
                /// </summary>
                public static Rune Bravado = new Rune
                {
                    Index = 1,
                    Name = "Bravado",
                    Description = "While Ignore Pain is active, gain 40% increased movement speed and knock enemies away as you run.",
                    Tooltip = "rune/ignore-pain/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 16,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase duration to 7 seconds.
                /// </summary>
                public static Rune IronHide = new Rune
                {
                    Index = 2,
                    Name = "Iron Hide",
                    Description = "Increase duration to 7 seconds.",
                    Tooltip = "rune/ignore-pain/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 16,
                    ModifiedDuration = TimeSpan.FromSeconds(7),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// While Ignore Pain is active, gain 5364 Life per Fury spent.
                /// </summary>
                public static Rune IgnoranceIsBliss = new Rune
                {
                    Index = 3,
                    Name = "Ignorance is Bliss",
                    Description = "While Ignore Pain is active, gain 5364 Life per Fury spent.",
                    Tooltip = "rune/ignore-pain/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 16,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Allies within 50 yards also gain 25% damage reduction and Immunity to control-impairing effects for 5 seconds.
                /// </summary>
                public static Rune MobRule = new Rune
                {
                    Index = 4,
                    Name = "Mob Rule",
                    Description = "Allies within 50 yards also gain 25% damage reduction and Immunity to control-impairing effects for 5 seconds.",
                    Tooltip = "rune/ignore-pain/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 16,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedAreaEffectRadius = 50f,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Instantly heal for 35% of maximum Life when activating Ignore Pain.
                /// </summary>
                public static Rune ContemptForWeakness = new Rune
                {
                    Index = 5,
                    Name = "Contempt for Weakness",
                    Description = "Instantly heal for 35% of maximum Life when activating Ignore Pain.",
                    Tooltip = "rune/ignore-pain/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 16,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Battle Rage

                /// <summary>
                /// Increase damage bonus to 15%.
                /// </summary>
                public static Rune MaraudersRage = new Rune
                {
                    Index = 1,
                    Name = "Marauder's Rage",
                    Description = "Increase damage bonus to 15%.",
                    Tooltip = "rune/battle-rage/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 17,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase movement speed by 15%.
                /// </summary>
                public static Rune Ferocity = new Rune
                {
                    Index = 2,
                    Name = "Ferocity",
                    Description = "Increase movement speed by 15%.",
                    Tooltip = "rune/battle-rage/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 17,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Critical Hits heal you and your pets for up to 21457 Life.
                /// </summary>
                public static Rune SwordsToPloughshares = new Rune
                {
                    Index = 3,
                    Name = "Swords to Ploughshares",
                    Description = "Critical Hits heal you and your pets for up to 21457 Life.",
                    Tooltip = "rune/battle-rage/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 17,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Gain 1% Critical Hit Chance for each enemy within 10 yards while under the effects of Battle Rage.
                /// </summary>
                public static Rune IntoTheFray = new Rune
                {
                    Index = 4,
                    Name = "Into the Fray",
                    Description = "Gain 1% Critical Hit Chance for each enemy within 10 yards while under the effects of Battle Rage.",
                    Tooltip = "rune/battle-rage/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 17,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Critical Hits cause an explosion of blood dealing 20% of the damage done to all other nearby enemies.
                /// </summary>
                public static Rune Bloodshed = new Rune
                {
                    Index = 5,
                    Name = "Bloodshed",
                    Description = "Critical Hits cause an explosion of blood dealing 20% of the damage done to all other nearby enemies.",
                    Tooltip = "rune/battle-rage/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 17,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Call of the Ancients

                /// <summary>
                /// The Ancients deal 540% weapon damage as Fire with each attack.
                /// </summary>
                public static Rune TheCouncilRises = new Rune
                {
                    Index = 1,
                    Name = "The Council Rises",
                    Description = "The Ancients deal 540% weapon damage as Fire with each attack.",
                    Tooltip = "rune/call-of-the-ancients/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 18,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Enemies hit by the Ancients are Chilled for 2 seconds and have 10% increased chance to be Critically Hit.The Ancients&#39; damage turns into Cold.
                /// </summary>
                public static Rune DutyToTheClan = new Rune
                {
                    Index = 2,
                    Name = "Duty to the Clan",
                    Description = "Enemies hit by the Ancients are Chilled for 2 seconds and have 10% increased chance to be Critically Hit.The Ancients&#39; damage turns into Cold.",
                    Tooltip = "rune/call-of-the-ancients/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 18,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Each point of Fury you spend heals you and your Ancients for 966 Life.
                /// </summary>
                public static Rune AncientsBlessing = new Rune
                {
                    Index = 3,
                    Name = "Ancients' Blessing",
                    Description = "Each point of Fury you spend heals you and your Ancients for 966 Life.",
                    Tooltip = "rune/call-of-the-ancients/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 18,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Gain 4 Fury every time an Ancient deals damage.
                /// </summary>
                public static Rune AncientsFury = new Rune
                {
                    Index = 4,
                    Name = "Ancients' Fury",
                    Description = "Gain 4 Fury every time an Ancient deals damage.",
                    Tooltip = "rune/call-of-the-ancients/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 18,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// 50% of all damage dealt to you is instead divided evenly between the Ancients.The Ancients&#39; damage turns into Lightning.
                /// </summary>
                public static Rune TogetherAsOne = new Rune
                {
                    Index = 5,
                    Name = "Together as One",
                    Description = "50% of all damage dealt to you is instead divided evenly between the Ancients.The Ancients&#39; damage turns into Lightning.",
                    Tooltip = "rune/call-of-the-ancients/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 18,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Ancient Spear

                /// <summary>
                /// Enemies hit are knocked back 5 yards.
                /// </summary>
                public static Rune Ranseur = new Rune
                {
                    Index = 1,
                    Name = "Ranseur",
                    Description = "Enemies hit are knocked back 5 yards.",
                    Tooltip = "rune/ancient-spear/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 19,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Add a chain to the spear to drag all enemies hit back to you and Slow them by 60% for 1 seconds.
                /// </summary>
                public static Rune Harpoon = new Rune
                {
                    Index = 2,
                    Name = "Harpoon",
                    Description = "Add a chain to the spear to drag all enemies hit back to you and Slow them by 60% for 1 seconds.",
                    Tooltip = "rune/ancient-spear/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 19,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase the damage to 640% weapon damage as Fire.
                /// </summary>
                public static Rune JaggedEdge = new Rune
                {
                    Index = 3,
                    Name = "Jagged Edge",
                    Description = "Increase the damage to 640% weapon damage as Fire.",
                    Tooltip = "rune/ancient-spear/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 19,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Expend all remaining Fury to deal 20% weapon damage for every point of Fury expended to enemies within 9 yards of the impact location.
                /// </summary>
                public static Rune BoulderToss = new Rune
                {
                    Index = 4,
                    Name = "Boulder Toss",
                    Description = "Expend all remaining Fury to deal 20% weapon damage for every point of Fury expended to enemies within 9 yards of the impact location.",
                    Tooltip = "rune/ancient-spear/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 19,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 9f,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Add a chain to the spear to throw all enemies hit behind you and Slow them by 60% for 1 seconds.
                /// </summary>
                public static Rune RageFlip = new Rune
                {
                    Index = 5,
                    Name = "Rage Flip",
                    Description = "Add a chain to the spear to throw all enemies hit behind you and Slow them by 60% for 1 seconds.",
                    Tooltip = "rune/ancient-spear/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 19,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: War Cry

                /// <summary>
                /// For the first 5 seconds, gain an additional 60% increased Armor.
                /// </summary>
                public static Rune HardenedWrath = new Rune
                {
                    Index = 1,
                    Name = "Hardened Wrath",
                    Description = "For the first 5 seconds, gain an additional 60% increased Armor.",
                    Tooltip = "rune/war-cry/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 20,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase Fury generated to 50.
                /// </summary>
                public static Rune Charge = new Rune
                {
                    Index = 2,
                    Name = "Charge!",
                    Description = "Increase Fury generated to 50.",
                    Tooltip = "rune/war-cry/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 20,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase maximum Life by 10% and Life regeneration by 13411 per second while affected by War Cry.
                /// </summary>
                public static Rune Invigorate = new Rune
                {
                    Index = 3,
                    Name = "Invigorate",
                    Description = "Increase maximum Life by 10% and Life regeneration by 13411 per second while affected by War Cry.",
                    Tooltip = "rune/war-cry/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 20,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase Dodge Chance by 30% while affected by War Cry.
                /// </summary>
                public static Rune VeteransWarning = new Rune
                {
                    Index = 4,
                    Name = "Veteran's Warning",
                    Description = "Increase Dodge Chance by 30% while affected by War Cry.",
                    Tooltip = "rune/war-cry/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 20,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Increase resistance to all elements by 20% while affected by War Cry.
                /// </summary>
                public static Rune Impunity = new Rune
                {
                    Index = 5,
                    Name = "Impunity",
                    Description = "Increase resistance to all elements by 20% while affected by War Cry.",
                    Tooltip = "rune/war-cry/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 20,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Wrath of the Berserker

                /// <summary>
                /// Activating Wrath of the Berserker deals 3400% weapon damage as Fire to all enemies within 15 yards.
                /// </summary>
                public static Rune ArreatsWail = new Rune
                {
                    Index = 1,
                    Name = "Arreat's Wail",
                    Description = "Activating Wrath of the Berserker deals 3400% weapon damage as Fire to all enemies within 15 yards.",
                    Tooltip = "rune/wrath-of-the-berserker/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 21,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 15f,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// While active, gain 50% increased damage.
                /// </summary>
                public static Rune Insanity = new Rune
                {
                    Index = 2,
                    Name = "Insanity",
                    Description = "While active, gain 50% increased damage.",
                    Tooltip = "rune/wrath-of-the-berserker/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 21,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// While active, Critical Hits have a chance to cause an eruption of blood dealing 300% weapon damage to enemies within 15 yards.
                /// </summary>
                public static Rune Slaughter = new Rune
                {
                    Index = 3,
                    Name = "Slaughter",
                    Description = "While active, Critical Hits have a chance to cause an eruption of blood dealing 300% weapon damage to enemies within 15 yards.",
                    Tooltip = "rune/wrath-of-the-berserker/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 21,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 15f,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Reduce all damage taken by 50%.
                /// </summary>
                public static Rune StridingGiant = new Rune
                {
                    Index = 4,
                    Name = "Striding Giant",
                    Description = "Reduce all damage taken by 50%.",
                    Tooltip = "rune/wrath-of-the-berserker/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 21,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// While active, gain 5364 Life per Fury spent.
                /// </summary>
                public static Rune ThriveOnChaos = new Rune
                {
                    Index = 5,
                    Name = "Thrive on Chaos",
                    Description = "While active, gain 5364 Life per Fury spent.",
                    Tooltip = "rune/wrath-of-the-berserker/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 21,
                    Class = ActorClass.Barbarian
                };
                #endregion

                #region Skill: Avalanche

                /// <summary>
                /// Chunks of molten lava are randomly launched at nearby enemies, dealing 6600% weapon damage as Fire over 5 seconds.
                /// </summary>
                public static Rune Volcano = new Rune
                {
                    Index = 1,
                    Name = "Volcano",
                    Description = "Chunks of molten lava are randomly launched at nearby enemies, dealing 6600% weapon damage as Fire over 5 seconds.",
                    Tooltip = "rune/avalanche/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Cooldown is reduced by 1 second for every 15 Fury spent.
                /// </summary>
                public static Rune Lahar = new Rune
                {
                    Index = 2,
                    Name = "Lahar",
                    Description = "Cooldown is reduced by 1 second for every 15 Fury spent.",
                    Tooltip = "rune/avalanche/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 22,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Cave-in from both sides pushes enemies together, dealing 2800% weapon damage as Cold and Slowing them by 60% for 3 seconds.
                /// </summary>
                public static Rune SnowcappedMountain = new Rune
                {
                    Index = 3,
                    Name = "Snow-Capped Mountain",
                    Description = "Cave-in from both sides pushes enemies together, dealing 2800% weapon damage as Cold and Slowing them by 60% for 3 seconds.",
                    Tooltip = "rune/avalanche/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Store up to 3 charges of Avalanche.
                /// </summary>
                public static Rune TectonicRift = new Rune
                {
                    Index = 4,
                    Name = "Tectonic Rift",
                    Description = "Store up to 3 charges of Avalanche.",
                    Tooltip = "rune/avalanche/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 22,
                    Class = ActorClass.Barbarian
                };

                /// <summary>
                /// Giant blocks of ice hit enemies for 2400% weapon damage as Cold and Freeze them.
                /// </summary>
                public static Rune Glacier = new Rune
                {
                    Index = 5,
                    Name = "Glacier",
                    Description = "Giant blocks of ice hit enemies for 2400% weapon damage as Cold and Freeze them.",
                    Tooltip = "rune/avalanche/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 22,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Barbarian
                };
                #endregion

            }
            public class Crusader : FieldCollection<Crusader, Rune>
            {

                /// <summary>
                /// No Rune
                /// </summary>
                public static Rune None = new Rune
                {
                    Index = 0,
                    Name = "None",
                    Description = "No Rune Selected",
                    Tooltip = string.Empty,
                    TypeId = string.Empty,
                    RuneIndex = -1,
                    Class = ActorClass.Crusader
                };

                #region Skill: Punish

                /// <summary>
                /// When you block with Hardened Senses active, you explode with fury dealing 75% weapon damage as Fire to enemies within 15 yards.
                /// </summary>
                public static Rune Roar = new Rune
                {
                    Index = 1,
                    Name = "Roar",
                    Description = "When you block with Hardened Senses active, you explode with fury dealing 75% weapon damage as Fire to enemies within 15 yards.",
                    Tooltip = "rune/punish/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 0,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 15f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// When you block with Hardened Senses active, you gain 15% increased Attack Speed for 3 seconds.
                /// </summary>
                public static Rune Celerity = new Rune
                {
                    Index = 2,
                    Name = "Celerity",
                    Description = "When you block with Hardened Senses active, you gain 15% increased Attack Speed for 3 seconds.",
                    Tooltip = "rune/punish/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// When you block with Hardened Senses active, you gain 12874 increased Life regeneration for 2 seconds.
                /// </summary>
                public static Rune Rebirth = new Rune
                {
                    Index = 3,
                    Name = "Rebirth",
                    Description = "When you block with Hardened Senses active, you gain 12874 increased Life regeneration for 2 seconds.",
                    Tooltip = "rune/punish/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// When you block with Hardened Senses active, you deal 140% weapon damage as Holy to the attacker.
                /// </summary>
                public static Rune Retaliate = new Rune
                {
                    Index = 4,
                    Name = "Retaliate",
                    Description = "When you block with Hardened Senses active, you deal 140% weapon damage as Holy to the attacker.",
                    Tooltip = "rune/punish/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 0,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// When you block with Hardened Senses active, you gain 15% increased Critical Hit Chance for your next attack.
                /// </summary>
                public static Rune Fury = new Rune
                {
                    Index = 5,
                    Name = "Fury",
                    Description = "When you block with Hardened Senses active, you gain 15% increased Critical Hit Chance for your next attack.",
                    Tooltip = "rune/punish/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 0,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Shield Bash

                /// <summary>
                /// The shield shatters into other smaller fragments, hitting more enemies for 740% weapon damage plus 335% of your shield Block Chance as damage.
                /// </summary>
                public static Rune ShatteredShield = new Rune
                {
                    Index = 1,
                    Name = "Shattered Shield",
                    Description = "The shield shatters into other smaller fragments, hitting more enemies for 740% weapon damage plus 335% of your shield Block Chance as damage.",
                    Tooltip = "rune/shield-bash/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 1,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The targeted monster is stunned for 1.5 seconds. All other monsters hit are knocked back.
                /// </summary>
                public static Rune OneOnOne = new Rune
                {
                    Index = 2,
                    Name = "One on One",
                    Description = "The targeted monster is stunned for 1.5 seconds. All other monsters hit are knocked back.",
                    Tooltip = "rune/shield-bash/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(1.5),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Additional shields erupt from you in a cross formation. Enemies hit by any of the additional shields take 155% weapon damage plus 100% of your shield Block Chance as damage.
                /// </summary>
                public static Rune ShieldCross = new Rune
                {
                    Index = 3,
                    Name = "Shield Cross",
                    Description = "Additional shields erupt from you in a cross formation. Enemies hit by any of the additional shields take 155% weapon damage plus 100% of your shield Block Chance as damage.",
                    Tooltip = "rune/shield-bash/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 1,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Increases the weapon damage of Shield bash to 875%.
                /// </summary>
                public static Rune Crumble = new Rune
                {
                    Index = 4,
                    Name = "Crumble",
                    Description = "Increases the weapon damage of Shield bash to 875%.",
                    Tooltip = "rune/shield-bash/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 1,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Shield Bash will now deal 1320% weapon damage plus 500% shield Block Chance as damage. The range is reduced to 8 yards.
                /// </summary>
                public static Rune Pound = new Rune
                {
                    Index = 5,
                    Name = "Pound",
                    Description = "Shield Bash will now deal 1320% weapon damage plus 500% shield Block Chance as damage. The range is reduced to 8 yards.",
                    Tooltip = "rune/shield-bash/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 1,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Slash

                /// <summary>
                /// The slash becomes pure lightning and has a 25% chance to stun enemies for 2 seconds.
                /// </summary>
                public static Rune Electrify = new Rune
                {
                    Index = 1,
                    Name = "Electrify",
                    Description = "The slash becomes pure lightning and has a 25% chance to stun enemies for 2 seconds.",
                    Tooltip = "rune/slash/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 2,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Carve a larger area in front of you, increasing the number of enemies hit.
                /// </summary>
                public static Rune Carve = new Rune
                {
                    Index = 2,
                    Name = "Carve",
                    Description = "Carve a larger area in front of you, increasing the number of enemies hit.",
                    Tooltip = "rune/slash/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 2,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Slash gains 20% increased Critical Hit Chance.
                /// </summary>
                public static Rune Crush = new Rune
                {
                    Index = 3,
                    Name = "Crush",
                    Description = "Slash gains 20% increased Critical Hit Chance.",
                    Tooltip = "rune/slash/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 2,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Gain 1% increased Attack Speed for every enemy hit for 3 seconds. This effect stacks up to 10 times.
                /// </summary>
                public static Rune Zeal = new Rune
                {
                    Index = 4,
                    Name = "Zeal",
                    Description = "Gain 1% increased Attack Speed for every enemy hit for 3 seconds. This effect stacks up to 10 times.",
                    Tooltip = "rune/slash/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 2,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Gain 5% increased armor for each enemy hit. This effect stacks up to 5 times.
                /// </summary>
                public static Rune Guard = new Rune
                {
                    Index = 5,
                    Name = "Guard",
                    Description = "Gain 5% increased armor for each enemy hit. This effect stacks up to 5 times.",
                    Tooltip = "rune/slash/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 2,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Shield Glare

                /// <summary>
                /// Blinded enemies take 20% more damage for 4 seconds.
                /// </summary>
                public static Rune DivineVerdict = new Rune
                {
                    Index = 1,
                    Name = "Divine Verdict",
                    Description = "Blinded enemies take 20% more damage for 4 seconds.",
                    Tooltip = "rune/shield-glare/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Enemies caught in the glare have a 50% chance to be charmed and fight for you for 8 seconds.
                /// </summary>
                public static Rune Uncertainty = new Rune
                {
                    Index = 2,
                    Name = "Uncertainty",
                    Description = "Enemies caught in the glare have a 50% chance to be charmed and fight for you for 8 seconds.",
                    Tooltip = "rune/shield-glare/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Gain 9 Wrath for each enemy Blinded.
                /// </summary>
                public static Rune ZealousGlare = new Rune
                {
                    Index = 3,
                    Name = "Zealous Glare",
                    Description = "Gain 9 Wrath for each enemy Blinded.",
                    Tooltip = "rune/shield-glare/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 3,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Enemies with health lower than 25% have a 50% chance to explode when Blinded, dealing 60% weapon damage to enemies within 8 yards.
                /// </summary>
                public static Rune EmblazonedShield = new Rune
                {
                    Index = 4,
                    Name = "Emblazoned Shield",
                    Description = "Enemies with health lower than 25% have a 50% chance to explode when Blinded, dealing 60% weapon damage to enemies within 8 yards.",
                    Tooltip = "rune/shield-glare/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 3,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 8f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Enemies hit by the glare are Slowed by 80% for 6 seconds.
                /// </summary>
                public static Rune Subdue = new Rune
                {
                    Index = 5,
                    Name = "Subdue",
                    Description = "Enemies hit by the glare are Slowed by 80% for 6 seconds.",
                    Tooltip = "rune/shield-glare/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Sweep Attack

                /// <summary>
                /// Enemies hit by the attack will catch on fire for 120% weapon damage over 2 seconds.
                /// </summary>
                public static Rune BlazingSweep = new Rune
                {
                    Index = 1,
                    Name = "Blazing Sweep",
                    Description = "Enemies hit by the attack will catch on fire for 120% weapon damage over 2 seconds.",
                    Tooltip = "rune/sweep-attack/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 4,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Enemies hit by the sweep attack have a 50% chance to be tripped and Stunned for 2 seconds.
                /// </summary>
                public static Rune TripAttack = new Rune
                {
                    Index = 2,
                    Name = "Trip Attack",
                    Description = "Enemies hit by the sweep attack have a 50% chance to be tripped and Stunned for 2 seconds.",
                    Tooltip = "rune/sweep-attack/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 4,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Heal for 5364 Life for each enemy hit.
                /// </summary>
                public static Rune HolyShock = new Rune
                {
                    Index = 3,
                    Name = "Holy Shock",
                    Description = "Heal for 5364 Life for each enemy hit.",
                    Tooltip = "rune/sweep-attack/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 4,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Enemies caught in the sweep are pulled toward you.Sweep Attack&#39;s damage turns into Holy.
                /// </summary>
                public static Rune GatheringSweep = new Rune
                {
                    Index = 4,
                    Name = "Gathering Sweep",
                    Description = "Enemies caught in the sweep are pulled toward you.Sweep Attack&#39;s damage turns into Holy.",
                    Tooltip = "rune/sweep-attack/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 4,
                    ModifiedElement = Element.Holy,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Sweep Attack increases your Armor by 20% for 3 seconds.
                /// </summary>
                public static Rune InspiringSweep = new Rune
                {
                    Index = 5,
                    Name = "Inspiring Sweep",
                    Description = "Sweep Attack increases your Armor by 20% for 3 seconds.",
                    Tooltip = "rune/sweep-attack/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 4,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Iron Skin

                /// <summary>
                /// While active, your Thorns is increased by 300%.
                /// </summary>
                public static Rune ReflectiveSkin = new Rune
                {
                    Index = 1,
                    Name = "Reflective Skin",
                    Description = "While active, your Thorns is increased by 300%.",
                    Tooltip = "rune/iron-skin/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 5,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Increase the duration to 7 seconds.
                /// </summary>
                public static Rune SteelSkin = new Rune
                {
                    Index = 2,
                    Name = "Steel Skin",
                    Description = "Increase the duration to 7 seconds.",
                    Tooltip = "rune/iron-skin/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(7),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// When Iron Skin expires the metal explodes off, dealing 1400% weapon damage to enemies within 12 yards.
                /// </summary>
                public static Rune ExplosiveSkin = new Rune
                {
                    Index = 3,
                    Name = "Explosive Skin",
                    Description = "When Iron Skin expires the metal explodes off, dealing 1400% weapon damage to enemies within 12 yards.",
                    Tooltip = "rune/iron-skin/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 5,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 12f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Your metal skin is electrified, giving you a 20% chance to Stun enemies within 10 yards for 2 seconds.
                /// </summary>
                public static Rune ChargedUpIronSkin = new Rune
                {
                    Index = 4,
                    Name = "Charged Up",
                    Description = "Your metal skin is electrified, giving you a 20% chance to Stun enemies within 10 yards for 2 seconds.",
                    Tooltip = "rune/iron-skin/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// If you take damage while Iron Skin is active, your movement speed is increased by 60% for 5 seconds and you can move through enemies unhindered.
                /// </summary>
                public static Rune Flash = new Rune
                {
                    Index = 5,
                    Name = "Flash",
                    Description = "If you take damage while Iron Skin is active, your movement speed is increased by 60% for 5 seconds and you can move through enemies unhindered.",
                    Tooltip = "rune/iron-skin/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Provoke

                /// <summary>
                /// For each enemy successfully taunted, you gain 1073 additional Life on Hit for 5 seconds. 
                /// </summary>
                public static Rune Cleanse = new Rune
                {
                    Index = 1,
                    Name = "Cleanse",
                    Description = "For each enemy successfully taunted, you gain 1073 additional Life on Hit for 5 seconds. ",
                    Tooltip = "rune/provoke/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Provoke no longer taunts, but instead causes enemies to flee in Fear for 8 seconds.
                /// </summary>
                public static Rune FleeFool = new Rune
                {
                    Index = 2,
                    Name = "Flee Fool",
                    Description = "Provoke no longer taunts, but instead causes enemies to flee in Fear for 8 seconds.",
                    Tooltip = "rune/provoke/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Taunted enemies have their attack speed reduced by 50% and movement speed Slowed by 80% for 4 seconds.
                /// </summary>
                public static Rune TooScaredToRun = new Rune
                {
                    Index = 3,
                    Name = "Too Scared to Run",
                    Description = "Taunted enemies have their attack speed reduced by 50% and movement speed Slowed by 80% for 4 seconds.",
                    Tooltip = "rune/provoke/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// For 4 seconds after casting Provoke, any damage you deal will also deal 50% weapon damage as Lightning.
                /// </summary>
                public static Rune ChargedUpProvoke = new Rune
                {
                    Index = 4,
                    Name = "Charged Up",
                    Description = "For 4 seconds after casting Provoke, any damage you deal will also deal 50% weapon damage as Lightning.",
                    Tooltip = "rune/provoke/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Gain 50% increased Block Chance for 4 seconds after casting Provoke.
                /// </summary>
                public static Rune HitMe = new Rune
                {
                    Index = 5,
                    Name = "Hit Me",
                    Description = "Gain 50% increased Block Chance for 4 seconds after casting Provoke.",
                    Tooltip = "rune/provoke/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Smite

                /// <summary>
                /// The holy chains explode dealing 60% weapon damage as Holy to enemies within 3 yards.
                /// </summary>
                public static Rune Shatter = new Rune
                {
                    Index = 1,
                    Name = "Shatter",
                    Description = "The holy chains explode dealing 60% weapon damage as Holy to enemies within 3 yards.",
                    Tooltip = "rune/smite/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 7,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 3f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Enemies hit by the chains have a 20% chance to be Immobilized in place for 1 seconds.
                /// </summary>
                public static Rune Shackle = new Rune
                {
                    Index = 2,
                    Name = "Shackle",
                    Description = "Enemies hit by the chains have a 20% chance to be Immobilized in place for 1 seconds.",
                    Tooltip = "rune/smite/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Increase the number of additional enemies hit to 5.
                /// </summary>
                public static Rune Surge = new Rune
                {
                    Index = 3,
                    Name = "Surge",
                    Description = "Increase the number of additional enemies hit to 5.",
                    Tooltip = "rune/smite/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 7,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Gain 6437 increased Life regeneration for 2 seconds for every enemy hit by the chains. This effect stacks up to 4 times.
                /// </summary>
                public static Rune Reaping = new Rune
                {
                    Index = 4,
                    Name = "Reaping",
                    Description = "Gain 6437 increased Life regeneration for 2 seconds for every enemy hit by the chains. This effect stacks up to 4 times.",
                    Tooltip = "rune/smite/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The chains bind those they hit, causing them to share one another&#39;s fate. Enemies who share fate will be stunned for 2 seconds if they move 15 yards away from each other.
                /// </summary>
                public static Rune SharedFate = new Rune
                {
                    Index = 5,
                    Name = "Shared Fate",
                    Description = "The chains bind those they hit, causing them to share one another&#39;s fate. Enemies who share fate will be stunned for 2 seconds if they move 15 yards away from each other.",
                    Tooltip = "rune/smite/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Blessed Hammer

                /// <summary>
                /// The hammer is engulfed in fire and has a 25% chance to scorch the ground over which it passes. Enemies who pass through the scorched ground take 330% weapon damage as Fire per second.
                /// </summary>
                public static Rune BurningWrath = new Rune
                {
                    Index = 1,
                    Name = "Burning Wrath",
                    Description = "The hammer is engulfed in fire and has a 25% chance to scorch the ground over which it passes. Enemies who pass through the scorched ground take 330% weapon damage as Fire per second.",
                    Tooltip = "rune/blessed-hammer/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 8,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The hammer is charged with lightning that occasionally arcs between you and the hammer as it spirals through the air, dealing 60% weapon damage as Lightning to enemies caught in the arcs. 
                /// </summary>
                public static Rune Thunderstruck = new Rune
                {
                    Index = 2,
                    Name = "Thunderstruck",
                    Description = "The hammer is charged with lightning that occasionally arcs between you and the hammer as it spirals through the air, dealing 60% weapon damage as Lightning to enemies caught in the arcs. ",
                    Tooltip = "rune/blessed-hammer/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 8,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// When the hammer hits an enemy there is a 50% chance that a new hammer will be created at the location of the enemy hit. This can only occur once per hammer.
                /// </summary>
                public static Rune Limitless = new Rune
                {
                    Index = 3,
                    Name = "Limitless",
                    Description = "When the hammer hits an enemy there is a 50% chance that a new hammer will be created at the location of the enemy hit. This can only occur once per hammer.",
                    Tooltip = "rune/blessed-hammer/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 8,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The hammer Slows enemies it passes through and has a 35% chance to explode on impact, dealing 460% weapon damage as Physical and Stunning enemies within 6 yards for 1 second.
                /// </summary>
                public static Rune BruteForce = new Rune
                {
                    Index = 4,
                    Name = "Brute Force",
                    Description = "The hammer Slows enemies it passes through and has a 35% chance to explode on impact, dealing 460% weapon damage as Physical and Stunning enemies within 6 yards for 1 second.",
                    Tooltip = "rune/blessed-hammer/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 8,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 6f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The Hammer now orbits you as you move.
                /// </summary>
                public static Rune Dominion = new Rune
                {
                    Index = 5,
                    Name = "Dominion",
                    Description = "The Hammer now orbits you as you move.",
                    Tooltip = "rune/blessed-hammer/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 8,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Steed Charge

                /// <summary>
                /// The war horse deals 500% of your Thorns every second to enemies through which you ride.
                /// </summary>
                public static Rune SpikedBarding = new Rune
                {
                    Index = 1,
                    Name = "Spiked Barding",
                    Description = "The war horse deals 500% of your Thorns every second to enemies through which you ride.",
                    Tooltip = "rune/steed-charge/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 9,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The war horse is engulfed in righteous fire, scorching all who cross its path for 550% weapon damage per second as Fire.
                /// </summary>
                public static Rune Nightmare = new Rune
                {
                    Index = 2,
                    Name = "Nightmare",
                    Description = "The war horse is engulfed in righteous fire, scorching all who cross its path for 550% weapon damage per second as Fire.",
                    Tooltip = "rune/steed-charge/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 9,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// While riding the war horse, you recover 15% of your maximum Life.
                /// </summary>
                public static Rune Rejuvenation = new Rune
                {
                    Index = 3,
                    Name = "Rejuvenation",
                    Description = "While riding the war horse, you recover 15% of your maximum Life.",
                    Tooltip = "rune/steed-charge/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 9,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Increase the duration to 3 seconds.
                /// </summary>
                public static Rune Endurance = new Rune
                {
                    Index = 4,
                    Name = "Endurance",
                    Description = "Increase the duration to 3 seconds.",
                    Tooltip = "rune/steed-charge/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 9,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Bind 5 monsters near you with chains and drag them as you ride, dealing 185% weapon damage as Holy every second.
                /// </summary>
                public static Rune DrawAndQuarter = new Rune
                {
                    Index = 5,
                    Name = "Draw and Quarter",
                    Description = "Bind 5 monsters near you with chains and drag them as you ride, dealing 185% weapon damage as Holy every second.",
                    Tooltip = "rune/steed-charge/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 9,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Laws of Valor

                /// <summary>
                /// Active: Empowering the Law also increases your Life on Hit by 21457.
                /// </summary>
                public static Rune Invincible = new Rune
                {
                    Index = 1,
                    Name = "Invincible",
                    Description = "Active: Empowering the Law also increases your Life on Hit by 21457.",
                    Tooltip = "rune/laws-of-valor/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 10,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: Empowering the Law also grants you a 100% chance to Stun all enemies within 10 yards for 5 seconds.
                /// </summary>
                public static Rune FrozenInTerror = new Rune
                {
                    Index = 2,
                    Name = "Frozen in Terror",
                    Description = "Active: Empowering the Law also grants you a 100% chance to Stun all enemies within 10 yards for 5 seconds.",
                    Tooltip = "rune/laws-of-valor/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 10,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: Empowering the Law also increases your Critical Hit Damage by 50%.
                /// </summary>
                public static Rune Critical = new Rune
                {
                    Index = 3,
                    Name = "Critical",
                    Description = "Active: Empowering the Law also increases your Critical Hit Damage by 50%.",
                    Tooltip = "rune/laws-of-valor/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 10,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: Empowering the law also reduces the Wrath cost of all skills by 50% for 5 seconds.
                /// </summary>
                public static Rune UnstoppableForce = new Rune
                {
                    Index = 4,
                    Name = "Unstoppable Force",
                    Description = "Active: Empowering the law also reduces the Wrath cost of all skills by 50% for 5 seconds.",
                    Tooltip = "rune/laws-of-valor/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 10,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: While the Law is empowered, each enemy killed increases the duration by 1 second, up to a maximum of 10 seconds of increased time.
                /// </summary>
                public static Rune AnsweredPrayer = new Rune
                {
                    Index = 5,
                    Name = "Answered Prayer",
                    Description = "Active: While the Law is empowered, each enemy killed increases the duration by 1 second, up to a maximum of 10 seconds of increased time.",
                    Tooltip = "rune/laws-of-valor/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 10,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Justice

                /// <summary>
                /// The hammer is charged with lightning and explodes on impact, dealing 60% weapon damage as Lightning to all enemies within 10 yards. Enemies caught in the explosion have a 20% chance to be stunned for 1 seconds.
                /// </summary>
                public static Rune Burst = new Rune
                {
                    Index = 1,
                    Name = "Burst",
                    Description = "The hammer is charged with lightning and explodes on impact, dealing 60% weapon damage as Lightning to all enemies within 10 yards. Enemies caught in the explosion have a 20% chance to be stunned for 1 seconds.",
                    Tooltip = "rune/justice/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 11,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// When the hammer hits an enemy, there is an 100% chance it will crack into 2 smaller hammers that fly out and deal 245% weapon damage as Holy.
                /// </summary>
                public static Rune Crack = new Rune
                {
                    Index = 2,
                    Name = "Crack",
                    Description = "When the hammer hits an enemy, there is an 100% chance it will crack into 2 smaller hammers that fly out and deal 245% weapon damage as Holy.",
                    Tooltip = "rune/justice/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 11,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The hammer seeks out nearby targets and deal 335% weapon damage.
                /// </summary>
                public static Rune HammerOfPursuit = new Rune
                {
                    Index = 3,
                    Name = "Hammer of Pursuit",
                    Description = "The hammer seeks out nearby targets and deal 335% weapon damage.",
                    Tooltip = "rune/justice/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 11,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Hurl a sword of justice at your enemies. When the sword hits an enemy, gain 5% increased movement speed for 3 seconds. This effect stacks up to 3 times.
                /// </summary>
                public static Rune SwordOfJustice = new Rune
                {
                    Index = 4,
                    Name = "Sword of Justice",
                    Description = "Hurl a sword of justice at your enemies. When the sword hits an enemy, gain 5% increased movement speed for 3 seconds. This effect stacks up to 3 times.",
                    Tooltip = "rune/justice/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 11,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Throw a bolt of holy power that heals you and your allies for 2146 - 3219 Life when it hits an enemy.
                /// </summary>
                public static Rune HolyBolt = new Rune
                {
                    Index = 5,
                    Name = "Holy Bolt",
                    Description = "Throw a bolt of holy power that heals you and your allies for 2146 - 3219 Life when it hits an enemy.",
                    Tooltip = "rune/justice/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 11,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Consecration

                /// <summary>
                /// Increase the radius of the consecrated ground to 24 yards and increase the amount you and your allies heal for to 48278 Life per second.
                /// </summary>
                public static Rune BathedInLight = new Rune
                {
                    Index = 1,
                    Name = "Bathed in Light",
                    Description = "Increase the radius of the consecrated ground to 24 yards and increase the amount you and your allies heal for to 48278 Life per second.",
                    Tooltip = "rune/consecration/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 12,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The consecrated ground becomes covered in nails. Enemies that walk into the area take 100% of your Thorns damage every second.
                /// </summary>
                public static Rune BedOfNails = new Rune
                {
                    Index = 2,
                    Name = "Bed of Nails",
                    Description = "The consecrated ground becomes covered in nails. Enemies that walk into the area take 100% of your Thorns damage every second.",
                    Tooltip = "rune/consecration/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 12,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The edge of the consecrated ground is surrounded by a sacred shield, preventing enemies from moving through it.The duration of the consecration is reduced to 5 seconds.
                /// </summary>
                public static Rune AegisPurgatory = new Rune
                {
                    Index = 3,
                    Name = "Aegis Purgatory",
                    Description = "The edge of the consecrated ground is surrounded by a sacred shield, preventing enemies from moving through it.The duration of the consecration is reduced to 5 seconds.",
                    Tooltip = "rune/consecration/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Enemies standing on consecrated ground take 155% weapon damage as Fire per second.
                /// </summary>
                public static Rune ShatteredGround = new Rune
                {
                    Index = 4,
                    Name = "Shattered Ground",
                    Description = "Enemies standing on consecrated ground take 155% weapon damage as Fire per second.",
                    Tooltip = "rune/consecration/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 12,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Enemies standing on consecrated ground have a 100% chance to be Feared for 3 seconds.
                /// </summary>
                public static Rune Fearful = new Rune
                {
                    Index = 5,
                    Name = "Fearful",
                    Description = "Enemies standing on consecrated ground have a 100% chance to be Feared for 3 seconds.",
                    Tooltip = "rune/consecration/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Laws of Justice

                /// <summary>
                /// Active: Empowering the Law also redirects 20% of the damage taken by your allies to you for the next 5 seconds.
                /// </summary>
                public static Rune ProtectTheInnocent = new Rune
                {
                    Index = 1,
                    Name = "Protect the Innocent",
                    Description = "Active: Empowering the Law also redirects 20% of the damage taken by your allies to you for the next 5 seconds.",
                    Tooltip = "rune/laws-of-justice/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 13,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: Empowering the Law also increases Armor for you and your allies by for 5 seconds.
                /// </summary>
                public static Rune ImmovableObject = new Rune
                {
                    Index = 2,
                    Name = "Immovable Object",
                    Description = "Active: Empowering the Law also increases Armor for you and your allies by for 5 seconds.",
                    Tooltip = "rune/laws-of-justice/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: Empowering the Law also surrounds you and your allies with shields of faith for 5 seconds. The shields absorb up to 26821 damage.
                /// </summary>
                public static Rune FaithsArmor = new Rune
                {
                    Index = 3,
                    Name = "Faith's Armor",
                    Description = "Active: Empowering the Law also surrounds you and your allies with shields of faith for 5 seconds. The shields absorb up to 26821 damage.",
                    Tooltip = "rune/laws-of-justice/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: While the Law is empowered, any enemy who attacks you or your allies will have their damage reduced by 15% for 5 seconds, stacking up to a maximum of 60%.
                /// </summary>
                public static Rune DecayingStrength = new Rune
                {
                    Index = 4,
                    Name = "Decaying Strength",
                    Description = "Active: While the Law is empowered, any enemy who attacks you or your allies will have their damage reduced by 15% for 5 seconds, stacking up to a maximum of 60%.",
                    Tooltip = "rune/laws-of-justice/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: Empowering the Law also grants immunity to control impairing effects to you and your allies for 5 seconds.
                /// </summary>
                public static Rune Bravery = new Rune
                {
                    Index = 5,
                    Name = "Bravery",
                    Description = "Active: Empowering the Law also grants immunity to control impairing effects to you and your allies for 5 seconds.",
                    Tooltip = "rune/laws-of-justice/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Falling Sword

                /// <summary>
                /// The ground you fall on becomes superheated for 6 seconds, dealing 310% weapon damage as Fire per second to all enemies who pass over it.
                /// </summary>
                public static Rune Superheated = new Rune
                {
                    Index = 1,
                    Name = "Superheated",
                    Description = "The ground you fall on becomes superheated for 6 seconds, dealing 310% weapon damage as Fire per second to all enemies who pass over it.",
                    Tooltip = "rune/falling-sword/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 14,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// You build a storm of lightning as you fall which covers the area you land on for 5 seconds. Lightning strikes random enemies under the cloud, dealing 605% weapon damage as Lightning and Stunning them for 2 seconds.
                /// </summary>
                public static Rune PartTheClouds = new Rune
                {
                    Index = 2,
                    Name = "Part the Clouds",
                    Description = "You build a storm of lightning as you fall which covers the area you land on for 5 seconds. Lightning strikes random enemies under the cloud, dealing 605% weapon damage as Lightning and Stunning them for 2 seconds.",
                    Tooltip = "rune/falling-sword/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 14,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// You land with such force that 3 Avatars of the Order are summoned forth to fight by your side for 5 seconds. Each Avatar attacks for 280% of your weapon damage as Physical.
                /// </summary>
                public static Rune RiseBrothers = new Rune
                {
                    Index = 3,
                    Name = "Rise Brothers",
                    Description = "You land with such force that 3 Avatars of the Order are summoned forth to fight by your side for 5 seconds. Each Avatar attacks for 280% of your weapon damage as Physical.",
                    Tooltip = "rune/falling-sword/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 14,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Reduce the cooldown by 1 seconds for each enemy hit by Falling Sword. The cooldown cannot be reduced below 10 seconds.
                /// </summary>
                public static Rune RapidDescent = new Rune
                {
                    Index = 4,
                    Name = "Rapid Descent",
                    Description = "Reduce the cooldown by 1 seconds for each enemy hit by Falling Sword. The cooldown cannot be reduced below 10 seconds.",
                    Tooltip = "rune/falling-sword/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 14,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// A flurry of swords is summoned at the impact location, dealing 230% weapon damage as Holy, hurling enemies around and incapacitating them for 5 seconds.
                /// </summary>
                public static Rune Flurry = new Rune
                {
                    Index = 5,
                    Name = "Flurry",
                    Description = "A flurry of swords is summoned at the impact location, dealing 230% weapon damage as Holy, hurling enemies around and incapacitating them for 5 seconds.",
                    Tooltip = "rune/falling-sword/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 14,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Blessed Shield

                /// <summary>
                /// The shield becomes charged with lightning and has a 25% chance to Stun the first enemy hit for 2 seconds. Each enemy hit after the first has a 5% reduced chance to be Stunned.
                /// </summary>
                public static Rune StaggeringShield = new Rune
                {
                    Index = 1,
                    Name = "Staggering Shield",
                    Description = "The shield becomes charged with lightning and has a 25% chance to Stun the first enemy hit for 2 seconds. Each enemy hit after the first has a 5% reduced chance to be Stunned.",
                    Tooltip = "rune/blessed-shield/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The shield erupts in flames and has a 33% chance to explode on impact, dealing 310% weapon damage as Fire to all enemies within 10 yards.
                /// </summary>
                public static Rune Combust = new Rune
                {
                    Index = 2,
                    Name = "Combust",
                    Description = "The shield erupts in flames and has a 33% chance to explode on impact, dealing 310% weapon damage as Fire to all enemies within 10 yards.",
                    Tooltip = "rune/blessed-shield/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 15,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// When the shield hits an enemy, your Armor is increased by 5% and Life regeneration is increased by 5% for 4 seconds.
                /// </summary>
                public static Rune DivineAegis = new Rune
                {
                    Index = 3,
                    Name = "Divine Aegis",
                    Description = "When the shield hits an enemy, your Armor is increased by 5% and Life regeneration is increased by 5% for 4 seconds.",
                    Tooltip = "rune/blessed-shield/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// When the shield hits an enemy, it splits into 3 small fragments that bounce between nearby enemies, dealing 170% weapon damage as Holy to all enemies hit.
                /// </summary>
                public static Rune ShatteringThrow = new Rune
                {
                    Index = 4,
                    Name = "Shattering Throw",
                    Description = "When the shield hits an enemy, it splits into 3 small fragments that bounce between nearby enemies, dealing 170% weapon damage as Holy to all enemies hit.",
                    Tooltip = "rune/blessed-shield/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 15,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The shield no longer bounces, but pierces through all enemies with a 50% chance to knock them aside.
                /// </summary>
                public static Rune PiercingShield = new Rune
                {
                    Index = 5,
                    Name = "Piercing Shield",
                    Description = "The shield no longer bounces, but pierces through all enemies with a 50% chance to knock them aside.",
                    Tooltip = "rune/blessed-shield/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 15,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Condemn

                /// <summary>
                /// As the explosion charges up, it sucks in enemies. The closer it is to exploding, the more enemies it sucks in.
                /// </summary>
                public static Rune Vacuum = new Rune
                {
                    Index = 1,
                    Name = "Vacuum",
                    Description = "As the explosion charges up, it sucks in enemies. The closer it is to exploding, the more enemies it sucks in.",
                    Tooltip = "rune/condemn/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 16,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The explosion now unleashes instantly.
                /// </summary>
                public static Rune Unleashed = new Rune
                {
                    Index = 2,
                    Name = "Unleashed",
                    Description = "The explosion now unleashes instantly.",
                    Tooltip = "rune/condemn/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 16,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Reduce the cooldown by 1 second for each enemy hit by the explosion.
                /// </summary>
                public static Rune EternalRetaliation = new Rune
                {
                    Index = 3,
                    Name = "Eternal Retaliation",
                    Description = "Reduce the cooldown by 1 second for each enemy hit by the explosion.",
                    Tooltip = "rune/condemn/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 16,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Increase the damage radius to 20 yards.
                /// </summary>
                public static Rune ShatteringExplosion = new Rune
                {
                    Index = 4,
                    Name = "Shattering Explosion",
                    Description = "Increase the damage radius to 20 yards.",
                    Tooltip = "rune/condemn/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 16,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// 50% of all damage taken while the explosion is building is added to the damage of the explosion.
                /// </summary>
                public static Rune Reciprocate = new Rune
                {
                    Index = 5,
                    Name = "Reciprocate",
                    Description = "50% of all damage taken while the explosion is building is added to the damage of the explosion.",
                    Tooltip = "rune/condemn/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 16,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Judgment

                /// <summary>
                /// For every enemy upon whom you pass judgment, you heal for 2682 Life per second for 3 seconds.
                /// </summary>
                public static Rune Penitence = new Rune
                {
                    Index = 1,
                    Name = "Penitence",
                    Description = "For every enemy upon whom you pass judgment, you heal for 2682 Life per second for 3 seconds.",
                    Tooltip = "rune/judgment/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// All enemies are drawn toward the center of the judged area.
                /// </summary>
                public static Rune MassVerdict = new Rune
                {
                    Index = 2,
                    Name = "Mass Verdict",
                    Description = "All enemies are drawn toward the center of the judged area.",
                    Tooltip = "rune/judgment/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 17,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Increase the duration of the Immobilize to 10 seconds.
                /// </summary>
                public static Rune Deliberation = new Rune
                {
                    Index = 3,
                    Name = "Deliberation",
                    Description = "Increase the duration of the Immobilize to 10 seconds.",
                    Tooltip = "rune/judgment/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(10),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Damage dealt to judged enemies has an 8% increased chance to be a Critical Hit.
                /// </summary>
                public static Rune Resolved = new Rune
                {
                    Index = 4,
                    Name = "Resolved",
                    Description = "Damage dealt to judged enemies has an 8% increased chance to be a Critical Hit.",
                    Tooltip = "rune/judgment/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 17,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Enemies in the judged area deal 40% reduced damage for 6 seconds.
                /// </summary>
                public static Rune Debilitate = new Rune
                {
                    Index = 5,
                    Name = "Debilitate",
                    Description = "Enemies in the judged area deal 40% reduced damage for 6 seconds.",
                    Tooltip = "rune/judgment/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Laws of Hope

                /// <summary>
                /// Active: Empowering the Law also increases movement speed for you and your allies by 50%, and allows everyone affected to run through enemies unimpeded.
                /// </summary>
                public static Rune WingsOfAngels = new Rune
                {
                    Index = 1,
                    Name = "Wings of Angels",
                    Description = "Active: Empowering the Law also increases movement speed for you and your allies by 50%, and allows everyone affected to run through enemies unimpeded.",
                    Tooltip = "rune/laws-of-hope/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 18,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: Empowering the Law also increases the maximum Life of you and your allies by 10%.
                /// </summary>
                public static Rune EternalHope = new Rune
                {
                    Index = 2,
                    Name = "Eternal Hope",
                    Description = "Active: Empowering the Law also increases the maximum Life of you and your allies by 10%.",
                    Tooltip = "rune/laws-of-hope/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 18,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: Empowering the Law also reduces all Physical damage taken by 25%.
                /// </summary>
                public static Rune HopefulCry = new Rune
                {
                    Index = 3,
                    Name = "Hopeful Cry",
                    Description = "Active: Empowering the Law also reduces all Physical damage taken by 25%.",
                    Tooltip = "rune/laws-of-hope/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 18,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: Empowering the Law also heals you and your allies for 1073 Life for every point of Wrath that you spend. 
                /// </summary>
                public static Rune FaithsReward = new Rune
                {
                    Index = 4,
                    Name = "Faith's Reward",
                    Description = "Active: Empowering the Law also heals you and your allies for 1073 Life for every point of Wrath that you spend. ",
                    Tooltip = "rune/laws-of-hope/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 18,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Active: Empowering the Law also reduces all non-Physical damage taken by 25%.
                /// </summary>
                public static Rune PromiseOfFaith = new Rune
                {
                    Index = 5,
                    Name = "Promise of Faith",
                    Description = "Active: Empowering the Law also reduces all non-Physical damage taken by 25%.",
                    Tooltip = "rune/laws-of-hope/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 18,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Akarat's Champion

                /// <summary>
                /// Dealing damage burns enemies with the power of Akarat, dealing 460% weapon damage as Fire over 3 seconds.
                /// </summary>
                public static Rune FireStarter = new Rune
                {
                    Index = 1,
                    Name = "Fire Starter",
                    Description = "Dealing damage burns enemies with the power of Akarat, dealing 460% weapon damage as Fire over 3 seconds.",
                    Tooltip = "rune/akarats-champion/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 19,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Increases the bonus Wrath regeneration from Akarat&#39;s Champion to 10.
                /// </summary>
                public static Rune EmbodimentOfPower = new Rune
                {
                    Index = 2,
                    Name = "Embodiment of Power",
                    Description = "Increases the bonus Wrath regeneration from Akarat&#39;s Champion to 10.",
                    Tooltip = "rune/akarats-champion/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 19,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Using Akarat&#39;s Champion reduces the remaining cooldown of your other abilities by 12 seconds.
                /// </summary>
                public static Rune Rally = new Rune
                {
                    Index = 3,
                    Name = "Rally",
                    Description = "Using Akarat&#39;s Champion reduces the remaining cooldown of your other abilities by 12 seconds.",
                    Tooltip = "rune/akarats-champion/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 19,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Gain 150% additional Armor while Akarat&#39;s Champion is active.The first time you take fatal damage while Akarat&#39;s Champion is active, you will be returned to full health.
                /// </summary>
                public static Rune Prophet = new Rune
                {
                    Index = 4,
                    Name = "Prophet",
                    Description = "Gain 150% additional Armor while Akarat&#39;s Champion is active.The first time you take fatal damage while Akarat&#39;s Champion is active, you will be returned to full health.",
                    Tooltip = "rune/akarats-champion/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 19,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Gain 15% increased attack speed while Akarat&#39;s Champion is active.
                /// </summary>
                public static Rune Hasteful = new Rune
                {
                    Index = 5,
                    Name = "Hasteful",
                    Description = "Gain 15% increased attack speed while Akarat&#39;s Champion is active.",
                    Tooltip = "rune/akarats-champion/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 19,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Fist of the Heavens

                /// <summary>
                /// The holy bolts crackle with holy lightning and zap enemies within 18 yards as they travel, dealing 40% weapon damage as Holy.
                /// </summary>
                public static Rune DivineWell = new Rune
                {
                    Index = 1,
                    Name = "Divine Well",
                    Description = "The holy bolts crackle with holy lightning and zap enemies within 18 yards as they travel, dealing 40% weapon damage as Holy.",
                    Tooltip = "rune/fist-of-the-heavens/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 20,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 18f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Summon a fiery storm that covers a 8 yard radius for 5 seconds, dealing 100% weapon damage as Fire every second to enemies who pass underneath it.
                /// </summary>
                public static Rune HeavensTempest = new Rune
                {
                    Index = 2,
                    Name = "Heaven's Tempest",
                    Description = "Summon a fiery storm that covers a 8 yard radius for 5 seconds, dealing 100% weapon damage as Fire every second to enemies who pass underneath it.",
                    Tooltip = "rune/fist-of-the-heavens/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 20,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Creates a fissure of lightning energy that deals 410% weapon damage as Lightning over 5 seconds to nearby enemies. If there is another fissure nearby, lightning will arc between them dealing an additional 135% weapon damage as Lightning with each arc.
                /// </summary>
                public static Rune Fissure = new Rune
                {
                    Index = 3,
                    Name = "Fissure",
                    Description = "Creates a fissure of lightning energy that deals 410% weapon damage as Lightning over 5 seconds to nearby enemies. If there is another fissure nearby, lightning will arc between them dealing an additional 135% weapon damage as Lightning with each arc.",
                    Tooltip = "rune/fist-of-the-heavens/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 20,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The bolt detonates with a shockwave on impact, causing all enemies hit to be knocked away from the blast and Slowed by 80% for 4 seconds.
                /// </summary>
                public static Rune Reverberation = new Rune
                {
                    Index = 4,
                    Name = "Reverberation",
                    Description = "The bolt detonates with a shockwave on impact, causing all enemies hit to be knocked away from the blast and Slowed by 80% for 4 seconds.",
                    Tooltip = "rune/fist-of-the-heavens/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 20,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Hurl a fist of Holy power that pierces through your enemies, dealing 270% weapon damage as Holy, and exploding at your target, dealing 435% weapon damage as Holy to enemies within 8 yards.The explosion creates 6 piercing charged bolts that crawl outward, dealing 185% weapon damage as Holy to enemies through whom they pass.
                /// </summary>
                public static Rune Retribution = new Rune
                {
                    Index = 5,
                    Name = "Retribution",
                    Description = "Hurl a fist of Holy power that pierces through your enemies, dealing 270% weapon damage as Holy, and exploding at your target, dealing 435% weapon damage as Holy to enemies within 8 yards.The explosion creates 6 piercing charged bolts that crawl outward, dealing 185% weapon damage as Holy to enemies through whom they pass.",
                    Tooltip = "rune/fist-of-the-heavens/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 20,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 8f,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Phalanx

                /// <summary>
                /// The summoned avatars no longer march forward, but will wield bows and attack enemies, dealing 185% weapon damage. These bowmen follow you as you move for 5 seconds.The Bowmen can only be summoned once every 15 seconds.
                /// </summary>
                public static Rune Bowmen = new Rune
                {
                    Index = 1,
                    Name = "Bowmen",
                    Description = "The summoned avatars no longer march forward, but will wield bows and attack enemies, dealing 185% weapon damage. These bowmen follow you as you move for 5 seconds.The Bowmen can only be summoned once every 15 seconds.",
                    Tooltip = "rune/phalanx/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 21,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The summoned avatars charge the target and perform a shield bash, dealing an additional 180% weapon damage to enemies at that location.
                /// </summary>
                public static Rune ShieldCharge = new Rune
                {
                    Index = 2,
                    Name = "Shield Charge",
                    Description = "The summoned avatars charge the target and perform a shield bash, dealing an additional 180% weapon damage to enemies at that location.",
                    Tooltip = "rune/phalanx/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 21,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Summon warhorses that deal 490% weapon damage and have a 30% chance to Stun enemies for 2 seconds.
                /// </summary>
                public static Rune Stampede = new Rune
                {
                    Index = 3,
                    Name = "Stampede",
                    Description = "Summon warhorses that deal 490% weapon damage and have a 30% chance to Stun enemies for 2 seconds.",
                    Tooltip = "rune/phalanx/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 21,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The avatars no longer walk forward, but stand at the summoned location, blocking all enemies from moving through.The Avatars can only be summoned once every 15 seconds.
                /// </summary>
                public static Rune ShieldBearers = new Rune
                {
                    Index = 4,
                    Name = "Shield Bearers",
                    Description = "The avatars no longer walk forward, but stand at the summoned location, blocking all enemies from moving through.The Avatars can only be summoned once every 15 seconds.",
                    Tooltip = "rune/phalanx/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 21,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Instead of sending the avatars out away from you, you summon 2 Avatars of the Order to protect you and fight by your side for 10 seconds. Each Avatar will attack for 560% of your weapon damage as Physical.The Avatars can only be summoned once every 30 seconds.
                /// </summary>
                public static Rune Bodyguard = new Rune
                {
                    Index = 5,
                    Name = "Bodyguard",
                    Description = "Instead of sending the avatars out away from you, you summon 2 Avatars of the Order to protect you and fight by your side for 10 seconds. Each Avatar will attack for 560% of your weapon damage as Physical.The Avatars can only be summoned once every 30 seconds.",
                    Tooltip = "rune/phalanx/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 21,
                    ModifiedDuration = TimeSpan.FromSeconds(10),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Heaven's Fury

                /// <summary>
                /// The ground touched by the ray becomes blessed, scorching it and dealing 1550% weapon damage over 5 seconds to enemies who walks through.
                /// </summary>
                public static Rune BlessedGround = new Rune
                {
                    Index = 1,
                    Name = "Blessed Ground",
                    Description = "The ground touched by the ray becomes blessed, scorching it and dealing 1550% weapon damage over 5 seconds to enemies who walks through.",
                    Tooltip = "rune/heavens-fury/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The ray of Holy power grows to encompass 12 yards, dealing 2766% weapon damage as Holy over 6 seconds to enemies caught within it.
                /// </summary>
                public static Rune Ascendancy = new Rune
                {
                    Index = 2,
                    Name = "Ascendancy",
                    Description = "The ray of Holy power grows to encompass 12 yards, dealing 2766% weapon damage as Holy over 6 seconds to enemies caught within it.",
                    Tooltip = "rune/heavens-fury/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// The ray splits into 3 smaller beams, each dealing 1980% weapon damage as Holy over 6 seconds.
                /// </summary>
                public static Rune SplitFury = new Rune
                {
                    Index = 3,
                    Name = "Split Fury",
                    Description = "The ray splits into 3 smaller beams, each dealing 1980% weapon damage as Holy over 6 seconds.",
                    Tooltip = "rune/heavens-fury/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Ground touched by the ray pulses with power for 6 seconds, stopping enemies who try to pass over it.
                /// </summary>
                public static Rune ThouShaltNotPass = new Rune
                {
                    Index = 4,
                    Name = "Thou Shalt Not Pass",
                    Description = "Ground touched by the ray pulses with power for 6 seconds, stopping enemies who try to pass over it.",
                    Tooltip = "rune/heavens-fury/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Call down a furious ray of Holy power that is focused through you in a beam across the battlefield, dealing 960% weapon damage as Holy to all enemies it hits.The cooldown is removed. Now costs 40 Wrath.
                /// </summary>
                public static Rune FiresOfHeaven = new Rune
                {
                    Index = 5,
                    Name = "Fires of Heaven",
                    Description = "Call down a furious ray of Holy power that is focused through you in a beam across the battlefield, dealing 960% weapon damage as Holy to all enemies it hits.The cooldown is removed. Now costs 40 Wrath.",
                    Tooltip = "rune/heavens-fury/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 22,
                    ModifiedCost = 40,
                    ModifiedElement = Element.Holy,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Crusader
                };
                #endregion

                #region Skill: Bombardment

                /// <summary>
                /// In place of the burning spheres, barrels of spikes are hurled. Damage of each barrel is increased by 200% of your Thorns.
                /// </summary>
                public static Rune BarrelsOfSpikes = new Rune
                {
                    Index = 1,
                    Name = "Barrels of Spikes",
                    Description = "In place of the burning spheres, barrels of spikes are hurled. Damage of each barrel is increased by 200% of your Thorns.",
                    Tooltip = "rune/bombardment/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 23,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Each impact has a 100% Critical Hit Chance.
                /// </summary>
                public static Rune Annihilate = new Rune
                {
                    Index = 2,
                    Name = "Annihilate",
                    Description = "Each impact has a 100% Critical Hit Chance.",
                    Tooltip = "rune/bombardment/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 23,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Each impact scatters 2 mines onto the battlefield that explode when enemies walk near them, dealing 160% weapon damage as Fire to all enemies within 10 yards.
                /// </summary>
                public static Rune MineField = new Rune
                {
                    Index = 3,
                    Name = "Mine Field",
                    Description = "Each impact scatters 2 mines onto the battlefield that explode when enemies walk near them, dealing 160% weapon damage as Fire to all enemies within 10 yards.",
                    Tooltip = "rune/bombardment/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 23,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// A single, much larger ball of explosive pitch is hurled at the targeted location dealing 3320% weapon damage to all enemies within 18 yards.
                /// </summary>
                public static Rune ImpactfulBombardment = new Rune
                {
                    Index = 4,
                    Name = "Impactful Bombardment",
                    Description = "A single, much larger ball of explosive pitch is hurled at the targeted location dealing 3320% weapon damage to all enemies within 18 yards.",
                    Tooltip = "rune/bombardment/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 23,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 18f,
                    Class = ActorClass.Crusader
                };

                /// <summary>
                /// Instead of randomly finding targets nearby, the bombardment will continue to fall on your initial target.
                /// </summary>
                public static Rune Targeted = new Rune
                {
                    Index = 5,
                    Name = "Targeted",
                    Description = "Instead of randomly finding targets nearby, the bombardment will continue to fall on your initial target.",
                    Tooltip = "rune/bombardment/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 23,
                    Class = ActorClass.Crusader
                };
                #endregion

            }
            public class WitchDoctor : FieldCollection<WitchDoctor, Rune>
            {

                /// <summary>
                /// No Rune
                /// </summary>
                public static Rune None = new Rune
                {
                    Index = 0,
                    Name = "None",
                    Description = "No Rune Selected",
                    Tooltip = string.Empty,
                    TypeId = string.Empty,
                    RuneIndex = -1,
                    Class = ActorClass.Witchdoctor
                };

                #region Skill: Poison Dart

                /// <summary>
                /// Shoot 3 Poison Darts that deal 110% weapon damage as Poison each.
                /// </summary>
                public static Rune Splinters = new Rune
                {
                    Index = 1,
                    Name = "Splinters",
                    Description = "Shoot 3 Poison Darts that deal 110% weapon damage as Poison each.",
                    Tooltip = "rune/poison-dart/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 0,
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Shoot a Cold dart that will Slow the enemy by 60% for 2 seconds.
                /// </summary>
                public static Rune NumbingDart = new Rune
                {
                    Index = 2,
                    Name = "Numbing Dart",
                    Description = "Shoot a Cold dart that will Slow the enemy by 60% for 2 seconds.",
                    Tooltip = "rune/poison-dart/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 50 Mana every time a Poison Dart hits an enemy. Converts the damage type to Physical.
                /// </summary>
                public static Rune SpinedDart = new Rune
                {
                    Index = 3,
                    Name = "Spined Dart",
                    Description = "Gain 50 Mana every time a Poison Dart hits an enemy. Converts the damage type to Physical.",
                    Tooltip = "rune/poison-dart/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 0,
                    ModifiedElement = Element.Physical,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Ignite the dart, dealing 565% weapon damage as Fire over 4 seconds.
                /// </summary>
                public static Rune FlamingDart = new Rune
                {
                    Index = 4,
                    Name = "Flaming Dart",
                    Description = "Ignite the dart, dealing 565% weapon damage as Fire over 4 seconds.",
                    Tooltip = "rune/poison-dart/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Transform your Poison Dart into a snake that has a 35% chance to Stun the enemy for 1.5 seconds.
                /// </summary>
                public static Rune SnakeToTheFace = new Rune
                {
                    Index = 5,
                    Name = "Snake to the Face",
                    Description = "Transform your Poison Dart into a snake that has a 35% chance to Stun the enemy for 1.5 seconds.",
                    Tooltip = "rune/poison-dart/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 0,
                    ModifiedDuration = TimeSpan.FromSeconds(1.5),
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Grasp of the Dead

                /// <summary>
                /// Removes the Mana cost and increases the amount enemies are Slowed to 80%.Damage type is changed to Cold.
                /// </summary>
                public static Rune UnbreakableGrasp = new Rune
                {
                    Index = 1,
                    Name = "Unbreakable Grasp",
                    Description = "Removes the Mana cost and increases the amount enemies are Slowed to 80%.Damage type is changed to Cold.",
                    Tooltip = "rune/grasp-of-the-dead/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 1,
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Increase the damage done to 1360% weapon damage as Physical.
                /// </summary>
                public static Rune GropingEels = new Rune
                {
                    Index = 2,
                    Name = "Groping Eels",
                    Description = "Increase the damage done to 1360% weapon damage as Physical.",
                    Tooltip = "rune/grasp-of-the-dead/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 1,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Enemies who die while in the area of Grasp of the Dead have a 70% chance to summon a Zombie Dog.Damage type is changed to Poison.
                /// </summary>
                public static Rune DeathIsLife = new Rune
                {
                    Index = 3,
                    Name = "Death Is Life",
                    Description = "Enemies who die while in the area of Grasp of the Dead have a 70% chance to summon a Zombie Dog.Damage type is changed to Poison.",
                    Tooltip = "rune/grasp-of-the-dead/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 1,
                    ModifiedElement = Element.Poison,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Reduce the cooldown of Grasp of the Dead to 4 seconds.Damage type is changed to Poison.
                /// </summary>
                public static Rune DesperateGrasp = new Rune
                {
                    Index = 4,
                    Name = "Desperate Grasp",
                    Description = "Reduce the cooldown of Grasp of the Dead to 4 seconds.Damage type is changed to Poison.",
                    Tooltip = "rune/grasp-of-the-dead/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    ModifiedElement = Element.Poison,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Corpses also fall from the sky, dealing 420% weapon damage as Physical over 3 seconds to nearby enemies.
                /// </summary>
                public static Rune RainOfCorpses = new Rune
                {
                    Index = 5,
                    Name = "Rain of Corpses",
                    Description = "Corpses also fall from the sky, dealing 420% weapon damage as Physical over 3 seconds to nearby enemies.",
                    Tooltip = "rune/grasp-of-the-dead/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Corpse Spiders

                /// <summary>
                /// Throw a jar with jumping spiders that leap up to 25 yards to reach their enemy and attack for a total of 645% weapon damage as Poison.
                /// </summary>
                public static Rune LeapingSpiders = new Rune
                {
                    Index = 1,
                    Name = "Leaping Spiders",
                    Description = "Throw a jar with jumping spiders that leap up to 25 yards to reach their enemy and attack for a total of 645% weapon damage as Poison.",
                    Tooltip = "rune/corpse-spiders/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 2,
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Throw a jar with a spider queen that births spiderlings, dealing 2625% weapon damage as Poison over 15 seconds.You may have one spider queen summoned at a time.
                /// </summary>
                public static Rune SpiderQueen = new Rune
                {
                    Index = 2,
                    Name = "Spider Queen",
                    Description = "Throw a jar with a spider queen that births spiderlings, dealing 2625% weapon damage as Poison over 15 seconds.You may have one spider queen summoned at a time.",
                    Tooltip = "rune/corpse-spiders/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 2,
                    ModifiedDuration = TimeSpan.FromSeconds(15),
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Throw a jar with widowmaker spiders deal a total of 700% weapon damage as Physical.
                /// </summary>
                public static Rune Widowmakers = new Rune
                {
                    Index = 3,
                    Name = "Widowmakers",
                    Description = "Throw a jar with widowmaker spiders deal a total of 700% weapon damage as Physical.",
                    Tooltip = "rune/corpse-spiders/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 2,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Throw a jar with paralyzing spiders that have a 100% chance to Slow enemies by 60% with every attack.
                /// </summary>
                public static Rune MedusaSpiders = new Rune
                {
                    Index = 4,
                    Name = "Medusa Spiders",
                    Description = "Throw a jar with paralyzing spiders that have a 100% chance to Slow enemies by 60% with every attack.",
                    Tooltip = "rune/corpse-spiders/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 2,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Throw a jar with fire spiders that return 3 Mana to you per hit.
                /// </summary>
                public static Rune BlazingSpiders = new Rune
                {
                    Index = 5,
                    Name = "Blazing Spiders",
                    Description = "Throw a jar with fire spiders that return 3 Mana to you per hit.",
                    Tooltip = "rune/corpse-spiders/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 2,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Summon Zombie Dogs

                /// <summary>
                /// Your Zombie Dogs gain an infectious bite that deals 120% of your weapon damage as Poison over 3 seconds.
                /// </summary>
                public static Rune RabidDogs = new Rune
                {
                    Index = 1,
                    Name = "Rabid Dogs",
                    Description = "Your Zombie Dogs gain an infectious bite that deals 120% of your weapon damage as Poison over 3 seconds.",
                    Tooltip = "rune/summon-zombie-dogs/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Enemies who hit or are hit by your Zombie Dogs are Chilled for 3 seconds.
                /// </summary>
                public static Rune ChilledToTheBone = new Rune
                {
                    Index = 2,
                    Name = "Chilled to the Bone",
                    Description = "Enemies who hit or are hit by your Zombie Dogs are Chilled for 3 seconds.",
                    Tooltip = "rune/summon-zombie-dogs/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Your Zombie Dogs absorb 10% of all damage done to you.
                /// </summary>
                public static Rune LifeLink = new Rune
                {
                    Index = 3,
                    Name = "Life Link",
                    Description = "Your Zombie Dogs absorb 10% of all damage done to you.",
                    Tooltip = "rune/summon-zombie-dogs/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 3,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Your Zombie Dogs burst into flames, burning nearby enemies for 80% of your weapon damage as Fire every second.
                /// </summary>
                public static Rune BurningDogs = new Rune
                {
                    Index = 4,
                    Name = "Burning Dogs",
                    Description = "Your Zombie Dogs burst into flames, burning nearby enemies for 80% of your weapon damage as Fire every second.",
                    Tooltip = "rune/summon-zombie-dogs/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 3,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Your Zombie Dogs heal you for 100% of your Life On Hit with every attack.
                /// </summary>
                public static Rune LeechingBeasts = new Rune
                {
                    Index = 5,
                    Name = "Leeching Beasts",
                    Description = "Your Zombie Dogs heal you for 100% of your Life On Hit with every attack.",
                    Tooltip = "rune/summon-zombie-dogs/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 3,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Firebats

                /// <summary>
                /// Summon fewer but larger bats that travel a long distance and deal 500% weapon damage as Fire.
                /// </summary>
                public static Rune DireBats = new Rune
                {
                    Index = 1,
                    Name = "Dire Bats",
                    Description = "Summon fewer but larger bats that travel a long distance and deal 500% weapon damage as Fire.",
                    Tooltip = "rune/firebats/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 4,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Firebats has an initial Mana cost of 250 mana but no longer has a channeling cost.Firebats&#39; damage type turns into Physical.
                /// </summary>
                public static Rune VampireBats = new Rune
                {
                    Index = 2,
                    Name = "Vampire Bats",
                    Description = "Firebats has an initial Mana cost of 250 mana but no longer has a channeling cost.Firebats&#39; damage type turns into Physical.",
                    Tooltip = "rune/firebats/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 4,
                    ModifiedElement = Element.Physical,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Diseased bats fly towards the enemy and infect them. Damage is slow at first, but can increase over time to a maximum of 720% weapon damage as Poison.
                /// </summary>
                public static Rune PlagueBats = new Rune
                {
                    Index = 3,
                    Name = "Plague Bats",
                    Description = "Diseased bats fly towards the enemy and infect them. Damage is slow at first, but can increase over time to a maximum of 720% weapon damage as Poison.",
                    Tooltip = "rune/firebats/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 4,
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Rapidly summon two bats that each seek out a nearby enemy for 750% weapon damage as Fire.
                /// </summary>
                public static Rune HungryBats = new Rune
                {
                    Index = 4,
                    Name = "Hungry Bats",
                    Description = "Rapidly summon two bats that each seek out a nearby enemy for 750% weapon damage as Fire.",
                    Tooltip = "rune/firebats/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 4,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Call forth a swirl of bats that damage nearby enemies for 425% weapon damage as Fire. The damage of the bats increases every second, up to a maximum of 850% weapon damage after 3 seconds.
                /// </summary>
                public static Rune CloudOfBats = new Rune
                {
                    Index = 5,
                    Name = "Cloud of Bats",
                    Description = "Call forth a swirl of bats that damage nearby enemies for 425% weapon damage as Fire. The damage of the bats increases every second, up to a maximum of 850% weapon damage after 3 seconds.",
                    Tooltip = "rune/firebats/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 4,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Horrify

                /// <summary>
                /// Enemies are no longer Immobilized and will instead run in Fear for 5 seconds.
                /// </summary>
                public static Rune Phobia = new Rune
                {
                    Index = 1,
                    Name = "Phobia",
                    Description = "Enemies are no longer Immobilized and will instead run in Fear for 5 seconds.",
                    Tooltip = "rune/horrify/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Increase movement speed by 20% for 4 seconds after casting Horrify.
                /// </summary>
                public static Rune Stalker = new Rune
                {
                    Index = 2,
                    Name = "Stalker",
                    Description = "Increase movement speed by 20% for 4 seconds after casting Horrify.",
                    Tooltip = "rune/horrify/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Increase the radius of Horrify to 24 yards. 
                /// </summary>
                public static Rune FaceOfDeath = new Rune
                {
                    Index = 3,
                    Name = "Face of Death",
                    Description = "Increase the radius of Horrify to 24 yards. ",
                    Tooltip = "rune/horrify/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 5,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 50% additional Armor for 8 seconds after casting Horrify.
                /// </summary>
                public static Rune FrighteningAspect = new Rune
                {
                    Index = 4,
                    Name = "Frightening Aspect",
                    Description = "Gain 50% additional Armor for 8 seconds after casting Horrify.",
                    Tooltip = "rune/horrify/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 55 Mana for every horrified enemy.
                /// </summary>
                public static Rune RuthlessTerror = new Rune
                {
                    Index = 5,
                    Name = "Ruthless Terror",
                    Description = "Gain 55 Mana for every horrified enemy.",
                    Tooltip = "rune/horrify/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 5,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Soul Harvest

                /// <summary>
                /// Gain mana and increase maximum Mana by 5% for each enemy harvested.
                /// </summary>
                public static Rune SwallowYourSoul = new Rune
                {
                    Index = 1,
                    Name = "Swallow Your Soul",
                    Description = "Gain mana and increase maximum Mana by 5% for each enemy harvested.",
                    Tooltip = "rune/soul-harvest/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 6,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 32185 Life for every harvested enemy.
                /// </summary>
                public static Rune Siphon = new Rune
                {
                    Index = 2,
                    Name = "Siphon",
                    Description = "Gain 32185 Life for every harvested enemy.",
                    Tooltip = "rune/soul-harvest/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 6,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Increase your Armor by 10% per harvested enemy and reduce their movement speed by 80% for 5 seconds.
                /// </summary>
                public static Rune Languish = new Rune
                {
                    Index = 3,
                    Name = "Languish",
                    Description = "Increase your Armor by 10% per harvested enemy and reduce their movement speed by 80% for 5 seconds.",
                    Tooltip = "rune/soul-harvest/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 5% increased movement speed for each enemy harvested.
                /// </summary>
                public static Rune SoulToWaste = new Rune
                {
                    Index = 4,
                    Name = "Soul to Waste",
                    Description = "Gain 5% increased movement speed for each enemy harvested.",
                    Tooltip = "rune/soul-harvest/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 6,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Harvested enemies also take 640% weapon damage as Physical.
                /// </summary>
                public static Rune VengefulSpirit = new Rune
                {
                    Index = 5,
                    Name = "Vengeful Spirit",
                    Description = "Harvested enemies also take 640% weapon damage as Physical.",
                    Tooltip = "rune/soul-harvest/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 6,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Plague of Toads

                /// <summary>
                /// Mutate to fire bullfrogs that explode for 245% weapon damage as Fire.
                /// </summary>
                public static Rune ExplosiveToads = new Rune
                {
                    Index = 1,
                    Name = "Explosive Toads",
                    Description = "Mutate to fire bullfrogs that explode for 245% weapon damage as Fire.",
                    Tooltip = "rune/plague-of-toads/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 7,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Mutate to frogs that pierce through enemies for 130% weapon damage as Physical.
                /// </summary>
                public static Rune PiercingToads = new Rune
                {
                    Index = 2,
                    Name = "Piercing Toads",
                    Description = "Mutate to frogs that pierce through enemies for 130% weapon damage as Physical.",
                    Tooltip = "rune/plague-of-toads/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 7,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Cause toads to rain from the sky that deal 182% weapon damage as Poison to enemies in the area over 2 seconds.
                /// </summary>
                public static Rune RainOfToads = new Rune
                {
                    Index = 3,
                    Name = "Rain of Toads",
                    Description = "Cause toads to rain from the sky that deal 182% weapon damage as Poison to enemies in the area over 2 seconds.",
                    Tooltip = "rune/plague-of-toads/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Mutate to yellow toads that deal 190% weapon damage as Poison and have a 15% chance to Confuse affected enemies for 4 seconds.
                /// </summary>
                public static Rune AddlingToads = new Rune
                {
                    Index = 4,
                    Name = "Addling Toads",
                    Description = "Mutate to yellow toads that deal 190% weapon damage as Poison and have a 15% chance to Confuse affected enemies for 4 seconds.",
                    Tooltip = "rune/plague-of-toads/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 9 Mana every time a toad hits an enemy.Plague of Toads&#39; damage turns into Cold.
                /// </summary>
                public static Rune ToadAffinity = new Rune
                {
                    Index = 5,
                    Name = "Toad Affinity",
                    Description = "Gain 9 Mana every time a toad hits an enemy.Plague of Toads&#39; damage turns into Cold.",
                    Tooltip = "rune/plague-of-toads/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 7,
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Haunt

                /// <summary>
                /// The spirit returns 4291 Life per second.Haunt&#39;s damage turns into Fire.
                /// </summary>
                public static Rune ConsumingSpirit = new Rune
                {
                    Index = 1,
                    Name = "Consuming Spirit",
                    Description = "The spirit returns 4291 Life per second.Haunt&#39;s damage turns into Fire.",
                    Tooltip = "rune/haunt/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 8,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Release two spirits with every cast.
                /// </summary>
                public static Rune ResentfulSpirits = new Rune
                {
                    Index = 2,
                    Name = "Resentful Spirits",
                    Description = "Release two spirits with every cast.",
                    Tooltip = "rune/haunt/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 8,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// If there are no enemies left, the spirit will linger for up to 10 seconds looking for new enemies.
                /// </summary>
                public static Rune LingeringSpirit = new Rune
                {
                    Index = 3,
                    Name = "Lingering Spirit",
                    Description = "If there are no enemies left, the spirit will linger for up to 10 seconds looking for new enemies.",
                    Tooltip = "rune/haunt/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 8,
                    ModifiedDuration = TimeSpan.FromSeconds(10),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Haunted enemies take 20% more damage from your attacks.Haunt&#39;s damage turns into Poison.
                /// </summary>
                public static Rune PoisonedSpirit = new Rune
                {
                    Index = 4,
                    Name = "Poisoned Spirit",
                    Description = "Haunted enemies take 20% more damage from your attacks.Haunt&#39;s damage turns into Poison.",
                    Tooltip = "rune/haunt/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 8,
                    ModifiedElement = Element.Poison,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// The spirit returns 13 Mana per second.Haunt&#39;s damage turns into Physical.
                /// </summary>
                public static Rune DrainingSpirit = new Rune
                {
                    Index = 5,
                    Name = "Draining Spirit",
                    Description = "The spirit returns 13 Mana per second.Haunt&#39;s damage turns into Physical.",
                    Tooltip = "rune/haunt/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 8,
                    ModifiedElement = Element.Physical,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Sacrifice

                /// <summary>
                /// Ichor erupts from the corpse of the Zombie Dog and Stuns enemies for 3 seconds.
                /// </summary>
                public static Rune BlackBlood = new Rune
                {
                    Index = 1,
                    Name = "Black Blood",
                    Description = "Ichor erupts from the corpse of the Zombie Dog and Stuns enemies for 3 seconds.",
                    Tooltip = "rune/sacrifice/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 9,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Each Zombie Dog you sacrifice has a 35% chance to resurrect as a new Zombie Dog.
                /// </summary>
                public static Rune NextOfKin = new Rune
                {
                    Index = 2,
                    Name = "Next of Kin",
                    Description = "Each Zombie Dog you sacrifice has a 35% chance to resurrect as a new Zombie Dog.",
                    Tooltip = "rune/sacrifice/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 9,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 280 Mana for each Zombie Dog you sacrifice.
                /// </summary>
                public static Rune Pride = new Rune
                {
                    Index = 3,
                    Name = "Pride",
                    Description = "Gain 280 Mana for each Zombie Dog you sacrifice.",
                    Tooltip = "rune/sacrifice/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 9,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Command all of your Zombie Dogs to charge the target at the same time, each dealing 1300% weapon damage as Physical.
                /// </summary>
                public static Rune ForTheMaster = new Rune
                {
                    Index = 4,
                    Name = "For the Master",
                    Description = "Command all of your Zombie Dogs to charge the target at the same time, each dealing 1300% weapon damage as Physical.",
                    Tooltip = "rune/sacrifice/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 9,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 20% increased damage done for 5 seconds after using Sacrifice.
                /// </summary>
                public static Rune ProvokeThePack = new Rune
                {
                    Index = 5,
                    Name = "Provoke the Pack",
                    Description = "Gain 20% increased damage done for 5 seconds after using Sacrifice.",
                    Tooltip = "rune/sacrifice/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 9,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Zombie Charger

                /// <summary>
                /// Summon a tower of zombies that falls over, dealing 880% weapon damage as Physical to any enemies it hits.
                /// </summary>
                public static Rune PileOn = new Rune
                {
                    Index = 1,
                    Name = "Pile On",
                    Description = "Summon a tower of zombies that falls over, dealing 880% weapon damage as Physical to any enemies it hits.",
                    Tooltip = "rune/zombie-charger/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 10,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// If the Zombie Charger kills any enemies, it will reanimate and charge nearby enemies for 480% weapon damage as Poison. This effect can repeat up to 2 times.
                /// </summary>
                public static Rune Undeath = new Rune
                {
                    Index = 2,
                    Name = "Undeath",
                    Description = "If the Zombie Charger kills any enemies, it will reanimate and charge nearby enemies for 480% weapon damage as Poison. This effect can repeat up to 2 times.",
                    Tooltip = "rune/zombie-charger/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 10,
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Zombie winter bears crawl out of the ground and run in all directions, dealing 280% weapon damage as Cold to nearby enemies.
                /// </summary>
                public static Rune LumberingCold = new Rune
                {
                    Index = 3,
                    Name = "Lumbering Cold",
                    Description = "Zombie winter bears crawl out of the ground and run in all directions, dealing 280% weapon damage as Cold to nearby enemies.",
                    Tooltip = "rune/zombie-charger/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 10,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Summon an explosive Zombie Dog that streaks toward the enemy before exploding, dealing 680% weapon damage as Fire to all enemies within 12 yards.
                /// </summary>
                public static Rune ExplosiveBeast = new Rune
                {
                    Index = 4,
                    Name = "Explosive Beast",
                    Description = "Summon an explosive Zombie Dog that streaks toward the enemy before exploding, dealing 680% weapon damage as Fire to all enemies within 12 yards.",
                    Tooltip = "rune/zombie-charger/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 10,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 12f,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Summon zombie bears that stampede towards your enemy. Each bear deals 520% weapon damage as Poison to enemies in the area.
                /// </summary>
                public static Rune ZombieBears = new Rune
                {
                    Index = 5,
                    Name = "Zombie Bears",
                    Description = "Summon zombie bears that stampede towards your enemy. Each bear deals 520% weapon damage as Poison to enemies in the area.",
                    Tooltip = "rune/zombie-charger/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 10,
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Spirit Walk

                /// <summary>
                /// Increase the duration of Spirit Walk to 3 seconds.
                /// </summary>
                public static Rune Jaunt = new Rune
                {
                    Index = 1,
                    Name = "Jaunt",
                    Description = "Increase the duration of Spirit Walk to 3 seconds.",
                    Tooltip = "rune/spirit-walk/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 11,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 20% of your maximum Mana when you activate Spirit Walk.
                /// </summary>
                public static Rune HonoredGuest = new Rune
                {
                    Index = 2,
                    Name = "Honored Guest",
                    Description = "Gain 20% of your maximum Mana when you activate Spirit Walk.",
                    Tooltip = "rune/spirit-walk/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 11,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// When Spirit Walk ends, your physical body erupts for 750% weapon damage as Fire to all enemies within 10 yards.
                /// </summary>
                public static Rune UmbralShock = new Rune
                {
                    Index = 3,
                    Name = "Umbral Shock",
                    Description = "When Spirit Walk ends, your physical body erupts for 750% weapon damage as Fire to all enemies within 10 yards.",
                    Tooltip = "rune/spirit-walk/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 11,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Increase your movement speed by an additional 100% while in the spirit realm.
                /// </summary>
                public static Rune Severance = new Rune
                {
                    Index = 4,
                    Name = "Severance",
                    Description = "Increase your movement speed by an additional 100% while in the spirit realm.",
                    Tooltip = "rune/spirit-walk/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 11,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 15% of your maximum Life when you activate Spirit Walk.
                /// </summary>
                public static Rune HealingJourney = new Rune
                {
                    Index = 5,
                    Name = "Healing Journey",
                    Description = "Gain 15% of your maximum Life when you activate Spirit Walk.",
                    Tooltip = "rune/spirit-walk/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 11,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Spirit Barrage

                /// <summary>
                /// Gain 12 Mana each time Spirit Barrage hits.
                /// </summary>
                public static Rune TheSpiritIsWilling = new Rune
                {
                    Index = 1,
                    Name = "The Spirit Is Willing",
                    Description = "Gain 12 Mana each time Spirit Barrage hits.",
                    Tooltip = "rune/spirit-barrage/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 12,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// An additional 3 spirits seek out other enemies and deal 65% weapon damage as Fire.
                /// </summary>
                public static Rune WellOfSouls = new Rune
                {
                    Index = 2,
                    Name = "Well of Souls",
                    Description = "An additional 3 spirits seek out other enemies and deal 65% weapon damage as Fire.",
                    Tooltip = "rune/spirit-barrage/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 12,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Summon a spectre that deals 750% weapon damage as Cold over 5 seconds to all enemies within 10 yards.You can have a maximum of 3 Phantasms out at one time.
                /// </summary>
                public static Rune Phantasm = new Rune
                {
                    Index = 3,
                    Name = "Phantasm",
                    Description = "Summon a spectre that deals 750% weapon damage as Cold over 5 seconds to all enemies within 10 yards.You can have a maximum of 3 Phantasms out at one time.",
                    Tooltip = "rune/spirit-barrage/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 6437 Life each time Spirit Barrage hits.
                /// </summary>
                public static Rune Phlebotomize = new Rune
                {
                    Index = 4,
                    Name = "Phlebotomize",
                    Description = "Gain 6437 Life each time Spirit Barrage hits.",
                    Tooltip = "rune/spirit-barrage/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 12,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Summon a spectre that hovers over you, unleashing spirit bolts at nearby enemies for 6000% weapon damage as Cold over 20 seconds.
                /// </summary>
                public static Rune Manitou = new Rune
                {
                    Index = 5,
                    Name = "Manitou",
                    Description = "Summon a spectre that hovers over you, unleashing spirit bolts at nearby enemies for 6000% weapon damage as Cold over 20 seconds.",
                    Tooltip = "rune/spirit-barrage/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(20),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Gargantuan

                /// <summary>
                /// The Gargantuan gains the Cleave ability, allowing its attacks to hit multiple enemies for 585% of your weapon damage as Cold.
                /// </summary>
                public static Rune Humongoid = new Rune
                {
                    Index = 1,
                    Name = "Humongoid",
                    Description = "The Gargantuan gains the Cleave ability, allowing its attacks to hit multiple enemies for 585% of your weapon damage as Cold.",
                    Tooltip = "rune/gargantuan/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 13,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// When the Gargantuan encounters an elite enemy or is near 5 enemies, it enrages for 15 seconds gaining: 20% movement speed 35% attack speed 200% Physical damageThis effect cannot occur more than once every 45 seconds. Elite enemies include champions, rares, bosses, and other players.
                /// </summary>
                public static Rune RestlessGiant = new Rune
                {
                    Index = 2,
                    Name = "Restless Giant",
                    Description = "When the Gargantuan encounters an elite enemy or is near 5 enemies, it enrages for 15 seconds gaining: 20% movement speed 35% attack speed 200% Physical damageThis effect cannot occur more than once every 45 seconds. Elite enemies include champions, rares, bosses, and other players.",
                    Tooltip = "rune/gargantuan/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(15),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Summon a more powerful Gargantuan to fight for you for 15 seconds. The Gargantuan&#39;s fists burn with fire, dealing 575% of your weapon damage as Fire and knocking enemies into the air.
                /// </summary>
                public static Rune WrathfulProtector = new Rune
                {
                    Index = 3,
                    Name = "Wrathful Protector",
                    Description = "Summon a more powerful Gargantuan to fight for you for 15 seconds. The Gargantuan&#39;s fists burn with fire, dealing 575% of your weapon damage as Fire and knocking enemies into the air.",
                    Tooltip = "rune/gargantuan/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(15),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// The Gargantuan is surrounded by a poison cloud that deals 135% weapon damage as Poison per second to nearby enemies.
                /// </summary>
                public static Rune BigStinker = new Rune
                {
                    Index = 4,
                    Name = "Big Stinker",
                    Description = "The Gargantuan is surrounded by a poison cloud that deals 135% weapon damage as Poison per second to nearby enemies.",
                    Tooltip = "rune/gargantuan/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 13,
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// The Gargantuan gains the ability to periodically slam enemies, dealing 200% of your weapon damage as Fire and stunning them for 3 seconds.
                /// </summary>
                public static Rune Bruiser = new Rune
                {
                    Index = 5,
                    Name = "Bruiser",
                    Description = "The Gargantuan gains the ability to periodically slam enemies, dealing 200% of your weapon damage as Fire and stunning them for 3 seconds.",
                    Tooltip = "rune/gargantuan/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Locust Swarm

                /// <summary>
                /// Locust Swarm has a 100% chance to jump to two additional enemies instead of one. 
                /// </summary>
                public static Rune Pestilence = new Rune
                {
                    Index = 1,
                    Name = "Pestilence",
                    Description = "Locust Swarm has a 100% chance to jump to two additional enemies instead of one. ",
                    Tooltip = "rune/locust-swarm/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 14,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Gain 25 Mana a second while the first enemy hit by a Locust Swarm cast is still affected by that swarm. 
                /// </summary>
                public static Rune DevouringSwarm = new Rune
                {
                    Index = 2,
                    Name = "Devouring Swarm",
                    Description = "Gain 25 Mana a second while the first enemy hit by a Locust Swarm cast is still affected by that swarm. ",
                    Tooltip = "rune/locust-swarm/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 14,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Enemies affected deal 25% reduced damage.
                /// </summary>
                public static Rune CloudOfInsects = new Rune
                {
                    Index = 3,
                    Name = "Cloud of Insects",
                    Description = "Enemies affected deal 25% reduced damage.",
                    Tooltip = "rune/locust-swarm/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 14,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Enemies killed while affected by Locust Swarm leave behind a cloud of locusts that deal 750% weapon damage as Poison over 3 seconds to enemies who stand in the area.
                /// </summary>
                public static Rune DiseasedSwarm = new Rune
                {
                    Index = 4,
                    Name = "Diseased Swarm",
                    Description = "Enemies killed while affected by Locust Swarm leave behind a cloud of locusts that deal 750% weapon damage as Poison over 3 seconds to enemies who stand in the area.",
                    Tooltip = "rune/locust-swarm/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 14,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Engulf the enemy with burning locusts that deal 1480% weapon damage as Fire over 8 seconds.
                /// </summary>
                public static Rune SearingLocusts = new Rune
                {
                    Index = 5,
                    Name = "Searing Locusts",
                    Description = "Engulf the enemy with burning locusts that deal 1480% weapon damage as Fire over 8 seconds.",
                    Tooltip = "rune/locust-swarm/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 14,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Firebomb

                /// <summary>
                /// Rather than exploding for area damage, each Firebomb can bounce to up to 6 additional enemies. Damage is reduced by 15% per bounce.
                /// </summary>
                public static Rune FlashFire = new Rune
                {
                    Index = 1,
                    Name = "Flash Fire",
                    Description = "Rather than exploding for area damage, each Firebomb can bounce to up to 6 additional enemies. Damage is reduced by 15% per bounce.",
                    Tooltip = "rune/firebomb/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 15,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// The skull can bounce up to 2 times.
                /// </summary>
                public static Rune RollTheBones = new Rune
                {
                    Index = 2,
                    Name = "Roll the Bones",
                    Description = "The skull can bounce up to 2 times.",
                    Tooltip = "rune/firebomb/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 15,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// The explosion creates a pool of fire that deals 60% weapon damage as Fire over 3 seconds.
                /// </summary>
                public static Rune FirePit = new Rune
                {
                    Index = 3,
                    Name = "Fire Pit",
                    Description = "The explosion creates a pool of fire that deals 60% weapon damage as Fire over 3 seconds.",
                    Tooltip = "rune/firebomb/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Create a column of flame that spews fire at the closest enemy for 880% weapon damage as Fire over 6 seconds.You may have up to 3 Pyrogeists active at a time.
                /// </summary>
                public static Rune Pyrogeist = new Rune
                {
                    Index = 4,
                    Name = "Pyrogeist",
                    Description = "Create a column of flame that spews fire at the closest enemy for 880% weapon damage as Fire over 6 seconds.You may have up to 3 Pyrogeists active at a time.",
                    Tooltip = "rune/firebomb/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// In addition to the base explosion, the skull creates a larger blast that deals an additional 30% weapon damage as Cold to all other enemies within 28 yards.
                /// </summary>
                public static Rune GhostBomb = new Rune
                {
                    Index = 5,
                    Name = "Ghost Bomb",
                    Description = "In addition to the base explosion, the skull creates a larger blast that deals an additional 30% weapon damage as Cold to all other enemies within 28 yards.",
                    Tooltip = "rune/firebomb/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 15,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 28f,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Hex

                /// <summary>
                /// The Fetish Shaman will periodically heal allies for 32185 Life.
                /// </summary>
                public static Rune HedgeMagic = new Rune
                {
                    Index = 1,
                    Name = "Hedge Magic",
                    Description = "The Fetish Shaman will periodically heal allies for 32185 Life.",
                    Tooltip = "rune/hex/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 16,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Hexed enemies take 15% additional damage.
                /// </summary>
                public static Rune Jinx = new Rune
                {
                    Index = 2,
                    Name = "Jinx",
                    Description = "Hexed enemies take 15% additional damage.",
                    Tooltip = "rune/hex/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 16,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Transform into an angry chicken for up to 2 seconds that can explode for 1350% weapon damage as Poison to all enemies within 12 yards.
                /// </summary>
                public static Rune AngryChicken = new Rune
                {
                    Index = 3,
                    Name = "Angry Chicken",
                    Description = "Transform into an angry chicken for up to 2 seconds that can explode for 1350% weapon damage as Poison to all enemies within 12 yards.",
                    Tooltip = "rune/hex/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 16,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 12f,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Summon a giant toad that pulls in enemies, briefly swallows them whole, then spits them back out with a layer of goo that deals 750% weapon damage as Poison over 5 seconds, Slows them, and increases their damage taken by 15%.
                /// </summary>
                public static Rune ToadOfHugeness = new Rune
                {
                    Index = 4,
                    Name = "Toad of Hugeness",
                    Description = "Summon a giant toad that pulls in enemies, briefly swallows them whole, then spits them back out with a layer of goo that deals 750% weapon damage as Poison over 5 seconds, Slows them, and increases their damage taken by 15%.",
                    Tooltip = "rune/hex/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 16,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Hexed enemies explode when killed, dealing 500% weapon damage as Fire to all enemies within 8 yards.
                /// </summary>
                public static Rune UnstableForm = new Rune
                {
                    Index = 5,
                    Name = "Unstable Form",
                    Description = "Hexed enemies explode when killed, dealing 500% weapon damage as Fire to all enemies within 8 yards.",
                    Tooltip = "rune/hex/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 16,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 8f,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Acid Cloud

                /// <summary>
                /// Increase the initial area of effect of Acid Cloud to 24 yards.
                /// </summary>
                public static Rune AcidRain = new Rune
                {
                    Index = 1,
                    Name = "Acid Rain",
                    Description = "Increase the initial area of effect of Acid Cloud to 24 yards.",
                    Tooltip = "rune/acid-cloud/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 17,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// The acid on the ground forms into a slime that irradiates nearby enemies for 600% weapon damage as Poison over 5 seconds.
                /// </summary>
                public static Rune LobBlobBomb = new Rune
                {
                    Index = 2,
                    Name = "Lob Blob Bomb",
                    Description = "The acid on the ground forms into a slime that irradiates nearby enemies for 600% weapon damage as Poison over 5 seconds.",
                    Tooltip = "rune/acid-cloud/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Create pools of frost to deal 720% weapon damage as Cold over 6 seconds.
                /// </summary>
                public static Rune SlowBurn = new Rune
                {
                    Index = 3,
                    Name = "Slow Burn",
                    Description = "Create pools of frost to deal 720% weapon damage as Cold over 6 seconds.",
                    Tooltip = "rune/acid-cloud/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Spit a cloud of acid that deals 333% weapon damage as Poison, followed by 400% weapon damage as Poison over 3 seconds.
                /// </summary>
                public static Rune KissOfDeath = new Rune
                {
                    Index = 4,
                    Name = "Kiss of Death",
                    Description = "Spit a cloud of acid that deals 333% weapon damage as Poison, followed by 400% weapon damage as Poison over 3 seconds.",
                    Tooltip = "rune/acid-cloud/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Raise a corpse from the ground that explodes for 700% weapon damage as Fire to enemies in the area.
                /// </summary>
                public static Rune CorpseBomb = new Rune
                {
                    Index = 5,
                    Name = "Corpse Bomb",
                    Description = "Raise a corpse from the ground that explodes for 700% weapon damage as Fire to enemies in the area.",
                    Tooltip = "rune/acid-cloud/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 17,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Mass Confusion

                /// <summary>
                /// Reduce the cooldown of Mass Confusion to 30 seconds.
                /// </summary>
                public static Rune UnstableRealm = new Rune
                {
                    Index = 1,
                    Name = "Unstable Realm",
                    Description = "Reduce the cooldown of Mass Confusion to 30 seconds.",
                    Tooltip = "rune/mass-confusion/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 18,
                    ModifiedDuration = TimeSpan.FromSeconds(30),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Enemies killed while Confused have a 100% chance to spawn a Zombie Dog.
                /// </summary>
                public static Rune Devolution = new Rune
                {
                    Index = 2,
                    Name = "Devolution",
                    Description = "Enemies killed while Confused have a 100% chance to spawn a Zombie Dog.",
                    Tooltip = "rune/mass-confusion/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 18,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Up to 10 enemies who are not Confused become Stunned for 3 seconds.
                /// </summary>
                public static Rune MassHysteria = new Rune
                {
                    Index = 3,
                    Name = "Mass Hysteria",
                    Description = "Up to 10 enemies who are not Confused become Stunned for 3 seconds.",
                    Tooltip = "rune/mass-confusion/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 18,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// All enemies in the area of Mass Confusion deal 30% less damage for 12 seconds.
                /// </summary>
                public static Rune Paranoia = new Rune
                {
                    Index = 4,
                    Name = "Paranoia",
                    Description = "All enemies in the area of Mass Confusion deal 30% less damage for 12 seconds.",
                    Tooltip = "rune/mass-confusion/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 18,
                    ModifiedDuration = TimeSpan.FromSeconds(12),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Amid the confusion, a giant spirit rampages through enemies, dealing 400% weapon damage per second as Physical to enemies it passes through.
                /// </summary>
                public static Rune MassHallucination = new Rune
                {
                    Index = 5,
                    Name = "Mass Hallucination",
                    Description = "Amid the confusion, a giant spirit rampages through enemies, dealing 400% weapon damage per second as Physical to enemies it passes through.",
                    Tooltip = "rune/mass-confusion/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 18,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Big Bad Voodoo

                /// <summary>
                /// Increase the duration of the ritual to 30 seconds.
                /// </summary>
                public static Rune JungleDrums = new Rune
                {
                    Index = 1,
                    Name = "Jungle Drums",
                    Description = "Increase the duration of the ritual to 30 seconds.",
                    Tooltip = "rune/big-bad-voodoo/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 19,
                    ModifiedDuration = TimeSpan.FromSeconds(30),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// The ritual restores 250 Mana per second while standing in the ritual area.
                /// </summary>
                public static Rune RainDance = new Rune
                {
                    Index = 2,
                    Name = "Rain Dance",
                    Description = "The ritual restores 250 Mana per second while standing in the ritual area.",
                    Tooltip = "rune/big-bad-voodoo/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 19,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// The Fetish increases the damage of all nearby allies by 15%.
                /// </summary>
                public static Rune SlamDance = new Rune
                {
                    Index = 3,
                    Name = "Slam Dance",
                    Description = "The Fetish increases the damage of all nearby allies by 15%.",
                    Tooltip = "rune/big-bad-voodoo/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 19,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// The ritual heals all nearby allies for 5% of their maximum Life per second and reduces all damage taken by 20%.
                /// </summary>
                public static Rune GhostTrance = new Rune
                {
                    Index = 4,
                    Name = "Ghost Trance",
                    Description = "The ritual heals all nearby allies for 5% of their maximum Life per second and reduces all damage taken by 20%.",
                    Tooltip = "rune/big-bad-voodoo/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 19,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Enemies who die in the ritual area have a 50% chance to resurrect as a Zombie Dog.
                /// </summary>
                public static Rune BoogieMan = new Rune
                {
                    Index = 5,
                    Name = "Boogie Man",
                    Description = "Enemies who die in the ritual area have a 50% chance to resurrect as a Zombie Dog.",
                    Tooltip = "rune/big-bad-voodoo/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 19,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Wall of Death

                /// <summary>
                /// Summon a deadly ring for 5 seconds that poisons nearby enemies for 1200% weapon damage as Poison over 8 seconds.
                /// </summary>
                public static Rune RingOfPoison = new Rune
                {
                    Index = 1,
                    Name = "Ring of Poison",
                    Description = "Summon a deadly ring for 5 seconds that poisons nearby enemies for 1200% weapon damage as Poison over 8 seconds.",
                    Tooltip = "rune/wall-of-death/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 20,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Increase the width of the wall to 50 yards. All enemies in front of you are knocked back behind the wall.
                /// </summary>
                public static Rune WallOfZombies = new Rune
                {
                    Index = 2,
                    Name = "Wall of Zombies",
                    Description = "Increase the width of the wall to 50 yards. All enemies in front of you are knocked back behind the wall.",
                    Tooltip = "rune/wall-of-death/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 20,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Raise a circle of zombies from the ground that traps and attacks nearby enemies for 1250% weapon damage as Physical over 5 seconds.
                /// </summary>
                public static Rune SurroundedByDeath = new Rune
                {
                    Index = 3,
                    Name = "Surrounded by Death",
                    Description = "Raise a circle of zombies from the ground that traps and attacks nearby enemies for 1250% weapon damage as Physical over 5 seconds.",
                    Tooltip = "rune/wall-of-death/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 20,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Raise a wall of flame 40 yards wide for 8 seconds that burns enemies who walk through, causing them to take 1100% weapon damage as Fire over 4 seconds.
                /// </summary>
                public static Rune FireWall = new Rune
                {
                    Index = 4,
                    Name = "Fire Wall",
                    Description = "Raise a wall of flame 40 yards wide for 8 seconds that burns enemies who walk through, causing them to take 1100% weapon damage as Fire over 4 seconds.",
                    Tooltip = "rune/wall-of-death/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 20,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Summon a circle of spirits for 6 seconds that deals 1200% weapon damage as Cold, Chills nearby enemies by 60%, and reduces their damage dealt by 25% for 3 seconds.
                /// </summary>
                public static Rune CommuningWithSpirits = new Rune
                {
                    Index = 5,
                    Name = "Communing with Spirits",
                    Description = "Summon a circle of spirits for 6 seconds that deals 1200% weapon damage as Cold, Chills nearby enemies by 60%, and reduces their damage dealt by 25% for 3 seconds.",
                    Tooltip = "rune/wall-of-death/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 20,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Fetish Army

                /// <summary>
                /// Each Fetish deals 680% weapon damage as Cold to any nearby enemy as it is summoned.
                /// </summary>
                public static Rune FetishAmbush = new Rune
                {
                    Index = 1,
                    Name = "Fetish Ambush",
                    Description = "Each Fetish deals 680% weapon damage as Cold to any nearby enemy as it is summoned.",
                    Tooltip = "rune/fetish-army/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 21,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Decrease the cooldown of Fetish Army to 90 seconds.
                /// </summary>
                public static Rune DevotedFollowing = new Rune
                {
                    Index = 2,
                    Name = "Devoted Following",
                    Description = "Decrease the cooldown of Fetish Army to 90 seconds.",
                    Tooltip = "rune/fetish-army/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 21,
                    ModifiedDuration = TimeSpan.FromSeconds(90),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Increase number of dagger-wielding Fetishes summoned by 3.
                /// </summary>
                public static Rune LegionOfDaggers = new Rune
                {
                    Index = 3,
                    Name = "Legion of Daggers",
                    Description = "Increase number of dagger-wielding Fetishes summoned by 3.",
                    Tooltip = "rune/fetish-army/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 21,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Summon an additional 2 Fetish casters who breathe fire in a cone in front of them and deal 85% of your weapon damage as Fire.
                /// </summary>
                public static Rune TikiTorchers = new Rune
                {
                    Index = 4,
                    Name = "Tiki Torchers",
                    Description = "Summon an additional 2 Fetish casters who breathe fire in a cone in front of them and deal 85% of your weapon damage as Fire.",
                    Tooltip = "rune/fetish-army/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 21,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Summon an additional 2 Hunter Fetishes that shoot blowdarts at enemies, dealing 130% of your weapon damage as Poison.
                /// </summary>
                public static Rune HeadHunters = new Rune
                {
                    Index = 5,
                    Name = "Head Hunters",
                    Description = "Summon an additional 2 Hunter Fetishes that shoot blowdarts at enemies, dealing 130% of your weapon damage as Poison.",
                    Tooltip = "rune/fetish-army/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 21,
                    ModifiedElement = Element.Poison,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

                #region Skill: Piranhas

                /// <summary>
                /// A giant bogadile emerges from the pool of water, Stuns, and bites a monster dealing 1100% weapon damage as Physical.
                /// </summary>
                public static Rune Bogadile = new Rune
                {
                    Index = 1,
                    Name = "Bogadile",
                    Description = "A giant bogadile emerges from the pool of water, Stuns, and bites a monster dealing 1100% weapon damage as Physical.",
                    Tooltip = "rune/piranhas/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 22,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Turn the piranhas into zombie piranhas. The piranhas will leap out from the pool savagely at nearby enemies
                /// </summary>
                public static Rune ZombiePiranhas = new Rune
                {
                    Index = 2,
                    Name = "Zombie Piranhas",
                    Description = "Turn the piranhas into zombie piranhas. The piranhas will leap out from the pool savagely at nearby enemies",
                    Tooltip = "rune/piranhas/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 22,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// The pool of piranhas becomes a tornado of piranhas that lasts 4 seconds. Nearby enemies are periodically sucked into the tornado.Increases the cooldown to 16 seconds.
                /// </summary>
                public static Rune Piranhado = new Rune
                {
                    Index = 3,
                    Name = "Piranhado",
                    Description = "The pool of piranhas becomes a tornado of piranhas that lasts 4 seconds. Nearby enemies are periodically sucked into the tornado.Increases the cooldown to 16 seconds.",
                    Tooltip = "rune/piranhas/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Turn each cast into a wave of piranhas that crash forward dealing 475% weapon damage and causing all enemies affected to take 15% increased damage for 8 seconds. 
                /// </summary>
                public static Rune WaveOfMutilation = new Rune
                {
                    Index = 4,
                    Name = "Wave of Mutilation",
                    Description = "Turn each cast into a wave of piranhas that crash forward dealing 475% weapon damage and causing all enemies affected to take 15% increased damage for 8 seconds. ",
                    Tooltip = "rune/piranhas/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };

                /// <summary>
                /// Changes the damage dealt to 400% weapon damage as Cold over 8 seconds, chilling enemies for the entire duration.
                /// </summary>
                public static Rune FrozenPiranhas = new Rune
                {
                    Index = 5,
                    Name = "Frozen Piranhas",
                    Description = "Changes the damage dealt to 400% weapon damage as Cold over 8 seconds, chilling enemies for the entire duration.",
                    Tooltip = "rune/piranhas/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.Witchdoctor
                };
                #endregion

            }
            public class DemonHunter : FieldCollection<DemonHunter, Rune>
            {

                /// <summary>
                /// No Rune
                /// </summary>
                public static Rune None = new Rune
                {
                    Index = 0,
                    Name = "None",
                    Description = "No Rune Selected",
                    Tooltip = string.Empty,
                    TypeId = string.Empty,
                    RuneIndex = -1,
                    Class = ActorClass.DemonHunter
                };

                #region Skill: Hungering Arrow

                /// <summary>
                /// Increase the chance for the arrow to pierce to 50%.
                /// </summary>
                public static Rune PuncturingArrow = new Rune
                {
                    Index = 1,
                    Name = "Puncturing Arrow",
                    Description = "Increase the chance for the arrow to pierce to 50%.",
                    Tooltip = "rune/hungering-arrow/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 0,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increase Hatred generated to 7.Hungering Arrow&#39;s damage turns into Fire.
                /// </summary>
                public static Rune SerratedArrow = new Rune
                {
                    Index = 2,
                    Name = "Serrated Arrow",
                    Description = "Increase Hatred generated to 7.Hungering Arrow&#39;s damage turns into Fire.",
                    Tooltip = "rune/hungering-arrow/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 0,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// If the arrow successfully pierces the first enemy, the arrow splits into 3 arrows.Hungering Arrow&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune ShatterShot = new Rune
                {
                    Index = 3,
                    Name = "Shatter Shot",
                    Description = "If the arrow successfully pierces the first enemy, the arrow splits into 3 arrows.Hungering Arrow&#39;s damage turns into Lightning.",
                    Tooltip = "rune/hungering-arrow/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 0,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Each consecutive pierce increases the damage of the arrow by 70%.Hungering Arrow&#39;s damage turns into Cold.
                /// </summary>
                public static Rune DevouringArrow = new Rune
                {
                    Index = 4,
                    Name = "Devouring Arrow",
                    Description = "Each consecutive pierce increases the damage of the arrow by 70%.Hungering Arrow&#39;s damage turns into Cold.",
                    Tooltip = "rune/hungering-arrow/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 0,
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Critical Hits cause a burst of bone to explode from the enemy, dealing 60% weapon damage to enemies within 10 yards.
                /// </summary>
                public static Rune SprayOfTeeth = new Rune
                {
                    Index = 5,
                    Name = "Spray of Teeth",
                    Description = "Critical Hits cause a burst of bone to explode from the enemy, dealing 60% weapon damage to enemies within 10 yards.",
                    Tooltip = "rune/hungering-arrow/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 0,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Impale

                /// <summary>
                /// The impact causes Knockback and has a 100% chance to Stun for 1.5 seconds.
                /// </summary>
                public static Rune Impact = new Rune
                {
                    Index = 1,
                    Name = "Impact",
                    Description = "The impact causes Knockback and has a 100% chance to Stun for 1.5 seconds.",
                    Tooltip = "rune/impale/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(1.5),
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// The enemy also burns for 500% weapon damage as Fire over 2 seconds.
                /// </summary>
                public static Rune ChemicalBurn = new Rune
                {
                    Index = 2,
                    Name = "Chemical Burn",
                    Description = "The enemy also burns for 500% weapon damage as Fire over 2 seconds.",
                    Tooltip = "rune/impale/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 1,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// The knife pierces through all enemies in a straight line for Cold damage.
                /// </summary>
                public static Rune Overpenetration = new Rune
                {
                    Index = 3,
                    Name = "Overpenetration",
                    Description = "The knife pierces through all enemies in a straight line for Cold damage.",
                    Tooltip = "rune/impale/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 1,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// The knife ricochets to 2 additional nearby enemies within 20 yards of each other.Impale&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune Ricochet = new Rune
                {
                    Index = 4,
                    Name = "Ricochet",
                    Description = "The knife ricochets to 2 additional nearby enemies within 20 yards of each other.Impale&#39;s damage turns into Lightning.",
                    Tooltip = "rune/impale/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 1,
                    ModifiedElement = Element.Lightning,
                    ModifiedAreaEffectRadius = 20f,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Critical Hits deal 330% additional damage.
                /// </summary>
                public static Rune GrievousWounds = new Rune
                {
                    Index = 5,
                    Name = "Grievous Wounds",
                    Description = "Critical Hits deal 330% additional damage.",
                    Tooltip = "rune/impale/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 1,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Entangling Shot

                /// <summary>
                /// Entangle and Slow up to 4 enemies with each shot.
                /// </summary>
                public static Rune ChainGang = new Rune
                {
                    Index = 1,
                    Name = "Chain Gang",
                    Description = "Entangle and Slow up to 4 enemies with each shot.",
                    Tooltip = "rune/entangling-shot/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 2,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Entangled enemies take 80% weapon damage as Lightning over 2 seconds.
                /// </summary>
                public static Rune ShockCollar = new Rune
                {
                    Index = 2,
                    Name = "Shock Collar",
                    Description = "Entangled enemies take 80% weapon damage as Lightning over 2 seconds.",
                    Tooltip = "rune/entangling-shot/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 2,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increase the Slow duration to 4 seconds.Entangling Shot&#39;s damage turns into Cold.
                /// </summary>
                public static Rune HeavyBurden = new Rune
                {
                    Index = 3,
                    Name = "Heavy Burden",
                    Description = "Increase the Slow duration to 4 seconds.Entangling Shot&#39;s damage turns into Cold.",
                    Tooltip = "rune/entangling-shot/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 2,
                    ModifiedDuration = TimeSpan.FromSeconds(4),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increase Hatred generated to 7.Entangling Shot&#39;s damage turns into Fire.
                /// </summary>
                public static Rune JusticeIsServed = new Rune
                {
                    Index = 4,
                    Name = "Justice is Served",
                    Description = "Increase Hatred generated to 7.Entangling Shot&#39;s damage turns into Fire.",
                    Tooltip = "rune/entangling-shot/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 2,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increase the Slow amount to 80%.
                /// </summary>
                public static Rune BountyHunter = new Rune
                {
                    Index = 5,
                    Name = "Bounty Hunter",
                    Description = "Increase the Slow amount to 80%.",
                    Tooltip = "rune/entangling-shot/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 2,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Caltrops

                /// <summary>
                /// Increase the slowing amount to 80%.
                /// </summary>
                public static Rune HookedSpines = new Rune
                {
                    Index = 1,
                    Name = "Hooked Spines",
                    Description = "Increase the slowing amount to 80%.",
                    Tooltip = "rune/caltrops/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 3,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// When the trap is sprung, all enemies in the area are immobilized for 2 seconds.
                /// </summary>
                public static Rune TorturousGround = new Rune
                {
                    Index = 2,
                    Name = "Torturous Ground",
                    Description = "When the trap is sprung, all enemies in the area are immobilized for 2 seconds.",
                    Tooltip = "rune/caltrops/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Enemies in the area also take 270% weapon damage as Physical over 6 seconds.
                /// </summary>
                public static Rune JaggedSpikes = new Rune
                {
                    Index = 3,
                    Name = "Jagged Spikes",
                    Description = "Enemies in the area also take 270% weapon damage as Physical over 6 seconds.",
                    Tooltip = "rune/caltrops/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 3,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Reduce the cost of Caltrops to 3 Discipline.
                /// </summary>
                public static Rune CarvedStakes = new Rune
                {
                    Index = 4,
                    Name = "Carved Stakes",
                    Description = "Reduce the cost of Caltrops to 3 Discipline.",
                    Tooltip = "rune/caltrops/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 3,
                    ModifiedCost = 3,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Become empowered while standing in the area of effect, gaining an additional 10% Critical Hit Chance.
                /// </summary>
                public static Rune BaitTheTrap = new Rune
                {
                    Index = 5,
                    Name = "Bait the Trap",
                    Description = "Become empowered while standing in the area of effect, gaining an additional 10% Critical Hit Chance.",
                    Tooltip = "rune/caltrops/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 3,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Rapid Fire

                /// <summary>
                /// Reduce the initial Hatred cost to 10 and ignite your arrows, causing them to deal Fire damage.
                /// </summary>
                public static Rune WitheringFire = new Rune
                {
                    Index = 1,
                    Name = "Withering Fire",
                    Description = "Reduce the initial Hatred cost to 10 and ignite your arrows, causing them to deal Fire damage.",
                    Tooltip = "rune/rapid-fire/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 4,
                    ModifiedCost = 10,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Enemies hit by Rapid Fire are Chilled by 80% for 2 seconds.Rapid Fire&#39;s damage turns into Cold.
                /// </summary>
                public static Rune FrostShots = new Rune
                {
                    Index = 2,
                    Name = "Frost Shots",
                    Description = "Enemies hit by Rapid Fire are Chilled by 80% for 2 seconds.Rapid Fire&#39;s damage turns into Cold.",
                    Tooltip = "rune/rapid-fire/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 4,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// While channeling Rapid Fire, launch 2 homing rockets every second. Each rocket deals 145% weapon damage as Physical to nearby enemies.
                /// </summary>
                public static Rune FireSupport = new Rune
                {
                    Index = 3,
                    Name = "Fire Support",
                    Description = "While channeling Rapid Fire, launch 2 homing rockets every second. Each rocket deals 145% weapon damage as Physical to nearby enemies.",
                    Tooltip = "rune/rapid-fire/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 4,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Fire lightning arrows that have a 50% chance to pierce through enemies.
                /// </summary>
                public static Rune HighVelocity = new Rune
                {
                    Index = 4,
                    Name = "High Velocity",
                    Description = "Fire lightning arrows that have a 50% chance to pierce through enemies.",
                    Tooltip = "rune/rapid-fire/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 4,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Rapidly fire grenades that explode for 545% weapon damage as Fire to all enemies within a 8 yard radius.
                /// </summary>
                public static Rune Bombardment = new Rune
                {
                    Index = 5,
                    Name = "Bombardment",
                    Description = "Rapidly fire grenades that explode for 545% weapon damage as Fire to all enemies within a 8 yard radius.",
                    Tooltip = "rune/rapid-fire/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 4,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Smoke Screen

                /// <summary>
                /// Gain 100% movement speed while invisible.
                /// </summary>
                public static Rune Displacement = new Rune
                {
                    Index = 1,
                    Name = "Displacement",
                    Description = "Gain 100% movement speed while invisible.",
                    Tooltip = "rune/smoke-screen/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 5,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increase the duration to 1.5 seconds.
                /// </summary>
                public static Rune LingeringFog = new Rune
                {
                    Index = 2,
                    Name = "Lingering Fog",
                    Description = "Increase the duration to 1.5 seconds.",
                    Tooltip = "rune/smoke-screen/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(1.5),
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Regenerate 15% Life while invisible.
                /// </summary>
                public static Rune HealingVapors = new Rune
                {
                    Index = 3,
                    Name = "Healing Vapors",
                    Description = "Regenerate 15% Life while invisible.",
                    Tooltip = "rune/smoke-screen/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 5,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Reduce the cost to 8 Discipline.
                /// </summary>
                public static Rune SpecialRecipe = new Rune
                {
                    Index = 4,
                    Name = "Special Recipe",
                    Description = "Reduce the cost to 8 Discipline.",
                    Tooltip = "rune/smoke-screen/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 5,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Remove the Discipline cost but increase the cooldown to 6 seconds.
                /// </summary>
                public static Rune VanishingPowder = new Rune
                {
                    Index = 5,
                    Name = "Vanishing Powder",
                    Description = "Remove the Discipline cost but increase the cooldown to 6 seconds.",
                    Tooltip = "rune/smoke-screen/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 5,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Vault

                /// <summary>
                /// While Vaulting, shoot 4 arrows for 75% weapon damage at nearby enemies. These shots are guaranteed Critical Hits.
                /// </summary>
                public static Rune ActionShot = new Rune
                {
                    Index = 1,
                    Name = "Action Shot",
                    Description = "While Vaulting, shoot 4 arrows for 75% weapon damage at nearby enemies. These shots are guaranteed Critical Hits.",
                    Tooltip = "rune/vault/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 6,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Enemies you vault through are knocked away and Stunned for 1.5 seconds.
                /// </summary>
                public static Rune RattlingRoll = new Rune
                {
                    Index = 2,
                    Name = "Rattling Roll",
                    Description = "Enemies you vault through are knocked away and Stunned for 1.5 seconds.",
                    Tooltip = "rune/vault/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(1.5),
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// After using Vault, your next Vault within 6 seconds has its Discipline cost reduced by 50%.
                /// </summary>
                public static Rune Tumble = new Rune
                {
                    Index = 3,
                    Name = "Tumble",
                    Description = "After using Vault, your next Vault within 6 seconds has its Discipline cost reduced by 50%.",
                    Tooltip = "rune/vault/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 6,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Remove the Discipline cost but add an 6 second cooldown.
                /// </summary>
                public static Rune Acrobatics = new Rune
                {
                    Index = 4,
                    Name = "Acrobatics",
                    Description = "Remove the Discipline cost but add an 6 second cooldown.",
                    Tooltip = "rune/vault/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 6,
                    ModifiedCooldown = TimeSpan.FromSeconds(6),
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Leave a trail of fire in your wake that deals 300% weapon damage as Fire over 3 seconds.
                /// </summary>
                public static Rune TrailOfCinders = new Rune
                {
                    Index = 5,
                    Name = "Trail of Cinders",
                    Description = "Leave a trail of fire in your wake that deals 300% weapon damage as Fire over 3 seconds.",
                    Tooltip = "rune/vault/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 6,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Bolas

                /// <summary>
                /// Increase the explosion radius to 20 yards.
                /// </summary>
                public static Rune VolatileExplosives = new Rune
                {
                    Index = 1,
                    Name = "Volatile Explosives",
                    Description = "Increase the explosion radius to 20 yards.",
                    Tooltip = "rune/bolas/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 7,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increase Hatred generated to 7.
                /// </summary>
                public static Rune ThunderBall = new Rune
                {
                    Index = 2,
                    Name = "Thunder Ball",
                    Description = "Increase Hatred generated to 7.",
                    Tooltip = "rune/bolas/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 7,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Shoot 3 bolas that each deal 160% weapon damage as Cold. The bolas no longer explode for area damage to nearby enemies.Enemies hit have a 50% chance to be Frozen for 1 seconds.
                /// </summary>
                public static Rune FreezingStrike = new Rune
                {
                    Index = 3,
                    Name = "Freezing Strike",
                    Description = "Shoot 3 bolas that each deal 160% weapon damage as Cold. The bolas no longer explode for area damage to nearby enemies.Enemies hit have a 50% chance to be Frozen for 1 seconds.",
                    Tooltip = "rune/bolas/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// When the bola explodes, you have a 15% chance to gain 2 Discipline.Bolas&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune BitterPill = new Rune
                {
                    Index = 4,
                    Name = "Bitter Pill",
                    Description = "When the bola explodes, you have a 15% chance to gain 2 Discipline.Bolas&#39;s damage turns into Lightning.",
                    Tooltip = "rune/bolas/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 7,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Augment the bola to deal 216% weapon damage as Fire to the enemy and 149% weapon damage as Fire to all other enemies within 14 yards, but increases the explosion delay to 2 seconds.
                /// </summary>
                public static Rune ImminentDoom = new Rune
                {
                    Index = 5,
                    Name = "Imminent Doom",
                    Description = "Augment the bola to deal 216% weapon damage as Fire to the enemy and 149% weapon damage as Fire to all other enemies within 14 yards, but increases the explosion delay to 2 seconds.",
                    Tooltip = "rune/bolas/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 7,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 14f,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Chakram

                /// <summary>
                /// A second Chakram mirrors the first. Each Chakram deals 220% weapon damage as Fire.
                /// </summary>
                public static Rune TwinChakrams = new Rune
                {
                    Index = 1,
                    Name = "Twin Chakrams",
                    Description = "A second Chakram mirrors the first. Each Chakram deals 220% weapon damage as Fire.",
                    Tooltip = "rune/chakram/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 8,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// The Chakram follows a slow curve, dealing 500% weapon damage as Cold to enemies along the path.
                /// </summary>
                public static Rune Serpentine = new Rune
                {
                    Index = 2,
                    Name = "Serpentine",
                    Description = "The Chakram follows a slow curve, dealing 500% weapon damage as Cold to enemies along the path.",
                    Tooltip = "rune/chakram/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 8,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// The Chakram spirals out from the targeted location dealing 380% weapon damage as Physical to enemies along the path.
                /// </summary>
                public static Rune RazorDisk = new Rune
                {
                    Index = 3,
                    Name = "Razor Disk",
                    Description = "The Chakram spirals out from the targeted location dealing 380% weapon damage as Physical to enemies along the path.",
                    Tooltip = "rune/chakram/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 8,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// The Chakram path turns into a loop, dealing 400% weapon damage as Lightning to enemies along its path.
                /// </summary>
                public static Rune Boomerang = new Rune
                {
                    Index = 4,
                    Name = "Boomerang",
                    Description = "The Chakram path turns into a loop, dealing 400% weapon damage as Lightning to enemies along its path.",
                    Tooltip = "rune/chakram/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 8,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Surround yourself with a cloud of spinning Chakrams, dealing 200% weapon damage per second as Physical to nearby enemies. Lasts 10 minutes.
                /// </summary>
                public static Rune ShurikenCloud = new Rune
                {
                    Index = 5,
                    Name = "Shuriken Cloud",
                    Description = "Surround yourself with a cloud of spinning Chakrams, dealing 200% weapon damage per second as Physical to nearby enemies. Lasts 10 minutes.",
                    Tooltip = "rune/chakram/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 8,
                    ModifiedDuration = TimeSpan.FromMinutes(10),
                    ModifiedCooldown = TimeSpan.FromMinutes(10),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Preparation

                /// <summary>
                /// Passive: Permanently increase maximum Discipline by 20.
                /// </summary>
                public static Rune Invigoration = new Rune
                {
                    Index = 1,
                    Name = "Invigoration",
                    Description = "Passive: Permanently increase maximum Discipline by 20.",
                    Tooltip = "rune/preparation/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 9,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Restore 75 Hatred.Preparation has a 20 second cooldown.
                /// </summary>
                public static Rune Punishment = new Rune
                {
                    Index = 2,
                    Name = "Punishment",
                    Description = "Restore 75 Hatred.Preparation has a 20 second cooldown.",
                    Tooltip = "rune/preparation/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 9,
                    ModifiedCooldown = TimeSpan.FromSeconds(20),
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Gain 40% Life when using Preparation.
                /// </summary>
                public static Rune BattleScars = new Rune
                {
                    Index = 3,
                    Name = "Battle Scars",
                    Description = "Gain 40% Life when using Preparation.",
                    Tooltip = "rune/preparation/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 9,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Gain 45 Discipline over 15 seconds instead of restoring it immediately.
                /// </summary>
                public static Rune FocusedMind = new Rune
                {
                    Index = 4,
                    Name = "Focused Mind",
                    Description = "Gain 45 Discipline over 15 seconds instead of restoring it immediately.",
                    Tooltip = "rune/preparation/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 9,
                    ModifiedDuration = TimeSpan.FromSeconds(15),
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// There is a 30% chance that Preparation&#39;s cooldown will not be triggered.
                /// </summary>
                public static Rune BackupPlan = new Rune
                {
                    Index = 5,
                    Name = "Backup Plan",
                    Description = "There is a 30% chance that Preparation&#39;s cooldown will not be triggered.",
                    Tooltip = "rune/preparation/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 9,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Fan of Knives

                /// <summary>
                /// Increase cooldown to 15 seconds and increase damage to 1600% weapon damage as Lightning.
                /// </summary>
                public static Rune PinpointAccuracy = new Rune
                {
                    Index = 1,
                    Name = "Pinpoint Accuracy",
                    Description = "Increase cooldown to 15 seconds and increase damage to 1600% weapon damage as Lightning.",
                    Tooltip = "rune/fan-of-knives/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 10,
                    ModifiedDuration = TimeSpan.FromSeconds(15),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Gain 40% additional armor for 6 seconds.Fan of Knives&#39; damage turns into Cold.
                /// </summary>
                public static Rune BladedArmor = new Rune
                {
                    Index = 2,
                    Name = "Bladed Armor",
                    Description = "Gain 40% additional armor for 6 seconds.Fan of Knives&#39; damage turns into Cold.",
                    Tooltip = "rune/fan-of-knives/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 10,
                    ModifiedDuration = TimeSpan.FromSeconds(6),
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Remove the cooldown but add a 30 Hatred cost.Fan of Knives&#39; damage turns into Fire.
                /// </summary>
                public static Rune KnivesExpert = new Rune
                {
                    Index = 3,
                    Name = "Knives Expert",
                    Description = "Remove the cooldown but add a 30 Hatred cost.Fan of Knives&#39; damage turns into Fire.",
                    Tooltip = "rune/fan-of-knives/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 10,
                    ModifiedCost = 30,
                    ModifiedCooldown = TimeSpan.Zero,
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Enemies hit are Stunned for 3 seconds.Fan of Knives&#39; damage turns into Fire.
                /// </summary>
                public static Rune FanOfDaggers = new Rune
                {
                    Index = 4,
                    Name = "Fan of Daggers",
                    Description = "Enemies hit are Stunned for 3 seconds.Fan of Knives&#39; damage turns into Fire.",
                    Tooltip = "rune/fan-of-knives/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 10,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Fire,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Also throw long-range knives that deal 620% weapon damage to 5 additional enemies.
                /// </summary>
                public static Rune AssassinsKnives = new Rune
                {
                    Index = 5,
                    Name = "Assassin's Knives",
                    Description = "Also throw long-range knives that deal 620% weapon damage to 5 additional enemies.",
                    Tooltip = "rune/fan-of-knives/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 10,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Evasive Fire

                /// <summary>
                /// Instead of backflipping, your Armor is increased by 25% for 3 seconds.
                /// </summary>
                public static Rune Hardened = new Rune
                {
                    Index = 1,
                    Name = "Hardened",
                    Description = "Instead of backflipping, your Armor is increased by 25% for 3 seconds.",
                    Tooltip = "rune/evasive-fire/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 11,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Whenever a backflip is triggered, leave a bomb behind that explodes for 150% weapon damage as Physical in a 12 yard radius after 0.6 seconds.
                /// </summary>
                public static Rune PartingGift = new Rune
                {
                    Index = 2,
                    Name = "Parting Gift",
                    Description = "Whenever a backflip is triggered, leave a bomb behind that explodes for 150% weapon damage as Physical in a 12 yard radius after 0.6 seconds.",
                    Tooltip = "rune/evasive-fire/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 11,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increase the damage of side bolts to 200% weapon damage as Fire.
                /// </summary>
                public static Rune CoveringFire = new Rune
                {
                    Index = 3,
                    Name = "Covering Fire",
                    Description = "Increase the damage of side bolts to 200% weapon damage as Fire.",
                    Tooltip = "rune/evasive-fire/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 11,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Instead of backflipping, increase Hatred generated to 7.Evasive Fire&#39;s damage turns into Cold.
                /// </summary>
                public static Rune Focus = new Rune
                {
                    Index = 4,
                    Name = "Focus",
                    Description = "Instead of backflipping, increase Hatred generated to 7.Evasive Fire&#39;s damage turns into Cold.",
                    Tooltip = "rune/evasive-fire/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 11,
                    ModifiedElement = Element.Cold,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increase the backflip distance to 15 yards.Evasive Fire&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune Surge = new Rune
                {
                    Index = 5,
                    Name = "Surge",
                    Description = "Increase the backflip distance to 15 yards.Evasive Fire&#39;s damage turns into Lightning.",
                    Tooltip = "rune/evasive-fire/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 11,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Grenade

                /// <summary>
                /// Increase Hatred generated to 7.
                /// </summary>
                public static Rune Tinkerer = new Rune
                {
                    Index = 1,
                    Name = "Tinkerer",
                    Description = "Increase Hatred generated to 7.",
                    Tooltip = "rune/grenade/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 12,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Throw cluster grenades that deal 200% weapon damage as Fire over a 9 yard radius.
                /// </summary>
                public static Rune ClusterGrenades = new Rune
                {
                    Index = 2,
                    Name = "Cluster Grenades",
                    Description = "Throw cluster grenades that deal 200% weapon damage as Fire over a 9 yard radius.",
                    Tooltip = "rune/grenade/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 12,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Throw out 3 grenades that explode for 160% weapon damage as Fire each.
                /// </summary>
                public static Rune GrenadeCache = new Rune
                {
                    Index = 3,
                    Name = "Grenade Cache",
                    Description = "Throw out 3 grenades that explode for 160% weapon damage as Fire each.",
                    Tooltip = "rune/grenade/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 12,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Hurl a Lightning grenade that has a 20% chance to Stun enemies for 1.5 seconds.
                /// </summary>
                public static Rune StunGrenade = new Rune
                {
                    Index = 4,
                    Name = "Stun Grenade",
                    Description = "Hurl a Lightning grenade that has a 20% chance to Stun enemies for 1.5 seconds.",
                    Tooltip = "rune/grenade/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(1.5),
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Throw a grenade that explodes for 160% weapon damage as Cold and leaves a cloud that deals an additional 120% weapon damage as Cold over 3 seconds to enemies who stand in the area and Chills them.
                /// </summary>
                public static Rune ColdGrenade = new Rune
                {
                    Index = 5,
                    Name = "Cold Grenade",
                    Description = "Throw a grenade that explodes for 160% weapon damage as Cold and leaves a cloud that deals an additional 120% weapon damage as Cold over 3 seconds to enemies who stand in the area and Chills them.",
                    Tooltip = "rune/grenade/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 12,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Shadow Power

                /// <summary>
                /// Slow the movement speed of enemies within 30 yards by 80% for 5 seconds.
                /// </summary>
                public static Rune NightBane = new Rune
                {
                    Index = 1,
                    Name = "Night Bane",
                    Description = "Slow the movement speed of enemies within 30 yards by 80% for 5 seconds.",
                    Tooltip = "rune/shadow-power/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 13,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedAreaEffectRadius = 30f,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Double the total amount of Life per Hit gained.
                /// </summary>
                public static Rune BloodMoon = new Rune
                {
                    Index = 2,
                    Name = "Blood Moon",
                    Description = "Double the total amount of Life per Hit gained.",
                    Tooltip = "rune/shadow-power/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 13,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Reduce the cost to 8 Discipline.
                /// </summary>
                public static Rune WellOfDarkness = new Rune
                {
                    Index = 3,
                    Name = "Well of Darkness",
                    Description = "Reduce the cost to 8 Discipline.",
                    Tooltip = "rune/shadow-power/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 13,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Reduce damage taken by 35% while Shadow Power is active.
                /// </summary>
                public static Rune Gloom = new Rune
                {
                    Index = 4,
                    Name = "Gloom",
                    Description = "Reduce damage taken by 35% while Shadow Power is active.",
                    Tooltip = "rune/shadow-power/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 13,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Gain 30% increased movement speed while Shadow Power is active.
                /// </summary>
                public static Rune ShadowGlide = new Rune
                {
                    Index = 5,
                    Name = "Shadow Glide",
                    Description = "Gain 30% increased movement speed while Shadow Power is active.",
                    Tooltip = "rune/shadow-power/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 13,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Spike Trap

                /// <summary>
                /// Increase to 2020% weapon damage as Cold.On detonation, the blast slows any targets hit by 60% for 3 seconds.
                /// </summary>
                public static Rune EchoingBlast = new Rune
                {
                    Index = 1,
                    Name = "Echoing Blast",
                    Description = "Increase to 2020% weapon damage as Cold.On detonation, the blast slows any targets hit by 60% for 3 seconds.",
                    Tooltip = "rune/spike-trap/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 14,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increase to 1900% weapon damage as Fire.Hatred generators will now detonate traps instead.
                /// </summary>
                public static Rune CustomTrigger = new Rune
                {
                    Index = 2,
                    Name = "Custom Trigger",
                    Description = "Increase to 1900% weapon damage as Fire.Hatred generators will now detonate traps instead.",
                    Tooltip = "rune/spike-trap/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 14,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increases damage to 1930% weapon damage as Physical.When deployed, enemies within range are instantly immobilized for 3 seconds.
                /// </summary>
                public static Rune ImpalingSpines = new Rune
                {
                    Index = 3,
                    Name = "Impaling Spines",
                    Description = "Increases damage to 1930% weapon damage as Physical.When deployed, enemies within range are instantly immobilized for 3 seconds.",
                    Tooltip = "rune/spike-trap/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 14,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// When triggered, lightning chains and hits nearby enemies within 10 yards.Lightning will also arc between all traps within 25 yards. All enemies are hit for 2010% weapon damage as Lightning over 3 hits.
                /// </summary>
                public static Rune LightningRod = new Rune
                {
                    Index = 4,
                    Name = "Lightning Rod",
                    Description = "When triggered, lightning chains and hits nearby enemies within 10 yards.Lightning will also arc between all traps within 25 yards. All enemies are hit for 2010% weapon damage as Lightning over 3 hits.",
                    Tooltip = "rune/spike-trap/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 14,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Simultaneously lay 2 traps per Spike Trap charge that detonate each for 960% weapon damage as Fire.
                /// </summary>
                public static Rune Scatter = new Rune
                {
                    Index = 5,
                    Name = "Scatter",
                    Description = "Simultaneously lay 2 traps per Spike Trap charge that detonate each for 960% weapon damage as Fire.",
                    Tooltip = "rune/spike-trap/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 14,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Companion

                /// <summary>
                /// Active: Your spider throws webs at all enemies within 25 yards of you and him, Slowing them by 80% for 5 seconds.Passive: Summons a spider companion that attacks enemies in front of him for 100% weapon damage as Physical.The spider&#39;s attacks Slow enemies by 60% for 3 seconds.
                /// </summary>
                public static Rune SpiderCompanion = new Rune
                {
                    Index = 1,
                    Name = "Spider Companion",
                    Description = "Active: Your spider throws webs at all enemies within 25 yards of you and him, Slowing them by 80% for 5 seconds.Passive: Summons a spider companion that attacks enemies in front of him for 100% weapon damage as Physical.The spider&#39;s attacks Slow enemies by 60% for 3 seconds.",
                    Tooltip = "rune/companion/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 25f,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Active: Instantly gain 50 Hatred.Passive: Summons a bat companion that attacks for 100% of your weapon damage as Physical.The bat grants you 1 Hatred per second.
                /// </summary>
                public static Rune BatCompanion = new Rune
                {
                    Index = 2,
                    Name = "Bat Companion",
                    Description = "Active: Instantly gain 50 Hatred.Passive: Summons a bat companion that attacks for 100% of your weapon damage as Physical.The bat grants you 1 Hatred per second.",
                    Tooltip = "rune/companion/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 15,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Active: Your boar charges to you, then taunts all enemies within 20 yards for 5 seconds.Passive: Summons a boar companion that attacks enemies for 100% of your weapon damage as Physical.The boar increases your and your party&#39;s Life regeneration by 10728 and resistance to all elements by 20%.
                /// </summary>
                public static Rune BoarCompanion = new Rune
                {
                    Index = 3,
                    Name = "Boar Companion",
                    Description = "Active: Your boar charges to you, then taunts all enemies within 20 yards for 5 seconds.Passive: Summons a boar companion that attacks enemies for 100% of your weapon damage as Physical.The boar increases your and your party&#39;s Life regeneration by 10728 and resistance to all elements by 20%.",
                    Tooltip = "rune/companion/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 20f,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Active: Instantly pick up all health globes and gold within 60 yards.Passive: Summons a pair of ferret companions that each attack for 100% of your weapon damage as Physical.The ferrets collect gold for you, increase gold found on monsters by 10%, and increase your movement speed by 10%.
                /// </summary>
                public static Rune FerretCompanion = new Rune
                {
                    Index = 4,
                    Name = "Ferret Companion",
                    Description = "Active: Instantly pick up all health globes and gold within 60 yards.Passive: Summons a pair of ferret companions that each attack for 100% of your weapon damage as Physical.The ferrets collect gold for you, increase gold found on monsters by 10%, and increase your movement speed by 10%.",
                    Tooltip = "rune/companion/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 15,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 60f,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Active: Your wolf howls, granting you and your allies within 60 yards 15% increased damage for 10 seconds.Passive: Summons a wolf companion that attacks enemies in front of him for 100% of your weapon damage as Physical.
                /// </summary>
                public static Rune WolfCompanion = new Rune
                {
                    Index = 5,
                    Name = "Wolf Companion",
                    Description = "Active: Your wolf howls, granting you and your allies within 60 yards 15% increased damage for 10 seconds.Passive: Summons a wolf companion that attacks enemies in front of him for 100% of your weapon damage as Physical.",
                    Tooltip = "rune/companion/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 15,
                    ModifiedDuration = TimeSpan.FromSeconds(10),
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 60f,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Strafe

                /// <summary>
                /// Leave an icy trail in your wake that deals 300% weapon damage as Cold over 3 seconds and Chills enemies for 3 seconds.
                /// </summary>
                public static Rune IcyTrail = new Rune
                {
                    Index = 1,
                    Name = "Icy Trail",
                    Description = "Leave an icy trail in your wake that deals 300% weapon damage as Cold over 3 seconds and Chills enemies for 3 seconds.",
                    Tooltip = "rune/strafe/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 16,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Movement speed increased to 100% of normal running speed while strafing.Strafe&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune DriftingShadow = new Rune
                {
                    Index = 2,
                    Name = "Drifting Shadow",
                    Description = "Movement speed increased to 100% of normal running speed while strafing.Strafe&#39;s damage turns into Lightning.",
                    Tooltip = "rune/strafe/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 16,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Throw out knives rather than arrows that deal an extra 140% damage on Critical Hits.
                /// </summary>
                public static Rune StingingSteel = new Rune
                {
                    Index = 3,
                    Name = "Stinging Steel",
                    Description = "Throw out knives rather than arrows that deal an extra 140% damage on Critical Hits.",
                    Tooltip = "rune/strafe/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 16,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// In addition to regular shots, shoot off homing rockets for 130% weapon damage as Fire.
                /// </summary>
                public static Rune RocketStorm = new Rune
                {
                    Index = 4,
                    Name = "Rocket Storm",
                    Description = "In addition to regular shots, shoot off homing rockets for 130% weapon damage as Fire.",
                    Tooltip = "rune/strafe/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 16,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Throw out bouncy grenades that explode for 460% weapon damage as Fire to enemies within 9 yards.
                /// </summary>
                public static Rune Demolition = new Rune
                {
                    Index = 5,
                    Name = "Demolition",
                    Description = "Throw out bouncy grenades that explode for 460% weapon damage as Fire to enemies within 9 yards.",
                    Tooltip = "rune/strafe/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 16,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 9f,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Elemental Arrow

                /// <summary>
                /// Shoot a ball of lightning that electrocutes enemies along its path for 300% weapon damage as Lightning.
                /// </summary>
                public static Rune BallLightning = new Rune
                {
                    Index = 1,
                    Name = "Ball Lightning",
                    Description = "Shoot a ball of lightning that electrocutes enemies along its path for 300% weapon damage as Lightning.",
                    Tooltip = "rune/elemental-arrow/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 17,
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Shoot a frost arrow that hits an enemy for 330% weapon damage as Cold then splits into up to 10 additional frost arrows. Enemies hit are Chilled by 60% for 1 seconds.
                /// </summary>
                public static Rune FrostArrow = new Rune
                {
                    Index = 2,
                    Name = "Frost Arrow",
                    Description = "Shoot a frost arrow that hits an enemy for 330% weapon damage as Cold then splits into up to 10 additional frost arrows. Enemies hit are Chilled by 60% for 1 seconds.",
                    Tooltip = "rune/elemental-arrow/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Shoot a fiery arrow that hits an enemy for 300% weapon damage as Fire and explodes, immolating the ground for 315% weapon damage as Fire over 2 seconds to enemies within 10 yards.
                /// </summary>
                public static Rune ImmolationArrow = new Rune
                {
                    Index = 3,
                    Name = "Immolation Arrow",
                    Description = "Shoot a fiery arrow that hits an enemy for 300% weapon damage as Fire and explodes, immolating the ground for 315% weapon damage as Fire over 2 seconds to enemies within 10 yards.",
                    Tooltip = "rune/elemental-arrow/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 10f,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Shoot an electrified bolt for 300% weapon damage as Lightning that Stuns enemies for 1 second on a Critical Hit.
                /// </summary>
                public static Rune LightningBolts = new Rune
                {
                    Index = 4,
                    Name = "Lightning Bolts",
                    Description = "Shoot an electrified bolt for 300% weapon damage as Lightning that Stuns enemies for 1 second on a Critical Hit.",
                    Tooltip = "rune/elemental-arrow/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 17,
                    ModifiedDuration = TimeSpan.FromSeconds(1),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Shoot a shadow tentacle that deals 300% weapon damage as Physical to enemies along its path and returns 0.4% of your maximum Life for each enemy hit.
                /// </summary>
                public static Rune NetherTentacles = new Rune
                {
                    Index = 5,
                    Name = "Nether Tentacles",
                    Description = "Shoot a shadow tentacle that deals 300% weapon damage as Physical to enemies along its path and returns 0.4% of your maximum Life for each enemy hit.",
                    Tooltip = "rune/elemental-arrow/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 17,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Marked for Death

                /// <summary>
                /// When the enemy is killed, the mark spreads to the closest 3 enemies within 30 yards. This effect can chain repeatedly.
                /// </summary>
                public static Rune Contagion = new Rune
                {
                    Index = 1,
                    Name = "Contagion",
                    Description = "When the enemy is killed, the mark spreads to the closest 3 enemies within 30 yards. This effect can chain repeatedly.",
                    Tooltip = "rune/marked-for-death/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 18,
                    ModifiedAreaEffectRadius = 30f,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Mark an area on the ground of 15 yard radius for 15 seconds. Enemies in the area take 15% additional damage.
                /// </summary>
                public static Rune ValleyOfDeath = new Rune
                {
                    Index = 2,
                    Name = "Valley of Death",
                    Description = "Mark an area on the ground of 15 yard radius for 15 seconds. Enemies in the area take 15% additional damage.",
                    Tooltip = "rune/marked-for-death/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 18,
                    ModifiedDuration = TimeSpan.FromSeconds(15),
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// 15% of damage dealt to the marked enemy is also divided evenly among all enemies within 20 yards.
                /// </summary>
                public static Rune GrimReaper = new Rune
                {
                    Index = 3,
                    Name = "Grim Reaper",
                    Description = "15% of damage dealt to the marked enemy is also divided evenly among all enemies within 20 yards.",
                    Tooltip = "rune/marked-for-death/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 18,
                    ModifiedAreaEffectRadius = 20f,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Attacks you make against the marked enemy generate 4 Hatred.
                /// </summary>
                public static Rune MortalEnemy = new Rune
                {
                    Index = 4,
                    Name = "Mortal Enemy",
                    Description = "Attacks you make against the marked enemy generate 4 Hatred.",
                    Tooltip = "rune/marked-for-death/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 18,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Attackers heal for up to 3% of their maximum Life when damaging the marked enemy.
                /// </summary>
                public static Rune DeathToll = new Rune
                {
                    Index = 5,
                    Name = "Death Toll",
                    Description = "Attackers heal for up to 3% of their maximum Life when damaging the marked enemy.",
                    Tooltip = "rune/marked-for-death/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 18,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Multishot

                /// <summary>
                /// Reduce the Hatred cost to 18.Multishot&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune FireAtWill = new Rune
                {
                    Index = 1,
                    Name = "Fire at Will",
                    Description = "Reduce the Hatred cost to 18.Multishot&#39;s damage turns into Lightning.",
                    Tooltip = "rune/multishot/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 19,
                    ModifiedCost = 18,
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Enemies hit are Chilled and have 8% increased chance to be Critically Hit for 3 seconds.
                /// </summary>
                public static Rune WindChill = new Rune
                {
                    Index = 2,
                    Name = "Wind Chill",
                    Description = "Enemies hit are Chilled and have 8% increased chance to be Critically Hit for 3 seconds.",
                    Tooltip = "rune/multishot/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 19,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Knockback the first 4 enemies hit.
                /// </summary>
                public static Rune SuppressionFire = new Rune
                {
                    Index = 3,
                    Name = "Suppression Fire",
                    Description = "Knockback the first 4 enemies hit.",
                    Tooltip = "rune/multishot/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 19,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increase the damage of Multishot to 500% weapon damage.
                /// </summary>
                public static Rune FullBroadside = new Rune
                {
                    Index = 4,
                    Name = "Full Broadside",
                    Description = "Increase the damage of Multishot to 500% weapon damage.",
                    Tooltip = "rune/multishot/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 19,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Every time you fire, launch 3 rockets at nearby enemies that each deal 300% weapon damage as Fire.
                /// </summary>
                public static Rune Arsenal = new Rune
                {
                    Index = 5,
                    Name = "Arsenal",
                    Description = "Every time you fire, launch 3 rockets at nearby enemies that each deal 300% weapon damage as Fire.",
                    Tooltip = "rune/multishot/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 19,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Sentry

                /// <summary>
                /// The turret will also fire homing rockets at random nearby enemies for 120% weapon damage as Fire.
                /// </summary>
                public static Rune SpitfireTurret = new Rune
                {
                    Index = 1,
                    Name = "Spitfire Turret",
                    Description = "The turret will also fire homing rockets at random nearby enemies for 120% weapon damage as Fire.",
                    Tooltip = "rune/sentry/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 20,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// The turret now fires piercing bolts.
                /// </summary>
                public static Rune ImpalingBolt = new Rune
                {
                    Index = 2,
                    Name = "Impaling Bolt",
                    Description = "The turret now fires piercing bolts.",
                    Tooltip = "rune/sentry/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 20,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Create a chain between you and the Sentry and between each Sentry that deals 300% weapon damage every second to each enemy it touches.
                /// </summary>
                public static Rune ChainOfTorment = new Rune
                {
                    Index = 3,
                    Name = "Chain of Torment",
                    Description = "Create a chain between you and the Sentry and between each Sentry that deals 300% weapon damage every second to each enemy it touches.",
                    Tooltip = "rune/sentry/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 20,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// The turret Chills all nearby enemies within 16 yards, Slowing their movement speed by 60%.
                /// </summary>
                public static Rune PolarStation = new Rune
                {
                    Index = 4,
                    Name = "Polar Station",
                    Description = "The turret Chills all nearby enemies within 16 yards, Slowing their movement speed by 60%.",
                    Tooltip = "rune/sentry/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 20,
                    ModifiedAreaEffectRadius = 16f,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// The turret also creates a shield that reduces damage taken by allies by 25%.
                /// </summary>
                public static Rune GuardianTurret = new Rune
                {
                    Index = 5,
                    Name = "Guardian Turret",
                    Description = "The turret also creates a shield that reduces damage taken by allies by 25%.",
                    Tooltip = "rune/sentry/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 20,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Cluster Arrow

                /// <summary>
                /// Enemies hit by the grenades have a 100% chance to be stunned for 1.5 seconds.Cluster Arrow&#39;s damage turns into Lightning.
                /// </summary>
                public static Rune DazzlingArrow = new Rune
                {
                    Index = 1,
                    Name = "Dazzling Arrow",
                    Description = "Enemies hit by the grenades have a 100% chance to be stunned for 1.5 seconds.Cluster Arrow&#39;s damage turns into Lightning.",
                    Tooltip = "rune/cluster-arrow/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 21,
                    ModifiedDuration = TimeSpan.FromSeconds(1.5),
                    ModifiedElement = Element.Lightning,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Instead of releasing grenades, release up to 2 rockets at nearby enemies that each deal 600% weapon damage as Physical.
                /// </summary>
                public static Rune ShootingStars = new Rune
                {
                    Index = 2,
                    Name = "Shooting Stars",
                    Description = "Instead of releasing grenades, release up to 2 rockets at nearby enemies that each deal 600% weapon damage as Physical.",
                    Tooltip = "rune/cluster-arrow/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 21,
                    ModifiedElement = Element.Physical,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Instead of releasing grenades, release up to 3 rockets at nearby enemies that each deal 450% weapon damage as Cold. You gain 2% Life per enemy hit.
                /// </summary>
                public static Rune Maelstrom = new Rune
                {
                    Index = 3,
                    Name = "Maelstrom",
                    Description = "Instead of releasing grenades, release up to 3 rockets at nearby enemies that each deal 450% weapon damage as Cold. You gain 2% Life per enemy hit.",
                    Tooltip = "rune/cluster-arrow/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 21,
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Launch a cluster through the air, dropping grenades in a straight line that each explode for 650% weapon damage as Fire.
                /// </summary>
                public static Rune ClusterBombs = new Rune
                {
                    Index = 4,
                    Name = "Cluster Bombs",
                    Description = "Launch a cluster through the air, dropping grenades in a straight line that each explode for 650% weapon damage as Fire.",
                    Tooltip = "rune/cluster-arrow/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 21,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Increase the damage of the explosion at the impact location to 850% weapon damage as Fire.
                /// </summary>
                public static Rune LoadedForBear = new Rune
                {
                    Index = 5,
                    Name = "Loaded for Bear",
                    Description = "Increase the damage of the explosion at the impact location to 850% weapon damage as Fire.",
                    Tooltip = "rune/cluster-arrow/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 21,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Rain of Vengeance

                /// <summary>
                /// Launch a volley of guided arrows that rain down on enemies for 3500% weapon damage over 8 seconds.
                /// </summary>
                public static Rune DarkCloud = new Rune
                {
                    Index = 1,
                    Name = "Dark Cloud",
                    Description = "Launch a volley of guided arrows that rain down on enemies for 3500% weapon damage over 8 seconds.",
                    Tooltip = "rune/rain-of-vengeance/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(8),
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Fire a massive volley of arrows at a large area. Arrows fall from the sky dealing 2800% weapon damage as Lightning over 5 seconds to all enemies in the area.
                /// </summary>
                public static Rune Shade = new Rune
                {
                    Index = 2,
                    Name = "Shade",
                    Description = "Fire a massive volley of arrows at a large area. Arrows fall from the sky dealing 2800% weapon damage as Lightning over 5 seconds to all enemies in the area.",
                    Tooltip = "rune/rain-of-vengeance/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Lightning,
                    ModifiedIsDamaging = true,
                    ModifiedAreaEffectRadius = 85f,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Summon a wave of 10 Shadow Beasts to tear across the ground, knocking back enemies and dealing 4600% total weapon damage as Fire over 3 seconds.
                /// </summary>
                public static Rune Stampede = new Rune
                {
                    Index = 3,
                    Name = "Stampede",
                    Description = "Summon a wave of 10 Shadow Beasts to tear across the ground, knocking back enemies and dealing 4600% total weapon damage as Fire over 3 seconds.",
                    Tooltip = "rune/rain-of-vengeance/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Summon a Shadow Beast that drops grenades from the sky dealing 5800% weapon damage as Fire over 2 seconds.
                /// </summary>
                public static Rune Anathema = new Rune
                {
                    Index = 4,
                    Name = "Anathema",
                    Description = "Summon a Shadow Beast that drops grenades from the sky dealing 5800% weapon damage as Fire over 2 seconds.",
                    Tooltip = "rune/rain-of-vengeance/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(2),
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Call a group of 8 Shadow Beasts to plummet from the sky at a targeted location dealing 3800% total weapon damage as Cold over 5 seconds and Freezing enemies hit for 2 seconds.
                /// </summary>
                public static Rune FlyingStrike = new Rune
                {
                    Index = 5,
                    Name = "Flying Strike",
                    Description = "Call a group of 8 Shadow Beasts to plummet from the sky at a targeted location dealing 3800% total weapon damage as Cold over 5 seconds and Freezing enemies hit for 2 seconds.",
                    Tooltip = "rune/rain-of-vengeance/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 22,
                    ModifiedDuration = TimeSpan.FromSeconds(5),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };
                #endregion

                #region Skill: Vengeance

                /// <summary>
                /// Instead of Homing Rockets, launch 2 Grenades at random enemies outside melee range on every attack that explode for 150% weapon damage each as Fire.
                /// </summary>
                public static Rune PersonalMortar = new Rune
                {
                    Index = 1,
                    Name = "Personal Mortar",
                    Description = "Instead of Homing Rockets, launch 2 Grenades at random enemies outside melee range on every attack that explode for 150% weapon damage each as Fire.",
                    Tooltip = "rune/vengeance/c",
                    TypeId = "c",
                    RuneIndex = 2,
                    SkillIndex = 23,
                    ModifiedElement = Element.Fire,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Vengeance fills your heart, reducing all damage taken by 50%.
                /// </summary>
                public static Rune DarkHeart = new Rune
                {
                    Index = 2,
                    Name = "Dark Heart",
                    Description = "Vengeance fills your heart, reducing all damage taken by 50%.",
                    Tooltip = "rune/vengeance/b",
                    TypeId = "b",
                    RuneIndex = 1,
                    SkillIndex = 23,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Instead of Homing Rockets, the side guns are powered up into slower-firing cannons that deal 225% weapon damage and heal you for 3.0% of maximum Life per enemy hit.
                /// </summary>
                public static Rune SideCannons = new Rune
                {
                    Index = 3,
                    Name = "Side Cannons",
                    Description = "Instead of Homing Rockets, the side guns are powered up into slower-firing cannons that deal 225% weapon damage and heal you for 3.0% of maximum Life per enemy hit.",
                    Tooltip = "rune/vengeance/d",
                    TypeId = "d",
                    RuneIndex = 3,
                    SkillIndex = 23,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Gain 10 Hatred per second.
                /// </summary>
                public static Rune Seethe = new Rune
                {
                    Index = 4,
                    Name = "Seethe",
                    Description = "Gain 10 Hatred per second.",
                    Tooltip = "rune/vengeance/e",
                    TypeId = "e",
                    RuneIndex = 4,
                    SkillIndex = 23,
                    Class = ActorClass.DemonHunter
                };

                /// <summary>
                /// Instead of Homing Rockets, summon allies from the shadows that attack for 120% weapon damage as Cold and Freeze your enemies for 3 seconds.
                /// </summary>
                public static Rune FromTheShadows = new Rune
                {
                    Index = 5,
                    Name = "From the Shadows",
                    Description = "Instead of Homing Rockets, summon allies from the shadows that attack for 120% weapon damage as Cold and Freeze your enemies for 3 seconds.",
                    Tooltip = "rune/vengeance/a",
                    TypeId = "a",
                    RuneIndex = 0,
                    SkillIndex = 23,
                    ModifiedDuration = TimeSpan.FromSeconds(3),
                    ModifiedElement = Element.Cold,
                    ModifiedIsDamaging = true,
                    Class = ActorClass.DemonHunter
                };
                #endregion

            }

    #endregion


    }
}