using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using Trinity.Components.Coroutines.Town;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Actors.Attributes;
using Trinity.Framework.Events;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Objects.Memory;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Zeta.Game.Internals.SNO;

namespace Trinity.Framework.Actors.Properties
{
    public class ItemProperties
    {
        static ItemProperties()
        {
            ChangeEvents.WorldId.Changed += WorldId_Changed;
        }

        private static void WorldId_Changed(ChangeEventArgs<int> args)
        {
            if (!GameData.TownLevelAreaIds.Contains(args.OldValue) && !ZetaDia.IsInTown)
            {
                Core.Logger.Debug($"Clearing ItemProperties Seen AnnIds");
                _seenActorAnnIds.Clear();
            }
        }

        private static readonly HashSet<string> _seenActorAnnIds = new HashSet<string>();

        public static void Create(TrinityItem actor)
        {
            if (actor.ActorType != ActorType.Item)
                return;

            if (actor.CommonData == null)
                return;

            var attributes = actor.Attributes;
            if (attributes == null)
                return;

            try
            {
                // TODO: This should be the same ACD as actor.CommonData?!
                var acd = ZetaDia.Actors.GetACDItemById(actor.AcdId);
                actor.InventorySlot = acd.InventorySlot;
                actor.InventoryColumn = acd.InventoryColumn;
                actor.InventoryRow = acd.InventoryRow;
            }
            catch (Exception)
            {
                actor.InventorySlot = InventorySlot.None;
            }
            
            #region Trading

            int gameTick = ZetaDia.Globals.GameTick;
            int tradeEndTime = attributes.ItemTradeEndTime <= gameTick ? 0 : attributes.ItemTradeEndTime - gameTick;
            actor.ItemTradeEndTime = TimeSpan.FromSeconds(tradeEndTime/60);

            actor.TradablePlayers = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                int playerTradeHigh = attributes.GetTradePlayerHigh(i);
                int playerTradeLow = attributes.GetTradePlayerLow(i);
                int playerTrade = (int) ((long) playerTradeHigh << 32 | (uint) playerTradeLow);
                if (playerTrade != 0)
                    actor.TradablePlayers.Add(playerTrade);
            }

            int playerId = ZetaDia.Storage.PlayerDataManager.ActivePlayerData.PlayerId;
            actor.IsTradeable = attributes.ItemTradeEndTime != 0 && actor.TradablePlayers.Contains(playerId);

            #endregion

            actor.InternalName = actor.Gbi?.InternalName ?? string.Empty;

            if (!_seenActorAnnIds.Contains(actor.PositionHash))
            {
                _seenActorAnnIds.Add(actor.PositionHash);

                if (actor.InventorySlot == InventorySlot.None)
                {
                    actor.OnDropped();
                }
            }
        }

        public static void Update(TrinityItem actor)
        {
            if (actor.ActorType != ActorType.Item)
                return;

            if (actor.CommonData == null)
                return;

            var columnChanged = actor.InventoryColumn != actor.LastInventoryColumn;
            var rowChanged = actor.InventoryRow != actor.LastInventoryRow;
            var slotChanged = actor.InventorySlot != actor.LastInventorySlot;

            actor.LastInventorySlot = actor.InventorySlot;
            actor.LastInventoryRow = actor.InventoryRow;
            actor.LastInventoryColumn = actor.InventoryColumn;

            if (actor.LastInventorySlot == InventorySlot.None && actor.InventorySlot == InventorySlot.BackpackItems)
            {
                actor.OnPickedUp();
            }

            if (columnChanged || rowChanged || slotChanged)
            {
                actor.OnMoved();
            }

            if (actor.InventorySlot == InventorySlot.BackpackItems && actor.IsUnidentified && !actor.Attributes.IsUnidentified)
            {
                actor.OnIdentified();
            }

            Create(actor);
        }

        public static GlobeTypes GetGlobeType(TrinityActor cacheObject)
        {
            switch (cacheObject.Type)
            {
                case TrinityObjectType.ProgressionGlobe:
                    if (GameData.GreaterProgressionGlobeSNO.Contains(cacheObject.ActorSnoId))
                        return GlobeTypes.GreaterRift;
                    return GlobeTypes.NephalemRift;

                case TrinityObjectType.PowerGlobe:
                    return GlobeTypes.Power;

                case TrinityObjectType.HealthGlobe:
                    return GlobeTypes.Health;
            }
            return GlobeTypes.None;
        }

        public static FollowerType GetFollowerType(int actorSnoId)
        {
            switch (actorSnoId)
            {
                case 363893:
                case 192942:
                case 4482: return FollowerType.Enchantress;
                case 363891:
                case 192940:
                case 52693: return FollowerType.Templar;
                case 363892:
                case 192941:
                case 52694: return FollowerType.Scoundrel;
            }
            return FollowerType.None;
        }

        public static string GetName(int gameBalanceId) => SnoManager.StringListHelper.GetStringListValue(SnoStringListType.Items, gameBalanceId);

        public static bool CanPickupItem(TrinityItem actor)
        {
            if (actor.InventorySlot != InventorySlot.None)
                return false;

            if (actor.Attributes?.ItemBoundToACDId != -1)
            {
                return actor.Attributes != null && actor.Attributes.ItemBoundToACDId == Core.Actors.Me?.AcdId;
            }

            if (actor.ItemQualityLevel >= ItemQuality.Legendary || actor.IsCraftingReagent)
            {
                if (actor.Attributes?.ItemBoundToACDId == -1)
                    return true;

                return actor.Attributes != null && actor.IsTradeable;
            }

            if (actor.IsEquipment && actor.ItemQualityLevel <= ItemQuality.Rare6)
                return true;
            
            return false;
        }
    }
}
