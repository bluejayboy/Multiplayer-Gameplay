using System;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public sealed class HostGameAction : MonoBehaviour
    {
        public static HostGameAction Instance { get; private set; }

        public Action OnIntermissionStart { get; set; }
        public Action OnIntermissionEnd { get; set; }

        public Action OnPreRoundStart { get; set; }
        public Action OnRoundStart { get; set; }
        public Action OnRoundPreEnd { get; set; }
        public Action OnRoundEnd { get; set; }

        public Action OnTimerEnd { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            OnIntermissionStart = null;
            OnIntermissionEnd = null;
            OnPreRoundStart = null;
            OnRoundStart = null;
            OnRoundPreEnd = null;
            OnRoundEnd = null;
            OnTimerEnd = null;
            Instance = null;
        }
    }
}