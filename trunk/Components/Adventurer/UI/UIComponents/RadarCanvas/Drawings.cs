using System.Collections.Concurrent;

namespace Trinity.Components.Adventurer.UI.UIComponents.RadarCanvas
{
    internal static class Drawings
    {
        public static ConcurrentDictionary<string, RelativeDrawing> Relative = new ConcurrentDictionary<string, RelativeDrawing>();
        public static ConcurrentDictionary<string, StaticDrawing> Static = new ConcurrentDictionary<string, StaticDrawing>();
    }
}
