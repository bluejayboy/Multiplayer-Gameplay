using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class LoadingRotator : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed = 360.0f;

        private Transform cachedTransform = null;

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void FixedUpdate()
        {
            var speed = rotateSpeed * Time.smoothDeltaTime;

            cachedTransform.Rotate(new Vector3(speed * Vector3.forward.x, speed * Vector3.forward.y, speed * Vector3.forward.z));
        }
    }
}