using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class TeleZone : MonoBehaviour
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
                ObbyGameMode.ChildInstance.Teleport(player.entity.Controller.GetPlayerConnection());
            }
        }
    }
}