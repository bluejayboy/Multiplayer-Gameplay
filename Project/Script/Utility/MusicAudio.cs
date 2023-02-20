using UnityEngine;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class MusicAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip[] soundtracks = null;

        private AudioSource audioSource = null;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            ApplySettings();
            audioSource.PlayAudioClip(soundtracks[Random.Range(0, soundtracks.Length)]);
        }

        private void Start()
        {
            GameOptionsMenu.Instance.OnSettingsChanged += ApplySettings;
        }

        private void OnDestroy()
        {
            if (GameOptionsMenu.Instance != null)
            {
                GameOptionsMenu.Instance.OnSettingsChanged -= ApplySettings;
            }
        }

        private void ApplySettings()
        {
            audioSource.volume = ObscuredPrefs.GetFloat("Music Volume");
        }
    }
}