using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public sealed class HostInteract : GlobalEventListener
    {
        public override void OnEvent(InteractEvent evnt)
        {
            BoltEntity entity = BoltNetwork.FindEntity(evnt.NetworkID);

            if (entity == null || !entity.IsAttached)
            {
                return;
            }

            if (entity.GetState<IPlayerState>() != null && entity.GetState<IPlayerState>().isFrozen)
            {
                return;
            }

            if (entity.GetState<IPlayerState>().CanInteract)
            {
                var playerInteract = entity.GetComponentInChildren<PlayerInteract>(true);

                playerInteract.Interact();
            }
        }
    }
}