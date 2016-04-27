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
    /// A 3D box element.
    /// The same material is used for all the faces through the Material property.
    /// Default size of a side is 1.0.
    /// </summary>
    public class Box3D : GeometryElement3D
    {
        /// <summary>
        /// Static constructor.
        /// No default back material for 3D closed shapes.
        /// </summary>
        static Box3D()
        {
            BackMaterialProperty.OverrideMetadata(
                typeof(Box3D),
                new PropertyMetadata(
                    null, 
                    VisualPropertyChanged));
            
            DefaultTextureMappingProperty.OverrideMetadata(
                typeof(Box3D),
                new PropertyMetadata(
                    true));
        }

        // private BoxSculptor _sculptor = new BoxSculptor();

        /// <summary>
        /// Called by UIElement3D.InvalidateModel() to update the 3D model.
        /// </summary>
        protected override void OnUpdateModel()
        {
            Sculptor = new BoxSculptor(VisibleSides);
            // _sculptor.Initialize(VisibleSides);
            //_sculptor.BuildMesh();
            //Geometry = _sculptor.Mesh;
            base.OnUpdateModel();
        }

        /// <summary>
        /// Gets or sets the sides of the model.
        /// Default is BoxSides.All
        /// XAML usage : VisibleSides="Back,Left,Right,Up,Down"
        /// </summary>
        public BoxSides VisibleSides
        {
            get { return (BoxSides)GetValue(VisibleSidesProperty); }
            set { SetValue(VisibleSidesProperty, value); }
        }

        /// <summary>
        /// Identifies the VisibleSides dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleSidesProperty =
            DependencyProperty.Register(
                "VisibleSides", 
                typeof(BoxSides), 
                typeof(Box3D), 
                new UIPropertyMetadata(
                    BoxSides.All,
                    VisualPropertyChanged));
    }
}
