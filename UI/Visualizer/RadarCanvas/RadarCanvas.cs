using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.UI;
using Trinity.Components.Combat;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Grid;
using Trinity.Framework.Objects;
using Trinity.Framework.Objects.Enums;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;
using CombineType = Trinity.Components.Adventurer.Game.Exploration.SceneMapping.CombineType;
using Cursors = System.Windows.Input.Cursors;
using DeathGates = Trinity.Components.Adventurer.Game.Exploration.SceneMapping.DeathGates;
using FlowDirection = System.Windows.FlowDirection;
using LineSegment = System.Windows.Media.LineSegment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using RectangularRegion = Trinity.Components.Adventurer.Game.Exploration.SceneMapping.RectangularRegion;


namespace Trinity.UI.Visualizer.RadarCanvas
{
    public class RadarCanvas : Canvas
    {
        public RadarCanvas()
        {
            ClipToBounds = true;

            CanvasData.OnCanvasSizeChanged += (before, after) =>
            {
                // Scene drawings are specific to a canvas size.
                // Changing canvas size means we have to redraw them all.
                Clear();
            };

            MouseWheel += (sender, args) => Zoom = args.Delta < 0 ? Zoom - 1 : Zoom + 1;
            MouseDown += MouseDownHandler;
            MouseMove += MouseMoveHandler;
            MouseUp += MouseUpHandler;

            Background = new SolidColorBrush(Color.FromArgb(0, 50, 50, 50));

            GameEvents.OnWorldChanged += (s, e) =>
            {
                Drawings.Relative.Clear();
            };
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            IsRightClick = e.RightButton == MouseButtonState.Pressed;
            IsLeftClick = e.LeftButton == MouseButtonState.Pressed;
            HasDragged = false;
            Cursor = Cursors.Hand;
            DragInitialPosition = Mouse.GetPosition(this);
            DragInitialPanOffset = CanvasData.PanOffset;

            //HandleMapClicked(sender, e);
        }

        public bool HasDragged { get; set; }
        public bool IsLeftClick { get; set; }
        public bool IsRightClick { get; set; }

        public Tuple<Vector3, Vector3, bool> CurrentRayCast { get; set; }
        public Tuple<Vector3, Vector3, bool> CurrentRayWalk { get; set; }
        public Tuple<Vector3, Vector3, bool> CurrentTestLine { get; set; }


        private void HandleMapClicked(object sender, MouseButtonEventArgs e)
        {
            var startWorldPosition = PointMorph.GetWorldPosition(DragInitialPosition, CanvasData);
            var clickedPosition = Mouse.GetPosition(this);
            var endWorldPosition = PointMorph.GetWorldPosition(clickedPosition, CanvasData);

            if (!IsDragging)
            {
                if (IsRightClick)
                {
                    var size = endWorldPosition - startWorldPosition;
                    var scene = Core.Scenes.FirstOrDefault(s => s.IsInScene(startWorldPosition));

                    Core.Logger.Log($"Right Mouse Drag from {startWorldPosition} to {endWorldPosition} in {scene?.Name} Size={Math.Abs(size.X)}*{Math.Abs(size.Y)}");
                    DeveloperUI.LogSceneInfoPosition(scene, startWorldPosition, endWorldPosition);

                }
                else if (IsLeftClick)
                {
                    bool isExplored = ZetaDia.Minimap.IsExplored(startWorldPosition, ZetaDia.Globals.WorldId);
                    Core.Logger.Log($"Clicked World Position = {startWorldPosition}, Distance={startWorldPosition.Distance(ZetaDia.Me.Position)}, IsExplored: {isExplored}");


                    var result = Core.Grids.Avoidance.CanRayWalk(startWorldPosition, Player.Actor.Position);
                    CurrentRayWalk = new Tuple<Vector3, Vector3, bool>(startWorldPosition, Player.Actor.Position, result);

                    //var isConnectedScene = Core.Scenes.CurrentScene.IsConnected(startWorldPosition);

                    SceneConnectionPath?.Clear();
                    var endScene = Core.Scenes.GetScene(startWorldPosition);
                    SceneConnectionPath = Core.Scenes.CurrentScene
                        .GetConnectedScenes(endScene)
                        .Select(s => s.ExitPosition).ToList();
                    if (SceneConnectionPath.Any())
                    {
                        SceneConnectionPath.Add(startWorldPosition);
                    }

                    //var result = Core.Grids.Avoidance.CanRayCast(clickedWorldPosition, Player.Actor.Position);
                    //CurrentRayCast = new Tuple<Vector3, Vector3, bool>(clickedWorldPosition, Player.Actor.Position, result);

                    //var gatePosition = DeathGates.GetBestGatePosition(clickedWorldPosition);
                    //CurrentTestLine = new Tuple<Vector3, Vector3, bool>(gatePosition, Player.Actor.Position, result);

                    //var sequence = DeathGates.CreateSequence();
                    //for (int i = 0; i < sequence.Count; i++)
                    //{
                    //    var scene = sequence[i];
                    //    Core.Logger.Log($"{i}: {(sequence.Index == i ? ">> " : "")}{scene.Name}");
                    //}

                    var hit = FindElementUnderClick(sender, e);
                    if (hit != null)
                    {
                        var actor = hit.RadarObject.Actor;
                        ClickCommand.Execute(actor);
                        InvalidateVisual();
                        return;
                    }
                }

                var node = Core.Avoidance.Grid.GetNearestNode(startWorldPosition);
                SelectedItem = new TrinityActor
                {
                    InternalName = $"Node[{node.GridPoint.X},{node.GridPoint.Y}] World[{(int)startWorldPosition.X},{(int)startWorldPosition.Y}]",
                    Distance = startWorldPosition.Distance(ZetaDia.Me.Position),
                    Position = startWorldPosition,
                };
            }

            // Trigger Canvas to Render
            InvalidateVisual();
        }

        public List<Vector3> SceneConnectionPath { get; set; }


        private RadarHitTestUtility.HitContainer FindElementUnderClick(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition((UIElement)sender);
            return HitTester.GetHit(position);
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                IsDragging = true;
                HasDragged = true;
                const double dragSpeed = 0.5;
                var position = e.GetPosition(this);
                var xOffset = position.X - DragInitialPosition.X;
                var yOffset = position.Y - DragInitialPosition.Y;

                DragOffset = new Point((int)(xOffset * dragSpeed) + DragInitialPanOffset.X, (int)(yOffset * dragSpeed) + DragInitialPanOffset.Y);

                if (IsLeftClick)
                {
                    CanvasData.PanOffset = DragOffset;
                }
            }
            else
            {
                IsDragging = false;
            }

            //if (DateTime.UtcNow.Subtract(LastMoveCursorCheck).TotalMilliseconds > 25)
            //{
            //    LastMoveCursorCheck = DateTime.UtcNow;
            //    if (FindElementUnderClick(sender, e) != null)
            //    {
            //        Mouse.OverrideCursor = Cursors.Hand;
            //    }
            //    else
            //    {
            //        Mouse.OverrideCursor = null;
            //    }
            //}
        }

        //public bool IsDragging { get; set; }

        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;

            if (IsLeftClick)
            {
                CanvasData.PanOffset = DragOffset;
            }
            Cursor = Cursors.Arrow;

            using (ZetaDia.Memory.AcquireFrame())
            {
                HandleMapClicked(sender, e);
            }
        }

        private void Clear()
        {
            Drawings.Relative.Clear();
            Clip = CanvasData.ClipRegion;
            CanvasData.PanOffset = new Point();
        }

        public RadarHitTestUtility HitTester = new RadarHitTestUtility();

        public bool IsMouseDown { get; set; }

        public Point DragOffset { get; set; }

        public Point DragInitialPosition { get; set; }

        public Point DragInitialPanOffset { get; set; }

        public const int DefaultGridSize = 5;

        /// <summary>
        /// The canvas size of grid squares (in pixels) for 1yd of game distance
        /// </summary>        
        public int GridSize = DefaultGridSize;

        public float Scale
        {
            get { return GridSize / (float)DefaultGridSize; }
        }

        /// <summary>
        /// The number of grid squares/yards to draw horizontal/vertical lines on
        /// </summary>
        public int GridLineFrequency = 10;

        /// <summary>
        /// How many compiled scene drawings to keep around
        /// </summary>
        public int RelativeGeometryStorageLimit = 100;

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
                typeof(RadarCanvas),
                new PropertyMetadata(null, OnItemsSourceChanged));

        //private System.Windows.Threading.DispatcherTimer dispatcherTimer;

        public IList ItemsSource
        {
            set { SetValue(ItemsSourceProperty, value); }
            get { return (IList)GetValue(ItemsSourceProperty); }
        }

        static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var radarCanvas = obj as RadarCanvas;
            if (radarCanvas != null)
                radarCanvas.OnItemsSourceChanged(args);
        }

        #endregion

        #region Click Command Property

        public static DependencyProperty ClickCommandProperty
            = DependencyProperty.Register(
                "ClickCommand",
                typeof(ICommand),
                typeof(RadarCanvas));

        public ICommand ClickCommand
        {
            get { return (ICommand)GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }

        #endregion

        #region Zoom Property

        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom",
                typeof(int),
                typeof(RadarCanvas),
                new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnZoomChanged));

        public int Zoom
        {
            set { SetValue(ZoomProperty, value); }
            get { return (int)GetValue(ZoomProperty); }
        }

        static void OnZoomChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Core.Logger.Verbose("[RadarUI] Zoom changed from {0} to {1}", args.OldValue, args.NewValue);
            var radarCanvas = obj as RadarCanvas;
            if (radarCanvas != null)
            {
                radarCanvas.GridSize = (int)args.NewValue;
                radarCanvas.Clear();
            }
        }

        #endregion

        #region IsDragging Property

        public static readonly DependencyProperty IsDraggingProperty =
            DependencyProperty.Register("IsDragging",
                typeof(bool),
                typeof(RadarCanvas),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, OnIsDraggingChanged));

        private static void OnIsDraggingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Core.Logger.Verbose("[RadarUI] Dragging Changed from {0} to {1}", e.OldValue, e.NewValue);
            var radarCanvas = d as RadarCanvas;
            if (radarCanvas != null)
            {
                // Update Pan Offset
            }
        }

        public bool IsDragging
        {
            set { SetValue(IsDraggingProperty, value); }
            get { return (bool)GetValue(IsDraggingProperty); }
        }

        #endregion

        #region Pan Property

        public static readonly DependencyProperty PanProperty =
            DependencyProperty.Register("Pan",
                typeof(int),
                typeof(RadarCanvas),
                new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPanChanged));

        public int Pan
        {
            set { SetValue(PanProperty, value); }
            get { return (int)GetValue(PanProperty); }
        }

        static void OnPanChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Core.Logger.Verbose("[RadarUI] Pan changed from {0} to {1}", args.OldValue, args.NewValue);
            var radarCanvas = obj as RadarCanvas;
            if (radarCanvas != null)
            {
                // Update Pan Offset
            }
        }

        #endregion

        #region Pan Property

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem",
                typeof(TrinityActor),
                typeof(RadarCanvas),
                new FrameworkPropertyMetadata(default(TrinityActor), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        public TrinityActor SelectedItem
        {
            get { return (TrinityActor)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Core.Logger.Log("SelectedItem changed from {0} to {1}", args.OldValue, args.NewValue);

            //var radarCanvas = obj as RadarCanvas;
            //if (radarCanvas != null)
            //{
            //    radarCanvas.SelectedItem
            //}
        }

        #endregion

        #region VisibilityFlags Property

        public static readonly DependencyProperty VisibilityFlagsProperty =
            DependencyProperty.Register("VisibilityFlags",
                typeof(RadarVisibilityFlags),
                typeof(RadarCanvas),
                new FrameworkPropertyMetadata(RadarVisibilityFlags.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnVisibilityFlagsChanged));

        public RadarVisibilityFlags VisibilityFlags
        {
            set { SetValue(VisibilityFlagsProperty, value); }
            get { return (RadarVisibilityFlags)GetValue(VisibilityFlagsProperty); }
        }

        static void OnVisibilityFlagsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Core.Logger.Verbose("[RadarUI] VisibilityFlags changed from {0} to {1}", args.OldValue, args.NewValue);
            var radarCanvas = obj as RadarCanvas;
            if (radarCanvas != null)
            {
                // Update Visibility Settings
            }
        }

        #endregion

        #region ItemSource Changed Event Handling

        /// <summary>
        /// ItemSource binding on control is set
        /// </summary>
        void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        {
            UpdateData();

            //var oldValue = args.OldValue as INotifyCollectionChanged;
            //if (oldValue != null)
            //    oldValue.CollectionChanged -= OnItemsSourceCollectionChanged;

            //var newValue = args.NewValue as INotifyCollectionChanged;
            //if (newValue != null)
            //    newValue.CollectionChanged += OnItemsSourceCollectionChanged;
        }

        /// <summary>
        /// When objects inside ItemSource collection change
        /// </summary>
        void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            // UpdateData();
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

        private DateTime LastUpdate = DateTime.MinValue;

        /// <summary>
        /// Go through the ItemSource collection and calculate their canvas positions
        /// </summary>
        void UpdateData()
        {
            try
            {

                if (DesiredSize.Height <= 0 || DesiredSize.Width <= 0)
                    return;

                if (!IsVisible || Core.Player.Actor == null || Core.Player.IsLoadingWorld || !Core.Player.IsInGame)
                    return;

                Objects.Clear();
                HitTester.Clear();

                CanvasData.Update(DesiredSize, GridSize);

                // Find the actor who should be in the center of the radar
                // and whos position all other points should be plotted against.

                var center = ItemsSource.OfType<TrinityActor>().FirstOrDefault(u => u.IsMe);
                if (center == null)
                    return;

                CenterActor = new RadarObject(center, CanvasData);
                CanvasData.CenterVector = CenterActor.Actor.Position;
                CanvasData.CenterMorph = CenterActor.Morph;

                // Calculate locations for all actors positions
                // on RadarObject ctor; or with .Update();

                var updatedSelection = false;

                foreach (var trinityObject in ItemsSource.OfType<TrinityActor>())
                {
                    var radarObject = new RadarObject(trinityObject, CanvasData);

                    if (!updatedSelection && SelectedItem != null && trinityObject.AcdId == SelectedItem.AcdId)
                    {
                        updatedSelection = true;
                        SelectedRadarObject = radarObject;
                    }

                    Objects.Add(radarObject);

                    if (radarObject.Actor.IsMe)
                        Player = radarObject;
                }

                UpdateExplorationData();

                UpdateRelativeDrawings();


                // Trigger Canvas to Render
                InvalidateVisual();

            }
            catch (Exception ex)
            {
                Core.Logger.Log("Exception in RadarUI.UpdateData(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private void UpdateExplorationData()
        {
            _explorationNodes = ExplorationGrid.Instance.WalkableNodes;
        }

        /// <summary>
        /// Using CenterActor was causing jerkyness, but using an actor for player offset from center actor,
        /// which is then in sync with all other actors smoothed it out.
        /// </summary>
        public RadarObject Player { get; set; }

        public RadarObject SelectedRadarObject { get; set; }


        /// <summary>
        /// Update relative drawings. Calculates new position for each scene drawing based on its origin point.
        /// </summary>
        private void UpdateRelativeDrawings()
        {
            var keysToRemove = new List<string>();
            foreach (var drawing in Drawings.Relative)
            {
                if (drawing.Value.WorldId != Core.Player.WorldDynamicId)
                {
                    keysToRemove.Add(drawing.Key);
                    continue;
                }

                DrawingUtilities.RelativeMove(drawing.Value.Drawing, drawing.Value.Origin.WorldVector);
            }

            RelativeDrawing removedItem;

            foreach (var key in keysToRemove)
            {
                Drawings.Relative.TryRemove(key, out removedItem);
            }

            if (Drawings.Relative.Count > RelativeGeometryStorageLimit)
            {
                var firstKey = Drawings.Relative.Keys.First();
                Drawings.Relative.TryRemove(firstKey, out removedItem);
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
                //DrawBackground(dc, CanvasData);

                if (CanvasData.CanvasSize.Width == 0 && CanvasData.CanvasSize.Height == 0 || CanvasData.CenterVector == Vector3.Zero)
                    return;

                if (CenterActor.Point.X == 0 && CenterActor.Point.Y == 0)
                    return;

                if (!Core.Player.IsInGame || Core.Player.IsLoadingWorld)
                    return;

                try
                {
                    // Background color is needed for mouse wheel events
                    dc.DrawRectangle(RadarResources.Background, null, CanvasData.ClipRegion.Rect);

                    if (VisibilityFlags.HasFlag(RadarVisibilityFlags.Terrain))
                    {
                        var drawExtras = VisibilityFlags.HasFlag(RadarVisibilityFlags.SceneInfo);
                        DrawScenes(dc, CanvasData, drawExtras, drawExtras);
                    }

                    if (VisibilityFlags.HasFlag(RadarVisibilityFlags.RangeGuide))
                    {
                        DrawRangeGuide(dc, CanvasData);
                    }

                    DrawTrinityGrid(dc, CanvasData);

                    DrawNotInCacheObjects(dc, CanvasData);

                    if (VisibilityFlags.HasFlag(RadarVisibilityFlags.CurrentPath))
                    {
                        DrawCurrentpath(dc, CanvasData);
                    }

                    if (VisibilityFlags.HasFlag(RadarVisibilityFlags.CurrentTarget))
                    {
                        DrawTargetting(dc, CanvasData);
                    }

                    if (VisibilityFlags.HasFlag(RadarVisibilityFlags.ExploreNodes))
                    {
                        DrawExploreNodes(dc, CanvasData);
                    }

                    foreach (var actor in Objects)
                    {
                        if (!actor.Morph.IsBeyondCanvas)
                        {
                            DrawActor(dc, CanvasData, actor);
                        }
                    }

                    DrawCurrentSceneConnectionPath(dc, CanvasData);

                    DrawDebugPosition(dc, CanvasData);

                    DrawClickRays(dc, CanvasData);

                    DrawDeathGates(dc, CanvasData);

                    DrawSceneDataRegions(dc, CanvasData);

                    DrawAdventurerNavigation(dc, CanvasData);

                    DrawMarkers(dc, CanvasData);

                    if (VisibilityFlags.HasFlag(RadarVisibilityFlags.KiteDirection))
                    {
                        DrawKiteDirection(dc, CanvasData);
                    }

                    if (VisibilityFlags.HasFlag(RadarVisibilityFlags.ActivePlayer))
                    {
                        DrawActivePlayer(dc, CanvasData, Player);
                    }

                    foreach (var avoidance in Core.Avoidance.CurrentAvoidances)
                    {
                        foreach (var actor in avoidance.Actors)
                        {
                            var part = avoidance.Definition.GetPart(actor.ActorSnoId);
                            if (part != null)
                            {
                                var r = part.Radius * GridSize;
                                dc.DrawEllipse(null, RadarResources.AvoidanceLightPen, actor.Position.ToCanvasPoint(), r, r);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Core.Logger.Log("Exception in RadarUI.OnRender(). {0} {1}", ex.Message, ex.InnerException);
                }
            }
        }

        private void DrawAdventurerNavigation(DrawingContext dc, CanvasData canvasData)
        {
            dc.DrawLine(RadarResources.BorderPen, CanvasData.CenterVector.ToCanvasPoint(), NavigationCoroutine.LastDestination.ToCanvasPoint());
        }

        private void DrawDeathGates(DrawingContext dc, CanvasData canvasData)
        {
            foreach (var rect in DeathGates.ExitRegion.Cast<RectangularRegion>())
            {
                if (rect.Max.X - rect.Min.X > 240 || rect.Max.Y - rect.Min.Y > 240)
                {
                    dc.DrawDrawing(GetRectangle(rect));
                }
                else
                {
                    dc.DrawDrawing(GetOutlineRectangle(rect));
                }
            }

            foreach (var rect in DeathGates.EnterRegion.Cast<RectangularRegion>())
            {
                if (rect.Max.X - rect.Min.X > 240 || rect.Max.Y - rect.Min.Y > 240)
                {
                    dc.DrawDrawing(GetRectangle(rect));
                }
                else
                {
                    dc.DrawDrawing(GetOutlineRectangle(rect));
                }
            }

            foreach (var zone in DeathGates.Scenes.Where(z => z.IsValid))
            {
                dc.DrawLine(RadarResources.SuccessPen, zone.ShallowPortalPosition.ToCanvasPoint(), zone.DeepPortalPosition.ToCanvasPoint());
            }
        }

        private void DrawSceneDataRegions(DrawingContext dc, CanvasData canvasData)
        {
            foreach (var scene in Core.Scenes)
            {
                foreach (var rect in scene.IgnoreRegions)
                {
                    dc.DrawDrawing(GetRectangle(rect as RectangularRegion));
                }
            }
        }

        private static DrawingGroup GetRectangle(RectangularRegion enterRegion)
        {
            var drawing = new DrawingGroup();
            if (enterRegion == null) return drawing;
            using (var groupdc = drawing.Open())
            {
                var pen = enterRegion.CombineType == CombineType.Add ? RadarResources.SceneFrameInclude : RadarResources.SceneFrameExclude;
                var enterTopLeft = new Vector3(enterRegion.Min.X, enterRegion.Min.Y, 0);
                var enterTopRight = new Vector3(enterRegion.Max.X, enterRegion.Min.Y, 0);
                var enterBottomLeft = new Vector3(enterRegion.Min.X, enterRegion.Max.Y, 0);
                var enterBottomRight = new Vector3(enterRegion.Max.X, enterRegion.Max.Y, 0);
                groupdc.DrawGeometry(null, pen, new LineGeometry(enterTopLeft.ToCanvasPoint(), enterTopRight.ToCanvasPoint()));
                groupdc.DrawGeometry(null, pen, new LineGeometry(enterBottomLeft.ToCanvasPoint(), enterBottomRight.ToCanvasPoint()));
                groupdc.DrawGeometry(null, pen, new LineGeometry(enterTopLeft.ToCanvasPoint(), enterBottomLeft.ToCanvasPoint()));
                groupdc.DrawGeometry(null, pen, new LineGeometry(enterTopRight.ToCanvasPoint(), enterBottomRight.ToCanvasPoint()));
            }
            return drawing;
        }

        private static DrawingGroup GetOutlineRectangle(RectangularRegion enterRegion)
        {
            var drawing = new DrawingGroup();
            using (var groupdc = drawing.Open())
            {
                var padding = 3;
                var pen = enterRegion.CombineType == CombineType.Add ? RadarResources.SceneFrameInclude : RadarResources.SceneFrameExclude;
                var enterTopLeft = new Vector3(enterRegion.Min.X + padding, enterRegion.Min.Y + padding, 0);
                var enterTopRight = new Vector3(enterRegion.Max.X - padding, enterRegion.Min.Y + padding, 0);
                var enterBottomLeft = new Vector3(enterRegion.Min.X + padding, enterRegion.Max.Y - padding, 0);
                var enterBottomRight = new Vector3(enterRegion.Max.X - padding, enterRegion.Max.Y - padding, 0);
                groupdc.DrawGeometry(null, pen, new LineGeometry(enterTopLeft.ToCanvasPoint(), enterTopRight.ToCanvasPoint()));
                groupdc.DrawGeometry(null, pen, new LineGeometry(enterBottomLeft.ToCanvasPoint(), enterBottomRight.ToCanvasPoint()));
                groupdc.DrawGeometry(null, pen, new LineGeometry(enterTopLeft.ToCanvasPoint(), enterBottomLeft.ToCanvasPoint()));
                groupdc.DrawGeometry(null, pen, new LineGeometry(enterTopRight.ToCanvasPoint(), enterBottomRight.ToCanvasPoint()));
                //groupdc.DrawGeometry(null, pen, new LineGeometry(enterBottomRight.ToCanvasPoint(), enterTopLeft.ToCanvasPoint()));
            }
            return drawing;
        }

        private void DrawMarkers(DrawingContext dc, CanvasData canvasData)
        {
            if (!VisibilityFlags.HasFlag(RadarVisibilityFlags.Markers))
                return;

            foreach (var marker in Core.Markers)
            {
                var pen = marker.MarkerType == WorldMarkerType.Objective ? RadarResources.EliteLightPen : RadarResources.MarkerPen;
                var markerPoint = marker.Position.ToCanvasPoint();
                dc.DrawLine(pen, CenterActor.Point, markerPoint);
                DrawLabel(dc, CanvasData, marker.Name, markerPoint, OrangeBrush, 45, 12, 3);
            }
        }

        private void DrawDebugPosition(DrawingContext dc, CanvasData canvasData)
        {
            if (VisualizerViewModel.DebugPosition != Vector3.Zero)
            {
                dc.DrawLine(RadarResources.SuccessPen, VisualizerViewModel.DebugPosition.ToCanvasPoint(), Player.Point);
            }
        }

        private void DrawClickRays(DrawingContext dc, CanvasData canvasData)
        {
            if (CurrentRayWalk != null && CurrentRayWalk.Item1 != Vector3.Zero && CurrentRayWalk.Item2 != Vector3.Zero)
            {
                var rayWalkPen = CurrentRayWalk.Item3 ? RadarResources.SuccessPen : RadarResources.FailurePen;
                dc.DrawLine(rayWalkPen, CurrentRayWalk.Item1.ToCanvasPoint(), CurrentRayWalk.Item2.ToCanvasPoint());
            }
            if (CurrentRayCast != null && CurrentRayCast.Item1 != Vector3.Zero && CurrentRayCast.Item2 != Vector3.Zero)
            {
                var rayWalkPen = CurrentRayCast.Item3 ? RadarResources.SuccessPen : RadarResources.FailurePen;
                dc.DrawLine(rayWalkPen, CurrentRayCast.Item1.ToCanvasPoint(), CurrentRayCast.Item2.ToCanvasPoint());
            }
            if (CurrentTestLine != null && CurrentTestLine.Item1 != Vector3.Zero && CurrentTestLine.Item2 != Vector3.Zero)
            {
                dc.DrawLine(RadarResources.BlackPen, CurrentTestLine.Item1.ToCanvasPoint(), CurrentTestLine.Item2.ToCanvasPoint());
            }
        }



        private DefaultNavigationProvider NavigationProvider { get; set; }
        public static Brush WhiteBrush { get; set; }

        private void DrawCurrentpath(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                if (NavigationProvider == null)
                    NavigationProvider = Navigator.GetNavigationProviderAs<DefaultNavigationProvider>();

                var currentPath = NavigationProvider.CurrentPath;

                for (int i = 1; i < currentPath.Count; i++)
                {
                    var to = currentPath[i];
                    var from = currentPath[i - 1];

                    if (currentPath.Index == i)
                        dc.DrawEllipse(null, RadarResources.CurrentPathPen1, to.ToCanvasPoint(), GridSize + 2, GridSize + 2);

                    dc.DrawLine(RadarResources.CurrentPathPen1, from.ToCanvasPoint(), to.ToCanvasPoint());
                }

                var losPathPosition = Core.Grids.Avoidance.GetPathCastPosition(50f);
                if (losPathPosition != Vector3.Zero)
                {
                    var losPathPoint = losPathPosition.ToCanvasPoint();
                    dc.DrawLine(RadarResources.BlackPen, Player.Point, losPathPoint);
                    dc.DrawEllipse(RedBrush, null, losPathPoint, GridSize, GridSize);
                }

            }
            catch (Exception ex)
            {
                Core.Logger.Log("Exception in RadarUI.DrawCurrentpath(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private void DrawCurrentSceneConnectionPath(DrawingContext dc, CanvasData canvas)
        {
            try
            { 
                if (SceneConnectionPath == null) return;
                for (int i = 0; i < SceneConnectionPath.Count; i++)
                {
                    var to = SceneConnectionPath[i];
                    var from = i == 0 ? canvas.CenterVector  : SceneConnectionPath[i - 1];
                    dc.DrawLine(RadarResources.CurrentPathPen2, from.ToCanvasPoint(), to.ToCanvasPoint());
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Log("Exception in RadarUI.DrawCurrentSceneConnectionPath(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private void DrawTargetting(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                var target = TrinityCombat.Targeting.CurrentTarget;
                if (target == null)
                    return;

                var targetPosition = target.Position;
                if (targetPosition == Vector3.Zero)
                    return;

                var targetPoint = targetPosition.ToCanvasPoint();
                dc.DrawEllipse(PinkBrush, null, targetPoint, GridSize, GridSize);
                dc.DrawLine(PinkPen, Player.Point, targetPoint);

                var power = TrinityCombat.Targeting.CurrentPower;
                if (power == null)
                    return;

                Point spellPoint;

                if (power.TargetAcdId == -1 && power.TargetPosition != Vector3.Zero)
                {
                    spellPoint = power.TargetPosition.ToCanvasPoint();
                }
                else
                {
                    var spellTarget = Core.Actors.RActorByAcdId<TrinityActor>(power.TargetAcdId);
                    if (spellTarget == null)
                        return;

                    spellPoint = spellTarget.Position.ToCanvasPoint();
                }

                dc.DrawEllipse(DeepPinkBrush, null, spellPoint, GridSize, GridSize);
                dc.DrawLine(DeepPinkPen, Player.Point, spellPoint);
            }
            catch (Exception ex)
            {
                Core.Logger.Log("Exception in RadarUI.DrawAvoidanceGrid(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private void DrawExploreNodes(DrawingContext dc, CanvasData canvas)
        {
            try
            {

                foreach (var node in _explorationNodes)
                {
                    var weight = ExplorationHelpers.PriorityDistanceFormula(node);

                    SolidColorBrush color;
                    if (node.IsVisited)
                        color = RadarResources.Node0;
                    else if (node.IsBlacklisted || !node.Scene.HasPlayerConnection && node.Scene.IsTopLevel)
                        color = BlackBrush;
                    else
                    {
                        if (weight < 0.05)
                        {
                            color = RadarResources.NodeA;
                        }
                        else if (weight < 0.15)
                        {
                            color = RadarResources.NodeB;
                        }
                        else if (weight < 0.35)
                        {
                            color = RadarResources.NodeC;
                        }
                        else if (weight < 0.60)
                        {
                            color = RadarResources.NodeD;
                        }
                        else
                        {
                            color = RadarResources.GreenBrush;
                        }
                    }

                    //var color = node.IsVisited ? PinkBrush :
                    //    node.IsBlacklisted ? BlackBrush :
                    //    node.HasEnoughNavigableCells && node.Priority ? DarkGreenBrush : 
                    //    node.HasEnoughNavigableCells ? GreenBrush : null;

                    var border = node.Priority ? RadarResources.BlackPen : null;

                    if (color != null)
                    {
                        var point = node.NavigableCenter.ToCanvasPoint();
                        dc.DrawEllipse(color, border, point, 5, 5);

                        if (node.IsBlacklisted || node.IsVisited)
                            continue;

                        var weightLabel = $"{weight:.000}";
                        DrawLabel(dc, canvas, weightLabel, point, color, 15, 12, 8);
                    }
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Log("Exception in RadarUI.DrawAvoidanceGrid(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }





        private void DrawTrinityGrid(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                if (Core.Avoidance.GridEnricher.CurrentNodes == null)
                    return;

                foreach (var node in Core.Avoidance.GridEnricher.CurrentNodes)
                {
                    DrawNavNode(dc, canvas, node);
                }

                foreach (var node in Core.BlockedCheck.Nodes)
                {
                    dc.DrawEllipse(BlackBrush, null, node.NavigableCenter.ToCanvasPoint(), 2 * Scale, 2 * Scale);
                }


            }
            catch (Exception ex)
            {
                Core.Logger.Verbose("Exception in RadarUI.DrawAvoidanceGrid(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        // I dont know what they're doing in the Brushes resource but its probably returning a new object from a color.
        // This is bad, very bad for performance. Use a fixed resource like these brushes below.
        internal static readonly SolidColorBrush BlueBrush = Brushes.Blue;
        internal static readonly SolidColorBrush SkyBlueBrush = Brushes.DeepSkyBlue;
        internal static readonly SolidColorBrush OrangeRedBrush = Brushes.OrangeRed;
        internal static readonly SolidColorBrush RedBrush = Brushes.DarkRed;
        internal static readonly SolidColorBrush GreenBrush = Brushes.Green;
        internal static readonly SolidColorBrush DarkGreenBrush = Brushes.DarkGreen;

        internal static readonly SolidColorBrush BlackBrush = Brushes.Black;
        internal static readonly SolidColorBrush DarkGrayBrush = Brushes.DarkGray;

        internal static readonly SolidColorBrush DeepPinkBrush = Brushes.DeepPink;
        internal static readonly Pen DeepPinkPen = new Pen(DeepPinkBrush, 2);

        internal static readonly SolidColorBrush PinkBrush = Brushes.Pink;
        internal static readonly Pen PinkPen = new Pen(PinkBrush, 2);

        internal static readonly SolidColorBrush LineGreenBrush = Brushes.LimeGreen;
        internal static readonly SolidColorBrush OrangeBrush = Brushes.Orange;
        internal static readonly SolidColorBrush YellowBrush = Brushes.Yellow;

        private void DrawNavNode(DrawingContext dc, CanvasData canvas, AvoidanceNode node)
        {
            if (node == null)
                return;

            var size = Math.Max(2, GridSize / 3);

            // Draw Layer 1
            if (node.AvoidanceFlags.HasFlag(AvoidanceFlags.Backtrack))
            {
                if (VisibilityFlags.HasFlag(RadarVisibilityFlags.BacktrackNodes))
                    DrawNodeArea(dc, canvas, node, RadarResources.WalkedTerrain);
            }

            if (node.AvoidanceFlags.HasFlag(AvoidanceFlags.ProjectileBlocking))
            {
                dc.DrawEllipse(BlackBrush, null, node.NavigableCenter.ToCanvasPoint(), size, size);
                return;
            }

            // Draw Layer 2
            if (node.AvoidanceFlags.HasFlag(AvoidanceFlags.Avoidance) && VisibilityFlags.HasFlag(RadarVisibilityFlags.Avoidance))
            {
                var weightedBrush = RadarResources.GetWeightedBrush(node.Weight, node.WeightPct);

                if (node.Weight > 2 && Core.Avoidance.GridEnricher.HighestNodeWeight > 2)
                {
                    dc.DrawEllipse(weightedBrush, null, node.NavigableCenter.ToCanvasPoint(), size * 1.5, size * 1.5);
                }

                dc.DrawEllipse(weightedBrush, null, node.NavigableCenter.ToCanvasPoint(), size, size);
                return;
            }

            if (node.AvoidanceFlags.HasFlag(AvoidanceFlags.Monster) && VisibilityFlags.HasFlag(RadarVisibilityFlags.Monsters))
            {
                dc.DrawEllipse(BlueBrush, null, node.NavigableCenter.ToCanvasPoint(), size, size);
                return;
            }

            if (node.AvoidanceFlags.HasFlag(AvoidanceFlags.KiteFrom) && VisibilityFlags.HasFlag(RadarVisibilityFlags.KiteFromNodes))
            {
                dc.DrawEllipse(SkyBlueBrush, null, node.NavigableCenter.ToCanvasPoint(), size, size);
                return;
            }

            if (node.AvoidanceFlags.HasFlag(AvoidanceFlags.AdjacentSafe) && VisibilityFlags.HasFlag(RadarVisibilityFlags.SafeNodes))
            {
                dc.DrawEllipse(GreenBrush, null, node.NavigableCenter.ToCanvasPoint(), size, size);
                return;
            }

            if (node.AvoidanceFlags.HasFlag(AvoidanceFlags.AllowWalk) && VisibilityFlags.HasFlag(RadarVisibilityFlags.WalkableNodes))
            {
                dc.DrawEllipse(DarkGrayBrush, null, node.NavigableCenter.ToCanvasPoint(), size, size);
            }

        }



        private Dictionary<DateTime, StaticTelegraph> _telegraphedNodes = new Dictionary<DateTime, StaticTelegraph>();
        private AvoidanceNode _testkiteNode;

        public class StaticTelegraph
        {
            public Vector3 Origin;
            public Vector3 Target;
        }

        private void DrawKiteDirection(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                var centroid = Core.PlayerHistory.Centroid;
                if (centroid == Vector3.Zero)
                    return;

                var canvasPoint = centroid.ToCanvasPoint();
                dc.DrawEllipse(Brushes.Black, null, canvasPoint, 5, 5);
                dc.DrawLine(new Pen(Brushes.Black, 2), Core.Player.Position.ToCanvasPoint(), canvasPoint);
            }
            catch (Exception ex)
            {
                Core.Logger.Log("Exception in RadarUI.DrawKiteDirection(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }


        private void DrawNodeArea(DrawingContext dc, CanvasData canvas, AvoidanceNode node, Brush brush, Pen pen = null)
        {
            try
            {
                if (node == null)
                    return;

                if (brush == null)
                    brush = YellowBrush;

                dc.DrawEllipse(brush, null, node.NavigableCenter.ToCanvasPoint(), 4 * Scale, 4 * Scale);

            }
            catch (Exception ex)
            {
                Core.Logger.Log("Exception in RadarUI.DrawRotatedRectangle(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }


        /// <summary>
        /// RotationTransform.TransformBounds will not preserve the shape of the rectangle through a rotation. 
        /// So we have to do it the hard way and treat it like an arbitrary polygon.
        /// </summary>
        private void DrawRectangle(DrawingContext dc, CanvasData canvas, Point topLeft, Point bottomLeft, Point bottomRight, Point topRight, Brush brush = null, Pen pen = null)
        {
            try
            {
                var drawing = new DrawingGroup();

                if (brush == null)
                    brush = YellowBrush;

                using (var groupdc = drawing.Open())
                {
                    var figures = new List<PathFigure>();

                    var segments = new[]
                    {
                        new LineSegment(bottomLeft, true),
                        new LineSegment(topLeft, true),
                        new LineSegment(topRight, true),
                    };

                    figures.Add(new PathFigure(bottomRight, segments, true));
                    var geo = new PathGeometry(figures, FillRule.Nonzero, null);
                    geo.GetOutlinedPathGeometry();
                    groupdc.DrawGeometry(brush, pen, geo);
                }

                dc.DrawDrawing(drawing);
            }
            catch (Exception ex)
            {
                Core.Logger.Log("Exception in RadarUI.DrawRotatedRectangle(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }


        public static class GrahamScan
        {
            public static IList<Vector3> GrahamScanCompute(IEnumerable<Vector3> initialPoints)
            {
                var initialPointsList = initialPoints.ToList();

                if (initialPointsList.Count < 2)
                    return initialPointsList.ToList();

                // Find point with minimum y; if more than one, minimize x also.
                int iMin = Enumerable.Range(0, initialPointsList.Count).Aggregate((jMin, jCur) =>
                {
                    if (initialPointsList[jCur].Y < initialPointsList[jMin].Y)
                        return jCur;
                    if (initialPointsList[jCur].Y > initialPointsList[jMin].Y)
                        return jMin;
                    if (initialPointsList[jCur].X < initialPointsList[jMin].X)
                        return jCur;
                    return jMin;
                });
                // Sort them by polar angle from iMin, 
                var sortQuery = Enumerable.Range(0, initialPointsList.Count)
                    .Where((i) => (i != iMin)) // Skip the min point
                    .Select((i) => new KeyValuePair<double, Vector3>(Math.Atan2(initialPointsList[i].Y - initialPointsList[iMin].Y, initialPointsList[i].X - initialPointsList[iMin].X), initialPointsList[i]))
                    .OrderBy((pair) => pair.Key)
                    .Select((pair) => pair.Value);
                List<Vector3> points = new List<Vector3>(initialPointsList.Count);
                points.Add(initialPointsList[iMin]);     // Add minimum point
                points.AddRange(sortQuery);          // Add the sorted points.

                int M = 0;
                for (int i = 1, N = points.Count; i < N; i++)
                {
                    bool keepNewPoint = true;
                    if (M == 0)
                    {
                        // Find at least one point not coincident with points[0]
                        keepNewPoint = !NearlyEqual(points[0], points[i]);
                    }
                    else
                    {
                        while (true)
                        {
                            var flag = WhichToRemoveFromBoundary(points[M - 1], points[M], points[i]);
                            if (flag == RemovalFlag.None)
                                break;
                            else if (flag == RemovalFlag.MidPoint)
                            {
                                if (M > 0)
                                    M--;
                                if (M == 0)
                                    break;
                            }
                            else if (flag == RemovalFlag.EndPoint)
                            {
                                keepNewPoint = false;
                                break;
                            }
                            else
                                throw new Exception("Unknown RemovalFlag");
                        }
                    }
                    if (keepNewPoint)
                    {
                        M++;
                        Swap(points, M, i);
                    }
                }
                // points[M] is now the last point in the boundary.  Remove the remainder.
                points.RemoveRange(M + 1, points.Count - M - 1);
                return points;
            }

            static void Swap<T>(IList<T> list, int i, int j)
            {
                if (i != j)
                {
                    T temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }

            public static double RelativeTolerance { get { return 1e-10; } }

            public static bool NearlyEqual(Vector3 a, Vector3 b)
            {
                return NearlyEqual(a.X, b.X) && NearlyEqual(a.Y, b.Y);
            }

            public static bool NearlyEqual(double a, double b)
            {
                return NearlyEqual(a, b, RelativeTolerance);
            }

            public static bool NearlyEqual(double a, double b, double epsilon)
            {
                // See here: http://floating-point-gui.de/errors/comparison/
                if (a == b)
                { // shortcut, handles infinities
                    return true;
                }

                double absA = Math.Abs(a);
                double absB = Math.Abs(b);
                double diff = Math.Abs(a - b);
                double sum = absA + absB;
                if (diff < 4 * double.Epsilon || sum < 4 * double.Epsilon)
                    // a or b is zero or both are extremely close to it
                    // relative error is less meaningful here
                    return true;

                // use relative error
                return diff / (absA + absB) < epsilon;
            }

            static double CCW(Vector3 p1, Vector3 p2, Vector3 p3)
            {
                // Compute (p2 - p1) X (p3 - p1)
                double cross1 = (p2.X - p1.X) * (p3.Y - p1.Y);
                double cross2 = (p2.Y - p1.Y) * (p3.X - p1.X);
                if (NearlyEqual(cross1, cross2))
                    return 0;
                return cross1 - cross2;
            }

            enum RemovalFlag
            {
                None,
                MidPoint,
                EndPoint
            };

            static RemovalFlag WhichToRemoveFromBoundary(Vector3 p1, Vector3 p2, Vector3 p3)
            {
                var cross = CCW(p1, p2, p3);
                if (cross < 0)
                    // Remove p2
                    return RemovalFlag.MidPoint;
                if (cross > 0)
                    // Remove none.
                    return RemovalFlag.None;
                // Check for being reversed using the dot product off the difference vectors.
                var dotp = (p3.X - p2.X) * (p2.X - p1.X) + (p3.Y - p2.Y) * (p2.Y - p1.Y);
                if (NearlyEqual(dotp, 0.0))
                    // Remove p2
                    return RemovalFlag.MidPoint;
                if (dotp < 0)
                    // Remove p3
                    return RemovalFlag.EndPoint;
                else
                    // Remove p2
                    return RemovalFlag.MidPoint;
            }
        }

        private bool _isSceneInfoVisible;
        private List<ExplorationNode> _explorationNodes;

        private void DrawScenes(DrawingContext dc, CanvasData canvas, bool sceneborders, bool sceneLabels)
        {
            try
            {
                if (Math.Abs(CenterActor.Point.X) < 0.01f && Math.Abs(CenterActor.Point.Y) < 0.01f)
                    return;

                var sceneInfoVisibility = VisibilityFlags.HasFlag(RadarVisibilityFlags.SceneInfo);
                if (_isSceneInfoVisible != sceneInfoVisibility)
                {
                    Drawings.Relative.Clear();
                    _isSceneInfoVisible = sceneInfoVisibility;
                }

                var worldId = Core.Player.WorldDynamicId;

                List<string> connectedScenesStr = new List<string>();
                foreach (var adventurerScene in Core.Scenes.CurrentWorldScenes.Where(s => s.DynamicWorldId == worldId).ToList())
                {
                    connectedScenesStr.Clear();
                    var connectedScenes = adventurerScene.ConnectedScenes().ToList();

                    if (sceneborders)
                    {
                        foreach (var connectedScene in connectedScenes)
                        {
                            if (connectedScene.Scene != null)
                            {
                                Vector2 edgePointA = connectedScene.EdgePointA;
                                Vector2 edgePointB = connectedScene.EdgePointB;

                                dc.DrawLine(RadarResources.SceneConnectionPen,
                                    edgePointA.ToVector3().ToCanvasPoint(),
                                    edgePointB.ToVector3().ToCanvasPoint());

                                connectedScenesStr.Add(connectedScene.Scene.Name + " " + connectedScene.Direction);
                            }
                        }
                    }

                    var exitPositions = adventurerScene.ExitPositions;
                        var unconnectedExits =
                            exitPositions.Where(
                                ep => connectedScenes.FirstOrDefault(cs => cs.Direction == ep.Key) == null).ToList();
                        foreach (var exitPosition in adventurerScene.ExitPositions.Values)
                        {
                            if (exitPosition != Vector3.Zero)
                            {
                                if (unconnectedExits.Any(ep => exitPosition.Distance(ep.Value) <= 15))
                                    dc.DrawEllipse(null, RadarResources.FailurePen, exitPosition.ToCanvasPoint(), 10, 10);
                                else
                                    dc.DrawEllipse(null, RadarResources.EliteLightPen, exitPosition.ToCanvasPoint(), 10,
                                        10);
                            }
                        }
                    

                    // Combine navcells into one drawing and store it; because they don't change relative to each other
                    // And because translating geometry for every navcell on every frame is waaaaay too slow.
                    if (Drawings.Relative.ContainsKey(adventurerScene.HashName))
                        continue;

                    //SceneDrawings.Instance.Record(adventurerScene);

                    var drawing = new DrawingGroup();

                    using (var groupdc = drawing.Open())
                    {
                        #region Terrain
                        if (adventurerScene.Cells != null)
                        {
                            var figures = new List<PathFigure>();
                            foreach (var navCell in adventurerScene.Cells.Where(nc => nc.IsWalkable))
                            {
                                var topLeft = new Vector3(navCell.MinX, navCell.MinY, 0);
                                var topRight = new Vector3(navCell.MaxX, navCell.MinY, 0);
                                var bottomLeft = new Vector3(navCell.MinX, navCell.MaxY, 0);
                                var bottomRight = new Vector3(navCell.MaxX, navCell.MaxY, 0);
                                var segments = new[]
                                               {
                                                   new LineSegment(topLeft.ToCanvasPoint(), true),
                                                   new LineSegment(topRight.ToCanvasPoint(), true),
                                                   new LineSegment(bottomRight.ToCanvasPoint(), true)
                                               };

                                figures.Add(new PathFigure(bottomLeft.ToCanvasPoint(), segments, true));
                            }
                            var geo = new PathGeometry(figures, FillRule.Nonzero, null);
                            geo.GetOutlinedPathGeometry();
                            groupdc.DrawGeometry(RadarResources.WalkableTerrain, null, geo);
                        }
                        #endregion

                        #region Scene Borders
                        if (sceneborders)
                        {
                            var sceneTopLeft = new Vector3(adventurerScene.Min.X, adventurerScene.Min.Y, 0);
                            var sceneTopRight = new Vector3(adventurerScene.Max.X, adventurerScene.Min.Y, 0);
                            var sceneBottomLeft = new Vector3(adventurerScene.Min.X, adventurerScene.Max.Y, 0);
                            var sceneBottomRight = new Vector3(adventurerScene.Max.X, adventurerScene.Max.Y, 0);

                            groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopLeft.ToCanvasPoint(), sceneTopRight.ToCanvasPoint()));
                            groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneBottomLeft.ToCanvasPoint(), sceneBottomRight.ToCanvasPoint()));
                            groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopLeft.ToCanvasPoint(), sceneBottomLeft.ToCanvasPoint()));
                            groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopRight.ToCanvasPoint(), sceneBottomRight.ToCanvasPoint()));
                        }
                        #endregion

                        #region Sub-Scene Borders
                        if (sceneborders && adventurerScene.SubScene != null)
                        {
                            var sceneTopLeft = new Vector3(adventurerScene.SubScene.Min.X, adventurerScene.SubScene.Min.Y, 0);
                            var sceneTopRight = new Vector3(adventurerScene.SubScene.Max.X, adventurerScene.SubScene.Min.Y, 0);
                            var sceneBottomLeft = new Vector3(adventurerScene.SubScene.Min.X, adventurerScene.SubScene.Max.Y, 0);
                            var sceneBottomRight = new Vector3(adventurerScene.SubScene.Max.X, adventurerScene.SubScene.Max.Y, 0);

                            groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopLeft.ToCanvasPoint(), sceneTopRight.ToCanvasPoint()));
                            groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneBottomLeft.ToCanvasPoint(), sceneBottomRight.ToCanvasPoint()));
                            groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopLeft.ToCanvasPoint(), sceneBottomLeft.ToCanvasPoint()));
                            groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopRight.ToCanvasPoint(), sceneBottomRight.ToCanvasPoint()));
                        }
                        #endregion

                        #region Scene Title

                        if (sceneborders)
                        {
                            var textPoint = adventurerScene.Center.ToVector3().ToCanvasPoint();
                            var glyphRun = DrawingUtilities.CreateGlyphRun(adventurerScene.Name, 10, textPoint);
                            groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun);
                            textPoint = adventurerScene.Center.ToVector3().ToCanvasPoint();
                            textPoint.Y = textPoint.Y + 20;
                            //(adventurerScene.HasChild ? "HasSubScene" : string.Empty) + " " + 
                            glyphRun = DrawingUtilities.CreateGlyphRun($"{adventurerScene.Min},{adventurerScene.Max}" + " " + (adventurerScene.SubScene != null ? $" ({adventurerScene.SubScene.Name})" : string.Empty), 8, textPoint);
                            groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun);

                            foreach (string conScene in connectedScenesStr)
                            {
                                textPoint.Y = textPoint.Y + 20;
                                glyphRun = DrawingUtilities.CreateGlyphRun(conScene, 8, textPoint);
                                groupdc.DrawGlyphRun(Brushes.Red, glyphRun);
                            }
                        }
                        #endregion
                    }

                    // Have to use SceneHash as key because scenes can appear multiple times with the same name
                    Drawings.Relative.TryAdd(adventurerScene.HashName, new RelativeDrawing
                    {
                        Drawing = drawing,
                        Origin = CenterActor.Morph,
                        Center = adventurerScene.Center.ToVector3(),
                        WorldId = adventurerScene.DynamicWorldId,
                    });
                }

                foreach (var pair in Drawings.Relative)
                {
                    if (pair.Value.Type != DrawingType.Scene)
                        continue;

                    if (pair.Value.WorldId != Core.Player.WorldDynamicId)
                        continue;

                    if (!pair.Value.Drawing.Bounds.IntersectsWith(CanvasData.ClipRegion.Rect))
                        continue;

                    dc.DrawDrawing(pair.Value.Drawing);
                }

            }
            catch (Exception ex)
            {
                Core.Logger.Log("Exception in RadarUI.DrawScenes(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        /// <summary>
        /// Range guide draws a circle every 20yard from player
        /// </summary>
        private void DrawRangeGuide(DrawingContext dc, CanvasData canvas)
        {
            for (var i = 20; i < 100; i += 20)
            {
                var radius = GridSize * i;
                dc.DrawEllipse(RadarResources.TransparentBrush, RadarResources.RangeGuidePen, Player.Point, radius, radius);
                dc.DrawText(new FormattedText(i.ToString(), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.LightYellow),
                    new Point(Player.Point.X - (radius + 20), Player.Point.Y));
            }
        }


        private void DrawSelection(DrawingContext dc, CanvasData canvas, RadarObject radarObject)
        {
            var actorRadius = radarObject.Actor.Radius * GridSize;
            dc.DrawEllipse(null, RadarResources.SelectionPen, radarObject.Point, actorRadius, actorRadius);
        }

        private void DrawActivePlayer(DrawingContext dc, CanvasData canvas, RadarObject radarObject)
        {

            var brush = radarObject.Actor.IsMe ? RadarResources.PlayerBrush : RadarResources.OtherPlayerBrush;
            brush.Opacity = 0.75;
            var actorRadius = radarObject.Actor.CollisionRadius * GridSize;
            dc.DrawEllipse(brush, null, radarObject.Point, actorRadius, actorRadius);
            dc.DrawEllipse(BlackBrush, null, radarObject.Point, GridSize, GridSize);
            var headingWorldPositionAtRadius = MathEx.GetPointAt(radarObject.Actor.Position, radarObject.Actor.CollisionRadius, radarObject.Actor.Rotation);
            dc.DrawLine(RadarResources.BlackPen, radarObject.Point, headingWorldPositionAtRadius.ToCanvasPoint());
            HitTester.AddEllipse(radarObject, radarObject.Point, actorRadius, actorRadius);
        }

        private void DrawNotInCacheObjects(DrawingContext dc, CanvasData canvas)
        {
            foreach (var actor in VisualizerViewModel.Instance.NotInCacheObjects)
            {
                var brush = actor.IsElite ? RadarResources.EliteBrush : RadarResources.GreyBrush;
                brush.Opacity = 0.75;
                var actorRadius = Math.Min(30, Math.Max(4, actor.CollisionRadius * GridSize));
                var radarObject = new RadarObject(actor, canvas);
                dc.DrawEllipse(brush, null, radarObject.Point, actorRadius, actorRadius);
                HitTester.AddEllipse(radarObject, radarObject.Point, actorRadius, actorRadius);
            }

        }


        private void DrawActor(DrawingContext dc, CanvasData canvas, RadarObject radarObject)
        {
            try
            {
                var res = RadarResources.GetActorResourceSet(radarObject);

                Pen pen = RadarResources.GreyPen;

                if (radarObject.Actor.IsInLineOfSight)
                    pen = RadarResources.LineOfSightPen;

                if (SelectedRadarObject == radarObject)
                    pen = RadarResources.SelectionPen;

                if (radarObject.Actor.IsBlacklisted)
                    pen = RadarResources.BlacklistedPen;

                var gridRadius = Math.Min(30, Math.Max(2 * GridSize, radarObject.Actor.CollisionRadius * GridSize));

                if (radarObject.Actor.Type == TrinityObjectType.BuffedRegion)
                {
                    var size = radarObject.Actor.AxialRadius * GridSize;
                    dc.DrawEllipse(RadarResources.BuffedRegionBrush, RadarResources.BuffedRegionPen, radarObject.Point, size, size);
                    HitTester.AddEllipse(radarObject, radarObject.Point, size, size);
                    return;
                }

                if (VisibilityFlags.HasFlag(RadarVisibilityFlags.RiftValue))
                {
                    var tco = radarObject.Actor as TrinityActor;
                    if (tco != null)
                    {
                        var value = $"{tco.RiftValuePct:0.00}%";
                        DrawLabel(dc, canvas, value, radarObject.Point, YellowBrush, 15, 12, 8);
                    }
                }
                else if (VisibilityFlags.HasFlag(RadarVisibilityFlags.Weighting))
                {
                    var tco = radarObject.Actor as TrinityActor;
                    if (tco != null)
                    {
                        var value = $"{tco.Weight:#.#}";
                        DrawLabel(dc, canvas, value, new Point(radarObject.Point.X + gridRadius, radarObject.Point.Y), YellowBrush, 15, 12, 8);
                    }
                }

                if (radarObject.Actor.IsGizmo && (VisibilityFlags.HasFlag(RadarVisibilityFlags.Gizmos) || VisibilityFlags.HasFlag(RadarVisibilityFlags.Misc)))
                {
                    dc.DrawEllipse(RadarResources.GizmoBrush, pen, radarObject.Point, GridSize, GridSize);

                    if (Zoom > 5)
                        DrawActorLabel(dc, canvas, radarObject, RadarResources.GizmoBrush);

                    HitTester.AddEllipse(radarObject, radarObject.Point, GridSize, GridSize);
                    return;
                }

                if (radarObject.Actor.IsActiveAvoidance && VisibilityFlags.HasFlag(RadarVisibilityFlags.Avoidance))
                {
                    res.Brush = RadarResources.AvoidanceBrush;
                    dc.DrawEllipse(res.Brush, null, radarObject.Point, GridSize / 2, GridSize / 2);
                    res.Brush.Opacity = 0.15;
                    dc.DrawEllipse(res.Brush, pen, radarObject.Point, gridRadius, gridRadius);
                    DrawActorLabel(dc, canvas, radarObject, RadarResources.AvoidanceTextBrush);
                }
                else if (radarObject.Actor.IsMonster && VisibilityFlags.HasFlag(RadarVisibilityFlags.Monsters))
                {
                    // Units may become avoidances in special circumstances, beast charge, about to expode etc
                    if (radarObject.Actor.IsActiveAvoidance)
                        res.Brush = RadarResources.AvoidanceBrush;

                    // Draw a dot in the center of the actor;
                    dc.DrawEllipse(res.Brush, null, radarObject.Point, GridSize / 2, GridSize / 2);

                    // Draw a circle representing the size of the actor
                    res.Brush.Opacity = radarObject.Actor.IsElite ? 0.75 : 0.15;

                    dc.DrawEllipse(res.Brush, pen, radarObject.Point, gridRadius, gridRadius);

                    DrawActorLabel(dc, canvas, radarObject, res.Brush);

                    HitTester.AddEllipse(radarObject, radarObject.Point, gridRadius, gridRadius);
                }
                else if (radarObject.Actor.ActorType == ActorType.Projectile && VisibilityFlags.HasFlag(RadarVisibilityFlags.Projectile))
                {
                    var start = MathEx.GetPointAt(radarObject.Actor.Position, GridSize * 0.5f, radarObject.Actor.Rotation);
                    var end = MathEx.GetPointAt(radarObject.Actor.Position, GridSize * 0.5f, radarObject.Actor.Rotation - (int)Math.PI);
                    dc.DrawLine(RadarResources.ProjectilePen, start.ToCanvasPoint(), end.ToCanvasPoint());
                }
                else if (VisibilityFlags.HasFlag(RadarVisibilityFlags.Misc) && !radarObject.Actor.IsMe)
                {
                    // Units may become avoidances in special circumstances, beast charge, about to expode etc
                    if (radarObject.Actor.IsActiveAvoidance)
                    {
                        res.Brush = RadarResources.AvoidanceBrush;
                        res.Brush.Opacity = 0.85;
                    }
                    else
                    {
                        res.Brush.Opacity = 0.5;
                    }

                    //var gridRadius = radarObject.Actor.Radius * GridSize;
                    dc.DrawEllipse(res.Brush, pen, radarObject.Point, gridRadius, gridRadius);

                    HitTester.AddEllipse(radarObject, radarObject.Point, gridRadius, gridRadius);
                }
            }
            catch (Exception ex)
            {
                Core.Logger.Log("Exception in RadarUI.DrawActor(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private static void DrawLabel(DrawingContext dc, CanvasData canvas, string text, Point origin, Brush brush, int charLimit = 15, int textSize = 12, double yOffset = 0)
        {
            if (text.Length > charLimit)
                text = text.Substring(0, charLimit);

            var point = new Point(origin.X, origin.Y + yOffset);

            // FormattedText is very slow, so instead create text manually with glyphs
            var glyphRun = DrawingUtilities.CreateGlyphRun(text, textSize, point);
            if (glyphRun != null)
                dc.DrawGlyphRun(brush, glyphRun);
        }

        private static void DrawActorLabel(DrawingContext dc, CanvasData canvas, RadarObject radarObject, Brush brush, double yOffset = 0)
        {
            var text = radarObject.Actor.IsGizmo ? radarObject.Actor.Type.ToString() : radarObject.Actor.InternalName;
            if (text == null)
                return;

            DrawLabel(dc, canvas, text, radarObject.Point, brush);
        }



    }







}
