using UnityEngine;
using System.Collections.Generic;

namespace Core.Pooling
{
    public class ObjectPool : MonoBehaviour
    {
        private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

        public static ObjectPool Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void CreatePool(string tag, GameObject prefab, int size)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            prefabDictionary[tag] = prefab;

            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary[tag] = objectPool;
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return null;
            }

            Queue<GameObject> objectPool = poolDictionary[tag];

            if (objectPool.Count == 0)
            {
                GameObject prefab = prefabDictionary[tag];
                GameObject newObj = Instantiate(prefab);
                objectPool.Enqueue(newObj);
            }

            GameObject objectToSpawn = objectPool.Dequeue();
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            IPoolable poolable = objectToSpawn.GetComponent<IPoolable>();
            poolable?.OnSpawnFromPool();

            return objectToSpawn;
        }

        public void ReturnToPool(string tag, GameObject objectToReturn)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return;
            }

            objectToReturn.SetActive(false);
            poolDictionary[tag].Enqueue(objectToReturn);
        }
    }

    public class ObjectPool<T> where T : class
    {
        private readonly System.Func<T> createFunc;
        private readonly System.Action<T> actionOnGet;
        private readonly Queue<T> pool;
        private readonly int maxSize;

        public ObjectPool(System.Func<T> createFunc, System.Action<T> actionOnGet = null, int initialSize = 10)
        {
            this.createFunc = createFunc;
            this.actionOnGet = actionOnGet;
            this.maxSize = initialSize * 2;
            pool = new Queue<T>(initialSize);

            for (int i = 0; i < initialSize; i++)
            {
                pool.Enqueue(createFunc());
            }
        }

        public T Get()
        {
            T item = pool.Count > 0 ? pool.Dequeue() : createFunc();
            actionOnGet?.Invoke(item);
            return item;
        }

        public void Return(T item)
        {
            if (pool.Count < maxSize)
            {
                pool.Enqueue(item);
            }
        }
    }
}
