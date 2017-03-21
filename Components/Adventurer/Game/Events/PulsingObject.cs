using System;
using Zeta.Bot;

namespace Trinity.Components.Adventurer.Game.Events
{
    public abstract class PulsingObject : IDisposable
    {
        #region OnPulse Implementation

        protected bool IsPulsing;

        protected void EnablePulse()
        {
            if (!IsPulsing)
            {
                Pulsator.OnPulse += OnPulse;
                IsPulsing = true;
            }
        }

        protected void DisablePulse()
        {
            if (IsPulsing)
            {
                Pulsator.OnPulse -= OnPulse;
                IsPulsing = false;
            }
        }

        private void OnPulse(object sender, EventArgs e)
        {
            OnPulse();
        }

        #endregion OnPulse Implementation

        public void Dispose()
        {
            DisablePulse();
        }

        protected abstract void OnPulse();
    }
}