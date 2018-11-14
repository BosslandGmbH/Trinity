using System;
using System.Linq;
using Trinity.Components.Coroutines.Town;
using Trinity.Framework.Actors.Properties;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Trinity.Framework.Reference;
using Trinity.Settings;
using Zeta.Bot.Settings;
using Zeta.Game;
using Zeta.Game.Internals.Actors;

namespace Trinity.Framework.Actors.Attributes
{
    internal static class ACDItemExtensions
    {
        public static int GetElementalDamage(this ACDItem item, DamageType damageType)
        {
            var key = new AttributeKey((int)ActorAttributeType.DamageDealtPercentBonus, (int)damageType);
            return (int)Math.Round(item.GetAttribute<float>(key.BaseAttribute, key.ModifierId) * 100, MidpointRounding.AwayFromZero);
        }

        public static float SkillDamagePercent(this ACDItem item, SNOPower power)
        {
            var key = new AttributeKey((int)ActorAttributeType.PowerDamagePercentBonus, (int)power);
            return item.GetAttribute<float>(key.BaseAttribute, key.ModifierId);
        }

        public static string InternalNameLowerCase(this ACDItem item)
        {
            return item.InternalName.ToLowerInvariant();
        }

        public static bool IsProtected(this ACDItem item)
        {
            return CharacterSettings.Instance.ProtectedBagSlots.Any(sq => sq.Row == item.InventoryRow && sq.Column == item.InventoryColumn);
        }

        public static TrinityItemQuality GetTrinityItemQuality(this ACDItem item)
        {
            var iql = item.ItemQualityLevel;

            if (iql == ItemQuality.Inferior)
                return TrinityItemQuality.Inferior;

            if (iql >= ItemQuality.Normal &&
                iql <= ItemQuality.Superior)
            {
                return TrinityItemQuality.Common;
            }

            if (iql >= ItemQuality.Magic1 &&
                iql <= ItemQuality.Magic3)
            {
                return TrinityItemQuality.Magic;
            }

            if (iql >= ItemQuality.Rare4 &&
                iql <= ItemQuality.Rare6)
            {
                return TrinityItemQuality.Rare;
            }

            if (iql >= ItemQuality.Legendary)
            {
                return TrinityItemQuality.Legendary;
            }

            return TrinityItemQuality.None;
        }

        public static RawItemType GetRawItemType(this ACDItem item)
        {
            return (RawItemType)item.ItemTypeGBId;
        }

        public static ItemType GetItemType(this ACDItem item)
        {
            return TypeConversions.GetItemType(item.GetRawItemType());
        }

        public static ItemBaseType GetItemBaseType(this ACDItem item)
        {
            return TypeConversions.GetItemBaseType(item.GetItemType());
        }

        public static TrinityItemType GetTrinityItemType(this ACDItem item)
        {
            return TypeConversions.GetTrinityItemType(item.GetRawItemType(), item.GemType);
        }

        public static TrinityItemBaseType GetTrinityItemBaseType(this ACDItem item)
        {
            return TypeConversions.GetTrinityItemBaseType(item.GetTrinityItemType());
        }

        public static bool GetIsCosmeticItem(this ACDItem item)
        {
            var rit = item.GetRawItemType();
            return rit == RawItemType.CosmeticPet ||
                   rit == RawItemType.CosmeticPennant ||
                   rit == RawItemType.CosmeticPortraitFrame ||
                   rit == RawItemType.CosmeticWings;
        }

        public static bool GetIsEquipment(this ACDItem item)
        {
            return TypeConversions.GetIsEquipment(item.GetTrinityItemBaseType());
        }

        public static bool GetIsTradeable(this ACDItem item)
        {
            int playerId = ZetaDia.Storage.PlayerDataManager.ActivePlayerData.PlayerId;
            return item.Stats.ItemTradeEndTime != 0 && item.Stats.ItemTradeablePlayers.Contains(playerId);
        }

        public static bool GetIsSalvageable(this ACDItem item)
        {
            return item.GetIsEquipment() &&
                   !item.IsVendorBought &&
                   item.Stats.RequiredLevel > 1 ||
                   item.IsPotion;
        }

        public static bool GetIsGold(this ACDItem item)
        {
            return item.GetRawItemType() == RawItemType.Gold;
        }

        public static bool CanPickupItem(this ACDItem item)
        {
            if (item.InventorySlot != InventorySlot.None)
                return false;

            if (item.Stats.BoundToACD != -1)
            {
                return item.Stats.BoundToACD == ZetaDia.Me?.ACDId;
            }

            if (item.ItemQualityLevel >= ItemQuality.Legendary || item.IsCraftingReagent)
            {
                if (item.Stats.BoundToACD == -1)
                    return true;

                return item.GetIsTradeable();
            }

            if (item.GetIsEquipment() && item.ItemQualityLevel <= ItemQuality.Rare6)
                return true;

            return false;
        }

        public static bool GetIsUntargetable(this ACDItem item)
        {
            return item != null &&
                   item.IsValid &&
                   item.Untargetable != 0 &&
                   !GameData.IgnoreUntargettableAttribute.Contains(item.ActorSnoId);
        }

        public static bool GetIsMyDroppedItem(this ACDItem item)
        {
            return TrinityTownRun.DroppedItemAnnIds.Contains(item.AnnId);
        }

        public static bool GetIsPickupNoClick(this ACDItem item)
        {
            return GameData.NoPickupClickItemTypes.Contains(item.GetTrinityItemType()) ||
                   GameData.NoPickupClickTypes.Contains(item.GetObjectType());
        }

        public static TrinityObjectType GetObjectType(this ACDItem item)
        {
            return CommonProperties.GetObjectType(item.ActorType, item.ActorSnoId, item.GizmoType, item.InternalName);
        }

        public static int GetGoldAmount(this ACDItem item)
        {
            return item.GetObjectType() == TrinityObjectType.Gold ? item.Stats.Gold : 0;
        }

        public static GlobeTypes GetGlobeType(this ACDItem item)
        {
            switch (item.GetObjectType())
            {
                case TrinityObjectType.ProgressionGlobe:
                    if (GameData.GreaterProgressionGlobeSNO.Contains(item.ActorSnoId))
                        return GlobeTypes.GreaterRift;
                    return GlobeTypes.NephalemRift;

                case TrinityObjectType.PowerGlobe:
                    return GlobeTypes.Power;

                case TrinityObjectType.HealthGlobe:
                    return GlobeTypes.Health;
            }
            return GlobeTypes.None;
        }
    }
}