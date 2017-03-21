using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Trinity.UI.Visualizer.RadarCanvas
{
    public static class DrawExtensions
    {
        private static readonly Pen DefaultPen;
        private static readonly SolidColorBrush DefaultBrush;

        static DrawExtensions ()
        {
            DefaultBrush = new SolidColorBrush(Colors.White);
            DefaultPen = new Pen(DefaultBrush, 1);            
        }

        public static void DrawPolygon(this DrawingContext dc, IEnumerable<Point> points, Brush brush = null, Pen pen = null)
        {
            var drawing = new DrawingGroup();
            points = points.ToList();

            if (!points.Any())
                return;

            pen = pen ?? DefaultPen;

            using (var groupdc = drawing.Open())
            {
                var streamGeometry = new StreamGeometry();
                using (var geometryContext = streamGeometry.Open())
                {
                    geometryContext.BeginFigure(points.First(), true, true);
                    geometryContext.PolyLineTo(points.Skip(1).ToList(), true, true);
                }

                var outlinedGeom = streamGeometry.GetOutlinedPathGeometry();
                groupdc.DrawGeometry(brush, pen, outlinedGeom);
            }
            dc.DrawDrawing(drawing);
        }
    }
}
