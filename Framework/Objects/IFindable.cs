using Zeta.Common;
using Zeta.Game;

namespace Trinity.Framework.Objects
{
    public interface IFindable
    {
        SNOActor ActorId { get; }
        Vector3 Position { get; }
        SNOWorld WorldSnoId { get; }
        float Distance { get; }
    }
}