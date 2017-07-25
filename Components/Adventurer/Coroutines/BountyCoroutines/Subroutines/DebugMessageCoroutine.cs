using System;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Framework;


namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class DebugMessageCoroutine : IBountySubroutine
    {
        private string _message;
        private bool _isDone;

        public DebugMessageCoroutine(string message)
        {
            _message = message;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public bool IsDone => _isDone;

        public async Task<bool> GetCoroutine()
        {
            CoroutineCoodinator.Current = this;

            Core.Logger.Warn(_message);
            _isDone = true;
            return true;
        }

        public void Reset()
        {
        }

        public string StatusText { get; set; }

        public void DisablePulse()
        {
        }

        public BountyData BountyData { get; }
    }
}