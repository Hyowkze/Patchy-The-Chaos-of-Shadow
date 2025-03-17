using UnityEngine;

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
