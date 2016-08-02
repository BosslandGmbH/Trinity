using System;
using Trinity;
using Trinity.Framework;
using Zeta.Game;
using Trinity.Technicals;
using Zeta.Bot;

namespace Trinity.Helpers
{
    public static class MemoryHelperTools
    {
        public static long LastUpdatedFrame { get; set; }
    }

    public class AquireFrameHelper : IDisposable
    {
        bool _disposed;
        private GreyMagic.FrameLockRelease _frameLockRelease;
        private GreyMagic.ExternalReadCache _externalReadCache;
        

        /// <summary>
        /// Enables reading DB data while bot is running or stopped
        /// </summary>
        public AquireFrameHelper()
        {
            _frameLockRelease = ZetaDia.Memory.ReleaseFrame(true);

            if (ZetaDia.Service.IsInGame)
            {
                if (!BotMain.IsRunning && MemoryHelperTools.LastUpdatedFrame != ZetaDia.Memory.Executor.FrameCount)
                {
                    MemoryHelperTools.LastUpdatedFrame = ZetaDia.Memory.Executor.FrameCount;
                    Core.Player.Update();
                    ZetaDia.Actors.Update();
                }

                _externalReadCache = ZetaDia.Memory.SaveCacheState();
                ZetaDia.Memory.TemporaryCacheState(false);
            }            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AquireFrameHelper()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                try
                {
                    if (_frameLockRelease != null)
                        _frameLockRelease.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.LogDebug("Exception disposing of MemoryHelper._frameLockRelease");
                }

                try
                {
                    if (_externalReadCache != null)
                        _externalReadCache.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.LogDebug("Exception disposing of MemoryHelper._externalReadCache");
                }
            }

            _externalReadCache = null;
            _frameLockRelease = null;

            _disposed = true;
        }
    }
}
