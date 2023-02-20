using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerVision : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private float kickSpeed = 100.0f;

        private Transform cachedTransform = null;
        private PlayerCommander playerCommander = null;

        private void Awake()
        {
            cachedTransform = transform;
            playerCommander = GetComponentInParent<PlayerCommander>();
        }

        private void OnEnable()
        {
            cachedTransform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
            ApplyPitch();
        }

        public void ApplyPitch()
        {
            if (entity.IsAttached)
            {
                cachedTransform.localRotation = Quaternion.Euler(state.Pitch, 0.0f, 0.0f);
            }
        }

        public void KickRecoil(float pitch, float yaw)
        {
            Random.InitState(state.ServerFrame);

            float randomYawRecoil = Random.Range(-yaw, yaw);

            playerCommander.LerpRotation(pitch, randomYawRecoil, kickSpeed);
        }
    }
}