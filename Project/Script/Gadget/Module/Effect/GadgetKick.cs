using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetKick : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private HitscanGadgetData data = null;
        [SerializeField] private PlayerVision playerVision = null;
        [SerializeField] private PlayerRebound playerRebound = null;

        private Transform cachedTransform = null;

        private void Awake()
        {
            cachedTransform = transform;
        }

        public void KickGadget()
        {
            if (entity.HasControl)
            {
                KickGadget(data.IdleKick);
                playerVision.KickRecoil(data.RecoilPitch, data.RecoilYaw);
            }

            playerRebound.KickRebound(data.ReboundPitch, data.ReboundYaw, data.ReboundRoll, data.ReboundDepth);
        }

        private void KickGadget(HitscanGadgetData.Kick pose)
        {
            var time = Time.smoothDeltaTime;

            cachedTransform.localPosition = Vector3.Lerp(cachedTransform.localPosition, pose.Position, data.KickSpeed * time);
            cachedTransform.localRotation = Quaternion.Lerp(cachedTransform.localRotation, Quaternion.Euler(pose.Rotation), data.KickSpeed * time);
        }
    }
}