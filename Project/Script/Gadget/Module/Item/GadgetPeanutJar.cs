using Photon.Bolt;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetPeanutJar : GadgetDraw, IPrimaryDown, IPrimaryFireUp, IEnable
    {
        public bool doNothing = false;

        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private Player player = null;
        [SerializeField] private PeanutBagGadgetData data = null;

        public GadgetData Data { get { return data; } }
        private GadgetPose pose;
        private bool isHealing;
        private CoroutineHandle handle;

        public override void Equip()
        {
            base.Equip();

            pose = pose ?? GetComponent<GadgetPose>();

            ResetTime();
        }

        public override void Dequip()
        {
            base.Dequip();

            ResetTime();
        }

        private void FixedUpdate()
        {
            if (!entity.IsAttached || !isHealing)
            {
                return;
            }

            transform.localPosition = Vector3.Slerp(transform.localPosition, data.HealPose.Position, 5 * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(data.HealPose.Rotation), 5 * Time.fixedDeltaTime);
        }

        public void PrimaryDown()
        {
            if (isHealing)
            {
                return;
            }

            if (!state.ActiveGadget.IsDrawn)
            {
                return;
            }

            if (!doNothing && state.ActiveHealth >= state.MaxHealth)
            {
                return;
            }

            state.ActiveGadget.PrimaryDown();

            handle = Timing.RunCoroutine(Heal());
        }

        public void PrimaryUp()
        {

        }

        public void PlayPrimaryDown()
        {
            audioSource.PlayAudioClip(data.PrimaryFire);
        }

        public void PlayPrimaryUp()
        {
            audioSource.PlayAudioClip(data.PrimaryUp);
        }

        private void ResetTime()
        {
            Timing.KillCoroutines(handle);
            isHealing = false;
            pose.IsDisabled = false;
        }

        private IEnumerator<float> Heal()
        {
            isHealing = true;
            pose.IsDisabled = true;

            yield return Timing.WaitForSeconds(data.HealTime);

            if (doNothing)
            {
                isHealing = false;
                pose.IsDisabled = false;

                yield break;
            }

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            //state.ActiveGadget.PrimaryUp();

            if (entity.IsOwner)
            {
                state.BriefHealth = state.MaxHealth - state.ActiveHealth;
                player.StartBriefHealth();

                state.OwnedGadgets[data.Slot].ActiveAmmo = -1;
                state.OwnedGadgets[data.Slot].MaxAmmo = -1;
                state.OwnedGadgets[data.Slot].PrefabID = default(PrefabId);
            }

            state.ActiveGadget.Slot = -1;
            state.OwnedGadgets[data.Slot].ID = string.Empty;

            ResetTime();
        }
    }
}