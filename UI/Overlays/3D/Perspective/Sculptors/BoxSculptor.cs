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
    /// A class to handle points and triangles of a box model.
    /// Default size of a side is 1.0.
    /// </summary>
    public class BoxSculptor : Sculptor
    {
        private Point3D _p000 = new Point3D(0, 0, 0);
        private Point3D _p100 = new Point3D(1, 0, 0);
        private Point3D _p110 = new Point3D(1, 1, 0);
        private Point3D _p010 = new Point3D(0, 1, 0);
        private Point3D _p001 = new Point3D(0, 0, 1);
        private Point3D _p011 = new Point3D(0, 1, 1);
        private Point3D _p101 = new Point3D(1, 0, 1);
        private Point3D _p111 = new Point3D(1, 1, 1);

        private const int _back = 0;
        private const int _left = 1;
        private const int _front = 2;
        private const int _right = 3;
        private const int _up = 4;
        private const int _down = 5;

        private Point3DCollection[] _sidePoints = new Point3DCollection[6];

        private BoxSides _sideLayout;

        /// <summary>
        /// Initializes a new instance of BoxSculptor.
        /// </summary>
        public BoxSculptor()
            : base()
        {
            this.Points.Add(_p000);
            this.Points.Add(_p100);
            this.Points.Add(_p110);
            this.Points.Add(_p010);
            this.Points.Add(_p001);
            this.Points.Add(_p011);
            this.Points.Add(_p101);
            this.Points.Add(_p111);

            _sidePoints[_back] = new Point3DCollection();
            _sidePoints[_back].Add(_p100);
            _sidePoints[_back].Add(_p000);
            _sidePoints[_back].Add(_p010);
            _sidePoints[_back].Add(_p110);

            _sidePoints[_left] = new Point3DCollection();
            _sidePoints[_left].Add(_p000);
            _sidePoints[_left].Add(_p001);
            _sidePoints[_left].Add(_p011);
            _sidePoints[_left].Add(_p010);

            _sidePoints[_front] = new Point3DCollection();
            _sidePoints[_front].Add(_p001);
            _sidePoints[_front].Add(_p101);
            _sidePoints[_front].Add(_p111);
            _sidePoints[_front].Add(_p011);

            _sidePoints[_right] = new Point3DCollection();
            _sidePoints[_right].Add(_p101);
            _sidePoints[_right].Add(_p100);
            _sidePoints[_right].Add(_p110);
            _sidePoints[_right].Add(_p111);

            _sidePoints[_up] = new Point3DCollection();
            _sidePoints[_up].Add(_p111);
            _sidePoints[_up].Add(_p110);
            _sidePoints[_up].Add(_p010);
            _sidePoints[_up].Add(_p011);

            _sidePoints[_down] = new Point3DCollection();
            _sidePoints[_down].Add(_p000);
            _sidePoints[_down].Add(_p100);
            _sidePoints[_down].Add(_p101);
            _sidePoints[_down].Add(_p001);
        }

        /// <summary>
        /// Initializes a new instance of BoxSculptor.
        /// </summary>
        /// <param name="sideLayout">Set of the visible sides.</param>
        public BoxSculptor(BoxSides sideLayout)
            : this()
        {
            Initialize(sideLayout);
        }

        /// <summary>
        /// Initializes an existing instance of BoxSculptor.
        /// </summary>
        /// <param name="sideLayout">Set of the visible sides.</param>
        public void Initialize(BoxSides sideLayout)
        {
            _sideLayout = sideLayout;
        }

        /// <summary>
        /// Building of the Triangles collections.
        /// </summary>
        protected override void CreateTriangles()
        {
            this.Triangles.Clear();

            if ((_sideLayout & BoxSides.Back) == BoxSides.Back)
            {
                CreateSideTriangles(_sidePoints[_back], TriangleSideKind.Front);
            }
            if ((_sideLayout & BoxSides.Left) == BoxSides.Left)
            {
                CreateSideTriangles(_sidePoints[_left], TriangleSideKind.Front);
            }
            if ((_sideLayout & BoxSides.Front) == BoxSides.Front)
            {
                CreateSideTriangles(_sidePoints[_front], TriangleSideKind.Front);
            }
            if ((_sideLayout & BoxSides.Right) == BoxSides.Right)
            {
                CreateSideTriangles(_sidePoints[_right], TriangleSideKind.Front);
            }
            if ((_sideLayout & BoxSides.Up) == BoxSides.Up)
            {
                CreateSideTriangles(_sidePoints[_up], TriangleSideKind.Front);
            }
            if ((_sideLayout & BoxSides.Down) == BoxSides.Down)
            {
                CreateSideTriangles(_sidePoints[_down], TriangleSideKind.Front);
            }
        }

        /// <summary>
        /// Applies a transformation to the points
        /// </summary>
        /// <param name="t">Transform3D object.</param>
        public override void Transform(Transform3D t)
        {
            base.Transform(t);
            foreach (Point3DCollection points in _sidePoints)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    points[i] = t.Transform(points[i]);
                }
            }
        }

        /// <summary>
        /// A method for building the TextureCoordinates collection of the mesh.
        /// </summary>
        // protected override void CreateTextureCoordinates()
        internal override void MapTexture()
        {
            Mesh.TextureCoordinates.Clear();
            for (int i = 0; i < 6; i++)
            {
                Helper3D.MapSquareTexture(Mesh, 0.0, 0.0, 1.0, 1.0);
            }
        }
    }
}
