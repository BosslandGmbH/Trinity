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
using System.Windows.Media.Media3D;
using Trinity.UI.Overlays._3D.Perspective.Primitives;

namespace Trinity.UI.Overlays._3D.Perspective.Sculptors
{
    /// <summary>
    /// A class to handle points and triangles of a polygonal model.
    /// Default radius is 1.0.
    /// </summary>
    public class PolygonSculptor : Sculptor
    {
        private Point3DCollection _notRoundedPoints = new Point3DCollection();

        /// <summary>
        /// Initializes a new instance of PolygonSculptor.
        /// </summary>
        public PolygonSculptor()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of PolygonSculptor.
        /// </summary>
        /// <param name="points">Points collection</param>
        public PolygonSculptor(Point3DCollection points)
            : base(points)
        {
        }

        /// <summary>
        /// Initializes a new instance of PolygonSculptor.
        /// </summary>
        /// <param name="points">Array of points</param>
        public PolygonSculptor(params Point3D[] points)
            : base(points)
        {
        }

        /// <summary>
        /// Initializes a new instance of PolygonSculptor.
        /// Vertices are automatically generated
        /// </summary>
        /// <param name="circumferenceSideCount">Side count</param>
        /// <param name="initialAngle">Angle between the axis [origin - first point] and the X-axis, in degrees</param>
        public PolygonSculptor(int circumferenceSideCount, double initialAngle)
        {
            _initialAngle = initialAngle;
            _circumferenceSideCount = circumferenceSideCount;
            CreatePoints();
        }

        /// <summary>
        /// Initializes a new instance of PolygonSculptor.
        /// Vertices are automatically generated
        /// </summary>
        /// <param name="circumferenceSideCount">Side count</param>
        /// <param name="initialAngle">Angle between the axis [origin - first point] and the X-axis, in degrees</param>
        /// <param name="roundingRate">Angle rounding rate. The value must be comprized between 0.0 and 0.5.</param>
        public PolygonSculptor(int circumferenceSideCount, double initialAngle, double roundingRate)
        {
            Initialize(circumferenceSideCount, initialAngle, roundingRate);
        }

        /// <summary>
        /// Initializes an existing instance of PolygonSculptor.
        /// Vertices are automatically generated
        /// </summary>
        /// <param name="circumferenceSideCount">Side count</param>
        /// <param name="initialAngle">Angle between the axis [origin - first point] and the X-axis, in degrees</param>
        /// <param name="roundingRate">Angle rounding rate. The value must be comprized between 0.0 and 0.5.</param>
        public void Initialize(int circumferenceSideCount, double initialAngle, double roundingRate)
        {
            _initialAngle = initialAngle;
            _roundingRate = roundingRate;
            _circumferenceSideCount = circumferenceSideCount;
            CreatePoints();
            // BuildTriangles(); // done by BasicModelVisual3D.BuildGeometry call
        }

        /// <summary>
        /// Initializes the Points and Triangles collections.
        /// Called By Sculptor.BuildMesh()
        /// </summary>
        protected override void CreateTriangles()
        {
            if (Points.Count == 0)
            {
                // CreatePoints(_circumferenceSideCount);
                CreatePoints();
            }
            if ((Triangles.Count == 0) &&
                (Points.Count > 0))
            {
                BuildTriangles();
            }
        }

        private Point3D _center;

        /// <summary>
        /// The polygon's center point.
        /// </summary>
        public Point3D Center
        {
            get { return _center; }
            set { _center = value; }
        }

        private double _initialAngle;

        /// <summary>
        /// Initial angle (first point), in degrees.
        /// </summary>
        public double InitialAngle
        {
            get { return _initialAngle; }
            set { _initialAngle = value; }
        }
       
        private bool _centered;

        /// <summary>
        /// Indicates if the sculptor has a center point.
        /// </summary>
        protected bool Centered
        {
            get { return _centered; }
            set { _centered = value; }
        } 

        private double _roundingRate;

        /// <summary>
        /// Gets or sets the angle rounding rate.
        /// The value must be comprized between 0.0 and 0.5.
        /// </summary>
        public double RoundingRate
        {
            get { return _roundingRate; }
            set
            {
                if ((value < 0.0) || (value > 0.5))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                else
                {
                    _roundingRate = value;
                }
            }
        }

        private int _circumferenceSideCount;

        /// <summary>
        /// Gets or sets the side count of the circumference.
        /// </summary>
        protected int CircumferenceSideCount
        {
            get { return _circumferenceSideCount; }
            set { _circumferenceSideCount = value; }
        } 

        /// <summary>
        /// Create the points of the polygon.
        /// </summary>
        protected virtual void CreatePoints()
        {
            if (_circumferenceSideCount < 3)
            {
                throw new ArgumentException("_circumferenceSideCount < 3");
            }
            this.Points.Clear();
            _center = new Point3D(0, 0, 0);
            _centered = true;
            double angle1 = 2 * Math.PI / _circumferenceSideCount;
            double angle = 0.0;
            for (int i = 1; i <= _circumferenceSideCount; i++)
            {
                // angle = (angle1 * i);
                angle = (angle1 * i) + GeometryHelper.DegreeToRadian(_initialAngle);
                Point3D p = new Point3D();
                p.X = Math.Cos(angle);
                p.Y = Math.Sin(angle);
                p.Z = 0;
                this.Points.Add(p);
            }
        }

        /// <summary>
        /// Reinitialize not rounded points list, if necessary
        /// (i.e. after a Points initialization).
        /// </summary>
        /// <seealso cref="RoundCorner"/>
        public void InitializeNotRoundedPointList()
        {
            Helper3D.ClonePoints(Points, _notRoundedPoints);
        }

        private bool _rounded;

        /// <summary>
        /// Round the polygon's corners.
        /// </summary>
        internal void RoundCorner()
        {
            if (_roundingRate > 0.0)
            {

                if (!_rounded)
                {
                    // ne doit être exécutée en principe qu'une fois
                    // pour permettre la binding et
                    // pour éviter les arrondis d'arrondis...
                    // inconvénient : si de nouveaux points sont ajoutés à PointList, on ne peut pas les arrondir
                    // la méthode publique InitializeNotRoundedPointList() est donc fournie pour forcer la réinitialisation
                    InitializeNotRoundedPointList();
                }

                Point3DCollection points = new Point3DCollection();
                for (int i = 0; i <= _notRoundedPoints.Count - 1; i++)
                {
                    Point3D pA = new Point3D();
                    Point3D pB = new Point3D();
                    Point3D pC = new Point3D();

                    pB = _notRoundedPoints[i];
                    if (i >= 1)
                    {
                        pA = _notRoundedPoints[i - 1];
                    }
                    else if (i == 0)
                    {
                        pA = _notRoundedPoints[_notRoundedPoints.Count - 1];
                    }
                    if (i < _notRoundedPoints.Count - 1)
                    {
                        pC = _notRoundedPoints[i + 1];
                    }
                    else if (i == _notRoundedPoints.Count - 1)
                    {
                        pC = _notRoundedPoints[0];
                    }

                    Point3DCollection pl = Helper3D.RoundCorner(pA, pB, pC, _roundingRate);
                    Helper3D.CopyPoints(pl, points);
                }
                Helper3D.ClonePoints(points, Points);
                _rounded = true;
            }
        }

        //double _circumference = 0.0;

        ///// <summary>
        ///// Gets the circumference length of the polygon (including round corners).
        ///// The value is calculated only if needed.
        ///// </summary>
        //public double Circumference
        //{
        //    get
        //    {
        //        if (_circumference == 0.0)
        //        {
        //            CalcCircumference();
        //        }
        //        return _circumference;
        //    }
        //}

        //private void CalcCircumference()
        //{
        //    _circumference = 0.0;
        //    for (int i = 0; i < Points.Count - 1; i++)
        //    {
        //        Point3D p1;
        //        if (i == 0)
        //        {
        //            p1 = Points[Points.Count - 1];
        //        }
        //        else
        //        {
        //            p1 = Points[i - 1];
        //        }
        //        Vector3D visual = Points[i] - p1;
        //        _circumference += visual.Length;
        //    }
        //}

        /// <summary>
        /// Build the triangles.
        /// </summary>
        public void BuildTriangles()
        {
            BuildTriangles(TriangleSideKind.Front);
        }

        /// <summary>
        /// Build the triangles, with a specific side orientation.
        /// </summary>
        /// <param name="tsk">TriangleSideKind value (triangle orientation).</param>
        public virtual void BuildTriangles(TriangleSideKind tsk)
        {
            RoundCorner();
            // _circumference = 0.0; // forces a refresh of the Circumference property
            this.Triangles.Clear();
            if (Points.Count < 3)
            {
                throw new ArgumentException("Points.Count < 3");
            }
            if (!_centered)
            {
                CreateSideTriangles(Points, tsk);
            }
            else
            {
                // center based triangles
                for (int i = 0; i <= Points.Count - 1; i++)
                {
                    if (tsk == TriangleSideKind.Front)
                    {
                        if (i < Points.Count - 1)
                        {
                            this.Triangles.Add(new Point3DTriplet(_center, Points[i], Points[i + 1]));
                        }
                        else
                        {
                            this.Triangles.Add(new Point3DTriplet(_center, Points[i], Points[0]));
                        }
                    }
                    else
                    {
                        if (i < Points.Count - 1)
                        {
                            this.Triangles.Add(new Point3DTriplet(_center, Points[i + 1], Points[i]));
                        }
                        else
                        {
                            this.Triangles.Add(new Point3DTriplet(_center, Points[0], Points[i]));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A method for building the TextureCoordinates collection of the mesh.
        /// </summary>
        internal override void MapTexture()
        {
            foreach(Point3DTriplet pt in this.Triangles)
            {
                foreach (Point3D p in pt.Points)
                {
                    Mesh.TextureCoordinates.Add(
                        new Point(p.X, -p.Y));
                }
            }
        }
    }
}
