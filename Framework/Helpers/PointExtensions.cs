using System;
using System.Windows;

namespace Trinity.Framework.Helpers
{
    public static class PointExtensions
    {
        public static double DistanceToPoint(this Point point, Point point2)
        {
            return Math.Sqrt((point2.X - point.X) * (point2.X - point.X) + (point2.Y - point.Y) * (point2.Y - point.Y));
        }

        public static double SquaredDistanceToPoint(this Point point, Point point2)
        {
            return (point2.X - point.X) * (point2.X - point.X) + (point2.Y - point.Y) * (point2.Y - point.Y);
        }

        public static bool IsBetweenTwoPoints(this Point targetPoint, Point point1, Point point2)
        {
            double minX = Math.Min(point1.X, point2.X);
            double minY = Math.Min(point1.Y, point2.Y);
            double maxX = Math.Max(point1.X, point2.X);
            double maxY = Math.Max(point1.Y, point2.Y);

            double targetX = targetPoint.X;
            double targetY = targetPoint.Y;

            return minX.LessThanOrClose(targetX)
                   && targetX.LessThanOrClose(maxX)
                   && minY.LessThanOrClose(targetY)
                   && targetY.LessThanOrClose(maxY);
        }
    }
}