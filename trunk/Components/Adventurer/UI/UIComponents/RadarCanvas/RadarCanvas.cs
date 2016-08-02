using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Coroutines;
using Trinity.Components.Adventurer.Game.Events;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Util;
using Zeta.Bot.Navigation;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.SNO;
//using Adventurer.Game.Grid;
using LineSegment = System.Windows.Media.LineSegment;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.UI.UIComponents.RadarCanvas
{

    public class RadarCanvas : Canvas
    {
        public RadarCanvas()
        {
            CacheMode = new BitmapCache();
            ClipToBounds = true;

            CanvasData.OnCanvasSizeChanged += (before, after) =>
            {
                // Scene drawings are specific to a canvas size.
                // Changing canvas size means we have to redraw them all.
                Drawings.Relative.Clear();
                Drawings.Static.Clear();
                Clip = CanvasData.ClipRegion;
            };
        }

        /// <summary>
        /// The canvas size of grid squares (in pixels) for 1yd of game distance
        /// </summary>
        public int GridSize = 2;

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
        public void UpdateData()
        {
            try
            {
                using (new PerformanceLogger("RadarUI UpdateData"))
                {
                    if (DesiredSize.Height <= 0 || DesiredSize.Width <= 0)
                        return;

                    if (!IsVisible || ZetaDia.Me == null || ZetaDia.IsLoadingWorld || !ZetaDia.IsInGame)
                        return;

                    Objects.Clear();

                    CanvasData.Update(DesiredSize, GridSize);

                    // Find the actor who should be in the center of the radar
                    // and whos position all other points should be plotted against.

                    using (ZetaDia.Memory.AcquireFrame())
                    {                        
                        var center = ZetaDia.Me;
                        if (center == null)
                            return;

                        CenterActor = new RadarObject()
                        {
                            CachedActorName = "Player",
                            CachedActorRadius = 6,
                            CachedActorPosition = AdvDia.MyPosition,                            
                        };

                        CenterActor.Morph.CanvasData = CanvasData;
                        CenterActor.Morph.Update(AdvDia.MyPosition);                 
                        CanvasData.CenterVector = CenterActor.CachedActorPosition;
                        CanvasData.CenterMorph = CenterActor.Morph;
                        UpdateRelativeDrawings();
                     

                        UpdateGridNodesData();
                        UpdateSceneData();
                    }
                }

                // Trigger Canvas to Render
                InvalidateVisual();

            }
            catch (Exception ex)
            {
                Logger.Debug("Exception in RadarUI.UpdateData(). {0} {1}", ex.Message, ex.InnerException);
            }
        }

        private void UpdateSceneData()
        {
            Scenes = ScenesStorage.CurrentWorldScenes.ToList();
        }

        public List<WorldScene> Scenes = new IndexedList<WorldScene>();

        private void UpdateGridNodesData()
        {
            if (!BotEvents.IsBotRunning)
                return;

            if (ZetaDia.IsLoadingWorld)
                return;

            try
            {
                var navigableNodes = new List<ExplorationNode>();
                var instance = ExplorationGrid.Instance;
                var minX = instance.MinX;
                var maxX = instance.MaxX;
                var minY = instance.MinY;
                var maxY = instance.MaxY;


                if (maxX <= 0 && maxY <= 0)
                    return;

                for (var x = minX; x <= maxX; x++)
                {
                    for (var y = minY; y <= maxY; y++)
                    {
                        var node = ExplorationGrid.Instance.InnerGrid[x, y];
                        if (node != null && node.HasEnoughNavigableCells)
                        {
                            navigableNodes.Add(node);
                        }
                    }
                }
                NavigableNodes = navigableNodes;
            }
            catch (Exception ex)
            {
                Logger.Error($"RadarCanvas Exception, UpdateGridNodesData: {ex}");
            }

        }

        public List<ExplorationNode> NavigableNodes = new IndexedList<ExplorationNode>();

        /// <summary>
        /// Update relative drawings. Calculates new position for each scene drawing based on its origin point.
        /// </summary>
        private void UpdateRelativeDrawings()
        {
            try
            {
                var keysToRemove = new List<string>();
                foreach (var drawing in Drawings.Relative)
                {
                    if (drawing.Value.WorldId != AdvDia.CurrentWorldDynamicId)
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
            catch (Exception ex)
            {
                Logger.Debug("Exception in RadarUI.UpdateRelativeDrawings(). {0} {1}", ex.Message, ex.InnerException);
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

                //if (Math.Abs(CanvasData.CanvasSize.Width) < 0.001f && Math.Abs(CanvasData.CanvasSize.Height) < 0.001f || CanvasData.CenterVector == Vector3.Zero)
                //    return;

                //if (Math.Abs(CenterActor.Point.X) < 0.001f && Math.Abs(CenterActor.Point.Y) < 0.001f)
                //    return;

                //if (!ZetaDia.IsInGame || ZetaDia.IsLoadingWorld)
                //    return;

                try
                {
                    DrawScenes(dc, CanvasData);
                    DrawGridNodes(dc, CanvasData);
                    DrawPointsOfInterest(dc, CanvasData);
                    DrawCurrentPath(dc, CanvasData);
                    DrawNavigation(dc, CanvasData);
                    DrawActor(dc, CanvasData, CenterActor);             
                }
                catch (Exception ex)
                {
                    Logger.Debug("Exception in RadarUI.OnRender(). {0} {1}", ex.Message, ex.InnerException);
                }
            }
        }

        private void DrawPointsOfInterest(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                foreach (var marker in AdvDia.CurrentWorldMarkers.Where(m => m.Id >= 0 && m.Id < 200))
                {
                    dc.DrawEllipse(Brushes.Yellow, RadarResources.GridPen, marker.Position.ToCanvasPoint(), 5, 5);
                    var textPoint = marker.Position.ToCanvasPoint();
                    textPoint.Y = textPoint.Y + 32;
                    var glyphRun = DrawingUtilities.CreateGlyphRun(marker.NameHash.ToString(), 8, textPoint);
                    dc.DrawGlyphRun(Brushes.Yellow, glyphRun);

                    var toObjectivePen = new Pen(Brushes.Yellow, 0.2);
                    dc.DrawLine(toObjectivePen, canvas.Center, marker.Position.ToCanvasPoint());
                }

            }
            catch (Exception ex)
            {
                Logger.Debug("Exception in RadarUI.DrawPointsOfInterest(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private void DrawNavigation(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                dc.DrawEllipse(Brushes.Gold, RadarResources.GridPen, NavigationCoroutine.LastDestination.ToCanvasPoint(), 6, 6);
                var toObjectivePen = new Pen(Brushes.Gold, 0.2);
                dc.DrawLine(toObjectivePen, canvas.Center, NavigationCoroutine.LastDestination.ToCanvasPoint());
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception in RadarUI.DrawPointsOfInterest(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }


        private void DrawGridNodes(DrawingContext dc, CanvasData canvas)
        {
            var curX = 0;
            var curY = 0;
            try
            {
                if (!BotEvents.IsBotRunning || !NavigableNodes.Any()) return;
                //for (var x = ExplorationGrid.Instance.MinX; x <= ExplorationGrid.Instance.MaxX; x++)
                //{
                //    for (var y = ExplorationGrid.Instance.MinY; y <= ExplorationGrid.Instance.MaxY; y++)
                //    {
                //        var node = ExplorationGrid.Instance.InnerGrid[x, y] as ExplorationNode;
                //        if (node != null && node.HasEnoughNavigableCells)
                //        {

                foreach (var node in NavigableNodes)
                {
                    Brush nodeBrush;
                    if (node.IsIgnored)
                    {
                        nodeBrush = Brushes.Black;
                    }
                    else if (node.IsBlacklisted)
                    {
                        nodeBrush = Brushes.Red;
                    }
                    else if (node.IsVisited)
                    {
                        nodeBrush = Brushes.SpringGreen;
                    }
                    else if (node.IsCurrentDestination)
                    {
                        nodeBrush = Brushes.DeepSkyBlue;
                    }
                    else
                    {
                        nodeBrush = Brushes.DarkGray;
                    }

                    dc.DrawEllipse(nodeBrush, RadarResources.GridPen, node.NavigableCenter.ToCanvasPoint(), GridSize * 2, GridSize * 2);
                }

                //var textPoint = node.Center.ToVector3().ToCanvasPoint();
                //var nodeTitle =
                //    ((1/node.NavigableCenter.Distance(AdvDia.MyPosition))*node.UnvisitedWeight).ToString();
                //var glyphRun = DrawingUtilities.CreateGlyphRun(nodeTitle, GridSize * 4, textPoint);
                //dc.DrawGlyphRun(nodeBrush, glyphRun);
                //        }

                //    }
                //}




            }
            catch (Exception ex)
            {
                Logger.Debug("Exception in RadarUI.DrawGridNodes(). {0} {1} {2}", ex.Message, curX, curY);
            }
        }

        //private void DrawNavNode(DrawingContext dc, CanvasData canvas, GridNode node)
        //{
        //    SolidColorBrush nodeBrush;
        //    if (node.Flags.HasFlag(NavCellFlags.RoundedCorner0) || node.Flags.HasFlag(NavCellFlags.RoundedCorner1) || node.Flags.HasFlag(NavCellFlags.RoundedCorner2) || node.Flags.HasFlag(NavCellFlags.RoundedCorner3))
        //    {
        //        nodeBrush = Brushes.SaddleBrown;
        //    }
        //    else if (node.Flags.HasFlag(NavCellFlags.AllowWalk))
        //    {
        //        nodeBrush = Brushes.Green;
        //    }
        //    else if (node.Flags.HasFlag(NavCellFlags.AllowProjectile))
        //    {
        //        nodeBrush = Brushes.DarkGray;
        //    }
        //    else
        //    {
        //        nodeBrush = Brushes.Black;
        //    }

        //    dc.DrawEllipse(nodeBrush, RadarResources.GridPen, node.NavigableCenter.ToCanvasPoint(), GridSize, GridSize);
        //}

        //private void DrawNodesInRadius(DrawingContext dc, CanvasData canvas)
        //{
        //    try
        //    {
        //        var nearestNode = MainGrid.Instance.GetNearestNodeToPosition(AdvDia.MyPosition);
        //        if (nearestNode == null) return;
        //        var nodes = MainGrid.Instance.GetNeighbors(nearestNode, 5);
        //        foreach (var node in nodes)
        //        {
        //            DrawNavNode(dc, canvas, node);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Debug("Exception in RadarUI.DrawNodesInRadius(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
        //    }
        //}

        /// <summary>
        /// Draw a Scene
        /// </summary>
        private void DrawScenes(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                //if (Math.Abs(CenterActor.Point.X) < 0.01f && Math.Abs(CenterActor.Point.Y) < 0.01f)
                //    return;

                var worldId = AdvDia.CurrentWorldDynamicId;
                foreach (var adventurerScene in Scenes.Where(s => s.DynamicWorldId == worldId).ToList())
                {
                    //var scene = adventurerScene.TrinityScene;

                    // Combine navcells into one drawing and store it; because they don't change relative to each other
                    // And because translating geometry for every navcell on every frame is waaaaay too slow.
                    if (Drawings.Relative.ContainsKey(adventurerScene.HashName)) continue;

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
                        var sceneTopLeft = new Vector3(adventurerScene.Min.X, adventurerScene.Min.Y, 0);
                        var sceneTopRight = new Vector3(adventurerScene.Max.X, adventurerScene.Min.Y, 0);
                        var sceneBottomLeft = new Vector3(adventurerScene.Min.X, adventurerScene.Max.Y, 0);
                        var sceneBottomRight = new Vector3(adventurerScene.Max.X, adventurerScene.Max.Y, 0);

                        groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopLeft.ToCanvasPoint(), sceneTopRight.ToCanvasPoint()));
                        groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneBottomLeft.ToCanvasPoint(), sceneBottomRight.ToCanvasPoint()));
                        groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopLeft.ToCanvasPoint(), sceneBottomLeft.ToCanvasPoint()));
                        groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopRight.ToCanvasPoint(), sceneBottomRight.ToCanvasPoint()));
                        #endregion

                        #region Scene Title
                        var textPoint = adventurerScene.Center.ToVector3().ToCanvasPoint();
                        var glyphRun = DrawingUtilities.CreateGlyphRun(adventurerScene.Name, 10, textPoint);
                        groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun);
                        textPoint = adventurerScene.Center.ToVector3().ToCanvasPoint();
                        textPoint.Y = textPoint.Y + 20;
                        glyphRun = DrawingUtilities.CreateGlyphRun($"[{adventurerScene.Min.X}, {adventurerScene.Min.Y}][{adventurerScene.Max.X}, {adventurerScene.Max.Y}]" + (adventurerScene.Max - adventurerScene.Min) + " " + (adventurerScene.HasChild ? "HasSubScene" : string.Empty) + " " + (adventurerScene.SubScene != null ? " (Loaded)" : string.Empty), 8, textPoint);
                        groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun);
                        #endregion

                        #region Nodes
                        var sceneNodePen = new Pen { Brush = Brushes.Black, Thickness = 0.1f };
                        foreach (var node in adventurerScene.Nodes)
                        {
                            if (node.HasEnoughNavigableCells)
                            {
                                groupdc.DrawGeometry(null, sceneNodePen,
                                    new LineGeometry(node.TopLeft.ToVector3().ToCanvasPoint(),
                                        node.TopRight.ToVector3().ToCanvasPoint()));
                                groupdc.DrawGeometry(null, sceneNodePen,
                                    new LineGeometry(node.TopLeft.ToVector3().ToCanvasPoint(),
                                        node.BottomLeft.ToVector3().ToCanvasPoint()));
                                groupdc.DrawGeometry(null, sceneNodePen,
                                    new LineGeometry(node.BottomLeft.ToVector3().ToCanvasPoint(),
                                        node.BottomRight.ToVector3().ToCanvasPoint()));
                                groupdc.DrawGeometry(null, sceneNodePen,
                                    new LineGeometry(node.TopRight.ToVector3().ToCanvasPoint(),
                                        node.BottomRight.ToVector3().ToCanvasPoint()));

                                //var textPoint1 = node.Center.ToVector3().ToCanvasPoint();
                                ////                                var glyphRun1 = DrawingUtilities.CreateGlyphRun(string.Format("{2} {0} ({1:P0})", node.NavigableCellCount, node.FillPercentage, node.LevelAreaSnoIdId), 8, textPoint1);
                                //var glyphRun1 = DrawingUtilities.CreateGlyphRun(node.LevelAreaSnoIdId.ToString(), 8, textPoint1);
                                //groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun1);
                            }
                            //foreach (var navPoint in node.Nodes.Where(n=>n.NodeFlags.HasFlag(NodeFlags.NearWall)))
                            //{
                            //    DrawNavNode(groupdc, canvas, navPoint);
                            //    //groupdc.DrawEllipse(Brushes.LightSlateGray, sceneNodePen, navPoint.Center.ToVector3().ToCanvasPoint(), (float)GridSize / 2, (float)GridSize / 2);
                            //}
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
                        //Image = new DrawingImage(drawing),
                        //ImageRect = drawing.Bounds

                    });
                }

                foreach (var pair in Drawings.Relative)
                {
                    //if (pair.Value.WorldId != AdvDia.WorldId)
                    //    continue;

                    if (!pair.Value.Drawing.Bounds.IntersectsWith(CanvasData.ClipRegion.Rect))
                        continue;
                    dc.DrawDrawing(pair.Value.Drawing);

                    //if (!pair.Value.ImageRect.IntersectsWith(CanvasData.ClipRegion.Rect))
                    //    continue;
                    //dc.DrawImage(pair.Value.Image, pair.Value.ImageRect);
                }

            }
            catch (Exception ex)
            {
                Logger.Debug("Exception in RadarUI.DrawScenes(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }


        private Brush GetNavCellBrush(NavCellFlags flags)
        {
            if (flags.HasFlag(NavCellFlags.AllowWalk))
            {
                return new SolidColorBrush(Colors.NavajoWhite) { Opacity = 0.2 };
            }
            if (flags.HasFlag(NavCellFlags.AllowProjectile))
            {
                return new SolidColorBrush(Colors.DarkGray) { Opacity = 0.2 };
            }
            if (flags.HasFlag(NavCellFlags.RoundedCorner0) || flags.HasFlag(NavCellFlags.RoundedCorner1) || flags.HasFlag(NavCellFlags.RoundedCorner2) || flags.HasFlag(NavCellFlags.RoundedCorner3))
            {
                return new SolidColorBrush(Colors.DarkGreen) { Opacity = 0.2 };
            }
            return new SolidColorBrush(Colors.Black);
        }

        /// <summary>
        /// Draw a Scene
        /// </summary>
        //private void DrawScenes2(DrawingContext dc, CanvasData canvas)
        //{
        //    try
        //    {
        //        if (Math.Abs(CenterActor.Point.X) < 0.01f && Math.Abs(CenterActor.Point.Y) < 0.01f)
        //            return;

        //        var worldId = AdvDia.WorldId;
        //        foreach (var adventurerScene in GridProvider.CachedScenes.Values.Where(s => s.WorldId == worldId
        //        //&&
        //        //centerActorPosition.X >= s.Min.X && centerActorPosition.Y >= s.Min.Y &&
        //        //centerActorPosition.X <= s.Max.X && centerActorPosition.Y <= s.Max.Y
        //        ).ToList())
        //        {
        //            // Combine navcells into one drawing and store it; because they don't change relative to each other
        //            // And because translating geometry for every navcell on every frame is waaaaay too slow.
        //            if (Drawings.Relative.ContainsKey(adventurerScene.NameHash)) continue;

        //            var drawing = new DrawingGroup();

        //            using (var groupdc = drawing.Open())
        //            {
        //                #region Terrain
        //                if (adventurerScene.SceneDefinition != null && adventurerScene.SceneDefinition.NavCellDefinitions != null)
        //                {
        //                    var figures = new List<PathFigure>();
        //                    foreach (var navCell in adventurerScene.SceneDefinition.NavCellDefinitions.Where(nc => nc.Flags.HasFlag(NavCellFlags.AllowWalk)))
        //                    {
        //                        var topLeft = new Vector3(adventurerScene.Min.X + navCell.Min.X, adventurerScene.Min.Y + navCell.Min.Y, 0);
        //                        var topRight = new Vector3(adventurerScene.Min.X + navCell.Max.X, adventurerScene.Min.Y + navCell.Min.Y, 0);
        //                        var bottomLeft = new Vector3(adventurerScene.Min.X + navCell.Min.X, adventurerScene.Min.Y + navCell.Max.Y, 0);
        //                        var bottomRight = new Vector3(adventurerScene.Min.X + navCell.Max.X, adventurerScene.Min.Y + navCell.Max.Y, 0);
        //                        var segments = new[]
        //                                       {
        //                                           new LineSegment(topLeft.ToCanvasPoint(), true),
        //                                           new LineSegment(topRight.ToCanvasPoint(), true),
        //                                           new LineSegment(bottomRight.ToCanvasPoint(), true)
        //                                       };

        //                        figures.Add(new PathFigure(bottomLeft.ToCanvasPoint(), segments, true));
        //                    }
        //                    var geo = new PathGeometry(figures, FillRule.Nonzero, null);
        //                    geo.GetOutlinedPathGeometry();
        //                    groupdc.DrawGeometry(RadarResources.WalkableTerrain, null, geo);
        //                }
        //                #endregion

        //                #region Scene Borders
        //                var sceneTopLeft = new Vector3(adventurerScene.Min.X, adventurerScene.Min.Y, 0);
        //                var sceneTopRight = new Vector3(adventurerScene.Max.X, adventurerScene.Min.Y, 0);
        //                var sceneBottomLeft = new Vector3(adventurerScene.Min.X, adventurerScene.Max.Y, 0);
        //                var sceneBottomRight = new Vector3(adventurerScene.Max.X, adventurerScene.Max.Y, 0);

        //                groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopLeft.ToCanvasPoint(), sceneTopRight.ToCanvasPoint()));
        //                groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneBottomLeft.ToCanvasPoint(), sceneBottomRight.ToCanvasPoint()));
        //                groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopLeft.ToCanvasPoint(), sceneBottomLeft.ToCanvasPoint()));
        //                groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopRight.ToCanvasPoint(), sceneBottomRight.ToCanvasPoint()));
        //                #endregion

        //                #region Scene Title
        //                var textPoint = adventurerScene.Center.ToVector3().ToCanvasPoint();
        //                var glyphRun = DrawingUtilities.CreateGlyphRun(adventurerScene.Name, 10, textPoint);
        //                groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun);
        //                textPoint = adventurerScene.Center.ToVector3().ToCanvasPoint();
        //                textPoint.Y = textPoint.Y + 20;
        //                glyphRun = DrawingUtilities.CreateGlyphRun((adventurerScene.Max - adventurerScene.Min).ToString(), 8, textPoint);
        //                groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun);
        //                #endregion

        //                #region Nodes
        //                //var sceneNodePen = new Pen { Brush = Brushes.Black, Thickness = 0.1f };
        //                //foreach (var cell in adventurerScene.GridCells)
        //                //{
        //                //    //if (node.HasEnoughNavigableCells)
        //                //    //{
        //                //    //    groupdc.DrawGeometry(null, sceneNodePen,
        //                //    //        new LineGeometry(node.TopLeft.ToVector3().ToCanvasPoint(),
        //                //    //            node.TopRight.ToVector3().ToCanvasPoint()));
        //                //    //    groupdc.DrawGeometry(null, sceneNodePen,
        //                //    //        new LineGeometry(node.TopLeft.ToVector3().ToCanvasPoint(),
        //                //    //            node.BottomLeft.ToVector3().ToCanvasPoint()));
        //                //    //    groupdc.DrawGeometry(null, sceneNodePen,
        //                //    //        new LineGeometry(node.BottomLeft.ToVector3().ToCanvasPoint(),
        //                //    //            node.BottomRight.ToVector3().ToCanvasPoint()));
        //                //    //    groupdc.DrawGeometry(null, sceneNodePen,
        //                //    //        new LineGeometry(node.TopRight.ToVector3().ToCanvasPoint(),
        //                //    //            node.BottomRight.ToVector3().ToCanvasPoint()));

        //                //    //    //var textPoint1 = node.Center.ToVector3().ToCanvasPoint();
        //                //    //    ////                                var glyphRun1 = DrawingUtilities.CreateGlyphRun(string.Format("{2} {0} ({1:P0})", node.NavigableCellCount, node.FillPercentage, node.LevelAreaSnoIdId), 8, textPoint1);
        //                //    //    //var glyphRun1 = DrawingUtilities.CreateGlyphRun(node.LevelAreaSnoIdId.ToString(), 8, textPoint1);
        //                //    //    //groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun1);
        //                //    //}
        //                //    //foreach (var navPoint in node.Nodes.Where(n=>n.NodeFlags.HasFlag(NodeFlags.NearWall)))
        //                //    //{
        //                //    DrawNavNode(groupdc, canvas, cell);
        //                //    //    //groupdc.DrawEllipse(Brushes.LightSlateGray, sceneNodePen, navPoint.Center.ToVector3().ToCanvasPoint(), (float)GridSize / 2, (float)GridSize / 2);
        //                //    //}
        //                //}
        //                #endregion




        //            }

        //            // Have to use SceneHash as key because scenes can appear multiple times with the same name
        //            Drawings.Relative.TryAdd(adventurerScene.NameHash, new RelativeDrawing
        //            {
        //                Drawing = drawing,
        //                Origin = CenterActor.Morph,
        //                Center = adventurerScene.Center.ToVector3(),
        //                WorldId = adventurerScene.WorldId,
        //                //Image = new DrawingImage(drawing),
        //                //ImageRect = drawing.Bounds

        //            });
        //        }

        //        foreach (var pair in Drawings.Relative)
        //        {
        //            //if (pair.Value.WorldId != AdvDia.WorldId)
        //            //    continue;

        //            if (!pair.Value.Drawing.Bounds.IntersectsWith(CanvasData.ClipRegion.Rect))
        //                continue;
        //            dc.DrawDrawing(pair.Value.Drawing);

        //            //if (!pair.Value.ImageRect.IntersectsWith(CanvasData.ClipRegion.Rect))
        //            //    continue;
        //            //dc.DrawImage(pair.Value.Image, pair.Value.ImageRect);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Debug("Exception in RadarUI.DrawScenes(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
        //    }
        //}


        private void DrawActor(DrawingContext dc, CanvasData canvas, RadarObject radarObject)
        {
            try
            {
                var res = RadarResources.GetActorResourceSet(radarObject);

                // Draw a dot in the center of the actor;
                dc.DrawEllipse(res.Brush, null, radarObject.Point, MarkerSize / 2, MarkerSize / 2);

                // Draw a circle representing the size of the actor
                res.Brush.Opacity = 0.25;
                var gridRadius = radarObject.CachedActorRadius * GridSize;

                var pen = RadarResources.LineOfSightLightPen;

                dc.DrawEllipse(Brushes.Red, new Pen(Brushes.Black, 0.1), radarObject.Point, gridRadius, gridRadius);
                dc.DrawEllipse(Brushes.Transparent, RadarResources.LineOfSightLightPen, radarObject.Point, 45 * GridSize, 45 * GridSize);

                DrawActorLabel(dc, canvas, radarObject, res.Brush);

            }
            catch (Exception ex)
            {
                Logger.Debug("Exception in RadarUI.DrawActor(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private void DrawCurrentPath(DrawingContext dc, CanvasData canvas)
        {
            try
            {
                var currentPath = Navigator.GetNavigationProviderAs<DefaultNavigationProvider>().CurrentPath;
                //var currentPath = NavigationCoroutine.NavigationProvider.CurrentPath;

                for (int i = 1; i < currentPath.Count; i++)
                {
                    var to = currentPath[i];
                    var from = currentPath[i - 1];

                    dc.DrawLine(RadarResources.CurrentPathPen, from.ToCanvasPoint(), to.ToCanvasPoint());
                }
                //if (currentPath.Count > 0)
                //{
                //    var newPath = NavigationGrid.Instance.GeneratePath(currentPath.First(),
                //        currentPath.Last());

                //    for (int i = 1; i < newPath.Count; i++)
                //    {
                //        var to = newPath[i];
                //        var from = newPath[i - 1];
                //        dc.DrawLine(RadarResources.NewPathPen, from.ToCanvasPoint(),
                //            to.ToCanvasPoint());
                //    }
                //}
            }
            catch (Exception ex)
            {
                Logger.Debug("Exception in RadarUI.DrawCurrentpath(). {0} {1} {2}", ex.Message, ex.InnerException, ex);
            }
        }

        private static void DrawActorLabel(DrawingContext dc, CanvasData canvas, RadarObject radarObject, Brush brush)
        {
            var text = radarObject.CachedActorName;

            if (text == null)
                return;

            const int textSize = 12;
            const int charLimit = 20;

            if (text.Length > charLimit)
                text = text.Substring(0, charLimit);

            // FormattedText is very slow, so instead create text manually with glyphs
            var glyphRun = DrawingUtilities.CreateGlyphRun(text, textSize, radarObject.Point);
            dc.DrawGlyphRun(brush, glyphRun);
        }



    }







}
