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
    /// A 3D isocahedron element.
    /// Default radius is 1.0.
    /// </summary>
    public class Isocahedron3D : GeometryElement3D
    {
        /// <summary>
        /// Static constructor.
        /// No default back material for 3D closed shapes.
        /// </summary>
        static Isocahedron3D()
        {
            BackMaterialProperty.OverrideMetadata(
                typeof(Isocahedron3D),
                new PropertyMetadata(
                    null, 
                    VisualPropertyChanged));
        }

        /// <summary>
        /// Indicates if the isocahedron is truncated.
        /// Default value is false
        /// </summary>
        public bool Truncated
        {
            get { return (bool)GetValue(TruncatedProperty); }
            set { SetValue(TruncatedProperty, value); }
        }

        /// <summary>
        /// Identifies the Truncated dependency property.
        /// </summary>
        public static readonly DependencyProperty TruncatedProperty =
            DependencyProperty.Register(
                "Truncated",
                typeof(bool),
                typeof(Isocahedron3D),
                new UIPropertyMetadata(false));

        /// <summary>
        /// Called by UIElement3D.InvalidateModel() to update the 3D model.
        /// </summary>
        protected override void OnUpdateModel()
        {
            // Sculptor sculptor;
            if (Truncated)
            {
                // sculptor = new TruncatedIsocahedronSculptor();
                Sculptor = new TruncatedIsocahedronSculptor();
            }
            else
            {
                // sculptor = new IsocahedronSculptor();
                Sculptor = new IsocahedronSculptor();
            }
            //sculptor.BuildMesh();
            //Geometry = sculptor.Mesh;
            base.OnUpdateModel();
        }
    }
}
