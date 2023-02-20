using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class NuclearAlarm : EntityBehaviour<IAudioState>
    {
        [SerializeField] private AudioClip alarmSound = null;

        private AudioSource audioSource = null;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public override void Attached()
        {
            state.OnPlaySound = () => audioSource.PlayAudioClip(alarmSound);

            if (HostGameAction.Instance == null)
            {
                return;
            }

            if (GameMode.Instance.Data.HasPreRound)
            {
                HostGameAction.Instance.OnPreRoundStart += state.PlaySound;
            }
            else
            {
                HostGameAction.Instance.OnRoundStart += state.PlaySound;
            }

            HostGameAction.Instance.OnRoundEnd += audioSource.Stop;
        }
    }
}