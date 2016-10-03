using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Combat;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Items;
using Trinity.Reference;
using Zeta.Game;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Game.Internals.Actors;

namespace Trinity.Coroutines
{
    /// <summary>
    /// Handles equipping items while levelling.
    /// </summary>
    public class AutoEquipItems
    {
        public AutoEquipItems()
        {            
            // Always run after start to help with debugging
            BotMain.OnStart += ibot => { _lastFreeBackpackSlots = 0; };
        }

        private DateTime _lastEquipCheckTime;
        private int _lastFreeBackpackSlots;

        private static AutoEquipItems _instance;
        public static AutoEquipItems Instance
        {
            get { return _instance ?? (_instance = (new AutoEquipItems())); }
        }        

        public async Task<bool> Execute()
        {
            if (!Core.Settings.Items.AutoEquipItems)
                return false;

            if (!Core.Player.IsValid || Core.Player.IsInCombat || !ZetaDia.IsInGame || ZetaDia.IsLoadingWorld)
                return false;

            if (DateTime.UtcNow.Subtract(_lastEquipCheckTime).TotalSeconds < 5)
                return false;

            if (!ZetaDia.Me.IsValid || !ZetaDia.Me.CommonData.IsValid || ZetaDia.Me.CommonData.IsDisposed || ZetaDia.Me.Level == 0 || ZetaDia.Me.Level >= 70 && Core.Settings.Items.AutoEquipAutoDisable)
                return false;

            if (_lastFreeBackpackSlots == Core.Player.FreeBackpackSlots)
                return false;

            await IdentifyLegendaries();

            Reset();

            _equippedItems = new Dictionary<InventorySlot, CachedACDItem>();

            foreach (var item in ZetaDia.Me.Inventory.Equipped)
            {
                // DB's Inventory Equipped collection sometimes gets messed up and has duplicate items. 
                if (_equippedItems.ContainsKey(item.InventorySlot))
                    continue;

                _equippedItems.Add(item.InventorySlot, CachedACDItem.GetTrinityItem(item));
            }

            _upgrades = _slots.ToDictionary(k => k, v => default(CachedACDItem));

            foreach (var slot in _slots)
            {
                CachedACDItem currentlyEquipped;
                if (!_equippedItems.TryGetValue(slot, out currentlyEquipped))
                     _equippedItems.Add(slot, null);

                var backpackItems = GetBackpackItemsForSlot(slot);
                if (backpackItems == null)
                    continue;

                Logger.LogVerbose("{0}: {1}, Weight={2}", slot, currentlyEquipped != null ? currentlyEquipped.RealName : "None", GetWeight(currentlyEquipped));

                foreach (var backpackItem in backpackItems)
                {
                    if (backpackItem.AcdItem.InventorySlot != InventorySlot.BackpackItems)
                        continue;

                    // Dont use a 2hander in some situations.
                    if (backpackItem.TwoHanded && (
                        Core.Player.ActorClass == ActorClass.Crusader || // Crusader uses shield bash ability to level.
                        Core.Player.ActorClass == ActorClass.DemonHunter && !backpackItem.IsClassItem))
                        continue;
                         
                    var bestUpgradeFoundSoFar = _upgrades[slot];                    
                    if (IsUpgrade(backpackItem, currentlyEquipped, bestUpgradeFoundSoFar))
                        _upgrades[slot] = backpackItem;                                         
                }
            }

            Logger.Log("Item Evaluation {0} Equipped, {1} Backpack Candidates, {2} Upgrades found",
                _equippedItems.Count(i => i.Value != null),
                BackpackEquipment.Count(),
                _upgrades.Count(i => i.Value != null));

            foreach (var upgrade in _upgrades)
            {
                if (upgrade.Value == null)
                    continue;

                //if (upgrade.Key == InventorySlot.LeftHand && upgrade.Value.TwoHanded)
                //{
                //    var offhand = _equippedItems[InventorySlot.RightHand];
                //    if (offhand != null)
                //        UnequipItem(offhand);
                //}
                    
                await EquipItem(upgrade.Value.AcdItem, upgrade.Key);
                await Coroutine.Sleep(500);
            }

            _lastEquipCheckTime = DateTime.UtcNow;
            _lastFreeBackpackSlots = Core.Player.FreeBackpackSlots;

            await EquipWeaponSocket();
            await EquipArmorSockets();

            return false;
        }

        private void UnequipItem(CachedACDItem item)
        {
            var location = DefaultLootProvider.FindBackpackLocation(true);
            if (location == DefaultLootProvider.NoFreeSlot)
                return;

            Logger.Log("Unequipping Item {0} ({1}) from slot {2}", item.RealName, item.ActorSnoId, item.InventorySlot);
            ZetaDia.Me.Inventory.MoveItem(item.DynamicId, ZetaDia.Me.CommonData.AnnId, InventorySlot.BackpackItems, (int)location.X, (int)location.Y);
        }

        /// <summary>
        /// Will identify the first legendary found in backpack.
        /// </summary>
        private static async Task<bool> IdentifyLegendaries()
        {
            Navigator.PlayerMover.MoveStop();
            await Coroutine.Sleep(500);
            var newLegendary = ZetaDia.Me.Inventory.Backpack.FirstOrDefault(i => i.ItemQualityLevel >= ItemQuality.Legendary && i.Unidentified > 0 && i.IsValid && !i.IsDisposed);
            if (newLegendary != null)
            {
                Logger.Log("Identifying Legendary");
                var dynamicId = newLegendary.AnnId;
                ZetaDia.Me.Inventory.IdentifyItem(dynamicId);
                await Coroutine.Sleep(750);
                while (ZetaDia.Me.LoopingAnimationEndTime > 0)
                {
                    await Coroutine.Sleep(750);
                }                        
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sockets equipped or backpack weapons rubies.
        /// </summary>
        private async Task<bool> EquipWeaponSocket()
        {           
            var gem = GetGemForAttributeType(PlayerAttributeType.Strength);
            if (gem == null)
                return false;

            var socketableWeapon = ZetaDia.Me.Inventory.Equipped.FirstOrDefault(i => i.InventorySlot == InventorySlot.LeftHand && i.NumSockets > 0 && i.NumSocketsFilled < i.NumSockets);
            if (socketableWeapon != null)
            {
                Logger.Log("Socketing {0} ({1}) into equipped weapon {2}", gem.InternalName, gem.GemQuality, socketableWeapon.Name);
                socketableWeapon.Socket(gem);
                return true;
            }

            var socketableBackpackWeapon = BackpackEquipment
                .OrderByDescending(i => i.AcdItem.Sockets)
                .ThenByDescending(i => i.WeaponDamagePerSecond)
                .FirstOrDefault(i => i.InventorySlot == InventorySlot.LeftHand && i.AcdItem.NumSockets > 0 && i.AcdItem.NumSockets < i.AcdItem.NumSocketsFilled);

            if (socketableBackpackWeapon != null)
            {
                Logger.Log("Socketing {0} ({1}) into backpack weapon {2}", gem.InternalName, gem.GemQuality, socketableBackpackWeapon.RealName);
                socketableBackpackWeapon.AcdItem.Socket(gem);
                return true;
            }

            return false;            
        }

        /// <summary>
        /// Sockets equipped or backpack armor with primary stat or vitality
        /// </summary>
        /// <returns></returns>
        private async Task<bool> EquipArmorSockets()
        { 
            var attrType = GetAttributeTypeForClass(Core.Player.ActorClass);
            var gem = GetGemForAttributeType(attrType);

            if (gem == null)
                gem = GetGemForAttributeType(PlayerAttributeType.Vitality);

            if (gem == null)
                return false;

            var socketableArmor = ZetaDia.Me.Inventory.Equipped.FirstOrDefault(i => (i.ItemBaseType == ItemBaseType.Armor || i.ItemBaseType == ItemBaseType.Jewelry) && i.NumSockets > 0 && i.NumSocketsFilled < i.NumSockets);
            if (socketableArmor != null)
            {
                Logger.Log("Socketing {0} ({1}) into equipped armor {2}", gem.InternalName, gem.GemQuality, socketableArmor.Name);
                socketableArmor.Socket(gem);
                return true;
            }

            var socketableBackpackArmor = BackpackEquipment
                .OrderByDescending(i => i.AcdItem.Sockets)
                .ThenByDescending(i => i.HighestPrimary)
                .FirstOrDefault(i => (i.BaseType == ItemBaseType.Armor || i.BaseType == ItemBaseType.Jewelry) && i.AcdItem.NumSockets > 0 && i.AcdItem.NumSockets < i.AcdItem.NumSocketsFilled);

            if (socketableBackpackArmor != null)
            {
                Logger.Log("Socketing {0} ({1}) into backpack armor {2}", gem.InternalName, gem.GemQuality, socketableBackpackArmor.RealName);
                socketableBackpackArmor.AcdItem.Socket(gem);
                return true;
            }

            return false;
        }

        private PlayerAttributeType GetAttributeTypeForClass(ActorClass actorClass)
        {
            switch (actorClass)
            {
                case ActorClass.Crusader:
                case ActorClass.Barbarian:
                    return PlayerAttributeType.Strength;
                case ActorClass.Monk:
                case ActorClass.DemonHunter:
                    return PlayerAttributeType.Dexterity;
                case ActorClass.Witchdoctor:
                case ActorClass.Wizard:
                    return PlayerAttributeType.Intelligence;
            }

            return PlayerAttributeType.None;
        }

        public enum PlayerAttributeType
        {
            None = 0,
            Intelligence,
            Strength,
            Dexterity,
            Vitality,
        }

        private ACDItem GetGemForAttributeType(PlayerAttributeType attributeType)
        {
            var gems = ZetaDia.Me.Inventory.Backpack.Where(i => i.IsGem).OrderByDescending(i => i.GemQuality);
            
            switch (attributeType)
            {
                case PlayerAttributeType.Strength:
                    return gems.FirstOrDefault(i => i.Name.ToLower().Contains("ruby"));
                case PlayerAttributeType.Intelligence:
                    return gems.FirstOrDefault(i => i.Name.ToLower().Contains("topaz"));
                case PlayerAttributeType.Dexterity:
                    return gems.FirstOrDefault(i => i.Name.ToLower().Contains("emerald"));
            }  
            
            return gems.FirstOrDefault(i => i.Name.ToLower().Contains("amethyst"));
        }

        private void Reset()
        {
            // Make backpack get a fresh list of items
            _backpackEquipment = null;
        }

        private IEnumerable<CachedACDItem> GetBackpackItemsForSlot(InventorySlot slot)
        {
            switch (slot)
            {
                case InventorySlot.Feet:
                    return BackpackEquipment.Where(i => i.ItemType == ItemType.Boots);

                case InventorySlot.Hands:
                    return BackpackEquipment.Where(i => i.ItemType == ItemType.Gloves);

                case InventorySlot.Head:
                    return BackpackEquipment.Where(i => i.ItemType == ItemType.Helm || i.ItemType == ItemType.WizardHat || i.ItemType == ItemType.SpiritStone || i.ItemType == ItemType.VoodooMask);

                case InventorySlot.Shoulders:
                    return BackpackEquipment.Where(i => i.ItemType == ItemType.Shoulder);

                case InventorySlot.Torso:
                    return BackpackEquipment.Where(i => i.ItemType == ItemType.Chest || i.ItemType == ItemType.Cloak);

                case InventorySlot.Waist:
                    return BackpackEquipment.Where(i => i.ItemType == ItemType.Belt || i.ItemType == ItemType.MightyBelt);

                case InventorySlot.Bracers:
                    return BackpackEquipment.Where(i => i.ItemType == ItemType.Bracer);

                case InventorySlot.Legs:
                    return BackpackEquipment.Where(i => i.ItemType == ItemType.Legs);

                case InventorySlot.Neck:
                    return BackpackEquipment.Where(i => i.ItemType == ItemType.Amulet);

                case InventorySlot.LeftFinger:
                case InventorySlot.RightFinger:
                    return BackpackEquipment.Where(i => i.ItemType == ItemType.Ring);

                case InventorySlot.RightHand:
                    if (Core.Settings.Items.AutoEquipIgnoreWeapons)
                        return null;

                    return BackpackEquipment.Where(i => i.TrinityItemBaseType == TrinityItemBaseType.Offhand || i.TrinityItemType == TrinityItemType.CrusaderShield);

                case InventorySlot.LeftHand:
                    if (Core.Settings.Items.AutoEquipIgnoreWeapons)
                        return null;

                    return BackpackEquipment.Where(i => i.BaseType == ItemBaseType.Weapon && (i.OneHanded || i.TwoHanded));
            }

            return null;
        }

        private IEnumerable<CachedACDItem> _backpackEquipment;
        public IEnumerable<CachedACDItem> BackpackEquipment
        {
            get { return _backpackEquipment ?? (_backpackEquipment = ZetaDia.Me.Inventory.Backpack
                    .Select(CachedACDItem.GetTrinityItem)
                    .Where(i => i.AcdItem.IsValid && i.IsEquipment && i.IsUsableByClass(Core.Player.ActorClass) && !i.IsUnidentified)); }
        }

        private IEnumerable<InventorySlot> _slots = new List<InventorySlot>
        {
            InventorySlot.Bracers,
            InventorySlot.Feet,
            InventorySlot.Hands,
            InventorySlot.Head,
            InventorySlot.LeftFinger,
            InventorySlot.RightFinger,
            InventorySlot.RightHand,
            InventorySlot.LeftHand,
            InventorySlot.Shoulders,
            InventorySlot.Torso,
            InventorySlot.Waist,
            InventorySlot.Neck,
            InventorySlot.Legs,
        };

        private Dictionary<InventorySlot, CachedACDItem> _upgrades;
        private Dictionary<InventorySlot, CachedACDItem> _equippedItems;

        private bool IsUpgrade(CachedACDItem item, CachedACDItem currentlyEquipped, CachedACDItem bestUpgradeSoFar)
        {
            return IsUpgrade(currentlyEquipped, item) && (bestUpgradeSoFar == null || IsUpgrade(bestUpgradeSoFar, item));
        }

        private bool IsUpgrade(CachedACDItem oldItem, CachedACDItem newItem)
        {
            if (newItem == null)
                return false;

            // Always equip empty slot except if its an offhand and a 2hander is equipped.
            if (oldItem == null)
            {
                if (newItem.IsOffHand)
                {
                    var equippedMainhand = _equippedItems[InventorySlot.LeftHand];
                    if (equippedMainhand != null && equippedMainhand.TwoHanded)
                        return false;
                }
                return true;
            };

            var newItemWeight = GetWeight(newItem);
            var oldItemWeight = GetWeight(oldItem);

            // Replacing a mainhand + offhand with a 2hander
            if (newItem.TwoHanded && !oldItem.TwoHanded)
            {
                var equippedOffhand = _equippedItems[InventorySlot.RightHand];
                var offHandWeight = GetWeight(equippedOffhand);
                var result = oldItemWeight + offHandWeight < newItemWeight;

                Logger.LogVerbose("   > {0}={1}, MainHand({3})+Offhand({4}) >> {2}", 
                    newItem.RealName, newItemWeight, result ? "Upgrade" : "Skip", oldItemWeight, offHandWeight);

                return result;
            }

            // Replacing a 2hander with mainhand + offhand
            if (!newItem.TwoHanded && oldItem.TwoHanded)
            {
                var bestOffhand = _upgrades[InventorySlot.RightHand];
                var offHandWeight = GetWeight(bestOffhand);
                var result = oldItemWeight < newItemWeight + offHandWeight;

                Logger.LogVerbose("   > {0}={1} + Offhand({4}), MainHand({3}) >> {2}",
                    newItem.RealName, newItemWeight, result ? "Upgrade" : "Skip", oldItemWeight, offHandWeight);

                return result;
            }

            Logger.LogVerbose("   > {0}={1} >> {2}",  newItem.RealName, newItemWeight, oldItemWeight < newItemWeight ? "Upgrade" : "Skip");
            return oldItemWeight < newItemWeight;
        }

        private double GetWeight(CachedACDItem item)
        {
            var weight = 0d;

            if (item == null)
                return weight;

            if (item.TrinityItemBaseType == TrinityItemBaseType.Offhand)
            {
                weight = GetOffhandWeight(item);
            }
            else
            {
                switch (item.BaseType)
                {
                    case ItemBaseType.Jewelry:
                    case ItemBaseType.Armor:
                        weight = GetArmorWeight(item);
                        break;

                    case ItemBaseType.Weapon:
                        weight = GetWeaponWeight(item);
                        break;
                }
            }

            if (item.Quality >= ItemQuality.Legendary && IsModifiedByGemOfEase(item.AcdItem))
                 return weight + 50000;

            if (ZetaDia.Me.Level < item.AcdItem.RequiredLevel)
                return 0;
                
            return weight;
        }

        public bool IsModifiedByGemOfEase(ACDItem acdItem)
        {
            // Note: RequiredLevel property still shows unmodified level req.
            var hasBeenCubedToRemoveLevelRequirement = acdItem.RemoveLevelReq == 1;
            var hasGemOfEaseSocketed = acdItem.ItemLevelRequirementOverride == 1;
            return hasBeenCubedToRemoveLevelRequirement || hasGemOfEaseSocketed;
        }

        private double GetOffhandWeight(CachedACDItem item)
        {
            var classMultiplier = item.IsClassItem ? 1.5 : 1;            
            return (GetAttributeWeight(item) + GetCriticalWeight(item) + item.AcdItem.DamageAverageTotalAll) * classMultiplier;            
        }

        private double GetArmorWeight(CachedACDItem item)
        {
            return GetAttributeWeight(item) + GetCriticalWeight(item) + GetMiscWeight(item);
        }

        private double GetWeaponWeight(CachedACDItem item)
        {
            var classMultiplier = item.IsClassItem && Core.Player.ActorClass == ActorClass.DemonHunter ? 2 : 1;

            return item.WeaponDamagePerSecond * classMultiplier + GetAttributeWeight(item) / 5;
        }

        private double GetAttributeWeight(CachedACDItem item)
        {
            return (item.HighestPrimary + item.Vitality) / 5;
        }

        private double GetMiscWeight(CachedACDItem item)
        {
            return (int) item.Quality + item.Armor/20;
        }

        private double GetCriticalWeight(CachedACDItem item)
        {
            return item.CritDamagePercent * 2 + item.CritPercent * 10;
        }

        public static async Task<bool> EquipItem(ACDItem item, InventorySlot slot)
        {
            Logger.LogVerbose("Equipping Item: {0} ({1}) - in slot {2} IsValid={3}, IsDisposed={4}",
                item.Name,
                item.ActorSnoId,
                slot,
                item.IsValid,
                item.IsDisposed);

            ZetaDia.Me.Inventory.EquipItem(item.AnnId, slot);

            await Coroutine.Sleep(500);

            if (ZetaDia.Me.Inventory.Equipped.Any(i => i.AnnId == item.AnnId))
            {
                Logger.LogVerbose("Item: {0} ({1}) was equipped", item.Name, item.ActorSnoId);
            }

            return true;
        }
    }

}



