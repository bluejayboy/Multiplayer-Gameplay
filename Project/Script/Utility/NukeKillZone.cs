using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class NukeKillZone : MonoBehaviour
    {
        public bool IsDisabled = false;

        private void OnTriggerEnter(Collider other)
        {
            if (IsDisabled || BoltNetwork.IsClient || !other.CompareTag(ScramConstant.PlayerTag) )
            {
                return;
            }

            var player = other.GetComponent<Player>();

            if (player != null && !GameMode.Instance.GetComponent<EvacuationGameMode>().EscapedHumans.Contains(player.entity.Controller.GetPlayerConnection()))
            {
                player.Die(Vector3.up, "the nuke");
            }
        }
    }
}