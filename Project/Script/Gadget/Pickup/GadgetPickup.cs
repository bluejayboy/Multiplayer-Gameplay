using Photon.Bolt;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetPickup : EntityBehaviour<IPickupState>, IInteractable
    {
        [SerializeField] private GadgetData data = null;
        [SerializeField] private Transform render = null;
        [SerializeField] private GameObject mag = null;
        [SerializeField] private float dropCooldown = 50.0f;

        private BoltEntity dropEntity = null;
        private float dropTimer = 0.0f;
        private int activeAmmo = -1;

        public string Display { get { return data.Display; } }

        private void OnTriggerStay(Collider other)
        {
            if (!entity.IsAttached || !entity.IsOwner || state.HasInteracted)
            {
                return;
            }

            var player = other.GetComponent<Player>();

            if (player == null || player.entity == null || player.state == null || !player.entity.IsAttached)
            {
                return;
            }

            if (!player.state.CanInteract || !string.IsNullOrEmpty(player.state.OwnedGadgets[data.Slot].ID))
            {
                return;
            }

            if (player.entity == dropEntity && dropTimer > BoltNetwork.ServerFrame)
            {
                return;
            }

            Interact(player);
        }

        public override void Attached()
        {
            var token = entity.AttachToken as PickupToken;
            var sword = GetComponentInChildren<GlowingSwords.Scripts.GlowingSword>(true);

            if (sword != null && token != null)
            {
                sword.BladeColor = token.Color;
            }

            state.SetTransforms(state.Transform, transform, render);

            if (mag != null)
            {
                mag.SetActive(token != null ? token.Ammo > 0 : true);
            }

            if (!entity.IsOwner)
            {
                return;
            }

            var rigidbody = GetComponent<Rigidbody>();

            rigidbody.isKinematic = false;

            if (token != null)
            {
                rigidbody.AddForceAtPosition(token.Force, transform.localPosition);
                rigidbody.AddTorque(token.Force);

                activeAmmo = token.Ammo;
                dropEntity = token.DropEntity;
                dropTimer = dropCooldown + BoltNetwork.ServerFrame;
            }
            else
            {
                if (data is HitscanGadgetData)
                {
                    var hitscan = data as HitscanGadgetData;

                    activeAmmo = hitscan.MaxAmmo;
                }
            }
        }

        public override void Detached()
        {
            if (!entity.IsOwner)
            {
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
            }
        }

        public void Interact(Player player)
        {
            if (!entity.IsAttached || !player.entity.IsAttached || state.HasInteracted)
            {
                return;
            }

            state.HasInteracted = true;

            var maxAmmo = -1;

            if (data is HitscanGadgetData)
            {
                var hitscan = data as HitscanGadgetData;

                maxAmmo = hitscan.MaxAmmo;
            }

            var shouldEquip = player.state.ActiveGadget.Slot == -1 || player.state.ActiveGadget.Slot == data.Slot ? true : false;

            player.entity.GetComponent<PlayerLoadout>().PickUpGadget(data.ID, data.Slot, activeAmmo, maxAmmo, shouldEquip);
            BoltGlobalEvent.SendPickup(data.ID, data.Slot, activeAmmo, maxAmmo, player.entity, shouldEquip);
            BoltPooler.Instance.Destroy(entity);
        }
    }
}