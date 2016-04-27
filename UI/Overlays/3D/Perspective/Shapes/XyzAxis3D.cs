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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Trinity.UI.Overlays._3D.Perspective.Primitives;
using Trinity.UI.Overlays._3D.Perspective.Sculptors;

namespace Trinity.UI.Overlays._3D.Perspective.Shapes
{
    /// <summary>
    /// A class that represents a 3D XYZ axis
    /// Default length of each axis is 1.0.
    /// Default radius of the arrows body is 0.025.
    /// Default radius of the arrows head is 0.05.
    /// </summary>
    public class XyzAxis3D : UIElement3D
    {
        private XyzAxisSculptor _sculptor = new XyzAxisSculptor();
        // private static Color DefaultColor = Colors.Red;
        private static Color DefaultColor = Colors.LightGray;
        private ModelUIElement3D _arrowsElement = new ModelUIElement3D();
        private GeometryModel3D _arrowsModel = new GeometryModel3D();
        private Material _arrowsMaterial;

        ModelUIElement3D[] _labelElements = new ModelUIElement3D[3];
        SquareSculptor[] _labelSculptors = new SquareSculptor[3];
        GeometryModel3D[] _labelModels = new GeometryModel3D[3];
        Material[] _labelMaterials = new Material[3];

        /// <summary>
        /// Initializes a new instance of XyzAxis3D.
        /// </summary>
        public XyzAxis3D()
        {
            SolidColorBrush solidColorBrush = new SolidColorBrush();
            solidColorBrush.Color = XyzAxis3D.DefaultColor;
            _arrowsMaterial = new DiffuseMaterial(solidColorBrush);

            for (int i = 0; i <= 2; i++)
            {
                _labelElements[i] = new ModelUIElement3D();
                _labelModels[i] = new GeometryModel3D();
                _labelMaterials[i] = BuildLabelMaterial((AxisDirection)i);
            }
        }

        /// <summary>
        /// Build the sculptor for the label.
        /// </summary>
        /// <param name="ad">An AxisDirection enum value.</param>
        /// <returns></returns>
        private SquareSculptor BuildLabelSculptor(AxisDirection ad)
        {
            SquareSculptor sculptor = new SquareSculptor();
            Transform3DGroup tg = new Transform3DGroup();
            double xyLength = 0.2;
            tg.Children.Add(new ScaleTransform3D(xyLength, xyLength, 1.0));
            switch (ad)
            {
                case AxisDirection.X:
                    // The 0.001 Z translation prevents flickering when displaying another surface on the X-Y axis
                    tg.Children.Add(new TranslateTransform3D(0.6, -(xyLength + Radius / 2.0), -0.001));
                    break;
                case AxisDirection.Y:
                    // The 0.001 Z translation prevents flickering when displaying another surface on the X-Y axis
                    tg.Children.Add(new TranslateTransform3D(Radius / 2.0, 0.6, -0.001));
                    break;
                case AxisDirection.Z:
                    tg.Children.Add(
                        new RotateTransform3D(
                            new AxisAngleRotation3D(new Vector3D(0, -1, 0), 90)));
                    // 2nd rotation so the 3 letters X, Y and Z are in the same direction
                    tg.Children.Add(
                        new RotateTransform3D(
                            new AxisAngleRotation3D(new Vector3D(0, 1, 0), 180)));
                    // The 0.001 X translation prevents flickering when displaying another surface on the Y-Z axis
                    // tg.Children.Add(new TranslateTransform3D(-0.001, -(xyLength + Radius / 2.0), 0.8));
                    double zTranslation = 
                        (this.CoordinateSystemKind == Primitives.CoordinateSystemKind.RightHanded) ? 0.8 : -0.8;
                    tg.Children.Add(new TranslateTransform3D(-0.001, -(xyLength + Radius / 2.0), zTranslation));
                    break;
            }
            sculptor.Transform(tg);
            sculptor.BuildMesh();
            sculptor.MapTexture();
            return sculptor;
        }

        /// <summary>
        /// Build the material for the label.
        /// </summary>
        /// <param name="ad">An AxisDirection enum value.</param>
        /// <returns></returns>
        private static Material BuildLabelMaterial(AxisDirection ad)
        {
            MaterialGroup mg = new MaterialGroup();
            mg.Children.Add(new DiffuseMaterial(new SolidColorBrush(Colors.LightGray)));

            TextBlock tb = new TextBlock();
            tb.FontFamily = new FontFamily("Verdana");
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.TextAlignment = TextAlignment.Center;
            tb.Foreground = Brushes.Black;
            VisualBrush vb = new VisualBrush();
            vb.Visual = tb;
            vb.Stretch = Stretch.Uniform;
            switch (ad)
            {
                case AxisDirection.X:
                    tb.Text = "X";
                    break;
                case AxisDirection.Y:
                    tb.Text = "Y";
                    break;
                case AxisDirection.Z:
                    tb.Text = "Z";
                    break;
            }
            mg.Children.Add(new DiffuseMaterial(vb));
            return mg;
        }

        /// <summary>
        /// Called by UIElement3D.InvalidateModel() to update the 3D model.
        /// </summary>
        protected override void OnUpdateModel()
        {
            _sculptor.Initialize(Length, Radius, Signed, CoordinateSystemKind);
            _sculptor.BuildMesh();
            _arrowsModel.Geometry = _sculptor.Mesh;
            _arrowsModel.Material = _arrowsMaterial;
            _arrowsElement.Model = _arrowsModel;
            this.AddVisual3DChild(_arrowsElement);

            for (int i = 0; i <= 2; i++)
            {
                _labelSculptors[i] = BuildLabelSculptor((AxisDirection)i);
                _labelModels[i].Geometry = _labelSculptors[i].Mesh;
                _labelModels[i].Material = _labelMaterials[i];
                _labelModels[i].BackMaterial = _labelMaterials[i];
                _labelElements[i].Model = _labelModels[i];
                this.AddVisual3DChild(_labelElements[i]);
            }
            base.OnUpdateModel();
        }

        /// <summary>
        /// Overrides Visual3D.GetVisual3DChild(int index)
        /// </summary>
        protected override Visual3D GetVisual3DChild(int index)
        {
            Visual3D v = null;
            if (index == 0)
            {
                v = _arrowsElement;
            }
            else
            {
                v = _labelElements[index - 1];
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
                return 1 + _labelElements.GetLength(0);
            }
        }

        /// <summary>
        /// Gets or sets the axis length.
        /// Default value is 1.0.
        /// </summary>
        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        /// <summary>
        /// Identifies the Length dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty =
            Arrow3D.LengthProperty.AddOwner(
                typeof(XyzAxis3D),
                new PropertyMetadata(
                    XyzAxisSculptor.DefaultLength,
                    VisualPropertyChanged));

        /// <summary>
        /// Gets or sets the axis body's radius.
        /// Default radius of the arrows body is 0.025.
        /// Default radius of the arrows head is 0.05.
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
            Ring3D.RadiusProperty.AddOwner(
                typeof(XyzAxis3D),
                new PropertyMetadata(
                    XyzAxisSculptor.DefaultRadius,
                    VisualPropertyChanged));

        /// <summary>
        /// Gets or sets the axis signed characteristic.
        /// Default value is false.
        /// </summary>
        public bool Signed
        {
            get { return (bool)GetValue(SignedProperty); }
            set { SetValue(SignedProperty, value); }
        }

        /// <summary>
        /// Identifies the Signed dependency property.
        /// </summary>
        public static readonly DependencyProperty SignedProperty =
            DependencyProperty.Register(
                "Signed",
                typeof(bool),
                typeof(XyzAxis3D),
                new PropertyMetadata(false, VisualPropertyChanged));

        /// <summary>
        /// Callback called when a visual dependency property value has changed (i.e. by databinding).
        /// </summary>
        /// <param name="d">Sender object</param>
        /// <param name="e">Callback arguments</param>
        protected static void VisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            XyzAxis3D element = (XyzAxis3D)d;
            element.InvalidateModel();
        }

        /// <summary>
        /// Gets or sets the axis color.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// Identifies the Color dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(
                "Color",
                typeof(Color),
                typeof(XyzAxis3D),
                new PropertyMetadata(
                    XyzAxis3D.DefaultColor,
                    ColorPropertyChanged));

        /// <summary>
        /// Callback called when the Color value has changed (i.e. by databinding).
        /// </summary>
        /// <param name="d">Sender object</param>
        /// <param name="e">Callback arguments</param>
        private static void ColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            XyzAxis3D element = (XyzAxis3D)d;

            SolidColorBrush solidColorBrush = new SolidColorBrush();
            solidColorBrush.Color = element.Color;
            element._arrowsMaterial = new DiffuseMaterial(solidColorBrush);
            element.InvalidateModel();
        }

        /// <summary>
        /// Gets or sets the kind of coordinate system.
        /// The default value is CoordinateSystemKind.RightHanded.
        /// </summary>
        public CoordinateSystemKind CoordinateSystemKind
        {
            get { return (CoordinateSystemKind)GetValue(CoordinateSystemKindProperty); }
            set { SetValue(CoordinateSystemKindProperty, value); }
        }

        /// <summary>
        /// Identifies the CoordinateSystemKind dependency property.
        /// </summary>
        public static readonly DependencyProperty CoordinateSystemKindProperty =
            DependencyProperty.Register(
                "CoordinateSystemKind", 
                typeof(CoordinateSystemKind), 
                typeof(XyzAxis3D), 
                new PropertyMetadata(
                    CoordinateSystemKind.RightHanded,
                    VisualPropertyChanged));
    }
}
