using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Modules
{
    public class InventoryCache : Module, IEnumerable<TrinityItem>
    {
        public InventoryCache()
        {
            GameEvents.OnGameJoined += (sender, args) => Update();
            GameEvents.OnWorldChanged += (sender, args) => Update();

            AllItems.Source = () => Core.Actors.Inventory.Where(i => !InvalidAnnIds.Contains(i.AnnId));
            Stash.Source = () => Core.Inventory.AllItems.Where(i => i.InventorySlot == InventorySlot.SharedStash);
            Backpack.Source = () => Core.Inventory.AllItems.Where(i => i.InventorySlot == InventorySlot.BackpackItems);
        }

        public HashSet<int> KanaisCubeIds { get; private set; } = new HashSet<int>();
        public HashSet<int> PlayerEquippedIds { get; private set; } = new HashSet<int>();
        public HashSet<int> EquippedIds { get; private set; } = new HashSet<int>();
        public List<TrinityItem> Equipped { get; private set; } = new List<TrinityItem>();
        public HashSet<int> InvalidAnnIds { get; private set; } = new HashSet<int>();
        public InventoryCurrency Currency { get; } = new InventoryCurrency();
        public InventorySlice AllItems { get; } = new InventorySlice();
        public InventorySlice Stash { get; } = new InventorySlice();
        public InventorySlice Backpack { get; } = new InventorySlice();
        public int BackpackItemCount { get; private set; }
        protected override void OnPulse() => Update();
        IEnumerator IEnumerable.GetEnumerator() => AllItems.GetEnumerator();
        public IEnumerator<TrinityItem> GetEnumerator() => AllItems.GetEnumerator();

        public void Update()
        {
            if (!ZetaDia.IsInGame || ZetaDia.Storage.PlayerDataManager.ActivePlayerData == null)
                return;

            var kanaisCubeIds = new HashSet<int>(ZetaDia.Storage.PlayerDataManager.ActivePlayerData.KanaisPowersAssignedActorSnoIds);
            var equipped = new List<TrinityItem>();
            var equippedIds = new HashSet<int>();
            var playerEquippedIds = new HashSet<int>();
            var backpackItemCount = 0;

            foreach (var item in Core.Actors.Inventory)
            {
                if (!item.IsValid)
                    continue;

                switch (item.InventorySlot)
                {
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
        public IEnumerable<TrinityItem> GetStacksUpToQuantity(List<TrinityItem> materialsStacks, int maxStackQuantity)
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

        public class InventorySlice : IEnumerable<TrinityItem>
        {
            public Func<IEnumerable<TrinityItem>> Source { get; set; }
            public List<TrinityItem> ByItemType(ItemType type) => Source().Where(i => i.ItemType == type).ToList();
            public List<TrinityItem> ByActorSno(int actorSno) => Source().Where(i => i.ActorSnoId == actorSno).ToList();
            public List<TrinityItem> ByQuality(TrinityItemQuality quality) => Source().Where(i => i.TrinityItemQuality == quality).ToList();
            public void Update() => Source().ForEach(i => i.OnUpdated());
            public IEnumerator<TrinityItem> GetEnumerator() => Source().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => Source().GetEnumerator();
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
                => recipe.All(requirement => PlayerData.GetCurrencyAmount(requirement.Key) >= requirement.Value);

            /// <summary>
            /// Extracts a legendary power; requires legendary item.
            /// </summary>
            private static Dictionary<CurrencyType, int> _currencyRecipeExtractLegendaryPower = new Dictionary<CurrencyType, int>
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
            private static Dictionary<CurrencyType, int> _currencyRecipeReforgeLegendary = new Dictionary<CurrencyType, int>
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
            private static Dictionary<CurrencyType, int> _currencyRecipeUpgradeRareItem = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ReusableParts, 50},
                {CurrencyType.ArcaneDust, 50},
                {CurrencyType.VeiledCrystal, 50},
                {CurrencyType.DeathsBreath, 25}
            };

            /// <summary>
            /// Produces random set item; requires set item.
            /// </summary>
            private static Dictionary<CurrencyType, int> _currencyRecipeConvertSetItem = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ForgottenSoul, 10},
                {CurrencyType.DeathsBreath, 10}
            };

            /// <summary>
            /// Produces ReusableParts (requires normal item) -or- Veiled Crystals (requires rare item)
            /// </summary>
            private static Dictionary<CurrencyType, int> _currencyRecipeConvertFromMagic = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ArcaneDust, 100},
                {CurrencyType.DeathsBreath, 1}
            };

            /// <summary>
            /// Produces Arcane Dust (requires magic item) -or- Veiled Crystals (requires rare item)
            /// </summary>
            private static Dictionary<CurrencyType, int> _currencyRecipeConvertFromNormal = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.ReusableParts, 100},
                {CurrencyType.DeathsBreath, 1}
            };

            /// <summary>
            /// Produces ReusableParts (requires normal item) -or- Arcane Dust (requires magic item)
            /// </summary>
            private static Dictionary<CurrencyType, int> _currencyRecipeConvertFromRare = new Dictionary<CurrencyType, int>
            {
                {CurrencyType.VeiledCrystal, 100},
                {CurrencyType.DeathsBreath, 1}
            };

            public long GetCurrency(CurrencyType type) => PlayerData.GetCurrencyAmount(type);
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