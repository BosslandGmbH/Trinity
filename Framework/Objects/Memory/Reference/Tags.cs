//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Trinity.Framework.Objects.Enums;
//using Trinity.Framework.Objects.Memory.Misc;

//namespace Trinity.Framework.Objects.Memory.Reference
//{
//    public class TagReference
//    {
//        public int Id { get; set; }
//        public TagType TagType { get; set; }
//        public MapDataType DataType { get; set; }
//        public string DisplayName { get; set; }
//        public string InternalName { get; set; }
//    }

//    public class Tags 
//    {
//        public static TagReference GetTag(TagType type)
//        {
//            TagReference item;
//            return Items.TryGetValue((int)type, out item) ? item : null;         
//        }

//        public static TagReference GetTag(int index)
//        {
//            TagReference item;
//            return Items.TryGetValue(index, out item) ? item : null;
//        }

//        public static Dictionary<int, TagReference> Items { get; } = new Dictionary<int, TagReference>
//        {
//            {
//                330944, new TagReference
//                {
//                    Id = 330944,
//                    TagType = TagType.TAG_POWER_SLOW_TIME_AMOUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Slow Time Amount",
//                    InternalName = "TAG_POWER_SLOW_TIME_AMOUNT",
//                }
//            },
//            {
//                330128, new TagReference
//                {
//                    Id = 330128,
//                    TagType = TagType.TAG_POWER_LIGHTNING_DAMAGE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Lightning Damage Delta",
//                    InternalName = "TAG_POWER_LIGHTNING_DAMAGE_DELTA",
//                }
//            },
//            {
//                337920, new TagReference
//                {
//                    Id = 337920,
//                    TagType = TagType.TAG_POWER_TOTEM_MAX_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Totem Max Count",
//                    InternalName = "TAG_POWER_TOTEM_MAX_COUNT",
//                }
//            },
//            {
//                332450, new TagReference
//                {
//                    Id = 332450,
//                    TagType = TagType.TAG_POWER_SKELETON_MAX_TOTAL_SUMMON_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Skeleton Summon Max Total Count",
//                    InternalName = "TAG_POWER_SKELETON_MAX_TOTAL_SUMMON_COUNT",
//                }
//            },
//            {
//                329952, new TagReference
//                {
//                    Id = 329952,
//                    TagType = TagType.TAG_POWER_ARCANE_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Arcane Duration",
//                    InternalName = "TAG_POWER_ARCANE_DURATION",
//                }
//            },
//            {
//                602112, new TagReference
//                {
//                    Id = 602112,
//                    TagType = TagType.TAG_POWER_CLEAVE_NUM_TARGETS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Cleave Num Targets",
//                    InternalName = "TAG_POWER_CLEAVE_NUM_TARGETS",
//                }
//            },
//            {
//                331840, new TagReference
//                {
//                    Id = 331840,
//                    TagType = TagType.TAG_POWER_AURA_AFFECTED_RADIUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Aura Affected Radius",
//                    InternalName = "TAG_POWER_AURA_AFFECTED_RADIUS",
//                }
//            },
//            {
//                330240, new TagReference
//                {
//                    Id = 330240,
//                    TagType = TagType.TAG_POWER_POISON_DAMAGE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Poison Damage Min",
//                    InternalName = "TAG_POWER_POISON_DAMAGE_MIN",
//                }
//            },
//            {
//                688128, new TagReference
//                {
//                    Id = 688128,
//                    TagType = TagType.TAG_POWER_EXPEL_MONSTER_SNO,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Expel Monster Actor",
//                    InternalName = "TAG_POWER_EXPEL_MONSTER_SNO",
//                }
//            },
//            {
//                332181, new TagReference
//                {
//                    Id = 332181,
//                    TagType = TagType.TAG_POWER_FURY_DEGENERATION_START,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fury Degeneration Start (in seconds)",
//                    InternalName = "TAG_POWER_FURY_DEGENERATION_START",
//                }
//            },
//            {
//                332016, new TagReference
//                {
//                    Id = 332016,
//                    TagType = TagType.TAG_POWER_MOVEMENT_SPEED_PERCENT_INCREASE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Movement Speed Percent Increase Delta",
//                    InternalName = "TAG_POWER_MOVEMENT_SPEED_PERCENT_INCREASE_DELTA",
//                }
//            },
//            {
//                682496, new TagReference
//                {
//                    Id = 682496,
//                    TagType = TagType.TAG_POWER_BURN_HOUSE_DAMAGE_PER_SECOND,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Burn House Damage Per Second",
//                    InternalName = "TAG_POWER_BURN_HOUSE_DAMAGE_PER_SECOND",
//                }
//            },
//            {
//                700448, new TagReference
//                {
//                    Id = 700448,
//                    TagType = TagType.TAG_POWER_BONUS_HEALING_RECEIVED_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Bonus Healing Received Percent",
//                    InternalName = "TAG_POWER_BONUS_HEALING_RECEIVED_PERCENT",
//                }
//            },
//            {
//                362496, new TagReference
//                {
//                    Id = 362496,
//                    TagType = TagType.TAG_POWER_PET_SEARCH_RADIUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Pet Search Radius",
//                    InternalName = "TAG_POWER_PET_SEARCH_RADIUS",
//                }
//            },
//            {
//                332304, new TagReference
//                {
//                    Id = 332304,
//                    TagType = TagType.TAG_POWER_DISINTEGRATE_BEAM_WIDTH,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Disintegrate Beam Width",
//                    InternalName = "TAG_POWER_DISINTEGRATE_BEAM_WIDTH",
//                }
//            },
//            {
//                594688, new TagReference
//                {
//                    Id = 594688,
//                    TagType = TagType.TAG_POWER_ZOMBIFY_DAMAGE_PER_SECOND,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Zombify Damage Per Second",
//                    InternalName = "TAG_POWER_ZOMBIFY_DAMAGE_PER_SECOND",
//                }
//            },
//            {
//                720896, new TagReference
//                {
//                    Id = 720896,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_ACTOR_0,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Death Portal Actor 0",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_ACTOR_0",
//                }
//            },
//            {
//                331057, new TagReference
//                {
//                    Id = 331057,
//                    TagType = TagType.TAG_POWER_CHARGED_BOLT_SPREAD_ANGLE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Charged Bolt Spread Angle",
//                    InternalName = "TAG_POWER_CHARGED_BOLT_SPREAD_ANGLE",
//                }
//            },
//            {
//                327728, new TagReference
//                {
//                    Id = 327728,
//                    TagType = TagType.TAG_POWER_SPELL_FUNC_1,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "SpellFunc 1",
//                    InternalName = "TAG_POWER_SPELL_FUNC_1",
//                }
//            },
//            {
//                594176, new TagReference
//                {
//                    Id = 594176,
//                    TagType = TagType.TAG_POWER_ZOMBIFY_POSSESSED_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Zombify Possessed Duration",
//                    InternalName = "TAG_POWER_ZOMBIFY_POSSESSED_DURATION",
//                }
//            },
//            {
//                655393, new TagReference
//                {
//                    Id = 655393,
//                    TagType = TagType.TAG_POWER_MANAREGEN_MANA_RESTORE_PERCENT_PER_SECOND,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mana Regen Restore % Per Second",
//                    InternalName = "TAG_POWER_MANAREGEN_MANA_RESTORE_PERCENT_PER_SECOND",
//                }
//            },
//            {
//                262480, new TagReference
//                {
//                    Id = 262480,
//                    TagType = TagType.TAG_POWER_USES_WEAPON_PROJECTILE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Uses Weapon Projectile",
//                    InternalName = "TAG_POWER_USES_WEAPON_PROJECTILE",
//                }
//            },
//            {
//                712704, new TagReference
//                {
//                    Id = 712704,
//                    TagType = TagType.TAG_POWER_BURROW_HIDES_ACTOR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Burrow Hides Actor",
//                    InternalName = "TAG_POWER_BURROW_HIDES_ACTOR",
//                }
//            },
//            {
//                332338, new TagReference
//                {
//                    Id = 332338,
//                    TagType = TagType.TAG_POWER_SPECIAL_WALK_TRAJECTORY_HEIGHT_IS_RELATIVE_TO_MAX,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Special Walk Height Is Relative To Max",
//                    InternalName = "TAG_POWER_SPECIAL_WALK_TRAJECTORY_HEIGHT_IS_RELATIVE_TO_MAX",
//                }
//            },
//            {
//                393216, new TagReference
//                {
//                    Id = 393216,
//                    TagType = TagType.TAG_POWER_SPECTRALBLADE_BLEED_DMG,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Spectral Blade Bleed Damage",
//                    InternalName = "TAG_POWER_SPECTRALBLADE_BLEED_DMG",
//                }
//            },
//            {
//                369920, new TagReference
//                {
//                    Id = 369920,
//                    TagType = TagType.TAG_POWER_FEASTOFSOULS_DAMAGE_PER_SECOND,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Feast of Souls Damage Per Second",
//                    InternalName = "TAG_POWER_FEASTOFSOULS_DAMAGE_PER_SECOND",
//                }
//            },
//            {
//                684802, new TagReference
//                {
//                    Id = 684802,
//                    TagType = TagType.TAG_POWER_TORNADO_TIME_BETWEEN_DIR_CHANGE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Tornado Time To Dir Change Min",
//                    InternalName = "TAG_POWER_TORNADO_TIME_BETWEEN_DIR_CHANGE_MIN",
//                }
//            },
//            {
//                327872, new TagReference
//                {
//                    Id = 327872,
//                    TagType = TagType.TAG_POWER_IS_LOBBED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsLobbed",
//                    InternalName = "TAG_POWER_IS_LOBBED",
//                }
//            },
//            {
//                330880, new TagReference
//                {
//                    Id = 330880,
//                    TagType = TagType.TAG_POWER_IMMOBILIZE_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Immobilize Duration Delta",
//                    InternalName = "TAG_POWER_IMMOBILIZE_DURATION_DELTA",
//                }
//            },
//            {
//                331664, new TagReference
//                {
//                    Id = 331664,
//                    TagType = TagType.TAG_POWER_WHIRLWIND_KNOCKBACK_MAGNITUDE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Whirlwind Knockback Magnitude",
//                    InternalName = "TAG_POWER_WHIRLWIND_KNOCKBACK_MAGNITUDE",
//                }
//            },
//            {
//                330064, new TagReference
//                {
//                    Id = 330064,
//                    TagType = TagType.TAG_POWER_FIRE_DAMAGE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fire Damage Delta",
//                    InternalName = "TAG_POWER_FIRE_DAMAGE_DELTA",
//                }
//            },
//            {
//                329888, new TagReference
//                {
//                    Id = 329888,
//                    TagType = TagType.TAG_POWER_ATTACK_RATING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Attack Rating",
//                    InternalName = "TAG_POWER_ATTACK_RATING",
//                }
//            },
//            {
//                651264, new TagReference
//                {
//                    Id = 651264,
//                    TagType = TagType.TAG_POWER_RESURRECTION_HEALTH_MULT_TO_START,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Resurrection Health Multiplier To Start",
//                    InternalName = "TAG_POWER_RESURRECTION_HEALTH_MULT_TO_START",
//                }
//            },
//            {
//                332896, new TagReference
//                {
//                    Id = 332896,
//                    TagType = TagType.TAG_POWER_RING_OF_FROST_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Ring of Frost Ring Lifetime",
//                    InternalName = "TAG_POWER_RING_OF_FROST_DURATION",
//                }
//            },
//            {
//                417792, new TagReference
//                {
//                    Id = 417792,
//                    TagType = TagType.TAG_POWER_RAPIDFIRE_ATTACK_SPEED_BONUS_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Rapid Fire Attack Speed Bonus Percent",
//                    InternalName = "TAG_POWER_RAPIDFIRE_ATTACK_SPEED_BONUS_PERCENT",
//                }
//            },
//            {
//                361536, new TagReference
//                {
//                    Id = 361536,
//                    TagType = TagType.TAG_POWER_NO_REFLECT_BUFFER_TIME,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Whirlwind Reflect Disabled Timer",
//                    InternalName = "TAG_POWER_NO_REFLECT_BUFFER_TIME",
//                }
//            },
//            {
//                684544, new TagReference
//                {
//                    Id = 684544,
//                    TagType = TagType.TAG_POWER_FALLEN_CHAMPION_LEADER_SHOUT_EFFECT_RADIUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fallen Champion Leader Shout Effect Radius",
//                    InternalName = "TAG_POWER_FALLEN_CHAMPION_LEADER_SHOUT_EFFECT_RADIUS",
//                }
//            },
//            {
//                630786, new TagReference
//                {
//                    Id = 630786,
//                    TagType = TagType.TAG_POWER_SAND_SHIELD_PROJECTILE_BLOCK_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Sand Shield Projectile Block Chance",
//                    InternalName = "TAG_POWER_SAND_SHIELD_PROJECTILE_BLOCK_CHANCE",
//                }
//            },
//            {
//                262434, new TagReference
//                {
//                    Id = 262434,
//                    TagType = TagType.TAG_POWER_ROPE_GROUND_SNO,
//                    DataType = MapDataType.RopeChainSno,
//                    DisplayName = "Rope Ground",
//                    InternalName = "TAG_POWER_ROPE_GROUND_SNO",
//                }
//            },
//            {
//                330448, new TagReference
//                {
//                    Id = 330448,
//                    TagType = TagType.TAG_POWER_SIEGE_DAMAGE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Siege Damage Delta",
//                    InternalName = "TAG_POWER_SIEGE_DAMAGE_DELTA",
//                }
//            },
//            {
//                339200, new TagReference
//                {
//                    Id = 339200,
//                    TagType = TagType.TAG_POWER_DEATH_STATUE_GHOUL_BUFF_RANGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Death Statue Ghoul Buff Range",
//                    InternalName = "TAG_POWER_DEATH_STATUE_GHOUL_BUFF_RANGE",
//                }
//            },
//            {
//                331232, new TagReference
//                {
//                    Id = 331232,
//                    TagType = TagType.TAG_POWER_FREEZE_DAMAGE_DONE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Freeze Damage Done Min",
//                    InternalName = "TAG_POWER_FREEZE_DAMAGE_DONE_MIN",
//                }
//            },
//            {
//                329880, new TagReference
//                {
//                    Id = 329880,
//                    TagType = TagType.TAG_POWER_LOBBED_HEIGHT_ABOVE_SOURCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Height Above Source",
//                    InternalName = "TAG_POWER_LOBBED_HEIGHT_ABOVE_SOURCE",
//                }
//            },
//            {
//                684805, new TagReference
//                {
//                    Id = 684805,
//                    TagType = TagType.TAG_POWER_TORNADO_WANDER_RADIUS_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Tornado Wander Radius Min",
//                    InternalName = "TAG_POWER_TORNADO_WANDER_RADIUS_MIN",
//                }
//            },
//            {
//                328613, new TagReference
//                {
//                    Id = 328613,
//                    TagType = TagType.TAG_POWER_TARGET_NORMAL_MONSTERS_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetNormalMonstersOnly",
//                    InternalName = "TAG_POWER_TARGET_NORMAL_MONSTERS_ONLY",
//                }
//            },
//            {
//                655873, new TagReference
//                {
//                    Id = 655873,
//                    TagType = TagType.TAG_POWER_DEATH_PROOF_PERCENT_TO_HEAL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Death Proof Percent To Heal",
//                    InternalName = "TAG_POWER_DEATH_PROOF_PERCENT_TO_HEAL",
//                }
//            },
//            {
//                333296, new TagReference
//                {
//                    Id = 333296,
//                    TagType = TagType.TAG_POWER_MIRROR_IMAGE_THORN_DAMAGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mirror Image Thorn Damage",
//                    InternalName = "TAG_POWER_MIRROR_IMAGE_THORN_DAMAGE",
//                }
//            },
//            {
//                331137, new TagReference
//                {
//                    Id = 331137,
//                    TagType = TagType.TAG_POWER_PLAY_SUMMONED_BY_MONSTER_ANIMATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Play Summoned By Monster Animation",
//                    InternalName = "TAG_POWER_PLAY_SUMMONED_BY_MONSTER_ANIMATION",
//                }
//            },
//            {
//                328112, new TagReference
//                {
//                    Id = 328112,
//                    TagType = TagType.TAG_POWER_TARGET_ENEMIES,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetEnemies",
//                    InternalName = "TAG_POWER_TARGET_ENEMIES",
//                }
//            },
//            {
//                330816, new TagReference
//                {
//                    Id = 330816,
//                    TagType = TagType.TAG_POWER_STUN_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Stun Chance",
//                    InternalName = "TAG_POWER_STUN_CHANCE",
//                }
//            },
//            {
//                330000, new TagReference
//                {
//                    Id = 330000,
//                    TagType = TagType.TAG_POWER_PHYSICAL_DAMAGE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Physical Damage Delta",
//                    InternalName = "TAG_POWER_PHYSICAL_DAMAGE_DELTA",
//                }
//            },
//            {
//                460032, new TagReference
//                {
//                    Id = 460032,
//                    TagType = TagType.TAG_POWER_VANISH_AT_HEALTH_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Vanish At Health Percent",
//                    InternalName = "TAG_POWER_VANISH_AT_HEALTH_PERCENT",
//                }
//            },
//            {
//                332178, new TagReference
//                {
//                    Id = 332178,
//                    TagType = TagType.TAG_POWER_FURY_ADD_PER_INTERVAL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fury Add Per Interval",
//                    InternalName = "TAG_POWER_FURY_ADD_PER_INTERVAL",
//                }
//            },
//            {
//                330608, new TagReference
//                {
//                    Id = 330608,
//                    TagType = TagType.TAG_POWER_DISEASE_RESISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Disease Damage Resistance",
//                    InternalName = "TAG_POWER_DISEASE_RESISTANCE",
//                }
//            },
//            {
//                328224, new TagReference
//                {
//                    Id = 328224,
//                    TagType = TagType.TAG_POWER_CONTACT_FRAME_IN_COOLDOWN,
//                    DataType = MapDataType.Frame,
//                    DisplayName = "ContactFrameType",
//                    InternalName = "TAG_POWER_CONTACT_FRAME_IN_COOLDOWN",
//                }
//            },
//            {
//                331224, new TagReference
//                {
//                    Id = 331224,
//                    TagType = TagType.TAG_POWER_LIFESTEAL_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Lifesteal Percent",
//                    InternalName = "TAG_POWER_LIFESTEAL_PERCENT",
//                }
//            },
//            {
//                332832, new TagReference
//                {
//                    Id = 332832,
//                    TagType = TagType.TAG_POWER_DAMAGE_AURA_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Damage Aura Duration",
//                    InternalName = "TAG_POWER_DAMAGE_AURA_DURATION",
//                }
//            },
//            {
//                328612, new TagReference
//                {
//                    Id = 328612,
//                    TagType = TagType.TAG_POWER_TARGET_IGNORE_LARGE_MONSTERS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetIgnoreLargeMonsters",
//                    InternalName = "TAG_POWER_TARGET_IGNORE_LARGE_MONSTERS",
//                }
//            },
//            {
//                655728, new TagReference
//                {
//                    Id = 655728,
//                    TagType = TagType.TAG_POWER_ARCANE_ARMOR_DEFENSE_BONUS_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Arcane Armor Defense Bonus Percent",
//                    InternalName = "TAG_POWER_ARCANE_ARMOR_DEFENSE_BONUS_PERCENT",
//                }
//            },
//            {
//                328614, new TagReference
//                {
//                    Id = 328614,
//                    TagType = TagType.TAG_POWER_TARGET_UNDEAD_MONSTERS_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetUndeadMonstersOnly",
//                    InternalName = "TAG_POWER_TARGET_UNDEAD_MONSTERS_ONLY",
//                }
//            },
//            {
//                361472, new TagReference
//                {
//                    Id = 361472,
//                    TagType = TagType.TAG_POWER_WHIRLWHIND_RANGE_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Whirlwind Range Bonus",
//                    InternalName = "TAG_POWER_WHIRLWHIND_RANGE_BONUS",
//                }
//            },
//            {
//                331138, new TagReference
//                {
//                    Id = 331138,
//                    TagType = TagType.TAG_POWER_SUMMONED_ACTOR_USE_DEFAULT_LEVEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Summoned Actor Use Default Level",
//                    InternalName = "TAG_POWER_SUMMONED_ACTOR_USE_DEFAULT_LEVEL",
//                }
//            },
//            {
//                655666, new TagReference
//                {
//                    Id = 655666,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_3_WITH_PARAM,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Generic Buff Attribute 3 And Parameter",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_3_WITH_PARAM",
//                }
//            },
//            {
//                332768, new TagReference
//                {
//                    Id = 332768,
//                    TagType = TagType.TAG_POWER_BUFF_DURATION_BETWEEN_PULSES,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Buff Duration Between Pulses",
//                    InternalName = "TAG_POWER_BUFF_DURATION_BETWEEN_PULSES",
//                }
//            },
//            {
//                331168, new TagReference
//                {
//                    Id = 331168,
//                    TagType = TagType.TAG_POWER_THUNDERSTORM_BOLTS_PER_SECOND,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Thunderstorm Bolts Per Second",
//                    InternalName = "TAG_POWER_THUNDERSTORM_BOLTS_PER_SECOND",
//                }
//            },
//            {
//                664320, new TagReference
//                {
//                    Id = 664320,
//                    TagType = TagType.TAG_POWER_STATIC_CHARGE_RADIUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Static Charge Radius",
//                    InternalName = "TAG_POWER_STATIC_CHARGE_RADIUS",
//                }
//            },
//            {
//                332024, new TagReference
//                {
//                    Id = 332024,
//                    TagType = TagType.TAG_POWER_SCALE_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Scale Bonus",
//                    InternalName = "TAG_POWER_SCALE_BONUS",
//                }
//            },
//            {
//                333056, new TagReference
//                {
//                    Id = 333056,
//                    TagType = TagType.TAG_POWER_FURY_TIME_BETWEEN_UPDATES,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fury Time Between Updates",
//                    InternalName = "TAG_POWER_FURY_TIME_BETWEEN_UPDATES",
//                }
//            },
//            {
//                368896, new TagReference
//                {
//                    Id = 368896,
//                    TagType = TagType.TAG_POWER_ROCKWORM_WEB_SLOW_MULTIPLIER,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Rockworm Web Movement Speed Multiplier",
//                    InternalName = "TAG_POWER_ROCKWORM_WEB_SLOW_MULTIPLIER",
//                }
//            },
//            {
//                334235, new TagReference
//                {
//                    Id = 334235,
//                    TagType = TagType.TAG_POWER_FURY_DEGENERATION_REDUCED_DECAY_RATE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fury Reduced Degen. (Per second)",
//                    InternalName = "TAG_POWER_FURY_DEGENERATION_REDUCED_DECAY_RATE",
//                }
//            },
//            {
//                655392, new TagReference
//                {
//                    Id = 655392,
//                    TagType = TagType.TAG_VS_PS4A_FUNC,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mana Regen Time Between Updates",
//                    InternalName = "TAG_POWER_MANAREGEN_TIME_BETWEEN_UPDATES",
//                }
//            },
//            {
//                365312, new TagReference
//                {
//                    Id = 365312,
//                    TagType = TagType.TAG_POWER_GHOST_SOULSIPHON_DAMAGE_PER_SECOND,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Ghost Soulsiphon Damage Per Second",
//                    InternalName = "TAG_POWER_GHOST_SOULSIPHON_DAMAGE_PER_SECOND",
//                }
//            },
//            {
//                330752, new TagReference
//                {
//                    Id = 330752,
//                    TagType = TagType.TAG_POWER_DAMAGE_PERCENT_ALL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Damage Percent All",
//                    InternalName = "TAG_POWER_DAMAGE_PERCENT_ALL",
//                }
//            },
//            {
//                331536, new TagReference
//                {
//                    Id = 331536,
//                    TagType = TagType.TAG_POWER_CURSE_DAMAGE_AMPLIFY_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Curse Damage Amplify Percent",
//                    InternalName = "TAG_POWER_CURSE_DAMAGE_AMPLIFY_PERCENT",
//                }
//            },
//            {
//                700416, new TagReference
//                {
//                    Id = 700416,
//                    TagType = TagType.TAG_POWER_SKELETON_KING_WHIRLWIND_APS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Skeleton King WW APS",
//                    InternalName = "TAG_POWER_SKELETON_KING_WHIRLWIND_APS",
//                }
//            },
//            {
//                329760, new TagReference
//                {
//                    Id = 329760,
//                    TagType = TagType.TAG_POWER_PAYLOAD_TYPE,
//                    DataType = MapDataType.PayloadType,
//                    DisplayName = "Payload Type",
//                    InternalName = "TAG_POWER_PAYLOAD_TYPE",
//                }
//            },
//            {
//                330544, new TagReference
//                {
//                    Id = 330544,
//                    TagType = TagType.TAG_POWER_HOLY_RESISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Holy Damage Resistance",
//                    InternalName = "TAG_POWER_HOLY_RESISTANCE",
//                }
//            },
//            {
//                331160, new TagReference
//                {
//                    Id = 331160,
//                    TagType = TagType.TAG_POWER_RUNE_FIND_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Rune Find Bonus",
//                    InternalName = "TAG_POWER_RUNE_FIND_BONUS",
//                }
//            },
//            {
//                361520, new TagReference
//                {
//                    Id = 361520,
//                    TagType = TagType.TAG_POWER_WHIRLWIND_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Whirlwind Duration Delta",
//                    InternalName = "TAG_POWER_WHIRLWIND_DURATION_DELTA",
//                }
//            },
//            {
//                655633, new TagReference
//                {
//                    Id = 655633,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_1,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Generic Buff Attribute 1 Formula",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_1",
//                }
//            },
//            {
//                338176, new TagReference
//                {
//                    Id = 338176,
//                    TagType = TagType.TAG_POWER_TOTEM_LIFESPAN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Totem Lifespan",
//                    InternalName = "TAG_POWER_TOTEM_LIFESPAN",
//                }
//            },
//            {
//                330208, new TagReference
//                {
//                    Id = 330208,
//                    TagType = TagType.TAG_POWER_COLD_DAMAGE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Cold Damage Percent",
//                    InternalName = "TAG_POWER_COLD_DAMAGE_PERCENT",
//                }
//            },
//            {
//                655664, new TagReference
//                {
//                    Id = 655664,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_3,
//                    DataType = MapDataType.Attribute,
//                    DisplayName = "Generic Buff Attribute 3",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_3",
//                }
//            },
//            {
//                600072, new TagReference
//                {
//                    Id = 600072,
//                    TagType = TagType.TAG_POWER_PLAGUE_OF_TOADS_TOAD_LIFESPAN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Plague of Toads Toad Lifespan",
//                    InternalName = "TAG_POWER_PLAGUE_OF_TOADS_TOAD_LIFESPAN",
//                }
//            },
//            {
//                332816, new TagReference
//                {
//                    Id = 332816,
//                    TagType = TagType.TAG_POWER_ROOT_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Root Duration",
//                    InternalName = "TAG_POWER_ROOT_DURATION",
//                }
//            },
//            {
//                332096, new TagReference
//                {
//                    Id = 332096,
//                    TagType = TagType.TAG_POWER_FURY_GAINED_PER_PERCENT_HEALTH_LOST,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fury Gained Per Percent Health Lost",
//                    InternalName = "TAG_POWER_FURY_GAINED_PER_PERCENT_HEALTH_LOST",
//                }
//            },
//            {
//                332704, new TagReference
//                {
//                    Id = 332704,
//                    TagType = TagType.TAG_POWER_THUNDERSTORM_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Thunderstorm Duration",
//                    InternalName = "TAG_POWER_THUNDERSTORM_DURATION",
//                }
//            },
//            {
//                330320, new TagReference
//                {
//                    Id = 330320,
//                    TagType = TagType.TAG_POWER_ARCANE_DAMAGE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Arcane Damage Delta",
//                    InternalName = "TAG_POWER_ARCANE_DAMAGE_DELTA",
//                }
//            },
//            {
//                331193, new TagReference
//                {
//                    Id = 331193,
//                    TagType = TagType.TAG_POWER_PROJECTILE_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Projectile Count",
//                    InternalName = "TAG_POWER_PROJECTILE_COUNT",
//                }
//            },
//            {
//                405504, new TagReference
//                {
//                    Id = 405504,
//                    TagType = TagType.TAG_POWER_TELEPORT_NUMBER_OF_IMAGES,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Teleport Number of Images",
//                    InternalName = "TAG_POWER_TELEPORT_NUMBER_OF_IMAGES",
//                }
//            },
//            {
//                655872, new TagReference
//                {
//                    Id = 655872,
//                    TagType = TagType.TAG_POWER_DEATH_PROOF_DEBUFF_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Death Proof Debuff Duration",
//                    InternalName = "TAG_POWER_DEATH_PROOF_DEBUFF_DURATION",
//                }
//            },
//            {
//                332272, new TagReference
//                {
//                    Id = 332272,
//                    TagType = TagType.TAG_POWER_PALADIN_RESURRECTION_COOLDOWN_TIME,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Paladin Resurrection Cooldown Time",
//                    InternalName = "TAG_POWER_PALADIN_RESURRECTION_COOLDOWN_TIME",
//                }
//            },
//            {
//                332752, new TagReference
//                {
//                    Id = 332752,
//                    TagType = TagType.TAG_POWER_BUFF_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Buff Duration Delta",
//                    InternalName = "TAG_POWER_BUFF_DURATION_DELTA",
//                }
//            },
//            {
//                684804, new TagReference
//                {
//                    Id = 684804,
//                    TagType = TagType.TAG_POWER_TORNADO_TIME_BETWEEN_DIR_CHANGE_GROWTH,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Tornado Time To Dir Change Min Growth/Change",
//                    InternalName = "TAG_POWER_TORNADO_TIME_BETWEEN_DIR_CHANGE_GROWTH",
//                }
//            },
//            {
//                600064, new TagReference
//                {
//                    Id = 600064,
//                    TagType = TagType.TAG_POWER_PLAGUE_OF_TOADS_NUM_FROGS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Plague of Toads Num Frogs",
//                    InternalName = "TAG_POWER_PLAGUE_OF_TOADS_NUM_FROGS",
//                }
//            },
//            {
//                684864, new TagReference
//                {
//                    Id = 684864,
//                    TagType = TagType.TAG_POWER_LACUNI_LEAP_TARGET_OFFSET,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Lacuni Leap Target Offset",
//                    InternalName = "TAG_POWER_LACUNI_LEAP_TARGET_OFFSET",
//                }
//            },
//            {
//                332480, new TagReference
//                {
//                    Id = 332480,
//                    TagType = TagType.TAG_POWER_CORPULENT_CRYPT_KID_SPAWN_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Num Crypt Kids To Spawn On Corpulent Explosion",
//                    InternalName = "TAG_POWER_CORPULENT_CRYPT_KID_SPAWN_COUNT",
//                }
//            },
//            {
//                647168, new TagReference
//                {
//                    Id = 647168,
//                    TagType = TagType.TAG_POWER_RESURRECTION_BUFF_TIME,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Resurrection Buff Time",
//                    InternalName = "TAG_POWER_RESURRECTION_BUFF_TIME",
//                }
//            },
//            {
//                720976, new TagReference
//                {
//                    Id = 720976,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Death Portal Chance",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_CHANCE",
//                }
//            },
//            {
//                708617, new TagReference
//                {
//                    Id = 708617,
//                    TagType = TagType.TAG_POWER_LEAP_DIST_MAX,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Leap Distance Max",
//                    InternalName = "TAG_POWER_LEAP_DIST_MAX",
//                }
//            },
//            {
//                331710, new TagReference
//                {
//                    Id = 331710,
//                    TagType = TagType.TAG_POWER_KNOCKBACK_POWER,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Knockback Power on Target",
//                    InternalName = "TAG_POWER_KNOCKBACK_POWER",
//                }
//            },
//            {
//                684807, new TagReference
//                {
//                    Id = 684807,
//                    TagType = TagType.TAG_POWER_TORNADO_WANDER_RADIUS_GROWTH,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Tornado Wander Radius Growth/Change",
//                    InternalName = "TAG_POWER_TORNADO_WANDER_RADIUS_GROWTH",
//                }
//            },
//            {
//                708704, new TagReference
//                {
//                    Id = 708704,
//                    TagType = TagType.TAG_POWER_MASTABLASTA_DRAIN_HATRED_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "MastaBlasta Drain Hatred Percent",
//                    InternalName = "TAG_POWER_MASTABLASTA_DRAIN_HATRED_PERCENT",
//                }
//            },
//            {
//                332050, new TagReference
//                {
//                    Id = 332050,
//                    TagType = TagType.TAG_POWER_IMPROVED_BATTLE_RAGE_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Improved Battle Rage Duration",
//                    InternalName = "TAG_POWER_IMPROVED_BATTLE_RAGE_DURATION",
//                }
//            },
//            {
//                430080, new TagReference
//                {
//                    Id = 430080,
//                    TagType = TagType.TAG_POWER_FEIGN_DEATH_UNTARGETABLE_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Feign Death Untargetable Duration",
//                    InternalName = "TAG_POWER_FEIGN_DEATH_UNTARGETABLE_DURATION",
//                }
//            },
//            {
//                680448, new TagReference
//                {
//                    Id = 680448,
//                    TagType = TagType.TAG_POWER_DAMAGE_REDUCTION_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Damage Reduction Percent",
//                    InternalName = "TAG_POWER_DAMAGE_REDUCTION_PERCENT",
//                }
//            },
//            {
//                262403, new TagReference
//                {
//                    Id = 262403,
//                    TagType = TagType.TAG_POWER_ROPE_CHAIN_SNO,
//                    DataType = MapDataType.RopeChainSno,
//                    DisplayName = "Chain Rope",
//                    InternalName = "TAG_POWER_ROPE_CHAIN_SNO",
//                }
//            },
//            {
//                328128, new TagReference
//                {
//                    Id = 328128,
//                    TagType = TagType.TAG_POWER_TARGET_CORPSE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetCorpse",
//                    InternalName = "TAG_POWER_TARGET_CORPSE",
//                }
//            },
//            {
//                264192, new TagReference
//                {
//                    Id = 264192,
//                    TagType = TagType.TAG_POWER_CONTACT_FRAME_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Contact Frame Effect Group - Male",
//                    InternalName = "TAG_POWER_CONTACT_FRAME_EFFECT_GROUP",
//                }
//            },
//            {
//                330144, new TagReference
//                {
//                    Id = 330144,
//                    TagType = TagType.TAG_POWER_LIGHTNING_DAMAGE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Lightning Damage Percent",
//                    InternalName = "TAG_POWER_LIGHTNING_DAMAGE_PERCENT",
//                }
//            },
//            {
//                329968, new TagReference
//                {
//                    Id = 329968,
//                    TagType = TagType.TAG_POWER_HOLY_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Holy Duration",
//                    InternalName = "TAG_POWER_HOLY_DURATION",
//                }
//            },
//            {
//                330624, new TagReference
//                {
//                    Id = 330624,
//                    TagType = TagType.TAG_POWER_DELIVERY_MECHANISM,
//                    DataType = MapDataType.DeliveryMechanism,
//                    DisplayName = "Delivery Mechanism",
//                    InternalName = "TAG_POWER_DELIVERY_MECHANISM",
//                }
//            },
//            {
//                332177, new TagReference
//                {
//                    Id = 332177,
//                    TagType = TagType.TAG_POWER_FURY_DEGENERATION_OUT_OF_COMBAT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fury Degeneration Out of Combat (per second)",
//                    InternalName = "TAG_POWER_FURY_DEGENERATION_OUT_OF_COMBAT",
//                }
//            },
//            {
//                332032, new TagReference
//                {
//                    Id = 332032,
//                    TagType = TagType.TAG_POWER_CRUSHING_BLOW_MAX_BUFF_LEVEL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Crushing Blow Max Buff Level",
//                    InternalName = "TAG_POWER_CRUSHING_BLOW_MAX_BUFF_LEVEL",
//                }
//            },
//            {
//                332640, new TagReference
//                {
//                    Id = 332640,
//                    TagType = TagType.TAG_POWER_SHIELD_DAMAGE_ABSORB_AMOUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Hitpoints That Shield Can Absorb",
//                    InternalName = "TAG_POWER_SHIELD_DAMAGE_ABSORB_AMOUNT",
//                }
//            },
//            {
//                330256, new TagReference
//                {
//                    Id = 330256,
//                    TagType = TagType.TAG_POWER_POISON_DAMAGE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Poison Damage Delta",
//                    InternalName = "TAG_POWER_POISON_DAMAGE_DELTA",
//                }
//            },
//            {
//                639232, new TagReference
//                {
//                    Id = 639232,
//                    TagType = TagType.TAG_POWER_DEATH_NOVA_RADIUS_FT_PER_SEC,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Death Nova Radius Ft Per Sec",
//                    InternalName = "TAG_POWER_DEATH_NOVA_RADIUS_FT_PER_SEC",
//                }
//            },
//            {
//                331040, new TagReference
//                {
//                    Id = 331040,
//                    TagType = TagType.TAG_POWER_PIERCE_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Pierce Chance",
//                    InternalName = "TAG_POWER_PIERCE_CHANCE",
//                }
//            },
//            {
//                332776, new TagReference
//                {
//                    Id = 332776,
//                    TagType = TagType.TAG_POWER_BUFF_MAX_STACK,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Generic Buff Max Stack 0",
//                    InternalName = "TAG_POWER_BUFF_MAX_STACK",
//                }
//            },
//            {
//                684546, new TagReference
//                {
//                    Id = 684546,
//                    TagType = TagType.TAG_POWER_INCOMING_DAMAGE_INCREASE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Incoming Damage Increase",
//                    InternalName = "TAG_POWER_INCOMING_DAMAGE_INCREASE",
//                }
//            },
//            {
//                369024, new TagReference
//                {
//                    Id = 369024,
//                    TagType = TagType.TAG_POWER_ROCKWORM_WEB_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Rockworm Web Duration",
//                    InternalName = "TAG_POWER_ROCKWORM_WEB_DURATION",
//                }
//            },
//            {
//                661760, new TagReference
//                {
//                    Id = 661760,
//                    TagType = TagType.TAG_POWER_LIGHTNING_SPEED_STACK_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Lightning Speed Stack Count",
//                    InternalName = "TAG_POWER_LIGHTNING_SPEED_STACK_COUNT",
//                }
//            },
//            {
//                332688, new TagReference
//                {
//                    Id = 332688,
//                    TagType = TagType.TAG_POWER_DISINTEGRATE_TIME_BETWEEN_UPDATES,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Disintegrate Time Between Updates",
//                    InternalName = "TAG_POWER_DISINTEGRATE_TIME_BETWEEN_UPDATES",
//                }
//            },
//            {
//                655394, new TagReference
//                {
//                    Id = 655394,
//                    TagType = TagType.TAG_POWER_MANAREGEN_TIME_TO_KICK_IN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mana Regen Time To Kick In",
//                    InternalName = "TAG_POWER_MANAREGEN_TIME_TO_KICK_IN",
//                }
//            },
//            {
//                552960, new TagReference
//                {
//                    Id = 552960,
//                    TagType = TagType.TAG_POWER_DOT_DAMAGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "DOT Damage",
//                    InternalName = "TAG_POWER_DOT_DAMAGE",
//                }
//            },
//            {
//                720912, new TagReference
//                {
//                    Id = 720912,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_ACTOR_1,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Death Portal Actor 1",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_ACTOR_1",
//                }
//            },
//            {
//                262406, new TagReference
//                {
//                    Id = 262406,
//                    TagType = TagType.TAG_POWER_TARGET_ATTACKED_SNO,
//                    DataType = MapDataType.TargetSno,
//                    DisplayName = "Target Attacked Particle",
//                    InternalName = "TAG_POWER_TARGET_ATTACKED_SNO",
//                }
//            },
//            {
//                684545, new TagReference
//                {
//                    Id = 684545,
//                    TagType = TagType.TAG_POWER_FALLEN_CHAMPION_LEADER_SHOUT_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fallen Champion Leader Shout Duration",
//                    InternalName = "TAG_POWER_FALLEN_CHAMPION_LEADER_SHOUT_DURATION",
//                }
//            },
//            {
//                369152, new TagReference
//                {
//                    Id = 369152,
//                    TagType = TagType.TAG_POWER_ROCKWORM_BURST_OUT_DISTANCE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Rockworm Burst Out Distance Min",
//                    InternalName = "TAG_POWER_ROCKWORM_BURST_OUT_DISTANCE_MIN",
//                }
//            },
//            {
//                684806, new TagReference
//                {
//                    Id = 684806,
//                    TagType = TagType.TAG_POWER_TORNADO_WANDER_RADIUS_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Tornado Wander Radius Delta",
//                    InternalName = "TAG_POWER_TORNADO_WANDER_RADIUS_DELTA",
//                }
//            },
//            {
//                600322, new TagReference
//                {
//                    Id = 600322,
//                    TagType = TagType.TAG_POWER_FIREBATS_INITIAL_DELAY,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fire Bats Initial Delay",
//                    InternalName = "TAG_POWER_FIREBATS_INITIAL_DELAY",
//                }
//            },
//            {
//                633344, new TagReference
//                {
//                    Id = 633344,
//                    TagType = TagType.TAG_POWER_CAUSES_SUPER_KNOCKBACK,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Causes Super Knockback",
//                    InternalName = "TAG_POWER_CAUSES_SUPER_KNOCKBACK",
//                }
//            },
//            {
//                720977, new TagReference
//                {
//                    Id = 720977,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_CHANCE_SPECIAL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Death Portal Chance (Special)",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_CHANCE_SPECIAL",
//                }
//            },
//            {
//                332362, new TagReference
//                {
//                    Id = 332362,
//                    TagType = TagType.TAG_POWER_SPECIAL_WALK_IS_KNOCKBACK,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Special Walk Is Knockback",
//                    InternalName = "TAG_POWER_SPECIAL_WALK_IS_KNOCKBACK",
//                }
//            },
//            {
//                330896, new TagReference
//                {
//                    Id = 330896,
//                    TagType = TagType.TAG_POWER_SLOW_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Slow Duration Min",
//                    InternalName = "TAG_POWER_SLOW_DURATION_MIN",
//                }
//            },
//            {
//                708688, new TagReference
//                {
//                    Id = 708688,
//                    TagType = TagType.TAG_POWER_MASTABLASTA_DRAIN_SPIRIT_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "MastaBlasta Drain Spirit Percent",
//                    InternalName = "TAG_POWER_MASTABLASTA_DRAIN_SPIRIT_PERCENT",
//                }
//            },
//            {
//                671744, new TagReference
//                {
//                    Id = 671744,
//                    TagType = TagType.TAG_POWER_POWERED_IMPACT_MANA_INCREASE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Powered Impact Mana Increase",
//                    InternalName = "TAG_POWER_POWERED_IMPACT_MANA_INCREASE",
//                }
//            },
//            {
//                330080, new TagReference
//                {
//                    Id = 330080,
//                    TagType = TagType.TAG_POWER_FIRE_DAMAGE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fire Damage Percent",
//                    InternalName = "TAG_POWER_FIRE_DAMAGE_PERCENT",
//                }
//            },
//            {
//                329904, new TagReference
//                {
//                    Id = 329904,
//                    TagType = TagType.TAG_POWER_ATTACK_RATING_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Attack Rating Percent",
//                    InternalName = "TAG_POWER_ATTACK_RATING_PERCENT",
//                }
//            },
//            {
//                330560, new TagReference
//                {
//                    Id = 330560,
//                    TagType = TagType.TAG_POWER_DISEASE_DAMAGE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Disease Damage Min",
//                    InternalName = "TAG_POWER_DISEASE_DAMAGE_MIN",
//                }
//            },
//            {
//                332576, new TagReference
//                {
//                    Id = 332576,
//                    TagType = TagType.TAG_POWER_ENERGY_SHIELD_MANA_COST_PER_DAMAGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Energy Shield Mana Cost Per Damage",
//                    InternalName = "TAG_POWER_ENERGY_SHIELD_MANA_COST_PER_DAMAGE",
//                }
//            },
//            {
//                681604, new TagReference
//                {
//                    Id = 681604,
//                    TagType = TagType.TAG_POWER_SANDSHARK_DELAY_BEFORE_SAND_ATTACK,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Sandshark Delay Before Sand Attack",
//                    InternalName = "TAG_POWER_SANDSHARK_DELAY_BEFORE_SAND_ATTACK",
//                }
//            },
//            {
//                655680, new TagReference
//                {
//                    Id = 655680,
//                    TagType = TagType.TAG_POWER_DEBUFF_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Debuff Duration Min",
//                    InternalName = "TAG_POWER_DEBUFF_DURATION",
//                }
//            },
//            {
//                610304, new TagReference
//                {
//                    Id = 610304,
//                    TagType = TagType.TAG_POWER_HOOKSHOT_HIT_RETURN_SPEED_MULT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Hookshot Hit Return Speed Mult",
//                    InternalName = "TAG_POWER_HOOKSHOT_HIT_RETURN_SPEED_MULT",
//                }
//            },
//            {
//                655618, new TagReference
//                {
//                    Id = 655618,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_0_WITH_PARAM,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Generic Buff Attribute 0 And Parameter",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_0_WITH_PARAM",
//                }
//            },
//            {
//                361216, new TagReference
//                {
//                    Id = 361216,
//                    TagType = TagType.TAG_POWER_WHIRLWHIND_RANGE_MULTIPLIER,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Whirlwind Range Multiplier",
//                    InternalName = "TAG_POWER_WHIRLWHIND_RANGE_MULTIPLIER",
//                }
//            },
//            {
//                332624, new TagReference
//                {
//                    Id = 332624,
//                    TagType = TagType.TAG_POWER_PUMMEL_COOLDOWN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Pummel Cooldown",
//                    InternalName = "TAG_POWER_PUMMEL_COOLDOWN",
//                }
//            },
//            {
//                338432, new TagReference
//                {
//                    Id = 338432,
//                    TagType = TagType.TAG_POWER_DEATH_STATUE_NUM_SHOTS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Death Statue Num Shots",
//                    InternalName = "TAG_POWER_DEATH_STATUE_NUM_SHOTS",
//                }
//            },
//            {
//                330464, new TagReference
//                {
//                    Id = 330464,
//                    TagType = TagType.TAG_POWER_SIEGE_DAMAGE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Siege Damage Percent",
//                    InternalName = "TAG_POWER_SIEGE_DAMAGE_PERCENT",
//                }
//            },
//            {
//                331248, new TagReference
//                {
//                    Id = 331248,
//                    TagType = TagType.TAG_POWER_FREEZE_DAMAGE_DONE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Freeze Damage Done Delta",
//                    InternalName = "TAG_POWER_FREEZE_DAMAGE_DONE_DELTA",
//                }
//            },
//            {
//                700435, new TagReference
//                {
//                    Id = 700435,
//                    TagType = TagType.TAG_POWER_POWER_GAINED_PER_SECOND,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Power Gained Per Second",
//                    InternalName = "TAG_POWER_POWER_GAINED_PER_SECOND",
//                }
//            },
//            {
//                331164, new TagReference
//                {
//                    Id = 331164,
//                    TagType = TagType.TAG_POWER_SALVAGE_FIND_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Salvage Find Bonus",
//                    InternalName = "TAG_POWER_SALVAGE_FIND_BONUS",
//                }
//            },
//            {
//                282624, new TagReference
//                {
//                    Id = 282624,
//                    TagType = TagType.TAG_POWER_UI_ANIMATION,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "UI Icon",
//                    InternalName = "TAG_POWER_UI_ANIMATION",
//                }
//            },
//            {
//                262407, new TagReference
//                {
//                    Id = 262407,
//                    TagType = TagType.TAG_POWER_TARGET_IMPACT_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Target Impact Effect Group",
//                    InternalName = "TAG_POWER_TARGET_IMPACT_EFFECT_GROUP",
//                }
//            },
//            {
//                329979, new TagReference
//                {
//                    Id = 329979,
//                    TagType = TagType.TAG_POWER_DOT_STACKING_METHOD,
//                    DataType = MapDataType.DotStackMethod,
//                    DisplayName = "Dot Stacking Method",
//                    InternalName = "TAG_POWER_DOT_STACKING_METHOD",
//                }
//            },
//            {
//                262432, new TagReference
//                {
//                    Id = 262432,
//                    TagType = TagType.TAG_POWER_ROPE_SNO,
//                    DataType = MapDataType.RopeChainSno,
//                    DisplayName = "Rope",
//                    InternalName = "TAG_POWER_ROPE_SNO",
//                }
//            },
//            {
//                692256, new TagReference
//                {
//                    Id = 692256,
//                    TagType = TagType.TAG_POWER_FREEZE_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Freeze Duration Delta",
//                    InternalName = "TAG_POWER_FREEZE_DURATION_DELTA",
//                }
//            },
//            {
//                684160, new TagReference
//                {
//                    Id = 684160,
//                    TagType = TagType.TAG_POWER_POOL_ACTOR,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Pool Actor",
//                    InternalName = "TAG_POWER_POOL_ACTOR",
//                }
//            },
//            {
//                370688, new TagReference
//                {
//                    Id = 370688,
//                    TagType = TagType.TAG_POWER_STONESPIKE_DELAY,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Stone Spike Attack Delay Time",
//                    InternalName = "TAG_POWER_STONESPIKE_DELAY",
//                }
//            },
//            {
//                332528, new TagReference
//                {
//                    Id = 332528,
//                    TagType = TagType.TAG_POWER_CONDUCTION_AURA_REFRESH_RATE_DECREASE_PER_TARGET,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Conduction Aura Refresh Rate Decrease Per Target",
//                    InternalName = "TAG_POWER_CONDUCTION_AURA_REFRESH_RATE_DECREASE_PER_TARGET",
//                }
//            },
//            {
//                361521, new TagReference
//                {
//                    Id = 361521,
//                    TagType = TagType.TAG_POWER_WHIRLWIND_MOVE_SPEED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Whirlwind Movement Speed",
//                    InternalName = "TAG_POWER_WHIRLWIND_MOVE_SPEED",
//                }
//            },
//            {
//                330832, new TagReference
//                {
//                    Id = 330832,
//                    TagType = TagType.TAG_POWER_WEB_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Web Duration Min",
//                    InternalName = "TAG_POWER_WEB_DURATION_MIN",
//                }
//            },
//            {
//                331616, new TagReference
//                {
//                    Id = 331616,
//                    TagType = TagType.TAG_POWER_FEAR_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fear Duration Min",
//                    InternalName = "TAG_POWER_FEAR_DURATION_MIN",
//                }
//            },
//            {
//                264276, new TagReference
//                {
//                    Id = 264276,
//                    TagType = TagType.TAG_POWER_CASTING_EFFECT_GROUP_FEMALE_2,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Casting Effect Group - Female 2",
//                    InternalName = "TAG_POWER_CASTING_EFFECT_GROUP_FEMALE_2",
//                }
//            },
//            {
//                330016, new TagReference
//                {
//                    Id = 330016,
//                    TagType = TagType.TAG_POWER_PHYSICAL_DAMAGE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Physical Damage Percent",
//                    InternalName = "TAG_POWER_PHYSICAL_DAMAGE_PERCENT",
//                }
//            },
//            {
//                331709, new TagReference
//                {
//                    Id = 331709,
//                    TagType = TagType.TAG_POWER_KNOCKBACK_GRAVITY_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Knockback Gravity Delta",
//                    InternalName = "TAG_POWER_KNOCKBACK_GRAVITY_DELTA",
//                }
//            },
//            {
//                329840, new TagReference
//                {
//                    Id = 329840,
//                    TagType = TagType.TAG_POWER_BASE_DAMAGE_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Base Damage Scalar",
//                    InternalName = "TAG_POWER_BASE_DAMAGE_SCALAR",
//                }
//            },
//            {
//                330496, new TagReference
//                {
//                    Id = 330496,
//                    TagType = TagType.TAG_POWER_HOLY_DAMAGE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Holy Damage Min",
//                    InternalName = "TAG_POWER_HOLY_DAMAGE_MIN",
//                }
//            },
//            {
//                332049, new TagReference
//                {
//                    Id = 332049,
//                    TagType = TagType.TAG_POWER_IMPROVED_BATTLE_RAGE_PROC_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Improved Battle Rage Proc Chance",
//                    InternalName = "TAG_POWER_IMPROVED_BATTLE_RAGE_PROC_CHANCE",
//                }
//            },
//            {
//                401408, new TagReference
//                {
//                    Id = 401408,
//                    TagType = TagType.TAG_POWER_WAVEOFFORCE_PROJECTILE_PERCENT_SPEED_INC,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Wave Of Force Projectile Percent Speed Inc",
//                    InternalName = "TAG_POWER_WAVEOFFORCE_PROJECTILE_PERCENT_SPEED_INC",
//                }
//            },
//            {
//                331156, new TagReference
//                {
//                    Id = 331156,
//                    TagType = TagType.TAG_POWER_GEM_FIND_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Gem Find Bonus",
//                    InternalName = "TAG_POWER_GEM_FIND_BONUS",
//                }
//            },
//            {
//                332185, new TagReference
//                {
//                    Id = 332185,
//                    TagType = TagType.TAG_POWER_FURY_MAX_DAMAGE_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fury Max Damage Bonus",
//                    InternalName = "TAG_POWER_FURY_MAX_DAMAGE_BONUS",
//                }
//            },
//            {
//                264275, new TagReference
//                {
//                    Id = 264275,
//                    TagType = TagType.TAG_POWER_CONTACT_FRAME_EFFECT_GROUP_FEMALE,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Contact Frame Effect Group - Female",
//                    InternalName = "TAG_POWER_CONTACT_FRAME_EFFECT_GROUP_FEMALE",
//                }
//            },
//            {
//                680768, new TagReference
//                {
//                    Id = 680768,
//                    TagType = TagType.TAG_POWER_PROC_COOLDOWN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Proc Cooldown Time",
//                    InternalName = "TAG_POWER_PROC_COOLDOWN",
//                }
//            },
//            {
//                655616, new TagReference
//                {
//                    Id = 655616,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_0,
//                    DataType = MapDataType.Attribute,
//                    DisplayName = "Generic Buff Attribute 0",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_0",
//                }
//            },
//            {
//                332097, new TagReference
//                {
//                    Id = 332097,
//                    TagType = TagType.TAG_POWER_FURY_GAINED_PER_SECOND_OF_ATTACK,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fury Gained Per Second Of Attack",
//                    InternalName = "TAG_POWER_FURY_GAINED_PER_SECOND_OF_ATTACK",
//                }
//            },
//            {
//                361488, new TagReference
//                {
//                    Id = 361488,
//                    TagType = TagType.TAG_POWER_WHIRLWIND_PULSE_RATE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Whirlwind Attack Pulse Rate",
//                    InternalName = "TAG_POWER_WHIRLWIND_PULSE_RATE",
//                }
//            },
//            {
//                332080, new TagReference
//                {
//                    Id = 332080,
//                    TagType = TagType.TAG_POWER_BLIZZARD_INITIAL_IMPACT_DELAY,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Blizzard Initial Impact Delay",
//                    InternalName = "TAG_POWER_BLIZZARD_INITIAL_IMPACT_DELAY",
//                }
//            },
//            {
//                332000, new TagReference
//                {
//                    Id = 332000,
//                    TagType = TagType.TAG_POWER_MOVEMENT_SPEED_PERCENT_INCREASE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Movement Speed Percent Increase Min",
//                    InternalName = "TAG_POWER_MOVEMENT_SPEED_PERCENT_INCREASE_MIN",
//                }
//            },
//            {
//                332560, new TagReference
//                {
//                    Id = 332560,
//                    TagType = TagType.TAG_POWER_MAGIC_MISSILE_JITTER_ANGLE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Magic Missile Jitter Angle",
//                    InternalName = "TAG_POWER_MAGIC_MISSILE_JITTER_ANGLE",
//                }
//            },
//            {
//                331184, new TagReference
//                {
//                    Id = 331184,
//                    TagType = TagType.TAG_POWER_PROJECTILE_SPEED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Projectile Speed",
//                    InternalName = "TAG_POWER_PROJECTILE_SPEED",
//                }
//            },
//            {
//                684929, new TagReference
//                {
//                    Id = 684929,
//                    TagType = TagType.TAG_POWER_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Duration Min",
//                    InternalName = "TAG_POWER_DURATION_MIN",
//                }
//            },
//            {
//                331708, new TagReference
//                {
//                    Id = 331708,
//                    TagType = TagType.TAG_POWER_KNOCKBACK_GRAVITY_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Knockback Gravity Min",
//                    InternalName = "TAG_POWER_KNOCKBACK_GRAVITY_MIN",
//                }
//            },
//            {
//                332288, new TagReference
//                {
//                    Id = 332288,
//                    TagType = TagType.TAG_POWER_ELECTROCUTE_PERCENT_DAMAGE_REDUCTION_PER_BOUNCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Electrocute Percent Damage Reduction Per Bounce",
//                    InternalName = "TAG_POWER_ELECTROCUTE_PERCENT_DAMAGE_REDUCTION_PER_BOUNCE",
//                }
//            },
//            {
//                425984, new TagReference
//                {
//                    Id = 425984,
//                    TagType = TagType.TAG_POWER_RAINOFGOLD_DAMAGE_TO_GOLD_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Rain Of Gold Damage To Gold Percent",
//                    InternalName = "TAG_POWER_RAINOFGOLD_DAMAGE_TO_GOLD_PERCENT",
//                }
//            },
//            {
//                262433, new TagReference
//                {
//                    Id = 262433,
//                    TagType = TagType.TAG_POWER_HITSOUND_OVERRIDE,
//                    DataType = MapDataType.SoundSetting,
//                    DisplayName = "Hitsound Override",
//                    InternalName = "TAG_POWER_HITSOUND_OVERRIDE",
//                }
//            },
//            {
//                524288, new TagReference
//                {
//                    Id = 524288,
//                    TagType = TagType.TAG_POWER_RESISTANCE_PERCENT_ALL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Resistance Percent All",
//                    InternalName = "TAG_POWER_RESISTANCE_PERCENT_ALL",
//                }
//            },
//            {
//                680192, new TagReference
//                {
//                    Id = 680192,
//                    TagType = TagType.TAG_POWER_SHIELD_ANGLE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Shield Angle",
//                    InternalName = "TAG_POWER_SHIELD_ANGLE",
//                }
//            },
//            {
//                332464, new TagReference
//                {
//                    Id = 332464,
//                    TagType = TagType.TAG_POWER_MIRROR_IMAGE_SUMMON_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mirror Image Summon Count",
//                    InternalName = "TAG_POWER_MIRROR_IMAGE_SUMMON_COUNT",
//                }
//            },
//            {
//                364544, new TagReference
//                {
//                    Id = 364544,
//                    TagType = TagType.TAG_POWER_GHOST_SOULSIPHON_SLOW_MULTIPLIER,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Ghost Soulsiphon Slow Multiplier",
//                    InternalName = "TAG_POWER_GHOST_SOULSIPHON_SLOW_MULTIPLIER",
//                }
//            },
//            {
//                458752, new TagReference
//                {
//                    Id = 458752,
//                    TagType = TagType.TAG_POWER_VANISH_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Vanish Duration",
//                    InternalName = "TAG_POWER_VANISH_DURATION",
//                }
//            },
//            {
//                330768, new TagReference
//                {
//                    Id = 330768,
//                    TagType = TagType.TAG_POWER_DEFENSE_PERCENT_ALL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Defense Percent All",
//                    InternalName = "TAG_POWER_DEFENSE_PERCENT_ALL",
//                }
//            },
//            {
//                331552, new TagReference
//                {
//                    Id = 331552,
//                    TagType = TagType.TAG_POWER_RING_OF_FROST_SLOW_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Ring of Frost Slow Chance",
//                    InternalName = "TAG_POWER_RING_OF_FROST_SLOW_CHANCE",
//                }
//            },
//            {
//                692240, new TagReference
//                {
//                    Id = 692240,
//                    TagType = TagType.TAG_POWER_FREEZE_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Freeze Duration Min",
//                    InternalName = "TAG_POWER_FREEZE_DURATION_MIN",
//                }
//            },
//            {
//                329776, new TagReference
//                {
//                    Id = 329776,
//                    TagType = TagType.TAG_POWER_PAYLOAD_PARAM_0,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Payload Param 0",
//                    InternalName = "TAG_POWER_PAYLOAD_PARAM_0",
//                }
//            },
//            {
//                397313, new TagReference
//                {
//                    Id = 397313,
//                    TagType = TagType.TAG_POWER_SPECTRALBLADE_NUMBER_OF_HITS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Spectral Blade Number of Hits",
//                    InternalName = "TAG_POWER_SPECTRALBLADE_NUMBER_OF_HITS",
//                }
//            },
//            {
//                655649, new TagReference
//                {
//                    Id = 655649,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_2,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Generic Buff Attribute 2 Formula",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_2",
//                }
//            },
//            {
//                684168, new TagReference
//                {
//                    Id = 684168,
//                    TagType = TagType.TAG_POWER_POOL_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Pool Duration",
//                    InternalName = "TAG_POWER_POOL_DURATION",
//                }
//            },
//            {
//                361544, new TagReference
//                {
//                    Id = 361544,
//                    TagType = TagType.TAG_POWER_REFLECT_DAMAGE_HEALTH_PERCENT_CAP,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Reflect Damage Health Percent Cap",
//                    InternalName = "TAG_POWER_REFLECT_DAMAGE_HEALTH_PERCENT_CAP",
//                }
//            },
//            {
//                548864, new TagReference
//                {
//                    Id = 548864,
//                    TagType = TagType.TAG_POWER_DOT_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "DOT Duration",
//                    InternalName = "TAG_POWER_DOT_DURATION",
//                }
//            },
//            {
//                337408, new TagReference
//                {
//                    Id = 337408,
//                    TagType = TagType.TAG_POWER_SUMMONED_GHOUL_DECAY_TIME,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Summoned Ghoul Decay Time",
//                    InternalName = "TAG_POWER_SUMMONED_GHOUL_DECAY_TIME",
//                }
//            },
//            {
//                659456, new TagReference
//                {
//                    Id = 659456,
//                    TagType = TagType.TAG_POWER_LIGHTNING_SPEED_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Lightning Speed Duration",
//                    InternalName = "TAG_POWER_LIGHTNING_SPEED_DURATION",
//                }
//            },
//            {
//                330224, new TagReference
//                {
//                    Id = 330224,
//                    TagType = TagType.TAG_POWER_COLD_RESISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Cold Damage Resistance",
//                    InternalName = "TAG_POWER_COLD_RESISTANCE",
//                }
//            },
//            {
//                329920, new TagReference
//                {
//                    Id = 329920,
//                    TagType = TagType.TAG_POWER_BLEED_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Bleed Duration",
//                    InternalName = "TAG_POWER_BLEED_DURATION",
//                }
//            },
//            {
//                332928, new TagReference
//                {
//                    Id = 332928,
//                    TagType = TagType.TAG_POWER_SLOW_TIME_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Slow Time Duration",
//                    InternalName = "TAG_POWER_SLOW_TIME_DURATION",
//                }
//            },
//            {
//                684808, new TagReference
//                {
//                    Id = 684808,
//                    TagType = TagType.TAG_POWER_TORNADO_MOVE_SPEED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Tornado Move Speed",
//                    InternalName = "TAG_POWER_TORNADO_MOVE_SPEED",
//                }
//            },
//            {
//                331936, new TagReference
//                {
//                    Id = 331936,
//                    TagType = TagType.TAG_POWER_DODGE_TRAVEL_SPEED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Dodge Travel Speed",
//                    InternalName = "TAG_POWER_DODGE_TRAVEL_SPEED",
//                }
//            },
//            {
//                330336, new TagReference
//                {
//                    Id = 330336,
//                    TagType = TagType.TAG_POWER_ARCANE_DAMAGE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Arcane Damage Percent",
//                    InternalName = "TAG_POWER_ARCANE_DAMAGE_PERCENT",
//                }
//            },
//            {
//                331120, new TagReference
//                {
//                    Id = 331120,
//                    TagType = TagType.TAG_POWER_SUMMONED_ACTOR_LIFE_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Summoned Actor Life Duration",
//                    InternalName = "TAG_POWER_SUMMONED_ACTOR_LIFE_DURATION",
//                }
//            },
//            {
//                708720, new TagReference
//                {
//                    Id = 708720,
//                    TagType = TagType.TAG_POWER_MASTABLASTA_DRAIN_DISCIPLINE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "MastaBlasta Drain Discipline Percent",
//                    InternalName = "TAG_POWER_MASTABLASTA_DRAIN_DISCIPLINE_PERCENT",
//                }
//            },
//            {
//                369664, new TagReference
//                {
//                    Id = 369664,
//                    TagType = TagType.TAG_POWER_FEASTOFSOULS_INITIAL_TARGET_SEARCH_RADIUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Feast of Souls Initial Target Search Radius",
//                    InternalName = "TAG_POWER_FEASTOFSOULS_INITIAL_TARGET_SEARCH_RADIUS",
//                }
//            },
//            {
//                331984, new TagReference
//                {
//                    Id = 331984,
//                    TagType = TagType.TAG_POWER_ATTACK_SPEED_PERCENT_INCREASE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Attack Speed Percent Increase Delta",
//                    InternalName = "TAG_POWER_ATTACK_SPEED_PERCENT_INCREASE_DELTA",
//                }
//            },
//            {
//                264193, new TagReference
//                {
//                    Id = 264193,
//                    TagType = TagType.TAG_POWER_CASTING_EFFECT_GROUP_2,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Casting Effect Group - Male 2",
//                    InternalName = "TAG_POWER_CASTING_EFFECT_GROUP_2",
//                }
//            },
//            {
//                332360, new TagReference
//                {
//                    Id = 332360,
//                    TagType = TagType.TAG_POWER_SPECIAL_WALK_PERTURB_DESTINATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Special Walk Perturb Destination",
//                    InternalName = "TAG_POWER_SPECIAL_WALK_PERTURB_DESTINATION",
//                }
//            },
//            {
//                700449, new TagReference
//                {
//                    Id = 700449,
//                    TagType = TagType.TAG_POWER_REDUCED_HEALING_RECEIVED_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Reduced Healing Received Percent",
//                    InternalName = "TAG_POWER_REDUCED_HEALING_RECEIVED_PERCENT",
//                }
//            },
//            {
//                684032, new TagReference
//                {
//                    Id = 684032,
//                    TagType = TagType.TAG_POWER_OVERRIDE_HIT_EFFECTS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Override Hit Effects",
//                    InternalName = "TAG_POWER_OVERRIDE_HIT_EFFECTS",
//                }
//            },
//            {
//                262415, new TagReference
//                {
//                    Id = 262415,
//                    TagType = TagType.TAG_POWER_SPAWNED_ACTOR,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Spawned Actor",
//                    InternalName = "TAG_POWER_SPAWNED_ACTOR",
//                }
//            },
//            {
//                332400, new TagReference
//                {
//                    Id = 332400,
//                    TagType = TagType.TAG_POWER_DODGE_TRAVEL_DISTANCE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Dodge Travel Distance Delta",
//                    InternalName = "TAG_POWER_DODGE_TRAVEL_DISTANCE_DELTA",
//                }
//            },
//            {
//                333280, new TagReference
//                {
//                    Id = 333280,
//                    TagType = TagType.TAG_POWER_MIRROR_IMAGE_BONUS_LIFE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mirror Image Bonus Life Percent",
//                    InternalName = "TAG_POWER_MIRROR_IMAGE_BONUS_LIFE_PERCENT",
//                }
//            },
//            {
//                720992, new TagReference
//                {
//                    Id = 720992,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_LIMIT_TO_PLAYERS_IN_GAME,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Death Portal Limit To Players In Game",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_LIMIT_TO_PLAYERS_IN_GAME",
//                }
//            },
//            {
//                262440, new TagReference
//                {
//                    Id = 262440,
//                    TagType = TagType.TAG_POWER_EFFECT_ACTOR_SNO,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Effect Actor",
//                    InternalName = "TAG_POWER_EFFECT_ACTOR_SNO",
//                }
//            },
//            {
//                684578, new TagReference
//                {
//                    Id = 684578,
//                    TagType = TagType.TAG_BLIND_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Blind Duration Delta",
//                    InternalName = "TAG_BLIND_DURATION_DELTA",
//                }
//            },
//            {
//                606720, new TagReference
//                {
//                    Id = 606720,
//                    TagType = TagType.TAG_POWER_ROOT_IS_UNTARGETABLE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Root Is Untargetable",
//                    InternalName = "TAG_POWER_ROOT_IS_UNTARGETABLE",
//                }
//            },
//            {
//                606208, new TagReference
//                {
//                    Id = 606208,
//                    TagType = TagType.TAG_POWER_ROOT_GRAB_POWER,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Root Grab Power",
//                    InternalName = "TAG_POWER_ROOT_GRAB_POWER",
//                }
//            },
//            {
//                661504, new TagReference
//                {
//                    Id = 661504,
//                    TagType = TagType.TAG_POWER_LIGHTNING_SPEED_MOVEMENT_SCALAR_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Lightning Speed Movement Scalar Bonus",
//                    InternalName = "TAG_POWER_LIGHTNING_SPEED_MOVEMENT_SCALAR_BONUS",
//                }
//            },
//            {
//                328144, new TagReference
//                {
//                    Id = 328144,
//                    TagType = TagType.TAG_POWER_TARGET_NEUTRAL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetNeutral",
//                    InternalName = "TAG_POWER_TARGET_NEUTRAL",
//                }
//            },
//            {
//                338944, new TagReference
//                {
//                    Id = 338944,
//                    TagType = TagType.TAG_POWER_DEATH_STATUE_GHOUL_DOT_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Death Statue Ghoul Dot Chance",
//                    InternalName = "TAG_POWER_DEATH_STATUE_GHOUL_DOT_CHANCE",
//                }
//            },
//            {
//                330976, new TagReference
//                {
//                    Id = 330976,
//                    TagType = TagType.TAG_POWER_POISON_CLOUD_INTERVAL_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Poison Cloud Interval Duration",
//                    InternalName = "TAG_POWER_POISON_CLOUD_INTERVAL_DURATION",
//                }
//            },
//            {
//                708608, new TagReference
//                {
//                    Id = 708608,
//                    TagType = TagType.TAG_POWER_SUFFOCATION_PER_SECOND,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Suffocation Per Second",
//                    InternalName = "TAG_POWER_SUFFOCATION_PER_SECOND",
//                }
//            },
//            {
//                330160, new TagReference
//                {
//                    Id = 330160,
//                    TagType = TagType.TAG_POWER_LIGHTNING_RESISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Lightning Damage Resistance",
//                    InternalName = "TAG_POWER_LIGHTNING_RESISTANCE",
//                }
//            },
//            {
//                329856, new TagReference
//                {
//                    Id = 329856,
//                    TagType = TagType.TAG_POWER_HIT_CHANCE,
//                    DataType = MapDataType.HighPrecision,
//                    DisplayName = "Hit Chance",
//                    InternalName = "TAG_POWER_HIT_CHANCE",
//                }
//            },
//            {
//                330640, new TagReference
//                {
//                    Id = 330640,
//                    TagType = TagType.TAG_POWER_TRIGGERS_HIT_PROCS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Triggers Hit Procs and Thorns",
//                    InternalName = "TAG_POWER_TRIGGERS_HIT_PROCS",
//                }
//            },
//            {
//                331264, new TagReference
//                {
//                    Id = 331264,
//                    TagType = TagType.TAG_POWER_HITPOINTS_TO_HEAL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Hitpoints Granted By Heal",
//                    InternalName = "TAG_POWER_HITPOINTS_TO_HEAL",
//                }
//            },
//            {
//                330272, new TagReference
//                {
//                    Id = 330272,
//                    TagType = TagType.TAG_POWER_POISON_DAMAGE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Poison Damage Percent",
//                    InternalName = "TAG_POWER_POISON_DAMAGE_PERCENT",
//                }
//            },
//            {
//                331056, new TagReference
//                {
//                    Id = 331056,
//                    TagType = TagType.TAG_POWER_CHARGED_BOLT_NUM_BOLTS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Charged Bolt Num Bolts",
//                    InternalName = "TAG_POWER_CHARGED_BOLT_NUM_BOLTS",
//                }
//            },
//            {
//                684928, new TagReference
//                {
//                    Id = 684928,
//                    TagType = TagType.TAG_POWER_SPIRIT_GAIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Spirit Gained",
//                    InternalName = "TAG_POWER_SPIRIT_GAIN",
//                }
//            },
//            {
//                631296, new TagReference
//                {
//                    Id = 631296,
//                    TagType = TagType.TAG_POWER_QUICKSAND_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Quicksand Duration",
//                    InternalName = "TAG_POWER_QUICKSAND_DURATION",
//                }
//            },
//            {
//                262657, new TagReference
//                {
//                    Id = 262657,
//                    TagType = TagType.TAG_POWER_CHARGEUP_ANIMATION,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Animation Charge Up",
//                    InternalName = "TAG_POWER_CHARGEUP_ANIMATION",
//                }
//            },
//            {
//                684801, new TagReference
//                {
//                    Id = 684801,
//                    TagType = TagType.TAG_POWER_TORNADO_DAMAGE_PULSE_RATE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Tornado Damage Pulse Rate",
//                    InternalName = "TAG_POWER_TORNADO_DAMAGE_PULSE_RATE",
//                }
//            },
//            {
//                630784, new TagReference
//                {
//                    Id = 630784,
//                    TagType = TagType.TAG_POWER_SAND_SHIELD_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Sand Shield Duration",
//                    InternalName = "TAG_POWER_SAND_SHIELD_DURATION",
//                }
//            },
//            {
//                708616, new TagReference
//                {
//                    Id = 708616,
//                    TagType = TagType.TAG_POWER_LEAP_DIST_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Leap Distance Min",
//                    InternalName = "TAG_POWER_LEAP_DIST_MIN",
//                }
//            },
//            {
//                262402, new TagReference
//                {
//                    Id = 262402,
//                    TagType = TagType.TAG_POWER_GROUND_IMPACT_SNO,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Ground Impact Actor",
//                    InternalName = "TAG_POWER_GROUND_IMPACT_SNO",
//                }
//            },
//            {
//                331200, new TagReference
//                {
//                    Id = 331200,
//                    TagType = TagType.TAG_POWER_MANA_DRAIN_AMOUNT_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mana Drain Amount Min",
//                    InternalName = "TAG_POWER_MANA_DRAIN_AMOUNT_MIN",
//                }
//            },
//            {
//                633601, new TagReference
//                {
//                    Id = 633601,
//                    TagType = TagType.TAG_POWER_SHRINE_BUFF_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Shrine Buff Bonus",
//                    InternalName = "TAG_POWER_SHRINE_BUFF_BONUS",
//                }
//            },
//            {
//                331697, new TagReference
//                {
//                    Id = 331697,
//                    TagType = TagType.TAG_POWER_CAUSES_KNOCKBACK,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Causes Knockback",
//                    InternalName = "TAG_POWER_CAUSES_KNOCKBACK",
//                }
//            },
//            {
//                684913, new TagReference
//                {
//                    Id = 684913,
//                    TagType = TagType.TAG_POWER_SAND_WALL_DEFLECTION_JITTER,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Sand Wall Deflection Jitter",
//                    InternalName = "TAG_POWER_SAND_WALL_DEFLECTION_JITTER",
//                }
//            },
//            {
//                332336, new TagReference
//                {
//                    Id = 332336,
//                    TagType = TagType.TAG_POWER_SPECIAL_WALK_TRAJECTORY_GRAVITY,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Special Walk Trajectory Gravity",
//                    InternalName = "TAG_POWER_SPECIAL_WALK_TRAJECTORY_GRAVITY",
//                }
//            },
//            {
//                413696, new TagReference
//                {
//                    Id = 413696,
//                    TagType = TagType.TAG_POWER_ANATOMY_CRIT_BONUS_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Anatomy Crit Bonus Percent",
//                    InternalName = "TAG_POWER_ANATOMY_CRIT_BONUS_PERCENT",
//                }
//            },
//            {
//                664064, new TagReference
//                {
//                    Id = 664064,
//                    TagType = TagType.TAG_POWER_STATIC_CHARGE_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Static Charge Duration",
//                    InternalName = "TAG_POWER_STATIC_CHARGE_DURATION",
//                }
//            },
//            {
//                720928, new TagReference
//                {
//                    Id = 720928,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_ACTOR_2,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Death Portal Actor 2",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_ACTOR_2",
//                }
//            },
//            {
//                329976, new TagReference
//                {
//                    Id = 329976,
//                    TagType = TagType.TAG_POWER_FIRE_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fire Duration",
//                    InternalName = "TAG_POWER_FIRE_DURATION",
//                }
//            },
//            {
//                667904, new TagReference
//                {
//                    Id = 667904,
//                    TagType = TagType.TAG_POWER_INTENSIFY_CRIT_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Intensify Crit Duration",
//                    InternalName = "TAG_POWER_INTENSIFY_CRIT_DURATION",
//                }
//            },
//            {
//                633360, new TagReference
//                {
//                    Id = 633360,
//                    TagType = TagType.TAG_POWER_FALLEN_MAX_SUMMON_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fallen Max Count",
//                    InternalName = "TAG_POWER_FALLEN_MAX_SUMMON_COUNT",
//                }
//            },
//            {
//                332641, new TagReference
//                {
//                    Id = 332641,
//                    TagType = TagType.TAG_POWER_SHIELD_DAMAGE_ABSORB_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Percent of Damage That Shield Can Absorb",
//                    InternalName = "TAG_POWER_SHIELD_DAMAGE_ABSORB_PERCENT",
//                }
//            },
//            {
//                327904, new TagReference
//                {
//                    Id = 327904,
//                    TagType = TagType.TAG_POWER_ALWAYS_HITS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "AlwaysHits",
//                    InternalName = "TAG_POWER_ALWAYS_HITS",
//                }
//            },
//            {
//                330912, new TagReference
//                {
//                    Id = 330912,
//                    TagType = TagType.TAG_POWER_SLOW_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Slow Duration Delta",
//                    InternalName = "TAG_POWER_SLOW_DURATION_DELTA",
//                }
//            },
//            {
//                684034, new TagReference
//                {
//                    Id = 684034,
//                    TagType = TagType.TAG_POWER_HIT_EFFECT,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Hit Effect",
//                    InternalName = "TAG_POWER_HIT_EFFECT",
//                }
//            },
//            {
//                331696, new TagReference
//                {
//                    Id = 331696,
//                    TagType = TagType.TAG_POWER_KNOCKBACK_MAGNITUDE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Knockback Magnitude",
//                    InternalName = "TAG_POWER_KNOCKBACK_MAGNITUDE",
//                }
//            },
//            {
//                329981, new TagReference
//                {
//                    Id = 329981,
//                    TagType = TagType.TAG_POWER_DOT_MAX_STACK_COUNT,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Dot Max Stack Count",
//                    InternalName = "TAG_POWER_DOT_MAX_STACK_COUNT",
//                }
//            },
//            {
//                330096, new TagReference
//                {
//                    Id = 330096,
//                    TagType = TagType.TAG_POWER_FIRE_RESISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fire Damage Resistance",
//                    InternalName = "TAG_POWER_FIRE_RESISTANCE",
//                }
//            },
//            {
//                329792, new TagReference
//                {
//                    Id = 329792,
//                    TagType = TagType.TAG_POWER_PAYLOAD_PARAM_1,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Payload Param 1",
//                    InternalName = "TAG_POWER_PAYLOAD_PARAM_1",
//                }
//            },
//            {
//                330576, new TagReference
//                {
//                    Id = 330576,
//                    TagType = TagType.TAG_POWER_DISEASE_DAMAGE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Disease Damage Delta",
//                    InternalName = "TAG_POWER_DISEASE_DAMAGE_DELTA",
//                }
//            },
//            {
//                332800, new TagReference
//                {
//                    Id = 332800,
//                    TagType = TagType.TAG_POWER_CURSE_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Curse Duration",
//                    InternalName = "TAG_POWER_CURSE_DURATION",
//                }
//            },
//            {
//                688640, new TagReference
//                {
//                    Id = 688640,
//                    TagType = TagType.TAG_POWER_EXPEL_MONSTER_SPAWN_INTERVAL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Expel Monster Spawn Interval",
//                    InternalName = "TAG_POWER_EXPEL_MONSTER_SPAWN_INTERVAL",
//                }
//            },
//            {
//                332186, new TagReference
//                {
//                    Id = 332186,
//                    TagType = TagType.TAG_POWER_FURY_DEGENERATION_REDUCED_DECAY_THRESHOLD,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fury Reduced Degen. threshold (fraction)",
//                    InternalName = "TAG_POWER_FURY_DEGENERATION_REDUCED_DECAY_THRESHOLD",
//                }
//            },
//            {
//                632832, new TagReference
//                {
//                    Id = 632832,
//                    TagType = TagType.TAG_POWER_FIREGRENADE_COUNTDOWN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fire Grenade Countdown",
//                    InternalName = "TAG_POWER_FIREGRENADE_COUNTDOWN",
//                }
//            },
//            {
//                684176, new TagReference
//                {
//                    Id = 684176,
//                    TagType = TagType.TAG_POWER_POOL_PULSE_INTERVAL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Pool Pulse Interval",
//                    InternalName = "TAG_POWER_POOL_PULSE_INTERVAL",
//                }
//            },
//            {
//                600320, new TagReference
//                {
//                    Id = 600320,
//                    TagType = TagType.TAG_POWER_FIREBATS_NUM_BATS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fire Bats Num Bats",
//                    InternalName = "TAG_POWER_FIREBATS_NUM_BATS",
//                }
//            },
//            {
//                682752, new TagReference
//                {
//                    Id = 682752,
//                    TagType = TagType.TAG_POWER_BURN_HOUSE_WATER_HEAL_AMOUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Burn House Water Heal Amount",
//                    InternalName = "TAG_POWER_BURN_HOUSE_WATER_HEAL_AMOUNT",
//                }
//            },
//            {
//                655634, new TagReference
//                {
//                    Id = 655634,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_1_WITH_PARAM,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Generic Buff Attribute 1 And Parameter",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_1_WITH_PARAM",
//                }
//            },
//            {
//                332256, new TagReference
//                {
//                    Id = 332256,
//                    TagType = TagType.TAG_POWER_ELECTROCUTE_AOE_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Electrocute Chance of AOE Attack on Target Death",
//                    InternalName = "TAG_POWER_ELECTROCUTE_AOE_CHANCE",
//                }
//            },
//            {
//                360448, new TagReference
//                {
//                    Id = 360448,
//                    TagType = TagType.TAG_POWER_DESTINATION_JITTER_RADIUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Destination Jitter Radius",
//                    InternalName = "TAG_POWER_DESTINATION_JITTER_RADIUS",
//                }
//            },
//            {
//                331856, new TagReference
//                {
//                    Id = 331856,
//                    TagType = TagType.TAG_POWER_NUMBER_OF_MISSILES,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Number of Missiles",
//                    InternalName = "TAG_POWER_NUMBER_OF_MISSILES",
//                }
//            },
//            {
//                332736, new TagReference
//                {
//                    Id = 332736,
//                    TagType = TagType.TAG_POWER_BUFF_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Buff Duration Min",
//                    InternalName = "TAG_POWER_BUFF_DURATION_MIN",
//                }
//            },
//            {
//                330480, new TagReference
//                {
//                    Id = 330480,
//                    TagType = TagType.TAG_POWER_SIEGE_RESISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Siege Damage Resistance",
//                    InternalName = "TAG_POWER_SIEGE_RESISTANCE",
//                }
//            },
//            {
//                331136, new TagReference
//                {
//                    Id = 331136,
//                    TagType = TagType.TAG_POWER_SUMMONED_ACTOR_LEVEL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Summoned Actor Level",
//                    InternalName = "TAG_POWER_SUMMONED_ACTOR_LEVEL",
//                }
//            },
//            {
//                264277, new TagReference
//                {
//                    Id = 264277,
//                    TagType = TagType.TAG_POWER_CONTACT_FRAME_EFFECT_GROUP_FEMALE_2,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Contact Frame Effect Group - Female 2",
//                    InternalName = "TAG_POWER_CONTACT_FRAME_EFFECT_GROUP_FEMALE_2",
//                }
//            },
//            {
//                332451, new TagReference
//                {
//                    Id = 332451,
//                    TagType = TagType.TAG_POWER_SUMMONED_ANIMATION,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Summoned Animation Tag",
//                    InternalName = "TAG_POWER_SUMMONED_ANIMATION",
//                }
//            },
//            {
//                329912, new TagReference
//                {
//                    Id = 329912,
//                    TagType = TagType.TAG_POWER_DISEASE_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Disease Duration",
//                    InternalName = "TAG_POWER_DISEASE_DURATION",
//                }
//            },
//            {
//                333288, new TagReference
//                {
//                    Id = 333288,
//                    TagType = TagType.TAG_POWER_MIRROR_IMAGE_DOUBLE_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mirror Image Double Chance",
//                    InternalName = "TAG_POWER_MIRROR_IMAGE_DOUBLE_CHANCE",
//                }
//            },
//            {
//                262448, new TagReference
//                {
//                    Id = 262448,
//                    TagType = TagType.TAG_POWER_ALT_EXPLOSION_SNO,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Alternate Explosion Actor",
//                    InternalName = "TAG_POWER_ALT_EXPLOSION_SNO",
//                }
//            },
//            {
//                632064, new TagReference
//                {
//                    Id = 632064,
//                    TagType = TagType.TAG_POWER_PIERCING_SHOT_PIERCE_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Piercing Shot Pierce Count",
//                    InternalName = "TAG_POWER_PIERCING_SHOT_PIERCE_COUNT",
//                }
//            },
//            {
//                633603, new TagReference
//                {
//                    Id = 633603,
//                    TagType = TagType.TAG_POWER_SHRINE_BUFF_RADIUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Shrine Buff Radius",
//                    InternalName = "TAG_POWER_SHRINE_BUFF_RADIUS",
//                }
//            },
//            {
//                600321, new TagReference
//                {
//                    Id = 600321,
//                    TagType = TagType.TAG_POWER_FIREBATS_REFRESH_RATE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fire Bats Refresh Rate",
//                    InternalName = "TAG_POWER_FIREBATS_REFRESH_RATE",
//                }
//            },
//            {
//                364800, new TagReference
//                {
//                    Id = 364800,
//                    TagType = TagType.TAG_POWER_GHOST_SOULSIPHON_MAX_CHANNELLING_TIME,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Ghost Soulsiphon Max Channelling Time",
//                    InternalName = "TAG_POWER_GHOST_SOULSIPHON_MAX_CHANNELLING_TIME",
//                }
//            },
//            {
//                328624, new TagReference
//                {
//                    Id = 328624,
//                    TagType = TagType.TAG_POWER_IS_PHYSICAL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Is Physical",
//                    InternalName = "TAG_POWER_IS_PHYSICAL",
//                }
//            },
//            {
//                330848, new TagReference
//                {
//                    Id = 330848,
//                    TagType = TagType.TAG_POWER_WEB_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Web Duration Delta",
//                    InternalName = "TAG_POWER_WEB_DURATION_DELTA",
//                }
//            },
//            {
//                331632, new TagReference
//                {
//                    Id = 331632,
//                    TagType = TagType.TAG_POWER_FEAR_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fear Duration Delta",
//                    InternalName = "TAG_POWER_FEAR_DURATION_DELTA",
//                }
//            },
//            {
//                655505, new TagReference
//                {
//                    Id = 655505,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_WEAPON_CLASS_REQUIREMENT_OFFHAND,
//                    DataType = MapDataType.WeaponClassRequirement,
//                    DisplayName = "Generic Buff Weapon Class Requirement Offhand",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_WEAPON_CLASS_REQUIREMENT_OFFHAND",
//                }
//            },
//            {
//                594432, new TagReference
//                {
//                    Id = 594432,
//                    TagType = TagType.TAG_POWER_ZOMBIFY_DEBUFF_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Zombify Debuff Duration",
//                    InternalName = "TAG_POWER_ZOMBIFY_DEBUFF_DURATION",
//                }
//            },
//            {
//                615937, new TagReference
//                {
//                    Id = 615937,
//                    TagType = TagType.TAG_POWER_BONEREAVER_HEAL_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Bonereaver Consume Heal Buff Duration",
//                    InternalName = "TAG_POWER_BONEREAVER_HEAL_DURATION",
//                }
//            },
//            {
//                330032, new TagReference
//                {
//                    Id = 330032,
//                    TagType = TagType.TAG_POWER_PHYSICAL_RESISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Physical Damage Resistance",
//                    InternalName = "TAG_POWER_PHYSICAL_RESISTANCE",
//                }
//            },
//            {
//                262405, new TagReference
//                {
//                    Id = 262405,
//                    TagType = TagType.TAG_POWER_TARGET_IMPACT_SNO,
//                    DataType = MapDataType.TargetSno,
//                    DisplayName = "Target Impact Particle",
//                    InternalName = "TAG_POWER_TARGET_IMPACT_SNO",
//                }
//            },
//            {
//                330512, new TagReference
//                {
//                    Id = 330512,
//                    TagType = TagType.TAG_POWER_HOLY_DAMAGE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Holy Damage Delta",
//                    InternalName = "TAG_POWER_HOLY_DAMAGE_DELTA",
//                }
//            },
//            {
//                262661, new TagReference
//                {
//                    Id = 262661,
//                    TagType = TagType.TAG_POWER_GENERIC_EFFECT_GROUP_0,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Generic Effect Group 0",
//                    InternalName = "TAG_POWER_GENERIC_EFFECT_GROUP_0",
//                }
//            },
//            {
//                593920, new TagReference
//                {
//                    Id = 593920,
//                    TagType = TagType.TAG_POWER_ZOMBIFY_MAX_POSSESSED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Zombify Max Possessed",
//                    InternalName = "TAG_POWER_ZOMBIFY_MAX_POSSESSED",
//                }
//            },
//            {
//                655729, new TagReference
//                {
//                    Id = 655729,
//                    TagType = TagType.TAG_POWER_ICE_ARMOR_DEFENSE_BONUS_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Ice Armor Defense Bonus Percent",
//                    InternalName = "TAG_POWER_ICE_ARMOR_DEFENSE_BONUS_PERCENT",
//                }
//            },
//            {
//                684800, new TagReference
//                {
//                    Id = 684800,
//                    TagType = TagType.TAG_POWER_TORNADO_RADIUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Tornado Damage Radius",
//                    InternalName = "TAG_POWER_TORNADO_RADIUS",
//                }
//            },
//            {
//                679936, new TagReference
//                {
//                    Id = 679936,
//                    TagType = TagType.TAG_POWER_PROJECTILE_REFLECT_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Projectile Reflect Chance",
//                    InternalName = "TAG_POWER_PROJECTILE_REFLECT_CHANCE",
//                }
//            },
//            {
//                330176, new TagReference
//                {
//                    Id = 330176,
//                    TagType = TagType.TAG_POWER_COLD_DAMAGE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Cold Damage Min",
//                    InternalName = "TAG_POWER_COLD_DAMAGE_MIN",
//                }
//            },
//            {
//                655632, new TagReference
//                {
//                    Id = 655632,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_1,
//                    DataType = MapDataType.Attribute,
//                    DisplayName = "Generic Buff Attribute 1",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_1",
//                }
//            },
//            {
//                337152, new TagReference
//                {
//                    Id = 337152,
//                    TagType = TagType.TAG_POWER_SUMMONED_GHOUL_MAX_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Summoned Ghoul Max Count",
//                    InternalName = "TAG_POWER_SUMMONED_GHOUL_MAX_COUNT",
//                }
//            },
//            {
//                635649, new TagReference
//                {
//                    Id = 635649,
//                    TagType = TagType.TAG_POWER_FROZEN_STACKED_DURATION_MAX,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Frozen Debuff Stacked Duration Max",
//                    InternalName = "TAG_POWER_FROZEN_STACKED_DURATION_MAX",
//                }
//            },
//            {
//                361504, new TagReference
//                {
//                    Id = 361504,
//                    TagType = TagType.TAG_POWER_WHIRLWIND_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Whirlwind Duration Min",
//                    InternalName = "TAG_POWER_WHIRLWIND_DURATION_MIN",
//                }
//            },
//            {
//                332912, new TagReference
//                {
//                    Id = 332912,
//                    TagType = TagType.TAG_POWER_CONDUCTION_AURA_BASE_REFRESH_RATE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Conduction Aura Base Refresh Rate",
//                    InternalName = "TAG_POWER_CONDUCTION_AURA_BASE_REFRESH_RATE",
//                }
//            },
//            {
//                332672, new TagReference
//                {
//                    Id = 332672,
//                    TagType = TagType.TAG_POWER_TORNADO_LIFE_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Tornado Life Duration Delta",
//                    InternalName = "TAG_POWER_TORNADO_LIFE_DURATION_DELTA",
//                }
//            },
//            {
//                721328, new TagReference
//                {
//                    Id = 721328,
//                    TagType = TagType.TAG_POWER_PAYLOAD_PIE_USE_ACTOR_FACING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Payload Pie Use Actor Facing",
//                    InternalName = "TAG_POWER_PAYLOAD_PIE_USE_ACTOR_FACING",
//                }
//            },
//            {
//                331072, new TagReference
//                {
//                    Id = 331072,
//                    TagType = TagType.TAG_POWER_ARCANE_ORB_NUM_GRENADES,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Arcane Orb Num Grenades",
//                    InternalName = "TAG_POWER_ARCANE_ORB_NUM_GRENADES",
//                }
//            },
//            {
//                684912, new TagReference
//                {
//                    Id = 684912,
//                    TagType = TagType.TAG_POWER_SAND_WALL_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Sand Wall Duration",
//                    InternalName = "TAG_POWER_SAND_WALL_DURATION",
//                }
//            },
//            {
//                708640, new TagReference
//                {
//                    Id = 708640,
//                    TagType = TagType.TAG_POWER_MASTABLASTA_DRAIN_ARCANUM_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "MastaBlasta Drain Arcanum Percent",
//                    InternalName = "TAG_POWER_MASTABLASTA_DRAIN_ARCANUM_PERCENT",
//                }
//            },
//            {
//                262410, new TagReference
//                {
//                    Id = 262410,
//                    TagType = TagType.TAG_POWER_WALL_FLOOR_EXPLOSION_SNO,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Projectile Wall/Floor Explosion",
//                    InternalName = "TAG_POWER_WALL_FLOOR_EXPLOSION_SNO",
//                }
//            },
//            {
//                333088, new TagReference
//                {
//                    Id = 333088,
//                    TagType = TagType.TAG_POWER_CONCENTRATION_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Concentration Duration",
//                    InternalName = "TAG_POWER_CONCENTRATION_DURATION",
//                }
//            },
//            {
//                618496, new TagReference
//                {
//                    Id = 618496,
//                    TagType = TagType.TAG_POWER_DESTRUCTABLE_OBJECT_DAMAGE_DELAY,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Destructable Object Damage Delay",
//                    InternalName = "TAG_POWER_DESTRUCTABLE_OBJECT_DAMAGE_DELAY",
//                }
//            },
//            {
//                264273, new TagReference
//                {
//                    Id = 264273,
//                    TagType = TagType.TAG_POWER_CASTING_EFFECT_GROUP_FEMALE,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Casting Effect Group - Female",
//                    InternalName = "TAG_POWER_CASTING_EFFECT_GROUP_FEMALE",
//                }
//            },
//            {
//                369408, new TagReference
//                {
//                    Id = 369408,
//                    TagType = TagType.TAG_POWER_ROCKWORM_BURST_OUT_DISTANCE_MAX,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Rockworm Burst Out Distance Max",
//                    InternalName = "TAG_POWER_ROCKWORM_BURST_OUT_DISTANCE_MAX",
//                }
//            },
//            {
//                633602, new TagReference
//                {
//                    Id = 633602,
//                    TagType = TagType.TAG_POWER_SHRINE_BUFF_ALLIES,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Shrine Buff Allies",
//                    InternalName = "TAG_POWER_SHRINE_BUFF_ALLIES",
//                }
//            },
//            {
//                633600, new TagReference
//                {
//                    Id = 633600,
//                    TagType = TagType.TAG_POWER_SHRINE_BUFF_ATTRIBUTE,
//                    DataType = MapDataType.Attribute,
//                    DisplayName = "Shrine Buff Atttribute",
//                    InternalName = "TAG_POWER_SHRINE_BUFF_ATTRIBUTE",
//                }
//            },
//            {
//                704512, new TagReference
//                {
//                    Id = 704512,
//                    TagType = TagType.TAG_POWER_SUMMONING_MACHINE_NODE_IS_INVULNERABLE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Summoning Machine Node Is Invulnerable",
//                    InternalName = "TAG_POWER_SUMMONING_MACHINE_NODE_IS_INVULNERABLE",
//                }
//            },
//            {
//                361728, new TagReference
//                {
//                    Id = 361728,
//                    TagType = TagType.TAG_POWER_ENRAGE_AMPLIFY_DAMAGE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Enrage Amplify Damage Percent",
//                    InternalName = "TAG_POWER_ENRAGE_AMPLIFY_DAMAGE_PERCENT",
//                }
//            },
//            {
//                330784, new TagReference
//                {
//                    Id = 330784,
//                    TagType = TagType.TAG_POWER_STUN_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Stun Duration Min",
//                    InternalName = "TAG_POWER_STUN_DURATION_MIN",
//                }
//            },
//            {
//                331568, new TagReference
//                {
//                    Id = 331568,
//                    TagType = TagType.TAG_POWER_BONUS_HITPOINT_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Bonus Hitpoint Percent",
//                    InternalName = "TAG_POWER_BONUS_HITPOINT_PERCENT",
//                }
//            },
//            {
//                684033, new TagReference
//                {
//                    Id = 684033,
//                    TagType = TagType.TAG_POWER_CRITICAL_HIT_EFFECT,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Critical Hit Effect",
//                    InternalName = "TAG_POWER_CRITICAL_HIT_EFFECT",
//                }
//            },
//            {
//                680832, new TagReference
//                {
//                    Id = 680832,
//                    TagType = TagType.TAG_POWER_BOUNCING_BALL_LIFETIME,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Bouncing Ball Lifetime",
//                    InternalName = "TAG_POWER_BOUNCING_BALL_LIFETIME",
//                }
//            },
//            {
//                331192, new TagReference
//                {
//                    Id = 331192,
//                    TagType = TagType.TAG_POWER_PROJECTILE_MIN_TARGET_DISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Projectile Min Target Distance",
//                    InternalName = "TAG_POWER_PROJECTILE_MIN_TARGET_DISTANCE",
//                }
//            },
//            {
//                643072, new TagReference
//                {
//                    Id = 643072,
//                    TagType = TagType.TAG_POWER_HEARTH_TIME,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Hearth Time",
//                    InternalName = "TAG_POWER_HEARTH_TIME",
//                }
//            },
//            {
//                655665, new TagReference
//                {
//                    Id = 655665,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_3,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Generic Buff Attribute 3 Formula",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_3",
//                }
//            },
//            {
//                630787, new TagReference
//                {
//                    Id = 630787,
//                    TagType = TagType.TAG_POWER_SAND_SHIELD_DODGE_CHANCE_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Sand Shield Dodge Chance",
//                    InternalName = "TAG_POWER_SAND_SHIELD_DODGE_CHANCE_BONUS",
//                }
//            },
//            {
//                332449, new TagReference
//                {
//                    Id = 332449,
//                    TagType = TagType.TAG_POWER_SKELETON_MAX_SUMMON_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Skeleton Summon Max Count",
//                    InternalName = "TAG_POWER_SKELETON_MAX_SUMMON_COUNT",
//                }
//            },
//            {
//                263680, new TagReference
//                {
//                    Id = 263680,
//                    TagType = TagType.TAG_POWER_GENERIC_EFFECT_GROUP_1,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Generic Effect Group 1",
//                    InternalName = "TAG_POWER_GENERIC_EFFECT_GROUP_1",
//                }
//            },
//            {
//                431360, new TagReference
//                {
//                    Id = 431360,
//                    TagType = TagType.TAG_POWER_FEIGN_DEATH_AT_HEALTH_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Feign Death At Health Percent",
//                    InternalName = "TAG_POWER_FEIGN_DEATH_AT_HEALTH_PERCENT",
//                }
//            },
//            {
//                700436, new TagReference
//                {
//                    Id = 700436,
//                    TagType = TagType.TAG_POWER_FAITH_GENERATION_BONUS_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Faith Generation Bonus Percent",
//                    InternalName = "TAG_POWER_FAITH_GENERATION_BONUS_PERCENT",
//                }
//            },
//            {
//                610560, new TagReference
//                {
//                    Id = 610560,
//                    TagType = TagType.TAG_POWER_HOOKSHOT_MISS_RETURN_SPEED_MULT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Hookshot Miss Return Speed Mult",
//                    InternalName = "TAG_POWER_HOOKSHOT_MISS_RETURN_SPEED_MULT",
//                }
//            },
//            {
//                330112, new TagReference
//                {
//                    Id = 330112,
//                    TagType = TagType.TAG_POWER_LIGHTNING_DAMAGE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Lightning Damage Min",
//                    InternalName = "TAG_POWER_LIGHTNING_DAMAGE_MIN",
//                }
//            },
//            {
//                338688, new TagReference
//                {
//                    Id = 338688,
//                    TagType = TagType.TAG_POWER_DEATH_STATUE_DAMAGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Death Statue Damage",
//                    InternalName = "TAG_POWER_DEATH_STATUE_DAMAGE",
//                }
//            },
//            {
//                409600, new TagReference
//                {
//                    Id = 409600,
//                    TagType = TagType.TAG_POWER_TELEPORT_INVULNERABLE_SECS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Teleport Invulnerable Secs",
//                    InternalName = "TAG_POWER_TELEPORT_INVULNERABLE_SECS",
//                }
//            },
//            {
//                332848, new TagReference
//                {
//                    Id = 332848,
//                    TagType = TagType.TAG_POWER_DODGE_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Dodge Duration",
//                    InternalName = "TAG_POWER_DODGE_DURATION",
//                }
//            },
//            {
//                332608, new TagReference
//                {
//                    Id = 332608,
//                    TagType = TagType.TAG_POWER_CONCENTRATION_COOLDOWN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Concentration Cooldown",
//                    InternalName = "TAG_POWER_CONCENTRATION_COOLDOWN",
//                }
//            },
//            {
//                330352, new TagReference
//                {
//                    Id = 330352,
//                    TagType = TagType.TAG_POWER_ARCANE_RESISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Arcane Damage Resistance",
//                    InternalName = "TAG_POWER_ARCANE_RESISTANCE",
//                }
//            },
//            {
//                331008, new TagReference
//                {
//                    Id = 331008,
//                    TagType = TagType.TAG_POWER_CRIT_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Crit Chance",
//                    InternalName = "TAG_POWER_CRIT_CHANCE",
//                }
//            },
//            {
//                663808, new TagReference
//                {
//                    Id = 663808,
//                    TagType = TagType.TAG_POWER_STATIC_CHARGE_DAMAGE_INTERVAL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Static Charge Damage Interval",
//                    InternalName = "TAG_POWER_STATIC_CHARGE_DAMAGE_INTERVAL",
//                }
//            },
//            {
//                684915, new TagReference
//                {
//                    Id = 684915,
//                    TagType = TagType.TAG_POWER_SAND_WALL_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Sand Wall Duration Delta",
//                    InternalName = "TAG_POWER_SAND_WALL_DURATION_DELTA",
//                }
//            },
//            {
//                635648, new TagReference
//                {
//                    Id = 635648,
//                    TagType = TagType.TAG_POWER_FROZEN_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Frozen Debuff Duration",
//                    InternalName = "TAG_POWER_FROZEN_DURATION",
//                }
//            },
//            {
//                332784, new TagReference
//                {
//                    Id = 332784,
//                    TagType = TagType.TAG_POWER_DEAD_TIME_UNTIL_RESURRECT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Paladin Dead Time Until Resurrect",
//                    InternalName = "TAG_POWER_DEAD_TIME_UNTIL_RESURRECT",
//                }
//            },
//            {
//                708624, new TagReference
//                {
//                    Id = 708624,
//                    TagType = TagType.TAG_POWER_MASTABLASTA_DRAIN_MANA_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "MastaBlasta Drain Mana Percent",
//                    InternalName = "TAG_POWER_MASTABLASTA_DRAIN_MANA_PERCENT",
//                }
//            },
//            {
//                332512, new TagReference
//                {
//                    Id = 332512,
//                    TagType = TagType.TAG_POWER_SLOW_TIME_PROJECTILE_SPEED_MULTIPLIER,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Slow Time Projectile Speed Multiplier",
//                    InternalName = "TAG_POWER_SLOW_TIME_PROJECTILE_SPEED_MULTIPLIER",
//                }
//            },
//            {
//                721008, new TagReference
//                {
//                    Id = 721008,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_WORLD_MONSTER_POWER,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Death Portal World Monster Power",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_WORLD_MONSTER_POWER",
//                }
//            },
//            {
//                332337, new TagReference
//                {
//                    Id = 332337,
//                    TagType = TagType.TAG_POWER_SPECIAL_WALK_TRAJECTORY_GRAVITY_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Special Walk Trajectory Gravity Delta",
//                    InternalName = "TAG_POWER_SPECIAL_WALK_TRAJECTORY_GRAVITY_DELTA",
//                }
//            },
//            {
//                327712, new TagReference
//                {
//                    Id = 327712,
//                    TagType = TagType.TAG_POWER_SPELL_FUNC_0,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "SpellFunc 0",
//                    InternalName = "TAG_POWER_SPELL_FUNC_0",
//                }
//            },
//            {
//                262662, new TagReference
//                {
//                    Id = 262662,
//                    TagType = TagType.TAG_POWER_ENDING_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Ending Effect Group",
//                    InternalName = "TAG_POWER_ENDING_EFFECT_GROUP",
//                }
//            },
//            {
//                655504, new TagReference
//                {
//                    Id = 655504,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_WEAPON_CLASS_REQUIREMENT,
//                    DataType = MapDataType.WeaponClassRequirement,
//                    DisplayName = "Generic Buff Weapon Class Requirement",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_WEAPON_CLASS_REQUIREMENT",
//                }
//            },
//            {
//                688384, new TagReference
//                {
//                    Id = 688384,
//                    TagType = TagType.TAG_POWER_EXPEL_MONSTER_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Expel Monster Count",
//                    InternalName = "TAG_POWER_EXPEL_MONSTER_COUNT",
//                }
//            },
//            {
//                684577, new TagReference
//                {
//                    Id = 684577,
//                    TagType = TagType.TAG_BLIND_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Blind Duration Min",
//                    InternalName = "TAG_BLIND_DURATION_MIN",
//                }
//            },
//            {
//                685057, new TagReference
//                {
//                    Id = 685057,
//                    TagType = TagType.TAG_POWER_PORTAL_MIN_RANGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Portal Spawn Range Min",
//                    InternalName = "TAG_POWER_PORTAL_MIN_RANGE",
//                }
//            },
//            {
//                262659, new TagReference
//                {
//                    Id = 262659,
//                    TagType = TagType.TAG_POWER_SOURCE_DEST_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Source-Dest Effect Group",
//                    InternalName = "TAG_POWER_SOURCE_DEST_EFFECT_GROUP",
//                }
//            },
//            {
//                262435, new TagReference
//                {
//                    Id = 262435,
//                    TagType = TagType.TAG_POWER_PROJECTILE_SCALE_VELOCITY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Projectile Scale Velocity",
//                    InternalName = "TAG_POWER_PROJECTILE_SCALE_VELOCITY",
//                }
//            },
//            {
//                372736, new TagReference
//                {
//                    Id = 372736,
//                    TagType = TagType.TAG_POWER_STONESPIKE_NUMBER_OF_SPIKES,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Stone Spike Number of Spikes",
//                    InternalName = "TAG_POWER_STONESPIKE_NUMBER_OF_SPIKES",
//                }
//            },
//            {
//                684579, new TagReference
//                {
//                    Id = 684579,
//                    TagType = TagType.TAG_POWER_HIT_CHANCE_DECREASE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Hit Chance Decrease",
//                    InternalName = "TAG_POWER_HIT_CHANCE_DECREASE",
//                }
//            },
//            {
//                330992, new TagReference
//                {
//                    Id = 330992,
//                    TagType = TagType.TAG_POWER_POISON_CLOUD_NUM_INTERVALS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Poison Cloud Num Intervals",
//                    InternalName = "TAG_POWER_POISON_CLOUD_NUM_INTERVALS",
//                }
//            },
//            {
//                331648, new TagReference
//                {
//                    Id = 331648,
//                    TagType = TagType.TAG_POWER_RETALIATION_KNOCKBACK_MAGNITUDE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Retaliation Knockback Magnitude",
//                    InternalName = "TAG_POWER_RETALIATION_KNOCKBACK_MAGNITUDE",
//                }
//            },
//            {
//                700432, new TagReference
//                {
//                    Id = 700432,
//                    TagType = TagType.TAG_POWER_ARCANUM_GAINED_PER_SECOND,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Arcanum Gained Per Second",
//                    InternalName = "TAG_POWER_ARCANUM_GAINED_PER_SECOND",
//                }
//            },
//            {
//                330048, new TagReference
//                {
//                    Id = 330048,
//                    TagType = TagType.TAG_POWER_FIRE_DAMAGE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fire Damage Min",
//                    InternalName = "TAG_POWER_FIRE_DAMAGE_MIN",
//                }
//            },
//            {
//                329872, new TagReference
//                {
//                    Id = 329872,
//                    TagType = TagType.TAG_POWER_LOBBED_PROJECTILE_GRAVITY,
//                    DataType = MapDataType.Force,
//                    DisplayName = "Projectile Gravity",
//                    InternalName = "TAG_POWER_LOBBED_PROJECTILE_GRAVITY",
//                }
//            },
//            {
//                365056, new TagReference
//                {
//                    Id = 365056,
//                    TagType = TagType.TAG_POWER_GHOST_SOULSIPHON_MAX_CHANNELLING_DISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Ghost Soulsiphon Max Channelling Distance",
//                    InternalName = "TAG_POWER_GHOST_SOULSIPHON_MAX_CHANNELLING_DISTANCE",
//                }
//            },
//            {
//                331280, new TagReference
//                {
//                    Id = 331280,
//                    TagType = TagType.TAG_POWER_HITPOINTS_TO_HEAL_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Hitpoints to Heal Percent",
//                    InternalName = "TAG_POWER_HITPOINTS_TO_HEAL_PERCENT",
//                }
//            },
//            {
//                332064, new TagReference
//                {
//                    Id = 332064,
//                    TagType = TagType.TAG_POWER_ELECTROCUTE_ATTACK_DELAY_TIME,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Electrocute Attack Delay Time",
//                    InternalName = "TAG_POWER_ELECTROCUTE_ATTACK_DELAY_TIME",
//                }
//            },
//            {
//                332544, new TagReference
//                {
//                    Id = 332544,
//                    TagType = TagType.TAG_POWER_DODGE_TRAVEL_ANGLE_OFFSET,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Dodge Travel Angle Offset",
//                    InternalName = "TAG_POWER_DODGE_TRAVEL_ANGLE_OFFSET",
//                }
//            },
//            {
//                330288, new TagReference
//                {
//                    Id = 330288,
//                    TagType = TagType.TAG_POWER_POISON_RESISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Poison Damage Resistance",
//                    InternalName = "TAG_POWER_POISON_RESISTANCE",
//                }
//            },
//            {
//                667648, new TagReference
//                {
//                    Id = 667648,
//                    TagType = TagType.TAG_POWER_INTENSIFY_CRIT_CHANCE_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Intensify Crit Chance Bonus",
//                    InternalName = "TAG_POWER_INTENSIFY_CRIT_CHANCE_BONUS",
//                }
//            },
//            {
//                360704, new TagReference
//                {
//                    Id = 360704,
//                    TagType = TagType.TAG_POWER_DESTINATION_JITTER_ATTEMPTS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Destination Jitter Attempts",
//                    InternalName = "TAG_POWER_DESTINATION_JITTER_ATTEMPTS",
//                }
//            },
//            {
//                684914, new TagReference
//                {
//                    Id = 684914,
//                    TagType = TagType.TAG_POWER_SPAWN_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Spawn Duration",
//                    InternalName = "TAG_POWER_SPAWN_DURATION",
//                }
//            },
//            {
//                369536, new TagReference
//                {
//                    Id = 369536,
//                    TagType = TagType.TAG_POWER_ROCKWORM_BURST_OUT_DELAY,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Rockworm Burst Out Delay",
//                    InternalName = "TAG_POWER_ROCKWORM_BURST_OUT_DELAY",
//                }
//            },
//            {
//                332720, new TagReference
//                {
//                    Id = 332720,
//                    TagType = TagType.TAG_POWER_THUNDERING_CRY_BUFF_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Thundering Cry Buff Duration",
//                    InternalName = "TAG_POWER_THUNDERING_CRY_BUFF_DURATION",
//                }
//            },
//            {
//                330432, new TagReference
//                {
//                    Id = 330432,
//                    TagType = TagType.TAG_POWER_SIEGE_DAMAGE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Siege Damage Min",
//                    InternalName = "TAG_POWER_SIEGE_DAMAGE_MIN",
//                }
//            },
//            {
//                331216, new TagReference
//                {
//                    Id = 331216,
//                    TagType = TagType.TAG_POWER_MANA_DRAIN_AMOUNT_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mana Drain Amount Delta",
//                    InternalName = "TAG_POWER_MANA_DRAIN_AMOUNT_DELTA",
//                }
//            },
//            {
//                332448, new TagReference
//                {
//                    Id = 332448,
//                    TagType = TagType.TAG_POWER_SKELETON_SUMMON_COUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Skeleton Summon Count Per Summon",
//                    InternalName = "TAG_POWER_SKELETON_SUMMON_COUNT",
//                }
//            },
//            {
//                332849, new TagReference
//                {
//                    Id = 332849,
//                    TagType = TagType.TAG_POWER_DODGE_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Dodge Chance",
//                    InternalName = "TAG_POWER_DODGE_CHANCE",
//                }
//            },
//            {
//                720944, new TagReference
//                {
//                    Id = 720944,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_ACTOR_3,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Death Portal Actor 3",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_ACTOR_3",
//                }
//            },
//            {
//                680704, new TagReference
//                {
//                    Id = 680704,
//                    TagType = TagType.TAG_POWER_PROC_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Proc Chance",
//                    InternalName = "TAG_POWER_PROC_CHANCE",
//                }
//            },
//            {
//                370176, new TagReference
//                {
//                    Id = 370176,
//                    TagType = TagType.TAG_POWER_FEASTOFSOULS_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Feast of Souls Duration",
//                    InternalName = "TAG_POWER_FEASTOFSOULS_DURATION",
//                }
//            },
//            {
//                262400, new TagReference
//                {
//                    Id = 262400,
//                    TagType = TagType.TAG_POWER_PROJECTILE_SNO,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Projectile Actor",
//                    InternalName = "TAG_POWER_PROJECTILE_SNO",
//                }
//            },
//            {
//                692224, new TagReference
//                {
//                    Id = 692224,
//                    TagType = TagType.TAG_POWER_SLOW_MULTIPLIER,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Slow Movement Speed Multiplier",
//                    InternalName = "TAG_POWER_SLOW_MULTIPLIER",
//                }
//            },
//            {
//                655985, new TagReference
//                {
//                    Id = 655985,
//                    TagType = TagType.TAG_POWER_NOVA_DELAY,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Nova Delay",
//                    InternalName = "TAG_POWER_NOVA_DELAY",
//                }
//            },
//            {
//                332496, new TagReference
//                {
//                    Id = 332496,
//                    TagType = TagType.TAG_POWER_SLOW_TIME_ATTACK_COOLDOWN_INCREASE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Slow Time Attack Cooldown Increase In Seconds",
//                    InternalName = "TAG_POWER_SLOW_TIME_ATTACK_COOLDOWN_INCREASE",
//                }
//            },
//            {
//                685056, new TagReference
//                {
//                    Id = 685056,
//                    TagType = TagType.TAG_POWER_PORTAL_ACTIVATION_DIST,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Portal Activate Range",
//                    InternalName = "TAG_POWER_PORTAL_ACTIVATION_DIST",
//                }
//            },
//            {
//                337664, new TagReference
//                {
//                    Id = 337664,
//                    TagType = TagType.TAG_POWER_SUMMONED_GHOUL_DISEASE_ATTACK_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Summoned Ghoul Disease Attack Chance",
//                    InternalName = "TAG_POWER_SUMMONED_GHOUL_DISEASE_ATTACK_CHANCE",
//                }
//            },
//            {
//                721009, new TagReference
//                {
//                    Id = 721009,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_WORLD_PLAYER_POWER,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Death Portal World Player Power",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_WORLD_PLAYER_POWER",
//                }
//            },
//            {
//                332321, new TagReference
//                {
//                    Id = 332321,
//                    TagType = TagType.TAG_POWER_SPECIAL_WALK_TRAJECTORY_HEIGHT_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Special Walk Trajectory Height Delta",
//                    InternalName = "TAG_POWER_SPECIAL_WALK_TRAJECTORY_HEIGHT_DELTA",
//                }
//            },
//            {
//                328096, new TagReference
//                {
//                    Id = 328096,
//                    TagType = TagType.TAG_POWER_TARGET_ALLIES,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetAllies",
//                    InternalName = "TAG_POWER_TARGET_ALLIES",
//                }
//            },
//            {
//                330928, new TagReference
//                {
//                    Id = 330928,
//                    TagType = TagType.TAG_POWER_SLOW_AMOUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Slow Amount",
//                    InternalName = "TAG_POWER_SLOW_AMOUNT",
//                }
//            },
//            {
//                331584, new TagReference
//                {
//                    Id = 331584,
//                    TagType = TagType.TAG_POWER_BONUS_MANA_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Bonus Mana Percent",
//                    InternalName = "TAG_POWER_BONUS_MANA_PERCENT",
//                }
//            },
//            {
//                329982, new TagReference
//                {
//                    Id = 329982,
//                    TagType = TagType.TAG_POWER_DOT_POWER,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Dot Power",
//                    InternalName = "TAG_POWER_DOT_POWER",
//                }
//            },
//            {
//                329984, new TagReference
//                {
//                    Id = 329984,
//                    TagType = TagType.TAG_POWER_PHYSICAL_DAMAGE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Physical Damage Min",
//                    InternalName = "TAG_POWER_PHYSICAL_DAMAGE_MIN",
//                }
//            },
//            {
//                330592, new TagReference
//                {
//                    Id = 330592,
//                    TagType = TagType.TAG_POWER_DISEASE_DAMAGE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Disease Damage Percent",
//                    InternalName = "TAG_POWER_DISEASE_DAMAGE_PERCENT",
//                }
//            },
//            {
//                614400, new TagReference
//                {
//                    Id = 614400,
//                    TagType = TagType.TAG_POWER_MASSCONFUSION_BUFF_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mass Confusion Buff Duration",
//                    InternalName = "TAG_POWER_MASSCONFUSION_BUFF_DURATION",
//                }
//            },
//            {
//                331705, new TagReference
//                {
//                    Id = 331705,
//                    TagType = TagType.TAG_POWER_KNOCKBACK_HEIGHT_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Knockback Height Delta",
//                    InternalName = "TAG_POWER_KNOCKBACK_HEIGHT_DELTA",
//                }
//            },
//            {
//                655681, new TagReference
//                {
//                    Id = 655681,
//                    TagType = TagType.TAG_POWER_DEBUFF_REFRESH_INTERVAL_IN_SECONDS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Debuff Refresh Interval In Seconds",
//                    InternalName = "TAG_POWER_DEBUFF_REFRESH_INTERVAL_IN_SECONDS",
//                }
//            },
//            {
//                332180, new TagReference
//                {
//                    Id = 332180,
//                    TagType = TagType.TAG_POWER_FURY_COEFFICIENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fury Coefficient",
//                    InternalName = "TAG_POWER_FURY_COEFFICIENT",
//                }
//            },
//            {
//                397312, new TagReference
//                {
//                    Id = 397312,
//                    TagType = TagType.TAG_POWER_SPECTRALBLADE_BLEED_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Spectral Blade Bleed Duration",
//                    InternalName = "TAG_POWER_SPECTRALBLADE_BLEED_DURATION",
//                }
//            },
//            {
//                630785, new TagReference
//                {
//                    Id = 630785,
//                    TagType = TagType.TAG_POWER_SAND_SHIELD_PROJECTILE_REFLECT_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Sand Shield Projectile Reflect Chance",
//                    InternalName = "TAG_POWER_SAND_SHIELD_PROJECTILE_REFLECT_CHANCE",
//                }
//            },
//            {
//                655650, new TagReference
//                {
//                    Id = 655650,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_2_WITH_PARAM,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Generic Buff Attribute 2 And Parameter",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_2_WITH_PARAM",
//                }
//            },
//            {
//                339456, new TagReference
//                {
//                    Id = 339456,
//                    TagType = TagType.TAG_POWER_SUMMON_CHANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Summon Chance",
//                    InternalName = "TAG_POWER_SUMMON_CHANCE",
//                }
//            },
//            {
//                332048, new TagReference
//                {
//                    Id = 332048,
//                    TagType = TagType.TAG_POWER_BATTLE_RAGE_MAX_STACK_LEVEL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Battle Rage Max Stack Level",
//                    InternalName = "TAG_POWER_BATTLE_RAGE_MAX_STACK_LEVEL",
//                }
//            },
//            {
//                331968, new TagReference
//                {
//                    Id = 331968,
//                    TagType = TagType.TAG_POWER_ATTACK_SPEED_PERCENT_INCREASE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Attack Speed Percent Increase Min",
//                    InternalName = "TAG_POWER_ATTACK_SPEED_PERCENT_INCREASE_MIN",
//                }
//            },
//            {
//                332656, new TagReference
//                {
//                    Id = 332656,
//                    TagType = TagType.TAG_POWER_TORNADO_LIFE_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Tornado Life Duration Min",
//                    InternalName = "TAG_POWER_TORNADO_LIFE_DURATION_MIN",
//                }
//            },
//            {
//                631297, new TagReference
//                {
//                    Id = 631297,
//                    TagType = TagType.TAG_POWER_QUICKSAND_SLOWAMOUNT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Quicksand Slow Amount",
//                    InternalName = "TAG_POWER_QUICKSAND_SLOWAMOUNT",
//                }
//            },
//            {
//                332341, new TagReference
//                {
//                    Id = 332341,
//                    TagType = TagType.TAG_POWER_SPECIAL_WALK_GO_THROUGH_OCCLUDED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Special Walk Go Through Occluded",
//                    InternalName = "TAG_POWER_SPECIAL_WALK_GO_THROUGH_OCCLUDED",
//                }
//            },
//            {
//                331152, new TagReference
//                {
//                    Id = 331152,
//                    TagType = TagType.TAG_POWER_GOLD_FIND_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Gold Find Bonus",
//                    InternalName = "TAG_POWER_GOLD_FIND_BONUS",
//                }
//            },
//            {
//                700434, new TagReference
//                {
//                    Id = 700434,
//                    TagType = TagType.TAG_POWER_FURY_GENERATION_BONUS_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fury Generation Bonus Percent",
//                    InternalName = "TAG_POWER_FURY_GENERATION_BONUS_PERCENT",
//                }
//            },
//            {
//                700433, new TagReference
//                {
//                    Id = 700433,
//                    TagType = TagType.TAG_POWER_SPIRIT_GENERATION_BONUS_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Spirit Generation Bonus Percent",
//                    InternalName = "TAG_POWER_SPIRIT_GENERATION_BONUS_PERCENT",
//                }
//            },
//            {
//                639488, new TagReference
//                {
//                    Id = 639488,
//                    TagType = TagType.TAG_POWER_DOESNT_CRIT,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Doesn't Crit",
//                    InternalName = "TAG_POWER_DOESNT_CRIT",
//                }
//            },
//            {
//                331196, new TagReference
//                {
//                    Id = 331196,
//                    TagType = TagType.TAG_POWER_PROJECTILE_JITTER,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Projectile Jitter",
//                    InternalName = "TAG_POWER_PROJECTILE_JITTER",
//                }
//            },
//            {
//                332384, new TagReference
//                {
//                    Id = 332384,
//                    TagType = TagType.TAG_POWER_DODGE_TRAVEL_DISTANCE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Dodge Travel Distance Min",
//                    InternalName = "TAG_POWER_DODGE_TRAVEL_DISTANCE_MIN",
//                }
//            },
//            {
//                721010, new TagReference
//                {
//                    Id = 721010,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_WORLD_QUEST_DESCRIPTION,
//                    DataType = MapDataType.Description,
//                    DisplayName = "Death Portal World Quest Description",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_WORLD_QUEST_DESCRIPTION",
//                }
//            },
//            {
//                638976, new TagReference
//                {
//                    Id = 638976,
//                    TagType = TagType.TAG_POWER_DEATH_NOVA_RADIUS_MAX,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Death Nova Radius Max",
//                    InternalName = "TAG_POWER_DEATH_NOVA_RADIUS_MAX",
//                }
//            },
//            {
//                331704, new TagReference
//                {
//                    Id = 331704,
//                    TagType = TagType.TAG_POWER_KNOCKBACK_HEIGHT_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Knockback Height Min",
//                    InternalName = "TAG_POWER_KNOCKBACK_HEIGHT_MIN",
//                }
//            },
//            {
//                329800, new TagReference
//                {
//                    Id = 329800,
//                    TagType = TagType.TAG_POWER_MAX_TARGETS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Max Targets",
//                    InternalName = "TAG_POWER_MAX_TARGETS",
//                }
//            },
//            {
//                606464, new TagReference
//                {
//                    Id = 606464,
//                    TagType = TagType.TAG_POWER_ROOT_END_FUNC,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "Root End Func",
//                    InternalName = "TAG_POWER_ROOT_END_FUNC",
//                }
//            },
//            {
//                262401, new TagReference
//                {
//                    Id = 262401,
//                    TagType = TagType.TAG_POWER_EXPLOSION_SNO,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Explosion Actor",
//                    InternalName = "TAG_POWER_EXPLOSION_SNO",
//                }
//            },
//            {
//                331194, new TagReference
//                {
//                    Id = 331194,
//                    TagType = TagType.TAG_POWER_PROJECTILE_SPREAD_ANGLE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Projectile Spread Angle",
//                    InternalName = "TAG_POWER_PROJECTILE_SPREAD_ANGLE",
//                }
//            },
//            {
//                681728, new TagReference
//                {
//                    Id = 681728,
//                    TagType = TagType.TAG_POWER_TEMPORAL_ARMOR_COOLDOWN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Temporal Armor Cooldown",
//                    InternalName = "TAG_POWER_TEMPORAL_ARMOR_COOLDOWN",
//                }
//            },
//            {
//                421888, new TagReference
//                {
//                    Id = 421888,
//                    TagType = TagType.TAG_POWER_FINDITEM_MAGIC_FIND_BONUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Find Item Magic Find Bonus",
//                    InternalName = "TAG_POWER_FINDITEM_MAGIC_FIND_BONUS",
//                }
//            },
//            {
//                330864, new TagReference
//                {
//                    Id = 330864,
//                    TagType = TagType.TAG_POWER_IMMOBILIZE_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Immobilize Duration Min",
//                    InternalName = "TAG_POWER_IMMOBILIZE_DURATION_MIN",
//                }
//            },
//            {
//                331520, new TagReference
//                {
//                    Id = 331520,
//                    TagType = TagType.TAG_POWER_MANA_GAINED_PER_SECOND,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mana Gained Per Second",
//                    InternalName = "TAG_POWER_MANA_GAINED_PER_SECOND",
//                }
//            },
//            {
//                708656, new TagReference
//                {
//                    Id = 708656,
//                    TagType = TagType.TAG_POWER_MASTABLASTA_DRAIN_FURY_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "MastaBlasta Drain Fury Percent",
//                    InternalName = "TAG_POWER_MASTABLASTA_DRAIN_FURY_PERCENT",
//                }
//            },
//            {
//                328168, new TagReference
//                {
//                    Id = 328168,
//                    TagType = TagType.TAG_POWER_TARGET_IGNORE_WRECKABLES,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetIgnoreWreckables",
//                    InternalName = "TAG_POWER_TARGET_IGNORE_WRECKABLES",
//                }
//            },
//            {
//                330528, new TagReference
//                {
//                    Id = 330528,
//                    TagType = TagType.TAG_POWER_HOLY_DAMAGE_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Holy Damage Percent",
//                    InternalName = "TAG_POWER_HOLY_DAMAGE_PERCENT",
//                }
//            },
//            {
//                615936, new TagReference
//                {
//                    Id = 615936,
//                    TagType = TagType.TAG_POWER_BONEREAVER_HEAL_PERCENTAGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Bonereaver Heal Percentage",
//                    InternalName = "TAG_POWER_BONEREAVER_HEAL_PERCENTAGE",
//                }
//            },
//            {
//                655617, new TagReference
//                {
//                    Id = 655617,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_0,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Generic Buff Attribute 0 Formula",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_0",
//                }
//            },
//            {
//                631552, new TagReference
//                {
//                    Id = 631552,
//                    TagType = TagType.TAG_POWER_RAPTORSPEED_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Raptor Speed Duration",
//                    InternalName = "TAG_POWER_RAPTORSPEED_DURATION",
//                }
//            },
//            {
//                684930, new TagReference
//                {
//                    Id = 684930,
//                    TagType = TagType.TAG_POWER_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Duration Delta",
//                    InternalName = "TAG_POWER_DURATION_DELTA",
//                }
//            },
//            {
//                631040, new TagReference
//                {
//                    Id = 631040,
//                    TagType = TagType.TAG_POWER_RAMPAGE_EXTRA_HITS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Rampage Extra Attacks",
//                    InternalName = "TAG_POWER_RAMPAGE_EXTRA_HITS",
//                }
//            },
//            {
//                684224, new TagReference
//                {
//                    Id = 684224,
//                    TagType = TagType.TAG_POWER_IMPENETRABLE_DEFENSE_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Impenetrable Defense Duration",
//                    InternalName = "TAG_POWER_IMPENETRABLE_DEFENSE_DURATION",
//                }
//            },
//            {
//                330192, new TagReference
//                {
//                    Id = 330192,
//                    TagType = TagType.TAG_POWER_COLD_DAMAGE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Cold Damage Delta",
//                    InternalName = "TAG_POWER_COLD_DAMAGE_DELTA",
//                }
//            },
//            {
//                655648, new TagReference
//                {
//                    Id = 655648,
//                    TagType = TagType.TAG_POWER_GENERIC_BUFF_ATTRIBUTE_2,
//                    DataType = MapDataType.Attribute,
//                    DisplayName = "Generic Buff Attribute 2",
//                    InternalName = "TAG_POWER_GENERIC_BUFF_ATTRIBUTE_2",
//                }
//            },
//            {
//                685058, new TagReference
//                {
//                    Id = 685058,
//                    TagType = TagType.TAG_POWER_PORTAL_DELTA_RANGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Portal Spawn Range Delta",
//                    InternalName = "TAG_POWER_PORTAL_DELTA_RANGE",
//                }
//            },
//            {
//                633345, new TagReference
//                {
//                    Id = 633345,
//                    TagType = TagType.TAG_POWER_CAUSES_KNOCKDOWN,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Causes Knockdown",
//                    InternalName = "TAG_POWER_CAUSES_KNOCKDOWN",
//                }
//            },
//            {
//                333024, new TagReference
//                {
//                    Id = 333024,
//                    TagType = TagType.TAG_POWER_BLIZZARD_TIME_BETWEEN_UPDATES,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Blizzard Time Between Updates",
//                    InternalName = "TAG_POWER_BLIZZARD_TIME_BETWEEN_UPDATES",
//                }
//            },
//            {
//                331904, new TagReference
//                {
//                    Id = 331904,
//                    TagType = TagType.TAG_POWER_TEMPLAR_PROTECTION_DAMAGE_ABSORB_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Templar Devotion Damage Absorb Percent",
//                    InternalName = "TAG_POWER_TEMPLAR_PROTECTION_DAMAGE_ABSORB_PERCENT",
//                }
//            },
//            {
//                332592, new TagReference
//                {
//                    Id = 332592,
//                    TagType = TagType.TAG_POWER_RING_OF_FROST_SLOW_DURATION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Ring of Frost Slow Duration",
//                    InternalName = "TAG_POWER_RING_OF_FROST_SLOW_DURATION",
//                }
//            },
//            {
//                330304, new TagReference
//                {
//                    Id = 330304,
//                    TagType = TagType.TAG_POWER_ARCANE_DAMAGE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Arcane Damage Min",
//                    InternalName = "TAG_POWER_ARCANE_DAMAGE_MIN",
//                }
//            },
//            {
//                331088, new TagReference
//                {
//                    Id = 331088,
//                    TagType = TagType.TAG_POWER_PAYLOAD_AFFECTED_BY_PET_AOE_SCALAR,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Payload Affected By Pet AOE Scalar",
//                    InternalName = "TAG_POWER_PAYLOAD_AFFECTED_BY_PET_AOE_SCALAR",
//                }
//            },
//            {
//                262660, new TagReference
//                {
//                    Id = 262660,
//                    TagType = TagType.TAG_POWER_CASTING_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Casting Effect Group - Male",
//                    InternalName = "TAG_POWER_CASTING_EFFECT_GROUP",
//                }
//            },
//            {
//                332320, new TagReference
//                {
//                    Id = 332320,
//                    TagType = TagType.TAG_POWER_SPECIAL_WALK_TRAJECTORY_HEIGHT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Special Walk Trajectory Height",
//                    InternalName = "TAG_POWER_SPECIAL_WALK_TRAJECTORY_HEIGHT",
//                }
//            },
//            {
//                368640, new TagReference
//                {
//                    Id = 368640,
//                    TagType = TagType.TAG_POWER_ROCKWORM_WEB_SPIT_DISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Rockworm Web Spit Distance",
//                    InternalName = "TAG_POWER_ROCKWORM_WEB_SPIT_DISTANCE",
//                }
//            },
//            {
//                264194, new TagReference
//                {
//                    Id = 264194,
//                    TagType = TagType.TAG_POWER_CONTACT_FRAME_EFFECT_GROUP_2,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Contact Frame Effect Group - Male 2",
//                    InternalName = "TAG_POWER_CONTACT_FRAME_EFFECT_GROUP_2",
//                }
//            },
//            {
//                360960, new TagReference
//                {
//                    Id = 360960,
//                    TagType = TagType.TAG_POWER_ROOT_TIMER_MODIFICATION_PER_STRUGGLE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Root Timer Modification Per Struggle",
//                    InternalName = "TAG_POWER_ROOT_TIMER_MODIFICATION_PER_STRUGGLE",
//                }
//            },
//            {
//                332368, new TagReference
//                {
//                    Id = 332368,
//                    TagType = TagType.TAG_POWER_CRIT_DAMAGE_BONUS_PERCENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Crit Damage Bonus Percent",
//                    InternalName = "TAG_POWER_CRIT_DAMAGE_BONUS_PERCENT",
//                }
//            },
//            {
//                721011, new TagReference
//                {
//                    Id = 721011,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_WORLD_QUEST_TITLE,
//                    DataType = MapDataType.Title,
//                    DisplayName = "Death Portal World Quest Title",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_WORLD_QUEST_TITLE",
//                }
//            },
//            {
//                720960, new TagReference
//                {
//                    Id = 720960,
//                    TagType = TagType.TAG_POWER_DEATH_PORTAL_DELAY,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Death Portal Delay",
//                    InternalName = "TAG_POWER_DEATH_PORTAL_DELAY",
//                }
//            },
//            {
//                262408, new TagReference
//                {
//                    Id = 262408,
//                    TagType = TagType.TAG_POWER_PROJECTILE_THROW_OVER,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Projectile Throw Over Guys",
//                    InternalName = "TAG_POWER_PROJECTILE_THROW_OVER",
//                }
//            },
//            {
//                327968, new TagReference
//                {
//                    Id = 327968,
//                    TagType = TagType.TAG_POWER_NEVER_CAUSES_RECOIL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "NeverCausesRecoil",
//                    InternalName = "TAG_POWER_NEVER_CAUSES_RECOIL",
//                }
//            },
//            {
//                330800, new TagReference
//                {
//                    Id = 330800,
//                    TagType = TagType.TAG_POWER_STUN_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Stun Duration Delta",
//                    InternalName = "TAG_POWER_STUN_DURATION_DELTA",
//                }
//            },
//            {
//                663552, new TagReference
//                {
//                    Id = 663552,
//                    TagType = TagType.TAG_POWER_STATIC_CHARGE_DAMAGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Static Charge Damage",
//                    InternalName = "TAG_POWER_STATIC_CHARGE_DAMAGE",
//                }
//            },
//            {
//                198400, new TagReference
//                {
//                    Id = 198400,
//                    TagType = TagType.TAG_SHADERMAP_MIN_DEFAULT,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Default Min",
//                    InternalName = "TAG_SHADERMAP_MIN_DEFAULT",
//                }
//            },
//            {
//                198145, new TagReference
//                {
//                    Id = 198145,
//                    TagType = TagType.TAG_SHADERMAP_HIGH_FADE,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Fade High",
//                    InternalName = "TAG_SHADERMAP_HIGH_FADE",
//                }
//            },
//            {
//                198752, new TagReference
//                {
//                    Id = 198752,
//                    TagType = TagType.TAG_SHADERMAP_HIGH_PP_FADE,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Fade High Per-Pixel",
//                    InternalName = "TAG_SHADERMAP_HIGH_PP_FADE",
//                }
//            },
//            {
//                198688, new TagReference
//                {
//                    Id = 198688,
//                    TagType = TagType.TAG_SHADERMAP_OCCLUDED,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Occluded",
//                    InternalName = "TAG_SHADERMAP_OCCLUDED",
//                }
//            },
//            {
//                197892, new TagReference
//                {
//                    Id = 197892,
//                    TagType = TagType.TAG_SHADERMAP_MED_UNSKINNED,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Unskinned Medium",
//                    InternalName = "TAG_SHADERMAP_MED_UNSKINNED",
//                }
//            },
//            {
//                198146, new TagReference
//                {
//                    Id = 198146,
//                    TagType = TagType.TAG_SHADERMAP_HIGH_REFLECTION,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Reflection High",
//                    InternalName = "TAG_SHADERMAP_HIGH_REFLECTION",
//                }
//            },
//            {
//                198768, new TagReference
//                {
//                    Id = 198768,
//                    TagType = TagType.TAG_SHADERMAP_PREPASS,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Depth Prepass",
//                    InternalName = "TAG_SHADERMAP_PREPASS",
//                }
//            },
//            {
//                198704, new TagReference
//                {
//                    Id = 198704,
//                    TagType = TagType.TAG_SHADERMAP_CONSOLE_DEFAULT,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Default Console",
//                    InternalName = "TAG_SHADERMAP_CONSOLE_DEFAULT",
//                }
//            },
//            {
//                198404, new TagReference
//                {
//                    Id = 198404,
//                    TagType = TagType.TAG_SHADERMAP_MIN_UNSKINNED,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Unskinned Min",
//                    InternalName = "TAG_SHADERMAP_MIN_UNSKINNED",
//                }
//            },
//            {
//                198144, new TagReference
//                {
//                    Id = 198144,
//                    TagType = TagType.TAG_SHADERMAP_HIGH_DEFAULT,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Default High",
//                    InternalName = "TAG_SHADERMAP_HIGH_DEFAULT",
//                }
//            },
//            {
//                198401, new TagReference
//                {
//                    Id = 198401,
//                    TagType = TagType.TAG_SHADERMAP_MIN_FADE,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Fade Min",
//                    InternalName = "TAG_SHADERMAP_MIN_FADE",
//                }
//            },
//            {
//                198784, new TagReference
//                {
//                    Id = 198784,
//                    TagType = TagType.TAG_SHADERMAP_MASK,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Stencil Mask",
//                    InternalName = "TAG_SHADERMAP_MASK",
//                }
//            },
//            {
//                198720, new TagReference
//                {
//                    Id = 198720,
//                    TagType = TagType.TAG_SHADERMAP_COOKIE,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Shadow Cookie",
//                    InternalName = "TAG_SHADERMAP_COOKIE",
//                }
//            },
//            {
//                198656, new TagReference
//                {
//                    Id = 198656,
//                    TagType = TagType.TAG_SHADERMAP_GHOST_ZPASS,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Ghost Z pass",
//                    InternalName = "TAG_SHADERMAP_GHOST_ZPASS",
//                }
//            },
//            {
//                197889, new TagReference
//                {
//                    Id = 197889,
//                    TagType = TagType.TAG_SHADERMAP_MED_FADE,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Fade Medium",
//                    InternalName = "TAG_SHADERMAP_MED_FADE",
//                }
//            },
//            {
//                198148, new TagReference
//                {
//                    Id = 198148,
//                    TagType = TagType.TAG_SHADERMAP_HIGH_UNSKINNED,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Unskinned High",
//                    InternalName = "TAG_SHADERMAP_HIGH_UNSKINNED",
//                }
//            },
//            {
//                198402, new TagReference
//                {
//                    Id = 198402,
//                    TagType = TagType.TAG_SHADERMAP_MIN_REFLECTION,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Reflection Min",
//                    InternalName = "TAG_SHADERMAP_MIN_REFLECTION",
//                }
//            },
//            {
//                197888, new TagReference
//                {
//                    Id = 197888,
//                    TagType = TagType.TAG_SHADERMAP_MED_DEFAULT,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Default Medium",
//                    InternalName = "TAG_SHADERMAP_MED_DEFAULT",
//                }
//            },
//            {
//                198736, new TagReference
//                {
//                    Id = 198736,
//                    TagType = TagType.TAG_SHADERMAP_HIGH_PP_DEFAULT,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Default High Per-Pixel",
//                    InternalName = "TAG_SHADERMAP_HIGH_PP_DEFAULT",
//                }
//            },
//            {
//                198672, new TagReference
//                {
//                    Id = 198672,
//                    TagType = TagType.TAG_SHADERMAP_HIGHLIGHT,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Highlight",
//                    InternalName = "TAG_SHADERMAP_HIGHLIGHT",
//                }
//            },
//            {
//                197890, new TagReference
//                {
//                    Id = 197890,
//                    TagType = TagType.TAG_SHADERMAP_MED_REFLECTION,
//                    DataType = MapDataType.ShaderMap,
//                    DisplayName = "Reflection Medium",
//                    InternalName = "TAG_SHADERMAP_MED_REFLECTION",
//                }
//            },
//            {
//                655423, new TagReference
//                {
//                    Id = 655423,
//                    TagType = TagType.TAG_VS_EARLY_DEPTH_STENCIL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "EarlyDepthStencil - Set Internally",
//                    InternalName = "TAG_VS_EARLY_DEPTH_STENCIL",
//                }
//            },
//            {
//                655390, new TagReference
//                {
//                    Id = 655390,
//                    TagType = TagType.TAG_VS_PS2A_FUNC,
//                    DataType = MapDataType.Func,
//                    DisplayName = "Stage 3 Alpha Function",
//                    InternalName = "TAG_VS_PS2A_FUNC",
//                }
//            },
//            {
//                655421, new TagReference
//                {
//                    Id = 655421,
//                    TagType = TagType.TAG_VS_FLIP_NORMAL_BACKFACE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Flip Normal (BackFace)",
//                    InternalName = "TAG_VS_FLIP_NORMAL_BACKFACE",
//                }
//            },
//            {
//                655388, new TagReference
//                {
//                    Id = 655388,
//                    TagType = TagType.TAG_VS_PS0A_FUNC,
//                    DataType = MapDataType.Func,
//                    DisplayName = "Stage 1 Alpha Function",
//                    InternalName = "TAG_VS_PS0A_FUNC",
//                }
//            },
//            {
//                655415, new TagReference
//                {
//                    Id = 655415,
//                    TagType = TagType.TAG_VS_USES_TANGENTS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Uses Tangents",
//                    InternalName = "TAG_VS_USES_TANGENTS",
//                }
//            },
//            {
//                655414, new TagReference
//                {
//                    Id = 655414,
//                    TagType = TagType.TAG_VS_VB_FORMAT,
//                    DataType = MapDataType.VBFormat,
//                    DisplayName = "VB Format",
//                    InternalName = "TAG_VS_VB_FORMAT",
//                }
//            },
//            {
//                655413, new TagReference
//                {
//                    Id = 655413,
//                    TagType = TagType.TAG_VS_ALPHATESTFUNC,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Alphatest Cmp Func - Set Internally",
//                    InternalName = "TAG_VS_ALPHATESTFUNC",
//                }
//            },
//            {
//                655412, new TagReference
//                {
//                    Id = 655412,
//                    TagType = TagType.TAG_VS_SHADOW_TECHNIQUE,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Shadow Technique - Set Internally",
//                    InternalName = "TAG_VS_SHADOW_TECHNIQUE",
//                }
//            },
//            {
//                655406, new TagReference
//                {
//                    Id = 655406,
//                    TagType = TagType.TAG_VS_FLOAT_TEX_COORD,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Float Tex Coords",
//                    InternalName = "TAG_VS_FLOAT_TEX_COORD",
//                }
//            },
//            {
//                655404, new TagReference
//                {
//                    Id = 655404,
//                    TagType = TagType.TAG_VS_CLOTH,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Enable Cloth",
//                    InternalName = "TAG_VS_CLOTH",
//                }
//            },
//            {
//                655371, new TagReference
//                {
//                    Id = 655371,
//                    TagType = TagType.TAG_VS_NUM_WAVES,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "# of Waves",
//                    InternalName = "TAG_VS_NUM_WAVES",
//                }
//            },
//            {
//                655369, new TagReference
//                {
//                    Id = 655369,
//                    TagType = TagType.TAG_VS_NUM_SPOT_LIGHTS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "# of Spot Lights",
//                    InternalName = "TAG_VS_NUM_SPOT_LIGHTS",
//                }
//            },
//            {
//                655363, new TagReference
//                {
//                    Id = 655363,
//                    TagType = TagType.TAG_VS_NUM_BONE_WEIGHTS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "# of Weights Per Bone",
//                    InternalName = "TAG_VS_NUM_BONE_WEIGHTS",
//                }
//            },
//            {
//                655362, new TagReference
//                {
//                    Id = 655362,
//                    TagType = TagType.TAG_VS_ENABLE_SKINNING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Enable Skinning",
//                    InternalName = "TAG_VS_ENABLE_SKINNING",
//                }
//            },
//            {
//                655422, new TagReference
//                {
//                    Id = 655422,
//                    TagType = TagType.TAG_VS_SSAO,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "SSAO",
//                    InternalName = "TAG_VS_SSAO",
//                }
//            },
//            {
//                655420, new TagReference
//                {
//                    Id = 655420,
//                    TagType = TagType.TAG_VS_PERPIXEL_LIGHTING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Per-Pixel Lighting",
//                    InternalName = "TAG_VS_PERPIXEL_LIGHTING",
//                }
//            },
//            {
//                655387, new TagReference
//                {
//                    Id = 655387,
//                    TagType = TagType.TAG_VS_PS5C_FUNC,
//                    DataType = MapDataType.Func,
//                    DisplayName = "Stage 6 Color Function",
//                    InternalName = "TAG_VS_PS5C_FUNC",
//                }
//            },
//            {
//                655385, new TagReference
//                {
//                    Id = 655385,
//                    TagType = TagType.TAG_VS_PS3C_FUNC,
//                    DataType = MapDataType.Func,
//                    DisplayName = "Stage 4 Color Function",
//                    InternalName = "TAG_VS_PS3C_FUNC",
//                }
//            },
//            {
//                655379, new TagReference
//                {
//                    Id = 655379,
//                    TagType = TagType.TAG_VS_TEXCOORD3_FUNC,
//                    DataType = MapDataType.LocationFunc,
//                    DisplayName = "TexCoord4 Function",
//                    InternalName = "TAG_VS_TEXCOORD3_FUNC",
//                }
//            },
//            {
//                655378, new TagReference
//                {
//                    Id = 655378,
//                    TagType = TagType.TAG_VS_TEXCOORD2_FUNC,
//                    DataType = MapDataType.LocationFunc,
//                    DisplayName = "TexCoord3 Function",
//                    InternalName = "TAG_VS_TEXCOORD2_FUNC",
//                }
//            },
//            {
//                655377, new TagReference
//                {
//                    Id = 655377,
//                    TagType = TagType.TAG_VS_TEXCOORD1_FUNC,
//                    DataType = MapDataType.LocationFunc,
//                    DisplayName = "TexCoord2 Function",
//                    InternalName = "TAG_VS_TEXCOORD1_FUNC",
//                }
//            },
//            {
//                655376, new TagReference
//                {
//                    Id = 655376,
//                    TagType = TagType.TAG_VS_TEXCOORD0_FUNC,
//                    DataType = MapDataType.LocationFunc,
//                    DisplayName = "TexCoord1 Function",
//                    InternalName = "TAG_VS_TEXCOORD0_FUNC",
//                }
//            },
//            {
//                655403, new TagReference
//                {
//                    Id = 655403,
//                    TagType = TagType.TAG_VS_PMA_FUNC,
//                    DataType = MapDataType.PMAFunc,
//                    DisplayName = "PMA Func",
//                    InternalName = "TAG_VS_PMA_FUNC",
//                }
//            },
//            {
//                655370, new TagReference
//                {
//                    Id = 655370,
//                    TagType = TagType.TAG_VS_NUM_DIRECTIONAL_LIGHTS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "# of Directional Lights",
//                    InternalName = "TAG_VS_NUM_DIRECTIONAL_LIGHTS",
//                }
//            },
//            {
//                655401, new TagReference
//                {
//                    Id = 655401,
//                    TagType = TagType.TAG_VS_SHADER_MODEL,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Shader Model - Set Internally",
//                    InternalName = "TAG_VS_SHADER_MODEL",
//                }
//            },
//            {
//                655368, new TagReference
//                {
//                    Id = 655368,
//                    TagType = TagType.TAG_VS_NUM_POINT_LIGHTS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "# of Point Lights",
//                    InternalName = "TAG_VS_NUM_POINT_LIGHTS",
//                }
//            },
//            {
//                655395, new TagReference
//                {
//                    Id = 655395,
//                    TagType = TagType.TAG_VS_TINT,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Enable Tint",
//                    InternalName = "TAG_VS_TINT",
//                }
//            },
//            {
//                655419, new TagReference
//                {
//                    Id = 655419,
//                    TagType = TagType.TAG_VS_ENABLE_NEAR_FADE_IN,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Near Fade In",
//                    InternalName = "TAG_VS_ENABLE_NEAR_FADE_IN",
//                }
//            },
//            {
//                655386, new TagReference
//                {
//                    Id = 655386,
//                    TagType = TagType.TAG_VS_PS4C_FUNC,
//                    DataType = MapDataType.Func,
//                    DisplayName = "Stage 5 Color Function",
//                    InternalName = "TAG_VS_PS4C_FUNC",
//                }
//            },
//            {
//                655417, new TagReference
//                {
//                    Id = 655417,
//                    TagType = TagType.TAG_VS_MESH_LIGHTING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Meshlighting Enabled",
//                    InternalName = "TAG_VS_MESH_LIGHTING",
//                }
//            },
//            {
//                655384, new TagReference
//                {
//                    Id = 655384,
//                    TagType = TagType.TAG_VS_PS2C_FUNC,
//                    DataType = MapDataType.Func,
//                    DisplayName = "Stage 3 Color Function",
//                    InternalName = "TAG_VS_PS2C_FUNC",
//                }
//            },
//            {
//                655411, new TagReference
//                {
//                    Id = 655411,
//                    TagType = TagType.TAG_VS_SHADOW_ENABLED,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Shadows Enabled - Set Internally",
//                    InternalName = "TAG_VS_SHADOW_ENABLED",
//                }
//            },
//            {
//                655410, new TagReference
//                {
//                    Id = 655410,
//                    TagType = TagType.TAG_VS_DIFF_ALPHA_IS_GLOSS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Diff alpha is gloss",
//                    InternalName = "TAG_VS_DIFF_ALPHA_IS_GLOSS",
//                }
//            },
//            {
//                655409, new TagReference
//                {
//                    Id = 655409,
//                    TagType = TagType.TAG_VS_WEATHER_SCALES_DEFORM,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Weather scales deformation",
//                    InternalName = "TAG_VS_WEATHER_SCALES_DEFORM",
//                }
//            },
//            {
//                655408, new TagReference
//                {
//                    Id = 655408,
//                    TagType = TagType.TAG_VS_ENABLE_DEFORM,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Enable Deformation",
//                    InternalName = "TAG_VS_ENABLE_DEFORM",
//                }
//            },
//            {
//                655375, new TagReference
//                {
//                    Id = 655375,
//                    TagType = TagType.TAG_VS_LIGHTING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Enable Lighting",
//                    InternalName = "TAG_VS_LIGHTING",
//                }
//            },
//            {
//                655373, new TagReference
//                {
//                    Id = 655373,
//                    TagType = TagType.TAG_VS_NUM_POINT_LINEAR_LIGHTS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "# of Point Lights (Linear Falloff)",
//                    InternalName = "TAG_VS_NUM_POINT_LINEAR_LIGHTS",
//                }
//            },
//            {
//                655402, new TagReference
//                {
//                    Id = 655402,
//                    TagType = TagType.TAG_VS_USES_SHADOWS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Uses Shadows",
//                    InternalName = "TAG_VS_USES_SHADOWS",
//                }
//            },
//            {
//                655400, new TagReference
//                {
//                    Id = 655400,
//                    TagType = TagType.TAG_VS_FRESNEL_POWER,
//                    DataType = MapDataType.HighPrecision,
//                    DisplayName = "Fresnel Power",
//                    InternalName = "TAG_VS_FRESNEL_POWER",
//                }
//            },
//            {
//                655367, new TagReference
//                {
//                    Id = 655367,
//                    TagType = TagType.TAG_VS_ENABLE_FOGGING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Enable Fogging",
//                    InternalName = "TAG_VS_ENABLE_FOGGING",
//                }
//            },
//            {
//                655366, new TagReference
//                {
//                    Id = 655366,
//                    TagType = TagType.TAG_VS_LIGHTMAP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Lightmap",
//                    InternalName = "TAG_VS_LIGHTMAP",
//                }
//            },
//            {
//                655365, new TagReference
//                {
//                    Id = 655365,
//                    TagType = TagType.TAG_VS_EDGEALPHA,
//                    DataType = MapDataType.Unk55,
//                    DisplayName = "Vertex Alpha Function",
//                    InternalName = "TAG_VS_EDGEALPHA",
//                }
//            },
//            {
//                655364, new TagReference
//                {
//                    Id = 655364,
//                    TagType = TagType.TAG_VS_TIMER_PERIOD,
//                    DataType = MapDataType.HighPrecision,
//                    DisplayName = "Timer period",
//                    InternalName = "TAG_VS_TIMER_PERIOD",
//                }
//            },
//            {
//                655424, new TagReference
//                {
//                    Id = 655424,
//                    TagType = TagType.TAG_VS_OUTPUT_FORMAT,
//                    DataType = MapDataType.OutputFormat,
//                    DisplayName = "Output Format",
//                    InternalName = "TAG_VS_OUTPUT_FORMAT",
//                }
//            },
//            {
//                655391, new TagReference
//                {
//                    Id = 655391,
//                    TagType = TagType.TAG_VS_PS3A_FUNC,
//                    DataType = MapDataType.Func,
//                    DisplayName = "Stage 4 Alpha Function",
//                    InternalName = "TAG_VS_PS3A_FUNC",
//                }
//            },
//            {
//                655389, new TagReference
//                {
//                    Id = 655389,
//                    TagType = TagType.TAG_VS_PS1A_FUNC,
//                    DataType = MapDataType.Func,
//                    DisplayName = "Stage 2 Alpha Function",
//                    InternalName = "TAG_VS_PS1A_FUNC",
//                }
//            },
//            {
//                655418, new TagReference
//                {
//                    Id = 655418,
//                    TagType = TagType.TAG_VS_ALPHA_TO_COVERAGE,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Alpha To Coverage",
//                    InternalName = "TAG_VS_ALPHA_TO_COVERAGE",
//                }
//            },
//            {
//                655416, new TagReference
//                {
//                    Id = 655416,
//                    TagType = TagType.TAG_VS_TRANSPARENT_ALPHA_TO_ONE,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Transparent Alpha To One",
//                    InternalName = "TAG_VS_TRANSPARENT_ALPHA_TO_ONE",
//                }
//            },
//            {
//                655383, new TagReference
//                {
//                    Id = 655383,
//                    TagType = TagType.TAG_VS_PS1C_FUNC,
//                    DataType = MapDataType.Func,
//                    DisplayName = "Stage 2 Color Function",
//                    InternalName = "TAG_VS_PS1C_FUNC",
//                }
//            },
//            {
//                655382, new TagReference
//                {
//                    Id = 655382,
//                    TagType = TagType.TAG_VS_PS0C_FUNC,
//                    DataType = MapDataType.Func,
//                    DisplayName = "Stage 1 Color Function",
//                    InternalName = "TAG_VS_PS0C_FUNC",
//                }
//            },
//            {
//                655381, new TagReference
//                {
//                    Id = 655381,
//                    TagType = TagType.TAG_VS_TEXCOORD5_FUNC,
//                    DataType = MapDataType.LocationFunc,
//                    DisplayName = "TexCoord6 Function",
//                    InternalName = "TAG_VS_TEXCOORD5_FUNC",
//                }
//            },
//            {
//                655380, new TagReference
//                {
//                    Id = 655380,
//                    TagType = TagType.TAG_VS_TEXCOORD4_FUNC,
//                    DataType = MapDataType.LocationFunc,
//                    DisplayName = "TexCoord5 Function",
//                    InternalName = "TAG_VS_TEXCOORD4_FUNC",
//                }
//            },
//            {
//                655407, new TagReference
//                {
//                    Id = 655407,
//                    TagType = TagType.TAG_VS_HERO_TINT,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Enable Hero Tint",
//                    InternalName = "TAG_VS_HERO_TINT",
//                }
//            },
//            {
//                655374, new TagReference
//                {
//                    Id = 655374,
//                    TagType = TagType.TAG_VS_MATERIAL_FUNC,
//                    DataType = MapDataType.MaterialFunc,
//                    DisplayName = "Material Function",
//                    InternalName = "TAG_VS_MATERIAL_FUNC",
//                }
//            },
//            {
//                655405, new TagReference
//                {
//                    Id = 655405,
//                    TagType = TagType.TAG_VS_CLOTH_SINGLE_SIDED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Cloth is single sided",
//                    InternalName = "TAG_VS_CLOTH_SINGLE_SIDED",
//                }
//            },
//            {
//                655372, new TagReference
//                {
//                    Id = 655372,
//                    TagType = TagType.TAG_VS_NUM_CYLINDRICAL_LIGHTS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "# of Cylindrical Lights",
//                    InternalName = "TAG_VS_NUM_CYLINDRICAL_LIGHTS",
//                }
//            },
//            {
//                655399, new TagReference
//                {
//                    Id = 655399,
//                    TagType = TagType.TAG_VS_FRESNEL_BIAS,
//                    DataType = MapDataType.HighPrecision,
//                    DisplayName = "Fresnel Bias",
//                    InternalName = "TAG_VS_FRESNEL_BIAS",
//                }
//            },
//            {
//                655398, new TagReference
//                {
//                    Id = 655398,
//                    TagType = TagType.TAG_VS_GLOW,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Enable Glow",
//                    InternalName = "TAG_VS_GLOW",
//                }
//            },
//            {
//                655397, new TagReference
//                {
//                    Id = 655397,
//                    TagType = TagType.TAG_VS_SHADOW_SELF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Shadow Self",
//                    InternalName = "TAG_VS_SHADOW_SELF",
//                }
//            },
//            {
//                655396, new TagReference
//                {
//                    Id = 655396,
//                    TagType = TagType.TAG_VS_MASK,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Enable Mask",
//                    InternalName = "TAG_VS_MASK",
//                }
//            },
//            {
//                721269, new TagReference
//                {
//                    Id = 721269,
//                    TagType = TagType.TAG_POWER_RUNEE_DAMAGE_TYPE,
//                    DataType = MapDataType.DamageType,
//                    DisplayName = "RuneE Damage Type",
//                    InternalName = "TAG_POWER_RUNEE_DAMAGE_TYPE",
//                }
//            },
//            {
//                264304, new TagReference
//                {
//                    Id = 264304,
//                    TagType = TagType.TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_0,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 0 Contact Frame Effect Group - Male",
//                    InternalName = "TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_0",
//                }
//            },
//            {
//                327684, new TagReference
//                {
//                    Id = 327684,
//                    TagType = TagType.TAG_POWER_TEMPLATE_RUNE_D,
//                    DataType = MapDataType.Template,
//                    DisplayName = "Template Rune D",
//                    InternalName = "TAG_POWER_TEMPLATE_RUNE_D",
//                }
//            },
//            {
//                270597, new TagReference
//                {
//                    Id = 270597,
//                    TagType = TagType.TAG_POWER_BUFF_5_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 5 Icon",
//                    InternalName = "TAG_POWER_BUFF_5_ICON",
//                }
//            },
//            {
//                270853, new TagReference
//                {
//                    Id = 270853,
//                    TagType = TagType.TAG_POWER_BUFF_5_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 5 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_5_HARMFUL_BUFF",
//                }
//            },
//            {
//                274688, new TagReference
//                {
//                    Id = 274688,
//                    TagType = TagType.TAG_POWER_BUFF_0_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 0 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_0_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                274432, new TagReference
//                {
//                    Id = 274432,
//                    TagType = TagType.TAG_POWER_BUFF_0_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 0 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_0_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                262914, new TagReference
//                {
//                    Id = 262914,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_3,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 3",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_3",
//                }
//            },
//            {
//                270343, new TagReference
//                {
//                    Id = 270343,
//                    TagType = TagType.TAG_POWER_BUFF_7_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 7 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_7_EFFECT_GROUP",
//                }
//            },
//            {
//                262658, new TagReference
//                {
//                    Id = 262658,
//                    TagType = TagType.TAG_POWER_ANIMATION_TAG_2,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Animation Tag 2",
//                    InternalName = "TAG_POWER_ANIMATION_TAG_2",
//                }
//            },
//            {
//                270599, new TagReference
//                {
//                    Id = 270599,
//                    TagType = TagType.TAG_POWER_BUFF_7_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 7 Icon",
//                    InternalName = "TAG_POWER_BUFF_7_ICON",
//                }
//            },
//            {
//                266752, new TagReference
//                {
//                    Id = 266752,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_10,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 10",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_10",
//                }
//            },
//            {
//                271892, new TagReference
//                {
//                    Id = 271892,
//                    TagType = TagType.TAG_POWER_BUFF_14_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 14 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_14_MERGES_TOOLTIP",
//                }
//            },
//            {
//                271895, new TagReference
//                {
//                    Id = 271895,
//                    TagType = TagType.TAG_POWER_BUFF_17_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 17 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_17_MERGES_TOOLTIP",
//                }
//            },
//            {
//                328720, new TagReference
//                {
//                    Id = 328720,
//                    TagType = TagType.TAG_POWER_LOS_CHECK,
//                    DataType = MapDataType.Unk92,
//                    DisplayName = "LOS Check",
//                    InternalName = "TAG_POWER_LOS_CHECK",
//                }
//            },
//            {
//                267920, new TagReference
//                {
//                    Id = 267920,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_59,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 59",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_59",
//                }
//            },
//            {
//                274441, new TagReference
//                {
//                    Id = 274441,
//                    TagType = TagType.TAG_POWER_BUFF_9_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 9 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_9_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                267664, new TagReference
//                {
//                    Id = 267664,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_49,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 49",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_49",
//                }
//            },
//            {
//                717568, new TagReference
//                {
//                    Id = 717568,
//                    TagType = TagType.TAG_POWER_APPLY_PASSIVE_AFTER_ITEM_PASSIVES,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Apply Passive After Item Passives",
//                    InternalName = "TAG_POWER_APPLY_PASSIVE_AFTER_ITEM_PASSIVES",
//                }
//            },
//            {
//                274462, new TagReference
//                {
//                    Id = 274462,
//                    TagType = TagType.TAG_POWER_BUFF_30_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 30 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_30_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                271637, new TagReference
//                {
//                    Id = 271637,
//                    TagType = TagType.TAG_POWER_BUFF_15_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 15 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_15_IS_DISPLAYED",
//                }
//            },
//            {
//                329828, new TagReference
//                {
//                    Id = 329828,
//                    TagType = TagType.TAG_POWER_COMBO_LEVEL_1_ON_HIT_COEFFICIENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Combo Level 1 On Hit Proc Coefficient",
//                    InternalName = "TAG_POWER_COMBO_LEVEL_1_ON_HIT_COEFFICIENT",
//                }
//            },
//            {
//                262675, new TagReference
//                {
//                    Id = 262675,
//                    TagType = TagType.TAG_POWER_ANIMATION_TAG_RUNE_C,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Animation Tag Rune C",
//                    InternalName = "TAG_POWER_ANIMATION_TAG_RUNE_C",
//                }
//            },
//            {
//                270358, new TagReference
//                {
//                    Id = 270358,
//                    TagType = TagType.TAG_POWER_BUFF_16_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 16 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_16_EFFECT_GROUP",
//                }
//            },
//            {
//                328656, new TagReference
//                {
//                    Id = 328656,
//                    TagType = TagType.TAG_POWER_USES_MAINHAND_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Uses Mainhand Only",
//                    InternalName = "TAG_POWER_USES_MAINHAND_ONLY",
//                }
//            },
//            {
//                271126, new TagReference
//                {
//                    Id = 271126,
//                    TagType = TagType.TAG_POWER_BUFF_16_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 16 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_16_SHOW_DURATION",
//                }
//            },
//            {
//                272658, new TagReference
//                {
//                    Id = 272658,
//                    TagType = TagType.TAG_POWER_BUFF_18_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 18 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_18_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                328288, new TagReference
//                {
//                    Id = 328288,
//                    TagType = TagType.TAG_POWER_START_WALK_AFTER_INTRO,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Start Walk After Intro",
//                    InternalName = "TAG_POWER_START_WALK_AFTER_INTRO",
//                }
//            },
//            {
//                274712, new TagReference
//                {
//                    Id = 274712,
//                    TagType = TagType.TAG_POWER_BUFF_24_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 24 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_24_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                274456, new TagReference
//                {
//                    Id = 274456,
//                    TagType = TagType.TAG_POWER_BUFF_24_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 24 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_24_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                328542, new TagReference
//                {
//                    Id = 328542,
//                    TagType = TagType.TAG_POWER_USE_CHARGE_STEERING,
//                    DataType = MapDataType.SteeringType,
//                    DisplayName = "Use Charge Steering",
//                    InternalName = "TAG_POWER_USE_CHARGE_STEERING",
//                }
//            },
//            {
//                262945, new TagReference
//                {
//                    Id = 262945,
//                    TagType = TagType.TAG_POWER_COMBO_SPELL_FUNC_END_2,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "Combo SpellFunc End 2",
//                    InternalName = "TAG_POWER_COMBO_SPELL_FUNC_END_2",
//                }
//            },
//            {
//                272411, new TagReference
//                {
//                    Id = 272411,
//                    TagType = TagType.TAG_POWER_BUFF_27_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 27 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_27_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                272169, new TagReference
//                {
//                    Id = 272169,
//                    TagType = TagType.TAG_POWER_BUFF_29_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 29 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_29_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                267808, new TagReference
//                {
//                    Id = 267808,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_52,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 52",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_52",
//                }
//            },
//            {
//                331952, new TagReference
//                {
//                    Id = 331952,
//                    TagType = TagType.TAG_POWER_WALKING_SPEED_MULTIPLIER,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Walking Speed Multiplier",
//                    InternalName = "TAG_POWER_WALKING_SPEED_MULTIPLIER",
//                }
//            },
//            {
//                270377, new TagReference
//                {
//                    Id = 270377,
//                    TagType = TagType.TAG_POWER_BUFF_29_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 29 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_29_EFFECT_GROUP",
//                }
//            },
//            {
//                721360, new TagReference
//                {
//                    Id = 721360,
//                    TagType = TagType.TAG_POWER_SCROLL_BUFF_EXCLUSIVE_TYPE,
//                    DataType = MapDataType.BuffType,
//                    DisplayName = "Scroll Buff Exclusive Type",
//                    InternalName = "TAG_POWER_SCROLL_BUFF_EXCLUSIVE_TYPE",
//                }
//            },
//            {
//                267552, new TagReference
//                {
//                    Id = 267552,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_42,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 42",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_42",
//                }
//            },
//            {
//                270625, new TagReference
//                {
//                    Id = 270625,
//                    TagType = TagType.TAG_POWER_BUFF_21_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 21 Icon",
//                    InternalName = "TAG_POWER_BUFF_21_ICON",
//                }
//            },
//            {
//                272160, new TagReference
//                {
//                    Id = 272160,
//                    TagType = TagType.TAG_POWER_BUFF_20_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 20 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_20_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                328546, new TagReference
//                {
//                    Id = 328546,
//                    TagType = TagType.TAG_POWER_IS_CANCELLABLE_BY_WALKING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Is Cancellable By Walking",
//                    InternalName = "TAG_POWER_IS_CANCELLABLE_BY_WALKING",
//                }
//            },
//            {
//                271394, new TagReference
//                {
//                    Id = 271394,
//                    TagType = TagType.TAG_POWER_BUFF_22_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 22 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_22_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                272162, new TagReference
//                {
//                    Id = 272162,
//                    TagType = TagType.TAG_POWER_BUFF_22_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 22 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_22_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                263221, new TagReference
//                {
//                    Id = 263221,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_3_RUNE_E,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 3 Rune E",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_3_RUNE_E",
//                }
//            },
//            {
//                266864, new TagReference
//                {
//                    Id = 266864,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_17,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 17",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_17",
//                }
//            },
//            {
//                713488, new TagReference
//                {
//                    Id = 713488,
//                    TagType = TagType.TAG_POWER_CONSOLE_MIN_CLIENT_WALK_SPEED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Min Client Walk Speed",
//                    InternalName = "TAG_POWER_CONSOLE_MIN_CLIENT_WALK_SPEED",
//                }
//            },
//            {
//                328129, new TagReference
//                {
//                    Id = 328129,
//                    TagType = TagType.TAG_POWER_CAST_TARGET_CORPSE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CastTargetCorpse",
//                    InternalName = "TAG_POWER_CAST_TARGET_CORPSE",
//                }
//            },
//            {
//                271877, new TagReference
//                {
//                    Id = 271877,
//                    TagType = TagType.TAG_POWER_BUFF_5_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 5 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_5_MERGES_TOOLTIP",
//                }
//            },
//            {
//                327808, new TagReference
//                {
//                    Id = 327808,
//                    TagType = TagType.TAG_POWER_IS_BASIC_ATTACK,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsBasicAttack",
//                    InternalName = "TAG_POWER_IS_BASIC_ATTACK",
//                }
//            },
//            {
//                272133, new TagReference
//                {
//                    Id = 272133,
//                    TagType = TagType.TAG_POWER_BUFF_5_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 5 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_5_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                721201, new TagReference
//                {
//                    Id = 721201,
//                    TagType = TagType.TAG_POWER_RUNEB_COMBO2_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneB Combo2 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEB_COMBO2_PROC_SCALAR",
//                }
//            },
//            {
//                329216, new TagReference
//                {
//                    Id = 329216,
//                    TagType = TagType.TAG_POWER_IS_UPGRADE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Is Upgrade",
//                    InternalName = "TAG_POWER_IS_UPGRADE",
//                }
//            },
//            {
//                329983, new TagReference
//                {
//                    Id = 329983,
//                    TagType = TagType.TAG_POWER_ON_HIT_PROC_COEFFICIENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "On Hit Proc Coefficient",
//                    InternalName = "TAG_POWER_ON_HIT_PROC_COEFFICIENT",
//                }
//            },
//            {
//                274692, new TagReference
//                {
//                    Id = 274692,
//                    TagType = TagType.TAG_POWER_BUFF_4_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 4 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_4_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                264320, new TagReference
//                {
//                    Id = 264320,
//                    TagType = TagType.TAG_POWER_COMBO_CASTING_EFFECT_GROUP_FEMALE_0,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 0 Casting Effect Group - Female",
//                    InternalName = "TAG_POWER_COMBO_CASTING_EFFECT_GROUP_FEMALE_0",
//                }
//            },
//            {
//                328325, new TagReference
//                {
//                    Id = 328325,
//                    TagType = TagType.TAG_POWER_FAILS_IF_FEARED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "FailsIfFeared",
//                    InternalName = "TAG_POWER_FAILS_IF_FEARED",
//                }
//            },
//            {
//                274445, new TagReference
//                {
//                    Id = 274445,
//                    TagType = TagType.TAG_POWER_BUFF_13_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 13 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_13_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                267328, new TagReference
//                {
//                    Id = 267328,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_34,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 34",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_34",
//                }
//            },
//            {
//                267072, new TagReference
//                {
//                    Id = 267072,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_24,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 24",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_24",
//                }
//            },
//            {
//                332880, new TagReference
//                {
//                    Id = 332880,
//                    TagType = TagType.TAG_POWER_AI_ACTION_DURATION_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "AI Action Duration Delta",
//                    InternalName = "TAG_POWER_AI_ACTION_DURATION_DELTA",
//                }
//            },
//            {
//                272651, new TagReference
//                {
//                    Id = 272651,
//                    TagType = TagType.TAG_POWER_BUFF_11_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 11 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_11_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                272660, new TagReference
//                {
//                    Id = 272660,
//                    TagType = TagType.TAG_POWER_BUFF_20_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 20 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_20_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                565248, new TagReference
//                {
//                    Id = 565248,
//                    TagType = TagType.TAG_POWER_RUN_IN_FRONT_DISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Run In Front Distance",
//                    InternalName = "TAG_POWER_RUN_IN_FRONT_DISTANCE",
//                }
//            },
//            {
//                266640, new TagReference
//                {
//                    Id = 266640,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_9,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 9",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_9",
//                }
//            },
//            {
//                721234, new TagReference
//                {
//                    Id = 721234,
//                    TagType = TagType.TAG_POWER_RUNED_COMBO3_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneD Combo3 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNED_COMBO3_PROC_SCALAR",
//                }
//            },
//            {
//                272652, new TagReference
//                {
//                    Id = 272652,
//                    TagType = TagType.TAG_POWER_BUFF_12_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 12 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_12_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                271368, new TagReference
//                {
//                    Id = 271368,
//                    TagType = TagType.TAG_POWER_BUFF_8_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 8 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_8_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                272661, new TagReference
//                {
//                    Id = 272661,
//                    TagType = TagType.TAG_POWER_BUFF_21_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 21 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_21_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                267792, new TagReference
//                {
//                    Id = 267792,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_51,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 51",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_51",
//                }
//            },
//            {
//                271112, new TagReference
//                {
//                    Id = 271112,
//                    TagType = TagType.TAG_POWER_BUFF_8_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 8 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_8_SHOW_DURATION",
//                }
//            },
//            {
//                267536, new TagReference
//                {
//                    Id = 267536,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_41,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 41",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_41",
//                }
//            },
//            {
//                274460, new TagReference
//                {
//                    Id = 274460,
//                    TagType = TagType.TAG_POWER_BUFF_28_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 28 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_28_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                272150, new TagReference
//                {
//                    Id = 272150,
//                    TagType = TagType.TAG_POWER_BUFF_16_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 16 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_16_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                263186, new TagReference
//                {
//                    Id = 263186,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_1_RUNE_B,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 1 Rune B",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_1_RUNE_B",
//                }
//            },
//            {
//                271641, new TagReference
//                {
//                    Id = 271641,
//                    TagType = TagType.TAG_POWER_BUFF_19_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 19 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_19_IS_DISPLAYED",
//                }
//            },
//            {
//                271894, new TagReference
//                {
//                    Id = 271894,
//                    TagType = TagType.TAG_POWER_BUFF_16_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 16 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_16_MERGES_TOOLTIP",
//                }
//            },
//            {
//                271897, new TagReference
//                {
//                    Id = 271897,
//                    TagType = TagType.TAG_POWER_BUFF_19_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 19 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_19_MERGES_TOOLTIP",
//                }
//            },
//            {
//                270611, new TagReference
//                {
//                    Id = 270611,
//                    TagType = TagType.TAG_POWER_BUFF_13_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 13 Icon",
//                    InternalName = "TAG_POWER_BUFF_13_ICON",
//                }
//            },
//            {
//                271891, new TagReference
//                {
//                    Id = 271891,
//                    TagType = TagType.TAG_POWER_BUFF_13_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 13 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_13_MERGES_TOOLTIP",
//                }
//            },
//            {
//                328528, new TagReference
//                {
//                    Id = 328528,
//                    TagType = TagType.TAG_POWER_CAN_USE_WHEN_DEAD,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Can Use When Dead",
//                    InternalName = "TAG_POWER_CAN_USE_WHEN_DEAD",
//                }
//            },
//            {
//                270630, new TagReference
//                {
//                    Id = 270630,
//                    TagType = TagType.TAG_POWER_BUFF_26_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 26 Icon",
//                    InternalName = "TAG_POWER_BUFF_26_ICON",
//                }
//            },
//            {
//                328296, new TagReference
//                {
//                    Id = 328296,
//                    TagType = TagType.TAG_POWER_ARC_MOVE_UNTIL_DEST_HEIGHT,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Arc Move Until Dest Height",
//                    InternalName = "TAG_POWER_ARC_MOVE_UNTIL_DEST_HEIGHT",
//                }
//            },
//            {
//                266528, new TagReference
//                {
//                    Id = 266528,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_2,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 2",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_2",
//                }
//            },
//            {
//                328432, new TagReference
//                {
//                    Id = 328432,
//                    TagType = TagType.TAG_POWER_REQUIRES_TARGET,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RequiresTarget",
//                    InternalName = "TAG_POWER_REQUIRES_TARGET",
//                }
//            },
//            {
//                270888, new TagReference
//                {
//                    Id = 270888,
//                    TagType = TagType.TAG_POWER_BUFF_28_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 28 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_28_HARMFUL_BUFF",
//                }
//            },
//            {
//                271138, new TagReference
//                {
//                    Id = 271138,
//                    TagType = TagType.TAG_POWER_BUFF_22_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 22 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_22_SHOW_DURATION",
//                }
//            },
//            {
//                721232, new TagReference
//                {
//                    Id = 721232,
//                    TagType = TagType.TAG_POWER_RUNED_COMBO1_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneD Combo1 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNED_COMBO1_PROC_SCALAR",
//                }
//            },
//            {
//                270882, new TagReference
//                {
//                    Id = 270882,
//                    TagType = TagType.TAG_POWER_BUFF_22_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 22 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_22_HARMFUL_BUFF",
//                }
//            },
//            {
//                272177, new TagReference
//                {
//                    Id = 272177,
//                    TagType = TagType.TAG_POWER_BUFF_31_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 31 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_31_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                684880, new TagReference
//                {
//                    Id = 684880,
//                    TagType = TagType.TAG_POWER_NO_INTERRUPT_TIMER,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "No Interrupt Timer",
//                    InternalName = "TAG_POWER_NO_INTERRUPT_TIMER",
//                }
//            },
//            {
//                331960, new TagReference
//                {
//                    Id = 331960,
//                    TagType = TagType.TAG_POWER_WALKING_DISTANCE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Walking Distance Min",
//                    InternalName = "TAG_POWER_WALKING_DISTANCE_MIN",
//                }
//            },
//            {
//                270385, new TagReference
//                {
//                    Id = 270385,
//                    TagType = TagType.TAG_POWER_BUFF_31_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 31 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_31_EFFECT_GROUP",
//                }
//            },
//            {
//                264305, new TagReference
//                {
//                    Id = 264305,
//                    TagType = TagType.TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_1,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 1 Contact Frame Effect Group - Male",
//                    InternalName = "TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_1",
//                }
//            },
//            {
//                682242, new TagReference
//                {
//                    Id = 682242,
//                    TagType = TagType.TAG_POWER_IMMUNE_TO_STUN_DURING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Immune to Stun during",
//                    InternalName = "TAG_POWER_IMMUNE_TO_STUN_DURING",
//                }
//            },
//            {
//                328386, new TagReference
//                {
//                    Id = 328386,
//                    TagType = TagType.TAG_POWER_TURNS_INTO_BASIC_ATTACK_MELEE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TurnsIntoBasicMeleeAttack",
//                    InternalName = "TAG_POWER_TURNS_INTO_BASIC_ATTACK_MELEE",
//                }
//            },
//            {
//                327984, new TagReference
//                {
//                    Id = 327984,
//                    TagType = TagType.TAG_POWER_CONTACT_FREEZES_FACING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "ContactFreezesFacing",
//                    InternalName = "TAG_POWER_CONTACT_FREEZES_FACING",
//                }
//            },
//            {
//                327680, new TagReference
//                {
//                    Id = 327680,
//                    TagType = TagType.TAG_POWER_TEMPLATE,
//                    DataType = MapDataType.Template,
//                    DisplayName = "Template",
//                    InternalName = "TAG_POWER_TEMPLATE",
//                }
//            },
//            {
//                270593, new TagReference
//                {
//                    Id = 270593,
//                    TagType = TagType.TAG_POWER_BUFF_1_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 1 Icon",
//                    InternalName = "TAG_POWER_BUFF_1_ICON",
//                }
//            },
//            {
//                272128, new TagReference
//                {
//                    Id = 272128,
//                    TagType = TagType.TAG_POWER_BUFF_0_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 0 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_0_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                263296, new TagReference
//                {
//                    Id = 263296,
//                    TagType = TagType.TAG_POWER_LOOPING_ANIMATION_TIME,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Looping Animation Time",
//                    InternalName = "TAG_POWER_LOOPING_ANIMATION_TIME",
//                }
//            },
//            {
//                271362, new TagReference
//                {
//                    Id = 271362,
//                    TagType = TagType.TAG_POWER_BUFF_2_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 2 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_2_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                272130, new TagReference
//                {
//                    Id = 272130,
//                    TagType = TagType.TAG_POWER_BUFF_2_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 2 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_2_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                264448, new TagReference
//                {
//                    Id = 264448,
//                    TagType = TagType.TAG_POWER_IS_COMBO_POWER,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsComboPower",
//                    InternalName = "TAG_POWER_IS_COMBO_POWER",
//                }
//            },
//            {
//                328021, new TagReference
//                {
//                    Id = 328021,
//                    TagType = TagType.TAG_POWER_SNAPS_TO_FACING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "SnapsToFacing",
//                    InternalName = "TAG_POWER_SNAPS_TO_FACING",
//                }
//            },
//            {
//                721157, new TagReference
//                {
//                    Id = 721157,
//                    TagType = TagType.TAG_POWER_RUNEE_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneE Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEE_PROC_SCALAR",
//                }
//            },
//            {
//                274443, new TagReference
//                {
//                    Id = 274443,
//                    TagType = TagType.TAG_POWER_BUFF_11_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 11 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_11_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                271366, new TagReference
//                {
//                    Id = 271366,
//                    TagType = TagType.TAG_POWER_BUFF_6_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 6 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_6_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                721217, new TagReference
//                {
//                    Id = 721217,
//                    TagType = TagType.TAG_POWER_RUNEC_COMBO2_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneC Combo2 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEC_COMBO2_PROC_SCALAR",
//                }
//            },
//            {
//                271107, new TagReference
//                {
//                    Id = 271107,
//                    TagType = TagType.TAG_POWER_BUFF_3_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 3 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_3_SHOW_DURATION",
//                }
//            },
//            {
//                270339, new TagReference
//                {
//                    Id = 270339,
//                    TagType = TagType.TAG_POWER_BUFF_3_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 3 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_3_EFFECT_GROUP",
//                }
//            },
//            {
//                272394, new TagReference
//                {
//                    Id = 272394,
//                    TagType = TagType.TAG_POWER_BUFF_10_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 10 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_10_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                272649, new TagReference
//                {
//                    Id = 272649,
//                    TagType = TagType.TAG_POWER_BUFF_9_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 9 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_9_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                266512, new TagReference
//                {
//                    Id = 266512,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_1,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 1",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_1",
//                }
//            },
//            {
//                262930, new TagReference
//                {
//                    Id = 262930,
//                    TagType = TagType.TAG_POWER_COMBO_TEMPLATE_3,
//                    DataType = MapDataType.Template,
//                    DisplayName = "Combo Template 3",
//                    InternalName = "TAG_POWER_COMBO_TEMPLATE_3",
//                }
//            },
//            {
//                270873, new TagReference
//                {
//                    Id = 270873,
//                    TagType = TagType.TAG_POWER_BUFF_19_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 19 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_19_HARMFUL_BUFF",
//                }
//            },
//            {
//                262674, new TagReference
//                {
//                    Id = 262674,
//                    TagType = TagType.TAG_POWER_ANIMATION_TAG_RUNE_B,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Animation Tag Rune B",
//                    InternalName = "TAG_POWER_ANIMATION_TAG_RUNE_B",
//                }
//            },
//            {
//                271129, new TagReference
//                {
//                    Id = 271129,
//                    TagType = TagType.TAG_POWER_BUFF_19_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 19 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_19_SHOW_DURATION",
//                }
//            },
//            {
//                274458, new TagReference
//                {
//                    Id = 274458,
//                    TagType = TagType.TAG_POWER_BUFF_26_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 26 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_26_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                329809, new TagReference
//                {
//                    Id = 329809,
//                    TagType = TagType.TAG_POWER_COMBO_ATTACK_RADIUS_1,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Combo Attack Radius 1",
//                    InternalName = "TAG_POWER_COMBO_ATTACK_RADIUS_1",
//                }
//            },
//            {
//                328082, new TagReference
//                {
//                    Id = 328082,
//                    TagType = TagType.TAG_POWER_ONLY_USABLE_IN_TOWN_PORTAL_AREAS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsOnlyUsableInTownPortalAreas",
//                    InternalName = "TAG_POWER_ONLY_USABLE_IN_TOWN_PORTAL_AREAS",
//                }
//            },
//            {
//                270354, new TagReference
//                {
//                    Id = 270354,
//                    TagType = TagType.TAG_POWER_BUFF_12_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 12 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_12_EFFECT_GROUP",
//                }
//            },
//            {
//                328736, new TagReference
//                {
//                    Id = 328736,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_FUNC,
//                    DataType = MapDataType.TargetFunc,
//                    DisplayName = "Custom Target Func",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_FUNC",
//                }
//            },
//            {
//                271911, new TagReference
//                {
//                    Id = 271911,
//                    TagType = TagType.TAG_POWER_BUFF_27_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 27 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_27_MERGES_TOOLTIP",
//                }
//            },
//            {
//                274706, new TagReference
//                {
//                    Id = 274706,
//                    TagType = TagType.TAG_POWER_BUFF_18_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 18 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_18_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                328536, new TagReference
//                {
//                    Id = 328536,
//                    TagType = TagType.TAG_POWER_SPECIALWALK_PLAYER_END_ANIM_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "SpecialWalk Player End Anim Scalar",
//                    InternalName = "TAG_POWER_SPECIALWALK_PLAYER_END_ANIM_SCALAR",
//                }
//            },
//            {
//                271653, new TagReference
//                {
//                    Id = 271653,
//                    TagType = TagType.TAG_POWER_BUFF_25_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 25 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_25_IS_DISPLAYED",
//                }
//            },
//            {
//                271140, new TagReference
//                {
//                    Id = 271140,
//                    TagType = TagType.TAG_POWER_BUFF_24_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 24 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_24_SHOW_DURATION",
//                }
//            },
//            {
//                681985, new TagReference
//                {
//                    Id = 681985,
//                    TagType = TagType.TAG_POWER_BREAKS_SNARE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Breaks Snare",
//                    InternalName = "TAG_POWER_BREAKS_SNARE",
//                }
//            },
//            {
//                328807, new TagReference
//                {
//                    Id = 328807,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_BUFF_SNO_BUFF_INDEX_2,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Custom Target Buff Power SNO Buff Index 2",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_BUFF_SNO_BUFF_INDEX_2",
//                }
//            },
//            {
//                328304, new TagReference
//                {
//                    Id = 328304,
//                    TagType = TagType.TAG_POWER_BREAKS_IMMOBILIZE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "BreaksImmobilize",
//                    InternalName = "TAG_POWER_BREAKS_IMMOBILIZE",
//                }
//            },
//            {
//                328440, new TagReference
//                {
//                    Id = 328440,
//                    TagType = TagType.TAG_POWER_PROC_TARGETS_SELF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "ProcTargetsSelf",
//                    InternalName = "TAG_POWER_PROC_TARGETS_SELF",
//                }
//            },
//            {
//                267312, new TagReference
//                {
//                    Id = 267312,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_33,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 33",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_33",
//                }
//            },
//            {
//                267056, new TagReference
//                {
//                    Id = 267056,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_23,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 23",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_23",
//                }
//            },
//            {
//                262961, new TagReference
//                {
//                    Id = 262961,
//                    TagType = TagType.TAG_POWER_COMBO_SPELL_FUNC_INTERRUPTED_2,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "Combo SpellFunc Interrupted 2",
//                    InternalName = "TAG_POWER_COMBO_SPELL_FUNC_INTERRUPTED_2",
//                }
//            },
//            {
//                274437, new TagReference
//                {
//                    Id = 274437,
//                    TagType = TagType.TAG_POWER_BUFF_5_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 5 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_5_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                721376, new TagReference
//                {
//                    Id = 721376,
//                    TagType = TagType.TAG_POWER_MONSTER_GENERIC_SUMMON_TAGS_INDEX,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Monster Generic Summon Tags Index",
//                    InternalName = "TAG_POWER_MONSTER_GENERIC_SUMMON_TAGS_INDEX",
//                }
//            },
//            {
//                272176, new TagReference
//                {
//                    Id = 272176,
//                    TagType = TagType.TAG_POWER_BUFF_30_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 30 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_30_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                271106, new TagReference
//                {
//                    Id = 271106,
//                    TagType = TagType.TAG_POWER_BUFF_2_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 2 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_2_SHOW_DURATION",
//                }
//            },
//            {
//                264321, new TagReference
//                {
//                    Id = 264321,
//                    TagType = TagType.TAG_POWER_COMBO_CASTING_EFFECT_GROUP_FEMALE_1,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 1 Casting Effect Group - Female",
//                    InternalName = "TAG_POWER_COMBO_CASTING_EFFECT_GROUP_FEMALE_1",
//                }
//            },
//            {
//                270850, new TagReference
//                {
//                    Id = 270850,
//                    TagType = TagType.TAG_POWER_BUFF_2_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 2 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_2_HARMFUL_BUFF",
//                }
//            },
//            {
//                713219, new TagReference
//                {
//                    Id = 713219,
//                    TagType = TagType.TAG_POWER_CONSOLE_PREFERS_RADIAL_TARGETING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Prefers Radial Targetting",
//                    InternalName = "TAG_POWER_CONSOLE_PREFERS_RADIAL_TARGETING",
//                }
//            },
//            {
//                272654, new TagReference
//                {
//                    Id = 272654,
//                    TagType = TagType.TAG_POWER_BUFF_14_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 14 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_14_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                262912, new TagReference
//                {
//                    Id = 262912,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_1,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 1",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_1",
//                }
//            },
//            {
//                328145, new TagReference
//                {
//                    Id = 328145,
//                    TagType = TagType.TAG_POWER_CAST_TARGET_NEUTRAL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CastTargetNeutral",
//                    InternalName = "TAG_POWER_CAST_TARGET_NEUTRAL",
//                }
//            },
//            {
//                271111, new TagReference
//                {
//                    Id = 271111,
//                    TagType = TagType.TAG_POWER_BUFF_7_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 7 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_7_SHOW_DURATION",
//                }
//            },
//            {
//                272646, new TagReference
//                {
//                    Id = 272646,
//                    TagType = TagType.TAG_POWER_BUFF_6_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 6 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_6_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                274452, new TagReference
//                {
//                    Id = 274452,
//                    TagType = TagType.TAG_POWER_BUFF_20_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 20 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_20_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                271363, new TagReference
//                {
//                    Id = 271363,
//                    TagType = TagType.TAG_POWER_BUFF_3_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 3 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_3_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                328000, new TagReference
//                {
//                    Id = 328000,
//                    TagType = TagType.TAG_POWER_NEVER_UPDATES_FACING_DURING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "NeverUpdatesFacing",
//                    InternalName = "TAG_POWER_NEVER_UPDATES_FACING_DURING",
//                }
//            },
//            {
//                327824, new TagReference
//                {
//                    Id = 327824,
//                    TagType = TagType.TAG_POWER_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsDisplayed",
//                    InternalName = "TAG_POWER_IS_DISPLAYED",
//                }
//            },
//            {
//                271619, new TagReference
//                {
//                    Id = 271619,
//                    TagType = TagType.TAG_POWER_BUFF_3_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 3 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_3_IS_DISPLAYED",
//                }
//            },
//            {
//                329232, new TagReference
//                {
//                    Id = 329232,
//                    TagType = TagType.TAG_POWER_UPGRADE_0,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Upgrade 0",
//                    InternalName = "TAG_POWER_UPGRADE_0",
//                }
//            },
//            {
//                328607, new TagReference
//                {
//                    Id = 328607,
//                    TagType = TagType.TAG_POWER_USES_WEAPON_RANGE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Uses Weapon Range",
//                    InternalName = "TAG_POWER_USES_WEAPON_RANGE",
//                }
//            },
//            {
//                271369, new TagReference
//                {
//                    Id = 271369,
//                    TagType = TagType.TAG_POWER_BUFF_9_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 9 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_9_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                328165, new TagReference
//                {
//                    Id = 328165,
//                    TagType = TagType.TAG_POWER_TARGET_PATHABLE_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetPathableOnly",
//                    InternalName = "TAG_POWER_TARGET_PATHABLE_ONLY",
//                }
//            },
//            {
//                264336, new TagReference
//                {
//                    Id = 264336,
//                    TagType = TagType.TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_FEMALE_0,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 0 Contact Frame Effect Group - Female",
//                    InternalName = "TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_FEMALE_0",
//                }
//            },
//            {
//                274710, new TagReference
//                {
//                    Id = 274710,
//                    TagType = TagType.TAG_POWER_BUFF_22_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 22 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_22_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                274709, new TagReference
//                {
//                    Id = 274709,
//                    TagType = TagType.TAG_POWER_BUFF_21_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 21 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_21_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                272409, new TagReference
//                {
//                    Id = 272409,
//                    TagType = TagType.TAG_POWER_BUFF_25_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 25 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_25_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                627456, new TagReference
//                {
//                    Id = 627456,
//                    TagType = TagType.TAG_POWER_DAMAGE_DISPLAY_POWER,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Damage Display Power",
//                    InternalName = "TAG_POWER_DAMAGE_DISPLAY_POWER",
//                }
//            },
//            {
//                329830, new TagReference
//                {
//                    Id = 329830,
//                    TagType = TagType.TAG_POWER_COMBO_LEVEL_3_ON_HIT_COEFFICIENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Combo Level 3 On Hit Proc Coefficient",
//                    InternalName = "TAG_POWER_COMBO_LEVEL_3_ON_HIT_COEFFICIENT",
//                }
//            },
//            {
//                328611, new TagReference
//                {
//                    Id = 328611,
//                    TagType = TagType.TAG_POWER_DOESNT_PREPLAY_ANIMATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Doesnt Preplay Animation",
//                    InternalName = "TAG_POWER_DOESNT_PREPLAY_ANIMATION",
//                }
//            },
//            {
//                328545, new TagReference
//                {
//                    Id = 328545,
//                    TagType = TagType.TAG_POWER_IS_CANCELLABLE_BY_ANY_POWER,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Is Cancellable By Any Power",
//                    InternalName = "TAG_POWER_IS_CANCELLABLE_BY_ANY_POWER",
//                }
//            },
//            {
//                272401, new TagReference
//                {
//                    Id = 272401,
//                    TagType = TagType.TAG_POWER_BUFF_17_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 17 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_17_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                721250, new TagReference
//                {
//                    Id = 721250,
//                    TagType = TagType.TAG_POWER_RUNEE_COMBO3_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneE Combo3 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEE_COMBO3_PROC_SCALAR",
//                }
//            },
//            {
//                561424, new TagReference
//                {
//                    Id = 561424,
//                    TagType = TagType.TAG_POWER_FOLLOW_WALK_ANIM_TAG,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Follow Walk Anim Tag",
//                    InternalName = "TAG_POWER_FOLLOW_WALK_ANIM_TAG",
//                }
//            },
//            {
//                271384, new TagReference
//                {
//                    Id = 271384,
//                    TagType = TagType.TAG_POWER_BUFF_18_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 18 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_18_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                329832, new TagReference
//                {
//                    Id = 329832,
//                    TagType = TagType.TAG_POWER_USES_MAINHAND_ONLY_COMBO2,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Combo Level 2 Uses Main Hand Only",
//                    InternalName = "TAG_POWER_USES_MAINHAND_ONLY_COMBO2",
//                }
//            },
//            {
//                271128, new TagReference
//                {
//                    Id = 271128,
//                    TagType = TagType.TAG_POWER_BUFF_18_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 18 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_18_SHOW_DURATION",
//                }
//            },
//            {
//                264290, new TagReference
//                {
//                    Id = 264290,
//                    TagType = TagType.TAG_POWER_COMBO_CASTING_EFFECT_GROUP_2,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 2 Casting Effect Group - Male",
//                    InternalName = "TAG_POWER_COMBO_CASTING_EFFECT_GROUP_2",
//                }
//            },
//            {
//                263202, new TagReference
//                {
//                    Id = 263202,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_2_RUNE_B,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 2 Rune B",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_2_RUNE_B",
//                }
//            },
//            {
//                271657, new TagReference
//                {
//                    Id = 271657,
//                    TagType = TagType.TAG_POWER_BUFF_29_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 29 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_29_IS_DISPLAYED",
//                }
//            },
//            {
//                713220, new TagReference
//                {
//                    Id = 713220,
//                    TagType = TagType.TAG_POWER_CONSOLE_FIRES_ON_BUTTON_UP_ONLY,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Fires On Button Up",
//                    InternalName = "TAG_POWER_CONSOLE_FIRES_ON_BUTTON_UP_ONLY",
//                }
//            },
//            {
//                271913, new TagReference
//                {
//                    Id = 271913,
//                    TagType = TagType.TAG_POWER_BUFF_29_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 29 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_29_MERGES_TOOLTIP",
//                }
//            },
//            {
//                328544, new TagReference
//                {
//                    Id = 328544,
//                    TagType = TagType.TAG_POWER_IS_CANCELLABLE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Is Cancellable",
//                    InternalName = "TAG_POWER_IS_CANCELLABLE",
//                }
//            },
//            {
//                717312, new TagReference
//                {
//                    Id = 717312,
//                    TagType = TagType.TAG_POWER_USES_POWER_FALLBACK_QUEUE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Uses Power Fallback Queue",
//                    InternalName = "TAG_POWER_USES_POWER_FALLBACK_QUEUE",
//                }
//            },
//            {
//                340141, new TagReference
//                {
//                    Id = 340141,
//                    TagType = TagType.TAG_POWER_IS_INVISIBLE_DURING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Is Invisible During",
//                    InternalName = "TAG_POWER_IS_INVISIBLE_DURING",
//                }
//            },
//            {
//                328320, new TagReference
//                {
//                    Id = 328320,
//                    TagType = TagType.TAG_POWER_FAILS_IF_IMMOBILIZED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "FailsIfImmobilized",
//                    InternalName = "TAG_POWER_FAILS_IF_IMMOBILIZED",
//                }
//            },
//            {
//                271621, new TagReference
//                {
//                    Id = 271621,
//                    TagType = TagType.TAG_POWER_BUFF_5_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 5 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_5_IS_DISPLAYED",
//                }
//            },
//            {
//                271108, new TagReference
//                {
//                    Id = 271108,
//                    TagType = TagType.TAG_POWER_BUFF_4_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 4 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_4_SHOW_DURATION",
//                }
//            },
//            {
//                721248, new TagReference
//                {
//                    Id = 721248,
//                    TagType = TagType.TAG_POWER_RUNEE_COMBO1_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneE Combo1 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEE_COMBO1_PROC_SCALAR",
//                }
//            },
//            {
//                272642, new TagReference
//                {
//                    Id = 272642,
//                    TagType = TagType.TAG_POWER_BUFF_2_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 2 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_2_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                267840, new TagReference
//                {
//                    Id = 267840,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_54,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 54",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_54",
//                }
//            },
//            {
//                682240, new TagReference
//                {
//                    Id = 682240,
//                    TagType = TagType.TAG_POWER_IMMUNE_TO_ROOT_DURING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Immune to Root during",
//                    InternalName = "TAG_POWER_IMMUNE_TO_ROOT_DURING",
//                }
//            },
//            {
//                267584, new TagReference
//                {
//                    Id = 267584,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_44,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 44",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_44",
//                }
//            },
//            {
//                272643, new TagReference
//                {
//                    Id = 272643,
//                    TagType = TagType.TAG_POWER_BUFF_3_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 3 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_3_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                272395, new TagReference
//                {
//                    Id = 272395,
//                    TagType = TagType.TAG_POWER_BUFF_11_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 11 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_11_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                329472, new TagReference
//                {
//                    Id = 329472,
//                    TagType = TagType.TAG_POWER_ICON_NORMAL,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Icon Normal",
//                    InternalName = "TAG_POWER_ICON_NORMAL",
//                }
//            },
//            {
//                271639, new TagReference
//                {
//                    Id = 271639,
//                    TagType = TagType.TAG_POWER_BUFF_17_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 17 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_17_IS_DISPLAYED",
//                }
//            },
//            {
//                270871, new TagReference
//                {
//                    Id = 270871,
//                    TagType = TagType.TAG_POWER_BUFF_17_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 17 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_17_HARMFUL_BUFF",
//                }
//            },
//            {
//                684850, new TagReference
//                {
//                    Id = 684850,
//                    TagType = TagType.TAG_POWER_CHECK_PATH_THEN_CLIP_TO_PATHABLE_ATTACK_RADIUS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Check Path Then Clip To Pathable Attack Radius",
//                    InternalName = "TAG_POWER_CHECK_PATH_THEN_CLIP_TO_PATHABLE_ATTACK_RADIUS",
//                }
//            },
//            {
//                327696, new TagReference
//                {
//                    Id = 327696,
//                    TagType = TagType.TAG_POWER_SPELL_FUNC_BEGIN,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "SpellFunc Begin",
//                    InternalName = "TAG_POWER_SPELL_FUNC_BEGIN",
//                }
//            },
//            {
//                266896, new TagReference
//                {
//                    Id = 266896,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_19,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 19",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_19",
//                }
//            },
//            {
//                272144, new TagReference
//                {
//                    Id = 272144,
//                    TagType = TagType.TAG_POWER_BUFF_10_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 10 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_10_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                271880, new TagReference
//                {
//                    Id = 271880,
//                    TagType = TagType.TAG_POWER_BUFF_8_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 8 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_8_MERGES_TOOLTIP",
//                }
//            },
//            {
//                271624, new TagReference
//                {
//                    Id = 271624,
//                    TagType = TagType.TAG_POWER_BUFF_8_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 8 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_8_IS_DISPLAYED",
//                }
//            },
//            {
//                328615, new TagReference
//                {
//                    Id = 328615,
//                    TagType = TagType.TAG_POWER_DOESNT_REQUIRE_ACTOR_TARGET,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetDoesntRequireActor",
//                    InternalName = "TAG_POWER_DOESNT_REQUIRE_ACTOR_TARGET",
//                }
//            },
//            {
//                274716, new TagReference
//                {
//                    Id = 274716,
//                    TagType = TagType.TAG_POWER_BUFF_28_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 28 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_28_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                271382, new TagReference
//                {
//                    Id = 271382,
//                    TagType = TagType.TAG_POWER_BUFF_16_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 16 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_16_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                721233, new TagReference
//                {
//                    Id = 721233,
//                    TagType = TagType.TAG_POWER_RUNED_COMBO2_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneD Combo2 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNED_COMBO2_PROC_SCALAR",
//                }
//            },
//            {
//                271377, new TagReference
//                {
//                    Id = 271377,
//                    TagType = TagType.TAG_POWER_BUFF_11_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 11 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_11_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                271123, new TagReference
//                {
//                    Id = 271123,
//                    TagType = TagType.TAG_POWER_BUFF_13_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 13 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_13_SHOW_DURATION",
//                }
//            },
//            {
//                270355, new TagReference
//                {
//                    Id = 270355,
//                    TagType = TagType.TAG_POWER_BUFF_13_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 13 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_13_EFFECT_GROUP",
//                }
//            },
//            {
//                272410, new TagReference
//                {
//                    Id = 272410,
//                    TagType = TagType.TAG_POWER_BUFF_26_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 26 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_26_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                262946, new TagReference
//                {
//                    Id = 262946,
//                    TagType = TagType.TAG_POWER_COMBO_SPELL_FUNC_END_3,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "Combo SpellFunc End 3",
//                    InternalName = "TAG_POWER_COMBO_SPELL_FUNC_END_3",
//                }
//            },
//            {
//                340142, new TagReference
//                {
//                    Id = 340142,
//                    TagType = TagType.TAG_POWER_CANNOT_DIE_DURING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Cannot Die During",
//                    InternalName = "TAG_POWER_CANNOT_DIE_DURING",
//                }
//            },
//            {
//                270889, new TagReference
//                {
//                    Id = 270889,
//                    TagType = TagType.TAG_POWER_BUFF_29_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 29 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_29_HARMFUL_BUFF",
//                }
//            },
//            {
//                271145, new TagReference
//                {
//                    Id = 271145,
//                    TagType = TagType.TAG_POWER_BUFF_29_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 29 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_29_SHOW_DURATION",
//                }
//            },
//            {
//                266784, new TagReference
//                {
//                    Id = 266784,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_12,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 12",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_12",
//                }
//            },
//            {
//                271392, new TagReference
//                {
//                    Id = 271392,
//                    TagType = TagType.TAG_POWER_BUFF_20_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 20 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_20_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                271136, new TagReference
//                {
//                    Id = 271136,
//                    TagType = TagType.TAG_POWER_BUFF_20_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 20 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_20_SHOW_DURATION",
//                }
//            },
//            {
//                329825, new TagReference
//                {
//                    Id = 329825,
//                    TagType = TagType.TAG_POWER_COMBO_ATTACK_SPEED_1,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Combo Attack Speed 1",
//                    InternalName = "TAG_POWER_COMBO_ATTACK_SPEED_1",
//                }
//            },
//            {
//                270370, new TagReference
//                {
//                    Id = 270370,
//                    TagType = TagType.TAG_POWER_BUFF_22_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 22 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_22_EFFECT_GROUP",
//                }
//            },
//            {
//                328752, new TagReference
//                {
//                    Id = 328752,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_PLAYERS_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Custom Target Players Only",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_PLAYERS_ONLY",
//                }
//            },
//            {
//                271665, new TagReference
//                {
//                    Id = 271665,
//                    TagType = TagType.TAG_POWER_BUFF_31_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 31 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_31_IS_DISPLAYED",
//                }
//            },
//            {
//                271921, new TagReference
//                {
//                    Id = 271921,
//                    TagType = TagType.TAG_POWER_BUFF_31_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 31 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_31_MERGES_TOOLTIP",
//                }
//            },
//            {
//                272645, new TagReference
//                {
//                    Id = 272645,
//                    TagType = TagType.TAG_POWER_BUFF_5_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 5 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_5_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                274433, new TagReference
//                {
//                    Id = 274433,
//                    TagType = TagType.TAG_POWER_BUFF_1_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 1 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_1_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                328192, new TagReference
//                {
//                    Id = 328192,
//                    TagType = TagType.TAG_POWER_SCALED_ANIMATION_TIMING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "ScaledAnimTiming",
//                    InternalName = "TAG_POWER_SCALED_ANIMATION_TIMING",
//                }
//            },
//            {
//                272640, new TagReference
//                {
//                    Id = 272640,
//                    TagType = TagType.TAG_POWER_BUFF_0_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 0 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_0_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                272384, new TagReference
//                {
//                    Id = 272384,
//                    TagType = TagType.TAG_POWER_BUFF_0_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 0 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_0_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                272391, new TagReference
//                {
//                    Id = 272391,
//                    TagType = TagType.TAG_POWER_BUFF_7_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 7 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_7_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                266560, new TagReference
//                {
//                    Id = 266560,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_4,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 4",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_4",
//                }
//            },
//            {
//                272647, new TagReference
//                {
//                    Id = 272647,
//                    TagType = TagType.TAG_POWER_BUFF_7_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 7 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_7_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                264704, new TagReference
//                {
//                    Id = 264704,
//                    TagType = TagType.TAG_POWER_IS_EMOTE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsEmote",
//                    InternalName = "TAG_POWER_IS_EMOTE",
//                }
//            },
//            {
//                329617, new TagReference
//                {
//                    Id = 329617,
//                    TagType = TagType.TAG_POWER_COST_RESOURCE_MIN_TO_CAST,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Resource Cost Min To Cast",
//                    InternalName = "TAG_POWER_COST_RESOURCE_MIN_TO_CAST",
//                }
//            },
//            {
//                274702, new TagReference
//                {
//                    Id = 274702,
//                    TagType = TagType.TAG_POWER_BUFF_14_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 14 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_14_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                274438, new TagReference
//                {
//                    Id = 274438,
//                    TagType = TagType.TAG_POWER_BUFF_6_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 6 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_6_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                327745, new TagReference
//                {
//                    Id = 327745,
//                    TagType = TagType.TAG_POWER_SPELL_FUNC_INTERRUPTED,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "SpellFunc Interrupted",
//                    InternalName = "TAG_POWER_SPELL_FUNC_INTERRUPTED",
//                }
//            },
//            {
//                274704, new TagReference
//                {
//                    Id = 274704,
//                    TagType = TagType.TAG_POWER_BUFF_16_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 16 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_16_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                274448, new TagReference
//                {
//                    Id = 274448,
//                    TagType = TagType.TAG_POWER_BUFF_16_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 16 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_16_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                270600, new TagReference
//                {
//                    Id = 270600,
//                    TagType = TagType.TAG_POWER_BUFF_8_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 8 Icon",
//                    InternalName = "TAG_POWER_BUFF_8_ICON",
//                }
//            },
//            {
//                264337, new TagReference
//                {
//                    Id = 264337,
//                    TagType = TagType.TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_FEMALE_1,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 1 Contact Frame Effect Group - Female",
//                    InternalName = "TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_FEMALE_1",
//                }
//            },
//            {
//                270344, new TagReference
//                {
//                    Id = 270344,
//                    TagType = TagType.TAG_POWER_BUFF_8_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 8 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_8_EFFECT_GROUP",
//                }
//            },
//            {
//                266768, new TagReference
//                {
//                    Id = 266768,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_11,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 11",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_11",
//                }
//            },
//            {
//                272670, new TagReference
//                {
//                    Id = 272670,
//                    TagType = TagType.TAG_POWER_BUFF_30_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 30 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_30_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                561152, new TagReference
//                {
//                    Id = 561152,
//                    TagType = TagType.TAG_POWER_FOLLOW_START_DISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Follow Start Distance",
//                    InternalName = "TAG_POWER_FOLLOW_START_DISTANCE",
//                }
//            },
//            {
//                262928, new TagReference
//                {
//                    Id = 262928,
//                    TagType = TagType.TAG_POWER_COMBO_TEMPLATE_1,
//                    DataType = MapDataType.Template,
//                    DisplayName = "Combo Template 1",
//                    InternalName = "TAG_POWER_COMBO_TEMPLATE_1",
//                }
//            },
//            {
//                328161, new TagReference
//                {
//                    Id = 328161,
//                    TagType = TagType.TAG_POWER_CAST_TARGET_GROUND_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CastTargetGroundOnly",
//                    InternalName = "TAG_POWER_CAST_TARGET_GROUND_ONLY",
//                }
//            },
//            {
//                270617, new TagReference
//                {
//                    Id = 270617,
//                    TagType = TagType.TAG_POWER_BUFF_19_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 19 Icon",
//                    InternalName = "TAG_POWER_BUFF_19_ICON",
//                }
//            },
//            {
//                272662, new TagReference
//                {
//                    Id = 272662,
//                    TagType = TagType.TAG_POWER_BUFF_22_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 22 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_22_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                681986, new TagReference
//                {
//                    Id = 681986,
//                    TagType = TagType.TAG_POWER_BREAKS_STUN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Breaks Stun",
//                    InternalName = "TAG_POWER_BREAKS_STUN",
//                }
//            },
//            {
//                329616, new TagReference
//                {
//                    Id = 329616,
//                    TagType = TagType.TAG_POWER_COST_RESOURCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Resource Cost",
//                    InternalName = "TAG_POWER_COST_RESOURCE",
//                }
//            },
//            {
//                271379, new TagReference
//                {
//                    Id = 271379,
//                    TagType = TagType.TAG_POWER_BUFF_13_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 13 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_13_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                328016, new TagReference
//                {
//                    Id = 328016,
//                    TagType = TagType.TAG_POWER_NEVER_UPDATES_FACING_STARTING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "NeverUpdatesFacingStarting",
//                    InternalName = "TAG_POWER_NEVER_UPDATES_FACING_STARTING",
//                }
//            },
//            {
//                274457, new TagReference
//                {
//                    Id = 274457,
//                    TagType = TagType.TAG_POWER_BUFF_25_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 25 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_25_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                327840, new TagReference
//                {
//                    Id = 327840,
//                    TagType = TagType.TAG_POWER_IS_OFFENSIVE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsOffensive",
//                    InternalName = "TAG_POWER_IS_OFFENSIVE",
//                }
//            },
//            {
//                271635, new TagReference
//                {
//                    Id = 271635,
//                    TagType = TagType.TAG_POWER_BUFF_13_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 13 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_13_IS_DISPLAYED",
//                }
//            },
//            {
//                274705, new TagReference
//                {
//                    Id = 274705,
//                    TagType = TagType.TAG_POWER_BUFF_17_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 17 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_17_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                270886, new TagReference
//                {
//                    Id = 270886,
//                    TagType = TagType.TAG_POWER_BUFF_26_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 26 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_26_HARMFUL_BUFF",
//                }
//            },
//            {
//                329248, new TagReference
//                {
//                    Id = 329248,
//                    TagType = TagType.TAG_POWER_UPGRADE_1,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Upgrade 1",
//                    InternalName = "TAG_POWER_UPGRADE_1",
//                }
//            },
//            {
//                271654, new TagReference
//                {
//                    Id = 271654,
//                    TagType = TagType.TAG_POWER_BUFF_26_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 26 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_26_IS_DISPLAYED",
//                }
//            },
//            {
//                270883, new TagReference
//                {
//                    Id = 270883,
//                    TagType = TagType.TAG_POWER_BUFF_23_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 23 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_23_HARMFUL_BUFF",
//                }
//            },
//            {
//                327860, new TagReference
//                {
//                    Id = 327860,
//                    TagType = TagType.TAG_POWER_IS_KNOCKBACK_MOVEMENT,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsKnockbackMovement",
//                    InternalName = "TAG_POWER_IS_KNOCKBACK_MOVEMENT",
//                }
//            },
//            {
//                267360, new TagReference
//                {
//                    Id = 267360,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_36,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 36",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_36",
//                }
//            },
//            {
//                267104, new TagReference
//                {
//                    Id = 267104,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_26,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 26",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_26",
//                }
//            },
//            {
//                328801, new TagReference
//                {
//                    Id = 328801,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_BUFF_SNO_BUFF_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Custom Target Buff Power SNO Buff Index",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_BUFF_SNO_BUFF_INDEX",
//                }
//            },
//            {
//                270897, new TagReference
//                {
//                    Id = 270897,
//                    TagType = TagType.TAG_POWER_BUFF_31_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 31 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_31_HARMFUL_BUFF",
//                }
//            },
//            {
//                721266, new TagReference
//                {
//                    Id = 721266,
//                    TagType = TagType.TAG_POWER_RUNEB_DAMAGE_TYPE,
//                    DataType = MapDataType.DamageType,
//                    DisplayName = "RuneB Damage Type",
//                    InternalName = "TAG_POWER_RUNEB_DAMAGE_TYPE",
//                }
//            },
//            {
//                271153, new TagReference
//                {
//                    Id = 271153,
//                    TagType = TagType.TAG_POWER_BUFF_31_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 31 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_31_SHOW_DURATION",
//                }
//            },
//            {
//                267824, new TagReference
//                {
//                    Id = 267824,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_53,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 53",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_53",
//                }
//            },
//            {
//                267568, new TagReference
//                {
//                    Id = 267568,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_43,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 43",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_43",
//                }
//            },
//            {
//                264306, new TagReference
//                {
//                    Id = 264306,
//                    TagType = TagType.TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_2,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 2 Contact Frame Effect Group - Male",
//                    InternalName = "TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_2",
//                }
//            },
//            {
//                263218, new TagReference
//                {
//                    Id = 263218,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_3_RUNE_B,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 3 Rune B",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_3_RUNE_B",
//                }
//            },
//            {
//                328768, new TagReference
//                {
//                    Id = 328768,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_MIN_RANGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Custom Target Min Range",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_MIN_RANGE",
//                }
//            },
//            {
//                328560, new TagReference
//                {
//                    Id = 328560,
//                    TagType = TagType.TAG_POWER_CANCELS_OTHER_POWERS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Cancels Other Powers",
//                    InternalName = "TAG_POWER_CANCELS_OTHER_POWERS",
//                }
//            },
//            {
//                713221, new TagReference
//                {
//                    Id = 713221,
//                    TagType = TagType.TAG_POWER_CONSOLE_USES_TARGET_RETICLE_WHEN_HELD,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Uses Target Reticle When Held",
//                    InternalName = "TAG_POWER_CONSOLE_USES_TARGET_RETICLE_WHEN_HELD",
//                }
//            },
//            {
//                271360, new TagReference
//                {
//                    Id = 271360,
//                    TagType = TagType.TAG_POWER_BUFF_0_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 0 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_0_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                271104, new TagReference
//                {
//                    Id = 271104,
//                    TagType = TagType.TAG_POWER_BUFF_0_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 0 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_0_SHOW_DURATION",
//                }
//            },
//            {
//                270338, new TagReference
//                {
//                    Id = 270338,
//                    TagType = TagType.TAG_POWER_BUFF_2_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 2 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_2_EFFECT_GROUP",
//                }
//            },
//            {
//                272397, new TagReference
//                {
//                    Id = 272397,
//                    TagType = TagType.TAG_POWER_BUFF_13_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 13 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_13_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                263424, new TagReference
//                {
//                    Id = 263424,
//                    TagType = TagType.TAG_POWER_ANIMATION_TAG_TURN_LEFT,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Animation Tag Turn Left",
//                    InternalName = "TAG_POWER_ANIMATION_TAG_TURN_LEFT",
//                }
//            },
//            {
//                268032, new TagReference
//                {
//                    Id = 268032,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_60,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 60",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_60",
//                }
//            },
//            {
//                682241, new TagReference
//                {
//                    Id = 682241,
//                    TagType = TagType.TAG_POWER_IMMUNE_TO_SNARE_DURING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Immune to Snare during",
//                    InternalName = "TAG_POWER_IMMUNE_TO_SNARE_DURING",
//                }
//            },
//            {
//                328336, new TagReference
//                {
//                    Id = 328336,
//                    TagType = TagType.TAG_POWER_RECOIL_IMMUNE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "ImmuneToRecoil",
//                    InternalName = "TAG_POWER_RECOIL_IMMUNE",
//                }
//            },
//            {
//                274708, new TagReference
//                {
//                    Id = 274708,
//                    TagType = TagType.TAG_POWER_BUFF_20_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 20 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_20_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                272131, new TagReference
//                {
//                    Id = 272131,
//                    TagType = TagType.TAG_POWER_BUFF_3_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 3 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_3_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                271124, new TagReference
//                {
//                    Id = 271124,
//                    TagType = TagType.TAG_POWER_BUFF_14_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 14 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_14_SHOW_DURATION",
//                }
//            },
//            {
//                274691, new TagReference
//                {
//                    Id = 274691,
//                    TagType = TagType.TAG_POWER_BUFF_3_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 3 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_3_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                622594, new TagReference
//                {
//                    Id = 622594,
//                    TagType = TagType.TAG_POWER_CONTROLLER_AUTO_TARGETS_LT,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Controller Auto Targets W/LT",
//                    InternalName = "TAG_POWER_CONTROLLER_AUTO_TARGETS_LT",
//                }
//            },
//            {
//                721264, new TagReference
//                {
//                    Id = 721264,
//                    TagType = TagType.TAG_POWER_NORUNE_DAMAGE_TYPE,
//                    DataType = MapDataType.DamageType,
//                    DisplayName = "NoRune Damage Type",
//                    InternalName = "TAG_POWER_NORUNE_DAMAGE_TYPE",
//                }
//            },
//            {
//                329625, new TagReference
//                {
//                    Id = 329625,
//                    TagType = TagType.TAG_POWER_COST_RESOURCE_REDUCTION_COEFFICIENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Resource Cost Reduction Coefficient",
//                    InternalName = "TAG_POWER_COST_RESOURCE_REDUCTION_COEFFICIENT",
//                }
//            },
//            {
//                274461, new TagReference
//                {
//                    Id = 274461,
//                    TagType = TagType.TAG_POWER_BUFF_29_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 29 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_29_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                267344, new TagReference
//                {
//                    Id = 267344,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_35,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 35",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_35",
//                }
//            },
//            {
//                267088, new TagReference
//                {
//                    Id = 267088,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_25,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 25",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_25",
//                }
//            },
//            {
//                329627, new TagReference
//                {
//                    Id = 329627,
//                    TagType = TagType.TAG_POWER_RESOURCE_GAINED_ON_FIRST_HIT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Resource Gained On First Hit",
//                    InternalName = "TAG_POWER_RESOURCE_GAINED_ON_FIRST_HIT",
//                }
//            },
//            {
//                270357, new TagReference
//                {
//                    Id = 270357,
//                    TagType = TagType.TAG_POWER_BUFF_15_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 15 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_15_EFFECT_GROUP",
//                }
//            },
//            {
//                272412, new TagReference
//                {
//                    Id = 272412,
//                    TagType = TagType.TAG_POWER_BUFF_28_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 28 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_28_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                272659, new TagReference
//                {
//                    Id = 272659,
//                    TagType = TagType.TAG_POWER_BUFF_19_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 19 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_19_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                713217, new TagReference
//                {
//                    Id = 713217,
//                    TagType = TagType.TAG_POWER_CONSOLE_AUTO_TARGET_LT_ENABLED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Auto Targeting LT Enabled",
//                    InternalName = "TAG_POWER_CONSOLE_AUTO_TARGET_LT_ENABLED",
//                }
//            },
//            {
//                329488, new TagReference
//                {
//                    Id = 329488,
//                    TagType = TagType.TAG_POWER_ICON_MOUSEOVER,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Icon Mouseover",
//                    InternalName = "TAG_POWER_ICON_MOUSEOVER",
//                }
//            },
//            {
//                271655, new TagReference
//                {
//                    Id = 271655,
//                    TagType = TagType.TAG_POWER_BUFF_27_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 27 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_27_IS_DISPLAYED",
//                }
//            },
//            {
//                328802, new TagReference
//                {
//                    Id = 328802,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_BUFF_SNO,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Custom Target Buff Power SNO",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_BUFF_SNO",
//                }
//            },
//            {
//                270887, new TagReference
//                {
//                    Id = 270887,
//                    TagType = TagType.TAG_POWER_BUFF_27_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 27 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_27_HARMFUL_BUFF",
//                }
//            },
//            {
//                328169, new TagReference
//                {
//                    Id = 328169,
//                    TagType = TagType.TAG_POWER_CAST_TARGET_IGNORE_WRECKABLES,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CastTargetIgnoreWreckables",
//                    InternalName = "TAG_POWER_CAST_TARGET_IGNORE_WRECKABLES",
//                }
//            },
//            {
//                329624, new TagReference
//                {
//                    Id = 329624,
//                    TagType = TagType.TAG_POWER_COST_RESOURCE_REDUCTION,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Resource Cost Reduction",
//                    InternalName = "TAG_POWER_COST_RESOURCE_REDUCTION",
//                }
//            },
//            {
//                271896, new TagReference
//                {
//                    Id = 271896,
//                    TagType = TagType.TAG_POWER_BUFF_18_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 18 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_18_MERGES_TOOLTIP",
//                }
//            },
//            {
//                271640, new TagReference
//                {
//                    Id = 271640,
//                    TagType = TagType.TAG_POWER_BUFF_18_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 18 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_18_IS_DISPLAYED",
//                }
//            },
//            {
//                270372, new TagReference
//                {
//                    Id = 270372,
//                    TagType = TagType.TAG_POWER_BUFF_24_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 24 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_24_EFFECT_GROUP",
//                }
//            },
//            {
//                328160, new TagReference
//                {
//                    Id = 328160,
//                    TagType = TagType.TAG_POWER_TARGET_GROUND_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetGroundOnly",
//                    InternalName = "TAG_POWER_TARGET_GROUND_ONLY",
//                }
//            },
//            {
//                272164, new TagReference
//                {
//                    Id = 272164,
//                    TagType = TagType.TAG_POWER_BUFF_24_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 24 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_24_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                721249, new TagReference
//                {
//                    Id = 721249,
//                    TagType = TagType.TAG_POWER_RUNEE_COMBO2_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneE Combo2 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEE_COMBO2_PROC_SCALAR",
//                }
//            },
//            {
//                271393, new TagReference
//                {
//                    Id = 271393,
//                    TagType = TagType.TAG_POWER_BUFF_21_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 21 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_21_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                328803, new TagReference
//                {
//                    Id = 328803,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_PREFERRED_RANGE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Custom Target Preferred Range Only",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_PREFERRED_RANGE",
//                }
//            },
//            {
//                682243, new TagReference
//                {
//                    Id = 682243,
//                    TagType = TagType.TAG_POWER_IMMUNE_TO_FEAR_DURING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Immune to Fear during",
//                    InternalName = "TAG_POWER_IMMUNE_TO_FEAR_DURING",
//                }
//            },
//            {
//                266544, new TagReference
//                {
//                    Id = 266544,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_3,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 3",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_3",
//                }
//            },
//            {
//                712960, new TagReference
//                {
//                    Id = 712960,
//                    TagType = TagType.TAG_POWER_CONSOLE_TARGET_THETA_GROWTH_PER_KILL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "",
//                    InternalName = "TAG_POWER_CONSOLE_TARGET_THETA_GROWTH_PER_KILL",
//                }
//            },
//            {
//                262962, new TagReference
//                {
//                    Id = 262962,
//                    TagType = TagType.TAG_POWER_COMBO_SPELL_FUNC_INTERRUPTED_3,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "Combo SpellFunc Interrupted 3",
//                    InternalName = "TAG_POWER_COMBO_SPELL_FUNC_INTERRUPTED_3",
//                }
//            },
//            {
//                328643, new TagReference
//                {
//                    Id = 328643,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_B,
//                    DataType = MapDataType.Unk90,
//                    DisplayName = "Special Death Type Rune B",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_B",
//                }
//            },
//            {
//                328577, new TagReference
//                {
//                    Id = 328577,
//                    TagType = TagType.TAG_POWER_NO_RECAST_UNTIL_LAST_ANIM_FRAME,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "No Recast Until Last Anim Frame",
//                    InternalName = "TAG_POWER_NO_RECAST_UNTIL_LAST_ANIM_FRAME",
//                }
//            },
//            {
//                271408, new TagReference
//                {
//                    Id = 271408,
//                    TagType = TagType.TAG_POWER_BUFF_30_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 30 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_30_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                271152, new TagReference
//                {
//                    Id = 271152,
//                    TagType = TagType.TAG_POWER_BUFF_30_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 30 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_30_SHOW_DURATION",
//                }
//            },
//            {
//                272385, new TagReference
//                {
//                    Id = 272385,
//                    TagType = TagType.TAG_POWER_BUFF_1_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 1 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_1_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                264322, new TagReference
//                {
//                    Id = 264322,
//                    TagType = TagType.TAG_POWER_COMBO_CASTING_EFFECT_GROUP_FEMALE_2,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 2 Casting Effect Group - Female",
//                    InternalName = "TAG_POWER_COMBO_CASTING_EFFECT_GROUP_FEMALE_2",
//                }
//            },
//            {
//                328576, new TagReference
//                {
//                    Id = 328576,
//                    TagType = TagType.TAG_POWER_CANCELS_AT_INTERRUPT_FRAME,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Cancels At Interrupt Frame",
//                    InternalName = "TAG_POWER_CANCELS_AT_INTERRUPT_FRAME",
//                }
//            },
//            {
//                272387, new TagReference
//                {
//                    Id = 272387,
//                    TagType = TagType.TAG_POWER_BUFF_3_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 3 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_3_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                272151, new TagReference
//                {
//                    Id = 272151,
//                    TagType = TagType.TAG_POWER_BUFF_17_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 17 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_17_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                274698, new TagReference
//                {
//                    Id = 274698,
//                    TagType = TagType.TAG_POWER_BUFF_10_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 10 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_10_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                329808, new TagReference
//                {
//                    Id = 329808,
//                    TagType = TagType.TAG_POWER_ATTACK_RADIUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Attack Radius",
//                    InternalName = "TAG_POWER_ATTACK_RADIUS",
//                }
//            },
//            {
//                328208, new TagReference
//                {
//                    Id = 328208,
//                    TagType = TagType.TAG_POWER_INFINITE_ANIMATION_TIMING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "InfiniteAnimTiming",
//                    InternalName = "TAG_POWER_INFINITE_ANIMATION_TIMING",
//                }
//            },
//            {
//                271383, new TagReference
//                {
//                    Id = 271383,
//                    TagType = TagType.TAG_POWER_BUFF_17_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 17 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_17_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                274697, new TagReference
//                {
//                    Id = 274697,
//                    TagType = TagType.TAG_POWER_BUFF_9_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 9 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_9_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                272137, new TagReference
//                {
//                    Id = 272137,
//                    TagType = TagType.TAG_POWER_BUFF_9_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 9 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_9_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                272656, new TagReference
//                {
//                    Id = 272656,
//                    TagType = TagType.TAG_POWER_BUFF_16_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 16 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_16_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                270345, new TagReference
//                {
//                    Id = 270345,
//                    TagType = TagType.TAG_POWER_BUFF_9_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 9 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_9_EFFECT_GROUP",
//                }
//            },
//            {
//                272400, new TagReference
//                {
//                    Id = 272400,
//                    TagType = TagType.TAG_POWER_BUFF_16_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 16 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_16_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                684917, new TagReference
//                {
//                    Id = 684917,
//                    TagType = TagType.TAG_POWER_PLAYER_CRIT_DAMAGE_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Player Crit Damage Scalar",
//                    InternalName = "TAG_POWER_PLAYER_CRIT_DAMAGE_SCALAR",
//                }
//            },
//            {
//                263185, new TagReference
//                {
//                    Id = 263185,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_1_RUNE_A,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 1 Rune A",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_1_RUNE_A",
//                }
//            },
//            {
//                716800, new TagReference
//                {
//                    Id = 716800,
//                    TagType = TagType.TAG_POWER_ICON_ROUND,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Icon Round",
//                    InternalName = "TAG_POWER_ICON_ROUND",
//                }
//            },
//            {
//                264720, new TagReference
//                {
//                    Id = 264720,
//                    TagType = TagType.TAG_POWER_EMOTE_CONVERSATION_SNO,
//                    DataType = MapDataType.ConversationSno,
//                    DisplayName = "Emote Conversation SNO",
//                    InternalName = "TAG_POWER_EMOTE_CONVERSATION_SNO",
//                }
//            },
//            {
//                329633, new TagReference
//                {
//                    Id = 329633,
//                    TagType = TagType.TAG_POWER_ONLY_FREE_CAST,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Only Free Cast",
//                    InternalName = "TAG_POWER_ONLY_FREE_CAST",
//                }
//            },
//            {
//                721280, new TagReference
//                {
//                    Id = 721280,
//                    TagType = TagType.TAG_POWER_GHOST_WALLWALK_EFFECT_SNO,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Ghost Wallwalk Effect",
//                    InternalName = "TAG_POWER_GHOST_WALLWALK_EFFECT_SNO",
//                }
//            },
//            {
//                272402, new TagReference
//                {
//                    Id = 272402,
//                    TagType = TagType.TAG_POWER_BUFF_18_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 18 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_18_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                270616, new TagReference
//                {
//                    Id = 270616,
//                    TagType = TagType.TAG_POWER_BUFF_18_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 18 Icon",
//                    InternalName = "TAG_POWER_BUFF_18_ICON",
//                }
//            },
//            {
//                328602, new TagReference
//                {
//                    Id = 328602,
//                    TagType = TagType.TAG_POWER_SENDS_FAILURE_REASON_TO_CLIENT,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Send Failure Reason To Client",
//                    InternalName = "TAG_POWER_SENDS_FAILURE_REASON_TO_CLIENT",
//                }
//            },
//            {
//                270360, new TagReference
//                {
//                    Id = 270360,
//                    TagType = TagType.TAG_POWER_BUFF_18_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 18 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_18_EFFECT_GROUP",
//                }
//            },
//            {
//                262944, new TagReference
//                {
//                    Id = 262944,
//                    TagType = TagType.TAG_POWER_COMBO_SPELL_FUNC_END_1,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "Combo SpellFunc End 1",
//                    InternalName = "TAG_POWER_COMBO_SPELL_FUNC_END_1",
//                }
//            },
//            {
//                328177, new TagReference
//                {
//                    Id = 328177,
//                    TagType = TagType.TAG_POWER_TRACKS_AFFECTED_ACD_INSTANCE_WIDE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TracksAffectedACDInstanceWide",
//                    InternalName = "TAG_POWER_TRACKS_AFFECTED_ACD_INSTANCE_WIDE",
//                }
//            },
//            {
//                270633, new TagReference
//                {
//                    Id = 270633,
//                    TagType = TagType.TAG_POWER_BUFF_29_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 29 Icon",
//                    InternalName = "TAG_POWER_BUFF_29_ICON",
//                }
//            },
//            {
//                272168, new TagReference
//                {
//                    Id = 272168,
//                    TagType = TagType.TAG_POWER_BUFF_28_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 28 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_28_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                329632, new TagReference
//                {
//                    Id = 329632,
//                    TagType = TagType.TAG_POWER_COST_HEALTH,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Health Cost",
//                    InternalName = "TAG_POWER_COST_HEALTH",
//                }
//            },
//            {
//                328032, new TagReference
//                {
//                    Id = 328032,
//                    TagType = TagType.TAG_POWER_DOESNT_CENTER,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Doesn't Center",
//                    InternalName = "TAG_POWER_DOESNT_CENTER",
//                }
//            },
//            {
//                271904, new TagReference
//                {
//                    Id = 271904,
//                    TagType = TagType.TAG_POWER_BUFF_20_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 20 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_20_MERGES_TOOLTIP",
//                }
//            },
//            {
//                327856, new TagReference
//                {
//                    Id = 327856,
//                    TagType = TagType.TAG_POWER_IS_TRANSLATE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsTranslate",
//                    InternalName = "TAG_POWER_IS_TRANSLATE",
//                }
//            },
//            {
//                271648, new TagReference
//                {
//                    Id = 271648,
//                    TagType = TagType.TAG_POWER_BUFF_20_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 20 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_20_IS_DISPLAYED",
//                }
//            },
//            {
//                329634, new TagReference
//                {
//                    Id = 329634,
//                    TagType = TagType.TAG_POWER_COST_ONLY_FREE_CAST,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Cost Only Free Cast",
//                    InternalName = "TAG_POWER_COST_ONLY_FREE_CAST",
//                }
//            },
//            {
//                329264, new TagReference
//                {
//                    Id = 329264,
//                    TagType = TagType.TAG_POWER_UPGRADE_2,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Upgrade 2",
//                    InternalName = "TAG_POWER_UPGRADE_2",
//                }
//            },
//            {
//                328069, new TagReference
//                {
//                    Id = 328069,
//                    TagType = TagType.TAG_POWER_MODAL_CURSOR_RADIUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Modal Cursor Radius",
//                    InternalName = "TAG_POWER_MODAL_CURSOR_RADIUS",
//                }
//            },
//            {
//                328647, new TagReference
//                {
//                    Id = 328647,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_D,
//                    DataType = MapDataType.Unk90,
//                    DisplayName = "Special Death Type Rune D",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_D",
//                }
//            },
//            {
//                270340, new TagReference
//                {
//                    Id = 270340,
//                    TagType = TagType.TAG_POWER_BUFF_4_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 4 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_4_EFFECT_GROUP",
//                }
//            },
//            {
//                272132, new TagReference
//                {
//                    Id = 272132,
//                    TagType = TagType.TAG_POWER_BUFF_4_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 4 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_4_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                263300, new TagReference
//                {
//                    Id = 263300,
//                    TagType = TagType.TAG_POWER_LOOPING_SUPPRESS_ITEM_TOOLTIPS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Looping Suppress Item Tooltips",
//                    InternalName = "TAG_POWER_LOOPING_SUPPRESS_ITEM_TOOLTIPS",
//                }
//            },
//            {
//                271361, new TagReference
//                {
//                    Id = 271361,
//                    TagType = TagType.TAG_POWER_BUFF_1_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 1 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_1_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                721154, new TagReference
//                {
//                    Id = 721154,
//                    TagType = TagType.TAG_POWER_RUNEB_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneB Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEB_PROC_SCALAR",
//                }
//            },
//            {
//                655984, new TagReference
//                {
//                    Id = 655984,
//                    TagType = TagType.TAG_POWER_PICKUP_ITEMS_WHILE_MOVING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Pickup Items While Moving",
//                    InternalName = "TAG_POWER_PICKUP_ITEMS_WHILE_MOVING",
//                }
//            },
//            {
//                328385, new TagReference
//                {
//                    Id = 328385,
//                    TagType = TagType.TAG_POWER_TURNS_INTO_BASIC_ATTACK_RANGED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TurnsIntoBasicRangedAttack",
//                    InternalName = "TAG_POWER_TURNS_INTO_BASIC_ATTACK_RANGED",
//                }
//            },
//            {
//                274701, new TagReference
//                {
//                    Id = 274701,
//                    TagType = TagType.TAG_POWER_BUFF_13_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 13 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_13_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                266816, new TagReference
//                {
//                    Id = 266816,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_14,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 14",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_14",
//                }
//            },
//            {
//                270598, new TagReference
//                {
//                    Id = 270598,
//                    TagType = TagType.TAG_POWER_BUFF_6_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 6 Icon",
//                    InternalName = "TAG_POWER_BUFF_6_ICON",
//                }
//            },
//            {
//                328784, new TagReference
//                {
//                    Id = 328784,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_MAX_RANGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Custom Target Max Range",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_MAX_RANGE",
//                }
//            },
//            {
//                270359, new TagReference
//                {
//                    Id = 270359,
//                    TagType = TagType.TAG_POWER_BUFF_17_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 17 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_17_EFFECT_GROUP",
//                }
//            },
//            {
//                328448, new TagReference
//                {
//                    Id = 328448,
//                    TagType = TagType.TAG_POWER_AFFECTED_BY_DUAL_WIELD,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "AffectedByDualWield",
//                    InternalName = "TAG_POWER_AFFECTED_BY_DUAL_WIELD",
//                }
//            },
//            {
//                270615, new TagReference
//                {
//                    Id = 270615,
//                    TagType = TagType.TAG_POWER_BUFF_17_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 17 Icon",
//                    InternalName = "TAG_POWER_BUFF_17_ICON",
//                }
//            },
//            {
//                327762, new TagReference
//                {
//                    Id = 327762,
//                    TagType = TagType.TAG_POWER_TARGETING_PASSABILITY_SNO,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Targeting Passability SNO",
//                    InternalName = "TAG_POWER_TARGETING_PASSABILITY_SNO",
//                }
//            },
//            {
//                271376, new TagReference
//                {
//                    Id = 271376,
//                    TagType = TagType.TAG_POWER_BUFF_10_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 10 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_10_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                274447, new TagReference
//                {
//                    Id = 274447,
//                    TagType = TagType.TAG_POWER_BUFF_15_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 15 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_15_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                271120, new TagReference
//                {
//                    Id = 271120,
//                    TagType = TagType.TAG_POWER_BUFF_10_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 10 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_10_SHOW_DURATION",
//                }
//            },
//            {
//                274703, new TagReference
//                {
//                    Id = 274703,
//                    TagType = TagType.TAG_POWER_BUFF_15_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 15 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_15_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                713222, new TagReference
//                {
//                    Id = 713222,
//                    TagType = TagType.TAG_POWER_CONSOLE_USE_AT_MAX_RANGE_WHEN_HELD,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Fires At Max Range When Held",
//                    InternalName = "TAG_POWER_CONSOLE_USE_AT_MAX_RANGE_WHEN_HELD",
//                }
//            },
//            {
//                270856, new TagReference
//                {
//                    Id = 270856,
//                    TagType = TagType.TAG_POWER_BUFF_8_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 8 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_8_HARMFUL_BUFF",
//                }
//            },
//            {
//                272413, new TagReference
//                {
//                    Id = 272413,
//                    TagType = TagType.TAG_POWER_BUFF_29_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 29 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_29_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                263187, new TagReference
//                {
//                    Id = 263187,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_1_RUNE_C,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 1 Rune C",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_1_RUNE_C",
//                }
//            },
//            {
//                328163, new TagReference
//                {
//                    Id = 328163,
//                    TagType = TagType.TAG_POWER_TARGET_NAV_MESH_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetNavMeshOnly",
//                    InternalName = "TAG_POWER_TARGET_NAV_MESH_ONLY",
//                }
//            },
//            {
//                272145, new TagReference
//                {
//                    Id = 272145,
//                    TagType = TagType.TAG_POWER_BUFF_11_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 11 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_11_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                684849, new TagReference
//                {
//                    Id = 684849,
//                    TagType = TagType.TAG_POWER_EXTENDS_TARGET_TO_ATTACK_RADIUS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Extends Target To Attack Radius",
//                    InternalName = "TAG_POWER_EXTENDS_TARGET_TO_ATTACK_RADIUS",
//                }
//            },
//            {
//                270353, new TagReference
//                {
//                    Id = 270353,
//                    TagType = TagType.TAG_POWER_BUFF_11_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 11 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_11_EFFECT_GROUP",
//                }
//            },
//            {
//                328352, new TagReference
//                {
//                    Id = 328352,
//                    TagType = TagType.TAG_POWER_KNOCKBACK_IMMUNE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "ImmuneToKnockback",
//                    InternalName = "TAG_POWER_KNOCKBACK_IMMUNE",
//                }
//            },
//            {
//                329811, new TagReference
//                {
//                    Id = 329811,
//                    TagType = TagType.TAG_POWER_COMBO_ATTACK_RADIUS_3,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Combo Attack Radius 3",
//                    InternalName = "TAG_POWER_COMBO_ATTACK_RADIUS_3",
//                }
//            },
//            {
//                272147, new TagReference
//                {
//                    Id = 272147,
//                    TagType = TagType.TAG_POWER_BUFF_13_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 13 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_13_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                271634, new TagReference
//                {
//                    Id = 271634,
//                    TagType = TagType.TAG_POWER_BUFF_12_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 12 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_12_IS_DISPLAYED",
//                }
//            },
//            {
//                328604, new TagReference
//                {
//                    Id = 328604,
//                    TagType = TagType.TAG_POWER_CALL_AIUPDATE_IMMEDIATELY_UPON_TERMINATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Call AIUpdate Immediately Upon Termination",
//                    InternalName = "TAG_POWER_CALL_AIUPDATE_IMMEDIATELY_UPON_TERMINATION",
//                }
//            },
//            {
//                270374, new TagReference
//                {
//                    Id = 270374,
//                    TagType = TagType.TAG_POWER_BUFF_26_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 26 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_26_EFFECT_GROUP",
//                }
//            },
//            {
//                721152, new TagReference
//                {
//                    Id = 721152,
//                    TagType = TagType.TAG_POWER_NORUNE_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "NoRune Proc Scalar",
//                    InternalName = "TAG_POWER_NORUNE_PROC_SCALAR",
//                }
//            },
//            {
//                271142, new TagReference
//                {
//                    Id = 271142,
//                    TagType = TagType.TAG_POWER_BUFF_26_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 26 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_26_SHOW_DURATION",
//                }
//            },
//            {
//                328606, new TagReference
//                {
//                    Id = 328606,
//                    TagType = TagType.TAG_POWER_USES_ATTACK_WARMUP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Uses Attack Warmup Time",
//                    InternalName = "TAG_POWER_USES_ATTACK_WARMUP",
//                }
//            },
//            {
//                327769, new TagReference
//                {
//                    Id = 327769,
//                    TagType = TagType.TAG_POWER_AI_PACK_COOLDOWN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "AI Pack Cooldown Time",
//                    InternalName = "TAG_POWER_AI_PACK_COOLDOWN",
//                }
//            },
//            {
//                270373, new TagReference
//                {
//                    Id = 270373,
//                    TagType = TagType.TAG_POWER_BUFF_25_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 25 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_25_EFFECT_GROUP",
//                }
//            },
//            {
//                329831, new TagReference
//                {
//                    Id = 329831,
//                    TagType = TagType.TAG_POWER_USES_MAINHAND_ONLY_COMBO1,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Combo Level 1 Uses Main Hand Only",
//                    InternalName = "TAG_POWER_USES_MAINHAND_ONLY_COMBO1",
//                }
//            },
//            {
//                267872, new TagReference
//                {
//                    Id = 267872,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_56,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 56",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_56",
//                }
//            },
//            {
//                267616, new TagReference
//                {
//                    Id = 267616,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_46,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 46",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_46",
//                }
//            },
//            {
//                270624, new TagReference
//                {
//                    Id = 270624,
//                    TagType = TagType.TAG_POWER_BUFF_20_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 20 Icon",
//                    InternalName = "TAG_POWER_BUFF_20_ICON",
//                }
//            },
//            {
//                328610, new TagReference
//                {
//                    Id = 328610,
//                    TagType = TagType.TAG_POWER_IS_UNINTERRUPTABLE_DURING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Is Uninterruptable During",
//                    InternalName = "TAG_POWER_IS_UNINTERRUPTABLE_DURING",
//                }
//            },
//            {
//                270368, new TagReference
//                {
//                    Id = 270368,
//                    TagType = TagType.TAG_POWER_BUFF_20_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 20 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_20_EFFECT_GROUP",
//                }
//            },
//            {
//                328049, new TagReference
//                {
//                    Id = 328049,
//                    TagType = TagType.TAG_POWER_AUTOASSIGN_LOCATION,
//                    DataType = MapDataType.Unk99,
//                    DisplayName = "AutoAssignLocation",
//                    InternalName = "TAG_POWER_AUTOASSIGN_LOCATION",
//                }
//            },
//            {
//                329504, new TagReference
//                {
//                    Id = 329504,
//                    TagType = TagType.TAG_POWER_ICON_PUSHED,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Icon Pushed",
//                    InternalName = "TAG_POWER_ICON_PUSHED",
//                }
//            },
//            {
//                270641, new TagReference
//                {
//                    Id = 270641,
//                    TagType = TagType.TAG_POWER_BUFF_31_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 31 Icon",
//                    InternalName = "TAG_POWER_BUFF_31_ICON",
//                }
//            },
//            {
//                327864, new TagReference
//                {
//                    Id = 327864,
//                    TagType = TagType.TAG_POWER_CHECKS_VERTICAL_MOVEMENT,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "ChecksVerticalMovement",
//                    InternalName = "TAG_POWER_CHECKS_VERTICAL_MOVEMENT",
//                }
//            },
//            {
//                263223, new TagReference
//                {
//                    Id = 263223,
//                    TagType = TagType.TAG_POWER_ANIMATION_MAX_SCALING,
//                    DataType = MapDataType.HighPrecision,
//                    DisplayName = "Scaled Animation Timing Max",
//                    InternalName = "TAG_POWER_ANIMATION_MAX_SCALING",
//                }
//            },
//            {
//                328176, new TagReference
//                {
//                    Id = 328176,
//                    TagType = TagType.TAG_POWER_NO_AFFECTED_ACD,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "NoAffectedACD",
//                    InternalName = "TAG_POWER_NO_AFFECTED_ACD",
//                }
//            },
//            {
//                721265, new TagReference
//                {
//                    Id = 721265,
//                    TagType = TagType.TAG_POWER_RUNEA_DAMAGE_TYPE,
//                    DataType = MapDataType.DamageType,
//                    DisplayName = "RuneA Damage Type",
//                    InternalName = "TAG_POWER_RUNEA_DAMAGE_TYPE",
//                }
//            },
//            {
//                622595, new TagReference
//                {
//                    Id = 622595,
//                    TagType = TagType.TAG_POWER_CONTROLLER_MIN_RANGE_HELD,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Min Range When Held",
//                    InternalName = "TAG_POWER_CONTROLLER_MIN_RANGE_HELD",
//                }
//            },
//            {
//                271872, new TagReference
//                {
//                    Id = 271872,
//                    TagType = TagType.TAG_POWER_BUFF_0_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 0 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_0_MERGES_TOOLTIP",
//                }
//            },
//            {
//                274690, new TagReference
//                {
//                    Id = 274690,
//                    TagType = TagType.TAG_POWER_BUFF_2_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 2 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_2_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                271616, new TagReference
//                {
//                    Id = 271616,
//                    TagType = TagType.TAG_POWER_BUFF_0_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 0 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_0_IS_DISPLAYED",
//                }
//            },
//            {
//                267392, new TagReference
//                {
//                    Id = 267392,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_38,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 38",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_38",
//                }
//            },
//            {
//                267136, new TagReference
//                {
//                    Id = 267136,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_28,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 28",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_28",
//                }
//            },
//            {
//                684918, new TagReference
//                {
//                    Id = 684918,
//                    TagType = TagType.TAG_POWER_MONSTER_CRIT_DAMAGE_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Monster Crit Damage Scalar",
//                    InternalName = "TAG_POWER_MONSTER_CRIT_DAMAGE_SCALAR",
//                }
//            },
//            {
//                263426, new TagReference
//                {
//                    Id = 263426,
//                    TagType = TagType.TAG_POWER_ANIMATION_TURN_THRESHOLD,
//                    DataType = MapDataType.HighPrecision,
//                    DisplayName = "Animation Turn Threshold",
//                    InternalName = "TAG_POWER_ANIMATION_TURN_THRESHOLD",
//                }
//            },
//            {
//                271879, new TagReference
//                {
//                    Id = 271879,
//                    TagType = TagType.TAG_POWER_BUFF_7_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 7 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_7_MERGES_TOOLTIP",
//                }
//            },
//            {
//                274435, new TagReference
//                {
//                    Id = 274435,
//                    TagType = TagType.TAG_POWER_BUFF_3_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 3 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_3_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                267856, new TagReference
//                {
//                    Id = 267856,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_55,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 55",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_55",
//                }
//            },
//            {
//                267600, new TagReference
//                {
//                    Id = 267600,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_45,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 45",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_45",
//                }
//            },
//            {
//                264338, new TagReference
//                {
//                    Id = 264338,
//                    TagType = TagType.TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_FEMALE_2,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 2 Contact Frame Effect Group - Female",
//                    InternalName = "TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_FEMALE_2",
//                }
//            },
//            {
//                327831, new TagReference
//                {
//                    Id = 327831,
//                    TagType = TagType.TAG_POWER_SHOW_ATTACK_WITHOUT_MOVE_TIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "ShowAttackWithoutMoveTip",
//                    InternalName = "TAG_POWER_SHOW_ATTACK_WITHOUT_MOVE_TIP",
//                }
//            },
//            {
//                272668, new TagReference
//                {
//                    Id = 272668,
//                    TagType = TagType.TAG_POWER_BUFF_28_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 28 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_28_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                328022, new TagReference
//                {
//                    Id = 328022,
//                    TagType = TagType.TAG_POWER_CLIENT_CONTROLS_FACING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "ClientControlsFacing",
//                    InternalName = "TAG_POWER_CLIENT_CONTROLS_FACING",
//                }
//            },
//            {
//                328592, new TagReference
//                {
//                    Id = 328592,
//                    TagType = TagType.TAG_POWER_IS_PASSIVE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Is Passive",
//                    InternalName = "TAG_POWER_IS_PASSIVE",
//                }
//            },
//            {
//                272403, new TagReference
//                {
//                    Id = 272403,
//                    TagType = TagType.TAG_POWER_BUFF_19_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 19 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_19_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                331600, new TagReference
//                {
//                    Id = 331600,
//                    TagType = TagType.TAG_POWER_MANA_PERCENT_TO_RESERVE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Mana Percent To Reserve",
//                    InternalName = "TAG_POWER_MANA_PERCENT_TO_RESERVE",
//                }
//            },
//            {
//                272166, new TagReference
//                {
//                    Id = 272166,
//                    TagType = TagType.TAG_POWER_BUFF_26_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 26 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_26_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                684848, new TagReference
//                {
//                    Id = 684848,
//                    TagType = TagType.TAG_POWER_CLIPS_TARGET_TO_ATTACK_RADIUS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Clips Target To Attack Radius",
//                    InternalName = "TAG_POWER_CLIPS_TARGET_TO_ATTACK_RADIUS",
//                }
//            },
//            {
//                272167, new TagReference
//                {
//                    Id = 272167,
//                    TagType = TagType.TAG_POWER_BUFF_27_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 27 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_27_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                329824, new TagReference
//                {
//                    Id = 329824,
//                    TagType = TagType.TAG_POWER_ATTACK_SPEED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Attack Speed",
//                    InternalName = "TAG_POWER_ATTACK_SPEED",
//                }
//            },
//            {
//                271910, new TagReference
//                {
//                    Id = 271910,
//                    TagType = TagType.TAG_POWER_BUFF_26_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 26 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_26_MERGES_TOOLTIP",
//                }
//            },
//            {
//                271399, new TagReference
//                {
//                    Id = 271399,
//                    TagType = TagType.TAG_POWER_BUFF_27_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 27 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_27_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                274450, new TagReference
//                {
//                    Id = 274450,
//                    TagType = TagType.TAG_POWER_BUFF_18_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 18 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_18_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                270627, new TagReference
//                {
//                    Id = 270627,
//                    TagType = TagType.TAG_POWER_BUFF_23_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 23 Icon",
//                    InternalName = "TAG_POWER_BUFF_23_ICON",
//                }
//            },
//            {
//                271907, new TagReference
//                {
//                    Id = 271907,
//                    TagType = TagType.TAG_POWER_BUFF_23_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 23 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_23_MERGES_TOOLTIP",
//                }
//            },
//            {
//                263201, new TagReference
//                {
//                    Id = 263201,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_2_RUNE_A,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 2 Rune A",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_2_RUNE_A",
//                }
//            },
//            {
//                274715, new TagReference
//                {
//                    Id = 274715,
//                    TagType = TagType.TAG_POWER_BUFF_27_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 27 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_27_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                266592, new TagReference
//                {
//                    Id = 266592,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_6,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 6",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_6",
//                }
//            },
//            {
//                270884, new TagReference
//                {
//                    Id = 270884,
//                    TagType = TagType.TAG_POWER_BUFF_24_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 24 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_24_HARMFUL_BUFF",
//                }
//            },
//            {
//                270628, new TagReference
//                {
//                    Id = 270628,
//                    TagType = TagType.TAG_POWER_BUFF_24_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 24 Icon",
//                    InternalName = "TAG_POWER_BUFF_24_ICON",
//                }
//            },
//            {
//                328618, new TagReference
//                {
//                    Id = 328618,
//                    TagType = TagType.TAG_POWER_CAST_TARGET_UNDEAD_MONSTERS_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CastTargetUndeadMonstersOnly",
//                    InternalName = "TAG_POWER_CAST_TARGET_UNDEAD_MONSTERS_ONLY",
//                }
//            },
//            {
//                266800, new TagReference
//                {
//                    Id = 266800,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_13,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 13",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_13",
//                }
//            },
//            {
//                329512, new TagReference
//                {
//                    Id = 329512,
//                    TagType = TagType.TAG_POWER_ICON_INACTIVE,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Icon Inactive",
//                    InternalName = "TAG_POWER_ICON_INACTIVE",
//                }
//            },
//            {
//                274693, new TagReference
//                {
//                    Id = 274693,
//                    TagType = TagType.TAG_POWER_BUFF_5_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 5 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_5_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                262960, new TagReference
//                {
//                    Id = 262960,
//                    TagType = TagType.TAG_POWER_COMBO_SPELL_FUNC_INTERRUPTED_1,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "Combo SpellFunc Interrupted 1",
//                    InternalName = "TAG_POWER_COMBO_SPELL_FUNC_INTERRUPTED_1",
//                }
//            },
//            {
//                270341, new TagReference
//                {
//                    Id = 270341,
//                    TagType = TagType.TAG_POWER_BUFF_5_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 5 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_5_EFFECT_GROUP",
//                }
//            },
//            {
//                329648, new TagReference
//                {
//                    Id = 329648,
//                    TagType = TagType.TAG_POWER_CLASS_RESTRICTION,
//                    DataType = MapDataType.ActorClass,
//                    DisplayName = "Class Restriction",
//                    InternalName = "TAG_POWER_CLASS_RESTRICTION",
//                }
//            },
//            {
//                328048, new TagReference
//                {
//                    Id = 328048,
//                    TagType = TagType.TAG_POWER_IS_MOUSE_ASSIGNABLE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsMouseAssignable",
//                    InternalName = "TAG_POWER_IS_MOUSE_ASSIGNABLE",
//                }
//            },
//            {
//                271920, new TagReference
//                {
//                    Id = 271920,
//                    TagType = TagType.TAG_POWER_BUFF_30_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 30 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_30_MERGES_TOOLTIP",
//                }
//            },
//            {
//                327744, new TagReference
//                {
//                    Id = 327744,
//                    TagType = TagType.TAG_POWER_SPELL_FUNC_END,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "SpellFunc End",
//                    InternalName = "TAG_POWER_SPELL_FUNC_END",
//                }
//            },
//            {
//                271664, new TagReference
//                {
//                    Id = 271664,
//                    TagType = TagType.TAG_POWER_BUFF_30_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 30 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_30_IS_DISPLAYED",
//                }
//            },
//            {
//                270592, new TagReference
//                {
//                    Id = 270592,
//                    TagType = TagType.TAG_POWER_BUFF_0_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 0 Icon",
//                    InternalName = "TAG_POWER_BUFF_0_ICON",
//                }
//            },
//            {
//                270336, new TagReference
//                {
//                    Id = 270336,
//                    TagType = TagType.TAG_POWER_BUFF_0_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 0 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_0_EFFECT_GROUP",
//                }
//            },
//            {
//                274439, new TagReference
//                {
//                    Id = 274439,
//                    TagType = TagType.TAG_POWER_BUFF_7_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 7 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_7_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                272398, new TagReference
//                {
//                    Id = 272398,
//                    TagType = TagType.TAG_POWER_BUFF_14_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 14 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_14_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                274695, new TagReference
//                {
//                    Id = 274695,
//                    TagType = TagType.TAG_POWER_BUFF_7_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 7 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_7_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                712963, new TagReference
//                {
//                    Id = 712963,
//                    TagType = TagType.TAG_POWER_CONSOLE_AUTO_TARGET_SEARCH_ANGLE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Auto Target Search Angle",
//                    InternalName = "TAG_POWER_CONSOLE_AUTO_TARGET_SEARCH_ANGLE",
//                }
//            },
//            {
//                262656, new TagReference
//                {
//                    Id = 262656,
//                    TagType = TagType.TAG_POWER_ANIMATION_TAG,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Animation Tag",
//                    InternalName = "TAG_POWER_ANIMATION_TAG",
//                }
//            },
//            {
//                327764, new TagReference
//                {
//                    Id = 327764,
//                    TagType = TagType.TAG_POWER_SYNERGY_POWER,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Synergy Power",
//                    InternalName = "TAG_POWER_SYNERGY_POWER",
//                }
//            },
//            {
//                267264, new TagReference
//                {
//                    Id = 267264,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_30,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 30",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_30",
//                }
//            },
//            {
//                272390, new TagReference
//                {
//                    Id = 272390,
//                    TagType = TagType.TAG_POWER_BUFF_6_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 6 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_6_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                267008, new TagReference
//                {
//                    Id = 267008,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_20,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 20",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_20",
//                }
//            },
//            {
//                329088, new TagReference
//                {
//                    Id = 329088,
//                    TagType = TagType.TAG_POWER_CONSUMES_ITEM,
//                    DataType = MapDataType.ActorSno,
//                    DisplayName = "Consumes Item",
//                    InternalName = "TAG_POWER_CONSUMES_ITEM",
//                }
//            },
//            {
//                622592, new TagReference
//                {
//                    Id = 622592,
//                    TagType = TagType.TAG_POWER_CONTROLLER_AUTO_TARGETS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Controller Auto Targets",
//                    InternalName = "TAG_POWER_CONTROLLER_AUTO_TARGETS",
//                }
//            },
//            {
//                270356, new TagReference
//                {
//                    Id = 270356,
//                    TagType = TagType.TAG_POWER_BUFF_14_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 14 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_14_EFFECT_GROUP",
//                }
//            },
//            {
//                272148, new TagReference
//                {
//                    Id = 272148,
//                    TagType = TagType.TAG_POWER_BUFF_14_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 14 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_14_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                327683, new TagReference
//                {
//                    Id = 327683,
//                    TagType = TagType.TAG_POWER_TEMPLATE_RUNE_C,
//                    DataType = MapDataType.Template,
//                    DisplayName = "Template Rune C",
//                    InternalName = "TAG_POWER_TEMPLATE_RUNE_C",
//                }
//            },
//            {
//                713472, new TagReference
//                {
//                    Id = 713472,
//                    TagType = TagType.TAG_POWER_CONSOLE_REFUNDS_RESOURCES_WITHOUT_DAMAGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Refunds Resources Without Damage",
//                    InternalName = "TAG_POWER_CONSOLE_REFUNDS_RESOURCES_WITHOUT_DAMAGE",
//                }
//            },
//            {
//                272648, new TagReference
//                {
//                    Id = 272648,
//                    TagType = TagType.TAG_POWER_BUFF_8_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 8 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_8_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                266576, new TagReference
//                {
//                    Id = 266576,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_5,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 5",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_5",
//                }
//            },
//            {
//                721170, new TagReference
//                {
//                    Id = 721170,
//                    TagType = TagType.TAG_POWER_NORUNE_COMBO3_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "NoRune Combo3 Proc Scalar",
//                    InternalName = "TAG_POWER_NORUNE_COMBO3_PROC_SCALAR",
//                }
//            },
//            {
//                262676, new TagReference
//                {
//                    Id = 262676,
//                    TagType = TagType.TAG_POWER_ANIMATION_TAG_RUNE_D,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Animation Tag Rune D",
//                    InternalName = "TAG_POWER_ANIMATION_TAG_RUNE_D",
//                }
//            },
//            {
//                271625, new TagReference
//                {
//                    Id = 271625,
//                    TagType = TagType.TAG_POWER_BUFF_9_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 9 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_9_IS_DISPLAYED",
//                }
//            },
//            {
//                328601, new TagReference
//                {
//                    Id = 328601,
//                    TagType = TagType.TAG_POWER_IS_ITEM_POWER,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Is Item Power",
//                    InternalName = "TAG_POWER_IS_ITEM_POWER",
//                }
//            },
//            {
//                622597, new TagReference
//                {
//                    Id = 622597,
//                    TagType = TagType.TAG_POWER_CONTROLLER_OFFSET_FROM_AUTO_TARGET,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Offset From Auto Target",
//                    InternalName = "TAG_POWER_CONTROLLER_OFFSET_FROM_AUTO_TARGET",
//                }
//            },
//            {
//                272392, new TagReference
//                {
//                    Id = 272392,
//                    TagType = TagType.TAG_POWER_BUFF_8_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 8 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_8_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                271881, new TagReference
//                {
//                    Id = 271881,
//                    TagType = TagType.TAG_POWER_BUFF_9_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 9 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_9_MERGES_TOOLTIP",
//                }
//            },
//            {
//                274718, new TagReference
//                {
//                    Id = 274718,
//                    TagType = TagType.TAG_POWER_BUFF_30_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 30 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_30_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                271125, new TagReference
//                {
//                    Id = 271125,
//                    TagType = TagType.TAG_POWER_BUFF_15_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 15 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_15_SHOW_DURATION",
//                }
//            },
//            {
//                328401, new TagReference
//                {
//                    Id = 328401,
//                    TagType = TagType.TAG_POWER_CHANNELLED_LOCKS_ACTORS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "ChannelledLocksActors",
//                    InternalName = "TAG_POWER_CHANNELLED_LOCKS_ACTORS",
//                }
//            },
//            {
//                274454, new TagReference
//                {
//                    Id = 274454,
//                    TagType = TagType.TAG_POWER_BUFF_22_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 22 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_22_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                271381, new TagReference
//                {
//                    Id = 271381,
//                    TagType = TagType.TAG_POWER_BUFF_15_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 15 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_15_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                272665, new TagReference
//                {
//                    Id = 272665,
//                    TagType = TagType.TAG_POWER_BUFF_25_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 25 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_25_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                270614, new TagReference
//                {
//                    Id = 270614,
//                    TagType = TagType.TAG_POWER_BUFF_16_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 16 Icon",
//                    InternalName = "TAG_POWER_BUFF_16_ICON",
//                }
//            },
//            {
//                272657, new TagReference
//                {
//                    Id = 272657,
//                    TagType = TagType.TAG_POWER_BUFF_17_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 17 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_17_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                328800, new TagReference
//                {
//                    Id = 328800,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_UNAFFECTED_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Custom Target Unaffected Only",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_UNAFFECTED_ONLY",
//                }
//            },
//            {
//                270375, new TagReference
//                {
//                    Id = 270375,
//                    TagType = TagType.TAG_POWER_BUFF_27_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 27 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_27_EFFECT_GROUP",
//                }
//            },
//            {
//                270631, new TagReference
//                {
//                    Id = 270631,
//                    TagType = TagType.TAG_POWER_BUFF_27_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 27 Icon",
//                    InternalName = "TAG_POWER_BUFF_27_ICON",
//                }
//            },
//            {
//                328600, new TagReference
//                {
//                    Id = 328600,
//                    TagType = TagType.TAG_POWER_IGNORES_RANGE_ON_SHIFT_CLICK,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Ignores Range On Shift Click",
//                    InternalName = "TAG_POWER_IGNORES_RANGE_ON_SHIFT_CLICK",
//                }
//            },
//            {
//                270872, new TagReference
//                {
//                    Id = 270872,
//                    TagType = TagType.TAG_POWER_BUFF_18_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 18 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_18_HARMFUL_BUFF",
//                }
//            },
//            {
//                263203, new TagReference
//                {
//                    Id = 263203,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_2_RUNE_C,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 2 Rune C",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_2_RUNE_C",
//                }
//            },
//            {
//                271652, new TagReference
//                {
//                    Id = 271652,
//                    TagType = TagType.TAG_POWER_BUFF_24_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 24 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_24_IS_DISPLAYED",
//                }
//            },
//            {
//                271396, new TagReference
//                {
//                    Id = 271396,
//                    TagType = TagType.TAG_POWER_BUFF_24_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 24 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_24_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                712964, new TagReference
//                {
//                    Id = 712964,
//                    TagType = TagType.TAG_POWER_CONSOLE_AUTO_TARGET_SEARCH_RANGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Auto Target Search Range",
//                    InternalName = "TAG_POWER_CONSOLE_AUTO_TARGET_SEARCH_RANGE",
//                }
//            },
//            {
//                268064, new TagReference
//                {
//                    Id = 268064,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_62,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 62",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_62",
//                }
//            },
//            {
//                272161, new TagReference
//                {
//                    Id = 272161,
//                    TagType = TagType.TAG_POWER_BUFF_21_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 21 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_21_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                270369, new TagReference
//                {
//                    Id = 270369,
//                    TagType = TagType.TAG_POWER_BUFF_21_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 21 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_21_EFFECT_GROUP",
//                }
//            },
//            {
//                328368, new TagReference
//                {
//                    Id = 328368,
//                    TagType = TagType.TAG_POWER_ROLL_TO_DESTINATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "RollToDestination",
//                    InternalName = "TAG_POWER_ROLL_TO_DESTINATION",
//                }
//            },
//            {
//                329827, new TagReference
//                {
//                    Id = 329827,
//                    TagType = TagType.TAG_POWER_COMBO_ATTACK_SPEED_3,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Combo Attack Speed 3",
//                    InternalName = "TAG_POWER_COMBO_ATTACK_SPEED_3",
//                }
//            },
//            {
//                271650, new TagReference
//                {
//                    Id = 271650,
//                    TagType = TagType.TAG_POWER_BUFF_22_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 22 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_22_IS_DISPLAYED",
//                }
//            },
//            {
//                328620, new TagReference
//                {
//                    Id = 328620,
//                    TagType = TagType.TAG_POWER_CAST_TARGET_ALLOW_DEAD_TARGETS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CastTargetAllowDeadTargets",
//                    InternalName = "TAG_POWER_CAST_TARGET_ALLOW_DEAD_TARGETS",
//                }
//            },
//            {
//                721168, new TagReference
//                {
//                    Id = 721168,
//                    TagType = TagType.TAG_POWER_NORUNE_COMBO1_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "NoRune Combo1 Proc Scalar",
//                    InternalName = "TAG_POWER_NORUNE_COMBO1_PROC_SCALAR",
//                }
//            },
//            {
//                267376, new TagReference
//                {
//                    Id = 267376,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_37,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 37",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_37",
//                }
//            },
//            {
//                267120, new TagReference
//                {
//                    Id = 267120,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_27,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 27",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_27",
//                }
//            },
//            {
//                270640, new TagReference
//                {
//                    Id = 270640,
//                    TagType = TagType.TAG_POWER_BUFF_30_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 30 Icon",
//                    InternalName = "TAG_POWER_BUFF_30_ICON",
//                }
//            },
//            {
//                270852, new TagReference
//                {
//                    Id = 270852,
//                    TagType = TagType.TAG_POWER_BUFF_4_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 4 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_4_HARMFUL_BUFF",
//                }
//            },
//            {
//                328322, new TagReference
//                {
//                    Id = 328322,
//                    TagType = TagType.TAG_POWER_FAILS_IF_STUNNED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "FailsIfStunned",
//                    InternalName = "TAG_POWER_FAILS_IF_STUNNED",
//                }
//            },
//            {
//                270384, new TagReference
//                {
//                    Id = 270384,
//                    TagType = TagType.TAG_POWER_BUFF_30_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 30 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_30_EFFECT_GROUP",
//                }
//            },
//            {
//                332416, new TagReference
//                {
//                    Id = 332416,
//                    TagType = TagType.TAG_POWER_RUN_NEARBY_DISTANCE_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Run Nearby Distance Min",
//                    InternalName = "TAG_POWER_RUN_NEARBY_DISTANCE_MIN",
//                }
//            },
//            {
//                327937, new TagReference
//                {
//                    Id = 327937,
//                    TagType = TagType.TAG_POWER_CAN_STEER,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Can Steer",
//                    InternalName = "TAG_POWER_CAN_STEER",
//                }
//            },
//            {
//                270596, new TagReference
//                {
//                    Id = 270596,
//                    TagType = TagType.TAG_POWER_BUFF_4_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 4 Icon",
//                    InternalName = "TAG_POWER_BUFF_4_ICON",
//                }
//            },
//            {
//                263224, new TagReference
//                {
//                    Id = 263224,
//                    TagType = TagType.TAG_POWER_BURROW_WEAPON_CLASS_OVERRIDE,
//                    DataType = MapDataType.Unk32,
//                    DisplayName = "Burrow Weapon Class Override",
//                    InternalName = "TAG_POWER_BURROW_WEAPON_CLASS_OVERRIDE",
//                }
//            },
//            {
//                329520, new TagReference
//                {
//                    Id = 329520,
//                    TagType = TagType.TAG_POWER_AUTO_PURCHASE_LEVEL,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Auto Purchase Level",
//                    InternalName = "TAG_POWER_AUTO_PURCHASE_LEVEL",
//                }
//            },
//            {
//                272386, new TagReference
//                {
//                    Id = 272386,
//                    TagType = TagType.TAG_POWER_BUFF_2_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 2 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_2_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                264449, new TagReference
//                {
//                    Id = 264449,
//                    TagType = TagType.TAG_POWER_REFRESHES_COMBO_LEVEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "RefreshesComboLevel",
//                    InternalName = "TAG_POWER_REFRESHES_COMBO_LEVEL",
//                }
//            },
//            {
//                721268, new TagReference
//                {
//                    Id = 721268,
//                    TagType = TagType.TAG_POWER_RUNED_DAMAGE_TYPE,
//                    DataType = MapDataType.DamageType,
//                    DisplayName = "RuneD Damage Type",
//                    InternalName = "TAG_POWER_RUNED_DAMAGE_TYPE",
//                }
//            },
//            {
//                329664, new TagReference
//                {
//                    Id = 329664,
//                    TagType = TagType.TAG_POWER_COST_CONTINUAL_RESOURCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Continual Resource Cost",
//                    InternalName = "TAG_POWER_COST_CONTINUAL_RESOURCE",
//                }
//            },
//            {
//                328064, new TagReference
//                {
//                    Id = 328064,
//                    TagType = TagType.TAG_POWER_IS_HOTBAR_ASSIGNABLE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsHotbarAssignable",
//                    InternalName = "TAG_POWER_IS_HOTBAR_ASSIGNABLE",
//                }
//            },
//            {
//                327888, new TagReference
//                {
//                    Id = 327888,
//                    TagType = TagType.TAG_POWER_IS_AIMED_AT_GROUND,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsAimedAtGround",
//                    InternalName = "TAG_POWER_IS_AIMED_AT_GROUND",
//                }
//            },
//            {
//                328535, new TagReference
//                {
//                    Id = 328535,
//                    TagType = TagType.TAG_POWER_WALKING_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Min Walk Duration",
//                    InternalName = "TAG_POWER_WALKING_DURATION_MIN",
//                }
//            },
//            {
//                721153, new TagReference
//                {
//                    Id = 721153,
//                    TagType = TagType.TAG_POWER_RUNEA_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneA Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEA_PROC_SCALAR",
//                }
//            },
//            {
//                271127, new TagReference
//                {
//                    Id = 271127,
//                    TagType = TagType.TAG_POWER_BUFF_17_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 17 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_17_SHOW_DURATION",
//                }
//            },
//            {
//                328960, new TagReference
//                {
//                    Id = 328960,
//                    TagType = TagType.TAG_POWER_ITEM_TYPE_REQUIREMENT,
//                    DataType = MapDataType.Requirement,
//                    DisplayName = "Item Type Requirement",
//                    InternalName = "TAG_POWER_ITEM_TYPE_REQUIREMENT",
//                }
//            },
//            {
//                270857, new TagReference
//                {
//                    Id = 270857,
//                    TagType = TagType.TAG_POWER_BUFF_9_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 9 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_9_HARMFUL_BUFF",
//                }
//            },
//            {
//                271888, new TagReference
//                {
//                    Id = 271888,
//                    TagType = TagType.TAG_POWER_BUFF_10_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 10 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_10_MERGES_TOOLTIP",
//                }
//            },
//            {
//                263188, new TagReference
//                {
//                    Id = 263188,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_1_RUNE_D,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 1 Rune D",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_1_RUNE_D",
//                }
//            },
//            {
//                271113, new TagReference
//                {
//                    Id = 271113,
//                    TagType = TagType.TAG_POWER_BUFF_9_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 9 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_9_SHOW_DURATION",
//                }
//            },
//            {
//                271632, new TagReference
//                {
//                    Id = 271632,
//                    TagType = TagType.TAG_POWER_BUFF_10_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 10 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_10_IS_DISPLAYED",
//                }
//            },
//            {
//                272405, new TagReference
//                {
//                    Id = 272405,
//                    TagType = TagType.TAG_POWER_BUFF_21_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 21 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_21_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                268048, new TagReference
//                {
//                    Id = 268048,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_61,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 61",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_61",
//                }
//            },
//            {
//                328539, new TagReference
//                {
//                    Id = 328539,
//                    TagType = TagType.TAG_POWER_DELAY_BEFORE_SETTING_TARGET,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Delay Before Setting Target",
//                    InternalName = "TAG_POWER_DELAY_BEFORE_SETTING_TARGET",
//                }
//            },
//            {
//                271385, new TagReference
//                {
//                    Id = 271385,
//                    TagType = TagType.TAG_POWER_BUFF_19_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 19 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_19_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                274707, new TagReference
//                {
//                    Id = 274707,
//                    TagType = TagType.TAG_POWER_BUFF_19_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 19 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_19_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                271633, new TagReference
//                {
//                    Id = 271633,
//                    TagType = TagType.TAG_POWER_BUFF_11_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 11 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_11_IS_DISPLAYED",
//                }
//            },
//            {
//                328609, new TagReference
//                {
//                    Id = 328609,
//                    TagType = TagType.TAG_POWER_IS_INVULNERABLE_DURING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Is Invulnerable During",
//                    InternalName = "TAG_POWER_IS_INVULNERABLE_DURING",
//                }
//            },
//            {
//                271889, new TagReference
//                {
//                    Id = 271889,
//                    TagType = TagType.TAG_POWER_BUFF_11_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 11 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_11_MERGES_TOOLTIP",
//                }
//            },
//            {
//                329745, new TagReference
//                {
//                    Id = 329745,
//                    TagType = TagType.TAG_POWER_ESCAPE_ATTACK_ANGLE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Escape Attack Angle",
//                    InternalName = "TAG_POWER_ESCAPE_ATTACK_ANGLE",
//                }
//            },
//            {
//                721267, new TagReference
//                {
//                    Id = 721267,
//                    TagType = TagType.TAG_POWER_RUNEC_DAMAGE_TYPE,
//                    DataType = MapDataType.DamageType,
//                    DisplayName = "RuneC Damage Type",
//                    InternalName = "TAG_POWER_RUNEC_DAMAGE_TYPE",
//                }
//            },
//            {
//                328808, new TagReference
//                {
//                    Id = 328808,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_UNAFFECTED_ONLY_2,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Custom Target Unaffected Only 2",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_UNAFFECTED_ONLY_2",
//                }
//            },
//            {
//                328805, new TagReference
//                {
//                    Id = 328805,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_CLOSEST_IN_PIE_ANGLE,
//                    DataType = MapDataType.HighPrecision,
//                    DisplayName = "Custom Target Closest In Pie Angle",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_CLOSEST_IN_PIE_ANGLE",
//                }
//            },
//            {
//                271400, new TagReference
//                {
//                    Id = 271400,
//                    TagType = TagType.TAG_POWER_BUFF_28_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 28 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_28_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                712962, new TagReference
//                {
//                    Id = 712962,
//                    TagType = TagType.TAG_POWER_CONSOLE_TARGET_NUM_GROWS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "",
//                    InternalName = "TAG_POWER_CONSOLE_TARGET_NUM_GROWS",
//                }
//            },
//            {
//                271144, new TagReference
//                {
//                    Id = 271144,
//                    TagType = TagType.TAG_POWER_BUFF_28_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 28 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_28_SHOW_DURATION",
//                }
//            },
//            {
//                328608, new TagReference
//                {
//                    Id = 328608,
//                    TagType = TagType.TAG_POWER_IS_COMPLETED_WHEN_WALKING_STOPS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Is Completed When Walking Stops",
//                    InternalName = "TAG_POWER_IS_COMPLETED_WHEN_WALKING_STOPS",
//                }
//            },
//            {
//                270880, new TagReference
//                {
//                    Id = 270880,
//                    TagType = TagType.TAG_POWER_BUFF_20_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 20 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_20_HARMFUL_BUFF",
//                }
//            },
//            {
//                622596, new TagReference
//                {
//                    Id = 622596,
//                    TagType = TagType.TAG_POWER_CONTROLLER_MAX_RANGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Max Range When Held",
//                    InternalName = "TAG_POWER_CONTROLLER_MAX_RANGE",
//                }
//            },
//            {
//                328240, new TagReference
//                {
//                    Id = 328240,
//                    TagType = TagType.TAG_POWER_REQUIRES_ACTOR_TARGET,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "RequiresActorTarget",
//                    InternalName = "TAG_POWER_REQUIRES_ACTOR_TARGET",
//                }
//            },
//            {
//                263222, new TagReference
//                {
//                    Id = 263222,
//                    TagType = TagType.TAG_POWER_ANIMATION_MIN_SCALING,
//                    DataType = MapDataType.HighPrecision,
//                    DisplayName = "Scaled Animation Timing Min",
//                    InternalName = "TAG_POWER_ANIMATION_MIN_SCALING",
//                }
//            },
//            {
//                263217, new TagReference
//                {
//                    Id = 263217,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_3_RUNE_A,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 3 Rune A",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_3_RUNE_A",
//                }
//            },
//            {
//                328384, new TagReference
//                {
//                    Id = 328384,
//                    TagType = TagType.TAG_POWER_TURNS_INTO_BASIC_ATTACK,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TurnsIntoBasicAttack",
//                    InternalName = "TAG_POWER_TURNS_INTO_BASIC_ATTACK",
//                }
//            },
//            {
//                271620, new TagReference
//                {
//                    Id = 271620,
//                    TagType = TagType.TAG_POWER_BUFF_4_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 4 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_4_IS_DISPLAYED",
//                }
//            },
//            {
//                721312, new TagReference
//                {
//                    Id = 721312,
//                    TagType = TagType.TAG_POWER_TELEPORT_NEAR_TARGET_DIST_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Teleport Near Target Dist Min",
//                    InternalName = "TAG_POWER_TELEPORT_NEAR_TARGET_DIST_MIN",
//                }
//            },
//            {
//                271364, new TagReference
//                {
//                    Id = 271364,
//                    TagType = TagType.TAG_POWER_BUFF_4_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 4 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_4_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                274689, new TagReference
//                {
//                    Id = 274689,
//                    TagType = TagType.TAG_POWER_BUFF_1_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 1 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_1_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                272129, new TagReference
//                {
//                    Id = 272129,
//                    TagType = TagType.TAG_POWER_BUFF_1_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 1 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_1_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                270337, new TagReference
//                {
//                    Id = 270337,
//                    TagType = TagType.TAG_POWER_BUFF_1_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 1 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_1_EFFECT_GROUP",
//                }
//            },
//            {
//                271618, new TagReference
//                {
//                    Id = 271618,
//                    TagType = TagType.TAG_POWER_BUFF_2_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 2 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_2_IS_DISPLAYED",
//                }
//            },
//            {
//                263425, new TagReference
//                {
//                    Id = 263425,
//                    TagType = TagType.TAG_POWER_ANIMATION_TAG_TURN_RIGHT,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Animation Tag Turn Right",
//                    InternalName = "TAG_POWER_ANIMATION_TAG_TURN_RIGHT",
//                }
//            },
//            {
//                267904, new TagReference
//                {
//                    Id = 267904,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_58,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 58",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_58",
//                }
//            },
//            {
//                267648, new TagReference
//                {
//                    Id = 267648,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_48,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 48",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_48",
//                }
//            },
//            {
//                270854, new TagReference
//                {
//                    Id = 270854,
//                    TagType = TagType.TAG_POWER_BUFF_6_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 6 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_6_HARMFUL_BUFF",
//                }
//            },
//            {
//                327685, new TagReference
//                {
//                    Id = 327685,
//                    TagType = TagType.TAG_POWER_TEMPLATE_RUNE_E,
//                    DataType = MapDataType.Template,
//                    DisplayName = "Template Rune E",
//                    InternalName = "TAG_POWER_TEMPLATE_RUNE_E",
//                }
//            },
//            {
//                328642, new TagReference
//                {
//                    Id = 328642,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_B,
//                    DataType = MapDataType.PowerSnoId2,
//                    DisplayName = "Special Death Chance Rune B",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_B",
//                }
//            },
//            {
//                274446, new TagReference
//                {
//                    Id = 274446,
//                    TagType = TagType.TAG_POWER_BUFF_14_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 14 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_14_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                271622, new TagReference
//                {
//                    Id = 271622,
//                    TagType = TagType.TAG_POWER_BUFF_6_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 6 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_6_IS_DISPLAYED",
//                }
//            },
//            {
//                328081, new TagReference
//                {
//                    Id = 328081,
//                    TagType = TagType.TAG_POWER_IS_USABLE_IN_COMBAT,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsUsableInCombat",
//                    InternalName = "TAG_POWER_IS_USABLE_IN_COMBAT",
//                }
//            },
//            {
//                270851, new TagReference
//                {
//                    Id = 270851,
//                    TagType = TagType.TAG_POWER_BUFF_3_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 3 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_3_HARMFUL_BUFF",
//                }
//            },
//            {
//                329536, new TagReference
//                {
//                    Id = 329536,
//                    TagType = TagType.TAG_POWER_ALWAYS_KNOWN,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Always Known",
//                    InternalName = "TAG_POWER_ALWAYS_KNOWN",
//                }
//            },
//            {
//                327936, new TagReference
//                {
//                    Id = 327936,
//                    TagType = TagType.TAG_POWER_TURNS_INTO_WALK,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TurnsIntoWalk",
//                    InternalName = "TAG_POWER_TURNS_INTO_WALK",
//                }
//            },
//            {
//                272650, new TagReference
//                {
//                    Id = 272650,
//                    TagType = TagType.TAG_POWER_BUFF_10_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 10 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_10_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                327760, new TagReference
//                {
//                    Id = 327760,
//                    TagType = TagType.TAG_POWER_CHILD_POWER,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Child Power",
//                    InternalName = "TAG_POWER_CHILD_POWER",
//                }
//            },
//            {
//                270608, new TagReference
//                {
//                    Id = 270608,
//                    TagType = TagType.TAG_POWER_BUFF_10_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 10 Icon",
//                    InternalName = "TAG_POWER_BUFF_10_ICON",
//                }
//            },
//            {
//                272393, new TagReference
//                {
//                    Id = 272393,
//                    TagType = TagType.TAG_POWER_BUFF_9_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 9 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_9_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                270352, new TagReference
//                {
//                    Id = 270352,
//                    TagType = TagType.TAG_POWER_BUFF_10_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 10 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_10_EFFECT_GROUP",
//                }
//            },
//            {
//                274696, new TagReference
//                {
//                    Id = 274696,
//                    TagType = TagType.TAG_POWER_BUFF_8_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 8 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_8_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                272414, new TagReference
//                {
//                    Id = 272414,
//                    TagType = TagType.TAG_POWER_BUFF_30_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 30 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_30_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                274440, new TagReference
//                {
//                    Id = 274440,
//                    TagType = TagType.TAG_POWER_BUFF_8_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 8 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_8_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                270865, new TagReference
//                {
//                    Id = 270865,
//                    TagType = TagType.TAG_POWER_BUFF_11_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 11 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_11_HARMFUL_BUFF",
//                }
//            },
//            {
//                272406, new TagReference
//                {
//                    Id = 272406,
//                    TagType = TagType.TAG_POWER_BUFF_22_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 22 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_22_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                274714, new TagReference
//                {
//                    Id = 274714,
//                    TagType = TagType.TAG_POWER_BUFF_26_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 26 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_26_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                271121, new TagReference
//                {
//                    Id = 271121,
//                    TagType = TagType.TAG_POWER_BUFF_11_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 11 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_11_SHOW_DURATION",
//                }
//            },
//            {
//                328806, new TagReference
//                {
//                    Id = 328806,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_BUFF_SNO_2,
//                    DataType = MapDataType.PowerSno2,
//                    DisplayName = "Custom Target Buff Power SNO 2",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_BUFF_SNO_2",
//                }
//            },
//            {
//                274713, new TagReference
//                {
//                    Id = 274713,
//                    TagType = TagType.TAG_POWER_BUFF_25_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 25 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_25_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                271890, new TagReference
//                {
//                    Id = 271890,
//                    TagType = TagType.TAG_POWER_BUFF_12_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 12 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_12_MERGES_TOOLTIP",
//                }
//            },
//            {
//                270610, new TagReference
//                {
//                    Id = 270610,
//                    TagType = TagType.TAG_POWER_BUFF_12_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 12 Icon",
//                    InternalName = "TAG_POWER_BUFF_12_ICON",
//                }
//            },
//            {
//                271398, new TagReference
//                {
//                    Id = 271398,
//                    TagType = TagType.TAG_POWER_BUFF_26_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 26 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_26_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                272664, new TagReference
//                {
//                    Id = 272664,
//                    TagType = TagType.TAG_POWER_BUFF_24_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 24 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_24_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                721186, new TagReference
//                {
//                    Id = 721186,
//                    TagType = TagType.TAG_POWER_RUNEA_COMBO3_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneA Combo3 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEA_COMBO3_PROC_SCALAR",
//                }
//            },
//            {
//                271139, new TagReference
//                {
//                    Id = 271139,
//                    TagType = TagType.TAG_POWER_BUFF_23_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 23 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_23_SHOW_DURATION",
//                }
//            },
//            {
//                328617, new TagReference
//                {
//                    Id = 328617,
//                    TagType = TagType.TAG_POWER_CAST_TARGET_NORMAL_MONSTERS_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CastTargetNormalMonstersOnly",
//                    InternalName = "TAG_POWER_CAST_TARGET_NORMAL_MONSTERS_ONLY",
//                }
//            },
//            {
//                272408, new TagReference
//                {
//                    Id = 272408,
//                    TagType = TagType.TAG_POWER_BUFF_24_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 24 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_24_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                270371, new TagReference
//                {
//                    Id = 270371,
//                    TagType = TagType.TAG_POWER_BUFF_23_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 23 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_23_EFFECT_GROUP",
//                }
//            },
//            {
//                329829, new TagReference
//                {
//                    Id = 329829,
//                    TagType = TagType.TAG_POWER_COMBO_LEVEL_2_ON_HIT_COEFFICIENT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Combo Level 2 On Hit Proc Coefficient",
//                    InternalName = "TAG_POWER_COMBO_LEVEL_2_ON_HIT_COEFFICIENT",
//                }
//            },
//            {
//                271141, new TagReference
//                {
//                    Id = 271141,
//                    TagType = TagType.TAG_POWER_BUFF_25_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 25 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_25_SHOW_DURATION",
//                }
//            },
//            {
//                713218, new TagReference
//                {
//                    Id = 713218,
//                    TagType = TagType.TAG_POWER_CONSOLE_LT_FIND_TARGET_IF_LOCKED_TARGET_OUT_OF_RANGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller LT Finds Target If Locked Target Out Of Range",
//                    InternalName = "TAG_POWER_CONSOLE_LT_FIND_TARGET_IF_LOCKED_TARGET_OUT_OF_RANGE",
//                }
//            },
//            {
//                271397, new TagReference
//                {
//                    Id = 271397,
//                    TagType = TagType.TAG_POWER_BUFF_25_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 25 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_25_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                274459, new TagReference
//                {
//                    Id = 274459,
//                    TagType = TagType.TAG_POWER_BUFF_27_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 27 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_27_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                266848, new TagReference
//                {
//                    Id = 266848,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_16,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 16",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_16",
//                }
//            },
//            {
//                328501, new TagReference
//                {
//                    Id = 328501,
//                    TagType = TagType.TAG_POWER_ALTERNATES_ANIMS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Alternates Anims",
//                    InternalName = "TAG_POWER_ALTERNATES_ANIMS",
//                }
//            },
//            {
//                328162, new TagReference
//                {
//                    Id = 328162,
//                    TagType = TagType.TAG_POWER_TARGET_CONTACT_PLANE_ONLY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "TargetContactPlaneOnly",
//                    InternalName = "TAG_POWER_TARGET_CONTACT_PLANE_ONLY",
//                }
//            },
//            {
//                328480, new TagReference
//                {
//                    Id = 328480,
//                    TagType = TagType.TAG_POWER_IS_DODGE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Is A Dodge Power",
//                    InternalName = "TAG_POWER_IS_DODGE",
//                }
//            },
//            {
//                328645, new TagReference
//                {
//                    Id = 328645,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_C,
//                    DataType = MapDataType.Unk90,
//                    DisplayName = "Special Death Type Rune C",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_C",
//                }
//            },
//            {
//                328616, new TagReference
//                {
//                    Id = 328616,
//                    TagType = TagType.TAG_POWER_CAST_TARGET_IGNORE_LARGE_MONSTERS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CastTargetIgnoreLargeMonsters",
//                    InternalName = "TAG_POWER_CAST_TARGET_IGNORE_LARGE_MONSTERS",
//                }
//            },
//            {
//                263219, new TagReference
//                {
//                    Id = 263219,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_3_RUNE_C,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 3 Rune C",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_3_RUNE_C",
//                }
//            },
//            {
//                328248, new TagReference
//                {
//                    Id = 328248,
//                    TagType = TagType.TAG_POWER_REQUIRES_SKILLPOINT,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "RequiresSkillPoint",
//                    InternalName = "TAG_POWER_REQUIRES_SKILLPOINT",
//                }
//            },
//            {
//                272388, new TagReference
//                {
//                    Id = 272388,
//                    TagType = TagType.TAG_POWER_BUFF_4_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 4 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_4_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                278528, new TagReference
//                {
//                    Id = 278528,
//                    TagType = TagType.TAG_POWER_REUSE_SCRIPT_STATE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "ReuseScriptState",
//                    InternalName = "TAG_POWER_REUSE_SCRIPT_STATE",
//                }
//            },
//            {
//                328256, new TagReference
//                {
//                    Id = 328256,
//                    TagType = TagType.TAG_POWER_DONT_WALK_CLOSER_IF_OUT_OF_RANGE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "DontWalkCloserIfOutOfRange",
//                    InternalName = "TAG_POWER_DONT_WALK_CLOSER_IF_OUT_OF_RANGE",
//                }
//            },
//            {
//                332864, new TagReference
//                {
//                    Id = 332864,
//                    TagType = TagType.TAG_POWER_AI_ACTION_DURATION_MIN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "AI Action Duration Min",
//                    InternalName = "TAG_POWER_AI_ACTION_DURATION_MIN",
//                }
//            },
//            {
//                622593, new TagReference
//                {
//                    Id = 622593,
//                    TagType = TagType.TAG_POWER_CONTROLLER_MIN_RANGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Min Range",
//                    InternalName = "TAG_POWER_CONTROLLER_MIN_RANGE",
//                }
//            },
//            {
//                721184, new TagReference
//                {
//                    Id = 721184,
//                    TagType = TagType.TAG_POWER_RUNEA_COMBO1_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneA Combo1 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEA_COMBO1_PROC_SCALAR",
//                }
//            },
//            {
//                270848, new TagReference
//                {
//                    Id = 270848,
//                    TagType = TagType.TAG_POWER_BUFF_0_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 0 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_0_HARMFUL_BUFF",
//                }
//            },
//            {
//                328644, new TagReference
//                {
//                    Id = 328644,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_C,
//                    DataType = MapDataType.PowerSnoId2,
//                    DisplayName = "Special Death Chance Rune C",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_C",
//                }
//            },
//            {
//                266624, new TagReference
//                {
//                    Id = 266624,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_8,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 8",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_8",
//                }
//            },
//            {
//                272653, new TagReference
//                {
//                    Id = 272653,
//                    TagType = TagType.TAG_POWER_BUFF_13_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 13 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_13_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                262913, new TagReference
//                {
//                    Id = 262913,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_2,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 2",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_2",
//                }
//            },
//            {
//                328646, new TagReference
//                {
//                    Id = 328646,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_D,
//                    DataType = MapDataType.PowerSnoId2,
//                    DisplayName = "Special Death Chance Rune D",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_D",
//                }
//            },
//            {
//                271623, new TagReference
//                {
//                    Id = 271623,
//                    TagType = TagType.TAG_POWER_BUFF_7_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 7 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_7_IS_DISPLAYED",
//                }
//            },
//            {
//                267776, new TagReference
//                {
//                    Id = 267776,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_50,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 50",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_50",
//                }
//            },
//            {
//                270855, new TagReference
//                {
//                    Id = 270855,
//                    TagType = TagType.TAG_POWER_BUFF_7_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 7 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_7_HARMFUL_BUFF",
//                }
//            },
//            {
//                267520, new TagReference
//                {
//                    Id = 267520,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_40,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 40",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_40",
//                }
//            },
//            {
//                274444, new TagReference
//                {
//                    Id = 274444,
//                    TagType = TagType.TAG_POWER_BUFF_12_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 12 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_12_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                272407, new TagReference
//                {
//                    Id = 272407,
//                    TagType = TagType.TAG_POWER_BUFF_23_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 23 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_23_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                270868, new TagReference
//                {
//                    Id = 270868,
//                    TagType = TagType.TAG_POWER_BUFF_14_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 14 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_14_HARMFUL_BUFF",
//                }
//            },
//            {
//                272663, new TagReference
//                {
//                    Id = 272663,
//                    TagType = TagType.TAG_POWER_BUFF_23_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 23 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_23_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                329810, new TagReference
//                {
//                    Id = 329810,
//                    TagType = TagType.TAG_POWER_COMBO_ATTACK_RADIUS_2,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Combo Attack Radius 2",
//                    InternalName = "TAG_POWER_COMBO_ATTACK_RADIUS_2",
//                }
//            },
//            {
//                327953, new TagReference
//                {
//                    Id = 327953,
//                    TagType = TagType.TAG_POWER_CANNOT_OVERKILL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Cannot Overkill",
//                    InternalName = "TAG_POWER_CANNOT_OVERKILL",
//                }
//            },
//            {
//                270612, new TagReference
//                {
//                    Id = 270612,
//                    TagType = TagType.TAG_POWER_BUFF_14_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 14 Icon",
//                    InternalName = "TAG_POWER_BUFF_14_ICON",
//                }
//            },
//            {
//                272399, new TagReference
//                {
//                    Id = 272399,
//                    TagType = TagType.TAG_POWER_BUFF_15_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 15 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_15_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                274717, new TagReference
//                {
//                    Id = 274717,
//                    TagType = TagType.TAG_POWER_BUFF_29_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 29 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_29_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                263189, new TagReference
//                {
//                    Id = 263189,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_1_RUNE_E,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 1 Rune E",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_1_RUNE_E",
//                }
//            },
//            {
//                272655, new TagReference
//                {
//                    Id = 272655,
//                    TagType = TagType.TAG_POWER_BUFF_15_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 15 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_15_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                266832, new TagReference
//                {
//                    Id = 266832,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_15,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 15",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_15",
//                }
//            },
//            {
//                327829, new TagReference
//                {
//                    Id = 327829,
//                    TagType = TagType.TAG_POWER_DISPLAYS_NO_DAMAGE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "DisplaysNoDamage",
//                    InternalName = "TAG_POWER_DISPLAYS_NO_DAMAGE",
//                }
//            },
//            {
//                270613, new TagReference
//                {
//                    Id = 270613,
//                    TagType = TagType.TAG_POWER_BUFF_15_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 15 Icon",
//                    InternalName = "TAG_POWER_BUFF_15_ICON",
//                }
//            },
//            {
//                327768, new TagReference
//                {
//                    Id = 327768,
//                    TagType = TagType.TAG_POWER_COOLDOWN,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Cooldown Time",
//                    InternalName = "TAG_POWER_COOLDOWN",
//                }
//            },
//            {
//                270869, new TagReference
//                {
//                    Id = 270869,
//                    TagType = TagType.TAG_POWER_BUFF_15_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 15 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_15_HARMFUL_BUFF",
//                }
//            },
//            {
//                274453, new TagReference
//                {
//                    Id = 274453,
//                    TagType = TagType.TAG_POWER_BUFF_21_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 21 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_21_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                712961, new TagReference
//                {
//                    Id = 712961,
//                    TagType = TagType.TAG_POWER_CONSOLE_TARGET_DIST_GROWTH_PER_KILL,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "",
//                    InternalName = "TAG_POWER_CONSOLE_TARGET_DIST_GROWTH_PER_KILL",
//                }
//            },
//            {
//                721156, new TagReference
//                {
//                    Id = 721156,
//                    TagType = TagType.TAG_POWER_RUNED_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneD Proc Scalar",
//                    InternalName = "TAG_POWER_RUNED_PROC_SCALAR",
//                }
//            },
//            {
//                329680, new TagReference
//                {
//                    Id = 329680,
//                    TagType = TagType.TAG_POWER_DONT_ALLOW_COOLDOWN_REDUCTION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Dont Allow Cooldown Reduction",
//                    InternalName = "TAG_POWER_DONT_ALLOW_COOLDOWN_REDUCTION",
//                }
//            },
//            {
//                328080, new TagReference
//                {
//                    Id = 328080,
//                    TagType = TagType.TAG_POWER_IS_USABLE_IN_TOWN,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsUsableInTown",
//                    InternalName = "TAG_POWER_IS_USABLE_IN_TOWN",
//                }
//            },
//            {
//                721169, new TagReference
//                {
//                    Id = 721169,
//                    TagType = TagType.TAG_POWER_NORUNE_COMBO2_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "NoRune Combo2 Proc Scalar",
//                    InternalName = "TAG_POWER_NORUNE_COMBO2_PROC_SCALAR",
//                }
//            },
//            {
//                329312, new TagReference
//                {
//                    Id = 329312,
//                    TagType = TagType.TAG_POWER_SKILL_POINT_COST,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Skill Point Cost",
//                    InternalName = "TAG_POWER_SKILL_POINT_COST",
//                }
//            },
//            {
//                274463, new TagReference
//                {
//                    Id = 274463,
//                    TagType = TagType.TAG_POWER_BUFF_31_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 31 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_31_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                274719, new TagReference
//                {
//                    Id = 274719,
//                    TagType = TagType.TAG_POWER_BUFF_31_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 31 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_31_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                271143, new TagReference
//                {
//                    Id = 271143,
//                    TagType = TagType.TAG_POWER_BUFF_27_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 27 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_27_SHOW_DURATION",
//                }
//            },
//            {
//                328976, new TagReference
//                {
//                    Id = 328976,
//                    TagType = TagType.TAG_POWER_REQUIRES_1H_ITEM,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Requires 1H Item",
//                    InternalName = "TAG_POWER_REQUIRES_1H_ITEM",
//                }
//            },
//            {
//                271395, new TagReference
//                {
//                    Id = 271395,
//                    TagType = TagType.TAG_POWER_BUFF_23_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 23 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_23_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                263204, new TagReference
//                {
//                    Id = 263204,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_2_RUNE_D,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 2 Rune D",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_2_RUNE_D",
//                }
//            },
//            {
//                271651, new TagReference
//                {
//                    Id = 271651,
//                    TagType = TagType.TAG_POWER_BUFF_23_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 23 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_23_IS_DISPLAYED",
//                }
//            },
//            {
//                271908, new TagReference
//                {
//                    Id = 271908,
//                    TagType = TagType.TAG_POWER_BUFF_24_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 24 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_24_MERGES_TOOLTIP",
//                }
//            },
//            {
//                271401, new TagReference
//                {
//                    Id = 271401,
//                    TagType = TagType.TAG_POWER_BUFF_29_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 29 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_29_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                271649, new TagReference
//                {
//                    Id = 271649,
//                    TagType = TagType.TAG_POWER_BUFF_21_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 21 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_21_IS_DISPLAYED",
//                }
//            },
//            {
//                271905, new TagReference
//                {
//                    Id = 271905,
//                    TagType = TagType.TAG_POWER_BUFF_21_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 21 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_21_MERGES_TOOLTIP",
//                }
//            },
//            {
//                721155, new TagReference
//                {
//                    Id = 721155,
//                    TagType = TagType.TAG_POWER_RUNEC_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneC Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEC_PROC_SCALAR",
//                }
//            },
//            {
//                713216, new TagReference
//                {
//                    Id = 713216,
//                    TagType = TagType.TAG_POWER_CONSOLE_AUTO_TARGET_ENABLED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Auto Targeting Enabled",
//                    InternalName = "TAG_POWER_CONSOLE_AUTO_TARGET_ENABLED",
//                }
//            },
//            {
//                267888, new TagReference
//                {
//                    Id = 267888,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_57,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 57",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_57",
//                }
//            },
//            {
//                267632, new TagReference
//                {
//                    Id = 267632,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_47,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 47",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_47",
//                }
//            },
//            {
//                717056, new TagReference
//                {
//                    Id = 717056,
//                    TagType = TagType.TAG_POWER_PERSISTS_ON_WORLD_DELETE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Persists on World Delete",
//                    InternalName = "TAG_POWER_PERSISTS_ON_WORLD_DELETE",
//                }
//            },
//            {
//                271109, new TagReference
//                {
//                    Id = 271109,
//                    TagType = TagType.TAG_POWER_BUFF_5_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 5 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_5_SHOW_DURATION",
//                }
//            },
//            {
//                271365, new TagReference
//                {
//                    Id = 271365,
//                    TagType = TagType.TAG_POWER_BUFF_5_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 5 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_5_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                270896, new TagReference
//                {
//                    Id = 270896,
//                    TagType = TagType.TAG_POWER_BUFF_30_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 30 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_30_HARMFUL_BUFF",
//                }
//            },
//            {
//                272641, new TagReference
//                {
//                    Id = 272641,
//                    TagType = TagType.TAG_POWER_BUFF_1_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 1 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_1_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                266496, new TagReference
//                {
//                    Id = 266496,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_0,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 0",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_0",
//                }
//            },
//            {
//                328400, new TagReference
//                {
//                    Id = 328400,
//                    TagType = TagType.TAG_POWER_IS_CHANNELLED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "IsChannelled",
//                    InternalName = "TAG_POWER_IS_CHANNELLED",
//                }
//            },
//            {
//                271636, new TagReference
//                {
//                    Id = 271636,
//                    TagType = TagType.TAG_POWER_BUFF_14_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 14 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_14_IS_DISPLAYED",
//                }
//            },
//            {
//                271380, new TagReference
//                {
//                    Id = 271380,
//                    TagType = TagType.TAG_POWER_BUFF_14_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 14 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_14_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                327681, new TagReference
//                {
//                    Id = 327681,
//                    TagType = TagType.TAG_POWER_TEMPLATE_RUNE_A,
//                    DataType = MapDataType.Template,
//                    DisplayName = "Template Rune A",
//                    InternalName = "TAG_POWER_TEMPLATE_RUNE_A",
//                }
//            },
//            {
//                274442, new TagReference
//                {
//                    Id = 274442,
//                    TagType = TagType.TAG_POWER_BUFF_10_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 10 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_10_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                267408, new TagReference
//                {
//                    Id = 267408,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_39,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 39",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_39",
//                }
//            },
//            {
//                267152, new TagReference
//                {
//                    Id = 267152,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_29,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 29",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_29",
//                }
//            },
//            {
//                270601, new TagReference
//                {
//                    Id = 270601,
//                    TagType = TagType.TAG_POWER_BUFF_9_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 9 Icon",
//                    InternalName = "TAG_POWER_BUFF_9_ICON",
//                }
//            },
//            {
//                272136, new TagReference
//                {
//                    Id = 272136,
//                    TagType = TagType.TAG_POWER_BUFF_8_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 8 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_8_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                271893, new TagReference
//                {
//                    Id = 271893,
//                    TagType = TagType.TAG_POWER_BUFF_15_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 15 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_15_MERGES_TOOLTIP",
//                }
//            },
//            {
//                328420, new TagReference
//                {
//                    Id = 328420,
//                    TagType = TagType.TAG_POWER_LOCKS_ACTORS_WHILE_SWEEPING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "LocksActorsWhileSweeping",
//                    InternalName = "TAG_POWER_LOCKS_ACTORS_WHILE_SWEEPING",
//                }
//            },
//            {
//                272149, new TagReference
//                {
//                    Id = 272149,
//                    TagType = TagType.TAG_POWER_BUFF_15_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 15 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_15_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                328084, new TagReference
//                {
//                    Id = 328084,
//                    TagType = TagType.TAG_POWER_IS_USABLE_IN_PVP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsUsableInPVP",
//                    InternalName = "TAG_POWER_IS_USABLE_IN_PVP",
//                }
//            },
//            {
//                270870, new TagReference
//                {
//                    Id = 270870,
//                    TagType = TagType.TAG_POWER_BUFF_16_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 16 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_16_HARMFUL_BUFF",
//                }
//            },
//            {
//                271638, new TagReference
//                {
//                    Id = 271638,
//                    TagType = TagType.TAG_POWER_BUFF_16_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 16 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_16_IS_DISPLAYED",
//                }
//            },
//            {
//                274451, new TagReference
//                {
//                    Id = 274451,
//                    TagType = TagType.TAG_POWER_BUFF_19_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 19 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_19_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                328097, new TagReference
//                {
//                    Id = 328097,
//                    TagType = TagType.TAG_POWER_CAST_TARGET_ALLIES,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CastTargetAllies",
//                    InternalName = "TAG_POWER_CAST_TARGET_ALLIES",
//                }
//            },
//            {
//                270867, new TagReference
//                {
//                    Id = 270867,
//                    TagType = TagType.TAG_POWER_BUFF_13_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 13 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_13_HARMFUL_BUFF",
//                }
//            },
//            {
//                327952, new TagReference
//                {
//                    Id = 327952,
//                    TagType = TagType.TAG_POWER_IS_TOGGLEABLE,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsToggleable",
//                    InternalName = "TAG_POWER_IS_TOGGLEABLE",
//                }
//            },
//            {
//                272666, new TagReference
//                {
//                    Id = 272666,
//                    TagType = TagType.TAG_POWER_BUFF_26_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 26 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_26_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                327776, new TagReference
//                {
//                    Id = 327776,
//                    TagType = TagType.TAG_POWER_IS_PRIMARY,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsPrimary",
//                    InternalName = "TAG_POWER_IS_PRIMARY",
//                }
//            },
//            {
//                272667, new TagReference
//                {
//                    Id = 272667,
//                    TagType = TagType.TAG_POWER_BUFF_27_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 27 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_27_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                681984, new TagReference
//                {
//                    Id = 681984,
//                    TagType = TagType.TAG_POWER_BREAKS_ROOT,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Breaks Root",
//                    InternalName = "TAG_POWER_BREAKS_ROOT",
//                }
//            },
//            {
//                264288, new TagReference
//                {
//                    Id = 264288,
//                    TagType = TagType.TAG_POWER_COMBO_CASTING_EFFECT_GROUP_0,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 0 Casting Effect Group - Male",
//                    InternalName = "TAG_POWER_COMBO_CASTING_EFFECT_GROUP_0",
//                }
//            },
//            {
//                721313, new TagReference
//                {
//                    Id = 721313,
//                    TagType = TagType.TAG_POWER_TELEPORT_NEAR_TARGET_DIST_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Teleport Near Target Dist Delta",
//                    InternalName = "TAG_POWER_TELEPORT_NEAR_TARGET_DIST_DELTA",
//                }
//            },
//            {
//                267296, new TagReference
//                {
//                    Id = 267296,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_32,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 32",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_32",
//                }
//            },
//            {
//                270881, new TagReference
//                {
//                    Id = 270881,
//                    TagType = TagType.TAG_POWER_BUFF_21_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 21 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_21_HARMFUL_BUFF",
//                }
//            },
//            {
//                271912, new TagReference
//                {
//                    Id = 271912,
//                    TagType = TagType.TAG_POWER_BUFF_28_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 28 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_28_MERGES_TOOLTIP",
//                }
//            },
//            {
//                267040, new TagReference
//                {
//                    Id = 267040,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_22,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 22",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_22",
//                }
//            },
//            {
//                271137, new TagReference
//                {
//                    Id = 271137,
//                    TagType = TagType.TAG_POWER_BUFF_21_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 21 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_21_SHOW_DURATION",
//                }
//            },
//            {
//                271656, new TagReference
//                {
//                    Id = 271656,
//                    TagType = TagType.TAG_POWER_BUFF_28_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 28 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_28_IS_DISPLAYED",
//                }
//            },
//            {
//                271906, new TagReference
//                {
//                    Id = 271906,
//                    TagType = TagType.TAG_POWER_BUFF_22_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 22 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_22_MERGES_TOOLTIP",
//                }
//            },
//            {
//                270626, new TagReference
//                {
//                    Id = 270626,
//                    TagType = TagType.TAG_POWER_BUFF_22_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 22 Icon",
//                    InternalName = "TAG_POWER_BUFF_22_ICON",
//                }
//            },
//            {
//                271409, new TagReference
//                {
//                    Id = 271409,
//                    TagType = TagType.TAG_POWER_BUFF_31_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 31 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_31_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                266608, new TagReference
//                {
//                    Id = 266608,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_7,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 7",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_7",
//                }
//            },
//            {
//                721202, new TagReference
//                {
//                    Id = 721202,
//                    TagType = TagType.TAG_POWER_RUNEB_COMBO3_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneB Combo3 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEB_COMBO3_PROC_SCALAR",
//                }
//            },
//            {
//                328641, new TagReference
//                {
//                    Id = 328641,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_A,
//                    DataType = MapDataType.Unk90,
//                    DisplayName = "Special Death Type Rune A",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_A",
//                }
//            },
//            {
//                272389, new TagReference
//                {
//                    Id = 272389,
//                    TagType = TagType.TAG_POWER_BUFF_5_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 5 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_5_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                271876, new TagReference
//                {
//                    Id = 271876,
//                    TagType = TagType.TAG_POWER_BUFF_4_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 4 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_4_MERGES_TOOLTIP",
//                }
//            },
//            {
//                328704, new TagReference
//                {
//                    Id = 328704,
//                    TagType = TagType.TAG_POWER_BRAIN_ACTION_TYPE,
//                    DataType = MapDataType.BrainAction,
//                    DisplayName = "Brain Action Type",
//                    InternalName = "TAG_POWER_BRAIN_ACTION_TYPE",
//                }
//            },
//            {
//                274436, new TagReference
//                {
//                    Id = 274436,
//                    TagType = TagType.TAG_POWER_BUFF_4_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 4 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_4_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                271617, new TagReference
//                {
//                    Id = 271617,
//                    TagType = TagType.TAG_POWER_BUFF_1_IS_DISPLAYED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 1 Is Displayed",
//                    InternalName = "TAG_POWER_BUFF_1_IS_DISPLAYED",
//                }
//            },
//            {
//                328496, new TagReference
//                {
//                    Id = 328496,
//                    TagType = TagType.TAG_POWER_SNAPS_TO_GROUND,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Snaps To Ground",
//                    InternalName = "TAG_POWER_SNAPS_TO_GROUND",
//                }
//            },
//            {
//                271873, new TagReference
//                {
//                    Id = 271873,
//                    TagType = TagType.TAG_POWER_BUFF_1_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 1 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_1_MERGES_TOOLTIP",
//                }
//            },
//            {
//                327682, new TagReference
//                {
//                    Id = 327682,
//                    TagType = TagType.TAG_POWER_TEMPLATE_RUNE_B,
//                    DataType = MapDataType.Template,
//                    DisplayName = "Template Rune B",
//                    InternalName = "TAG_POWER_TEMPLATE_RUNE_B",
//                }
//            },
//            {
//                263298, new TagReference
//                {
//                    Id = 263298,
//                    TagType = TagType.TAG_POWER_LOOPING_IS_SELF_INTERRUPTING,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Looping Not Self Interrupting",
//                    InternalName = "TAG_POWER_LOOPING_IS_SELF_INTERRUPTING",
//                }
//            },
//            {
//                328632, new TagReference
//                {
//                    Id = 328632,
//                    TagType = TagType.TAG_POWER_CAUSES_CLOSING_COOLDOWN,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Causes Closing Cooldown",
//                    InternalName = "TAG_POWER_CAUSES_CLOSING_COOLDOWN",
//                }
//            },
//            {
//                270342, new TagReference
//                {
//                    Id = 270342,
//                    TagType = TagType.TAG_POWER_BUFF_6_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 6 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_6_EFFECT_GROUP",
//                }
//            },
//            {
//                328640, new TagReference
//                {
//                    Id = 328640,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_A,
//                    DataType = MapDataType.PowerSnoId2,
//                    DisplayName = "Special Death Chance Rune A",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_A",
//                }
//            },
//            {
//                271110, new TagReference
//                {
//                    Id = 271110,
//                    TagType = TagType.TAG_POWER_BUFF_6_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 6 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_6_SHOW_DURATION",
//                }
//            },
//            {
//                274694, new TagReference
//                {
//                    Id = 274694,
//                    TagType = TagType.TAG_POWER_BUFF_6_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 6 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_6_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                328083, new TagReference
//                {
//                    Id = 328083,
//                    TagType = TagType.TAG_POWER_CAST_TARGET_MUST_BE_IN_TELEPORTABLE_AREA,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CastTargetMustBeInTeleportableArea",
//                    InternalName = "TAG_POWER_CAST_TARGET_MUST_BE_IN_TELEPORTABLE_AREA",
//                }
//            },
//            {
//                272404, new TagReference
//                {
//                    Id = 272404,
//                    TagType = TagType.TAG_POWER_BUFF_20_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 20 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_20_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                328272, new TagReference
//                {
//                    Id = 328272,
//                    TagType = TagType.TAG_POWER_CAN_PATH_DURING_WALK,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CanPathDuringWalk",
//                    InternalName = "TAG_POWER_CAN_PATH_DURING_WALK",
//                }
//            },
//            {
//                272396, new TagReference
//                {
//                    Id = 272396,
//                    TagType = TagType.TAG_POWER_BUFF_12_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 12 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_12_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                274455, new TagReference
//                {
//                    Id = 274455,
//                    TagType = TagType.TAG_POWER_BUFF_23_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 23 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_23_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                721200, new TagReference
//                {
//                    Id = 721200,
//                    TagType = TagType.TAG_POWER_RUNEB_COMBO1_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneB Combo1 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEB_COMBO1_PROC_SCALAR",
//                }
//            },
//            {
//                274711, new TagReference
//                {
//                    Id = 274711,
//                    TagType = TagType.TAG_POWER_BUFF_23_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 23 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_23_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                270864, new TagReference
//                {
//                    Id = 270864,
//                    TagType = TagType.TAG_POWER_BUFF_10_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 10 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_10_HARMFUL_BUFF",
//                }
//            },
//            {
//                557056, new TagReference
//                {
//                    Id = 557056,
//                    TagType = TagType.TAG_POWER_FOLLOW_STOP_DISTANCE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Follow Stop Distance",
//                    InternalName = "TAG_POWER_FOLLOW_STOP_DISTANCE",
//                }
//            },
//            {
//                267280, new TagReference
//                {
//                    Id = 267280,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_31,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 31",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_31",
//                }
//            },
//            {
//                262673, new TagReference
//                {
//                    Id = 262673,
//                    TagType = TagType.TAG_POWER_ANIMATION_TAG_RUNE_A,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Animation Tag Rune A",
//                    InternalName = "TAG_POWER_ANIMATION_TAG_RUNE_A",
//                }
//            },
//            {
//                272669, new TagReference
//                {
//                    Id = 272669,
//                    TagType = TagType.TAG_POWER_BUFF_29_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 29 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_29_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                267024, new TagReference
//                {
//                    Id = 267024,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_21,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 21",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_21",
//                }
//            },
//            {
//                262929, new TagReference
//                {
//                    Id = 262929,
//                    TagType = TagType.TAG_POWER_COMBO_TEMPLATE_2,
//                    DataType = MapDataType.Template,
//                    DisplayName = "Combo Template 2",
//                    InternalName = "TAG_POWER_COMBO_TEMPLATE_2",
//                }
//            },
//            {
//                561408, new TagReference
//                {
//                    Id = 561408,
//                    TagType = TagType.TAG_POWER_FOLLOW_MATCH_TARGET_SPEED,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Follow Match Target Speed",
//                    InternalName = "TAG_POWER_FOLLOW_MATCH_TARGET_SPEED",
//                }
//            },
//            {
//                329697, new TagReference
//                {
//                    Id = 329697,
//                    TagType = TagType.TAG_POWER_MAX_CHARGES,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Max Charges",
//                    InternalName = "TAG_POWER_MAX_CHARGES",
//                }
//            },
//            {
//                272153, new TagReference
//                {
//                    Id = 272153,
//                    TagType = TagType.TAG_POWER_BUFF_19_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 19 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_19_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                328292, new TagReference
//                {
//                    Id = 328292,
//                    TagType = TagType.TAG_POWER_SET_TARGET_AFTER_INTRO,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Set Target After Intro",
//                    InternalName = "TAG_POWER_SET_TARGET_AFTER_INTRO",
//                }
//            },
//            {
//                270361, new TagReference
//                {
//                    Id = 270361,
//                    TagType = TagType.TAG_POWER_BUFF_19_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 19 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_19_EFFECT_GROUP",
//                }
//            },
//            {
//                721344, new TagReference
//                {
//                    Id = 721344,
//                    TagType = TagType.TAG_POWER_HOTBAR_EXCLUSIVE_TYPE,
//                    DataType = MapDataType.HotBarType,
//                    DisplayName = "Hotbar Exclusive Type",
//                    InternalName = "TAG_POWER_HOTBAR_EXCLUSIVE_TYPE",
//                }
//            },
//            {
//                327956, new TagReference
//                {
//                    Id = 327956,
//                    TagType = TagType.TAG_POWER_LOOKSWITCH_DOESNT_INTERRUPT,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Lookswitch Doesnt Interrupt",
//                    InternalName = "TAG_POWER_LOOKSWITCH_DOESNT_INTERRUPT",
//                }
//            },
//            {
//                270609, new TagReference
//                {
//                    Id = 270609,
//                    TagType = TagType.TAG_POWER_BUFF_11_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 11 Icon",
//                    InternalName = "TAG_POWER_BUFF_11_ICON",
//                }
//            },
//            {
//                271378, new TagReference
//                {
//                    Id = 271378,
//                    TagType = TagType.TAG_POWER_BUFF_12_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 12 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_12_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                274449, new TagReference
//                {
//                    Id = 274449,
//                    TagType = TagType.TAG_POWER_BUFF_17_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 17 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_17_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                329826, new TagReference
//                {
//                    Id = 329826,
//                    TagType = TagType.TAG_POWER_COMBO_ATTACK_SPEED_2,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Combo Attack Speed 2",
//                    InternalName = "TAG_POWER_COMBO_ATTACK_SPEED_2",
//                }
//            },
//            {
//                272146, new TagReference
//                {
//                    Id = 272146,
//                    TagType = TagType.TAG_POWER_BUFF_12_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 12 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_12_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                272415, new TagReference
//                {
//                    Id = 272415,
//                    TagType = TagType.TAG_POWER_BUFF_31_DONT_APPLY_VISUAL_TO_PETS,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 31 Dont Apply Visual To Pets",
//                    InternalName = "TAG_POWER_BUFF_31_DONT_APPLY_VISUAL_TO_PETS",
//                }
//            },
//            {
//                263205, new TagReference
//                {
//                    Id = 263205,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_2_RUNE_E,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 2 Rune E",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_2_RUNE_E",
//                }
//            },
//            {
//                272671, new TagReference
//                {
//                    Id = 272671,
//                    TagType = TagType.TAG_POWER_BUFF_31_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 31 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_31_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                270629, new TagReference
//                {
//                    Id = 270629,
//                    TagType = TagType.TAG_POWER_BUFF_25_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 25 Icon",
//                    InternalName = "TAG_POWER_BUFF_25_ICON",
//                }
//            },
//            {
//                270885, new TagReference
//                {
//                    Id = 270885,
//                    TagType = TagType.TAG_POWER_BUFF_25_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 25 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_25_HARMFUL_BUFF",
//                }
//            },
//            {
//                329696, new TagReference
//                {
//                    Id = 329696,
//                    TagType = TagType.TAG_POWER_COST_CHARGES,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Charge Cost",
//                    InternalName = "TAG_POWER_COST_CHARGES",
//                }
//            },
//            {
//                270632, new TagReference
//                {
//                    Id = 270632,
//                    TagType = TagType.TAG_POWER_BUFF_28_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 28 Icon",
//                    InternalName = "TAG_POWER_BUFF_28_ICON",
//                }
//            },
//            {
//                327920, new TagReference
//                {
//                    Id = 327920,
//                    TagType = TagType.TAG_POWER_CANNOT_LMB_ASSIGN,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CannotLMBAssign",
//                    InternalName = "TAG_POWER_CANNOT_LMB_ASSIGN",
//                }
//            },
//            {
//                270376, new TagReference
//                {
//                    Id = 270376,
//                    TagType = TagType.TAG_POWER_BUFF_28_EFFECT_GROUP,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Buff 28 Effect Group",
//                    InternalName = "TAG_POWER_BUFF_28_EFFECT_GROUP",
//                }
//            },
//            {
//                329698, new TagReference
//                {
//                    Id = 329698,
//                    TagType = TagType.TAG_POWER_RECHARGE_TIME,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Recharge Time",
//                    InternalName = "TAG_POWER_RECHARGE_TIME",
//                }
//            },
//            {
//                721185, new TagReference
//                {
//                    Id = 721185,
//                    TagType = TagType.TAG_POWER_RUNEA_COMBO2_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneA Combo2 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEA_COMBO2_PROC_SCALAR",
//                }
//            },
//            {
//                328992, new TagReference
//                {
//                    Id = 328992,
//                    TagType = TagType.TAG_POWER_REQUIRES_2H_ITEM,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Requires 2H Item",
//                    InternalName = "TAG_POWER_REQUIRES_2H_ITEM",
//                }
//            },
//            {
//                329833, new TagReference
//                {
//                    Id = 329833,
//                    TagType = TagType.TAG_POWER_USES_MAINHAND_ONLY_COMBO3,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Combo Level 3 Uses Main Hand Only",
//                    InternalName = "TAG_POWER_USES_MAINHAND_ONLY_COMBO3",
//                }
//            },
//            {
//                263220, new TagReference
//                {
//                    Id = 263220,
//                    TagType = TagType.TAG_POWER_COMBO_ANIMATION_3_RUNE_D,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Combo Animation 3 Rune D",
//                    InternalName = "TAG_POWER_COMBO_ANIMATION_3_RUNE_D",
//                }
//            },
//            {
//                268080, new TagReference
//                {
//                    Id = 268080,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_63,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 63",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_63",
//                }
//            },
//            {
//                272644, new TagReference
//                {
//                    Id = 272644,
//                    TagType = TagType.TAG_POWER_BUFF_4_SHOW_ACTIVE_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 4 Show Active On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_4_SHOW_ACTIVE_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                270849, new TagReference
//                {
//                    Id = 270849,
//                    TagType = TagType.TAG_POWER_BUFF_1_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 1 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_1_HARMFUL_BUFF",
//                }
//            },
//            {
//                721218, new TagReference
//                {
//                    Id = 721218,
//                    TagType = TagType.TAG_POWER_RUNEC_COMBO3_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneC Combo3 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEC_COMBO3_PROC_SCALAR",
//                }
//            },
//            {
//                328649, new TagReference
//                {
//                    Id = 328649,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_E,
//                    DataType = MapDataType.Unk90,
//                    DisplayName = "Special Death Type Rune E",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_E",
//                }
//            },
//            {
//                712965, new TagReference
//                {
//                    Id = 712965,
//                    TagType = TagType.TAG_POWER_CONSOLE_AUTO_TARGET_CLOSE_RANGE,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Controller Auto Target Close Range",
//                    InternalName = "TAG_POWER_CONSOLE_AUTO_TARGET_CLOSE_RANGE",
//                }
//            },
//            {
//                271105, new TagReference
//                {
//                    Id = 271105,
//                    TagType = TagType.TAG_POWER_BUFF_1_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 1 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_1_SHOW_DURATION",
//                }
//            },
//            {
//                274434, new TagReference
//                {
//                    Id = 274434,
//                    TagType = TagType.TAG_POWER_BUFF_2_SHOW_IN_BUFF_HOLDER,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 2 Show In Buff Holder",
//                    InternalName = "TAG_POWER_BUFF_2_SHOW_IN_BUFF_HOLDER",
//                }
//            },
//            {
//                271874, new TagReference
//                {
//                    Id = 271874,
//                    TagType = TagType.TAG_POWER_BUFF_2_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 2 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_2_MERGES_TOOLTIP",
//                }
//            },
//            {
//                262663, new TagReference
//                {
//                    Id = 262663,
//                    TagType = TagType.TAG_POWER_OFFHAND_ANIMATION_TAG,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Offhand Animation Tag (for dual wield)",
//                    InternalName = "TAG_POWER_OFFHAND_ANIMATION_TAG",
//                }
//            },
//            {
//                270594, new TagReference
//                {
//                    Id = 270594,
//                    TagType = TagType.TAG_POWER_BUFF_2_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 2 Icon",
//                    InternalName = "TAG_POWER_BUFF_2_ICON",
//                }
//            },
//            {
//                266880, new TagReference
//                {
//                    Id = 266880,
//                    TagType = TagType.TAG_POWER_SCRIPT_FORMULA_18,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Script Formula 18",
//                    InternalName = "TAG_POWER_SCRIPT_FORMULA_18",
//                }
//            },
//            {
//                274699, new TagReference
//                {
//                    Id = 274699,
//                    TagType = TagType.TAG_POWER_BUFF_11_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 11 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_11_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                272134, new TagReference
//                {
//                    Id = 272134,
//                    TagType = TagType.TAG_POWER_BUFF_6_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 6 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_6_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                272135, new TagReference
//                {
//                    Id = 272135,
//                    TagType = TagType.TAG_POWER_BUFF_7_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 7 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_7_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                271878, new TagReference
//                {
//                    Id = 271878,
//                    TagType = TagType.TAG_POWER_BUFF_6_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 6 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_6_MERGES_TOOLTIP",
//                }
//            },
//            {
//                332432, new TagReference
//                {
//                    Id = 332432,
//                    TagType = TagType.TAG_POWER_RUN_NEARBY_DISTANCE_DELTA,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Run Nearby Distance Delta",
//                    InternalName = "TAG_POWER_RUN_NEARBY_DISTANCE_DELTA",
//                }
//            },
//            {
//                271367, new TagReference
//                {
//                    Id = 271367,
//                    TagType = TagType.TAG_POWER_BUFF_7_PLAYER_CAN_CANCEL,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 7 Player Can Cancel",
//                    InternalName = "TAG_POWER_BUFF_7_PLAYER_CAN_CANCEL",
//                }
//            },
//            {
//                270595, new TagReference
//                {
//                    Id = 270595,
//                    TagType = TagType.TAG_POWER_BUFF_3_ICON,
//                    DataType = MapDataType.IconSno,
//                    DisplayName = "Buff 3 Icon",
//                    InternalName = "TAG_POWER_BUFF_3_ICON",
//                }
//            },
//            {
//                271875, new TagReference
//                {
//                    Id = 271875,
//                    TagType = TagType.TAG_POWER_BUFF_3_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 3 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_3_MERGES_TOOLTIP",
//                }
//            },
//            {
//                328512, new TagReference
//                {
//                    Id = 328512,
//                    TagType = TagType.TAG_POWER_CAN_USE_WHEN_FEARED,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Can Use When Feared",
//                    InternalName = "TAG_POWER_CAN_USE_WHEN_FEARED",
//                }
//            },
//            {
//                274700, new TagReference
//                {
//                    Id = 274700,
//                    TagType = TagType.TAG_POWER_BUFF_12_SHOW_DURATION_ON_SKILL_BUTTON,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 12 Show Duration On Skill Button",
//                    InternalName = "TAG_POWER_BUFF_12_SHOW_DURATION_ON_SKILL_BUTTON",
//                }
//            },
//            {
//                329744, new TagReference
//                {
//                    Id = 329744,
//                    TagType = TagType.TAG_POWER_ESCAPE_ATTACK_RADIUS,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Escape Attack Radius",
//                    InternalName = "TAG_POWER_ESCAPE_ATTACK_RADIUS",
//                }
//            },
//            {
//                328648, new TagReference
//                {
//                    Id = 328648,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_E,
//                    DataType = MapDataType.PowerSnoId2,
//                    DisplayName = "Special Death Chance Rune E",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_E",
//                }
//            },
//            {
//                262677, new TagReference
//                {
//                    Id = 262677,
//                    TagType = TagType.TAG_POWER_ANIMATION_TAG_RUNE_E,
//                    DataType = MapDataType.Animation,
//                    DisplayName = "Animation Tag Rune E",
//                    InternalName = "TAG_POWER_ANIMATION_TAG_RUNE_E",
//                }
//            },
//            {
//                681987, new TagReference
//                {
//                    Id = 681987,
//                    TagType = TagType.TAG_POWER_BREAKS_FEAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Breaks Fear",
//                    InternalName = "TAG_POWER_BREAKS_FEAR",
//                }
//            },
//            {
//                328532, new TagReference
//                {
//                    Id = 328532,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_CHANCE,
//                    DataType = MapDataType.PowerSnoId2,
//                    DisplayName = "Special Death Chance",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_CHANCE",
//                }
//            },
//            {
//                328804, new TagReference
//                {
//                    Id = 328804,
//                    TagType = TagType.TAG_POWER_CUSTOM_TARGET_NEEDS_HEAL_HP_PERCENT,
//                    DataType = MapDataType.HighPrecision,
//                    DisplayName = "Custom Target Needs Heal HP Percent",
//                    InternalName = "TAG_POWER_CUSTOM_TARGET_NEEDS_HEAL_HP_PERCENT",
//                }
//            },
//            {
//                328416, new TagReference
//                {
//                    Id = 328416,
//                    TagType = TagType.TAG_POWER_CANNOT_LOCK_ONTO_ACTORS,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CannotLockOntoActors",
//                    InternalName = "TAG_POWER_CANNOT_LOCK_ONTO_ACTORS",
//                }
//            },
//            {
//                328534, new TagReference
//                {
//                    Id = 328534,
//                    TagType = TagType.TAG_POWER_SPECIAL_DEATH_TYPE,
//                    DataType = MapDataType.Unk90,
//                    DisplayName = "Special Death Type",
//                    InternalName = "TAG_POWER_SPECIAL_DEATH_TYPE",
//                }
//            },
//            {
//                271122, new TagReference
//                {
//                    Id = 271122,
//                    TagType = TagType.TAG_POWER_BUFF_12_SHOW_DURATION,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 12 Show Duration",
//                    InternalName = "TAG_POWER_BUFF_12_SHOW_DURATION",
//                }
//            },
//            {
//                721216, new TagReference
//                {
//                    Id = 721216,
//                    TagType = TagType.TAG_POWER_RUNEC_COMBO1_PROC_SCALAR,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "RuneC Combo1 Proc Scalar",
//                    InternalName = "TAG_POWER_RUNEC_COMBO1_PROC_SCALAR",
//                }
//            },
//            {
//                270866, new TagReference
//                {
//                    Id = 270866,
//                    TagType = TagType.TAG_POWER_BUFF_12_HARMFUL_BUFF,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 12 Is Harmful",
//                    InternalName = "TAG_POWER_BUFF_12_HARMFUL_BUFF",
//                }
//            },
//            {
//                327697, new TagReference
//                {
//                    Id = 327697,
//                    TagType = TagType.TAG_POWER_SPELL_FUNC_CREATE,
//                    DataType = MapDataType.SpellFunc,
//                    DisplayName = "SpellFunc Create",
//                    InternalName = "TAG_POWER_SPELL_FUNC_CREATE",
//                }
//            },
//            {
//                272163, new TagReference
//                {
//                    Id = 272163,
//                    TagType = TagType.TAG_POWER_BUFF_23_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 23 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_23_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                272152, new TagReference
//                {
//                    Id = 272152,
//                    TagType = TagType.TAG_POWER_BUFF_18_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 18 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_18_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                271909, new TagReference
//                {
//                    Id = 271909,
//                    TagType = TagType.TAG_POWER_BUFF_25_MERGES_TOOLTIP,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "Buff 25 Merges Tooltip",
//                    InternalName = "TAG_POWER_BUFF_25_MERGES_TOOLTIP",
//                }
//            },
//            {
//                328538, new TagReference
//                {
//                    Id = 328538,
//                    TagType = TagType.TAG_POWER_USE_SPECIALWALK_STEERING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Use SpecialWalk Steering",
//                    InternalName = "TAG_POWER_USE_SPECIALWALK_STEERING",
//                }
//            },
//            {
//                264289, new TagReference
//                {
//                    Id = 264289,
//                    TagType = TagType.TAG_POWER_COMBO_CASTING_EFFECT_GROUP_1,
//                    DataType = MapDataType.ActorGroup,
//                    DisplayName = "Combo 1 Casting Effect Group - Male",
//                    InternalName = "TAG_POWER_COMBO_CASTING_EFFECT_GROUP_1",
//                }
//            },
//            {
//                272165, new TagReference
//                {
//                    Id = 272165,
//                    TagType = TagType.TAG_POWER_BUFF_25_MERGE_TOOLTIP_INDEX,
//                    DataType = MapDataType.Shader,
//                    DisplayName = "Buff 25 Merge Tooltip Index",
//                    InternalName = "TAG_POWER_BUFF_25_MERGE_TOOLTIP_INDEX",
//                }
//            },
//            {
//                633616, new TagReference
//                {
//                    Id = 633616,
//                    TagType = TagType.TAG_POWER_IS_UNTARGETABLE_DURING,
//                    DataType = MapDataType.Formula,
//                    DisplayName = "Is Untargetable During",
//                    InternalName = "TAG_POWER_IS_UNTARGETABLE_DURING",
//                }
//            },
//            {
//                328113, new TagReference
//                {
//                    Id = 328113,
//                    TagType = TagType.TAG_POWER_CAST_TARGET_ENEMIES,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "CastTargetEnemies",
//                    InternalName = "TAG_POWER_CAST_TARGET_ENEMIES",
//                }
//            },
//            {
//                327792, new TagReference
//                {
//                    Id = 327792,
//                    TagType = TagType.TAG_POWER_IS_PUNCH,
//                    DataType = MapDataType.Bool,
//                    DisplayName = "IsPunch",
//                    InternalName = "TAG_POWER_IS_PUNCH",
//                }
//            },
//        };


//    }
//}
