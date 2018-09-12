using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Bot;


namespace Trinity.Modules
{
    public class Performance : Module
    {
        protected override int UpdateIntervalMs => 1000;

        protected override void OnPulse() => UpdateTicksPerSecond();

        public int DefaultTPS { get; } = 15;

        private void UpdateTicksPerSecond()
        {
        }
    }
}
