using System.Threading.Tasks;

namespace Trinity.Components.Adventurer.Coroutines.CommonSubroutines
{
    public interface ISubroutine
    {
        bool IsDone { get; }

        Task<bool> GetCoroutine();

        void Reset();

        void DisablePulse();
    }
}