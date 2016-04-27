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
using Trinity.UI.Overlays._3D.Perspective.Primitives;
using Trinity.UI.Overlays._3D.Perspective.Sculptors;

namespace Trinity.UI.Overlays._3D.Perspective.Shapes
{
    /// <summary>
    /// A 3D flat square model.
    /// Default size of a side is 1.0.
    /// </summary>
    public class Square3D : GeometryElement3D
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static Square3D()
        {
            DefaultTextureMappingProperty.OverrideMetadata(
                typeof(Square3D),
                new PropertyMetadata(
                    true));
        }

        // private SquareSculptor _sculptor = new SquareSculptor();

        /// <summary>
        /// Called by UIElement3D.InvalidateModel() to update the 3D model.
        /// </summary>
        protected override void OnUpdateModel()
        {
            Sculptor = new SquareSculptor();
            //_sculptor.BuildMesh();
            //Geometry = _sculptor.Mesh;
            base.OnUpdateModel();
        }
    }
}
