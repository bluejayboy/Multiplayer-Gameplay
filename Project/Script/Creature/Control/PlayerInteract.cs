using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerInteract : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private float interactDistance = 2.0f;
        [SerializeField] private LayerMask interactLayerMask = default(LayerMask);

        private Transform cachedTransform = null;
        private Player player = null;
        private IHoldInteractable activeHoldInteractable = null;

        private void Awake()
        {
            cachedTransform = transform;
            player = GetComponentInParent<Player>();
        }

        private void OnEnable()
        {
            activeHoldInteractable = null;
        }

        private void Update()
        {
            DisplayInteract();
        }

        public override void Detached()
        {
            ReleaseInteract();
        }

        private void DisplayInteract()
        {
#if ISDEDICATED
                return;
#endif

            if (!entity.IsAttached || !state.CanInteract)
            {
                return;
            }

            if (!entity.HasControl)
            {
                return;
            }

            IInteractable interactable = GetInteractable();

            PlayerHud.Instance.SetInteractText((interactable != null && !string.IsNullOrEmpty(interactable.Display)) ? "(" + InputCode.Interact.ToString() + ") " + interactable.Display : string.Empty);
        }

        public void Interact()
        {
            if (!entity.IsAttached || !entity.IsOwner || !state.CanInteract)
            {
                return;
            }

            IInteractable interactable = GetInteractable();

            if (interactable != null && interactable != GetHoldInteractable())
            {
                interactable.Interact(player);
            }
        }

        public void HoldInteract()
        {
            if (!entity.IsAttached || !entity.IsOwner || !state.CanInteract)
            {
                return;
            }

            IHoldInteractable holdInteractable = GetHoldInteractable();

            if (activeHoldInteractable == null || activeHoldInteractable == holdInteractable)
            {
                if (holdInteractable != null && (holdInteractable.ActivePlayer == null || holdInteractable.ActivePlayer == player || (holdInteractable.ActivePlayer != null && !holdInteractable.ActivePlayer.entity.IsAttached)))
                {
                    activeHoldInteractable = holdInteractable;

                    if (activeHoldInteractable != null)
                    {
                        activeHoldInteractable.Interact(player);
                    }
                }
            }
            else
            {
                ReleaseInteract();
            }
        }

        public void ReleaseInteract()
        {
            if (!entity.IsAttached || !entity.IsOwner || !state.CanInteract)
            {
                return;
            }

            if (activeHoldInteractable != null)
            {
                activeHoldInteractable.Uninteract();
            }

            activeHoldInteractable = null;
            state.InteractTimer = 0.0f;
        }

        public IInteractable GetInteractable()
        {
            if (!state.CanInteract)
            {
                return null;
            }

            var ray = new Ray(cachedTransform.position, cachedTransform.forward);
            var hit = default(RaycastHit);

            if (Physics.Raycast(ray, out hit, interactDistance, interactLayerMask))
            {
                return hit.collider.GetComponentInParent<IInteractable>();
            }
            else
            {
                return null;
            }
        }

        public IHoldInteractable GetHoldInteractable()
        {
            if (!state.CanInteract)
            {
                return null;
            }

            var ray = new Ray(cachedTransform.position, cachedTransform.forward);
            var hit = default(RaycastHit);

            if (Physics.Raycast(ray, out hit, interactDistance, interactLayerMask))
            {
                return hit.collider.GetComponentInParent<IHoldInteractable>();
            }
            else
            {
                return null;
            }
        }
    }
}