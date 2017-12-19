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
            if (Core.Settings.Advanced.TpsEnabled)
            {
                if (BotMain.TicksPerSecond != Core.Settings.Advanced.TpsLimit)
                {
                    BotMain.TicksPerSecond = Core.Settings.Advanced.TpsLimit;
                    Core.Logger.Verbose(LogCategory.None, "DB的处理量设置到 {0}", Core.Settings.Advanced.TpsLimit);
                }
            }
            else
            {
                if (BotMain.TicksPerSecond != DefaultTPS)
                {
                    BotMain.TicksPerSecond = DefaultTPS;
                    Core.Logger.Verbose(LogCategory.None, "恢复DB的处理量到默认: {0}", BotMain.TicksPerSecond);
                }
            }
        }
    }
}
