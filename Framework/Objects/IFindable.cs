using Zeta.Common;

namespace Trinity.Framework.Objects
{
    public interface IFindable
    {
        int ActorId { get; }
        Vector3 Position { get; }
        int WorldSnoId { get; }
        float Distance { get; }
    }
}


