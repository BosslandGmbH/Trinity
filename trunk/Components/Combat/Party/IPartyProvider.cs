using System.Collections.Generic;
using Zeta.Common;

namespace Trinity.Components.Combat.Party
{
    public interface IPartyProvider
    {
        IEnumerable<IPartyMember> Members { get; }
        IEnumerable<IPartyMember> Followers { get; }
        IPartyMember Leader { get; }
        ITargetable PriorityTarget { get; }
        Vector3 FightLocation { get; }
    }
}