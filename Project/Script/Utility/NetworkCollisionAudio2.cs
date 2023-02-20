using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class NetworkCollisionAudio2 : EntityBehaviour<IProjectileState>
    {
        [SerializeField] private AudioClip collisionSound = null;

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
            state.OnCollide = () => audioSource.PlayAudioClip(collisionSound);
        }
    }
}