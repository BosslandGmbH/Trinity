using System.Collections.Concurrent;
using IronPython.Modules;

namespace Trinity.UI.Visualizer.RadarCanvas
{
    public enum DrawingType
    {
        Scene = 0,
        NodeArea,
    }

    internal static class Drawings
    {
        public static ConcurrentDictionary<string, RelativeDrawing> Relative { get; } = new ConcurrentDictionary<string, RelativeDrawing>();   
    }
}
