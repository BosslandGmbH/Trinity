using Trinity.Components.Adventurer.Coroutines.CommonSubroutines;
using Trinity.Components.Adventurer.Game.Quests;

namespace Trinity.Components.Adventurer.Coroutines.BountyCoroutines.Subroutines
{
    public interface IBountySubroutine : ISubroutine
    {
        BountyData BountyData { get; }
    }
}
