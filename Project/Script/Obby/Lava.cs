using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Lava : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            if (BoltNetwork.IsClient || !other.gameObject.CompareTag(ScramConstant.PlayerTag))
            {
                return;
            }

            var player = other.gameObject.GetComponent<Player>();

            if (player != null)
            {
                player.Die(Vector3.up, "the world");
            }
        }
    }
}