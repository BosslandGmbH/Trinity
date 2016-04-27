using System;
using Trinity.Technicals;
using Zeta.Bot;

namespace Trinity.Framework
{
    public class Utility
    {
        public DateTime LastUpdated = DateTime.MinValue;

        public Utility()
        {
            UpdateInterval = TimeSpan.FromMilliseconds(100);
        }

        public void Enable()
        {
            Pulsator.OnPulse += BasePulse;
            GameEvents.OnWorldChanged -= BaseWorldChanged;
        }

        public void Disable()
        {
            Pulsator.OnPulse -= BasePulse;
            GameEvents.OnWorldChanged -= BaseWorldChanged;
        }

        /// <summary>
        /// How quickly OnPulse() will be executed
        /// </summary>
        protected TimeSpan UpdateInterval { get; set; }

        internal void BasePulse(object sender, EventArgs eventArgs)
        {
            if (DateTime.UtcNow.Subtract(LastUpdated).TotalMilliseconds < UpdateInterval.TotalMilliseconds)
                return;

            OnPulse();
            LastUpdated = DateTime.UtcNow;
        }

        protected virtual void OnPulse()
        {

        }

        private void BaseWorldChanged(object sender, EventArgs e)
        {
            OnWorldChanged();
        }

        protected virtual void OnWorldChanged()
        {

        }
    }
}

