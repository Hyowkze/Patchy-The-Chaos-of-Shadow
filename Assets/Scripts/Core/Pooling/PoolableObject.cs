using UnityEngine;
using Core.Pooling;

namespace Core.Pooling
{
    public abstract class PoolableObject : MonoBehaviour, IPoolable
    {
        protected string poolTag;
        protected bool isActive;

        public virtual void OnSpawnFromPool()
        {
            isActive = true;
        }

        public virtual void ReturnToPool()
        {
            if (!isActive) return;
            
            isActive = false;
            ObjectPool.Instance.ReturnToPool(poolTag, gameObject);
        }

        protected virtual void OnDisable()
        {
            if (isActive)
            {
                ReturnToPool();
            }
        }
    }
}
