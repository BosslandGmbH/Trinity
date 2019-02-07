using System.Windows.Media;
using Zeta.Game;

namespace Trinity.UI.Visualizer.RadarCanvas
{
    /// <summary>
    /// Object for saving and reusing static drawings.
    /// Static drawings like you might assume, do not move.
    /// Example: Grid Lines.
    /// </summary>
    public class StaticDrawing
    {
        public DrawingGroup Drawing { get; set; }
        public SNOWorld WorldId { get; set; }
    }
}
