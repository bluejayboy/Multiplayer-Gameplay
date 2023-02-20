using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerAnimator : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private GameObject jumpTrail = null;
        [SerializeField] private GameObject landingSmoke = null;
        [SerializeField] private Footprint footprint = null;
        [SerializeField] private float footprintThreshold = 0.22f;

        private Transform cachedTransform = null;
        private Player player = null;
        private CreatureData creatureData = null;
        private Pooler pooler = null;
        private float footstepTimer = 0.0f;
        private bool isLeftFootstep = false;

        private void Awake()
        {
            cachedTransform = transform;
            player = GetComponent<Player>();
        }

        private void Start()
        {
#if ISDEDICATED
                return;
#endif

            pooler = Pooler.Instance;
        }

        public override void Attached()
        {
            var token = entity.AttachToken as PlayerToken;

            if (token == null)
            {
                return;
            }

            creatureData = player.List.Datas[token.Creature];

            state.Effect.OnShakeScreen += () => CameraPlay.EarthQuakeShake(0.5f, 30.0f, 3.0f);

#if ISDEDICATED
                return;
#endif

            state.Effect.OnJump += () => pooler.Pooliate(jumpTrail.name, cachedTransform.position, cachedTransform.rotation);
            state.Effect.OnLandHard += () => pooler.Pooliate(landingSmoke.name, cachedTransform.position, cachedTransform.rotation);
        }

        public void Animate(IPlayerCommandInput input)
        {
            if (state.isFrozen)
            {
                state.IsGrounded = true;
                state.Horizontal = 0;
                state.Vertical = 0;

                return;
            }

            state.IsGrounded = state.Movement.IsGrounded;
            state.Horizontal = 0;
            state.Vertical = 0;

            if (input.Left ^ input.Right)
            {
                state.Horizontal = input.Left ? -1 : 1;
                state.Horizontal = Mathf.Clamp(state.Horizontal * state.Movement.CharacterVelocity.magnitude, -1.0f, 1.0f);
            }

            if (input.Backward ^ input.Forward)
            {
                state.Vertical = input.Backward ? -1 : 1;
                state.Vertical = Mathf.Clamp(state.Vertical * state.Movement.CharacterVelocity.magnitude, -1.0f, 1.0f);
            }

            if (state.Movement.IsGrounded)
            {
                if (state.Horizontal != 0.0f || state.Vertical != 0.0f)
                {
                    if (state.Movement.CharacterVelocity.sqrMagnitude > 0.1f)
                    {
                        if (footstepTimer < BoltNetwork.ServerFrame)
                        {
                            float footstepRate = (state.Movement.IsCrouching) ? creatureData.Crouch.FootstepRate : creatureData.Stand.FootstepRate;

                            footstepTimer = footstepRate + BoltNetwork.ServerFrame;

                            state.Effect.StepFoot();
                        }
                    }
                }
            }

            if (input.Jump && state.Movement.JumpFrames == 29)
            {
                state.Effect.Jump();
            }

            if (!state.IsSpace)
                if (input.Crouch && !state.IsCrouching)
                {
                    state.IsCrouching = true;
                    state.Effect.Crouch();
                }
                else if (!state.Movement.IsCrouching && state.IsCrouching)
                {
                    state.IsCrouching = false;
                    state.Effect.Uncrouch();
                }
        }
    }
}