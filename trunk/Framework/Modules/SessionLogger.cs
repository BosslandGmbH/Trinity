using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;
using Trinity.Components.Combat;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Events;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Reference;
using Trinity.Settings;
using Zeta.Bot;
using Zeta.Game.Internals.Actors;
using ItemEvents = Trinity.Framework.Events.ItemEvents;

namespace Trinity.Framework.Modules
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

        public enum GameStatType
        {
            None = 0,
            Joined,
            Left,
            Death
        }
            

        protected override void OnPluginEnabled()
        {
            BotMain.OnStart += bot => Start();
            BotMain.OnStop += bot => Stop();      
            BotMain.OnShutdownRequested += (sender, args) => Stop();     
             
            GameEvents.OnGameJoined += (sender, args) => RecordStat(() => Current.Games.Joined++);
            GameEvents.OnGameLeft += (sender, args) => RecordStat(() => Current.Games.Joined++);
            GameEvents.OnPlayerDied += (sender, args) => RecordStat(() => Current.Player.Deaths++);

            ItemEvents.OnItemDropped += item => RecordItemStats(Current.Dropped, item);
            ItemEvents.OnItemPickedUp += item => RecordItemStats(Current.PickedUp, item);
            ItemEvents.OnItemSalvaged += item => RecordItemStats(Current.Salvaged, item);
            ItemEvents.OnItemStashed += item => RecordItemStats(Current.Stashed, item);
            ItemEvents.OnItemSold += item => RecordItemStats(Current.Sold, item);
        }

        private void RecordStat(Action action)
        {
            if (!IsRecording || !IsSettingEnabled)
                return;

            action();
        }

        private void Gate(Action v)
        {
            if(IsRecording && IsSettingEnabled) v();            
        }

        protected override int UpdateIntervalMs => 500;

        protected override void OnPulse()
        {
            if (!IsRecording || !IsSettingEnabled)
                return;

            foreach (var actor in Core.Actors.AllRActors)
            {
                RecordActorStats(actor);
            }
        }

        private void RecordActorStats(TrinityActor actor)
        {
            if (!IsRecording || !IsSettingEnabled)
                return;

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
            if (!IsRecording || !IsSettingEnabled)
                return;

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
            Save();
        }

        public TimeSpan Duration => Current.StopTime - Current.StartTime;

        public void Save()
        {
            var file = $"Session - {Core.Player.ActorClass} - {Duration:g} - {Current.StartTime.ToLocalTime():ddd dd-MMM-yy hh-mm-ss}.xml";            
            var path = Path.Combine(FileManager.LoggingPath, file);
            var xml = EasyXmlSerializer.Serialize(Current);
            File.WriteAllText(path, xml);
        }

    }


}
