using System;
using System.Collections.Generic;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Reference
{

    /// <summary>
    /// Contains hard-coded meta-lists of ActorSnoId's, Spells and other non-dynamic info
    /// </summary>
    internal class GameData
    {
        public const SNOActor PlayerHeadstoneSNO = SNOActor.PlayerHeadstone;

        public const int RiftTrialLevelAreaId = 405915;

        public const int WallerSNO = 226808;


        public static HashSet<SNOActor> NecroCorpseSnoIds = new HashSet<SNOActor>
        {
            SNOActor.P6_Necro_Corpse_Flesh
        };

        /// <summary>
        /// Some questing sequences have odd line of sight issues around stairs
        /// Areas listed here should have the bot move onto the same ZDiff as target
        /// Before casting spells.
        /// </summary>
        public static HashSet<SNOScene> ForceSameZDiffSceneSnoIds = new HashSet<SNOScene>
        {
            //World: x1_Heaven_Pandemonium_Portal, Id: 339160, AnnId: 2000355341, IsGenerated: False
            //Scene: x1_Heaven_Pandemonium_Portal_E03_S03, SnoId: 339214,
            //LevelArea: x1_Heaven_Pandemonium_Portal, Id: 339468
            //Quest: x1_ToHeaven: The Pandemonium Gate (263851) MainQuest Step: GoGate (15)
            //Objective: 0: Go to the Pandemonium Gate (InProgress)
            SNOScene.x1_Heaven_Pandemonium_Portal_E03_S03,
            SNOScene.x1_Heaven_Pandemonium_Portal_E03_S02,
            SNOScene.x1_Heaven_Pandemonium_Portal_E02_S03,
            SNOScene.x1_Heaven_Pandemonium_Portal_E02_S02
        };

        public static HashSet<GizmoType> PortalTypes = new HashSet<GizmoType>
        {
            GizmoType.Portal,
            GizmoType.BossPortal,
            GizmoType.DungeonPortal,
            GizmoType.HearthPortal,
            GizmoType.ReturnPointPortal,
            GizmoType.SecretPortal,
            GizmoType.TownPortal,
        };

        public static HashSet<GizmoType> DestructibleTypes = new HashSet<GizmoType>
        {
            GizmoType.BreakableChest,
            GizmoType.BreakableDoor,
            GizmoType.DestroyableObject,
            GizmoType.ReformingDestroyableObject,
        };

        public static HashSet<SNOActor> IsCursedChestOrShrine = new HashSet<SNOActor>
        {
            SNOActor.x1_Global_Chest_CursedChest,
            (SNOActor)364601,
            SNOActor.x1_Global_Chest_CursedChest_B,
            SNOActor.x1_Spawner_Skeleton_C_Cursed,
            SNOActor.x1_Spawner_Shield_Skeleton_C_Cursed,
            (SNOActor)368169,
            SNOActor.x1_leaperAngel_A_CursedChest,
            SNOActor.Spiderling_A_CursedChest,
            SNOActor.TentacleBear_A_CursedChest,
            SNOActor.TentacleBear_B_CursedChest,
            SNOActor.TentacleBear_C_CursedChest,
            SNOActor.x1_Global_Chest_CursedChest_B_MutantEvent,
            SNOActor.x1_Spawner_Zombie_Inferno_C_CursedChest
        };
        
        public static HashSet<SNOWorld> MenuWorldSnoIds = new HashSet<SNOWorld>
        {
            SNOWorld.AxeBadData,
            SNOWorld.AxeFlatWorld,
            SNOWorld.BattleNet_Act1Screen,
            SNOWorld.BattleNet_Act2Screen,
            SNOWorld.BattleNet_Act3Screen,
            SNOWorld.BattleNet_Act4Screen,
            SNOWorld.BattleNet_Act5Screen,
            SNOWorld.BattleNet_MainScreen,
            SNOWorld.BattleNet_PVPScreen
        };

        /// <summary>
        /// Client effects that will be let into the object cache
        /// </summary>
        public static readonly HashSet<SNOActor> AllowedClientEffects = new HashSet<SNOActor>
        {
            SNOActor.p2_itemPassive_unique_ring_017_dome,
            SNOActor.monk_serenity_goldRings_AOE,
            SNOActor.monk_serenity_goldRings_health,
            SNOActor.monk_serenity_goldRings_immune,
            SNOActor.monk_serenity_goldRings_reflect,
            SNOActor.monk_serenity_goldRings_spirit,
        };

        public static readonly Dictionary<Skill, Set> AllRuneSetsBySkill = new Dictionary<Skill, Set>
        {
            { Skills.Barbarian.FuriousCharge, Sets.TheLegacyOfRaekor },
            { Skills.Wizard.Archon, Sets.VyrsAmazingArcana },
        };

        public static readonly HashSet<SNOActor> AvoidanceSNO = new HashSet<SNOActor>
        {
            SNOActor.ButcherLair_FloorPanel_UpperRight_Base,
            SNOActor.ButcherLair_FloorPanel_LowerRight_Base,
            SNOActor.ButcherLair_FloorPanel_MidMiddle_Base,
            SNOActor.ButcherLair_FloorPanel_UpperMid_Base,
            SNOActor.ButcherLair_FloorPanel_LowerMid_Base,
            SNOActor.ButcherLair_FloorPanel_UpperLeft_Base,
            SNOActor.ButcherLair_FloorPanel_LowerLeft_Base,
            SNOActor.fastMummy_Gibs,
            SNOActor.X1_Unique_Monster_Generic_AOE_DOT_Fire_10foot,
            SNOActor.p4_ratKing_ratBall_model,
            SNOActor.X1_Unique_Monster_Generic_Projectile_Poison,
            SNOActor.a2dun_Zolt_Random_FallingRocks_C,
            SNOActor.ZK_tornado_antimatter,
            SNOActor.ZK_tornado_model,
            //X1_LR_Boss_morluSpellcaster_Ice ActorSnoId=344119
            SNOActor.g_monster_projectile_cold_impact,
            SNOActor.Grenadier_Proj_mortar_inpact,
	        SNOActor.MorluSpellcaster_Meteor_Pending,
	        SNOActor.Demonic_Meteor_Pending,
	        SNOActor.x1_sniperAngel_shardBolt_bolt,
	        SNOActor.x1_sniperAngel_shardBolt_orb,
            SNOActor. a1dun_leor_BigFireGrate,
            SNOActor. arcaneEnchantedDummy_spawn,
            SNOActor.monsterAffix_Frozen_deathExplosion_Proxy,
            SNOActor.monsterAffix_Molten_deathStart_Proxy,
            SNOActor.monsterAffix_Molten_deathExplosion_Proxy,
            SNOActor.monsterAffix_Electrified_deathExplosion_proxy,
            SNOActor.monsterAffix_Desecrator_telegraph,
            SNOActor.monsterAffix_Desecrator_damage_AOE,
            SNOActor.monsterAffix_Vortex_proxy,
            SNOActor.monsterAffix_Vortex_model,
            SNOActor.monsterAffix_Molten_trail,
            SNOActor.monsterAffix_healthLink_jumpActor,
            SNOActor.monsterAffix_Plagued_endCloud,
            //SNOActor.monsterAffix_frenzySwipe,
            SNOActor.monsterAffix_vortex_target_trailActor,
            //SNOActor.monsterAffix_missileDampening_shield_add
            (SNOActor)219458,
            SNOActor.MonsterAffix_ArcaneEnchanted_PetSweep,
            SNOActor.monsterAffix_missileDampening_outsideGeo,
            SNOActor.MonsterAffix_ArcaneEnchanted_PetSweep_reverse,
            SNOActor.MonsterAffix_ArcaneEnchanted_Proxy,
            SNOActor.MonsterAffix_ArcaneEnchanted_trailActor,
            SNOActor.monsterAffix_frozen_iceClusters,
            SNOActor.x1_MonsterAffix_frozenPulse_monster,
            SNOActor.x1_MonsterAffix_frozenPulse_shard_search,
            SNOActor.x1_MonsterAffix_frozenPulse_shard,
            SNOActor.monsterAffix_plagued_groundGeo,
            SNOActor.monsterAffix_molten_fireRing,
            SNOActor.monsterAffix_waller_wall,
            SNOActor.monsterAffix_Avenger_glowSphere,
            SNOActor.monsterAffix_ghostly_distGeo,
            SNOActor.monsterAffix_waller_model,
            SNOActor.monsterAffix_invulnerableMinion_distGeo,
            SNOActor.monsterAffix_linked_chainHit,
            SNOActor.monsterAffix_entangler_ringGlow_geo,
            SNOActor.monsterAffix_molten_bomb_buildUp_geo,
            SNOActor.monsterAffix_invulnerableMinion_colorGeo,
            SNOActor.MonsterAffix_Mortar_Pending,
            SNOActor.x1_MonsterAffix_CorpseBomber_projectile,
            SNOActor.X1_MonsterAffix_corpseBomber_bomb,
            SNOActor.X1_MonsterAffix_LightningStorm_Wanderer,
            SNOActor.X1_MonsterAffix_TeleportMines,
            SNOActor.x1_MonsterAffix_CorpseBomber_bomb_start,
            SNOActor.x1_MonsterAffix_Thunderstorm_Impact,
            SNOActor.X1_MonsterAffix_Orbiter_Projectile,
            SNOActor.X1_MonsterAffix_Orbiter_FocalPoint,
            SNOActor.x1_Spawner_Skeleton_MonsterAffix_World_1,
            SNOActor.x1_Spawner_Skeleton_MonsterAffix_World_2,
            SNOActor.x1_Spawner_Skeleton_MonsterAffix_World_3,
            SNOActor.x1_Spawner_Skeleton_MonsterAffix_World_4,
            SNOActor.x1_Spawner_Skeleton_MonsterAffix_World_5,
            SNOActor.x1_MonsterAffix_orbiter_projectile_orb,
            SNOActor.x1_MonsterAffix_orbiter_projectile_focus,
            SNOActor.x1_MonsterAffix_orbiter_glowSphere,
            SNOActor.x1_monsteraffix_mortar_blastwave,
            SNOActor.x1_monsterAffix_generic_coldDOT_runeGeo,
            SNOActor.x1_monsterAffix_generic_coldDOT_rune_emitter,
            SNOActor.MonsterAffix_Avenger_ArcaneEnchanted_PetSweep,
            SNOActor.MonsterAffix_Avenger_ArcaneEnchanted_PetSweep_reverse,
            SNOActor.X1_MonsterAffix_Avenger_Orbiter_Projectile,
            SNOActor.X1_MonsterAffix_Avenger_Orbiter_FocalPoint,
            SNOActor.x1_MonsterAffix_Avenger_CorpseBomber_bomb_start,
            SNOActor.x1_MonsterAffix_Avenger_CorpseBomber_projectile,
            SNOActor.x1_MonsterAffix_Avenger_frozenPulse_monster,
            SNOActor.x1_MonsterAffix_Avenger_arcaneEnchanted_dummySpawn,
            SNOActor.x1_MonsterAffix_Avenger_ArcaneEnchanted_trailActor,
            SNOActor.x1_MonsterAffix_Avenger_orbiter_projectile_orb,
            SNOActor.x1_MonsterAffix_Avenger_orbiter_projectile_focus,
            SNOActor.x1_MonsterAffix_Avenger_corpseBomber_slime,
	        SNOActor.x1_Bog_bloodSpring_medium,
	        SNOActor.x1_Bog_bloodSpring_large,
	        SNOActor.x1_Bog_bloodSpring_small,
            SNOActor.p4_RatKing_RatBallMonster,
            SNOActor.x1_LR_Boss_MalletDemon_FallingRocks,
            SNOActor.X1_LR_Boss_FireNova_projectile,
            //SNOActor.X1_LR_Boss_AsteroidRain
            (SNOActor)378237,
            SNOActor.x1_LR_boss_terrorDemon_A_projectile,
            SNOActor.x1_Bog_Bear_Trap
        };

        public static readonly HashSet<int> BlackHoleIds = new HashSet<int>
        {
            // x1_Wizard_blackHole_proxy
            336410,
            // x1_Wizard_Blackhole_sphere
            337757,
            // x1_Wizard_Blackhole_SuperMassive_sphere
            341373,
            // x1_Wizard_blackHole_SuperMassive_proxy
            341381,
            // x1_Wizard_blackHole_AbsoluteZero_proxy
            341396,
            // x1_Wizard_Blackhole_AbsoluteZero_sphere
            341410,
            // x1_Wizard_blackHole_EventHorizon_proxy
            341411,
            // x1_Wizard_Blackhole_EventHorizon_sphere
            341412,
            // x1_Wizard_blackHole_CosmicGate_proxy
            341426,
            // x1_Wizard_Blackhole_CosmicGate_sphere
            341427,
            // x1_Wizard_blackHole_SpellSteal_proxy
            341441,
            // x1_Wizard_Blackhole_SpellSteal_sphere
            341442,
            // x1_Wizard_Blackhole_CosmicGate_FallOffSphere
            342082,
            // X1_Wizard_BlackHole_SpellSteal_GlowSphere
            343197,
            // Wizard_BlackHole_CosmicGate_Impact
            343293,
            // x1_Wizard_BlackHole_CosmicGate_impactSphere
            343300,
        };

        public static readonly HashSet<SNOActor> BloodShardSNO = new HashSet<SNOActor>
        {
            SNOActor.HoradricRelic,
        };

        public static readonly HashSet<SNOActor> BuffedLocationSno = new HashSet<SNOActor>
        {
            SNOActor.p2_itemPassive_unique_ring_017_dome,
            SNOActor.monk_serenity_goldRings_AOE,
            SNOActor.monk_serenity_goldRings_health,
            SNOActor.monk_serenity_goldRings_immune,
            SNOActor.monk_serenity_goldRings_reflect,
            SNOActor.monk_serenity_goldRings_spirit,
        };

        public static readonly HashSet<SNOAnim> ChargeAnimations = new HashSet<SNOAnim>
        {
            SNOAnim.barbarian_male_groundStomp_chargeTest, //7661,
            SNOAnim.barbarian_male_groundStomp_chargeTestEnd, //7662,
            SNOAnim.barbarian_male_1HS_Furious_Charge_End, //115910,
            SNOAnim.barbarian_male_1HS_Furious_Charge_Loop, //115911,
            SNOAnim.barbarian_male_STF_Furious_Charge_End, //115912,
            SNOAnim.barbarian_male_STF_Furious_Charge_loop, //115913,
            SNOAnim.barbarian_male_2HT_Furious_Charge_End, //115914,
            SNOAnim.barbarian_male_2HT_Furious_Charge_loop, //115915,
            SNOAnim.barbarian_male_1HT_Furious_Charge_End, //115926,
            SNOAnim.barbarian_male_1HT_Furious_Charge_loop, //115927,
            SNOAnim.Barbarian_Female_1HS_Furious_Charge_End, //116117,
            SNOAnim.Barbarian_Female_1HS_Furious_Charge_Loop, //116118,
            SNOAnim.Barbarian_Female_1HT_Furious_Charge_End, //116119,
            SNOAnim.Barbarian_Female_1HT_Furious_Charge_Loop, //116120,
            SNOAnim.Barbarian_Female_2HT_Furious_Charge_End, //116121,
            SNOAnim.Barbarian_Female_2HT_Furious_Charge_Loop, //116122,
            SNOAnim.Barbarian_Female_1HS_Furious_Charge_Loop_Cannibal, //437861,
            SNOAnim.Barbarian_Female_1HS_Furious_Charge_End_Cannibal, //437862,
        };

        public static readonly HashSet<TrinityObjectType> DestroyableTypes = new HashSet<TrinityObjectType>
        {
            TrinityObjectType.Barricade,
            TrinityObjectType.Destructible,
            TrinityObjectType.HealthGlobe,
            TrinityObjectType.ProgressionGlobe
        };

        public static readonly HashSet<SNOActor> ExtremePriorityInteractable = new HashSet<SNOActor>
        {
            SNOActor.a3dun_Keep_Bridge_Switch // switch with bridge area is marked as walkable so raycast passes incorrectly.
        };

        /// <summary>
        /// Monsters who pretend to be dead/hidden
        /// </summary>
        public static readonly HashSet<SNOActor> FakeDeathMonsters = new HashSet<SNOActor>()
        {
            SNOActor.morluMelee_A,
            SNOActor.morluMelee_B,
            SNOActor.morluMelee_C,
            SNOActor.morluMelee_A_Unique_01,
            SNOActor.morluMelee_A_Unique_02,
            SNOActor.P2_morluMelee_A_Unique_01,
            SNOActor.P2_morluMelee_A_Unique_02,
        };

        public static readonly HashSet<SNOActor> ForceTypeAsBarricade = new HashSet<SNOActor>
        {
            SNOActor.trDun_Cath_WoodDoor_A_Barricaded,
            SNOActor.a4dun_Garden_Corruption_Monster,
            SNOActor.a4dun_Garden_Corruption_HellRift_Monster,
        };

        public static readonly HashSet<SNOActor> GreaterProgressionGlobeSNO = new HashSet<SNOActor>
        {
            SNOActor.p1_tiered_rifts_Orb_flippy,
            SNOActor.p1_tiered_rifts_Orb
        };

        public static readonly HashSet<TrinityObjectType> InteractableTypes = new HashSet<TrinityObjectType>
        {
            TrinityObjectType.Item,
            TrinityObjectType.Door,
            TrinityObjectType.Container,
            TrinityObjectType.HealthWell,
            TrinityObjectType.CursedChest,
            TrinityObjectType.Interactable,
            TrinityObjectType.Shrine,
            TrinityObjectType.CursedShrine
        };

        public static readonly HashSet<SNOActor> KeywardenSno = new HashSet<SNOActor>
        {
            SNOActor.DuneDervish_B_Unique_Uber
        };

        public static readonly HashSet<SNOAnim> LeapAnimations = new HashSet<SNOAnim>
        {
             SNOAnim.Barbarian_Female_DW_Leap_Attack_Air, //7411
             SNOAnim.Barbarian_Female_DW_Leap_Attack_ContactEnd, //7412
             SNOAnim.barbarian_male_1HT_LeapAttack_Air, //7521
             SNOAnim.barbarian_male_1HT_LeapAttack_ContactEnd, //7522
             SNOAnim.barbarian_male_1HT_LeapAttack_Start, //7523
             SNOAnim.barbarian_male_2HS_Leap_Attack_Air, //7546
             SNOAnim.barbarian_male_2HS_Leap_Attack_ContactEnd, //7547
             SNOAnim.barbarian_male_2HS_Leap_Attack_Start, //7548
             SNOAnim.barbarian_male_2HT_LeapAttack_Air, //7569
             SNOAnim.barbarian_male_2HT_LeapAttack_ContactEnd, //7570
             SNOAnim.barbarian_male_2HT_LeapAttack_Start, //7571
             SNOAnim.barbarian_male_DW_LeapAttack_Air, //7632
             SNOAnim.barbarian_male_DW_LeapAttack_ContactEnd, //7633
             SNOAnim.barbarian_male_DW_LeapAttack_Start, //7634
             SNOAnim.barbarian_male_Leap_Attack_Air, //7701
             SNOAnim.barbarian_male_Leap_Attack_ContactEnd, //7702
             SNOAnim.barbarian_male_Leap_Attack_Start, //7703
             SNOAnim.barbarian_male_STF_LeapAttack_Air, //7718
             SNOAnim.barbarian_male_STF_LeapAttack_ContactEnd, //7719
             SNOAnim.barbarian_male_STF_LeapAttack_Start, //7720
             SNOAnim.barbarian_male_Leap_Attack_Air_Cannibal //437875,
        };

        public static readonly Dictionary<Item, string> MinionInternalNameTokenByItem = new Dictionary<Item, string>
        {
            { Legendary.Maximus, "DemonChains_ItemPassive" },
            { Legendary.HauntOfVaxo, "_shadowClone_" }
        };

        public static readonly HashSet<TrinityItemType> NoPickupClickItemTypes = new HashSet<TrinityItemType>
        {
            TrinityItemType.PowerGlobe,
            TrinityItemType.HealthGlobe,
            TrinityItemType.ProgressionGlobe,
            TrinityItemType.HoradricRelic,
        };

        public static readonly HashSet<TrinityObjectType> NoPickupClickTypes = new HashSet<TrinityObjectType>
        {
            TrinityObjectType.HealthGlobe,
            TrinityObjectType.ProgressionGlobe,
            TrinityObjectType.PowerGlobe,
            TrinityObjectType.Gold,
        };

        public static readonly HashSet<int> NormalProgressionGlobeSNO = new HashSet<int>
        {
            436807, //p1_normal_rifts_Orb
            434918, //p1_rifts_Orb_flippy
        };

        public static readonly Dictionary<SNOActor, TrinityObjectType> ObjectTypeOverrides = new Dictionary<SNOActor, TrinityObjectType>
        {
            // Needed for A5 bounty clicky on death orbs
            //ActorSnoId=334466 AnimSnoId=x1_Death_Orb_Event_Worldstone_idle TargetedCount=2 Type=Unit
            { SNOActor.x1_Death_Orb_Little_Event_Worldstone, TrinityObjectType.Interactable },
            { SNOActor.x1_westm_Bridge, TrinityObjectType.Interactable },
            { SNOActor.px_Highlands_Camp_ResurgentCult_Portal, TrinityObjectType.Interactable },
            { SNOActor.a1dun_Leoric_IronMaiden_Event, TrinityObjectType.Interactable} //a1dun_Leoric_IronMaiden_Event, Type=Door GizmoType=Door ActorSnoId=221574
            //{ 54882, ObjectType.Door } //a3dun_Keep_Door_Wooden_A, Type=Interactable GizmoType=Door
            //{ 433403, ObjectType.Interactable}
        };

        public static readonly HashSet<int> PlayerUseAnimationIds = new HashSet<int>
        {
            // Barbarian_Female_Emote_Use
            7434,
            // barbarian_male_Emote_Use
            7656,
            // Monk_Male_Emote_Use
            9114,
            // WitchDoctor_Female_Emote_Use
            10975,
            // WitchDoctor_Male_Emote_Use
            11077,
            // Wizard_Female_HTH_Emote_Use
            11235,
            // Wizard_Male_HTH_Emote_Use
            11331,
            // Monk_Male_DW_SS_Emote_Use
            54147,
            // Monk_Male_DW_SF_Emote_Use
            54227,
            // Monk_Male_1HS_Emote_Use
            54257,
            // Demonhunter_Female_emote_use
            104096,
            // Monk_Female_Emote_Use
            115246,
            // Demonhunter_Male_emote_use
            129089,
            // WitchDoctor_Male_1HT_Emote_Use
            214609,
            // WitchDoctor_Female_1HT_Emote_Use
            214656,
            // WitchDoctor_Female_2HT_Emote_Use
            214680,
            // barbarian_male_1HT_Emote_Use
            224290,
            // Barbarian_Female_1HT_Emote_Use
            224732,
            // x1_Crusader_Female_emote_use_loop
            319741,
            // x1_Crusader_Female_emote_use
            319742,
            // x1_Crusader_Male_HTH_Emote_use_01
            325715,
        };

        public static readonly Dictionary<Item, SNOPower> PowerByItem = new Dictionary<Item, SNOPower>
        {
            { Legendary.HarringtonWaistguard, SNOPower.ItemPassive_Unique_Ring_685_x1 },
            { Legendary.PoxFaulds, SNOPower.itemPassive_Unique_Pants_007_x1 },
            { Legendary.RechelsRingOfLarceny, SNOPower.ItemPassive_Unique_Ring_508_x1 },
            //{ Legendary.BottomlessPotionofKulleAid, SNOPower.X1_Legendary_Potion_06 },
            //{ Legendary.PridesFall, SNOPower.ItemPassive_Unique_Helm_017_x1 },
            { Legendary.KekegisUnbreakableSpirit, SNOPower.ItemPassive_Unique_Ring_569_x1 },
        };

        public static readonly HashSet<SNOActor> PowerGlobeSNO = new HashSet<SNOActor>
        {
            SNOActor.Console_PowerGlobe,
            SNOActor.Console_powerGlobe_castBuff_geo,
            SNOActor.Console_PowerGlobe_NoFlippy,
            SNOActor.Console_PowerGlobe_v2_x1_NoFlippy,
        };

        public static readonly HashSet<SNOActor> ProgressionGlobeSNO = new HashSet<SNOActor>
        {
            SNOActor.p1_normal_rifts_Orb,
            SNOActor.p1_tiered_rifts_Orb_flippy,
            SNOActor.p1_tiered_rifts_Orb,
            SNOActor.p1_rifts_Orb_flippy
        };

        public static readonly HashSet<SNOActor> Shrines = new HashSet<SNOActor>
        {
            SNOActor.Shrine_Global_Fortune,
            SNOActor.Shrine_Global_Blessed,
            SNOActor.Shrine_Global_Frenzied,
            SNOActor.Shrine_Global_Reloaded,
            SNOActor.Shrine_Global_Enlightened,
            SNOActor.Shrine_Global_Hoarder,
            SNOActor.x1_LR_Shrine_Infinite_Casting,
            SNOActor.x1_LR_Shrine_Electrified,
            SNOActor.x1_LR_Shrine_Invulnerable,
            SNOActor.x1_LR_Shrine_Run_Speed,
            SNOActor.x1_LR_Shrine_Damage
        };

        //public static readonly HashSet<SNOAnim> SteedChargeAnimations = new HashSet<SNOAnim>
        //{
        //    SNOAnim.x1_Crusader_Male_SteedCharge,
        //    SNOAnim.x1_Crusader_Female_SteedCharge
        //};
        public static readonly HashSet<SNOAnim> SteedChargeAnimations = new HashSet<SNOAnim>
        {
            SNOAnim.x1_Crusader_Male_1HS_Shield_attack_ShieldBash_Charge_01, //308091,
            SNOAnim.x1_Crusader_Male_1HS_Shield_attack_ShieldBash_Charge_Outtro_01, //308092,
            SNOAnim.x1_Crusader_Female_1HS_Shield_attack_ShieldBash_Charge_Outro, //311186,
            SNOAnim.x1_Crusader_Female_1HS_Shield_attack_ShieldBash_Charge, //311187,
            SNOAnim.x1_Crusader_Female_SteedCharge, //311892,
            SNOAnim.x1_Crusader_Male_SteedCharge,
            SNOAnim.x1_Crusader_Male_cast_Phalanx_ShieldCharge_01, //339946,
            SNOAnim.x1_Crusader_Male_2HT_Shield_attack_ShieldBash_Charge_01, //357447,
            SNOAnim.x1_Crusader_Male_2HT_Shield_attack_ShieldBash_Charge_Outtro_01, //357448,
            SNOAnim.x1_Crusader_Female_2HT_Shield_attack_ShieldBash_Charge, //358735,
            SNOAnim.x1_Crusader_Female_2HT_Shield_attack_ShieldBash_Charge_Outro, //358736,
        };

        public static HashSet<MonsterType> AllyMonsterTypes = new HashSet<MonsterType>
        {
            MonsterType.Ally,
            MonsterType.Scenery,
            MonsterType.Helper,
            MonsterType.Team,
        };

        public static List<SNOPower> AlwaysCanCastPowers = new List<SNOPower>
        {
            SNOPower.Witchdoctor_Hex_Explode,
            SNOPower.Weapon_Ranged_Wand,
            SNOPower.Weapon_Ranged_Projectile,
            SNOPower.Weapon_Melee_Instant,
            SNOPower.Axe_Operate_Gizmo,
            SNOPower.Axe_Operate_NPC,
            SNOPower.Walk,
        };

        public static HashSet<int> ArchonSkillIds = new HashSet<int>
        {
            135166, 135238, 135663, 166616, 167355, 167648, 392883, 392884, 392885, 392886, 392887, 392888, 392889, 392890, 392891,
        };

        public static HashSet<int> AvoidanceBuffSNO = new HashSet<int>
        {
            201454,
            201464,
            201426,
            201438,
            200969,
            201423,
            201242,
        };

        public static Dictionary<int, Vector3> ButcherPanelPositions = new Dictionary<int, Vector3>
        {
            { 201426, new Vector3(121, 121, 0)}, // ButcherLair_FloorPanel_MidMiddle_Base
            { 201242, new Vector3(158, 111, 0)}, // ButcherLair_FloorPanel_LowerLeft_Base
            { 200969, new Vector3(152, 151, 0)}, // ButcherLair_FloorPanel_LowerMid_Base
            { 201438, new Vector3(91, 91, 0)}, // ButcherLair_FloorPanel_UpperMid_Base
            { 201423, new Vector3(133, 78, 0)}, // ButcherLair_FloorPanel_UpperLeft_Base
            { 201464, new Vector3(107, 160, 0)}, // ButcherLair_FloorPanel_LowerRight_Base
            { 201454, new Vector3(80, 134, 0)}, // ButcherLair_FloorPanel_UpperRight_Base
        };

        // Chamber of Suffering (Butcher's Lair)
        public static HashSet<Vector3> ChamberOfSufferingSafePoints = new HashSet<Vector3>
        {
            new Vector3(122.3376f, 120.1721f, 0), // Center
            new Vector3(138.504f,  88.64854f, 0), // Top Left
            new Vector3(98.61596f, 95.93278f, 0), // Top
            new Vector3(93.04589f, 134.9459f, 0), // Top Right
            new Vector3(107.9791f, 150.6952f, 0), // Bottom Right
            new Vector3(146.8563f, 144.0836f, 0), // Bottom
            new Vector3(151.9562f, 104.8417f, 0), // Bottom Left
        };

        public static List<SNOPower> ChargeBasedPowers = new List<SNOPower>
        {
            SNOPower.p6_Necro_BoneSpirit,
            SNOPower.Barbarian_Revenge,
            SNOPower.Barbarian_FuriousCharge,
            SNOPower.X1_Monk_DashingStrike,
            SNOPower.DemonHunter_Sentry,
            SNOPower.DemonHunter_SpikeTrap,
        };

        public static HashSet<SNOPower> CheckVariantBuffs = new HashSet<SNOPower>
        {
            SNOPower.ItemPassive_Unique_Ring_735_x1,
            SNOPower.ItemPassive_Unique_Ring_735_x1,
        };

        public static HashSet<SNOActor> CorruptGrowthIds = new HashSet<SNOActor>()
        {
            SNOActor.a4dun_Garden_Corruption_Monster,
            SNOActor.a4dun_Garden_Corruption_HellRift_Monster,
            SNOActor.a4dun_Garden_Corruption_Monster_Despair
        };

        public static HashSet<SNOActor> CosmeticSnoIds = new HashSet<SNOActor>
        {
            //331905, //ActorId: 331905, Type: Item, Name: Amberwing
            SNOActor.Cosmetic_Portrait_Frame_1,
            SNOActor.Cosmetic_Wings_bird,
            SNOActor.Cosmetic_Portrait_Frame_Whimsyshire,
            SNOActor.cos_wings_Bird_01_wizF,
            SNOActor.AngelWings_common_model, //150054
            SNOActor.Fate_angelWings_model, //159931
            SNOActor.AngelWings_corrupt_model, //170563
            SNOActor.Hope_angelWings_model, //174571
            SNOActor.Hope_angelWings_model_Dim, //195016
            SNOActor.hero_angelWings_model, //198230
            SNOActor.angel_death_wings_dissipate_emitter, //199650
            SNOActor.AngelWings_Blue, //210787
            SNOActor.BugWings, //255315
            SNOActor.hero_bugWings_barbF, //255316
            SNOActor.hero_bugWings_barbM, //255718
            SNOActor.hero_bugWings_DHF, //255719
            SNOActor.hero_bugWings_DHM, //255720
            SNOActor.hero_bugWings_monkF, //255721
            SNOActor.hero_bugWings_monkM, //255722
            SNOActor.hero_bugWings_WDF, //255723
            SNOActor.hero_bugWings_WDM, //255724
            SNOActor.hero_bugWings_wizF, //255725
            SNOActor.hero_bugWings_wizM, //255726
            SNOActor.hero_bugWings_cdrF, //261498
            SNOActor.hero_bugWings_cdrM, //261499
            SNOActor.x1_AngelWings_dark_TEST_model, //301144
            SNOActor.AngelWings_Red, //317018
            SNOActor.hero_redWings_model, //317119
            SNOActor.x1_AngelWings_Short_Cool_model, //328514
            SNOActor.x1_Dark_Angel_Wings, //348370
            SNOActor.x1_Urzael_Wings_flameSweep, //363632
            SNOActor.x1_Urzael_Wings_transform_midway, //363657
            SNOActor.x1_Malthael_Death_Short_Wings_model, //370161
            SNOActor.AngelWings_common_model_noRagdoll, //377926
            SNOActor.hero_angelWings_Imperius_model, //378320
            SNOActor.x1_AngelWings_Imperius, //378342
            SNOActor.hero_angelWings_model_gold, //378978
            SNOActor.Cosmetic_Wings_Blue, //409078
            SNOActor.Cosmetic_Wings_Red, //409079
            SNOActor.Cosmetic_Wings_Bug, //409080
            SNOActor.Cosmetic_Wings_Imperius, //409081
            SNOActor.cos_wings_angel_twoTone_purple_01, //429082
            SNOActor.cos_wings_demonic_blackBlue_01, //429085
            SNOActor.cos_wings_demonic_blackYellow_01, //429087
            SNOActor.cos_wings_angel_twoTone_gold_01, //429091
            SNOActor.cos_wings_demonic_blackRed_01, //429094
            SNOActor.cos_wings_angel_twoTone_greyscale_01, //429103
            SNOActor.cos_wings_angel_flame_cyan_01, //429110
            SNOActor.cos_wings_demonic_blackCyan_01, //429114
            SNOActor.cos_wings_angel_flame_orange_01, //429181
            SNOActor.cos_wings_bat, //429863
            SNOActor.cos_wings_bat_DHM, //431031
            SNOActor.cos_wings_bat_DHF, //431033
            SNOActor.cos_wings_bat_barbM, //431036
            SNOActor.cos_wings_bat_barbF, //431038
            SNOActor.cos_wings_bat_wizM, //431041
            SNOActor.cos_wings_bat_wizF, //431043
            SNOActor.cos_wings_bat_WDM, //431046
            SNOActor.cos_wings_bat_WDF, //431048
            SNOActor.cos_wings_bat_monkM, //431056
            SNOActor.cos_wings_bat_monkF, //431058
            SNOActor.cos_wings_bat_cdrM, //431061
            SNOActor.cos_wings_bat_cdrF, //431063
            SNOActor.cos_wings_bat_02_barbM, //435216
            SNOActor.cos_wings_bat_02_barbF, //435218
            SNOActor.cos_wings_bat_02_wizM, //435222
            SNOActor.cos_wings_bat_02_wizF, //435224
            SNOActor.cos_wings_bat_02_WDM, //435227
            SNOActor.cos_wings_bat_02_WDF, //435229
            SNOActor.cos_wings_bat_02_monkM, //435232
            SNOActor.cos_wings_bat_02_monkF, //435234
            SNOActor.cos_wings_bat_02_cdrM, //435237
            SNOActor.cos_wings_bat_02_cdrF, //435239
            SNOActor.cos_wings_bat_02_DHF, //435240
            SNOActor.cos_wings_bat_02_DHM, //435241
            SNOActor.cos_wings_bat_03_DHM, //435245
            SNOActor.cos_wings_bat_03_DHF, //435247
            SNOActor.cos_wings_bat_03_barbM, //435250
            SNOActor.cos_wings_bat_03_barbF, //435252
            SNOActor.cos_wings_bat_03_wizM, //435255
            SNOActor.cos_wings_bat_03_wizF, //435257
            SNOActor.cos_wings_bat_03_WDM, //435260
            SNOActor.cos_wings_bat_03_WDF, //435262
            SNOActor.cos_wings_bat_03_monkM, //435265
            SNOActor.cos_wings_bat_03_monkF, //435267
            SNOActor.cos_wings_bat_03_cdrM, //435270
            SNOActor.cos_wings_bat_03_cdrF, //435272
            SNOActor.cos_wings_bat_04_DHM, //436085
            SNOActor.cos_wings_bat_04_DHF, //436087
            SNOActor.cos_wings_bat_04_barbM, //436090
            SNOActor.cos_wings_bat_04_barbF, //436092
            SNOActor.cos_wings_bat_04_wizM, //436095
            SNOActor.cos_wings_bat_04_wizF, //436097
            SNOActor.cos_wings_bat_04_WDM, //436100
            SNOActor.cos_wings_bat_04_WDF, //436102
            SNOActor.cos_wings_bat_04_monkM, //436105
            SNOActor.cos_wings_bat_04_monkF, //436107
            SNOActor.cos_wings_bat_04_cdrM, //436110
            SNOActor.cos_wings_bat_04_cdrF, //436112
            SNOActor.cos_wings_bat_05_DHM, //436203
            SNOActor.cos_wings_bat_05_DHF, //436206
            SNOActor.cos_wings_bat_05_barbM, //436209
            SNOActor.cos_wings_bat_05_barbF, //436211
            SNOActor.cos_wings_bat_05_wizM, //436214
            SNOActor.cos_wings_bat_05_wizF, //436216
            SNOActor.cos_wings_bat_05_WDM, //436219
            SNOActor.cos_wings_bat_05_WDF, //436221
            SNOActor.cos_wings_bat_05_monkM, //436224
            SNOActor.cos_wings_bat_05_monkF, //436226
            SNOActor.cos_wings_bat_05_cdrM, //436229
            SNOActor.cos_wings_bat_05_cdrF, //436231
            SNOActor.cos_wings_bat_06_DHM, //436236
            SNOActor.cos_wings_bat_06_DHF, //436239
            SNOActor.cos_wings_bat_06_barbM, //436242
            SNOActor.cos_wings_bat_06_barbF, //436244
            SNOActor.cos_wings_bat_06_wizM, //436247
            SNOActor.cos_wings_bat_06_wizF, //436249
            SNOActor.cos_wings_bat_06_WDM, //436252
            SNOActor.cos_wings_bat_06_WDF, //436254
            SNOActor.cos_wings_bat_06_monkM, //436257
            SNOActor.cos_wings_bat_06_monkF, //436259
            SNOActor.cos_wings_bat_06_cdrM, //436262
            SNOActor.cos_wings_bat_06_cdrF, //436264
            SNOActor.cos_wings_bat_07_DHM, //437351
            SNOActor.cos_wings_bat_07_DHF, //437353
            SNOActor.cos_wings_bat_07_barbM, //437356
            SNOActor.cos_wings_bat_07_barbF, //437359
            SNOActor.cos_wings_bat_07_wizM, //437362
            SNOActor.cos_wings_bat_07_wizF, //437364
            SNOActor.cos_wings_bat_07_WDM, //437368
            SNOActor.cos_wings_bat_07_WDF, //437370
            SNOActor.cos_wings_bat_07_monkM, //437373
            SNOActor.cos_wings_bat_07_monkF, //437375
            SNOActor.cos_wings_bat_07_cdrM, //437378
            SNOActor.cos_wings_bat_07_cdrF, //437380
            SNOActor.cos_wings_angel_twoTone_blue_01, //437586
        };

        public static HashSet<SNOActor> CursedChestSNO = new HashSet<SNOActor>
        {
            SNOActor.x1_Global_Chest_CursedChest,
            SNOActor.x1_Global_Chest_CursedChest_B,
            SNOActor.x1_Global_Chest_CursedChest_B_MutantEvent,
        };

        public static HashSet<SNOActor> CursedShrineSNO = new HashSet<SNOActor>
        {
            //x1_Event_CursedShrine
            (SNOActor)364601,
            //SNOActor.x1_Event_CursedShrine_Heaven
            (SNOActor)368169,
        };

        /// <summary>
        /// ActorSnoId that we always want to completely ignore
        /// </summary>
        public static HashSet<SNOActor> ExcludedActorIds = new HashSet<SNOActor>
        {
            SNOActor.Invalid,
            SNOActor.Generic_Proxy,
            SNOActor.Start_Location_0,
            SNOActor.X1_AdventureMode_HubBanterTrigger,
            SNOActor.BoxTrigger,
            SNOActor.SphereTrigger,
            SNOActor.BoxTriggerOneShot,
            SNOActor.Checkpoint,
            SNOActor.TriggerSphereOneShot,
            SNOActor.Savepoint,
            (SNOActor)143853, //DH_discipline ActorSnoId=143853
            (SNOActor)143854, //DH_hatred ActorSnoId=143854
            (SNOActor)4433, //HealthBall_forecast ActorSnoId=4433
            SNOActor.bottomBarUI,
            SNOActor.JobsDone,
            SNOActor.bottomBar_buttonFX,
            SNOActor.JobsDone_shineBox,
            SNOActor.actor_isSlowed_color,
            SNOActor.HirelingHeadstone,
            (SNOActor)4434, //HealthBall_liquid ActorSnoId=4434
            SNOActor.invisBoxCollision_flippy,
            SNOActor.g_LightGlow_Orange,
            SNOActor.a3dun_battlefield_Demon_ChainPylon_locked,
        };

        /// <summary>
        /// Actor types that we always want to completely ignore
        /// </summary>
        public static HashSet<ActorType> ExcludedActorTypes = new HashSet<ActorType>
        {
            ActorType.Environment,
            ActorType.AxeSymbol,
            //ActorType.CustomBrain, // Some avoidance are custom brain like herald of pestilence creepMobArm
            //ActorType.ClientEffect, // Some avoidance are client effect like thunderstorm, fastmummy poison clouds.
            ActorType.Invalid,
            ActorType.Critter,
        };

        public static HashSet<int> ExtendedKillRadiusSceneSnoIds = new HashSet<int>
        {
            28768, // trDun_Crypt_NSEW_JarSouls_01
        };

        public static HashSet<int> FastMonsterSNO = new HashSet<int>
        {
            5212
        };

        //public static Dictionary<int, AvoidanceType> AvoidanceProjectileSNO = new Dictionary<int, AvoidanceType>
        //{
        //    { 343539, AvoidanceType.Orbiter },
        //    { 316389, AvoidanceType.PoisonEnchanted },
        //    { 340319, AvoidanceType.PoisonEnchanted },
        //    { 5212, AvoidanceType.BeeWasp },
        //    { 4103, AvoidanceType.ShamanFire },
        //    { 160154, AvoidanceType.MoltenBall },
        //    { 123842, AvoidanceType.AzmoFireball },
        //    { 139741, AvoidanceType.ZoltTwister },
        //    { 166686, AvoidanceType.MaghdaProjectille },
        //    { 185999, AvoidanceType.DiabloRingOfFire },
        //    { 136533, AvoidanceType.DiabloLightning },
        //};
        public static HashSet<SNOActor> GoldSNO = new HashSet<SNOActor>
        {
            SNOActor.GoldCoin,
            SNOActor.GoldLarge,
            SNOActor.GoldMedium,
            SNOActor.GoldSmall,
            SNOActor.PlacedGold,
            SNOActor.GoldCoins,
        };

        //public static Dictionary<int, AvoidanceType> AvoidanceTypeSNO = new Dictionary<int, AvoidanceType>
        //{
        //    {349774, AvoidanceType.FrozenPulse},
        //    {343539, AvoidanceType.Orbiter},
        //    {316389, AvoidanceType.PoisonEnchanted},
        //    {340319, AvoidanceType.PoisonEnchanted},
        //    {341512, AvoidanceType.Thunderstorm},
        //    {337109, AvoidanceType.Wormhole},
        //    {123839, AvoidanceType.AzmodanBody},
        //    {123124, AvoidanceType.AzmodanPool},
        //    {123842, AvoidanceType.AzmoFireball},
        //    {219702, AvoidanceType.Arcane},
        //    {221225, AvoidanceType.Arcane},
        //    {3337, AvoidanceType.BeastCharge},
        //    {5212, AvoidanceType.BeeWasp},
        //    {161822, AvoidanceType.Belial},
        //    {161833, AvoidanceType.Belial},
        //    {201454, AvoidanceType.ButcherFloorPanel},
        //    {201464, AvoidanceType.ButcherFloorPanel},
        //    {201426, AvoidanceType.ButcherFloorPanel},
        //    {201438, AvoidanceType.ButcherFloorPanel},
        //    {200969, AvoidanceType.ButcherFloorPanel},
        //    {201423, AvoidanceType.ButcherFloorPanel},
        //    {201242, AvoidanceType.ButcherFloorPanel},
        //    {226350, AvoidanceType.DiabloRingOfFire},
        //    {226525, AvoidanceType.DiabloRingOfFire},
        //    {84608, AvoidanceType.Desecrator},
        //    {93837, AvoidanceType.GhomGas},
        //    {3847, AvoidanceType.Grotesque},
        //    {168031, AvoidanceType.DiabloPrison},
        //    {214845, AvoidanceType.DiabloMeteor},
        //    {402, AvoidanceType.IceBall},
        //    {223675, AvoidanceType.IceBall},
        //    {260377, AvoidanceType.IceTrail},
        //    {432, AvoidanceType.MageFire},
        //    {166686, AvoidanceType.MaghdaProjectille},
        //    {160154, AvoidanceType.MoltenBall},
        //    {4803, AvoidanceType.MoltenCore},
        //    {4804, AvoidanceType.MoltenCore},
        //    {224225, AvoidanceType.MoltenCore},
        //    {247987, AvoidanceType.MoltenCore},
        //    {95868, AvoidanceType.MoltenTrail},
        //    {250031, AvoidanceType.Mortar},
        //    {108869, AvoidanceType.PlagueCloud},
        //    {3865, AvoidanceType.PlagueHand},
        //    {5482, AvoidanceType.PoisonTree},
        //    {6578, AvoidanceType.PoisonTree},
        //    {4103, AvoidanceType.ShamanFire},
        //    //{185924, AvoidanceType.ZoltBubble},
        //    {139741, AvoidanceType.ZoltTwister}
        //};
        public static HashSet<SNOActor> HealthGlobeSNO = new HashSet<SNOActor>
        {
            SNOActor.HealthGlobe,
            SNOActor.HealthGlobe_02,
            SNOActor.healthGlobe_swipe,
            SNOActor.HealthGlobe_03,
            SNOActor.HealthGlobe_04,
            SNOActor.X1_NegativeHealthGlobe,
            SNOActor.x1_healthGlobe,
            SNOActor.x1_healthGlobe_playerIsHealed_attract,
            SNOActor.HealthGlobe_steak,
            SNOActor.HealthGlobe_steak_02,
            SNOActor.x1_healthGlobe_steak_model,
        };

        public static HashSet<int> HealthWellSNO = new HashSet<int>
        {
            //caOut_Healthwell
            3648,
            //HealthWell_Global
            138989,
            //HealthWell_Water_Plane
            139129,
            //a4_Heaven_HealthWell_Global
            218885,
            //a4dun_DIablo_Arena_Health_Well
            180575,
        };

        public static HashSet<SNOActor> HerdingMatsSnoIds = new HashSet<SNOActor>()
        {
            SNOActor.Cow_Gem_Flippy,
            SNOActor.Cow_Gem,
            SNOActor.Cow_Bone,
            SNOActor.Cow_Water,
            SNOActor.CowBell,
        };

        public static List<SNOPower> InteractPowers = new List<SNOPower>
        {
            SNOPower.Axe_Operate_Gizmo,
            SNOPower.Axe_Operate_NPC,
        };

        public static Dictionary<int, MonsterType> MonsterTypeOverrides = new Dictionary<int, MonsterType>
        {
            { 86624, MonsterType.Undead }, // Jondar, DB thinks he's permanent ally
        };

        public static HashSet<MonsterType> NonHostileMonsterTypes = new HashSet<MonsterType>
        {
            MonsterType.Ally,
            MonsterType.Scenery,
            MonsterType.Team,
            MonsterType.None,
            MonsterType.Helper,
        };

        /// <summary>
        /// Mark the area underneath these objects @ radius 'unwalkable' - use with caution.
        /// </summary>
        public static Dictionary<SNOActor, float> PathFindingObstacles = new Dictionary<SNOActor, float>
        {
            {SNOActor.a3_Battlefield_demonic_forge, 16f},
            {SNOActor.a3_crater_st_demonic_forge, 16f},
            {SNOActor.Player_Shared_Stash, 6f},
            {SNOActor.PoolOfReflection, 8f},
            {SNOActor.a2dun_Aqd_Act_Wood_Platform_A_01, 5f}
        };

        public static HashSet<SNOActor> PetSnoIds = new HashSet<SNOActor>()
        {
            SNOActor.x1_promoPet_fallenHound_skeletal, // 360090,
            SNOActor.Cosmetic_Pet_FallenHound_Skeletal, // 410448,
            SNOActor.cos_pet_teddybear_01, // 430903,
            SNOActor.cos_pet_thousandPounder_01, // 430905,
            SNOActor.cos_pet_teddybear_02, // 430906,
            SNOActor.cos_pet_teddybear_03, // 430908,
            SNOActor.cos_pet_teddybear_04, // 430910,
            SNOActor.cos_pet_fallenHound_01, // 433063,
            SNOActor.cos_pet_protossProbe_01, // 434428,
            SNOActor.cos_pet_crawlingHand_01, // 436532,
            SNOActor.cos_pet_crawlingHand_02, // 436534,
            SNOActor.cos_pet_crawlingHand_03, // 436961,
            SNOActor.cos_pet_teddybear_05, // 437119,
            SNOActor.cos_pet_fallenHound_02, // 437807,
            SNOActor.cos_pet_fallenHound_03, // 439196,
            SNOActor.cos_pet_fallenHound_04, // 439221,
            SNOActor.cos_pet_fallenHound_05, // 439233,
            SNOActor.cos_pet_fallenHound_06, // 439237,
            SNOActor.cos_pet_goatman_melee_01, // 439337,
            SNOActor.p3_cos_pet_Murkgoblin_A, // 441208,
            SNOActor.cos_pet_goatman_melee_02, // 441214,
            SNOActor.p3_cos_pet_jadeSerpent, // 441437,
            SNOActor.Cosmetic_Pet_hoodednightmare_01,
            SNOActor.Cosmetic_Pet_squire,
            SNOActor.Cosmetic_Pet_cowking_01,
            SNOActor.Cosmetic_Pet_bladedsoul_01,
		    SNOActor.treasureGoblin_K,
            SNOActor.Cosmetic_Pet_mimic_01,
            SNOActor.TentacleBear_C_Unique_Cosmetic_02,
            SNOActor.Cosmetic_Portrait_Frame_1,

            SNOActor.Cosmetic_Pet_ghost_01,  //ActorId:  Type: Item, Name: Haunting Hannah
            SNOActor.Cosmetic_Pet_fallenHound_04, //ActorId:  Type: Item, Name: Buddy
            SNOActor.Cosmetic_Pet_spider_01, //ActorId:  Type: Item, Name: Charlotte
            SNOActor.Cosmetic_Pet_teddyBear_04, //Type: Item, Name: Overseer Lady Josephine
            SNOActor.Cosmetic_Pet_VoodooDoll_01, //Type: Item, Name: Ms. Madeleine
            SNOActor.Cosmetic_Pet_zombie_01, //Type: Item, Name: Liv Moore
            SNOActor.Cosmetic_Pet_ghost_01, //Type: Item, Name: Haunting Hannah
            SNOActor.Cosmetic_Pet_FloatingSkull_02, //Type: Item, Name: Humbart Wessel
            SNOActor.Cosmetic_Pet_succubus_01, //Cosmetic_Pet_succubus_01-39125 ActorSnoId: 451042
            SNOActor.cos_pet_squire, //Monster Name: cos_pet_squire-216 ActorSnoId: 445705,
            SNOActor.Cosmetic_Pet_fallenHound_04, //Type: Item, Name: Buddy
            SNOActor.cos_pet_spider_01,  //Name: cos_pet_spider_01-10773 ActorSnoId: 450065, Distance: 545.0016
            SNOActor.Cosmetic_Pet_goatman_melee_02, //Lady Morhanlu: ActorId: 451017, Type: Item, Name: ?????
            SNOActor.cos_pet_teddybear_04, //20DFD214] Type: Monster Name: cos_pet_teddybear_04-411489 ActorSnoId: 430910, Distance: 11.76465
            SNOActor.Cosmetic_Pet_FloatingSkull_02, //ActorId: 451031, Type: Item, Name: Cosmetic_Pet_FloatingSkull_02-22954, Distance2d: 23.11925, CollisionRadius: 0, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0

            SNOActor.Cosmetic_Pet_FloatingSkull_01, //Blaze ActorId: 451025, Type: Item, Name: ?????
            SNOActor.Cosmetic_Pet_crawlingHand_03, //Friendly Gauntlet //ActorId: 451049, Type: Item, Name: ?????? ????

            //ActorId: 451031, Type: Item, Name: Humbart Wessel
        };

        public static GameBalanceTable PetTable = new GameBalanceTable
        {
            { 2390, 95923716, -1080354364, "Cosmetic_Pet_gluttony_01" },
            { 2391, 703053224, 1957183816, "P5_Cosmetic_Pet_gluttony_01" },
            { 2392, -529993811, -2093357235, "P5_Cosmetic_Pet_babysiege_01" },
            { 2393, 909568905, 747097929, "Cosmetic_Pet_babysiege_01" },
            { 2394, 291388177, 1463579313, "Cosmetic_Pet_fallenHound_04" },
            { 2395, 270653493, 584358837, "P5_Cosmetic_Pet_fallenHound_04" },
            { 2396, -210446749, -1284908509, "Cosmetic_Pet_ghost_01" },
            { 2397, 2057463943, 1567133735, "P5_Cosmetic_Pet_ghost_01" },
            { 2398, 402823953, 600203953, "P5_Cosmetic_Pet_goatman_melee_02" },
            { 2399, 1508058349, 293595309, "Cosmetic_Pet_goatman_melee_02" },
            { 2400, -661725708, -463270508, "P5_Cosmetic_Pet_hoodednightmare_01" },
            { 2401, 347688656, 647365264, "Cosmetic_Pet_hoodednightmare_01" },
            { 2402, 1270811089, 780480881, "P5_Cosmetic_Pet_mimic_01" },
            { 2403, -997099603, -2071561363, "Cosmetic_Pet_mimic_01" },
            { 2404, -968523326, 1763080546, "P5_Cosmetic_Pet_nightmare_02" },
            { 2405, 471039390, 308568414, "Cosmetic_Pet_nightmare_02" },
            { 2406, 1336361309, -127619875, "Cosmetic_Pet_FloatingSkull_01" },
            { 2407, 231126913, 178988769, "P5_Cosmetic_Pet_FloatingSkull_01" },
            { 2408, 231126914, 178988770, "P5_Cosmetic_Pet_FloatingSkull_02" },
            { 2409, 1336361310, -127619874, "Cosmetic_Pet_FloatingSkull_02" },
            { 2410, 1209305989, 111806277, "Cosmetic_Pet_spider_01" },
            { 2411, -1259052503, -260080183, "P5_Cosmetic_Pet_spider_01" },
            { 2412, 719029307, 951272411, "P5_Cosmetic_Pet_squire" },
            { 2413, 1967180567, -660480297, "Cosmetic_Pet_squire" },
            { 2414, -593309777, 660820815, "P5_Cosmetic_Pet_succubus_01" },
            { 2415, -1200439285, 1918249931, "Cosmetic_Pet_succubus_01" },
            { 2416, 1327895705, 1387230233, "P5_Cosmetic_Pet_teddyBear_04" },
            { 2417, -1527508875, -67281899, "Cosmetic_Pet_teddyBear_04" },
            { 2418, -1125588669, 783287459, "P5_Cosmetic_Pet_VoodooDoll_01" },
            { 2419, -864659297, 29027359, "Cosmetic_Pet_VoodooDoll_01" },
            { 2420, -1358268778, 140259862, "P5_Cosmetic_Pet_crawlingHand_03" },
            { 2421, -674024206, -910235502, "Cosmetic_Pet_crawlingHand_03" },
            { 2422, -1526517200, 910697968, "Cosmetic_Pet_cowking_01" },
            { 2423, -1377968812, 1523346676, "P5_Cosmetic_Pet_cowking_01" },
            { 2424, -1967557379, 1260835005, "Cosmetic_Pet_bladedsoul_01" },
            { 2425, 2066480545, 2015095105, "P5_Cosmetic_Pet_bladedsoul_01" },
            { 2426, -819510535, -1632973127, "Cosmetic_Pet_yeti_01" },
            { 2427, 941170845, 15258685, "P5_Cosmetic_Pet_yeti_01" },
            { 2428, -1162235772, 2035231812, "Cosmetic_Pet_zombie_01" },
            { 2429, 664373032, 1663345352, "P5_Cosmetic_Pet_zombie_01" },
        };

        public static HashSet<SNOActor> PlayerBannerSNO = new HashSet<SNOActor>
        {
            SNOActor.Banner_Player_1,
            SNOActor.Banner_Player_2,
            SNOActor.Banner_Player_3,
            SNOActor.Banner_Player_4,
            SNOActor.Banner_Player_1_Act2,
            SNOActor.Banner_Player_2_Act2,
            SNOActor.Banner_Player_3_Act2,
            SNOActor.Banner_Player_4_Act2,
            SNOActor.Banner_Player_1_Act5,
            SNOActor.Banner_Player_2_Act5,
            SNOActor.Banner_Player_3_Act5,
            SNOActor.Banner_Player_4_Act5,
        };

        public static List<TrinityObjectType> PlayerBlockingTypes = new List<TrinityObjectType>
        {
            TrinityObjectType.Barricade,
            TrinityObjectType.Door
        };

        /// <summary>
        /// SNOs that belong to a player, spells, summons, effects, projectiles etc.
        /// </summary>
        public static HashSet<int> PlayerOwnedSNO = new HashSet<int>
        {
            493, 3368, 6509, 6511, 6513, 6514, 6515, 6516, 6519, 6522, 6523, 6524, 6528, 6535, 6542, 6543, 6550, 6551, 6553, 6554,
            6558, 6560, 6561, 6562, 6563, 58362, 61419, 61445, 62054, 71129, 75631, 75642, 75726, 75731, 75732, 76019, 77097, 77098,
            77116, 77333, 77805, 80600, 80745, 80757, 80758, 81103, 81229, 81230, 81231, 81232, 81238, 81239, 81515, 82109, 82111,
            82660, 82972, 83024, 83025, 83028, 83043, 83959, 83964, 84504, 86082, 86769, 86790, 87621, 87913, 88032, 90364, 91424,
            91440, 91441, 91702, 92030, 92031, 92032, 93067, 93076, 93560, 93582, 93592, 93718, 97691, 97821, 98010, 99565, 99566,
            99567, 99572, 99574, 99629, 107916, 112560, 112572, 112585, 112588, 112594, 112675, 112697, 112806, 112808, 112811,
            117557, 130029, 130030, 130035, 130073, 130074, 130668, 141936, 144003, 147977, 148060, 148070, 148077, 148220, 148634,
            148700, 149837, 154769, 161695, 161772, 162301, 164699, 166051, 166130, 166172, 167260, 167261, 167262, 167263, 167382,
            167397, 167419, 167463, 167564, 167628, 167724, 167807, 167814, 167817, 167978, 170169, 170199, 170268, 170287, 170385,
            170405, 170445, 170496, 170574, 170592, 170935, 171179, 171180, 171184, 171185, 171225, 171226, 176247, 176248, 176262,
            176265, 176287, 176288, 176356, 176390, 176407, 176440, 176600, 176653, 185106, 185125, 185226, 185233, 185263, 185273,
            185283, 185301, 185309, 185316, 185459, 185513, 185660, 185661, 185662, 185663, 189372, 189373, 189375, 189458, 189460,
            191967, 192126, 192210, 192211, 192271, 199154, 201526, 210804, 210896, 215324, 215420, 215488, 215511, 215516, 215700,
            215711, 215809, 215853, 216040, 216069, 216462, 216529, 216817, 216818, 216851, 216874, 216890, 216897, 216905, 216941,
            216956, 216975, 216988, 217121, 217130, 217139, 217142, 217172, 217180, 217287, 217307, 217311, 217457, 217458, 217459,
            219070, 219196, 219200, 219254, 219292, 219295, 219300, 219314, 219315, 219316, 219391, 219392, 219393, 226648, 249225,
            249226, 249227, 249228, 249975, 249976, 251688, 251689, 251690, 261341, 261342, 261343, 261344, 261616, 261617, 299099,
            300476, 302468, 314792, 315588, 316207, 316239, 316270, 316271, 317398, 317409, 317501, 317507, 317652, 317809, 319692,
            319698, 319732, 319771, 322022, 322236, 322350, 322406, 322488, 323029, 323092, 323149, 323897, 324143, 324451, 324459,
            324466, 325154, 325552, 325804, 325807, 325813, 325815, 326285, 326305, 326308, 326313, 326755, 328146, 328161, 328171,
            328199, 336410, 337757, 339443, 339473, 341373, 341381, 341396, 341410, 341411, 341412, 341426, 341427, 341441, 341442,
            342082, 343197, 343293, 343300, 347101, 362960, 362961, 366983, 381908, 381915, 381917, 381919, 385215, 385216, 394102,
            396290, 396291, 396292, 396293, 409287, 409352, 409430, 409523, 322, 3276, 3277, 3280, 3281, 3282, 3283, 3289, 3290,
            3291, 3297, 3298, 3303, 3305, 3306, 3308, 3309, 3310, 3314, 3315, 3316, 3317, 3319, 51303, 74636, 79400, 90443, 90535,
            90536, 92895, 93481, 93903, 99541, 100800, 100832, 100839, 100934, 101068, 108742, 108746, 108767, 108772, 108784,
            108789, 108808, 108819, 108868, 108907, 108920, 109151, 136261, 143994, 158990, 159030, 159614, 159626, 159631, 159940,
            160587, 160685, 160818, 160893, 161452, 161457, 161599, 161607, 161654, 161657, 161890, 161892, 161893, 161894, 161960,
            162005, 162087, 162114, 162386, 162387, 162548, 162577, 162590, 162593, 162621, 162622, 162623, 162766, 162839, 162920,
            162929, 163353, 163462, 163494, 163501, 163541, 163552, 163783, 163792, 163861, 163925, 163949, 163968, 164066, 164112,
            164708, 164709, 164710, 164712, 164713, 164714, 164747, 164770, 164788, 164804, 165040, 165043, 165069, 165381, 165382,
            165514, 165515, 165560, 165561, 165988, 166214, 166222, 166223, 166438, 168307, 168440, 168460, 173342, 174723, 189078,
            189094, 198346, 209487, 220559, 220562, 220565, 220569, 220632, 252478, 252479, 317733, 356731, 358574, 360571, 362283,
            362949, 362951, 363301, 363760, 363765, 364460, 364953, 365194, 365291, 365338, 365340, 365342, 365534, 365789, 373063,
            373074, 373211, 374667, 374683, 375757, 396470, 408515, 408532, 410567, 75678, 75887, 77569, 77604, 77813, 87564, 88244,
            88251, 111307, 111330, 111503, 129228, 129603, 129621, 129785, 129787, 129788, 129934, 131701, 141937, 143995, 147809,
            148845, 148846, 148847, 149944, 149946, 149947, 149948, 151832, 151842, 153029, 153864, 153865, 153866, 153867, 153868,
            154674, 155092, 155147, 155749, 155938, 158843, 158844, 158916, 158917, 158940, 158941, 160932, 165558, 166549, 166550,
            166556, 166557, 166582, 166583, 166584, 166585, 166618, 166619, 166620, 166621, 166636, 166637, 167169, 167171, 167172,
            167218, 194565, 194566, 200561, 200672, 200808, 212547, 314804, 347447, 349626, 360546, 360547, 360548, 360549, 360550,
            360561, 360563, 360564, 360697, 360773, 361118, 361213, 361214, 362954, 362955, 428572, 428574, 129784, 129932, 130366,
            130572, 130661, 131664, 131672, 132068, 132615, 132732, 133714, 133741, 134841, 134917, 135207, 136149, 141402, 141681,
            141734, 143853, 143854, 147960, 148788, 148900, 149338, 149770, 149790, 149935, 149949, 149975, 150024, 150025, 150026,
            150027, 150036, 150037, 150038, 150039, 150061, 150062, 150063, 150064, 150065, 150449, 151591, 151805, 151929, 151998,
            152116, 152269, 152589, 152736, 152857, 152863, 153075, 153352, 154093, 154194, 154198, 154199, 154200, 154201, 154227,
            154292, 154590, 154591, 154592, 154593, 154595, 154657, 154736, 154750, 154811, 155096, 155149, 155159, 155276, 155280,
            155353, 155374, 155376, 155734, 155848, 156100, 157728, 159098, 159102, 159144, 160612, 162563, 165340, 165467, 165767,
            166462, 166613, 166732, 167223, 167235, 168815, 173827, 178664, 180622, 180640, 181748, 182234, 182263, 186050, 193463,
            193493, 193496, 193497, 193499, 193500, 196030, 196615, 200810, 215242, 215727, 218467, 218504, 219494, 219509, 219534,
            219577, 219580, 219609, 219610, 220527, 220800, 221261, 221440, 222109, 222115, 222117, 222122, 222128, 222130, 222135,
            222141, 222143, 222151, 230674, 251704, 251710, 261665, 366893, 366897, 366921, 366933, 366935, 367223, 367232, 367251,
            367258, 370495, 370496, 375869, 375871, 376739, 376743, 396318, 405388, 406116, 408096, 408100, 408103, 408300, 408327,
            408333, 408335, 408379, 408422, 408568, 410274, 410299, 428075, 253211, 253416, 253717, 255056, 256083, 256180, 257376,
            257777, 257782, 258133, 263866, 265474, 266503, 277722, 277805, 277808, 280196, 280462, 280702, 284686, 284872, 284920,
            285209, 285380, 285476, 286805, 286925, 289010, 289991, 290291, 290315, 290325, 290330, 290340, 290460, 290494, 290508,
            290521, 290532, 291672, 292175, 292608, 292837, 292849, 293223, 293293, 293307, 293312, 293318, 293332, 293342, 293644,
            293682, 293701, 293725, 293762, 293769, 293773, 293797, 293843, 293850, 293866, 293872, 293895, 293912, 293915, 293919,
            293927, 293930, 293947, 293998, 294005, 294009, 294012, 294023, 294047, 294052, 294057, 294169, 294182, 294330, 294333,
            294336, 294340, 294348, 294351, 294355, 294358, 294361, 294365, 294368, 294371, 294375, 294378, 294381, 294436, 294439,
            294442, 294446, 294449, 294452, 294456, 294459, 294462, 294466, 294469, 294472, 294476, 294479, 294482, 294517, 294520,
            294523, 294527, 294530, 294533, 294537, 294540, 294543, 294547, 294550, 294553, 294557, 294560, 294563, 294732, 295009,
            295012, 295015, 300142, 304115, 306040, 306210, 306225, 308143, 312522, 312552, 312658, 312661, 314802, 316079, 316835,
            319402, 323582, 324046, 324050, 324059, 324081, 324126, 324856, 324878, 325092, 325528, 326101, 327375, 327987, 329016,
            330019, 330042, 330082, 330728, 332236, 332450, 332465, 332631, 332644, 332702, 332705, 332759, 332760, 335993, 335995,
            336138, 336201, 336202, 336203, 336279, 336285, 336286, 336339, 336356, 336360, 336361, 336468, 336710, 336968, 337088,
            337184, 338224, 338225, 338226, 338227, 338246, 338252, 338598, 338678, 338807, 339343, 340460, 342209, 342213, 342257,
            342332, 342336, 342562, 342581, 342587, 342919, 342938, 342940, 342945, 343022, 343099, 343180, 343250, 343262, 343614,
            343792, 343879, 343881, 343954, 344224, 344305, 344330, 344543, 344546, 344571, 344573, 344577, 344588, 345114, 345224,
            345228, 345232, 345249, 345295, 345296, 345305, 345682, 345787, 345800, 345892, 346151, 346291, 346293, 346915, 347135,
            347248, 347249, 347360, 347421, 347466, 347643, 347647, 347677, 347792, 347798, 348203, 348262, 348731, 348735, 348766,
            348991, 348993, 349465, 349483, 349485, 349498, 349509, 349510, 349534, 349540, 349705, 349895, 349932, 349942, 349943,
            349959, 349977, 349989, 349991, 350052, 350072, 350083, 350135, 350158, 350204, 350219, 350461, 350468, 350631, 350634,
            350636, 350685, 350686, 350706, 350808, 351031, 351051, 351093, 351139, 352414, 352433, 352455, 352677, 352680, 352683,
            352687, 352723, 352737, 352757, 352871, 352933, 352942, 352969, 352985, 353516, 353843, 354249, 354412, 354579, 354606,
            354969, 355126, 355171, 356719, 356879, 356904, 357248, 357258, 357261, 357317, 357350, 357351, 357355, 357356, 357358,
            357369, 357381, 357382, 357387, 357388, 357426, 357432, 357436, 357439, 357472, 357475, 357574, 357584, 357587, 357645,
            357802, 357803, 357903, 357914, 357916, 357946, 357949, 358001, 358010, 358106, 358235, 358236, 358243, 358244, 358245,
            358246, 359040, 359041, 359042, 359043, 359044, 359045, 359709, 362665, 362952, 362953, 363775, 363890, 369795, 369975,
            369976, 369977, 369978, 369979, 369980, 428280, 6443, 6450, 6451, 6452, 6453, 6457, 6459, 6463, 6465, 6489, 51353,
            59155, 61398, 71336, 102723, 103215, 103217, 103235, 104079, 105606, 105763, 105772, 105792, 105795, 105816, 105828,
            105829, 106426, 106561, 106569, 106593, 106841, 106862, 107507, 107662, 107881, 107889, 107899, 108238, 108389, 108520,
            108536, 108543, 108550, 108556, 108560, 109122, 109123, 110959, 111243, 111338, 111345, 111372, 111530, 111535, 111566,
            113765, 120950, 121595, 121904, 121908, 121919, 121920, 121960, 122281, 122305, 123587, 123910, 123911, 123912, 123913,
            131202, 131504, 131640, 135016, 146534, 171491, 171501, 171502, 175354, 179772, 179776, 179778, 179779, 179780, 181767,
            181773, 181818, 181842, 181867, 181871, 181880, 182042, 182050, 182056, 182095, 182102, 182119, 182136, 182153, 182574,
            182576, 182603, 182608, 182610, 182612, 183977, 184445, 184585, 184968, 184999, 185025, 186469, 188447, 188484, 191204,
            193964, 193965, 193966, 193967, 193968, 193969, 193970, 193971, 193972, 193973, 194308, 194359, 216050, 251637, 348308,
            354714, 356987, 356991, 357125, 357569, 357846, 358018, 358120, 358358, 358653, 359900, 361799, 366173, 376693, 395431,
            404291, 404352, 404785, 404802, 405092, 405095, 405096, 405097, 405098, 405104, 426125, 426135, 432690, 432691, 432692,
            432693, 432694, 432695, 6483, 6487, 6494, 69308, 71643, 74042, 74056, 101441, 105501, 105502, 105812, 105953, 105955,
            105956, 105957, 105958, 105969, 105977, 106385, 106502, 106504, 106584, 106731, 106749, 107011, 107030, 107031, 107035,
            107067, 107107, 107112, 107114, 107162, 107223, 107265, 107705, 110714, 112311, 112327, 112338, 112345, 112347, 117574,
            134115, 134121, 134123, 134125, 134127, 134128, 140874, 144001, 182271, 182276, 182278, 182283, 186485, 193295, 206229,
            206230, 215811, 215813, 215814, 215815, 215816, 215817, 215818, 215819, 215820, 215822, 215835, 215841, 215844, 215847,
            215852, 218915, 218916, 251609, 251610, 314817, 362958, 362959, 3919, 4699, 4700, 4701, 4716, 4719, 4724, 4725, 4730,
            4731, 51327, 59822, 71909, 97399, 97458, 97558, 98829, 98835, 98836, 98871, 98883, 98940, 99063, 99096, 99694, 101550,
            110525, 110526, 110546, 110549, 117329, 120216, 120239, 120365, 121145, 122566, 123865, 123885, 128993, 129183, 129197,
            129329, 129333, 136022, 136893, 136925, 137408, 137413, 137491, 137525, 137527, 137528, 137567, 137572, 137656, 137675,
            137752, 137761, 137767, 137779, 137780, 137781, 137848, 137859, 137928, 137943, 137964, 137968, 137976, 138261, 139403,
            139419, 139431, 139455, 139780, 139869, 139889, 140271, 140312, 140779, 140878, 141074, 141081, 141143, 141175, 141186,
            141192, 141198, 141341, 141354, 141376, 141577, 141581, 141622, 141630, 141700, 141736, 141773, 141938, 142048, 142433,
            142478, 142503, 142514, 142719, 142737, 142788, 142826, 142845, 142851, 143216, 143225, 143273, 143445, 143504, 143509,
            143513, 143598, 143601, 143759, 143770, 143773, 143776, 143797, 143799, 143800, 143806, 143814, 143818, 143996, 144045,
            144046, 144100, 144199, 144209, 144214, 144218, 144234, 144461, 144765, 144770, 144773, 144774, 144775, 144776, 144785,
            144786, 144788, 144789, 144790, 144791, 144832, 144837, 144838, 144840, 144841, 144842, 144843, 144891, 144949, 144950,
            144952, 144953, 144954, 144955, 144974, 145010, 145025, 145028, 145089, 145195, 145196, 145295, 145310, 145461, 145485,
            145503, 145541, 145659, 145685, 145709, 145715, 146041, 146048, 146502, 146593, 146596, 147043, 147136, 147227, 147239,
            147257, 147934, 147935, 147936, 147937, 147938, 149848, 149849, 149851, 150410, 150411, 150412, 150413, 150414, 150415,
            150416, 150417, 150418, 150419, 168878, 169077, 169123, 169890, 169891, 169904, 169905, 169906, 169907, 169908, 169909,
            170972, 171008, 172170, 172187, 172191, 172193, 172489, 176921, 176924, 176955, 181336, 181431, 181568, 182353, 182360,
            182365, 182370, 182384, 191350, 192095, 192103, 197887, 202172, 213766, 215087, 215205, 215635, 218913, 218914, 223650,
            224033, 224150, 224172, 247403, 247405, 247407, 249990, 249994, 305732, 305733, 305734, 305830, 305831, 305832, 317351,
            317352, 317353, 317354, 317355, 317356, 319337, 319583, 319776, 320135, 320136, 323124, 357875, 357880, 357881, 358124,
            358125, 358126, 358128, 358129, 362956, 362957, 363236, 363237, 366341, 367774, 374071, 374080, 374084, 376290, 391711,
            391762, 392434, 392477, 392611, 392620, 395888, 395892, 396441, 396442, 396443, 396444, 396445, 396650, 409250, 409362,
            409385, 409486, 409487, 409488, 409528, 409533, 409641, 409688, 409705, 409708, 409710, 409745, 409748, 409852, 409853,
            409858, 409861, 409930, 410107, 410215, 426074, 426080, 426081, 426083, 426091, 426092, 426095, 426103, 426106, 426107,
            426110, 426121, 426123,
        };

        public static HashSet<int> PlayerSNO = new HashSet<int>
        {
            //Wizard_Female
            6526,
            //Wizard_Female_characterSelect
            6527,
            //Wizard_Male
            6544,
            //Wizard_Male_characterSelect
            6545,
            //Wizard_Male_FrontEnd
            218883,
            //Wizard_Female_FrontEnd
            218917,
            //Barbarian_Female
            3285,
            //Barbarian_Female_characterSelect
            3287,
            //Barbarian_Male
            3301,
            //Barbarian_Male_characterSelect
            3302,
            //Barbarian_Male_FrontEnd
            218882,
            //Barbarian_Female_FrontEnd
            218909,
            //Demonhunter_Female_FrontEnd
            218911,
            //Demonhunter_Male_FrontEnd
            218912,
            //Demonhunter_Female
            74706,
            //Demonhunter_Male
            75207,
            //X1_Crusader_Male
            238284,
            //X1_Crusader_Female
            238286,
            //X1_Crusader_Male_FrontEnd
            238287,
            //X1_Crusader_Female_FrontEnd
            238288,
            //Crusader_Female_characterSelect
            279361,
            //Crusader_Male_characterSelect
            279362,
            //WitchDoctor_Male
            6485,
            //WitchDoctor_Male_characterSelect
            6486,
            //WitchDoctor_Female
            6481,
            //WitchDoctor_Female_characterSelect
            6482,
            //Monk_Male
            4721,
            //Monk_Male_characterSelect
            4722,
            //Monk_Female
            4717,
            //Monk_Female_characterSelect
            4718,
        };

        public static HashSet<int> ProjectileBlockingGizmos = new HashSet<int>
        {
            3048, // a2dun_Zolt_Sand_Wall // 103: TeamId (-3993) i:1 f:0 Value=1 IsValid=True
        };

        public static HashSet<SNOPower> ResetNavigationPowers = new HashSet<SNOPower>
        {
            SNOPower.X1_Monk_DashingStrike,
            //SNOPower.Barbarian_FuriousCharge,
            SNOPower.Wizard_Teleport,
            SNOPower.Wizard_Archon_Teleport,
        };

        public static HashSet<SNOActor> ShrineSNO = new HashSet<SNOActor>
        {
            SNOActor.shrine_fxSphere_corrupt, //5333,
            SNOActor.Shrine_Global, //135384,
            SNOActor.shrine_fxGeo_model_Global, //139931,
            SNOActor.Shrine_Global_Glow, //156680,
            SNOActor.Shrine_Global_Blessed, //176074,
            SNOActor.Shrine_Global_Enlightened, //176075,
            SNOActor.Shrine_Global_Fortune, //176076,
            SNOActor.Shrine_Global_Frenzied, //176077,
            SNOActor.a4_Heaven_Shrine_Global_Blessed, //225025,
            SNOActor.a4_Heaven_Shrine_Global_Fortune, //225027,
            SNOActor.a4_Heaven_Shrine_Global_Frenzied, //225028,
            SNOActor.a4_Heaven_Shrine_Global_Enlightened, //225030,
            SNOActor.a4_Heaven_Shrine_Global_DemonCorrupted_Blessed, //225261,
            SNOActor.a4_Heaven_Shrine_Global_DemonCorrupted_Enlightened, //225262,
            SNOActor.a4_Heaven_Shrine_Global_DemonCorrupted_Fortune, //225263,
            SNOActor.a4_Heaven_Shrine_Global_DemonCorrupted_Frenzied, //225266,
            SNOActor.a4_Heaven_Shrine_Global_DemonCorrupted_Hoarder, //260342,
            SNOActor.a4_Heaven_Shrine_Global_DemonCorrupted_Reloaded, //260343,
            SNOActor.a4_Heaven_Shrine_Global_Hoarder, //260344,
            SNOActor.a4_Heaven_Shrine_Global_Reloaded, //260345,
            SNOActor.Shrine_Global_Hoarder, //260346,
            SNOActor.Shrine_Global_Reloaded, //260347,
            SNOActor.Shrine_TreasureGoblin, //269349,
            SNOActor.PVP_Shrine_Murderball, //275729,
            SNOActor.x1_LR_Shrine_Damage, //330695,
            SNOActor.x1_LR_Shrine_Electrified, //330696,
            SNOActor.x1_LR_Shrine_Infinite_Casting, //330697,
            SNOActor.x1_LR_Shrine_Invulnerable, //330698,
            SNOActor.x1_LR_Shrine_Run_Speed, //330699,
            //SNOActor.x1_Event_CursedShrine, //364601,
            //SNOActor.x1_Event_CursedShrine_Heaven, //368169,
            SNOActor.x1_player_isShielded_riftShrine_model, //369696,
            SNOActor.x1_LR_Shrine_Electrified_TieredRift, //398654,
            SNOActor.a4_Heaven_Shrine_TreasureGoblin, //434409,
            SNOActor.shrine_Shadow, //434722,
        };

        public static HashSet<SNOActor> SlowTimeSNO = new HashSet<SNOActor>()
        {
            SNOActor.slowTime_model_01,
            SNOActor.slowTime_model_02,
        };

        public static HashSet<SNOActor> SpiderPetIds = new HashSet<SNOActor>
        {
            SNOActor.witchDoctor_CorpseSpider_crimsonRune,
            SNOActor.witchDoctor_CorpseSpider_indigoRune,
            SNOActor.witchDoctor_CorpseSpider,
            SNOActor.witchDoctor_CorpseSpider_obsidianRune,
            SNOActor.witchDoctor_CorpseSpider_goldenRune,
            SNOActor.witchDoctor_CorpseSpider_alabasterRune,
        };

        public static HashSet<int> SummonerSNO = new HashSet<int>
        {
            //TriuneSummoner_fireBall_obj
            467,
            //SkeletonSummoner_A
            5387,
            //SkeletonSummoner_B
            5388,
            //SkeletonSummoner_C
            5389,
            //SkeletonSummoner_D
            5390,
            //TriuneSummoner_A
            6035,
            //TriuneSummoner_B
            6036,
            //TriuneSummoner_C
            6038,
            //TriuneSummoner_D
            6039,
            //SkeletonSummoner_A_TemplarIntro
            104728,
            //TownAttack_TriuneSummonerBoss_C
            105539,
            //triuneSummoner_summonRope_glow
            111554,
            //TriuneSummoner_B_RabbitHoleEvent
            111580,
            //Spawner_SkeletonSummoner_A_Immediate_Chand
            117947,
            //TriuneSummoner_A_Unique_SwordOfJustice
            131131,
            //a4_heaven_hellportal_summoner_loc
            143502,
            //TownAttack_SummonerSpawner
            173527,
            //TownAttack_Summoner
            178297,
            //TownAttack_Summoner_Unique
            178619,
            //SkeletonSummoner_E
            182279,
            //TriuneSummoner_A_CainEvent
            186039,
            //TriuneSummoner_A_Unique_01
            218662,
            //TriuneSummoner_A_Unique_02
            218664,
            //TriuneSummoner_C_Unique_01
            222001,
            //x1_TriuneSummoner_WestMCultist
            288215,
            //x1_Spawner_TriuneSummoner_A
            290730,
            //x1_Spawner_SkeletonSummoner_A
            292297,
            //x1_westm_Soul_Summoner
            298827,
            //x1_westm_Soul_Summoner_Hands
            301425,
            //x1_Spawner_SkeletonSummoner_D
            303859,
            //x1_TEST_FallenChampion_LR_Summoner
            308285,
            //x1_westm_Soul_Summoner_Spawner
            308823,
            //x1_soul_summoner_hands_trail
            316395,
            //x1_westm_Soul_Summoner_twoHands
            316560,
            //x1_Soul_Summoner_glowSphere
            316716,
            //x1_westm_Soul_Summoner_GhostChase
            330609,
            //x1_devilshand_unique_SkeletonSummoner_B
            332432,
            //x1_devilshand_unique_TriuneSummoner_C
            332433,
            //x1_TriuneSummoner_C_Unique_01
            341240,
            //X1_LR_Boss_SkeletonSummoner_C
            359094,
            //x1_Heaven_Soul_Summoner
            361480,
            //TriuneSummoner_Unique_Cultist_Leader_Hershberg
            396812,
            //TriuneSummoner_Unique_Cultist_Leader_Son_of_Jacob
            396836,
            //TriuneSummoner_Unique_Cultist_Leader_Poirier
            396849,
            //TriuneSummoner_Unique_Cultist_Leader_Buckley
            396863,
        };

        public static HashSet<SNOLevelArea> TownLevelAreaIds = new HashSet<SNOLevelArea>()
        {
            SNOLevelArea.A1_Tristram_Adventure_Mode_Hub,
            SNOLevelArea.A2_caOut_CT_RefugeeCamp_Hub,
            SNOLevelArea.A3_Dun_Keep_Hub,
            SNOLevelArea.x1_Westm_Hub
        };

        public static GameBalanceTable TransmogTable = new GameBalanceTable
        {
            //{ 1902, 1549850923, -929962837, "X1_PromoTransmogPlan_01" },
            //{ 1903, 1549850924, -929962836, "X1_PromoTransmogPlan_02" },
            //{ 2057, 1549850925, -929962835, "X1_PromoTransmogPlan_03" },
            //{ 2058, -1723454145, 1869712607, "p1_SeasonTransmogPlan_01" },
            { 2324, -1915655615, 2129793537, "TransmogHelm_001" },
            { 2325, -692981644, -919506892, "TransmogShoulders_001" },
            { 2374, 1159896909, 42569453, "TransmogHelm_Illidan" },
            { 2430, -1915655614, 2129793538, "TransmogHelm_002" },
            { 2431, 1268253959, -1471771449, "TransmogMace_241_001" },
            { 2433, -1901213639, -2127738887, "TransmogFlail_241_001" },
            { 2434, -1901213638, -2127738886, "TransmogFlail_241_002" },
            { 2435, 868864647, -1871160761, "TransmogFist_241_001" },
            { 2436, 1460786533, 1234261285, "TransmogStaff_241_001" },
            { 2437, -1901213637, -2127738885, "TransmogFlail_241_003" },
            { 2438, -1901213636, -2127738884, "TransmogFlail_241_004" },
            { 2439, 868864648, -1871160760, "TransmogFist_241_002" },
            { 2440, 1286110481, -1453914927, "TransmogXbow_241_001" },
            { 2441, 1345846545, -1990947631, "TransmogAxe_241_003" },
            { 2445, 2089009826, 1862484578, "TransmogSword_241_003" },
            { 331905, 2089009825, 0, "transmogSword_241_002" }, //Amberwing
        };

        public static HashSet<SNOAnim> VaultAnimations = new HashSet<SNOAnim>
        {
            SNOAnim.Demonhunter_Male_Cast_BackFlip_mid,
            SNOAnim.Demonhunter_Female_Cast_BackFlip_mid,
            SNOAnim.Demonhunter_Male_Cast_BackFlip_out,
            SNOAnim.Demonhunter_Female_Cast_BackFlip_out,
            SNOAnim.Demonhunter_Male_Cast_BackFlip_in,
            SNOAnim.Demonhunter_Female_Cast_BackFlip_in,
        };

        public static HashSet<int> WhirlWindIgnoreSNO = new HashSet<int>
        {
            4304,
            5984,
            5985,
            5987,
            5988,
        };

        public static GameBalanceTable WingsTable = new GameBalanceTable
        {
            { 217, 237118774, 600882390, "AngelWings_Blue" },
            { 1025, -1736870778, 234604614, "BugWings" },
            { 1733, -1424453175, -1283279511, "AngelWings_Red" },
            { 2051, -1364948604, -1558843100, "x1_AngelWings_Imperius" },
            { 2366, -816230663, 686014105, "Wings_Mercy_Console" },
            { 2367, 2036418836, -34240396, "Cosmetic_Wings_SetDngn_Mastery" },
            { 2368, 671128753, 1237156881, "Cosmetic_Wings_SetDngn_Basic" },
            { 2388, 224691469, 327053421, "Console_Wings_SetDngn_Basic" },
            { 2389, 1197521008, 1000537040, "Console_Wings_SetDngn_Mastery" },
            { 2432, 1905890384, 808390672, "Cosmetic_Wings_bfly_07" },
            { 2442, 1081297790, -1546363074, "Cosmetic_Wings_bird" },
            { 2443, -562468108, 436504212, "P5_Cosmetic_Wings_bfly_07" },
            { 2444, -166853470, 65389634, "P5_Cosmetic_Wings_bird" },
            { 2446, 1905890380, 808390668, "Cosmetic_Wings_bfly_03" },
            { 2447, 702834906, -371626854, "Cosmetic_Wings_bat_07" },
            { 2450, -1324221698, -1304179010, "P5_Cosmetic_wings_bat_07" },
            { 2451, 702834903, -371626857, "Cosmetic_Wings_bat_04" },
            { 2452, -1324221701, -1814551909, "P5_Cosmetic_Wings_bat_04" },
            { 2453, 1905890378, -1863878678, "Cosmetic_Wings_Bfly_01" },
            { 2454, -562468114, 436504206, "P5_Cosmetic_Wings_bfly_01" },
            { 2455, -562468112, 436504208, "P5_Cosmetic_Wings_bfly_03" },
        };

        public HashSet<SNOPower> NonCombatPowers = new HashSet<SNOPower>
        {
            SNOPower.None,
            SNOPower.Walk
        };

        internal static HashSet<SNOAnim> ActorChargeAnimations = new HashSet<SNOAnim>
        {
            SNOAnim.Beast_start_charge_02,
            SNOAnim.Beast_charge_02,
            SNOAnim.Beast_charge_04,
            SNOAnim.Butcher_Attack_Charge_01_in,
            SNOAnim.Butcher_Attack_Chain_01_out,
            SNOAnim.Butcher_Attack_05_telegraph,
            SNOAnim.Butcher_Attack_Chain_01_in,
            SNOAnim.Butcher_BreakFree_run_01,
        };

        internal static HashSet<string> ActorIgnoreNameParts = new HashSet<string>
        {
            "markerlocation",
            "start_location",
            "checkpoint",
            "savepoint",
            "triggersphere",
            "minimapicon",
            "proxy",
            "invisbox",
            "trigger",
            "invisible"
        };

        internal static HashSet<string> ActorIgnoreNames = new HashSet<string>
        {
            "markerlocation",
            "generic_proxy",
            "hireling",
            "start_location",
            "spheretrigger",
            "checkpoint",
            "conductorproxymaster",
            "boxtrigger",
            "savepoint",
            "triggersphere",
            "minimapicon",
        };


        private static Dictionary<int, string> _animatioNamesById;

        public static HashSet<int> AlwaysRaycastWorlds { get; } = new HashSet<int>()
        {
            271233, // Pandemonium Fortress 1
            271235, // Pandemonium Fortress 2
        };

        public static HashSet<SNOActor> AncientIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.Barbarian_CallOfTheAncients_1,
            SNOActor.Barbarian_CallOfTheAncients_2,
            SNOActor.Barbarian_CallOfTheAncients_3
        };

        public static Dictionary<int, string> AnimatioNamesById => _animatioNamesById ?? (_animatioNamesById = SNOAnim.Invalid.ToDictionary(true));


        /// <summary>
        /// A list of SNO's that are projectiles (so constantly look for new locations while avoiding)
        /// </summary>
        public static HashSet<int> AvoidanceProjectiles { get; } = new HashSet<int>
        {
            5212,   // Bees-Wasps
            4103,   // Sha-Ball
            160154, // Molten Ball
            123842, // Azmo fireball
            139741, // Zolt Twister
            166686, // Maghda Projectile
            185999, // Diablo Expanding Fire
            196526, // Diablo Expanding Fire
            136533, // Diablo Lightning Breath
            343539, // Orbiter X1_MonsterAffix_Orbiter_Projectile
            341512, // Thunderstorm
            316389, // PoisonEnchanted x1_MonsterAffix_CorpseBomber_projectile (316389)
            340319, // PoisonEnchanted x1_MonsterAffix_CorpseBomber_bomb_start (340319)

            // A5

            338889, // x1_Adria_bouncingProjectile

            362850, // x1_Urzael_Cannonball_burning_invisible
            346976, // x1_Urzael_Cannonball_burning_impact
            346975, // x1_Urzael_Cannonball_burning

            335505, // x1_malthael_drainSoul_ghost
            325136, // x1_Malthael_DeathFogMonster
            340512, // x1_Malthael_Mephisto_LightningObject

            377086, // X1_Unique_Monster_Generic_Projectile_Arcane
            377087, // X1_Unique_Monster_Generic_Projectile_Cold
            377088, // X1_Unique_Monster_Generic_Projectile_Fire
            377089, // X1_Unique_Monster_Generic_Projectile_Holy
            377090, // X1_Unique_Monster_Generic_Projectile_Lightning
            377091, // X1_Unique_Monster_Generic_Projectile_Physical
            377092, // X1_Unique_Monster_Generic_Projectile_Poison

            3528,   // Butcher_hook

            // 4394, //g_ChargedBolt_Projectile-200915 (4394) Type=Projectile
            // 368392, // x1_Cesspool_Slime_Posion_Attack_Projectile-222254 (368392) Type=Projectile
            (int)SNOActor.x1_LR_boss_terrorDemon_A_projectile,
        };

        /// <summary>
        /// A list of all the SNO's to avoid - you could add
        /// </summary>
        public static HashSet<int> Avoidances { get; } = new HashSet<int>
        {
            219702, // Arcane
            221225, // Arcane 2
            5482,   // Poison Tree
            6578,   // Poison Tree
            4803,   // monsterAffix_Molten_deathStart_Proxy
            4804,   // monsterAffix_Molten_deathExplosion_Proxy
            4806,   // monsterAffix_Electrified_deathExplosion_proxy
            224225, // Molten Core 2
            247987, // Molten Core 2
            95868,  // Molten Trail
            108869, // Plague Cloud
            402,    // Ice Balls
            223675, // Ice Balls
            5212,   // Bees-Wasps
            3865,   // Plague-Hands
            123124, // Azmo Pools
            123842, // Azmo fireball
            123839, // Azmo bodies
            161822, // Belial 1
            161833, // Belial 2
            4103,   // Sha-Ball
            160154, // Mol Ball
            432,    // Mage Fire
            168031, // Diablo Prison
            214845, // Diablo Meteor
            260377, // Ice-trail
            //185924, // Zolt Bubble
            139741, // Zolt Twister
            93837,  // Ghom Gas
            166686, // Maghda Proj
            226350, // Diablo Ring of Fire
            226525, // Diablo Ring of Fire
            250031, // Mortar MonsterAffix_Mortar_Pending

            84608,  // Desecrator monsterAffix_Desecrator_damage_AOE
            84606,  // Desecrator monsterAffix_Desecrator_telegraph

            /* 2.0 */
            349774, // FrozenPulse x1_MonsterAffix_frozenPulse_monster
            343539, // Orbiter X1_MonsterAffix_Orbiter_Projectile
            316389, // PoisonEnchanted x1_MonsterAffix_CorpseBomber_projectile (316389)
            340319, // PoisonEnchanted x1_MonsterAffix_CorpseBomber_bomb_start (340319)
            341512, // Thunderstorm x1_MonsterAffix_Thunderstorm_Impact
            337109, // Wormhole X1_MonsterAffix_TeleportMines

            338889, // x1_Adria_bouncingProjectile
            360738, // X1_Adria_arcanePool
            358404, // X1_Adria_blood_large

            360598, // x1_Urzael_CeilingDebris_DamagingFire_wall
            359205, // x1_Urzael_ceilingDebris_Impact_Beam
            360883, // x1_Urzael_ceilingDebris_Impact_Circle

            362850, // x1_Urzael_Cannonball_burning_invisible
            346976, // x1_Urzael_Cannonball_burning_impact
            346975, // x1_Urzael_Cannonball_burning

            335505, // x1_malthael_drainSoul_ghost
            325136, // x1_Malthael_DeathFogMonster
            340512, // x1_Malthael_Mephisto_LightningObject

            359703, // X1_Unique_Monster_Generic_AOE_DOT_Cold_10foot
            363356, // X1_Unique_Monster_Generic_AOE_DOT_Cold_20foot
            359693, // X1_Unique_Monster_Generic_AOE_DOT_Fire_10foot
            363357, // X1_Unique_Monster_Generic_AOE_DOT_Fire_20foot
            364542, // X1_Unique_Monster_Generic_AOE_DOT_Lightning_10foot
            364543, // X1_Unique_Monster_Generic_AOE_DOT_Lightning_20foot
            377537, // X1_Unique_Monster_Generic_AOE_DOT_Lightning_hardpoints
            360046, // X1_Unique_Monster_Generic_AOE_DOT_Poison_10foot
            363358, // X1_Unique_Monster_Generic_AOE_DOT_Poison_20foot
            368156, // X1_Unique_Monster_Generic_AOE_Lightning_Ring
            358917, // X1_Unique_Monster_Generic_AOE_Sphere_Distortion
            377086, // X1_Unique_Monster_Generic_Projectile_Arcane
            377087, // X1_Unique_Monster_Generic_Projectile_Cold
            377088, // X1_Unique_Monster_Generic_Projectile_Fire
            377089, // X1_Unique_Monster_Generic_Projectile_Holy
            377090, // X1_Unique_Monster_Generic_Projectile_Lightning
            377091, // X1_Unique_Monster_Generic_Projectile_Physical
            377092, // X1_Unique_Monster_Generic_Projectile_Poison
            (int)SNOActor.x1_LR_boss_terrorDemon_A_projectile,
        };

        /// <summary>
        /// The duration the AoE from AvoidanceSpawners should be avoided for
        /// </summary>
        public static Dictionary<int, TimeSpan> AvoidanceSpawnerDuration { get; } = new Dictionary<int, TimeSpan>
        {
            {221225, TimeSpan.FromSeconds(10)},   // Arcane
            {219702, TimeSpan.FromSeconds(10)},   // Arcane
            {5482, TimeSpan.FromSeconds(15)},   // Poison Tree
            {6578, TimeSpan.FromSeconds(15)},   // Poison Tree
            {316389, TimeSpan.FromSeconds(6)}, // PoisonEnchanted
            {340319, TimeSpan.FromSeconds(6)}, // PoisonEnchanted
            {4803, TimeSpan.FromSeconds(10)}, // Molten Core
            {4804, TimeSpan.FromSeconds(10)}, // Molten Core
            {224225, TimeSpan.FromSeconds(10)}, // Molten Core
            {247987, TimeSpan.FromSeconds(10)}, // Molten Core
            {159369, TimeSpan.FromSeconds(3)}, // Morlu Meteor
        };

        /// <summary>
        /// A list of SNO's that spawn AoE then disappear from the object manager
        /// </summary>
        public static HashSet<int> AvoidanceSpawners { get; } = new HashSet<int>
        {
            221225,
            219702,
            5482,   // Poison Tree
            6578,   // Poison Tree
            316389, // PoisonEnchanted x1_MonsterAffix_CorpseBomber_projectile (316389)
            340319, // PoisonEnchanted x1_MonsterAffix_CorpseBomber_bomb_start (340319)
            159369, //MorluSpellcaster_Meteor_Pending-178011 (159369)
        };

        /// <summary>
        /// This list is used for animations where the avoidance point should be the player's current location
        /// </summary>
        public static HashSet<int> AvoidAnimationAtPlayer { get; } = new HashSet<int>
        {
            (int)SNOAnim.Beast_start_charge_02, // A1 Savage Beast Charge - needs special handling!
            (int)SNOAnim.Beast_charge_02, // A1 Savage Beast Charge - needs special handling!
            (int)SNOAnim.Beast_charge_04, // A1 Savage Beast Charge - needs special handling!
            (int)SNOAnim.morluSpellcaster_attack_AOE_01, //morluSpellcaster_D
            (int)SNOAnim.X1_LR_Boss_morluSpellcaster_generic_cast, //morluSpellcaster_D
            (int)SNOAnim.snakeMan_melee_generic_cast_01, //X1_LR_Boss_Snakeman_Melee_Belial (360281)
        };

        /// <summary>
        /// Contains ActorSnoId's of world objects that should be blacklisted
        /// </summary>
        public static HashSet<SNOActor> BlackListIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.x1_BogCave_Stalagmite_Group_C,                    // Destructible object that can't be reached.
            SNOActor.x1_BogCave_Stalagmite_Group_D,                    // Destructible object that can't be reached.
            SNOActor.x1_BogCave_Stalagmite_Fungus_E,                   // Destructible object that can't be reached.

            SNOActor.Gharbad_The_Weak_Ghost,                           // Event NPC
            SNOActor.x1_NPC_Westmarch_Gorrel_NonUnique,                // NPC for bounty: Noble Deaths.
            SNOActor.bastionsKeepGuard_Melee_A_01_snatched,            // NPC you can't attack.
            SNOActor.bastionsKeepGuard_Melee_A_02_BlazeOfGlory,        // NPC you can't attack.
            SNOActor.Bucket_Crane_caOut_Mining_Breakable,              // Obstacle in Dahlgur Oasis.
            SNOActor.trDun_Cath_Bannister_x6,                          // Unbreakable obstacle.
            SNOActor.A2C2DyingGhostGuy,                                // NPC for bounty: The Guardian Spirits.
            SNOActor.Angel_Trooper_A,                                  // NPC for bounty: Lost Host
            SNOActor.x1_SurvivorCaptain_Rescue_Guards,                 // NPC for bounty: The Last Stand
            SNOActor.bastionsKeepGuard_Melee_A_02_NPC_RescueEscort,
            SNOActor.P4_Forest_DyingGuy,                               // Guy in Greyhollow Island the bot tries to attack.

            // Obstacle you can't destroy.
            SNOActor.LS_a3dun_Keep_Exploding_Arch_A,
            SNOActor.P6_Church_IronMaiden,

            // these bounty chests are not clickable they have a protection globe on them during event then sno changes.
            SNOActor.x1_Global_Chest_Locked,

            // prevent trin from interacting with this NPC as part of mysterious cave bounty due to
            // extremely poor navigation in the little nook where he lives. profile to manually move within scene.
            SNOActor.A2_UniqueVendor_Event_MapVendor,

            // prevent trin from interacting before clear area, bounty must force interact
            SNOActor.px_Bounty_Ramparts_Camp_Switch, // a4 catapault command bounty,

            SNOActor.p43_AD_Valor_Pedestal_Locked,

            SNOActor.x1_PandExt_Ballista_Angelic_A,

            SNOActor.a2dun_Cald_Belial_Acid_Attack,
            SNOActor.x1_Malthael_DeathFogMonster,

            SNOActor.p4_Leoric_Chandelier,
            SNOActor.a2dun_Swr_Act_Iron_Railing_A_01,
            SNOActor.x1_BogCave_Stalagmite_Fungus_A, //255245,

            SNOActor.PT_Blacksmith,
            SNOActor.trOut_OldTristram_AdriaCellar_Cauldron, //-6946 (131123)

            SNOActor.trDun_Cath_Chandelier_Trap, //Name = trDun_Cath_Chandelier_Trap - 25291.SNO = 5744,

            SNOActor.a2dun_Spider_Ground_Spawner, //

            SNOActor.a2dun_Zolt_SandBridgeBase, // bridge that is of type door, can't be interacted with.
            SNOActor.a3dun_crater_st_Demon_ChainPylon_Fire_Azmodan, // door with irregular properties

            SNOActor.caOut_Oasis_Sub240_Water_Money_Center_Wheel,
            SNOActor.a1dun_leor_firewall1,
            SNOActor.a1dun_leor_firewall2, //(89579) fire floor tiles

            SNOActor.a2dun_Zolt_Random_Breakable_Statue,

            SNOActor.x1_Catacombs_Breakable_Corner_Wall,
            SNOActor.x1_Catacombs_Breakable_Wall_A,

            SNOActor.Cow_B, //95263

            SNOActor.a1dun_Leor_Jail_Door_SuperLocked_A_Fake,
            SNOActor.a3dun_Keep_BucketMetal_A_Breakable,
            SNOActor.x1_Catacombs_Breakable_Half_Bridge_Walkway_A,
            SNOActor.x1_PandExt_Breakable_Angel_B, // x1_PandExt_Breakable_Angel_B-159982 (338281)

            SNOActor.a4dunSpire_Interactives_Izual_Ice_Barrier_A,

            SNOActor.trDun_Cath_Braizer_Trap, // hanging chandelier triggered by trap switch

            SNOActor.x1_BogCave_Breakable_Stalagmites_A,
            SNOActor.x1_BogCave_Breakable_Stalagmites_B,
            SNOActor.x1_BogCave_Breakable_Stalagmites_C,
            SNOActor.x1_BogCave_Breakable_Stalagmites_E,
            SNOActor.x1_BogCave_Breakable_Stalagmites_F,
            SNOActor.x1_BogCave_Breakable_Stalagmites_G,
            SNOActor.px_Cave_A_Breakable_Stalagmites_A,
            SNOActor.px_Cave_A_Breakable_Stalagmites_B,
            SNOActor.px_Cave_A_Breakable_Stalagmites_C,
            SNOActor. px_Cave_A_Breakable_Stalagmites_E,
            SNOActor.px_Cave_A_Breakable_Stalagmites_F,
            SNOActor.px_Cave_A_Breakable_Stalagmites_G,

            SNOActor.x1_Adria_CauldronSpawner_Temp,
            SNOActor.x1_Fortress_Ring_Hazard,
            SNOActor.P4_Ruins_Frost_Trap_Swinging_Blade,

            SNOActor.a2dun_Aqd_Act_Wood_Platform_A_01, //Name=a2dun_Aqd_Act_Wood_Platform_A_01-30200. SNO=62125,
            SNOActor.a2dun_Swr_Breakable_Wall_A, //a2dun_Swr_Breakable_Wall_A-305214 (2965)

            SNOActor.a2dun_Zolt_SandBridgeBase_BossFight, //ActorId: 165415, Type: Gizmo, Name: a2dun_Zolt_SandBridgeBase_BossFight-921, Distance2d: 41.67792, CollisionRadius: 18.31754, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0
            //156328, //ActorId: 156328, Type: Gizmo, Name: a2dun_Zolt_black_soulstone-515, Distance2d: 41.42352, CollisionRadius: 20.39608, MinimapActive: 0, MinimapIconOverride: -1, MinimapDisableArrow: 0

            SNOActor.trDun_book_pile_b,
            SNOActor.x1_WestmHub_GuardNoHelmUnarmed,
            // World Objects
            SNOActor.trDun_Crypt_DeathoftheKing_Sword_Clickable,
            SNOActor.a1dun_Leor_Candlestand_1,
            SNOActor.a1dun_Leor_Barrel_Torture_A_01,
            SNOActor.a1dun_leor_Torture_Table_A_01,
            SNOActor.a1dun_leor_Basket_of_Heads,
            SNOActor.a1dun_Leor_Fire_Gutter_Light,
            SNOActor.a1dun_leor_Torture_Rolling_Wheel,
            SNOActor.a1dun_Leor_Iron_Maiden,
            SNOActor.a1dun_Leor_Barrel_Water,
            SNOActor.a1dun_Leor_Jail_SittingGuard_A,
            SNOActor.MinimapIconStairs_Switch,
            SNOActor.demonFlyer_A_swoop,
            SNOActor.a3dun_keep_TableGroup_D_Breakable,
            SNOActor.Beast_Corpse_A_01,
            SNOActor.a3dun_rmpt_OilVat_A,
            SNOActor.trOut_TristramField_Cart_Broken_Burnt,
            SNOActor.WoodFenceE_Fields_trOut,
            SNOActor.WoodFenceC_Fields_trOut,
            SNOActor.Hen_House_trOut_Farms,
            SNOActor.trOut_Wilderness_Planter_A,
            SNOActor.trOut_Wilderness_Planter_B,
            SNOActor.trOut_Wagon_Barricade,
            SNOActor.trOut_Wilderness_Gargoyle_A,
            SNOActor.trOut_Wilderness_Planter_A,
            SNOActor.trOut_Pumpkin_D1,
            SNOActor.trOut_Farms_Butcher_Smoke_House,
            SNOActor.trOut_FesteringWoods_Neph_Column_B,
            SNOActor.Bucket_A_Fields_trOut,
            SNOActor.trOut_FesteringWoods_Neph_Column_C_Broken_Base,
            SNOActor.Tombstone_A_Wilderness_trOut_Wilderness,
            SNOActor.trOut_OldTristramTombstoneDestructibleA,
            SNOActor.trOut_OldTristramTombstoneDestructibleB,
            SNOActor.trOut_OldTristramTombstoneDestructibleD,
            SNOActor.trOut_OldTristramTombstoneDestructibleE,
            SNOActor.Tombstone_B_Wilderness_trOut_Wilderness,
            SNOActor.a3dun_rmpt_Exploding_Grate_A_BridgeEvent,
            SNOActor.caOut_Breakable_Wagon_a,
            SNOActor.a2dun_Zolt_Random_Breakable_Arch_A,
            SNOActor.a2dun_Zolt_Random_Breakable_Corner,
            SNOActor.a2dun_Zolt_Random_Breakable_HalfWall_A,
            SNOActor.Fence_A_caOut_Mining_Interactive,
            SNOActor.Fence_C_caOut_Mining_Interactive,
            SNOActor.a3Battlefield_Props_burnt_supply_wagon_B_Breakable,
            SNOActor.a3dun_rmpt_Exploding_Barrel_Snow,
            SNOActor.a4dun_Diablo_Bone_Prison,
            SNOActor.a3dun_Keep_Exploding_Arch_A,
            SNOActor.a3dun_Crater_DemonClawBomb_A_Monster,
            (SNOActor)15119,
            SNOActor.a3dun_keep_TableGroup_A_Breakable,
            SNOActor.a3dun_Keep_Bed_A_Breakable,
            SNOActor.a2dun_Zolt_FloatingLights,
            (SNOActor)5776,
            SNOActor.destructibleCandles3,
            SNOActor.a3dun_Keep_Crate_D_Snow,
            SNOActor.a3dun_Keep_Exploding_Barrel,
            SNOActor.CaOut_Target_Dummy,
            SNOActor.a3dun_rmpt_catapult_follower_event_gate,
            SNOActor.a2dun_Zolt_Breakable_Pillar_A,
            SNOActor.trDun_Cave_SwordOfJustice_Blade,
            SNOActor.caOut_Cage,
            //a3dun_crater_st_Demo_ChainPylon_Fire_Azmodan, a3dun_crater_st_Demon_ChainPylon_Fire_MistressOfPain
            SNOActor.a3dun_crater_st_Demon_ChainPylon_Fire_MistressOfPain,
            SNOActor.trOut_Leor_painting,
            SNOActor.trDun_Magic_Painting_H_NoSpawn,
            (SNOActor)263014,
            (SNOActor)249192, (SNOActor)251416, SNOActor.Skeleton_A_Unique_Ring_015, SNOActor.Skeleton_Archer_A_Unique_Ring_015, // summoned skeleton from ring
            // Units below here
            SNOActor.Voiceover_FreedPrisoner,
            SNOActor.Raven_Pecking,
            SNOActor.Raven_Perched,
            SNOActor.caOut_Raven_Pecking_A,
            SNOActor.caOut_Raven_Perched_A,
            SNOActor.GhostKnight2,
            SNOActor.GhostKnight3,
            SNOActor.Scoundrel,
            SNOActor.Enchantress,
            SNOActor.Templar,
            SNOActor.Hireling_Templar,
            SNOActor.caldeumPoor_Legless_Male_02,
            SNOActor.Kadin,
            SNOActor.Aleser,
            SNOActor.Caliem,
            SNOActor.Bezir,
            SNOActor.SuperCaldeumGuard_Spear_B,
            SNOActor.DavydImpostor,
            SNOActor.Davyd,
            SNOActor.Caliem,
            SNOActor.Aleser,
            SNOActor.Kadin,
            SNOActor.caldeumGuard_Cleaver_A,
            SNOActor.Maghda_A_TempProjection,
            SNOActor.caldeumGuard_Spear_Imperial,
            SNOActor.caldeumGuard_Captain_B_Khamsin,
            SNOActor.caldeumGuard_Spear_Imperial,
            SNOActor.Khamsin_Mine_Unique,
            SNOActor.Tyrael_Act3,
            SNOActor.SacrificeLadyA,
            SNOActor.A3_Hub_SacrificeLadyNew,
            SNOActor.Mystic_B,
            SNOActor.Fetish_Melee_A,
            SNOActor.Fetish_doubleStack_Shaman_A,
            SNOActor.witchDoctor_CorpseSpider,
            SNOActor.Witchdoctor_spiritWalk_Dummy,
            SNOActor.DemonVoiceover,
            SNOActor.ZoltunKulleFollower,
            SNOActor.a1dun_leor_BigFireGrate,
            SNOActor.Voiceover_JailWarden,
            SNOActor.TownAttackVillager,
            SNOActor.OmniNPC_Tristram_Male_B,
            SNOActor.OmniNPC_Tristram_Female_B,
            SNOActor.Crater_St_SuccubusPrison_Male_A,
            SNOActor.Twister_model,
            SNOActor.Witchdoctor_spiritWalk_Dummy_female,
            SNOActor.KulleVoiceover,
            SNOActor.Fetish_Skeleton_A,
            SNOActor.Fetish_Ranged_A,
            SNOActor.Witchdoctor_BigBadVoodoo_fetish_blue,
            SNOActor.Witchdoctor_BigBadVoodoo_fetish,
            SNOActor.Witchdoctor_BigBadVoodoo_fetish_red,
            SNOActor.Witchdoctor_BigBadVoodoo_fetish_yellow,
            SNOActor.Witchdoctor_BigBadVoodoo_fetish_purple,
            SNOActor.A1_GenericVendor_Tinker_Voiceover,
            SNOActor.NPC_Human_Male_Event_FarmAmbush,
            SNOActor.Wizard_HydraHead_Acid_1,
            (SNOActor)81226,
            (SNOActor)81227,
            SNOActor.witchDoctor_CorpseSpider_obsidianRune,
            SNOActor.witchDoctor_CorpseSpider_indigoRune,
            SNOActor.witchDoctor_CorpseSpider_goldenRune,
            SNOActor.witchDoctor_CorpseSpider_alabasterRune,
            SNOActor.witchDoctor_CorpseSpider_crimsonRune,
            SNOActor.Fetish_Hex_D,
            SNOActor.Fetish_Hex_E,
            SNOActor.Fetish_Hex_A,
            SNOActor.Fetish_Hex_C,
            SNOActor.demonFlyer_A_swoop,
            (SNOActor)249320,
            SNOActor.Wizard_HydraHead_Arcane_3,
            SNOActor.Wizard_HydraHead_Arcane_2,
            SNOActor.Wizard_Hydra_RuneArcane_Pool,
            SNOActor.Wizard_HydraHead_Arcane_1,
            SNOActor.Catapult_a3dunKeep_WarMachines_Snow_Firing,
            SNOActor.rmpt_FloorExplosion_directional,
            SNOActor.Wizard_HydraHead_Default_3,
            SNOActor.Wizard_HydraHead_Default_2,
            SNOActor.Wizard_HydraHead_Default_1,
            SNOActor.Wizard_HydraHead_Lightning_2,
            SNOActor.Wizard_HydraHead_Lightning_3,
            SNOActor.Wizard_HydraHead_Lightning_1,
            SNOActor.Wizard_HydraHead_Frost_2,
            SNOActor.Wizard_HydraHead_Frost_3,
            SNOActor.Wizard_HydraHead_Frost_1,
            SNOActor.Wizard_HydraHead_Big,
            (SNOActor)249190,
            (SNOActor)251396,
            SNOActor.bastionsKeepGuard_Melee_A_01_NPC,
            SNOActor.bastionsKeepGuard_Melee_A_01,
            SNOActor.LootType2_BastionsKeepGuard_Frosty_Melee_Corpse_05,
            SNOActor.LustVoiceover,
            (SNOActor)245828,
            SNOActor.Diablo_VO,
            SNOActor.demonFlyer_A_bomber,
            SNOActor.caldeumMiddleClass_Male_A_TimedDungeon,
            SNOActor.a3dun_keep_firewall2,
            SNOActor.a3dun_keep_firewall1,
            SNOActor.BelialVoiceover,
            (SNOActor)245910,
            SNOActor.Monk_female_mysticAlly_golden,
            SNOActor.Monk_female_mysticAlly,
            SNOActor.Monk_female_mysticAlly_indigo,
            SNOActor.Monk_female_mysticAlly_crimson,
            SNOActor.Monk_female_mysticAlly_alabaster,
            SNOActor.Monk_female_mysticAlly_obsidian,
            SNOActor.Monk_male_mysticAlly,
            SNOActor.Monk_male_mysticAlly_indigo,
            SNOActor.Monk_male_mysticAlly_crimson,
            SNOActor.Monk_male_mysticAlly_golden,
            SNOActor.Monk_male_mysticAlly_alabaster,
            SNOActor.Monk_male_mysticAlly_obsidian,
            SNOActor.SkeletonKing_Target_Proxy,
            SNOActor.Generic_Proxy,
            SNOActor.DH_Companion_Ferret,
            SNOActor.DH_companion_spider,
            SNOActor.DH_Companion,
            SNOActor.DH_Companion_RuneE,
            SNOActor.DH_companion_Boar,
            SNOActor.DH_Companion_RuneC,
            SNOActor.a3dun_Crater_DemonClawBomb_A_Monster,
            SNOActor.bastionsKeepGuard_Melee_A_01_NPC_Frosty,
            SNOActor.trDun_Magic_Painting,
            SNOActor.trDun_Magic_Painting_B,
            SNOActor.trDun_Magic_Painting_C,
            SNOActor.trDun_Magic_Painting_E_NoSpawn,
            SNOActor.MinimapIconStairs,
            SNOActor.EnchantressNPC,
            (SNOActor)249338,
            (SNOActor)251416,
            (SNOActor)249192,
            SNOActor.Skeleton_A_Necromancer,
            SNOActor.a4dun_Aspect_Ghost_01,
            SNOActor.a4dun_Aspect_Ghost_02,
            SNOActor.a4dun_Aspect_Ghost_05,
            SNOActor.a4dun_Spire_HellFissure,
            SNOActor.a4dun_spire_Spike_Trap_Monster,
            SNOActor.demonFlyer_C_bomber,
            (SNOActor)245838,
            //bone pile
            SNOActor.demonFlyer_C_bomber,
            (SNOActor)245838,
            // rrrix act 1
            SNOActor.GhostKnight1_Festering,
            (SNOActor)245919,
            SNOActor.trDun_SkeletonKing_Sealed_Door,
            SNOActor.trOut_Town_Attack_Church_Switch,
            SNOActor.a3dun_keep_firewall2,
            SNOActor.trOut_Cath_Entrance_Door,
            SNOActor.a1dunLeor_Interactive_Door_Locked_B,
            SNOActor.Crypt_Alcove_Of_Rot_Custom,
            SNOActor.a1dunLeor_Interactive_Door_Locked_A,
            SNOActor.a1dun_Leor_Spike_TrapDoor,
            SNOActor.a1dun_leor_firewall1,
            SNOActor.trDun_Crypt_Skeleton_King_Throne_Parts,
            // rrrix act 2
            SNOActor.a2dun_Zolt_Head_Container,
            SNOActor.caOut_Boneyards_Bloodhawknest,
            SNOActor.Intro_Jeweler,
            SNOActor.KulleVoiceover,
            SNOActor.BelialVoiceover,
            SNOActor.a2dun_Swr_Grate_Spawner_a,
            SNOActor.caldeumFormerRichGuy,
            SNOActor.caldeumEliteChapLady,
            SNOActor.Caldeum_FormerNoblewoman,
            SNOActor.caldeumGuard_Captain_B_Ravd,
            SNOActor.caldeumGuard_Cleaver_A_Jarulf,
            SNOActor.a2dun_Cald_Belial_Magic_Blocker,
            (SNOActor)180802,
            (SNOActor)180809,
            SNOActor.caldeumPoor_Female_D_Town,
            SNOActor.caldeumPoor_Female_E_Town,
            SNOActor.caldeumPoor_Male_D_Town,
            SNOActor.caldeumTortured_Poor_Female_B_Town,
            SNOActor.caldeumTortured_Poor_Female_C_Town,
            SNOActor.caldeumTortured_Poor_Male_G_Town,
            SNOActor.caldeumPoor_Male_C_Town,
            SNOActor.caldeumPoor_Male_E_Town,
            SNOActor.caldeumChild_Male_A_Town,
            SNOActor.caldeumPoor_Male_B_Town,
            SNOActor.caldeumMiddleClass_Male_C_NoWanderNoTurn_Town,
            SNOActor.caldeumTortured_Poor_Male_C_Town,
            SNOActor.vizjereiMale_A_Town,
            SNOActor.caldeumGuard_Cleaver_A_Town,
            SNOActor.caldeumGuard_Spear_Imperial_Town,
            SNOActor.ZoltunKulleFollower,
            SNOActor.vizjereiMale_A_RichThief,
            SNOActor.caldeumWealthy_Male_A_Fountain,
            SNOActor.caldeumPoor_Female_B_Town_Gate,
            SNOActor.Faraz,
            SNOActor.Bahaar,
            SNOActor.caldeumChild_Female_C,
            SNOActor.caldeumChild_Female_B_Follow,
            SNOActor.caldeumPoor_Female_Shopper,
            SNOActor.a2dun_Cave_Worms_Bush_A,
            SNOActor.a2dun_Cave_Worms_Bush_B,
            SNOActor.a2dun_Cave_Worms_Bush_C,
            SNOActor.a2dun_Cave_Worms_Rocks_A,
            SNOActor.a2dun_Cave_Worms_Rocks_B,
            SNOActor.caldeumMiddleClass_Male_A_Ambient,
            SNOActor.caldeumPoor_Male_A_Ambient,
            SNOActor.caldeumChild_Male_A_Town_Bunny,
            SNOActor.caldeumPoor_Male_C_Ambient,
            SNOActor.caldeumPoor_Male_D_Ambient,
            SNOActor.caldeumPoor_Male_F_Ambient,
            SNOActor.g_portal_Ladder_Short_Blue_largeRadius,
            SNOActor.caOut_Raven_Perched_A,
            SNOActor.caOut_Raven_Pecking_A,
            SNOActor.Zakarum_Female_Wealthy_Gates,
            SNOActor.a2dun_Zolt_Head_Container,
            SNOActor.a2dun_Swr_Arch_Spawner_ZombieCrawler_01,
            SNOActor.HakanProjection,
            SNOActor.a2dun_Zolt_Random_Breakable_Corner,
            SNOActor.Asheara,
            SNOActor.caldeumChild_Female_A,
            SNOActor.caldeumPoor_Male_A,
            SNOActor.caldeumPoor_Male_B,
            SNOActor.caldeumTortured_Poor_Male_B,
            SNOActor.caldeumTortured_Poor_Male_G,
            SNOActor.Leah,
            SNOActor.Hireling_Templar,
            SNOActor.SphereTrigger,
            SNOActor.a2dun_Swr_Arch_Spawner_CryptChild_01,
            SNOActor.SuperCaldeumGuard_Cleaver_A,
            SNOActor.Hakan,
            SNOActor.caOut_Raven_Pecking_A,
            SNOActor.SuperCaldeumGuard_Cleaver_A,
            SNOActor.CaOut_Target_Dummy,
            SNOActor.trOut_Cultists_Summoning_Portal_B,
            SNOActor.a2dun_Cald_Belial_Acid_Attack,
            SNOActor.a2dun_Zolt_Shadow_Realm_Portal_Exit_B_Door,
            SNOActor.caOut_Oasis_Mine_Entrance_A,
            SNOActor.Belial_Spray_Marker,
            // rrrix act 3
            SNOActor.a3_Battlefield_GuardCatapult_Door,
            SNOActor.a3dun_Keep_Barrel_Snow_No_Skirt,
            // uber fire chains in Realm of Turmoil and Iron Gate in Realm of Chaos
            (SNOActor)263014,
            SNOActor.trDun_Cath_BookcaseShelves_A,
            SNOActor.trDun_Cath_BookcaseShelves_B,
            SNOActor.trDun_Gargoyle_01,
            SNOActor.trDun_Cath_Breakable_pillar,
            SNOActor.a1dun_Leor_Spike_Spawner_Switch,
            SNOActor.X1_Pand_HexMaze_PortalChampSummon, // Monster that looks like a portal, and summons champions.

            // 4798, old necromancer bounty quest actor

            // Pandemonium Fortress,
            SNOActor.X1_Fortress_Rack_C,
            // SNOActor.X1_Fortress_Rack_C_Stump,
            SNOActor.X1_Fortress_Rack_B,
            //SNOActor.X1_Fortress_Rack_B_Stump,
            SNOActor.X1_Fortress_Rack_D,
            //SNOActor.X1_Fortress_Rack_D_Stump,
            SNOActor.X1_Fortress_Rack_E,
            //SNOActor.X1_Fortress_Rack_E_Stump,
            SNOActor.X1_Fortress_Rack_F,
            //SNOActor.X1_Fortress_Rack_F_Stump,
            SNOActor.X1_Fortress_FloatRubble_A,
            SNOActor.X1_Fortress_FloatRubble_B,
            SNOActor.X1_Fortress_FloatRubble_C,
            SNOActor.X1_Fortress_FloatRubble_D,
            SNOActor.X1_Fortress_FloatRubble_E,
            SNOActor.X1_Fortress_FloatRubble_F,
            SNOActor.x1_Abattoir_furnaceWall,
            SNOActor.x1_abattoir_furnace_01,
            SNOActor.x1_Abattoir_furnaceSpinner_Event_Phase2, //-- this is a rotating avoidance, with a fire "beam" about 45f in length
            SNOActor.x1_Catacombs_Weapon_Rack_Raise,
            SNOActor.x1_Westm_HeroWorship03_VO,
            SNOActor.X1_Westm_Door_Giant_Closed,
            SNOActor.X1_Westm_Door_Giant_Opening_Event,
            SNOActor.x1_Pand_Ext_ImperiusCharge_Barricade,
            SNOActor.x1_Westm_DeathOrb_Caster_TEST,
            SNOActor.X1_Westm_Breakable_Wolf_Head_A,

            SNOActor.x1_Urzael_SoundSpawner,
            SNOActor.x1_Urzael_SoundSpawner_04,
            SNOActor.x1_Urzael_SoundSpawner_02,
            SNOActor.x1_Urzael_SoundSpawner_03,
            SNOActor.x1_Fortress_Crystal_Prison_Shield,
            SNOActor.X1_WarpToPortal,
            SNOActor.x1_Malthael_Boss_Orb_Collapse,
            SNOActor.x1_Fortress_Portal_Switch,
            SNOActor.X1_WarpToPortal,
            SNOActor.x1_Catacombs_Breakable_Window_Relief,

            //x1_Westm_HeroWorship01_VO Dist=10 IsElite=False LoS=True HP=1,00 Dir=SW - Name=x1
            //It's trying to attack that in town
            SNOActor.x1_Westm_HeroWorship01_VO,
            SNOActor.x1_Westm_HeroWorship02_VO,
            SNOActor.x1_Westm_HeroWorship03_VO,

            SNOActor.px_Bounty_Ramparts_Camp_CatapultIdle, // not interactable
            SNOActor.px_Bounty_Ramparts_Camp_CatapultFiring,

            // Bulba
            SNOActor.px_Wilderness_Camp_TemplarPrisoners,
            SNOActor.px_Highlands_Camp_ResurgentCult_Totem,
            //434366, //A1 : Wortham Survivors
            //432770, //A1: Queen's Desert
            //433051, //A2: Prisoners of the Cult
            //432331, //A2: Blood and Iron
            //432885, //A2: The Ancient Devices
            //433184, //A3: The Lost Patrol
            //433295, //A3: The Demon Gates
            //433385, //A3: Catapult Command (It's a switch instead of chest, so might be pre 2.2 bounty) // needed for quest. 'catapault command'
            //433402, //A4: Hell Portals
            //433124, //A4: Tormented Angels
            //433246, //A5: Death's Embrace
            //433316, //A5: Rathma's Gift

			SNOActor.x1_Catacombs_chest_rare_GardenEvent,
			SNOActor.a4dun_spire_CorruptedWallAngel_Column_2,
            SNOActor.px_Bounty_Camp_Pinger_450,
            SNOActor.px_Bounty_Camp_Pinger,
            SNOActor.a2dun_Aqd_Act_Waterwheel_Lever_A_01_WaterPuzzle,
            SNOActor.X1_Westm_Door_Giant_Closed_A,
            SNOActor.x1_Monstrosity_ScorpionBug_A_CryptCursedShrine
        };

        // A list of bosses in the game, just to make CERTAIN they are treated as elites
        /// <summary>
        /// Contains ActorSnoId of known Bosses
        /// </summary>
        public static HashSet<int> BossIds { get; } = new HashSet<int>
        {
            // Siegebreaker (96192), Azmodan (89690), Cydea (95250), Heart-thing (193077),
            96192,                   89690,           95250,         193077,
            //Kulle (80509), Small Belial (220160), Big Belial (3349), Diablo 1 (114917), terror Diablo (133562)
            80509,           220160,                3349,              114917,            133562,
            62975, // Belial TrueForm
            //Maghda, Kamyr (MiniBoss before Belial)
            6031, 51298,
            // Ghom
            87642,
            // I dunno?
            255929, 256711, 256508, 256187, 256189, 256709,
            // Another Cydaea
            137139,
            // Diablo shadow clones (needs all of them, there is a male & female version of each class!)
            144001, 144003, 143996, 143994,
            // Jondar, Chancellor, Queen Araneae (act 1 dungeons), Skeleton King, Butcher
            86624, 156353, 51341, 5350, 3526,
            361347, //Jondar from the Adventure mode
            215103, // Istaku
            4630, // Rakanoth
            256015, // Xah'Rith Keywarden
            115403, // A1 Cain Skeleton boss
            4373, 4376, 177539, // A1 Robbers
            168240, // A2 Jewler quest
            84919, // Skeleton King
            108444, // ZombieFemale_A_TristramQuest (Wretched Mothers)
            176889, // ZombieFemale_Unique_WretchedQueen
            129439, //Arsect The Venomous
            164502, // sandMonster_A_Head_Guardian
            378665, // Greed

            // A5
            316839, // x1_deathOrb_bodyPile
            375106, // A5 x1_Death_Orb_Monster
            375111, // A5 x1_Death_Orb_Master_Monster
            279394, // A5 Adria

            300862, // X1_BigRed_Chronodemon_Event_ForgottenWar
            318425, // X1_CoreEliteDemon_Chronodemon_Event_ForgottenWar

            300866, // X1_Angel_TrooperBoss_Event_ForgottenWar

            346482, // X1_PandExt_TimeTrap
            367456, // x1_Pand_Ext_Event_Hive_Blocker

            347276, // x1_Fortress_Soul_Grinder_A

            374751, // x1_PortalGuardian_A
            307339, // X1_Rockworm_Pand_Unique_HexMaze
            297730, // x1_Malthael_Boss
        };

        /// <summary>
        /// Contains the list of Boss Level Area ID's
        /// </summary>
        public static HashSet<SNOLevelArea> BossLevelAreaIDs { get; } = new HashSet<SNOLevelArea>
        {
            SNOLevelArea.A1_trOut_Tristram_Inn,
            SNOLevelArea.A3_Dun_Keep_Hub_Inn,
            SNOLevelArea.A2_Dun_Zolt_BossFight_Level04,
            SNOLevelArea.A1_trOut_Tristram_CainsHouse,
            SNOLevelArea.A1_trDun_Cain_Intro,
            SNOLevelArea.A1_trDun_Level07D,
            SNOLevelArea.A1_C6_SpiderCave_01_Queen,
            SNOLevelArea.A1_trDun_ButchersLair_02,
            SNOLevelArea.A2_caOut_Cellar_Alcarnus_Main,
            SNOLevelArea.A2_Dun_Swr_Adria_Level01,
            SNOLevelArea.A2_Belial_Room_Intro,
            SNOLevelArea.A2_Belial_Room_01,
            SNOLevelArea.A3_Gluttony_Boss,
            SNOLevelArea.A3_Battlefield_C,
            SNOLevelArea.A3_dun_Crater_ST_Level04B,
            SNOLevelArea.A3_AzmodanFight,
            SNOLevelArea.A4_dun_LibraryOfFate,
            SNOLevelArea.a4dun_spire_exterior,
            SNOLevelArea.A4_dun_Garden3_SpireEntrance,
            SNOLevelArea.A4_dun_Diablo_Arena,
            SNOLevelArea.a4dun_Diablo_ShadowRealm_01,
            SNOLevelArea.A4_dun_Diablo_Arena_Phase3,
            SNOLevelArea.A2_caOut_CT_RefugeeCamp,
            SNOLevelArea.A1_trDun_Level07D,
            SNOLevelArea.X1_Malthael_Boss_Arena,
            SNOLevelArea.A4_dun_Diablo_Arena
        };

        public static HashSet<int> BountyTurnInQuests { get; } = new HashSet<int>()
        {
            356988, //x1_AdventureMode_BountyTurnin_A1
            356994, //x1_AdventureMode_BountyTurnin_A2
            356996, //x1_AdventureMode_BountyTurnin_A3
            356999, //x1_AdventureMode_BountyTurnin_A4
            357001, //x1_AdventureMode_BountyTurnin_A5
        };

        /// <summary>
        /// This list is used when an actor has an attribute BuffVisualEffect=1, e.g. fire floors in The Butcher arena
        /// </summary>
        public static HashSet<int> ButcherFloorPanels { get; } = new HashSet<int>
        {
            // Butcher Floor Panels
            201454, 201464, 201426, 201438, 200969, 201423, 201242,
        };

        /// <summary>
        /// Chests/average-level containers that deserve a bit of extra radius (ie - they are more worthwhile to loot than "random" misc/junk containers)
        /// </summary>
        public static HashSet<SNOActor> ContainerWhiteListIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.x1_Catacombs_Weapon_Rack_Raise,
            SNOActor.TrOut_Fields_Chest,
            SNOActor.TrOut_Highlands_Chest,
            SNOActor.CaOut_Oasis_Chest,
            SNOActor.trOut_wilderness_chest,
            SNOActor.trOut_Tristram_chest,
            SNOActor.a1dun_Leor_Chest,
            SNOActor.a1dun_Cath_chest,
            SNOActor.a3dun_Crater_Chest,
            SNOActor.caOut_StingingWinds_Chest,
            SNOActor.A3_Battlefield_Chest_Snowy,
            SNOActor.A3_Battlefield_Chest_Frosty,
            SNOActor.Lore_AzmodanChest3,
            SNOActor.Lore_AzmodanChest2,
            SNOActor.a3dun_Keep_Chest_A,
            SNOActor.a3dun_Crater_ST_Chest,
            SNOActor.a3dun_Keep_Chest_Rare,
            SNOActor.a1dun_Random_Cloud,
            SNOActor.a1dun_random_pot_of_gold_A,
            SNOActor.Pinata,
            SNOActor.a2dun_Spider_EggSack__Chest,
            SNOActor.a2dun_Spider_Chest,
            //SNOActor.X1_PandExt_ArmorRack, // Weaponracks on battlefields of eternity --> best place to farm white crafting materials
            SNOActor.a3Battlefield_Props_Demonic_Container_A,
            SNOActor.trOut_Wilderness_Scarecrow_A,
            SNOActor.trOut_OldTristram_AnvilOfFury,
            SNOActor.x1_PandExt_Time_Activator,
            SNOActor.LootType3_GraveGuard_C_Corpse_03,
            SNOActor.Adventurer_A_Corpse_01_WarrivEvent,
            SNOActor.trDun_Cath_FloorSpawner_01,
            SNOActor.trDun_Cath_BookcaseShelves_A,
            SNOActor.Cath_Lecturn_LachdanansScroll,
            SNOActor.Lore_Inarius_Corrupt,
            SNOActor.FesteringWoods_WarriorsRest_Lore
        };

        public static Dictionary<SNOActor, float> CustomObjectRadius { get; } = new Dictionary<SNOActor, float>
        {
            {SNOActor.a3dun_Keep_Bridge_Switch, 4},
            {SNOActor.a3dun_Keep_Bridge, 4},
            {SNOActor.a3dun_Keep_Door_Wooden_A, 40},
            {SNOActor.Belial, 25},
            {SNOActor.SpiderQueen, 10},
            {SNOActor.a4dun_Garden_Purification_Well_Enlightened, 35},
            {SNOActor.a4dun_DIablo_Arena_Health_Well, 10},
            {SNOActor.x1_Death_Orb_Master_Monster, 45f},
            {SNOActor.x1_PandExt_Time_Activator, 15f},
            {SNOActor.x1_Catacombs_Nephalem_Event_Switch, 5f},
            {SNOActor.x1_westm_Bridge, 40f},
            {SNOActor.a3dun_Keep_SiegeTowerDoor_A, 14f},
            {SNOActor.x1_Malthael_DeathFogMonster, 15f},
            {SNOActor.a4dun_Garden_Corruption_Monster, 20f},
            {SNOActor.a4dun_Garden_Corruption_HellRift_Monster, 20f},
            {SNOActor.a4dun_Garden_Corruption_Monster_Despair, 20f},
            {SNOActor.p6_Event_Moor_BloodCollection_Altar, 22f},
            {SNOActor.Rockworm_Stationary_QueenWorm, 12f}
        };

        public static Dictionary<int, float> DefaultAvoidanceAnimationCustomRadius { get; } = new Dictionary<int, float>()
        {
            {(int)SNOAnim.morluSpellcaster_attack_AOE_01, 20f },
            {(int)SNOAnim.x1_deathMaiden_attack_special_360_01, 15f},
            {(int)SNOAnim.x1_Squigglet_Generic_Cast, 40f}, // Rift Boss Slime AOE
        };

        //       Line 15116:     p1_rifts_Orb_flippy = 434918,
        //Line 15230:     p1_normal_rifts_Orb = 436807,
        public static Dictionary<int, float> DefaultAvoidanceCustomRadius { get; } = new Dictionary<int, float>()
        {
            {330824, 35f }, // A5 Urzael animations
            {360598, 10f }, // x1_Urzael_CeilingDebris_DamagingFire_wall
            {359205, 20f }, // x1_Urzael_ceilingDebris_Impact_Beam
            {360883, 20f }, // x1_Urzael_ceilingDebris_Impact_Circle
            {362850, 12f }, // x1_Urzael_Cannonball_burning_invisible
            {346976, 12f }, // x1_Urzael_Cannonball_burning_impact
            {346975, 12f }, // x1_Urzael_Cannonball_burning

            {360738, 12f}, // X1_Adria_arcanePool
            {338889, 5f}, // x1_Adria_bouncingProjectile
            {358404, 15f}, // X1_Adria_blood_large

            {335505, 5f}, // x1_malthael_drainSoul_ghost
            {325136, 20f}, // x1_Malthael_DeathFogMonster
            {340512, 8f}, // x1_Malthael_Mephisto_LightningObject
            {343767, 35f }, // Mallet Demons
            {106709, 35f }, // Mallet Demons
            {219736, 35f }, // Mallet Demons
            {219751, 35f }, // Mallet Demons

            {159369, 20f }, //MorluMeteor
            {4103, 25f}, // Meteor
            {3528, 15f}, // Butcher_hook

            {359703, 13f}, // X1_Unique_Monster_Generic_AOE_DOT_Cold_10foot
            {363356, 25f}, // X1_Unique_Monster_Generic_AOE_DOT_Cold_20foot
            {359693, 13f}, // X1_Unique_Monster_Generic_AOE_DOT_Fire_10foot
            {363357, 25f}, // X1_Unique_Monster_Generic_AOE_DOT_Fire_20foot
            {364542, 13f}, // X1_Unique_Monster_Generic_AOE_DOT_Lightning_10foot
            {364543, 25f}, // X1_Unique_Monster_Generic_AOE_DOT_Lightning_20foot
            {377537, 13f}, // X1_Unique_Monster_Generic_AOE_DOT_Lightning_hardpoints
            {360046, 13f}, // X1_Unique_Monster_Generic_AOE_DOT_Poison_10foot
            {363358, 25f}, // X1_Unique_Monster_Generic_AOE_DOT_Poison_20foot
            {368156, 13f}, // X1_Unique_Monster_Generic_AOE_Lightning_Ring
            {358917, 13f}, // X1_Unique_Monster_Generic_AOE_Sphere_Distortion
            {377086, 13f}, // X1_Unique_Monster_Generic_Projectile_Arcane
            {377087, 13f}, // X1_Unique_Monster_Generic_Projectile_Cold
            {377088, 13f}, // X1_Unique_Monster_Generic_Projectile_Fire
            {377089, 13f}, // X1_Unique_Monster_Generic_Projectile_Holy
            {377090, 13f}, // X1_Unique_Monster_Generic_Projectile_Lightning
            {377091, 13f}, // X1_Unique_Monster_Generic_Projectile_Physical
            {377092, 13f}, // X1_Unique_Monster_Generic_Projectile_Poison
            {123842,60f},

            {(int)SNOActor.x1_LR_boss_terrorDemon_A_projectile, 10f},

            {4803, 25f}, // Molten Core
            {4804, 25f}, // Molten Core
            {224225, 25f}, // Molten Core
            {247987, 25f}, // Molten Core
            {139741, 15},
        };

        public static HashSet<SNOActor> DemonHunterPetIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.DH_Companion_Ferret,
            SNOActor.DH_companion_spider,
            SNOActor.DH_Companion,
            SNOActor.DH_Companion_RuneE,
            SNOActor.DH_companion_Boar,
            SNOActor.DH_Companion_RuneC,
            SNOActor.DH_Companion_RuneD,
            SNOActor.DH_Companion_RuneE,
            (SNOActor)334861,
        };

        public static HashSet<SNOActor> DemonHunterSentryIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.DH_sentry,
            SNOActor.DH_sentry_addsMissiles,
            SNOActor.DH_sentry_addsDuration,
            SNOActor.DH_sentry_tether,
            SNOActor.DH_sentry_addsHeals,
            SNOActor.DH_sentry_addsShield,
        };

        /// <summary>
        /// Destructible things that need targeting by a location instead of an AcdId (stuff you can't "click on" to destroy in-game)
        /// </summary>
        public static HashSet<int> DestroyAtLocationIds { get; } = new HashSet<int>
        {
            170657, 116409, 121586, 155255, 104596, 93306,
        };

        /// <summary>
        /// This is the RadiusDistance at which destructibles and barricades (logstacks, large crates, carts, etc.) are added to the cache
        /// </summary>
        public static Dictionary<int, float> DestructableObjectRadius { get; } = new Dictionary<int, float>
        {
            {(int)SNOActor.x1_Westm_Barricade_Round, 25},
            {2972, 10}, {80357, 16}, {116508, 10}, {113932, 8}, {197514, 18}, {108587, 8}, {108618, 8}, {108612, 8}, {116409, 18}, {121586, 22},
            {195101, 10}, {195108, 25}, {170657, 5}, {181228, 10}, {211959, 25}, {210418, 25}, {174496, 4}, {193963, 5}, {159066, 12}, {160570, 12},
            {55325, 5}, {5718, 14}, {5909, 10}, {5792, 8}, {108194, 8}, {129031, 30}, {192867, 3.5f}, {155255, 8}, {54530, 6}, {157541, 6},
            {93306, 10},
        };

        // De-weight to zero but they still need to be processed for ClsoedDoor checks. todo proper fix.
        public static HashSet<SNOActor> DoorsToAlwaysIgnore { get; } = new HashSet<SNOActor>
        {
            SNOActor.caOut_Oasis_Door_Aqueduct_A_Top,
            SNOActor.p43_AD_trDun_Cath_WoodDoor_Lazarus,
            SNOActor.p43_AD_a1dun_Leor_Jail_Door_SuperLocked_A,
            SNOActor.p43_AD_a1dun_Leor_Gate_A,
            SNOActor.P6_Church_BloodPool,
        };

        public static HashSet<SNOQuest> EventQuests { get; } = new HashSet<SNOQuest>()
        {
            SNOQuest.x1_Catacombs_NS_06Mutant_Evant,
            SNOQuest.x1_Event_Horde_HunterKillers,
            SNOQuest.x1_Event_WaveFight_AncientEvils,
            SNOQuest.X1_Graveyard_GraveRobber_Event,
            SNOQuest.X1_Pand_Ext_ForgottenWar_Adventure,

            SNOQuest.x1_Event_Horde_ArmyOfHell,
            SNOQuest.x1_Event_Horde_Bonepit,
            SNOQuest.x1_Event_Horde_DeathCellar,
            SNOQuest.x1_Event_Horde_Deathfire,
            SNOQuest.x1_Event_Horde_DesertFortress,
            SNOQuest.x1_Event_Horde_Dustbowl,
            SNOQuest.x1_Event_Horde_FleshpitGrove,
            SNOQuest.x1_Event_Horde_FlyingAssasins,
            SNOQuest.x1_Event_Horde_FoulHatchery,
            SNOQuest.x1_Event_Horde_GhoulSwarm,
            SNOQuest.x1_Event_Horde_GuardSlaughter,
            SNOQuest.x1_Event_Horde_HunterKillers,
            SNOQuest.x1_Event_Horde_InfernalSky,
            SNOQuest.x1_Event_Horde_SpiderTrap,
            SNOQuest.x1_Event_Horde_UdderChaos,
            SNOQuest.x1_Event_Jar_Of_Souls_NecroVersion,
            SNOQuest.x1_Event_SpeedKill_Angel_Corrupt_A,
            SNOQuest.x1_Event_SpeedKill_BileCrawler_A,
            SNOQuest.x1_Event_SpeedKill_Bloodhawk_A,
            SNOQuest.x1_Event_SpeedKill_Boss_Adria,
            SNOQuest.x1_Event_SpeedKill_Boss_Despair,
            SNOQuest.x1_Event_SpeedKill_Boss_Ghom,
            SNOQuest.x1_Event_SpeedKill_Boss_Maghda,
            SNOQuest.x1_Event_SpeedKill_Boss_SkeletonKing,
            SNOQuest.x1_Event_SpeedKill_Champion_BigRed_A,
            SNOQuest.x1_Event_SpeedKill_Champion_FallenHound_D,
            SNOQuest.x1_Event_SpeedKill_Champion_FleshPitFlyer_C,
            SNOQuest.x1_Event_SpeedKill_Champion_GhostA,
            SNOQuest.x1_Event_SpeedKill_Champion_GoatmanB,
            SNOQuest.x1_Event_SpeedKill_Champion_MalletDemon_A,
            SNOQuest.x1_Event_SpeedKill_Champion_SandShark_A,
            SNOQuest.x1_Event_SpeedKill_Champion_SoulRipper_A,
            SNOQuest.x1_Event_SpeedKill_Champion_SquiggletA,
            SNOQuest.x1_Event_SpeedKill_Champion_SummonableA,
            SNOQuest.x1_Event_SpeedKill_Champion_azmodanBodyguard_A,
            SNOQuest.x1_Event_SpeedKill_Champion_creepMob_A,
            SNOQuest.x1_Event_SpeedKill_Champion_morluSpellcaster_A,
            SNOQuest.x1_Event_SpeedKill_Champion_x1_FloaterAngel_A,
            SNOQuest.x1_Event_SpeedKill_Champon_x1_Rockworm_Pand_A,
            SNOQuest.x1_Event_SpeedKill_CoreEliteDemon_A,
            SNOQuest.x1_Event_SpeedKill_Fallen_C,
            SNOQuest.x1_Event_SpeedKill_GhostHumansA,
            SNOQuest.x1_Event_SpeedKill_Ghoul_E,
            SNOQuest.x1_Event_SpeedKill_GoatmanA,
            SNOQuest.x1_Event_SpeedKill_Goatman_Melee_A_Ghost,
            SNOQuest.x1_Event_SpeedKill_Goatmutant_B,
            SNOQuest.x1_Event_SpeedKill_Lacuni_B,
            SNOQuest.x1_Event_SpeedKill_Monstrosity_Scorpion_A,
            SNOQuest.x1_Event_SpeedKill_Rare_Ghoul_B,
            SNOQuest.x1_Event_SpeedKill_Rare_Skeleton2HandA,
            SNOQuest.x1_Event_SpeedKill_Rare_ThousandPounder,
            SNOQuest.x1_Event_SpeedKill_Rare_demonTrooper_C,
            SNOQuest.x1_Event_SpeedKill_Rare_x1_westmarchBrute_C,
            SNOQuest.x1_Event_SpeedKill_Skeleton_E,
            SNOQuest.x1_Event_SpeedKill_SkeletonsA,
            SNOQuest.x1_Event_SpeedKill_Snakeman_A,
            SNOQuest.x1_Event_SpeedKill_Spiderling_B,
            SNOQuest.x1_Event_SpeedKill_Swarm_A,
            SNOQuest.x1_Event_SpeedKill_TentacleBears,
            SNOQuest.x1_Event_SpeedKill_TriuneCultist_C,
            SNOQuest.x1_Event_SpeedKill_TriuneVesselA,
            SNOQuest.x1_Event_SpeedKill_ZombieB,
            SNOQuest.x1_Event_SpeedKill_demonFlyer_B,
            SNOQuest.x1_Event_SpeedKill_electricEel_A,
            SNOQuest.x1_Event_SpeedKill_fastMummy_A,
            SNOQuest.x1_Event_SpeedKill_morluMelee_B,
            SNOQuest.x1_Event_SpeedKill_x1_BileCrawler_Skeletal_A,
            SNOQuest.x1_Event_SpeedKill_x1_BogFamily_A,
            SNOQuest.x1_Event_SpeedKill_x1_Monstrosity_ScorpionBug_A,
            SNOQuest.x1_Event_SpeedKill_x1_Skeleton_Ghost_A,
            SNOQuest.x1_Event_SpeedKill_x1_Tentacle_A,
            SNOQuest.x1_Event_SpeedKill_x1_bogBlight_Maggot_A,
            SNOQuest.x1_Event_SpeedKill_x1_leaperAngel_A,
            SNOQuest.x1_Event_SpeedKill_x1_portalGuardianMinion_A,
            SNOQuest.x1_Event_SpeedKill_x1_westmarchHound_A,
            SNOQuest.x1_Event_WaveFight_AncientEvils,
            SNOQuest.x1_Event_WaveFight_ArmyOfTheDead,
            SNOQuest.x1_Event_WaveFight_BloodClanAssault,
            SNOQuest.x1_Event_WaveFight_ChamberOfBone,
            SNOQuest.x1_Event_WaveFight_CultistLegion,
            SNOQuest.x1_Event_WaveFight_DeathChill,
            SNOQuest.x1_Event_WaveFight_FallenWarband,
            SNOQuest.x1_Event_WaveFight_ForsakenSoldiers,
            SNOQuest.x1_Event_WaveFight_HostileRealm,
            SNOQuest.x1_Event_WaveFight_Juggernaut,
            SNOQuest.x1_Event_WaveFight_KhazraWarband,
            SNOQuest.x1_Event_WaveFight_SunkenGrave,
        };

        /// <summary>
        /// ActorSnoId's of Very fast moving mobs (eg wasps), for special skill-selection decisions
        /// </summary>
        public static HashSet<int> FastMovingMonsterIds { get; } = new HashSet<int>
        {
            5212
        };

        public static HashSet<SNOActor> FetishArmyIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.Fetish_Melee_A,
            SNOActor.Fetish_Skeleton_A,
            SNOActor.Fetish_Melee_fire,
            SNOActor.Fetish_doubleStack_Shaman_A,
            SNOActor.Fetish_Melee_poison,
            SNOActor.Fetish_Melee_Sycophants,
        };

        public static HashSet<SNOActor> ForceDestructibles { get; } = new HashSet<SNOActor>()
        {
            SNOActor.x1_westm_Door_Wide_Clicky,
            SNOActor.a3dun_Keep_Door_Destructable,
            SNOActor.p1_Tgoblin_Vase_C,
            SNOActor.trOut_Highlands_Goatman_Totem_Gharbad,
            SNOActor.x1_westm_Graveyard_Floor_Sarcophagus_Undead_Husband_Event,
            SNOActor.p1_Tgoblin_Gold_Pile_A,
            SNOActor.p1_Tgoblin_Gold_Pile_C,
            SNOActor.p1_Tgoblin_Vase_A,
            SNOActor.p1_Tgoblin_Vase_C,
            SNOActor.Pinata,
            SNOActor.p4_Ruins_Frost_Goatman_Drum_A,
            SNOActor.a2dun_Aqd_Mummy_Spawner_Muck,
        };

        /// <summary>
        /// This is only intended for use with special bounty caves that spawn units
        /// as part of a quest that cannot be identified as quest monsters by any
        /// other means - use with caution.
        /// </summary>
        public static HashSet<SNOScene> ForceKillAllSceneSnoIds { get; } = new HashSet<SNOScene>
        {
            SNOScene.caOut_Interior_G_x01_y01,
            SNOScene.a2dun_Zolt_Random_W_03_Poltahr, //ragnyr idol bounty fight scene.
            SNOScene.caOut_Sub240x240_Mine_Destroyed,
            SNOScene.p4_ruins_frost_NSEW_03_Ziggurat, // king of the ziggaurat?
            SNOScene.a3dun_Keep_SW_03_Forge
        };

        /// <summary>
        /// World-object dictionaries eg large object lists, ignore lists etc.
        /// A list of SNO's to *FORCE* to type: Item. (BE CAREFUL WITH THIS!).
        /// </summary>
        public static HashSet<SNOActor> ForceToItemOverrideIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.DemonTrebuchetKey,
            SNOActor.DemonKey_Destruction,
            SNOActor.DemonKey_Hate,
            SNOActor.DemonKey_Terror
        };

        public static HashSet<int> ForceTownPortalLevelAreaIds { get; } = new HashSet<int>
        {
            55313, // Act 2 Caldeum Bazaar
        };

        public static HashSet<int> ForceUseWOTBIds { get; } = new HashSet<int>()
        {
            256015, 256000, 255996
        };

        public static HashSet<SNOActor> GargantuanIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.WD_Gargantuan_Cooldown,
            SNOActor.WD_Gargantuan_Cleave,
            SNOActor.WD_Gargantuan_Slam,
            SNOActor.WD_Gargantuan_Poison,
            SNOActor.WD_Gargantuan_Absorb,
            SNOActor.WD_Gargantuan,
            SNOActor.Legendary_WD_Gargantuan,
            SNOActor.Legendary_WD_Gargantuan_Cleave,
            SNOActor.Legendary_WD_Gargantuan_Cooldown,
            SNOActor.Legendary_WD_Gargantuan_Slam,
            SNOActor.Legendary_WD_Gargantuan_Poison,
            SNOActor.Legendary_WD_Gargantuan_Absorb,
        };

        /// <summary>
        /// A list of all known SNO's of treasure goblins/bandits etc.
        /// </summary>
        public static HashSet<SNOActor> GoblinIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.treasureGoblin_A, //5984
            SNOActor.treasureGoblin_B, //5985
            SNOActor.treasureGoblin_backpack, //5986
            SNOActor.treasureGoblin_C, //5987
            //SNOActor.treasureGoblin_Portal, //54862
            //SNOActor.treasureGoblin_Portal_Proxy, //54887
            //SNOActor.treasureGoblin_portal_emitter, //59948
            //SNOActor.treasureGoblin_portal_opening, //60549
            //SNOActor.treasureGoblin_portal_closing, //60558
            //SNOActor.treasureGoblin_portal_summon_trailActor, //108403
            //SNOActor.treasureGoblin_stunImmune_trailActor, //129286
            //SNOActor.Lore_Bestiary_TreasureGoblin, //147316
            SNOActor.Shrine_TreasureGoblin, //269349
            //SNOActor.treasureGoblin_A_Slave, //326803
            SNOActor.X1_Treasure_Goblin_Event_Runner, //330135
            //SNOActor.treasureGoblin_C_Unique_DevilsHand, //343046
            //SNOActor.p1_Tgoblin_Chest_Bomb, //382664
            //SNOActor.p1_Tgoblin_Gold_Pile_Gib, //385658
            //SNOActor.p1_Tgoblin_Gold_Pile_C, //386274
            //SNOActor.p1_Tgoblin_Gold_Pile_C_Gib, //386434
            //SNOActor.p1_Tgoblin_Gold_Pile_C_ribbons, //386558
            //SNOActor.p1_TGoblin_Gate, //389569
            //SNOActor.p1_Tgoblin_Gold_Pile_A, //390498
            //SNOActor.p1_TGoblin_LightBeam_A, //390682
            //SNOActor.p1_TGoblin_Torch, //391021
            //SNOActor.p1_TGoblin_Gondola, //391270
            SNOActor.treasureGoblin_G, //391593
            //SNOActor.p1_Tgoblin_Vase_A, //391765
            //SNOActor.p1_Tgoblin_Vault_Door, //392094
            //SNOActor.p1_TGoblin_Gondola_Vertical, //392098
            //SNOActor.p1_Tgoblin_Vase_C, //392606
            //SNOActor.p1_TGoblin_Gondola_Card_Left, //392657
            //SNOActor.p1_TGoblin_Gondola_Card_Right, //392660
            SNOActor.p1_treasureGoblin_inBackpack_A, //394196
            //SNOActor.p1_TGoblin_CenterPiece_Gem, //394555
            //SNOActor.p1_Tgoblin_Greed_Bait, //403041
            //SNOActor.p1_Greed_crowds_goblinPortal, //403532
            //SNOActor.p1_treasureGoblin_jump_trailActor, //403549
            SNOActor.p1_Tgoblin_basket_goblin, //403843
            SNOActor.p1_Tgoblin_basket_goblin, //403843
            //SNOActor.p1_greed_crowds_goblinCoinThrow_2, //404109
            //SNOActor.p1_Tgoblin_BG_Torch, //404302
            //SNOActor.p1_TGoblin_Gate_Closed, //404625
            //SNOActor.p1_TGoblin_CenterPiece_Gem_Fogged, //404731
            //SNOActor.p1_TGoblin_CenterPiece_Gem_INVIS, //404733
            //SNOActor.p1_Greed_crowds_goblinPortal_no_dist, //404854
            SNOActor.p1_treasureGoblin_tentacle_A, //405186
            SNOActor.p1_treasureGoblin_tentacle_backpack, //405189
            //SNOActor.p1_Tgoblin_Chest_Bomb_Chunks, //405308
            //SNOActor.p1_Tgoblin_Greed_Bait_Chunks, //405339
            SNOActor.p1_Portal_Tentacle_goblin, //405590
            SNOActor.treasureGoblin_D_Splitter, //408354
            SNOActor.treasureGoblin_E, //408655
            SNOActor.MarkerLocation_GoblinPortalIn, //408679
            SNOActor.treasureGoblin_F, //408989
            SNOActor.treasureGoblin_Portal_Open, //410460
            SNOActor.treasureGoblin_D_Splitter_02, //410572
            SNOActor.treasureGoblin_D_Splitter_03, //410574
            SNOActor.treasureGoblin_H, //413289
            //SNOActor.P2_Goblin_Spawner_LR, //425478
            SNOActor.p1_treasureGoblin_teleport_shell, //428094
            SNOActor.p1_treasureGoblin_backpack_B, //428205
            SNOActor.p1_treasureGoblin_backpack_F, //428206
            SNOActor.p1_treasureGoblin_backpack_C, //428211
            SNOActor.p1_treasureGoblin_backpack_H, //428213
            SNOActor.p1_treasureGoblin_backpack_D, //428247
            SNOActor.treasureGoblin_I, //428663
            SNOActor.treasureGoblin_J, //429161
            SNOActor.p1_treasureGoblin_backpack_J, //429526
            SNOActor.treasureGoblin_B_WhatsNew, //429615
            SNOActor.treasureGoblin_F_WhatsNew, //429619
            SNOActor.treasureGoblin_C_WhatsNew, //429620
            SNOActor.treasureGoblin_B_FX_WhatsNew, //429624
            //SNOActor.MarkerLocation_SpecialGoblinRiftSpawn, //429649
            SNOActor.p1_treasureGoblin_backpack_E, //429660
            //SNOActor.Spawner_MarkerLocation_SpecialGoblinRiftSpawn, //429676
            //SNOActor.treasureGoblin_A_LegacyPuzzleRing, //429689
            SNOActor.p1_Portal_Tentacle_goblin_NoAccess, //430916
            SNOActor.p1_treasureGoblin_backpack_I, //433905
            SNOActor.a4_Heaven_Shrine_TreasureGoblin, //434409
            SNOActor.treasureGoblin_J_WhatsNew, //434630
            SNOActor.treasureGoblin_J_FX_WhatsNew, //434631
            SNOActor.treasureGoblin_E_WhatsNew, //434632
            SNOActor.treasureGoblin_D_WhatsNew, //434633
            SNOActor.treasureGoblin_Anniversary_Event, //434745
            SNOActor.p1_Portal_Anniversary_goblin_NoAccess, //435639
            SNOActor.p3_cos_pet_Murkgoblin_A, //441208
            SNOActor.treasureGoblin_K, // Yahoo !treasureGoblin_K-613 ActorSnoId: 450993, Menagerist Goblin

            ////    //5984, 5985, 5987, 5988, 405186, 380657

            //// treasureGoblin_A
            //5984,
            //// treasureGoblin_B
            //5985,
            //// treasureGoblin_backpack
            //5986,
            //// treasureGoblin_C
            //5987,
            //// treasureGoblin_Portal
            ////54862,
            //// treasureGoblin_Portal_Proxy
            ////54887,
            //// treasureGoblin_portal_emitter
            ////59948,
            //// treasureGoblin_portal_opening
            ////60549,
            //// treasureGoblin_portal_closing
            ////60558,
            //// treasureGoblin_portal_summon_trailActor
            ////108403,
            //// treasureGoblin_stunImmune_trailActor
            ////129286,
            //// Lore_Bestiary_TreasureGoblin
            ////147316,
            //// treasureGoblin_A_Slave
            ////326803,
            //// treasureGoblin_C_Unique_DevilsHand
            //343046,
            //// p1_treasureGobin_A_Unique_GreedMinion
            //380657,
            //// treasureGoblin_G
            //391593,
            //// p1_treasureGoblin_inBackpack_A
            //394196,
            //// p1_treasureGoblin_jump_trailActor
            ////403549,
            //// p1_treasureGoblin_tentacle_A
            //405186,
            //// p1_treasureGoblin_tentacle_backpack
            //405189,
            //// treasureGoblin_D_Splitter
            //408354,
            //// treasureGoblin_E
            //408655,
            //// treasureGoblin_F
            //408989,
            //// treasureGoblin_Portal_Open
            ////410460,
            //// treasureGoblin_D_Splitter_02
            //410572,
            //// treasureGoblin_D_Splitter_03
            //410574,
            //// treasureGoblin_H
            //413289,
            //// p1_treasureGoblin_teleport_shell
            ////428094,
            //// p1_treasureGoblin_backpack_B
            //428205,
            //// p1_treasureGoblin_backpack_F
            //428206,
            //// p1_treasureGoblin_backpack_C
            //428211,
            //// p1_treasureGoblin_backpack_H
            //428213,
            //// p1_treasureGoblin_backpack_D
            //428247,
            //// treasureGoblin_I
            //428663,
            //// treasureGoblin_J
            //429161,
            //// p1_treasureGoblin_backpack_J
            //429526,
            //// treasureGoblin_B_WhatsNew
            //429615,
            //// treasureGoblin_F_WhatsNew
            //429619,
            //// treasureGoblin_C_WhatsNew
            //429620,
            //// treasureGoblin_B_FX_WhatsNew
            //429624,
            //// p1_treasureGoblin_backpack_E
            //429660,
            //// treasureGoblin_A_LegacyPuzzleRing
            ////429689,
            //// p1_treasureGoblin_backpack_I
            //433905,
            //// treasureGoblin_J_WhatsNew
            //434630,
            //// treasureGoblin_J_FX_WhatsNew
            //434631,
            //// treasureGoblin_E_WhatsNew
            //434632,
            //// treasureGoblin_D_WhatsNew
            //434633,
            //// treasureGoblin_Anniversary_Event
            //434745,
        };

        public static HashSet<SNOActor> HidingUnits { get; } = new HashSet<SNOActor>()
        {
            SNOActor.p6_Shepherd,
            SNOActor.Rockworm_Stationary_QueenWorm,
        };

        public static HashSet<SNOActor> HighPriorityInteractables { get; } = new HashSet<SNOActor>
        {
            SNOActor.a3dun_Keep_Bridge_Switch,
            SNOActor.a3dun_Keep_Bridge_Switch_B,
            SNOActor.A4dun_Garden_Chest_Cosmetic_02,
        };

        public static HashSet<SNOActor> IgnoreUntargettableAttribute { get; } = new HashSet<SNOActor>()
        {
            SNOActor.snakeMan_Melee_A, // A2 Snakem
        };

        public static Dictionary<SNOActor, int> InteractEndAnimations { get; } = new Dictionary<SNOActor, int>()
        {
            {SNOActor.x1_Bog_Beacon_B, 348093},
        };

        /// <summary>
        /// Interactable whitelist - things that need interacting with like special wheels, levers - they will be blacklisted for 30 seconds after one-use
        /// </summary>
        public static HashSet<SNOActor> InteractWhiteListIds { get; } = new HashSet<SNOActor>
        {
            //454511, //p43_AD_Valor_BloodStone-21326
            SNOActor.p43_trDun_Lecturn_ArkainesValor,
            SNOActor.p43_trDun_Lecturn_ChamberOfBone,
            //455675, //p43_AD_Valor_Pedestal-21061 (Sno=455675
            SNOActor.p43_trDun_Lecturn_HallsOfTheBlind, //p43_trdun_lecturn_hallsoftheblind- 453770

            //328830//x1_Fortress_Portal_Switch (328830)
            SNOActor.x1_westm_Bridge, // x1_westm_Bridge
            SNOActor.TentacleLord, // TentacleLord (209133)  QuestSnoId: 434753 QuestStep: 1, Description: Slay the Infernal Bovine herd!
            SNOActor.x1_Global_Chest_StartsClean, // Special Event Chest
            SNOActor.A4dun_Garden_Chest_Cosmetic_02, // Mystical Chest - Falcon's Wings
            SNOActor.a3dun_Keep_Bridge_Switch, // a3dun_Keep_Bridge_Switch
            SNOActor.a3dun_Keep_Bridge_Switch_B, // a3dun_Keep_Bridge_Switch_B
            SNOActor.a3dun_Keep_Bridge, // a3dun_Keep_Bridge
            SNOActor.a3dun_Keep_Door_Wooden_A, // a3dun_Keep_Door_Wooden_A
            SNOActor.a1dun_Leor_Spike_Spawner_Switch, // a1dun_Leor_Spike_Spawner_Switch
            SNOActor.Ghost_Jail_Prisoner, // A1 Cursed Hold Prisoners
            SNOActor.trDun_Cath_Chandelier_Trap_Switch2, // A1 Cathedral Switch
            SNOActor.x1_Global_Chest_CursedChest_B, // Cursed Chest - Damp Cellar

            SNOActor.a2dun_Aqd_Chest_Special_GreedyMiner, // Bounty Quest: A Miners' Gold requires it to be opened.

            // A5
            SNOActor.x1_Bog_Beacon_B, // Paths of the Drowned - portal switches - x1_Bog_Beacon_B
            SNOActor.X1_PandExt_SiegeRune, // A5 Siege Rune Path of War

            SNOActor.x1_Malthael_NPC, // A5 Spirit of Malthael - Tower of Korelan
            SNOActor.x1_Catacombs_Nephalem_Event_Switch, // A5 Nephalem Switch - Passage to Corvus

            SNOActor.X1_Angel_Common_Event_GreatWeapon, // X1_Angel_Common_Event_GreatWeapon
        };

        /// <summary>
        /// Last used-timers for all abilities to prevent spamming D3 memory for cancast checks too often
        /// These should NEVER need manually editing
        /// But you do need to make sure every skill used by TrinityPlugin is listed in here once!
        /// </summary>
        public static Dictionary<SNOPower, DateTime> LastUseAbilityTimeDefaults { get; internal set; } = new Dictionary<SNOPower, DateTime>
        {
            {SNOPower.DrinkHealthPotion, DateTime.MinValue},
            {SNOPower.Weapon_Melee_Instant, DateTime.MinValue},
            {SNOPower.Weapon_Ranged_Instant, DateTime.MinValue},
        };

        /// <summary>
        /// Objects that should never be ignored due to no Line of Sight (LoS) or ZDiff
        /// </summary>
        public static HashSet<SNOActor> LineOfSightWhitelist { get; } = new HashSet<SNOActor>
        {
            SNOActor.a2dun_Zolt_Pedestal,
            SNOActor.Siege_wallMonster_C,
            SNOActor.x1_westm_Bridge,
            SNOActor.a1dun_Leor_BloodWell_A,
            SNOActor.a4dun_DIablo_Arena_Health_Well,
            SNOActor.Catapult_a3dunKeep_WarMachines_Breakable,
            SNOActor.Belial_TreasureGrant,
            SNOActor.Belial,
            SNOActor.a4dun_Garden_Corruption_HellRift_Monster,
            SNOActor.a3dun_Crater_ST_GiantDemonHeart_Mob,
            SNOActor.x1_Death_Orb_Monster,
            SNOActor.x1_Death_Orb_Master_Monster,
            SNOActor.x1_Pand_BatteringRam_Hook_B_low,
            SNOActor.x1_Catacombs_Nephalem_Event_Switch,
            SNOActor.x1_Fortress_Soul_Grinder_A,
            SNOActor.x1_Pand_BatteringRam_Hook_B,
            SNOActor.x1_Urzael_Boss
        };

        /// <summary>
        /// Dictionary of Actor SNO's and cooresponding weighting/Priority
        /// </summary>
        public static Dictionary<int, int> MonsterCustomWeights { get; } = new Dictionary<int, int>
        {
            // Wood wraiths all this line (495 & 496 & 6572 & 139454 & 139456 & 170324 & 170325)
            {495, 901}, {496, 901}, {6572, 901}, {139454, 901}, {139456, 901}, {170324, 901}, {170325, 901},
            // -- added 4099 (act 2 fallen shaman)
            // Fallen Shaman prophets goblin Summoners (365 & 4100)
            {365, 1901}, {4099, 1901}, {4100, 1901},
            // The annoying grunts summoned by the above
            {4084, -250},
            // Wretched mothers that summon zombies in act 1 (6639)
            {6639, 951},
            // Fallen lunatic (4095)
            {4095, 2999},
            // Pestilence hands (4738)
            {4738, 1901},
            // Belial Minions
            {104014, 1500},
            // Act 1 doors that skeletons get stuck behind
            {454, 1500},
            // Cydaea boss (95250)
            {95250, 1501},

            //Cydaea Spiderlings (137139)
            {137139, -301},
            // GoatMutantshaman Elite (4304)
            {4304, 999},
            // GoatMutantshaman (4300)
            {4300, 901},
            // Succubus (5508)
            {5508, 801},
            // skeleton summoners (5387, 5388, 5389)
            {5387, 951}, {5388, 951}, {5389, 951},
            // Weak skeletons summoned by the above
            {5395, -401},
            // Wasp/Bees - Act 2 annoying flyers (5212)
            {5212, 751},
            // Dark summoner - summons the helion dogs (6035)
            {6035, 501},
            // Dark berserkers - has the huge damaging slow hit (6052)
            {6052, 501},
            // The giant undead fat grotesques that explode in act 1 (3847)
            {3847, 401},
            // Hive pods that summon flyers in act 1 (4152, 4153, 4154)
            {4152, 901}, {4153, 901}, {4154, 901},
            // Totems in act 1 that summon the ranged goatmen (166452)
            {166452, 901},
            // Totems in act 1 dungeons that summon skeletons (176907)
            {176907, 901},
            // Act 2 Sand Monster + Zultun Kulle (kill sand monster first)
            {226849, 20000}, {80509, 1100},
            // Maghda and her minions
            {6031, 801}, {178512, 901},
            // Uber Bosses - Skeleton King {255929}, Maghda {256189} & Pets {219702} which must be killed first
            {255929, 2999}, {219702, 1999}, {256189, 999},
            // Uber Bosses - Zoltun Kulle {256508} & Siegebreaker {256187}
            // Siegebreaker removed so the focus remains on Zoltun Kulle until he is dead
            {256508, 2999},
            //{256508, 2999}, {256187, 1899},
            // Uber Bosses - Ghom {256709} & Rakanot {256711}
            {256709, 2999}, {256711, 1899},

            // A5 Forgotton War trash
            { 300864, -300 },
        };

        public static HashSet<SNOActor> MysticAllyIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.Monk_female_mysticAlly_golden,
            SNOActor.Monk_female_mysticAlly,
            SNOActor.Monk_female_mysticAlly_indigo,
            SNOActor.Monk_female_mysticAlly_crimson,
            SNOActor.Monk_female_mysticAlly_alabaster,
            SNOActor.Monk_female_mysticAlly_obsidian,
            SNOActor.Monk_male_mysticAlly,
            SNOActor.Monk_male_mysticAlly_indigo,
            SNOActor.Monk_male_mysticAlly_crimson,
            SNOActor.Monk_male_mysticAlly_golden,
            SNOActor.Monk_male_mysticAlly_alabaster,
            SNOActor.Monk_male_mysticAlly_obsidian
        };

        /// <summary>
        /// Navigation obstacles for standard navigation down dungeons etc. to help DB movement
        /// MAKE SURE you add the *SAME* SNO to the "size" dictionary below, and include a reasonable size (keep it smaller rather than larger) for the SNO.
        /// </summary>
        public static HashSet<SNOActor> NavigationObstacleIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.trOut_FesteringWoods_Neph_Column_B,
            SNOActor.a3_Battlefield_demonic_forge,
            SNOActor.a3_crater_st_demonic_forge,
            SNOActor.a3dun_crater_st_Demon_ChainPylon_Fire_Azmodan,
            SNOActor.a3dun_crater_st_Demon_ChainPylon_Fire_Azmodan_Blocker,
            SNOActor.a3dun_Bridge_ammunition_A,
            SNOActor.trOut_Wagon_Barricade,
            SNOActor.Beast_Corpse_A_01,
            SNOActor.caOut_Boneyard_SkullSpawner,
            SNOActor.Blacksmith_Lore,
            SNOActor.trOut_FesteringWoods_Neph_Column_B,
            SNOActor.trOut_FesteringWoods_Neph_Column_B_Broken_Base,
            SNOActor.trOut_FesteringWoods_Neph_Column_C_Broken_Base_Bottom,
            SNOActor.trOut_FesteringWoods_Neph_Column_C_Broken_Base,
            SNOActor.x1_Bog_bloodSpring_small,
            SNOActor.x1_Bog_bloodSpring_medium,
            SNOActor.x1_Bog_bloodSpring_large,
            SNOActor.x1_Pand_Ext_Ordnance_Mine,
            SNOActor.x1_Bog_Family_Guard_Tower_Stump,
            SNOActor.x1_Westm_Hub_Stool_A,
            SNOActor.px_Bounty_Camp_Hellportals_Frame,
            SNOActor.px_Bounty_Camp_Pinger,
            SNOActor.px_Bounty_Camp_Hellportals_Frame,
            SNOActor.px_Bounty_Camp_Pinger,
            SNOActor.px_Bounty_Ramparts_Camp_CatapultFiring,
            SNOActor.px_Bounty_Ramparts_Camp_CatapultIdle,
            SNOActor.Catapult_a3dunKeep_WarMachines_Snow_Firing
        };

        /// <summary>
        /// A list of LevelAreaSnoIdId's that the bot should always ignore Line of Sight
        /// </summary>
        public static HashSet<int> NeverRaycastLevelAreaIds { get; } = new HashSet<int>()
        {
            405915, // p1_TieredRift_Challenge
        };

        public static HashSet<int> NeverTownPortalLevelAreaIds { get; } = new HashSet<int>()
        {
            202446, // A1 New Tristram "Attack Area"
            //19947, // A1 New Tristram "Attack Area"
            175738, // A4 Bounty Quest with moving floor panels.
            284069, // A5 Westmarch Overlook
            308323, // A5 Westmarch Wolf Gate
            315938, // A5 Westmarch Wolf Gate
            316374, // A5 Westmarch Storehouse
            311624, // A5 Westmarch Cathedral Courtyard
            311623, // A5 Streets of Westmarch
            309413, // A5 Westmarch Cathedral
        };

        public static HashSet<int> NoCheckKillRange { get; } = new HashSet<int>
        {
            210120, // A4 Corrupt Growth
            210268, // A4 Corrupt Growth
        };

        /// <summary>
        /// Size of the navigation obstacles above (actual SNO list must be matching the above list!)
        /// </summary>
        public static Dictionary<int, float> ObstacleCustomRadius { get; } = new Dictionary<int, float>
        {
            {174900, 25}, {194682, 20}, {81699, 20}, {3340, 12}, {123325, 25}, {185391, 25},
            {104596, 15}, // trOut_FesteringWoods_Neph_Column_B
            {104632, 15}, // trOut_FesteringWoods_Neph_Column_B_Broken_Base
            {105303, 15}, // trOut_FesteringWoods_Neph_Column_C_Broken_Base_Bottom
            {104827, 15}, // trOut_FesteringWoods_Neph_Column_C_Broken_Base
            {355898, 12}, // x1_Bog_Family_Guard_Tower_Stump
            {376917, 10},
            {293900, 20f}, // Azmo Blocking Shit

            // DyingHymn A4 Bounties
            {433402, 8},
            {434971, 10},
        };

        public static HashSet<SNOLevelArea> PandemoniumFortressLevelAreaIds { get; } = new HashSet<SNOLevelArea>
        {
            SNOLevelArea.X1_LR_Tileset_Fortress
        };

        public static HashSet<SNOWorld> PandemoniumFortressWorlds { get; } = new HashSet<SNOWorld>
        {
            SNOWorld.x1_fortress_level_01,
            SNOWorld.x1_fortress_level_02,
        };

        public static HashSet<int> QuestLevelAreaIds { get; } = new HashSet<int>
        {
            202446, // A1 New Tristram "Attack Area"
            19947, // A1 New Tristram
            109457, // A1 New Tristram Inn
            109457, // A1 The Slaughtered Calf Inn
            62968, // A1 The Hidden Cellar
            60714, // A1 Leoric's Passage
            83110, // A1 Cellar of the Damned
            19935, // A1 Wortham
            100854, // A1 Khazra Den
            94672, // A1 Cursed Hold

            60757, // A2 Belial's chambers
            55313, // A2 Caldeum Bazaar
            102964, // A2 City of Caldeum

            309413, // A5 Westmarch Cathedral

            336846, // x1_westm_KingEvent01 - Westmarch Commons Contested Villa
            405915, // p1_TieredRift_Challenge
        };

        /// <summary>
        /// Contains ActorSnoId of ranged units that should be attacked even if outside of kill radius
        /// </summary>
        public static HashSet<int> RangedMonsterIds { get; } = new HashSet<int>
        {
            365, 4100, // fallen
            4304, 4300, // goat shaman
            4738, // pestilence
            4299, // goat ranged
            62736, 130794, // demon flyer
            5508, // succubus
            5388, 4286, 256015, 256000, 255996,
            5984, 5985, 5987, 5988, 405186, //goblins
        };

        public static HashSet<SNOActor> RayWalkTargetingOnlyActors { get; } = new HashSet<SNOActor>
        {
            SNOActor.X1_Pand_Ext_Ordnance_Tower_Shock_A,
        };

        /// <summary>
        /// Resplendent chest SNO list
        /// </summary>
        public static HashSet<SNOActor> ResplendentChestIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.CaOut_Oasis_Chest_Rare,
            SNOActor.a1dun_Leor_Chest_Rare,
            SNOActor.trDun_Chest_Rare,
            SNOActor.caOut_StingingWinds_Chest_Rare,
            SNOActor.A3_Battlefield_Chest_Rare_Snowy,
            SNOActor.A3_Battlefield_Chest_Rare_Frosty,
            SNOActor.Pinata,
            SNOActor.TrOut_Highlands_Chest_Rare,
            SNOActor.a3dun_Keep_Chest_Rare,
            SNOActor.TrOut_Highlands_Chest_Rare,
            SNOActor.TrOut_Fields_Chest_Rare,
            SNOActor.a1dun_Cath_chest_Rare,
            SNOActor.a2dun_Spider_Chest_Rare,
            SNOActor.x1_Global_Chest_StartsClean,
            SNOActor.x1_Global_Chest,
            SNOActor.x1_PandExt_Time_Activator,
            SNOActor.x1_Global_Chest_BossBounty
        };

        /// <summary>
        /// Contains all the Exit Name Hashes in Rifts
        /// </summary>
        public static List<int> RiftPortalHashes { get; } = new List<int>()
        {
            1938876094,
            1938876095,
            1938876096,
            1938876097,
            1938876098,
            1938876099,
            1938876100,
            1938876101,
            1938876102,
        };

        // PlayerHeadstone
        /// <summary>
        /// Contains a list of Rift WorldId's
        /// </summary>
        public static List<SNOWorld> RiftWorldIds { get; } = new List<SNOWorld>()
        {
            SNOWorld.X1_LR_Level_01,
            SNOWorld.X1_LR_Level_02,
            SNOWorld.X1_LR_Level_03,
            SNOWorld.X1_LR_Level_04,
            SNOWorld.X1_LR_Level_05,
            SNOWorld.X1_LR_Level_06,
            SNOWorld.X1_LR_Level_07,
            SNOWorld.X1_LR_Level_08,
            SNOWorld.X1_LR_Level_09,
            SNOWorld.X1_LR_Level_10,
            (SNOWorld)288806,
        };

        // De-weight to zero but they still need to be processed for ClsoedDoor checks. todo proper fix.
        public static Dictionary<SNOScene, SNOActor> SceneSpecificDoorsIgnore { get; } = new Dictionary<SNOScene, SNOActor>
        {
            { SNOScene.x1_Catacombs_NSE_01_Valor, SNOActor.p43_AD_Catacombs_Door_A },              // p43_AD_Catacombs_Door_A-13146 (454346)  // D1 Valor Scene Event
            { SNOScene.a2dun_Aqd_NSE_Vault_01, SNOActor.a2dun_Aqd_GodHead_Door_LargePuzzle },   // a2dun_Aqd_GodHead_Door_LargePuzzle  // Bounty: Lost Treasure of Khan Dakab
        };

        /// <summary>
        /// A list of crappy "summoned mobs" we should always ignore unless they are very close to us, eg "grunts", summoned skeletons etc.
        /// </summary>
        public static HashSet<int> ShortRangeAttackMonsterIds { get; } = new HashSet<int>
        {
            4084, 4085, 5395, 144315,
        };

        /// <summary>
        /// A list of LevelAreaSnoIdId's that the bot should always use Straight line pathing (no navigator)
        /// </summary>
        public static HashSet<int> StraightLinePathingLevelAreaIds { get; } = new HashSet<int>
        {
            60757, // Belial's chambers
            405915, // p1_TieredRift_Challenge
        };

        //monsterAffix_waller_model (226808)
        public static HashSet<string> VanityItems { get; } = new HashSet<string>()
        {
            "x1_AngelWings_Imperius", // Wings of Valor
            "X1_SpectralHound_Skull_promo", // Liber Canis Mortui
            "WoDFlag", // Warsong Pennant
        };

        public static HashSet<string> WhiteItemCraftingWhiteList { get; } = new HashSet<string>()
        {
            "Ascended Pauldrons",
            "Ascended Armor",
            "Ascended Bracers",
            "Ascended Crown",
            "Ascended Pauldrons",
            "Archon Sash",
            "Ascended Faulds",
            "Ascended Greaves",
            "Ascended Gauntlets",

            "Limb Cleaver",
            "Doubleshot",
            "Whirlwind Staff",
            "Flesh Render",
            "Penetrator",
            "Ascended Shield",
            "Punyal",
            "Dire Axe",
            "Tsunami Blade",
            "Kerykeion",
            "Steppes Smasher",
            "Grandfather Flail",
            "Oxybeles",
            "Persuader",
            "Skullsplitter",
            "Suwaiya",
            "Tecpatl",
            "Diabolic Wand"
        };

        public static HashSet<SNOActor> WizardHydraIds { get; } = new HashSet<SNOActor>
        {
            // Some hydras are 3 monsters, only use one of their heads.
            SNOActor.Wizard_HydraHead_Frost_1,
            SNOActor.Wizard_HydraHead_Big,
            SNOActor.Wizard_HydraHead_fire2_1,
            SNOActor. Wizard_HydraHead_Lightning_1,
            SNOActor. Wizard_HydraHead_Arcane_1,
            SNOActor. Wizard_HydraHead_Default_1,
        };

        // Three special lists used purely for checking for the existance of a player's summoned mystic ally, gargantuan, or zombie dog
        public static HashSet<SNOActor> ZombieDogIds { get; } = new HashSet<SNOActor>
        {
            SNOActor.WD_ZombieDogRune_healthLink,
            SNOActor.WD_ZombieDogRune_lifeSteal,
            SNOActor.WD_ZombieDogRune_fire,
            SNOActor.WD_ZombieDogRune_healthGlobe,
            SNOActor.WD_ZombieDogRune_poison,
            SNOActor.WD_ZombieDog,
        };

        /// <summary>
        /// Add an ActorSnoId to the blacklist. Returns false if the blacklist already contains the ActorSnoId
        /// </summary>
        /// <param name="actorId"></param>
        /// <returns></returns>
        public static bool AddToBlacklist(SNOActor actorId)
        {
            if (!BlackListIds.Contains(actorId))
            {
                BlackListIds.Add(actorId);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// The SNOAnim enum is so large that preprocessing to a dictionary has a significant performance improvement
        /// </summary>
        public static string GetAnimationNameLowerCase(SNOAnim anim)
        {
            if (anim == SNOAnim.Invalid)
                return string.Empty;

            AnimatioNamesById.TryGetValue((int)anim, out var result);

            return result ?? string.Empty;
        }

        public static class ItemGameBalanceIds
        {
            public const int ArcaneDust = -1585802162;
            public const int DeathsBreath = 2087837753;
            public const int ForgottenSoul = 2073430088;
            public const int ReusableParts = -363607620;
            public const int VeiledCrystal = -605947593;
        }

        /*
         * Combat-related dictionaries/defaults
         */
    }
}
