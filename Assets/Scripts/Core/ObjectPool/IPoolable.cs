namespace Core.Pooling
{
    public interface IPoolable
    {
        void OnSpawnFromPool();
        void ReturnToPool();
    }
}
