using Photon.Bolt;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class BoltPooler : MonoBehaviour
    {
        public static BoltPooler Instance { get; private set; }

        [SerializeField] private BoltPoolerData data;

        private Dictionary<PrefabId, List<BoltEntity>> vacantPools = new Dictionary<PrefabId, List<BoltEntity>>();
        private List<BoltEntity> refreshPools = new List<BoltEntity>();

        private void Start()
        {
            if (BoltNetwork.IsClient)
            {
                return;
            }

            Instance = this;

            for (var i = 0; i < data.Pools.Length; i++)
            {
                vacantPools.Add(data.Pools[i].PrefabId, new List<BoltEntity>());

                for (var j = 0; j < 5; j++)
                {
                    var newEntity = BoltNetwork.Instantiate(data.Pools[i].PrefabId);

                    BoltNetwork.Detach(newEntity);
                    newEntity.gameObject.SetActive(false);
                    vacantPools[data.Pools[i].PrefabId].Add(newEntity);
                }
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void SpawnWeapons()
        {
            for (int i = 0; i < data.Weapons.Length; i++)
            {
                BoltNetwork.Instantiate(data.Weapons[i], new Vector3(0,10,i), Quaternion.identity);
            }
        }

        public BoltEntity Instantiate(PrefabId id, IProtocolToken token, Vector3 position, Quaternion rotation, bool canRefresh)
        {
            if (id == default(PrefabId) || vacantPools == null || vacantPools.Count <= 0)
            {
                return null;
            }

            if (!vacantPools.ContainsKey(id))
            {
                vacantPools.Add(id, new List<BoltEntity>());
            }

            if (vacantPools[id].Count <= 0)
            {
                var newEntity = BoltNetwork.Instantiate(id);

                BoltNetwork.Detach(newEntity);
                newEntity.gameObject.SetActive(false);
                vacantPools[id].Add(newEntity);
            }

            BoltEntity entity = vacantPools[id][0];

            if (vacantPools[id].Contains(entity))
            {
                vacantPools[id].Remove(entity);
            }

            if (canRefresh && !refreshPools.Contains(entity))
            {
                refreshPools.Add(entity);
            }

            entity.transform.SetPositionAndRotation(position, rotation);
            entity.gameObject.SetActive(true);

            if (token != null)
            {
                BoltNetwork.Attach(entity, token);
            }
            else
            {
                BoltNetwork.Attach(entity);
            }

            return entity;
        }

        public void Destroy(BoltEntity entity)
        {
            if (!entity.IsAttached || vacantPools[entity.PrefabId].Contains(entity))
            {
                return;
            }

            if (entity.IsAttached && entity.IsControlled)
            {
                if (entity.HasControl)
                {
                    entity.ReleaseControl();
                }
                else
                {
                    entity.RevokeControl();
                }
            }

            BoltNetwork.Detach(entity);
            entity.gameObject.SetActive(false);
            vacantPools[entity.PrefabId].Add(entity);
        }

        public void Destroy(BoltEntity entity, int seconds)
        {
            if (!entity.IsAttached || vacantPools[entity.PrefabId].Contains(entity))
            {
                return;
            }

            Timing.RunCoroutine(DelayDestroy(entity, seconds));
        }

        private IEnumerator<float> DelayDestroy(BoltEntity entity, int seconds)
        {
            yield return Timing.WaitForSeconds(seconds);

            if (this == null || entity == null || !entity.IsAttached || vacantPools[entity.PrefabId].Contains(entity))
            {
                yield break;
            }

            Destroy(entity);
        }

        public void Refresh()
        {
            for (int i = 0; i < refreshPools.Count; i++)
            {
                Destroy(refreshPools[i]);
            }

            refreshPools.Clear();
        }
    }
}