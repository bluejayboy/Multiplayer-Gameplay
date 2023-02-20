using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Bolt;
using MEC;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class ThirdPersonGadgetController : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private Transform gadget = null;
        [SerializeField] private Vector3 standPosition = default(Vector3);
        [SerializeField] private Vector3 crouchPosition = default(Vector3);

        [SerializeField] private Slap preSlap = default(Slap);
        [SerializeField] private Slap postSlap = default(Slap);

        [SerializeField] private Slap preBlock = default(Slap);
        [SerializeField] private Slap postBlock = default(Slap);

        [SerializeField] private float recoil = 1.0f;
        [SerializeField] private float kickSpeed = 100.0f;
        [SerializeField] private float returnSpeed = 10.0f;

        private Transform cachedTransform = null;
        private Slap activeSlap = default(Slap);

        public bool IsSlapping { get; set; }
        public bool IsBlocking { get; set; }

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void OnDisable()
        {
            IsSlapping = false;
        }

        private void FixedUpdate()
        {
            ApplyPitch(Time.fixedDeltaTime);
        }

        private void ApplyPitch(float time)
        {
            if (!entity.IsAttached)
            {
                return;
            }

            var activePosition = (state.Movement.IsCrouching) ? crouchPosition : standPosition;
            var finalPitch = state.DisableArmLook ? Mathf.Clamp(state.Pitch, -25, 25) : state.Pitch;

            cachedTransform.localPosition = Vector3.Lerp(cachedTransform.localPosition, activePosition, returnSpeed * time);
            cachedTransform.localRotation = Quaternion.Euler(finalPitch, 0.0f, 0.0f);

            if (IsSlapping)
            {
                gadget.localPosition = Vector3.Lerp(gadget.localPosition, activeSlap.Position, activeSlap.Speed * time);
                gadget.localRotation = Quaternion.Lerp(gadget.localRotation, Quaternion.Euler(activeSlap.Rotation), activeSlap.Speed * time);
            }
            else
            {
                gadget.localPosition = Vector3.Lerp(gadget.localPosition, default(Vector3), returnSpeed * time);
                gadget.localRotation = Quaternion.Lerp(gadget.localRotation, Quaternion.identity, returnSpeed * time);
            }
        }

        public void Recoil()
        {
            if (entity.IsAttached)
            {
                gadget.localPosition = Vector3.Lerp(gadget.localPosition, new Vector3(0.0f, 0.0f, -recoil), kickSpeed * Time.smoothDeltaTime);
            }
        }

        public IEnumerator<float> BlockGadget(float preDelay, float postDelay)
        {
            IsSlapping = true;

            activeSlap = preBlock;

            yield return Timing.WaitForSeconds(preDelay);

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            activeSlap = preBlock;

            yield return Timing.WaitForSeconds(postDelay);

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            IsSlapping = false;
        }

        public IEnumerator<float> SlapGadget(float preDelay, float postDelay)
        {
            IsSlapping = true;

            activeSlap = preSlap;

            yield return Timing.WaitForSeconds(preDelay);

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            activeSlap = postSlap;

            yield return Timing.WaitForSeconds(postDelay);

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            IsSlapping = false;
        }

        [Serializable]
        private struct Slap
        {
            [SerializeField] private Vector3 position;
            [SerializeField] public Vector3 Position { get { return position; } }

            [SerializeField] private Vector3 rotation;
            [SerializeField] public Vector3 Rotation { get { return rotation; } }

            [SerializeField] private float speed;
            public float Speed { get { return speed; } }
        }
    }
}