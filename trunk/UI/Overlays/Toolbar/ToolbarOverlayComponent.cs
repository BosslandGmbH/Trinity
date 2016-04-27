using System.IO;
using System.Windows.Controls;
using Buddy.Overlay.Controls;
using Trinity.Framework.Helpers;
using Trinity.Technicals;
using Zeta.Game;

namespace Trinity.UI.Overlays
{
    // Shows a button that can clicked and a label on top that can be used to drag the button around on screen.
    public class ToolbarOverlayComponent : OverlayBase
    {
        public static ToolbarOverlayComponent CreateNew()
        {
            return new ToolbarOverlayComponent();
        }

        public ToolbarOverlayComponent() : base(true) { }

        private OverlayControl _control;
        private ToolbarOverlayViewModel DataContext;

        public override OverlayControl Control
        {
            get
            {
                if (DataContext == null)
                    DataContext = new ToolbarOverlayViewModel(); 

                if (_control == null)
                {
                    var toolbarXamlPath = Path.Combine(FileManager.UiPath, "Overlays", "Toolbar", "ToolbarOverlay.xaml");
                    if (!File.Exists(toolbarXamlPath))
                        throw new FileNotFoundException(string.Format("{0} not found", toolbarXamlPath));

                    var toolbarControl = UILoader.LoadAndTransformXamlFile<Control>(toolbarXamlPath);
                    toolbarControl.DataContext = DataContext;

                    const int controlWidth = 200;
                    const int controlHeight = 25;
                    
                    _control = new OverlayControl
                    {
                        X = ZetaDia.Overlay.UnscaledOverlayWidth / 2 - controlWidth,
                        Y = controlHeight,
                        Content = toolbarControl, 
                        AllowMoving = false,
                        AllowResizing = false
                    };
                }
                return _control;
            }
        }
    }
}
