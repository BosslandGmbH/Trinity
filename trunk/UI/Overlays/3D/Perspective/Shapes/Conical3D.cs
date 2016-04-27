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
    /// A 3D conical model.
    /// By default, the direction of the cone is the Z axis, and the length is 1.0.
    /// Default radius is 1.0.
    /// </summary>
    public class Conical3D : PolygonalElement3D
    {
        /// <summary>
        /// Static constructor.
        /// No default back material for 3D closed shapes.
        /// </summary>
        static Conical3D()
        {
            BackMaterialProperty.OverrideMetadata(
                typeof(Conical3D),
                new PropertyMetadata(
                    null, 
                    VisualPropertyChanged));
        }

        // private ConicalSculptor _sculptor = new ConicalSculptor();

        /// <summary>
        /// Called by UIElement3D.InvalidateModel() to update the 3D model.
        /// </summary>
        protected override void OnUpdateModel()
        {
            Sculptor = new ConicalSculptor(SideCount, InitialAngle, RoundingRate);
            // _sculptor.Initialize(SideCount, InitialAngle, RoundingRate);
            //_sculptor.BuildMesh();
            //Geometry = _sculptor.Mesh;
            base.OnUpdateModel();
        }
    }
}
