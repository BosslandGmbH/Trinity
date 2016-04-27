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

namespace Trinity.UI.Overlays._3D.Perspective.Primitives
{
    /// <summary>
    /// Provides event data for the MeshBuilt event of GeometryElement3D.
    /// </summary>
    public class MeshBuiltEventArgs : EventArgs
    {
        ///// <summary>
        ///// Initializes a new MeshBuiltEventArgs object.
        ///// </summary>
        ///// <param name="mesh">A MeshGeometry3D object.</param>
        ///// <param name="triangles">The collection of the triangles of the mesh.</param>
        //public MeshBuiltEventArgs(MeshGeometry3D mesh, Collection<Point3DTriplet> triangles)
        //{
        //    _mesh = mesh;
        //    _triangles = triangles;
        //}

        /// <summary>
        /// Initializes a new MeshBuiltEventArgs object.
        /// </summary>
        /// <param name="mesh">A MeshGeometry3D object.</param>
        public MeshBuiltEventArgs(MeshGeometry3D mesh)
        {
            _mesh = mesh;
        }

        private MeshGeometry3D _mesh;

        /// <summary>
        /// Gets the MeshGeometry3D object.
        /// </summary>
        public MeshGeometry3D Mesh
        {
            get { return _mesh; }
        }

        //private Collection<Point3DTriplet> _triangles;

        ///// <summary>
        ///// Gets the sculptor's triangles collection.
        ///// </summary>
        //public Collection<Point3DTriplet> Triangles
        //{
        //    get { return _triangles; }
        //}

        //private bool _textureCoordinatesHandled;

        ///// <summary>
        ///// Indicates if the texture coordinates are handled in this event handler.
        ///// If true, the underlying Sculptor object will not create the default texture coordinates.
        ///// The default value is false.
        ///// </summary>
        //public bool TextureCoordinatesHandled
        //{
        //    get { return _textureCoordinatesHandled; }
        //    set { _textureCoordinatesHandled = value; }
        //}
    }
}
