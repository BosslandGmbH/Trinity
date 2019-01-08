using Trinity.Framework.Objects;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Components.Combat.Resources
{
    public interface ITargetable
    {
        SNOActor ActorSnoId { get; }
        int AcdId { get; }
        string Name { get; }
        TrinityObjectType Type { get; }
        Vector3 Position { get; }
        SNOWorld WorldDynamicId { get; }
        float Distance { get; }
    }
}