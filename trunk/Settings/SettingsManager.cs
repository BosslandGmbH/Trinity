using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Trinity.Config;
using Trinity.Helpers;
using Trinity.Technicals;
using Trinity.UI;
using Trinity.UI.UIComponents;
using Trinity.UIComponents;

namespace Trinity.Settings
{
    public class SettingsManager
    {
        public static string SaveDirectory => Path.Combine(FileManager.SettingsPath, "Saved");

        public static ICommand SaveAsSettingsCommand => new RelayCommand(param =>
        {
            try
            {
                var filePath = GetSaveFilePath();
                if (string.IsNullOrEmpty(filePath))
                    return;

                Logger.LogNormal($"Saving file to {filePath}");
                UILoader.DataContext.ViewModel.SaveToFile(filePath);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception in LoadSettingsCommand {ex}");
            }
        });

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

        public static ICommand LoadSettingsCommand => new RelayCommand(param =>
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
                var newSettings = new TrinitySetting();
                newSettings.LoadSettingsFromFile(filePath);
                UILoader.DataContext.LoadSettings(newSettings);
                
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception in LoadSettingsCommand {ex}");
            }            
        });

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

    }
}
