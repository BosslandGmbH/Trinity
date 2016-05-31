using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.Coroutines.Town;
using Trinity.Technicals;
using Zeta.Bot;
using Zeta.Game;

namespace Trinity.Framework
{
    public class Utility
    {
        public DateTime LastUpdated = DateTime.MinValue;

        public Utility()
        {
            Utilities.Add(new WeakReference<Utility>(this));
        }

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

        protected TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMilliseconds(80);

        internal void FirePulse(object sender, EventArgs eventArgs)
        {
            if (DateTime.UtcNow.Subtract(LastUpdated).TotalMilliseconds < UpdateInterval.TotalMilliseconds)
                return;

            if (ZetaDia.Me == null)
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

        public static List<WeakReference<Utility>> Utilities = new List<WeakReference<Utility>>();

        public static void PulseAll()
        {
            foreach (var utilReference in Utilities.ToList())
            {
                Utility util;
                if (utilReference.TryGetTarget(out util))
                {
                    util.FirePulse(null, EventArgs.Empty);
                }
                else
                {
                    Utilities.Remove(utilReference);
                }
            }
        }

        public static void EnableAll()
        {
            foreach (var utilReference in Utilities.ToList())
            {
                Utility util;
                if (utilReference.TryGetTarget(out util))
                {
                    util.Enable();
                }
                else
                {
                    Utilities.Remove(utilReference);
                }
            }
        }

        public static void DisableAll()
        {
            foreach (var utilReference in Utilities.ToList())
            {
                Utility util;
                if (utilReference.TryGetTarget(out util))
                {
                    util.Enable();
                }
                else
                {
                    Utilities.Remove(utilReference);
                }
            }
        }

    }
}

