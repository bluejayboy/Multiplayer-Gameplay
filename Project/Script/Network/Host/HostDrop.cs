using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour(BoltNetworkModes.Server)]
    public sealed class HostDrop : GlobalEventListener
    {
        public override void OnEvent(DropEvent evnt)
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

            entity.GetComponent<PlayerLoadout>().DropGadget(evnt.Slot, true, true);
        }
    }
}