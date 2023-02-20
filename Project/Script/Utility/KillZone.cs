using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class KillZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (BoltNetwork.IsClient || !other.CompareTag(ScramConstant.PlayerTag))
            {
                return;
            }

            var player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Die(Vector3.up, "the world");
            }
        }
    }
}