using System;
using Trinity.Helpers;
using Trinity.Objects;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Reference
{
    public static class Passives
    {
        #region Imported Passives Data

        // AUTO-GENERATED on Mon, 18 Jan 2016 04:57:25 GMT

        public class Crusader : FieldCollection<Crusader, Passive>
        {
            /// <summary>
            /// You can wield a two-handed weapon in your main hand while bearing a shield in your off hand.Your damage dealt is reduced by 20%.
            /// </summary>
            public static Passive HeavenlyStrength = new Passive
            {
                Index = 0,
                Name = "Heavenly Strength",
                SNOPower = SNOPower.X1_Crusader_Passive_HeavenlyStrength,
                RequiredLevel = 10,
                Slug = "heavenly-strength",
                IconSlug = "x1_crusader_passive_heavenlystrength",
                Description =
                    "You can wield a two-handed weapon in your main hand while bearing a shield in your off hand.Your damage dealt is reduced by 20%.",
                Tooltip = "skill/crusader/heavenly-strength",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// While wielding a one-handed weapon, your attack speed is increased by 15% and all cooldowns are reduced by 15%.
            /// </summary>
            public static Passive Fervor = new Passive
            {
                Index = 1,
                Name = "Fervor",
                SNOPower = SNOPower.X1_Crusader_Passive_Fervor,
                RequiredLevel = 10,
                Slug = "fervor",
                IconSlug = "x1_crusader_passive_fervor",
                Description =
                    "While wielding a one-handed weapon, your attack speed is increased by 15% and all cooldowns are reduced by 15%.",
                Tooltip = "skill/crusader/fervor",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Increase Life regeneration by 2682.Reduce all non-Physical damage taken by 20%.
            /// </summary>
            public static Passive Vigilant = new Passive
            {
                Index = 2,
                Name = "Vigilant",
                SNOPower = SNOPower.X1_Crusader_Passive_Vigilant,
                RequiredLevel = 13,
                Slug = "vigilant",
                IconSlug = "x1_crusader_passive_vigilant",
                Description = "Increase Life regeneration by 2682.Reduce all non-Physical damage taken by 20%.",
                Tooltip = "skill/crusader/vigilant",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Your primary skills generate an additional 3 Wrath.Increase maximum Wrath by 30.
            /// </summary>
            public static Passive Righteousness = new Passive
            {
                Index = 3,
                Name = "Righteousness",
                SNOPower = SNOPower.X1_Crusader_Passive_Righteousness,
                RequiredLevel = 16,
                Slug = "righteousness",
                IconSlug = "x1_crusader_passive_righteousness",
                Description = "Your primary skills generate an additional 3 Wrath.Increase maximum Wrath by 30.",
                Tooltip = "skill/crusader/righteousness",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Blocking an attack generates 6 Wrath.
            /// </summary>
            public static Passive Insurmountable = new Passive
            {
                Index = 4,
                Name = "Insurmountable",
                SNOPower = SNOPower.X1_Crusader_Passive_Insurmountable,
                RequiredLevel = 20,
                Slug = "insurmountable",
                IconSlug = "x1_crusader_passive_insurmountable",
                Description = "Blocking an attack generates 6 Wrath.",
                Tooltip = "skill/crusader/insurmountable",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Increase the attack speed of Punish, Slash, Smite and Justice by 15%.
            /// </summary>
            public static Passive Fanaticism = new Passive
            {
                Index = 5,
                Name = "Fanaticism",
                SNOPower = SNOPower.X1_Crusader_Passive_NephalemMajesty,
                RequiredLevel = 20,
                Slug = "fanaticism",
                IconSlug = "x1_crusader_passive_nephalemmajesty",
                Description = "Increase the attack speed of Punish, Slash, Smite and Justice by 15%.",
                Tooltip = "skill/crusader/fanaticism",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// When you receive fatal damage, you instead become immune to damage, gain 35% increased damage and gain 107284 Life per Kill for 5 seconds.This effect may occur once every 60 seconds.
            /// </summary>
            public static Passive Indestructible = new Passive
            {
                Index = 6,
                Name = "Indestructible",
                SNOPower = SNOPower.X1_Crusader_Passive_Indestructible,
                RequiredLevel = 25,
                Slug = "indestructible",
                IconSlug = "x1_crusader_passive_indestructible",
                Description =
                    "When you receive fatal damage, you instead become immune to damage, gain 35% increased damage and gain 107284 Life per Kill for 5 seconds.This effect may occur once every 60 seconds.",
                Tooltip = "skill/crusader/indestructible",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// The amount of damage dealt by your weapon is increased by 10%.Whenever you deal Holy damage, you heal up to 1% of your total Life.
            /// </summary>
            public static Passive HolyCause = new Passive
            {
                Index = 7,
                Name = "Holy Cause",
                SNOPower = SNOPower.X1_Crusader_Passive_HolyCause,
                RequiredLevel = 27,
                Slug = "holy-cause",
                IconSlug = "x1_crusader_passive_holycause",
                Description =
                    "The amount of damage dealt by your weapon is increased by 10%.Whenever you deal Holy damage, you heal up to 1% of your total Life.",
                Tooltip = "skill/crusader/holy-cause",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Each point of Wrath spent heals you for 1341 Life.Heal amount is increased by 1% of your Health Globe Healing Bonus.
            /// </summary>
            public static Passive Wrathful = new Passive
            {
                Index = 8,
                Name = "Wrathful",
                SNOPower = SNOPower.X1_Crusader_Passive_Wrathful,
                RequiredLevel = 30,
                Slug = "wrathful",
                IconSlug = "x1_crusader_passive_wrathful",
                Description =
                    "Each point of Wrath spent heals you for 1341 Life.Heal amount is increased by 1% of your Health Globe Healing Bonus.",
                Tooltip = "skill/crusader/wrathful",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Your Armor is increased by a percent equal to your shield&#39;s Block Chance.
            /// </summary>
            public static Passive DivineFortress = new Passive
            {
                Index = 9,
                Name = "Divine Fortress",
                SNOPower = SNOPower.X1_Crusader_Passive_DivineFortress,
                RequiredLevel = 30,
                Slug = "divine-fortress",
                IconSlug = "x1_crusader_passive_divinefortress",
                Description = "Your Armor is increased by a percent equal to your shield&#39;s Block Chance.",
                Tooltip = "skill/crusader/divine-fortress",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// The cooldown of Steed Charge is reduced by 25% and Bombardment by 35%.Damage dealt by Phalanx is increased 20%.
            /// </summary>
            public static Passive LordCommander = new Passive
            {
                Index = 10,
                Name = "Lord Commander",
                SNOPower = SNOPower.X1_Crusader_Passive_LordCommander,
                RequiredLevel = 35,
                Slug = "lord-commander",
                IconSlug = "x1_crusader_passive_lordcommander",
                Description =
                    "The cooldown of Steed Charge is reduced by 25% and Bombardment by 35%.Damage dealt by Phalanx is increased 20%.",
                Tooltip = "skill/crusader/lord-commander",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// You can no longer Dodge, but your Block Chance is increased by 30%.
            /// </summary>
            public static Passive HoldYourGround = new Passive
            {
                Index = 11,
                Name = "Hold Your Ground",
                SNOPower = SNOPower.X1_Crusader_Passive_HoldYourGround,
                RequiredLevel = 40,
                Slug = "hold-your-ground",
                IconSlug = "x1_crusader_passive_holdyourground",
                Description = "You can no longer Dodge, but your Block Chance is increased by 30%.",
                Tooltip = "skill/crusader/hold-your-ground",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Increase the duration of the Active effect of all Laws by 5 seconds.
            /// </summary>
            public static Passive LongArmOfTheLaw = new Passive
            {
                Index = 12,
                Name = "Long Arm of the Law",
                SNOPower = SNOPower.X1_Crusader_Passive_LongArmOfTheLaw,
                RequiredLevel = 45,
                Slug = "long-arm-of-the-law",
                IconSlug = "x1_crusader_passive_longarmofthelaw",
                Description = "Increase the duration of the Active effect of all Laws by 5 seconds.",
                Tooltip = "skill/crusader/long-arm-of-the-law",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Your Thorns is increased by 50%.
            /// </summary>
            public static Passive IronMaiden = new Passive
            {
                Index = 13,
                Name = "Iron Maiden",
                SNOPower = SNOPower.X1_Crusader_Passive_IronMaiden,
                RequiredLevel = 50,
                Slug = "iron-maiden",
                IconSlug = "x1_crusader_passive_ironmaiden",
                Description = "Your Thorns is increased by 50%.",
                Tooltip = "skill/crusader/iron-maiden",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Whenever you successfully block, you gain 16093 Life.
            /// </summary>
            public static Passive Renewal = new Passive
            {
                Index = 14,
                Name = "Renewal",
                SNOPower = SNOPower.X1_Crusader_Passive_Renewal,
                RequiredLevel = 55,
                Slug = "renewal",
                IconSlug = "x1_crusader_passive_renewal",
                Description = "Whenever you successfully block, you gain 16093 Life.",
                Tooltip = "skill/crusader/renewal",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Gain 1.5% Strength for every gem socketed into your gear.
            /// </summary>
            public static Passive Finery = new Passive
            {
                Index = 15,
                Name = "Finery",
                SNOPower = SNOPower.X1_Crusader_Passive_Finery,
                RequiredLevel = 60,
                Slug = "finery",
                IconSlug = "x1_crusader_passive_finery",
                Description = "Gain 1.5% Strength for every gem socketed into your gear.",
                Tooltip = "skill/crusader/finery",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Increase the damage of Justice and Blessed Hammer by 20%.
            /// </summary>
            public static Passive Blunt = new Passive
            {
                Index = 16,
                Name = "Blunt",
                SNOPower = SNOPower.X1_Crusader_Passive_Blunt,
                RequiredLevel = 65,
                Slug = "blunt",
                IconSlug = "x1_crusader_passive_blunt",
                Description = "Increase the damage of Justice and Blessed Hammer by 20%.",
                Tooltip = "skill/crusader/blunt",
                Class = ActorClass.Crusader
            };

            /// <summary>
            /// Increase the damage of Punish, Shield Bash and Blessed Shield by 20%.Reduce the cooldown of Shield Glare by 30%.
            /// </summary>
            public static Passive ToweringShield = new Passive
            {
                Index = 17,
                Name = "Towering Shield",
                SNOPower = SNOPower.X1_Crusader_Passive_ToweringShield,
                RequiredLevel = 70,
                Slug = "towering-shield",
                IconSlug = "x1_crusader_passive_toweringshield",
                Description =
                    "Increase the damage of Punish, Shield Bash and Blessed Shield by 20%.Reduce the cooldown of Shield Glare by 30%.",
                Tooltip = "skill/crusader/towering-shield",
                Class = ActorClass.Crusader
            };
        }

        public class Monk : FieldCollection<Monk, Passive>
        {
            /// <summary>
            /// Damage you deal reduces enemy damage by 20% for 4 seconds.
            /// </summary>
            public static Passive Resolve = new Passive
            {
                Index = 0,
                Name = "Resolve",
                SNOPower = SNOPower.Monk_Passive_Resolve,
                RequiredLevel = 10,
                Slug = "resolve",
                IconSlug = "monk_passive_resolve",
                Description = "Damage you deal reduces enemy damage by 20% for 4 seconds.",
                Tooltip = "skill/monk/resolve",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Increase movement speed by 10%.
            /// </summary>
            public static Passive FleetFooted = new Passive
            {
                Index = 1,
                Name = "Fleet Footed",
                SNOPower = SNOPower.Monk_Passive_FleetFooted,
                RequiredLevel = 10,
                Slug = "fleet-footed",
                IconSlug = "monk_passive_fleetfooted",
                Description = "Increase movement speed by 10%.",
                Tooltip = "skill/monk/fleet-footed",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Increase maximum Spirit by 50 and increase Spirit Regeneration by 4 per second.Spirit fuels your defensive and offensive abilities.
            /// </summary>
            public static Passive ExaltedSoul = new Passive
            {
                Index = 2,
                Name = "Exalted Soul",
                SNOPower = SNOPower.Monk_Passive_ExaltedSoul,
                RequiredLevel = 13,
                Slug = "exalted-soul",
                IconSlug = "monk_passive_exaltedsoul",
                Description =
                    "Increase maximum Spirit by 50 and increase Spirit Regeneration by 4 per second.Spirit fuels your defensive and offensive abilities.",
                Tooltip = "skill/monk/exalted-soul",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Every point of Spirit spent heals you for 429 Life.Heal amount is increased by 0.4% of your Health Globe Healing Bonus.
            /// </summary>
            public static Passive Transcendence = new Passive
            {
                Index = 3,
                Name = "Transcendence",
                SNOPower = SNOPower.Monk_Passive_Transcendence,
                RequiredLevel = 16,
                Slug = "transcendence",
                IconSlug = "monk_passive_transcendence",
                Description =
                    "Every point of Spirit spent heals you for 429 Life.Heal amount is increased by 0.4% of your Health Globe Healing Bonus.",
                Tooltip = "skill/monk/transcendence",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// The Spirit costs of Mantra activation effects are reduced by 50% and you gain 4 Spirit every second when you have a Mantra learned.
            /// </summary>
            public static Passive ChantOfResonance = new Passive
            {
                Index = 4,
                Name = "Chant of Resonance",
                SNOPower = SNOPower.Monk_Passive_ChantOfResonance,
                RequiredLevel = 20,
                Slug = "chant-of-resonance",
                IconSlug = "monk_passive_chantofresonance",
                Description =
                    "The Spirit costs of Mantra activation effects are reduced by 50% and you gain 4 Spirit every second when you have a Mantra learned.",
                Tooltip = "skill/monk/chant-of-resonance",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Dealing damage to enemies above 75% Life increases your attack speed by 30% for 4 seconds.
            /// </summary>
            public static Passive SeizeTheInitiative = new Passive
            {
                Index = 5,
                Name = "Seize the Initiative",
                SNOPower = SNOPower.Monk_Passive_SeizeTheInitiative,
                RequiredLevel = 20,
                Slug = "seize-the-initiative",
                IconSlug = "monk_passive_seizetheinitiative",
                Description =
                    "Dealing damage to enemies above 75% Life increases your attack speed by 30% for 4 seconds.",
                Tooltip = "skill/monk/seize-the-initiative",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// While dual-wielding, you gain a 35% chance to dodge incoming attacks. While using a two-handed weapon, all Spirit generation is increased by 15%.
            /// </summary>
            public static Passive TheGuardiansPath = new Passive
            {
                Index = 6,
                Name = "The Guardian's Path",
                SNOPower = SNOPower.Monk_Passive_TheGuardiansPath,
                RequiredLevel = 24,
                Slug = "the-guardians-path",
                IconSlug = "monk_passive_theguardianspath",
                Description =
                    "While dual-wielding, you gain a 35% chance to dodge incoming attacks. While using a two-handed weapon, all Spirit generation is increased by 15%.",
                Tooltip = "skill/monk/the-guardians-path",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Reduce all non-Physical damage taken by 25%.
            /// </summary>
            public static Passive SixthSense = new Passive
            {
                Index = 7,
                Name = "Sixth Sense",
                SNOPower = SNOPower.Monk_Passive_SixthSense,
                RequiredLevel = 27,
                Slug = "sixth-sense",
                IconSlug = "monk_passive_sixthsense",
                Description = "Reduce all non-Physical damage taken by 25%.",
                Tooltip = "skill/monk/sixth-sense",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Each enemy within 12 yards increases your damage by 4%, up to a maximum of 20%.
            /// </summary>
            public static Passive Determination = new Passive
            {
                Index = 8,
                Name = "Determination",
                SNOPower = SNOPower.p1_Monk_Passive_Provocation,
                RequiredLevel = 30,
                Slug = "determination",
                IconSlug = "p1_monk_passive_provocation",
                Description = "Each enemy within 12 yards increases your damage by 4%, up to a maximum of 20%.",
                Tooltip = "skill/monk/determination",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// You deal 20% more damage to enemies that are Blind, Frozen or Stunned.
            /// </summary>
            public static Passive RelentlessAssault = new Passive
            {
                Index = 9,
                Name = "Relentless Assault",
                SNOPower = SNOPower.p1_Monk_Passive_RelentlessAssault, //SNOPower.P1_Monk_Passive_RelentlessAssault,
                RequiredLevel = 30,
                Slug = "relentless-assault",
                IconSlug = "p1_monk_passive_relentlessassault",
                Description = "You deal 20% more damage to enemies that are Blind, Frozen or Stunned.",
                Tooltip = "skill/monk/relentless-assault",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Reduce all cooldowns by 20%.
            /// </summary>
            public static Passive BeaconOfYtar = new Passive
            {
                Index = 10,
                Name = "Beacon of Ytar",
                SNOPower = SNOPower.Monk_Passive_BeaconOfYtar,
                RequiredLevel = 35,
                Slug = "beacon-of-ytar",
                IconSlug = "monk_passive_beaconofytar",
                Description = "Reduce all cooldowns by 20%.",
                Tooltip = "skill/monk/beacon-of-ytar",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Increase the attack speed of Spirit Generators by 15%.
            /// </summary>
            public static Passive Alacrity = new Passive
            {
                Index = 11,
                Name = "Alacrity",
                SNOPower = SNOPower.Monk_Passive_GuidingLight, //Monk_Passive_Guidinglight,
                RequiredLevel = 40,
                Slug = "alacrity",
                IconSlug = "monk_passive_guidinglight",
                Description = "Increase the attack speed of Spirit Generators by 15%.",
                Tooltip = "skill/monk/alacrity",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// 40% of your single elemental resistances from items instead increases your resistance to all elements.
            /// </summary>
            public static Passive Harmony = new Passive
            {
                Index = 12,
                Name = "Harmony",
                SNOPower = SNOPower.p1_Monk_Passive_Harmony,
                RequiredLevel = 45,
                Slug = "harmony",
                IconSlug = "p1_monk_passive_harmony",
                Description =
                    "40% of your single elemental resistances from items instead increases your resistance to all elements.",
                Tooltip = "skill/monk/harmony",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Each different Spirit Generator you use increases your damage by 10% for 3 seconds.
            /// </summary>
            public static Passive CombinationStrike = new Passive
            {
                Index = 13,
                Name = "Combination Strike",
                SNOPower = SNOPower.Monk_Passive_CombinationStrike,
                RequiredLevel = 50,
                Slug = "combination-strike",
                IconSlug = "monk_passive_combinationstrike",
                Description = "Each different Spirit Generator you use increases your damage by 10% for 3 seconds.",
                Tooltip = "skill/monk/combination-strike",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// When receiving fatal damage, you instead restore 35% Life and 35% Spirit and are immune to damage and control impairing effects for 2 seconds.This effect may occur once every 60 seconds.
            /// </summary>
            public static Passive NearDeathExperience = new Passive
            {
                Index = 14,
                Name = "Near Death Experience",
                SNOPower = SNOPower.Monk_Passive_NearDeathExperience,
                RequiredLevel = 58,
                Slug = "near-death-experience",
                IconSlug = "monk_passive_neardeathexperience",
                Description =
                    "When receiving fatal damage, you instead restore 35% Life and 35% Spirit and are immune to damage and control impairing effects for 2 seconds.This effect may occur once every 60 seconds.",
                Tooltip = "skill/monk/near-death-experience",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Each ally affected by your Mantras increases your damage by 5%, up to a maximum of 20%, and has 5% increased damage.
            /// </summary>
            public static Passive Unity = new Passive
            {
                Index = 15,
                Name = "Unity",
                SNOPower = SNOPower.X1_Monk_Passive_Unity,
                RequiredLevel = 64,
                Slug = "unity",
                IconSlug = "x1_monk_passive_unity",
                Description =
                    "Each ally affected by your Mantras increases your damage by 5%, up to a maximum of 20%, and has 5% increased damage.",
                Tooltip = "skill/monk/unity",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Moving 25 yards increases your damage by 20% for 6 seconds.
            /// </summary>
            public static Passive Momentum = new Passive
            {
                Index = 16,
                Name = "Momentum",
                SNOPower = SNOPower.X1_Monk_Passive_Momentum,
                RequiredLevel = 66,
                Slug = "momentum",
                IconSlug = "x1_monk_passive_momentum",
                Description = "Moving 25 yards increases your damage by 20% for 6 seconds.",
                Tooltip = "skill/monk/momentum",
                Class = ActorClass.Monk
            };

            /// <summary>
            /// Every third hit from a Spirit Generator increases the damage of your next damaging Spirit Spender by 40%.
            /// </summary>
            public static Passive MythicRhythm = new Passive
            {
                Index = 17,
                Name = "Mythic Rhythm",
                SNOPower = SNOPower.X1_Monk_Passive_MythicRhythm,
                RequiredLevel = 68,
                Slug = "mythic-rhythm",
                IconSlug = "x1_monk_passive_mythicrhythm",
                Description =
                    "Every third hit from a Spirit Generator increases the damage of your next damaging Spirit Spender by 40%.",
                Tooltip = "skill/monk/mythic-rhythm",
                Class = ActorClass.Monk
            };
        }

        public class WitchDoctor : FieldCollection<WitchDoctor, Passive>
        {
            /// <summary>
            /// Reduce all damage taken by you and your pets by 15%.
            /// </summary>
            public static Passive JungleFortitude = new Passive
            {
                Index = 0,
                Name = "Jungle Fortitude",
                SNOPower = SNOPower.Witchdoctor_Passive_JungleFortitude,
                RequiredLevel = 10,
                Slug = "jungle-fortitude",
                IconSlug = "witchdoctor_passive_junglefortitude",
                Description = "Reduce all damage taken by you and your pets by 15%.",
                Tooltip = "skill/witch-doctor/jungle-fortitude",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// When an enemy dies within 20 yards, there is a 15% chance that a Zombie Dog will automatically emerge.The range of this effect is increased by your gold pickup radius.
            /// </summary>
            public static Passive CircleOfLife = new Passive
            {
                Index = 1,
                Name = "Circle of Life",
                SNOPower = SNOPower.Witchdoctor_Passive_CircleOfLife,
                RequiredLevel = 10,
                Slug = "circle-of-life",
                IconSlug = "witchdoctor_passive_circleoflife",
                Description =
                    "When an enemy dies within 20 yards, there is a 15% chance that a Zombie Dog will automatically emerge.The range of this effect is increased by your gold pickup radius.",
                Tooltip = "skill/witch-doctor/circle-of-life",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// Maximum Mana is increased by 10%. Regenerate 2% of your maximum Mana per second.Mana fuels your offensive and defensive skills.
            /// </summary>
            public static Passive SpiritualAttunement = new Passive
            {
                Index = 2,
                Name = "Spiritual Attunement",
                SNOPower = SNOPower.Witchdoctor_Passive_SpiritualAttunement,
                RequiredLevel = 13,
                Slug = "spiritual-attunement",
                IconSlug = "witchdoctor_passive_spiritualattunement",
                Description =
                    "Maximum Mana is increased by 10%. Regenerate 2% of your maximum Mana per second.Mana fuels your offensive and defensive skills.",
                Tooltip = "skill/witch-doctor/spiritual-attunement",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// When you are healed by a health globe, gain 10% of your maximum Mana and 10% Intelligence for 15 seconds. The Intelligence bonus stacks up to 5 times.
            /// </summary>
            public static Passive GruesomeFeast = new Passive
            {
                Index = 3,
                Name = "Gruesome Feast",
                SNOPower = SNOPower.Witchdoctor_Passive_GruesomeFeast,
                RequiredLevel = 16,
                Slug = "gruesome-feast",
                IconSlug = "witchdoctor_passive_gruesomefeast",
                Description =
                    "When you are healed by a health globe, gain 10% of your maximum Mana and 10% Intelligence for 15 seconds. The Intelligence bonus stacks up to 5 times.",
                Tooltip = "skill/witch-doctor/gruesome-feast",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// 20% of Mana costs are paid with Life. In addition, you regenerate 1% of your maximum Life per second.
            /// </summary>
            public static Passive BloodRitual = new Passive
            {
                Index = 4,
                Name = "Blood Ritual",
                SNOPower = SNOPower.Witchdoctor_Passive_BloodRitual,
                RequiredLevel = 20,
                Slug = "blood-ritual",
                IconSlug = "witchdoctor_passive_bloodritual",
                Description =
                    "20% of Mana costs are paid with Life. In addition, you regenerate 1% of your maximum Life per second.",
                Tooltip = "skill/witch-doctor/blood-ritual",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// When you deal Poison damage to an enemy, its damage is reduced by 25% for 5 seconds.
            /// </summary>
            public static Passive BadMedicine = new Passive
            {
                Index = 5,
                Name = "Bad Medicine",
                SNOPower = SNOPower.Witchdoctor_Passive_BadMedicine,
                RequiredLevel = 20,
                Slug = "bad-medicine",
                IconSlug = "witchdoctor_passive_badmedicine",
                Description = "When you deal Poison damage to an enemy, its damage is reduced by 25% for 5 seconds.",
                Tooltip = "skill/witch-doctor/bad-medicine",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// Your Life, and that of your Zombie Dogs and Gargantuan are increased by 20%.Additionally, you may have 1 additional Zombie Dog summoned at one time.
            /// </summary>
            public static Passive ZombieHandler = new Passive
            {
                Index = 6,
                Name = "Zombie Handler",
                SNOPower = SNOPower.Witchdoctor_Passive_ZombieHandler,
                RequiredLevel = 24,
                Slug = "zombie-handler",
                IconSlug = "witchdoctor_passive_zombiehandler",
                Description =
                    "Your Life, and that of your Zombie Dogs and Gargantuan are increased by 20%.Additionally, you may have 1 additional Zombie Dog summoned at one time.",
                Tooltip = "skill/witch-doctor/zombie-handler",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// All of your damage is increased by 20%, but your Mana costs are increased by 30%.
            /// </summary>
            public static Passive PierceTheVeil = new Passive
            {
                Index = 7,
                Name = "Pierce the Veil",
                SNOPower = SNOPower.Witchdoctor_Passive_PierceTheVeil,
                RequiredLevel = 27,
                Slug = "pierce-the-veil",
                IconSlug = "witchdoctor_passive_piercetheveil",
                Description = "All of your damage is increased by 20%, but your Mana costs are increased by 30%.",
                Tooltip = "skill/witch-doctor/pierce-the-veil",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// When you receive fatal damage, you automatically enter the spirit realm for 2 seconds and heal to 50% of your maximum Life. This effect may occur once every 60 seconds.
            /// </summary>
            public static Passive SpiritVessel = new Passive
            {
                Index = 8,
                Name = "Spirit Vessel",
                SNOPower = SNOPower.Witchdoctor_Passive_SpiritVessel,
                RequiredLevel = 30,
                Slug = "spirit-vessel",
                IconSlug = "witchdoctor_passive_spiritvessel",
                Description =
                    "When you receive fatal damage, you automatically enter the spirit realm for 2 seconds and heal to 50% of your maximum Life. This effect may occur once every 60 seconds.",
                Tooltip = "skill/witch-doctor/spirit-vessel",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// When you hit enemies with your spells, you have up to a 15% chance to summon a dagger-wielding Fetish to fight by your side for 60 seconds.
            /// </summary>
            public static Passive FetishSycophants = new Passive
            {
                Index = 9,
                Name = "Fetish Sycophants",
                SNOPower = SNOPower.Witchdoctor_Passive_FetishSycophants,
                RequiredLevel = 30,
                Slug = "fetish-sycophants",
                IconSlug = "witchdoctor_passive_fetishsycophants",
                Description =
                    "When you hit enemies with your spells, you have up to a 15% chance to summon a dagger-wielding Fetish to fight by your side for 60 seconds.",
                Tooltip = "skill/witch-doctor/fetish-sycophants",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// Spirit spells return 100 Mana over 10 seconds.Spirit spells are: Haunt Horrify Mass Confusion Soul Harvest Spirit Barrage Spirit Walk
            /// </summary>
            public static Passive RushOfEssence = new Passive
            {
                Index = 10,
                Name = "Rush of Essence",
                SNOPower = SNOPower.Witchdoctor_Passive_RushOfEssence,
                RequiredLevel = 36,
                Slug = "rush-of-essence",
                IconSlug = "witchdoctor_passive_rushofessence",
                Description =
                    "Spirit spells return 100 Mana over 10 seconds.Spirit spells are: Haunt Horrify Mass Confusion Soul Harvest Spirit Barrage Spirit Walk",
                Tooltip = "skill/witch-doctor/rush-of-essence",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// When you deal damage with Corpse Spiders, Firebomb, Plague of Toads, or Poison Dart, your Mana regeneration is increased by 40% for 5 seconds.
            /// </summary>
            public static Passive VisionQuest = new Passive
            {
                Index = 11,
                Name = "Vision Quest",
                SNOPower = SNOPower.Witchdoctor_Passive_VisionQuest,
                RequiredLevel = 40,
                Slug = "vision-quest",
                IconSlug = "witchdoctor_passive_visionquest",
                Description =
                    "When you deal damage with Corpse Spiders, Firebomb, Plague of Toads, or Poison Dart, your Mana regeneration is increased by 40% for 5 seconds.",
                Tooltip = "skill/witch-doctor/vision-quest",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// While you have a Gargantuan, Zombie Dog, or Fetish summoned, your movement speed is increased by 15%.This bonus is increased to 30% while a Gargantuan, Zombie Dog, or Fetish is not in combat.Additionally, you may have 1 additional Zombie Dog summoned at one time.
            /// </summary>
            public static Passive FierceLoyalty = new Passive
            {
                Index = 12,
                Name = "Fierce Loyalty",
                SNOPower = SNOPower.Witchdoctor_Passive_FierceLoyalty,
                RequiredLevel = 45,
                Slug = "fierce-loyalty",
                IconSlug = "witchdoctor_passive_fierceloyalty",
                Description =
                    "While you have a Gargantuan, Zombie Dog, or Fetish summoned, your movement speed is increased by 15%.This bonus is increased to 30% while a Gargantuan, Zombie Dog, or Fetish is not in combat.Additionally, you may have 1 additional Zombie Dog summoned at one time.",
                Tooltip = "skill/witch-doctor/fierce-loyalty",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// Gain 1% of your maximum Life and Mana and reduce the cooldown of all of your skills by 1 second when an enemy dies within 20 yards.The range is extended by items that increase your gold pickup radius.
            /// </summary>
            public static Passive GraveInjustice = new Passive
            {
                Index = 13,
                Name = "Grave Injustice",
                SNOPower = SNOPower.Witchdoctor_Passive_GraveInjustice,
                RequiredLevel = 50,
                Slug = "grave-injustice",
                IconSlug = "witchdoctor_passive_graveinjustice",
                Description =
                    "Gain 1% of your maximum Life and Mana and reduce the cooldown of all of your skills by 1 second when an enemy dies within 20 yards.The range is extended by items that increase your gold pickup radius.",
                Tooltip = "skill/witch-doctor/grave-injustice",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// Reduce the cooldowns of the following skills by 25%: Hex Gargantuan Fetish Army Summon Zombie Dogs Big Bad Voodoo Mass Confusion
            /// </summary>
            public static Passive TribalRites = new Passive
            {
                Index = 14,
                Name = "Tribal Rites",
                SNOPower = SNOPower.Witchdoctor_Passive_TribalRites,
                RequiredLevel = 55,
                Slug = "tribal-rites",
                IconSlug = "witchdoctor_passive_tribalrites",
                Description =
                    "Reduce the cooldowns of the following skills by 25%: Hex Gargantuan Fetish Army Summon Zombie Dogs Big Bad Voodoo Mass Confusion",
                Tooltip = "skill/witch-doctor/tribal-rites",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// You deal 25% additional damage to enemies within 20 yards.
            /// </summary>
            public static Passive ConfidenceRitual = new Passive
            {
                Index = 15,
                Name = "Confidence Ritual",
                SNOPower = SNOPower.X1_WitchDoctor_Passive_ConfidenceRitual, //X1_Witchdoctor_Passive_ConfidenceRitual,
                RequiredLevel = 60,
                Slug = "confidence-ritual",
                IconSlug = "x1_witchdoctor_passive_confidenceritual",
                Description = "You deal 25% additional damage to enemies within 20 yards.",
                Tooltip = "skill/witch-doctor/confidence-ritual",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// Your Haunt, Locust Swarm and the damage amplification from Piranhas last almost forever. 
            /// </summary>
            public static Passive CreepingDeath = new Passive
            {
                Index = 16,
                Name = "Creeping Death",
                SNOPower = SNOPower.Witchdoctor_Passive_CreepingDeath,
                RequiredLevel = 64,
                Slug = "creeping-death",
                IconSlug = "witchdoctor_passive_creepingdeath",
                Description =
                    "Your Haunt, Locust Swarm and the damage amplification from Piranhas last almost forever. ",
                Tooltip = "skill/witch-doctor/creeping-death",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// You and your pets gain 120 Physical, Poison, Fire, and Cold Resistance for every enemy within 20 yards.The range of this effect is increased by your gold pickup radius.
            /// </summary>
            public static Passive SwamplandAttunement = new Passive
            {
                Index = 17,
                Name = "Swampland Attunement",
                SNOPower = SNOPower.Witchdoctor_Passive_PhysicalAttunement, //Witchdoctor_Passive_Physicalattunement,
                RequiredLevel = 66,
                Slug = "swampland-attunement",
                IconSlug = "witchdoctor_passive_physicalattunement",
                Description =
                    "You and your pets gain 120 Physical, Poison, Fire, and Cold Resistance for every enemy within 20 yards.The range of this effect is increased by your gold pickup radius.",
                Tooltip = "skill/witch-doctor/swampland-attunement",
                Class = ActorClass.Witchdoctor
            };

            /// <summary>
            /// The damage of your Zombie Dogs and Gargantuan is increased by 50%.Additionally, you may have 1 additional Zombie Dog summoned at one time.
            /// </summary>
            public static Passive MidnightFeast = new Passive
            {
                Index = 18,
                Name = "Midnight Feast",
                SNOPower = SNOPower.Witchdoctor_Passive_MidnightFeast,
                RequiredLevel = 68,
                Slug = "midnight-feast",
                IconSlug = "witchdoctor_passive_midnightfeast",
                Description =
                    "The damage of your Zombie Dogs and Gargantuan is increased by 50%.Additionally, you may have 1 additional Zombie Dog summoned at one time.",
                Tooltip = "skill/witch-doctor/midnight-feast",
                Class = ActorClass.Witchdoctor
            };
        }

        public class DemonHunter : FieldCollection<DemonHunter, Passive>
        {
            /// <summary>
            /// Enemies hit by your Hatred spenders are Slowed by 80% for 2 seconds.
            /// </summary>
            public static Passive ThrillOfTheHunt = new Passive
            {
                Index = 0,
                Name = "Thrill of the Hunt",
                SNOPower = SNOPower.DemonHunter_Passive_ThrillOfTheHunt,
                RequiredLevel = 10,
                Slug = "thrill-of-the-hunt",
                IconSlug = "demonhunter_passive_thrillofthehunt",
                Description = "Enemies hit by your Hatred spenders are Slowed by 80% for 2 seconds.",
                Tooltip = "skill/demon-hunter/thrill-of-the-hunt",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Whenever you use Vault, Shadow Power, Smoke Screen, or backflip with Evasive Fire you gain 60% movement speed for 2 seconds.
            /// </summary>
            public static Passive TacticalAdvantage = new Passive
            {
                Index = 1,
                Name = "Tactical Advantage",
                SNOPower = SNOPower.DemonHunter_Passive_TacticalAdvantage,
                RequiredLevel = 10,
                Slug = "tactical-advantage",
                IconSlug = "demonhunter_passive_tacticaladvantage",
                Description =
                    "Whenever you use Vault, Shadow Power, Smoke Screen, or backflip with Evasive Fire you gain 60% movement speed for 2 seconds.",
                Tooltip = "skill/demon-hunter/tactical-advantage",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Your maximum Hatred is increased by 25. In addition, gain 30 Hatred and 3 Discipline when you are healed by a health globe.
            /// </summary>
            public static Passive BloodVengeance = new Passive
            {
                Index = 2,
                Name = "Blood Vengeance",
                SNOPower = SNOPower.DemonHunter_Passive_Vengeance,
                RequiredLevel = 13,
                Slug = "blood-vengeance",
                IconSlug = "demonhunter_passive_vengeance",
                Description =
                    "Your maximum Hatred is increased by 25. In addition, gain 30 Hatred and 3 Discipline when you are healed by a health globe.",
                Tooltip = "skill/demon-hunter/blood-vengeance",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// As long as there are no enemies within 10 yards, all damage is increased by 20%.
            /// </summary>
            public static Passive SteadyAim = new Passive
            {
                Index = 3,
                Name = "Steady Aim",
                SNOPower = SNOPower.DemonHunter_Passive_SteadyAim,
                RequiredLevel = 16,
                Slug = "steady-aim",
                IconSlug = "demonhunter_passive_steadyaim",
                Description = "As long as there are no enemies within 10 yards, all damage is increased by 20%.",
                Tooltip = "skill/demon-hunter/steady-aim",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Increase damage against Slowed or Chilled enemies by 20%.
            /// </summary>
            public static Passive CullTheWeak = new Passive
            {
                Index = 4,
                Name = "Cull the Weak",
                SNOPower = SNOPower.DemonHunter_Passive_CullTheWeak,
                RequiredLevel = 20,
                Slug = "cull-the-weak",
                IconSlug = "demonhunter_passive_culltheweak",
                Description = "Increase damage against Slowed or Chilled enemies by 20%.",
                Tooltip = "skill/demon-hunter/cull-the-weak",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Your primary skills generate an additional 4 Hatred.
            /// </summary>
            public static Passive NightStalker = new Passive
            {
                Index = 5,
                Name = "Night Stalker",
                SNOPower = SNOPower.DemonHunter_Passive_NightStalker,
                RequiredLevel = 20,
                Slug = "night-stalker",
                IconSlug = "demonhunter_passive_nightstalker",
                Description = "Your primary skills generate an additional 4 Hatred.",
                Tooltip = "skill/demon-hunter/night-stalker",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Gain 3.0% Life regeneration per second for every second you remain stationary, stacking up to 3 times. This bonus is reset 5 seconds after you move.
            /// </summary>
            public static Passive Brooding = new Passive
            {
                Index = 6,
                Name = "Brooding",
                SNOPower = SNOPower.DemonHunter_Passive_Brooding,
                RequiredLevel = 25,
                Slug = "brooding",
                IconSlug = "demonhunter_passive_brooding",
                Description =
                    "Gain 3.0% Life regeneration per second for every second you remain stationary, stacking up to 3 times. This bonus is reset 5 seconds after you move.",
                Tooltip = "skill/demon-hunter/brooding",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Increase movement speed by 20% for 4 seconds when you hit an enemy.
            /// </summary>
            public static Passive HotPursuit = new Passive
            {
                Index = 7,
                Name = "Hot Pursuit",
                SNOPower = SNOPower.DemonHunter_Passive_HotPursuit,
                RequiredLevel = 27,
                Slug = "hot-pursuit",
                IconSlug = "demonhunter_passive_hotpursuit",
                Description = "Increase movement speed by 20% for 4 seconds when you hit an enemy.",
                Tooltip = "skill/demon-hunter/hot-pursuit",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Gain a bonus based on your weapon type:Bow: 8% increased damageCrossbow: 50% Critical Hit DamageHand Crossbow: 5% Critical Hit Chance2nd Hand Crossbow: 1 Hatred per Second
            /// </summary>
            public static Passive Archery = new Passive
            {
                Index = 8,
                Name = "Archery",
                SNOPower = SNOPower.DemonHunter_Passive_Archery,
                RequiredLevel = 30,
                Slug = "archery",
                IconSlug = "demonhunter_passive_archery",
                Description =
                    "Gain a bonus based on your weapon type:Bow: 8% increased damageCrossbow: 50% Critical Hit DamageHand Crossbow: 5% Critical Hit Chance2nd Hand Crossbow: 1 Hatred per Second",
                Tooltip = "skill/demon-hunter/archery",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Enemies you Slow, Chill, or hit with Fan of Knives, Spike Trap, Caltrops, Grenades, and Sentry fire have their damage reduced by 25% for 5 seconds.
            /// </summary>
            public static Passive NumbingTraps = new Passive
            {
                Index = 9,
                Name = "Numbing Traps",
                SNOPower = SNOPower.DemonHunter_Passive_NumbingTraps,
                RequiredLevel = 30,
                Slug = "numbing-traps",
                IconSlug = "demonhunter_passive_numbingtraps",
                Description =
                    "Enemies you Slow, Chill, or hit with Fan of Knives, Spike Trap, Caltrops, Grenades, and Sentry fire have their damage reduced by 25% for 5 seconds.",
                Tooltip = "skill/demon-hunter/numbing-traps",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Reduce the Discipline cost of all skills by 10%. Increase your Armor and resistance to all damage types by 10%.Discipline is used to fuel many of your tactical and defensive skills.
            /// </summary>
            public static Passive Perfectionist = new Passive
            {
                Index = 10,
                Name = "Perfectionist",
                SNOPower = SNOPower.DemonHunter_Passive_Perfectionist,
                RequiredLevel = 35,
                Slug = "perfectionist",
                IconSlug = "demonhunter_passive_perfectionist",
                Description =
                    "Reduce the Discipline cost of all skills by 10%. Increase your Armor and resistance to all damage types by 10%.Discipline is used to fuel many of your tactical and defensive skills.",
                Tooltip = "skill/demon-hunter/perfectionist",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Increase the duration of your Caltrops, Marked for Death, Spike Trap, and Sentry by 100%.Increase the maximum number and charges of Sentries to 3 and number of Spike Traps to 6.
            /// </summary>
            public static Passive CustomEngineering = new Passive
            {
                Index = 11,
                Name = "Custom Engineering",
                SNOPower = SNOPower.DemonHunter_Passive_CustomEngineering,
                RequiredLevel = 40,
                Slug = "custom-engineering",
                IconSlug = "demonhunter_passive_customengineering",
                Description =
                    "Increase the duration of your Caltrops, Marked for Death, Spike Trap, and Sentry by 100%.Increase the maximum number and charges of Sentries to 3 and number of Spike Traps to 6.",
                Tooltip = "skill/demon-hunter/custom-engineering",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Increase the damage of grenades by 10%.Increase the explosion size of grenades by 20%.Upon death, you drop a giant grenade that explodes for 1000% weapon damage as Fire.
            /// </summary>
            public static Passive Grenadier = new Passive
            {
                Index = 12,
                Name = "Grenadier",
                SNOPower = SNOPower.DemonHunter_Passive_Grenadier,
                RequiredLevel = 45,
                Slug = "grenadier",
                IconSlug = "demonhunter_passive_grenadier",
                Description =
                    "Increase the damage of grenades by 10%.Increase the explosion size of grenades by 20%.Upon death, you drop a giant grenade that explodes for 1000% weapon damage as Fire.",
                Tooltip = "skill/demon-hunter/grenadier",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Gain 4% Critical Hit Chance every second. This bonus is reset 1 seconds after you successfully critically hit.
            /// </summary>
            public static Passive Sharpshooter = new Passive
            {
                Index = 13,
                Name = "Sharpshooter",
                SNOPower = SNOPower.DemonHunter_Passive_Sharpshooter,
                RequiredLevel = 50,
                Slug = "sharpshooter",
                IconSlug = "demonhunter_passive_sharpshooter",
                Description =
                    "Gain 4% Critical Hit Chance every second. This bonus is reset 1 seconds after you successfully critically hit.",
                Tooltip = "skill/demon-hunter/sharpshooter",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Increase damage of rockets by 100%.In addition, you have a 20% chance to fire a homing rocket for 150% weapon damage when you attack.
            /// </summary>
            public static Passive Ballistics = new Passive
            {
                Index = 14,
                Name = "Ballistics",
                SNOPower = SNOPower.DemonHunter_Passive_Ballistics,
                RequiredLevel = 55,
                Slug = "ballistics",
                IconSlug = "demonhunter_passive_ballistics",
                Description =
                    "Increase damage of rockets by 100%.In addition, you have a 20% chance to fire a homing rocket for 150% weapon damage when you attack.",
                Tooltip = "skill/demon-hunter/ballistics",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Gain 18507 Life per Hit.Heal amount is increased by 75% of your Life per Kill.
            /// </summary>
            public static Passive Leech = new Passive
            {
                Index = 15,
                Name = "Leech",
                SNOPower = SNOPower.X1_DemonHunter_Passive_Leech,
                RequiredLevel = 60,
                Slug = "leech",
                IconSlug = "x1_demonhunter_passive_leech",
                Description = "Gain 18507 Life per Hit.Heal amount is increased by 75% of your Life per Kill.",
                Tooltip = "skill/demon-hunter/leech",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// You deal 40% additional damage to enemies above 75% health.
            /// </summary>
            public static Passive Ambush = new Passive
            {
                Index = 16,
                Name = "Ambush",
                SNOPower = SNOPower.X1_DemonHunter_Passive_Ambush,
                RequiredLevel = 64,
                Slug = "ambush",
                IconSlug = "x1_demonhunter_passive_ambush",
                Description = "You deal 40% additional damage to enemies above 75% health.",
                Tooltip = "skill/demon-hunter/ambush",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// When you receive fatal damage, you instead vanish for 2 seconds and regenerate 50% of maximum Life.This effect may occur once every 60 seconds.
            /// </summary>
            public static Passive Awareness = new Passive
            {
                Index = 17,
                Name = "Awareness",
                SNOPower = SNOPower.X1_DemonHunter_Passive_Awareness,
                RequiredLevel = 66,
                Slug = "awareness",
                IconSlug = "x1_demonhunter_passive_awareness",
                Description =
                    "When you receive fatal damage, you instead vanish for 2 seconds and regenerate 50% of maximum Life.This effect may occur once every 60 seconds.",
                Tooltip = "skill/demon-hunter/awareness",
                Class = ActorClass.DemonHunter
            };

            /// <summary>
            /// Gain 25% Critical Hit Chance against enemies who are more than 20 yards away from any other enemies.
            /// </summary>
            public static Passive SingleOut = new Passive
            {
                Index = 18,
                Name = "Single Out",
                SNOPower = SNOPower.X1_DemonHunter_Passive_SingleOut,
                RequiredLevel = 68,
                Slug = "single-out",
                IconSlug = "x1_demonhunter_passive_singleout",
                Description =
                    "Gain 25% Critical Hit Chance against enemies who are more than 20 yards away from any other enemies.",
                Tooltip = "skill/demon-hunter/single-out",
                Class = ActorClass.DemonHunter
            };
        }

        public class Wizard : FieldCollection<Wizard, Passive>
        {
            /// <summary>
            /// Being healed by a health globe causes the next Arcane Power Spender you cast to be cast for free.You can have up to 10 charges of Power Hungry.
            /// </summary>
            public static Passive PowerHungry = new Passive
            {
                Index = 0,
                Name = "Power Hungry",
                SNOPower = SNOPower.Wizard_Passive_PowerHungry,
                RequiredLevel = 10,
                Slug = "power-hungry",
                IconSlug = "wizard_passive_powerhungry",
                Description =
                    "Being healed by a health globe causes the next Arcane Power Spender you cast to be cast for free.You can have up to 10 charges of Power Hungry.",
                Tooltip = "skill/wizard/power-hungry",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// Decrease damage taken by 17%.
            /// </summary>
            public static Passive Blur = new Passive
            {
                Index = 1,
                Name = "Blur",
                SNOPower = SNOPower.Wizard_Passive_Blur,
                RequiredLevel = 10,
                Slug = "blur",
                IconSlug = "wizard_passive_blur",
                Description = "Decrease damage taken by 17%.",
                Tooltip = "skill/wizard/blur",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// Reduce all cooldowns by 20%.
            /// </summary>
            public static Passive Evocation = new Passive
            {
                Index = 2,
                Name = "Evocation",
                SNOPower = SNOPower.Wizard_Passive_Evocation,
                RequiredLevel = 13,
                Slug = "evocation",
                IconSlug = "wizard_passive_evocation",
                Description = "Reduce all cooldowns by 20%.",
                Tooltip = "skill/wizard/evocation",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// Increase all damage done by 15%, but decrease Armor and resistances by 10%.
            /// </summary>
            public static Passive GlassCannon = new Passive
            {
                Index = 3,
                Name = "Glass Cannon",
                SNOPower = SNOPower.Wizard_Passive_GlassCannon,
                RequiredLevel = 16,
                Slug = "glass-cannon",
                IconSlug = "wizard_passive_glasscannon",
                Description = "Increase all damage done by 15%, but decrease Armor and resistances by 10%.",
                Tooltip = "skill/wizard/glass-cannon",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// When you cast a Signature spell, you gain 5 Arcane Power.The following skills are Signature spells: Magic Missile Shock Pulse Spectral Blade Electrocute
            /// </summary>
            public static Passive Prodigy = new Passive
            {
                Index = 4,
                Name = "Prodigy",
                SNOPower = SNOPower.Wizard_Passive_Prodigy,
                RequiredLevel = 20,
                Slug = "prodigy",
                IconSlug = "wizard_passive_prodigy",
                Description =
                    "When you cast a Signature spell, you gain 5 Arcane Power.The following skills are Signature spells: Magic Missile Shock Pulse Spectral Blade Electrocute",
                Tooltip = "skill/wizard/prodigy",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// Increase your maximum Arcane Power by 20 and Arcane Power regeneration by 2.5 per second.
            /// </summary>
            public static Passive AstralPresence = new Passive
            {
                Index = 5,
                Name = "Astral Presence",
                SNOPower = SNOPower.Wizard_Passive_AstralPresence,
                RequiredLevel = 24,
                Slug = "astral-presence",
                IconSlug = "wizard_passive_astralpresence",
                Description =
                    "Increase your maximum Arcane Power by 20 and Arcane Power regeneration by 2.5 per second.",
                Tooltip = "skill/wizard/astral-presence",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// When you take more than 15% of your maximum Life in damage within 1 second, the cooldowns on Mirror Image, Slow Time, and Teleport are reset.When you use Mirror Image, Slow Time, or Teleport, your movement speed is increased by 30% for 3 seconds.
            /// </summary>
            public static Passive Illusionist = new Passive
            {
                Index = 6,
                Name = "Illusionist",
                SNOPower = SNOPower.Wizard_Passive_Illusionist,
                RequiredLevel = 27,
                Slug = "illusionist",
                IconSlug = "wizard_passive_illusionist",
                Description =
                    "When you take more than 15% of your maximum Life in damage within 1 second, the cooldowns on Mirror Image, Slow Time, and Teleport are reset.When you use Mirror Image, Slow Time, or Teleport, your movement speed is increased by 30% for 3 seconds.",
                Tooltip = "skill/wizard/illusionist",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// Enemies chilled or frozen by you take 10% more damage from all sources for the duration of the chill or Freeze.
            /// </summary>
            public static Passive ColdBlooded = new Passive
            {
                Index = 7,
                Name = "Cold Blooded",
                SNOPower = SNOPower.Wizard_Passive_ColdBlooded,
                RequiredLevel = 30,
                Slug = "cold-blooded",
                IconSlug = "wizard_passive_coldblooded",
                Description =
                    "Enemies chilled or frozen by you take 10% more damage from all sources for the duration of the chill or Freeze.",
                Tooltip = "skill/wizard/cold-blooded",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// Fire damage dealt to enemies applies a burning effect, increasing their chance to be critically hit by 6% for 3 seconds.
            /// </summary>
            public static Passive Conflagration = new Passive
            {
                Index = 8,
                Name = "Conflagration",
                SNOPower = SNOPower.Wizard_Passive_Conflagration,
                RequiredLevel = 34,
                Slug = "conflagration",
                IconSlug = "wizard_passive_conflagration",
                Description =
                    "Fire damage dealt to enemies applies a burning effect, increasing their chance to be critically hit by 6% for 3 seconds.",
                Tooltip = "skill/wizard/conflagration",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// Lightning spells have a 15% chance to Stun all targets hit for 1.5 seconds.
            /// </summary>
            public static Passive Paralysis = new Passive
            {
                Index = 9,
                Name = "Paralysis",
                SNOPower = SNOPower.Wizard_Passive_Paralysis,
                RequiredLevel = 37,
                Slug = "paralysis",
                IconSlug = "wizard_passive_paralysis",
                Description = "Lightning spells have a 15% chance to Stun all targets hit for 1.5 seconds.",
                Tooltip = "skill/wizard/paralysis",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// As long as you have not taken damage in the last 5 seconds you gain a protective shield that absorbs the next 60% of your Life in damage.
            /// </summary>
            public static Passive GalvanizingWard = new Passive
            {
                Index = 10,
                Name = "Galvanizing Ward",
                SNOPower = SNOPower.Wizard_Passive_GalvanizingWard,
                RequiredLevel = 40,
                Slug = "galvanizing-ward",
                IconSlug = "wizard_passive_galvanizingward",
                Description =
                    "As long as you have not taken damage in the last 5 seconds you gain a protective shield that absorbs the next 60% of your Life in damage.",
                Tooltip = "skill/wizard/galvanizing-ward",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// Enemies that take Arcane damage are slowed by 80% for 2 seconds.
            /// </summary>
            public static Passive TemporalFlux = new Passive
            {
                Index = 11,
                Name = "Temporal Flux",
                SNOPower = SNOPower.Wizard_Passive_TemporalFlux,
                RequiredLevel = 45,
                Slug = "temporal-flux",
                IconSlug = "wizard_passive_temporalflux",
                Description = "Enemies that take Arcane damage are slowed by 80% for 2 seconds.",
                Tooltip = "skill/wizard/temporal-flux",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// Killing an enemy grants a shield that absorbs 2% of your Life in damage for 3 seconds. This effect can stack up to 10 times.Refreshing Dominance will set the shield to its maximum possible potency and each stack will increase its total duration by 0.5 seconds.
            /// </summary>
            public static Passive Dominance = new Passive
            {
                Index = 12,
                Name = "Dominance",
                SNOPower = SNOPower.x1_Wizard_Passive_ArcaneAegis,
                RequiredLevel = 50,
                Slug = "dominance",
                IconSlug = "x1_wizard_passive_arcaneaegis",
                Description =
                    "Killing an enemy grants a shield that absorbs 2% of your Life in damage for 3 seconds. This effect can stack up to 10 times.Refreshing Dominance will set the shield to its maximum possible potency and each stack will increase its total duration by 0.5 seconds.",
                Tooltip = "skill/wizard/dominance",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// When you cast a Signature spell, you gain a Flash of Insight. After 5 Flashes of Insight, your next non-Signature spell deals 60% additional damage. The following skills are Signature spells: Magic Missile Shock Pulse Spectral Blade Electrocute
            /// </summary>
            public static Passive ArcaneDynamo = new Passive
            {
                Index = 13,
                Name = "Arcane Dynamo",
                SNOPower = SNOPower.Wizard_Passive_ArcaneDynamo,
                RequiredLevel = 55,
                Slug = "arcane-dynamo",
                IconSlug = "wizard_passive_arcanedynamo",
                Description =
                    "When you cast a Signature spell, you gain a Flash of Insight. After 5 Flashes of Insight, your next non-Signature spell deals 60% additional damage. The following skills are Signature spells: Magic Missile Shock Pulse Spectral Blade Electrocute",
                Tooltip = "skill/wizard/arcane-dynamo",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// When you receive fatal damage, you instead gain a shield equal to 400% of your maximum Life for 5 seconds and release a shockwave that knocks enemies back and Stuns them for 3 seconds. This effect may occur once every 60 seconds.
            /// </summary>
            public static Passive UnstableAnomaly = new Passive
            {
                Index = 14,
                Name = "Unstable Anomaly",
                SNOPower = SNOPower.Wizard_Passive_UnstableAnomaly,
                RequiredLevel = 60,
                Slug = "unstable-anomaly",
                IconSlug = "wizard_passive_unstableanomaly",
                Description =
                    "When you receive fatal damage, you instead gain a shield equal to 400% of your maximum Life for 5 seconds and release a shockwave that knocks enemies back and Stuns them for 3 seconds. This effect may occur once every 60 seconds.",
                Tooltip = "skill/wizard/unstable-anomaly",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// Standing still for 1.5 seconds increases the following attributes: Armor: 20% All Resistances: 20% Damage: 10%
            /// </summary>
            public static Passive UnwaveringWill = new Passive
            {
                Index = 15,
                Name = "Unwavering Will",
                SNOPower = SNOPower.X1_Wizard_Passive_UnwaveringWill,
                RequiredLevel = 64,
                Slug = "unwavering-will",
                IconSlug = "x1_wizard_passive_unwaveringwill",
                Description =
                    "Standing still for 1.5 seconds increases the following attributes: Armor: 20% All Resistances: 20% Damage: 10%",
                Tooltip = "skill/wizard/unwavering-will",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// You deal 30% additional damage to enemies within 15 yards.
            /// </summary>
            public static Passive Audacity = new Passive
            {
                Index = 16,
                Name = "Audacity",
                SNOPower = SNOPower.X1_Wizard_Passive_Audacity,
                RequiredLevel = 66,
                Slug = "audacity",
                IconSlug = "x1_wizard_passive_audacity",
                Description = "You deal 30% additional damage to enemies within 15 yards.",
                Tooltip = "skill/wizard/audacity",
                Class = ActorClass.Wizard
            };

            /// <summary>
            /// Damaging enemies with Arcane, Cold, Fire or Lightning will cause them to take 5% more damage from all sources for 5 seconds. Each different damage type applies a stack, stacking up to 4 times.Elemental damage from your weapon contributes to Elemental Exposure.
            /// </summary>
            public static Passive ElementalExposure = new Passive
            {
                Index = 17,
                Name = "Elemental Exposure",
                SNOPower = SNOPower.X1_Wizard_Passive_ElementalExposure,
                RequiredLevel = 68,
                Slug = "elemental-exposure",
                IconSlug = "x1_wizard_passive_elementalexposure",
                Description =
                    "Damaging enemies with Arcane, Cold, Fire or Lightning will cause them to take 5% more damage from all sources for 5 seconds. Each different damage type applies a stack, stacking up to 4 times.Elemental damage from your weapon contributes to Elemental Exposure.",
                Tooltip = "skill/wizard/elemental-exposure",
                Class = ActorClass.Wizard
            };
        }

        public class Barbarian : FieldCollection<Barbarian, Passive>
        {
            /// <summary>
            /// When you are healed by a health globe, gain 2% Life regeneration per second and 4% increased movement speed for 15 seconds. This bonus stacks up to 5 times.
            /// </summary>
            public static Passive PoundOfFlesh = new Passive
            {
                Index = 0,
                Name = "Pound of Flesh",
                SNOPower = SNOPower.Barbarian_Passive_PoundOfFlesh,
                RequiredLevel = 10,
                Slug = "pound-of-flesh",
                IconSlug = "barbarian_passive_poundofflesh",
                Description =
                    "When you are healed by a health globe, gain 2% Life regeneration per second and 4% increased movement speed for 15 seconds. This bonus stacks up to 5 times.",
                Tooltip = "skill/barbarian/pound-of-flesh",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// You deal 40% additional damage to enemies below 30% health.
            /// </summary>
            public static Passive Ruthless = new Passive
            {
                Index = 1,
                Name = "Ruthless",
                SNOPower = SNOPower.Barbarian_Passive_Ruthless,
                RequiredLevel = 10,
                Slug = "ruthless",
                IconSlug = "barbarian_passive_ruthless",
                Description = "You deal 40% additional damage to enemies below 30% health.",
                Tooltip = "skill/barbarian/ruthless",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// Fatal damage instead reduces you to 15% Life. For 3 seconds afterward, you take 95% reduced damage and are immune to all control-impairing effects.This effect may occur once every 60 seconds.
            /// </summary>
            public static Passive NervesOfSteel = new Passive
            {
                Index = 2,
                Name = "Nerves of Steel",
                SNOPower = SNOPower.Barbarian_Passive_NervesOfSteel,
                RequiredLevel = 13,
                Slug = "nerves-of-steel",
                IconSlug = "barbarian_passive_nervesofsteel",
                Description =
                    "Fatal damage instead reduces you to 15% Life. For 3 seconds afterward, you take 95% reduced damage and are immune to all control-impairing effects.This effect may occur once every 60 seconds.",
                Tooltip = "skill/barbarian/nerves-of-steel",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// Gain a bonus based on the weapon type of your main hand weapon:Swords/Daggers: 8% increased damageMaces/Axes: 5% Critical Hit ChancePolearms/Spears: 8% attack speedMighty Weapons: 2 Fury per hit
            /// </summary>
            public static Passive WeaponsMaster = new Passive
            {
                Index = 3,
                Name = "Weapons Master",
                SNOPower = SNOPower.Barbarian_Passive_WeaponsMaster,
                RequiredLevel = 16,
                Slug = "weapons-master",
                IconSlug = "barbarian_passive_weaponsmaster",
                Description =
                    "Gain a bonus based on the weapon type of your main hand weapon:Swords/Daggers: 8% increased damageMaces/Axes: 5% Critical Hit ChancePolearms/Spears: 8% attack speedMighty Weapons: 2 Fury per hit",
                Tooltip = "skill/barbarian/weapons-master",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// The duration of your shouts is doubled. After using a shout you and all allies within 100 yards regenerate 3% of maximum Life per second for 120 seconds.Your shouts are: Battle Rage Threatening Shout War Cry
            /// </summary>
            public static Passive InspiringPresence = new Passive
            {
                Index = 4,
                Name = "Inspiring Presence",
                SNOPower = SNOPower.Barbarian_Passive_InspiringPresence,
                RequiredLevel = 20,
                Slug = "inspiring-presence",
                IconSlug = "barbarian_passive_inspiringpresence",
                Description =
                    "The duration of your shouts is doubled. After using a shout you and all allies within 100 yards regenerate 3% of maximum Life per second for 120 seconds.Your shouts are: Battle Rage Threatening Shout War Cry",
                Tooltip = "skill/barbarian/inspiring-presence",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// You deal 25% additional damage while near maximum Fury.
            /// </summary>
            public static Passive BerserkerRage = new Passive
            {
                Index = 5,
                Name = "Berserker Rage",
                SNOPower = SNOPower.Barbarian_Passive_BerserkerRage,
                RequiredLevel = 20,
                Slug = "berserker-rage",
                IconSlug = "barbarian_passive_berserkerrage",
                Description = "You deal 25% additional damage while near maximum Fury.",
                Tooltip = "skill/barbarian/berserker-rage",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// Each point of Fury spent heals you for 966 Life.Heal amount is increased by 1% of your Health Globe Healing Bonus.
            /// </summary>
            public static Passive Bloodthirst = new Passive
            {
                Index = 6,
                Name = "Bloodthirst",
                SNOPower = SNOPower.Barbarian_Passive_Bloodthirst,
                RequiredLevel = 24,
                Slug = "bloodthirst",
                IconSlug = "barbarian_passive_bloodthirst",
                Description =
                    "Each point of Fury spent heals you for 966 Life.Heal amount is increased by 1% of your Health Globe Healing Bonus.",
                Tooltip = "skill/barbarian/bloodthirst",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// Increase all Fury generation by 10%.Increase maximum Fury by 20.Fury is used to fuel your most powerful attacks.
            /// </summary>
            public static Passive Animosity = new Passive
            {
                Index = 7,
                Name = "Animosity",
                SNOPower = SNOPower.Barbarian_Passive_Animosity,
                RequiredLevel = 27,
                Slug = "animosity",
                IconSlug = "barbarian_passive_animosity",
                Description =
                    "Increase all Fury generation by 10%.Increase maximum Fury by 20.Fury is used to fuel your most powerful attacks.",
                Tooltip = "skill/barbarian/animosity",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// Reduce all non-Physical damage by 20%. When you take damage from a ranged or elemental attack, you have a chance to gain 2 Fury.
            /// </summary>
            public static Passive Superstition = new Passive
            {
                Index = 8,
                Name = "Superstition",
                SNOPower = SNOPower.Barbarian_Passive_Superstition,
                RequiredLevel = 30,
                Slug = "superstition",
                IconSlug = "barbarian_passive_superstition",
                Description =
                    "Reduce all non-Physical damage by 20%. When you take damage from a ranged or elemental attack, you have a chance to gain 2 Fury.",
                Tooltip = "skill/barbarian/superstition",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// Increase Armor by 25%.Increase Thorns damage dealt by 50%.
            /// </summary>
            public static Passive ToughAsNails = new Passive
            {
                Index = 9,
                Name = "Tough as Nails",
                SNOPower = SNOPower.Barbarian_Passive_ToughAsNails,
                RequiredLevel = 30,
                Slug = "tough-as-nails",
                IconSlug = "barbarian_passive_toughasnails",
                Description = "Increase Armor by 25%.Increase Thorns damage dealt by 50%.",
                Tooltip = "skill/barbarian/tough-as-nails",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// Increase the damage of Weapon Throw, Seismic Slam, Ancient Spear, and Avalanche by 30% against enemies more than 15 yards away from you.
            /// </summary>
            public static Passive NoEscape = new Passive
            {
                Index = 10,
                Name = "No Escape",
                SNOPower = SNOPower.Barbarian_Passive_NoEscape,
                RequiredLevel = 35,
                Slug = "no-escape",
                IconSlug = "barbarian_passive_noescape",
                Description =
                    "Increase the damage of Weapon Throw, Seismic Slam, Ancient Spear, and Avalanche by 30% against enemies more than 15 yards away from you.",
                Tooltip = "skill/barbarian/no-escape",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// While below 35% Life, all skills cost 50% less Fury, Life per Fury Spent is doubled, and all damage taken is reduced by 50%.
            /// </summary>
            public static Passive Relentless = new Passive
            {
                Index = 11,
                Name = "Relentless",
                SNOPower = SNOPower.Barbarian_Passive_Relentless,
                RequiredLevel = 40,
                Slug = "relentless",
                IconSlug = "barbarian_passive_relentless",
                Description =
                    "While below 35% Life, all skills cost 50% less Fury, Life per Fury Spent is doubled, and all damage taken is reduced by 50%.",
                Tooltip = "skill/barbarian/relentless",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// As long as there are 3 enemies within 12 yards, all of your damage is increased by 20%.
            /// </summary>
            public static Passive Brawler = new Passive
            {
                Index = 12,
                Name = "Brawler",
                SNOPower = SNOPower.Barbarian_Passive_Brawler,
                RequiredLevel = 45,
                Slug = "brawler",
                IconSlug = "barbarian_passive_brawler",
                Description = "As long as there are 3 enemies within 12 yards, all of your damage is increased by 20%.",
                Tooltip = "skill/barbarian/brawler",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// The duration of control-impairing effects on you are reduced by 50%. In addition, whenever a Stun, Freeze, Fear, or Immobilize is cast on you, you have a chance to recover 20% of your maximum Life.
            /// </summary>
            public static Passive Juggernaut = new Passive
            {
                Index = 13,
                Name = "Juggernaut",
                SNOPower = SNOPower.Barbarian_Passive_Juggernaut,
                RequiredLevel = 50,
                Slug = "juggernaut",
                IconSlug = "barbarian_passive_juggernaut",
                Description =
                    "The duration of control-impairing effects on you are reduced by 50%. In addition, whenever a Stun, Freeze, Fear, or Immobilize is cast on you, you have a chance to recover 20% of your maximum Life.",
                Tooltip = "skill/barbarian/juggernaut",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// You no longer degenerate Fury. Instead, you generate 2 Fury every 1 seconds.
            /// </summary>
            public static Passive Unforgiving = new Passive
            {
                Index = 14,
                Name = "Unforgiving",
                SNOPower = SNOPower.Barbarian_Passive_Unforgiving,
                RequiredLevel = 55,
                Slug = "unforgiving",
                IconSlug = "barbarian_passive_unforgiving",
                Description = "You no longer degenerate Fury. Instead, you generate 2 Fury every 1 seconds.",
                Tooltip = "skill/barbarian/unforgiving",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// Reduce the cooldowns of your: Earthquake by 15 seconds. Call of the Ancients by 30 seconds. Wrath of the Berserker by 30 seconds.
            /// </summary>
            public static Passive BoonOfBulkathos = new Passive
            {
                Index = 15,
                Name = "Boon of Bul-Kathos",
                SNOPower = SNOPower.Barbarian_Passive_BoonOfBulKathos,
                RequiredLevel = 60,
                Slug = "boon-of-bulkathos",
                IconSlug = "barbarian_passive_boonofbulkathos",
                Description =
                    "Reduce the cooldowns of your: Earthquake by 15 seconds. Call of the Ancients by 30 seconds. Wrath of the Berserker by 30 seconds.",
                Tooltip = "skill/barbarian/boon-of-bulkathos",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// Gain 30 Fury when Avalanche or Earthquake is triggered.
            /// </summary>
            public static Passive EarthenMight = new Passive
            {
                Index = 16,
                Name = "Earthen Might",
                SNOPower = SNOPower.X1_Barbarian_Passive_EarthenMight,
                RequiredLevel = 64,
                Slug = "earthen-might",
                IconSlug = "x1_barbarian_passive_earthenmight",
                Description = "Gain 30 Fury when Avalanche or Earthquake is triggered.",
                Tooltip = "skill/barbarian/earthen-might",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// While wielding a shield, reduce all damage taken by 30% and reduce Fury costs by 20%.
            /// </summary>
            public static Passive SwordAndBoard = new Passive
            {
                Index = 17,
                Name = "Sword and Board",
                SNOPower = SNOPower.X1_Barbarian_Passive_SwordAndBoard,
                RequiredLevel = 66,
                Slug = "sword-and-board",
                IconSlug = "x1_barbarian_passive_swordandboard",
                Description = "While wielding a shield, reduce all damage taken by 30% and reduce Fury costs by 20%.",
                Tooltip = "skill/barbarian/sword-and-board",
                Class = ActorClass.Barbarian
            };

            /// <summary>
            /// Increase Strength by 1% for 8 seconds after killing or assisting in killing an enemy. This effect stacks up to 25 times.
            /// </summary>
            public static Passive Rampage = new Passive
            {
                Index = 18,
                Name = "Rampage",
                SNOPower = SNOPower.X1_Barbarian_Passive_Rampage,
                RequiredLevel = 68,
                Slug = "rampage",
                IconSlug = "x1_barbarian_passive_rampage",
                Description =
                    "Increase Strength by 1% for 8 seconds after killing or assisting in killing an enemy. This effect stacks up to 25 times.",
                Tooltip = "skill/barbarian/rampage",
                Class = ActorClass.Barbarian
            };
        }

        #endregion
    }
}