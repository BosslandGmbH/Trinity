using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using Zeta.Common;
using Zeta.Game.Internals.SNO;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Trinity.Components.Adventurer.UI.UIComponents.RadarCanvas
{
    /// <summary>
    /// Useful tools for drawing stuff
    /// </summary>
    public static class DrawingUtilities
    {
        /// <summary>
        /// Convert to a Drawing System.Drawing.Color
        /// </summary>
        public static Color ToDrawingColor(this System.Windows.Media.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Convert to a System.Windows.Media.Color
        /// </summary>
        public static System.Windows.Media.Color ToMediaColor(this Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Point ToCanvasPoint(this Vector3 positionVector, CanvasData canvasData = null)
        {
            return new PointMorph(positionVector, canvasData ?? CanvasData.LastCanvas).Point;
        }

        public static Point ToCanvasPoint(this Point point, CanvasData canvasData = null)
        {
            return new PointMorph(point.ToVector3(), canvasData ?? CanvasData.LastCanvas).Point;
        }

        public static void RelativeMove(DrawingGroup group, Vector3 origin, CanvasData canvasData = null)
        {
            var originPoint = new PointMorph(origin, CanvasData.LastCanvas);
            var transform = new TranslateTransform(originPoint.Point.X - CanvasData.LastCanvas.Center.X, originPoint.Point.Y - CanvasData.LastCanvas.Center.Y);
            group.Transform = transform;
        }

        public static System.Drawing.Point ToDrawingPoint(this Point point)
        {
            return new System.Drawing.Point((int)point.X, (int)point.Y);
        }

        public static Point ToPoint(this Vector3 worldVector)
        {
            return new Point(worldVector.X, worldVector.Y);
        }

        public static Vector3 ToVector3(this Point point)
        {
            return new Vector3((float)point.X, (float)point.Y, 0);
        }

        public static Rect ToCanvasRect(this AABB bounds)
        {
            var max = bounds.Max;
            var min = bounds.Min;
            var center = new Vector3((int)(max.X - (max.X - min.X) / 2), (int)(max.Y - (max.Y - min.Y) / 2), 0);
            var width = Math.Abs(max.X - min.X);
            var height = Math.Abs(max.Y - min.Y);
            return new Rect(center.ToCanvasPoint(), new Size((int)width, (int)height));
        }

        public static Point Center(this Rectangle rect)
        {
            return new Point(rect.Left + rect.Width / 2,
                             rect.Top + rect.Height / 2);
        }

        private static Dictionary<ushort,double> _glyphWidths = new Dictionary<ushort, double>();
        private static GlyphTypeface _glyphTypeface;
        public static GlyphRun CreateGlyphRun(string text, double size, Point position)
        {
            if (_glyphTypeface == null)
            {
                Typeface typeface = new Typeface("Arial");
                if (!typeface.TryGetGlyphTypeface(out _glyphTypeface))
                    throw new InvalidOperationException("No glyphtypeface found");                
            }

            ushort[] glyphIndexes = new ushort[text.Length];
            double[] advanceWidths = new double[text.Length];

            var totalWidth = 0d;
            double glyphWidth;

            for (int n = 0; n < text.Length; n++)
            {
                ushort glyphIndex = (ushort)(text[n] - 29);
                glyphIndexes[n] = glyphIndex;

                if (!_glyphWidths.TryGetValue(glyphIndex, out glyphWidth))
                {
                    glyphWidth = _glyphTypeface.AdvanceWidths[glyphIndex] * size;
                    _glyphWidths.Add(glyphIndex, glyphWidth);
                }
                advanceWidths[n] = glyphWidth;
                totalWidth += glyphWidth;
            }

            var offsetPosition = new Point(position.X - (totalWidth / 2), position.Y - 10 - size);

            GlyphRun glyphRun = new GlyphRun(_glyphTypeface, 0, false, size, glyphIndexes, offsetPosition, advanceWidths, null, null, null, null, null, null);

            return glyphRun;
        }

    }
}
