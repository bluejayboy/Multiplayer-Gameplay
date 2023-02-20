using Photon.Bolt;
using MEC;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class FaceAnimator : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private float loopSpeed = 3.0f;
        [SerializeField] private float animationSpeed = 0.05f;
        [SerializeField] private Texture[] textures = null;
        [SerializeField] private Texture[] damageTextures = null;

        private Material material = null;

        private CoroutineHandle blinkHandle = default(CoroutineHandle);
        private CoroutineHandle damageHandle = default(CoroutineHandle);

        private bool isStopped;

        private void Awake()
        {
#if ISDEDICATED
                return;
#endif

            material = GetComponent<Renderer>().materials[2];
            //blinkHandle = Timing.RunCoroutine(Animate());
        }

        public override void Attached()
        {
#if ISDEDICATED
                return;
#endif

            if (!isStopped)
            {
                state.Effect.OnTakeDamage += RunDamage;
            }
        }

        public void Run()
        {
            Timing.ResumeCoroutines(blinkHandle);
        }

        public void Stop()
        {
            isStopped = true;

            Timing.PauseCoroutines(blinkHandle);
            Timing.KillCoroutines(damageHandle);

            if (entity != null && state != null)
            {
                state.Effect.OnTakeDamage -= RunDamage;
            }
        }

        private void RunDamage()
        {
            Timing.KillCoroutines(damageHandle);
            //damageHandle = Timing.RunCoroutine(Damage());
        }

        private IEnumerator<float> Animate()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(loopSpeed);

                if (this == null)
                {
                    yield break;
                }

                for (var i = 0; i < textures.Length; i++)
                {
                    yield return Timing.WaitForSeconds(animationSpeed);

                    if (this == null)
                    {
                        yield break;
                    }

#if !ISDEDICATED
                    material.mainTexture = textures[i];
#endif
                }
            }
        }

        private IEnumerator<float> Damage()
        {
            Timing.PauseCoroutines(blinkHandle);

            for (var i = 0; i < damageTextures.Length; i++)
            {
                yield return Timing.WaitForSeconds(animationSpeed);

                if (this == null)
                {
                    yield break;
                }

#if !ISDEDICATED
                material.mainTexture = damageTextures[i];
#endif
            }

            Timing.ResumeCoroutines(blinkHandle);
        }
    }
}