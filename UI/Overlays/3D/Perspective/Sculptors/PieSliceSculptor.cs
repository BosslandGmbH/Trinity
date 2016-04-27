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

using System.Windows.Media.Media3D;
using Trinity.UI.Overlays._3D.Perspective.Primitives;

namespace Trinity.UI.Overlays._3D.Perspective.Sculptors
{
    /// <summary>
    /// A class to handle points and triangles of a 3D pie slice.
    /// </summary>
    // Contribution by Philippe Jovelin - 14/02/08
    // Refactoring and "sector explosion" by Olivier Dewit - april-may 2008
    public class PieSliceSculptor : Sculptor
    {
        private int _circumferenceSideCount;
        private double _initialAngle;
        private double _angleValue;
        private double _pieRadius;
        private bool _isExploded;
        private double _explosionOffset = SliceSculptor.DefaultExplosionOffset;

        /// <summary>
        /// Initializes a new instance of PieSliceSculptor.
        /// </summary>
        // odewit 19/04/08
        public PieSliceSculptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of PieSliceSculptor.
        /// </summary>
        /// <param name="circumferenceSideCount">Model circumference side count.</param>
        /// <param name="initialAngle">Angle between the axis [origin - first point] and the X-axis, in degrees.</param>
        /// <param name="angleValue">Angle value of the slice.</param>
        /// <param name="pieRadius">Radius of the slice.</param>
        /// <param name="isExploded">Indicates if the sector is exploded (usefull for pie charts).</param>
        /// <param name="explosionOffset">Translation offset of the explosion.</param>
        // odewit 19/04/08
        public PieSliceSculptor(
            int circumferenceSideCount, 
            double initialAngle, 
            double angleValue, 
            double pieRadius,
            bool isExploded,
            double explosionOffset)
        {
            Initialize(
                circumferenceSideCount, 
                initialAngle, 
                angleValue, 
                pieRadius,
                isExploded,
                explosionOffset);
        }

        /// <summary>
        /// Initializes a new instance of PieSliceSculptor.
        /// </summary>
        /// <param name="circumferenceSideCount">Model circumference side count.</param>
        /// <param name="initialAngle">Angle between the axis [origin - first point] and the X-axis, in degrees.</param>
        /// <param name="angleValue">Angle value of the slice.</param>
        /// <param name="pieRadius">Radius of the slice.</param>
        /// <param name="isExploded">Indicates if the sector is exploded (usefull for pie charts).</param>
        /// <param name="explosionOffset">Translation offset of the explosion.</param>
        public void Initialize(
            int circumferenceSideCount, 
            double initialAngle, 
            double angleValue, 
            double pieRadius,
            bool isExploded,
            double explosionOffset)
        {
            _circumferenceSideCount = circumferenceSideCount;
            _initialAngle = initialAngle;
            _angleValue = angleValue;
            _pieRadius = pieRadius;
            _isExploded = isExploded;
            _explosionOffset = explosionOffset;
        }

        /// <summary>
        /// Initializes the Points and Triangles collections.
        /// Called By Sculptor.BuildMesh()
        /// </summary>
        protected override void CreateTriangles()
        {

            SliceSculptor ss1 = new SliceSculptor(
                _circumferenceSideCount, 
                _initialAngle, 
                _angleValue, 
                _pieRadius,
                _isExploded,
                _explosionOffset);
            ss1.BuildTriangles(TriangleSideKind.Back);
            foreach (Point3D p in ss1.Points)
            {
                this.Points.Add(p);
            }
            foreach (Point3DTriplet tpl in ss1.Triangles)
            {
                this.Triangles.Add(tpl);
            }

            SliceSculptor ss2 = new SliceSculptor(
                _circumferenceSideCount, 
                _initialAngle, 
                _angleValue, 
                _pieRadius,
                _isExploded,
                _explosionOffset);
            ss2.Center = new Point3D(
                ss2.Center.X,
                ss2.Center.Y,
                ss2.Center.Z + 1.0);
            for (int i = 0; i < ss2.Points.Count; i++)
            {
                ss2.Points[i] = new Point3D(
                    ss2.Points[i].X,
                    ss2.Points[i].Y,
                    ss2.Points[i].Z + 1.0);
                this.Points.Add(ss2.Points[i]);
            }
            ss2.BuildTriangles(TriangleSideKind.Front);
            foreach (Point3D p in ss2.Points)
            {
                this.Points.Add(p);
            }
            foreach (Point3DTriplet tpl in ss2.Triangles)
            {
                this.Triangles.Add(tpl);
            }

            for (int i = 0; i < ss1.Points.Count; i++)
            {
                if (i < ss1.Points.Count - 1)
                {
                    // rounded side
                    this.Triangles.Add(new Point3DTriplet(ss1.Points[i], ss2.Points[i + 1], ss2.Points[i]));
                    this.Triangles.Add(new Point3DTriplet(ss1.Points[i], ss1.Points[i + 1], ss2.Points[i + 1]));
                }
                else
                {
                    // closing sides
                    this.Triangles.Add(new Point3DTriplet(ss1.Center, ss1.Points[0], ss2.Points[0]));
                    this.Triangles.Add(new Point3DTriplet(new Point3D(ss1.Center.X, ss1.Center.Y, 1), ss1.Center, ss2.Points[0]));

                    this.Triangles.Add(new Point3DTriplet(ss1.Center, ss2.Points[i], ss1.Points[i]));
                    this.Triangles.Add(new Point3DTriplet(new Point3D(ss1.Center.X, ss1.Center.Y, 1), ss2.Points[i], ss1.Center));
                }
            }
        }
    }
}
