using System;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public sealed class HostPlayerAction : MonoBehaviour
    {
        public static HostPlayerAction Instance { get; private set; }

        public Action<Player> OnSpawn { get; set; }
        public Action<PlayerConnection, Vector3, float> OnDeath { get; set; }
        public Action<BoltEntity> TurretDeath { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            OnSpawn = null;
            OnDeath = null;
            TurretDeath = null;
            Instance = null;
        }
    }
}