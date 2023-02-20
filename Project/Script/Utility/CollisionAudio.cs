using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class CollisionAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip collisionSound = null;

        private AudioSource audioSource = null;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            audioSource.PlayAudioClip(collisionSound);
        }
    }
}