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
    /// A 3D arrow element.
    /// By default, the direction of the arrow is the Z axis, and the length is 1.0.
    /// Default radius of the body is 0.1.
    /// Default radius of the head is 0.2.
    /// </summary>
    public class Arrow3D : GeometryElement3D
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static Arrow3D()
        {
            // No default back material for 3D closed shapes.
            BackMaterialProperty.OverrideMetadata(
                typeof(Arrow3D),
                new PropertyMetadata(
                    null, 
                    VisualPropertyChanged));
        }

        // private ArrowSculptor _sculptor = new ArrowSculptor();

        /// <summary>
        /// Called by UIElement3D.InvalidateModel() to update the 3D model.
        /// </summary>
        protected override void OnUpdateModel()
        {
            Sculptor = new ArrowSculptor(Length);

            // _sculptor.Initialize(Length);
            // _sculptor.BuildMesh();
            // Geometry = _sculptor.Mesh;
            base.OnUpdateModel();
        }

        /// <summary>
        /// Gets or sets the axis length.
        /// </summary>
        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        /// <summary>
        /// Identifies the Length dependency property.
        /// Default value is 1.0.
        /// </summary>
        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register(
                "Length",
                typeof(double),
                typeof(Arrow3D),
                new PropertyMetadata(
                    ArrowSculptor.DefaultLength,
                    VisualPropertyChanged));

    }
}
