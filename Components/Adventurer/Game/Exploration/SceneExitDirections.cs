using System;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    [Flags]
    public enum SceneExitDirections
    {
        Unknown = 0,
        East = 1,
        West = 2,
        North = 4,
        South = 8,
        All = East | West | North | South,
    }
}