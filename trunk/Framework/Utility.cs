using System;
using Trinity.Technicals;
using Zeta.Bot;

namespace Trinity.Framework
{
    public class Utility
    {
        public DateTime LastUpdated = DateTime.MinValue;

        public void Enable()
        {
            Pulsator.OnPulse += FirePulse;
            GameEvents.OnWorldChanged -= FireWorldChanged;
        }

        public void Disable()
        {
            Pulsator.OnPulse -= FirePulse;
            GameEvents.OnWorldChanged -= FireWorldChanged;
        }

        protected TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMilliseconds(100);

        internal void FirePulse(object sender, EventArgs eventArgs)
        {
            if (DateTime.UtcNow.Subtract(LastUpdated).TotalMilliseconds < UpdateInterval.TotalMilliseconds)
                return;

            OnPulse();
            LastUpdated = DateTime.UtcNow;
        }

        protected virtual void OnPulse()
        {

        }

        private void FireWorldChanged(object sender, EventArgs e)
        {
            OnWorldChanged();
        }

        protected virtual void OnWorldChanged()
        {

        }
    }
}

