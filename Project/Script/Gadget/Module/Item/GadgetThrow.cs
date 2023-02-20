using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetThrow : EntityBehaviour<IPlayerState>, ISecondaryFire
    {
        [SerializeField] private Transform spawn = null;
        [SerializeField] private GadgetData data = null;
        [SerializeField] private AudioSource audioSource = null;

        public void SecondaryDown()
        {
            if (!state.ActiveGadget.IsDrawn)
            {
                return;
            }

            if (entity.IsOwner)
            {
                var token = new PickupToken
                {
                    Force = default,
                    Ammo = -1,
                    DropEntity = entity.IsAttached ? entity : null,
                    Color = state.Color
                };

                BoltEntity projectile = BoltPooler.Instance.Instantiate(BoltPrefabs.Knife_Pickup, token, spawn.position, spawn.rotation, true);

                projectile.GetComponent<KnifeProjectile>().AddForce(spawn.forward * 1500, state.PenName, entity.Controller.GetPlayerConnection());

                state.OwnedGadgets[data.Slot].ActiveAmmo = -1;
                state.OwnedGadgets[data.Slot].MaxAmmo = -1;
                state.OwnedGadgets[data.Slot].PrefabID = default(PrefabId);
            }

            state.ActiveGadget.Slot = -1;
            state.OwnedGadgets[data.Slot].ID = string.Empty;
            state.ActiveGadget.SecondaryDown();
            state.AltMoveSpeed = 1;
        }

        public void PlaySecondaryDown()
        {
            audioSource.PlayAudioClip(data.SecondaryFireSound);
        }
    }
}