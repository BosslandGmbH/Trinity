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
using System.Windows.Media.Media3D;
using Trinity.UI.Overlays._3D.Perspective.Primitives;

namespace Trinity.UI.Overlays._3D.Perspective.Sculptors
{
    /// <summary>
    /// A class to handle points and triangles of a 3D flat slice.
    /// </summary>
    // Contribution by Philippe Jovelin - 14/02/08 (ex- TruncatedPolygonSculptor)
    // Refactoring and "sector explosion" by Olivier Dewit - april-may 2008
    public class SliceSculptor : PolygonSculptor
    {
        /// <summary>
        /// Initializes a new instance of SliceSculptor.
        /// </summary>
        /// <param name="circumferenceSideCount">Model circumference side count.</param>
        /// <param name="initialAngle">Angle between the axis [origin - first point] and the X-axis, in degrees.</param>
        /// <param name="angleValue">Angle value of the slice.</param>
        /// <param name="sliceRadius">Radius of the slice.</param>
        /// <param name="isExploded">Indicates if the sector is exploded (usefull for pie charts).</param>
        /// <param name="explosionOffset">Translation offset of the explosion.</param>
        public SliceSculptor(
            int circumferenceSideCount, 
            double initialAngle, 
            double angleValue, 
            double sliceRadius,
            bool isExploded,
            double explosionOffset)
        {
            InitialAngle = initialAngle;
            _angleValue = angleValue;
            _sliceRadius = sliceRadius;
            _isExploded = isExploded;
            _explosionOffset = explosionOffset;
            CircumferenceSideCount = circumferenceSideCount;
            CreatePoints();
        }

        private double _sliceRadius;

        /// <summary>
        /// Gets or Sets the radius of the slice
        /// (radius of the pseudo-circle containing the slice).
        /// </summary>
        public double SliceRadius
        {
            get { return _sliceRadius; }
            set { _sliceRadius = value; }
        }

        private double _angleValue;

        /// <summary>
        /// Gets or sets the angle value of the slice.
        /// </summary>
        public double AngleValue
        {
            get { return _angleValue; }
            set { _angleValue = value; }
        }

        private bool _isExploded;

        /// Indicates if the sector is exploded (usefull for pie charts).
        /// Default value is false.
        public bool IsExploded
        {
            get { return _isExploded; }
            set { _isExploded = value; }
        }

        /// <summary>
        /// Default value for ExplosionOffset property.
        /// </summary>
        public const double DefaultExplosionOffset = 0.15;

        private double _explosionOffset = DefaultExplosionOffset;

        /// <summary>
        /// Gets or sets the offset of the translation due to the explosion (when IsExploded is true).
        /// Default value is 0.15.
        /// </summary>
        public double ExplosionOffset
        {
            get { return _explosionOffset; }
            set { _explosionOffset = value; }
        }

        /// <summary>
        /// Create the points of the slice.
        /// </summary>
        protected override void CreatePoints()
        {
            if (CircumferenceSideCount < 3)
            {
                throw new ArgumentException("_circumferenceSideCount < 3");
            }

            // odewit
            TranslateTransform3D explosionTransform = null;
            if (_isExploded)
            {
                double angleRad = GeometryHelper.DegreeToRadian(InitialAngle + AngleValue / 2.0);
                double offsetX = Math.Cos(angleRad) * _explosionOffset;
                double offsetY = Math.Sin(angleRad) * _explosionOffset;
                explosionTransform = new TranslateTransform3D(offsetX, offsetY, 0.0);
            }

            this.Points.Clear();
            // Center = new Point3D(0, 0, 0);
            if (_isExploded)
            {
                Center = new Point3D(
                    explosionTransform.OffsetX, 
                    explosionTransform.OffsetY, 
                    0);
            }
            else
            {
                Center = new Point3D(0, 0, 0);
            }
            Centered = true;
            double angle1 = 2 * Math.PI / CircumferenceSideCount;
            double angle = 0.0;
            double initialAngleRad = GeometryHelper.DegreeToRadian(InitialAngle);
            double angleValueRad = GeometryHelper.DegreeToRadian(AngleValue);
            for (int i = 0; i <= CircumferenceSideCount; i++)
            {
                angle = (angle1 * i) + initialAngleRad; // odewit
                Point3D p = new Point3D();
                p.X = Math.Cos(angle) * SliceRadius;
                p.Y = Math.Sin(angle) * SliceRadius;
                p.Z = 0;

                if (initialAngleRad + angleValueRad >= angle) // odewit
                //this.Points.Add(point);
                {
                    if (_isExploded)
                    {
                        Points.Add(explosionTransform.Transform(p));
                    }
                    else
                    {
                        Points.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the Points and Triangles collections.
        /// Called By Sculptor.BuildMesh()
        /// </summary>
        public override void BuildTriangles(TriangleSideKind tsk)
        {
            this.Triangles.Clear();
            if (Points.Count < 3)
            {
                throw new ArgumentException("Points.Count < 3");
            }
            if (!Centered)
            {
                CreateSideTriangles(Points, tsk);
            }
            else
            {
                // faces supérieure et inférieure
                // upper and lower sides
                for (int i = 0; i <= Points.Count - 1; i++)
                {
                    if (tsk == TriangleSideKind.Front)
                    {
                        if (i < Points.Count - 1)
                        {
                            this.Triangles.Add(new Point3DTriplet(Center, Points[i], Points[i + 1]));
                        }
                    }
                    else
                    {
                        if (i < Points.Count - 1)
                        {
                            this.Triangles.Add(new Point3DTriplet(Center, Points[i + 1], Points[i]));
                        }
                    }
                }
            }
        }
    }
}
