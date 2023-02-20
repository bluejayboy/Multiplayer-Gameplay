using System;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    //[BoltGlobalBehaviour]
    public sealed class ScramUpdate : MonoBehaviour
    {
        public static ScramUpdate Instance { get; private set; }

        public Action OnUpdate { get; set; }
        public Action OnFixedUpdate { get; set; }
        public Action<float> OnSmoothDeltaTimeUpdate { get; set; }
        public Action<float> OnFixedDeltaTimeUpdate { get; set; }
        public Action<float> OnFrameDeltaTimeUpdate { get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Update()
        {
            if (OnUpdate != null)
            {
                OnUpdate.Invoke();
            }
        }

        private void LateUpdate()
        {
            if (OnSmoothDeltaTimeUpdate != null)
            {
                OnSmoothDeltaTimeUpdate.Invoke(Time.smoothDeltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (OnFixedUpdate != null)
            {
                OnFixedUpdate.Invoke();
            }

            if (OnFixedDeltaTimeUpdate != null)
            {
                OnFixedDeltaTimeUpdate.Invoke(Time.fixedDeltaTime);
            }

            if (OnFrameDeltaTimeUpdate != null)
            {
                OnFrameDeltaTimeUpdate.Invoke(BoltNetwork.FrameDeltaTime);
            }
        }
    }
}