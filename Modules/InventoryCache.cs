using log4net;
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
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();

        public InventoryCache()
        {
            GameEvents.OnGameJoined += (sender, args) => Update();
            GameEvents.OnWorldChanged += (sender, args) => Update();

            AllItems.Source = () => Core.Actors.Inventory.Where(i => !InvalidAnnIds.Contains(i.AnnId));
            Stash.Source = () => AllItems.Where(i => i.InventorySlot == InventorySlot.SharedStash);
            Backpack.Source = () => AllItems.Where(i => i.InventorySlot == InventorySlot.BackpackItems);
        }

        public HashSet<int> KanaisCubeIds { get; private set; } = new HashSet<int>();
        public HashSet<int> PlayerEquippedIds { get; private set; } = new HashSet<int>();
        public HashSet<int> EquippedIds { get; private set; } = new HashSet<int>();
        public List<ACDItem> Equipped { get; private set; } = new List<ACDItem>();
        public HashSet<int> InvalidAnnIds { get; private set; } = new HashSet<int>();
        public InventoryCurrency Currency { get; } = new InventoryCurrency();
        public InventorySlice AllItems { get; } = new InventorySlice();
        public InventorySlice Stash { get; } = new InventorySlice();
        public InventorySlice Backpack { get; } = new InventorySlice();
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
            if (!ZetaDia.IsInGame || ZetaDia.Storage.PlayerDataManager.ActivePlayerData == null)
                return;

            var kanaisCubeIds = new HashSet<int>(ZetaDia.Storage.PlayerDataManager.ActivePlayerData.KanaisPowersAssignedActorSnoIds);
            var equipped = new List<ACDItem>();
            var equippedIds = new HashSet<int>();
            var playerEquippedIds = new HashSet<int>();
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
            BackpackItemCount = backpackItemCount;
        }

        public void Clear()
        {
            InvalidAnnIds.Clear();
        }

        /// <summary>
        /// Get a subset of items up to the desired quantity.
        /// </summary>
        public IEnumerable<ACDItem> GetStacksUpToQuantity(List<ACDItem> materialsStacks, int maxStackQuantity)
        {
            if (materialsStacks == null || !materialsStacks.Any() || materialsStacks.Count == 1)
            {
                return materialsStacks;
            }
            long dbQuantity = 0, overlimit = 0;
            var first = materialsStacks.First();
            if (first.ItemStackQuantity == 0 && maxStackQuantity == 1 && materialsStacks.All(i => !i.IsCraftingReagent))
            {
                return materialsStacks.Take(maxStackQuantity);
            }
            var toBeAdded = materialsStacks.TakeWhile(db =>
            {
                var thisStackQuantity = db.ItemStackQuantity;
                if (dbQuantity + thisStackQuantity < maxStackQuantity)
                {
                    dbQuantity += thisStackQuantity;
                    return true;
                }
                overlimit++;
                return overlimit == 1;
            });
            return toBeAdded.ToList();
        }

        public class InventorySlice : IEnumerable<ACDItem>
        {
            public Func<IEnumerable<ACDItem>> Source { get; set; }
            public List<ACDItem> ByItemType(ItemType type)
            {
                return Source().Where(i => i.GetItemType() == type).ToList();
            }

            public List<ACDItem> ByActorSno(int actorSno)
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
                        return HasCurrency(_currencyRecipeExtractLegendaryPower);
                    case TransmuteRecipe.ReforgeLegendary:
                        return HasCurrency(_currencyRecipeReforgeLegendary);
                    case TransmuteRecipe.UpgradeRareItem:
                        return HasCurrency(_currencyRecipeUpgradeRareItem);
                    case TransmuteRecipe.ConvertSetItem:
                        return HasCurrency(_currencyRecipeConvertSetItem);
                    case TransmuteRecipe.ConvertCraftingMaterialsFromNormal:
                        return HasCurrency(_currencyRecipeConvertFromNormal);
                    case TransmuteRecipe.ConvertCraftingMaterialsFromMagic:
                        return HasCurrency(_currencyRecipeConvertFromMagic);
                    case TransmuteRecipe.ConvertCraftingMaterialsFromRare:
                        return HasCurrency(_currencyRecipeConvertFromRare);
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
            private static readonly Dictionary<CurrencyType, int> _currencyRecipeExtractLegendaryPower = new Dictionary<CurrencyType, int>
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
            private static readonly Dictionary<CurrencyType, int> _currencyRecipeReforgeLegendary = new Dictionary<CurrencyType, int>
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
            private static readonly Dictionary<CurrencyType, int> _currencyRecipeUpgradeRareItem = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ReusableParts, 50},
                {CurrencyType.ArcaneDust, 50},
                {CurrencyType.VeiledCrystal, 50},
                {CurrencyType.DeathsBreath, 25}
            };

            /// <summary>
            /// Produces random set item; requires set item.
            /// </summary>
            private static readonly Dictionary<CurrencyType, int> _currencyRecipeConvertSetItem = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ForgottenSoul, 10},
                {CurrencyType.DeathsBreath, 10}
            };

            /// <summary>
            /// Produces ReusableParts (requires normal item) -or- Veiled Crystals (requires rare item)
            /// </summary>
            private static readonly Dictionary<CurrencyType, int> _currencyRecipeConvertFromMagic = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ArcaneDust, 100},
                {CurrencyType.DeathsBreath, 1}
            };

            /// <summary>
            /// Produces Arcane Dust (requires magic item) -or- Veiled Crystals (requires rare item)
            /// </summary>
            private static readonly Dictionary<CurrencyType, int> _currencyRecipeConvertFromNormal = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ReusableParts, 100},
                {CurrencyType.DeathsBreath, 1}
            };

            /// <summary>
            /// Produces ReusableParts (requires normal item) -or- Arcane Dust (requires magic item)
            /// </summary>
            private static readonly Dictionary<CurrencyType, int> _currencyRecipeConvertFromRare = new Dictionary<CurrencyType, int>
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
