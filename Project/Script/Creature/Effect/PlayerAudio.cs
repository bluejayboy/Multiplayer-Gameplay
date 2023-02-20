using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerAudio : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private AudioClip[] footsteps = null;

        [SerializeField] private AudioSource mainAudioSource;
        [SerializeField] private AudioClip jump = null;
        [SerializeField] private AudioClip softLand = null;
        [SerializeField] private AudioClip hardLand = null;
        [SerializeField] private AudioClip crouch = null;
        [SerializeField] private AudioClip uncrouch = null;
        [SerializeField] private AudioClip fallDamage = null;
        [SerializeField] private AudioClip meleeHit = null;
        [SerializeField] private AudioClip pickUp = null;
        [SerializeField] private AudioClip drop = null;
        [SerializeField] private float crouchVolume = 0.2f;

        private Player player = null;
        private CreatureData data = null;

        private void Awake()
        {
            player = GetComponent<Player>();
        }

        public override void Attached()
        {
            var token = entity.AttachToken as PlayerToken;

            if (token == null)
            {
                return;
            }

            data = player.List.Datas[token.Creature];

            state.Effect.OnJump += () => mainAudioSource.PlayAudioClip(jump);
            state.Effect.OnCrouch = () => mainAudioSource.PlayAudioClip(crouch);
            state.Effect.OnUncrouch = () => mainAudioSource.PlayAudioClip(uncrouch);
            state.Effect.OnLandSoft = () => mainAudioSource.PlayAudioClip(softLand);
            state.Effect.OnLandHard += () => mainAudioSource.PlayAudioClip(hardLand);
            state.Effect.OnTakeFallDamage += () => mainAudioSource.PlayAudioClip(fallDamage);
            state.Effect.OnTakeFallDamage += () => mainAudioSource.PlayAudioClip(data.Grunts[Random.Range(0, data.Grunts.Length)]);
            state.Effect.OnTakeDamage += () => mainAudioSource.PlayAudioClip(data.Grunts[Random.Range(0, data.Grunts.Length)]);
            state.Effect.OnMeleeHit = () => mainAudioSource.PlayAudioClip(meleeHit);
            state.Effect.OnPickUp = () => mainAudioSource.PlayAudioClip(pickUp);
            state.Effect.OnDrop = () => mainAudioSource.PlayAudioClip(drop);

            state.Effect.OnStepFoot += () =>
            {
                var finalVolume = (state.Movement.IsCrouching) ? crouchVolume : 1.0f;

                mainAudioSource.PlayAudioClip(footsteps[Random.Range(0, footsteps.Length)], finalVolume);
            };
        }
    }
}