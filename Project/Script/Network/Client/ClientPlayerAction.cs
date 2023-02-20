using System;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour]
    public sealed class ClientPlayerAction : MonoBehaviour
    {
        public static ClientPlayerAction Instance { get; private set; }

        public Action OnSpawn { get; set; }
        public Action OnSpectate { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance.OnSpawn = null;
            Instance.OnSpectate = null;
            Instance = null;
        }
    }
}