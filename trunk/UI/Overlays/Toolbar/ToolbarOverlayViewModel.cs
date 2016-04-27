using System.Windows;
using System.Windows.Input;
using Trinity.UI.UIComponents;
using Trinity.UIComponents;

namespace Trinity.UI.Overlays
{
    public class ToolbarOverlayViewModel
    {
        public ICommand LaunchRaderWindowCommand { get; set; }
        public ICommand LaunchTrinitySettingsCommand { get; set; }

        public ToolbarOverlayViewModel()
        {
            //LaunchRaderWindowCommand = new RelayCommand(param =>
            //{
            //    Application.Current.Dispatcher.Invoke(CacheUI.ToggleRadarWindow);                       
            //});

            LaunchTrinitySettingsCommand = new RelayCommand(param =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UILoader.GetDisplayWindow().Show();
                });
            });
        }

        public string ImagePathSettings 
        {
            get { return "Images\\settings.png"; }
        }

        public string ImagePathRadar
        {
            get { return "Images\\radar.png"; }
        }

    }
}
