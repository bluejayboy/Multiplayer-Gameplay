using Photon.Bolt;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetSpank : EntityBehaviour<IPlayerState>, IPrimaryDown, IEnable
    {
        [SerializeField] private ThirdPersonGadgetController thirdPersonGadget = null;

        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private HitscanGadgetData data = null;
        [SerializeField] private GadgetPhysics gadgetPhysics = null;

        private Transform cachedTransform = null;
        private HitscanGadgetData.SecondaryAttack activeSlap = default(HitscanGadgetData.SecondaryAttack);
        private float timer = 0.0f;

        private CoroutineHandle handle;
        private CoroutineHandle handle2;
        private CoroutineHandle handle3;

        private bool isDrawn;

        public GadgetData Data { get { return data; } }

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void FixedUpdate()
        {
            ApplySlap(Time.fixedDeltaTime);
        }

        public void Equip()
        {
            isDrawn = false;
            handle3 = Timing.RunCoroutine(DrawSlap());
        }

        public void Dequip()
        {
            isDrawn = false;

            Timing.KillCoroutines(handle);
            Timing.KillCoroutines(handle2);
            Timing.KillCoroutines(handle3);

            if (entity.IsAttached)
            {
                state.ActiveGadget.IsSlapping = false;
            }

            thirdPersonGadget.IsSlapping = false;
        }

        public void PrimaryDown()
        {
            if (!isDrawn)
            {
                return;
            }

            if (timer > BoltNetwork.ServerFrame)
            {
                return;
            }

            timer = data.SecondaryFireRate + BoltNetwork.ServerFrame;
            state.ActiveGadget.PrimaryDown();

            handle = Timing.RunCoroutine(DealImpact());
        }

        public void PlayPrimaryDown()
        {
            handle2 = Timing.RunCoroutine(thirdPersonGadget.SlapGadget(data.PreSlapDelay, data.PostSlapDelay));
            audioSource.PlayAudioClip(data.SecondaryFireSound);
        }

        private void ApplySlap(float time)
        {
            if (entity.IsAttached && state.ActiveGadget.IsSlapping)
            {
                cachedTransform.localPosition = Vector3.Lerp(cachedTransform.localPosition, activeSlap.Position, activeSlap.Speed * time);
                cachedTransform.localRotation = Quaternion.Lerp(cachedTransform.localRotation, Quaternion.Euler(activeSlap.Rotation), activeSlap.Speed * time);
            }
        }

        private IEnumerator<float> DrawSlap()
        {
            yield return Timing.WaitForSeconds(data.DrawTime);

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            isDrawn = true;
        }

        private IEnumerator<float> DealImpact()
        {
            state.ActiveGadget.IsDrawn = false;
            state.ActiveGadget.IsSlapping = true;
            activeSlap = data.PreSlap;

            yield return Timing.WaitForSeconds(data.PreSlapDelay);

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            activeSlap = data.PostSlap;

            gadgetPhysics.OverlapSphere(data.SlapDamage, data.ImpactRadius, data.ImpactForce);

            yield return Timing.WaitForSeconds(data.PostSlapDelay);

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            state.ActiveGadget.IsSlapping = false;

            yield return Timing.WaitForSeconds(data.DrawTime);

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            state.ActiveGadget.IsDrawn = true;
        }
    }
}