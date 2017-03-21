using System;
using Trinity.Framework.Helpers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Trinity.UI.Visualizer.RadarCanvas
{
    /// <summary>
    /// Provides basic hit testing
    /// </summary>
    public class RadarHitTestUtility
    {
        public Dictionary<Tuple<int,int>, HitContainer> HitStorage = new Dictionary<Tuple<int, int>, HitContainer>();

        public void AddEllipse(RadarObject radarObject, Point point, double x, double y)
        {
            var elipse = new EllipseGeometry
            {
                Center = point,
                RadiusX = x,
                RadiusY = y,
            };

            var key = new Tuple<int, int>((int)point.X, (int)point.Y);

            var hit = new HitContainer
            {
                Geometry = elipse,
                Bounds = elipse.Bounds,
                RadarObject = radarObject
            };

            if(!HitStorage.ContainsKey(key))
                HitStorage.Add(key, hit);
        }

        /// <summary>
        /// Gets first radar object whose center point is within distance of point.
        /// </summary>
        public HitContainer GetSimpleHit(Point point, int distance = 5)
        {
            foreach (var hit in HitStorage)
            {
                if (Math.Abs(hit.Key.Item1 - point.X) <= distance && Math.Abs(hit.Key.Item2 - point.Y) <= distance)
                {
                    hit.Value.Intersection = IntersectionDetail.NotCalculated;
                    return hit.Value;
                }
            }
            return default(HitContainer);
        }

        /// <summary>
        /// Gets first radar object that contains point
        /// </summary>
        public HitContainer GetHit(Point point)
        {
            var PreCheckDistance = 50;

            foreach (var hit in HitStorage)
            {
                if (Math.Abs(hit.Key.Item1 - point.X) <= PreCheckDistance && Math.Abs(hit.Key.Item2 - point.Y) <= PreCheckDistance)
                {
                    if (hit.Value.Geometry.FillContains(point))
                    {
                        hit.Value.Intersection = IntersectionDetail.FullyInside;
                        return hit.Value;
                    }                    
                }
            }
            return default(HitContainer);
        }

        /// <summary>
        /// Gets first radar object that intersects with geometry
        /// </summary>
        public HitContainer GetHit(Geometry geometry)
        {
            var PreCheckDistance = 50;
            var GeometryCheckTolerance = 1;
            var center = geometry.Bounds.Center();

            foreach (var hit in HitStorage)
            {
                if (Math.Abs(hit.Key.Item1 - center.X) <= PreCheckDistance && Math.Abs(hit.Key.Item2 - center.Y) <= PreCheckDistance)
                {
                    var result = hit.Value.Geometry.FillContainsWithDetail(geometry, GeometryCheckTolerance, ToleranceType.Absolute);
                    if (result != IntersectionDetail.Empty)
                    {
                        hit.Value.Intersection = result;
                        return hit.Value;
                    }
                }
            }
            return default(HitContainer);
        }

        public class HitContainer
        {
            public RadarObject RadarObject { get; set; }
            public Geometry Geometry { get; set; }
            public Rect Bounds { get; set; }
            public IntersectionDetail Intersection { get; set; }
        }

        public void Clear()
        {
            HitStorage.Clear();
        }
    }
}