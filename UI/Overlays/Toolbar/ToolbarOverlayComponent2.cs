using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using Buddy.Overlay.Controls;
using Trinity.Framework.Helpers;
using Trinity.Technicals;
using Zeta.Game;

namespace Trinity.UI.Overlays
{

    public class ToolbarOverlayComponent2 : OverlayBase
    {
        public static ToolbarOverlayComponent2 CreateNew()
        {
            return new ToolbarOverlayComponent2();
        }

        public ToolbarOverlayComponent2() : base(true)
        {
            ShouldScale = true;
        }

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

                    var myViewbox = new Viewbox
                    {
                        StretchDirection = StretchDirection.Both,
                        Stretch = Stretch.UniformToFill,
                        Child = toolbarControl
                    };

                    _control = new OverlayControl
                    {
                        X = ZetaDia.Overlay.UnscaledOverlayWidth / 2 - myViewbox.ActualWidth / 2,
                        Y = myViewbox.ActualHeight,
                        Content = myViewbox, 
                        AllowResizing = true                        
                    };
                   
                }
                return _control;
            }
        }
    }
}
