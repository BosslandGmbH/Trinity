using System.Windows.Forms;
using System.Windows.Media;

namespace Trinity.Components.Adventurer.UI.UIComponents.RadarCanvas
{
    /// <summary>
    /// OnRender doesnt like creating Brushes, Pens etc; this class allows us to pre-create and reuse them.
    /// Overlaps with system Brushes/Pens becausse they are read-only so they cannot have their properties modified. eg. Opacity.
    /// </summary>
    public static class RadarResources
    {
        private static SolidColorBrush _Node9;
        private static SolidColorBrush _node8;
        private static SolidColorBrush _node7;
        private static SolidColorBrush _node6;
        private static SolidColorBrush _node5;
        private static SolidColorBrush _node4;
        private static SolidColorBrush _node3;
        private static SolidColorBrush _node2;
        private static SolidColorBrush _node1;
        private static SolidColorBrush _node0;


        public class ResourceSet
        {
            public SolidColorBrush Brush;
            public Pen Pen;
        }

        static RadarResources()
        {


            CurrentPathPen = new Pen(Brushes.Yellow, 3);
            NewPathPen = new Pen(Brushes.Orange, 3);


            RangeGuidePen = new Pen(Brushes.LightYellow, 0.1);
            WalkableTerrainBorder = new Pen(Brushes.NavajoWhite, 0.3);
            TransparentBrush = new SolidColorBrush(Colors.Transparent);
            BorderPen = new Pen(Brushes.Black, 0.1);
            GridPen = new Pen(Brushes.Black, 0.1);

            AvoidanceBrush = new SolidColorBrush(Colors.Red);
            AvoidanceLightPen = new Pen(new SolidColorBrush(ControlPaint.Light(AvoidanceBrush.Color.ToDrawingColor(), 50).ToMediaColor()), 1);

            ActorDefaultBrush = new SolidColorBrush(Colors.White);

            PlayerBrush = new SolidColorBrush(Colors.White);
            PlayerLightPen = new Pen(new SolidColorBrush(ControlPaint.Light(PlayerBrush.Color.ToDrawingColor(), 50).ToMediaColor()), 1);

            ItemBrush = new SolidColorBrush(Colors.DarkSeaGreen);
            ItemLightPen = new Pen(new SolidColorBrush(ControlPaint.Light(ItemBrush.Color.ToDrawingColor(), 50).ToMediaColor()), 1);

            SafeBrush = new SolidColorBrush(Colors.DarkSeaGreen);
            SafeBrushLightPen = new Pen(new SolidColorBrush(ControlPaint.Light(SafeBrush.Color.ToDrawingColor(), 50).ToMediaColor()), 1);


            EliteBrush = new SolidColorBrush(Colors.Blue);
            EliteLightPen = new Pen(new SolidColorBrush(ControlPaint.Light(EliteBrush.Color.ToDrawingColor(), 50).ToMediaColor()), 1);

            HostileUnitBrush = new SolidColorBrush(Colors.DodgerBlue);
            HostileUnitLightPen = new Pen(new SolidColorBrush(ControlPaint.Light(HostileUnitBrush.Color.ToDrawingColor(), 50).ToMediaColor()), 1);

            UnitBrush = new SolidColorBrush(Colors.LightSkyBlue);
            UnitLightPen = new Pen(new SolidColorBrush(ControlPaint.Light(UnitBrush.Color.ToDrawingColor(), 50).ToMediaColor()), 1);

            TransparentBrush = new SolidColorBrush(Colors.Transparent);
            TransparentPen = new Pen(TransparentBrush, 2);

            LabelBrush = new SolidColorBrush(Colors.White);

            GizmoBrush = new SolidColorBrush(Colors.Yellow);
            GizmoLightPen = new Pen(new SolidColorBrush(ControlPaint.Light(GizmoBrush.Color.ToDrawingColor(), 50).ToMediaColor()), 1);

            WhiteDashPen = new Pen(new SolidColorBrush(Colors.WhiteSmoke), 1)
            {
                DashStyle = DashStyles.DashDotDot,
                DashCap = PenLineCap.Flat
            };

            LineOfSightPen = new Pen(new SolidColorBrush(Colors.Yellow), 1);


            LineOfSightLightBrush = new SolidColorBrush(ControlPaint.Light(Colors.Yellow.ToDrawingColor(), 50).ToMediaColor())
            {
                Opacity = 0.5
            };

            LineOfSightLightPen = new Pen(LineOfSightLightBrush, 1);


            UsedGizmoBrush = new SolidColorBrush(Colors.Yellow)
            {
                Opacity = 0.5,
            };

            UsedGizmoPen = new Pen(UsedGizmoBrush, 2)
            {
                DashStyle = DashStyles.DashDotDot
            };

            WalkableTerrain = new SolidColorBrush(Colors.NavajoWhite){Opacity = 0.2};


            _Node9 = new SolidColorBrush(Color.FromRgb(104, 0, 0))
            {
                Opacity = 1
            };

            _node8 = new SolidColorBrush(Color.FromRgb(130, 0, 0))
            {
                Opacity = 1
            };

            _node7 = new SolidColorBrush(Color.FromRgb(160, 0, 0))
            {
                Opacity = 1
            };

            _node6 = new SolidColorBrush(Color.FromRgb(180, 0, 0))
            {
                Opacity = 1
            };

            _node5 = new SolidColorBrush(Color.FromRgb(200, 0, 0))
            {
                Opacity = 1
            };

            _node4 = new SolidColorBrush(Color.FromRgb(215, 0, 0))
            {
                Opacity = 1
            };


            _node3 = new SolidColorBrush(Color.FromRgb(228, 0, 0))
            {
                Opacity = 1
            };

            _node2 = new SolidColorBrush(Color.FromRgb(240, 0, 0))
            {
                Opacity = 1
            };

            _node1 = new SolidColorBrush(Color.FromRgb(255, 0, 0))
            {
                Opacity = 1
            };

            _node0 = new SolidColorBrush(Colors.White)
            {
                Opacity = 0.5
            };
        }

        public static Brush GetWeightedBrush(float max, float current)
        {

            /*
             * 	maroon	#800000	(128,0,0)
                dark red	#8B0000	(139,0,0)
                brown	#A52A2A	(165,42,42)
                firebrick	#B22222	(178,34,34)
                crimson	#DC143C	(220,20,60)
                red	#FF0000	(255,0,0)
                tomato	#FF6347	(255,99,71)
                coral	#FF7F50	(255,127,80)
             */

            //if (max <= 0)
            //    return new SolidColorBrush(Colors.Transparent);

            var currentPct = current / max;

            if (currentPct >= 0.9)
            {
                return _Node9;
            }

            if (currentPct >= 0.8)
            {
                return _node8;
            }

            if (currentPct >= 0.7)
            {
                return _node7;
            }

            if (currentPct >= 0.6)
            {
                return _node6;
            }

            if (currentPct >= 0.5)
            {
                return _node5;
            }

            if (currentPct >= 0.4)
            {
                return _node4;
            }

            if (currentPct >= 0.2)
            {
                return _node3;
            }

            if (currentPct >= 0.2)
            {
                return _node2;
            }

            if (currentPct >= 0.001)
            {
                return _node1;
            }

            return _node0;
        }



        /// <summary>
        /// Returns a base color for actor based on type and stuff
        /// </summary>
        public static ResourceSet GetActorResourceSet(RadarObject radarObject)
        {
            var res = new ResourceSet();
            res.Brush = ActorDefaultBrush;


            res.Brush = TransparentBrush;
            res.Pen = TransparentPen;

            return res;
        }

        public static Brush WalkableTerrain { get; set; }
        public static Pen WalkableTerrainBorder { get; set; }

        public static SolidColorBrush GizmoBrush { get; set; }

        public static SolidColorBrush UsedGizmoBrush { get; set; }

        public static SolidColorBrush TransparentBrush { get; set; }

        public static Pen RangeGuidePen { get; set; }

        public static Pen BorderPen { get; set; }

        public static Pen GridPen { get; set; }

        public static Pen WhiteDashPen { get; set; }

        public static Pen UsedGizmoPen { get; set; }

        public static Pen LineOfSightPen { get; set; }

        public static SolidColorBrush LabelBrush { get; set; }

        public static Pen CurrentPathPen { get; set; }
        public static Pen NewPathPen { get; set; }

        public static SolidColorBrush AvoidanceBrush { get; set; }

        public static SolidColorBrush ActorDefaultBrush { get; set; }

        public static SolidColorBrush ItemBrush { get; set; }

        public static SolidColorBrush PlayerBrush { get; set; }

        public static SolidColorBrush EliteBrush { get; set; }

        public static SolidColorBrush HostileUnitBrush { get; set; }

        public static SolidColorBrush UnitBrush { get; set; }

        public static Pen AvoidanceLightPen { get; set; }

        public static Pen EliteLightPen { get; set; }

        public static Pen HostileUnitLightPen { get; set; }

        public static Pen UnitLightPen { get; set; }

        public static Pen GizmoLightPen { get; set; }

        public static Pen TransparentPen { get; set; }

        public static Pen ItemLightPen { get; set; }

        public static Pen PlayerLightPen { get; set; }

        public static SolidColorBrush SafeBrush { get; set; }

        public static Pen SafeBrushLightPen { get; set; }

        public static Pen LineOfSightLightPen { get; set; }

        public static SolidColorBrush LineOfSightLightBrush { get; set; }
    }
}
