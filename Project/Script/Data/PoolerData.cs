using System;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Pooler Data")]
    public sealed class PoolerData : ScriptableObject
    {
        [SerializeField] private Pool[] pools = null;

        public Pool[] Pools { get { return pools; } }

        [Serializable]
        public struct Pool
        {
            [SerializeField] private int amount;
            public int Amount { get { return amount; } }

            [SerializeField] private GameObject prefab;
            public GameObject Prefab { get { return prefab; } }
        }
    }
}