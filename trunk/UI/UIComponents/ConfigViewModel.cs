using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Trinity.Components.Adventurer;
using Trinity.Components.Adventurer.UI;
using Trinity.Components.Combat;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Trinity.Items;
using Trinity.Items.ItemList;
using Trinity.Routines;
using Trinity.Settings;
using Trinity.Settings.Loot;
using Trinity.Settings.Paragon;
using Zeta.Bot;
using Zeta.Game;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Trinity.UI.UIComponents
{
    public class ConfigViewModel
    {
        private readonly TrinityStorage _storage;
        private readonly TrinityStorage _originalStorage;
        private SettingsModel _model;

        public TrinityStorage ViewStorage => _storage;

        private void SaveSettings(TrinityStorage storage)
        {
            storage.CopyTo(_originalStorage);
            _originalStorage.Save();
                             
            TrinityPlugin.SetBotTicksPerSecond();
        }

        public void LoadSettings(TrinityStorage model, IEnumerable<string> ignorePropertyNames = null)
        {
            TrinityStorage.CopyTo(model, _storage, ignorePropertyNames);
            _storage.LoadDynamicSettings();
            _storage.FireOnLoadedEvents();    
        }

        public System.Windows.Controls.UserControl AdventurerSettings
        {
            get
            {
                var advSettings = Adventurer.Instance as IDynamicSetting;
                return new System.Windows.Controls.UserControl
                {
                    Content = advSettings.GetControl(),
                    DataContext = advSettings.GetDataContext()
                };
            }
        }

        public System.Windows.Controls.UserControl AvoidanceSettings
        {
            get
            {
                var advSettings = Core.Avoidance as IDynamicSetting;
                return new System.Windows.Controls.UserControl
                {
                    Content = advSettings.GetControl(),
                    DataContext = advSettings.GetDataContext()
                };
            }
        }

        public ItemSettings Items => _model.Items;
        public WeightingSettings Weighting => _model.Weighting;
        public AdvancedSettings Advanced => _model.Advanced;
        public KanaisCubeSetting KanaisCube => _model.KanaisCube;
        public ParagonSettings Paragon => _model.Paragon;
        public CombatSettings Combat => _model.Combat;
        public ItemListSettings ItemList => _model.ItemList;
        public RoutineSettings Routine => _model.Routine;
        public GameInfo GameInfo => GameInfo.Instance;

        public ConfigViewModel(TrinityStorage trinityStorage, SettingsModel model)
        {
            try
            {
                _model = model;
                _originalStorage = trinityStorage;
                _storage = new TrinityStorage();
                _originalStorage.CopyTo(_storage);
                InitializeResetCommand();

                SaveCommand = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            SaveSettings(_storage);
                            UILoader.CloseWindow();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log("Exception in UI SaveCommand {0}", ex);
                        }
                    });
                DumpBackpackCommand = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            Logger.Log(
                                "\n############################################\n"
                                + "\nDumping Backpack Items. This will hang your client. Please wait....\n"
                                + "##########################");
                            UILoader.CloseWindow();
                            DebugUtil.DumpItems(DebugUtil.DumpItemLocation.Backpack);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogCategory.UserInformation, "Exception dumping Backpack: {0}", ex);
                        }
                    });
                DumpQuickItemsCommand = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            Logger.Log(
                                "\n############################################\n"
                                + "\nQuick Dumping Items. This will hang your client. Please wait....\n"
                                + "##########################");
                            UILoader.CloseWindow();
                            DebugUtil.DumpQuickItems();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogCategory.UserInformation, "Exception Quick Dumping: {0}", ex);
                        }
                    });
                DumpAllItemsCommand = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            Logger.Log(
                                "\n############################################\n"
                                + "\nDumping ALL Items. This will hang your client. Please wait....\n"
                                + "##########################");
                            UILoader.CloseWindow();
                            DebugUtil.DumpItems(DebugUtil.DumpItemLocation.All);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogCategory.UserInformation, "Exception Dumping ALL Items: {0}", ex);
                        }
                    });
                DumpSkillsAndItemsCommand = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            UILoader.CloseWindow();
                            DebugUtil.LogBuildAndItems(TrinityLogLevel.Info);                          
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogCategory.UserInformation, "Exception Dumping Skill/Rune/Passive Items: {0}", ex);
                        }
                    });
                DumpInvalidItemsCommand = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            UILoader.CloseWindow();
                            DebugUtil.LogInvalidItems(TrinityLogLevel.Info);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogCategory.UserInformation, "Exception Dumping Invalid Items: {0}", ex);
                        }
                    });
                DumpItemSNOReference = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            UILoader.CloseWindow();
                            DebugUtil.DumpItemSNOReference();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogCategory.UserInformation, "Exception in DumpItemSNOReference: {0}", ex);
                        }
                    });
                DumpReferenceItems = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            UILoader.CloseWindow();
                            DebugUtil.DumpReferenceItems();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogCategory.UserInformation, "Exception in DumpReferenceItems: {0}", ex);
                        }
                    });                
                DumpMerchantItemsCommand = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            Logger.Log(
                                "\n############################################\n"
                                + "\nDumping Merchant Items. This will hang your client. Please wait....\n"
                                + "##########################");
                            UILoader.CloseWindow();
                            DebugUtil.DumpItems(DebugUtil.DumpItemLocation.Merchant);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogCategory.UserInformation, "Exception dumping Merchant: {0}", ex);
                        }
                    });
                DumpEquippedCommand = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            Logger.Log(
                                "\n############################################\n"
                                + "\nDumping Equipped Items. This will hang your client. Please wait....\n"
                                + "##########################");
                            UILoader.CloseWindow();
                            DebugUtil.DumpItems(DebugUtil.DumpItemLocation.Equipped);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogCategory.UserInformation, "Exception dumping Equipped: {0}", ex);
                        }
                    });
                DumpGroundItemsCommand = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            Logger.Log(
                                "\n############################################\n"
                                + "\nDumping Ground Items. This will hang your client. Please wait....\n"
                                + "##########################");
                            UILoader.CloseWindow();
                            DebugUtil.DumpItems(DebugUtil.DumpItemLocation.Ground);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogCategory.UserInformation, "Exception dumping Ground: {0}", ex);
                        }
                    });
                DumpStashCommand = new RelayCommand(
                    parameter =>
                    {
                        try
                        {
                            Logger.Log(
                                "\n############################################\n"
                                + "\nDumping Stash Items. This will hang your client. Please wait....\n"
                                + "##########################");
                            UILoader.CloseWindow();
                            DebugUtil.DumpItems(DebugUtil.DumpItemLocation.Stash);
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(LogCategory.UserInformation, "Exception dumping Stash: {0}", ex);
                        }
                    });
                TestScoreCommand = new RelayCommand(
                    parameter =>
                    {

                    });
                HelpLinkCommand = new RelayCommand(
                    parameter =>
                    {
                        var link = parameter as string;
                        if (!string.IsNullOrWhiteSpace(link))
                        {
                            Process.Start(link);
                        }
                    });
            }
            catch (Exception ex)
            {
                Logger.LogError("Error creating TrinityPlugin View Model {0}", ex);
            }
        }

        public ICommand DumpSkillsCommand { get; private set; }
        public ICommand LoadItemRuleSetCommand { get; private set; }
        public ICommand HelpLinkCommand { get; private set; }
        public ICommand TestScoreCommand { get; private set; }
        public ICommand DumpBackpackCommand { get; private set; }
        public ICommand DumpQuickItemsCommand { get; private set; }
        public ICommand DumpAllItemsCommand { get; private set; }
        public ICommand DumpSkillsAndItemsCommand { get; private set; }
        public ICommand DumpInvalidItemsCommand { get; private set; }
        public ICommand DumpItemSNOReference { get; private set; }
        public ICommand DumpReferenceItems { get; private set; }
        public ICommand DumpEquippedLegendaryCommand { get; private set; }
        public ICommand DumpMerchantItemsCommand { get; private set; }
        public ICommand DumpEquippedCommand { get; private set; }
        public ICommand DumpGroundItemsCommand { get; private set; }
        public ICommand DumpStashCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand ResetTownRunCommand { get; private set; }
        public ICommand ResetItemCommand { get; private set; }
        public ICommand ResetItemRulesCommand { get; private set; }
        public ICommand ReloadScriptRulesCommand { get; private set; }
        public ICommand ResetAdvancedCommand { get; private set; }
        public ICommand ResetAllCommand { get; private set; }


        internal static Grid MainWindowGrid()
        {
            return (Application.Current.MainWindow.Content as Grid);
        }

        private void InitializeResetCommand()
        {
            try
            {      
                ResetAllCommand = new RelayCommand(
                    parameter => _storage.UserRequestedReset());
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception initializing commands {0}", ex);
            }
        }

    }
}