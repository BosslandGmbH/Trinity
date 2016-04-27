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
    /// A 3D ring model.
    /// Default radius is 10.0.
    /// </summary>
    public class Ring3D : PolygonalElement3D
    {
        /// <summary>
        /// Static constructor.
        /// No default back material for 3D closed shapes.
        /// </summary>
        static Ring3D()
        {
            BackMaterialProperty.OverrideMetadata(
                typeof(Ring3D),
                new PropertyMetadata(
                    null, 
                    VisualPropertyChanged));
        }

        /// <summary>
        /// Gets or sets the ring radius.
        /// Default value is 10.0.
        /// </summary>
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the Radius dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(
                "Radius",
                typeof(double),
                typeof(Ring3D),
                new UIPropertyMetadata(10.0, VisualPropertyChanged));

        /// <summary>
        /// Gets or sets the ring bar segment count.
        /// Default value is 40. Minimum value is 3.
        /// </summary>
        public int SegmentCount
        {
            get { return (int)GetValue(SegmentCountProperty); }
            set { SetValue(SegmentCountProperty, value); }
        }

        /// <summary>
        /// Identifies the SegmentCount dependency property.
        /// </summary>
        public static readonly DependencyProperty SegmentCountProperty =
            DependencyProperty.Register(
                "SegmentCount",
                typeof(int),
                typeof(Ring3D),
                new UIPropertyMetadata(40, VisualPropertyChanged),
                IsValidSegmentCountValue);

        /// <summary>
        /// Validation of the SegmentCount value.
        /// </summary>
        /// <param name="value">Value to test.</param>
        /// <returns>Boolean value.</returns>
        private static bool IsValidSegmentCountValue(object value)
        {
            int i = (int)value;
            return i >= 3;
        }

        // private RingSculptor _sculptor = new RingSculptor();

        /// <summary>
        /// Called by UIElement3D.InvalidateModel() to update the 3D model.
        /// </summary>
        protected override void OnUpdateModel()
        {
            Sculptor = new RingSculptor(Radius, SegmentCount, SideCount, InitialAngle, RoundingRate);
            //_sculptor.Initialize(Radius, SegmentCount, SideCount, InitialAngle, RoundingRate);
            //_sculptor.BuildMesh();
            //Geometry = _sculptor.Mesh;
            base.OnUpdateModel();
        }
    }
}
