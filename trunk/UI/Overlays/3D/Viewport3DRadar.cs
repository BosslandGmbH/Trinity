using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Xml;
using Adventurer.Util;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Helpers;
using Trinity.Objects;
using Trinity.UI.Overlays._3D;
using Trinity.UI.Overlays._3D.Perspective;
using Trinity.UI.Overlays._3D.Perspective.Primitives;
using Trinity.UI.Overlays._3D.Perspective.Shapes;
using Trinity.UI.Overlays._3D._3DTools;
using Zeta.Bot;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals;
using Logger = Trinity.Technicals.Logger;
using Quaternion = System.Windows.Media.Media3D.Quaternion;
using Timer = System.Timers.Timer;

namespace Trinity.UI.Overlays
{
    [ContentProperty("Children")]
    public class Viewport3DRadar : FrameworkElement // Viewport3D //FrameworkElement
    {
        private readonly ConcurrentDictionary<int, ViewPort3DActor> _actors = new ConcurrentDictionary<int, ViewPort3DActor>();
        private readonly SolidColorBrush _blueBrush = new SolidColorBrush(Colors.Blue);
        private readonly HashSet<int> _currentRActorGuids = new HashSet<int>();
        private readonly ModelVisual3D _defaultLightingModel = new ModelVisual3D();
        private readonly Stopwatch _movementTimer = new Stopwatch();
        private readonly Timer _refreshTimer;
        private int _frame;
        private Point3D _lastCameraPosition;
        private Vector3 _lastMoveDelta;
        private Spherical3D _playerSphere;
        private Timer _renderTimer;
        private Viewport3D _viewport = new Viewport3D();
        private ScreenSpaceLines3D _wireframe;
        private ModelVisual3D BlueSphere = new ModelVisual3D();

        public Viewport3DRadar()
        {
            this.Focusable = true;

            ClipToBounds = true;
            AllowDrop = false;
            IsHitTestVisible = false;
            SnapsToDevicePixels = false;
            VisualBitmapScalingMode = BitmapScalingMode.NearestNeighbor;

            _camera = new PerspectiveCamera();

            _viewport.Focusable = false;
            _viewport.ClipToBounds = true;
            _viewport.AllowDrop = false;
            _viewport.IsHitTestVisible = false;
            _viewport.SnapsToDevicePixels = true;
            _viewport.Camera = _camera;
            _viewport.VerticalAlignment = VerticalAlignment.Top;
            _viewport.HorizontalAlignment = HorizontalAlignment.Left;

            //CameraData cam;
            //using (ZetaDia.Memory.AcquireFrame())
            //{                
            //    cam = ZetaDia.Camera.Cameras[0];
            //}
            //Vector3 position = cam.Position;
            //float fov = cam.FieldOfView;

            //_camera.Position = position.ToPoint3D();
            //_camera.FieldOfView = fov;

            _camera.UpDirection = ViewPortModel.Instance.CameraUpDirection;
            _camera.LookDirection = ViewPortModel.Instance.CameraLookDirection;
            //_camera.NearPlaneDistance = 0.001f;
            _camera.Position = ViewPortModel.Instance.CameraPosition;


            //var center = new TranslateTransform3D(c.x, c.y, c.z);
            //var rot_x = new AxisAngleRotation3D(new Vector3D(1, 0, 0), myObject.ViewRotX);
            //var rot_y = new AxisAngleRotation3D(new Vector3D(0, 1, 0), myObject.ViewRotY);
            //var rot_z = new AxisAngleRotation3D(new Vector3D(0, 0, 1), myObject.ViewRotZ);
            //var zoom = new ScaleTransform3D(myObject.ViewZoom, myObject.ViewZoom, myObject.ViewZoom);
            //Transform3DGroup t = new Transform3DGroup();
            //t.Children.Add(zoom);
            //// the order of the following three is significant
            //t.Children.Add(new RotateTransform3D(rot_y));
            //t.Children.Add(new RotateTransform3D(rot_x));
            //t.Children.Add(new RotateTransform3D(rot_z));
            //t.Children.Add(center);
            //myViewport3D.Camera.Transform = t;
            //_camTransform = _camera.Transform;

            ViewPortModel.Instance.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "CameraFieldOfView")
                    _camera.FieldOfView = ViewPortModel.Instance.CameraFieldOfView;

                if (args.PropertyName == "CameraUpDirection")
                    _camera.UpDirection = ViewPortModel.Instance.CameraUpDirection;

                if (args.PropertyName == "CameraLookDirection" || args.PropertyName == "CameraLookDirection")
                    _camera.LookDirection = ViewPortModel.Instance.CameraLookDirection;

                if (args.PropertyName == "CameraPosition")
                    CameraMove();
                //    _camera.Position = ViewPortModel.Instance.CameraLookDirection;
            };

            SizeChanged += OnSizeChanged;



            CreateLighting();

            AddVisualChild(_viewport);

            _refreshTimer = new Timer(1000);
            _refreshTimer.Elapsed += TimerElapsed;
            _refreshTimer.Enabled = true;

            _renderTimer = new Timer(10);
            _renderTimer.Elapsed += RenderTimerElapsed;
            _renderTimer.Enabled = true;

            //Task.Factory.StartNew(RenderTask, TaskCreationOptions.LongRunning);

            //Task.Factory.StartNew(UpdatePosition, TaskCreationOptions.LongRunning);

            CompositionTarget.Rendering += CompositionTargetOnRendering;

            _dispatcher = Dispatcher;

            this.Focusable = true;
        }

        private void CameraMove(Vector3 position = default(Vector3))
        {
            var model = ViewPortModel.Instance;
            if (position == Vector3.Zero)
            {
                position = model.PlayerPosition;

                //CachedCameraData cam = new CachedCameraData();
                //using (ZetaDia.Memory.AcquireFrame())
                //{
                //    cam = new CachedCameraData(ZetaDia.Camera.Cameras[0]);
                //}
                //position = cam.Position;
                ////float fov = cam.FieldOfView;
            }

            var newCameraPos = GetCamPosition(position, model);
            //var cam = (PerspectiveCamera)Camera;
            _camera.Position = newCameraPos;
        }

        /// <summary>
        /// Gets a collection of the Visual3D children of the Workshop3D.
        /// </summary>
        [System.ComponentModel.DesignerSerializationVisibility(
            System.ComponentModel.DesignerSerializationVisibility.Content)]
        public Visual3DCollection Children
        {
            get { return (Visual3DCollection)GetValue(ChildrenProperty); }
        }

        /// <summary>
        /// Key of the Children dependency property.
        /// </summary>
        private static readonly DependencyPropertyKey ChildrenPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "Children",
                typeof(Visual3DCollection),
                typeof(Viewport3DRadar),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the Children dependency property.
        /// </summary>
        public static readonly DependencyProperty ChildrenProperty =
            ChildrenPropertyKey.DependencyProperty;

        /// <summary>
        /// Overrides Visual3D.GetVisualChild(int index)
        /// </summary>
        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            //if (index == 1)
            //{
            //    return _rsd;
            //}
            return _viewport;
        }

        /// <summary>
        /// Overrides Visual.VisualChildrenCount
        /// </summary>
        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        ///// <summary>
        ///// Measures the size in layout required for child elements and determines a size for the element.
        ///// </summary>
        ///// <param name="availableSize">The available size that this element can give to child elements.</param>
        ///// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        //protected override Size MeasureOverride(Size availableSize)
        //{
        //    Size desiredSize = new Size();



        //    //_rsd.Measure(availableSize);
        //    //desiredSize.Width += _rsd.DesiredSize.Width;
        //    //desiredSize.Height += _rsd.DesiredSize.Height;

        //    return availableSize;
        //}

        /// <summary>
        /// Positions child elements and determines a size for the element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rViewport = new Rect(finalSize);
            _viewport.Arrange(rViewport);

            //double space = 15.0;

            //// put the command panel in the bottom-left corner
            //Rect r = new Rect(
            //    rViewport.Width - _rsd.DesiredSize.Width - space,
            //    rViewport.Bottom - _rsd.DesiredSize.Height - space,
            //    _rsd.DesiredSize.Width,
            //    _rsd.DesiredSize.Height);
            //_rsd.Arrange(r);

            return finalSize;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs size)
        {
            _viewport.Height = size.NewSize.Height;
            _viewport.Width = size.NewSize.Width;
        }


        //<!--<Viewport3D.Camera>
        //    <PerspectiveCamera x:Name="camMain" 
        //        NearPlaneDistance="0.0001"
        //        FieldOfView="{Binding CameraFieldOfView, Mode=TwoWay, FallbackValue=20}"                        
        //        UpDirection="0,0,1" 
        //        LookDirection="{Binding CameraLookDirection, Mode=TwoWay}">
        //    </PerspectiveCamera>
        //</Viewport3D.Camera>-->

        Stopwatch UpdateSw = new Stopwatch();

        //private void UpdatePosition()
        //{
        //    while (true)
        //    {
        //        if (_camTransform.CheckAccess())
        //        {
        //            try
        //            {
        //                using (ZetaDia.Memory.AcquireFrame())
        //                {
        //                    Logger.Log("UpdateSw = {0} ZPos={1} TX={2} TY={3} TZ={4} CamAxx={5}", UpdateSw.Elapsed.TotalMilliseconds, _currentPosition,
        //                        _camTransform.Value.OffsetX, _camTransform.Value.OffsetY, _camTransform.Value.OffsetZ, _camera.CheckAccess());
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.Log("{0}", ex);
        //            }

        //            //var camPos = GetCamPosition(_projectedPosition, model);
        //            //_camTransform.Value.Translate();Translate(_lastMoveDelta.ToVector3D());
        //        }
        //        else
        //        {
        //            Logger.Log("No Access to Transform");
        //        }
        //        UpdateSw.Restart();
        //        Thread.Sleep(5);
        //    }
        //    //    if (DateTime.UtcNow.Subtract(_lastUpdate).TotalMilliseconds > 2.5)
        //    //    {
        //    //        using (ZetaDia.Memory.ReleaseFrame())
        //    //        {

        //    //        }
        //    //        using (ZetaDia.Memory.AcquireFrame(true))
        //    //        {
        //    //            //PlayerPosition = ZetaDia.Me.Position;

        //    //            //PlayerPosition = ViewPortModel.Instance.PlayerPosition; //ZetaInternals.ActivePlayerData.Position; // - _axialCylinderPosition; //MathEx.GetPointAt(ZetaDia.Me.Position, ZetaDia.Me.Position  ZetaDia.Me.CollisionSphere.Center, ZetaDia.Me.Movement.Rotation);// MathEx.WrapAngle((float)(ZetaDia.Me.Movement.Rotation + Math.PI)));
        //    //            ////ViewPortModel.Instance.PlayerPosition = PlayerPosition;

        //    //            //if (_lastPosition == PlayerPosition)
        //    //            //{
        //    //            //    Logger.Log("Updated Position: {0} UNCHANGED", DateTime.UtcNow.Subtract(_lastUpdate).TotalMilliseconds);
        //    //            //}
        //    //            //else
        //    //            //{
        //    //            //    if (!_cancellationToken.CanBeCanceled)
        //    //            //    {
        //    //            //        var action = new Action(CamRender);
        //    //            //        _cancellationToken = new CancellationToken();
        //    //            //        _dispatcher.Invoke(action, DispatcherPriority.Send, _cancellationToken);
        //    //            //    }
        //    //            //    else
        //    //            //    {
        //    //            //        Logger.Log("Update Pending");
        //    //            //    }

        //    //            //    Logger.Log("Updated Position: {0}", DateTime.UtcNow.Subtract(_lastUpdate).TotalMilliseconds);
        //    //            //}

        //    //            _lastUpdate = DateTime.UtcNow;
        //    //            _lastPosition = PlayerPosition;
        //    //        }
        //    //    }
        //    //}

        //}

        public Vector3 PlayerPosition { get; set; }

        //private Stopwatch renderSw = new Stopwatch();
        //private void RenderTask()
        //{
        //    while (true)
        //    {
        //        if (renderSw.Elapsed.TotalMilliseconds > 5 || !renderSw.IsRunning)
        //        {
        //            try
        //            {
        //                Application.Current.Dispatcher.BeginInvoke(new Action(CamRender));
        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.LogError("Exception {0}", ex);
        //            }

        //            renderSw.Restart();
        //        }
        //    }        
        //}

        public Point3D CenterPosition { get; set; }
        //private void InitializeCamera()
        //{
        //    //var camera = (_viewport.Camera as PerspectiveCamera);
        //    var camera = (Camera as PerspectiveCamera);
        //    //if (camera == null)
        //    //    return;

        //    camera.UpDirection = new Vector3D(0,0,1);
        //    camera.Position = new Point3D(111, 120, 110);
        //    //camera.LookDirection = new Vector3D(-9, -10, -9);
        //    camera.LookDirection = new Vector3D(-1,-1,-1);
        //    camera.NearPlaneDistance = 0;
        //    camera.FarPlaneDistance = 0;
        //    camera.FieldOfView = 20;
        //}

        private void SetNotAnimating(object sender, EventArgs eventArgs)
        {
            _isAnimating = false;
        }

        private void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
        {
            //if (BotMain.IsPausedForStateExecution)
            //    return;

            //SmoothedCameraMove();

            //CameraMove();
        }

        private static Point3D GetCamPosition(Vector3 centerPos, ViewPortModel model)
        {
            return new Point3D(centerPos.X + model.CameraPositionX, centerPos.Y + model.CameraPositionY, centerPos.Z + model.CameraPositionZ);
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(Refresh));
        }

        private void RenderTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(CamRender));
        }



        private void CamRender()
        {
            //Logger.Log("Time Since CamRender = {0}", DateTime.UtcNow.Subtract(_lastCamRender).TotalMilliseconds);

            //_lastCamRender = DateTime.UtcNow;

            if (BotMain.IsPausedForStateExecution)
                return;

            SmoothedCameraMove();
        }

        private void CreateLighting()
        {
            var light1 = new DirectionalLight(Colors.White, new Vector3D(0.0, -1.0, 1.0));
            var light2 = new DirectionalLight(Colors.White, new Vector3D(-1.0, -1.0, -1.0));
            var light3 = new DirectionalLight(Colors.White, new Vector3D(1.0, -1.0, -1.0));
            var light4 = new DirectionalLight(Colors.White, new Vector3D(0.0, 1.0, 0.0));
            light1.Freeze();
            light2.Freeze();
            light3.Freeze();
            light4.Freeze();
            var mg = new Model3DGroup();
            mg.Children.Add(light1);
            mg.Children.Add(light2);
            mg.Children.Add(light3);
            mg.Children.Add(light4);
            _defaultLightingModel.Content = mg;
            _viewport.Children.Add(_defaultLightingModel);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsPositiveInfinity(availableSize.Width) || double.IsPositiveInfinity(availableSize.Height))
            {
                return Size.Empty;
            }
            return availableSize;
        }

        private void Refresh()
        {
            try
            {
                var objects = ViewPortModel.Instance.Objects.ToList();

                _currentRActorGuids.Clear();

                foreach (var updatedWorldActor in objects)
                {
                    if (updatedWorldActor.Distance >= 30f)
                        continue;

                    ViewPort3DActor actor;
                    if (!_actors.TryGetValue(updatedWorldActor.RActorId, out actor))
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(async () => await CreateVisual(updatedWorldActor)));
                    }
                    else
                    {
                        actor.Update(updatedWorldActor);
                    }

                    if (!_currentRActorGuids.Contains(updatedWorldActor.RActorId))
                        _currentRActorGuids.Add(updatedWorldActor.RActorId);
                }

                foreach (var child in _viewport.Children.ToList())
                {
                    var visual = child as GeometryElement3D;
                    if (visual == null)
                        continue;

                    if (!_currentRActorGuids.Contains(visual.Id))
                    {
                        Logger.Log("Removing {0}", visual.Id);
                        ViewPort3DActor removedActor;
                        _actors.TryRemove(visual.Id, out removedActor);
                        _viewport.Children.Remove(child);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Exception in Viewport3DRadar.Refresh(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private async Task<bool> CreateVisual(TrinityActor updatedWorldActor)
        {
            ViewPort3DActor actor;
            var visual = CreateActorVisual(updatedWorldActor);
            visual.Id = updatedWorldActor.RActorId;
            visual.DefaultTextureMapping = false;
            visual.Freeze();
            actor = new ViewPort3DActor(updatedWorldActor, null, visual);

            _actors.TryAdd(updatedWorldActor.RActorId, actor);

            //_viewport.Children.Add(actor.Visual);
            _viewport.Children.Add(actor.Visual);
            Logger.Log("Created model for {0}, ({1})", updatedWorldActor.InternalName, updatedWorldActor.RActorId);
            return true;
        }

        private GeometryElement3D CreateActorVisual(TrinityActor actor)
        {
            var sphere = new Spherical3D();
            sphere.Material = GetSurfaceMaterial();
            return sphere;
        }

        public MaterialGroup GetSurfaceMaterial()
        {
            var materialGroup = new MaterialGroup();
            var emmMat = new EmissiveMaterial(_blueBrush);
            //emmMat.Freeze();
            var interactiveMaterial = new DiffuseMaterial();
            //interactiveMaterial.Freeze();
            //interactiveMaterial.SetValue(InteractiveVisual3D.IsInteractiveMaterialProperty, true);
            //materialGroup.Children.Add(interactiveMaterial);
            materialGroup.Children.Add(emmMat);
            materialGroup.Children.Add(new DiffuseMaterial());
            //var specMat = new SpecularMaterial(new SolidColorBrush(Colors.White), 30);
            //materialGroup.Children.Add(specMat);
            materialGroup.Freeze();
            return materialGroup;
        }

        #region Interpolated Movement

        private Point3DAnimation _cameraPositionAnimation;
        private Vector3 _lastPosition;
        private DateTime _lastFrameTime = DateTime.UtcNow;
        private Vector3 _lastPositionDelta;
        private double _lastTimeDelta;
        private DateTime _lastSmoothMoved = DateTime.MinValue;
        private Vector3 _lastSmoothMoveStartPosition;
        private bool _isAnimating;
        private DateTime _lastCamRender = DateTime.MinValue;
        private DateTime _lastUpdate = DateTime.MinValue;
        private Dispatcher _dispatcher;
        private CancellationToken _cancellationToken;
        private PerspectiveCamera _camera;
        private Vector3 _lastSmoothMoveTargetPosition;
        private double _lastMagnitude;
        private Vector3 _projectedPosition;
        private Vector3 _currentPosition;
        private Transform3D _camTransform;
        private CachedCameraData _lastCamData = new CachedCameraData();
        //private DateTime _timeSincePositionChange = DateTime.MinValue;



        private void SmoothedCameraMove()
        {
            var model = ViewPortModel.Instance;
            _currentPosition = model.PlayerPosition;
            if (_currentPosition == Vector3.Zero)
                return;

            var delta = _currentPosition - _lastPosition;
            var magnitude = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y + delta.Z * delta.Z);


            //var unitDelta = new Vector3(delta.X / (float)magnitude, delta.Y, delta.Z / (float)magnitude);


            var timeDelta = _movementTimer.Elapsed.TotalMilliseconds;
            var MoveFrameInterval = Math.Min((float)_lastTimeDelta, 250f);

            var pctTime = timeDelta / MoveFrameInterval;

            if (!_movementTimer.IsRunning)
            {
                _movementTimer.Start();
            }
            else
            {
                //var dir = Vector3.NormalizedDirection(position, _lastPosition);
                //var stopAnim = false;

                //var unitTime = MoveFrameInterval / (float)timeDelta;

                //if (magnitude > 0 && Math.Abs(magnitude - _lastMagnitude) > magnitude * 0.2) // || _lastPosition == position && Math.Abs(magnitude - _lastMagnitude) > double.Epsilon)
                //{
                //    Logger.Log("Accel/Decel Detected");
                //    _viewport.Camera.ApplyAnimationClock(ProjectionCamera.PositionProperty, null);
                //    _isAnimating = false;
                //    stopAnim = true;
                //    ((PerspectiveCamera)_viewport.Camera).Position = GetCamPosition(position, model);
                //}

                if (_lastPosition != _currentPosition)
                {
                    _lastTimeDelta = _movementTimer.Elapsed.Milliseconds;
                    _lastMoveDelta = delta;
                    _movementTimer.Restart();


                    CachedCameraData cam = new CachedCameraData();
                    using (ZetaDia.Memory.AcquireFrame())
                    {
                        cam = new CachedCameraData(ZetaDia.Camera.Cameras[0]);
                    }
                    Vector3 position = cam.Position;
                    float fov = cam.FieldOfView;

                    var camDelta = cam.Position - _lastCamData.Position;
                    var camMagnitude = Math.Sqrt(camDelta.X * camDelta.X + camDelta.Y * camDelta.Y + camDelta.Z * camDelta.Z);

                    CameraMove();

                    ///*
                    //    Float8: 0 => -0.3513986 
                    //    FloatC: 0 => 0.8483514                     
                    //    Float10: 0 => -0.1515441                     
                    //    Float14: 0 => -0.3658597                     
                    //    Float18: 0 => 440.08                     
                    //    Position: <0, 0, 0> => <475.08, 66.18381, 0>                     
                    //    Dword2C: 0 => 1065353216                     
                    //    float30: 0 => 1                     
                    //    float34: 0 => 1                     
                    //    float38: 0 => 1                    
                    //    Dword44: 0 => 1 
                    //*/

                    //var test = new Quaternion(cam.Float8, cam.Float10, cam.Float14, cam.Float18);
                    //var r = new QuaternionRotation3D(test);
                    //var transform = new Transform3DGroup();
                    //transform.Children.Add(new RotateTransform3D(r));
                    //_camera.Transform = transform;

                    ////_camera.Transform = r;

                    //Logger.Log("Cam Changes: {0}", _lastCamData.DetailedCompare(cam));

                    ////float170 = Magnitude

                    //_lastCamData = cam;
                    //_camera.FieldOfView = fov;
                    //_camera.Position = position.ToPoint3D();


                    //TotalDx += dx; TotalDy += dy;

                    //double theta = TotalDx / 3;
                    //double phi = TotalDy / 3;
                    //Vector3D thetaAxis = new Vector3D(0, 1, 0);
                    //Vector3D phiAxis = new Vector3D(-1, 0, 0);

                    //Transform3DGroup group = mGeometry.Transform as Transform3DGroup;
                    //group.Children.Clear();
                    //QuaternionRotation3D r;
                    //r = new QuaternionRotation3D(new Quaternion(thetaAxis, theta));
                    //group.Children.Add(new RotateTransform3D(r));
                    //r = new QuaternionRotation3D(new Quaternion(phiAxis, phi));
                    //group.Children.Add(new RotateTransform3D(r));


                    //Logger.Log("CamData={0}", cam);

                    ////var unitDelta = new Vector3(delta.X*unitTime, delta.Y*unitTime, delta.Z*unitTime);

                    //_projectedPosition = _currentPosition + delta;
                    ////var newCameraPos = GetCamPosition(_projectedPosition, model);

                    //CameraMove();

                    Logger.Log("Position Changed PosDelta={0} PosMagnitude={1} CamDelta={2} CamMag={3}",
                        delta, delta.Magnitude, camDelta, camDelta.Magnitude);

                    ////if (!stopAnim)
                    ////{

                    //var newCameraPos = GetCamPosition(projectedPosition, model);
                    //////var cam = (PerspectiveCamera)Camera;
                    ////if (_cameraPositionAnimation != null)
                    ////{
                    ////    _cameraPositionAnimation.Completed -= SetNotAnimating;
                    ////}

                    //_cameraPositionAnimation = new Point3DAnimation(newCameraPos, TimeSpan.FromMilliseconds(MoveFrameInterval));
                    //var ease = new CircleEase();
                    //_cameraPositionAnimation.EasingFunction = ease;
                    //ease.EasingMode = EasingMode.EaseIn;
                    ////_cameraPositionAnimation.EasingFunction = ease;
                    ////_cameraPositionAnimation.Completed += SetNotAnimating;
                    //_isAnimating = true;
                    //_viewport.Camera.ApplyAnimationClock(ProjectionCamera.PositionProperty, _cameraPositionAnimation.CreateClock(), HandoffBehavior.SnapshotAndReplace);
                    //_lastSmoothMoveStartPosition = position;
                    //_lastSmoothMoveTargetPosition = projectedPosition;
                    //_lastSmoothMoved = DateTime.UtcNow;
                    //_movementTimer.Restart();

                    //}
                }

                //else
                //{
                //    _cameraPositionAnimation.
                //}
            }


            //var ease = new CubicEase();
            //ease.EasingMode = EasingMode.EaseInOut;



            //Vector3 projectedPosition;
            //if (position == _lastPosition || timeDelta == 0)
            //{
            //    Logger.Log("Position Unchanged");
            //    //projectedPosition = position;                
            //    //_lastSmoothMoved = DateTime.UtcNow;
            //}
            //else
            //{
            //    if (magnitude > 0 && magnitude < 1)
            //    {
            //        // Stop Animation.
            //        Logger.Log("Accel/Decel Detected");
            //        _viewport.Camera.ApplyAnimationClock(ProjectionCamera.PositionProperty, null);
            //        ((PerspectiveCamera) _viewport.Camera).Position = GetCamPosition(position, model);
            //    }
            //    else
            //    {
            //        var projectedPosition = position.LerpAddition(dir, pctTime);

            //        //_viewport.Camera.ApplyAnimationClock(ProjectionCamera.PositionProperty, null);
            //        //_isAnimating = false;
            //        //CameraMove();

            //        if (!_isAnimating)
            //        {
            //            var newCameraPos = GetCamPosition(projectedPosition, model);
            //            //var cam = (PerspectiveCamera)Camera;
            //            if (_cameraPositionAnimation != null)
            //            {
            //                _cameraPositionAnimation.Completed -= SetNotAnimating;
            //            }
            //            _cameraPositionAnimation = new Point3DAnimation(newCameraPos, TimeSpan.FromMilliseconds(MoveFrameInterval));
            //            //_cameraPositionAnimation.EasingFunction = ease;
            //            _cameraPositionAnimation.Completed += SetNotAnimating;
            //            _isAnimating = true;
            //            _viewport.Camera.ApplyAnimationClock(ProjectionCamera.PositionProperty, _cameraPositionAnimation.CreateClock());
            //            _lastSmoothMoveStartPosition = position;
            //            _lastSmoothMoveTargetPosition = projectedPosition;
            //            _lastSmoothMoved = DateTime.UtcNow;
            //            _movementTimer.Restart();
            //        }
            //    }
            //}

            //position + new Vector3(dir.X * pctTime, dir.Y * pctTime, dir.Z * pctTime);

            //if (timeDelta < 15)
            //{
            //    CameraMove();
            //}
            //else
            //{

            //}

            //Logger.Log("SinceLastRender={0} LastPosTime={1:0.000} TimeAnim={2} RawPos={3} PctTime={4} IsAnim={5} Magnitude={6} LTime▲={7}",
            //    DateTime.UtcNow.Subtract(_lastFrameTime).TotalMilliseconds,
            //    _movementTimer.Elapsed.TotalMilliseconds,
            //    DateTime.UtcNow.Subtract(_lastSmoothMoved).TotalMilliseconds,
            //    _currentPosition, pctTime, _isAnimating, delta.Magnitude, _lastTimeDelta);

            _lastMagnitude = magnitude;
            //_lastTimeDelta = timeDelta;
            _lastPositionDelta = delta;
            _lastPosition = _currentPosition;
            _lastFrameTime = DateTime.UtcNow;
        }

        public void CameraTranslate(Vector3 position)
        {
            PerspectiveCamera camera = (PerspectiveCamera)_viewport.Camera;

            //var newCameraPos = GetCamPosition(projectedPosition, model);

            //if (d == null)
            //{
            //    // Stops the animation

            //    // testing _cameraTranslating is not reliable
            //    // if (_cameraTranslating && camera.HasAnimatedProperties)

            //    if (camera.HasAnimatedProperties)
            //    {
            //        Point3D currentPosition = camera.Position;
            //        camera.ApplyAnimationClock(PerspectiveCamera.PositionProperty, null);
            //        camera.Position = currentPosition;
            //        // _cameraTranslating = false;
            //    }
            //}
            //else
            //{
            Vector3D v = camera.LookDirection;
            Vector3D vY = new Vector3D(0.0, 1.0, 0.0);
            Vector3D vRelativeHorizontalAxis = Vector3D.CrossProduct(v, vY);
            Vector3D vRelativeVerticalAxis = Vector3D.CrossProduct(v, vRelativeHorizontalAxis);

            AxisAngleRotation3D rotation = new AxisAngleRotation3D();
            rotation.Axis = vRelativeVerticalAxis;
            rotation.Angle = 135.0;
            RotateTransform3D transform = new RotateTransform3D(rotation);
            v = transform.Transform(camera.LookDirection);

            //if (d != Direction.Forward)
            //{
            //    AxisAngleRotation3D rotation = new AxisAngleRotation3D();
            //    switch (d)
            //    {
            //        case Direction.Forward | Direction.Left:
            //            rotation.Axis = vRelativeVerticalAxis;
            //            rotation.Angle = -45.0;
            //            break;
            //        case Direction.Left:
            //            rotation.Axis = vRelativeVerticalAxis;
            //            rotation.Angle = -90.0;
            //            break;
            //        case Direction.Backward | Direction.Left:
            //            rotation.Axis = vRelativeVerticalAxis;
            //            rotation.Angle = -135.0;
            //            break;
            //        case Direction.Backward:
            //            rotation.Axis = vRelativeVerticalAxis;
            //            rotation.Angle = 180.0;
            //            break;
            //        case Direction.Backward | Direction.Right:
            //            rotation.Axis = vRelativeVerticalAxis;
            //            rotation.Angle = 135.0;
            //            break;
            //        case Direction.Right:
            //            rotation.Axis = vRelativeVerticalAxis;
            //            rotation.Angle = 90.0;
            //            break;
            //        case Direction.Forward | Direction.Right:
            //            rotation.Axis = vRelativeVerticalAxis;
            //            rotation.Angle = 45.0;
            //            break;
            //        case Direction.Up:
            //            rotation.Axis = vRelativeHorizontalAxis;
            //            rotation.Angle = 90.0;
            //            break;
            //        case Direction.Down:
            //            rotation.Axis = vRelativeHorizontalAxis;
            //            rotation.Angle = -90.0;
            //            break;
            //    }
            //    RotateTransform3D transform = new RotateTransform3D(rotation);
            //    v = transform.Transform(camera.LookDirection);
            //}

            Point3D p = new Point3D(v.X, v.Y, v.Z);
            _cameraPositionAnimation.By = p;
            //_cameraTranslating = true;
            camera.ApplyAnimationClock(PerspectiveCamera.PositionProperty, _cameraPositionAnimation.CreateClock());
        }


        #endregion
    }

    internal class CachedCameraData
    {
        public CachedCameraData()
        {

        }

        public CachedCameraData(CameraData data)
        {
            CopyFrom(data);
        }

        private void CopyFrom<T>(T data)
        {
            if (data == null)
                return;

            var type = data.GetType();
            var thisType = GetType();

            foreach (var f in type.GetFields())
            {
                var field = thisType.GetField(f.Name);
                field?.SetValue(this, f.GetValue(data));
            }

            foreach (var p in type.GetProperties())
            {
                var prop = thisType.GetProperty(p.Name);
                prop?.SetValue(this, p.GetValue(data));
            }
        }

        public int ObserverSnoId { get; set; }

        public int Index { get; set; }

        public float Float8 { get; set; }

        public float FloatC { get; set; }

        public float Float10 { get; set; }

        public float Float14 { get; set; }

        public float Float18 { get; set; }

        public Vector3 Position { get; set; }

        public int Dword28 { get; set; }

        public int Dword2C { get; set; }

        public float float30 { get; set; }

        public float float34 { get; set; }

        public float float38 { get; set; }

        public float float3C { get; set; }

        public int Dword40 { get; set; }

        public int Dword44 { get; set; }

        public int Dword48 { get; set; }

        public float FieldOfView { get; set; }

        public float float50 { get; set; }

        public float float54 { get; set; }

        public float float58 { get; set; }

        public float float5C { get; set; }

        public float float60 { get; set; }

        public float float64 { get; set; }

        public float float68 { get; set; }

        public int Dword6C { get; set; }

        public int Dword70 { get; set; }

        public int Dword74 { get; set; }

        public int Dword78 { get; set; }

        public float float7C { get; set; }

        public float float80 { get; set; }

        public float float84 { get; set; }

        public float float88 { get; set; }

        public float float8C { get; set; }

        public float float90 { get; set; }

        public float float94 { get; set; }

        public float float98 { get; set; }

        public float float9C { get; set; }

        public float floatA0 { get; set; }

        public float floatA4 { get; set; }

        public float floatA8 { get; set; }

        public float floatAC { get; set; }

        public float floatB0 { get; set; }

        public float floatB4 { get; set; }

        public float floatB8 { get; set; }

        public float floatBC { get; set; }

        public float floatC0 { get; set; }

        public float floatC4 { get; set; }

        public float floatC8 { get; set; }

        public float floatCC { get; set; }

        public float floatD0 { get; set; }

        public float floatD4 { get; set; }

        public float floatD8 { get; set; }

        public float floatDC { get; set; }

        public float floatE0 { get; set; }

        public float floatE4 { get; set; }

        public float floatE8 { get; set; }

        public float floatEC { get; set; }

        public float floatF0 { get; set; }

        public float floatF4 { get; set; }

        public float floatF8 { get; set; }

        public float floatFC { get; set; }

        public float float100 { get; set; }

        public float float104 { get; set; }

        public float float108 { get; set; }

        public float float10C { get; set; }

        public float float110 { get; set; }

        public float float114 { get; set; }

        public float float118 { get; set; }

        public float float11C { get; set; }

        public float float120 { get; set; }

        public float float124 { get; set; }

        public float float128 { get; set; }

        public float float12C { get; set; }

        public float float130 { get; set; }

        public float float134 { get; set; }

        public float float138 { get; set; }

        public float float13C { get; set; }

        public float float140 { get; set; }

        public float float144 { get; set; }

        public float float148 { get; set; }

        public float float14C { get; set; }

        public float float150 { get; set; }

        public float float154 { get; set; }

        public int Dword158 { get; set; }

        public float float15C { get; set; }

        public float float160 { get; set; }

        public float float164 { get; set; }

        public int Dword168 { get; set; }

        public float float16C { get; set; }

        public float float170 { get; set; }

        public float float174 { get; set; }

        public int Dword178 { get; set; }

        public float float17C { get; set; }

        public float float180 { get; set; }

        public float float184 { get; set; }

        public float float188 { get; set; }

        public float float18C { get; set; }

        public int Dword190 { get; set; }

        public int Dword194 { get; set; }

        public int Dword198 { get; set; }

        public int Dword19C { get; set; }

        public float float1A0 { get; set; }

        public int Dword1A4 { get; set; }

        public int Dword1A8 { get; set; }

        public int Dword1AC { get; set; }

        public int Dword1B0 { get; set; }

        public float float1B4 { get; set; }

        public float float1B8 { get; set; }

        public int Dword1BC { get; set; }

        public int Dword1C0 { get; set; }

        public float float1C4 { get; set; }

        public int Dword1C8 { get; set; }

        public float float1CC { get; set; }

        public int Dword1D0 { get; set; }

        public int Dword1D4 { get; set; }

        public int Dword1D8 { get; set; }

        public int Dword1DC { get; set; }

        public float float1E0 { get; set; }

        public int Dword1E4 { get; set; }

        public int Dword1E8 { get; set; }

        public int Dword1EC { get; set; }

        public int Dword1F0 { get; set; }

        public float float1F4 { get; set; }

        public float float1F8 { get; set; }

        public int Dword1FC { get; set; }

        public int Dword200 { get; set; }

        public float float204 { get; set; }

        public int Dword208 { get; set; }

        public float float20C { get; set; }

        public float float210 { get; set; }

        public float float214 { get; set; }

        public float float218 { get; set; }

        public float float21C { get; set; }

        public float float220 { get; set; }

        public float float224 { get; set; }

        public int Dword228 { get; set; }

        public int Dword22C { get; set; }

        public int Dword230 { get; set; }

        public override string ToString()
        {
            return string.Format("ObserverSnoId: {0}, Index: {1}, Float8: {2}, FloatC: {3}, Float10: {4}, Float14: {5}, Float18: {6}, Position: {7}, Dword28: {8}, Dword2C: {9}, float30: {10}, float34: {11}, float38: {12}, float3C: {13}, Dword40: {14}, Dword44: {15}, Dword48: {16}, FieldOfView: {17}, float50: {18}, float54: {19}, float58: {20}, float5C: {21}, float60: {22}, float64: {23}, float68: {24}, Dword6C: {25}, Dword70: {26}, Dword74: {27}, Dword78: {28}, float7C: {29}, float80: {30}, float84: {31}, float88: {32}, float8C: {33}, float90: {34}, float94: {35}, float98: {36}, float9C: {37}, floatA0: {38}, floatA4: {39}, floatA8: {40}, floatAC: {41}, floatB0: {42}, floatB4: {43}, floatB8: {44}, floatBC: {45}, floatC0: {46}, floatC4: {47}, floatC8: {48}, floatCC: {49}, floatD0: {50}, floatD4: {51}, floatD8: {52}, floatDC: {53}, floatE0: {54}, floatE4: {55}, floatE8: {56}, floatEC: {57}, floatF0: {58}, floatF4: {59}, floatF8: {60}, floatFC: {61}, float100: {62}, float104: {63}, float108: {64}, float10C: {65}, float110: {66}, float114: {67}, float118: {68}, float11C: {69}, float120: {70}, float124: {71}, float128: {72}, float12C: {73}, float130: {74}, float134: {75}, float138: {76}, float13C: {77}, float140: {78}, float144: {79}, float148: {80}, float14C: {81}, float150: {82}, float154: {83}, Dword158: {84}, float15C: {85}, float160: {86}, float164: {87}, Dword168: {88}, float16C: {89}, float170: {90}, float174: {91}, Dword178: {92}, float17C: {93}, float180: {94}, float184: {95}, float188: {96}, float18C: {97}, Dword190: {98}, Dword194: {99}, Dword198: {100}, Dword19C: {101}, float1A0: {102}, Dword1A4: {103}, Dword1A8: {104}, Dword1AC: {105}, Dword1B0: {106}, float1B4: {107}, float1B8: {108}, Dword1BC: {109}, Dword1C0: {110}, float1C4: {111}, Dword1C8: {112}, float1CC: {113}, Dword1D0: {114}, Dword1D4: {115}, Dword1D8: {116}, Dword1DC: {117}, float1E0: {118}, Dword1E4: {119}, Dword1E8: {120}, Dword1EC: {121}, Dword1F0: {122}, float1F4: {123}, float1F8: {124}, Dword1FC: {125}, Dword200: {126}, float204: {127}, Dword208: {128}, float20C: {129}, float210: {130}, float214: {131}, float218: {132}, float21C: {133}, float220: {134}, float224: {135}, Dword228: {136}, Dword22C: {137}, Dword230: {138}", new object[]
            {
                this.ObserverSnoId,
                this.Index,
                this.Float8,
                this.FloatC,
                this.Float10,
                this.Float14,
                this.Float18,
                this.Position,
                this.Dword28,
                this.Dword2C,
                this.float30,
                this.float34,
                this.float38,
                this.float3C,
                this.Dword40,
                this.Dword44,
                this.Dword48,
                this.FieldOfView,
                this.float50,
                this.float54,
                this.float58,
                this.float5C,
                this.float60,
                this.float64,
                this.float68,
                this.Dword6C,
                this.Dword70,
                this.Dword74,
                this.Dword78,
                this.float7C,
                this.float80,
                this.float84,
                this.float88,
                this.float8C,
                this.float90,
                this.float94,
                this.float98,
                this.float9C,
                this.floatA0,
                this.floatA4,
                this.floatA8,
                this.floatAC,
                this.floatB0,
                this.floatB4,
                this.floatB8,
                this.floatBC,
                this.floatC0,
                this.floatC4,
                this.floatC8,
                this.floatCC,
                this.floatD0,
                this.floatD4,
                this.floatD8,
                this.floatDC,
                this.floatE0,
                this.floatE4,
                this.floatE8,
                this.floatEC,
                this.floatF0,
                this.floatF4,
                this.floatF8,
                this.floatFC,
                this.float100,
                this.float104,
                this.float108,
                this.float10C,
                this.float110,
                this.float114,
                this.float118,
                this.float11C,
                this.float120,
                this.float124,
                this.float128,
                this.float12C,
                this.float130,
                this.float134,
                this.float138,
                this.float13C,
                this.float140,
                this.float144,
                this.float148,
                this.float14C,
                this.float150,
                this.float154,
                this.Dword158,
                this.float15C,
                this.float160,
                this.float164,
                this.Dword168,
                this.float16C,
                this.float170,
                this.float174,
                this.Dword178,
                this.float17C,
                this.float180,
                this.float184,
                this.float188,
                this.float18C,
                this.Dword190,
                this.Dword194,
                this.Dword198,
                this.Dword19C,
                this.float1A0,
                this.Dword1A4,
                this.Dword1A8,
                this.Dword1AC,
                this.Dword1B0,
                this.float1B4,
                this.float1B8,
                this.Dword1BC,
                this.Dword1C0,
                this.float1C4,
                this.Dword1C8,
                this.float1CC,
                this.Dword1D0,
                this.Dword1D4,
                this.Dword1D8,
                this.Dword1DC,
                this.float1E0,
                this.Dword1E4,
                this.Dword1E8,
                this.Dword1EC,
                this.Dword1F0,
                this.float1F4,
                this.float1F8,
                this.Dword1FC,
                this.Dword200,
                this.float204,
                this.Dword208,
                this.float20C,
                this.float210,
                this.float214,
                this.float218,
                this.float21C,
                this.float220,
                this.float224,
                this.Dword228,
                this.Dword22C,
                this.Dword230
            });
        }
    }

}