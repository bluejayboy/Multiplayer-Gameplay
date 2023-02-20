using Photon.Bolt;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetGrenade : GadgetDraw, IPrimaryDown, IPrimaryFireUp, ISecondaryFire, IEnable
    {
        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private BoltEntity projectile = null;
        [SerializeField] private Transform spawn = null;
        [SerializeField] private GrenadeGadgetData data = null;

        public GadgetData Data { get { return data; } }
        private GadgetPose pose;

        private bool isReady;
        private bool isReleased;
        private bool isThrowing;
        private bool isThrown;
        private bool isCanceled;

        private bool isChargingAnimation;

        private CoroutineHandle chargeHandle;
        private CoroutineHandle throwHandle;

        public override void Equip()
        {
            base.Equip();

            pose = pose ?? GetComponent<GadgetPose>();

            Cancel();
        }

        public override void Dequip()
        {
            base.Dequip();

            Cancel();
        }

        private void FixedUpdate()
        {
            if (!entity.IsAttached || !pose.IsDisabled)
            {
                return;
            }

            GadgetData.Pose finalPose = isChargingAnimation ? data.ChargePose : data.ThrowPose;
            float time = isChargingAnimation ? 5 : 10;

            transform.localPosition = Vector3.Slerp(transform.localPosition, finalPose.Position, time * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(finalPose.Rotation), time * Time.fixedDeltaTime);
        }

        public void PrimaryDown()
        {
            if (!state.ActiveGadget.IsDrawn)
            {
                return;
            }

            if (isThrowing)
            {
                return;
            }

            pose.IsDisabled = true;
            isThrowing = true;
            isReady = false;
            isReleased = false;
            isThrown = false;
            isCanceled = false;

            chargeHandle = Timing.RunCoroutine(Charge());
        }

        public void PrimaryUp()
        {
            if (!state.ActiveGadget.IsDrawn)
            {
                return;
            }

            if (isCanceled)
            {
                return;
            }

            isReleased = true;

            if (isReady)
            {
                throwHandle = Timing.RunCoroutine(Throw());
            }
        }

        public void SecondaryDown()
        {
            Cancel();
        }

        private IEnumerator<float> Charge()
        {
            isChargingAnimation = true;

            state.ActiveGadget.PrimaryDown();

            yield return Timing.WaitForSeconds(0.3f);

            if (isCanceled || this == null || !entity.IsAttached)
            {
                yield break;
            }

            isReady = true;

            if (isReleased)
            {
                throwHandle = Timing.RunCoroutine(Throw());
            }
        }

        private IEnumerator<float> Throw()
        {
            if (isThrown)
            {
                yield break;
            }

            isThrown = true;
            isChargingAnimation = false;

            state.ActiveGadget.PrimaryUp();

            yield return Timing.WaitForSeconds(0.2f);

            if (entity.IsOwner)
            {
                BoltEntity projectile = BoltPooler.Instance.Instantiate(this.projectile.PrefabId, null, spawn.position, spawn.rotation, true);

                projectile.GetComponent<Projectile>().AddForce(spawn.forward * 800, state.PenName, entity.Controller.GetPlayerConnection());

                state.OwnedGadgets[data.Slot].ActiveAmmo = -1;
                state.OwnedGadgets[data.Slot].MaxAmmo = -1;
                state.OwnedGadgets[data.Slot].PrefabID = default(PrefabId);
            }

            state.ActiveGadget.Slot = -1;
            state.OwnedGadgets[data.Slot].ID = string.Empty;
        }

        private void Cancel()
        {
            isCanceled = true;
            isReady = false;
            isReleased = false;
            isThrown = false;
            isThrowing = false;
            pose.IsDisabled = false;

            Timing.KillCoroutines(chargeHandle);
            Timing.KillCoroutines(throwHandle);
        }

        public void PlayPrimaryDown()
        {
            audioSource.PlayAudioClip(data.PrimaryFire);
        }

        public void PlayPrimaryUp()
        {
            audioSource.PlayAudioClip(data.PrimaryUp);
        }

        public void PlaySecondaryDown()
        {
            audioSource.PlayAudioClip(data.SecondaryFireSound);
        }
    }
}