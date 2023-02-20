using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Frigidbody : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private float mass = 3.0f;
        [SerializeField] private float pushStrength = 3.0f;

        private PlayerCommander playerCommander = null;
        private PlayerController playerController = null;

        public Vector3 Force { get; private set; }

        private void Awake()
        {
            playerCommander = GetComponent<PlayerCommander>();
            playerController = GetComponent<PlayerController>();
        }

        public override void Attached()
        {
            Force = Vector3.zero;
            playerCommander.WillKnockBack = false;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!entity.IsOwner)
            {
                return;
            }

            if (hit.collider.attachedRigidbody != null)
            {
                hit.collider.attachedRigidbody.velocity = new Vector3(hit.moveDirection.x * pushStrength, hit.moveDirection.y * pushStrength, hit.moveDirection.z * pushStrength);
            }
        }

        public void AddForce(Vector3 force)
        {
            Force = new Vector3(force.x / mass, force.y / mass, force.z / mass);

            if (entity.HasControl)
            {
                playerCommander.WillKnockBack |= true;
            }
        }

        public void AddDirectForce(Vector3 force)
        {
            Force = new Vector3(force.x / mass, force.y / mass, force.z / mass);

            playerController.KnockBack(true, BoltNetwork.FrameDeltaTime);
        }
    }
}