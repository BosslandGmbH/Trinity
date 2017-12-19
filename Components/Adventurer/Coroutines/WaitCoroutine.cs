using System;
using Trinity.Framework;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Quests;
using Zeta.Common.Helpers;
using Zeta.Game;

namespace Trinity.Components.Adventurer.Coroutines
{
    public class WaitCoroutine : IBountySubroutine
    {
        private readonly TimeSpan _waitTime;
        private bool _isDone;

        public bool IsDone
        {
            get { return _isDone; }
        }

        private WaitTimer _waitTimer;
        private int _worldId;
        private int _questId;

        public WaitCoroutine(int milliSeconds)
        {
            _waitTime = TimeSpan.FromMilliseconds(milliSeconds);
        }

        public WaitCoroutine(int questId, int worldSnoId, int milliSeconds)
        {
            _worldId = worldSnoId;
            _questId = questId;
            _waitTime = TimeSpan.FromMilliseconds(milliSeconds);
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            SafeZerg.Instance.DisableZerg();
            if (_waitTimer == null)
            {
                _waitTimer = new WaitTimer(_waitTime);
                _waitTimer.Reset();

                string status = $"[等待] 等待 {_waitTime.TotalSeconds} 秒";
                StatusText = status;
                Core.Logger.Debug(status);
            }

            if (ZetaDia.Globals.WorldSnoId != _worldId)
            {
                StatusText = "[等待] 因为 World id 不正确, 停止等待";
                Core.Logger.Debug(StatusText);
                _isDone = true;
                return true;
            }

            if (!_waitTimer.IsFinished)
                return false;

            _isDone = true;
            return true;
        }

        public void Reset()
        {
            _isDone = false;
            _waitTimer = null;
        }

        public string StatusText { get; set; }

        public void DisablePulse()
        {
        }

        public BountyData BountyData
        {
            get { return null; }
        }
    }
}