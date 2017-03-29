using System.Collections.Generic;
using System.Linq;
using Trinity.Framework;
using Trinity.Components.Combat.Resources;
using Trinity.Framework.Actors.ActorTypes;
using Zeta.Common;

namespace Trinity.Components.Combat
{
    public interface IPartyProvider
    {
        IEnumerable<IPartyMember> Members { get; }
        IEnumerable<IPartyMember> Followers { get; }
        IPartyMember Leader { get; }
        ITargetable PriorityTarget { get; }
        Vector3 FightLocation { get; }
    }

    public class DefaultPartyProvider : IPartyProvider
    {
        private IEnumerable<TrinityPlayer> Players => Core.Actors.Actors.OfType<TrinityPlayer>();
        public IEnumerable<IPartyMember> Members => Players;
        public IEnumerable<IPartyMember> Followers => Enumerable.Empty<IPartyMember>();
        public IPartyMember Leader => Players.OrderByDescending(p => p.HitPointsMax).FirstOrDefault();
        public ITargetable PriorityTarget => Leader.Target;
        public Vector3 FightLocation => Leader.Position;

        public override string ToString() => $"{GetType().Name}: Members={Members.Count()}";
    }
}