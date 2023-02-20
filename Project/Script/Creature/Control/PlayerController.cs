using Photon.Bolt;
using System;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private Animator animator = null;
        [SerializeField] private Transform render = null;
        [SerializeField] private Transform vision = null;
        [SerializeField] private Pose stand = default(Pose);
        [SerializeField] private Pose crouch = default(Pose);
        [SerializeField] private float poseSpeed = 10.0f;
        [SerializeField] private float stepOffset = 0.4f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpForce = 5.0f;
        [SerializeField] private int fallDamage = 5;
        [SerializeField] private float fallDamageThreshold = 6.0f;
        [SerializeField] private float softLandThreshold = 1.0f;
        [SerializeField] private float hardLandThreshold = 2.0f;
        [SerializeField] private float IgnoreFallDamageForce = 15.0f;
        [SerializeField] private LayerMask originalEnvironmentLayerMask = default(LayerMask);
        [SerializeField] private LayerMask environmentLayerMask = default(LayerMask);

        private Transform cachedTransform = null;
        private Player player;
        private Frigidbody frigidbody = null;
        private CharacterController characterController = null;
        private PlayerBounce playerBounce = null;
        private Transform playerBounceTransform = null;
        private PlayerRebound playerRebound = null;
        private IDamageable damageable = null;
        private CreatureData creatureData = null;
        private Pose activePose = default(Pose);
        private float altitude = 0.0f;

        private float ySpeed = 0.0f;

        private float jumpSpeed;
        private float crouchSpeed;

        public LayerMask EnvironmentLayerMask { get { return environmentLayerMask; } set { environmentLayerMask = value; } }

        private void Awake()
        {
            cachedTransform = transform;
            player = GetComponent<Player>();
            frigidbody = GetComponent<Frigidbody>();
            characterController = GetComponent<CharacterController>();
            playerBounce = GetComponentInChildren<PlayerBounce>(true);
            playerBounceTransform = playerBounce.transform;
            playerRebound = GetComponentInChildren<PlayerRebound>(true);
            damageable = GetComponent<IDamageable>();
            activePose = stand;
            altitude = cachedTransform.position.y;
        }

        public override void Attached()
        {
            activePose = stand;
            altitude = cachedTransform.position.y;

            var token = entity.AttachToken as PlayerToken;

            if (token == null)
            {
                return;
            }

            creatureData = player.List.Datas[token.Creature];
            stand.MoveSpeed = creatureData.Stand.MoveSpeed;
            crouch.MoveSpeed = creatureData.Crouch.MoveSpeed;

            gameObject.layer = LayerMask.NameToLayer(token.TeamLayer + " Collider");
            environmentLayerMask &= ~(1 << LayerMask.NameToLayer(token.TeamLayer + " Collider"));

            state.SetTransforms(state.Transform, cachedTransform, render);
            state.SetAnimator(animator);
            state.AddCallback("Movement.ColliderCenter", UpdateCrouch);
            state.AddCallback("Movement.ColliderHeight", UpdateCrouch);

            if (!entity.IsOwner)
            {
                return;
            }

            state.playerGravity = 1;
            state.MainMoveSpeed = 1;
            state.AltMoveSpeed = 1;

            if (!CanUncrouch())
            {
                state.Movement.IsCrouching = true;
                activePose = crouch;
                vision.localPosition = activePose.VisionCenter;
                state.Movement.ColliderCenter = activePose.ColliderCenter;
                state.Movement.ColliderHeight = activePose.ColliderHeight;
                characterController.center = state.Movement.ColliderCenter;
                characterController.height = state.Movement.ColliderHeight;
            }
        }

        public override void Detached()
        {
            environmentLayerMask = originalEnvironmentLayerMask;
        }

        public override void ControlGained()
        {
            state.SetTransforms(state.Transform, cachedTransform);

            if (!CanUncrouch())
            {
                state.Movement.IsCrouching = true;
                activePose = crouch;
                vision.localPosition = activePose.VisionCenter;
                state.Movement.ColliderCenter = activePose.ColliderCenter;
                state.Movement.ColliderHeight = activePose.ColliderHeight;
                characterController.center = state.Movement.ColliderCenter;
                characterController.height = state.Movement.ColliderHeight;
            }
        }

        private void UpdateCrouch()
        {
            if (entity.IsControllerOrOwner)
            {
                return;
            }

            characterController.center = state.Movement.ColliderCenter;
            characterController.height = state.Movement.ColliderHeight;
        }

        public MovementObject GetMovement(IPlayerCommandInput input)
        {
            if (!state.isFrozen)
            {
                float time = BoltNetwork.FrameDeltaTime;

                Jump(input.Jump, time);
                KnockBack(input.KnockBack, time);
                Crouch(input.Crouch, time);
                Walk(input.Left, input.Right, input.Backward, input.Forward, input.Yaw);
                CheckTunneling();
                Move(state.Movement.Velocity, time);

                cachedTransform.rotation = Quaternion.Euler(0.0f, input.Yaw, 0.0f);
            }
            else
            {
                bool isGrounded = false;

                isGrounded = isGrounded || characterController.isGrounded;
                isGrounded = isGrounded || Physics.CheckSphere(GetBottomPosition(), characterController.radius, environmentLayerMask, QueryTriggerInteraction.Ignore);

                if (isGrounded && !state.Movement.IsGrounded)
                {
                    state.Movement.Velocity = default(Vector3);
                }

                state.Movement.LocalPosition = cachedTransform.localPosition;
                state.Movement.Position = cachedTransform.position;
                state.Movement.CharacterVelocity = characterController.velocity;
                state.Movement.IsGrounded = isGrounded;
            }

            return state.Movement;
        }

        public void SetMovement(IPlayerCommandResult result)
        {
            state.Movement.LocalPosition = result.LocalPosition;
            state.Movement.Position = result.Position;
            state.Movement.Velocity = result.Velocity;
            state.Movement.JumpFrames = result.JumpFrames;
            state.Movement.ForceFrames = result.ForceFrames;
            state.Movement.IsGrounded = result.IsGrounded;
            state.Movement.IsCrouching = result.IsCrouching;
            state.Movement.StepOffset = result.StepOffset;
            state.Movement.ColliderCenter = result.ColliderCenter;
            state.Movement.ColliderHeight = result.ColliderHeight;

            if (cachedTransform.parent != null && state.Movement.Position != default(Vector3) && Vector3.Distance(state.Movement.Position, state.Movement.LocalPosition) > 2.0f)
            {
                cachedTransform.localPosition = state.Movement.LocalPosition;
            }
            else
            {
                cachedTransform.position = state.Movement.Position;
            }

            characterController.stepOffset = state.Movement.StepOffset;
            characterController.center = state.Movement.ColliderCenter;
            characterController.height = state.Movement.ColliderHeight;
        }

        private void Move(Vector3 velocity, float time)
        {
            bool isGrounded = false;

            isGrounded = isGrounded || characterController.Move(new Vector3(velocity.x * time, velocity.y * time, velocity.z * time)) == CollisionFlags.Below;
            isGrounded = isGrounded || characterController.isGrounded;
            isGrounded = isGrounded || Physics.CheckSphere(GetBottomPosition(), characterController.radius, environmentLayerMask, QueryTriggerInteraction.Ignore);

            if (isGrounded && !state.Movement.IsGrounded)
            {
                state.Movement.Velocity = default(Vector3);
            }

            state.Movement.LocalPosition = cachedTransform.localPosition;
            state.Movement.Position = cachedTransform.position;
            state.Movement.CharacterVelocity = characterController.velocity;
            state.Movement.IsGrounded = isGrounded;
        }

        private void Walk(bool isLeft, bool isRight, bool isBackward, bool isForward, float yaw)
        {
            if (state.IsImmobilized)
            {
                state.Movement.Velocity = new Vector3(0.0f, state.Movement.Velocity.y, 0.0f);

                return;
            }

            var input = default(Vector3);
            var roll = 0.0f;

            if (isLeft ^ isRight)
            {
                input.x = isLeft ? -1 : 1;
                roll = (isLeft) ? 1.5f : -1.5f;
            }

            if (isBackward ^ isForward)
            {
                input.z = isBackward ? -1 : 1;
            }

            var aimModifier = state.ActiveGadget.IsAiming ? 0.5f : 1.0f;

            playerBounceTransform.localRotation = Quaternion.Lerp(playerBounceTransform.localRotation, Quaternion.Euler(0.0f, 0.0f, roll), 5.0f * BoltNetwork.FrameDeltaTime);

            input = Vector3.Normalize(Quaternion.Euler(0.0f, yaw, 0.0f) * input);
            state.Movement.Velocity = new Vector3(input.x * (activePose.MoveSpeed * state.MainMoveSpeed* state.AltMoveSpeed * aimModifier), state.Movement.Velocity.y, input.z * (activePose.MoveSpeed * state.MainMoveSpeed * state.AltMoveSpeed * aimModifier));
        }

        private void Jump(bool willJump, float time)
        {
            if (state.IsSpace)
            {
                jumpSpeed = willJump ? activePose.MoveSpeed : 0;
                state.Movement.Velocity = new Vector3(state.Movement.Velocity.x, jumpSpeed + crouchSpeed, state.Movement.Velocity.z);

                return;
            }

            if (state.Movement.IsGrounded)
            {
                state.Movement.StepOffset = stepOffset;

                if (willJump && state.Movement.JumpFrames <= 0 && !state.IsImmobilized)
                {
                    state.Movement.JumpFrames = 30;
                }
            }
            else
            {
                state.Movement.StepOffset = 0.0001f;
                state.Movement.Velocity = new Vector3(state.Movement.Velocity.x, state.Movement.Velocity.y + (gravity * state.playerGravity * time), state.Movement.Velocity.z);
            }

            characterController.stepOffset = state.Movement.StepOffset;

            if (state.Movement.JumpFrames > 0)
            {
                float force = (float)state.Movement.JumpFrames / 30 * jumpForce;

                Move(new Vector3(0.0f, force, 0.0f), time);
            }

            state.Movement.JumpFrames = Mathf.Max(0, state.Movement.JumpFrames - 1);
            state.Movement.Velocity = new Vector3(state.Movement.Velocity.x, Mathf.Min(state.Movement.Velocity.y, gravity * state.playerGravity), state.Movement.Velocity.z);
        }

        public void KnockBack(bool willKnockBack, float time)
        {
            if (!entity.IsAttached || state.IsImmobilized)
            {
                return;
            }

            if (willKnockBack)
            {
                if (frigidbody.Force.magnitude >= IgnoreFallDamageForce)
                {
                    altitude = cachedTransform.position.y;
                }

                state.Movement.ForceFrames = 30;
            }

            if (state.Movement.ForceFrames > 0)
            {
                Vector3 finalForce = (state.Movement.IsCrouching) ? new Vector3(frigidbody.Force.x / 3.0f, frigidbody.Force.y / 3.0f, frigidbody.Force.z / 3.0f) : frigidbody.Force;
                float forceMultiplier = (float)state.Movement.ForceFrames / 30;
                var force = new Vector3(finalForce.x * forceMultiplier, finalForce.y * forceMultiplier * 2.0f, finalForce.z * forceMultiplier);

                Move(force, time);
            }

            state.Movement.ForceFrames = Mathf.Max(0, state.Movement.ForceFrames - 1);
        }

        private void Crouch(bool willCrouch, float time)
        {
            if (state.IsSpace)
            {
                activePose = stand;
                crouchSpeed = willCrouch ? -activePose.MoveSpeed : 0;

                return;
            }

            if (willCrouch && !state.Movement.IsCrouching)
            {
                state.Movement.IsCrouching = true;
            }
            else if (!willCrouch && state.Movement.IsCrouching && CanUncrouch())
            {
                state.Movement.IsCrouching = false;
            }

            activePose = (state.Movement.IsCrouching) ? crouch : stand;
            vision.localPosition = Vector3.Lerp(vision.localPosition, activePose.VisionCenter, poseSpeed * time);
            state.Movement.ColliderCenter = Vector3.Lerp(characterController.center, activePose.ColliderCenter, poseSpeed * time);
            state.Movement.ColliderHeight = Mathf.Lerp(characterController.height, activePose.ColliderHeight, poseSpeed * time);
            characterController.center = state.Movement.ColliderCenter;
            characterController.height = state.Movement.ColliderHeight;
        }

        private bool checkedFall;

        public void CheckFall()
        {
            if (state.Movement.IsGrounded && !checkedFall)
            {
                checkedFall = true;

                playerBounce.Bounce((altitude - cachedTransform.position.y) / 10.0f);

                if (cachedTransform.position.y < altitude - hardLandThreshold)
                {
                    state.Effect.LandHard();
                }
                else if (cachedTransform.position.y < altitude - softLandThreshold)
                {
                    state.Effect.LandSoft();
                }

                if (cachedTransform.position.y < altitude - fallDamageThreshold)
                {
                    var damage = fallDamage * Mathf.Abs(Mathf.CeilToInt(altitude - fallDamageThreshold));

                    damage = Mathf.Clamp(damage, 1, 50);

                    state.Effect.TakeFallDamage();
                    playerRebound.KickRebound(damage / 3.0f, damage / 3.0f, damage / 3.0f, 0.0f);

                    if (entity.HasControl)
                    {
                        PlayerHud.Instance.DisplayDamage();
                    }

                    if (entity.IsOwner)
                    {
                        damageable.TakeDamage(damage, Vector3.up, "fall damage", null, true);
                    }
                }

                altitude = cachedTransform.position.y;
            }
            else
            {
                checkedFall = false;

                if (altitude < cachedTransform.position.y)
                {
                    altitude = cachedTransform.position.y;
                }
            }
        }

        private void CheckTunneling()
        {
            var hit = default(RaycastHit);

            if (Physics.Raycast(GetCenterPosition(), Vector3.down, out hit, characterController.height / 2.0f, environmentLayerMask, QueryTriggerInteraction.Ignore))
            {
                cachedTransform.position = hit.point;
            }
        }

        private bool CanUncrouch()
        {
            var spherePosition = cachedTransform.position;
            spherePosition.y += stand.ColliderHeight - characterController.radius + 0.01f;

            if (Physics.CheckSphere(spherePosition, characterController.radius, environmentLayerMask, QueryTriggerInteraction.Ignore))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private Vector3 GetBottomPosition()
        {
            Vector3 position = cachedTransform.position;
            position.y += characterController.radius - (characterController.skinWidth * 2.0f);

            return position;
        }

        private Vector3 GetCenterPosition()
        {
            Vector3 position = cachedTransform.position;
            position.y += characterController.height / 2.0f;

            return position;
        }

        [Serializable]
        private struct Pose
        {
            [SerializeField] private Vector3 visionCenter;
            public Vector3 VisionCenter { get { return visionCenter; } }

            [SerializeField] private Vector3 colliderCenter;
            public Vector3 ColliderCenter { get { return colliderCenter; } }

            [SerializeField] private float colliderHeight;
            public float ColliderHeight { get { return colliderHeight; } }

            public float MoveSpeed { get; set; }
            public float FootstepRate { get; set; }
        }
    }
}