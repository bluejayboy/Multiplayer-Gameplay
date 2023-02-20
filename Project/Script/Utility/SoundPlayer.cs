using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] sounds;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            for (var i = 0; i < sounds.Length; i++)
            {
                audioSource.PlayAudioClip(sounds[i]);
            }
        }
    }
}