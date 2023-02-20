using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Vehicle : EntityBehaviour<IVehicleState>
    {
        [SerializeField] private Transform render;

        private void OnTriggerEnter(Collider other)
        {
            var playerEntity = other.GetComponent<BoltEntity>();

            if (playerEntity == null || !playerEntity.IsAttached || !playerEntity.IsControllerOrOwner)
            {
                return;
            }

            other.transform.SetParent(render, true);
            playerEntity.GetState<IPlayerState>().IsSpace = false;
        }

        private void OnTriggerExit(Collider other)
        {
            var playerEntity = other.GetComponent<BoltEntity>();

            if (playerEntity == null || !playerEntity.IsAttached || !playerEntity.IsControllerOrOwner)
            {
                return;
            }

            other.transform.SetParent(null, true);
            playerEntity.GetState<IPlayerState>().IsSpace = true;
        }
    }
}