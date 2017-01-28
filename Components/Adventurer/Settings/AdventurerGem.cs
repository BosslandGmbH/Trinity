using System.Collections.Generic;
using System.Runtime.Serialization;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Settings
{
    [DataContract]
    public class AdventurerGem
    {
        public int Guid { get; set; }
        [DataMember]
        public int SNO { get; set; }
        [DataMember]
        public int Rank { get; set; }

        public string DisplayRank => IsMaxRank ? "MAX" : Rank.ToString();

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int UpgradeChance { get; set; }
        [DataMember]
        public bool IsEquiped { get; set; }
        [DataMember]
        public bool IsMaxRank { get; set; }
        [DataMember]
        public int MaxRank { get; set; }

        public bool HasRankCap { get; set; }

        public string DisplayName => $"{Name} (Rank: {Rank}, Upgrade Chance: {UpgradeChance}%)";

        public AdventurerGem(TrinityItem gem, int griftLevel)
        {
            Guid = gem.AcdId;
            SNO = gem.ActorSnoId;
            Rank = gem.Attributes.JewelRank;
            Name = gem.Name;

            if (GemCaps.ContainsKey(SNO))
            {
                MaxRank = GemCaps[SNO];
                IsMaxRank = Rank >= MaxRank;
                HasRankCap = MaxRank > 0;
            }

            IsEquiped = !IsMaxRank && gem.InventorySlot == InventorySlot.Socket;
            UpgradeChance = IsMaxRank ? 0 : CalculateUpgradeChance(griftLevel, Rank);
        }

        //public AdventurerGem(ACDItem gem, int griftLevel)
        //{
        //    Guid = gem.ACDId;
        //    SNO = gem.ActorSnoId;
        //    Rank = gem.JewelRank;
        //    Name = gem.Name;

        //    if (GemCaps.ContainsKey(SNO))
        //    {
        //        MaxRank = GemCaps[SNO];
        //        IsMaxRank = Rank >= MaxRank;
        //        HasRankCap = MaxRank > 0;
        //    }

        //    IsEquiped = !IsMaxRank && gem.InventorySlot == InventorySlot.Socket;
        //    UpgradeChance = IsMaxRank ? 0 : CalculateUpgradeChance(griftLevel, Rank);
        //}

        public static Dictionary<int, int> GemCaps = new Dictionary<int, int>
        {
            {428355, 50}, // iceblink
            {405796, 150}, //ActorId: 405796, Type: Item, Name: Gogok of Swiftness
            //{405797, 50}, // invigorating gemstone
            {405803, 50}, // Boon of the Hoarder
            //{405783, 25}, //ActorId: 405783, Type: Item, Name: Gem of Ease
            {428033, 100},//ActorId: 428033, Type: Item, Name: Esoteric Alteration  
            {428346, 100}, //Mutilation Guard 428346 
        };

        /// <summary>
        /// Gets the number of upgrades that would be possible for this gem on a given rift level and chance percent
        /// </summary>
        /// <param name="riftLevel">rift level</param>
        /// <param name="maxAttempts">max attempts</param>
        /// <param name="requiredChance">percentage ie. 80. defaults to settings value</param>
        /// <returns>number of possible upgrades</returns>
        public int GetUpgrades(int riftLevel, int maxAttempts, int requiredChance = -1)
        {
            if (requiredChance < 0)
            {
                requiredChance = PluginSettings.Current.GreaterRiftGemUpgradeChance;
            }
            for (var i = 0; i <= maxAttempts; i++)
            {
                var rank = Rank + i;
                var chance = CalculateUpgradeChance(riftLevel, rank);
                if (chance < requiredChance || MaxRank != 0 && rank >= MaxRank)
                {
                    //Logger.Debug($"{Name} RiftLevel={riftLevel} Chance={chance} RequiredChance={requiredChance} CurrentRank={Rank} TestingRank={rank} MaxRank={MaxRank} Upgrades={i} ");
                    return i;
                }
            }
            return maxAttempts;
        }

        public void UpdateUpgradeChance(int griftLevel)
        {
            UpgradeChance = GetUpgradeChance(griftLevel);
        }

        public int GetUpgradeChance(int griftLevel)
        {
            return IsMaxRank ? 0 : CalculateUpgradeChance(griftLevel, Rank);
        }

        private int CalculateUpgradeChance(int griftLevel, int rank)
        {
            var result = griftLevel - rank;
            if (result >= 10) return 100;
            if (result >= 9) return 90;
            if (result >= 8) return 80;
            if (result >= 7) return 70;
            if (result >= 0) return 60;
            if (result >= -1) return 30;
            if (result >= -2) return 15;
            if (result >= -3) return 8;
            if (result >= -4) return 4;
            if (result >= -5) return 2;
            if (result >= -15) return 1;
            return 0;
        }


    }
}