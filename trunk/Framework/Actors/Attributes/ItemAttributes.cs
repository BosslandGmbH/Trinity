using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Objects.Memory.Attributes;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Actors.Attributes
{    
    public class ItemAttributes : Objects.Memory.Attributes.Attributes
    {
        public ItemAttributes() { }
        public ItemAttributes(int groupId) : base(groupId) { }

        public List<int> ItemTradePlayerLow => GetCachedAttributes<int>(ActorAttributeType.ItemTradePlayerLow);
        public List<int> ItemTradePlayerHigh => GetCachedAttributes<int>(ActorAttributeType.ItemTradePlayerHigh);

        public Dictionary<DamageType, float> DamageWeaponPercentTotals => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponPercentTotal); // DamageWeaponPercentTotal (240) = i:1035489772 f:0.09 v:0.09 ModifierType=DamageType Modifier=0 
        public Dictionary<SNOPower, float> Skills => GetCachedAttributes<SNOPower, float>(ActorAttributeType.Skill); // Skill (125304932) = i:1 f:1.401298E-45 v:1 ModifierType=PowerSnoId Modifier=30592 
        public Dictionary<SNOPower, float> ItemPowerPassives => GetCachedAttributes<SNOPower, float>(ActorAttributeType.ItemPowerPassive); // ItemPowerPassive (1648481537) = i:1061326684 f:0.76 v:0.76 ModifierType=PowerSnoId Modifier=402461 
        public Dictionary<int, float> Requirements => GetCachedAttributes<int, float>(ActorAttributeType.Requirement); // Requirement (233853) = i:1116471296 f:70 v:70 ModifierType=1 Modifier=57 
        public Dictionary<DamageType, float> Resistances => GetCachedAttributes<DamageType, float>(ActorAttributeType.Resistance); // Resistance (8285) = i:1128333312 f:193 v:193 ModifierType=DamageType Modifier=2 
        public Dictionary<DamageType, float> DamageWeaponMins => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponMin); // DamageWeaponMin (16613) = i:1151246336 f:1269 v:1269 ModifierType=DamageType Modifier=4 
        public Dictionary<DamageType, float> DamageWeaponAverages => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponAverage); // DamageWeaponAverage (232) = i:1129709568 f:214 v:214 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageWeaponMinTotalMainHands => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponMinTotalMainHand); // DamageWeaponMinTotalMainHand (12822) = i:1155997696 f:1849 v:1849 ModifierType=DamageType Modifier=3 
        public Dictionary<DamageType, float> DamageWeaponAverageTotals => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponAverageTotal); // DamageWeaponAverageTotal (12521) = i:1156628480 f:1926 v:1926 ModifierType=DamageType Modifier=3 
        public Dictionary<SNOPower, float> PowerDamagePercentBonuses => GetCachedAttributes<SNOPower, float>(ActorAttributeType.PowerDamagePercentBonus); // PowerDamagePercentBonus (1332085920) = i:1038174126 f:0.11 v:0.11 ModifierType=PowerSnoId Modifier=325216 
        public Dictionary<int, int> SetItemCounts => GetCachedAttributes<int, int>(ActorAttributeType.SetItemCount); // SetItemCount (-1399073762) = i:1 f:1.401298E-45 v:1 ModifierType=17 Modifier=-341571 
        public Dictionary<DamageType, float> DamageDealtPercentBonuses => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageDealtPercentBonus); // DamageDealtPercentBonus (242) = i:1044549468 f:0.19 v:0.19 ModifierType=DamageType Modifier=0 
        public Dictionary<ResourceType, float> ResourceMaxBonuses => GetCachedAttributes<ResourceType, float>(ActorAttributeType.ResourceMaxBonus); // ResourceMaxBonus (28823) = i:1088421888 f:7 v:7 ModifierType=ResourceType Modifier=7 
        public Dictionary<DamageType, float> DamageWeaponDeltas => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponDelta); // DamageWeaponDelta (220) = i:1129709568 f:214 v:214 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageWeaponDeltaSubTotals => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponDeltaSubTotal); // DamageWeaponDeltaSubTotal (221) = i:1129709568 f:214 v:214 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageWeaponDeltaTotals => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponDeltaTotal); // DamageWeaponDeltaTotal (225) = i:1141891400 f:575.52 v:575.52 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageWeaponBonusDeltaX1s => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponBonusDeltaX1); // DamageWeaponBonusDeltaX1 (228) = i:1134362624 f:314 v:314 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageWeaponMinTotals => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponMinTotal); // DamageWeaponMinTotal (230) = i:1152306955 f:1398.47 v:1398.47 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageWeaponBonusMinX1s => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponBonusMinX1); // DamageWeaponBonusMinX1 (236) = i:1150484480 f:1176 v:1176 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageDeltaTotals => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageDeltaTotal); // DamageDeltaTotal (211) = i:1120927744 f:104 v:104 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageMins => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageMin); // DamageMin (212) = i:1119354880 f:92 v:92 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageMinTotals => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageMinTotal); // DamageMinTotal (214) = i:1119354880 f:92 v:92 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageMinSubtotals => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageMinSubtotal); // DamageMinSubtotal (218) = i:1119354880 f:92 v:92 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageWeaponDeltaTotalMainHands => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponDeltaTotalMainHand); // DamageWeaponDeltaTotalMainHand (16920) = i:1132986368 f:272 v:272 ModifierType=DamageType Modifier=4 
        public Dictionary<DamageType, float> DamageDeltas => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageDelta); // DamageDelta (210) = i:1120927744 f:104 v:104 ModifierType=DamageType Modifier=0 
        public Dictionary<DamageType, float> DamageWeaponMaxTotals => GetCachedAttributes<DamageType, float>(ActorAttributeType.DamageWeaponMaxTotal); // 223: DamageWeaponMaxTotal(-3873) [DamageType: Poison: 4 ]  i:0 f:1825 Value=1825 


        public float DamageWeaponMin => GetFirstCachedAttribute<float>(ActorAttributeType.DamageWeaponMin); // DamageWeaponMin (16613) = i:1151246336 f:1269 v:1269 ModifierType=DamageType Modifier=4 
        public float DamageWeaponDelta => GetFirstCachedAttribute<float>(ActorAttributeType.DamageWeaponDelta); // DamageWeaponDelta (220) = i:1129709568 f:214 v:214 ModifierType=DamageType Modifier=0 
        public float DamageWeaponMaxTotal => GetFirstCachedAttribute<float>(ActorAttributeType.DamageWeaponMaxTotal); // 223: DamageWeaponMaxTotal(-3873) [DamageType: Poison: 4 ]  i:0 f:1825 Value=1825 
        public float DamageDealtPercentBonus => GetFirstCachedAttribute<float>(ActorAttributeType.DamageDealtPercentBonus); // DamageDealtPercentBonus (242) = i:1044549468 f:0.19 v:0.19 ModifierType=DamageType Modifier=0 
        public float DamageWeaponPercentTotal => GetFirstCachedAttribute<float>(ActorAttributeType.DamageWeaponPercentTotal); // DamageWeaponPercentTotal (240) = i:1035489772 f:0.09 v:0.09 ModifierType=DamageType Modifier=0 
        public float DamageWeaponDeltaTotalMainHand => GetFirstCachedAttribute<float>(ActorAttributeType.DamageWeaponDeltaTotalMainHand); // DamageWeaponDeltaTotalMainHand (16920) = i:1132986368 f:272 v:272 ModifierType=DamageType Modifier=4 
        public float DamageWeaponMinTotal => GetFirstCachedAttribute<float>(ActorAttributeType.DamageWeaponMinTotal); // DamageWeaponMinTotal (230) = i:1152306955 f:1398.47 v:1398.47 ModifierType=DamageType Modifier=0 
        public float DamageWeaponBonusMinX1 => GetFirstCachedAttribute<float>(ActorAttributeType.DamageWeaponBonusMinX1); // 236: DamageWeaponBonusMinX1(-3860) ModKey=236 Mod=0 ModifierType=DamageType Value = 1591
        public float DamageWeaponBonusDeltaX1 => GetFirstCachedAttribute<float>(ActorAttributeType.DamageWeaponBonusDeltaX1); //228: DamageWeaponBonusDeltaX1(-3868) ModKey=228 Mod=0 ModifierType=DamageType Value = 385

        public float PotionBonusLifeOnKill => GetCachedAttribute<float>(ActorAttributeType.PotionBonusLifeOnKill); // PotionBonusLifeOnKill (-2735) = i:1194553856 f:45938 v:45938 ModifierType=None Modifier=-1 
        public float CritPercentBonusCapped => GetCachedAttribute<float>(ActorAttributeType.CritPercentBonusCapped); // CritPercentBonusCapped (-3850) = i:1036831949 f:0.1 v:0.1 ModifierType=None Modifier=-1 
        public float IntelligenceItem => GetCachedAttribute<float>(ActorAttributeType.IntelligenceItem); // IntelligenceItem (-2898) = i:1147863040 f:940 v:940 ModifierType=None Modifier=-1 
        public float ArmorItemTotal => GetCachedAttribute<float>(ActorAttributeType.ArmorItemTotal); // ArmorItemTotal (-4059) = i:1141850112 f:573 v:573 ModifierType=None Modifier=-1 
        public float ArmorItemSubTotal => GetCachedAttribute<float>(ActorAttributeType.ArmorItemSubTotal); // ArmorItemSubTotal (-4060) = i:1141850112 f:573 v:573 ModifierType=None Modifier=-1 
        public int ArmorItem => GetCachedAttribute<int>(ActorAttributeType.ArmorItem); // ArmorItem (-4063) = i:1141855573 f:573.3333 v:573.3333 ModifierType=None Modifier=-1 
        public int Vitality => GetCachedAttribute<int>(ActorAttributeType.VitalityItem); // VitalityItem (-2897) = i:1147142144 f:896 v:896 ModifierType=None Modifier=-1 
        public int Strength => GetCachedAttribute<int>(ActorAttributeType.StrengthItem); // StrengthItem (-2900) = i:1148354560 f:970 v:970 ModifierType=None Modifier=-1 
        public int Intelligence => GetCachedAttribute<int>(ActorAttributeType.IntelligenceItem);
        public int Dexterity => GetCachedAttribute<int>(ActorAttributeType.DexterityItem);
        public GemQuality GemQuality => GetCachedAttribute<GemQuality>(ActorAttributeType.GemQuality);
        public float BlockAmountItemDelta => GetCachedAttribute<float>(ActorAttributeType.BlockAmountItemDelta); // BlockAmountItemDelta (-3825) = i:1171529728 f:6788 v:6788 ModifierType=None Modifier=-1 
        public float BlockAmountItemMin => GetCachedAttribute<float>(ActorAttributeType.BlockAmountItemMin); // BlockAmountItemMin (-3826) = i:1185865728 f:22384 v:22384 ModifierType=None Modifier=-1 
        public float BlockChanceItemTotal => GetCachedAttribute<float>(ActorAttributeType.BlockChanceItemTotal); // BlockChanceItemTotal (-3833) = i:1047904256 f:0.2399902 v:0.2399902 ModifierType=None Modifier=-1 
        public float BlockChanceItem => GetCachedAttribute<float>(ActorAttributeType.BlockChanceItem); // BlockChanceItem (-3834) = i:1040515072 f:0.1298828 v:0.1298828 ModifierType=None Modifier=-1 
        public float BlockChanceBonusItem => GetCachedAttribute<float>(ActorAttributeType.BlockChanceBonusItem); // BlockChanceBonusItem (-3835) = i:1038172160 f:0.1099854 v:0.1099854 ModifierType=None Modifier=-1 
        public float ResourceCostReductionPercentAll => GetCachedAttribute<float>(ActorAttributeType.ResourceCostReductionPercentAll); // ResourceCostReductionPercentAll (-3364) = i:1028440064 f:0.04998779 v:0.04998779 ModifierType=None Modifier=-1 
        public float AttacksPerSecondPercent => GetCachedAttribute<float>(ActorAttributeType.AttacksPerSecondPercent); // AttacksPerSecondPercent (-3895) = i:1032805417 f:0.07 v:0.07 ModifierType=None Modifier=-1 
        public bool IsAncient => GetAttribute<bool>(ActorAttributeType.AncientRank); // AncientRank (-3691) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1 
        public bool IsUnidentified => GetAttribute<bool>(ActorAttributeType.Unidentified); // AncientRank (-3691) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1 
        public int Post212Drop2 => GetCachedAttribute<int>(ActorAttributeType.Post212Drop2); // Post212Drop2 (-3692) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1 
        public int Loot20Drop => GetCachedAttribute<int>(ActorAttributeType.Loot20Drop); // Loot20Drop (-3694) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1 
        public int Seed => GetCachedAttribute<int>(ActorAttributeType.Seed); // Seed (-3698) = i:-180189030 f:-2.466006E+32 v:-1.80189E+08 ModifierType=None Modifier=-1 
        public int RequiredLevel => GetCachedAttribute<int>(new AttributeKey((int)ActorAttributeType.Requirement, (int)RequirementType.EquipItem)); // ItemLegendaryItemLevelOverride (-3706) = i:70 f:9.809089E-44 v:70 ModifierType=None Modifier=-1 

        public int ItemLegendaryItemLevelOverride => GetCachedAttribute<int>(ActorAttributeType.ItemLegendaryItemLevelOverride);
        public int ItemBindingLevelOverride => GetCachedAttribute<int>(ActorAttributeType.ItemBindingLevelOverride); // ItemBindingLevelOverride (-3707) = i:2 f:2.802597E-45 v:2 ModifierType=None Modifier=-1 
        public int ItemBoundToAnnId => GetCachedAttribute<int>(ActorAttributeType.ItemBoundToACD); // ItemBoundToACD (-3709) = i:2014707801 f:1.216956E+34 v:2.014708E+09 ModifierType=None Modifier=-1 
        public int Sockets => GetCachedAttribute<int>(ActorAttributeType.Sockets); // Sockets (-3712) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1 
        public ItemQuality ItemQualityLevel => GetCachedAttribute<ItemQuality>(ActorAttributeType.ItemQualityLevel); // ItemQualityLevel (-3720) = i:9 f:1.261169E-44 v:9 ModifierType=None Modifier=-1 
        public int DurabilityMax => GetAttribute<int>(ActorAttributeType.DurabilityMax); // DurabilityMax (-3722) = i:286 f:4.007714E-43 v:286 ModifierType=None Modifier=-1 
        public int DurabilityCur => GetAttribute<int>(ActorAttributeType.DurabilityCur); // DurabilityCur (-3723) = i:286 f:4.007714E-43 v:286 ModifierType=None Modifier=-1 
        public float HitpointsOnHit => GetCachedAttribute<float>(ActorAttributeType.HitpointsOnHit); // HitpointsOnHit (-3751) = i:1176764416 f:10496 v:10496 ModifierType=None Modifier=-1 
        public int ItemStackQuantityLo => GetAttribute<int>(ActorAttributeType.ItemStackQuantityLo); // ItemStackQuantityLo (-3702) = i:5000 f:7.006492E-42 v:5000 ModifierType=None Modifier=-1 
        public int JewelRank => GetAttribute<int>(ActorAttributeType.JewelRank); // JewelRank (-2707) = i:50 f:7.006492E-44 v:50 ModifierType=None Modifier=-1 
        public float OnHitFearProcChance => GetCachedAttribute<float>(ActorAttributeType.OnHitFearProcChance); // OnHitFearProcChance (-2991) = i:1011769344 f:0.01259613 v:0.01259613 ModifierType=None Modifier=-1 
        public float HitpointsOnKill => GetCachedAttribute<float>(ActorAttributeType.HitpointsOnKill); // HitpointsOnKill (-3750) = i:1172201472 f:7116 v:7116 ModifierType=None Modifier=-1 
        public float MovementScalar => GetCachedAttribute<float>(ActorAttributeType.MovementScalar); // MovementScalar (-3927) = i:1038172160 f:0.1099854 v:0.1099854 ModifierType=None Modifier=-1 
        public int ItemLegendaryItemBaseItem => GetCachedAttribute<int>(ActorAttributeType.ItemLegendaryItemBaseItem); // ItemLegendaryItemBaseItem (-2746) = i:1146967350 f:885.3314 v:1.146967E+09 ModifierType=None Modifier=-1 
        public float DamageWeaponPercentAll => GetCachedAttribute<float>(ActorAttributeType.DamageWeaponPercentAll); // DamageWeaponPercentAll (-3857) = i:1035489772 f:0.09 v:0.09 ModifierType=None Modifier=-1 
        public float DamageWeaponMinTotalAll => GetCachedAttribute<float>(ActorAttributeType.DamageWeaponMinTotalAll); // DamageWeaponMinTotalAll (-3865) = i:1152306955 f:1398.47 v:1398.47 ModifierType=None Modifier=-1 
        public float DamageWeaponDeltaTotalAll => GetCachedAttribute<float>(ActorAttributeType.DamageWeaponDeltaTotalAll); // DamageWeaponDeltaTotalAll (-3870) = i:1141891400 f:575.52 v:575.52 ModifierType=None Modifier=-1 
        public float AttacksPerSecondItemTotal => GetCachedAttribute<float>(ActorAttributeType.AttacksPerSecondItemTotal); // AttacksPerSecondItemTotal (-3900) = i:1069547520 f:1.5 v:1.5 ModifierType=None Modifier=-1 
        public float AttacksPerSecondItemSubtotal => GetCachedAttribute<float>(ActorAttributeType.AttacksPerSecondItemSubtotal); // AttacksPerSecondItemSubtotal (-3902) = i:1069547520 f:1.5 v:1.5 ModifierType=None Modifier=-1 
        public float AttacksPerSecondItem => GetCachedAttribute<float>(ActorAttributeType.AttacksPerSecondItem); // AttacksPerSecondItem (-3904) = i:1069547520 f:1.5 v:1.5 ModifierType=None Modifier=-1 
        public float AttacksPerSecondItemMainHand => GetCachedAttribute<float>(ActorAttributeType.AttacksPerSecondItemMainHand); // AttacksPerSecondItemMainHand (-3566) = i:1069547520 f:1.5 v:1.5 ModifierType=None Modifier=-1 
        public float DamageWeaponAverageTotalAll => GetCachedAttribute<float>(ActorAttributeType.DamageWeaponAverageTotalAll); // DamageWeaponAverageTotalAll (-3862) = i:1154113536 f:1619 v:1619 ModifierType=None Modifier=-1 
        public float DamageWeaponMaxTotalAll => GetCachedAttribute<float>(ActorAttributeType.DamageWeaponMaxTotalAll); // DamageWeaponMaxTotalAll (-3872) = i:1156104192 f:1862 v:1862 ModifierType=None Modifier=-1 
        public float SplashDamageEffectPercent => GetCachedAttribute<float>(ActorAttributeType.SplashDamageEffectPercent); // SplashDamageEffectPercent (-2754) = i:1047233823 f:0.23 v:0.23 ModifierType=None Modifier=-1 
        public float AttacksPerSecondItemTotalMainHand => GetCachedAttribute<float>(ActorAttributeType.AttacksPerSecondItemTotalMainHand); // AttacksPerSecondItemTotalMainHand (-3564) = i:1069547520 f:1.5 v:1.5 ModifierType=None Modifier=-1 
        public float HealthGlobeBonusHealth => GetCachedAttribute<float>(ActorAttributeType.HealthGlobeBonusHealth); // HealthGlobeBonusHealth (-4010) = i:1191871744 f:35461 v:35461 ModifierType=None Modifier=-1 
        public float GoldPickUpRadius => GetCachedAttribute<float>(ActorAttributeType.GoldPickUpRadius); // GoldPickUpRadius (-3032) = i:1073741824 f:2 v:2 ModifierType=None Modifier=-1 
        public int ItemStackQuantityHi => GetAttribute<int>(ActorAttributeType.ItemStackQuantityHi);
        public float WeaponOnHitBlindProcChanceMainHand => GetCachedAttribute<float>(ActorAttributeType.WeaponOnHitBlindProcChanceMainHand); // WeaponOnHitBlindProcChanceMainHand (-2948) = i:1050132480 f:0.2963867 v:0.2963867 ModifierType=None Modifier=-1 
        public float WeaponOnHitBlindProcChance => GetCachedAttribute<float>(ActorAttributeType.WeaponOnHitBlindProcChance); // WeaponOnHitBlindProcChance (-2966) = i:1050132480 f:0.2963867 v:0.2963867 ModifierType=None Modifier=-1 
        public float PowerCooldownReductionPercentAll => GetCachedAttribute<float>(ActorAttributeType.PowerCooldownReductionPercentAll); // PowerCooldownReductionPercentAll (-3888) = i:1032798208 f:0.06994629 v:0.06994629 ModifierType=None Modifier=-1 
        public float SpendingResourceHealsPercent => GetCachedAttribute<float>(ActorAttributeType.SpendingResourceHealsPercent); // SpendingResourceHealsPercent (28745) = i:1162813440 f:3314 v:3314 ModifierType=ResourceType Modifier=7 
        public int TieredLootRunKeyLevel => GetCachedAttribute<int>(ActorAttributeType.TieredLootRunKeyLevel); // TieredLootRunKeyLevel (-2712) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1 
        public float DamageAverageTotalAll => GetCachedAttribute<float>(ActorAttributeType.DamageAverageTotalAll); // DamageAverageTotalAll (-3879) = i:1125122048 f:144 v:144 ModifierType=None Modifier=-1 
        public float DamageMinTotalAll => GetCachedAttribute<float>(ActorAttributeType.DamageMinTotalAll); // DamageMinTotalAll (-3880) = i:1119354880 f:92 v:92 ModifierType=None Modifier=-1 
        public float DamageDeltaTotalAll => GetCachedAttribute<float>(ActorAttributeType.DamageDeltaTotalAll); // DamageDeltaTotalAll (-3881) = i:1120927744 f:104 v:104 ModifierType=None Modifier=-1 
        public float PotionBonusBuffDuration => GetCachedAttribute<float>(ActorAttributeType.PotionBonusBuffDuration); // PotionBonusBuffDuration (-2737) = i:1084227584 f:5 v:5 ModifierType=None Modifier=-1 
        public float PotionBonusArmorPercent => GetCachedAttribute<float>(ActorAttributeType.PotionBonusArmorPercent); // PotionBonusArmorPercent (-2740) = i:1040522936 f:0.13 v:0.13 ModifierType=None Modifier=-1 
        public float WeaponOnHitStunProcChanceMainHand => GetCachedAttribute<float>(ActorAttributeType.WeaponOnHitStunProcChanceMainHand); // WeaponOnHitStunProcChanceMainHand (-2951) = i:1019510784 f:0.02398682 v:0.02398682 ModifierType=None Modifier=-1 
        public float WeaponOnHitStunProcChance => GetCachedAttribute<float>(ActorAttributeType.WeaponOnHitStunProcChance); // WeaponOnHitStunProcChance (-2967) = i:1019510784 f:0.02398682 v:0.02398682 ModifierType=None Modifier=-1 
        public float WeaponOnHitFreezeProcChanceMainHand => GetCachedAttribute<float>(ActorAttributeType.WeaponOnHitFreezeProcChanceMainHand); // WeaponOnHitFreezeProcChanceMainHand (-2945) = i:1014235136 f:0.01489258 v:0.01489258 ModifierType=None Modifier=-1 
        public float WeaponOnHitFreezeProcChance => GetCachedAttribute<float>(ActorAttributeType.WeaponOnHitFreezeProcChance); // WeaponOnHitFreezeProcChance (-2965) = i:1014235136 f:0.01489258 v:0.01489258 ModifierType=None Modifier=-1 
        public int DyeType => GetCachedAttribute<int>(ActorAttributeType.DyeType); // DyeType (-3695) = i:16 f:2.242078E-44 v:16 ModifierType=None Modifier=-1 
        public int ItemIndestructible => GetCachedAttribute<int>(ActorAttributeType.ItemIndestructible); // ItemIndestructible (-2893) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1   
        public int ItemLevelRequirementReduction => GetCachedAttribute<int>(ActorAttributeType.ItemLevelRequirementReduction); // ItemLevelRequirementReduction (-2896) = i:39 f:5.465064E-44 v:39 ModifierType=None Modifier=-1 
        public int ItemLevelRequirementOverride => GetCachedAttribute<int>(ActorAttributeType.ItemLevelRequirementOverride); // ItemLevelRequirementOverride (-2895) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1 
        public float WeaponRanged => GetCachedAttribute<float>(ActorAttributeType.WeaponRanged); // WeaponRanged (-3578) = i:1065353216 f:1 v:1 ModifierType=None Modifier=-1 
        public float Weapon2H => GetCachedAttribute<float>(ActorAttributeType.Weapon2H); // Weapon2H (-3580) = i:1065353216 f:1 v:1 ModifierType=None Modifier=-1         
        public float DamagePercentReductionFromMelee => GetCachedAttribute<float>(ActorAttributeType.DamagePercentReductionFromMelee); // DamagePercentReductionFromMelee (-2979) = i:1032805416 f:0.06999999 v:0.06999999 ModifierType=None Modifier=-1    
        public bool IsVendorBought => GetCachedAttribute<bool>(ActorAttributeType.IsVendorBought); // IsVendorBought (-3696) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1 
        public float CubeEnchantedStrengthItem => GetCachedAttribute<float>(ActorAttributeType.CubeEnchantedStrengthItem); // CubeEnchantedStrengthItem (-3670) = i:1134723072 f:325 v:325 ModifierType=None Modifier=-1 
        public int CubeEnchantedGemType => GetCachedAttribute<int>(ActorAttributeType.CubeEnchantedGemType); // CubeEnchantedGemType (-3671) = i:3 f:4.203895E-45 v:3 ModifierType=None Modifier=-1 
        public int CubeEnchantedGemRank => GetCachedAttribute<int>(ActorAttributeType.CubeEnchantedGemRank); // CubeEnchantedGemRank (-3672) = i:65 f:9.10844E-44 v:65 ModifierType=None Modifier=-1 
        public int EnchantedAffixCount => GetCachedAttribute<int>(ActorAttributeType.EnchantedAffixCount); // EnchantedAffixCount (-3678) = i:9 f:1.261169E-44 v:9 ModifierType=None Modifier=-1 
        public int EnchantedAffixSeed => GetCachedAttribute<int>(ActorAttributeType.EnchantedAffixSeed); // EnchantedAffixSeed (-3679) = i:974478628 f:0.0005697778 v:9.744787E+08 ModifierType=None Modifier=-1 
        public int EnchantedAffixNew => GetCachedAttribute<int>(ActorAttributeType.EnchantedAffixNew); // EnchantedAffixNew (-3680) = i:-889190756 f:-8390300 v:-8.891908E+08 ModifierType=None Modifier=-1 
        public int EnchantedAffixOld => GetCachedAttribute<int>(ActorAttributeType.EnchantedAffixOld); // EnchantedAffixOld (-3681) = i:-415821016 f:-8.645464E+23 v:-4.15821E+08 ModifierType=None Modifier=-1 
        public int ConsumableAddSockets => GetCachedAttribute<int>(ActorAttributeType.ConsumableAddSockets); // ConsumableAddSockets (-3688) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1     
        public float HitpointsMaxPercentBonusItem => GetCachedAttribute<float>(ActorAttributeType.HitpointsMaxPercentBonusItem); // HitpointsMaxPercentBonusItem (-3961) = i:1040515072 f:0.1298828 v:0.1298828 ModifierType=None Modifier=-1 
        public bool IsCrafted => GetCachedAttribute<bool>(ActorAttributeType.IsCrafted); // IsCrafted (-3697) = i:1 f:1.401298E-45 v:1 ModifierType=None Modifier=-1               
        public float ResistanceAll => GetCachedAttribute<float>(ActorAttributeType.ResistanceAll); // ResistanceAll (-4000) = i:1120141312 f:98 v:98 ModifierType=None Modifier=-1 
        public float DamagePercentReductionFromRanged => GetCachedAttribute<float>(ActorAttributeType.DamagePercentReductionFromRanged); // DamagePercentReductionFromRanged (-2980) = i:1031127696 f:0.06 v:0.06 ModifierType=None Modifier=-1 
        public float ArmorBonusItem => GetCachedAttribute<float>(ActorAttributeType.ArmorBonusItem); // ArmorBonusItem (-4062) = i:1137082368 f:397 v:397 ModifierType=None Modifier=-1 
        public float AttacksPerSecondItemPercent => GetCachedAttribute<float>(ActorAttributeType.AttacksPerSecondItemPercent); // AttacksPerSecondItemPercent (-3903) = i:1031127695 f:0.06 v:0.06 ModifierType=None Modifier=-1 
        public float WeaponOnHitFearProcChance => GetCachedAttribute<float>(ActorAttributeType.WeaponOnHitFearProcChance); // WeaponOnHitFearProcChance (-2968) = i:1037590528 f:0.1056519 v:0.1056519 ModifierType=None Modifier=-1 
        public int ItemStackQuantity => (int)((long)ItemStackQuantityHi << 32 | (uint)ItemStackQuantityLo);
        public bool IsTradeable => GetCachedAttribute<bool>(ActorAttributeType.ItemTradeEndTime);
        public int SocketsFilled => GetCachedAttribute<int>(ActorAttributeType.SocketsFilled);
        public int ArcaneDamage => GetCachedAttribute<int>(ActorAttributeType.DamageWeaponMaxArcane);
        public int ColdDamage => GetCachedAttribute<int>(ActorAttributeType.DamageWeaponMaxCold);
        public int FireDamage => GetCachedAttribute<int>(ActorAttributeType.DamageWeaponMaxFire);
        public int HolyDamage => GetCachedAttribute<int>(ActorAttributeType.DamageWeaponMaxHoly);
        public int LightningDamage => GetCachedAttribute<int>(ActorAttributeType.DamageWeaponMaxLightning);
        public int PoisonDamage => GetCachedAttribute<int>(ActorAttributeType.DamageWeaponMaxPoison);
        public int PhysicalDamageTotal => GetCachedAttribute<int>(ActorAttributeType.DamageWeaponMaxTotalPhysical);
        public int PhysicalDamageBase => GetCachedAttribute<int>(ActorAttributeType.DamageWeaponMaxPhysical);
        public int PhysicalDamage => PhysicalDamageTotal - PhysicalDamageBase;
        public float DamageWeaponBonusMinPhysical => GetCachedAttribute<float>(ActorAttributeType.DamageWeaponBonusMinPhysical);
        public float DamageDeltaPhysical => GetCachedAttribute<float>(ActorAttributeType.DamageDeltaPhysical);
        public float LifeOnHit => GetCachedAttribute<float>(ActorAttributeType.HitpointsOnHit);
        public float LifePercent => GetCachedAttribute<float>(ActorAttributeType.HitpointsMaxPercentBonusItem);
        public float LifeStealPercent => GetCachedAttribute<float>(ActorAttributeType.StealHealthPercent) * 100;
        public float HealthPerSecond => GetCachedAttribute<float>(ActorAttributeType.HitpointsRegenPerSecond);
        public float MagicFindPercent => GetCachedAttribute<float>(ActorAttributeType.MagicFind) * 100;
        public float GoldFindPercent => GetCachedAttribute<float>(ActorAttributeType.GoldFind) * 100;
        public int MovementSpeedPercent => (int)Math.Round(GetCachedAttribute<float>(ActorAttributeType.MovementScalar) * 100);
        public float PickUpRadius => GetCachedAttribute<float>(ActorAttributeType.GoldPickUpRadius);
        public float CritPercent => GetCachedAttribute<float>(ActorAttributeType.CritPercentBonusCapped) * 100;
        public float CritDamagePercent => GetCachedAttribute<float>(ActorAttributeType.CritDamagePercent) * 100;
        public float BlockChancePercent => GetCachedAttribute<float>(ActorAttributeType.BlockChanceItemTotal) * 100;
        public float Thorns => GetCachedAttribute<float>(ActorAttributeType.ThornsFixedPhysical);
        public float ResourceCostReduction => GetCachedAttribute<float>(ActorAttributeType.ResourceCostReductionPercentAll) * 100;
        public long ItemAssignedHero => (long)ItemAssignedHeroHi << 32 | (uint)ItemAssignedHeroLow;
        public int ItemAssignedHeroHi => GetCachedAttribute<int>(ActorAttributeType.ItemAssignedHeroHigh);
        public int ItemAssignedHeroLow => GetCachedAttribute<int>(ActorAttributeType.ItemAssignedHeroLow); // ItemAssignedHeroLow (-2720) = i:66795861 f:1.476562E-36 v:6.679586E+07 ModifierType=None Modifier=-1 
        public int GoldAmount => GetCachedAttribute<int>(ActorAttributeType.Gold); // Gold (-4047) = i:3717 f:5.208626E-42 v:3717 ModifierType=None Modifier=-1 
        public float MinDamageTotal => DamageWeaponMinTotalAll;
        public float MaxDamageTotal => DamageWeaponMaxTotalAll;
        public float AttackSpeed => AttacksPerSecondItemTotal;
        public float DamagePerSecond => DamageWeaponAverageTotalAll * AttacksPerSecondItemTotal;
        public float WeaponDamagePercent => DamageWeaponPercentTotal * 100;
        public float AttackSpeedBonusPercent => AttacksPerSecondPercent * 100;
        public int ResourceCostReductionPercent => (int)Math.Round(ResourceCostReductionPercentAll * 100);
        public float ResistAll => GetCachedAttribute<float>(ActorAttributeType.ResistanceAll);
        public float ResistArcane => GetCachedAttribute<float>(ActorAttributeType.ResistanceArcane);
        public float ResistCold => GetCachedAttribute<float>(ActorAttributeType.ResistanceCold);
        public float ResistFire => GetCachedAttribute<float>(ActorAttributeType.ResistanceFire);
        public float ResistHoly => GetCachedAttribute<float>(ActorAttributeType.ResistanceHoly);
        public float ResistLightning => GetCachedAttribute<float>(ActorAttributeType.ResistanceLightning);
        public float ResistPhysical => GetCachedAttribute<float>(ActorAttributeType.ResistancePhysical);
        public float ResistPoison => GetCachedAttribute<float>(ActorAttributeType.ResistancePoison);
        public float AreaDamagePercent => SplashDamageEffectPercent * 100;
        public int CooldownPercent => (int)Math.Round(PowerCooldownReductionPercentAll * 100);
        public float FireSkillDamagePercentBonus => GetCachedAttribute<float>(ActorAttributeType.DamageDealtPercentBonusFire) * 100;
        public float ColdSkillDamagePercentBonus => GetCachedAttribute<float>(ActorAttributeType.DamageDealtPercentBonusCold) * 100;
        public float LightningSkillDamagePercentBonus => GetCachedAttribute<float>(ActorAttributeType.DamageDealtPercentBonusLightning) * 100;
        public float ArcaneSkillDamagePercentBonus => GetCachedAttribute<float>(ActorAttributeType.DamageDealtPercentBonusArcane) * 100;
        public float HolySkillDamagePercentBonus => GetCachedAttribute<float>(ActorAttributeType.DamageDealtPercentBonusHoly) * 100;
        public float PoisonSkillDamagePercentBonus => GetCachedAttribute<float>(ActorAttributeType.DamageDealtPercentBonusPoison) * 100;
        public float PhysicalSkillDamagePercentBonus => GetCachedAttribute<float>(ActorAttributeType.DamageDealtPercentBonusPhysical) * 100;
        public float DamageAgainstElites => GetCachedAttribute<float>(ActorAttributeType.DamagePercentBonusVsElites);
        public float DamageFromElites => GetCachedAttribute<float>(ActorAttributeType.DamagePercentReductionFromElites);
        public float WrathRegen => GetCachedAttribute<float>(ActorAttributeType.ResourceRegenPerSecondFaith);
        public float MaximumWrath => GetCachedAttribute<float>(ActorAttributeType.ResourceMaxBonusFaith);
        public float ChanceToFreeze => GetCachedAttribute<float>(ActorAttributeType.OnHitFreezeProcChance);
        public float ChanceToBlind => GetCachedAttribute<float>(ActorAttributeType.OnHitBlindProcChance);
        public float ChanceToImmobilize => GetCachedAttribute<float>(ActorAttributeType.OnHitImmobilizeProcChance);
        public float ChanceToStun => GetCachedAttribute<float>(ActorAttributeType.OnHitStunProcChance);
        public float MaxDiscipline => GetCachedAttribute<float>(ActorAttributeType.ResourceMaxPercentBonusDiscipline);
        public float MaxMana => GetCachedAttribute<float>(ActorAttributeType.ResourceMaxPercentBonusMana);
        public float MaxArcanePower => GetCachedAttribute<float>(ActorAttributeType.ResourceMaxPercentBonusArcanum);
        public float MaxSpirit => GetCachedAttribute<float>(ActorAttributeType.ResourceMaxPercentBonusSpirit);
        public float MaxFury => GetCachedAttribute<float>(ActorAttributeType.ResourceMaxPercentBonusFury);
        public float MaxWrath => GetCachedAttribute<float>(ActorAttributeType.ResourceMaxPercentBonusFaith);
        public float HatredRegen => GetCachedAttribute<float>(ActorAttributeType.ResourceRegenPerSecondHatred);
        public float SpiritRegen => GetCachedAttribute<float>(ActorAttributeType.ResourceRegenPerSecondSpirit);
        public float ManaRegen => GetCachedAttribute<float>(ActorAttributeType.ResourceRegenBonusPercentMana);
        public float LifePerSpirit => GetCachedAttribute<float>(ActorAttributeType.SpendingResourceHealsPercentSpirit);
        public float LifePerWrath => GetCachedAttribute<float>(ActorAttributeType.SpendingResourceHealsPercentFaith);
        public float LifePerFury => GetCachedAttribute<float>(ActorAttributeType.SpendingResourceHealsPercentFury);
        public float ArcaneOnCrit => GetCachedAttribute<float>(ActorAttributeType.ResourceOnCritArcanum);

        public int Gold => GetCachedAttribute<int>(ActorAttributeType.Gold);

        public int BlockChanceBonusPercent => (int)Math.Round(BlockChanceBonusItem * 100, MidpointRounding.AwayFromZero);

        public int PrimaryStat => GetPrimaryAttribute();

        public int GetPrimaryAttribute()
        {
            if (Strength > 0) return Strength;
            if (Intelligence > 0) return Intelligence;
            if (Dexterity > 0) return Dexterity;
            return 0;
        }

        public int GetElementalDamage(DamageType damageType)
        {
            var key = new AttributeKey((int)ActorAttributeType.DamageDealtPercentBonus, (int)damageType);
            return (int)Math.Round(GetCachedAttribute<float>(key) * 100, MidpointRounding.AwayFromZero);
        }

        public float SkillDamagePercent(SNOPower power)
        {
            var key = new AttributeKey((int)ActorAttributeType.PowerDamagePercentBonus, (int)power);
            return GetCachedAttribute<float>(key);
        }

        public float MaxDamage
        {
            get
            {
                var dmg = DamageWeaponBonusMinX1;
                if (Math.Abs(dmg) > float.Epsilon)
                {
                    return dmg + DamageWeaponBonusDeltaX1;
                }
                var max = DamageWeaponMaxTotal;
                if (Math.Abs(max) > float.Epsilon)
                {
                    return max;
                }
                return DamageWeaponMin + DamageWeaponDeltaTotalMainHand;
            }
        }
        public float MinDamage => DamageWeaponMaxTotal - DamageWeaponDelta;


        public string Summary()
        {
            return Items
                .Where(i => i.Value.Integer != 0 || i.Value.Single > float.Epsilon)
                .Where(i => !_attributeSummaryExclusions.Contains(i.Value.Attribute) && !_attributeSummaryExclusions.Contains(i.Value.Key.BaseAttribute))
                .Aggregate($"Attributes: ({Items.Count}): {Environment.NewLine}",
                (current, attr) => current + $"  	{attr.Value.Key.BaseAttribute} {((int)attr.Value.Modifer == -1 ? string.Empty : attr.Value.Modifer)} = {attr.Value.GetValue()} {Environment.NewLine}");
        }

        private readonly HashSet<ActorAttributeType> _attributeSummaryExclusions = new HashSet<ActorAttributeType>
        {
            ActorAttributeType.ItemBoundToACD,
            ActorAttributeType.PickedUpTime,
            ActorAttributeType.ItemBindingLevelOverride,
            ActorAttributeType.Seed,
            ActorAttributeType.Loot20Drop,
            ActorAttributeType.Post212Drop2,
            ActorAttributeType.ItemLegendaryItemLevelOverride,
            ActorAttributeType.ItemLegendaryItemBaseItem,
            ActorAttributeType.GizmoHasBeenOperated,
            ActorAttributeType.ItemTradePlayerLow,
            ActorAttributeType.ItemTradePlayerHigh,
            ActorAttributeType.IdentifyCost
        };

    }
}


