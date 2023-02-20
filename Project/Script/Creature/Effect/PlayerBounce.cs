using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerBounce : MonoBehaviour
    {
        [SerializeField] private float bounceSpeed = 100.0f;
        [SerializeField] private float returnSpeed = 7.0f;

        private Transform cachedTransform = null;

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void OnEnable()
        {
            cachedTransform.localPosition = Vector3.zero;
        }

        private void FixedUpdate()
        {
            ReturnBounce(BoltNetwork.FrameDeltaTime);
        }

        private void ReturnBounce(float time)
        {
            cachedTransform.localPosition = Vector3.Slerp(cachedTransform.localPosition, default(Vector3), returnSpeed * time);
        }

        public void Bounce(float bounce)
        {
            bounce = Mathf.Clamp(bounce, -10.0f, 10.0f);

            cachedTransform.localPosition = Vector3.Slerp(cachedTransform.localPosition, new Vector3(0.0f, cachedTransform.localPosition.y - bounce, 0.0f), bounceSpeed * BoltNetwork.FrameDeltaTime);
        }
    }
}