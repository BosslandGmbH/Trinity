using System.Windows;
using System.Windows.Media;

namespace Trinity.Components.Adventurer.UI.UIComponents.RadarCanvas
{
    /// <summary>
    /// Object for saving and reusing static drawings.
    /// Static drawings like you might assume, do not move.
    /// Example: Grid Lines.
    /// </summary>
    internal class StaticDrawing
    {
        public DrawingGroup Drawing { get; set; }
        public DrawingImage Image { get; set; }
        public Rect ImageRect { get; set; }
        public int WorldId { get; set; }
    }
}
