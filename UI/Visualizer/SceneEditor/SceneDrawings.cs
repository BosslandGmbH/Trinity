using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Serialization;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Framework.Helpers;
using Trinity.UI.Visualizer.RadarCanvas;
using Zeta.Common;
using LineSegment = System.Windows.Media.LineSegment;

namespace Trinity.UI.Visualizer.SceneEditor
{
    public class SavedScene
    {
        private CData _drawingData;
        public CData DrawingData
        {
            get { return _drawingData; }
            set
            {
                _drawingData = value;
                Drawing = (DrawingGroup)XamlReader.Parse(value);
            }
        }

        [XmlIgnore]
        public DrawingGroup Drawing { get; private set; }
        public int UniqueId { get; set; }
        public string UniqueName { get; set; }
        public SavedScene SubScene { get; set; }
        public int SceneSnoId { get; set; }
        public bool HasChild { get; set; }
        public Vector2 Size { get; set; }
        public string InternalName { get; set; }
        public bool IsTopLevel { get; set; }

        public static int GetUniqueId(WorldScene scene)
        {
            return scene == null ? 0 : 31 * scene.SnoId + GetUniqueId(scene.SubScene);
        }

        public static string GetUniqueName(WorldScene scene)
        {
            return scene == null ? string.Empty : $"{scene.Name}_{GetUniqueName(scene.SubScene)}";
        }

        public static SavedScene Create(WorldScene scene)
        {            
            if (scene == null)
                return null;

            var drawingGroup = CreateDrawingGroup(scene);
            var subScene = Create(scene.SubScene);

            return new SavedScene
            {
                SceneSnoId = scene.SnoId,
                IsTopLevel = scene.IsTopLevel,
                HasChild = scene.HasChild,    
                InternalName = scene.Name,
                DrawingData = XamlWriter.Save(drawingGroup),
                Drawing = drawingGroup,
                Size = scene.Max - scene.Min,
                SubScene = subScene,
                UniqueId = GetUniqueId(scene),
                UniqueName = GetUniqueName(scene)
            };
        }

        public static DrawingGroup CreateDrawingGroup(WorldScene adventurerScene)
        {
            var drawing = new DrawingGroup();
            using (var groupdc = drawing.Open())
            {
                if (adventurerScene.Cells != null)
                {
                    var figures = (
                        from navCell in adventurerScene.Cells.Where(nc => nc.IsWalkable)
                        let topLeft = new Vector3(navCell.MinX, navCell.MinY, 0)
                        let topRight = new Vector3(navCell.MaxX, navCell.MinY, 0)
                        let bottomLeft = new Vector3(navCell.MinX, navCell.MaxY, 0)
                        let bottomRight = new Vector3(navCell.MaxX, navCell.MaxY, 0)
                        let segments = new[]
                        {
                            new LineSegment(topLeft.ToPoint(), true),
                            new LineSegment(topRight.ToPoint(), true),
                            new LineSegment(bottomRight.ToPoint(), true)
                        }
                        select new PathFigure(bottomLeft.ToPoint(), segments, true)
                    ).ToList();

                    var geo = new PathGeometry(figures, FillRule.Nonzero, null);
                    geo.GetOutlinedPathGeometry();
                    groupdc.DrawGeometry(RadarResources.WalkableTerrain, null, geo);
                }

                var sceneTopLeft = new Vector3(adventurerScene.Min.X, adventurerScene.Min.Y, 0);
                var sceneTopRight = new Vector3(adventurerScene.Max.X, adventurerScene.Min.Y, 0);
                var sceneBottomLeft = new Vector3(adventurerScene.Min.X, adventurerScene.Max.Y, 0);
                var sceneBottomRight = new Vector3(adventurerScene.Max.X, adventurerScene.Max.Y, 0);

                groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopLeft.ToPoint(), sceneTopRight.ToPoint()));
                groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneBottomLeft.ToPoint(), sceneBottomRight.ToPoint()));
                groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopLeft.ToPoint(), sceneBottomLeft.ToPoint()));
                groupdc.DrawGeometry(null, RadarResources.WalkableTerrainBorder, new LineGeometry(sceneTopRight.ToPoint(), sceneBottomRight.ToPoint()));

                var textPoint = adventurerScene.Center.ToVector3().ToCanvasPoint();
                var glyphRun = DrawingUtilities.CreateGlyphRun(adventurerScene.Name, 10, textPoint);
                groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun);
                textPoint = adventurerScene.Center.ToVector3().ToCanvasPoint();
                textPoint.Y = textPoint.Y + 20;
                glyphRun = DrawingUtilities.CreateGlyphRun((adventurerScene.Max - adventurerScene.Min) + " " + (adventurerScene.HasChild ? "HasSubScene" : string.Empty) + " " + (adventurerScene.SubScene != null ? " (Loaded)" : string.Empty), 8, textPoint);
                groupdc.DrawGlyphRun(Brushes.Wheat, glyphRun);
            }
            return drawing;
        }

    }

    public sealed class SceneDrawings
    {
        public static SceneDrawings Instance => _instance.Value;
        private static readonly HashSet<int> LoadedSceneIds = new HashSet<int>();
        private static readonly Lazy<SceneDrawings> _instance = new Lazy<SceneDrawings>(() => new SceneDrawings());
        private readonly Dictionary<int, SavedScene> _scenes  = new Dictionary<int, SavedScene>();
    
        private SceneDrawings()
        {
            Application.Current.MainWindow.Closing += (sender, args) => Save();
            Load();
        }

        private void Load()
        {
            var directory = Path.Combine(FileManager.DemonBuddyPath, "Data", "Scene");
            if (!Directory.Exists(directory))
                return;

            foreach (var filePath in Directory.GetFiles(directory, "*.xml", SearchOption.TopDirectoryOnly))
            {
                var text = File.ReadAllText(filePath);
                var sceneDrawing = EasyXmlSerializer.Deserialize<SavedScene>(text);
                var key = sceneDrawing.SceneSnoId;
                _scenes.Add(key, sceneDrawing);
                LoadedSceneIds.Add(key);
            }
        }

        public void Save()
        {
            var directory = Path.Combine(FileManager.DemonBuddyPath, "Data", "Scene");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var toSave = _scenes.Where(s => !LoadedSceneIds.Contains(s.Key));
            foreach (var scenePair in toSave)
            {
                var filePath = Path.Combine(directory, scenePair.Value.UniqueName + ".xml");
                var sceneDrawing = EasyXmlSerializer.Serialize(scenePair.Value);
                File.WriteAllText(filePath, sceneDrawing);
            }
        }

        public bool Record(WorldScene scene)
        {
            var key = scene.SnoId;
            if (_scenes.ContainsKey(key))
                return false;

            var sceneDrawing = SavedScene.Create(scene);
            _scenes.Add(scene.SnoId, sceneDrawing);
            return true;
        }

        public SavedScene GetSceneDrawing(int sceneSnoId)
        {
            if (_scenes.ContainsKey(sceneSnoId))
                return _scenes[sceneSnoId];

            var scene = ScenesStorage.CurrentWorldScenes.FirstOrDefault(s => s.SnoId == sceneSnoId);
            if (scene == null)
                return null;

            var sceneDrawing = SavedScene.Create(scene);
            _scenes.Add(sceneSnoId, sceneDrawing);
            return sceneDrawing;
        }


    }



}

