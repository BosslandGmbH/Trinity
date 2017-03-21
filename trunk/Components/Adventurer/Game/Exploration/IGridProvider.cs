namespace Trinity.Components.Adventurer.Game.Exploration
{
    public interface IGridProvider
    {
        void Update();

        IGrid<INode> Navigation { get; }
        IGrid<INode> Exploration { get; }
    }
}