using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class AudioPlayer : SerializedMonoBehaviour
    {
        public static AudioPlayer Instance { get; private set; }

        [SerializeField] private Dictionary<string, AudioClip> audioClips = null;

        private AudioSource audioSource = null;

        private void Awake()
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void PlayAudioClip(AudioClip audioClip)
        {
            audioSource.PlayAudioClip(audioClip);
        }

        public void PlayAudioClip(string audioClipName)
        {
            audioSource.PlayAudioClip(audioClips[audioClipName]);
        }
    }
}