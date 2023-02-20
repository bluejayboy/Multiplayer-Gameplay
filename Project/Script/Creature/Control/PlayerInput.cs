using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerInput : EntityBehaviour<IPlayerState>
    {
        private PlayerLoadout playerLoadout = null;
        private PlayerInteract playerInteract = null;

        private void Awake()
        {
            playerLoadout = GetComponent<PlayerLoadout>();
            playerInteract = GetComponentInChildren<PlayerInteract>(true);
        }

        private void Update()
        {
            ApplyInput();
        }

        private void ApplyInput()
        {
            if (!entity.IsAttached || !entity.HasControl || ScramInput.IsPausedOrChatting)
            {
                return;
            }

            if (BoltNetwork.IsServer && DebugMode.IsDebug)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    BoltPooler.Instance.Instantiate(BoltPrefabs.Lightsaber_Pickup, null, transform.position, transform.rotation, true);
                }

                if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    BoltPooler.Instance.Instantiate(BoltPrefabs.Sword_Pickup, null, transform.position, transform.rotation, true);
                }

                if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    BoltPooler.Instance.Instantiate(BoltPrefabs.Knife_Pickup, null, transform.position, transform.rotation, true);
                }

                if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    BoltPooler.Instance.Instantiate(BoltPrefabs.Grenade_Pickup, null, transform.position, transform.rotation, true);
                }

                if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    BoltPooler.Instance.Instantiate(BoltPrefabs.AK47_Pickup, null, transform.position, transform.rotation, true);
                }

                if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    BoltPooler.Instance.Instantiate(BoltPrefabs.MAC10_Pickup, null, transform.position, transform.rotation, true);
                }

                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                   BoltNetwork. Instantiate(BoltPrefabs.Nuke, null, new Vector3(0, 50,0), Quaternion.identity);
                }

                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    BoltPooler.Instance.Refresh();
                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    state.IsInvincible = !state.IsInvincible;
                }

                if (Input.GetKeyDown(KeyCode.O))
                {
                    state.unlimitedAmmo = !state.unlimitedAmmo;
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Fart.Create(entity, EntityTargets.Everyone).Send();
            }

            if (state.isFrozen)
            {
                return;
            }

            Interact();
            Throw();
        }

        private void Interact()
        {
            if (!state.CanInteract || playerInteract.GetInteractable() == null || playerInteract.GetInteractable() == playerInteract.GetHoldInteractable())
            {
                return;
            }

            if (Input.GetKeyDown(InputCode.Interact))
            {
                BoltGlobalEvent.SendInteract(entity.NetworkId);
            }
        }

        private void Throw()
        {
            if (Input.GetKeyDown(InputCode.Throw))
            {
                if (!state.CanInteract || state.ActiveGadget.Slot < 0 || string.IsNullOrEmpty(state.OwnedGadgets[state.ActiveGadget.Slot].ID))
                {
                    return;
                }

                BoltGlobalEvent.SendDrop(state.ActiveGadget.Slot, entity.NetworkId);
                playerLoadout.DropGadget(state.ActiveGadget.Slot, true, true);
            }
        }
    }
}