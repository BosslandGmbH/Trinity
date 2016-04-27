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

namespace Trinity.UI.Overlays._3D.Perspective.Primitives
{
    /// <summary>
    /// Basic abstract class for Perspective.Wpf3D elements with 
    /// an inner GeometryModel3D with polygonal dimensions.  
    /// </summary>
    public class PolygonalElement3D : GeometryElement3D
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static PolygonalElement3D()
        {
            // No default back material for 3D closed shapes.
            BackMaterialProperty.OverrideMetadata(
                typeof(PolygonalElement3D),
                new PropertyMetadata(
                    null, 
                    VisualPropertyChanged));
        }

        /// <summary>
        /// Gets or sets the model side count.
        /// Default value is 4. Minimum value is 3.
        /// </summary>
        public int SideCount
        {
            get { return (int)GetValue(SideCountProperty); }
            set { SetValue(SideCountProperty, value); }
        }

        /// <summary>
        /// Identifies the SideCount dependency property.
        /// </summary>
        public static readonly DependencyProperty SideCountProperty =
            DependencyProperty.Register(
                "SideCount", 
                typeof(int), 
                typeof(PolygonalElement3D),
                new PropertyMetadata(
                    4, 
                    VisualPropertyChanged),
                SideCountValidateValue);

        /// <summary>
        /// Validation of the SideCount value.
        /// </summary>
        /// <param name="value">Value to test.</param>
        /// <returns>Boolean value.</returns>
        private static bool SideCountValidateValue(object value)
        {
            int i = (int)value;
            return i >= 3;
        }
        
        /// <summary>
        /// Gets or sets the model's initial angle value (in degrees).
        /// Default value is 0.0.
        /// </summary>
        public double InitialAngle
        {
            get { return (double)GetValue(InitialAngleProperty); }
            set { SetValue(InitialAngleProperty, value); }
        }

        /// <summary>
        /// Identifies the InitialAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty InitialAngleProperty =
            DependencyProperty.Register(
                "InitialAngle", 
                typeof(double), 
                typeof(PolygonalElement3D),
                new PropertyMetadata(
                    0.0, 
                    VisualPropertyChanged));

        /// <summary>
        /// Gets or sets the model's angle rounding rate.
        /// The value must be comprized between 0.0 and 0.5.
        /// Default value is 0.0.
        /// </summary>
        public double RoundingRate
        {
            get { return (double)GetValue(RoundingRateProperty); }
            set { SetValue(RoundingRateProperty, value); }
        }

        /// <summary>
        /// Identifies the RoundingRate dependency property.
        /// </summary>
        public static readonly DependencyProperty RoundingRateProperty =
            DependencyProperty.Register(
                "RoundingRate", 
                typeof(double), 
                typeof(PolygonalElement3D),
                new PropertyMetadata(
                    0.0, 
                    VisualPropertyChanged),
                RoundingRateValidateValue);

        /// <summary>
        /// Validation of the RoundingRate value.
        /// </summary>
        /// <param name="value">Value to test.</param>
        /// <returns>Boolean value.</returns>
        private static bool RoundingRateValidateValue(object value)
        {
            double d = (double)value;
            return (d >= 0.0) && (d <= 0.5);
        }
    }
}
