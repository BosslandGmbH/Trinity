using System.Linq;
using System.Runtime.Serialization;
using log4net;
using Trinity.Framework;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Adventurer.Settings
{
    [DataContract]
    public class AdventurerGem
    {
        private static readonly ILog s_logger = Logger.GetLoggerInstanceForType();

        public int Guid { get; set; }

        [DataMember]
        public SNOActor SNO { get; set; }

        [DataMember]
        public int Rank { get; set; }

        public string DisplayRank => IsMaxRank ? "MAX" : Rank.ToString();

        public string Name { get; set; }

        public int UpgradeChance => IsMaxRank ? 0 : CalculateUpgradeChance(CurrentRiftLevel, Rank);

        public int CurrentRiftSettingsSelection => PluginSettings.Current.GreaterRiftLevel;

        public int CurrentRiftLevel { get; set; }

        public bool IsEquiped { get; set; }

        public bool IsMaxRank { get; set; }

        public int MaxRank { get; set; }

        public bool HasRankCap { get; set; }

        public AdventurerGemSetting Settings { get; set; }

        public string DisplayName => $"{Name} (Rank: {Rank}, Upgrade Chance: {UpgradeChance}% @ {CurrentRiftLevel})";

        public AdventurerGem()
        {
        }

        public AdventurerGem(ACDItem gem, int riftLevel)
        {
            Guid = gem.AnnId;
            SNO = gem.ActorSnoId;
            Rank = gem.Stats.JewelRank;
            Name = gem.Name;
            CurrentRiftLevel = riftLevel;

            Settings = PluginSettings.Current.Gems.GemSettings.FirstOrDefault(g => g.Sno == gem.ActorSnoId);
            if (Settings == null)
            {
                s_logger.Error($"[{nameof(AdventurerGem)}] Gems Settings Entry not found for {gem.Name} ({gem.ActorSnoId}), if its a new gem, it needs to be added to Trinity's Gems.cs reference");
                return;
            }

            MaxRank = Settings.MaxRank;
            IsMaxRank = Rank >= Settings.MaxRank;
            HasRankCap = Settings.MaxRank > 0;

            IsEquiped = !IsMaxRank && gem.InventorySlot == InventorySlot.Socket;
        }

        /// <summary>
        /// Gets the number of upgrades that would be possible for this gem on a given rift level and chance percent
        /// </summary>
        /// <param name="riftLevel">rift level</param>
        /// <param name="maxAttempts">max attempts</param>
        /// <param name="requiredChance">percentage ie. 80. defaults to settings value</param>
        /// <returns>number of possible upgrades</returns>
        public int GetUpgrades(int riftLevel, int maxAttempts, int requiredChance)
        {
            if (!Settings.IsEnabled)
                return 0;

            for (var i = 0; i <= maxAttempts; i++)
            {
                var rank = Rank + i;
                var chance = CalculateUpgradeChance(riftLevel, rank);
                if (chance < requiredChance || MaxRank != 0 && rank >= MaxRank)
                {
                    //Core.Logger.Debug($"{Name} RiftLevel={riftLevel} Chance={chance} RequiredChance={requiredChance} CurrentRank={Rank} TestingRank={rank} MaxRank={MaxRank} Upgrades={i} ");
                    return i;
                }
            }
            return maxAttempts;
        }

        public int GetUpgradeChance(int griftLevel)
        {
            return CalculateUpgradeChance(griftLevel, Rank);
        }

        private int CalculateUpgradeChance(int griftLevel, int rank)
        {
            if (IsMaxRank) return 0;
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