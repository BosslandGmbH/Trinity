namespace Trinity.Components.Adventurer.Coroutines.CommonSubroutines
{
    public interface ISubroutine : ICoroutine
    {
        bool IsDone { get; }

        void DisablePulse();
    }
}