using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Components.Adventurer.Util;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Settings
{
    [DataContract]
    public class AdventurerGems
    {
        [DataMember]
        public List<AdventurerGem> Gems { get; set; }

        public void UpdateGems(int greaterRiftLevel, bool prioritizeEquipedGems)
        {
            var gemsInInventory = new List<AdventurerGem>();
            var result = SafeFrameLock.ExecuteWithinFrameLock(() =>
            {
                if (!ZetaDia.IsInGame || ZetaDia.Me == null || !ZetaDia.Me.IsValid) return;
                gemsInInventory = ZetaDia.Actors.GetActorsOfType<ACDItem>()
                    .Where(i => i.IsValid && i.ItemType == ItemType.LegendaryGem)
                    .Select(i => new AdventurerGem(i, greaterRiftLevel))
                    .Distinct(new AdventurerGemComparer())
                    .OrderByDescending(i => i.Rank)
                    .ToList();
            }, true);
            if (!result.Success)
            {
                Logger.Debug("[UpdateGems] " + result.Exception.Message);
                return;
            }

            if (gemsInInventory.Count == 0) return;
            if (Gems == null)
            {
                Gems = gemsInInventory;
            }
            else
            {
                var updatedList = new List<AdventurerGem>();
                foreach (var gem in Gems)
                {
                    var inventoryGem = GetMatchingInventoryGem(gem, gemsInInventory);
                    if (inventoryGem != null)
                    {
                        updatedList.Add(inventoryGem);
                        gemsInInventory.Remove(inventoryGem);
                    }
                }
                updatedList.AddRange(gemsInInventory);

                Gems = updatedList;
            }

            Gems = Gems.OrderBy(i => i.IsMaxRank ? 1 : 0).ToList();
            if (prioritizeEquipedGems)
            {
                Gems = Gems.OrderByDescending(i => i.IsEquiped ? 1 : 0).ToList();
            }
        }

        private AdventurerGem GetMatchingInventoryGem(AdventurerGem gem, List<AdventurerGem> inventoryGems)
        {
            return inventoryGems.FirstOrDefault(g => g.SNO == gem.SNO && g.Rank >= gem.Rank);
        }

    }

    public class AdventurerGemComparer : IEqualityComparer<AdventurerGem>
    {
        public bool Equals(AdventurerGem x, AdventurerGem y)
        {
            return x.Guid == y.Guid;
        }

        public int GetHashCode(AdventurerGem obj)
        {
            return obj.Guid;
        }
    }
}