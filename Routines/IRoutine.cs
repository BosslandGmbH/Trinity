using System.Threading.Tasks;
using Trinity.Components.Combat;
using Zeta.Game;

namespace Trinity.Routines
{
    public interface IRoutine
    {
        ActorClass Class { get; }
        string Author { get; }
        string DisplayName { get; }
        string Description { get; }

        TrinityPower GetCombatPower();
        TrinityPower GetBuffPower();
        TrinityPower GetDefensivePower();
        TrinityPower GetDestructiblePower();
        TrinityPower GetMovementPower();

        Task<bool> OnHandleTarget();
    }
}



