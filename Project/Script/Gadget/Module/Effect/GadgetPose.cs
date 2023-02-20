using Photon.Bolt;
using UnityEngine;
using System;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetPose : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private Camera worldViewer = null;

        private Transform cachedTransform = null;
        private GadgetData data = null;

        public bool IsDisabled { get; set; }

        private void Awake()
        {
            var split = gameObject.name.Split(new string[] { " FP" }, StringSplitOptions.None);
            var id = split[0];

            data = GetComponentInParent<PlayerLoadout>().Gadgets[id].Data;
            cachedTransform = transform;
        }

        private void OnEnable()
        {
            cachedTransform.localPosition = new Vector3(data.IdlePose.Position.x, data.IdlePose.Position.y - 1.0f, data.IdlePose.Position.z);
            worldViewer.fieldOfView = InputCode.FieldOfView;
        }

        private void OnDisable()
        {
            worldViewer.fieldOfView = InputCode.FieldOfView;
        }

        private void FixedUpdate()
        {
            ApplyPose(Time.fixedDeltaTime);
        }

        private void ApplyPose(float time)
        {
            if (!entity.IsAttached || state.ActiveGadget.IsSlapping || IsDisabled)
            {
                return;
            }

            GadgetData.Pose finalPose = data.IdlePose;

            if (data is HitscanGadgetData)
            {
                var hitscan = data as HitscanGadgetData;

                finalPose = (state.ActiveGadget.IsAiming) ? hitscan.AimPose : hitscan.IdlePose;
                worldViewer.fieldOfView = Mathf.Lerp(worldViewer.fieldOfView, InputCode.FieldOfView / finalPose.FovDivisor, data.PoseSpeed * time);
            }

            cachedTransform.localPosition = Vector3.Lerp(cachedTransform.localPosition, finalPose.Position, data.PoseSpeed * time);
            cachedTransform.localRotation = Quaternion.Lerp(cachedTransform.localRotation, Quaternion.Euler(finalPose.Rotation), data.PoseSpeed * time);
        }
    }
}