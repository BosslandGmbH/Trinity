using System.IO;
using System.Windows.Controls;
using Buddy.Overlay.Controls;
using Trinity.Framework.Helpers;
using Trinity.Technicals;

namespace Trinity.UI.Overlays
{
    public class Viewport3DComponent
    {
        private UserControl _control;
        private ViewPortModel _dataContext;

        public UserControl Control
        {
            get
            {
                if (_dataContext == null)
                    _dataContext = new ViewPortModel();

                if (_control == null)
                {
                    var toolbarXamlPath = Path.Combine(FileManager.UiPath, "Overlays", "3D", "Viewport.xaml");

                    if (!File.Exists(toolbarXamlPath))
                        throw new FileNotFoundException(string.Format("{0} not found", toolbarXamlPath));

                    _control = UILoader.LoadAndTransformXamlFile<UserControl>(toolbarXamlPath);

                    _control.DataContext = _dataContext;
                }

                return _control;
            }
        }
    }
}
