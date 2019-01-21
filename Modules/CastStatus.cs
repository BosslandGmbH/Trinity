using System;
using Trinity.Framework;
using Trinity.Framework.Helpers;
using Trinity.Framework.Objects;
using Zeta.Common;
using Zeta.Game;
using Zeta.Game.Internals.Actors;


namespace Trinity.Modules
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

    /// <summary>
    /// Keeps track of any spells that are being channelled
    /// </summary>
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

        private static int BasicCompletionTimeOfTeleportingActions = 3750;

        /// <summary>
        /// Stone of recall spell (town portal)
        /// </summary>
        public CastTrackInfo StoneOfRecall = new CastTrackInfo(SNOPower.UseStoneOfRecall,
            i => i.DurationMs > BasicCompletionTimeOfTeleportingActions || ZetaDia.Globals.IsLoadingWorld || ZetaDia.Globals.WorldSnoId != i.WorldId);

        /// <summary>
        /// Teleport to player spell
        /// </summary>
        public CastTrackInfo TeleportToPlayer = new CastTrackInfo(SNOPower.TeleportToPlayer_Cast,
            i => i.DurationMs > BasicCompletionTimeOfTeleportingActions || ZetaDia.Globals.IsLoadingWorld || i.StartPosition.Distance(ZetaDia.Me.Position) > 100f);

        /// <summary>
        /// Teleport spell (using the waypoint map).
        /// </summary>
        public CastTrackInfo TeleportToWaypoint = new CastTrackInfo(SNOPower.TeleportToWaypoint_Cast,
            i => i.DurationMs > BasicCompletionTimeOfTeleportingActions || ZetaDia.Globals.IsLoadingWorld || ZetaDia.Globals.WorldSnoId != i.WorldId);

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
                    Core.Logger.Verbose($"Casting {info.Power} was Successful");
                    break;

                case CastResult.Failed:
                    Core.Logger.Verbose($"Casting {info.Power} Failed! Elapsed={info.DurationMs}ms");
                    break;

                case CastResult.Casting:
                    Core.Logger.Verbose($"Casting {info.Power}, Elapsed={info.DurationMs}ms");
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
                Core.Logger.Verbose($"Started Casting {info.Power}");
                info.StartPosition = ZetaDia.Me.Position;
                info.StartTime = DateTime.UtcNow;
                info.WorldId = ZetaDia.Globals.WorldSnoId;
                info.LastResult = CastResult.Casting;
                info.DurationMs = 0;
                return CastResult.Casting;
            }

            Core.Logger.Verbose($"Stopped Casting {info.Power} after {info.DurationMs}ms");

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
