using System;
using System.ComponentModel;
using System.Windows;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects.Enums;
using Trinity.Objects;
using Zeta.Common;
using Zeta.Game.Internals.SNO;
using Trinity.Technicals;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.UI.UIComponents.RadarCanvas
{
    /// <summary>
    /// RadarObject wraps a TrinityObject to add a canvas plot location.
    /// </summary>
    public class RadarObject // : INotifyPropertyChanged
    {
        private TrinityCacheObject _actor;

        /// <summary>
        /// Contains the actors position and other useful information.
        /// </summary>
        public PointMorph Morph = new PointMorph();

        /// <summary>
        /// Position in game world space for a point at radius distance 
        /// from actor's center and in the direction the actor is facing.
        /// </summary>
        public Vector3 HeadingVectorAtRadius { get; set; }

        /// <summary>
        /// Position on canvas (in pixels) for a point at radius distance 
        /// from actor's center and in the direction the actor is facing.
        /// </summary>
        public Point HeadingPointAtRadius { get; set; }

        /// <summary>
        /// Actors current position on canvas (in pixels).
        /// </summary>
        public Point Point
        {
            get { return Morph.Point; }
        }

        /// <summary>
        /// RadarObject wraps a TrinityObject to add a canvas plot location.
        /// </summary>
        public RadarObject(TrinityCacheObject obj, CanvasData canvasData)
        {
            Actor = obj;
            Morph.CanvasData = canvasData;
            Update();
        }

        /// <summary>
        /// Updates the plot location on canvas based on Item's current position.
        /// </summary>
        public void Update()
        {
            try
            {
                Morph.Update(Actor.Position);
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in RadarUI.RadarObject.Update(). {0} {1}", ex.Message, ex.InnerException);
            }
        }

        public TrinityCacheObject Actor { get; set; }
    }
}
