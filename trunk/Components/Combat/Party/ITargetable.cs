using Trinity.Framework.Actors.ActorTypes;
using Zeta.Common;

namespace Trinity.Components.Combat.Party
{
    public interface ITargetable
    {
        int ActorSnoId { get; }
        int AcdId { get; }
        string Name { get; }
        TrinityObjectType Type { get; }
        Vector3 Position { get; }
        int WorldDynamicId { get; }
        float Distance { get; }
    }    
}
