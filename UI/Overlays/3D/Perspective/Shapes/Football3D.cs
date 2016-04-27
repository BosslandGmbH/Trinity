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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Trinity.UI.Overlays._3D.Perspective.Sculptors;

namespace Trinity.UI.Overlays._3D.Perspective.Shapes
{
    /// <summary>
    /// A 3D football (soccer ball) element.
    /// Default radius is 1.0.
    /// </summary>
    public class Football3D : UIElement3D
    {
        private TruncatedIsocahedronSculptor _sculptor = new TruncatedIsocahedronSculptor();
        private static Material _defaultHexagonMaterial, _defaultPentagonMaterial;

        private List<ModelUIElement3D> _pentagonList = new List<ModelUIElement3D>();

        /// <summary>
        /// Gets the ModelUIElement3D pentagonal object list
        /// </summary>
        internal List<ModelUIElement3D> PentagonList
        {
            get { return _pentagonList; }
        }

        private List<ModelUIElement3D> _hexagonList = new List<ModelUIElement3D>();

        /// <summary>
        /// Gets the ModelUIElement3D hexagonal object list
        /// </summary>
        internal List<ModelUIElement3D> HexagonList
        {
            get { return _hexagonList; }
        }

        /// <summary>
        /// Initializes a new instance of FootBall3D.
        /// </summary>
        public Football3D()
        {
            SolidColorBrush pentagonBrush = new SolidColorBrush();
            pentagonBrush.Color = Colors.Black;
            pentagonBrush.Opacity = 1.0;
            _defaultPentagonMaterial = new DiffuseMaterial(pentagonBrush);

            SolidColorBrush hexagonBrush = new SolidColorBrush();
            hexagonBrush.Color = Colors.White;
            hexagonBrush.Opacity = 1.0;
            _defaultHexagonMaterial = new DiffuseMaterial(hexagonBrush);

            _sculptor.BuildMesh();

            foreach (PolygonSculptor ps in _sculptor.PentagonList)
            {
                GeometryModel3D model = new GeometryModel3D();
                model.Geometry = ps.Mesh;
                model.Material = _defaultPentagonMaterial;

                ModelUIElement3D element = new ModelUIElement3D();
                element.Model = model;
                _pentagonList.Add(element);
                this.AddVisual3DChild(element);
            }

            foreach (PolygonSculptor ps in _sculptor.HexagonList)
            {
                GeometryModel3D model = new GeometryModel3D();
                model.Geometry = ps.Mesh;
                model.Material = _defaultHexagonMaterial;

                ModelUIElement3D element = new ModelUIElement3D();
                element.Model = model;
                _hexagonList.Add(element);
                this.AddVisual3DChild(element);
            }
        }

        /// <summary>
        /// Overrides Visual3D.GetVisual3DChild(int index)
        /// </summary>
        protected override Visual3D GetVisual3DChild(int index)
        {
            Visual3D v;
            if (index >= this.PentagonList.Count)
            {
                v = (Visual3D)this.HexagonList[index - _sculptor.PentagonList.Count];
            }
            else
            {
                v = (Visual3D)this.PentagonList[index];
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
                return this.PentagonList.Count + this.HexagonList.Count;
            }
        }

        /// <summary>
        /// Identifies the HexagonMaterial dependency property.
        /// </summary>
        public static DependencyProperty HexagonMaterialProperty =
            DependencyProperty.Register(
                "HexagonMaterial",
                typeof(Material),
                typeof(Football3D), new PropertyMetadata(
                    _defaultHexagonMaterial, 
                    HexagonMaterialPropertyChanged));

        /// <summary>
        /// Gets or sets the model hexagons material.
        /// </summary>
        public Material HexagonMaterial
        {
            get { return (Material)GetValue(HexagonMaterialProperty); }
            set { SetValue(HexagonMaterialProperty, value); }
        }

        /// <summary>
        /// Callback called when the HexagonMaterial property's value has changed.
        /// Assign the material to the inner models.
        /// </summary>
        /// <param name="d">Sender object</param>
        /// <param name="e">Callback arguments</param>
        internal static void HexagonMaterialPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Football3D fb = ((Football3D)d);
            foreach (ModelUIElement3D element in fb._hexagonList)
            {
                ((GeometryModel3D)element.Model).Material = fb.HexagonMaterial;
            }
        }

        /// <summary>
        /// Identifies the PentagonMaterial dependency property.
        /// </summary>
        public static DependencyProperty PentagonMaterialProperty =
            DependencyProperty.Register(
                "PentagonMaterial",
                typeof(Material),
                typeof(Football3D), new PropertyMetadata(
                    _defaultPentagonMaterial, 
                    PentagonMaterialPropertyChanged));

        /// <summary>
        /// Gets or sets the model pentagons material.
        /// </summary>
        public Material PentagonMaterial
        {
            get { return (Material)GetValue(PentagonMaterialProperty); }
            set { SetValue(PentagonMaterialProperty, value); }
        }

        /// <summary>
        /// Callback called when the PentagonMaterial property's value has changed.
        /// Assign the material to the inner models.
        /// </summary>
        /// <param name="d">Sender object</param>
        /// <param name="e">Callback arguments</param>
        internal static void PentagonMaterialPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Football3D fb = ((Football3D)d);
            foreach (ModelUIElement3D element in fb._pentagonList)
            {
                ((GeometryModel3D)element.Model).Material = fb.PentagonMaterial;
            }
        }
    }
}
