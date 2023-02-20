using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class NetworkCollisionAudio : EntityBehaviour<IPickupState>
    {
        [SerializeField] private AudioClip collisionSound = null;
        [SerializeField] private GameObject collisionParticle = null;

        private AudioSource audioSource = null;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (entity.IsAttached && entity.IsOwner)
            {
                state.Collide();
            }
        }

        public override void Attached()
        {
            state.OnCollide += () => audioSource.PlayAudioClip(collisionSound);

            if (collisionParticle != null)
            {
                state.OnCollide += () => collisionParticle.SetActive(true);
            }
        }
    }
}