using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;
using Trinity.Config;
using Trinity.Framework;
using Trinity.Helpers;
using Trinity.Technicals;
using Trinity.UI;
using Trinity.UI.RadarUI;
using Trinity.UI.UIComponents;
using Trinity.UIComponents;

namespace Trinity.Settings
{
    public class SettingsManager
    {
        public static string SaveDirectory => Path.Combine(FileManager.SettingsPath, "Saved");

        public static TrinitySetting GetCurrentSettingsForExport(IEnumerable<SettingsSelectionItem> sections = null)
        {
            var settings = new TrinitySetting();
            Core.Settings.CopyTo(settings);
            settings.Notification = null;

            if (sections != null)
            {
                RemoveSections(settings, sections);
            }

            return settings;
        }

        public static string GetCurrrentSettingsExportCode(IEnumerable<SettingsSelectionItem> sections = null)
        {
            var settings = GetCurrentSettingsForExport(sections);
            var xml = TrinitySetting.GetSettingsXml(settings);
            var code = ExportHelper.Compress(xml);;
            return code;
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

                    Logger.LogNormal($"Saving file to {filePath}");
                    var exportSettings = new TrinitySetting();
                    UILoader.DataContext.ViewModel.CopyTo(exportSettings);

                    UpdateSections("Exporting", exportSettings, selectionViewModel);
                    exportSettings.SaveToFile(filePath);
                }                
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception in LoadSettingsCommand {ex}");
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

                Logger.LogNormal($"Loading file: {filePath}");

                // Load settings into Settings window view model only
                // User still has to click save for it to actually be applied.    

                var settings = TrinitySetting.GetSettingsFromFile(filePath);
                var importedSections = GetSections(settings);
                SettingsSelectionViewModel selectionViewModel;

                if (TryGetImportSelections(importedSections, out selectionViewModel))
                {
                    UpdateSections("Importing", settings, selectionViewModel);
                    UILoader.DataContext.LoadSettings(settings);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception in LoadSettingsCommand {ex}");
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
                Title = "Sections to Import",
                Description = "Only the sections selected below will be imported.",
                OkButtonText = "Import",
            };

            EnableSections(importedParts, dataContext);

            var window = UILoader.CreateNonModalWindow(
                "Modals\\SettingsSelection.xaml",
                "Import Settings",
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
                Title = "Sections to Export",
                Description = "Only the sections selected below will be exported.",
                OkButtonText = "Export",
            };

            EnableSections(SettingsSelectionViewModel.GetDefaultSelections(), dataContext);

            var window = UILoader.CreateNonModalWindow(
                "Modals\\SettingsSelection.xaml",
                "Export Settings",
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
        public static HashSet<SettingsSelectionItem> GetSections(TrinitySetting settings)
        {
            var result = new HashSet<SettingsSelectionItem>();
            if (settings.Combat != null)
                result.Add(new SettingsSelectionItem(SettingsSection.Combat));
            if (settings.Loot?.ItemList != null)
                result.Add(new SettingsSelectionItem(SettingsSection.ItemList));
            if (settings.Gambling != null)
                result.Add(new SettingsSelectionItem(SettingsSection.Gambling));
            if (settings.KanaisCube != null)
                result.Add(new SettingsSelectionItem(SettingsSection.KanaisCube));
            if (settings.Loot?.TownRun != null)
                result.Add(new SettingsSelectionItem(SettingsSection.TownRun));
            if (settings.WorldObject != null)
                result.Add(new SettingsSelectionItem(SettingsSection.Objects));
            if (settings.Paragon != null)
                result.Add(new SettingsSelectionItem(SettingsSection.Paragon));
            if (settings.Advanced != null)
                result.Add(new SettingsSelectionItem(SettingsSection.Advanced));
            if (settings.Avoidance != null)
                result.Add(new SettingsSelectionItem(SettingsSection.Avoidance));
            if (settings.Loot?.Pickup != null)
                 result.Add(new SettingsSelectionItem(SettingsSection.ItemPickup));
            if (settings.Loot?.ItemRules != null)
                result.Add(new SettingsSelectionItem(SettingsSection.ItemRules));

            if (settings.Dynamic?.Settings != null)
            {
                foreach (var item in settings.Dynamic.Settings)
                {
                    result.Add(new SettingsSelectionItem(SettingsSection.Dynamic, item.Name));
                }
            }
                
            Logger.Log($"File contains {result.Count} sections: {string.Join(", ", result)}");
            return result;
        }

        public static void RemoveSections(TrinitySetting settings, IEnumerable<SettingsSelectionItem> sections)
        {
            settings.Notification = null;

            if (sections != null)
            {
                foreach (var section in sections)
                {
                    ClearSection(settings, section);
                }
            }
        }

        /// <summary>
        /// Clear the specified parts of a TrinitySetting object so that they have no data.
        /// </summary>
        public static void UpdateSections(string actionDescripter, TrinitySetting settings, SettingsSelectionViewModel selectionsViewModel)
        {
            settings.Notification = null;

            foreach (var sectionEntry in selectionsViewModel.Selections)
            {
                if (sectionEntry.IsSelected)
                {
                    if(!string.IsNullOrEmpty(actionDescripter))
                        Logger.Log($"{actionDescripter} Section: {sectionEntry}");

                    continue;
                }
                ClearSection(settings, sectionEntry);
            }
        }

        private static void ClearSection(TrinitySetting settings, SettingsSelectionItem sectionDefinition)
        {
            switch (sectionDefinition.Section)
            {
                case SettingsSection.Combat:
                    settings.Combat = null;
                    break;
                case SettingsSection.ItemList:
                    if (settings.Loot != null)
                        settings.Loot.ItemList = null;
                    break;
                case SettingsSection.Gambling:
                    settings.Gambling = null;
                    break;
                case SettingsSection.KanaisCube:
                    settings.KanaisCube = null;
                    break;
                case SettingsSection.ItemPickup:
                    if (settings.Loot != null)
                        settings.Loot.Pickup = null;
                    break;
                case SettingsSection.TownRun:
                    if (settings.Loot != null)
                        settings.Loot.TownRun = null;
                    break;
                case SettingsSection.Objects:
                    settings.WorldObject = null;
                    break;
                case SettingsSection.Paragon:
                    settings.Paragon = null;
                    break;
                case SettingsSection.Advanced:
                    settings.Advanced = null;
                    break;
                case SettingsSection.Avoidance:
                    settings.Avoidance = null;
                    break;
                case SettingsSection.ItemRules:
                    if (settings.Loot != null)
                        settings.Loot.ItemRules = null;
                    break;
                case SettingsSection.Dynamic:
                    var foundItem = settings.Dynamic?.Settings.FirstOrDefault(s => s.Name == sectionDefinition.SectionName);
                    if (foundItem != null)
                    {
                        settings.Dynamic.Settings.Remove(foundItem);
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
