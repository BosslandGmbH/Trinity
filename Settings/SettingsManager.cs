using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Trinity.Framework.Objects;
using Trinity.Settings.Modals;
using Trinity.UI;
using Trinity.UI.UIComponents;


namespace Trinity.Settings
{
    public class SettingsManager
    {
        private static List<IDynamicSetting> _settings = new List<IDynamicSetting>();

        public static void Add(IDynamicSetting dynamicSettings)
        {
            _settings.Add(dynamicSettings);
        }

        public static void AddRange(IEnumerable<IDynamicSetting> dynamicSettings)
        {
            _settings.AddRange(dynamicSettings);
        }

        public static IEnumerable<IDynamicSetting> GetDynamicSettings()
        {
            var result = new List<IDynamicSetting>();
            result.AddRange(RoutineManager.Instance.DynamicSettings);
            result.AddRange(ModuleManager.DynamicSettings);
            result.AddRange(TrinitySettings.Settings.DynamicSettings);
            result.AddRange(_settings);
            return result;
        }

        public static string SaveDirectory => Path.Combine(FileManager.SettingsPath, "Saved");

        public static TrinityStorage GetCurrentSettingsForExport(IEnumerable<SettingsSelectionItem> sections = null)
        {
            return new TrinityStorage();
        }

        public static string GetCurrrentSettingsExportCode(IEnumerable<SettingsSelectionItem> sections = null)
        {
            return string.Empty;
        }

        /// <summary>
        /// Handle the process of exporting a settings file
        /// Fired when user clicks the Export button on Trinity settings window.
        /// </summary>
        public static ICommand ExportSettingsCommand => new RelayCommand(param =>
        {
            try
            {
                SettingsSelectionViewModel selectionViewModel;
                if (TryGetExportSelections(out selectionViewModel))
                {
                    var filePath = GetSaveFilePath();
                    if (string.IsNullOrEmpty(filePath))
                        return;

                    Core.Logger.Log($"保存文件到 {filePath}");
                    var exportSettings = new TrinityStorage();
                    UILoader.DataContext.ViewStorage.CopyTo(exportSettings);

                    UpdateSections("导出", exportSettings, selectionViewModel);
                    exportSettings.SaveToFile(filePath);
                }                
            }
            catch (Exception ex)
            {
                Core.Logger.Error($"在加载设置命令中的异常  {ex}");
            }
        });

        /// <summary>
        /// Handle the process of importing a settings file
        /// Fired when user clicks the Import button on Trinity settings window.
        /// </summary>
        public static ICommand ImportSettingsCommand => new RelayCommand(param =>
        {
            try
            {
                var filePath = GetLoadFilePath();
                if (string.IsNullOrEmpty(filePath))
                    return;

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"{filePath} not found");

                Core.Logger.Log($"加载文件: {filePath}");

                // Load settings into Settings window view model only
                // User still has to click save for it to actually be applied.    

                var settings = TrinityStorage.GetSettingsFromFile(filePath);
                var importedSections = GetSections(settings);
                SettingsSelectionViewModel selectionViewModel;

                if (TryGetImportSelections(importedSections, out selectionViewModel))
                {
                    UpdateSections("导入", settings, selectionViewModel);
                    UILoader.DataContext.LoadSettings(settings);
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Error($"在加载设置命令中的异常  {ex}");
            }
        });

        /// <summary>
        /// Get user to pick a filename and location for saving.
        /// </summary>
        private static string GetSaveFilePath()
        {
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Xml Files (.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 1,
                Title = "Save Settings Xml File",
                InitialDirectory = SaveDirectory,
                OverwritePrompt = true,
            };

            var userClickedOk = saveFileDialog.ShowDialog();
            if (userClickedOk == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return null;
        }

        /// <summary>
        /// Get user to pick a file to load
        /// </summary>
        private static string GetLoadFilePath()
        {
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Xml Files (.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 1,
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                InitialDirectory = SaveDirectory
            };
            var userClickedOk = openFileDialog.ShowDialog();
            if (userClickedOk == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        /// <summary>
        /// Try to get user to pick settings sections for import.
        /// </summary>
        public static bool TryGetImportSelections(HashSet<SettingsSelectionItem> importedParts, out SettingsSelectionViewModel selectionViewModel)
        {
            var dataContext = new SettingsSelectionViewModel
            {
                Title = "他人设置 代码导入",
                Description = "只有勾上的选项内容才会被导入.倒序排列依次是探索、策略、装备列表、战斗、卡奈魔盒、巅峰加点、冒险、高级、权重、物品、躲避、冒险、最后是各职业特定策略",
                OkButtonText = "导入",
            };

            EnableSections(importedParts, dataContext);

            var window = UILoader.CreateNonModalWindow(
                "Modals\\SettingsSelection.xaml",
                "导入设置",
                dataContext,
                350,
                225
            );

            return OpenSelectionsDialog(out selectionViewModel, dataContext, window);
        }

        /// <summary>
        /// Try to get user to pick settings sections for export.
        /// </summary>
        public static bool TryGetExportSelections(out SettingsSelectionViewModel selectionViewModel)
        {
            var dataContext = new SettingsSelectionViewModel
            {
                Title = "当前设置 代码导出",
                Description = "只有勾上的选项内容才会被导出.倒序排列依次是探索、策略、装备列表、战斗、卡奈魔盒、巅峰加点、冒险、高级、权重、物品、躲避、冒险、最后是各职业特定策略",
                OkButtonText = "导出",
            };

            EnableSections(SettingsSelectionViewModel.GetDefaultSelections(), dataContext);

            var window = UILoader.CreateNonModalWindow(
                "Modals\\SettingsSelection.xaml",
                "导出设置",
                dataContext,
                350,
                225
            );

            return OpenSelectionsDialog(out selectionViewModel, dataContext, window);
        }

        /// <summary>
        /// Open dialog for user to pick which sections of the settings to do something with.
        /// </summary>
        private static bool OpenSelectionsDialog(out SettingsSelectionViewModel selectionViewModel, SettingsSelectionViewModel dataContext, Window window)
        {
            dataContext.Selections = dataContext.Selections.OrderByDescending(s => s.IsEnabled).ToList();
            window.Owner = UILoader.ConfigWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            dataContext.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(dataContext.IsWindowOpen) && !dataContext.IsWindowOpen)
                    window.Close();
            };

            window.ContentRendered += (o, args) => dataContext.IsWindowOpen = true;
            window.Closed += (o, args) => dataContext.IsWindowOpen = false;
            window.ShowDialog();
            selectionViewModel = dataContext;
            return selectionViewModel.DialogResult == DialogResult.OK;
        }

        /// <summary>
        /// Look through a TrinitySetting object and return a list of the sections that are populated with data.
        /// </summary>
        public static HashSet<SettingsSelectionItem> GetSections(TrinityStorage storages)
        {
            var result = new HashSet<SettingsSelectionItem>();
            if (storages.Dynamic?.Settings != null)
            {
                foreach (var item in storages.Dynamic.Settings)
                {
                    result.Add(new SettingsSelectionItem(SettingsSection.Dynamic, item.Name));
                }
            }
                
            Core.Logger.Log($"文件包含 {result.Count} 部分: {string.Join(", ", result)}");
            return result;
        }

        public static void RemoveSections(TrinityStorage storages, IEnumerable<SettingsSelectionItem> sections)
        {
            if (sections != null)
            {
                foreach (var section in sections)
                {
                    ClearSection(storages, section);
                }
            }
        }

        /// <summary>
        /// Clear the specified parts of a TrinitySetting object so that they have no data.
        /// </summary>
        public static void UpdateSections(string actionDescripter, TrinityStorage storages, SettingsSelectionViewModel selectionsViewModel)
        {
            foreach (var sectionEntry in selectionsViewModel.Selections)
            {
                if (sectionEntry.IsSelected)
                {
                    if(!string.IsNullOrEmpty(actionDescripter))
                        Core.Logger.Log($"{actionDescripter} 部分: {sectionEntry}");

                    continue;
                }
                ClearSection(storages, sectionEntry);
            }
        }

        private static void ClearSection(TrinityStorage storages, SettingsSelectionItem sectionDefinition)
        {
            switch (sectionDefinition.Section)
            {
                case SettingsSection.Dynamic:
                    var foundItem = storages.Dynamic?.Settings.FirstOrDefault(s => s.Name == sectionDefinition.SectionName);
                    if (foundItem != null)
                    {
                        storages.Dynamic.Settings.Remove(foundItem);
                    }
                    break;
            }
        }

        /// <summary>
        /// Make checkboxes on the selections dialog enabled and clickable. (they default to disabled).
        /// </summary>
        private static void EnableSections(ICollection<SettingsSelectionItem> validSections, SettingsSelectionViewModel selectionViewModel)
        {
            foreach (var item in selectionViewModel.Selections)
            {
                if (validSections.Any(s => s.SectionName == item.SectionName))
                {
                    item.IsEnabled = true;
                    item.IsSelected = true;
                }
            }
        }
    }
}
