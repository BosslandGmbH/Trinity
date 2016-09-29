using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using JetBrains.Annotations;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Reference;
using Trinity.Settings;
using Trinity.Settings.Loot;
using Trinity.UI;
using Trinity.UI.UIComponents;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.Items.ItemList
{
    /// <summary>
    /// Settings for ItemList looting
    /// </summary>
    [DataContract(Namespace = "")]
    public class ItemListSettings : ITrinitySetting<ItemListSettings>, INotifyPropertyChanged, ITrinitySettingEvents
    {
        #region Fields

        private static List<LItem> _TrinityItems;
        private FullyObservableCollection<LItem> _displayItems;
        private List<LItem> _selectedItems = new List<LItem>();
        private GroupingType _grouping;
        private string _filterText;
        private DeferredAction _deferredAction;
        private CollectionViewSource _collection;
        private string _exportCode;
        private string _validationMessage;
        private ModalPage _selectedModalPage;
        private bool _isModalVisible;
        private Dictionary<int, LItem> _viewPortal;
        private bool _alwaysStashAncients;
        private bool _alwaysTrashNonAncients;
        private FullyObservableCollection<LItem> _itemTypes;
        private int _selectedTabIndex;
        private bool _upgradeRules;

        #endregion

        #region Constructors

        public ItemListSettings()
        {
            Logger.Log($"Itemlist ViewModel Created from Thread {Thread.CurrentThread.ManagedThreadId}");
            _displayItems = new FullyObservableCollection<LItem>();
            _collection = new CollectionViewSource();
            _itemTypes = new FullyObservableCollection<LItem>();
            CacheReferenceItems();
            Grouping = GroupingType.None;
            Reset();
        }

        /// <summary>
        /// Setup work called on Construction / Reset
        /// </summary>
        private void Initialization()
        {
            CacheReferenceItems();
            DisplayItems = new FullyObservableCollection<LItem>(_TrinityItems, true);
            BindEvents();
            LoadCommands();
            GroupsExpandedByDefault = false;
        }

        /// <summary>
        /// Selected settings is always available for loot rule processing etc via ItemList.Selected property.
        /// But we only care about the UI Control being properly populated if the settings window is open.
        /// </summary>
        private void SettingsWindowOpened()
        {
            CreateView();
            UpdateSelectedItems();
        }

        /// <summary>
        /// Configure the CollectionViewSource
        /// </summary>
        public void CreateView()
        {
            Collection = new CollectionViewSource();
            Collection.Source = DisplayItems;
            ChangeGrouping(Grouping);
            ChangeSorting(SortingType.Name);
            CreateItemTypes();
        }

        private void CreateItemTypes()
        {
            if (_itemTypes == null)
            {
                _itemTypes = new FullyObservableCollection<LItem>();
            }

            var itemTypes = new List<TrinityItemType>((TrinityItemType[])Enum.GetValues(typeof(TrinityItemType)));

            var itemTypeImages = itemTypes.ToDictionary(k => k, v => string.Empty);

            foreach (var item in _TrinityItems)
            {
                itemTypeImages[item.TrinityItemType] = item.IconUrl;
            }

            var existingRules = ItemTypes.ToDictionary(k => k.Id, v => v);

            foreach (var itemType in AllowedTrinityItemTypes)
            {
                LItem item;

                if (!existingRules.TryGetValue((int)itemType, out item))
                {
                    item = new LItem
                    {
                        Name = itemType.ToString(),
                        Id = (int)itemType,
                        Type = LItem.ILType.Slot,
                        TrinityItemType = itemType,
                        ItemSelectionType = itemType.ToItemSelectionType(),
                        IconUrl = itemTypeImages[itemType]
                    };
                    item.LoadCommands();
                    ItemTypes.Add(item);
                    continue;
                }

                item.Name = itemType.ToString();
                item.Type = LItem.ILType.Slot;
                item.TrinityItemType = itemType;
                item.ItemSelectionType = itemType.ToItemSelectionType();
                item.IconUrl = itemTypeImages[itemType];
                item.LoadCommands();
            }

            ItemTypes.Sort(itr => itr.Name);
        }

        /// <summary>
        /// Property used for saving/loading from settings file.
        /// </summary>
        [DataMember(IsRequired = false)]
        public List<LItem> SelectedItemTypes
        {
            get
            {
                if (ItemTypes == null)
                    return new List<LItem>();

                return ItemTypes.Where(s => s.IsSelected).ToList();
            }
            set
            {
                if (ItemTypes == null)
                {
                    ItemTypes = new FullyObservableCollection<LItem>();
                }

                if (!ItemTypes.Any())
                {
                    foreach (var itemType in value)
                    {
                        itemType.IsSelected = true;
                        ItemTypes.Add(itemType);
                    }
                    return;
                }

                var existingRules = ItemTypes.ToDictionary(k => k.Id, v => v);

                foreach (var itemType in value)
                {
                    LItem item;
                    if (existingRules.TryGetValue(itemType.Id, out item))
                    {
                        item.Rules = itemType.Rules;
                        item.Ops = itemType.Ops;
                        item.IsSelected = true;
                        continue;
                    }
                    ItemTypes.Add(itemType);
                }
            }
        }

        public static readonly HashSet<TrinityItemType> AllowedTrinityItemTypes = new HashSet<TrinityItemType>
        {
            //TrinityItemType.Amethyst,
            TrinityItemType.Amulet,
            TrinityItemType.Axe,
            TrinityItemType.Belt,
            TrinityItemType.Boots,
            TrinityItemType.Bracer,
            TrinityItemType.CeremonialKnife,
            TrinityItemType.Chest,
            TrinityItemType.Cloak,
            //TrinityItemType.ConsumableAddSockets,
            //TrinityItemType.CraftTome,
            //TrinityItemType.CraftingMaterial,
            //TrinityItemType.CraftingPlan,
            TrinityItemType.CrusaderShield,
            TrinityItemType.Dagger,
            //TrinityItemType.Diamond,
            //TrinityItemType.Dye,
            //TrinityItemType.Emerald,
            TrinityItemType.FistWeapon,
            TrinityItemType.FollowerEnchantress,
            TrinityItemType.FollowerScoundrel,
            TrinityItemType.FollowerTemplar,
            TrinityItemType.Flail,
            TrinityItemType.Gloves,
            TrinityItemType.HandCrossbow,
            //TrinityItemType.HealthGlobe,
            //TrinityItemType.HealthPotion,
            TrinityItemType.Helm,
            //TrinityItemType.HoradricRelic,
            //TrinityItemType.HoradricCache,
            //TrinityItemType.InfernalKey,
            TrinityItemType.Legs,
            //TrinityItemType.LootRunKey,
            TrinityItemType.Mace,
            TrinityItemType.MightyBelt,
            TrinityItemType.MightyWeapon,
            TrinityItemType.Mojo,
            TrinityItemType.Orb,
            //TrinityItemType.PowerGlobe,
            //TrinityItemType.ProgressionGlobe,
            TrinityItemType.Quiver,
            TrinityItemType.Ring,
            //TrinityItemType.Ruby,
            TrinityItemType.Shield,
            TrinityItemType.Shoulder,
            TrinityItemType.Spear,
            //TrinityItemType.SpecialItem,
            TrinityItemType.SpiritStone,
            //TrinityItemType.StaffOfHerding,
            TrinityItemType.Sword,
            //TrinityItemType.TieredLootrunKey,
            //TrinityItemType.Topaz,
            TrinityItemType.TwoHandAxe,
            TrinityItemType.TwoHandBow,
            TrinityItemType.TwoHandCrossbow,
            TrinityItemType.TwoHandDaibo,
            TrinityItemType.TwoHandFlail,
            TrinityItemType.TwoHandMace,
            TrinityItemType.TwoHandMighty,
            TrinityItemType.TwoHandPolearm,
            TrinityItemType.TwoHandStaff,
            TrinityItemType.TwoHandSword,
            TrinityItemType.VoodooMask,
            TrinityItemType.Wand,
            TrinityItemType.WizardHat,
            //TrinityItemType.Gold,
            //TrinityItemType.PortalDevice,
            //TrinityItemType.UberReagent
        };

        public LItem GetitemTypeRule(TrinityItemType itemType)
        {
            return ItemTypes.FirstOrDefault(sr => sr.TrinityItemType == itemType);
        }

        /// <summary>
        /// Convert Legendary items to SettingsItem objects only once to a static collection.
        /// </summary>
        public static void CacheReferenceItems()
        {
            if (_TrinityItems == null)
                _TrinityItems = Legendary.ToList().Where(i => !i.IsCrafted && i.Id != 0).Select(i => new LItem(i)).ToList();
        }

        /// <summary>
        /// Wire up for events
        /// </summary>
        public void BindEvents()
        {
            UILoader.OnSettingsWindowOpened -= SettingsWindowOpened;
            UILoader.OnSettingsWindowOpened += SettingsWindowOpened;

            DisplayItems.ChildElementPropertyChanged -= SyncSelectedItem;
            DisplayItems.ChildElementPropertyChanged += SyncSelectedItem;

            TrinitySetting.OnUserRequestedReset -= OnUserRequestedReset;
            TrinitySetting.OnUserRequestedReset += OnUserRequestedReset;
        }

        /// <summary>
        /// Reset is called many times for many reasons, we need to only reset selected
        /// when the user has clicked the reset button in the settings window
        /// because running UpdateSelectedItems() 4-5 times during load is costly.
        /// </summary>
        private void OnUserRequestedReset()
        {
            SelectedItems.Clear();
            CreateView();
            UpdateSelectedItems();
        }

        #endregion

        #region Enums

        public enum GroupingType
        {
            None,
            BaseType,
            ItemType,
            SetName,
            IsEquipped,
            ClassRestriction,
            IsSetItem,
            IsCrafted,
            IsValid,
            IsLegendaryAffixed,
            IsSelected
        }

        public enum SortingType
        {
            None,
            Name,
            Id
        }

        public enum ModalPage
        {
            None,
            Import,
            Export
        }

        #endregion

        #region Properties

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AlwaysStashAncients
        {
            get { return _alwaysStashAncients; }
            set
            {
                if (_alwaysStashAncients != value)
                {
                    _alwaysStashAncients = value;
                    OnPropertyChanged();
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool AlwaysTrashNonAncients
        {
            get { return _alwaysTrashNonAncients; }
            set
            {
                if (_alwaysTrashNonAncients != value)
                {
                    _alwaysTrashNonAncients = value;
                    OnPropertyChanged();
                }
            }
        }

        [DataMember(IsRequired = false)]
        [DefaultValue(false)]
        public bool UpgradeRules
        {
            get { return _upgradeRules; }
            set
            {
                if (_upgradeRules != value)
                {
                    _upgradeRules = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The CollectionView runs on top of the DisplayItems and adds additional functionality for grouping, sorting and
        /// filtering.
        /// </summary>
        public CollectionViewSource Collection
        {
            get { return _collection; }
            set
            {
                if (_collection != value)
                {
                    _collection = value;
                    OnPropertyChanged();
                }
            }
        }

        public FullyObservableCollection<LItem> ItemTypes
        {
            get
            {
                return _itemTypes;
            }
            set
            {
                if (_itemTypes == null || !_itemTypes.Any())
                {
                    SetField(ref _itemTypes, value);
                    CreateItemTypes();
                }
                SetField(ref _itemTypes, value);
            }
        }

        [IgnoreDataMember]
        public Tab SelectedTab => (Tab)SelectedTabIndex;

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set { SetField(ref _selectedTabIndex, value); }
        }

        public enum Tab
        {
            Legendary,
            ItemType
        }

        /// <summary>
        /// Current grouping
        /// </summary>
        [DataMember]
        public GroupingType Grouping
        {
            get { return _grouping; }
            set
            {
                if (_grouping != value)
                {
                    _grouping = value;
                    OnPropertyChanged();
                    ChangeGrouping(value);
                }
            }
        }

        /// <summary>
        /// Filtering text
        /// </summary>
        [IgnoreDataMember]
        public string FilterText
        {
            get { return _filterText; }
            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsFiltered));
                    ChangeFilterPending(value);
                }
            }
        }

        /// <summary>
        /// If the view is currently filtered
        /// </summary>
        [IgnoreDataMember]
        public bool IsFiltered
        {
            get { return !string.IsNullOrEmpty(FilterText); }
        }

        /// <summary>
        /// Main collection for all items, underlies CollectionViewSource
        /// This is only used for displaying the UI and user interaction.
        /// </summary>
        [IgnoreDataMember]
        public FullyObservableCollection<LItem> DisplayItems
        {
            get { return _displayItems; }
            set
            {
                if (_displayItems != value)
                {
                    _displayItems = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Contains the currently selected items; this is persisted to the settings file.
        /// Code elsewhere in trinity (such as loot engien) can check items against it at any time.
        /// LItems here have a minimal set of data; only Ids of the items and rules are are saved.
        /// </summary>
        [DataMember(IsRequired = false)]
        public List<LItem> SelectedItems
        {
            get { return _selectedItems; }
            set
            {
                if (_selectedItems != value)
                {
                    _selectedItems = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Whether the groupings are automatically expanded
        /// </summary>
        public bool GroupsExpandedByDefault { get; set; }

        /// <summary>
        /// Compressed/Encoded string of selected items and their rules
        /// </summary>
        public string ExportCode
        {
            get { return _exportCode; }
            set
            {
                if (_exportCode != value)
                {
                    _exportCode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Message for Import/Export user information
        /// </summary>
        public string ValidationMessage
        {
            get { return _validationMessage; }
            set
            {
                if (_validationMessage != value)
                {
                    _validationMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Selected panel for the import/export modal
        /// </summary>
        public ModalPage SelectedModalPage
        {
            get { return _selectedModalPage; }
            set
            {
                if (_selectedModalPage != value)
                {
                    _selectedModalPage = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Hides/Shows the modal
        /// </summary>
        public bool IsModalVisible
        {
            get { return _isModalVisible; }
            set
            {
                if (_isModalVisible != value)
                {
                    _isModalVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        public ICommand ResetFilterCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand ImportCommand { get; set; }
        public ICommand LoadModalCommand { get; set; }
        public ICommand CloseModalCommand { get; set; }
        public ICommand EnableItemListCommand { get; set; }
        public ICommand AdvancedOptionCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }
        public ICommand SelectNoneCommand { get; set; }
        public ICommand ClearRulesCommand { get; set; }
        public ICommand AddAllSetsCommand { get; set; }
        public ICommand AddAllLegendaryAffixCommand { get; set; }
        public ICommand Add24ItemsCommand { get; set; }

        public ICommand LoadEquippedItemsCommand { get; set; }
        public ICommand LoadStashedItemsCommand { get; set; }

        public void LoadCommands()
        {
            LoadEquippedItemsCommand = new RelayCommand(parameter =>
            {
                try
                {
                    if (ZetaDia.Me == null || !ZetaDia.IsInGame)
                    {
                        if (BotMain.IsRunning)
                        {
                            Logger.Log("Must be in a game to use this feature");
                        }
                        else
                        {
                            using (ZetaDia.Memory.AcquireFrame())
                            {
                                ZetaDia.Actors.Update();
                                Logger.Log("Scanning Character for Equipped Items");
                                SelectItems(ZetaDia.Me.Inventory.Equipped);
                            }
                        }
                    }
                    else
                    {
                        ZetaDia.Actors.Update();
                        Logger.Log("Scanning Character for Equipped Items");
                        SelectItems(ZetaDia.Me.Inventory.Equipped);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Exception in SelectAllCommand {0}", ex);
                }
            });

            LoadStashedItemsCommand = new RelayCommand(parameter =>
            {
                try
                {
                    if (ZetaDia.Me == null || !ZetaDia.IsInGame)
                    {
                        if (BotMain.IsRunning)
                        {
                            Logger.Log("Must be in a game to use this feature");
                        }
                        else
                        {
                            using (ZetaDia.Memory.AcquireFrame())
                            {
                                ZetaDia.Actors.Update();
                                Logger.Log("Scanning Character for Stashed Items");
                                SelectItems(ZetaDia.Me.Inventory.StashItems);
                            }
                        }
                    }
                    else
                    {
                        ZetaDia.Actors.Update();
                        Logger.Log("Scanning Character for Stashed Items");
                        SelectItems(ZetaDia.Me.Inventory.StashItems);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Exception in SelectAllCommand {0}", ex);
                }
            });

            AdvancedOptionCommand = new RelayCommand(parameter =>
            {
                try
                {
                    if (parameter == null)
                        return;

                    Logger.LogVerbose("AdvancedOptionCommand Fired {0}", parameter.ToString());

                    var item = parameter as ComboBoxItem;
                    var selectedPropertyName = item != null ? item.Tag.ToString() : parameter.ToString();

                    switch (selectedPropertyName)
                    {
                        case "SelectAllCommand":
                            SelectAllCommand.Execute(null);
                            break;

                        case "SelectNoneCommand":
                            SelectNoneCommand.Execute(null);
                            break;

                        case "ClearRulesCommand":
                            ClearRulesCommand.Execute(null);
                            break;

                        case "AddAllSetsCommand":
                            AddAllSetsCommand.Execute(null);
                            break;

                        case "AddAllLegendaryAffixCommand":
                            AddAllLegendaryAffixCommand.Execute(null);
                            break;

                        case "Add24ItemsCommand":
                            Add24ItemsCommand.Execute(null);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Exception in AdvancedOptionCommand: {0} {1}", ex.Message, ex.InnerException);
                }
            });

            SelectAllCommand = new RelayCommand(parameter =>
            {
                try
                {
                    if (SelectedTab == Tab.Legendary)
                    {
                        Logger.Log("Selecting all items");
                        using (Collection.DeferRefresh())
                        {
                            SelectedItems = new List<LItem>(DisplayItems);
                            UpdateSelectedItems();
                        }
                        return;
                    }

                    if (SelectedTab == Tab.ItemType)
                    {
                        Logger.Log("Selecting all item types.");
                        ItemTypes.ForEach(i => i.IsSelected = true);
                    }

                }
                catch (Exception ex)
                {
                    Logger.LogError("Exception in SelectAllCommand {0}", ex);
                }
            });

            Add24ItemsCommand = new RelayCommand(parameter =>
            {
                try
                {
                    Logger.Log("Add24ItemsCommand Not Implemented");
                    AddToSelection(item => PresetLists.Patch24Items.Contains(item.Id));
                }
                catch (Exception ex)
                {
                    Logger.LogError("Exception in SelectAllCommand {0}", ex);
                }
            });

            AddAllSetsCommand = new RelayCommand(parameter =>
            {
                try
                {
                    Logger.Log("AddAllSetsCommand Not Implemented");
                    AddToSelection(item => item.IsSetItem);
                }
                catch (Exception ex)
                {
                    Logger.LogError("Exception in SelectAllCommand {0}", ex);
                }
            });

            AddAllLegendaryAffixCommand = new RelayCommand(parameter =>
            {
                try
                {
                    Logger.Log("Selecting all items with a legendary affix");
                    AddToSelection(item => !string.IsNullOrEmpty(item.LegendaryAffix));
                }
                catch (Exception ex)
                {
                    Logger.LogError("Exception in SelectAllLegendaryAffixCommand {0}", ex);
                }
            });

            SelectNoneCommand = new RelayCommand(parameter =>
            {
                try
                {
                    if (SelectedTab == Tab.Legendary)
                    {
                        Logger.Log("Deselecting all legendary items");
                        using (Collection.DeferRefresh())
                        {
                            SelectedItems = new List<LItem>();
                            CreateView();
                            UpdateSelectedItems();
                        }
                        return;
                    }

                    if (SelectedTab == Tab.ItemType)
                    {
                        using (ItemTypes.ViewSource.DeferRefresh())
                        {
                            foreach (var itemType in ItemTypes)
                            {
                                itemType.IsSelected = false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Exception in SelectNoneCommand {0}", ex);
                }
            });

            ClearRulesCommand = new RelayCommand(parameter =>
            {
                try
                {
                    if (SelectedTab == Tab.Legendary)
                    {
                        Logger.Log("Removing rules from all legendary items.");
                        using (Collection.DeferRefresh())
                        {
                            SelectedItems.ForEach(i => i.Rules = new ObservableCollection<LRule>());
                            UpdateSelectedItems();
                        }
                        return;
                    }

                    if (SelectedTab == Tab.ItemType)
                    {
                        Logger.Log("Removing rules from all item types.");
                        ItemTypes.ForEach(i => i.Rules = new FullyObservableCollection<LRule>());
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("Exception in ClearAllRulesCommand {0}", ex);
                }
            });

            ResetFilterCommand = new RelayCommand(parameter => { FilterText = string.Empty; });

            EnableItemListCommand = new RelayCommand(parameter =>
            {
                Logger.Log("Setting ItemFilterMode to ItemList");
                UILoader.DataContext.Loot.Pickup.ItemFilterMode = ItemFilterMode.ItemList;
            });

            LoadModalCommand = new RelayCommand(parameter =>
            {
                if (parameter == null)
                    return;

                ModalPage page;
                if (Enum.TryParse(parameter.ToString(), out page))
                {
                    if (page != ModalPage.None)
                    {
                        SelectedModalPage = page;
                        IsModalVisible = true;
                    }

                    ExportCode = string.Empty;

                    if (page == ModalPage.Export)
                        ExportCommand.Execute(parameter);
                }

                Logger.Log("Selecting modal content... {0}", parameter.ToString());
            });

            CloseModalCommand = new RelayCommand(parameter => { IsModalVisible = false; });

            ImportCommand = new RelayCommand(parameter =>
            {
                Logger.Log("Importing ItemList...");

                var oldSlected = _selectedItems.Count;

                ImportFromCode(ExportCode);

                Logger.Log("Selected Before = {0} After = {1}", oldSlected, _selectedItems.Count);

                IsModalVisible = false;
            });

            ExportCommand = new RelayCommand(parameter =>
            {
                Logger.Log("Exporting ItemList... {0}", parameter);
                ExportCode = CreateExportCode();
            });
        }

        private void SelectItems(IEnumerable<ACDItem> collection)
        {
            var itemIds = new HashSet<int>(collection.Where(i => i.ItemQualityLevel >= ItemQuality.Legendary).Select(i => i.ActorSnoId));
            AddToSelection(item => itemIds.Contains(item.Id));
        }

        private void AddToSelection(Func<LItem, bool> match)
        {
            var newSelections = new List<LItem>(_selectedItems);
            foreach (var item in DisplayItems)
            {
                if (match(item))
                {
                    newSelections.Add(item);
                    item.IsSelected = true;
                }
            }
            _selectedItems = new List<LItem>(newSelections.DistinctBy(i => i.Id));
            CreateView();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates an export code of the current state
        /// </summary>
        public string CreateExportCode()
        {
            var settingsXml = TrinitySetting.GetSettingsXml(this);
            return ExportHelper.Compress(settingsXml);
        }

        /// <summary>
        /// Decodes an export code and applies it to the current state.
        /// </summary>
        public ItemListSettings ImportFromCode(string code)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrWhiteSpace(code))
            {
                ValidationMessage = "You must enter an import/export code";
                Logger.Log("You must enter an import/export code");
            }
            try
            {
                var decompressedXml = ExportHelper.Decompress(ExportCode);
                var newSettings = TrinitySetting.GetSettingsInstance<ItemListSettings>(decompressedXml);

                Grouping = GroupingType.None;

                using (Collection.DeferRefresh())
                {
                    SelectedItems = newSettings.SelectedItems;
                    UpdateSelectedItems();
                }

                ItemTypes = newSettings.ItemTypes;

            }
            catch (Exception ex)
            {
                ValidationMessage = string.Format("Error importing itemlist. {0} {1}", ex.Message, ex.InnerException);
                Logger.Log("Error importing itemlist. {0} {1}", ex.Message, ex.InnerException);
            }
            return this;
        }

        /// <summary>
        /// Change the grouping property
        /// </summary>
        /// <param name="groupingType"></param>
        internal void ChangeGrouping(GroupingType groupingType)
        {
            if (Collection == null)
                return;

            if (groupingType == GroupingType.IsSelected)
            {
                Collection.LiveGroupingProperties.Clear();
                Collection.LiveGroupingProperties.Add("IsSelected");
                Collection.IsLiveGroupingRequested = true;
            }
            else
            {
                Collection.IsLiveGroupingRequested = false;
            }

            // Prevent the collection from updating until outside of the using block.
            using (Collection.DeferRefresh())
            {
                Collection.GroupDescriptions.Clear();
                if (groupingType != GroupingType.None)
                    Collection.GroupDescriptions.Add(new PropertyGroupDescription(groupingType.ToString()));
            }
        }

        /// <summary>
        /// Change the sorting order
        /// </summary>
        /// <param name="sortingType"></param>
        internal void ChangeSorting(SortingType sortingType)
        {
            if (Collection == null)
                return;

            using (Collection.DeferRefresh())
            {
                Collection.SortDescriptions.Clear();
                Collection.SortDescriptions.Add(new SortDescription(sortingType.ToString(), ListSortDirection.Ascending));
            }
        }

        /// <summary>
        /// Change the search filter when the user stops typing
        /// </summary>
        /// <param name="property"></param>
        internal void ChangeFilterPending(string property)
        {
            if (_deferredAction == null)
                _deferredAction = DeferredAction.Create(ExecuteFilter);

            _deferredAction.Defer(TimeSpan.FromMilliseconds(250));
        }

        /// <summary>
        /// Rebind the filter to get it to fire
        /// </summary>
        private void ExecuteFilter()
        {
            Collection.Filter -= FilterHandler;
            Collection.Filter += FilterHandler;
        }

        /// <summary>
        /// Expression that is run against every item in the collection to filter
        /// </summary>
        private void FilterHandler(object sender, FilterEventArgs e)
        {
            if (string.IsNullOrEmpty(FilterText))
                e.Accepted = true;

            var item = e.Item as LItem;

            if (item == null || string.IsNullOrEmpty(item.Name))
            {
                e.Accepted = false;
            }
            else
            {
                var filterText = FilterText.ToLowerInvariant();

                if (item.Name.ToLowerInvariant().Contains(filterText))
                {
                    e.Accepted = true;
                    return;
                }

                if (item.SetName.ToLowerInvariant().Contains(filterText))
                {
                    e.Accepted = true;
                    return;
                }

                if (item.Id.ToString().Equals(filterText))
                {
                    e.Accepted = true;
                    return;
                }

                e.Accepted = false;
            }
        }

        /// <summary>
        /// Updates the selected collection whenever an object is set to selected.
        /// </summary>
        /// <param name="args"></param>
        public void SyncSelectedItem(ChildElementPropertyChangedEventArgs args)
        {
            var item = args.ChildElement as LItem;
            if (item != null && args.PropertyName.ToString() == "IsSelected")
            {
                var match = _selectedItems.FirstOrDefault(i => i.Id == item.Id);
                if (match != null)
                {
                    if (!item.IsSelected)
                    {
                        // Remove
                        _selectedItems.Remove(match);
                        Logger.LogVerbose("Removed {0} ({2}) from Selected Items, NewSelectedCount={1}", item.Name, _selectedItems.Count, item.Id);
                    }
                }
                else if (match == null && item.IsSelected)
                {
                    _selectedItems.Add(item);
                    Logger.LogVerbose("Added {0} ({2}) to Selected Items, NewSelectedCount={1}", item.Name, _selectedItems.Count, item.Id);
                }
            }
        }

        /// <summary>
        /// Updates the DisplayItems collection to match the Selected collection.
        /// </summary>
        public void UpdateSelectedItems()
        {
            if (_selectedItems == null || _displayItems == null || _collection == null || _collection.View == null || _collection.View.SourceCollection == null)
            {
                Logger.Log("Skipping UpdateSelectedItems due to Null");
                return;
            }

            // Prevent the collection from updating until outside of the using block.
            using (Collection.DeferRefresh())
            {
                var selectedDictionary = _selectedItems.DistinctBy(i => i.Id).ToDictionary(k => k.Id, v => v);
                var castView = _collection.View.SourceCollection.Cast<LItem>();

                castView.ForEach(item =>
                {
                    // After XML settings load _selectedItems will contain LItem husks that are lacking useful information, just what was saved.
                    // We want to take the saved information and make an object that is fully populated and linked with the UI collection.

                    LItem selectedItem;

                    if (selectedDictionary.TryGetValue(item.Id, out selectedItem))
                    {
                        selectedItem.Rules.ForEach(r =>
                        {
                            r.TrinityItemType = item.TrinityItemType;
                            r.ItemStatRange = item.GetItemStatRange(r.ItemProperty);
                        });
                        item.IsSelected = true;
                        item.Rules = selectedItem.Rules;
                        item.Ops = selectedItem.Ops;

                        // Replacing the reference to automatically receive changes from UI.
                        _selectedItems.Remove(selectedItem);
                        _selectedItems.Add(item);
                    }
                    else
                    {
                        if (item.IsSelected)
                        {
                            Logger.LogVerbose("Update: Deselecting {0}", item.Name);
                            item.IsSelected = false;
                        }
                    }
                });
            }
        }

        #endregion

        #region ITrinitySetting

        public void Reset()
        {
            TrinitySetting.Reset(this);
            Initialization();
        }

        public void CopyTo(ItemListSettings setting)
        {
            TrinitySetting.CopyTo(this, setting);
            setting.SelectedItems = SelectedItems;
        }

        public ItemListSettings Clone()
        {
            return TrinitySetting.Clone(this);
        }

        [OnDeserializing]
        internal void OnDeserializingMethod(StreamingContext context)
        {

        }

        #endregion

        #region INotifyPropertyChanged

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void OnSave()
        {
            Logger.Log("Saving ItemList Data");
        }

        public void OnLoaded()
        {
            Logger.Log("Loading ItemList Data");
            CreateView();
            UpdateSelectedItems();
            OnPropertyChanged(nameof(Collection));
        }
    }
}