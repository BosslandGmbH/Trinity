//------------------------------------------------------------------
//
//  For licensing information and to get the latest version go to:
//  http://www.codeplex.com/perspective
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY
//  OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//  LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//  FITNESS FOR A PARTICULAR PURPOSE.
//
//------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Trinity.UI.Overlays._3D.Perspective
{
    public enum Handedness3D
    {
        /// <summary>
        /// A left handed (LH) coordinate system
        /// </summary>
        LeftHanded,
        /// <summary>
        /// A right handed (RH) coordinate system
        /// </summary>
        RightHanded
    }

    public enum AxisDirection
    {
        /// <summary>
        /// The X axis.
        /// </summary>
        X,
        /// <summary>
        /// The Y axis.
        /// </summary>
        Y,
        /// <summary>
        /// The Z axis.
        /// </summary>
        Z
    }

    /// <summary>
    /// A helper class for 3D operations.
    /// </summary>
    public static class Helper3D
    {
        /// <summary>
        /// Duplicate two Point3DCollection objects.
        /// </summary>
        /// <param name="from">Original collection.</param>
        /// <param name="to">Recipient collection.</param>
        public static void ClonePoints(Point3DCollection from, Point3DCollection to)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            to.Clear();
            CopyPoints(from, to);
        }

        /// <summary>
        /// Copy the points of a Point3DCollection objects in an other one.
        /// </summary>
        /// <param name="from">Original collection.</param>
        /// <param name="to">Recipient collection.</param>
        public static void CopyPoints(Point3DCollection from, Point3DCollection to)
        {
            if (from == null)
            {
                throw new ArgumentNullException("from");
            }
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            foreach (Point3D p in from)
            {
                to.Add(p);
            }
        }

        /// <summary>
        /// Convert a degree angle value in radian.
        /// </summary>
        /// <remarks>
        /// DEPRECATED. Use GeometryHelper.DegreeToRadian() instead.
        /// </remarks>
        /// <param name="degree">Angle value in degree.</param>
        /// <returns>Angle value in radian.</returns>
        public static double DegreeToRadian(double degree)
        {
            return (degree / 180.0) * Math.PI;
        }

        /// <summary>
        /// Convert a radian angle value in degree.
        /// </summary>
        /// <remarks>
        /// DEPRECATED. Use GeometryHelper.RadianToDegree() instead.
        /// </remarks>
        /// <param name="radian">Angle value in radian.</param>
        /// <returns>Angle value in degree.</returns>
        public static double RadianToDegree(double radian)
        {
            return radian * 180.0 / Math.PI;
        }


        /// <summary>
        /// Rounds a vertex of a triangle.
        /// </summary>
        /// <param name="pA">First point.</param>
        /// <param name="pB">Second point. Vertex to round</param>
        /// <param name="pC">Third point.</param>
        /// <param name="roundingRate">Vertex rounding rate. The value must be comprized between 0.0 and 0.5.</param>
        /// <returns></returns>
        public static Point3DCollection RoundCorner(Point3D pA, Point3D pB, Point3D pC, double roundingRate)
        {
            if (!((pA.Z == pB.Z) && (pB.Z == pC.Z))
                )
            {
                throw new ArgumentOutOfRangeException("pA");
            }

            if ((roundingRate < 0.0)
                || (roundingRate > 0.5)
                )
            {
                throw new ArgumentOutOfRangeException("roundingRate");
            }

            Point3DCollection points = new Point3DCollection();

            int roundingDefinition = (int)(roundingRate * 40.0);

            Vector3D v1 = new Vector3D();
            v1 = pA - pB;
            v1.X = Math.Round(v1.X, 3);
            v1.Y = Math.Round(v1.Y, 3);
            v1.Z = Math.Round(v1.Z, 3);
            Point3D p1 = Point3D.Add(pB, Vector3D.Multiply(v1, roundingRate));

            Vector3D v2 = new Vector3D();
            v2 = pC - pB;
            v2.X = Math.Round(v2.X, 3);
            v2.Y = Math.Round(v2.Y, 3);
            v2.Z = Math.Round(v2.Z, 3);
            Point3D p2 = Point3D.Add(pB, Vector3D.Multiply(v2, roundingRate));

            // v1 is the normal vector for the linear curve
            // v1.X*x + v1.Y*y + c1 = 0;
            // p1 is owned by this curve so
            double c1 = -(v1.X * p1.X) - (v1.Y * p1.Y);

            // same for v2 and p2
            double c2 = -(v2.X * p2.X) - (v2.Y * p2.Y);

            // center for the arc that owns p1 and p2
            Point3D center = new Point3D();

            if (v1.Y == 0.0)
            {
                if (v1.X == 0.0)
                {
                    throw new InvalidOperationException();
                }
                center.X = -c1 / v1.X;
                if (v2.Y == 0.0)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    center.Y = (-c2 - v2.X * center.X) / v2.Y;
                }
            }
            else
            {
                if (v2.Y == 0.0)
                {
                    if (v2.X == 0.0)
                    {
                        throw new InvalidOperationException();
                    }
                    center.X = -c2 / v2.X;
                }
                else
                {
                    center.X = (c1 / v1.Y - c2 / v2.Y) / (v2.X / v2.Y - v1.X / v1.Y);
                }
                center.Y = (-c1 - v1.X * center.X) / v1.Y;
            }
            center.Z = pB.Z;

            // angle of the arc between p1 and p2
            // 360 - 180 - Vector3D.AngleBetween(v1, v2)
            double angleArc = GeometryHelper.DegreeToRadian(180 - Vector3D.AngleBetween(v1, v2));

            // angle of each part
            double angleStep = angleArc / roundingDefinition;

            Vector3D vRadius = p1 - center;

            double angleBaseDeg = Vector3D.AngleBetween(new Vector3D(1, 0, 0), vRadius);
            // necessar adjustment because of Vector3D.AngleBetween() - see documentation
            if (p1.Y < 0.0)
            {
                angleBaseDeg = 360 - angleBaseDeg;
            }
            double angleBase = GeometryHelper.DegreeToRadian(angleBaseDeg);

            points.Add(p1);
            // points of the arc
            for (int j = 1; j <= roundingDefinition - 1; j++)
            {
                double angle = angleBase + (angleStep * j);
                Point3D p = new Point3D();
                p.X = center.X + Math.Cos(angle) * vRadius.Length;
                p.Y = center.Y + Math.Sin(angle) * vRadius.Length;
                p.Z = pB.Z;
                points.Add(p);
            }
            points.Add(p2);

            return points;
        }

        /// <summary>
        /// Add points and triangle indices to a mesh from a triangle.
        /// Points must be passed counter-clockwise.
        /// Normals are calculated automatically.
        /// </summary>
        /// <param name="mesh">MeshGeometry3D object.</param>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point.</param>
        /// <param name="p3">Third point.</param>
        public static void BuildTriangleMesh(MeshGeometry3D mesh, Point3D p1, Point3D p2, Point3D p3)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException("mesh");
            }

            mesh.Positions.Add(p1);
            int p1Index = mesh.Positions.Count - 1;
            mesh.Positions.Add(p2);
            int p2Index = mesh.Positions.Count - 1;
            mesh.Positions.Add(p3);
            int p3Index = mesh.Positions.Count - 1;

            mesh.TriangleIndices.Add(p1Index);
            mesh.TriangleIndices.Add(p2Index);
            mesh.TriangleIndices.Add(p3Index);

            //Vector3D normal = CalculateNormal(p1, p2, p3);
            //mesh.Normals.Add(normal);
            //mesh.Normals.Add(normal);
            //mesh.Normals.Add(normal);
        }

        //From : 
        //http://www.kindohm.com/technical/WPF3DTutorial.htm
        //http://www.limsi.fr/Individu/jacquemi/IG-TR-7-8-9/surf-maillage-vn.html
        private static Vector3D CalculateNormal(Point3D p1, Point3D p2, Point3D p3)
        {
            Vector3D v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            Vector3D v2 = new Vector3D(p3.X - p2.X, p3.Y - p2.Y, p3.Z - p2.Z);

            return Vector3D.CrossProduct(v1, v2);
        }

        /// <summary>
        /// Maps a texture to a 3D square surface.
        /// </summary>
        /// <param name="mesh">A MeshGeometry object.</param>
        /// <param name="minX">The left X coordinate of the 3D square.</param>
        /// <param name="minY">The lower Y coordinate of the 3D square.</param>
        /// <param name="maxX">The right X coordinate of the 3D square.</param>
        /// <param name="maxY">The upper Y coordinate of the 3D square.</param>
        public static void MapSquareTexture(MeshGeometry3D mesh, double minX, double minY, double maxX, double maxY)
        {
            mesh.TextureCoordinates.Add(new Point(minX, maxY));
            mesh.TextureCoordinates.Add(new Point(maxX, maxY));
            mesh.TextureCoordinates.Add(new Point(maxX, minY));

            mesh.TextureCoordinates.Add(new Point(minX, maxY));
            mesh.TextureCoordinates.Add(new Point(maxX, minY));
            mesh.TextureCoordinates.Add(new Point(minX, minY));
        }

        /// <summary>
        /// Rotation of a point around one of the 3 axes according to a given angle 
        /// </summary>
        /// <param name="point">Point to rotate</param>
        /// <param name="angle">Angle (in radians)</param>
        /// <param name="rotationAxis">Rotation axis : X, Y ou Z</param>
        /// <returns>A new Point3D object corresponding to the rotation</returns>
        public static Point3D RotatePoint(Point3D point, double angle, AxisDirection rotationAxis)
        {
            Vector3D axis = new Vector3D();
            switch (rotationAxis)
            {
                case AxisDirection.X:
                    axis.X = 1.0;
                    axis.Y = 0.0;
                    axis.Z = 0.0;
                    break;

                case AxisDirection.Y:
                    axis.X = 0.0;
                    axis.Y = 1.0;
                    axis.Z = 0.0;
                    break;

                case AxisDirection.Z:
                    axis.X = 0.0;
                    axis.Y = 0.0;
                    axis.Z = 1.0;
                    break;
            }
            return RotatePoint(point, RadianToDegree(angle), axis);
        }

        /// <summary>
        /// Rotation of a point around an axis according to a given angle.
        /// </summary>
        /// <param name="source">Source point.</param>
        /// <param name="angle">Rotation angle (in degree).</param>
        /// <param name="axis">Axis vector.</param>
        /// <returns>A new Point3D object corresponding to the rotation.</returns>
        public static Point3D RotatePoint(Point3D source, double angle, Vector3D axis)
        {
            AxisAngleRotation3D rotation = new AxisAngleRotation3D();
            rotation.Angle = angle;
            rotation.Axis = axis;
            RotateTransform3D transform = new RotateTransform3D(rotation);
            return transform.Transform(source);
        }

        /// <summary>
        /// Rotation of a vector around an axis according to a given angle.
        /// </summary>
        /// <param name="source">Source vector.</param>
        /// <param name="angle">Rotation angle (in degree).</param>
        /// <param name="axis">Axis vector.</param>
        /// <returns>A new Vector3D object corresponding to the rotation</returns>
        public static Vector3D RotateVector(Vector3D source, double angle, Vector3D axis)
        {
            AxisAngleRotation3D rotation = new AxisAngleRotation3D();
            rotation.Angle = angle;
            rotation.Axis = axis;
            RotateTransform3D transform = new RotateTransform3D(rotation);
            return transform.Transform(source);
        }

        //public static Point PointToScreen(Point point, Visual visual)
        //{
        //    MouseDevice

        //    PresentationSource presentationSource = PresentationSource.FromVisual(visual);
        //    if (presentationSource == null)
        //    {
        //        throw new InvalidOperationException(SR.Get(SRID.Visual_NoPresentationSource, new object[0]));
        //    }
        //    GeneralTransform transform = this.TransformToAncestor(presentationSource.RootVisual);
        //    if ((transform == null) || !transform.TryTransform(point, out point))
        //    {
        //        throw new InvalidOperationException(SR.Get(SRID.Visual_CannotTransformPoint, new object[0]));
        //    }
        //    point = PointUtil.RootToClient(point, presentationSource);
        //    point = PointUtil.ClientToScreen(point, presentationSource);
        //    return point;
        //}


        /// <summary>
        /// Returns the viewport of a Visual3D
        /// (Inspired by VisualTreeHelper.GetContainingVisual2D(DependencyObject reference))
        /// </summary>
        /// <param name="visual">A Visual3D object.</param>
        /// <returns>A Viewport3D object.</returns>
        public static Viewport3D GetViewport3D(Visual3D visual)
        {
            Viewport3D viewport = null;
            DependencyObject d = (DependencyObject)visual;
            while (d != null)
            {
                viewport = d as Viewport3D;
                if (viewport != null)
                {
                    return viewport;
                }
                d = VisualTreeHelper.GetParent(d);
            }
            return viewport;
        }

        /// <summary>
        /// Creates a 3D scaling matrix.
        /// </summary>
        /// <param name="scaleX">The x-axis scale factor.</param>
        /// <param name="scaleY">The y-axis scale factor.</param>
        /// <param name="scaleZ">The z-axis scale factor.</param>
        /// <returns>A Matrix3D object.</returns>
        public static Matrix3D GetScaleMatrix(double scaleX, double scaleY, double scaleZ)
        {
            //var m = new Matrix3D();
            //m.Scale(new Vector3D(scaleX, scaleY, scaleZ));
            //return m;
            return new Matrix3D
            {
                M11 = scaleX,
                M22 = scaleY,
                M33 = scaleZ
            };
        }

        /// <summary>
        /// Creates a 3D scaling matrix.
        /// </summary>
        /// <param name="scaleFactor">The scale factor.</param>
        /// <returns>A Matrix3D object.</returns>
        public static Matrix3D GetScaleMatrix(double scaleFactor)
        {
            return new Matrix3D
            {
                M11 = scaleFactor,
                M22 = scaleFactor,
                M33 = scaleFactor
            };
        }

        /// <summary>
        /// Creates a 3D translation matrix.
        /// </summary>
        /// <param name="offsetX">The distance to translate along the x-axis.</param>
        /// <param name="offsetY">The distance to translate along the y-axis.</param>
        /// <param name="offsetZ">The distance to translate along the z-axis.</param>
        /// <returns>A Matrix3D object.</returns>
        public static Matrix3D GetTranslationMatrix(double offsetX, double offsetY, double offsetZ)
        {
            //var m = new Matrix3D();
            //m.Translate(new Vector3D(offsetX, offsetY, offsetZ));
            //return m;
            return new Matrix3D
            {
                OffsetX = offsetX,
                OffsetY = offsetY,
                OffsetZ = offsetZ
            };
        }

        /// <summary>
        /// Creates a 3D X-rotation matrix.
        /// </summary>
        /// <remarks>For a right-handed 3D system (like WPF), a positive angle value results in a counter-clockwise rotation around the axis. For a left-handed 3D system, a positive angle value results in a clockwise rotation around the axis.</remarks>
        /// <param name="angle">Angle value in degree.</param>
        /// <param name="handedness">The handedness of the coordinate system (optional). Under WPF, the defaut value is Right-handed.</param>
        /// <returns>A Matrix3D object.</returns>
        public static Matrix3D GetXRotationMatrix(double angle, Handedness3D handedness = Handedness3D.RightHanded)
        {
            //var m = new Matrix3D();
            //m.Rotate(new Quaternion(new Vector3D(1, 0, 0), angle));
            //return m;

            // var radianAngle = GeometryHelper.DegreeToRadian(angle);
            double radianAngle = 0.0;
            if (handedness == Handedness3D.LeftHanded)
            {
                radianAngle = (2 * Math.PI) - GeometryHelper.DegreeToRadian(angle);
            }
            else
            {
                radianAngle = GeometryHelper.DegreeToRadian(angle);
            }
            return new Matrix3D
            {
                M22 = Math.Cos(radianAngle),
                M32 = -Math.Sin(radianAngle),
                M23 = Math.Sin(radianAngle),
                M33 = Math.Cos(radianAngle)
            };
        }

        /// <summary>
        /// Creates a 3D Y-rotation matrix.
        /// </summary>
        /// <remarks>For a right-handed 3D system (like WPF), a positive angle value results in a counter-clockwise rotation around the axis. For a left-handed 3D system, a positive angle value results in a clockwise rotation around the axis.</remarks>
        /// <param name="angle">Angle value in degree.</param>
        /// <param name="handedness">The handedness of the coordinate system (optional). Under WPF, the defaut value is Right-handed.</param>
        /// <returns>A Matrix3D object.</returns>
        public static Matrix3D GetYRotationMatrix(double angle, Handedness3D handedness = Handedness3D.RightHanded)
        {
            //var m = new Matrix3D();
            //m.Rotate(new Quaternion(new Vector3D(0, 1, 0), angle));
            //return m;

            double radianAngle = 0.0;
            // angle inverted in respect to the relative orientation of Z to X
            // var radianAngle = (2 * Math.PI) - GeometryHelper.DegreeToRadian(angle);
            if (handedness == Handedness3D.LeftHanded)
            {
                radianAngle = GeometryHelper.DegreeToRadian(angle);
            }
            else
            {
                radianAngle = (2 * Math.PI) - GeometryHelper.DegreeToRadian(angle);
            }
            return new Matrix3D
            {
                M11 = Math.Cos(radianAngle),
                M31 = -Math.Sin(radianAngle),
                M13 = Math.Sin(radianAngle),
                M33 = Math.Cos(radianAngle)
            };
        }

        /// <summary>
        /// Creates a 3D Z-rotation matrix.
        /// </summary>
        /// <remarks>For a right-handed 3D system (like WPF), a positive angle value results in a counter-clockwise rotation around the axis. For a left-handed 3D system, a positive angle value results in a clockwise rotation around the axis.</remarks>
        /// <param name="angle">Angle value in degree.</param>
        /// <param name="handedness">The handedness of the coordinate system (optional). Under WPF, the defaut value is Right-handed.</param>
        /// <returns>A Matrix3D object.</returns>
        public static Matrix3D GetZRotationMatrix(double angle, Handedness3D handedness = Handedness3D.RightHanded)
        {
            //var m = new Matrix3D();
            //m.Rotate(new Quaternion(new Vector3D(0, 0, 1), angle));
            //return m;

            // var radianAngle = GeometryHelper.DegreeToRadian(angle);
            double radianAngle = 0.0;
            if (handedness == Handedness3D.LeftHanded)
            {
                radianAngle = (2 * Math.PI) - GeometryHelper.DegreeToRadian(angle);
            }
            else
            {
                radianAngle = GeometryHelper.DegreeToRadian(angle);
            }
            return new Matrix3D
            {
                M11 = Math.Cos(radianAngle),
                M21 = -Math.Sin(radianAngle),
                M12 = Math.Sin(radianAngle),
                M22 = Math.Cos(radianAngle)
            };
        }
    }
}
