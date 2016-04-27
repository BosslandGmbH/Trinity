using Zeta.Common;

namespace Trinity.Objects
{
    /// <summary>
    /// Lightweight interface for objects moving and targetting
    /// </summary>
    public interface IFindable
    {
        int ActorId { get; }
        Vector3 Position { get; }
        int WorldSnoId { get; }
        float Distance { get; }
    }
}


