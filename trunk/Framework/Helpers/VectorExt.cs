using System;
using System.Windows;
using System.Windows.Media.Media3D;
using Zeta.Common;

namespace Trinity.Framework.Helpers
{
    public static class VectorExt
    {
        private const double DegToRad = Math.PI / 180;

        public static Vector2 Rotate(this Vector2 v, double degrees)
        {
            return v.RotateRadians(degrees * DegToRad);
        }

        public static Vector2 RotateRadians(this Vector2 v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector2((float)(ca * v.X - sa * v.Y), (float)(sa * v.X + ca * v.Y));
        }

        public static Vector3 Rotate(this Vector3 v, double degrees)
        {
            return v.RotateRadians(degrees * DegToRad);
        }

        public static Vector3 RotateRadians(this Vector3 v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Vector3((float)(ca * v.X - sa * v.Y), (float)(sa * v.X + ca * v.Y), v.Z);
        }

        public static Point Rotate(this Point p, Point origin, double degrees)
        {
            var theta = degrees * DegToRad;
            var x = Math.Cos(theta) * (p.X - origin.X) - Math.Sin(theta) * (p.Y - origin.Y) + origin.X;
            var y = Math.Sin(theta) * (p.X - origin.X) + Math.Cos(theta) * (p.Y - origin.Y) + origin.Y;
            return new Point(x, y);
        }

        public static Point RotateRadians(this Point v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new Point((float)(ca * v.X - sa * v.Y), (float)(sa * v.X + ca * v.Y));
        }

        public static Point FlipX(this Point p, Point origin)
        {
            return new Point(origin.X - (p.X - origin.X), p.Y);
        }

        public static Point3D FlipX(this Point3D p, Point3D origin)
        {
            return new Point3D(origin.X - (p.X - origin.X), p.Y, p.Z);
        }

        public static Point FlipY(this Point p, Point origin)
        {
            return new Point(p.X, origin.Y - (p.Y - origin.Y));
        }

        public static Point3D FlipY(this Point3D p, Point3D origin)
        {
            return new Point3D(p.X, origin.Y - (p.Y - origin.Y), p.Z);
        }

        public static Point FlipBoth(this Point p, Point origin)
        {
            return new Point(origin.X - (p.X - origin.X), origin.Y - (p.Y - origin.Y));
        }

        public static Vector3 DeltaXY(this Vector3 p, Vector3 other)
        {
            return new Vector3(p.X - other.Z, p.Y - other.Y, p.Z);
        }

        public static Vector3 LerpAddition(this Vector3 p, Vector3 change, double pct)
        {
            return new Vector3(p.X + (float)(change.X * pct), p.Y + (float)(change.Y * pct), p.Z + (float)(change.Z * pct));
        }

        public static Vector3D ToVector3D(this Vector3 p)
        {
            return new Vector3D(p.X,p.Y,p.Z);
        }

        public static Vector3 Invert(this Vector3 p)
        {
            return new Vector3(p.X * -1, p.Y * -1, p.Z * -1);
        }

        public static Point3D ToPoint3D(this Vector3 p)
        {
            return new Point3D(p.X * -1, p.Y * -1, p.Z * -1);
        }

        public static Point3D Rotate(Point3D p, Point3D origin, double degrees)
        {
            var theta = degrees * DegToRad;
            var x = Math.Cos(theta) * (p.X - origin.X) - Math.Sin(theta) * (p.Y - origin.Y) + origin.X;
            var y = Math.Sin(theta) * (p.X - origin.X) + Math.Cos(theta) * (p.Y - origin.Y) + origin.Y;
            var z = p.Z;
            return new Point3D(x, y, z);
        }




    }
}