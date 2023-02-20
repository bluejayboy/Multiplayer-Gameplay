using MEC;
using System.Collections.Generic;
using UnityEngine;
using BeautifyEffect;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class HockeyAbility : Ability
    {
        [SerializeField] private AudioClip healSound = null;
        [SerializeField] private Color32 color;

        private CoroutineHandle handle;
        private Beautify beautify = null;

        protected override void Awake()
        {
            base.Awake();

            beautify = transform.parent.parent.GetComponentInChildren<Beautify>(true);
        }

        public override void Attached()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            base.Attached();

            state.ActiveAbility.OnStartAbility += PlaySound;
        }

        public override void ControlGained()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            base.ControlGained();

            state.AddCallback("ActiveAbility.IsUsingAbility", ApplyHeal);
        }

        public override void Detached()
        {
            if (beautify != null)
            {
                beautify.vignetting = false;
            }

            Timing.KillCoroutines(handle);
        }

        public override void ApplyAbility()
        {
            if (state.ActiveAbility.AbilityTimer < state.ActiveAbility.AbilityRate || state.ActiveAbility.IsUsingAbility)
            {
                return;
            }

            state.ActiveAbility.AbilityTimer = 0.0f;
            state.ActiveAbility.IsUsingAbility = true;
            state.ActiveAbility.StartAbility();

            handle = Timing.RunCoroutine(Heal());
        }

        private void ApplyHeal()
        {
            beautify.vignettingColor = color;
            beautify.vignetting = state.ActiveAbility.IsUsingAbility;
        }

        private void PlaySound()
        {
            audioSource.PlayAudioClip(healSound);
        }

        private IEnumerator<float> Heal()
        {
            state.MainMoveSpeed = 2;

            yield return Timing.WaitForSeconds(5.0f);

            state.MainMoveSpeed = 1;
            state.ActiveAbility.IsUsingAbility = false;
        }
    }
}