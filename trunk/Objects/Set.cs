using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Reference;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Objects
{
    public enum SetBonus
    {
        None = 0,
        First,
        Second,
        Third,
    }

    /// <summary>
    /// A collection of items that when equipped together provide special bonuses
    /// </summary>
    public class Set
    {
        public Set()
        {
            ClassRestriction = ActorClass.Invalid;
        }

        public string Name { get; set; }

        /// <summary>
        /// Number of items required to receive the first set bonus
        /// </summary>
        public int FirstBonusItemCount { get; set; }

        /// <summary>
        /// Number of items required to receive second set bonus, 0 = no bonus possible
        /// </summary>
        public int SecondBonusItemCount { get; set; }

        /// <summary>
        /// Number of items required to receive third set bonus, 0 = no bonus possible
        /// </summary>
        public int ThirdBonusItemCount { get; set; }

        /// <summary>
        /// Class this set is restricted to
        /// </summary>
        public ActorClass ClassRestriction { get; set; }

        /// <summary>
        /// If this set may only be used by a specific class
        /// </summary>
        public bool IsClassRestricted => ClassRestriction != ActorClass.Invalid;

        /// <summary>
        /// All items in this set
        /// </summary>
        public List<Item> Items { get; set; }

        /// <summary>
        /// All the ActorSnoId ids for the items in this set
        /// </summary>
        private HashSet<int> _itemIds;
        public HashSet<int> ItemIds
        {
            get { return _itemIds ?? (_itemIds = new HashSet<int>(Items.Select(i => i.Id))); }
        }

        /// <summary>
        /// Items of this set that are currently equipped
        /// </summary>
        public List<Item> EquippedItems
        {
            get { return Items.Where(i => Core.Inventory.PlayerEquippedIds.Contains(i.Id)).ToList(); }
        }

        /// <summary>
        /// Items of this set that are currently equipped, as ACDItem
        /// </summary>
        public List<TrinityItem> EquippedTrinityItems
        {
            get { return Core.Inventory.Equipped.Where(i => ItemIds.Contains(i.ActorSnoId)).ToList(); }
        }

        public bool IsFirstBonusActive => IsBonusActive(FirstBonusItemCount);

        public bool IsSecondBonusActive => IsBonusActive(SecondBonusItemCount);

        public bool IsThirdBonusActive => IsBonusActive(ThirdBonusItemCount);

        public bool IsBonusActive(SetBonus setBonus)
        {
            switch (setBonus)
            {
                case SetBonus.First:
                    return IsFirstBonusActive;
                case SetBonus.Second:
                    return IsSecondBonusActive;
                case SetBonus.Third:
                    return IsThirdBonusActive;
            }
            return false;
        }

        public bool IsBonusActive(int requiredItemCountForBonus)
        {
            var actualRequired = requiredItemCountForBonus - (Legendary.RingOfRoyalGrandeur.IsEquipped ? 1 : 0);
            if (actualRequired < 2) actualRequired = 2;
            return requiredItemCountForBonus > 0 && EquippedItems.Count >= actualRequired;
        }

        public bool IsOneBonusSet => !IsThreeBonusSet && !IsTwoBonusSet;

        public bool IsTwoBonusSet => !IsThreeBonusSet && SecondBonusItemCount > 0;

        public bool IsThreeBonusSet => ThirdBonusItemCount > 0;

        /// <summary>
        /// Items required to get the maximum set bonus
        /// </summary>
        public int MaxBonusItemCount => IsThreeBonusSet ? ThirdBonusItemCount : (IsTwoBonusSet ? SecondBonusItemCount : FirstBonusItemCount);

        public bool IsMaxBonusActive => IsBonusActive(MaxBonusItemCount);

        /// <summary>
        /// Items required to get the maximum set bonus
        /// </summary>
        public int MaxBonuses => IsThreeBonusSet ? 3 : (IsTwoBonusSet ? 2 : 1);

        /// <summary>
        /// Items required to get the maximum set bonus
        /// </summary>
        public int CurrentBonuses => IsThirdBonusActive ? 3 : (IsSecondBonusActive ? 2 : (IsFirstBonusActive) ? 1 : 0);

        /// <summary>
        /// Is this set is equipped enough for the maximum set bonus
        /// </summary>
        public bool IsFullyEquipped => IsThreeBonusSet ? IsThirdBonusActive : (IsTwoBonusSet ? IsSecondBonusActive : IsFirstBonusActive);

        /// <summary>
        /// Is this set is equipped enough for any set bonus
        /// </summary>
        public bool IsEquipped => IsThirdBonusActive || IsSecondBonusActive || IsFirstBonusActive;
    }
}
