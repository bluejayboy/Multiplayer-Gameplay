using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PickupSpawner : MonoBehaviour
    {
        [SerializeField] private BoltEntity[] pickups = null;
        public Transform[] spawns = null;

        private void Start()
        {
            if (HostGameAction.Instance == null)
            {
                return;
            }

            if (GameMode.Instance.Data.HasPreRound)
            {
                HostGameAction.Instance.OnPreRoundStart += SpawnAll;
            }
            else
            {
                HostGameAction.Instance.OnRoundStart += SpawnAll;
            }
        }

        private void SpawnAll()
        {
            for (var i = 0; i < spawns.Length; i++)
            {
                BoltEntity randomPickup = pickups[Random.Range(0, pickups.Length)];

                BoltPooler.Instance.Instantiate(randomPickup.PrefabId, null, spawns[i].position, spawns[i].rotation, true);
            }
        }
    }
}