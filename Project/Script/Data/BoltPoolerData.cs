using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    [CreateAssetMenu(menuName = "Scriptable Objects/Bolt Pooler Data")]
    public sealed class BoltPoolerData : ScriptableObject
    {
        [SerializeField] private BoltEntity[] pools = null;

        public BoltEntity[] Pools { get { return pools; } }

        public BoltEntity[] Weapons;
    }
}