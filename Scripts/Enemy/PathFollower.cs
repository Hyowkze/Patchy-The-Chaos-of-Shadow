using UnityEngine;
using System.Collections.Generic;

public interface IPathFollower
{
    bool HasReachedCurrentTarget();
    void UpdateTargetIndex();
    Vector2 GetMoveDirection();
}

[RequireComponent(typeof(Transform))]
public class PathFollower : MonoBehaviour, IPathFollower
{
    [SerializeField] private float reachDistance = 0.1f;
    [SerializeField] private bool loopPath = true;
    
    private List<Vector2> path = new List<Vector2>();
    private int currentTargetIndex;
    private Transform cachedTransform;

    private void Awake()
    {
        cachedTransform = transform;
    }

    public bool HasReachedCurrentTarget()
    {
        if (path == null || path.Count == 0) return true;
        return Vector2.Distance(cachedTransform.position, path[currentTargetIndex]) < reachDistance;
    }

    public void UpdateTargetIndex()
    {
        if (!loopPath && currentTargetIndex >= path.Count - 1)
        {
            return;
        }
        currentTargetIndex = (currentTargetIndex + 1) % path.Count;
    }

    public Vector2 GetMoveDirection()
    {
        if (path == null || path.Count == 0) return Vector2.zero;
        return (path[currentTargetIndex] - (Vector2)cachedTransform.position).normalized;
    }
}
