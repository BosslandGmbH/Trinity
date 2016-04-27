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
    /// A class to handle points and triangles of a 3D arrow model.
    /// Default radius of the head is 0.2.
    /// Default radius of the body is 0.1.
    /// By default, the direction of the arrow is the Z axis, and the length is 1.0.
    /// </summary>
    public class ArrowSculptor : Sculptor
    {
        internal const double DefaultLength = 1.0;
        private double _length = DefaultLength;

        /// <summary>
        /// Initializes a new instance of ArrowSculptor.
        /// </summary>
        public ArrowSculptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of ArrowSculptor.
        /// </summary>
        /// <param name="length">Length of each axis.</param>
        public ArrowSculptor(double length)
        {
            Initialize(length);
        }

        /// <summary>
        /// Initializes an existing instance of XyzAxisSculptor.
        /// </summary>
        /// <param name="length">Length of each axis.</param>
        public void Initialize(double length)
        {
            _length = length;
        }

        // ConicalSculptor _conicalSculptor;
        // BarSculptor _barSculptor;

        /// <summary>
        /// Initializes the Points and Triangles collections.
        /// Called By Sculptor.BuildMesh()
        /// </summary>
        protected override void CreateTriangles()
        {
            // ConicalSculptor conicalSculptor = new ConicalSculptor(4, 0.0, 0.5);
            ConicalSculptor _conicalSculptor = new ConicalSculptor(40, 0.0, 0.0);
            _conicalSculptor.BuildMesh();
            Transform3DGroup tgHead = new Transform3DGroup();
            tgHead.Children.Add(new ScaleTransform3D(0.2, 0.2, 0.2));
            tgHead.Children.Add(new TranslateTransform3D(0.0, 0.0, _length - 1.0 + 0.8));
            _conicalSculptor.Transform(tgHead);
            CopyFrom(_conicalSculptor);

            BarSculptor _barSculptor = new BarSculptor();
            // barSculptor.Initialize(4, 0.0, 0.5);
            _barSculptor.Initialize(40, 0.0, 0.0);
            _barSculptor.BuildMesh();
            _barSculptor.Transform(new ScaleTransform3D(0.1, 0.1, _length - 1.0 + 0.8));
            CopyFrom(_barSculptor);
        }

        ///// <summary>
        ///// A method for building the TextureCoordinates collection of the mesh.
        ///// </summary>
        //internal override void MapTexture()
        //{
        //    _conicalSculptor.MapTexture();
        //    foreach (Point point in _conicalSculptor.Mesh.TextureCoordinates)
        //    {
        //        Mesh.TextureCoordinates.Add(point);
        //    }

        //    _barSculptor.MapTexture();
        //    foreach (Point point in _barSculptor.Mesh.TextureCoordinates)
        //    {
        //        Mesh.TextureCoordinates.Add(point);
        //    }
        //}
    }
}
