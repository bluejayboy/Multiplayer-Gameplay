using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class TurretRecoil : MonoBehaviour
    {
        [SerializeField] private float pitch = 0.1f;
        [SerializeField] private float yaw = 0.1f;
        [SerializeField] private float kickSpeed = 100.0f;

        private TurretCommander commander = null;

        private void Awake()
        {
            commander = GetComponentInParent<TurretCommander>();
        }

        public void KickRecoil(int serverFrame)
        {
            Random.InitState(serverFrame);

            float randomYawRecoil = Random.Range(-yaw, yaw);

            commander.LerpRotation(pitch, randomYawRecoil, kickSpeed);
        }
    }
}