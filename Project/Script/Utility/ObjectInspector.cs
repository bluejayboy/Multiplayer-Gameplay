using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ObjectInspector : MonoBehaviour
    {
        [SerializeField] private float deccelerationRate = 0.5f;
        [SerializeField] private float speed = 500.0f;

        private Transform cachedTransform = null;
        private float deccelerationTimer = 0.0f;
        private float mouse;

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void OnMouseDrag()
        {
            cachedTransform.Rotate(Vector3.up, speed * -InputCode.MouseX * Mathf.Deg2Rad);
        }

        private void OnMouseUp()
        {
            mouse = -InputCode.MouseX;
            deccelerationTimer = deccelerationRate;
        }

        private void LateUpdate()
        {
            if (deccelerationTimer > 0.0f)
            {
                cachedTransform.Rotate(Vector3.up, speed * deccelerationTimer * mouse * Mathf.Deg2Rad);

                deccelerationTimer -= Time.smoothDeltaTime;
            }
        }
    }
}