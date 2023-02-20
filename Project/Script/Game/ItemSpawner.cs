using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ItemSpawner : MonoBehaviour
    {
        [SerializeField] private Item[] itemSpawns;
        [SerializeField] private Transform[] spawnpoints;
        [SerializeField] private int spawnCount = 1;

        private List<BoltEntity> activeItemSpawns = new List<BoltEntity>();
        private List<Transform> activeSpawnpoints = new List<Transform>();

        private bool isDestroyed = false;

        private void Awake()
        {
            if (BoltNetwork.IsClient || string.IsNullOrEmpty(ItemController.Password))
            {
                return;
            }

            InvokeRepeating("SpawnItems", 0.0f, 180.0f);
        }

        private void SpawnItems()
        {
            for (int i = 0; i < activeItemSpawns.Count; i++)
            {
                if (activeItemSpawns[i] != null)
                {
                    BoltNetwork.Destroy(activeItemSpawns[i]);
                }
            }

            activeItemSpawns.Clear();
            activeSpawnpoints.Clear();

            for (int i = 0; i < spawnpoints.Length; i++)
            {
                activeSpawnpoints.Add(spawnpoints[i]);
            }

            for (int i = 0; i < spawnCount; i++)
            {
                for (var j = 0; j < itemSpawns.Length; j++)
                {
                    var randomValue = Random.value;

                    if (randomValue <= itemSpawns[j].chance)
                    {
                        var randomSpawnpoint = activeSpawnpoints[Random.Range(0, activeSpawnpoints.Count)];

                        activeItemSpawns.Add(BoltNetwork.Instantiate(itemSpawns[j].prefab, randomSpawnpoint.localPosition, randomSpawnpoint.localRotation));
                        activeSpawnpoints.Remove(randomSpawnpoint);
                        Debug.LogError(activeSpawnpoints.Count);
                        break;
                    }
                }
            }
        }

        [System.Serializable]
        private class Item
        {
            public BoltEntity prefab;
            public float chance;
        }
    }
}