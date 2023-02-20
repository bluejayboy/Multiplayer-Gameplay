using UnityEngine;

namespace Scram
{
    public static class ScramAudio
    {
        public static void PlaySelf(this AudioSource audioSource)
        {
            if (audioSource.isActiveAndEnabled)
            {
                audioSource.Play();
            }
        }

        public static void PlayAudioClip(this AudioSource audioSource, AudioClip audioClip)
        {
            if (audioSource.isActiveAndEnabled)
            {
                audioSource.PlayOneShot(audioClip);
            }
        }

        public static void PlayAudioClip(this AudioSource audioSource, AudioClip audioClip, float volume)
        {
            if (audioSource.isActiveAndEnabled)
            {
                audioSource.PlayOneShot(audioClip, volume);
            }
        }
    }
}