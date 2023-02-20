using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class GadgetFalter : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private float minimumFalter = 0.005f;
        [SerializeField] private float maximumFalter = 0.01f;
        [SerializeField] private float falterSpeed = 10.0f;

        private Transform cachedTransform = null;

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void LateUpdate()
        {
            ApplyFalter(Time.smoothDeltaTime);
        }

        private void ApplyFalter(float time)
        {
            if (!entity.IsAttached || ScramInput.IsPausedOrChatting)
            {
                return;
            }

            var moveX = 0.0f;
            var moveY = 0.0f;

            if (Input.GetKey(InputCode.Left) ^ Input.GetKey(InputCode.Right))
            {
                moveX = Input.GetKey(InputCode.Left) ? -1 : 1;
                moveX = Mathf.Clamp(moveX * state.Movement.CharacterVelocity.magnitude, 0.0f, 1.0f);
            }

            if (Input.GetKey(InputCode.Backward) ^ Input.GetKey(InputCode.Forward))
            {
                moveY = Input.GetKey(InputCode.Backward) ? -1 : 1;
                moveY = Mathf.Clamp(moveY * state.Movement.CharacterVelocity.magnitude, 0.0f, 1.0f);
            }

            float horizontal = (InputCode.MouseX + moveX) * -minimumFalter;
            horizontal = Mathf.Clamp(horizontal, -maximumFalter, maximumFalter);

            float vertical = (InputCode.MouseY + moveY) * -minimumFalter;
            vertical = Mathf.Clamp(vertical, -maximumFalter, maximumFalter);

            var falterPosition = new Vector3(horizontal, vertical, cachedTransform.localPosition.z);

            cachedTransform.localPosition = Vector3.Lerp(cachedTransform.localPosition, falterPosition, falterSpeed * time);
        }
    }
}