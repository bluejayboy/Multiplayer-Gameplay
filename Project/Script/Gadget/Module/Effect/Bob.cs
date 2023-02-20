using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Bob : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private float strength = 0.005f;
        [SerializeField] private float speed = 0.1f;

        private Transform cachedTransform = null;

        private float timer = 0.0f;

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void OnEnable()
        {
            timer = 0;
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            if (!entity.IsAttached)
            {
                return;
            }

            var activeStrength = strength;
            var activeSpeed = speed;

            if (state.Movement.IsGrounded)
            {
                if (state.Horizontal != 0.0f || state.Vertical != 0.0f)
                {
                    activeStrength = strength * state.Movement.CharacterVelocity.magnitude;
                    activeSpeed = speed * state.Movement.CharacterVelocity.magnitude;
                }
            }

            var waveslice = Mathf.Sin(timer);

            timer += activeSpeed;

            if (timer > Mathf.PI * 2.0f)
            {
                timer -= Mathf.PI * 2.0f;
            }

            const float horizontal = 1.0f;
            const float vertical = 1.0f;

            var position = cachedTransform.localPosition;

            if (waveslice != 0.0f)
            {
                float wave = waveslice * activeStrength;
                float axes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);

                axes = Mathf.Clamp(axes, 0.0f, 1.0f);
                wave = axes * wave;
                position.y += wave;
            }
            else
            {
                position.y = 0.0f;
            }

            cachedTransform.localPosition = position;
        }
    }
}