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

using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace Trinity.UI.Overlays._3D.Perspective.Primitives
{
    /// <summary>
    /// A class to generate points and triangles of a 3D model.
    /// </summary>
    public class Sculptor
    {
        /// <summary>
        /// Initializes a new instance of Sculptor.
        /// </summary>
        public Sculptor()
        {
            _points = new Point3DCollection();
        }

        /// <summary>
        /// Initializes a new instance of Sculptor.
        /// </summary>
        /// <param name="points">Points collection</param>
        public Sculptor(Point3DCollection points)
        {
            _points = points;
        }

        /// <summary>
        /// Initializes a new instance of Sculptor.
        /// </summary>
        /// <param name="points">Array of points</param>
        public Sculptor(params Point3D[] points)
        {
            _points = new Point3DCollection();
            foreach (Point3D p in points)
            {
                this._points.Add(p);
            }
        }

        /// <summary>
        ///  Initializes a new instance of Sculptor.
        /// </summary>
        /// <param name="s">Source Sculptor</param>
        public Sculptor(Sculptor s)
        {
            _points = new Point3DCollection();
            CopyFrom(s);
        }

        private Point3DCollection _points;

        /// <summary>
        /// Gets the sculptor's points collection.
        /// </summary>
        public Point3DCollection Points
        {
            get { return _points; }
        }

        private Collection<Point3DTriplet> _triangles = new Collection<Point3DTriplet>();

        /// <summary>
        /// Gets the sculptor's triangles collection.
        /// </summary>
        public Collection<Point3DTriplet> Triangles
        {
            get { return _triangles; }
        }

        /// <summary>
        /// Clears the triangles and points collections.
        /// </summary>
        public void Clear()
        {
            _triangles.Clear();
            _points.Clear();
        }

        /// <summary>
        /// Virtual method to override for building the Triangles collections.
        /// </summary>
        protected virtual void CreateTriangles()
        {
        }

        /// <summary>
        /// Virtual method to override for building the TextureCoordinates collection of the mesh.
        /// </summary>
        // protected virtual void CreateTextureCoordinates()
        internal virtual void MapTexture()
        {
        }

        /// <summary>
        /// Build a MeshGeometry3D object from the Sculptor object.
        /// Points should be initialized before the call.
        /// Fires a BuiltMesh event, where custom texture coordinates may be created.
        /// </summary>
        /// <returns>MeshGeometry3D object</returns>
        public void BuildMesh()
        {
            CreateTriangles();
            _mesh = new MeshGeometry3D();
            foreach (Point3DTriplet tpl in this._triangles)
            {
                Helper3D.BuildTriangleMesh(_mesh, tpl.Points[0], tpl.Points[1], tpl.Points[2]);
            }
        }

        private MeshGeometry3D _mesh;

        /// <summary>
        /// Gets the model mesh (geometry).
        /// </summary>
        public MeshGeometry3D Mesh
        {
            get
            {
                if (_mesh == null)
                {
                    BuildMesh();
                }
                return _mesh;
            }
        }
        
        /// <summary>
        /// Applies a transformation to the points
        /// </summary>
        /// <param name="t">Transform3D object.</param>
        public virtual void Transform(Transform3D t)
        {
           for (int i = 0; i < _points.Count; i++)
           {
               _points[i] = t.Transform(_points[i]);
           }

           for (int j = 0; j < _triangles.Count; j++)
           {
               for (int k = 0; k < 3; k++)
               {
                   _triangles[j].Points[k] = t.Transform(_triangles[j].Points[k]);
               }
           }
        }

        /// <summary>
        /// Creates the triangles of a side.
        /// </summary>
        /// <param name="points">Collection of Point3D (may be an other collection than Points (i.e. a side collection).</param>
        /// <param name="tsk">TriangleSideKind Value.</param>
        protected void CreateSideTriangles(Point3DCollection points, TriangleSideKind tsk)
        {
            for (int i = 0; i < points.Count - 2; i++)
            {
                if (tsk == TriangleSideKind.Front)
                {
                    this.Triangles.Add(new Point3DTriplet(points[0], points[i + 1], points[i + 2]));
                }
                else
                {
                    this.Triangles.Add(new Point3DTriplet(points[0], points[i + 2], points[i + 1]));
                }
            }
        }

        /// <summary>
        /// Copies the points and triangles from a Sculptor object
        /// </summary>
        /// <param name="s">Source Sculptor</param>
        protected void CopyFrom(Sculptor s)
        {
            foreach (Point3D p in s.Points)
            {
                this.Points.Add(p);
            }
            foreach (Point3DTriplet tpl in s.Triangles)
            {
                this.Triangles.Add(tpl);
            }
        }
    }
}
