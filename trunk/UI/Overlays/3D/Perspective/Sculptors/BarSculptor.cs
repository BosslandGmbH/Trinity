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

using System.Windows;
using System.Windows.Media.Media3D;
using Trinity.UI.Overlays._3D.Perspective.Primitives;

namespace Trinity.UI.Overlays._3D.Perspective.Sculptors
{
    /// <summary>
    /// A class to handle points and triangles of a 3D bar model.
    /// Default radius is 1.0.
    /// By default, the direction of the bar is the Z axis, and the length is 1.0.
    /// </summary>
    public class BarSculptor : Sculptor
    {
        private int _circumferenceSideCount;
        private double _initialAngle;
        private double _roundingRate;

        /// <summary>
        /// Initializes a new instance of BarSculptor.
        /// </summary>
        public BarSculptor()
        {
            //TextureOnSide = true;
            TexturePosition = TexturePositions.OnSides;
        }

        /// <summary>
        /// Initializes a new instance of BarSculptor.
        /// </summary>
        /// <param name="circumferenceSideCount">Model circumference side count.</param>
        /// <param name="initialAngle">Angle between the axis [origin - first point] and the X-axis, in degrees.</param>
        /// <param name="roundingRate">Angle rounding rate. The value must be comprized between 0.0 and 0.5.</param>
        public BarSculptor(int circumferenceSideCount, double initialAngle, double roundingRate)
        {
            Initialize(circumferenceSideCount, initialAngle, roundingRate);
        }

        /// <summary>
        /// Initializes an existing instance of BarSculptor.
        /// </summary>
        /// <param name="circumferenceSideCount">Model circumference side count.</param>
        /// <param name="initialAngle">Angle between the axis [origin - first point] and the X-axis, in degrees.</param>
        /// <param name="roundingRate">Angle rounding rate. The value must be comprized between 0.0 and 0.5.</param>
        public void Initialize(int circumferenceSideCount, double initialAngle, double roundingRate)
        {
            _circumferenceSideCount = circumferenceSideCount;
            _initialAngle = initialAngle;
            _roundingRate = roundingRate;
        }

        //public bool TextureOnEnd { get; set; }
        //public bool TextureOnSide { get; set; }

        /// <summary>
        /// Gets or sets the position a a texture. Default is OnSides.
        /// </summary>
        public TexturePositions TexturePosition { get; set; }

        private PolygonSculptor _ps1;

        /// <summary>
        /// Gets the 1st PolygonSculptor.
        /// </summary>
        public PolygonSculptor PolygonSculptor1
        {
            get { return _ps1; }
        }

        private PolygonSculptor _ps2;

        /// <summary>
        /// Gets the 2nd PolygonSculptor.
        /// </summary>
        public PolygonSculptor PolygonSculptor2
        {
            get { return _ps2; }
        }

        /// <summary>
        /// Initializes the Points and Triangles collections.
        /// Called By Sculptor.BuildMesh()
        /// </summary>
        protected override void CreateTriangles()
        {
            _ps1 = new PolygonSculptor(_circumferenceSideCount, _initialAngle);
            _ps1.RoundingRate = _roundingRate;
            _ps1.BuildTriangles(TriangleSideKind.Back);
            foreach (Point3D p in _ps1.Points)
            {
                this.Points.Add(p);
            }
            foreach (Point3DTriplet tpl in _ps1.Triangles)
            {
                this.Triangles.Add(tpl);
            }

            _ps2 = new PolygonSculptor(_circumferenceSideCount, _initialAngle);
            _ps2.Center = new Point3D(
                _ps2.Center.X,
                _ps2.Center.Y,
                _ps2.Center.Z + 1.0);
            for (int i = 0; i < _ps2.Points.Count; i++)
            {
                _ps2.Points[i] = new Point3D(
                    _ps2.Points[i].X,
                    _ps2.Points[i].Y,
                    _ps2.Points[i].Z + 1.0);
                this.Points.Add(_ps2.Points[i]);
            }
            _ps2.RoundingRate = _roundingRate;
            _ps2.BuildTriangles(TriangleSideKind.Front);

            foreach (Point3DTriplet tpl in _ps2.Triangles)
            {
                this.Triangles.Add(tpl);
            }

            for (int i = 0; i < _ps1.Points.Count; i++)
            {
                if (i < _ps1.Points.Count - 1)
                {
                    this.Triangles.Add(new Point3DTriplet(_ps1.Points[i], _ps2.Points[i + 1], _ps2.Points[i]));
                    this.Triangles.Add(new Point3DTriplet(_ps1.Points[i], _ps1.Points[i + 1], _ps2.Points[i + 1]));
                }
                else
                {
                    this.Triangles.Add(new Point3DTriplet(_ps1.Points[i], _ps2.Points[0], _ps2.Points[i]));
                    this.Triangles.Add(new Point3DTriplet(_ps1.Points[i], _ps1.Points[0], _ps2.Points[0]));
                }
            }
        }

        /// <summary>
        /// A method for building the TextureCoordinates collection of the mesh.
        /// </summary>
        internal override void MapTexture()
        {
            Mesh.TextureCoordinates.Clear();
            if (TexturePosition == TexturePositions.OnEnds)
            {
                foreach (Point3DTriplet pt in _ps1.Triangles)
                {
                    foreach (Point3D p in pt.Points)
                    {
                        Mesh.TextureCoordinates.Add(
                            new Point(p.X, -p.Y));
                    }
                }
                foreach (Point3DTriplet pt in _ps2.Triangles)
                {
                    foreach (Point3D p in pt.Points)
                    {
                        Mesh.TextureCoordinates.Add(
                            new Point(p.X, -p.Y));
                    }
                }
            }
            else if (TexturePosition == TexturePositions.OnSides)
            {
                double minY = 0.0;
                double maxY = 1.0;
                double minX = 0.0;
                double maxX;
                double xPos = 0.0;
                double segmentLength = 1.0;

                foreach (Point3DTriplet pt in _ps1.Triangles)
                {
                    foreach (Point3D p in pt.Points)
                    {
                        Mesh.TextureCoordinates.Add(new Point(0.0, 0.0));
                    }
                }

                foreach (Point3DTriplet pt in _ps2.Triangles)
                {
                    foreach (Point3D p in pt.Points)
                    {
                        Mesh.TextureCoordinates.Add(new Point(0.0, 0.0));
                    }
                }

                for (int i = 0; i <= _ps1.Points.Count - 1; i++)
                {
                    if (_roundingRate != 0.0)
                    {
                        Point3D p2;
                        if (i == _ps1.Points.Count - 1)
                        {
                            p2 = _ps1.Points[0];
                        }
                        else
                        {
                            p2 = _ps1.Points[i + 1];
                        }
                        Vector3D v = p2 - _ps1.Points[i];
                        segmentLength = v.Length;
                    }
                    minX = xPos;
                    xPos += segmentLength;
                    maxX = xPos;

                    Mesh.TextureCoordinates.Add(new Point(minX, maxY));
                    Mesh.TextureCoordinates.Add(new Point(maxX, minY));
                    Mesh.TextureCoordinates.Add(new Point(minX, minY));

                    Mesh.TextureCoordinates.Add(new Point(minX, maxY));
                    Mesh.TextureCoordinates.Add(new Point(maxX, maxY));
                    Mesh.TextureCoordinates.Add(new Point(maxX, minY));
                }
            }
        }
    }
}
