using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Client)]
    public sealed class ClientIgnoreCollider : GlobalEventListener
    {
        [SerializeField] private GameObject explosion;

        public override void OnEvent(IgnoreColliderEvent evnt)
        {
#if ISDEDICATED
                return;
#endif

            var isIgnore = evnt.IsIgnore;
            var player = BoltNetwork.FindEntity(evnt.Player).GetComponent<Player>();
            var collider = BoltNetwork.FindEntity(evnt.Collider).GetComponentInChildren<Collider>();

            player.IgnoreCollider(collider, isIgnore);
        }
    }
}