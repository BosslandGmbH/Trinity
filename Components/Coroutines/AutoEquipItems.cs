using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Trinity.Components.Combat;
using Trinity.Framework.Objects;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Game;
using Zeta.Game.Internals;
using Zeta.Game.Internals.Actors;

namespace Trinity.Components.Coroutines
{
    /// <summary>
    /// Handles equipping items while levelling.
    /// todo: refactor AutoEquipItems to use TrinityItem. This is the last place that uses the old CachedACDItem.
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

            if (!Core.Player.IsValid || Core.Player.IsInCombat || !ZetaDia.IsInGame || ZetaDia.Globals.IsLoadingWorld)
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

            foreach (var item in InventoryManager.Equipped)
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

                Core.Logger.Verbose("{0}: {1}, Weight={2}", slot, currentlyEquipped != null ? currentlyEquipped.RealName : "None", GetWeight(currentlyEquipped));

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

            Core.Logger.Log("Item Evaluation {0} Equipped, {1} Backpack Candidates, {2} Upgrades found",
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
            var location = DefaultLootProvider.FindBackpackLocation(true, false);
            if (location == DefaultLootProvider.NoFreeSlot)
                return;

            Core.Logger.Log("Unequipping Item {0} ({1}) from slot {2}", item.RealName, item.ActorSnoId, item.InventorySlot);
            InventoryManager.MoveItem(item.DynamicId, ZetaDia.Me.CommonData.AnnId, InventorySlot.BackpackItems, (int)location.X, (int)location.Y);
        }

        /// <summary>
        /// Will identify the first legendary found in backpack.
        /// </summary>
        private static async Task<bool> IdentifyLegendaries()
        {
            Navigator.PlayerMover.MoveStop();
            await Coroutine.Sleep(500);
            var newLegendary = InventoryManager.Backpack.FirstOrDefault(i => i.ItemQualityLevel >= ItemQuality.Legendary && i.Unidentified > 0 && i.IsValid && !i.IsDisposed);
            if (newLegendary != null)
            {
                Core.Logger.Log("Identifying Legendary");
                var dynamicId = newLegendary.AnnId;
                InventoryManager.IdentifyItem(dynamicId);
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

            var socketableWeapon = InventoryManager.Equipped.FirstOrDefault(i => i.InventorySlot == InventorySlot.LeftHand && i.NumSockets > 0 && i.NumSocketsFilled < i.NumSockets);
            if (socketableWeapon != null)
            {
                Core.Logger.Log("Socketing {0} ({1}) into equipped weapon {2}", gem.InternalName, gem.GemQuality, socketableWeapon.Name);
                socketableWeapon.Socket(gem);
                return true;
            }

            var socketableBackpackWeapon = BackpackEquipment
                .OrderByDescending(i => i.AcdItem.Sockets)
                .ThenByDescending(i => i.WeaponDamagePerSecond)
                .FirstOrDefault(i => i.InventorySlot == InventorySlot.LeftHand && i.AcdItem.NumSockets > 0 && i.AcdItem.NumSockets < i.AcdItem.NumSocketsFilled);

            if (socketableBackpackWeapon != null)
            {
                Core.Logger.Log("Socketing {0} ({1}) into backpack weapon {2}", gem.InternalName, gem.GemQuality, socketableBackpackWeapon.RealName);
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

            var socketableArmor = InventoryManager.Equipped.FirstOrDefault(i => (i.ItemBaseType == ItemBaseType.Armor || i.ItemBaseType == ItemBaseType.Jewelry) && i.NumSockets > 0 && i.NumSocketsFilled < i.NumSockets);
            if (socketableArmor != null)
            {
                Core.Logger.Log("Socketing {0} ({1}) into equipped armor {2}", gem.InternalName, gem.GemQuality, socketableArmor.Name);
                socketableArmor.Socket(gem);
                return true;
            }

            var socketableBackpackArmor = BackpackEquipment
                .OrderByDescending(i => i.AcdItem.Sockets)
                .ThenByDescending(i => i.HighestPrimary)
                .FirstOrDefault(i => (i.BaseType == ItemBaseType.Armor || i.BaseType == ItemBaseType.Jewelry) && i.AcdItem.NumSockets > 0 && i.AcdItem.NumSockets < i.AcdItem.NumSocketsFilled);

            if (socketableBackpackArmor != null)
            {
                Core.Logger.Log("Socketing {0} ({1}) into backpack armor {2}", gem.InternalName, gem.GemQuality, socketableBackpackArmor.RealName);
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

                case ActorClass.Necromancer:
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
            var gems = InventoryManager.Backpack.Where(i => i.IsGem).OrderByDescending(i => i.GemQuality);

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
            get
            {
                return _backpackEquipment ?? (_backpackEquipment = InventoryManager.Backpack
                  .Select(CachedACDItem.GetTrinityItem)
                  .Where(i => i.AcdItem.IsValid && i.IsEquipment && i.IsUsableByClass(Core.Player.ActorClass) && !i.IsUnidentified));
            }
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

                Core.Logger.Verbose("   > {0}={1}, MainHand({3})+Offhand({4}) >> {2}",
                    newItem.RealName, newItemWeight, result ? "Upgrade" : "Skip", oldItemWeight, offHandWeight);

                return result;
            }

            // Replacing a 2hander with mainhand + offhand
            if (!newItem.TwoHanded && oldItem.TwoHanded)
            {
                var bestOffhand = _upgrades[InventorySlot.RightHand];
                var offHandWeight = GetWeight(bestOffhand);
                var result = oldItemWeight < newItemWeight + offHandWeight;

                Core.Logger.Verbose("   > {0}={1} + Offhand({4}), MainHand({3}) >> {2}",
                    newItem.RealName, newItemWeight, result ? "Upgrade" : "Skip", oldItemWeight, offHandWeight);

                return result;
            }

            Core.Logger.Verbose("   > {0}={1} >> {2}", newItem.RealName, newItemWeight, oldItemWeight < newItemWeight ? "Upgrade" : "Skip");
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
            return (int)item.Quality + item.Armor / 20;
        }

        private double GetCriticalWeight(CachedACDItem item)
        {
            return item.CritDamagePercent * 2 + item.CritPercent * 10;
        }

        public static async Task<bool> EquipItem(ACDItem item, InventorySlot slot)
        {
            Core.Logger.Verbose("Equipping Item: {0} ({1}) - in slot {2} IsValid={3}, IsDisposed={4}",
                item.Name,
                item.ActorSnoId,
                slot,
                item.IsValid,
                item.IsDisposed);

            InventoryManager.EquipItem(item.AnnId, slot);

            await Coroutine.Sleep(500);

            if (InventoryManager.Equipped.Any(i => i.AnnId == item.AnnId))
            {
                Core.Logger.Verbose("Item: {0} ({1}) was equipped", item.Name, item.ActorSnoId);
            }

            return true;
        }
    }

    public class CachedACDItem : IComparable
    {
        public string InternalName { get; set; }
        public string RealName { get; set; }
        public int Level { get; set; }
        public ItemQuality Quality { get; set; }
        public int GoldAmount { get; set; }
        public int BalanceID { get; set; }
        public int DynamicId { get; set; }
        public int ActorSnoId { get; set; }
        public bool OneHanded { get; set; }
        public bool TwoHanded { get; set; }
        public DyeType DyeType { get; set; }
        public ItemType ItemType { get; set; }
        public ItemBaseType BaseType { get; set; }
        public TrinityItemBaseType TrinityItemBaseType { get; set; }
        public TrinityItemType TrinityItemType { get; set; }
        public FollowerType FollowerType { get; set; }
        public bool IsUnidentified { get; set; }
        public long ItemStackQuantity { get; set; }
        public float Dexterity { get; set; }
        public float Intelligence { get; set; }
        public float Strength { get; set; }
        public float Vitality { get; set; }
        public float LifePercent { get; set; }
        public float LifeOnHit { get; set; }
        public float LifeSteal { get; set; }
        public float HealthPerSecond { get; set; }
        public float MagicFind { get; set; }
        public float GoldFind { get; set; }
        public float MovementSpeed { get; set; }
        public float PickUpRadius { get; set; }
        public float Sockets { get; set; }
        public float CritPercent { get; set; }
        public float CritDamagePercent { get; set; }
        public float AttackSpeedPercent { get; set; }
        public float MinDamage { get; set; }
        public float MaxDamage { get; set; }
        public float BlockChance { get; set; }
        public float Thorns { get; set; }
        public float ResistAll { get; set; }
        public float ResistArcane { get; set; }
        public float ResistCold { get; set; }
        public float ResistFire { get; set; }
        public float ResistHoly { get; set; }
        public float ResistLightning { get; set; }
        public float ResistPhysical { get; set; }
        public float ResistPoison { get; set; }
        public float WeaponDamagePerSecond { get; set; }
        public float ArmorBonus { get; set; }
        public float MaxDiscipline { get; set; }
        public float MaxMana { get; set; }
        public float ArcaneOnCrit { get; set; }
        public float ManaRegen { get; set; }
        public float GlobeBonus { get; set; }
        public float HatredRegen { get; set; }
        public float MaxFury { get; set; }
        public float SpiritRegen { get; set; }
        public float MaxSpirit { get; set; }
        public float HealthPerSpiritSpent { get; set; }
        public float MaxArcanePower { get; set; }
        public float DamageReductionPhysicalPercent { get; set; }
        public float ArmorTotal { get; set; }
        public float Armor { get; set; }
        public float FireDamagePercent { get; set; }
        public float LightningDamagePercent { get; set; }
        public float ColdDamagePercent { get; set; }
        public float PoisonDamagePercent { get; set; }
        public float ArcaneDamagePercent { get; set; }
        public float HolyDamagePercent { get; set; }
        public float HealthGlobeBonus { get; set; }
        public float WeaponAttacksPerSecond { get; set; }
        public float WeaponMaxDamage { get; set; }
        public float WeaponMinDamage { get; set; }
        public ACDItem AcdItem { get; set; }
        public int InventoryRow { get; set; }
        public int InventoryColumn { get; set; }
        public string ItemLink { get; set; }
        public bool IsAncient { get; set; }
        public bool IsEquipment { get; set; }
        public bool IsSalvageable { get; set; }
        public int GameBalanceId { get; set; }
        public InventorySlot InventorySlot { get; set; }
        public double HighestPrimary { get; set; }
        public bool IsClassItem { get; set; }
        public bool IsOffHand { get; set; }

        public CachedACDItem(ItemStats stats)
        {
            CacheStats(stats);
        }

        private void CacheStats(ItemStats stats)
        {
            WeaponDamagePerSecond = stats.WeaponDamagePerSecond;
            Dexterity = stats.Dexterity;
            Intelligence = stats.Intelligence;
            Strength = stats.Strength;
            Vitality = stats.Vitality;
            LifePercent = stats.LifePercent;
            LifeOnHit = stats.LifeOnHit;
            LifeSteal = stats.LifeSteal;
            HealthPerSecond = stats.HealthPerSecond;
            MagicFind = stats.MagicFind;
            GoldFind = stats.GoldFind;
            MovementSpeed = stats.MovementSpeed;
            PickUpRadius = stats.PickUpRadius;
            Sockets = stats.Sockets;
            CritPercent = stats.CritPercent;
            CritDamagePercent = stats.CritDamagePercent;
            AttackSpeedPercent = stats.AttackSpeedPercent;
            MinDamage = stats.MinDamage;
            MaxDamage = stats.MaxDamage;
            BlockChance = stats.BlockChance;
            Thorns = stats.Thorns;
            ResistAll = stats.ResistAll;
            ResistArcane = stats.ResistArcane;
            ResistCold = stats.ResistCold;
            ResistFire = stats.ResistFire;
            ResistHoly = stats.ResistHoly;
            ResistLightning = stats.ResistLightning;
            ResistPhysical = stats.ResistPhysical;
            ResistPoison = stats.ResistPoison;
            WeaponDamagePerSecond = stats.WeaponDamagePerSecond;
            ArmorBonus = stats.ArmorBonus;
            MaxDiscipline = stats.MaxDiscipline;
            MaxMana = stats.MaxMana;
            ArcaneOnCrit = stats.ArcaneOnCrit;
            ManaRegen = stats.ManaRegen;
            GlobeBonus = stats.HealthGlobeBonus;
            HatredRegen = stats.HatredRegen;
            MaxFury = stats.MaxFury;
            SpiritRegen = stats.SpiritRegen;
            MaxSpirit = stats.MaxSpirit;
            HealthPerSpiritSpent = stats.HealthPerSpiritSpent;
            MaxArcanePower = stats.MaxArcanePower;
            DamageReductionPhysicalPercent = stats.DamageReductionPhysicalPercent;
            ArmorTotal = stats.ArmorTotal;
            Armor = stats.Armor;
            //FireDamagePercent = stats.FireDamagePercent;
            //LightningDamagePercent = stats.LightningDamagePercent;
            //ColdDamagePercent = stats.ColdDamagePercent;
            //PoisonDamagePercent = stats.PoisonDamagePercent;
            //ArcaneDamagePercent = stats.ArcaneDamagePercent;
            //HolyDamagePercent = stats.HolyDamagePercent;
            HealthGlobeBonus = stats.HealthGlobeBonus;
            WeaponAttacksPerSecond = stats.WeaponAttacksPerSecond;
            WeaponMaxDamage = stats.WeaponMaxDamage;
            WeaponMinDamage = stats.WeaponMinDamage;
            HighestPrimary = stats.HighestPrimaryAttribute;
        }

        public CachedACDItem()
        {
        }

        public int CompareTo(object obj)
        {
            CachedACDItem item = (CachedACDItem)obj;

            if (InventoryRow < item.InventoryRow)
                return -1;
            if (InventoryColumn < item.InventoryColumn)
                return -1;
            if (InventoryColumn == item.InventoryColumn && InventoryRow == item.InventoryRow)
                return 0;
            return 1;
        }

        public static CachedACDItem GetTrinityItem(ACDItem item)
        {
            try
            {
                if (!item.IsValid)
                    return default(CachedACDItem);

                CachedACDItem cItem = new CachedACDItem(item.Stats)
                {
                    AcdItem = item,
                    InternalName = item.InternalName,
                    RealName = item.Name,
                    Level = item.Level,
                    Quality = item.GetItemQuality(),
                    GoldAmount = item.Gold,
                    BalanceID = item.GameBalanceId,
                    DynamicId = item.AnnId,
                    ActorSnoId = item.ActorSnoId,
                    OneHanded = item.IsOneHand,
                    TwoHanded = item.IsTwoHand,
                    DyeType = item.DyeType,
                    ItemType = item.ItemType,
                    BaseType = item.ItemBaseType,
                    FollowerType = item.FollowerSpecialType,
                    IsUnidentified = item.IsUnidentified,
                    ItemStackQuantity = item.ItemStackQuantity,
                    InventoryRow = item.InventoryRow,
                    InventoryColumn = item.InventoryColumn,
                    ItemLink = item.ItemLink,
                    GameBalanceId = item.GameBalanceId,
                    TrinityItemType = TypeConversions.DetermineItemType(item.InternalName, item.ItemType, item.FollowerSpecialType),
                    IsAncient = item.GetAttribute<int>(ActorAttributeType.AncientRank) > 0,
                    InventorySlot = item.InventorySlot,
                };

                TrinityItemBaseType trinityItemBaseType = TypeConversions.GetTrinityItemBaseType(TypeConversions.DetermineItemType(item.InternalName, item.ItemType, item.FollowerSpecialType));
                cItem.TrinityItemBaseType = trinityItemBaseType;
                cItem.IsEquipment = GetIsEquipment(trinityItemBaseType);
                cItem.IsSalvageable = GetIsSalvageable(cItem);
                cItem.IsClassItem = GetIsClassItem(cItem);
                cItem.IsOffHand = GetIsOffhand(cItem);
                return cItem;
            }
            catch (Exception ex)
            {
                Core.Logger.Error("Error getting TrinityItem {0}", ex.Message);
                return default(CachedACDItem);
            }
        }

        private static bool GetIsOffhand(CachedACDItem cItem)
        {
            switch (cItem.ItemType)
            {
                case ItemType.Mojo:
                case ItemType.Quiver:
                case ItemType.CrusaderShield:
                case ItemType.Shield:
                case ItemType.Orb:
                case ItemType.Phylactery:
                    return true;
            }
            return false;
        }

        public static bool GetIsClassItem(CachedACDItem cItem)
        {
            switch (cItem.ItemType)
            {
                case ItemType.Mojo:
                case ItemType.Quiver:
                case ItemType.Orb:
                case ItemType.CrusaderShield:
                case ItemType.MightyWeapon:
                case ItemType.MightyBelt:
                case ItemType.SpiritStone:
                case ItemType.Daibo:
                case ItemType.Flail:
                case ItemType.Cloak:
                case ItemType.WizardHat:
                case ItemType.CeremonialDagger:
                case ItemType.VoodooMask:
                case ItemType.FistWeapon:
                case ItemType.Crossbow:
                case ItemType.HandCrossbow:
                case ItemType.Bow:
                case ItemType.Scythe:
                case ItemType.Phylactery:
                    return true;
            }
            return false;
        }

        public static bool GetIsSalvageable(CachedACDItem cItem)
        {
            if (!cItem.IsEquipment)
                return false;

            if (cItem.AcdItem.IsVendorBought)
                return false;

            return true;
        }

        public static bool GetIsEquipment(TrinityItemBaseType baseType)
        {
            switch (baseType)
            {
                case TrinityItemBaseType.Armor:
                case TrinityItemBaseType.Jewelry:
                case TrinityItemBaseType.Offhand:
                case TrinityItemBaseType.WeaponOneHand:
                case TrinityItemBaseType.WeaponRange:
                case TrinityItemBaseType.WeaponTwoHand:
                case TrinityItemBaseType.FollowerItem:
                    return true;

                default:
                    return false;
            }
        }

        public bool IsUsableByClass(ActorClass actorClass)
        {
            switch (TrinityItemType)
            {
                case TrinityItemType.Mojo:
                case TrinityItemType.CeremonialKnife:
                case TrinityItemType.VoodooMask:
                    if (actorClass != ActorClass.Witchdoctor)
                        return false;
                    break;

                case TrinityItemType.SpiritStone:
                case TrinityItemType.FistWeapon:
                case TrinityItemType.TwoHandDaibo:
                    if (actorClass != ActorClass.Monk)
                        return false;
                    break;

                case TrinityItemType.MightyBelt:
                case TrinityItemType.MightyWeapon:
                case TrinityItemType.TwoHandMighty:
                    if (actorClass != ActorClass.Barbarian)
                        return false;
                    break;

                case TrinityItemType.Orb:
                case TrinityItemType.WizardHat:
                case TrinityItemType.Wand:
                    if (actorClass != ActorClass.Wizard)
                        return false;
                    break;

                case TrinityItemType.Flail:
                case TrinityItemType.TwoHandFlail:
                case TrinityItemType.CrusaderShield:
                    if (actorClass != ActorClass.Crusader)
                        return false;
                    break;

                case TrinityItemType.Cloak:
                case TrinityItemType.Quiver:
                case TrinityItemType.TwoHandBow:
                case TrinityItemType.HandCrossbow:
                case TrinityItemType.TwoHandCrossbow:
                    if (actorClass != ActorClass.DemonHunter)
                        return false;
                    break;

                case TrinityItemType.Scythe:
                case TrinityItemType.Phylactery:
                case TrinityItemType.TwoHandScythe:
                    if (actorClass != ActorClass.Necromancer)
                        return false;
                    break;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            var other = obj as CachedACDItem;
            return other != null && other.DynamicId == DynamicId;
        }

        protected bool Equals(CachedACDItem other)
        {
            return DynamicId == other.DynamicId && ActorSnoId == other.ActorSnoId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DynamicId * 397) ^ ActorSnoId;
            }
        }
    }
}