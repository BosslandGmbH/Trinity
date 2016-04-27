using System.Collections.Concurrent;

namespace Trinity.UI.UIComponents.RadarCanvas
{
    public enum DrawingType
    {
        Scene = 0,
        NodeArea,
    }

    internal static class Drawings
    {
        public static ConcurrentDictionary<string, RelativeDrawing> Relative = new ConcurrentDictionary<string, RelativeDrawing>();
        public static ConcurrentDictionary<string, StaticDrawing> Static = new ConcurrentDictionary<string, StaticDrawing>();        
    }
}
