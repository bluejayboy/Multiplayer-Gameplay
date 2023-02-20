using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class TurretRebound : MonoBehaviour
    {
        [SerializeField] private float pitch = 0.1f;
        [SerializeField] private float yaw = 0.1f;
        [SerializeField] private float roll = 0.1f;
        [SerializeField] private float depth = 0.1f;
        [SerializeField] private float kickSpeed = 100.0f;
        [SerializeField] private float returnSpeed = 7.0f;

        private Transform cachedTransform = null;

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void FixedUpdate()
        {
            var time = BoltNetwork.FrameDeltaTime;

            ReturnPosition(time);
            ReturnRotation(time);
        }

        private void ReturnPosition(float time)
        {
            cachedTransform.localPosition = Vector3.Lerp(cachedTransform.localPosition, default(Vector3), returnSpeed * time);
        }

        private void ReturnRotation(float time)
        {
            cachedTransform.localRotation = Quaternion.Lerp(cachedTransform.localRotation, Quaternion.identity, returnSpeed * time);
        }

        public void KickRebound(int serverFrame)
        {
            Random.InitState(serverFrame);

            float randomYaw = Random.Range(-yaw, yaw);
            float randomRoll = Random.Range(-roll, roll);

            cachedTransform.localPosition = Vector3.Lerp(cachedTransform.localPosition, new Vector3(0.0f, 0.0f, -depth), kickSpeed * Time.smoothDeltaTime);
            cachedTransform.localRotation = Quaternion.Lerp(cachedTransform.localRotation, Quaternion.Euler(cachedTransform.localRotation.eulerAngles - new Vector3(pitch, randomYaw, randomRoll)), kickSpeed * BoltNetwork.FrameDeltaTime);
        }
    }
}