using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class RagdollView : MonoBehaviour
    {
        [SerializeField] private Transform target = null;

        [SerializeField] private float positionSpeed = 5.0f;
        [SerializeField] private Vector3 position = default(Vector3);

        [SerializeField] private float wallCheckRadius = 0.1f;
        [SerializeField] private LayerMask wallLayerMask = default(LayerMask);

        private Transform cachedTransform = null;
        private Transform parent = null;

        private void Awake()
        {
            cachedTransform = transform;
            parent = transform.parent;
        }

        private void LateUpdate()
        {
            ApplyView(Time.smoothDeltaTime);
        }

        private void ApplyView(float time)
        {
            ApplyPitch();
            CheckWall(time);
        }

        private void ApplyPitch()
        {
            parent.LookAt(target);
        }

        private void CheckWall(float time)
        {
            Vector3 direction = cachedTransform.position - parent.position;
            var hit = default(RaycastHit);
            float distance = Mathf.Abs(position.z);

            if (Physics.SphereCast(parent.position, wallCheckRadius, direction, out hit, distance, wallLayerMask, QueryTriggerInteraction.Ignore))
            {
                MoveFromWall(hit, parent.position, direction);
            }
            else
            {
                cachedTransform.localPosition = Vector3.Lerp(cachedTransform.localPosition, position, positionSpeed);
            }
        }

        private void MoveFromWall(RaycastHit hit, Vector3 parentPosition, Vector3 direction)
        {
            cachedTransform.position = parentPosition + (new Vector3(direction.normalized.x * hit.distance, direction.normalized.y * hit.distance, direction.normalized.z * hit.distance));
        }
    }
}