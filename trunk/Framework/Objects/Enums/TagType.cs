using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Framework.Objects.Enums
{
    public enum TagType // 2.4.2.39192
    {
        TAG_POWER_TORNADO_TIME_BETWEEN_DIR_CHANGE_DELTA = 684803, //  Tornado Time To Dir Change Delta
        TAG_POWER_SLOW_TIME_AMOUNT = 330944, //  Slow Time Amount
        TAG_POWER_LIGHTNING_DAMAGE_DELTA = 330128, //  Lightning Damage Delta
        TAG_POWER_TOTEM_MAX_COUNT = 337920, //  Totem Max Count
        TAG_POWER_SKELETON_MAX_TOTAL_SUMMON_COUNT = 332450, //  Skeleton Summon Max Total Count
        TAG_POWER_ARCANE_DURATION = 329952, //  Arcane Duration
        TAG_POWER_CLEAVE_NUM_TARGETS = 602112, //  Cleave Num Targets
        TAG_POWER_AURA_AFFECTED_RADIUS = 331840, //  Aura Affected Radius
        TAG_POWER_POISON_DAMAGE_MIN = 330240, //  Poison Damage Min
        TAG_POWER_EXPEL_MONSTER_SNO = 688128, //  Expel Monster Actor
        TAG_POWER_FURY_DEGENERATION_START = 332181, //  Fury Degeneration Start (in seconds)
        TAG_POWER_MOVEMENT_SPEED_PERCENT_INCREASE_DELTA = 332016, //  Movement Speed Percent Increase Delta
        TAG_POWER_BURN_HOUSE_DAMAGE_PER_SECOND = 682496, //  Burn House Damage Per Second
        TAG_POWER_BONUS_HEALING_RECEIVED_PERCENT = 700448, //  Bonus Healing Received Percent
        TAG_POWER_PET_SEARCH_RADIUS = 362496, //  Pet Search Radius
        TAG_POWER_DISINTEGRATE_BEAM_WIDTH = 332304, //  Disintegrate Beam Width
        TAG_POWER_ZOMBIFY_DAMAGE_PER_SECOND = 594688, //  Zombify Damage Per Second
        TAG_POWER_DEATH_PORTAL_ACTOR_0 = 720896, //  Death Portal Actor 0
        TAG_POWER_CHARGED_BOLT_SPREAD_ANGLE = 331057, //  Charged Bolt Spread Angle
        TAG_POWER_SPELL_FUNC_1 = 327728, //  SpellFunc 1
        TAG_POWER_ZOMBIFY_POSSESSED_DURATION = 594176, //  Zombify Possessed Duration
        TAG_POWER_MANAREGEN_MANA_RESTORE_PERCENT_PER_SECOND = 655393, //  Mana Regen Restore % Per Second
        TAG_POWER_USES_WEAPON_PROJECTILE = 262480, //  Uses Weapon Projectile
        TAG_POWER_BURROW_HIDES_ACTOR = 712704, //  Burrow Hides Actor
        TAG_POWER_SPECIAL_WALK_TRAJECTORY_HEIGHT_IS_RELATIVE_TO_MAX = 332338, //  Special Walk Height Is Relative To Max
        TAG_POWER_SPECTRALBLADE_BLEED_DMG = 393216, //  Spectral Blade Bleed Damage
        TAG_POWER_FEASTOFSOULS_DAMAGE_PER_SECOND = 369920, //  Feast of Souls Damage Per Second
        TAG_POWER_TORNADO_TIME_BETWEEN_DIR_CHANGE_MIN = 684802, //  Tornado Time To Dir Change Min
        TAG_POWER_IS_LOBBED = 327872, //  IsLobbed
        TAG_POWER_IMMOBILIZE_DURATION_DELTA = 330880, //  Immobilize Duration Delta
        TAG_POWER_WHIRLWIND_KNOCKBACK_MAGNITUDE = 331664, //  Whirlwind Knockback Magnitude
        TAG_POWER_FIRE_DAMAGE_DELTA = 330064, //  Fire Damage Delta
        TAG_POWER_ATTACK_RATING = 329888, //  Attack Rating
        TAG_POWER_RESURRECTION_HEALTH_MULT_TO_START = 651264, //  Resurrection Health Multiplier To Start
        TAG_POWER_RING_OF_FROST_DURATION = 332896, //  Ring of Frost Ring Lifetime
        TAG_POWER_RAPIDFIRE_ATTACK_SPEED_BONUS_PERCENT = 417792, //  Rapid Fire Attack Speed Bonus Percent
        TAG_POWER_NO_REFLECT_BUFFER_TIME = 361536, //  Whirlwind Reflect Disabled Timer
        TAG_POWER_FALLEN_CHAMPION_LEADER_SHOUT_EFFECT_RADIUS = 684544, //  Fallen Champion Leader Shout Effect Radius
        TAG_POWER_SAND_SHIELD_PROJECTILE_BLOCK_CHANCE = 630786, //  Sand Shield Projectile Block Chance
        TAG_POWER_ROPE_GROUND_SNO = 262434, //  Rope Ground
        TAG_POWER_SIEGE_DAMAGE_DELTA = 330448, //  Siege Damage Delta
        TAG_POWER_DEATH_STATUE_GHOUL_BUFF_RANGE = 339200, //  Death Statue Ghoul Buff Range
        TAG_POWER_FREEZE_DAMAGE_DONE_MIN = 331232, //  Freeze Damage Done Min
        TAG_POWER_LOBBED_HEIGHT_ABOVE_SOURCE = 329880, //  Height Above Source
        TAG_POWER_TORNADO_WANDER_RADIUS_MIN = 684805, //  Tornado Wander Radius Min
        TAG_POWER_TARGET_NORMAL_MONSTERS_ONLY = 328613, //  TargetNormalMonstersOnly
        TAG_POWER_DEATH_PROOF_PERCENT_TO_HEAL = 655873, //  Death Proof Percent To Heal
        TAG_POWER_MIRROR_IMAGE_THORN_DAMAGE = 333296, //  Mirror Image Thorn Damage
        TAG_POWER_PLAY_SUMMONED_BY_MONSTER_ANIMATION = 331137, //  Play Summoned By Monster Animation
        TAG_POWER_TARGET_ENEMIES = 328112, //  TargetEnemies
        TAG_POWER_STUN_CHANCE = 330816, //  Stun Chance
        TAG_POWER_PHYSICAL_DAMAGE_DELTA = 330000, //  Physical Damage Delta
        TAG_POWER_VANISH_AT_HEALTH_PERCENT = 460032, //  Vanish At Health Percent
        TAG_POWER_FURY_ADD_PER_INTERVAL = 332178, //  Fury Add Per Interval
        TAG_POWER_DISEASE_RESISTANCE = 330608, //  Disease Damage Resistance
        TAG_POWER_CONTACT_FRAME_IN_COOLDOWN = 328224, //  ContactFrameType
        TAG_POWER_LIFESTEAL_PERCENT = 331224, //  Lifesteal Percent
        TAG_POWER_DAMAGE_AURA_DURATION = 332832, //  Damage Aura Duration
        TAG_POWER_TARGET_IGNORE_LARGE_MONSTERS = 328612, //  TargetIgnoreLargeMonsters
        TAG_POWER_ARCANE_ARMOR_DEFENSE_BONUS_PERCENT = 655728, //  Arcane Armor Defense Bonus Percent
        TAG_POWER_TARGET_UNDEAD_MONSTERS_ONLY = 328614, //  TargetUndeadMonstersOnly
        TAG_POWER_WHIRLWHIND_RANGE_BONUS = 361472, //  Whirlwind Range Bonus
        TAG_POWER_SUMMONED_ACTOR_USE_DEFAULT_LEVEL = 331138, //  Summoned Actor Use Default Level
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_3_WITH_PARAM = 655666, //  Generic Buff Attribute 3 And Parameter
        TAG_POWER_BUFF_DURATION_BETWEEN_PULSES = 332768, //  Buff Duration Between Pulses
        TAG_POWER_THUNDERSTORM_BOLTS_PER_SECOND = 331168, //  Thunderstorm Bolts Per Second
        TAG_POWER_STATIC_CHARGE_RADIUS = 664320, //  Static Charge Radius
        TAG_POWER_SCALE_BONUS = 332024, //  Scale Bonus
        TAG_POWER_FURY_TIME_BETWEEN_UPDATES = 333056, //  Fury Time Between Updates
        TAG_POWER_ROCKWORM_WEB_SLOW_MULTIPLIER = 368896, //  Rockworm Web Movement Speed Multiplier
        TAG_POWER_FURY_DEGENERATION_REDUCED_DECAY_RATE = 334235, //  Fury Reduced Degen. (Per second)
        TAG_POWER_MANAREGEN_TIME_BETWEEN_UPDATES = 655392, //  Mana Regen Time Between Updates
        TAG_POWER_GHOST_SOULSIPHON_DAMAGE_PER_SECOND = 365312, //  Ghost Soulsiphon Damage Per Second
        TAG_POWER_DAMAGE_PERCENT_ALL = 330752, //  Damage Percent All
        TAG_POWER_CURSE_DAMAGE_AMPLIFY_PERCENT = 331536, //  Curse Damage Amplify Percent
        TAG_POWER_SKELETON_KING_WHIRLWIND_APS = 700416, //  Skeleton King WW APS
        TAG_POWER_PAYLOAD_TYPE = 329760, //  Payload Type
        TAG_POWER_HOLY_RESISTANCE = 330544, //  Holy Damage Resistance
        TAG_POWER_RUNE_FIND_BONUS = 331160, //  Rune Find Bonus
        TAG_POWER_WHIRLWIND_DURATION_DELTA = 361520, //  Whirlwind Duration Delta
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_1 = 655633, //  Generic Buff Attribute 1 Formula
        TAG_POWER_TOTEM_LIFESPAN = 338176, //  Totem Lifespan
        TAG_POWER_COLD_DAMAGE_PERCENT = 330208, //  Cold Damage Percent
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_3 = 655664, //  Generic Buff Attribute 3
        TAG_POWER_PLAGUE_OF_TOADS_TOAD_LIFESPAN = 600072, //  Plague of Toads Toad Lifespan
        TAG_POWER_ROOT_DURATION = 332816, //  Root Duration
        TAG_POWER_FURY_GAINED_PER_PERCENT_HEALTH_LOST = 332096, //  Fury Gained Per Percent Health Lost
        TAG_POWER_THUNDERSTORM_DURATION = 332704, //  Thunderstorm Duration
        TAG_POWER_ARCANE_DAMAGE_DELTA = 330320, //  Arcane Damage Delta
        TAG_POWER_PROJECTILE_COUNT = 331193, //  Projectile Count
        TAG_POWER_TELEPORT_NUMBER_OF_IMAGES = 405504, //  Teleport Number of Images
        TAG_POWER_DEATH_PROOF_DEBUFF_DURATION = 655872, //  Death Proof Debuff Duration
        TAG_POWER_PALADIN_RESURRECTION_COOLDOWN_TIME = 332272, //  Paladin Resurrection Cooldown Time
        TAG_POWER_BUFF_DURATION_DELTA = 332752, //  Buff Duration Delta
        TAG_POWER_TORNADO_TIME_BETWEEN_DIR_CHANGE_GROWTH = 684804, //  Tornado Time To Dir Change Min Growth/Change
        TAG_POWER_PLAGUE_OF_TOADS_NUM_FROGS = 600064, //  Plague of Toads Num Frogs
        TAG_POWER_LACUNI_LEAP_TARGET_OFFSET = 684864, //  Lacuni Leap Target Offset
        TAG_POWER_CORPULENT_CRYPT_KID_SPAWN_COUNT = 332480, //  Num Crypt Kids To Spawn On Corpulent Explosion
        TAG_POWER_RESURRECTION_BUFF_TIME = 647168, //  Resurrection Buff Time
        TAG_POWER_DEATH_PORTAL_CHANCE = 720976, //  Death Portal Chance
        TAG_POWER_LEAP_DIST_MAX = 708617, //  Leap Distance Max
        TAG_POWER_KNOCKBACK_POWER = 331710, //  Knockback Power on Target
        TAG_POWER_TORNADO_WANDER_RADIUS_GROWTH = 684807, //  Tornado Wander Radius Growth/Change
        TAG_POWER_MASTABLASTA_DRAIN_HATRED_PERCENT = 708704, //  MastaBlasta Drain Hatred Percent
        TAG_POWER_IMPROVED_BATTLE_RAGE_DURATION = 332050, //  Improved Battle Rage Duration
        TAG_POWER_FEIGN_DEATH_UNTARGETABLE_DURATION = 430080, //  Feign Death Untargetable Duration
        TAG_POWER_DAMAGE_REDUCTION_PERCENT = 680448, //  Damage Reduction Percent
        TAG_POWER_ROPE_CHAIN_SNO = 262403, //  Chain Rope
        TAG_POWER_TARGET_CORPSE = 328128, //  TargetCorpse
        TAG_POWER_CONTACT_FRAME_EFFECT_GROUP = 264192, //  Contact Frame Effect Group - Male
        TAG_POWER_LIGHTNING_DAMAGE_PERCENT = 330144, //  Lightning Damage Percent
        TAG_POWER_HOLY_DURATION = 329968, //  Holy Duration
        TAG_POWER_DELIVERY_MECHANISM = 330624, //  Delivery Mechanism
        TAG_POWER_FURY_DEGENERATION_OUT_OF_COMBAT = 332177, //  Fury Degeneration Out of Combat (per second)
        TAG_POWER_CRUSHING_BLOW_MAX_BUFF_LEVEL = 332032, //  Crushing Blow Max Buff Level
        TAG_POWER_SHIELD_DAMAGE_ABSORB_AMOUNT = 332640, //  Hitpoints That Shield Can Absorb
        TAG_POWER_POISON_DAMAGE_DELTA = 330256, //  Poison Damage Delta
        TAG_POWER_DEATH_NOVA_RADIUS_FT_PER_SEC = 639232, //  Death Nova Radius Ft Per Sec
        TAG_POWER_PIERCE_CHANCE = 331040, //  Pierce Chance
        TAG_POWER_BUFF_MAX_STACK = 332776, //  Generic Buff Max Stack 0
        TAG_POWER_INCOMING_DAMAGE_INCREASE = 684546, //  Incoming Damage Increase
        TAG_POWER_ROCKWORM_WEB_DURATION = 369024, //  Rockworm Web Duration
        TAG_POWER_LIGHTNING_SPEED_STACK_COUNT = 661760, //  Lightning Speed Stack Count
        TAG_POWER_DISINTEGRATE_TIME_BETWEEN_UPDATES = 332688, //  Disintegrate Time Between Updates
        TAG_POWER_MANAREGEN_TIME_TO_KICK_IN = 655394, //  Mana Regen Time To Kick In
        TAG_POWER_DOT_DAMAGE = 552960, //  DOT Damage
        TAG_POWER_DEATH_PORTAL_ACTOR_1 = 720912, //  Death Portal Actor 1
        TAG_POWER_TARGET_ATTACKED_SNO = 262406, //  Target Attacked Particle
        TAG_POWER_FALLEN_CHAMPION_LEADER_SHOUT_DURATION = 684545, //  Fallen Champion Leader Shout Duration
        TAG_POWER_ROCKWORM_BURST_OUT_DISTANCE_MIN = 369152, //  Rockworm Burst Out Distance Min
        TAG_POWER_TORNADO_WANDER_RADIUS_DELTA = 684806, //  Tornado Wander Radius Delta
        TAG_POWER_FIREBATS_INITIAL_DELAY = 600322, //  Fire Bats Initial Delay
        TAG_POWER_CAUSES_SUPER_KNOCKBACK = 633344, //  Causes Super Knockback
        TAG_POWER_DEATH_PORTAL_CHANCE_SPECIAL = 720977, //  Death Portal Chance (Special)
        TAG_POWER_SPECIAL_WALK_IS_KNOCKBACK = 332362, //  Special Walk Is Knockback
        TAG_POWER_SLOW_DURATION_MIN = 330896, //  Slow Duration Min
        TAG_POWER_MASTABLASTA_DRAIN_SPIRIT_PERCENT = 708688, //  MastaBlasta Drain Spirit Percent
        TAG_POWER_POWERED_IMPACT_MANA_INCREASE = 671744, //  Powered Impact Mana Increase
        TAG_POWER_FIRE_DAMAGE_PERCENT = 330080, //  Fire Damage Percent
        TAG_POWER_ATTACK_RATING_PERCENT = 329904, //  Attack Rating Percent
        TAG_POWER_DISEASE_DAMAGE_MIN = 330560, //  Disease Damage Min
        TAG_POWER_ENERGY_SHIELD_MANA_COST_PER_DAMAGE = 332576, //  Energy Shield Mana Cost Per Damage
        TAG_POWER_SANDSHARK_DELAY_BEFORE_SAND_ATTACK = 681604, //  Sandshark Delay Before Sand Attack
        TAG_POWER_DEBUFF_DURATION = 655680, //  Debuff Duration Min
        TAG_POWER_HOOKSHOT_HIT_RETURN_SPEED_MULT = 610304, //  Hookshot Hit Return Speed Mult
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_0_WITH_PARAM = 655618, //  Generic Buff Attribute 0 And Parameter
        TAG_POWER_WHIRLWHIND_RANGE_MULTIPLIER = 361216, //  Whirlwind Range Multiplier
        TAG_POWER_PUMMEL_COOLDOWN = 332624, //  Pummel Cooldown
        TAG_POWER_DEATH_STATUE_NUM_SHOTS = 338432, //  Death Statue Num Shots
        TAG_POWER_SIEGE_DAMAGE_PERCENT = 330464, //  Siege Damage Percent
        TAG_POWER_FREEZE_DAMAGE_DONE_DELTA = 331248, //  Freeze Damage Done Delta
        TAG_POWER_POWER_GAINED_PER_SECOND = 700435, //  Power Gained Per Second
        TAG_POWER_SALVAGE_FIND_BONUS = 331164, //  Salvage Find Bonus
        TAG_POWER_UI_ANIMATION = 282624, //  UI Icon
        TAG_POWER_TARGET_IMPACT_EFFECT_GROUP = 262407, //  Target Impact Effect Group
        TAG_POWER_DOT_STACKING_METHOD = 329979, //  Dot Stacking Method
        TAG_POWER_ROPE_SNO = 262432, //  Rope
        TAG_POWER_FREEZE_DURATION_DELTA = 692256, //  Freeze Duration Delta
        TAG_POWER_POOL_ACTOR = 684160, //  Pool Actor
        TAG_POWER_STONESPIKE_DELAY = 370688, //  Stone Spike Attack Delay Time
        TAG_POWER_CONDUCTION_AURA_REFRESH_RATE_DECREASE_PER_TARGET = 332528, //  Conduction Aura Refresh Rate Decrease Per Target
        TAG_POWER_WHIRLWIND_MOVE_SPEED = 361521, //  Whirlwind Movement Speed
        TAG_POWER_WEB_DURATION_MIN = 330832, //  Web Duration Min
        TAG_POWER_FEAR_DURATION_MIN = 331616, //  Fear Duration Min
        TAG_POWER_CASTING_EFFECT_GROUP_FEMALE_2 = 264276, //  Casting Effect Group - Female 2
        TAG_POWER_PHYSICAL_DAMAGE_PERCENT = 330016, //  Physical Damage Percent
        TAG_POWER_KNOCKBACK_GRAVITY_DELTA = 331709, //  Knockback Gravity Delta
        TAG_POWER_BASE_DAMAGE_SCALAR = 329840, //  Base Damage Scalar
        TAG_POWER_HOLY_DAMAGE_MIN = 330496, //  Holy Damage Min
        TAG_POWER_IMPROVED_BATTLE_RAGE_PROC_CHANCE = 332049, //  Improved Battle Rage Proc Chance
        TAG_POWER_WAVEOFFORCE_PROJECTILE_PERCENT_SPEED_INC = 401408, //  Wave Of Force Projectile Percent Speed Inc
        TAG_POWER_GEM_FIND_BONUS = 331156, //  Gem Find Bonus
        TAG_POWER_FURY_MAX_DAMAGE_BONUS = 332185, //  Fury Max Damage Bonus
        TAG_POWER_CONTACT_FRAME_EFFECT_GROUP_FEMALE = 264275, //  Contact Frame Effect Group - Female
        TAG_POWER_PROC_COOLDOWN = 680768, //  Proc Cooldown Time
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_0 = 655616, //  Generic Buff Attribute 0
        TAG_POWER_FURY_GAINED_PER_SECOND_OF_ATTACK = 332097, //  Fury Gained Per Second Of Attack
        TAG_POWER_WHIRLWIND_PULSE_RATE = 361488, //  Whirlwind Attack Pulse Rate
        TAG_POWER_BLIZZARD_INITIAL_IMPACT_DELAY = 332080, //  Blizzard Initial Impact Delay
        TAG_POWER_MOVEMENT_SPEED_PERCENT_INCREASE_MIN = 332000, //  Movement Speed Percent Increase Min
        TAG_POWER_MAGIC_MISSILE_JITTER_ANGLE = 332560, //  Magic Missile Jitter Angle
        TAG_POWER_PROJECTILE_SPEED = 331184, //  Projectile Speed
        TAG_POWER_DURATION_MIN = 684929, //  Duration Min
        TAG_POWER_KNOCKBACK_GRAVITY_MIN = 331708, //  Knockback Gravity Min
        TAG_POWER_ELECTROCUTE_PERCENT_DAMAGE_REDUCTION_PER_BOUNCE = 332288, //  Electrocute Percent Damage Reduction Per Bounce
        TAG_POWER_RAINOFGOLD_DAMAGE_TO_GOLD_PERCENT = 425984, //  Rain Of Gold Damage To Gold Percent
        TAG_POWER_HITSOUND_OVERRIDE = 262433, //  Hitsound Override
        TAG_POWER_RESISTANCE_PERCENT_ALL = 524288, //  Resistance Percent All
        TAG_POWER_SHIELD_ANGLE = 680192, //  Shield Angle
        TAG_POWER_MIRROR_IMAGE_SUMMON_COUNT = 332464, //  Mirror Image Summon Count
        TAG_POWER_GHOST_SOULSIPHON_SLOW_MULTIPLIER = 364544, //  Ghost Soulsiphon Slow Multiplier
        TAG_POWER_VANISH_DURATION = 458752, //  Vanish Duration
        TAG_POWER_DEFENSE_PERCENT_ALL = 330768, //  Defense Percent All
        TAG_POWER_RING_OF_FROST_SLOW_CHANCE = 331552, //  Ring of Frost Slow Chance
        TAG_POWER_FREEZE_DURATION_MIN = 692240, //  Freeze Duration Min
        TAG_POWER_PAYLOAD_PARAM_0 = 329776, //  Payload Param 0
        TAG_POWER_SPECTRALBLADE_NUMBER_OF_HITS = 397313, //  Spectral Blade Number of Hits
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_2 = 655649, //  Generic Buff Attribute 2 Formula
        TAG_POWER_POOL_DURATION = 684168, //  Pool Duration
        TAG_POWER_REFLECT_DAMAGE_HEALTH_PERCENT_CAP = 361544, //  Reflect Damage Health Percent Cap
        TAG_POWER_DOT_DURATION = 548864, //  DOT Duration
        TAG_POWER_SUMMONED_GHOUL_DECAY_TIME = 337408, //  Summoned Ghoul Decay Time
        TAG_POWER_LIGHTNING_SPEED_DURATION = 659456, //  Lightning Speed Duration
        TAG_POWER_COLD_RESISTANCE = 330224, //  Cold Damage Resistance
        TAG_POWER_BLEED_DURATION = 329920, //  Bleed Duration
        TAG_POWER_SLOW_TIME_DURATION = 332928, //  Slow Time Duration
        TAG_POWER_TORNADO_MOVE_SPEED = 684808, //  Tornado Move Speed
        TAG_POWER_DODGE_TRAVEL_SPEED = 331936, //  Dodge Travel Speed
        TAG_POWER_ARCANE_DAMAGE_PERCENT = 330336, //  Arcane Damage Percent
        TAG_POWER_SUMMONED_ACTOR_LIFE_DURATION = 331120, //  Summoned Actor Life Duration
        TAG_POWER_MASTABLASTA_DRAIN_DISCIPLINE_PERCENT = 708720, //  MastaBlasta Drain Discipline Percent
        TAG_POWER_FEASTOFSOULS_INITIAL_TARGET_SEARCH_RADIUS = 369664, //  Feast of Souls Initial Target Search Radius
        TAG_POWER_ATTACK_SPEED_PERCENT_INCREASE_DELTA = 331984, //  Attack Speed Percent Increase Delta
        TAG_POWER_CASTING_EFFECT_GROUP_2 = 264193, //  Casting Effect Group - Male 2
        TAG_POWER_SPECIAL_WALK_PERTURB_DESTINATION = 332360, //  Special Walk Perturb Destination
        TAG_POWER_REDUCED_HEALING_RECEIVED_PERCENT = 700449, //  Reduced Healing Received Percent
        TAG_POWER_OVERRIDE_HIT_EFFECTS = 684032, //  Override Hit Effects
        TAG_POWER_SPAWNED_ACTOR = 262415, //  Spawned Actor
        TAG_POWER_DODGE_TRAVEL_DISTANCE_DELTA = 332400, //  Dodge Travel Distance Delta
        TAG_POWER_MIRROR_IMAGE_BONUS_LIFE_PERCENT = 333280, //  Mirror Image Bonus Life Percent
        TAG_POWER_DEATH_PORTAL_LIMIT_TO_PLAYERS_IN_GAME = 720992, //  Death Portal Limit To Players In Game
        TAG_POWER_EFFECT_ACTOR_SNO = 262440, //  Effect Actor
        TAG_BLIND_DURATION_DELTA = 684578, //  Blind Duration Delta
        TAG_POWER_ROOT_IS_UNTARGETABLE = 606720, //  Root Is Untargetable
        TAG_POWER_ROOT_GRAB_POWER = 606208, //  Root Grab Power
        TAG_POWER_LIGHTNING_SPEED_MOVEMENT_SCALAR_BONUS = 661504, //  Lightning Speed Movement Scalar Bonus
        TAG_POWER_TARGET_NEUTRAL = 328144, //  TargetNeutral
        TAG_POWER_DEATH_STATUE_GHOUL_DOT_CHANCE = 338944, //  Death Statue Ghoul Dot Chance
        TAG_POWER_POISON_CLOUD_INTERVAL_DURATION = 330976, //  Poison Cloud Interval Duration
        TAG_POWER_SUFFOCATION_PER_SECOND = 708608, //  Suffocation Per Second
        TAG_POWER_LIGHTNING_RESISTANCE = 330160, //  Lightning Damage Resistance
        TAG_POWER_HIT_CHANCE = 329856, //  Hit Chance
        TAG_POWER_TRIGGERS_HIT_PROCS = 330640, //  Triggers Hit Procs and Thorns
        TAG_POWER_HITPOINTS_TO_HEAL = 331264, //  Hitpoints Granted By Heal
        TAG_POWER_POISON_DAMAGE_PERCENT = 330272, //  Poison Damage Percent
        TAG_POWER_CHARGED_BOLT_NUM_BOLTS = 331056, //  Charged Bolt Num Bolts
        TAG_POWER_SPIRIT_GAIN = 684928, //  Spirit Gained
        TAG_POWER_QUICKSAND_DURATION = 631296, //  Quicksand Duration
        TAG_POWER_CHARGEUP_ANIMATION = 262657, //  Animation Charge Up
        TAG_POWER_TORNADO_DAMAGE_PULSE_RATE = 684801, //  Tornado Damage Pulse Rate
        TAG_POWER_SAND_SHIELD_DURATION = 630784, //  Sand Shield Duration
        TAG_POWER_LEAP_DIST_MIN = 708616, //  Leap Distance Min
        TAG_POWER_GROUND_IMPACT_SNO = 262402, //  Ground Impact Actor
        TAG_POWER_MANA_DRAIN_AMOUNT_MIN = 331200, //  Mana Drain Amount Min
        TAG_POWER_SHRINE_BUFF_BONUS = 633601, //  Shrine Buff Bonus
        TAG_POWER_CAUSES_KNOCKBACK = 331697, //  Causes Knockback
        TAG_POWER_SAND_WALL_DEFLECTION_JITTER = 684913, //  Sand Wall Deflection Jitter
        TAG_POWER_SPECIAL_WALK_TRAJECTORY_GRAVITY = 332336, //  Special Walk Trajectory Gravity
        TAG_POWER_ANATOMY_CRIT_BONUS_PERCENT = 413696, //  Anatomy Crit Bonus Percent
        TAG_POWER_STATIC_CHARGE_DURATION = 664064, //  Static Charge Duration
        TAG_POWER_DEATH_PORTAL_ACTOR_2 = 720928, //  Death Portal Actor 2
        TAG_POWER_FIRE_DURATION = 329976, //  Fire Duration
        TAG_POWER_INTENSIFY_CRIT_DURATION = 667904, //  Intensify Crit Duration
        TAG_POWER_FALLEN_MAX_SUMMON_COUNT = 633360, //  Fallen Max Count
        TAG_POWER_SHIELD_DAMAGE_ABSORB_PERCENT = 332641, //  Percent of Damage That Shield Can Absorb
        TAG_POWER_ALWAYS_HITS = 327904, //  AlwaysHits
        TAG_POWER_SLOW_DURATION_DELTA = 330912, //  Slow Duration Delta
        TAG_POWER_HIT_EFFECT = 684034, //  Hit Effect
        TAG_POWER_KNOCKBACK_MAGNITUDE = 331696, //  Knockback Magnitude
        TAG_POWER_DOT_MAX_STACK_COUNT = 329981, //  Dot Max Stack Count
        TAG_POWER_FIRE_RESISTANCE = 330096, //  Fire Damage Resistance
        TAG_POWER_PAYLOAD_PARAM_1 = 329792, //  Payload Param 1
        TAG_POWER_DISEASE_DAMAGE_DELTA = 330576, //  Disease Damage Delta
        TAG_POWER_CURSE_DURATION = 332800, //  Curse Duration
        TAG_POWER_EXPEL_MONSTER_SPAWN_INTERVAL = 688640, //  Expel Monster Spawn Interval
        TAG_POWER_FURY_DEGENERATION_REDUCED_DECAY_THRESHOLD = 332186, //  Fury Reduced Degen. threshold (fraction)
        TAG_POWER_FIREGRENADE_COUNTDOWN = 632832, //  Fire Grenade Countdown
        TAG_POWER_POOL_PULSE_INTERVAL = 684176, //  Pool Pulse Interval
        TAG_POWER_FIREBATS_NUM_BATS = 600320, //  Fire Bats Num Bats
        TAG_POWER_BURN_HOUSE_WATER_HEAL_AMOUNT = 682752, //  Burn House Water Heal Amount
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_1_WITH_PARAM = 655634, //  Generic Buff Attribute 1 And Parameter
        TAG_POWER_ELECTROCUTE_AOE_CHANCE = 332256, //  Electrocute Chance of AOE Attack on Target Death
        TAG_POWER_DESTINATION_JITTER_RADIUS = 360448, //  Destination Jitter Radius
        TAG_POWER_NUMBER_OF_MISSILES = 331856, //  Number of Missiles
        TAG_POWER_BUFF_DURATION_MIN = 332736, //  Buff Duration Min
        TAG_POWER_SIEGE_RESISTANCE = 330480, //  Siege Damage Resistance
        TAG_POWER_SUMMONED_ACTOR_LEVEL = 331136, //  Summoned Actor Level
        TAG_POWER_CONTACT_FRAME_EFFECT_GROUP_FEMALE_2 = 264277, //  Contact Frame Effect Group - Female 2
        TAG_POWER_SUMMONED_ANIMATION = 332451, //  Summoned Animation Tag
        TAG_POWER_DISEASE_DURATION = 329912, //  Disease Duration
        TAG_POWER_MIRROR_IMAGE_DOUBLE_CHANCE = 333288, //  Mirror Image Double Chance
        TAG_POWER_ALT_EXPLOSION_SNO = 262448, //  Alternate Explosion Actor
        TAG_POWER_PIERCING_SHOT_PIERCE_COUNT = 632064, //  Piercing Shot Pierce Count
        TAG_POWER_SHRINE_BUFF_RADIUS = 633603, //  Shrine Buff Radius
        TAG_POWER_FIREBATS_REFRESH_RATE = 600321, //  Fire Bats Refresh Rate
        TAG_POWER_GHOST_SOULSIPHON_MAX_CHANNELLING_TIME = 364800, //  Ghost Soulsiphon Max Channelling Time
        TAG_POWER_IS_PHYSICAL = 328624, //  Is Physical
        TAG_POWER_WEB_DURATION_DELTA = 330848, //  Web Duration Delta
        TAG_POWER_FEAR_DURATION_DELTA = 331632, //  Fear Duration Delta
        TAG_POWER_GENERIC_BUFF_WEAPON_CLASS_REQUIREMENT_OFFHAND = 655505, //  Generic Buff Weapon Class Requirement Offhand
        TAG_POWER_ZOMBIFY_DEBUFF_DURATION = 594432, //  Zombify Debuff Duration
        TAG_POWER_BONEREAVER_HEAL_DURATION = 615937, //  Bonereaver Consume Heal Buff Duration
        TAG_POWER_PHYSICAL_RESISTANCE = 330032, //  Physical Damage Resistance
        TAG_POWER_TARGET_IMPACT_SNO = 262405, //  Target Impact Particle
        TAG_POWER_HOLY_DAMAGE_DELTA = 330512, //  Holy Damage Delta
        TAG_POWER_GENERIC_EFFECT_GROUP_0 = 262661, //  Generic Effect Group 0
        TAG_POWER_ZOMBIFY_MAX_POSSESSED = 593920, //  Zombify Max Possessed
        TAG_POWER_ICE_ARMOR_DEFENSE_BONUS_PERCENT = 655729, //  Ice Armor Defense Bonus Percent
        TAG_POWER_TORNADO_RADIUS = 684800, //  Tornado Damage Radius
        TAG_POWER_PROJECTILE_REFLECT_CHANCE = 679936, //  Projectile Reflect Chance
        TAG_POWER_COLD_DAMAGE_MIN = 330176, //  Cold Damage Min
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_1 = 655632, //  Generic Buff Attribute 1
        TAG_POWER_SUMMONED_GHOUL_MAX_COUNT = 337152, //  Summoned Ghoul Max Count
        TAG_POWER_FROZEN_STACKED_DURATION_MAX = 635649, //  Frozen Debuff Stacked Duration Max
        TAG_POWER_WHIRLWIND_DURATION_MIN = 361504, //  Whirlwind Duration Min
        TAG_POWER_CONDUCTION_AURA_BASE_REFRESH_RATE = 332912, //  Conduction Aura Base Refresh Rate
        TAG_POWER_TORNADO_LIFE_DURATION_DELTA = 332672, //  Tornado Life Duration Delta
        TAG_POWER_PAYLOAD_PIE_USE_ACTOR_FACING = 721328, //  Payload Pie Use Actor Facing
        TAG_POWER_ARCANE_ORB_NUM_GRENADES = 331072, //  Arcane Orb Num Grenades
        TAG_POWER_SAND_WALL_DURATION = 684912, //  Sand Wall Duration
        TAG_POWER_MASTABLASTA_DRAIN_ARCANUM_PERCENT = 708640, //  MastaBlasta Drain Arcanum Percent
        TAG_POWER_WALL_FLOOR_EXPLOSION_SNO = 262410, //  Projectile Wall/Floor Explosion
        TAG_POWER_CONCENTRATION_DURATION = 333088, //  Concentration Duration
        TAG_POWER_DESTRUCTABLE_OBJECT_DAMAGE_DELAY = 618496, //  Destructable Object Damage Delay
        TAG_POWER_CASTING_EFFECT_GROUP_FEMALE = 264273, //  Casting Effect Group - Female
        TAG_POWER_ROCKWORM_BURST_OUT_DISTANCE_MAX = 369408, //  Rockworm Burst Out Distance Max
        TAG_POWER_SHRINE_BUFF_ALLIES = 633602, //  Shrine Buff Allies
        TAG_POWER_SHRINE_BUFF_ATTRIBUTE = 633600, //  Shrine Buff Atttribute
        TAG_POWER_SUMMONING_MACHINE_NODE_IS_INVULNERABLE = 704512, //  Summoning Machine Node Is Invulnerable
        TAG_POWER_ENRAGE_AMPLIFY_DAMAGE_PERCENT = 361728, //  Enrage Amplify Damage Percent
        TAG_POWER_STUN_DURATION_MIN = 330784, //  Stun Duration Min
        TAG_POWER_BONUS_HITPOINT_PERCENT = 331568, //  Bonus Hitpoint Percent
        TAG_POWER_CRITICAL_HIT_EFFECT = 684033, //  Critical Hit Effect
        TAG_POWER_BOUNCING_BALL_LIFETIME = 680832, //  Bouncing Ball Lifetime
        TAG_POWER_PROJECTILE_MIN_TARGET_DISTANCE = 331192, //  Projectile Min Target Distance
        TAG_POWER_HEARTH_TIME = 643072, //  Hearth Time
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_3 = 655665, //  Generic Buff Attribute 3 Formula
        TAG_POWER_SAND_SHIELD_DODGE_CHANCE_BONUS = 630787, //  Sand Shield Dodge Chance
        TAG_POWER_SKELETON_MAX_SUMMON_COUNT = 332449, //  Skeleton Summon Max Count
        TAG_POWER_GENERIC_EFFECT_GROUP_1 = 263680, //  Generic Effect Group 1
        TAG_POWER_FEIGN_DEATH_AT_HEALTH_PERCENT = 431360, //  Feign Death At Health Percent
        TAG_POWER_FAITH_GENERATION_BONUS_PERCENT = 700436, //  Faith Generation Bonus Percent
        TAG_POWER_HOOKSHOT_MISS_RETURN_SPEED_MULT = 610560, //  Hookshot Miss Return Speed Mult
        TAG_POWER_LIGHTNING_DAMAGE_MIN = 330112, //  Lightning Damage Min
        TAG_POWER_DEATH_STATUE_DAMAGE = 338688, //  Death Statue Damage
        TAG_POWER_TELEPORT_INVULNERABLE_SECS = 409600, //  Teleport Invulnerable Secs
        TAG_POWER_DODGE_DURATION = 332848, //  Dodge Duration
        TAG_POWER_CONCENTRATION_COOLDOWN = 332608, //  Concentration Cooldown
        TAG_POWER_ARCANE_RESISTANCE = 330352, //  Arcane Damage Resistance
        TAG_POWER_CRIT_CHANCE = 331008, //  Crit Chance
        TAG_POWER_STATIC_CHARGE_DAMAGE_INTERVAL = 663808, //  Static Charge Damage Interval
        TAG_POWER_SAND_WALL_DURATION_DELTA = 684915, //  Sand Wall Duration Delta
        TAG_POWER_FROZEN_DURATION = 635648, //  Frozen Debuff Duration
        TAG_POWER_DEAD_TIME_UNTIL_RESURRECT = 332784, //  Paladin Dead Time Until Resurrect
        TAG_POWER_MASTABLASTA_DRAIN_MANA_PERCENT = 708624, //  MastaBlasta Drain Mana Percent
        TAG_POWER_SLOW_TIME_PROJECTILE_SPEED_MULTIPLIER = 332512, //  Slow Time Projectile Speed Multiplier
        TAG_POWER_DEATH_PORTAL_WORLD_MONSTER_POWER = 721008, //  Death Portal World Monster Power
        TAG_POWER_SPECIAL_WALK_TRAJECTORY_GRAVITY_DELTA = 332337, //  Special Walk Trajectory Gravity Delta
        TAG_POWER_SPELL_FUNC_0 = 327712, //  SpellFunc 0
        TAG_POWER_ENDING_EFFECT_GROUP = 262662, //  Ending Effect Group
        TAG_POWER_GENERIC_BUFF_WEAPON_CLASS_REQUIREMENT = 655504, //  Generic Buff Weapon Class Requirement
        TAG_POWER_EXPEL_MONSTER_COUNT = 688384, //  Expel Monster Count
        TAG_BLIND_DURATION_MIN = 684577, //  Blind Duration Min
        TAG_POWER_PORTAL_MIN_RANGE = 685057, //  Portal Spawn Range Min
        TAG_POWER_SOURCE_DEST_EFFECT_GROUP = 262659, //  Source-Dest Effect Group
        TAG_POWER_PROJECTILE_SCALE_VELOCITY = 262435, //  Projectile Scale Velocity
        TAG_POWER_STONESPIKE_NUMBER_OF_SPIKES = 372736, //  Stone Spike Number of Spikes
        TAG_POWER_HIT_CHANCE_DECREASE = 684579, //  Hit Chance Decrease
        TAG_POWER_POISON_CLOUD_NUM_INTERVALS = 330992, //  Poison Cloud Num Intervals
        TAG_POWER_RETALIATION_KNOCKBACK_MAGNITUDE = 331648, //  Retaliation Knockback Magnitude
        TAG_POWER_ARCANUM_GAINED_PER_SECOND = 700432, //  Arcanum Gained Per Second
        TAG_POWER_FIRE_DAMAGE_MIN = 330048, //  Fire Damage Min
        TAG_POWER_LOBBED_PROJECTILE_GRAVITY = 329872, //  Projectile Gravity
        TAG_POWER_GHOST_SOULSIPHON_MAX_CHANNELLING_DISTANCE = 365056, //  Ghost Soulsiphon Max Channelling Distance
        TAG_POWER_HITPOINTS_TO_HEAL_PERCENT = 331280, //  Hitpoints to Heal Percent
        TAG_POWER_ELECTROCUTE_ATTACK_DELAY_TIME = 332064, //  Electrocute Attack Delay Time
        TAG_POWER_DODGE_TRAVEL_ANGLE_OFFSET = 332544, //  Dodge Travel Angle Offset
        TAG_POWER_POISON_RESISTANCE = 330288, //  Poison Damage Resistance
        TAG_POWER_INTENSIFY_CRIT_CHANCE_BONUS = 667648, //  Intensify Crit Chance Bonus
        TAG_POWER_DESTINATION_JITTER_ATTEMPTS = 360704, //  Destination Jitter Attempts
        TAG_POWER_SPAWN_DURATION = 684914, //  Spawn Duration
        TAG_POWER_ROCKWORM_BURST_OUT_DELAY = 369536, //  Rockworm Burst Out Delay
        TAG_POWER_THUNDERING_CRY_BUFF_DURATION = 332720, //  Thundering Cry Buff Duration
        TAG_POWER_SIEGE_DAMAGE_MIN = 330432, //  Siege Damage Min
        TAG_POWER_MANA_DRAIN_AMOUNT_DELTA = 331216, //  Mana Drain Amount Delta
        TAG_POWER_SKELETON_SUMMON_COUNT = 332448, //  Skeleton Summon Count Per Summon
        TAG_POWER_DODGE_CHANCE = 332849, //  Dodge Chance
        TAG_POWER_DEATH_PORTAL_ACTOR_3 = 720944, //  Death Portal Actor 3
        TAG_POWER_PROC_CHANCE = 680704, //  Proc Chance
        TAG_POWER_FEASTOFSOULS_DURATION = 370176, //  Feast of Souls Duration
        TAG_POWER_PROJECTILE_SNO = 262400, //  Projectile Actor
        TAG_POWER_SLOW_MULTIPLIER = 692224, //  Slow Movement Speed Multiplier
        TAG_POWER_NOVA_DELAY = 655985, //  Nova Delay
        TAG_POWER_SLOW_TIME_ATTACK_COOLDOWN_INCREASE = 332496, //  Slow Time Attack Cooldown Increase In Seconds
        TAG_POWER_PORTAL_ACTIVATION_DIST = 685056, //  Portal Activate Range
        TAG_POWER_SUMMONED_GHOUL_DISEASE_ATTACK_CHANCE = 337664, //  Summoned Ghoul Disease Attack Chance
        TAG_POWER_DEATH_PORTAL_WORLD_PLAYER_POWER = 721009, //  Death Portal World Player Power
        TAG_POWER_SPECIAL_WALK_TRAJECTORY_HEIGHT_DELTA = 332321, //  Special Walk Trajectory Height Delta
        TAG_POWER_TARGET_ALLIES = 328096, //  TargetAllies
        TAG_POWER_SLOW_AMOUNT = 330928, //  Slow Amount
        TAG_POWER_BONUS_MANA_PERCENT = 331584, //  Bonus Mana Percent
        TAG_POWER_DOT_POWER = 329982, //  Dot Power
        TAG_POWER_PHYSICAL_DAMAGE_MIN = 329984, //  Physical Damage Min
        TAG_POWER_DISEASE_DAMAGE_PERCENT = 330592, //  Disease Damage Percent
        TAG_POWER_MASSCONFUSION_BUFF_DURATION = 614400, //  Mass Confusion Buff Duration
        TAG_POWER_KNOCKBACK_HEIGHT_DELTA = 331705, //  Knockback Height Delta
        TAG_POWER_DEBUFF_REFRESH_INTERVAL_IN_SECONDS = 655681, //  Debuff Refresh Interval In Seconds
        TAG_POWER_FURY_COEFFICIENT = 332180, //  Fury Coefficient
        TAG_POWER_SPECTRALBLADE_BLEED_DURATION = 397312, //  Spectral Blade Bleed Duration
        TAG_POWER_SAND_SHIELD_PROJECTILE_REFLECT_CHANCE = 630785, //  Sand Shield Projectile Reflect Chance
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_2_WITH_PARAM = 655650, //  Generic Buff Attribute 2 And Parameter
        TAG_POWER_SUMMON_CHANCE = 339456, //  Summon Chance
        TAG_POWER_BATTLE_RAGE_MAX_STACK_LEVEL = 332048, //  Battle Rage Max Stack Level
        TAG_POWER_ATTACK_SPEED_PERCENT_INCREASE_MIN = 331968, //  Attack Speed Percent Increase Min
        TAG_POWER_TORNADO_LIFE_DURATION_MIN = 332656, //  Tornado Life Duration Min
        TAG_POWER_QUICKSAND_SLOWAMOUNT = 631297, //  Quicksand Slow Amount
        TAG_POWER_SPECIAL_WALK_GO_THROUGH_OCCLUDED = 332341, //  Special Walk Go Through Occluded
        TAG_POWER_GOLD_FIND_BONUS = 331152, //  Gold Find Bonus
        TAG_POWER_FURY_GENERATION_BONUS_PERCENT = 700434, //  Fury Generation Bonus Percent
        TAG_POWER_SPIRIT_GENERATION_BONUS_PERCENT = 700433, //  Spirit Generation Bonus Percent
        TAG_POWER_DOESNT_CRIT = 639488, //  Doesn't Crit
        TAG_POWER_PROJECTILE_JITTER = 331196, //  Projectile Jitter
        TAG_POWER_DODGE_TRAVEL_DISTANCE_MIN = 332384, //  Dodge Travel Distance Min
        TAG_POWER_DEATH_PORTAL_WORLD_QUEST_DESCRIPTION = 721010, //  Death Portal World Quest Description
        TAG_POWER_DEATH_NOVA_RADIUS_MAX = 638976, //  Death Nova Radius Max
        TAG_POWER_KNOCKBACK_HEIGHT_MIN = 331704, //  Knockback Height Min
        TAG_POWER_MAX_TARGETS = 329800, //  Max Targets
        TAG_POWER_ROOT_END_FUNC = 606464, //  Root End Func
        TAG_POWER_EXPLOSION_SNO = 262401, //  Explosion Actor
        TAG_POWER_PROJECTILE_SPREAD_ANGLE = 331194, //  Projectile Spread Angle
        TAG_POWER_TEMPORAL_ARMOR_COOLDOWN = 681728, //  Temporal Armor Cooldown
        TAG_POWER_FINDITEM_MAGIC_FIND_BONUS = 421888, //  Find Item Magic Find Bonus
        TAG_POWER_IMMOBILIZE_DURATION_MIN = 330864, //  Immobilize Duration Min
        TAG_POWER_MANA_GAINED_PER_SECOND = 331520, //  Mana Gained Per Second
        TAG_POWER_MASTABLASTA_DRAIN_FURY_PERCENT = 708656, //  MastaBlasta Drain Fury Percent
        TAG_POWER_TARGET_IGNORE_WRECKABLES = 328168, //  TargetIgnoreWreckables
        TAG_POWER_HOLY_DAMAGE_PERCENT = 330528, //  Holy Damage Percent
        TAG_POWER_BONEREAVER_HEAL_PERCENTAGE = 615936, //  Bonereaver Heal Percentage
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_FORMULA_0 = 655617, //  Generic Buff Attribute 0 Formula
        TAG_POWER_RAPTORSPEED_DURATION = 631552, //  Raptor Speed Duration
        TAG_POWER_DURATION_DELTA = 684930, //  Duration Delta
        TAG_POWER_RAMPAGE_EXTRA_HITS = 631040, //  Rampage Extra Attacks
        TAG_POWER_IMPENETRABLE_DEFENSE_DURATION = 684224, //  Impenetrable Defense Duration
        TAG_POWER_COLD_DAMAGE_DELTA = 330192, //  Cold Damage Delta
        TAG_POWER_GENERIC_BUFF_ATTRIBUTE_2 = 655648, //  Generic Buff Attribute 2
        TAG_POWER_PORTAL_DELTA_RANGE = 685058, //  Portal Spawn Range Delta
        TAG_POWER_CAUSES_KNOCKDOWN = 633345, //  Causes Knockdown
        TAG_POWER_BLIZZARD_TIME_BETWEEN_UPDATES = 333024, //  Blizzard Time Between Updates
        TAG_POWER_TEMPLAR_PROTECTION_DAMAGE_ABSORB_PERCENT = 331904, //  Templar Devotion Damage Absorb Percent
        TAG_POWER_RING_OF_FROST_SLOW_DURATION = 332592, //  Ring of Frost Slow Duration
        TAG_POWER_ARCANE_DAMAGE_MIN = 330304, //  Arcane Damage Min
        TAG_POWER_PAYLOAD_AFFECTED_BY_PET_AOE_SCALAR = 331088, //  Payload Affected By Pet AOE Scalar
        TAG_POWER_CASTING_EFFECT_GROUP = 262660, //  Casting Effect Group - Male
        TAG_POWER_SPECIAL_WALK_TRAJECTORY_HEIGHT = 332320, //  Special Walk Trajectory Height
        TAG_POWER_ROCKWORM_WEB_SPIT_DISTANCE = 368640, //  Rockworm Web Spit Distance
        TAG_POWER_CONTACT_FRAME_EFFECT_GROUP_2 = 264194, //  Contact Frame Effect Group - Male 2
        TAG_POWER_ROOT_TIMER_MODIFICATION_PER_STRUGGLE = 360960, //  Root Timer Modification Per Struggle
        TAG_POWER_CRIT_DAMAGE_BONUS_PERCENT = 332368, //  Crit Damage Bonus Percent
        TAG_POWER_DEATH_PORTAL_WORLD_QUEST_TITLE = 721011, //  Death Portal World Quest Title
        TAG_POWER_DEATH_PORTAL_DELAY = 720960, //  Death Portal Delay
        TAG_POWER_PROJECTILE_THROW_OVER = 262408, //  Projectile Throw Over Guys
        TAG_POWER_NEVER_CAUSES_RECOIL = 327968, //  NeverCausesRecoil
        TAG_POWER_STUN_DURATION_DELTA = 330800, //  Stun Duration Delta
        TAG_POWER_STATIC_CHARGE_DAMAGE = 663552, //  Static Charge Damage

        TAG_POWER_RUNEE_DAMAGE_TYPE = 721269, //  RuneE Damage Type, = 721269
        TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_0 = 264304, // Combo 0 Contact Frame Effect Group - Male, = 264304
        TAG_POWER_TEMPLATE_RUNE_D = 327684, // Template Rune D, = 327684
        TAG_POWER_BUFF_5_ICON = 270597, // Buff 5 Icon, = 270597
        TAG_POWER_BUFF_5_HARMFUL_BUFF = 270853, // Buff 5 Is Harmful, = 270853
        TAG_POWER_BUFF_0_SHOW_DURATION_ON_SKILL_BUTTON = 274688, // Buff 0 Show Duration On Skill Button, = 274688
        TAG_POWER_BUFF_0_SHOW_IN_BUFF_HOLDER = 274432, // Buff 0 Show In Buff Holder, = 274432
        TAG_POWER_COMBO_ANIMATION_3 = 262914, // Combo Animation 3, = 262914
        TAG_POWER_BUFF_7_EFFECT_GROUP = 270343, // Buff 7 Effect Group, = 270343
        TAG_POWER_ANIMATION_TAG_2 = 262658, // Animation Tag 2, = 262658
        TAG_POWER_BUFF_7_ICON = 270599, // Buff 7 Icon, = 270599
        TAG_POWER_SCRIPT_FORMULA_10 = 266752, // Script Formula 10, = 266752
        TAG_POWER_BUFF_14_MERGES_TOOLTIP = 271892, // Buff 14 Merges Tooltip, = 271892
        TAG_POWER_BUFF_17_MERGES_TOOLTIP = 271895, // Buff 17 Merges Tooltip, = 271895
        TAG_POWER_LOS_CHECK = 328720, // LOS Check, = 328720
        TAG_POWER_SCRIPT_FORMULA_59 = 267920, // Script Formula 59, = 267920
        TAG_POWER_BUFF_9_SHOW_IN_BUFF_HOLDER = 274441, // Buff 9 Show In Buff Holder, = 274441
        TAG_POWER_SCRIPT_FORMULA_49 = 267664, // Script Formula 49, = 267664
        TAG_POWER_APPLY_PASSIVE_AFTER_ITEM_PASSIVES = 717568, // Apply Passive After Item Passives, = 717568
        TAG_POWER_BUFF_30_SHOW_IN_BUFF_HOLDER = 274462, // Buff 30 Show In Buff Holder, = 274462
        TAG_POWER_BUFF_15_IS_DISPLAYED = 271637, // Buff 15 Is Displayed, = 271637
        TAG_POWER_COMBO_LEVEL_1_ON_HIT_COEFFICIENT = 329828, // Combo Level 1 On Hit Proc Coefficient, = 329828
        TAG_POWER_ANIMATION_TAG_RUNE_C = 262675, // Animation Tag Rune C, = 262675
        TAG_POWER_BUFF_16_EFFECT_GROUP = 270358, // Buff 16 Effect Group, = 270358
        TAG_POWER_USES_MAINHAND_ONLY = 328656, // Uses Mainhand Only, = 328656
        TAG_POWER_BUFF_16_SHOW_DURATION = 271126, // Buff 16 Show Duration, = 271126
        TAG_POWER_BUFF_18_SHOW_ACTIVE_ON_SKILL_BUTTON = 272658, // Buff 18 Show Active On Skill Button, = 272658
        TAG_POWER_START_WALK_AFTER_INTRO = 328288, // Start Walk After Intro, = 328288
        TAG_POWER_BUFF_24_SHOW_DURATION_ON_SKILL_BUTTON = 274712, // Buff 24 Show Duration On Skill Button, = 274712
        TAG_POWER_BUFF_24_SHOW_IN_BUFF_HOLDER = 274456, // Buff 24 Show In Buff Holder, = 274456
        TAG_POWER_USE_CHARGE_STEERING = 328542, // Use Charge Steering, = 328542
        TAG_POWER_COMBO_SPELL_FUNC_END_2 = 262945, // Combo SpellFunc End 2, = 262945
        TAG_POWER_BUFF_27_DONT_APPLY_VISUAL_TO_PETS = 272411, // Buff 27 Dont Apply Visual To Pets, = 272411
        TAG_POWER_BUFF_29_MERGE_TOOLTIP_INDEX = 272169, // Buff 29 Merge Tooltip Index, = 272169
        TAG_POWER_SCRIPT_FORMULA_52 = 267808, // Script Formula 52, = 267808
        TAG_POWER_WALKING_SPEED_MULTIPLIER = 331952, // Walking Speed Multiplier, = 331952
        TAG_POWER_BUFF_29_EFFECT_GROUP = 270377, // Buff 29 Effect Group, = 270377
        TAG_POWER_SCROLL_BUFF_EXCLUSIVE_TYPE = 721360, // Scroll Buff Exclusive Type, = 721360
        TAG_POWER_SCRIPT_FORMULA_42 = 267552, // Script Formula 42, = 267552
        TAG_POWER_BUFF_21_ICON = 270625, // Buff 21 Icon, = 270625
        TAG_POWER_BUFF_20_MERGE_TOOLTIP_INDEX = 272160, // Buff 20 Merge Tooltip Index, = 272160
        TAG_POWER_IS_CANCELLABLE_BY_WALKING = 328546, // Is Cancellable By Walking, = 328546
        TAG_POWER_BUFF_22_PLAYER_CAN_CANCEL = 271394, // Buff 22 Player Can Cancel, = 271394
        TAG_POWER_BUFF_22_MERGE_TOOLTIP_INDEX = 272162, // Buff 22 Merge Tooltip Index, = 272162
        TAG_POWER_COMBO_ANIMATION_3_RUNE_E = 263221, // Combo Animation 3 Rune E, = 263221
        TAG_POWER_SCRIPT_FORMULA_17 = 266864, // Script Formula 17, = 266864
        TAG_POWER_CONSOLE_MIN_CLIENT_WALK_SPEED = 713488, // Min Client Walk Speed, = 713488
        TAG_POWER_CAST_TARGET_CORPSE = 328129, // CastTargetCorpse, = 328129
        TAG_POWER_BUFF_5_MERGES_TOOLTIP = 271877, // Buff 5 Merges Tooltip, = 271877
        TAG_POWER_IS_BASIC_ATTACK = 327808, // IsBasicAttack, = 327808
        TAG_POWER_BUFF_5_MERGE_TOOLTIP_INDEX = 272133, // Buff 5 Merge Tooltip Index, = 272133
        TAG_POWER_RUNEB_COMBO2_PROC_SCALAR = 721201, // RuneB Combo2 Proc Scalar, = 721201
        TAG_POWER_IS_UPGRADE = 329216, // Is Upgrade, = 329216
        TAG_POWER_ON_HIT_PROC_COEFFICIENT = 329983, // On Hit Proc Coefficient, = 329983
        TAG_POWER_BUFF_4_SHOW_DURATION_ON_SKILL_BUTTON = 274692, // Buff 4 Show Duration On Skill Button, = 274692
        TAG_POWER_COMBO_CASTING_EFFECT_GROUP_FEMALE_0 = 264320, // Combo 0 Casting Effect Group - Female, = 264320
        TAG_POWER_FAILS_IF_FEARED = 328325, // FailsIfFeared, = 328325
        TAG_POWER_BUFF_13_SHOW_IN_BUFF_HOLDER = 274445, // Buff 13 Show In Buff Holder, = 274445
        TAG_POWER_SCRIPT_FORMULA_34 = 267328, // Script Formula 34, = 267328
        TAG_POWER_SCRIPT_FORMULA_24 = 267072, // Script Formula 24, = 267072
        TAG_POWER_AI_ACTION_DURATION_DELTA = 332880, // AI Action Duration Delta, = 332880
        TAG_POWER_BUFF_11_SHOW_ACTIVE_ON_SKILL_BUTTON = 272651, // Buff 11 Show Active On Skill Button, = 272651
        TAG_POWER_BUFF_20_SHOW_ACTIVE_ON_SKILL_BUTTON = 272660, // Buff 20 Show Active On Skill Button, = 272660
        TAG_POWER_RUN_IN_FRONT_DISTANCE = 565248, // Run In Front Distance, = 565248
        TAG_POWER_SCRIPT_FORMULA_9 = 266640, // Script Formula 9, = 266640
        TAG_POWER_RUNED_COMBO3_PROC_SCALAR = 721234, // RuneD Combo3 Proc Scalar, = 721234
        TAG_POWER_BUFF_12_SHOW_ACTIVE_ON_SKILL_BUTTON = 272652, // Buff 12 Show Active On Skill Button, = 272652
        TAG_POWER_BUFF_8_PLAYER_CAN_CANCEL = 271368, // Buff 8 Player Can Cancel, = 271368
        TAG_POWER_BUFF_21_SHOW_ACTIVE_ON_SKILL_BUTTON = 272661, // Buff 21 Show Active On Skill Button, = 272661
        TAG_POWER_SCRIPT_FORMULA_51 = 267792, // Script Formula 51, = 267792
        TAG_POWER_BUFF_8_SHOW_DURATION = 271112, // Buff 8 Show Duration, = 271112
        TAG_POWER_SCRIPT_FORMULA_41 = 267536, // Script Formula 41, = 267536
        TAG_POWER_BUFF_28_SHOW_IN_BUFF_HOLDER = 274460, // Buff 28 Show In Buff Holder, = 274460
        TAG_POWER_BUFF_16_MERGE_TOOLTIP_INDEX = 272150, // Buff 16 Merge Tooltip Index, = 272150
        TAG_POWER_COMBO_ANIMATION_1_RUNE_B = 263186, // Combo Animation 1 Rune B, = 263186
        TAG_POWER_BUFF_19_IS_DISPLAYED = 271641, // Buff 19 Is Displayed, = 271641
        TAG_POWER_BUFF_16_MERGES_TOOLTIP = 271894, // Buff 16 Merges Tooltip, = 271894
        TAG_POWER_BUFF_19_MERGES_TOOLTIP = 271897, // Buff 19 Merges Tooltip, = 271897
        TAG_POWER_BUFF_13_ICON = 270611, // Buff 13 Icon, = 270611
        TAG_POWER_BUFF_13_MERGES_TOOLTIP = 271891, // Buff 13 Merges Tooltip, = 271891
        TAG_POWER_CAN_USE_WHEN_DEAD = 328528, // Can Use When Dead, = 328528
        TAG_POWER_BUFF_26_ICON = 270630, // Buff 26 Icon, = 270630
        TAG_POWER_ARC_MOVE_UNTIL_DEST_HEIGHT = 328296, // Arc Move Until Dest Height, = 328296
        TAG_POWER_SCRIPT_FORMULA_2 = 266528, // Script Formula 2, = 266528
        TAG_POWER_REQUIRES_TARGET = 328432, // RequiresTarget, = 328432
        TAG_POWER_BUFF_28_HARMFUL_BUFF = 270888, // Buff 28 Is Harmful, = 270888
        TAG_POWER_BUFF_22_SHOW_DURATION = 271138, // Buff 22 Show Duration, = 271138
        TAG_POWER_RUNED_COMBO1_PROC_SCALAR = 721232, // RuneD Combo1 Proc Scalar, = 721232
        TAG_POWER_BUFF_22_HARMFUL_BUFF = 270882, // Buff 22 Is Harmful, = 270882
        TAG_POWER_BUFF_31_MERGE_TOOLTIP_INDEX = 272177, // Buff 31 Merge Tooltip Index, = 272177
        TAG_POWER_NO_INTERRUPT_TIMER = 684880, // No Interrupt Timer, = 684880
        TAG_POWER_WALKING_DISTANCE_MIN = 331960, // Walking Distance Min, = 331960
        TAG_POWER_BUFF_31_EFFECT_GROUP = 270385, // Buff 31 Effect Group, = 270385
        TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_1 = 264305, // Combo 1 Contact Frame Effect Group - Male, = 264305
        TAG_POWER_IMMUNE_TO_STUN_DURING = 682242, // Immune to Stun during, = 682242
        TAG_POWER_TURNS_INTO_BASIC_ATTACK_MELEE = 328386, // TurnsIntoBasicMeleeAttack, = 328386
        TAG_POWER_CONTACT_FREEZES_FACING = 327984, // ContactFreezesFacing, = 327984
        TAG_POWER_TEMPLATE = 327680, // Template, = 327680
        TAG_POWER_BUFF_1_ICON = 270593, // Buff 1 Icon, = 270593
        TAG_POWER_BUFF_0_MERGE_TOOLTIP_INDEX = 272128, // Buff 0 Merge Tooltip Index, = 272128
        TAG_POWER_LOOPING_ANIMATION_TIME = 263296, // Looping Animation Time, = 263296
        TAG_POWER_BUFF_2_PLAYER_CAN_CANCEL = 271362, // Buff 2 Player Can Cancel, = 271362
        TAG_POWER_BUFF_2_MERGE_TOOLTIP_INDEX = 272130, // Buff 2 Merge Tooltip Index, = 272130
        TAG_POWER_IS_COMBO_POWER = 264448, // IsComboPower, = 264448
        TAG_POWER_SNAPS_TO_FACING = 328021, // SnapsToFacing, = 328021
        TAG_POWER_RUNEE_PROC_SCALAR = 721157, // RuneE Proc Scalar, = 721157
        TAG_POWER_BUFF_11_SHOW_IN_BUFF_HOLDER = 274443, // Buff 11 Show In Buff Holder, = 274443
        TAG_POWER_BUFF_6_PLAYER_CAN_CANCEL = 271366, // Buff 6 Player Can Cancel, = 271366
        TAG_POWER_RUNEC_COMBO2_PROC_SCALAR = 721217, // RuneC Combo2 Proc Scalar, = 721217
        TAG_POWER_BUFF_3_SHOW_DURATION = 271107, // Buff 3 Show Duration, = 271107
        TAG_POWER_BUFF_3_EFFECT_GROUP = 270339, // Buff 3 Effect Group, = 270339
        TAG_POWER_BUFF_10_DONT_APPLY_VISUAL_TO_PETS = 272394, // Buff 10 Dont Apply Visual To Pets, = 272394
        TAG_POWER_BUFF_9_SHOW_ACTIVE_ON_SKILL_BUTTON = 272649, // Buff 9 Show Active On Skill Button, = 272649
        TAG_POWER_SCRIPT_FORMULA_1 = 266512, // Script Formula 1, = 266512
        TAG_POWER_COMBO_TEMPLATE_3 = 262930, // Combo Template 3, = 262930
        TAG_POWER_BUFF_19_HARMFUL_BUFF = 270873, // Buff 19 Is Harmful, = 270873
        TAG_POWER_ANIMATION_TAG_RUNE_B = 262674, // Animation Tag Rune B, = 262674
        TAG_POWER_BUFF_19_SHOW_DURATION = 271129, // Buff 19 Show Duration, = 271129
        TAG_POWER_BUFF_26_SHOW_IN_BUFF_HOLDER = 274458, // Buff 26 Show In Buff Holder, = 274458
        TAG_POWER_COMBO_ATTACK_RADIUS_1 = 329809, // Combo Attack Radius 1, = 329809
        TAG_POWER_ONLY_USABLE_IN_TOWN_PORTAL_AREAS = 328082, // IsOnlyUsableInTownPortalAreas, = 328082
        TAG_POWER_BUFF_12_EFFECT_GROUP = 270354, // Buff 12 Effect Group, = 270354
        TAG_POWER_CUSTOM_TARGET_FUNC = 328736, // Custom Target Func, = 328736
        TAG_POWER_BUFF_27_MERGES_TOOLTIP = 271911, // Buff 27 Merges Tooltip, = 271911
        TAG_POWER_BUFF_18_SHOW_DURATION_ON_SKILL_BUTTON = 274706, // Buff 18 Show Duration On Skill Button, = 274706
        TAG_POWER_SPECIALWALK_PLAYER_END_ANIM_SCALAR = 328536, // SpecialWalk Player End Anim Scalar, = 328536
        TAG_POWER_BUFF_25_IS_DISPLAYED = 271653, // Buff 25 Is Displayed, = 271653
        TAG_POWER_BUFF_24_SHOW_DURATION = 271140, // Buff 24 Show Duration, = 271140
        TAG_POWER_BREAKS_SNARE = 681985, // Breaks Snare, = 681985
        TAG_POWER_CUSTOM_TARGET_BUFF_SNO_BUFF_INDEX_2 = 328807, // Custom Target Buff Power SNO Buff Index 2, = 328807
        TAG_POWER_BREAKS_IMMOBILIZE = 328304, // BreaksImmobilize, = 328304
        TAG_POWER_PROC_TARGETS_SELF = 328440, // ProcTargetsSelf, = 328440
        TAG_POWER_SCRIPT_FORMULA_33 = 267312, // Script Formula 33, = 267312
        TAG_POWER_SCRIPT_FORMULA_23 = 267056, // Script Formula 23, = 267056
        TAG_POWER_COMBO_SPELL_FUNC_INTERRUPTED_2 = 262961, // Combo SpellFunc Interrupted 2, = 262961
        TAG_POWER_BUFF_5_SHOW_IN_BUFF_HOLDER = 274437, // Buff 5 Show In Buff Holder, = 274437
        TAG_POWER_MONSTER_GENERIC_SUMMON_TAGS_INDEX = 721376, // Monster Generic Summon Tags Index, = 721376
        TAG_POWER_BUFF_30_MERGE_TOOLTIP_INDEX = 272176, // Buff 30 Merge Tooltip Index, = 272176
        TAG_POWER_BUFF_2_SHOW_DURATION = 271106, // Buff 2 Show Duration, = 271106
        TAG_POWER_COMBO_CASTING_EFFECT_GROUP_FEMALE_1 = 264321, // Combo 1 Casting Effect Group - Female, = 264321
        TAG_POWER_BUFF_2_HARMFUL_BUFF = 270850, // Buff 2 Is Harmful, = 270850
        TAG_POWER_CONSOLE_PREFERS_RADIAL_TARGETING = 713219, // Controller Prefers Radial Targetting, = 713219
        TAG_POWER_BUFF_14_SHOW_ACTIVE_ON_SKILL_BUTTON = 272654, // Buff 14 Show Active On Skill Button, = 272654
        TAG_POWER_COMBO_ANIMATION_1 = 262912, // Combo Animation 1, = 262912
        TAG_POWER_CAST_TARGET_NEUTRAL = 328145, // CastTargetNeutral, = 328145
        TAG_POWER_BUFF_7_SHOW_DURATION = 271111, // Buff 7 Show Duration, = 271111
        TAG_POWER_BUFF_6_SHOW_ACTIVE_ON_SKILL_BUTTON = 272646, // Buff 6 Show Active On Skill Button, = 272646
        TAG_POWER_BUFF_20_SHOW_IN_BUFF_HOLDER = 274452, // Buff 20 Show In Buff Holder, = 274452
        TAG_POWER_BUFF_3_PLAYER_CAN_CANCEL = 271363, // Buff 3 Player Can Cancel, = 271363
        TAG_POWER_NEVER_UPDATES_FACING_DURING = 328000, // NeverUpdatesFacing, = 328000
        TAG_POWER_IS_DISPLAYED = 327824, // IsDisplayed, = 327824
        TAG_POWER_BUFF_3_IS_DISPLAYED = 271619, // Buff 3 Is Displayed, = 271619
        TAG_POWER_UPGRADE_0 = 329232, // Upgrade 0, = 329232
        TAG_POWER_USES_WEAPON_RANGE = 328607, // Uses Weapon Range, = 328607
        TAG_POWER_BUFF_9_PLAYER_CAN_CANCEL = 271369, // Buff 9 Player Can Cancel, = 271369
        TAG_POWER_TARGET_PATHABLE_ONLY = 328165, // TargetPathableOnly, = 328165
        TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_FEMALE_0 = 264336, // Combo 0 Contact Frame Effect Group - Female, = 264336
        TAG_POWER_BUFF_22_SHOW_DURATION_ON_SKILL_BUTTON = 274710, // Buff 22 Show Duration On Skill Button, = 274710
        TAG_POWER_BUFF_21_SHOW_DURATION_ON_SKILL_BUTTON = 274709, // Buff 21 Show Duration On Skill Button, = 274709
        TAG_POWER_BUFF_25_DONT_APPLY_VISUAL_TO_PETS = 272409, // Buff 25 Dont Apply Visual To Pets, = 272409
        TAG_POWER_DAMAGE_DISPLAY_POWER = 627456, // Damage Display Power, = 627456
        TAG_POWER_COMBO_LEVEL_3_ON_HIT_COEFFICIENT = 329830, // Combo Level 3 On Hit Proc Coefficient, = 329830
        TAG_POWER_DOESNT_PREPLAY_ANIMATION = 328611, // Doesnt Preplay Animation, = 328611
        TAG_POWER_IS_CANCELLABLE_BY_ANY_POWER = 328545, // Is Cancellable By Any Power, = 328545
        TAG_POWER_BUFF_17_DONT_APPLY_VISUAL_TO_PETS = 272401, // Buff 17 Dont Apply Visual To Pets, = 272401
        TAG_POWER_RUNEE_COMBO3_PROC_SCALAR = 721250, // RuneE Combo3 Proc Scalar, = 721250
        TAG_POWER_FOLLOW_WALK_ANIM_TAG = 561424, // Follow Walk Anim Tag, = 561424
        TAG_POWER_BUFF_18_PLAYER_CAN_CANCEL = 271384, // Buff 18 Player Can Cancel, = 271384
        TAG_POWER_USES_MAINHAND_ONLY_COMBO2 = 329832, // Combo Level 2 Uses Main Hand Only, = 329832
        TAG_POWER_BUFF_18_SHOW_DURATION = 271128, // Buff 18 Show Duration, = 271128
        TAG_POWER_COMBO_CASTING_EFFECT_GROUP_2 = 264290, // Combo 2 Casting Effect Group - Male, = 264290
        TAG_POWER_COMBO_ANIMATION_2_RUNE_B = 263202, // Combo Animation 2 Rune B, = 263202
        TAG_POWER_BUFF_29_IS_DISPLAYED = 271657, // Buff 29 Is Displayed, = 271657
        TAG_POWER_CONSOLE_FIRES_ON_BUTTON_UP_ONLY = 713220, // Controller Fires On Button Up, = 713220
        TAG_POWER_BUFF_29_MERGES_TOOLTIP = 271913, // Buff 29 Merges Tooltip, = 271913
        TAG_POWER_IS_CANCELLABLE = 328544, // Is Cancellable, = 328544
        TAG_POWER_USES_POWER_FALLBACK_QUEUE = 717312, // Uses Power Fallback Queue, = 717312
        TAG_POWER_IS_INVISIBLE_DURING = 340141, // Is Invisible During, = 340141
        TAG_POWER_FAILS_IF_IMMOBILIZED = 328320, // FailsIfImmobilized, = 328320
        TAG_POWER_BUFF_5_IS_DISPLAYED = 271621, // Buff 5 Is Displayed, = 271621
        TAG_POWER_BUFF_4_SHOW_DURATION = 271108, // Buff 4 Show Duration, = 271108
        TAG_POWER_RUNEE_COMBO1_PROC_SCALAR = 721248, // RuneE Combo1 Proc Scalar, = 721248
        TAG_POWER_BUFF_2_SHOW_ACTIVE_ON_SKILL_BUTTON = 272642, // Buff 2 Show Active On Skill Button, = 272642
        TAG_POWER_SCRIPT_FORMULA_54 = 267840, // Script Formula 54, = 267840
        TAG_POWER_IMMUNE_TO_ROOT_DURING = 682240, // Immune to Root during, = 682240
        TAG_POWER_SCRIPT_FORMULA_44 = 267584, // Script Formula 44, = 267584
        TAG_POWER_BUFF_3_SHOW_ACTIVE_ON_SKILL_BUTTON = 272643, // Buff 3 Show Active On Skill Button, = 272643
        TAG_POWER_BUFF_11_DONT_APPLY_VISUAL_TO_PETS = 272395, // Buff 11 Dont Apply Visual To Pets, = 272395
        TAG_POWER_ICON_NORMAL = 329472, // Icon Normal, = 329472
        TAG_POWER_BUFF_17_IS_DISPLAYED = 271639, // Buff 17 Is Displayed, = 271639
        TAG_POWER_BUFF_17_HARMFUL_BUFF = 270871, // Buff 17 Is Harmful, = 270871
        TAG_POWER_CHECK_PATH_THEN_CLIP_TO_PATHABLE_ATTACK_RADIUS = 684850, // Check Path Then Clip To Pathable Attack Radius, = 684850
        TAG_POWER_SPELL_FUNC_BEGIN = 327696, // SpellFunc Begin, = 327696
        TAG_POWER_SCRIPT_FORMULA_19 = 266896, // Script Formula 19, = 266896
        TAG_POWER_BUFF_10_MERGE_TOOLTIP_INDEX = 272144, // Buff 10 Merge Tooltip Index, = 272144
        TAG_POWER_BUFF_8_MERGES_TOOLTIP = 271880, // Buff 8 Merges Tooltip, = 271880
        TAG_POWER_BUFF_8_IS_DISPLAYED = 271624, // Buff 8 Is Displayed, = 271624
        TAG_POWER_DOESNT_REQUIRE_ACTOR_TARGET = 328615, // TargetDoesntRequireActor, = 328615
        TAG_POWER_BUFF_28_SHOW_DURATION_ON_SKILL_BUTTON = 274716, // Buff 28 Show Duration On Skill Button, = 274716
        TAG_POWER_BUFF_16_PLAYER_CAN_CANCEL = 271382, // Buff 16 Player Can Cancel, = 271382
        TAG_POWER_RUNED_COMBO2_PROC_SCALAR = 721233, // RuneD Combo2 Proc Scalar, = 721233
        TAG_POWER_BUFF_11_PLAYER_CAN_CANCEL = 271377, // Buff 11 Player Can Cancel, = 271377
        TAG_POWER_BUFF_13_SHOW_DURATION = 271123, // Buff 13 Show Duration, = 271123
        TAG_POWER_BUFF_13_EFFECT_GROUP = 270355, // Buff 13 Effect Group, = 270355
        TAG_POWER_BUFF_26_DONT_APPLY_VISUAL_TO_PETS = 272410, // Buff 26 Dont Apply Visual To Pets, = 272410
        TAG_POWER_COMBO_SPELL_FUNC_END_3 = 262946, // Combo SpellFunc End 3, = 262946
        TAG_POWER_CANNOT_DIE_DURING = 340142, // Cannot Die During, = 340142
        TAG_POWER_BUFF_29_HARMFUL_BUFF = 270889, // Buff 29 Is Harmful, = 270889
        TAG_POWER_BUFF_29_SHOW_DURATION = 271145, // Buff 29 Show Duration, = 271145
        TAG_POWER_SCRIPT_FORMULA_12 = 266784, // Script Formula 12, = 266784
        TAG_POWER_BUFF_20_PLAYER_CAN_CANCEL = 271392, // Buff 20 Player Can Cancel, = 271392
        TAG_POWER_BUFF_20_SHOW_DURATION = 271136, // Buff 20 Show Duration, = 271136
        TAG_POWER_COMBO_ATTACK_SPEED_1 = 329825, // Combo Attack Speed 1, = 329825
        TAG_POWER_BUFF_22_EFFECT_GROUP = 270370, // Buff 22 Effect Group, = 270370
        TAG_POWER_CUSTOM_TARGET_PLAYERS_ONLY = 328752, // Custom Target Players Only, = 328752
        TAG_POWER_BUFF_31_IS_DISPLAYED = 271665, // Buff 31 Is Displayed, = 271665
        TAG_POWER_BUFF_31_MERGES_TOOLTIP = 271921, // Buff 31 Merges Tooltip, = 271921
        TAG_POWER_BUFF_5_SHOW_ACTIVE_ON_SKILL_BUTTON = 272645, // Buff 5 Show Active On Skill Button, = 272645
        TAG_POWER_BUFF_1_SHOW_IN_BUFF_HOLDER = 274433, // Buff 1 Show In Buff Holder, = 274433
        TAG_POWER_SCALED_ANIMATION_TIMING = 328192, // ScaledAnimTiming, = 328192
        TAG_POWER_BUFF_0_SHOW_ACTIVE_ON_SKILL_BUTTON = 272640, // Buff 0 Show Active On Skill Button, = 272640
        TAG_POWER_BUFF_0_DONT_APPLY_VISUAL_TO_PETS = 272384, // Buff 0 Dont Apply Visual To Pets, = 272384
        TAG_POWER_BUFF_7_DONT_APPLY_VISUAL_TO_PETS = 272391, // Buff 7 Dont Apply Visual To Pets, = 272391
        TAG_POWER_SCRIPT_FORMULA_4 = 266560, // Script Formula 4, = 266560
        TAG_POWER_BUFF_7_SHOW_ACTIVE_ON_SKILL_BUTTON = 272647, // Buff 7 Show Active On Skill Button, = 272647
        TAG_POWER_IS_EMOTE = 264704, // IsEmote, = 264704
        TAG_POWER_COST_RESOURCE_MIN_TO_CAST = 329617, // Resource Cost Min To Cast, = 329617
        TAG_POWER_BUFF_14_SHOW_DURATION_ON_SKILL_BUTTON = 274702, // Buff 14 Show Duration On Skill Button, = 274702
        TAG_POWER_BUFF_6_SHOW_IN_BUFF_HOLDER = 274438, // Buff 6 Show In Buff Holder, = 274438
        TAG_POWER_SPELL_FUNC_INTERRUPTED = 327745, // SpellFunc Interrupted, = 327745
        TAG_POWER_BUFF_16_SHOW_DURATION_ON_SKILL_BUTTON = 274704, // Buff 16 Show Duration On Skill Button, = 274704
        TAG_POWER_BUFF_16_SHOW_IN_BUFF_HOLDER = 274448, // Buff 16 Show In Buff Holder, = 274448
        TAG_POWER_BUFF_8_ICON = 270600, // Buff 8 Icon, = 270600
        TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_FEMALE_1 = 264337, // Combo 1 Contact Frame Effect Group - Female, = 264337
        TAG_POWER_BUFF_8_EFFECT_GROUP = 270344, // Buff 8 Effect Group, = 270344
        TAG_POWER_SCRIPT_FORMULA_11 = 266768, // Script Formula 11, = 266768
        TAG_POWER_BUFF_30_SHOW_ACTIVE_ON_SKILL_BUTTON = 272670, // Buff 30 Show Active On Skill Button, = 272670
        TAG_POWER_FOLLOW_START_DISTANCE = 561152, // Follow Start Distance, = 561152
        TAG_POWER_COMBO_TEMPLATE_1 = 262928, // Combo Template 1, = 262928
        TAG_POWER_CAST_TARGET_GROUND_ONLY = 328161, // CastTargetGroundOnly, = 328161
        TAG_POWER_BUFF_19_ICON = 270617, // Buff 19 Icon, = 270617
        TAG_POWER_BUFF_22_SHOW_ACTIVE_ON_SKILL_BUTTON = 272662, // Buff 22 Show Active On Skill Button, = 272662
        TAG_POWER_BREAKS_STUN = 681986, // Breaks Stun, = 681986
        TAG_POWER_COST_RESOURCE = 329616, // Resource Cost, = 329616
        TAG_POWER_BUFF_13_PLAYER_CAN_CANCEL = 271379, // Buff 13 Player Can Cancel, = 271379
        TAG_POWER_NEVER_UPDATES_FACING_STARTING = 328016, // NeverUpdatesFacingStarting, = 328016
        TAG_POWER_BUFF_25_SHOW_IN_BUFF_HOLDER = 274457, // Buff 25 Show In Buff Holder, = 274457
        TAG_POWER_IS_OFFENSIVE = 327840, // IsOffensive, = 327840
        TAG_POWER_BUFF_13_IS_DISPLAYED = 271635, // Buff 13 Is Displayed, = 271635
        TAG_POWER_BUFF_17_SHOW_DURATION_ON_SKILL_BUTTON = 274705, // Buff 17 Show Duration On Skill Button, = 274705
        TAG_POWER_BUFF_26_HARMFUL_BUFF = 270886, // Buff 26 Is Harmful, = 270886
        TAG_POWER_UPGRADE_1 = 329248, // Upgrade 1, = 329248
        TAG_POWER_BUFF_26_IS_DISPLAYED = 271654, // Buff 26 Is Displayed, = 271654
        TAG_POWER_BUFF_23_HARMFUL_BUFF = 270883, // Buff 23 Is Harmful, = 270883
        TAG_POWER_IS_KNOCKBACK_MOVEMENT = 327860, // IsKnockbackMovement, = 327860
        TAG_POWER_SCRIPT_FORMULA_36 = 267360, // Script Formula 36, = 267360
        TAG_POWER_SCRIPT_FORMULA_26 = 267104, // Script Formula 26, = 267104
        TAG_POWER_CUSTOM_TARGET_BUFF_SNO_BUFF_INDEX = 328801, // Custom Target Buff Power SNO Buff Index, = 328801
        TAG_POWER_BUFF_31_HARMFUL_BUFF = 270897, // Buff 31 Is Harmful, = 270897
        TAG_POWER_RUNEB_DAMAGE_TYPE = 721266, // RuneB Damage Type, = 721266
        TAG_POWER_BUFF_31_SHOW_DURATION = 271153, // Buff 31 Show Duration, = 271153
        TAG_POWER_SCRIPT_FORMULA_53 = 267824, // Script Formula 53, = 267824
        TAG_POWER_SCRIPT_FORMULA_43 = 267568, // Script Formula 43, = 267568
        TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_2 = 264306, // Combo 2 Contact Frame Effect Group - Male, = 264306
        TAG_POWER_COMBO_ANIMATION_3_RUNE_B = 263218, // Combo Animation 3 Rune B, = 263218
        TAG_POWER_CUSTOM_TARGET_MIN_RANGE = 328768, // Custom Target Min Range, = 328768
        TAG_POWER_CANCELS_OTHER_POWERS = 328560, // Cancels Other Powers, = 328560
        TAG_POWER_CONSOLE_USES_TARGET_RETICLE_WHEN_HELD = 713221, // Controller Uses Target Reticle When Held, = 713221
        TAG_POWER_BUFF_0_PLAYER_CAN_CANCEL = 271360, // Buff 0 Player Can Cancel, = 271360
        TAG_POWER_BUFF_0_SHOW_DURATION = 271104, // Buff 0 Show Duration, = 271104
        TAG_POWER_BUFF_2_EFFECT_GROUP = 270338, // Buff 2 Effect Group, = 270338
        TAG_POWER_BUFF_13_DONT_APPLY_VISUAL_TO_PETS = 272397, // Buff 13 Dont Apply Visual To Pets, = 272397
        TAG_POWER_ANIMATION_TAG_TURN_LEFT = 263424, // Animation Tag Turn Left, = 263424
        TAG_POWER_SCRIPT_FORMULA_60 = 268032, // Script Formula 60, = 268032
        TAG_POWER_IMMUNE_TO_SNARE_DURING = 682241, // Immune to Snare during, = 682241
        TAG_POWER_RECOIL_IMMUNE = 328336, // ImmuneToRecoil, = 328336
        TAG_POWER_BUFF_20_SHOW_DURATION_ON_SKILL_BUTTON = 274708, // Buff 20 Show Duration On Skill Button, = 274708
        TAG_POWER_BUFF_3_MERGE_TOOLTIP_INDEX = 272131, // Buff 3 Merge Tooltip Index, = 272131
        TAG_POWER_BUFF_14_SHOW_DURATION = 271124, // Buff 14 Show Duration, = 271124
        TAG_POWER_BUFF_3_SHOW_DURATION_ON_SKILL_BUTTON = 274691, // Buff 3 Show Duration On Skill Button, = 274691
        TAG_POWER_CONTROLLER_AUTO_TARGETS_LT = 622594, // Controller Auto Targets W/LT, = 622594
        TAG_POWER_NORUNE_DAMAGE_TYPE = 721264, // NoRune Damage Type, = 721264
        TAG_POWER_COST_RESOURCE_REDUCTION_COEFFICIENT = 329625, // Resource Cost Reduction Coefficient, = 329625
        TAG_POWER_BUFF_29_SHOW_IN_BUFF_HOLDER = 274461, // Buff 29 Show In Buff Holder, = 274461
        TAG_POWER_SCRIPT_FORMULA_35 = 267344, // Script Formula 35, = 267344
        TAG_POWER_SCRIPT_FORMULA_25 = 267088, // Script Formula 25, = 267088
        TAG_POWER_RESOURCE_GAINED_ON_FIRST_HIT = 329627, // Resource Gained On First Hit, = 329627
        TAG_POWER_BUFF_15_EFFECT_GROUP = 270357, // Buff 15 Effect Group, = 270357
        TAG_POWER_BUFF_28_DONT_APPLY_VISUAL_TO_PETS = 272412, // Buff 28 Dont Apply Visual To Pets, = 272412
        TAG_POWER_BUFF_19_SHOW_ACTIVE_ON_SKILL_BUTTON = 272659, // Buff 19 Show Active On Skill Button, = 272659
        TAG_POWER_CONSOLE_AUTO_TARGET_LT_ENABLED = 713217, // Controller Auto Targeting LT Enabled, = 713217
        TAG_POWER_ICON_MOUSEOVER = 329488, // Icon Mouseover, = 329488
        TAG_POWER_BUFF_27_IS_DISPLAYED = 271655, // Buff 27 Is Displayed, = 271655
        TAG_POWER_CUSTOM_TARGET_BUFF_SNO = 328802, // Custom Target Buff Power SNO, = 328802
        TAG_POWER_BUFF_27_HARMFUL_BUFF = 270887, // Buff 27 Is Harmful, = 270887
        TAG_POWER_CAST_TARGET_IGNORE_WRECKABLES = 328169, // CastTargetIgnoreWreckables, = 328169
        TAG_POWER_COST_RESOURCE_REDUCTION = 329624, // Resource Cost Reduction, = 329624
        TAG_POWER_BUFF_18_MERGES_TOOLTIP = 271896, // Buff 18 Merges Tooltip, = 271896
        TAG_POWER_BUFF_18_IS_DISPLAYED = 271640, // Buff 18 Is Displayed, = 271640
        TAG_POWER_BUFF_24_EFFECT_GROUP = 270372, // Buff 24 Effect Group, = 270372
        TAG_POWER_TARGET_GROUND_ONLY = 328160, // TargetGroundOnly, = 328160
        TAG_POWER_BUFF_24_MERGE_TOOLTIP_INDEX = 272164, // Buff 24 Merge Tooltip Index, = 272164
        TAG_POWER_RUNEE_COMBO2_PROC_SCALAR = 721249, // RuneE Combo2 Proc Scalar, = 721249
        TAG_POWER_BUFF_21_PLAYER_CAN_CANCEL = 271393, // Buff 21 Player Can Cancel, = 271393
        TAG_POWER_CUSTOM_TARGET_PREFERRED_RANGE = 328803, // Custom Target Preferred Range Only, = 328803
        TAG_POWER_IMMUNE_TO_FEAR_DURING = 682243, // Immune to Fear during, = 682243
        TAG_POWER_SCRIPT_FORMULA_3 = 266544, // Script Formula 3, = 266544
        TAG_POWER_CONSOLE_TARGET_THETA_GROWTH_PER_KILL = 712960, // , = 712960
        TAG_POWER_COMBO_SPELL_FUNC_INTERRUPTED_3 = 262962, // Combo SpellFunc Interrupted 3, = 262962
        TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_B = 328643, // Special Death Type Rune B, = 328643
        TAG_POWER_NO_RECAST_UNTIL_LAST_ANIM_FRAME = 328577, // No Recast Until Last Anim Frame, = 328577
        TAG_POWER_BUFF_30_PLAYER_CAN_CANCEL = 271408, // Buff 30 Player Can Cancel, = 271408
        TAG_POWER_BUFF_30_SHOW_DURATION = 271152, // Buff 30 Show Duration, = 271152
        TAG_POWER_BUFF_1_DONT_APPLY_VISUAL_TO_PETS = 272385, // Buff 1 Dont Apply Visual To Pets, = 272385
        TAG_POWER_COMBO_CASTING_EFFECT_GROUP_FEMALE_2 = 264322, // Combo 2 Casting Effect Group - Female, = 264322
        TAG_POWER_CANCELS_AT_INTERRUPT_FRAME = 328576, // Cancels At Interrupt Frame, = 328576
        TAG_POWER_BUFF_3_DONT_APPLY_VISUAL_TO_PETS = 272387, // Buff 3 Dont Apply Visual To Pets, = 272387
        TAG_POWER_BUFF_17_MERGE_TOOLTIP_INDEX = 272151, // Buff 17 Merge Tooltip Index, = 272151
        TAG_POWER_BUFF_10_SHOW_DURATION_ON_SKILL_BUTTON = 274698, // Buff 10 Show Duration On Skill Button, = 274698
        TAG_POWER_ATTACK_RADIUS = 329808, // Attack Radius, = 329808
        TAG_POWER_INFINITE_ANIMATION_TIMING = 328208, // InfiniteAnimTiming, = 328208
        TAG_POWER_BUFF_17_PLAYER_CAN_CANCEL = 271383, // Buff 17 Player Can Cancel, = 271383
        TAG_POWER_BUFF_9_SHOW_DURATION_ON_SKILL_BUTTON = 274697, // Buff 9 Show Duration On Skill Button, = 274697
        TAG_POWER_BUFF_9_MERGE_TOOLTIP_INDEX = 272137, // Buff 9 Merge Tooltip Index, = 272137
        TAG_POWER_BUFF_16_SHOW_ACTIVE_ON_SKILL_BUTTON = 272656, // Buff 16 Show Active On Skill Button, = 272656
        TAG_POWER_BUFF_9_EFFECT_GROUP = 270345, // Buff 9 Effect Group, = 270345
        TAG_POWER_BUFF_16_DONT_APPLY_VISUAL_TO_PETS = 272400, // Buff 16 Dont Apply Visual To Pets, = 272400
        TAG_POWER_PLAYER_CRIT_DAMAGE_SCALAR = 684917, // Player Crit Damage Scalar, = 684917
        TAG_POWER_COMBO_ANIMATION_1_RUNE_A = 263185, // Combo Animation 1 Rune A, = 263185
        TAG_POWER_ICON_ROUND = 716800, // Icon Round, = 716800
        TAG_POWER_EMOTE_CONVERSATION_SNO = 264720, // Emote Conversation SNO, = 264720
        TAG_POWER_ONLY_FREE_CAST = 329633, // Only Free Cast, = 329633
        TAG_POWER_GHOST_WALLWALK_EFFECT_SNO = 721280, // Ghost Wallwalk Effect, = 721280
        TAG_POWER_BUFF_18_DONT_APPLY_VISUAL_TO_PETS = 272402, // Buff 18 Dont Apply Visual To Pets, = 272402
        TAG_POWER_BUFF_18_ICON = 270616, // Buff 18 Icon, = 270616
        TAG_POWER_SENDS_FAILURE_REASON_TO_CLIENT = 328602, // Send Failure Reason To Client, = 328602
        TAG_POWER_BUFF_18_EFFECT_GROUP = 270360, // Buff 18 Effect Group, = 270360
        TAG_POWER_COMBO_SPELL_FUNC_END_1 = 262944, // Combo SpellFunc End 1, = 262944
        TAG_POWER_TRACKS_AFFECTED_ACD_INSTANCE_WIDE = 328177, // TracksAffectedACDInstanceWide, = 328177
        TAG_POWER_BUFF_29_ICON = 270633, // Buff 29 Icon, = 270633
        TAG_POWER_BUFF_28_MERGE_TOOLTIP_INDEX = 272168, // Buff 28 Merge Tooltip Index, = 272168
        TAG_POWER_COST_HEALTH = 329632, // Health Cost, = 329632
        TAG_POWER_DOESNT_CENTER = 328032, // Doesn't Center, = 328032
        TAG_POWER_BUFF_20_MERGES_TOOLTIP = 271904, // Buff 20 Merges Tooltip, = 271904
        TAG_POWER_IS_TRANSLATE = 327856, // IsTranslate, = 327856
        TAG_POWER_BUFF_20_IS_DISPLAYED = 271648, // Buff 20 Is Displayed, = 271648
        TAG_POWER_COST_ONLY_FREE_CAST = 329634, // Cost Only Free Cast, = 329634
        TAG_POWER_UPGRADE_2 = 329264, // Upgrade 2, = 329264
        TAG_POWER_MODAL_CURSOR_RADIUS = 328069, // Modal Cursor Radius, = 328069
        TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_D = 328647, // Special Death Type Rune D, = 328647
        TAG_POWER_BUFF_4_EFFECT_GROUP = 270340, // Buff 4 Effect Group, = 270340
        TAG_POWER_BUFF_4_MERGE_TOOLTIP_INDEX = 272132, // Buff 4 Merge Tooltip Index, = 272132
        TAG_POWER_LOOPING_SUPPRESS_ITEM_TOOLTIPS = 263300, // Looping Suppress Item Tooltips, = 263300
        TAG_POWER_BUFF_1_PLAYER_CAN_CANCEL = 271361, // Buff 1 Player Can Cancel, = 271361
        TAG_POWER_RUNEB_PROC_SCALAR = 721154, // RuneB Proc Scalar, = 721154
        TAG_POWER_PICKUP_ITEMS_WHILE_MOVING = 655984, // Pickup Items While Moving, = 655984
        TAG_POWER_TURNS_INTO_BASIC_ATTACK_RANGED = 328385, // TurnsIntoBasicRangedAttack, = 328385
        TAG_POWER_BUFF_13_SHOW_DURATION_ON_SKILL_BUTTON = 274701, // Buff 13 Show Duration On Skill Button, = 274701
        TAG_POWER_SCRIPT_FORMULA_14 = 266816, // Script Formula 14, = 266816
        TAG_POWER_BUFF_6_ICON = 270598, // Buff 6 Icon, = 270598
        TAG_POWER_CUSTOM_TARGET_MAX_RANGE = 328784, // Custom Target Max Range, = 328784
        TAG_POWER_BUFF_17_EFFECT_GROUP = 270359, // Buff 17 Effect Group, = 270359
        TAG_POWER_AFFECTED_BY_DUAL_WIELD = 328448, // AffectedByDualWield, = 328448
        TAG_POWER_BUFF_17_ICON = 270615, // Buff 17 Icon, = 270615
        TAG_POWER_TARGETING_PASSABILITY_SNO = 327762, // Targeting Passability SNO, = 327762
        TAG_POWER_BUFF_10_PLAYER_CAN_CANCEL = 271376, // Buff 10 Player Can Cancel, = 271376
        TAG_POWER_BUFF_15_SHOW_IN_BUFF_HOLDER = 274447, // Buff 15 Show In Buff Holder, = 274447
        TAG_POWER_BUFF_10_SHOW_DURATION = 271120, // Buff 10 Show Duration, = 271120
        TAG_POWER_BUFF_15_SHOW_DURATION_ON_SKILL_BUTTON = 274703, // Buff 15 Show Duration On Skill Button, = 274703
        TAG_POWER_CONSOLE_USE_AT_MAX_RANGE_WHEN_HELD = 713222, // Fires At Max Range When Held, = 713222
        TAG_POWER_BUFF_8_HARMFUL_BUFF = 270856, // Buff 8 Is Harmful, = 270856
        TAG_POWER_BUFF_29_DONT_APPLY_VISUAL_TO_PETS = 272413, // Buff 29 Dont Apply Visual To Pets, = 272413
        TAG_POWER_COMBO_ANIMATION_1_RUNE_C = 263187, // Combo Animation 1 Rune C, = 263187
        TAG_POWER_TARGET_NAV_MESH_ONLY = 328163, // TargetNavMeshOnly, = 328163
        TAG_POWER_BUFF_11_MERGE_TOOLTIP_INDEX = 272145, // Buff 11 Merge Tooltip Index, = 272145
        TAG_POWER_EXTENDS_TARGET_TO_ATTACK_RADIUS = 684849, // Extends Target To Attack Radius, = 684849
        TAG_POWER_BUFF_11_EFFECT_GROUP = 270353, // Buff 11 Effect Group, = 270353
        TAG_POWER_KNOCKBACK_IMMUNE = 328352, // ImmuneToKnockback, = 328352
        TAG_POWER_COMBO_ATTACK_RADIUS_3 = 329811, // Combo Attack Radius 3, = 329811
        TAG_POWER_BUFF_13_MERGE_TOOLTIP_INDEX = 272147, // Buff 13 Merge Tooltip Index, = 272147
        TAG_POWER_BUFF_12_IS_DISPLAYED = 271634, // Buff 12 Is Displayed, = 271634
        TAG_POWER_CALL_AIUPDATE_IMMEDIATELY_UPON_TERMINATION = 328604, // Call AIUpdate Immediately Upon Termination, = 328604
        TAG_POWER_BUFF_26_EFFECT_GROUP = 270374, // Buff 26 Effect Group, = 270374
        TAG_POWER_NORUNE_PROC_SCALAR = 721152, // NoRune Proc Scalar, = 721152
        TAG_POWER_BUFF_26_SHOW_DURATION = 271142, // Buff 26 Show Duration, = 271142
        TAG_POWER_USES_ATTACK_WARMUP = 328606, // Uses Attack Warmup Time, = 328606
        TAG_POWER_AI_PACK_COOLDOWN = 327769, // AI Pack Cooldown Time, = 327769
        TAG_POWER_BUFF_25_EFFECT_GROUP = 270373, // Buff 25 Effect Group, = 270373
        TAG_POWER_USES_MAINHAND_ONLY_COMBO1 = 329831, // Combo Level 1 Uses Main Hand Only, = 329831
        TAG_POWER_SCRIPT_FORMULA_56 = 267872, // Script Formula 56, = 267872
        TAG_POWER_SCRIPT_FORMULA_46 = 267616, // Script Formula 46, = 267616
        TAG_POWER_BUFF_20_ICON = 270624, // Buff 20 Icon, = 270624
        TAG_POWER_IS_UNINTERRUPTABLE_DURING = 328610, // Is Uninterruptable During, = 328610
        TAG_POWER_BUFF_20_EFFECT_GROUP = 270368, // Buff 20 Effect Group, = 270368
        TAG_POWER_AUTOASSIGN_LOCATION = 328049, // AutoAssignLocation, = 328049
        TAG_POWER_ICON_PUSHED = 329504, // Icon Pushed, = 329504
        TAG_POWER_BUFF_31_ICON = 270641, // Buff 31 Icon, = 270641
        TAG_POWER_CHECKS_VERTICAL_MOVEMENT = 327864, // ChecksVerticalMovement, = 327864
        TAG_POWER_ANIMATION_MAX_SCALING = 263223, // Scaled Animation Timing Max, = 263223
        TAG_POWER_NO_AFFECTED_ACD = 328176, // NoAffectedACD, = 328176
        TAG_POWER_RUNEA_DAMAGE_TYPE = 721265, // RuneA Damage Type, = 721265
        TAG_POWER_CONTROLLER_MIN_RANGE_HELD = 622595, // Controller Min Range When Held, = 622595
        TAG_POWER_BUFF_0_MERGES_TOOLTIP = 271872, // Buff 0 Merges Tooltip, = 271872
        TAG_POWER_BUFF_2_SHOW_DURATION_ON_SKILL_BUTTON = 274690, // Buff 2 Show Duration On Skill Button, = 274690
        TAG_POWER_BUFF_0_IS_DISPLAYED = 271616, // Buff 0 Is Displayed, = 271616
        TAG_POWER_SCRIPT_FORMULA_38 = 267392, // Script Formula 38, = 267392
        TAG_POWER_SCRIPT_FORMULA_28 = 267136, // Script Formula 28, = 267136
        TAG_POWER_MONSTER_CRIT_DAMAGE_SCALAR = 684918, // Monster Crit Damage Scalar, = 684918
        TAG_POWER_ANIMATION_TURN_THRESHOLD = 263426, // Animation Turn Threshold, = 263426
        TAG_POWER_BUFF_7_MERGES_TOOLTIP = 271879, // Buff 7 Merges Tooltip, = 271879
        TAG_POWER_BUFF_3_SHOW_IN_BUFF_HOLDER = 274435, // Buff 3 Show In Buff Holder, = 274435
        TAG_POWER_SCRIPT_FORMULA_55 = 267856, // Script Formula 55, = 267856
        TAG_POWER_SCRIPT_FORMULA_45 = 267600, // Script Formula 45, = 267600
        TAG_POWER_COMBO_CONTACT_FRAME_EFFECT_GROUP_FEMALE_2 = 264338, // Combo 2 Contact Frame Effect Group - Female, = 264338
        TAG_POWER_SHOW_ATTACK_WITHOUT_MOVE_TIP = 327831, // ShowAttackWithoutMoveTip, = 327831
        TAG_POWER_BUFF_28_SHOW_ACTIVE_ON_SKILL_BUTTON = 272668, // Buff 28 Show Active On Skill Button, = 272668
        TAG_POWER_CLIENT_CONTROLS_FACING = 328022, // ClientControlsFacing, = 328022
        TAG_POWER_IS_PASSIVE = 328592, // Is Passive, = 328592
        TAG_POWER_BUFF_19_DONT_APPLY_VISUAL_TO_PETS = 272403, // Buff 19 Dont Apply Visual To Pets, = 272403
        TAG_POWER_MANA_PERCENT_TO_RESERVE = 331600, // Mana Percent To Reserve, = 331600
        TAG_POWER_BUFF_26_MERGE_TOOLTIP_INDEX = 272166, // Buff 26 Merge Tooltip Index, = 272166
        TAG_POWER_CLIPS_TARGET_TO_ATTACK_RADIUS = 684848, // Clips Target To Attack Radius, = 684848
        TAG_POWER_BUFF_27_MERGE_TOOLTIP_INDEX = 272167, // Buff 27 Merge Tooltip Index, = 272167
        TAG_POWER_ATTACK_SPEED = 329824, // Attack Speed, = 329824
        TAG_POWER_BUFF_26_MERGES_TOOLTIP = 271910, // Buff 26 Merges Tooltip, = 271910
        TAG_POWER_BUFF_27_PLAYER_CAN_CANCEL = 271399, // Buff 27 Player Can Cancel, = 271399
        TAG_POWER_BUFF_18_SHOW_IN_BUFF_HOLDER = 274450, // Buff 18 Show In Buff Holder, = 274450
        TAG_POWER_BUFF_23_ICON = 270627, // Buff 23 Icon, = 270627
        TAG_POWER_BUFF_23_MERGES_TOOLTIP = 271907, // Buff 23 Merges Tooltip, = 271907
        TAG_POWER_COMBO_ANIMATION_2_RUNE_A = 263201, // Combo Animation 2 Rune A, = 263201
        TAG_POWER_BUFF_27_SHOW_DURATION_ON_SKILL_BUTTON = 274715, // Buff 27 Show Duration On Skill Button, = 274715
        TAG_POWER_SCRIPT_FORMULA_6 = 266592, // Script Formula 6, = 266592
        TAG_POWER_BUFF_24_HARMFUL_BUFF = 270884, // Buff 24 Is Harmful, = 270884
        TAG_POWER_BUFF_24_ICON = 270628, // Buff 24 Icon, = 270628
        TAG_POWER_CAST_TARGET_UNDEAD_MONSTERS_ONLY = 328618, // CastTargetUndeadMonstersOnly, = 328618
        TAG_POWER_SCRIPT_FORMULA_13 = 266800, // Script Formula 13, = 266800
        TAG_POWER_ICON_INACTIVE = 329512, // Icon Inactive, = 329512
        TAG_POWER_BUFF_5_SHOW_DURATION_ON_SKILL_BUTTON = 274693, // Buff 5 Show Duration On Skill Button, = 274693
        TAG_POWER_COMBO_SPELL_FUNC_INTERRUPTED_1 = 262960, // Combo SpellFunc Interrupted 1, = 262960
        TAG_POWER_BUFF_5_EFFECT_GROUP = 270341, // Buff 5 Effect Group, = 270341
        TAG_POWER_CLASS_RESTRICTION = 329648, // Class Restriction, = 329648
        TAG_POWER_IS_MOUSE_ASSIGNABLE = 328048, // IsMouseAssignable, = 328048
        TAG_POWER_BUFF_30_MERGES_TOOLTIP = 271920, // Buff 30 Merges Tooltip, = 271920
        TAG_POWER_SPELL_FUNC_END = 327744, // SpellFunc End, = 327744
        TAG_POWER_BUFF_30_IS_DISPLAYED = 271664, // Buff 30 Is Displayed, = 271664
        TAG_POWER_BUFF_0_ICON = 270592, // Buff 0 Icon, = 270592
        TAG_POWER_BUFF_0_EFFECT_GROUP = 270336, // Buff 0 Effect Group, = 270336
        TAG_POWER_BUFF_7_SHOW_IN_BUFF_HOLDER = 274439, // Buff 7 Show In Buff Holder, = 274439
        TAG_POWER_BUFF_14_DONT_APPLY_VISUAL_TO_PETS = 272398, // Buff 14 Dont Apply Visual To Pets, = 272398
        TAG_POWER_BUFF_7_SHOW_DURATION_ON_SKILL_BUTTON = 274695, // Buff 7 Show Duration On Skill Button, = 274695
        TAG_POWER_CONSOLE_AUTO_TARGET_SEARCH_ANGLE = 712963, // Controller Auto Target Search Angle, = 712963
        TAG_POWER_ANIMATION_TAG = 262656, // Animation Tag, = 262656
        TAG_POWER_SYNERGY_POWER = 327764, // Synergy Power, = 327764
        TAG_POWER_SCRIPT_FORMULA_30 = 267264, // Script Formula 30, = 267264
        TAG_POWER_BUFF_6_DONT_APPLY_VISUAL_TO_PETS = 272390, // Buff 6 Dont Apply Visual To Pets, = 272390
        TAG_POWER_SCRIPT_FORMULA_20 = 267008, // Script Formula 20, = 267008
        TAG_POWER_CONSUMES_ITEM = 329088, // Consumes Item, = 329088
        TAG_POWER_CONTROLLER_AUTO_TARGETS = 622592, // Controller Auto Targets, = 622592
        TAG_POWER_BUFF_14_EFFECT_GROUP = 270356, // Buff 14 Effect Group, = 270356
        TAG_POWER_BUFF_14_MERGE_TOOLTIP_INDEX = 272148, // Buff 14 Merge Tooltip Index, = 272148
        TAG_POWER_TEMPLATE_RUNE_C = 327683, // Template Rune C, = 327683
        TAG_POWER_CONSOLE_REFUNDS_RESOURCES_WITHOUT_DAMAGE = 713472, // Refunds Resources Without Damage, = 713472
        TAG_POWER_BUFF_8_SHOW_ACTIVE_ON_SKILL_BUTTON = 272648, // Buff 8 Show Active On Skill Button, = 272648
        TAG_POWER_SCRIPT_FORMULA_5 = 266576, // Script Formula 5, = 266576
        TAG_POWER_NORUNE_COMBO3_PROC_SCALAR = 721170, // NoRune Combo3 Proc Scalar, = 721170
        TAG_POWER_ANIMATION_TAG_RUNE_D = 262676, // Animation Tag Rune D, = 262676
        TAG_POWER_BUFF_9_IS_DISPLAYED = 271625, // Buff 9 Is Displayed, = 271625
        TAG_POWER_IS_ITEM_POWER = 328601, // Is Item Power, = 328601
        TAG_POWER_CONTROLLER_OFFSET_FROM_AUTO_TARGET = 622597, // Controller Offset From Auto Target, = 622597
        TAG_POWER_BUFF_8_DONT_APPLY_VISUAL_TO_PETS = 272392, // Buff 8 Dont Apply Visual To Pets, = 272392
        TAG_POWER_BUFF_9_MERGES_TOOLTIP = 271881, // Buff 9 Merges Tooltip, = 271881
        TAG_POWER_BUFF_30_SHOW_DURATION_ON_SKILL_BUTTON = 274718, // Buff 30 Show Duration On Skill Button, = 274718
        TAG_POWER_BUFF_15_SHOW_DURATION = 271125, // Buff 15 Show Duration, = 271125
        TAG_POWER_CHANNELLED_LOCKS_ACTORS = 328401, // ChannelledLocksActors, = 328401
        TAG_POWER_BUFF_22_SHOW_IN_BUFF_HOLDER = 274454, // Buff 22 Show In Buff Holder, = 274454
        TAG_POWER_BUFF_15_PLAYER_CAN_CANCEL = 271381, // Buff 15 Player Can Cancel, = 271381
        TAG_POWER_BUFF_25_SHOW_ACTIVE_ON_SKILL_BUTTON = 272665, // Buff 25 Show Active On Skill Button, = 272665
        TAG_POWER_BUFF_16_ICON = 270614, // Buff 16 Icon, = 270614
        TAG_POWER_BUFF_17_SHOW_ACTIVE_ON_SKILL_BUTTON = 272657, // Buff 17 Show Active On Skill Button, = 272657
        TAG_POWER_CUSTOM_TARGET_UNAFFECTED_ONLY = 328800, // Custom Target Unaffected Only, = 328800
        TAG_POWER_BUFF_27_EFFECT_GROUP = 270375, // Buff 27 Effect Group, = 270375
        TAG_POWER_BUFF_27_ICON = 270631, // Buff 27 Icon, = 270631
        TAG_POWER_IGNORES_RANGE_ON_SHIFT_CLICK = 328600, // Ignores Range On Shift Click, = 328600
        TAG_POWER_BUFF_18_HARMFUL_BUFF = 270872, // Buff 18 Is Harmful, = 270872
        TAG_POWER_COMBO_ANIMATION_2_RUNE_C = 263203, // Combo Animation 2 Rune C, = 263203
        TAG_POWER_BUFF_24_IS_DISPLAYED = 271652, // Buff 24 Is Displayed, = 271652
        TAG_POWER_BUFF_24_PLAYER_CAN_CANCEL = 271396, // Buff 24 Player Can Cancel, = 271396
        TAG_POWER_CONSOLE_AUTO_TARGET_SEARCH_RANGE = 712964, // Controller Auto Target Search Range, = 712964
        TAG_POWER_SCRIPT_FORMULA_62 = 268064, // Script Formula 62, = 268064
        TAG_POWER_BUFF_21_MERGE_TOOLTIP_INDEX = 272161, // Buff 21 Merge Tooltip Index, = 272161
        TAG_POWER_BUFF_21_EFFECT_GROUP = 270369, // Buff 21 Effect Group, = 270369
        TAG_POWER_ROLL_TO_DESTINATION = 328368, // RollToDestination, = 328368
        TAG_POWER_COMBO_ATTACK_SPEED_3 = 329827, // Combo Attack Speed 3, = 329827
        TAG_POWER_BUFF_22_IS_DISPLAYED = 271650, // Buff 22 Is Displayed, = 271650
        TAG_POWER_CAST_TARGET_ALLOW_DEAD_TARGETS = 328620, // CastTargetAllowDeadTargets, = 328620
        TAG_POWER_NORUNE_COMBO1_PROC_SCALAR = 721168, // NoRune Combo1 Proc Scalar, = 721168
        TAG_POWER_SCRIPT_FORMULA_37 = 267376, // Script Formula 37, = 267376
        TAG_POWER_SCRIPT_FORMULA_27 = 267120, // Script Formula 27, = 267120
        TAG_POWER_BUFF_30_ICON = 270640, // Buff 30 Icon, = 270640
        TAG_POWER_BUFF_4_HARMFUL_BUFF = 270852, // Buff 4 Is Harmful, = 270852
        TAG_POWER_FAILS_IF_STUNNED = 328322, // FailsIfStunned, = 328322
        TAG_POWER_BUFF_30_EFFECT_GROUP = 270384, // Buff 30 Effect Group, = 270384
        TAG_POWER_RUN_NEARBY_DISTANCE_MIN = 332416, // Run Nearby Distance Min, = 332416
        TAG_POWER_CAN_STEER = 327937, // Can Steer, = 327937
        TAG_POWER_BUFF_4_ICON = 270596, // Buff 4 Icon, = 270596
        TAG_POWER_BURROW_WEAPON_CLASS_OVERRIDE = 263224, // Burrow Weapon Class Override, = 263224
        TAG_POWER_AUTO_PURCHASE_LEVEL = 329520, // Auto Purchase Level, = 329520
        TAG_POWER_BUFF_2_DONT_APPLY_VISUAL_TO_PETS = 272386, // Buff 2 Dont Apply Visual To Pets, = 272386
        TAG_POWER_REFRESHES_COMBO_LEVEL = 264449, // RefreshesComboLevel, = 264449
        TAG_POWER_RUNED_DAMAGE_TYPE = 721268, // RuneD Damage Type, = 721268
        TAG_POWER_COST_CONTINUAL_RESOURCE = 329664, // Continual Resource Cost, = 329664
        TAG_POWER_IS_HOTBAR_ASSIGNABLE = 328064, // IsHotbarAssignable, = 328064
        TAG_POWER_IS_AIMED_AT_GROUND = 327888, // IsAimedAtGround, = 327888
        TAG_POWER_WALKING_DURATION_MIN = 328535, // Min Walk Duration, = 328535
        TAG_POWER_RUNEA_PROC_SCALAR = 721153, // RuneA Proc Scalar, = 721153
        TAG_POWER_BUFF_17_SHOW_DURATION = 271127, // Buff 17 Show Duration, = 271127
        TAG_POWER_ITEM_TYPE_REQUIREMENT = 328960, // Item Type Requirement, = 328960
        TAG_POWER_BUFF_9_HARMFUL_BUFF = 270857, // Buff 9 Is Harmful, = 270857
        TAG_POWER_BUFF_10_MERGES_TOOLTIP = 271888, // Buff 10 Merges Tooltip, = 271888
        TAG_POWER_COMBO_ANIMATION_1_RUNE_D = 263188, // Combo Animation 1 Rune D, = 263188
        TAG_POWER_BUFF_9_SHOW_DURATION = 271113, // Buff 9 Show Duration, = 271113
        TAG_POWER_BUFF_10_IS_DISPLAYED = 271632, // Buff 10 Is Displayed, = 271632
        TAG_POWER_BUFF_21_DONT_APPLY_VISUAL_TO_PETS = 272405, // Buff 21 Dont Apply Visual To Pets, = 272405
        TAG_POWER_SCRIPT_FORMULA_61 = 268048, // Script Formula 61, = 268048
        TAG_POWER_DELAY_BEFORE_SETTING_TARGET = 328539, // Delay Before Setting Target, = 328539
        TAG_POWER_BUFF_19_PLAYER_CAN_CANCEL = 271385, // Buff 19 Player Can Cancel, = 271385
        TAG_POWER_BUFF_19_SHOW_DURATION_ON_SKILL_BUTTON = 274707, // Buff 19 Show Duration On Skill Button, = 274707
        TAG_POWER_BUFF_11_IS_DISPLAYED = 271633, // Buff 11 Is Displayed, = 271633
        TAG_POWER_IS_INVULNERABLE_DURING = 328609, // Is Invulnerable During, = 328609
        TAG_POWER_BUFF_11_MERGES_TOOLTIP = 271889, // Buff 11 Merges Tooltip, = 271889
        TAG_POWER_ESCAPE_ATTACK_ANGLE = 329745, // Escape Attack Angle, = 329745
        TAG_POWER_RUNEC_DAMAGE_TYPE = 721267, // RuneC Damage Type, = 721267
        TAG_POWER_CUSTOM_TARGET_UNAFFECTED_ONLY_2 = 328808, // Custom Target Unaffected Only 2, = 328808
        TAG_POWER_CUSTOM_TARGET_CLOSEST_IN_PIE_ANGLE = 328805, // Custom Target Closest In Pie Angle, = 328805
        TAG_POWER_BUFF_28_PLAYER_CAN_CANCEL = 271400, // Buff 28 Player Can Cancel, = 271400
        TAG_POWER_CONSOLE_TARGET_NUM_GROWS = 712962, // , = 712962
        TAG_POWER_BUFF_28_SHOW_DURATION = 271144, // Buff 28 Show Duration, = 271144
        TAG_POWER_IS_COMPLETED_WHEN_WALKING_STOPS = 328608, // Is Completed When Walking Stops, = 328608
        TAG_POWER_BUFF_20_HARMFUL_BUFF = 270880, // Buff 20 Is Harmful, = 270880
        TAG_POWER_CONTROLLER_MAX_RANGE = 622596, // Controller Max Range When Held, = 622596
        TAG_POWER_REQUIRES_ACTOR_TARGET = 328240, // RequiresActorTarget, = 328240
        TAG_POWER_ANIMATION_MIN_SCALING = 263222, // Scaled Animation Timing Min, = 263222
        TAG_POWER_COMBO_ANIMATION_3_RUNE_A = 263217, // Combo Animation 3 Rune A, = 263217
        TAG_POWER_TURNS_INTO_BASIC_ATTACK = 328384, // TurnsIntoBasicAttack, = 328384
        TAG_POWER_BUFF_4_IS_DISPLAYED = 271620, // Buff 4 Is Displayed, = 271620
        TAG_POWER_TELEPORT_NEAR_TARGET_DIST_MIN = 721312, // Teleport Near Target Dist Min, = 721312
        TAG_POWER_BUFF_4_PLAYER_CAN_CANCEL = 271364, // Buff 4 Player Can Cancel, = 271364
        TAG_POWER_BUFF_1_SHOW_DURATION_ON_SKILL_BUTTON = 274689, // Buff 1 Show Duration On Skill Button, = 274689
        TAG_POWER_BUFF_1_MERGE_TOOLTIP_INDEX = 272129, // Buff 1 Merge Tooltip Index, = 272129
        TAG_POWER_BUFF_1_EFFECT_GROUP = 270337, // Buff 1 Effect Group, = 270337
        TAG_POWER_BUFF_2_IS_DISPLAYED = 271618, // Buff 2 Is Displayed, = 271618
        TAG_POWER_ANIMATION_TAG_TURN_RIGHT = 263425, // Animation Tag Turn Right, = 263425
        TAG_POWER_SCRIPT_FORMULA_58 = 267904, // Script Formula 58, = 267904
        TAG_POWER_SCRIPT_FORMULA_48 = 267648, // Script Formula 48, = 267648
        TAG_POWER_BUFF_6_HARMFUL_BUFF = 270854, // Buff 6 Is Harmful, = 270854
        TAG_POWER_TEMPLATE_RUNE_E = 327685, // Template Rune E, = 327685
        TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_B = 328642, // Special Death Chance Rune B, = 328642
        TAG_POWER_BUFF_14_SHOW_IN_BUFF_HOLDER = 274446, // Buff 14 Show In Buff Holder, = 274446
        TAG_POWER_BUFF_6_IS_DISPLAYED = 271622, // Buff 6 Is Displayed, = 271622
        TAG_POWER_IS_USABLE_IN_COMBAT = 328081, // IsUsableInCombat, = 328081
        TAG_POWER_BUFF_3_HARMFUL_BUFF = 270851, // Buff 3 Is Harmful, = 270851
        TAG_POWER_ALWAYS_KNOWN = 329536, // Always Known, = 329536
        TAG_POWER_TURNS_INTO_WALK = 327936, // TurnsIntoWalk, = 327936
        TAG_POWER_BUFF_10_SHOW_ACTIVE_ON_SKILL_BUTTON = 272650, // Buff 10 Show Active On Skill Button, = 272650
        TAG_POWER_CHILD_POWER = 327760, // Child Power, = 327760
        TAG_POWER_BUFF_10_ICON = 270608, // Buff 10 Icon, = 270608
        TAG_POWER_BUFF_9_DONT_APPLY_VISUAL_TO_PETS = 272393, // Buff 9 Dont Apply Visual To Pets, = 272393
        TAG_POWER_BUFF_10_EFFECT_GROUP = 270352, // Buff 10 Effect Group, = 270352
        TAG_POWER_BUFF_8_SHOW_DURATION_ON_SKILL_BUTTON = 274696, // Buff 8 Show Duration On Skill Button, = 274696
        TAG_POWER_BUFF_30_DONT_APPLY_VISUAL_TO_PETS = 272414, // Buff 30 Dont Apply Visual To Pets, = 272414
        TAG_POWER_BUFF_8_SHOW_IN_BUFF_HOLDER = 274440, // Buff 8 Show In Buff Holder, = 274440
        TAG_POWER_BUFF_11_HARMFUL_BUFF = 270865, // Buff 11 Is Harmful, = 270865
        TAG_POWER_BUFF_22_DONT_APPLY_VISUAL_TO_PETS = 272406, // Buff 22 Dont Apply Visual To Pets, = 272406
        TAG_POWER_BUFF_26_SHOW_DURATION_ON_SKILL_BUTTON = 274714, // Buff 26 Show Duration On Skill Button, = 274714
        TAG_POWER_BUFF_11_SHOW_DURATION = 271121, // Buff 11 Show Duration, = 271121
        TAG_POWER_CUSTOM_TARGET_BUFF_SNO_2 = 328806, // Custom Target Buff Power SNO 2, = 328806
        TAG_POWER_BUFF_25_SHOW_DURATION_ON_SKILL_BUTTON = 274713, // Buff 25 Show Duration On Skill Button, = 274713
        TAG_POWER_BUFF_12_MERGES_TOOLTIP = 271890, // Buff 12 Merges Tooltip, = 271890
        TAG_POWER_BUFF_12_ICON = 270610, // Buff 12 Icon, = 270610
        TAG_POWER_BUFF_26_PLAYER_CAN_CANCEL = 271398, // Buff 26 Player Can Cancel, = 271398
        TAG_POWER_BUFF_24_SHOW_ACTIVE_ON_SKILL_BUTTON = 272664, // Buff 24 Show Active On Skill Button, = 272664
        TAG_POWER_RUNEA_COMBO3_PROC_SCALAR = 721186, // RuneA Combo3 Proc Scalar, = 721186
        TAG_POWER_BUFF_23_SHOW_DURATION = 271139, // Buff 23 Show Duration, = 271139
        TAG_POWER_CAST_TARGET_NORMAL_MONSTERS_ONLY = 328617, // CastTargetNormalMonstersOnly, = 328617
        TAG_POWER_BUFF_24_DONT_APPLY_VISUAL_TO_PETS = 272408, // Buff 24 Dont Apply Visual To Pets, = 272408
        TAG_POWER_BUFF_23_EFFECT_GROUP = 270371, // Buff 23 Effect Group, = 270371
        TAG_POWER_COMBO_LEVEL_2_ON_HIT_COEFFICIENT = 329829, // Combo Level 2 On Hit Proc Coefficient, = 329829
        TAG_POWER_BUFF_25_SHOW_DURATION = 271141, // Buff 25 Show Duration, = 271141
        TAG_POWER_CONSOLE_LT_FIND_TARGET_IF_LOCKED_TARGET_OUT_OF_RANGE = 713218, // Controller LT Finds Target If Locked Target Out Of Range, = 713218
        TAG_POWER_BUFF_25_PLAYER_CAN_CANCEL = 271397, // Buff 25 Player Can Cancel, = 271397
        TAG_POWER_BUFF_27_SHOW_IN_BUFF_HOLDER = 274459, // Buff 27 Show In Buff Holder, = 274459
        TAG_POWER_SCRIPT_FORMULA_16 = 266848, // Script Formula 16, = 266848
        TAG_POWER_ALTERNATES_ANIMS = 328501, // Alternates Anims, = 328501
        TAG_POWER_TARGET_CONTACT_PLANE_ONLY = 328162, // TargetContactPlaneOnly, = 328162
        TAG_POWER_IS_DODGE = 328480, // Is A Dodge Power, = 328480
        TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_C = 328645, // Special Death Type Rune C, = 328645
        TAG_POWER_CAST_TARGET_IGNORE_LARGE_MONSTERS = 328616, // CastTargetIgnoreLargeMonsters, = 328616
        TAG_POWER_COMBO_ANIMATION_3_RUNE_C = 263219, // Combo Animation 3 Rune C, = 263219
        TAG_POWER_REQUIRES_SKILLPOINT = 328248, // RequiresSkillPoint, = 328248
        TAG_POWER_BUFF_4_DONT_APPLY_VISUAL_TO_PETS = 272388, // Buff 4 Dont Apply Visual To Pets, = 272388
        TAG_POWER_REUSE_SCRIPT_STATE = 278528, // ReuseScriptState, = 278528
        TAG_POWER_DONT_WALK_CLOSER_IF_OUT_OF_RANGE = 328256, // DontWalkCloserIfOutOfRange, = 328256
        TAG_POWER_AI_ACTION_DURATION_MIN = 332864, // AI Action Duration Min, = 332864
        TAG_POWER_CONTROLLER_MIN_RANGE = 622593, // Controller Min Range, = 622593
        TAG_POWER_RUNEA_COMBO1_PROC_SCALAR = 721184, // RuneA Combo1 Proc Scalar, = 721184
        TAG_POWER_BUFF_0_HARMFUL_BUFF = 270848, // Buff 0 Is Harmful, = 270848
        TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_C = 328644, // Special Death Chance Rune C, = 328644
        TAG_POWER_SCRIPT_FORMULA_8 = 266624, // Script Formula 8, = 266624
        TAG_POWER_BUFF_13_SHOW_ACTIVE_ON_SKILL_BUTTON = 272653, // Buff 13 Show Active On Skill Button, = 272653
        TAG_POWER_COMBO_ANIMATION_2 = 262913, // Combo Animation 2, = 262913
        TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_D = 328646, // Special Death Chance Rune D, = 328646
        TAG_POWER_BUFF_7_IS_DISPLAYED = 271623, // Buff 7 Is Displayed, = 271623
        TAG_POWER_SCRIPT_FORMULA_50 = 267776, // Script Formula 50, = 267776
        TAG_POWER_BUFF_7_HARMFUL_BUFF = 270855, // Buff 7 Is Harmful, = 270855
        TAG_POWER_SCRIPT_FORMULA_40 = 267520, // Script Formula 40, = 267520
        TAG_POWER_BUFF_12_SHOW_IN_BUFF_HOLDER = 274444, // Buff 12 Show In Buff Holder, = 274444
        TAG_POWER_BUFF_23_DONT_APPLY_VISUAL_TO_PETS = 272407, // Buff 23 Dont Apply Visual To Pets, = 272407
        TAG_POWER_BUFF_14_HARMFUL_BUFF = 270868, // Buff 14 Is Harmful, = 270868
        TAG_POWER_BUFF_23_SHOW_ACTIVE_ON_SKILL_BUTTON = 272663, // Buff 23 Show Active On Skill Button, = 272663
        TAG_POWER_COMBO_ATTACK_RADIUS_2 = 329810, // Combo Attack Radius 2, = 329810
        TAG_POWER_CANNOT_OVERKILL = 327953, // Cannot Overkill, = 327953
        TAG_POWER_BUFF_14_ICON = 270612, // Buff 14 Icon, = 270612
        TAG_POWER_BUFF_15_DONT_APPLY_VISUAL_TO_PETS = 272399, // Buff 15 Dont Apply Visual To Pets, = 272399
        TAG_POWER_BUFF_29_SHOW_DURATION_ON_SKILL_BUTTON = 274717, // Buff 29 Show Duration On Skill Button, = 274717
        TAG_POWER_COMBO_ANIMATION_1_RUNE_E = 263189, // Combo Animation 1 Rune E, = 263189
        TAG_POWER_BUFF_15_SHOW_ACTIVE_ON_SKILL_BUTTON = 272655, // Buff 15 Show Active On Skill Button, = 272655
        TAG_POWER_SCRIPT_FORMULA_15 = 266832, // Script Formula 15, = 266832
        TAG_POWER_DISPLAYS_NO_DAMAGE = 327829, // DisplaysNoDamage, = 327829
        TAG_POWER_BUFF_15_ICON = 270613, // Buff 15 Icon, = 270613
        TAG_POWER_COOLDOWN = 327768, // Cooldown Time, = 327768
        TAG_POWER_BUFF_15_HARMFUL_BUFF = 270869, // Buff 15 Is Harmful, = 270869
        TAG_POWER_BUFF_21_SHOW_IN_BUFF_HOLDER = 274453, // Buff 21 Show In Buff Holder, = 274453
        TAG_POWER_CONSOLE_TARGET_DIST_GROWTH_PER_KILL = 712961, // , = 712961
        TAG_POWER_RUNED_PROC_SCALAR = 721156, // RuneD Proc Scalar, = 721156
        TAG_POWER_DONT_ALLOW_COOLDOWN_REDUCTION = 329680, // Dont Allow Cooldown Reduction, = 329680
        TAG_POWER_IS_USABLE_IN_TOWN = 328080, // IsUsableInTown, = 328080
        TAG_POWER_NORUNE_COMBO2_PROC_SCALAR = 721169, // NoRune Combo2 Proc Scalar, = 721169
        TAG_POWER_SKILL_POINT_COST = 329312, // Skill Point Cost, = 329312
        TAG_POWER_BUFF_31_SHOW_IN_BUFF_HOLDER = 274463, // Buff 31 Show In Buff Holder, = 274463
        TAG_POWER_BUFF_31_SHOW_DURATION_ON_SKILL_BUTTON = 274719, // Buff 31 Show Duration On Skill Button, = 274719
        TAG_POWER_BUFF_27_SHOW_DURATION = 271143, // Buff 27 Show Duration, = 271143
        TAG_POWER_REQUIRES_1H_ITEM = 328976, // Requires 1H Item, = 328976
        TAG_POWER_BUFF_23_PLAYER_CAN_CANCEL = 271395, // Buff 23 Player Can Cancel, = 271395
        TAG_POWER_COMBO_ANIMATION_2_RUNE_D = 263204, // Combo Animation 2 Rune D, = 263204
        TAG_POWER_BUFF_23_IS_DISPLAYED = 271651, // Buff 23 Is Displayed, = 271651
        TAG_POWER_BUFF_24_MERGES_TOOLTIP = 271908, // Buff 24 Merges Tooltip, = 271908
        TAG_POWER_BUFF_29_PLAYER_CAN_CANCEL = 271401, // Buff 29 Player Can Cancel, = 271401
        TAG_POWER_BUFF_21_IS_DISPLAYED = 271649, // Buff 21 Is Displayed, = 271649
        TAG_POWER_BUFF_21_MERGES_TOOLTIP = 271905, // Buff 21 Merges Tooltip, = 271905
        TAG_POWER_RUNEC_PROC_SCALAR = 721155, // RuneC Proc Scalar, = 721155
        TAG_POWER_CONSOLE_AUTO_TARGET_ENABLED = 713216, // Controller Auto Targeting Enabled, = 713216
        TAG_POWER_SCRIPT_FORMULA_57 = 267888, // Script Formula 57, = 267888
        TAG_POWER_SCRIPT_FORMULA_47 = 267632, // Script Formula 47, = 267632
        TAG_POWER_PERSISTS_ON_WORLD_DELETE = 717056, // Persists on World Delete, = 717056
        TAG_POWER_BUFF_5_SHOW_DURATION = 271109, // Buff 5 Show Duration, = 271109
        TAG_POWER_BUFF_5_PLAYER_CAN_CANCEL = 271365, // Buff 5 Player Can Cancel, = 271365
        TAG_POWER_BUFF_30_HARMFUL_BUFF = 270896, // Buff 30 Is Harmful, = 270896
        TAG_POWER_BUFF_1_SHOW_ACTIVE_ON_SKILL_BUTTON = 272641, // Buff 1 Show Active On Skill Button, = 272641
        TAG_POWER_SCRIPT_FORMULA_0 = 266496, // Script Formula 0, = 266496
        TAG_POWER_IS_CHANNELLED = 328400, // IsChannelled, = 328400
        TAG_POWER_BUFF_14_IS_DISPLAYED = 271636, // Buff 14 Is Displayed, = 271636
        TAG_POWER_BUFF_14_PLAYER_CAN_CANCEL = 271380, // Buff 14 Player Can Cancel, = 271380
        TAG_POWER_TEMPLATE_RUNE_A = 327681, // Template Rune A, = 327681
        TAG_POWER_BUFF_10_SHOW_IN_BUFF_HOLDER = 274442, // Buff 10 Show In Buff Holder, = 274442
        TAG_POWER_SCRIPT_FORMULA_39 = 267408, // Script Formula 39, = 267408
        TAG_POWER_SCRIPT_FORMULA_29 = 267152, // Script Formula 29, = 267152
        TAG_POWER_BUFF_9_ICON = 270601, // Buff 9 Icon, = 270601
        TAG_POWER_BUFF_8_MERGE_TOOLTIP_INDEX = 272136, // Buff 8 Merge Tooltip Index, = 272136
        TAG_POWER_BUFF_15_MERGES_TOOLTIP = 271893, // Buff 15 Merges Tooltip, = 271893
        TAG_POWER_LOCKS_ACTORS_WHILE_SWEEPING = 328420, // LocksActorsWhileSweeping, = 328420
        TAG_POWER_BUFF_15_MERGE_TOOLTIP_INDEX = 272149, // Buff 15 Merge Tooltip Index, = 272149
        TAG_POWER_IS_USABLE_IN_PVP = 328084, // IsUsableInPVP, = 328084
        TAG_POWER_BUFF_16_HARMFUL_BUFF = 270870, // Buff 16 Is Harmful, = 270870
        TAG_POWER_BUFF_16_IS_DISPLAYED = 271638, // Buff 16 Is Displayed, = 271638
        TAG_POWER_BUFF_19_SHOW_IN_BUFF_HOLDER = 274451, // Buff 19 Show In Buff Holder, = 274451
        TAG_POWER_CAST_TARGET_ALLIES = 328097, // CastTargetAllies, = 328097
        TAG_POWER_BUFF_13_HARMFUL_BUFF = 270867, // Buff 13 Is Harmful, = 270867
        TAG_POWER_IS_TOGGLEABLE = 327952, // IsToggleable, = 327952
        TAG_POWER_BUFF_26_SHOW_ACTIVE_ON_SKILL_BUTTON = 272666, // Buff 26 Show Active On Skill Button, = 272666
        TAG_POWER_IS_PRIMARY = 327776, // IsPrimary, = 327776
        TAG_POWER_BUFF_27_SHOW_ACTIVE_ON_SKILL_BUTTON = 272667, // Buff 27 Show Active On Skill Button, = 272667
        TAG_POWER_BREAKS_ROOT = 681984, // Breaks Root, = 681984
        TAG_POWER_COMBO_CASTING_EFFECT_GROUP_0 = 264288, // Combo 0 Casting Effect Group - Male, = 264288
        TAG_POWER_TELEPORT_NEAR_TARGET_DIST_DELTA = 721313, // Teleport Near Target Dist Delta, = 721313
        TAG_POWER_SCRIPT_FORMULA_32 = 267296, // Script Formula 32, = 267296
        TAG_POWER_BUFF_21_HARMFUL_BUFF = 270881, // Buff 21 Is Harmful, = 270881
        TAG_POWER_BUFF_28_MERGES_TOOLTIP = 271912, // Buff 28 Merges Tooltip, = 271912
        TAG_POWER_SCRIPT_FORMULA_22 = 267040, // Script Formula 22, = 267040
        TAG_POWER_BUFF_21_SHOW_DURATION = 271137, // Buff 21 Show Duration, = 271137
        TAG_POWER_BUFF_28_IS_DISPLAYED = 271656, // Buff 28 Is Displayed, = 271656
        TAG_POWER_BUFF_22_MERGES_TOOLTIP = 271906, // Buff 22 Merges Tooltip, = 271906
        TAG_POWER_BUFF_22_ICON = 270626, // Buff 22 Icon, = 270626
        TAG_POWER_BUFF_31_PLAYER_CAN_CANCEL = 271409, // Buff 31 Player Can Cancel, = 271409
        TAG_POWER_SCRIPT_FORMULA_7 = 266608, // Script Formula 7, = 266608
        TAG_POWER_RUNEB_COMBO3_PROC_SCALAR = 721202, // RuneB Combo3 Proc Scalar, = 721202
        TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_A = 328641, // Special Death Type Rune A, = 328641
        TAG_POWER_BUFF_5_DONT_APPLY_VISUAL_TO_PETS = 272389, // Buff 5 Dont Apply Visual To Pets, = 272389
        TAG_POWER_BUFF_4_MERGES_TOOLTIP = 271876, // Buff 4 Merges Tooltip, = 271876
        TAG_POWER_BRAIN_ACTION_TYPE = 328704, // Brain Action Type, = 328704
        TAG_POWER_BUFF_4_SHOW_IN_BUFF_HOLDER = 274436, // Buff 4 Show In Buff Holder, = 274436
        TAG_POWER_BUFF_1_IS_DISPLAYED = 271617, // Buff 1 Is Displayed, = 271617
        TAG_POWER_SNAPS_TO_GROUND = 328496, // Snaps To Ground, = 328496
        TAG_POWER_BUFF_1_MERGES_TOOLTIP = 271873, // Buff 1 Merges Tooltip, = 271873
        TAG_POWER_TEMPLATE_RUNE_B = 327682, // Template Rune B, = 327682
        TAG_POWER_LOOPING_IS_SELF_INTERRUPTING = 263298, // Looping Not Self Interrupting, = 263298
        TAG_POWER_CAUSES_CLOSING_COOLDOWN = 328632, // Causes Closing Cooldown, = 328632
        TAG_POWER_BUFF_6_EFFECT_GROUP = 270342, // Buff 6 Effect Group, = 270342
        TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_A = 328640, // Special Death Chance Rune A, = 328640
        TAG_POWER_BUFF_6_SHOW_DURATION = 271110, // Buff 6 Show Duration, = 271110
        TAG_POWER_BUFF_6_SHOW_DURATION_ON_SKILL_BUTTON = 274694, // Buff 6 Show Duration On Skill Button, = 274694
        TAG_POWER_CAST_TARGET_MUST_BE_IN_TELEPORTABLE_AREA = 328083, // CastTargetMustBeInTeleportableArea, = 328083
        TAG_POWER_BUFF_20_DONT_APPLY_VISUAL_TO_PETS = 272404, // Buff 20 Dont Apply Visual To Pets, = 272404
        TAG_POWER_CAN_PATH_DURING_WALK = 328272, // CanPathDuringWalk, = 328272
        TAG_POWER_BUFF_12_DONT_APPLY_VISUAL_TO_PETS = 272396, // Buff 12 Dont Apply Visual To Pets, = 272396
        TAG_POWER_BUFF_23_SHOW_IN_BUFF_HOLDER = 274455, // Buff 23 Show In Buff Holder, = 274455
        TAG_POWER_RUNEB_COMBO1_PROC_SCALAR = 721200, // RuneB Combo1 Proc Scalar, = 721200
        TAG_POWER_BUFF_23_SHOW_DURATION_ON_SKILL_BUTTON = 274711, // Buff 23 Show Duration On Skill Button, = 274711
        TAG_POWER_BUFF_10_HARMFUL_BUFF = 270864, // Buff 10 Is Harmful, = 270864
        TAG_POWER_FOLLOW_STOP_DISTANCE = 557056, // Follow Stop Distance, = 557056
        TAG_POWER_SCRIPT_FORMULA_31 = 267280, // Script Formula 31, = 267280
        TAG_POWER_ANIMATION_TAG_RUNE_A = 262673, // Animation Tag Rune A, = 262673
        TAG_POWER_BUFF_29_SHOW_ACTIVE_ON_SKILL_BUTTON = 272669, // Buff 29 Show Active On Skill Button, = 272669
        TAG_POWER_SCRIPT_FORMULA_21 = 267024, // Script Formula 21, = 267024
        TAG_POWER_COMBO_TEMPLATE_2 = 262929, // Combo Template 2, = 262929
        TAG_POWER_FOLLOW_MATCH_TARGET_SPEED = 561408, // Follow Match Target Speed, = 561408
        TAG_POWER_MAX_CHARGES = 329697, // Max Charges, = 329697
        TAG_POWER_BUFF_19_MERGE_TOOLTIP_INDEX = 272153, // Buff 19 Merge Tooltip Index, = 272153
        TAG_POWER_SET_TARGET_AFTER_INTRO = 328292, // Set Target After Intro, = 328292
        TAG_POWER_BUFF_19_EFFECT_GROUP = 270361, // Buff 19 Effect Group, = 270361
        TAG_POWER_HOTBAR_EXCLUSIVE_TYPE = 721344, // Hotbar Exclusive Type, = 721344
        TAG_POWER_LOOKSWITCH_DOESNT_INTERRUPT = 327956, // Lookswitch Doesnt Interrupt, = 327956
        TAG_POWER_BUFF_11_ICON = 270609, // Buff 11 Icon, = 270609
        TAG_POWER_BUFF_12_PLAYER_CAN_CANCEL = 271378, // Buff 12 Player Can Cancel, = 271378
        TAG_POWER_BUFF_17_SHOW_IN_BUFF_HOLDER = 274449, // Buff 17 Show In Buff Holder, = 274449
        TAG_POWER_COMBO_ATTACK_SPEED_2 = 329826, // Combo Attack Speed 2, = 329826
        TAG_POWER_BUFF_12_MERGE_TOOLTIP_INDEX = 272146, // Buff 12 Merge Tooltip Index, = 272146
        TAG_POWER_BUFF_31_DONT_APPLY_VISUAL_TO_PETS = 272415, // Buff 31 Dont Apply Visual To Pets, = 272415
        TAG_POWER_COMBO_ANIMATION_2_RUNE_E = 263205, // Combo Animation 2 Rune E, = 263205
        TAG_POWER_BUFF_31_SHOW_ACTIVE_ON_SKILL_BUTTON = 272671, // Buff 31 Show Active On Skill Button, = 272671
        TAG_POWER_BUFF_25_ICON = 270629, // Buff 25 Icon, = 270629
        TAG_POWER_BUFF_25_HARMFUL_BUFF = 270885, // Buff 25 Is Harmful, = 270885
        TAG_POWER_COST_CHARGES = 329696, // Charge Cost, = 329696
        TAG_POWER_BUFF_28_ICON = 270632, // Buff 28 Icon, = 270632
        TAG_POWER_CANNOT_LMB_ASSIGN = 327920, // CannotLMBAssign, = 327920
        TAG_POWER_BUFF_28_EFFECT_GROUP = 270376, // Buff 28 Effect Group, = 270376
        TAG_POWER_RECHARGE_TIME = 329698, // Recharge Time, = 329698
        TAG_POWER_RUNEA_COMBO2_PROC_SCALAR = 721185, // RuneA Combo2 Proc Scalar, = 721185
        TAG_POWER_REQUIRES_2H_ITEM = 328992, // Requires 2H Item, = 328992
        TAG_POWER_USES_MAINHAND_ONLY_COMBO3 = 329833, // Combo Level 3 Uses Main Hand Only, = 329833
        TAG_POWER_COMBO_ANIMATION_3_RUNE_D = 263220, // Combo Animation 3 Rune D, = 263220
        TAG_POWER_SCRIPT_FORMULA_63 = 268080, // Script Formula 63, = 268080
        TAG_POWER_BUFF_4_SHOW_ACTIVE_ON_SKILL_BUTTON = 272644, // Buff 4 Show Active On Skill Button, = 272644
        TAG_POWER_BUFF_1_HARMFUL_BUFF = 270849, // Buff 1 Is Harmful, = 270849
        TAG_POWER_RUNEC_COMBO3_PROC_SCALAR = 721218, // RuneC Combo3 Proc Scalar, = 721218
        TAG_POWER_SPECIAL_DEATH_TYPE_RUNE_E = 328649, // Special Death Type Rune E, = 328649
        TAG_POWER_CONSOLE_AUTO_TARGET_CLOSE_RANGE = 712965, // Controller Auto Target Close Range, = 712965
        TAG_POWER_BUFF_1_SHOW_DURATION = 271105, // Buff 1 Show Duration, = 271105
        TAG_POWER_BUFF_2_SHOW_IN_BUFF_HOLDER = 274434, // Buff 2 Show In Buff Holder, = 274434
        TAG_POWER_BUFF_2_MERGES_TOOLTIP = 271874, // Buff 2 Merges Tooltip, = 271874
        TAG_POWER_OFFHAND_ANIMATION_TAG = 262663, // Offhand Animation Tag (for dual wield), = 262663
        TAG_POWER_BUFF_2_ICON = 270594, // Buff 2 Icon, = 270594
        TAG_POWER_SCRIPT_FORMULA_18 = 266880, // Script Formula 18, = 266880
        TAG_POWER_BUFF_11_SHOW_DURATION_ON_SKILL_BUTTON = 274699, // Buff 11 Show Duration On Skill Button, = 274699
        TAG_POWER_BUFF_6_MERGE_TOOLTIP_INDEX = 272134, // Buff 6 Merge Tooltip Index, = 272134
        TAG_POWER_BUFF_7_MERGE_TOOLTIP_INDEX = 272135, // Buff 7 Merge Tooltip Index, = 272135
        TAG_POWER_BUFF_6_MERGES_TOOLTIP = 271878, // Buff 6 Merges Tooltip, = 271878
        TAG_POWER_RUN_NEARBY_DISTANCE_DELTA = 332432, // Run Nearby Distance Delta, = 332432
        TAG_POWER_BUFF_7_PLAYER_CAN_CANCEL = 271367, // Buff 7 Player Can Cancel, = 271367
        TAG_POWER_BUFF_3_ICON = 270595, // Buff 3 Icon, = 270595
        TAG_POWER_BUFF_3_MERGES_TOOLTIP = 271875, // Buff 3 Merges Tooltip, = 271875
        TAG_POWER_CAN_USE_WHEN_FEARED = 328512, // Can Use When Feared, = 328512
        TAG_POWER_BUFF_12_SHOW_DURATION_ON_SKILL_BUTTON = 274700, // Buff 12 Show Duration On Skill Button, = 274700
        TAG_POWER_ESCAPE_ATTACK_RADIUS = 329744, // Escape Attack Radius, = 329744
        TAG_POWER_SPECIAL_DEATH_CHANCE_RUNE_E = 328648, // Special Death Chance Rune E, = 328648
        TAG_POWER_ANIMATION_TAG_RUNE_E = 262677, // Animation Tag Rune E, = 262677
        TAG_POWER_BREAKS_FEAR = 681987, // Breaks Fear, = 681987
        TAG_POWER_SPECIAL_DEATH_CHANCE = 328532, // Special Death Chance, = 328532
        TAG_POWER_CUSTOM_TARGET_NEEDS_HEAL_HP_PERCENT = 328804, // Custom Target Needs Heal HP Percent, = 328804
        TAG_POWER_CANNOT_LOCK_ONTO_ACTORS = 328416, // CannotLockOntoActors, = 328416
        TAG_POWER_SPECIAL_DEATH_TYPE = 328534, // Special Death Type, = 328534
        TAG_POWER_BUFF_12_SHOW_DURATION = 271122, // Buff 12 Show Duration, = 271122
        TAG_POWER_RUNEC_COMBO1_PROC_SCALAR = 721216, // RuneC Combo1 Proc Scalar, = 721216
        TAG_POWER_BUFF_12_HARMFUL_BUFF = 270866, // Buff 12 Is Harmful, = 270866
        TAG_POWER_SPELL_FUNC_CREATE = 327697, // SpellFunc Create, = 327697
        TAG_POWER_BUFF_23_MERGE_TOOLTIP_INDEX = 272163, // Buff 23 Merge Tooltip Index, = 272163
        TAG_POWER_BUFF_18_MERGE_TOOLTIP_INDEX = 272152, // Buff 18 Merge Tooltip Index, = 272152
        TAG_POWER_BUFF_25_MERGES_TOOLTIP = 271909, // Buff 25 Merges Tooltip, = 271909
        TAG_POWER_USE_SPECIALWALK_STEERING = 328538, // Use SpecialWalk Steering, = 328538
        TAG_POWER_COMBO_CASTING_EFFECT_GROUP_1 = 264289, // Combo 1 Casting Effect Group - Male, = 264289
        TAG_POWER_BUFF_25_MERGE_TOOLTIP_INDEX = 272165, // Buff 25 Merge Tooltip Index, = 272165
        TAG_POWER_IS_UNTARGETABLE_DURING = 633616, // Is Untargetable During, = 633616
        TAG_POWER_CAST_TARGET_ENEMIES = 328113, // CastTargetEnemies, = 328113
        TAG_POWER_IS_PUNCH = 327792, // IsPunch, = 327792

        TAG_SHADERMAP_MIN_DEFAULT = 198400, //  Default Min
        TAG_SHADERMAP_HIGH_FADE = 198145, //  Fade High
        TAG_SHADERMAP_HIGH_PP_FADE = 198752, //  Fade High Per-Pixel
        TAG_SHADERMAP_OCCLUDED = 198688, //  Occluded
        TAG_SHADERMAP_MED_UNSKINNED = 197892, //  Unskinned Medium
        TAG_SHADERMAP_HIGH_REFLECTION = 198146, //  Reflection High
        TAG_SHADERMAP_PREPASS = 198768, //  Depth Prepass
        TAG_SHADERMAP_CONSOLE_DEFAULT = 198704, //  Default Console
        TAG_SHADERMAP_MIN_UNSKINNED = 198404, //  Unskinned Min
        TAG_SHADERMAP_HIGH_DEFAULT = 198144, //  Default High
        TAG_SHADERMAP_MIN_FADE = 198401, //  Fade Min
        TAG_SHADERMAP_MASK = 198784, //  Stencil Mask
        TAG_SHADERMAP_COOKIE = 198720, //  Shadow Cookie
        TAG_SHADERMAP_GHOST_ZPASS = 198656, //  Ghost Z pass
        TAG_SHADERMAP_MED_FADE = 197889, //  Fade Medium
        TAG_SHADERMAP_HIGH_UNSKINNED = 198148, //  Unskinned High
        TAG_SHADERMAP_MIN_REFLECTION = 198402, //  Reflection Min
        TAG_SHADERMAP_MED_DEFAULT = 197888, //  Default Medium
        TAG_SHADERMAP_HIGH_PP_DEFAULT = 198736, //  Default High Per-Pixel
        TAG_SHADERMAP_HIGHLIGHT = 198672, //  Highlight
        TAG_SHADERMAP_MED_REFLECTION = 197890, //  Reflection Medium

        TAG_VS_EARLY_DEPTH_STENCIL = 655423, //  EarlyDepthStencil - Set Internally
        TAG_VS_PS2A_FUNC = 655390, //  Stage 3 Alpha Function
        TAG_VS_FLIP_NORMAL_BACKFACE = 655421, //  Flip Normal (BackFace)
        TAG_VS_PS0A_FUNC = 655388, //  Stage 1 Alpha Function
        TAG_VS_USES_TANGENTS = 655415, //  Uses Tangents
        TAG_VS_VB_FORMAT = 655414, //  VB Format
        TAG_VS_ALPHATESTFUNC = 655413, //  Alphatest Cmp Func - Set Internally
        TAG_VS_SHADOW_TECHNIQUE = 655412, //  Shadow Technique - Set Internally
        TAG_VS_FLOAT_TEX_COORD = 655406, //  Float Tex Coords
        TAG_VS_CLOTH = 655404, //  Enable Cloth
        TAG_VS_NUM_WAVES = 655371, //  # of Waves
        TAG_VS_NUM_SPOT_LIGHTS = 655369, //  # of Spot Lights
        TAG_VS_NUM_BONE_WEIGHTS = 655363, //  # of Weights Per Bone
        TAG_VS_ENABLE_SKINNING = 655362, //  Enable Skinning
        TAG_VS_SSAO = 655422, //  SSAO
        TAG_VS_PERPIXEL_LIGHTING = 655420, //  Per-Pixel Lighting
        TAG_VS_PS5C_FUNC = 655387, //  Stage 6 Color Function
        TAG_VS_PS3C_FUNC = 655385, //  Stage 4 Color Function
        TAG_VS_TEXCOORD3_FUNC = 655379, //  TexCoord4 Function
        TAG_VS_TEXCOORD2_FUNC = 655378, //  TexCoord3 Function
        TAG_VS_TEXCOORD1_FUNC = 655377, //  TexCoord2 Function
        TAG_VS_TEXCOORD0_FUNC = 655376, //  TexCoord1 Function
        TAG_VS_PMA_FUNC = 655403, //  PMA Func
        TAG_VS_NUM_DIRECTIONAL_LIGHTS = 655370, //  # of Directional Lights
        TAG_VS_SHADER_MODEL = 655401, //  Shader Model - Set Internally
        TAG_VS_NUM_POINT_LIGHTS = 655368, //  # of Point Lights
        TAG_VS_TINT = 655395, //  Enable Tint
        TAG_VS_GLOSSY = 655394, //  Enable Gloss
        TAG_VS_PS5A_FUNC = 655393, //  Stage 6 Alpha Function
        TAG_VS_PS4A_FUNC = 655392, //  Stage 5 Alpha Function
        TAG_VS_ENABLE_NEAR_FADE_IN = 655419, //  Near Fade In
        TAG_VS_PS4C_FUNC = 655386, //  Stage 5 Color Function
        TAG_VS_MESH_LIGHTING = 655417, //  Meshlighting Enabled
        TAG_VS_PS2C_FUNC = 655384, //  Stage 3 Color Function
        TAG_VS_SHADOW_ENABLED = 655411, //  Shadows Enabled - Set Internally
        TAG_VS_DIFF_ALPHA_IS_GLOSS = 655410, //  Diff alpha is gloss
        TAG_VS_WEATHER_SCALES_DEFORM = 655409, //  Weather scales deformation
        TAG_VS_ENABLE_DEFORM = 655408, //  Enable Deformation
        TAG_VS_LIGHTING = 655375, //  Enable Lighting
        TAG_VS_NUM_POINT_LINEAR_LIGHTS = 655373, //  # of Point Lights (Linear Falloff)
        TAG_VS_USES_SHADOWS = 655402, //  Uses Shadows
        TAG_VS_FRESNEL_POWER = 655400, //  Fresnel Power
        TAG_VS_ENABLE_FOGGING = 655367, //  Enable Fogging
        TAG_VS_LIGHTMAP = 655366, //  Lightmap
        TAG_VS_EDGEALPHA = 655365, //  Vertex Alpha Function
        TAG_VS_TIMER_PERIOD = 655364, //  Timer period
        TAG_VS_OUTPUT_FORMAT = 655424, //  Output Format
        TAG_VS_PS3A_FUNC = 655391, //  Stage 4 Alpha Function
        TAG_VS_PS1A_FUNC = 655389, //  Stage 2 Alpha Function
        TAG_VS_ALPHA_TO_COVERAGE = 655418, //  Alpha To Coverage
        TAG_VS_TRANSPARENT_ALPHA_TO_ONE = 655416, //  Transparent Alpha To One
        TAG_VS_PS1C_FUNC = 655383, //  Stage 2 Color Function
        TAG_VS_PS0C_FUNC = 655382, //  Stage 1 Color Function
        TAG_VS_TEXCOORD5_FUNC = 655381, //  TexCoord6 Function
        TAG_VS_TEXCOORD4_FUNC = 655380, //  TexCoord5 Function
        TAG_VS_HERO_TINT = 655407, //  Enable Hero Tint
        TAG_VS_MATERIAL_FUNC = 655374, //  Material Function
        TAG_VS_CLOTH_SINGLE_SIDED = 655405, //  Cloth is single sided
        TAG_VS_NUM_CYLINDRICAL_LIGHTS = 655372, //  # of Cylindrical Lights
        TAG_VS_FRESNEL_BIAS = 655399, //  Fresnel Bias
        TAG_VS_GLOW = 655398, //  Enable Glow
        TAG_VS_SHADOW_SELF = 655397, //  Shadow Self
        TAG_VS_MASK = 655396, //  Enable Mask	
    }
}