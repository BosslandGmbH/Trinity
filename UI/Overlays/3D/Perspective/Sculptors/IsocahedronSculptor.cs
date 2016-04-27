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
    /// A class to handle points and triangles of a isocahedron model.
    /// Default radius is 1.0.
    /// </summary>
    public class IsocahedronSculptor : Sculptor
    {
        private Point3D
            _p1a, _p1b, _p1c, _p1d,
            _p2a, _p2b, _p2c, _p2d,
            _p3a, _p3b, _p3c, _p3d;

        /// <summary>
        /// Initializes the Points and Triangles collections.
        /// Called By Sculptor.BuildMesh()
        /// </summary>
        protected override void CreateTriangles()
        {
            // L'isocaèdre est obtenu en s'appuyant sur les sommets des trois rectangles d'or dont 
            // les sommets diagonaux ont pour coordonnées :
            // (0, 1, t):(0, -1, -t), (-1, t, 0):(1, -t, 0) et (t, 0, 1):(-t, 0, -1)
            // avec (t = (rac(5) - 1)/2).
            // http://www.limsi.fr/Individu/jacquemi/IG-TR-7-8-9/surf-polyedr-isoca.html

            double t = (Math.Sqrt(5) - 1) / 2;

            // rectangle 1
            _p1a = new Point3D(0, 1, t);
            _p1b = new Point3D(0, -1, t);
            _p1c = new Point3D(0, -1, -t);
            _p1d = new Point3D(0, 1, -t);
            this.Points.Add(_p1a);
            this.Points.Add(_p1b);
            this.Points.Add(_p1c);
            this.Points.Add(_p1d);

            // rectangle 2
            _p2a = new Point3D(-1, t, 0);
            _p2b = new Point3D(-1, -t, 0);
            _p2c = new Point3D(1, -t, 0);
            _p2d = new Point3D(1, t, 0);
            this.Points.Add(_p2a);
            this.Points.Add(_p2b);
            this.Points.Add(_p2c);
            this.Points.Add(_p2d);

            // rectangle 3
            _p3a = new Point3D(t, 0, 1);
            _p3b = new Point3D(t, 0, -1);
            _p3c = new Point3D(-t, 0, -1);
            _p3d = new Point3D(-t, 0, 1);
            this.Points.Add(_p3a);
            this.Points.Add(_p3b);
            this.Points.Add(_p3c);
            this.Points.Add(_p3d);

            this.Triangles.Add(new Point3DTriplet(_p1a, _p3d, _p3a));
            this.Triangles.Add(new Point3DTriplet(_p1a, _p3a, _p2d));
            this.Triangles.Add(new Point3DTriplet(_p1a, _p2d, _p1d));
            this.Triangles.Add(new Point3DTriplet(_p1a, _p1d, _p2a));
            this.Triangles.Add(new Point3DTriplet(_p1a, _p2a, _p3d));
            this.Triangles.Add(new Point3DTriplet(_p1d, _p2d, _p3b));
            this.Triangles.Add(new Point3DTriplet(_p1d, _p3b, _p3c));
            this.Triangles.Add(new Point3DTriplet(_p1d, _p3c, _p2a));
            this.Triangles.Add(new Point3DTriplet(_p2a, _p2b, _p3d));
            this.Triangles.Add(new Point3DTriplet(_p2a, _p3c, _p2b));

            this.Triangles.Add(new Point3DTriplet(_p1b, _p2c, _p3a));
            this.Triangles.Add(new Point3DTriplet(_p1b, _p3a, _p3d));
            this.Triangles.Add(new Point3DTriplet(_p1b, _p3d, _p2b));
            this.Triangles.Add(new Point3DTriplet(_p1b, _p2b, _p1c));
            this.Triangles.Add(new Point3DTriplet(_p1b, _p1c, _p2c));
            this.Triangles.Add(new Point3DTriplet(_p1c, _p3b, _p2c));
            this.Triangles.Add(new Point3DTriplet(_p1c, _p3c, _p3b));
            this.Triangles.Add(new Point3DTriplet(_p1c, _p2b, _p3c));
            this.Triangles.Add(new Point3DTriplet(_p2c, _p2d, _p3a));
            this.Triangles.Add(new Point3DTriplet(_p2c, _p3b, _p2d));
        }
        internal Point3D P1a
        {
            get { return _p1a; }
        }
        internal Point3D P1b
        {
            get { return _p1b; }
        }
        internal Point3D P1c
        {
            get { return _p1c; }
        }
        internal Point3D P1d
        {
            get { return _p1d; }
        }
        internal Point3D P2a
        {
            get { return _p2a; }
        }
        internal Point3D P2b
        {
            get { return _p2b; }
        }
        internal Point3D P2c
        {
            get { return _p2c; }
        }
        internal Point3D P2d
        {
            get { return _p2d; }
        }
        internal Point3D P3a
        {
            get { return _p3a; }
        }
        internal Point3D P3b
        {
            get { return _p3b; }
        }
        internal Point3D P3c
        {
            get { return _p3c; }
        }
        internal Point3D P3d
        {
            get { return _p3d; }
        }
    }
}
