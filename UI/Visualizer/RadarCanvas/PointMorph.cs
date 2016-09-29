using System;
using System.Windows;
using Trinity.Framework.Helpers;
using Zeta.Common;
using Logger = Trinity.Framework.Helpers.Logger;

namespace Trinity.UI.Visualizer.RadarCanvas
{
    /// <summary>
    /// PointMorph handles the translation of a Vector3 world space position into Canvas space.
    /// </summary>
    public class PointMorph
    {
        public PointMorph() { }

        public PointMorph(CanvasData canvasData)
        {
            CanvasData = canvasData;
        }

        /// <summary>
        /// PointMorph handles the translation of a Vector3 world space position into Canvas space.
        /// </summary>
        public PointMorph(Vector3 vectorPosition, CanvasData canvasData)
        {
            CanvasData = canvasData;
            Update(vectorPosition);
        }

        /// <summary>
        /// Information about the canvas
        /// </summary>
        public CanvasData CanvasData { get; set; }

        /// <summary>
        /// Point in GridSquare (Yards) Space before translations
        /// </summary>
        public Point RawGridPoint { get; set; }

        /// <summary>
        /// Point before any translations
        /// </summary>
        public Point RawPoint { get; set; }

        /// <summary>
        /// Flipped and Rotated point
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// Point coods based on Grid Scale
        /// </summary>
        public Point GridPoint { get; set; }

        /// <summary>
        /// If the point is located outside of the canvas bounds
        /// </summary>
        public bool IsBeyondCanvas { get; set; }

        /// <summary>
        /// If the point is Zero
        /// </summary>
        public bool IsZero
        {
            get { return Point.X == 0 || Point.Y == 0; }
        }

        /// <summary>
        /// Game world distance from this point to the center actor on X-Axis
        /// </summary>
        public float RawWorldDistanceX { get; set; }

        /// <summary>
        /// Game world distance from this point to the center actor on Y-Axis
        /// </summary>
        public float RawWorldDistanceY { get; set; }

        /// <summary>
        /// Canvas (pixels) distance from this point to the center actor on Y-Axis
        /// </summary>
        public float RawDrawDistanceX { get; set; }

        /// <summary>
        /// Canvas (pixels) distance from this point to the center actor on Y-Axis
        /// </summary>
        public float RawDrawDistanceY { get; set; }

        /// <summary>
        /// Absolute canvas X-Axis coodinate for this actor (in pixels)
        /// </summary>
        public double RawDrawPositionX { get; set; }

        /// <summary>
        /// Absolute canvas Y-Axis coodinate for this actor (in pixels)
        /// </summary>
        public double RawDrawPositionY { get; set; }

        /// <summary>
        /// Absolute game world vector
        /// </summary>
        public Vector3 WorldVector { get; set; }

        /// <summary>
        /// Calculates Canvas position with a given game world position
        /// </summary>
        public void Update(Vector3 position)
        {
            try
            {
                WorldVector = position;

                var centerActorPosition = CanvasData.CenterVector;

                // Distance from Actor to Player
                RawWorldDistanceX = centerActorPosition.X - position.X;
                RawWorldDistanceY = centerActorPosition.Y - position.Y;

                // We want 1 yard of game distance to = Gridsize
                RawDrawDistanceX = RawWorldDistanceX * (float)CanvasData.GridSquareSize.Width;
                RawDrawDistanceY = RawWorldDistanceY * (float)CanvasData.GridSquareSize.Height;

                // Distance on canvas from center to actor
                RawDrawPositionX = (CanvasData.Center.X + RawDrawDistanceX);
                RawDrawPositionY = (CanvasData.Center.Y + RawDrawDistanceY);

                // Points in Canvas and Grid Scale
                RawPoint = new Point(RawDrawPositionX, RawDrawPositionY);
                RawGridPoint = new Point(RawDrawPositionX / CanvasData.GridSquareSize.Width, RawDrawPositionY / CanvasData.GridSquareSize.Height);

                // Switched to manual calculations because WPF transforms are very slow 
                // (0.0015ms+ each versus 0.0000ms for raw math).
                Point = RawPoint.Rotate(CanvasData.Center, CanvasData.GobalRotationAngle);
                Point = Point.FlipX(CanvasData.Center);
                Point = new Point(Point.X + CanvasData.PanOffset.X, Point.Y + CanvasData.PanOffset.Y);

                GridPoint = new Point((int)(Point.X / CanvasData.GridSquareSize.Width), (int)(Point.Y / CanvasData.GridSquareSize.Height));
                IsBeyondCanvas = Point.X < 0 || Point.X > CanvasData.CanvasSize.Width || Point.Y < 0 || Point.Y > CanvasData.CanvasSize.Height;
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.PointMorph.Update(). {0} {1}", ex.Message, ex.InnerException);
            }
        }

        public static Vector3 GetWorldPosition(Point canvasPoint, CanvasData canvasData)
        {
            var unPannedPoint = new Point(canvasPoint.X - canvasData.PanOffset.X, canvasPoint.Y - canvasData.PanOffset.Y);
            var unFlippedPoint = unPannedPoint.FlipX(canvasData.Center);
            var rawPoint = unFlippedPoint.Rotate(canvasData.Center, -canvasData.GobalRotationAngle);
            var x = Math.Abs(((rawPoint.X - canvasData.Center.X) / (float)canvasData.GridSquareSize.Width) - canvasData.CenterVector.X);
            var y = Math.Abs(((rawPoint.Y - canvasData.Center.Y) / (float)canvasData.GridSquareSize.Height) - canvasData.CenterVector.Y);
            return new Vector3((float)x, (float)y, canvasData.CenterVector.Z);
        }


    }
}

