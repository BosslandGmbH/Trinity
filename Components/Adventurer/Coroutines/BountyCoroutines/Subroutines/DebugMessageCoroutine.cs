using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Components.Adventurer.Cache;
using Trinity.Components.Adventurer.Game.Actors;
using Trinity.Components.Adventurer.Game.Combat;
using Trinity.Components.Adventurer.Game.Exploration;
using Trinity.Components.Adventurer.Game.Quests;
using Trinity.Components.Adventurer.Util;
using Zeta.Common;
using Zeta.Game;
using Logger = Trinity.Components.Adventurer.Util.Logger;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public class DebugMessageCoroutine : IBountySubroutine
    {
        private string _message;
        private bool _isDone;

        public DebugMessageCoroutine(string message)
        {
            _message = message;
        }

        public bool IsDone => _isDone;

        public async Task<bool> GetCoroutine()
        {
            Logger.Warn(_message);
            _isDone = true;
            return true;
        }

        public void Reset()
        {
 
        }

        public void DisablePulse()
        {

        }

        public BountyData BountyData { get; }
    }
}
