using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Combat;
using Trinity.Components.Combat.Resources;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Trinity.Framework.Avoidance.Structures;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Bot;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;
using CombineType = Trinity.Components.Adventurer.Game.Exploration.SceneMapping.CombineType;
using DeathGates = Trinity.Components.Adventurer.Game.Exploration.SceneMapping.DeathGates;
using LineSegment = System.Windows.Media.LineSegment;
using Logger = Trinity.Framework.Helpers.Logger;
using RectangularRegion = Trinity.Components.Adventurer.Game.Exploration.SceneMapping.RectangularRegion;
using ScenesStorage = Trinity.Components.Adventurer.Game.Exploration.ScenesStorage;

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
                Drawings.Static.Clear();
            };
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            IsRightClick = e.RightButton == MouseButtonState.Pressed;
            IsLeftClick = e.LeftButton == MouseButtonState.Pressed;
            Cursor = Cursors.Hand;
            DragInitialPosition = Mouse.GetPosition(this);
            DragInitialPanOffset = CanvasData.PanOffset;

            //HandleMapClicked(sender, e);
        }

        public bool IsLeftClick { get; set; }
        public bool IsRightClick { get; set; }

        public Tuple<Vector3, Vector3, bool> CurrentRayCast { get; set; }
        public Tuple<Vector3, Vector3, bool> CurrentRayWalk { get; set; }
        public Tuple<Vector3, Vector3, bool> CurrentTestLine { get; set; }


        private void HandleMapClicked(object sender, MouseButtonEventArgs e)
        {
            var clickedWorldPosition = PointMorph.GetWorldPosition(DragInitialPosition, CanvasData);

            Logger.Log($"Clicked World Position = {clickedWorldPosition}, Distance={clickedWorldPosition.Distance(ZetaDia.Me.Position)}");

            if (!IsDragging)
            {
                if (IsRightClick)
                {
                    var result = Core.Grids.Avoidance.CanRayWalk(clickedWorldPosition, Player.Actor.Position);
                    CurrentRayWalk = new Tuple<Vector3, Vector3, bool>(clickedWorldPosition, Player.Actor.Position, result);
                }
                else if (IsLeftClick)
                {
                    var result = Core.Grids.Avoidance.CanRayCast(clickedWorldPosition, Player.Actor.Position);

                    CurrentRayCast = new Tuple<Vector3, Vector3, bool>(clickedWorldPosition, Player.Actor.Position, result);

                    var gatePosition = DeathGates.GetBestGatePosition(clickedWorldPosition);
                    CurrentTestLine = new Tuple<Vector3, Vector3, bool>(gatePosition, Player.Actor.Position, result);

                    var sequence = DeathGates.CreateSequence();
                    for (int i = 0; i < sequence.Count; i++)
                    {
                        var scene = sequence[i];
                        Logger.Log($"{i}: {(sequence.Index == i ? ">> " : "")}{scene.Name}");
                    }

                    //var zone = DeathGates.GetZoneForPosition(clickedWorldPosition);
                    //if (zone != null)
                    //{
                    //    var gatePos = zone.NavigableGatePosition;
                    //    if (gatePos != Vector3.Zero)
                    //    {
                    //        CurrentTestLine = new Tuple<Vector3, Vector3, bool>(gatePos, Player.Actor.Position, result);
                    //    }
                    //}
                }

                var hit = FindElementUnderClick(sender, e);
                if (hit != null)
                {
                    var actor = hit.RadarObject.Actor;
                    ClickCommand.Execute(actor);

                    // Trigger Canvas to Render
                    InvalidateVisual();

                    return;
                }

                var node = Core.Avoidance.Grid.GetNearestNode(clickedWorldPosition);
                SelectedItem = new TrinityActor
                {
                    InternalName = $"Node[{node.GridPoint.X},{node.GridPoint.Y}] World[{(int)clickedWorldPosition.X},{(int)clickedWorldPosition.Y}]",
                    Distance = clickedWorldPosition.Distance(ZetaDia.Me.Position),
                    Position = clickedWorldPosition,
                };
            }

            // Trigger Canvas to Render
            InvalidateVisual();
        }


        private RadarHitTestUtility.HitContainer FindElementUnderClick(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition((UIElement)sender);
            return HitTester.GetHit(position);
        }

        private DateTime LastMoveCursorCheck = DateTime.MinValue;

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
            {
                IsDragging = true;
                const double dragSpeed = 0.5;
                var position = e.GetPosition(this);
                var xOffset = position.X - DragInitialPosition.X;
                var yOffset = position.Y - DragInitialPosition.Y;
                DragOffset = new Point((int)(xOffset * dragSpeed) + DragInitialPanOffset.X, (int)(yOffset * dragSpeed) + DragInitialPanOffset.Y);
                CanvasData.PanOffset = DragOffset;
            }
            else
            {
                IsDragging = false;
            }

            if (DateTime.UtcNow.Subtract(LastMoveCursorCheck).TotalMilliseconds > 25)
            {
                LastMoveCursorCheck = DateTime.UtcNow;
                if (FindElementUnderClick(sender, e) != null)
                {
                    Mouse.OverrideCursor = Cursors.Hand;
                }
                else
                {
                    Mouse.OverrideCursor = null;
                }
            }
        }

        //public bool IsDragging { get; set; }

        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = false;
            Cursor = Cursors.Arrow;
            CanvasData.PanOffset = DragOffset;

            HandleMapClicked(sender, e);
        }

        private void Clear()
        {
            Drawings.Relative.Clear();
            Drawings.Static.Clear();
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
            Logger.LogVerbose("[RadarUI] Zoom changed from {0} to {1}", args.OldValue, args.NewValue);
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
            Logger.LogVerbose("[RadarUI] Dragging Changed from {0} to {1}", e.OldValue, e.NewValue);
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
            Logger.LogVerbose("[RadarUI] Pan changed from {0} to {1}", args.OldValue, args.NewValue);
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
            Logger.Log("SelectedItem changed from {0} to {1}", args.OldValue, args.NewValue);

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
            Logger.LogVerbose("[RadarUI] VisibilityFlags changed from {0} to {1}", args.OldValue, args.NewValue);
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
                //using (new PerformanceLogger("RadarUI UpdateData"))
                //{

                //if (DateTime.UtcNow.Subtract(LastUpdate).TotalMilliseconds < 5)
                //    return;

                //LastUpdate = DateTime.UtcNow;

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

                //UpdateAvoidanceGridData();

                //UpdateTestData();

                // Trigger LastKiteNodes collection.
                //_testkiteNode = TrinDia.Avoidance.BestKiteNode;

                //Logger.Log("Heading={0}",ZetaDia.Me.Movement.RotationDegrees);

                //if (VisibilityFlags.HasFlag(RadarVisibilityFlags.Clusters))
                //{
                //    UpdateValueCluster();
                //}

                //}

                // Trigger Canvas to Render
                InvalidateVisual();

            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.UpdateData(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private void UpdateExplorationData()
        {
            _explorationNodes = ExplorationGrid.Instance.WalkableNodes;            
        }

        #region ValueCluster

        private List<Vector3> ValueCluster = new List<Vector3>();
        private Vector3 ValueClusterCenter;
        private double ClusterValue;

        private void UpdateValueCluster()
        {
            List<Vector3> valueClusterPositions;
            Vector3 valueClusterCenter;
            double clusterValue;
            TargetUtil.GetValueClusterUnits(45f, 0.03, 14f, out valueClusterPositions, out valueClusterCenter, out clusterValue);
            ValueCluster = valueClusterPositions;
            ValueClusterCenter = valueClusterCenter;
            ClusterValue = clusterValue;
        }

        #endregion



        /// <summary>
        /// Using CenterActor was causing jerkyness, but using an actor for player offset from center actor,
        /// which is then in sync with all other actors smoothed it out.
        /// </summary>
        public RadarObject Player { get; set; }

        public RadarObject SelectedRadarObject { get; set; }

        //private List<TrinityUnit> _clusterUnits = new List<TrinityUnit>();
        //private IEnumerable<IGrouping<int, TrinityUnit>> _clusterGroups = new List<IGrouping<int, TrinityUnit>>();
        //private void UpdateTestData()
        //{
        //    _clusterUnits.Clear();
        //    _clusterUnits = CacheManager.Monsters.Where(m => m.UnitsNearby >= 2 && m.Distance < 200f).OrderBy(m => m.UnitsNearby).ToList();
        //    _clusterGroups = CacheManager.Monsters.Where(m => m.UnitsNearby >= 2 && m.Distance < 200f).GroupBy(m => m.UnitsNearby);

        //}

        //private List<TrinityNode> _nodes = new List<TrinityNode>();
        //private TrinityNode _safeTrinityNode;
        //private void UpdateAvoidanceGridData()
        //{
        //    if (CacheManager.Me.NearestNode == null)
        //        return;

        //    if (!TrinityGrid.CurrentNodes.Any())
        //        return;

        //    //_safeNode = TrinityGrid.FindSafeNode(10f, 40f);

        //    // used for debug spiral
        //    //_walkableNodesNearPlayer = TrinityGrid.FindNodes(CacheManager.Me, node => !node.Flags.HasFlag(WeightFlags.NavigationImpairing) && node.WeightPct <= 0.1, 30f,10f );
        //    //_nodes = TrinityGrid.FindSafeNodes(10f, 40f);
        //}

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

                    //if (VisibilityFlags.HasFlag(RadarVisibilityFlags.RadarDebug))
                    //{
                    DrawClickRays(dc, CanvasData);

                    DrawDeathGates(dc, CanvasData);

                    DrawMarkers(dc, CanvasData);

                    //DrawMinimap(dc, CanvasData);

                    //}

                    //if (VisibilityFlags.HasFlag(RadarVisibilityFlags.Clusters))
                    //{
                    //    DrawClusters(dc, CanvasData);
                    //}

                    if (VisibilityFlags.HasFlag(RadarVisibilityFlags.KiteDirection))
                    {
                        DrawKiteDirection(dc, CanvasData);
                    }

                    //if (VisibilityFlags.HasFlag(RadarVisibilityFlags.SafeNodes))
                    //{
                    //    DrawSafeNodes(dc, CanvasData);
                    //}

                    //if (VisibilityFlags.HasFlag(RadarVisibilityFlags.KiteNodes))
                    //{
                    //    DrawKiteNodes(dc, CanvasData);
                    //}
                    //if (VisibilityFlags.HasFlag(RadarVisibilityFlags.KiteFromNodes))
                    //{
                    //    DrawKiteFromNodes(dc, CanvasData);
                    //}

                    //if (VisibilityFlags.HasFlag(RadarVisibilityFlags.NotInCache))
                    //{

                    //}

                    //if (VisibilityFlags.HasFlag(RadarVisibilityFlags.CombatRadius))
                    //{
                    //    DrawCombatRadius(dc, CanvasData);
                    //}

                    //if (VisibilityFlags.HasFlag(RadarVisibilityFlags.UnwalkableNodes))
                    //{
                    //    DrawObstacles(dc, CanvasData);
                    //}

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



                    //DrawLineToSafeNode(dc, CanvasData);

                    //if (VisibilityFlags.HasFlag(RadarVisibilityFlags.RadarDebug))
                    //{
                    //    DrawDebugVisuals(dc, CanvasData);
                    //}


                }
                catch (Exception ex)
                {
                    Logger.Log("Exception in RadarUI.OnRender(). {0} {1}", ex.Message, ex.InnerException);
                }
            }
        }

        private void DrawMinimap(DrawingContext dc, CanvasData canvasData)
        {
            foreach (var item in Core.Minimap.MinimapIcons)
            {
                var radius = 5 * GridSize;
                dc.DrawEllipse(PinkBrush, null, item.Position.ToCanvasPoint(), radius, radius);
            }
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

        private static DrawingGroup GetRectangle(RectangularRegion enterRegion)
        {
            var drawing = new DrawingGroup();
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

            //BitmapImage img = new BitmapImage(new Uri("c:\\demo.jpg"));
            //dc.DrawImage(img, new Rect(0, 0, img.PixelWidth, img.PixelHeight));

            //foreach (var zone in DeathGates.Zones.Where(z => z.PortalScene != null))
            //{
            //    dc.DrawLine(RadarResources.SuccessPen, zone.EnterPosition.ToCanvasPoint(), zone.ExitPosition.ToCanvasPoint());
            //}      
            foreach (var marker in Core.Markers.CurrentWorldMarkers)
            {
                var markerPoint = marker.Position.ToCanvasPoint();
                dc.DrawLine(RadarResources.MarkerPen, CenterActor.Point, markerPoint);
                DrawLabel(dc, CanvasData, marker.Name, markerPoint, OrangeBrush, 45, 12, 3);
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

        private void DrawCombatRadius(DrawingContext dc, CanvasData canvasData)
        {
            //try
            //{
            //    var combatRadius = Core.Settings.Combat.Misc.NonEliteRange;
            //    var radius = combatRadius * GridSize;
            //    dc.DrawEllipse(null, RadarResources.GreyPen, Player.Point, radius, radius);

            //    dc.DrawText(new FormattedText($"{combatRadius}", CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.LightYellow),
            //        new Point(Player.Point.X, Player.Point.Y - (radius + 20)));
            //}
            //catch (Exception ex)
            //{
            //    Logger.Log("Exception in RadarUI.DrawCurrentpath(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            //}
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

                    dc.DrawLine(RadarResources.CurrentPathPen1, from.ToCanvasPoint(), to.ToCanvasPoint());
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.DrawCurrentpath(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }



        private PathGeometry _lastClusterGeom;
        private DateTime _lastClusterGeomCreationTime = DateTime.MinValue;

        private void DrawClusters(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                if (ValueCluster != null && ValueCluster.Any())
                {

                    var points = GrahamScan.GrahamScanCompute(ValueCluster);

                    dc.DrawPolygon(points.Select(p => p.ToCanvasPoint()), RadarResources.TransparentBrush, RadarResources.WhiteDashPen);

                    var center = ValueClusterCenter.ToCanvasPoint();

                    dc.DrawEllipse(BlackBrush, null, center, 5, 5);

                    var text = $"{ClusterValue:0.00}%";

                    DrawLabel(dc, canvas, text, center, YellowBrush);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.DrawClusters(). {0} {1} {2}",
                    ex.Message, ex.InnerException, ex);
            }
        }
        
        private void DrawTargetting(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                var target = Combat.Targeting.CurrentTarget;
                if (target == null)
                    return;

                var targetPosition = target.Position;
                if (targetPosition == Vector3.Zero)
                    return;

                var targetPoint = targetPosition.ToCanvasPoint();
                dc.DrawEllipse(PinkBrush, null, targetPoint, GridSize, GridSize);
                dc.DrawLine(PinkPen, Player.Point, targetPoint);

                var power = Combat.Targeting.CurrentPower;
                if (power == null)
                    return;

                Point spellPoint;

                if (power.TargetAcdId == -1 && power.TargetPosition != Vector3.Zero)
                {
                    spellPoint = power.TargetPosition.ToCanvasPoint();                    
                }
                else
                {
                    var spellTarget = Core.Actors.GetActorByAcdId<TrinityActor>(power.TargetAcdId);
                    if (spellTarget == null)
                        return;

                    spellPoint = spellTarget.Position.ToCanvasPoint();
                }

                dc.DrawEllipse(DeepPinkBrush, null, spellPoint, GridSize, GridSize);
                dc.DrawLine(DeepPinkPen, Player.Point, spellPoint);
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.DrawAvoidanceGrid(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private void DrawExploreNodes(DrawingContext dc, CanvasData canvas)
        {
            try
            {

                foreach (var node in _explorationNodes)
                {
                    var color = node.IsVisited ? PinkBrush : node.IsBlacklisted ? BlackBrush : node.HasEnoughNavigableCells ? GreenBrush : null;
                    if (color != null)
                    {
                        dc.DrawEllipse(color, null, node.NavigableCenter.ToCanvasPoint(), 5, 5);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.DrawAvoidanceGrid(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private void DrawKiteNodes(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                if (!Core.Avoidance.GridEnricher.KiteNodeLayer.Any())
                    return;

                foreach (var pos in Core.Avoidance.GridEnricher.KiteNodeLayer)
                {
                    dc.DrawEllipse(null, new Pen(LineGreenBrush, 1 * Scale), pos.NavigableCenter.ToCanvasPoint(), 4 * Scale, 4 * Scale);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.DrawKiteNodes(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private void DrawKiteFromNodes(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                if (!Core.Avoidance.GridEnricher.KiteFromLayer.Any())
                    return;

                foreach (var pos in Core.Avoidance.GridEnricher.KiteFromLayer)
                {
                    dc.DrawEllipse(null, new Pen(RadarResources.LabelBrush, 1 * Scale), pos.NavigableCenter.ToCanvasPoint(), 4 * Scale, 4 * Scale);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.DrawKiteFromNodes(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private void DrawSafeNodes(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                Vector3 position;

                foreach (var node in Core.Avoidance.GridEnricher.SafeNodeLayer)
                {
                    dc.DrawEllipse(BlackBrush, null, node.NavigableCenter.ToCanvasPoint(), 3 * Scale, 3 * Scale);
                }

                if (Core.Avoidance.Avoider.TryGetSafeSpot(out position))
                {
                    dc.DrawEllipse(BlueBrush, null, position.ToCanvasPoint(), 4 * Scale, 4 * Scale);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.DrawAvoidanceGrid(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
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
                Logger.LogVerbose("Exception in RadarUI.DrawAvoidanceGrid(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        // I dont know what they're doing in the Brushes resource but its probably returning a new object from a color.
        // This is bad, very bad for performance. Use a fixed resource like these brushes below.
        internal static readonly SolidColorBrush BlueBrush = Brushes.Blue;
        internal static readonly SolidColorBrush SkyBlueBrush = Brushes.DeepSkyBlue;
        internal static readonly SolidColorBrush OrangeRedBrush = Brushes.OrangeRed;
        internal static readonly SolidColorBrush RedBrush = Brushes.DarkRed;
        internal static readonly SolidColorBrush GreenBrush = Brushes.Green;
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

            //if (!node.AvoidanceFlags.HasFlag(AvoidanceFlags.AllowWalk) && VisibilityFlags.HasFlag(RadarVisibilityFlags.UnwalkableNodes))
            //{
            //    dc.DrawEllipse(BlackBrush, null, node.NavigableCenter.ToCanvasPoint(), size, size);
            //}
        }

        ///// <summary>
        ///// Draws a line between unit actors and the nearest avoidance node
        ///// </summary>
        ///// <param name="dc"></param>
        ///// <param name="canvas"></param>
        //private void DrawNearestAvoidanceNodes(DrawingContext dc, CanvasData canvas)
        //{
        //    try
        //    {
        //        foreach (var unit in CacheManager.Units)
        //        {
        //            var nearestnode = unit.NearestNode;
        //            if (nearestnode != null && nearestnode.Distance < TrinityGrid.MaxRange)
        //            {
        //                dc.DrawLine(RadarResources.LineOfSightPen, nearestnode.Position.ToCanvasPoint(), unit.Position.ToCanvasPoint());
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log("Exception in RadarUI.DrawNearestAvoidanceNodes(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
        //    }
        //}

        private void DrawLineTo(DrawingContext dc, CanvasData canvas, Vector3 position, Pen pen = null)
        {
            try
            {
                if (position == Vector3.Zero)
                    return;

                if (pen == null)
                {
                    pen = RadarResources.HighlightPen;
                }
                dc.DrawLine(pen, canvas.Center, position.ToCanvasPoint());
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.DrawNearestAvoidanceNodes(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        ///// <summary>
        ///// Draws shows all the safenodes near the player
        ///// </summary>
        ///// <param name="dc"></param>
        ///// <param name="canvas"></param>
        //private void DrawSafeNodes(DrawingContext dc, CanvasData canvas)
        //{
        //    try
        //    {
        //        //Logger.Log("TelegraphBeamAvoidance");

        //        //TrinityGrid.TelegraphBeamAvoidance(CacheManager.Me.Position, CacheManager.Me.Movement.GetHeadingDegreesFromMemory(), 50f);

        //        //Logger.Log("Drawing FindSafeNodes");

        //        //var nodes = TrinityGrid.FindSafeNodes(10f, 40f);

        //        // Spiral Node Finder Test
        //        for (int i = 0; i < _nodes.Count; i++)
        //        {
        //            var from = i == 0 ? CacheManager.Me.NearestNode : _nodes[i - 1];
        //            var to = _nodes[i];

        //            if (from == null || to == null)
        //                continue;

        //            dc.DrawLine(RadarResources.LineOfSightLightPen, from.Position.ToCanvasPoint(), to.Position.ToCanvasPoint());
        //            dc.DrawEllipse(RadarResources.SafeBrush, null, to.Position.ToCanvasPoint(), 4, 4);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log("Exception in RadarUI.DrawSpiralTest(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
        //    }
        //}

        private Dictionary<DateTime, StaticTelegraph> _telegraphedNodes = new Dictionary<DateTime, StaticTelegraph>();
        private AvoidanceNode _testkiteNode;

        public class StaticTelegraph
        {
            public Vector3 Origin;
            public Vector3 Target;
            //public float Distance;
            //public double DirectionRadian;
            //public double DirectionDegree;
            //public Vector3 MaxCastPosition
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
                Logger.Log("Exception in RadarUI.DrawKiteDirection(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        /// <summary>
        /// Draw stuff for testing
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="canvas"></param>
        private void DrawTest(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                //var centroid = PositionCache.GetCentroid;
                //if (centroid == Vector3.Zero)
                //    return;

                //var anchorPos = PositionCache.GetCentroid.ToCanvasPoint();
                //dc.DrawEllipse(Brushes.Black, null, anchorPos, 8, 8);
                //dc.DrawLine(new Pen(Brushes.Black, 3), CacheManager.Me.Position.ToCanvasPoint(), anchorPos);

                //var poisonEnchantedObjects = CacheManager.Avoidances.Where(a => a.AvoidanceType == AvoidanceType.PoisonEnchanted).GroupBy(a => a.CreationTime);

                //foreach (var enchanted in poisonEnchantedObjects)
                //{
                //    if (poisonEnchantedObjects.Where(o => o.NearbyActors(o.ActorSnoId, 20f) > 0)
                //    {
                //        o.Position.Round(10);
                //    })) ;
                //}

                //foreach (var avoidanceGroup in CacheManager.Avoidances
                //    .Where(a => a.AvoidanceType == AvoidanceType.PoisonEnchanted)
                //    .GroupBy(a => a.CreationTime))
                //{
                //    Logger.Log("Group Detected {0}", avoidanceGroup.Key);
                //}

                //foreach (var avoidanceGroup in CacheManager.Avoidances.Where(a => a.ActorSnoId == 226808).GroupBy(a => a.CreationTime))
                //{
                //    Vector3 origin;
                //    Vector3 target;

                //    if (!_telegraphedNodes.ContainsKey(avoidanceGroup.Key))
                //    {
                //        // new 
                //        var caster = CacheManager.EliteRareUniqueBoss.OrderBy(m => m.Position.Distance(CacheManager.Me.Position)).FirstOrDefault(m => m.MonsterAffixes.Contains(TrinityMonsterAffix.Waller));
                //        if (caster == null)
                //            continue;

                //        Logger.Log("SummonerId={0}",caster.SummonerId);

                //        origin = caster.Position;
                //        target = CacheManager.Me.Position;

                //        _telegraphedNodes.Add(avoidanceGroup.Key, new StaticTelegraph
                //        {
                //            Origin = origin,
                //            Target = target
                //        });

                //    }
                //    else
                //    {
                //        // existing
                //        var cached = _telegraphedNodes[avoidanceGroup.Key];
                //        origin = cached.Origin;
                //        target = cached.Target;
                //    }

                //    var wallgroup = avoidanceGroup.OrderByDescending(a => a.Position.Distance(origin)).ToList();
                //    if (!wallgroup.Any())
                //        continue;


                //        var furtherestAwayWall = wallgroup.First();
                //        TrinityObject endWall = null;

                //        //dc.DrawLine(RadarResources.CurrentPathPen, origin.ToCanvasPoint(), endWall.Position.ToCanvasPoint());

                //        var distance = origin.Distance(target);
                //        var directionRadian = MathUtil.FindDirectionRadian(origin, target);
                //        var directionDegree = MathUtil.RadianToDegree(directionRadian);
                //        var halfPI = Math.PI/2;
                //        var effectiveRadius = (furtherestAwayWall.Radius / 2.5f) * 2;

                //    if (wallgroup.Count > 4)
                //    {
                //        endWall = wallgroup.First();
                //        var endWallStart = MathEx.GetPointAt(endWall.Position, effectiveRadius, MathUtil.WrapAngle((float) (directionRadian - Math.PI - halfPI)));
                //        var endWallEnd = MathEx.GetPointAt(endWall.Position, effectiveRadius, (float) (directionRadian - halfPI));
                //        dc.DrawLine(RadarResources.CurrentPathPen, endWallStart.ToCanvasPoint(), endWallEnd.ToCanvasPoint());                        
                //    }

                //    //var le = new LineEquation(endWallStart.ToPoint(), endWallEnd.ToPoint());                        

                //        foreach (var sideWall in wallgroup)
                //        {
                //            if (endWall != null && sideWall.Equals(endWall))
                //                continue;

                //            var start = MathEx.GetPointAt(sideWall.Position, effectiveRadius, MathUtil.WrapAngle((float)(directionRadian - Math.PI)));
                //            var end = MathEx.GetPointAt(sideWall.Position, effectiveRadius, (float)directionRadian);

                //            //var le2 = new LineEquation(start.ToPoint(), end.ToPoint());                            
                //            //Point intersection;
                //            //if (le2.IntersectsWithLine(le, out intersection))
                //            //{
                //            //    end = intersection.ToVector3();
                //            //    if (end.Distance(endWallStart) < end.Distance(endWallEnd))
                //            //        endWallStart = end;
                //            //    else
                //            //        endWallEnd = end;
                //            //}                                

                //            dc.DrawLine(RadarResources.CurrentPathPen, start.ToCanvasPoint(), end.ToCanvasPoint());
                //        }

                //}

            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.DrawHeadingTest(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }


        ///// <summary>
        ///// Draws a line between unit actors and the nearest avoidance node
        ///// </summary>
        ///// <param name="dc"></param>
        ///// <param name="canvas"></param>
        private void DrawLineToSafeNode(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                //var safestNode = Core.Avoidance.KiteNodes.FirstOrDefault();
                //if(safestNode != null)
                //    dc.DrawLine(RadarResources.LineOfSightPen, canvas.Center, safestNode.NavigableCenter.ToCanvasPoint());
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.DrawTest(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        //private void DrawActualObjectBounds(DrawingContext dc, CanvasData canvas, TrinityObject trinityObject)
        //{
        //    try
        //    {
        //        //var worldWidth = trinityObject.Object.ActorInfo.AxialCylinder.Ax1;
        //        //var worldHeight = trinityObject.Object.ActorInfo.AxialCylinder.Ax2;
        //        //var canvasWidth = worldWidth * GridSize;
        //        //var canvasHeight = worldHeight * GridSize;
        //        var worldPosition = new Vector3(trinityObject.Position.X, trinityObject.Position.Y, 0);
        //        var point = worldPosition.ToCanvasPoint();
        //        //var rect = new Rect(point, new Size(canvasWidth, canvasHeight));
        //        //var angle = 360-(trinityObject.RotationDegrees - canvas.GobalRotation);

        //        //rect.Offset(-canvasWidth / 2, -canvasHeight / 2);

        //        //var rotationTransform = new RotateTransform(angle, point.X, point.Y);
        //        ////var topLeft = rotationTransform.Transform(rect.TopLeft);
        //        ////var topRight = rotationTransform.Transform(rect.TopRight);
        //        ////var bottomLeft = rotationTransform.Transform(rect.BottomLeft);
        //        ////var bottomRight = rotationTransform.Transform(rect.BottomRight);


        //        //var testRect = RectangleF.(left, top, right, bottom);
        //        //testRect.Offset(-10, -10);

        //        //dc.DrawRectangle(Brushes.Tomato, null, rect.);

        //        var shape = trinityObject.Shape;
        //        shape.Figures.ForEach(f => f.);


        //        //figures.Add(new PathFigure(bottomRight, segments, true));
        //        var geo = new PathGeometry(shape.Figures, FillRule.Nonzero, null);
        //        var transform = new ScaleTransform(10,10);
        //        transform.Transform(geo);
        //        geo.Bounds.Offset();
        //        dc.DrawGeometry(Brushes.Yellow, null, geo); 
        //        //geo.GetOutlinedPathGeometry();


        //        //var transforms = new TransformGroup();
        //        //transforms.Children.Add(new ScaleTransform(10, 10));
        //        //transforms.Children.Add(canvas.);

        //        //shape.Transform = transforms;
        //        //DrawRotatedRectangle(dc, canvas, topLeft, bottomLeft, bottomRight, topRight);

        //        dc.DrawGeometry(Brushes.GreenYellow, null, shape);

        //        dc.DrawEllipse(Brushes.Tomato, null, point, 4, 4);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log("Exception in RadarUI.DrawActualObjectBounds(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
        //    }
        //}

        //private void DrawActualObjectBounds(DrawingContext dc, CanvasData canvas, TrinityObject trinityObject)
        //{
        //    try
        //    {
        //        var worldWidth = trinityObject.Object.ActorInfo.AxialCylinder.Ax1;
        //        var worldHeight = trinityObject.Object.ActorInfo.AxialCylinder.Ax2;
        //        var canvasWidth = worldWidth * GridSize;
        //        var canvasHeight = worldHeight * GridSize;
        //        var worldPosition = new Vector3(trinityObject.Position.X, trinityObject.Position.Y, 0);
        //        var point = worldPosition.ToCanvasPoint();
        //        var rect = new Rect(point, new Size(canvasWidth, canvasHeight));
        //        var angle = 360-(trinityObject.RotationDegrees - canvas.GobalRotation);

        //        rect.Offset(-canvasWidth / 2, -canvasHeight / 2);

        //        var rotationTransform = new RotateTransform(angle, point.X, point.Y);
        //        var topLeft = rotationTransform.Transform(rect.TopLeft);
        //        var topRight = rotationTransform.Transform(rect.TopRight);
        //        var bottomLeft = rotationTransform.Transform(rect.BottomLeft);
        //        var bottomRight = rotationTransform.Transform(rect.BottomRight);


        //        //var testRect = RectangleF.(left, top, right, bottom);
        //        //testRect.Offset(-10, -10);

        //        //dc.DrawRectangle(Brushes.Tomato, null, rect.);

        //        //MathUtil.GetRotatedPathGeometry();
        //        DrawRotatedRectangle(dc, canvas, topLeft, bottomLeft, bottomRight, topRight);

        //        dc.DrawEllipse(Brushes.Tomato, null, point, 4, 4);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log("Exception in RadarUI.DrawActualObjectBounds(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
        //    }
        //}

        //private void DrawActualObjectBounds(DrawingContext dc, CanvasData canvas, TrinityObject trinityObject)
        //{
        //    try
        //    {
        //        //var rect = trinityObject.ActorInfo.AABBBounds.ToCanvasRect();
        //        var rect = trinityObject.ActorInfo.AABBBounds.ToRelativeCanvasRect(trinityObject.Position);

        //        //var bounds = trinityObject.InnerBounds;
        //        var topLeft = rect.TopLeft.ToCanvasPoint();
        //        var topRight = rect.TopRight.ToCanvasPoint();
        //        var bottomLeft = rect.BottomLeft.ToCanvasPoint();
        //        var bottomRight = rect.BottomRight.ToCanvasPoint();

        //        DrawRectangle(dc, canvas, topLeft, bottomLeft, bottomRight, topRight);
        //        //DrawPolygon(dc, canvas, bounds.Select(p => p.ToCanvasPoint()));
        //        //DrawPolygon
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log("Exception in RadarUI.DrawActualObjectBounds(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
        //    }
        //}

        private void DrawNodeArea(DrawingContext dc, CanvasData canvas, AvoidanceNode node, Brush brush, Pen pen = null)
        {
            try
            {
                if (node == null)
                    return;

                if (brush == null)
                    brush = YellowBrush;

                dc.DrawEllipse(brush, null, node.NavigableCenter.ToCanvasPoint(), 4 * Scale, 4 * Scale);

                //var id = "Node_" + node.Center.X + node.Center.Y;

                //if (!Drawings.Relative.ContainsKey(id))
                //{
                //    var drawing = new DrawingGroup();

                //    if (brush == null)
                //        brush = Brushes.Yellow;

                //    using (var groupdc = drawing.Open())
                //    {
                //        var figures = new List<PathFigure>();

                //        var segments = new[]
                //        {
                //            new LineSegment(node.BottomLeft.ToCanvasPoint(), true),
                //            new LineSegment(node.TopLeft.ToCanvasPoint(), true),
                //            new LineSegment(node.TopRight.ToCanvasPoint(), true),
                //        };

                //        figures.Add(new PathFigure(node.BottomRight.ToCanvasPoint(), segments, true));
                //        var geo = new PathGeometry(figures, FillRule.Nonzero, null);
                //        groupdc.DrawGeometry(brush, pen, geo);
                //    }

                //    var d = new RelativeDrawing
                //    {
                //        Drawing = drawing,
                //        Origin = CenterActor.Morph,
                //        Center = node.Center,
                //        WorldId = CacheManager.Me.WorldId,
                //        Type = DrawingType.NodeArea
                //    };

                //    Drawings.Relative.TryAdd(id, d);
                //    dc.DrawDrawing(d.Drawing);
                //}
                //else
                //{       
                //    dc.DrawDrawing(Drawings.Relative[id].Drawing);
                //}
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.DrawRotatedRectangle(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
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
                Logger.Log("Exception in RadarUI.DrawRotatedRectangle(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }



        //https://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
        //int pnpoly(int nvert, float[] vertx, float[] verty, float testx, float testy)
        //{
        //    int i, j, c = 0;
        //    for (i = 0, j = nvert - 1; i < nvert; j = i++)
        //    {
        //        if (((verty[i] > testy) != (verty[j] > testy)) &&
        //         (testx < (vertx[j] - vertx[i]) * (testy - verty[i]) / (verty[j] - verty[i]) + vertx[i]))
        //            c = ~c;
        //    }
        //    return c;
        //}

        //public List<Vector3> GetOutlineEdges(List<Vector3> points)
        //{
        //    var output = new List<Point>();

        //    output = GrahamScan.GrahamScanCompute(points);

        //    //float min = 0;
        //    //float max = 0;
        //    //Polygon.
        //    //for (int i = 0; i < points.Count; i++)
        //    //{
        //    //    var point = points.ElementAt(i);                
        //    //    if(point.)
        //    //}
        //    //var groupByMultiplicity = points.GroupBy(p => p, new PointComparer());

        //    //var output = groupByMultiplicity.Where(grp => grp.Count() == 1).Select(grp => grp.Key).ToList();

        //    return output;
        //}

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

        //public List<Point> GetOutlineEdges(IEnumerable<Point> points)
        //{
        //    var groupByMultiplicity = points.GroupBy(p => p, new PointComparer());

        //    var output = groupByMultiplicity.Where(grp => grp.Count() == 1).Select(grp => grp.Key).ToList();

        //    return output;
        //}

        //public class PointComparer : IEqualityComparer<Point>
        //{
        //    public bool Equals(Point a, Point b)
        //    {
        //        var aX = (int)a.X;
        //        var aY = (int)a.Y;
        //        var bX = (int)b.Y;
        //        var bY = (int)b.Y;
        //        bool r1 = (aX == bX && aY == bY);
        //        bool r2 = (aX == bY && aX == bY);
        //        bool r3 = (aX < bX && aY < bY);
        //        bool r4 = (aX > bX && aY > bY);
        //        return r1 || r2 || r3 || r4;
        //    }

        //    public int GetHashCode(Point obj)
        //    {
        //        return 0;
        //    }
        //}

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

                foreach (var adventurerScene in ScenesStorage.CurrentWorldScenes.Where(s => s.DynamicWorldId == worldId).ToList())
                {

                    // Combine navcells into one drawing and store it; because they don't change relative to each other
                    // And because translating geometry for every navcell on every frame is waaaaay too slow.
                    if (Drawings.Relative.ContainsKey(adventurerScene.HashName))
                        continue;

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

                        #region Scene Title

                        if (sceneborders)
                        {
                            var textPoint = adventurerScene.Center.ToVector3().ToCanvasPoint();
                            var glyphRun = DrawingUtilities.CreateGlyphRun(adventurerScene.Name, 10, textPoint);
                            groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun);
                            textPoint = adventurerScene.Center.ToVector3().ToCanvasPoint();
                            textPoint.Y = textPoint.Y + 20;
                            glyphRun = DrawingUtilities.CreateGlyphRun((adventurerScene.Max - adventurerScene.Min) + " " + (adventurerScene.HasChild ? "HasSubScene" : string.Empty) + " " + (adventurerScene.SubScene != null ? " (Loaded)" : string.Empty), 8, textPoint);
                            groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun);
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
                Logger.Log("Exception in RadarUI.DrawScenes(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
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

        //public class AnimationCounter
        //{
        //    private Stopwatch _sw = new Stopwatch();

        //    private System.Threading.Timer AutoRunTimer;

        //    public int Max { get; }

        //    public int Value { get { return _sw.ElapsedMilliseconds; } }

        //    public AnimationCounter(int max)
        //    {
        //        Max = max;
        //        AutoRunTimer = new System.Threading.Timer(tick, "", 
        //            TimeSpan.FromMilliseconds(250), 
        //            TimeSpan.FromMilliseconds(250));
        //    }

        //    private void tick(object data)
        //    {
        //        if(_sw.ElapsedMilliseconds > Max)
        //    }
        //}

        //private AnimationCounter SelectedAnimationCounter = new AnimationCounter(360);

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
                var brush = RadarResources.GreyBrush;
                brush.Opacity = 0.75;
                var actorRadius = Math.Min(30, Math.Max(4, actor.CollisionRadius * GridSize));
                var radarObject = new RadarObject(actor, canvas);
                dc.DrawEllipse(brush, null, radarObject.Point, actorRadius, actorRadius);
                HitTester.AddEllipse(radarObject, radarObject.Point, actorRadius, actorRadius);
            }

        }

        private void DrawDebugVisuals(DrawingContext dc, CanvasData canvas)
        {
            RadarDebug.Render(dc, canvas);
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
                    res.Brush.Opacity = 0.15;

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
                Logger.Log("Exception in RadarUI.DrawActor(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
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


        //private void DrawGrid(DrawingContext dc, CanvasData canvas, int gridLineFrequency)
        //{
        //    StaticDrawing sd;

        //    if (!Drawings.Static.TryGetValue("Grid", out sd))
        //    {
        //        var drawing = new DrawingGroup();

        //        using (var groupdc = drawing.Open())
        //        {
        //            // vertical lines
        //            int pos = 0;
        //            do
        //            {
        //                groupdc.DrawLine(RadarResources.GridPen, new Point(pos, 0), new Point(pos, (int)canvas.CanvasSize.Height));
        //                pos += (int)canvas.GridSquareSize.Height * gridLineFrequency;

        //            } while (pos < canvas.CanvasSize.Width);

        //            // horizontal lines
        //            pos = 0;
        //            do
        //            {
        //                groupdc.DrawLine(RadarResources.GridPen, new Point(0, pos), new Point((int)canvas.CanvasSize.Width, pos));
        //                pos += (int)canvas.GridSquareSize.Width * gridLineFrequency;

        //            } while (pos < canvas.CanvasSize.Height);
        //        }

        //        drawing.Freeze();

        //        Drawings.Static.TryAdd("Grid", new StaticDrawing
        //        {
        //            Drawing = drawing,
        //            WorldId = CacheManager.WorldId
        //        });
        //    }
        //    else
        //    {
        //        dc.DrawDrawing(sd.Drawing);
        //    }
        //}

    }







}
