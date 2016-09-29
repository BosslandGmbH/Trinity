using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Zeta.Common;

namespace Trinity.UI.Visualizer.RadarCanvas
{
    /// <summary>
    /// Houses canvas information, so a bunch of structs can be accessed by reference.
    /// </summary>
    public class CanvasData
    {
        /// <summary>
        /// Updates the canvas information (for if canvas size changes)
        /// </summary>
        public void Update(Size canvasSize, int gridSize)
        {
            if (canvasSize.Height == 0 || canvasSize.Width == 0)
            {
                return;
            }
                

            var previousSize = CanvasSize;

            Center = new Point(canvasSize.Width / 2, canvasSize.Height / 2);

            GobalRotationAngle = -135;

            CanvasSize = canvasSize;
            Grid = new Size((int)(canvasSize.Width / gridSize), (int)(canvasSize.Height / gridSize));
            GridSquareSize = new Size(gridSize, gridSize);
            LastCanvas = this;

            ClipRegion = new RectangleGeometry(new Rect(new Point(20, 20), new Size(canvasSize.Width - 40, canvasSize.Height - 40)));

            if (OnCanvasSizeChanged != null && !previousSize.Equals(CanvasSize))
                OnCanvasSizeChanged(previousSize, CanvasSize);
        }

        /// <summary>
        /// Center of the canvas (in pixels)
        /// </summary>
        public Point Center;

        /// <summary>
        /// Size of the canvas (in pixels)
        /// </summary>
        public Size CanvasSize;

        /// <summary>
        /// Size of the canvas (in grid squares)
        /// </summary>
        public Size Grid;

        /// <summary>
        /// Size of a single grid square
        /// </summary>
        public Size GridSquareSize;

        /// <summary>
        /// Angle to rotate all points
        /// </summary>
        public int GobalRotationAngle;

        /// <summary>
        /// The world space vector3 for the center for the canvas
        /// </summary>
        public Vector3 CenterVector { get; set; }

        /// <summary>
        /// Last instance for acess via extensions/properties.
        /// </summary>
        public static CanvasData LastCanvas { get; set; }
        
        public delegate void CanvasSizeChanged(Size sizeBefore, Size sizeAfter);

        public event CanvasSizeChanged OnCanvasSizeChanged;

        public RectangleGeometry ClipRegion { get; set; }

        public PointMorph CenterMorph { get; set; }

        public Point PanOffset { get; set; }
        public double Scale { get; set; }

        public Point3D CenterOffset = new Point3D();
    }
}
