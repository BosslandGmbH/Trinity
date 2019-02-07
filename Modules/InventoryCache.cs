using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Modules
{
    public class InventoryCache : Module, IEnumerable<ACDItem>
    {
        private static readonly ILogger s_logger = Logger.GetLoggerInstanceForType();

        public InventoryCache()
        {
            GameEvents.OnGameJoined += (sender, args) => Update();
            GameEvents.OnWorldChanged += (sender, args) => Update();

            AllItems.Source = () => InventoryManager.AllItems.Where(i => !InvalidAnnIds.Contains(i.AnnId));
            Stash.Source = () => AllItems.Where(i => i.InventorySlot == InventorySlot.SharedStash);
        }

        public HashSet<SNOActor> KanaisCubeIds { get; private set; } = new HashSet<SNOActor>();
        public HashSet<SNOActor> PlayerEquippedIds { get; private set; } = new HashSet<SNOActor>();
        public HashSet<SNOActor> EquippedIds { get; private set; } = new HashSet<SNOActor>();
        public List<ACDItem> Equipped { get; private set; } = new List<ACDItem>();
        public HashSet<int> InvalidAnnIds { get; } = new HashSet<int>();
        public InventoryCurrency Currency { get; } = new InventoryCurrency();
        public InventorySlice AllItems { get; } = new InventorySlice();
        public InventorySlice Stash { get; } = new InventorySlice();
        public int BackpackItemCount { get; private set; }
        protected override void OnPulse()
        {
            Update();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AllItems.GetEnumerator();
        }

        public IEnumerator<ACDItem> GetEnumerator()
        {
            return AllItems.GetEnumerator();
        }

        public void Update()
        {
            /*
            if (!ZetaDia.IsInGame || ZetaDia.Storage.PlayerDataManager.ActivePlayerData == null)
                return;

            var kanaisCubeIds = new HashSet<SNOActor>(ZetaDia.Storage.PlayerDataManager.ActivePlayerData.KanaisPowersAssignedActorSnoIds);
            var equipped = new List<ACDItem>();
            var equippedIds = new HashSet<SNOActor>();
            var playerEquippedIds = new HashSet<SNOActor>();
            var backpackItemCount = 0;
            var stashItemCount = 0;

            foreach (var item in AllItems)
            {
                if (!item.IsValid)
                    continue;

                switch (item.InventorySlot)
                {
                    case InventorySlot.SharedStash:
                        stashItemCount++;
                        break;

                    case InventorySlot.BackpackItems:
                        backpackItemCount++;
                        break;

                    case InventorySlot.Bracers:
                    case InventorySlot.Feet:
                    case InventorySlot.Hands:
                    case InventorySlot.Head:
                    case InventorySlot.Waist:
                    case InventorySlot.Shoulders:
                    case InventorySlot.Torso:
                    case InventorySlot.LeftFinger:
                    case InventorySlot.RightFinger:
                    case InventorySlot.RightHand:
                    case InventorySlot.LeftHand:
                    case InventorySlot.Legs:
                    case InventorySlot.Neck:
                    case InventorySlot.Socket:
                        equipped.Add(item);
                        equippedIds.Add(item.ActorSnoId);
                        playerEquippedIds.Add(item.ActorSnoId);
                        break;
                }
            }

            foreach (var id in kanaisCubeIds)
                equippedIds.Add(id);

            KanaisCubeIds = kanaisCubeIds;
            Equipped = equipped;
            EquippedIds = equippedIds;
            PlayerEquippedIds = playerEquippedIds;
            BackpackItemCount = backpackItemCount;*/

            BackpackItemCount = Core.Actors.Inventory.Count(c => c.InventorySlot == InventorySlot.BackpackItems);
        }

        public void Clear()
        {
            InvalidAnnIds.Clear();
        }

        public class InventorySlice : IEnumerable<ACDItem>
        {
            public Func<IEnumerable<ACDItem>> Source { get; set; }
            public List<ACDItem> ByItemType(ItemType type)
            {
                return Source().Where(i => i.GetItemType() == type).ToList();
            }

            public List<ACDItem> ByActorSno(SNOActor actorSno)
            {
                return Source().Where(i => i.ActorSnoId == actorSno).ToList();
            }

            public List<ACDItem> ByQuality(TrinityItemQuality quality)
            {
                return Source().Where(i =>
{
    switch (quality)
    {
        case TrinityItemQuality.Invalid:
            return false;
        case TrinityItemQuality.None:
            return false;
        case TrinityItemQuality.Inferior:
            return i.ItemQualityLevel == ItemQuality.Inferior;
        case TrinityItemQuality.Common:
            return i.ItemQualityLevel >= ItemQuality.Normal && i.ItemQualityLevel <= ItemQuality.Superior;
        case TrinityItemQuality.Magic:
            return i.ItemQualityLevel >= ItemQuality.Magic1 && i.ItemQualityLevel <= ItemQuality.Magic3;
        case TrinityItemQuality.Rare:
            return i.ItemQualityLevel >= ItemQuality.Rare4 && i.ItemQualityLevel <= ItemQuality.Rare6;
        case TrinityItemQuality.Legendary:
            return i.ItemQualityLevel == ItemQuality.Legendary;
        case TrinityItemQuality.Set:
            return i.IsSetItem();
        default:
            throw new ArgumentOutOfRangeException(nameof(quality), quality, null);
    }
}).ToList();
            }

            public IEnumerator<ACDItem> GetEnumerator()
            {
                return Source().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return Source().GetEnumerator();
            }
        }

        public class InventoryCurrency
        {
            private PlayerData PlayerData => ZetaDia.Storage.PlayerDataManager.ActivePlayerData;

            public bool HasCurrency(TransmuteRecipe recipe)
            {
                switch (recipe)
                {
                    case TransmuteRecipe.ExtractLegendaryPower:
                        return HasCurrency(s_currencyRecipeExtractLegendaryPower);
                    case TransmuteRecipe.ReforgeLegendary:
                        return HasCurrency(s_currencyRecipeReforgeLegendary);
                    case TransmuteRecipe.UpgradeRareItem:
                        return HasCurrency(s_currencyRecipeUpgradeRareItem);
                    case TransmuteRecipe.ConvertSetItem:
                        return HasCurrency(s_currencyRecipeConvertSetItem);
                    case TransmuteRecipe.ConvertCraftingMaterialsFromNormal:
                        return HasCurrency(s_currencyRecipeConvertFromNormal);
                    case TransmuteRecipe.ConvertCraftingMaterialsFromMagic:
                        return HasCurrency(s_currencyRecipeConvertFromMagic);
                    case TransmuteRecipe.ConvertCraftingMaterialsFromRare:
                        return HasCurrency(s_currencyRecipeConvertFromRare);
                }

                return true; // recipes that dont require currency.
            }

            public bool HasCurrency(IDictionary<CurrencyType, int> recipe)
            {
                return recipe.All(requirement => PlayerData.GetCurrencyAmount(requirement.Key) >= requirement.Value);
            }

            /// <summary>
            /// Extracts a legendary power; requires legendary item.
            /// </summary>
            private static readonly Dictionary<CurrencyType, int> s_currencyRecipeExtractLegendaryPower = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.KhanduranRune, 1},
                {CurrencyType.CaldeumNightshade, 1},
                {CurrencyType.ArreatWarTapestry, 1},
                {CurrencyType.CorruptedAngelFlesh, 1},
                {CurrencyType.WestmarchHolyWater, 1},
                {CurrencyType.DeathsBreath, 5}
            };

            /// <summary>
            /// Produces random legendary; requires legendary item.
            /// </summary>
            private static readonly Dictionary<CurrencyType, int> s_currencyRecipeReforgeLegendary = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.KhanduranRune, 5},
                {CurrencyType.CaldeumNightshade, 5},
                {CurrencyType.ArreatWarTapestry, 5},
                {CurrencyType.CorruptedAngelFlesh, 5},
                {CurrencyType.WestmarchHolyWater, 5},
                {CurrencyType.ForgottenSoul, 50}
            };

            /// <summary>
            /// Produces legendary item; requires rare item.
            /// </summary>
            private static readonly Dictionary<CurrencyType, int> s_currencyRecipeUpgradeRareItem = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ReusableParts, 50},
                {CurrencyType.ArcaneDust, 50},
                {CurrencyType.VeiledCrystal, 50},
                {CurrencyType.DeathsBreath, 25}
            };

            /// <summary>
            /// Produces random set item; requires set item.
            /// </summary>
            private static readonly Dictionary<CurrencyType, int> s_currencyRecipeConvertSetItem = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ForgottenSoul, 10},
                {CurrencyType.DeathsBreath, 10}
            };

            /// <summary>
            /// Produces ReusableParts (requires normal item) -or- Veiled Crystals (requires rare item)
            /// </summary>
            private static readonly Dictionary<CurrencyType, int> s_currencyRecipeConvertFromMagic = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ArcaneDust, 100},
                {CurrencyType.DeathsBreath, 1}
            };

            /// <summary>
            /// Produces Arcane Dust (requires magic item) -or- Veiled Crystals (requires rare item)
            /// </summary>
            private static readonly Dictionary<CurrencyType, int> s_currencyRecipeConvertFromNormal = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ReusableParts, 100},
                {CurrencyType.DeathsBreath, 1}
            };

            /// <summary>
            /// Produces ReusableParts (requires normal item) -or- Arcane Dust (requires magic item)
            /// </summary>
            private static readonly Dictionary<CurrencyType, int> s_currencyRecipeConvertFromRare = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.VeiledCrystal, 100},
                {CurrencyType.DeathsBreath, 1}
            };

            public long GetCurrency(CurrencyType type)
            {
                return PlayerData.GetCurrencyAmount(type);
            }

            public long OrganDiablo => PlayerData.GetCurrencyAmount(CurrencyType.DemonOrganDiablo);
            public long OrganLeoric => PlayerData.GetCurrencyAmount(CurrencyType.DemonOrganSkeletonKing);
            public long OrganGhom => PlayerData.GetCurrencyAmount(CurrencyType.DemonOrganGhom);
            public long OrganSiegeBreaker => PlayerData.GetCurrencyAmount(CurrencyType.DemonOrganSiegeBreaker);
            public long OrganEye => PlayerData.GetCurrencyAmount(CurrencyType.DemonOrganEye);
            public long OrganTooth => PlayerData.GetCurrencyAmount(CurrencyType.DemonOrganTooth);
            public long OrganSpineCord => PlayerData.GetCurrencyAmount(CurrencyType.DemonOrganSpineCord);
            public long Gold => PlayerData.GetCurrencyAmount(CurrencyType.Gold);
            public long BloodShards => PlayerData.GetCurrencyAmount(CurrencyType.BloodShards);
            public long ArcaneDust => PlayerData.GetCurrencyAmount(CurrencyType.ArcaneDust);
            public long ReusableParts => PlayerData.GetCurrencyAmount(CurrencyType.ReusableParts);
            public long VeiledCrystals => PlayerData.GetCurrencyAmount(CurrencyType.VeiledCrystal);
            public long DeathsBreath => PlayerData.GetCurrencyAmount(CurrencyType.DeathsBreath);
            public long ForgottenSoul => PlayerData.GetCurrencyAmount(CurrencyType.ForgottenSoul);
            public long CaldeumNightshade => PlayerData.GetCurrencyAmount(CurrencyType.CaldeumNightshade);
            public long WestmarchHolyWater => PlayerData.GetCurrencyAmount(CurrencyType.WestmarchHolyWater);
            public long ArreatWarTapestry => PlayerData.GetCurrencyAmount(CurrencyType.ArreatWarTapestry);
            public long CorruptedAngelFlesh => PlayerData.GetCurrencyAmount(CurrencyType.CorruptedAngelFlesh);
            public long KhanduranRune => PlayerData.GetCurrencyAmount(CurrencyType.KhanduranRune);
        }
    }
}
