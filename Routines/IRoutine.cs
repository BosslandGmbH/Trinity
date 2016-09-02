using System.Threading.Tasks;
using Trinity.Components.Combat;
using Trinity.Objects;
using Zeta.Common;
using Zeta.Game;

namespace Trinity.Routines
{
    public interface IRoutine
    {
        ActorClass Class { get; }
        string Author { get; }
        string DisplayName { get; }
        string Description { get; }
        Build RequiredBuild { get; }

        TrinityPower GetCombatPower();
        TrinityPower GetBuffPower();
        TrinityPower GetDefensivePower();
        TrinityPower GetDestructiblePower();
        TrinityPower GetMovementPower(Vector3 destination);

        Task<bool> OnHandleTarget();
    }
}



