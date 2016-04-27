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

using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Trinity.UI.Overlays._3D.Perspective.Primitives;

namespace Trinity.UI.Overlays._3D.Perspective.Sculptors
{
    /// <summary>
    /// A class to handle points and triangles of a truncated isocahedron model.
    /// Default radius is 1.0.
    /// </summary>
    public class TruncatedIsocahedronSculptor : Sculptor
    {
        /// <summary>
        /// Initializes the Points and Triangles collections.
        /// Called By Sculptor.BuildMesh()
        /// </summary>
        protected override void CreateTriangles()
        {
            IsocahedronSculptor sculptor = new IsocahedronSculptor();
            sculptor.BuildMesh();
            foreach (Point3DTriplet tpl in sculptor.Triangles)
            {
                Point3D p1 = tpl.Points[0];
                Point3D p2 = tpl.Points[1];
                Point3D p3 = tpl.Points[2];

                // troncature des sommets du triangle => 1 hexagone (= 4 triangles)
                Vector3D v1 = Point3D.Subtract(p2, p1);
                Point3D pA = Point3D.Add(p1, Vector3D.Multiply(v1, _truncRate));
                Point3D pB = Point3D.Add(p1, Vector3D.Multiply(v1, 1 - _truncRate));

                Vector3D v2 = Point3D.Subtract(p3, p2);
                Point3D pC = Point3D.Add(p2, Vector3D.Multiply(v2, _truncRate));
                Point3D pD = Point3D.Add(p2, Vector3D.Multiply(v2, 1 - _truncRate));

                Vector3D v3 = Point3D.Subtract(p1, p3);
                Point3D pE = Point3D.Add(p3, Vector3D.Multiply(v3, _truncRate));
                Point3D pF = Point3D.Add(p3, Vector3D.Multiply(v3, 1 - _truncRate));

                this.Points.Add(pA);
                this.Points.Add(pB);
                this.Points.Add(pC);
                this.Points.Add(pD);
                this.Points.Add(pE);
                this.Points.Add(pF);

                PolygonSculptor hv = new PolygonSculptor(pA, pB, pC, pD, pE, pF);
                hv.BuildTriangles();
                foreach (Point3DTriplet tpl2 in hv.Triangles)
                {
                    this.Triangles.Add(tpl2);
                }
                _hexagonList.Add(hv);
            }

            BuildPentagon(sculptor.P1a, sculptor.P3a, sculptor.P2d, sculptor.P1d, sculptor.P2a, sculptor.P3d);
            BuildPentagon(sculptor.P1b, sculptor.P3a, sculptor.P3d, sculptor.P2b, sculptor.P1c, sculptor.P2c);
            BuildPentagon(sculptor.P1c, sculptor.P3b, sculptor.P2c, sculptor.P1b, sculptor.P2b, sculptor.P3c);
            BuildPentagon(sculptor.P1d, sculptor.P1a, sculptor.P2d, sculptor.P3b, sculptor.P3c, sculptor.P2a);

            BuildPentagon(sculptor.P2a, sculptor.P1a, sculptor.P1d, sculptor.P3c, sculptor.P2b, sculptor.P3d);
            BuildPentagon(sculptor.P2b, sculptor.P3d, sculptor.P2a, sculptor.P3c, sculptor.P1c, sculptor.P1b);
            BuildPentagon(sculptor.P2c, sculptor.P2d, sculptor.P3a, sculptor.P1b, sculptor.P1c, sculptor.P3b);
            BuildPentagon(sculptor.P2d, sculptor.P3a, sculptor.P2c, sculptor.P3b, sculptor.P1d, sculptor.P1a);

            BuildPentagon(sculptor.P3a, sculptor.P2c, sculptor.P2d, sculptor.P1a, sculptor.P3d, sculptor.P1b);
            BuildPentagon(sculptor.P3b, sculptor.P2d, sculptor.P2c, sculptor.P1c, sculptor.P3c, sculptor.P1d);
            BuildPentagon(sculptor.P3c, sculptor.P2b, sculptor.P2a, sculptor.P1d, sculptor.P3b, sculptor.P1c);
            BuildPentagon(sculptor.P3d, sculptor.P1a, sculptor.P2a, sculptor.P2b, sculptor.P1b, sculptor.P3a);
        }

        private const double _truncRate = 1.0 / 3.0;

        private List<PolygonSculptor> _hexagonList = new List<PolygonSculptor>();

        /// <summary>
        /// Gets the PolygonSculptor hexagonal object list
        /// </summary>
        internal List<PolygonSculptor> HexagonList
        {
            get { return _hexagonList; }
        }

        private List<PolygonSculptor> _pentagonList = new List<PolygonSculptor>();

        /// <summary>
        /// Gets the PolygonSculptor pentagonal object list
        /// </summary>
        internal List<PolygonSculptor> PentagonList
        {
            get { return _pentagonList; }
        }

        /// <summary>
        /// Truncate a vertex by building a pentagon
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="p5"></param>
        private void BuildPentagon(Point3D p0, Point3D p1, Point3D p2, Point3D p3, Point3D p4, Point3D p5)
        {
            Vector3D v1 = Point3D.Subtract(p1, p0);
            Point3D p1a = Point3D.Add(p0, Vector3D.Multiply(v1, _truncRate));

            Vector3D v2 = Point3D.Subtract(p2, p0);
            Point3D p2a = Point3D.Add(p0, Vector3D.Multiply(v2, _truncRate));

            Vector3D v3 = Point3D.Subtract(p3, p0);
            Point3D p3a = Point3D.Add(p0, Vector3D.Multiply(v3, _truncRate));

            Vector3D v4 = Point3D.Subtract(p4, p0);
            Point3D p4a = Point3D.Add(p0, Vector3D.Multiply(v4, _truncRate));

            Vector3D v5 = Point3D.Subtract(p5, p0);
            Point3D p5a = Point3D.Add(p0, Vector3D.Multiply(v5, _truncRate));

            this.Points.Add(p1a);
            this.Points.Add(p2a);
            this.Points.Add(p3a);
            this.Points.Add(p4a);
            this.Points.Add(p5a);

            PolygonSculptor pv = new PolygonSculptor(p1a, p2a, p3a, p4a, p5a);
            pv.BuildTriangles();
            foreach (Point3DTriplet tpl in pv.Triangles)
            {
                this.Triangles.Add(tpl);
            }
            this._pentagonList.Add(pv);
        }

    }
}
