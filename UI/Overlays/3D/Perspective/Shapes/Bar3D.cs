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
    /// A 3D bar element.
    /// By default, the direction of the bar is the Z axis, and the length is 1.0.
    /// Default radius is 1.0.
    /// </summary>
    public class Bar3D : PolygonalElement3D
    {
        /// <summary>
        /// Static constructor.
        /// No default back material for 3D closed shapes.
        /// </summary>
        static Bar3D()
        {
            BackMaterialProperty.OverrideMetadata(
                typeof(Bar3D),
                new PropertyMetadata(
                    null, 
                    VisualPropertyChanged));
        }



        /// <summary>
        /// Called by UIElement3D.InvalidateModel() to update the 3D model.
        /// </summary>
        protected override void OnUpdateModel()
        {
            Sculptor = new BarSculptor(SideCount, InitialAngle, RoundingRate);
            (Sculptor as BarSculptor).TexturePosition = this.TexturePosition;
            // _sculptor.Initialize(SideCount, InitialAngle, RoundingRate);
            //_sculptor.BuildMesh();
            //Geometry = _sculptor.Mesh;
            base.OnUpdateModel();
        }


        /// <summary>
        /// Gets or sets the position a a texture. Default is OnSides.
        /// </summary>
        public TexturePositions TexturePosition
        {
            get { return (TexturePositions)GetValue(TexturePositionProperty); }
            set { SetValue(TexturePositionProperty, value); }
        }

        /// <summary>
        /// Identifies the TexturePosition dependency property.
        /// </summary>
        public static readonly DependencyProperty TexturePositionProperty =
            DependencyProperty.Register("TexturePosition", typeof(TexturePositions), typeof(Bar3D), new PropertyMetadata(TexturePositions.OnSides));



        
    }
}
