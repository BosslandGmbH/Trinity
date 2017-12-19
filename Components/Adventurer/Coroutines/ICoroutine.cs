using System;
using System.Threading.Tasks;

namespace Trinity.Components.Adventurer.Coroutines
{
    public interface ICoroutine
    {
        Task<bool> GetCoroutine();

        Guid Id { get; }

        void Reset();

        string StatusText { get; }
    }
}