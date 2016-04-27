using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Trinity.Objects;
using Trinity.UI.Overlays._3D.Perspective.Primitives;
using Trinity.UI.Overlays._3D._3DTools;
using Trinity.UI.UIComponents.RadarCanvas;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.UI.Overlays._3D
{
    public class ViewPort3DActor
    {
        public IActor Actor;
        public CanvasData Canvas;
        public Point3D Offset;
        public GeometryElement3D Visual;
        public Transform3DGroup TransformGroup;
        public TranslateTransform3D Translation;
        public ScaleTransform3D Scale;

        public ViewPort3DActor(IActor obj, CanvasData canvasData, GeometryElement3D visual)
        {
            Canvas = canvasData;
            Visual = visual;
            //TransformGroup = new Transform3DGroup();
            //Translation = new TranslateTransform3D();
            //Visual.Transform = TransformGroup;
            Update(obj);

            
        }


        public void Update(IActor obj)
        {
            try
            {
                Actor = obj;
                //Offset = GetDrawOffset(Actor, Canvas);

                if (TransformGroup == null)
                {
                    TransformGroup = new Transform3DGroup();
                    //Scale = new ScaleTransform3D(0.3, 0.3, 0.3);
                    Translation = new TranslateTransform3D(Actor.Position.X, Actor.Position.Y, Actor.Position.Z);
                    TransformGroup.Children.Add(Translation);
                    //TransformGroup.Children.Add(Scale);
                    Visual.Transform = TransformGroup;
                }
                //else
                //{
                //    Translation = new TranslateTransform3D(Actor.Position.X * Canvas.Scale, Actor.Position.Y * Canvas.Scale, Actor.Position.Z * Canvas.Scale);

                //}


                //else
                //{
                //    Translation.OffsetX = Offset.X;
                //    Translation.OffsetY = Offset.Y;
                //    Translation.OffsetZ = Offset.Z;
                //}

            }
            catch (Exception ex)
            {
                Logger.Log("Exception in ViewPort3DActor.Update(). {0} {1}", ex.Message, ex.InnerException);
            }
        }


        public static Point3D GetDrawOffset(IActor actor, CanvasData canvas)
        {
            var gridSize = (float) canvas.GridSquareSize.Height;
            var position = actor.Position;
            var centerActorPosition = canvas.CenterVector;

            var worldDifferenceX = centerActorPosition.X - position.X;
            var worldDifferenceY = centerActorPosition.Y - position.Y;
            var worldDifferenceZ = centerActorPosition.Z - position.Z;

            //var drawDifferenceX = worldDifferenceX * gridSize;
            //var drawDifferenceY = worldDifferenceY * gridSize;
            //var drawDifferenceZ = worldDifferenceZ * gridSize;

            var center = new Point3D(0, 0, 0);
            var drawPoint = new Point3D(position.X, position.Y, position.Z);
            drawPoint = Rotate(drawPoint, center, 45);
            drawPoint = drawPoint.FlipX(center);
            return drawPoint;
        }

        private const double DegToRad = Math.PI / 180;

        public static Point3D Rotate(Point3D p, Point3D origin, double degrees)
        {
            var theta = degrees * DegToRad;
            var x = Math.Cos(theta) * (p.X - origin.X) - Math.Sin(theta) * (p.Y - origin.Y) + origin.X;
            var y = Math.Sin(theta) * (p.X - origin.X) + Math.Cos(theta) * (p.Y - origin.Y) + origin.Y;
            var z = p.Z;
            return new Point3D(x, y, z);
        }
    }
}

