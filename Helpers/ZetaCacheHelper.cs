using System;
using Zeta.Game;

namespace Trinity.Helpers
{
    public class ZetaCacheHelper: IDisposable
    {
        internal const string Validator = "Q0RQYXRjaGVy";

        private GreyMagic.ExternalReadCache externalReadCache;
        private GreyMagic.FrameLock frame;
        public ZetaCacheHelper()
        {
            frame = ZetaDia.Memory.AcquireFrame();
            externalReadCache = ZetaDia.Memory.SaveCacheState();
            ZetaDia.Memory.TemporaryCacheState(false);
            ZetaDia.Actors.Update();
        }

        ~ZetaCacheHelper()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (externalReadCache != null)
                externalReadCache.Dispose();
            if (frame != null)
                frame.Dispose();
        }
    }
}
