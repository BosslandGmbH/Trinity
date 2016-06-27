using System;
using Trinity.Technicals;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;
using Logger = Trinity.Technicals.Logger;

namespace Trinity.Framework.Modules
{
    public enum CastResult
    {
        None = 0,
        Failed,
        Success,
        Casting,
        NotCasting,
        Transition
    }

    public class CastStatus : Module
    {
        protected override int UpdateIntervalMs => 50;

        public class CastTrackInfo
        {
            public bool WasCasting;
            public int TotalCastTimeMs;
            public DateTime StartTime;
            public Vector3 StartPosition;
            public int WorldId;
            public SNOPower Power;
            public int DurationMs;
            public CastResult LastResult;
            public Predicate<CastTrackInfo> Success;
            public bool LastTickSkipped;

            public CastTrackInfo(SNOPower power, Predicate<CastTrackInfo> success)
            {
                Power = power;
                Success = success;
            }

            public bool IsCasting => CheckIsCasting(Power);
        }

        #region Events

        public delegate void CastStatusEvent(CastTrackInfo info);

        /// <summary>
        /// When an item has been salvaged.
        /// </summary>
        public event CastStatusEvent CastSuccess = info => { };

        /// <summary>
        /// Channelled spell has failed to finish casting.
        /// </summary>
        public event CastStatusEvent CastFailure = info => { };

        #endregion

        #region Skills

        /// <summary>
        /// Stone of recall spell (town portal)
        /// </summary>
        public CastTrackInfo StoneOfRecall = new CastTrackInfo(SNOPower.UseStoneOfRecall,
            i => i.DurationMs > 3900 || ZetaDia.IsLoadingWorld || ZetaDia.CurrentWorldSnoId != i.WorldId);

        /// <summary>
        /// Teleport to player spell
        /// </summary>
        public CastTrackInfo TeleportToPlayer = new CastTrackInfo(SNOPower.TeleportToPlayer_Cast,
            i => i.DurationMs > 3900 || ZetaDia.IsLoadingWorld || i.StartPosition.Distance(ZetaDia.Me.Position) > 100f);

        /// <summary>
        /// Teleport spell (using the waypoint map).
        /// </summary>
        public CastTrackInfo TeleportToWaypoint = new CastTrackInfo(SNOPower.TeleportToWaypoint_Cast,
            i => i.DurationMs > 3900 || ZetaDia.IsLoadingWorld || ZetaDia.CurrentWorldSnoId != i.WorldId);

        #endregion

        private static bool CheckIsCasting(SNOPower power)
        {
            return ZetaDia.IsInGame && ZetaDia.Me?.CommonData?.GetAttribute<int>(((int)power << 12) +
                ((int)ActorAttributeType.PowerBuff0VisualEffectNone & 0xFFF)) == 1;
        }

        protected override void OnPulse()
        {
            using (new PerformanceLogger("Utilty.CastStatus.Pulse"))
            {
                CheckCasting(StoneOfRecall);
                CheckCasting(TeleportToPlayer);
                CheckCasting(TeleportToWaypoint);
            }                
        }

        private void CheckCasting(CastTrackInfo info)
        {
            switch (GetCastStatus(info))
            {
                case CastResult.None:
                case CastResult.NotCasting:
                case CastResult.Transition:
                    return;

                case CastResult.Success:
                    Logger.LogVerbose($"Casting {info.Power} was Successful");
                    break;

                case CastResult.Failed:
                    Logger.LogVerbose($"Casting {info.Power} Failed! Elapsed={info.DurationMs}ms");
                    break;

                case CastResult.Casting:
                    Logger.LogVerbose($"Casting {info.Power}, Elapsed={info.DurationMs}ms");
                    break;

            }
        }

        private CastResult GetCastStatus(CastTrackInfo info)
        {
            var isCasting = info.IsCasting;

            info.DurationMs = (int)DateTime.UtcNow.Subtract(info.StartTime).TotalMilliseconds;

            if (info.WasCasting == isCasting)
            {
                var result = isCasting ? CastResult.Casting : CastResult.NotCasting;
                info.LastResult = result;
                return result;
            }

            info.WasCasting = isCasting;

            if (isCasting)
            {
                Logger.LogVerbose($"Started Casting {info.Power}");
                info.StartPosition = ZetaDia.Me.Position;
                info.StartTime = DateTime.UtcNow;
                info.WorldId = ZetaDia.CurrentWorldSnoId;
                info.LastResult = CastResult.Casting;
                info.DurationMs = 0;
                return CastResult.Casting;
            }

            Logger.LogVerbose($"Stopped Casting {info.Power} after {info.DurationMs}ms");

            if (info.Success(info))
            {
                info.LastResult = CastResult.Success;
                CastSuccess(info);
                return CastResult.Success;
            }

            info.LastResult = CastResult.Failed;
            CastFailure(info);
            return CastResult.Failed;
        }

    }
}
