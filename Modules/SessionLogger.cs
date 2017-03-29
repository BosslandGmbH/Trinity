using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.IO;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Objects;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Bot;
using ItemEvents = Trinity.Framework.Events.ItemEvents;

namespace Trinity.Modules
{
    [Serializable]
    public class StatsSession
    {
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public DateTime StopTime { get; set; } = DateTime.MinValue;
        public GameStats Games { get; set; } = new GameStats();
        public ItemStats Stashed { get; set; } = new ItemStats();
        public ItemStats Sold { get; set; } = new ItemStats();
        public ItemStats Salvaged { get; set; } = new ItemStats();
        public ItemStats Dropped { get; set; } = new ItemStats();
        public ItemStats PickedUp { get; set; } = new ItemStats();
        public PlayerStats Player { get; set; } = new PlayerStats();
        public ActorStats Actors { get; set; } = new ActorStats();
    }

    [Serializable]
    public class ActorStats
    {
        public int Shrines { get; set; }
        public int Elites { get; set; }
        public int Bosses { get; set; }
        public int Goblins { get; set; }
    }

    [Serializable]
    public class GameStats
    {
        public int Joined { get; set; }
        public int Left { get; set; }
    }

    [Serializable]
    public class PlayerStats
    {
        public int Deaths { get; set; }
    }

    [Serializable]
    public class ItemStats
    {
        public int Total { get; set; }
        public int Sets { get; set; }
        public int Normals { get; set; }
        public int Rares { get; set; }
        public int Magics { get; set; }
        public int Ancients { get; set; }
        public int Crafting { get; set; }
        public int CraftingStackQuantity { get; set; }
        public int Pets { get; set; }
        public int Wings { get; set; }
        public int Gems { get; set; }
        public int Potions { get; set; }
        public int Legendaries { get; set; }
        public int Ubers { get; set; }
        public int Transmog { get; set; }
        public int Gifts { get; set; }
        public int Equipment { get; set; }
    }

    public class SessionLogger : Module
    {
        private HashSet<int> SeenActorAnnIds { get; set; } = new HashSet<int>();

        public StatsSession Current { get; set; } = new StatsSession();

        public bool IsRecording { get; set; }

        public bool IsSettingEnabled => Core.Settings.Advanced.LogStats;

        public bool IsWired { get; private set; }

        protected override void OnPluginEnabled()
        {
            if (IsWired) return;

            BotMain.OnStart += bot => Start();
            BotMain.OnStop += bot => Stop();
            BotMain.OnShutdownRequested += (sender, args) => Stop();

            GameEvents.OnGameJoined += (sender, args) => RecordStat(() => Current.Games.Joined++);
            GameEvents.OnGameLeft += (sender, args) => RecordStat(() => Current.Games.Left++);
            GameEvents.OnPlayerDied += (sender, args) => RecordStat(() => Current.Player.Deaths++);
            GameEvents.OnWorldChanged += (sender, args) => SeenActorAnnIds.Clear();

            ItemEvents.OnItemDropped += item => RecordItemStats(Current.Dropped, item);
            ItemEvents.OnItemPickedUp += item => RecordItemStats(Current.PickedUp, item);
            ItemEvents.OnItemSalvaged += item => RecordItemStats(Current.Salvaged, item);
            ItemEvents.OnItemStashed += item => RecordItemStats(Current.Stashed, item);
            ItemEvents.OnItemSold += item => RecordItemStats(Current.Sold, item);

            IsWired = true;
        }

        private void RecordStat(Action action)
        {
            if (!IsRunning) return;

            action();
        }

        protected override int UpdateIntervalMs => 500;

        public bool IsRunning => IsRecording && IsSettingEnabled && IsWired && TrinityPlugin.IsEnabled;

        protected override void OnPulse()
        {
            if (!IsRunning) return;

            foreach (var actor in Core.Actors.Actors)
            {
                RecordActorStats(actor);
            }
        }

        private void RecordActorStats(TrinityActor actor)
        {
            if (!IsRunning) return;

            if (SeenActorAnnIds.Contains(actor.AnnId))
                return;

            SeenActorAnnIds.Add(actor.AnnId);

            if (actor.IsUnit && actor.IsHostile)
            {
                if (actor.IsElite)
                    Current.Actors.Elites++;

                if (actor.IsBoss)
                    Current.Actors.Bosses++;
            }

            if (actor.ShrineType != ShrineTypes.None)
                Current.Actors.Shrines++;

            if (actor.IsTreasureGoblin)
                Current.Actors.Goblins++;
        }

        public void RecordItemStats(ItemStats stats, TrinityItem item)
        {
            if (!IsRunning) return;

            stats.Total++;

            if (item.IsAncient)
                stats.Ancients++;

            if (GameData.PetTable.Contains(item.GameBalanceId) || GameData.PetSnoIds.Contains(item.ActorSnoId))
                stats.Pets++;

            if (GameData.TransmogTable.Contains(item.GameBalanceId))
                stats.Transmog++;

            if (Core.Settings.Items.SpecialItems.HasFlag(SpecialItemTypes.Wings) && GameData.WingsTable.Contains(item.GameBalanceId) || GameData.CosmeticSnoIds.Contains(item.ActorSnoId))
                stats.Wings++;

            if (item.TrinityItemType == TrinityItemType.HealthPotion)
                stats.Potions++;

            if (item.TrinityItemType == TrinityItemType.UberReagent)
                stats.Ubers++;

            if (item.TrinityItemType == TrinityItemType.ConsumableAddSockets)
                stats.Gifts++;

            if (item.IsCraftingReagent)
            {
                stats.Crafting++;
                stats.CraftingStackQuantity += item.ItemStackQuantity;
            }

            if (item.IsEquipment)
            {
                stats.Equipment++;

                switch (item.TrinityItemQuality)
                {
                    case TrinityItemQuality.Set:
                        stats.Sets++;
                        break;
                    case TrinityItemQuality.Legendary:
                        stats.Legendaries++;
                        break;
                    case TrinityItemQuality.Rare:
                        stats.Rares++;
                        break;
                    case TrinityItemQuality.Magic:
                        stats.Magics++;
                        break;
                    case TrinityItemQuality.Inferior:
                    case TrinityItemQuality.Common:
                        stats.Normals++;
                        break;
                }
            }
        }

        private void Start()
        {
            SeenActorAnnIds = new HashSet<int>();
            Current = new StatsSession();
            Current.StartTime = DateTime.UtcNow;
            IsRecording = true;
        }

        private void Stop()
        {
            if (!IsRecording)
                return;

            IsRecording = false;
            Current.StopTime = DateTime.UtcNow;

            if (IsSettingEnabled)
                Save();
        }

        public TimeSpan Duration => Current.StopTime - Current.StartTime;

        public void Save()
        {
            var file = $"Session - {Core.Player.ActorClass} - {($"{Duration}:g").Replace(":", "-")} - {Current.StartTime.ToLocalTime():ddd dd-MMM-yy hh-mm-ss}.xml";
            var path = Path.Combine(FileManager.LoggingPath, file);
            var xml = EasyXmlSerializer.Serialize(Current);
            File.WriteAllText(path, xml);
        }

    }


}
