//IPathNode.cs
using UnityEngine;
using System;

namespace Core.Pathfinding
{
    public interface IPathNode
    {
        Vector3 WorldPosition { get; }
        bool Walkable { get; }
        int GridX { get; }
        int GridY { get; }
        int GCost { get; set; }
        int HCost { get; set; }
        IPathNode Parent { get; set; }
        int FCost { get; }
    }

    public class PathNode : IPathNode, IComparable<PathNode>
    {
        public Vector3 WorldPosition { get; private set; }
        public bool Walkable { get; private set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }
        public int GCost { get; set; } = int.MaxValue;
        public int HCost { get; set; } = 0;
        public IPathNode Parent { get; set; }
        public int FCost => GCost + HCost;

        public PathNode(bool walkable, Vector3 worldPosition, int gridX, int gridY)
        {
            Walkable = walkable;
            WorldPosition = worldPosition;
            GridX = gridX;
            GridY = gridY;
        }

        public int CompareTo(PathNode other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0)
            {
                compare = HCost.CompareTo(other.HCost);
            }
            return compare;
        }
    }
}
