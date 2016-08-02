using System;
using System.ComponentModel;
using System.Windows;
using Trinity.Components.Adventurer.Game.Actors;
using Zeta.Common;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.UI.UIComponents.RadarCanvas
{
    /// <summary>
    /// RadarObject wraps a TrinityObject to add a canvas plot location.
    /// </summary>
    public class RadarObject : INotifyPropertyChanged
    {
        private DiaObject _actor;

        /// <summary>
        /// Contains the actors position and other useful information.
        /// </summary>
        public PointMorph Morph = new PointMorph();

        /// <summary>
        /// Actors current position on canvas (in pixels).
        /// </summary>
        public Point Point
        {
            get { return Morph.Point; }
        }

        public bool IsValid { get { return Actor.IsFullyValid(); } }

        public RadarObject()
        {
        }

        /// <summary>
        /// RadarObject wraps a TrinityObject to add a canvas plot location.
        /// </summary>
        public RadarObject(DiaObject obj, CanvasData canvasData)
        {
            Actor = obj;
            CachedActorName = obj.Name;
            CachedActorPosition = obj.Position;
            CachedActorRadius = obj.CollisionSphere.Radius;
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
                if (!Actor.IsValid)
                    return;

                CachedActorPosition = Actor.Position;
                Morph.Update(CachedActorPosition);

                // Try to make sure OnRender() doesnt call into DB memory.



            }
            catch (Exception ex)
            {
                Util.Logger.Debug("Exception in RadarUI.RadarObject.Update(). {0} {1}", ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// The game object
        /// </summary>
        public DiaObject Actor
        {
            set
            {
                if (!Equals(value, _actor))
                {
                    _actor = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Actor"));
                }
            }
            get { return _actor; }
        }

        #region PropertyChanged Handling

        public event PropertyChangedEventHandler PropertyChanged;



        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }

        #endregion

        public override int GetHashCode()
        {
            return Actor.GetHashCode();
        }

        public string CachedActorName { get; set; }

        public float CachedActorRadius { get; set; }

        public Vector3 CachedActorPosition { get; set; }

    }
}
