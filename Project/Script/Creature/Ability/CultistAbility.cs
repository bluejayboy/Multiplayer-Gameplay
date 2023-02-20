using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class CultistAbility : Ability
    {
        [SerializeField] private float cloakLifetime = 5.0f;
        [SerializeField] private AudioClip cloakSound = null;
        [SerializeField] private AudioClip uncloakSound = null;
        [SerializeField] private Material firstPersonCloakMaterial = null;
        [SerializeField] private Material thirdPersonCloakMaterial = null;

        private CoroutineHandle handle;
        private PlayerMaterial playerVisual;
        private MaterialChanger[] mats;

        protected override void Awake()
        {
            base.Awake();

            playerVisual = GetComponentInParent<PlayerMaterial>();
        }

        public override void Attached()
        {
            if (playerVisual != null)
            {
                RemoveCloakMaterial();
                playerVisual.SetBodyInvisible(false);
            }

            if (!gameObject.activeSelf)
            {
                return;
            }

            base.Attached();

            state.AddCallback("ActiveAbility.IsUsingAbility", ApplyCloak);
        }

        public override void Detached()
        {
            if (playerVisual != null)
            {
                RemoveCloakMaterial();
                playerVisual.SetBodyInvisible(false);
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
        }

        private void ApplyCloak()
        {
            if (state.ActiveAbility.IsUsingAbility)
            {
                handle = Timing.RunCoroutine(Cloak());
            }
        }

        private void ApplyCloakMaterial(Material material)
        {
#if ISDEDICATED
            return;
#endif

            mats = transform.parent.parent.GetComponentsInChildren<MaterialChanger>(true);

            for (var i = 0; i < mats.Length; i++)
            {
                mats[i].ApplyMaterial(material);
            }
        }

        private void RemoveCloakMaterial()
        {
#if ISDEDICATED
            return;
#endif

            if (mats == null)
            {
                return;
            }

            for (var i = 0; i < mats.Length; i++)
            {
                mats[i].RemoveMaterial();
            }
        }

        private IEnumerator<float> Cloak()
        {
            if (entity.HasControl)
            {
                ApplyCloakMaterial(firstPersonCloakMaterial);
                playerVisual.ApplyFirstPersonMaterial(firstPersonCloakMaterial, true);
            }
            else
            {
                ApplyCloakMaterial(thirdPersonCloakMaterial);
                playerVisual.ApplyThirdPersonMaterial(thirdPersonCloakMaterial);
            }

            audioSource.PlayAudioClip(cloakSound);

            double currentTime = TimeController.GetTime();

            yield return Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime) >= cloakLifetime)
                {
                    return true;
                }

                return false;
            });

            if (this == null || !entity.IsAttached)
            {
                yield break;
            }

            if (entity.IsOwner)
            {
                state.ActiveAbility.IsUsingAbility = false;
            }

            RemoveCloakMaterial();
            playerVisual.SetBodyInvisible(entity.HasControl);
            audioSource.PlayAudioClip(uncloakSound);
        }

        [Serializable]
        private struct Graphic
        {
            public Renderer Renderer { get; set; }
            public Material[] Materials { get; set; }
        }
    }
}