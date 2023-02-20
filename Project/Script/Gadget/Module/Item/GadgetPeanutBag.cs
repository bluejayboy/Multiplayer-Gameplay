using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetPeanutBag : GadgetDraw, IPrimaryDown, IPrimaryFireHold, IPrimaryFireUp, ISecondaryFire, IEnable
    {
        [SerializeField] private AudioSource audioSource = null;
        [SerializeField] private PeanutBagGadgetData data = null;

        public GadgetData Data { get { return data; } }
        private GadgetPose pose;

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

            transform.localPosition = Vector3.Slerp(transform.localPosition, data.HealPose.Position, 5 * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(data.HealPose.Rotation), 5 * Time.fixedDeltaTime);
        }

        public override void Attached()
        {
            state.ActiveGadget.MaxHealTime = data.HealTime;
        }

        public void PrimaryDown()
        {

        }

        public void PrimaryHold()
        {
            Heal();
        }

        public void PrimaryUp()
        {
            Cancel();
        }

        public void SecondaryDown()
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

        public void PlaySecondaryDown()
        {
            audioSource.PlayAudioClip(data.SecondaryFireSound);
        }

        private void Heal()
        {
            if (!entity.IsAttached)
            {
                return;
            }

            if (!state.ActiveGadget.IsDrawn)
            {
                return;
            }

            if (state.ActiveHealth >= state.MaxHealth)
            {
                return;
            }

            if (state.ActiveGadget.HealTimer <= 0)
            {
                state.ActiveGadget.PrimaryDown();
            }

            state.IsImmobilized = true;
            pose.IsDisabled = true;
            state.ActiveGadget.HealTimer += BoltNetwork.FrameDeltaTime;

            if (state.ActiveGadget.HealTimer < data.HealTime)
            {
                return;
            }

            state.ActiveGadget.PrimaryUp();

            if (entity.IsOwner)
            {
                state.BriefHealth = 0;
                state.ActiveHealth = state.MaxHealth;
                state.OwnedGadgets[data.Slot].ActiveAmmo = -1;
                state.OwnedGadgets[data.Slot].MaxAmmo = -1;
                state.OwnedGadgets[data.Slot].PrefabID = default(PrefabId);
            }

            state.ActiveGadget.Slot = -1;
            state.OwnedGadgets[data.Slot].ID = string.Empty;

            Cancel();
        }

        private void Cancel()
        {
            state.ActiveGadget.HealTimer = 0.0f;
            state.IsImmobilized = false;
            pose.IsDisabled = false;
        }
    }
}