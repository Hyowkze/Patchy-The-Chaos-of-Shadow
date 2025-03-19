using UnityEngine;

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

        void Reset();
        bool IsWalkable(LayerMask unwalkableMask);
        float GetDistance(IPathNode other);
    }

    public class PathNode : IPathNode
    {
        public Vector3 WorldPosition { get; private set; }
        public bool Walkable { get; private set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public IPathNode Parent { get; set; }
        public int FCost => GCost + HCost;

        public void Initialize(bool walkable, Vector3 worldPosition, int gridX, int gridY)
        {
            Walkable = walkable;
            WorldPosition = worldPosition;
            GridX = gridX;
            GridY = gridY;
            Reset();
        }

        public void Reset()
        {
            GCost = int.MaxValue;
            HCost = 0;
            Parent = null;
        }

        public bool IsWalkable(LayerMask unwalkableMask)
        {
            return !Physics2D.OverlapCircle(WorldPosition, 0.5f, unwalkableMask);
        }

        public float GetDistance(IPathNode other)
        {
            int dstX = Mathf.Abs(GridX - other.GridX);
            int dstY = Mathf.Abs(GridY - other.GridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}
