using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
using BeautifyEffect;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class NaziAbility : Ability
    {
        [SerializeField] private string gadgetName = "Mutant Luger";
        [SerializeField] private AudioClip naziSound = null;
        [SerializeField] private AudioClip shieldSound = null;
        [SerializeField] private Material shieldMaterial = null;
        [SerializeField] private float shieldLifetime = 5.0f;
        [SerializeField] private float heilTime = 1.0f;
        [SerializeField] private Color32 color;

        private Transform cachedParent = null;
        private AudioSource newAudioSource = null;
        private PlayerMaterial playerVisual = null;
        private GadgetHeil firstPersonHeil = null;
        private GadgetHeil thirdPersonHeil = null;
        private Beautify beautify = null;
        private Hitbox[] hitboxes = null;
        private MaterialChanger[] mats;

        private CoroutineHandle handle;
        private CoroutineHandle handle2;

        protected override void Awake()
        {
            var gadget = GetComponentInParent<PlayerLoadout>().Gadgets[gadgetName];

            cachedParent = transform.parent.parent;
            audioSource = cachedParent.GetComponent<AudioSource>();
            newAudioSource = GetComponent<AudioSource>();
            playerVisual = GetComponentInParent<PlayerMaterial>();
            firstPersonHeil = gadget.FirstPerson.GetComponent<GadgetHeil>();
            thirdPersonHeil = gadget.ThirdPerson.GetComponent<GadgetHeil>();
            beautify = cachedParent.GetComponentInChildren<Beautify>(true);
            hitboxes = cachedParent.GetComponentsInChildren<Hitbox>(true);
        }

        public override void Attached()
        {
            if (playerVisual != null)
            {
                RemoveMaterial();
                playerVisual.SetBodyInvisible(false);
            }

            if (!gameObject.activeSelf)
            {
                return;
            }

            base.Attached();

            state.AddCallback("ActiveAbility.IsUsingAbility", ApplyShield);
            state.AddCallback("IsInvincible", ApplyMat);
        }

        public override void Detached()
        {
            if (playerVisual != null)
            {
                RemoveMaterial();
                playerVisual.SetBodyInvisible(false);
            }

            if (beautify != null)
            {
                beautify.vignetting = false;
            }

            if (hitboxes != null)
            {
                for (var i = 0; i < hitboxes.Length; i++)
                {
                    hitboxes[i].gameObject.tag = "Flesh";
                }
            }

            Timing.KillCoroutines(handle);
            Timing.KillCoroutines(handle2);
        }

        public override void ApplyAbility()
        {
            if (state.ActiveAbility.AbilityTimer < state.ActiveAbility.AbilityRate || state.ActiveAbility.IsUsingAbility)
            {
                return;
            }

            state.IsImmobilized = true;
            state.ActiveAbility.AbilityTimer = 0.0f;
            state.ActiveAbility.IsUsingAbility = true;
        }

        private void ApplyShield()
        {
            if (state.ActiveAbility.IsUsingAbility)
            {
                handle = Timing.RunCoroutine(Heil());
            }
        }

        private void ApplyMat()
        {
            if (state.IsInvincible)
            {
                ApplyMaterial();

                for (var i = 0; i < hitboxes.Length; i++)
                {
                    hitboxes[i].gameObject.tag = "Metal";
                }

                if (entity.HasControl)
                {
                    beautify.vignettingColor = color;
                    beautify.vignetting = true;
                    playerVisual.ApplyFirstPersonMaterial(shieldMaterial, false);
                }
                else
                {
                    playerVisual.ApplyThirdPersonMaterial(shieldMaterial);
                }

                audioSource.PlayAudioClip(shieldSound);
            }
            else
            {
                RemoveMaterial();
                playerVisual.SetBodyInvisible(entity.HasControl);

                for (var i = 0; i < hitboxes.Length; i++)
                {
                    hitboxes[i].gameObject.tag = "Flesh";
                }

                if (entity.HasControl)
                {
                    beautify.vignetting = false;
                }
            }
        }

        private void ApplyMaterial()
        {
#if ISDEDICATED
            return;
#endif

            mats = transform.parent.parent.GetComponentsInChildren<MaterialChanger>(true);

            for (var i = 0; i < mats.Length; i++)
            {
                mats[i].ApplyMaterial(shieldMaterial);
            }
        }

        private void RemoveMaterial()
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

        private IEnumerator<float> Heil()
        {
            if (entity.IsControllerOrOwner)
            {
                handle2 = Timing.RunCoroutine(firstPersonHeil.Heil());
            }
            else
            {
                handle2 = Timing.RunCoroutine(thirdPersonHeil.Heil());
            }

            newAudioSource.PlayAudioClip(naziSound);

            double currentTime = TimeController.GetTime();

            yield return Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime) >= heilTime)
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
                state.IsInvincible = true;
            }

            double currentTime2 = TimeController.GetTime();

            yield return Timing.WaitUntilTrue(() =>
            {
                if (TimeController.GetTime(currentTime2) >= shieldLifetime)
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
                state.IsInvincible = false;
                state.IsImmobilized = false;
            }
        }

        [Serializable]
        private struct Graphic
        {
            public Renderer Renderer { get; set; }
            public Material[] Materials { get; set; }
        }
    }
}