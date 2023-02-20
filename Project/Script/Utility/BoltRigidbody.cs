using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class BoltRigidbody : EntityBehaviour<IMotionState>
    {
        [SerializeField] private Transform render = null;

        private Rigidbody cachedRigidbody = null;
        private Vector3 initialPosition = default(Vector3);
        private Quaternion initialRotation = Quaternion.identity;

        private void Awake()
        {
            cachedRigidbody = GetComponent<Rigidbody>();
            initialPosition = transform.localPosition;
            initialRotation = transform.localRotation;
        }

        private void Start()
        {
            if (HostGameAction.Instance != null)
            {
                HostGameAction.Instance.OnRoundEnd += ResetPositionAndRotation;
            }
        }

        public override void Attached()
        {
            state.SetTransforms(state.Transform, transform, render);

            if (entity.IsOwner)
            {
                cachedRigidbody.isKinematic = false;
            }
        }

        private void ResetPositionAndRotation()
        {
            transform.localPosition = initialPosition;
            transform.localRotation = initialRotation;
        }
    }
}