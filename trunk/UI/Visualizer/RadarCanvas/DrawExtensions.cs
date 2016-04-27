using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Adventurer.Game.Exploration;
using Trinity.Combat.Abilities;
using Trinity.DbProvider;
using Trinity.Framework;
using Trinity.Framework.Avoidance;
using Trinity.Framework.Grid;
using Trinity.Objects;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;
using LineSegment = System.Windows.Media.LineSegment;
using Logger = Trinity.Technicals.Logger;
using Edge = System.Tuple<int, int>;

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
