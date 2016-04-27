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
    /// A 3D sector, which may be used to build pie charts.
    /// </summary>
    // Contribution by Philippe Jovelin - 14/02/08
    // Refactoring and "sector explosion" by Olivier Dewit - april-may 2008
    public class PieSlice3D : PolygonalElement3D
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static PieSlice3D()
        {
            SideCountProperty.OverrideMetadata(
                typeof(PieSlice3D),
                new PropertyMetadata(
                    360,
                    VisualPropertyChanged));
        }

        /// <summary>
        /// Gets or sets the pie slice angle value (in degrees).
        /// Default value is 0.0.
        /// </summary>
        public double AngleValue
        {
            get { return (double)GetValue(AngleValueProperty); }
            set { SetValue(AngleValueProperty, value); }
        }

        /// <summary>
        /// Identifies the AngleValue dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleValueProperty =
            DependencyProperty.Register(
                "AngleValue",
                typeof(double),
                typeof(PieSlice3D),
                new PropertyMetadata(
                    0.0, 
                    VisualPropertyChanged));

        /// <summary>
        /// Gets or sets the pie radius.
        /// Default value is 1.0.
        /// </summary>
        public double PieRadius
        {
            get { return (double)GetValue(PieRadiusProperty); }
            set { SetValue(PieRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the PieRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty PieRadiusProperty =
            DependencyProperty.Register(
                "PieRadius",
                typeof(double),
                typeof(PieSlice3D),
                new PropertyMetadata(
                    1.0, 
                    VisualPropertyChanged));

        /// <summary>
        /// Indicates if the sector is exploded (usefull for pie charts).
        /// Default value is false.
        /// </summary>
        public bool IsExploded
        {
            get { return (bool)GetValue(IsExplodedProperty); }
            set { SetValue(IsExplodedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsExploded dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExplodedProperty =
            DependencyProperty.Register(
                "IsExploded",
                typeof(bool),
                typeof(PieSlice3D),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the offset of the translation due to the explosion (when IsExploded is true).
        /// Default value is 0.15.
        /// </summary>
        public double ExplosionOffset
        {
            get { return (double)GetValue(ExplosionOffsetProperty); }
            set { SetValue(ExplosionOffsetProperty, value); }
        }

        /// <summary>
        /// Identifies the ExplosionOffset dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplosionOffsetProperty =
            DependencyProperty.Register(
                "ExplosionOffset", 
                typeof(double), 
                typeof(PieSlice3D), 
                new PropertyMetadata(
                    SliceSculptor.DefaultExplosionOffset));

        /// <summary>
        /// Called by UIElement3D.InvalidateModel() to update the 3D model.
        /// </summary>
        protected override void OnUpdateModel()
        {
            Sculptor = new PieSliceSculptor(
                SideCount, 
                InitialAngle, 
                AngleValue, 
                PieRadius,
                IsExploded,
                ExplosionOffset);
            base.OnUpdateModel();
        }
    }
}
