using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Common;

namespace Trinity.Components.Combat.Party
{
    public class DefaultPartyProvider : IPartyProvider
    {
        private IEnumerable<TrinityPlayer> Players => Core.Actors.AllRActors.OfType<TrinityPlayer>();
        public IEnumerable<IPartyMember> Members => Players;
        public IEnumerable<IPartyMember> Followers => Enumerable.Empty<IPartyMember>();
        public IPartyMember Leader => Players.OrderByDescending(p => p.HitPointsMax).FirstOrDefault();
        public ITargetable PriorityTarget => Leader.Target;
        public Vector3 FightLocation => Leader.Position;
        public override string ToString() => $"{GetType().Name}: Members={Members.Count()}";
    }
}

