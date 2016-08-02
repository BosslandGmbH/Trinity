using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Trinity.Components.Adventurer.UI.UIComponents.RadarCanvas;
using Trinity.Components.Adventurer.Util;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.UI.UIComponents.RadarViewport
{

    public class RadarViewport : Viewport3D
    {
 

        public RadarViewport()
        {



            ClipToBounds = true;            
            
            //CanvasData.OnCanvasSizeChanged += (before, after) =>
            //{
            //    // Scene drawings are specific to a canvas size.
            //    // Changing canvas size means we have to redraw them all.
            //    Drawings.Relative.Clear();
            //    Drawings.Static.Clear();
            //    Clip = CanvasData.ClipRegion;
            //};          

            //if (_internalTimer == null)
            //{
            //    _internalTimer = new DispatcherTimer();
            //    _internalTimer.Tick += InternalTimerTick;
            //    _internalTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            //    _internalTimer.Start();
            //}
        }

        //private void InternalTimerTick(object sender, EventArgs e)
        //{
        //    Application.Current.Dispatcher.Invoke(() =>
        //    {

        //    });
        //}

        /// <summary>
        /// The canvas size of grid squares (in pixels) for 1yd of game distance
        /// </summary>
        public int GridSize = 5;

        /// <summary>
        /// The number of grid squares/yards to draw horizontal/vertical lines on
        /// </summary>
        public int GridLineFrequency = 10;

        /// <summary>
        /// How many compiled scene drawings to keep around
        /// </summary>
        public int RelativeGeometryStorageLimit = 100;

        /// <summary>
        /// The size (in pixels) to draw actor markers
        /// </summary>
        public double MarkerSize = 5;

        /// <summary>
        /// The actor who should be at the center of the radar
        /// </summary>
        public RadarObject CenterActor { get; set; }

        /// <summary>
        /// Collection of game objects
        /// </summary>
        public List<RadarObject> Objects = new List<RadarObject>();

        /// <summary>
        /// Information about the WPF canvas we'll be drawing on
        /// </summary>
        public CanvasData CanvasData = new CanvasData();

        #region ItemSource Property

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource",
                typeof(IList),
                typeof(RadarViewport),
                new PropertyMetadata(null, OnItemsSourceChanged));
        
        //private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        
        public IList ItemsSource
        {
            set { SetValue(ItemsSourceProperty, value); }
            get { return (IList)GetValue(ItemsSourceProperty); }
        }

        static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var radarCanvas = obj as RadarViewport;
            if (radarCanvas != null) 
                radarCanvas.OnItemsSourceChanged(args);
        }

        #endregion

        #region ItemSource Changed Event Handling

        /// <summary>
        /// ItemSource binding on control is set
        /// </summary>
        void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateData();


            var oldValue = args.OldValue as INotifyCollectionChanged;
            if (oldValue != null)
                oldValue.CollectionChanged -= OnItemsSourceCollectionChanged;

            var newValue = args.NewValue as INotifyCollectionChanged;
            if (newValue != null)
                newValue.CollectionChanged += OnItemsSourceCollectionChanged;
        }

        /// <summary>
        /// When objects inside ItemSource collection change
        /// </summary>
        void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {

            UpdateData();
        }

        #endregion

        #region Canvas Changed Event Handling

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CanvasData.Update(DesiredSize, GridSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableSize.Width == double.PositiveInfinity || availableSize.Height == double.PositiveInfinity)
            {
                return Size.Empty;
            }
            
            // Not entirely sure what this does
            return availableSize;
        }

        #endregion

        /// <summary>
        /// Go through the ItemSource collection and calculate their canvas positions
        /// </summary>
        void UpdateData()
        {
            try
            {
                using (new PerformanceLogger("RadarUI UpdateData"))
                {
                    Logger.Debug("DataUpdating");
                    
                    if (DesiredSize.Height <= 0 || DesiredSize.Width <= 0)
                        return;

                    if (!IsVisible || ZetaDia.Me == null || ZetaDia.IsLoadingWorld || !ZetaDia.IsInGame)
                        return;

                    Objects.Clear();

                    CanvasData.Update(DesiredSize, GridSize);

                    // Find the actor who should be in the center of the radar
                    // and whos position all other points should be plotted against.

                    var center = ZetaDia.Me;
                    if (center == null)
                        return;

                    CenterActor = new RadarObject(center, CanvasData);
                    CanvasData.CenterVector = CenterActor.Actor.Position;
                    CanvasData.CenterMorph = CenterActor.Morph;

                    // Calculate locations for all actors positions
                    // on RadarObject ctor; or with .Update();

                    foreach (var trinityObject in ItemsSource.OfType<DiaObject>())
                    {
                        var radarObject = new RadarObject(trinityObject, CanvasData);
                        Objects.Add(radarObject);
                    }

                    Logger.Debug("DataUpdated");

                    //UpdateRelativeDrawings();

                    //UpdateAvoidanceGridData();

                    //Logger.Log("Heading={0}",ZetaDia.Me.Movement.RotationDegrees);

                }

                // Trigger Canvas to Render
                InvalidateVisual();  

            }
            catch (Exception ex)
            {
                Logger.Debug("Exception in RadarUI.UpdateData(). {0} {1}", ex.Message, ex.InnerException);
            }
        }



      
        /// <summary>
        /// OnRender is a core part of Canvas, replace it with our render code. Can be manually triggered by InvalidateVisual();
        /// Its very important to minimize work in OnRender, use UpdateData() to cache whatever it is you need.
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            using (new PerformanceLogger("RadarUI Render"))
            {
                Logger.Debug("OnRender Called");

                if (CanvasData.CanvasSize.Width == 0 && CanvasData.CanvasSize.Height == 0 || CanvasData.CenterVector == Vector3.Zero)
                    return;

                if (CenterActor.Point.X == 0 && CenterActor.Point.Y == 0)
                    return;

                if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld)
                    return;

                try
                {
                    //DrawGrid(dc, CanvasData, GridLineFrequency);

                    //DrawScenes(dc, CanvasData);

                    //DrawRangeGuide(dc, CanvasData);

                    


                    //DrawAvoidanceGrid(dc, CanvasData);

                    //dc.DrawRectangle(null, new Pen(Brushes.WhiteSmoke, 1), CanvasData.ClipRegion.Rect);

                    //DrawCurrentpath(dc, CanvasData);

                    //DrawSafeNodes(dc, CanvasData);

                    var mat = GetSurfaceMaterial(Colors.PaleGoldenrod);

                    foreach (var actor in Objects)
                    {
                        var cube = GetCube(mat, 
                            new Point3D(actor.Point.Y, actor.Point.X, 0),
                            new Point3D(actor.Point.Y+5, actor.Point.X+5, 5));

                        Children.Add(cube);
                        //if (!actor.Morph.IsBeyondCanvas)
                        //{
                        //    DrawActor(dc, CanvasData, actor);
                        //    //DrawActualObjectBounds(dc, CanvasData, actor.Actor);
                        //}                                                    
                    }
                   
                    //DrawLineToSafeNode(dc, CanvasData);

                    //DrawTest(dc, CanvasData);
                    base.OnRender(dc);

                }
                catch (Exception ex)
                {
                    Logger.Debug("Exception in RadarUI.OnRender(). {0} {1}", ex.Message, ex.InnerException);
                }
            }
        }

        public ModelVisual3D GetCube(MaterialGroup materialGroup, Point3D nearPoint, Point3D farPoint)
        {
            var cube = new Model3DGroup();
            var p0 = new Point3D(farPoint.X, farPoint.Y, farPoint.Z);
            var p1 = new Point3D(nearPoint.X, farPoint.Y, farPoint.Z);
            var p2 = new Point3D(nearPoint.X, farPoint.Y, nearPoint.Z);
            var p3 = new Point3D(farPoint.X, farPoint.Y, nearPoint.Z);
            var p4 = new Point3D(farPoint.X, nearPoint.Y, farPoint.Z);
            var p5 = new Point3D(nearPoint.X, nearPoint.Y, farPoint.Z);
            var p6 = new Point3D(nearPoint.X, nearPoint.Y, nearPoint.Z);
            var p7 = new Point3D(farPoint.X, nearPoint.Y, nearPoint.Z);
            //front side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p3, p2, p6));
            cube.Children.Add(CreateTriangleModel(materialGroup, p3, p6, p7));
            //right side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p2, p1, p5));
            cube.Children.Add(CreateTriangleModel(materialGroup, p2, p5, p6));
            //back side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p1, p0, p4));
            cube.Children.Add(CreateTriangleModel(materialGroup, p1, p4, p5));
            //left side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p0, p3, p7));
            cube.Children.Add(CreateTriangleModel(materialGroup, p0, p7, p4));
            //top side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p7, p6, p5));
            cube.Children.Add(CreateTriangleModel(materialGroup, p7, p5, p4));
            //bottom side triangles
            cube.Children.Add(CreateTriangleModel(materialGroup, p2, p3, p0));
            cube.Children.Add(CreateTriangleModel(materialGroup, p2, p0, p1));
            var model = new ModelVisual3D();
            model.Content = cube;
            return model;
        }

        public MaterialGroup GetSurfaceMaterial(Color colour)
        {
            var materialGroup = new MaterialGroup();
            var emmMat = new EmissiveMaterial(new SolidColorBrush(colour));
            materialGroup.Children.Add(emmMat);
            materialGroup.Children.Add(new DiffuseMaterial(new SolidColorBrush(colour)));
            var specMat = new SpecularMaterial(new SolidColorBrush(Colors.White), 30);
            materialGroup.Children.Add(specMat);
            return materialGroup;
        }

        private Model3DGroup CreateTriangleModel(Material material, Point3D p0, Point3D p1, Point3D p2)
        {
            var mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            var normal = CalculateNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);

            var model = new GeometryModel3D(mesh, material);

            var group = new Model3DGroup();
            group.Children.Add(model);
            return group;
        }

        private Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            var v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }




    }







}
