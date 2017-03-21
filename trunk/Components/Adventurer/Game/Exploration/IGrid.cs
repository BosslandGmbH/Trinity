using System;
using System.Collections.Generic;
using Zeta.Common;

namespace Trinity.Components.Adventurer.Game.Exploration
{
    public interface IGrid<T> : IGrid
    {
        T NearestNode { get; }

        T GetNearestNode(Vector3 position);

        List<T> GetNeighbors(T node, int distance = 1);

        List<T> GetNodesInRadius(Vector3 center, Func<T, bool> condition, float maxDistance = 30f, float minDistance = -1f);

        List<T> GetNodesInRadius(Vector3 center, float radius);

        List<T> GetNodesInRadius(T node, float radius);

        T GetNodeInDirection(T node, Direction direction);

        GridPoint ToGridPoint(Vector3 position);

        Vector3 GetWorldPoint(GridPoint gridPoint);

        bool CanRayCast(Vector3 from, Vector3 to);

        bool CanRayWalk(Vector3 from, Vector3 to);
    }

    public interface IGrid
    {
        int WorldDynamicId { get; }

        void Update(ISceneData sceneData);

        void Reset();
    }

    public interface ISceneData
    {
        int WorldDynamicId { get; }
        List<IGroupNode> ExplorationNodes { get; }
    }
}