using System;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class StageCleaner : MonoBehaviour
    {
        [SerializeField] private float seconds = 180.0f; // change that bitch to 10 minutes

        private void Awake()
        {
            seconds = 20.0f; // not going through every scene trying to change variables.
            if (BoltNetwork.IsServer)
            {
                InvokeRepeating("Clean", 0.0f, seconds);
                InvokeRepeating("DeepClean", 0.0f, 360.0f); // deep clean every 20 minutes.
            }
        }

        private void Clean()
        {
            BoltPooler.Instance.Refresh();
        }

        private void DeepClean(){
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}