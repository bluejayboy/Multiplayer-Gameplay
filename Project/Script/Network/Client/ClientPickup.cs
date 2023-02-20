using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [BoltGlobalBehaviour]
    public sealed class ClientPickup : GlobalEventListener
    {
        public override void OnEvent(PickupEvent evnt)
        {
            var entity = BoltNetwork.FindEntity(evnt.NetworkID);

            if (entity == null)
            {
                return;
            }

            entity.GetComponent<PlayerLoadout>().PickUpGadget(evnt.ID, evnt.Slot, evnt.ActiveAmmo, evnt.MaxAmmo, evnt.WillEquip);
        }
    }
}