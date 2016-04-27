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
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Trinity.UI.Overlays._3D.Perspective.Sculptors;

namespace Trinity.UI.Overlays._3D.Perspective.Shapes
{
    /// <summary>
    /// A 3D bar element, with a specific texture for the ends.
    /// By default, the direction of the bar is the Z axis, and the length is 1.0.
    /// Default radius is 1.0.
    /// </summary>
    public class CompositeBar3D : UIElement3D // PolygonalElement3D
    {
        private BarSculptor _sculptor;
        private static Material _defaultMaterial;

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
                typeof(CompositeBar3D),
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
                typeof(CompositeBar3D),
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
                typeof(CompositeBar3D),
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


        /// <summary>
        /// Gets or sets the element's material.
        /// A default material is provided.
        /// </summary>
        public Material Material
        {
            get { return (Material)GetValue(MaterialProperty); }
            set { SetValue(MaterialProperty, value); }
        }

        /// <summary>
        /// Identifies the Material dependency property.
        /// </summary>
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(Material), typeof(CompositeBar3D),
            new PropertyMetadata(
                    _defaultMaterial,
                    MaterialPropertyChanged));


        /// <summary>
        /// Callback called when the Material property's value has changed.
        /// Assign the material to the inner model.
        /// </summary>
        /// <param name="d">Sender object</param>
        /// <param name="e">Callback arguments</param>
        internal static void MaterialPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CompositeBar3D bar = ((CompositeBar3D)d);
            if ((bar._element0 != null) && (bar._element0.Model != null))
            {
                ((GeometryModel3D)bar._element0.Model).Material = bar.Material;
            }
        }        

        /// <summary>
        /// Identifies the EndMaterial dependency property.
        /// </summary>
        public static DependencyProperty EndMaterialProperty =
            DependencyProperty.Register(
                "EndMaterial",
                typeof(Material),
                typeof(CompositeBar3D), new PropertyMetadata(
                    _defaultMaterial,
                    EndMaterialPropertyChanged));


        /// <summary>
        /// Gets or sets the model polygon ends material.
        /// </summary>
        public Material EndMaterial
        {
            get { return (Material)GetValue(EndMaterialProperty); }
            set { SetValue(EndMaterialProperty, value); }
        }

        /// <summary>
        /// Callback called when the EndMaterial property's value has changed.
        /// Assign the material to the inner models.
        /// </summary>
        /// <param name="d">Sender object</param>
        /// <param name="e">Callback arguments</param>
        internal static void EndMaterialPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CompositeBar3D bar = ((CompositeBar3D)d);
            if ((bar._element1 != null) && (bar._element1.Model !=null))
            {
                ((GeometryModel3D)bar._element1.Model).Material = bar.EndMaterial;
                ((GeometryModel3D)bar._element2.Model).Material = bar.EndMaterial;
            }
        }

        /// <summary>
        /// Indicates if default texture coordinates are calculated.
        /// Default value is false. 
        /// </summary>
        public bool DefaultTextureMapping
        {
            get { return (bool)GetValue(DefaultTextureMappingProperty); }
            set { SetValue(DefaultTextureMappingProperty, value); }
        }

        /// <summary>
        /// Identifies the DefaultTextureMapping dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultTextureMappingProperty =
            DependencyProperty.Register(
                "DefaultTextureMapping",
                typeof(bool),
                typeof(CompositeBar3D),
                new PropertyMetadata(
                    false));


        /// <summary>
        /// Callback to call in subclasses when a visual dependency property value has changed (i.e. by databinding).
        /// </summary>
        /// <param name="d">Sender object</param>
        /// <param name="e">Callback arguments</param>
        protected static void VisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CompositeBar3D element = (CompositeBar3D)d;
            element.InvalidateModel();
        }

        ModelUIElement3D _element1, _element2, _element0;

        /// <summary>
        /// Constructor.
        /// </summary>
        public CompositeBar3D()
            : base()
        {
            SolidColorBrush polygonBrush = new SolidColorBrush();
            polygonBrush.Color = Color.FromArgb(0xFF, 0xFF, 0xDE, 0x46);
            polygonBrush.Opacity = 1.0;
            _defaultMaterial = new DiffuseMaterial(polygonBrush);


            GeometryModel3D model0 = new GeometryModel3D();
            model0.Material = _defaultMaterial;
            _element0 = new ModelUIElement3D();
            _element0.Model = model0;
            this.AddVisual3DChild(_element0);

            GeometryModel3D model1 = new GeometryModel3D();
            model1.Material = _defaultMaterial;
            _element1 = new ModelUIElement3D();
            _element1.Model = model1;
            this.AddVisual3DChild(_element1);

            GeometryModel3D model2 = new GeometryModel3D();
            model2.Material = _defaultMaterial;
            _element2 = new ModelUIElement3D();
            _element2.Model = model2;
            this.AddVisual3DChild(_element2);
        }

        /// <summary>
        /// Called by UIElement3D.InvalidateModel() to update the 3D model.
        /// </summary>
        protected override void OnUpdateModel()
        {
            _sculptor = new BarSculptor(SideCount, InitialAngle, RoundingRate);
            _sculptor.BuildMesh();
            if (DefaultTextureMapping)
            {
                _sculptor.MapTexture();
                (_sculptor as BarSculptor).PolygonSculptor1.MapTexture();
                (_sculptor as BarSculptor).PolygonSculptor2.MapTexture();
            }
            ((GeometryModel3D)_element0.Model).Geometry = (_sculptor as BarSculptor).Mesh;
            ((GeometryModel3D)_element1.Model).Geometry = (_sculptor as BarSculptor).PolygonSculptor1.Mesh;
            ((GeometryModel3D)_element2.Model).Geometry = (_sculptor as BarSculptor).PolygonSculptor2.Mesh;
            InvalidateProperty(MaterialProperty);
            InvalidateProperty(EndMaterialProperty);
            base.OnUpdateModel(); 
        }

        /// <summary>
        /// Overrides Visual3D.GetVisual3DChild(int index)
        /// </summary>
        protected override Visual3D GetVisual3DChild(int index)
        {
            Visual3D v = null;
            switch (index)
            {
                case 0:
                    v = (Visual3D)_element0;
                    break;
                case 1:
                    v = (Visual3D)_element1;
                    break;
                case 2:
                    v = (Visual3D)_element2;
                    break;
            }
            return v;
        }

        /// <summary>
        /// Overrides Visual3D.Visual3DChildrenCount
        /// </summary>
        protected override int Visual3DChildrenCount
        {
            get
            {
                return 3;
            }
        }
    }
}
