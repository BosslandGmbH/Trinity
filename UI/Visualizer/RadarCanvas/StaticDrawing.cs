using System.Windows.Media;

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
        public int WorldId { get; set; }
    }
}
