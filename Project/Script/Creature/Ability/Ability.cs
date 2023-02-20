using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public abstract class Ability : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private float abilityRate = 10.0f;

        protected AudioSource audioSource = null;

        public float AbilityRate { get { return abilityRate; } }

        public abstract void ApplyAbility();

        protected virtual void Awake()
        {
            audioSource = GetComponentInParent<AudioSource>();
        }

        public void FixedUpdate()
        {
            if (!entity.IsAttached || !entity.IsOwner)
            {
                return;
            }

            if (state.ActiveAbility.AbilityTimer < state.ActiveAbility.AbilityRate)
            {
                state.ActiveAbility.AbilityTimer += BoltNetwork.FrameDeltaTime;
                state.ActiveAbility.AbilityTimer = Mathf.Clamp(state.ActiveAbility.AbilityTimer, 0.0f, state.ActiveAbility.AbilityRate);
            }
        }
    }
}