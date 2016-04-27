using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Trinity.UI.Visualizer.RadarCanvas;
using Zeta.Bot;
using Zeta.Common;

namespace Trinity.UI.UIComponents.RadarCanvas
{

    public class DrawOptions
    {
        public DrawOptions()
        {
            Created = DateTime.UtcNow;
            Duration = TimeSpan.Zero;
        }

        public IEnumerable<Vector3> Points { get; set; }
        public RadarDebug.DrawColor Color { get; set; }
        public RadarDebug.DrawType Type { get; set; }
        public DateTime Created { get; set; }
        public TimeSpan Duration { get; set; }
        public string Name { get; set; }
        public bool Follow { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + Points.GetHashCode();
                hash = hash * 23 + Duration.GetHashCode();
                return hash;
            }
        }
    }

    public class RadarDebug
    {
        static RadarDebug()
        {
            Pulsator.OnPulse += (sender, args) => Clear();
        }

        private static void Clear()
        {
            _dataStore = _dataStore.Where(data => data.Created + data.Duration > DateTime.UtcNow).ToList();
        }

        public enum DrawType
        {
            None = 0,
            Elipse,
            PolyLine,
            Text
        }

        public enum DrawColor
        {
            None,
            Yellow,
            Green,
            Black,
            Blue,
            White,
        }

        private static List<DrawOptions> _dataStore = new List<DrawOptions>();

        public static void Draw(IEnumerable<Vector3> points, int drawTimeMs = 50, DrawType type = DrawType.Elipse, DrawColor color = DrawColor.Yellow)
        {
            Draw(new DrawOptions
            {
                Points = points,
                Name = string.Empty,
                Color = color,
                Type = type,
                Duration = TimeSpan.FromMilliseconds(drawTimeMs)
            });
        }

        public static void DrawElipse(IEnumerable<Vector3> points, int drawTimeMs = 50, DrawColor color = DrawColor.Yellow)
        {
            Draw(new DrawOptions
            {
                Points = points,
                Name = string.Empty,
                Color = color,
                Type = DrawType.Elipse,
                Duration = TimeSpan.FromMilliseconds(drawTimeMs)
            });
        }

        public static void DrawText(Vector3 position, string text, int drawTimeMs = 50, DrawColor color = DrawColor.White)
        {
            Draw(new DrawOptions
            {
                Points = new List<Vector3> { position },
                Name = text,
                Color = color,
                Type = DrawType.Text,
                Duration = TimeSpan.FromMilliseconds(drawTimeMs)
            });
        }

        public static void Draw(DrawOptions drawOptions)
        {
            if (!_dataStore.Contains(drawOptions))
                _dataStore.Add(drawOptions);
        }

        private const int DebugDrawSize = 6;

        public static void Render(DrawingContext dc, CanvasData cd)
        {
            foreach (var data in _dataStore)
            {
                switch (data.Type)
                {
                    case DrawType.Elipse:

                        foreach (var point in data.Points)
                        {
                            dc.DrawEllipse(GetBrush(data.Color), null, point.ToCanvasPoint(), DebugDrawSize, DebugDrawSize);
                        }

                        break;

                    case DrawType.PolyLine:

                        var points = RadarCanvas.GrahamScan.GrahamScanCompute(data.Points).Select(p => p.ToCanvasPoint());
                        dc.DrawPolygon(points, GetBrush(data.Color));

                        break;

                    case DrawType.Text:

                        var txtPos = data.Points.First().ToCanvasPoint();
                        txtPos.Y += 15;
                        var gr = DrawingUtilities.CreateGlyphRun(data.Name, 10, txtPos);
                        dc.DrawGlyphRun(GetBrush(data.Color), gr);
                        break;
                }
            }
        }

        private static Brush GetBrush(DrawColor color)
        {
            switch (color)
            {
                case DrawColor.Blue:
                    return RadarCanvas.BlueBrush;
                case DrawColor.Black:
                    return RadarCanvas.BlackBrush;
                case DrawColor.Yellow:
                    return RadarCanvas.YellowBrush;
                case DrawColor.Green:
                    return RadarCanvas.GreenBrush;
                case DrawColor.White:
                    return RadarCanvas.WhiteBrush;
                default:
                    return RadarCanvas.YellowBrush;
            }
        }

    }
}
