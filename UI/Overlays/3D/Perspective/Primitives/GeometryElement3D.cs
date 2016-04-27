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

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Trinity.UI.Overlays._3D.Perspective.Primitives
{
    /// <summary>
    /// Basic abstract class for Perspective.Wpf3D elements with 
    /// an inner model.
    /// </summary>
    public abstract class GeometryElement3D : UIElement3D
    {
        /// <summary>
        /// Initializes a new instance of GeometryElement3D.
        /// </summary>
        protected GeometryElement3D()
        {
        }

        public int Id { get; set; }

        /// <summary>
        /// Initializes a new instance of GeometryElement3D.
        /// </summary>
        /// <param name="geometry">A Geometry3D object (i.e. MeshGeometry3D)</param>
        protected GeometryElement3D(Geometry3D geometry)
        {
            Geometry = geometry;
        }

        /// <summary>
        /// Gets or sets the element's model.
        /// This is a private property
        /// Pattern described here : http://blogs.msdn.com/wpf3d/archive/2007/09/05/subclassing-uielement3d.aspx
        /// </summary>
        private Model3D Model
        {
            get { return (Model3D)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        /// <summary>
        /// Identifies the Model dependency property.
        /// This is a private field
        /// Pattern described here : http://blogs.msdn.com/wpf3d/archive/2007/09/05/subclassing-uielement3d.aspx
        /// </summary>
        private static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register(
                "Model", 
                typeof(Model3D), 
                typeof(GeometryElement3D),
                new PropertyMetadata(ModelPropertyChanged));

        /// <summary>
        /// Callback called when the Model property's value has changed.
        /// Assign the Visual3DModel protected CLR property
        /// Pattern described here : http://blogs.msdn.com/wpf3d/archive/2007/09/05/subclassing-uielement3d.aspx
        /// </summary>
        /// <param name="d">Sender object</param>
        /// <param name="e">Callback arguments</param>
        private static void ModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GeometryElement3D element = (GeometryElement3D)d;
            element.Visual3DModel = (Model3D)e.NewValue;
        }

        /// <summary>
        /// Static constructor.
        /// Initialization of the default material.
        /// </summary>
        static GeometryElement3D()
        {
            SolidColorBrush solidColorBrush = new SolidColorBrush();
            // solidColorBrush.Color = Colors.Goldenrod;
            // solidColorBrush.Color = Colors.LightGray;
            // solidColorBrush.Opacity = 1.0;
            solidColorBrush.Color = Color.FromArgb(0xFF, 0xFF, 0xDE, 0x46);
        
            Material defaultMaterial = new DiffuseMaterial(solidColorBrush);

            BackMaterialProperty = DependencyProperty.Register(
                "BackMaterial", 
                typeof(Material), 
                typeof(GeometryElement3D),
                new PropertyMetadata(defaultMaterial, VisualPropertyChanged));

            MaterialProperty = DependencyProperty.Register(
                "Material",
                typeof(Material),
                typeof(GeometryElement3D),
                new PropertyMetadata(defaultMaterial, VisualPropertyChanged));
        }

        /// <summary>
        /// Gets or sets the element's back material.
        /// A default material is provided.
        /// </summary>
        public Material BackMaterial
        {
            get { return (Material)GetValue(BackMaterialProperty); }
            set { SetValue(BackMaterialProperty, value); }
        }

        /// <summary>
        /// Identifies the BackMaterial dependency property.
        /// </summary>
        public static readonly DependencyProperty BackMaterialProperty;

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
        public static readonly DependencyProperty MaterialProperty;

        /// <summary>
        /// Called by UIElement3D.InvalidateModel() to update the 3D model.
        /// Assignation of a new model to the Model property
        /// Pattern described here : http://blogs.msdn.com/wpf3d/archive/2007/09/05/subclassing-uielement3d.aspx
        /// </summary>
        protected override void OnUpdateModel()
        {
            if (IsFrozen)
                return;

            if (_sculptor != null)
            {
                _sculptor.BuildMesh();
                Geometry = _sculptor.Mesh;
                if (_meshBuilt != null)
                {
                    // MeshBuiltEventArgs e = new MeshBuiltEventArgs(_sculptor.Mesh, _sculptor.Triangles);
                    MeshBuiltEventArgs e = new MeshBuiltEventArgs(_sculptor.Mesh);
                    _meshBuilt(this, e);
                    if (_sculptor.Mesh.TextureCoordinates.Count == 0)
                    {
                        DoMapTexture();
                    }
                    //if (!e.TextureCoordinatesHandled)
                    //{
                    //    _sculptor.MapTexture();
                    //}
                }
                else
                {
                    DoMapTexture();
                }
            }

            GeometryModel3D model = new GeometryModel3D();
            // model.Geometry = BuildGeometry();
            model.Geometry = this.Geometry;
            model.Material = this.Material;
            model.BackMaterial = this.BackMaterial;
            Model = model;
        }

        public bool IsFrozen;

        public void Freeze()
        {
            OnUpdateModel();
            IsFrozen = true;            

            if (Geometry != null && Geometry.CanFreeze)
                Geometry.Freeze();
            if (Material != null && Material.CanFreeze)
                Material.Freeze();
            if (BackMaterial != null && BackMaterial.CanFreeze)
                BackMaterial.Freeze();
            if (Model != null && Model.CanFreeze)
                Model.Freeze();
        }

        private void DoMapTexture()
        {
            if ((_sculptor != null) && DefaultTextureMapping)
            {
                _sculptor.MapTexture();
            }
        }

        /// <summary>
        /// Indicates if default texture coordinates are calculated (for classes that implement MapTexture()).
        /// Default value depends of the class. 
        /// It is true for Box3D and Square3D.
        /// It is false for other classes (when texture coordinates computing is intensive).
        /// This property is ignored if custom texture coordinates are specified in the MeshBuilt event.
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
                typeof(GeometryElement3D), 
                new PropertyMetadata(
                    false));

        /// <summary>
        /// Callback to call in subclasses when a visual dependency property value has changed (i.e. by databinding).
        /// </summary>
        /// <param name="d">Sender object</param>
        /// <param name="e">Callback arguments</param>
        protected static void VisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GeometryElement3D element = (GeometryElement3D)d;
            element.InvalidateModel();
        }

        /// <summary>
        /// Gets or sets the element's geometry.
        /// </summary>
        public Geometry3D Geometry
        {
            get { return (Geometry3D)GetValue(GeometryProperty); }
            set { SetValue(GeometryProperty, value); }
        }

        /// <summary>
        /// Identifies the Material dependency property.
        /// </summary>
        protected static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register(
                "Geometry", 
                typeof(Geometry3D), 
                typeof(GeometryElement3D),
                new PropertyMetadata(VisualPropertyChanged));

        private Sculptor _sculptor;

        /// <summary>
        /// Gets or sets the associated Sculptor object.
        /// </summary>
        // protected Sculptor Sculptor
        public Sculptor Sculptor
        {
            get { return _sculptor; }
            set { _sculptor = value; }
        }

        private event EventHandler<MeshBuiltEventArgs> _meshBuilt;

        /// <summary>
        /// Event fired when the the positions and triangle indices of the mesh are built.
        /// This is a CLR event.
        /// </summary>
        public event EventHandler<MeshBuiltEventArgs> MeshBuilt
        {
            add
            {
                _meshBuilt += value;
            }
            remove
            {
                _meshBuilt -= value;
            }
        }
    }
}
