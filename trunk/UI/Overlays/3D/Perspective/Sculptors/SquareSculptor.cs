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
    /// A class to handle points and triangles of a square model.
    /// Default size of a side is 1.0.
    /// </summary>
    public class SquareSculptor : Sculptor
    {
        Point3D _p000 = new Point3D(0, 0, 0);
        Point3D _p100 = new Point3D(1, 0, 0);
        Point3D _p110 = new Point3D(1, 1, 0);
        Point3D _p010 = new Point3D(0, 1, 0);

        /// <summary>
        /// Initializes a new instance of SquareSculptor.
        /// </summary>
        public SquareSculptor()
            : base()
        {
            this.Points.Add(_p000);
            this.Points.Add(_p100);
            this.Points.Add(_p110);
            this.Points.Add(_p010);
        }

        /// <summary>
        /// Initializes the Points and Triangles collections.
        /// Called By Sculptor.BuildMesh()
        /// </summary>
        protected override void CreateTriangles()
        {
            this.Triangles.Clear();
            CreateSideTriangles(Points, TriangleSideKind.Front);
        }

        /// <summary>
        /// A method for building the TextureCoordinates collection of the mesh.
        /// </summary>
        internal override void MapTexture()
        {
            Mesh.TextureCoordinates.Clear();
            Helper3D.MapSquareTexture(Mesh, 0.0, 0.0, 1.0, 1.0);
        }
    }
}
