using Zeta.Common;

namespace Trinity.UI.Visualizer.RadarCanvas
{
    /// <summary>    
    /// Object for saving and reusing relative drawings.
    /// 
    /// Relative drawings that are composed of multiple parts, have the location of each part baked in relative to each other.
    /// When we save the final drawing the parts can no longer be translated individually. 
    /// For example: a drawing of a scene composed of many navcells.
    /// </summary>
    public class RelativeDrawing : StaticDrawing
    {
        /// <summary>
        /// The origin point is the center position at the time the drawing was created.
        /// At this time the scene was in the correct position on the canvas relative to the center/origin.
        /// So with the drawing's origin we can calculate the correct scene location for any center point.
        /// by using an X/Y translation. DrawingUtilities.RelativeMove().
        /// </summary>
        public PointMorph Origin { get; set; }

        public Vector3 Center { get; set; }

        public DrawingType Type { get; set; }
    }
}
