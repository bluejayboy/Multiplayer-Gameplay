using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Pooler : MonoBehaviour
    {
        public static Pooler Instance { get; private set; }

        [SerializeField] private PoolerData data;

        private Dictionary<string, Queue<GameObject>> queues = new Dictionary<string, Queue<GameObject>>(100);

        private void Awake()
        {
            Instance = this;

            for (var i = 0; i < data.Pools.Length; i++)
            {
                var queue = new Queue<GameObject>();

                for (var j = 0; j < data.Pools[i].Amount; j++)
                {
                    var instance = Instantiate(data.Pools[i].Prefab, transform);

                    instance.SetActive(false);
                    queue.Enqueue(instance);
                }

                queues.Add(data.Pools[i].Prefab.name, queue);
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public GameObject Pooliate(string id, Vector3 position, Quaternion rotation)
        {
            if (queues.Count <= 0 || !queues.ContainsKey(id))
            {
                return null;
            }

            GameObject pool = queues[id].Dequeue();

            if (pool == null)
            {
                return null;
            }

            pool.SetActive(false);
            pool.SetActive(true);
            pool.transform.SetPositionAndRotation(position, rotation);

            queues[id].Enqueue(pool);

            return pool;
        }

        public GameObject Pooliate(string id)
        {
            if (queues.Count <= 0)
            {
                return null;
            }

            GameObject pool = queues[id].Dequeue();

            pool.SetActive(false);
            pool.SetActive(true);
            pool.transform.SetAsLastSibling();

            queues[id].Enqueue(pool);

            return pool;
        }
    }
}