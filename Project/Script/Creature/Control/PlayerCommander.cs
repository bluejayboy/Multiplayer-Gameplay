using Photon.Bolt;
using System.Collections.Generic;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class PlayerCommander : EntityBehaviour<IPlayerState>
    {
        [SerializeField] private float minimumPitch = -90.0f;
        [SerializeField] private float maximumPitch = 90.0f;
        [SerializeField] private KeyCode[] slots = null;

        private PlayerController playerController = null;
        private PlayerAnimator playerAnimator = null;
        private PlayerLoadout playerLoadout = null;
        private PlayerInteract playerInteract = null;

        private bool isLeft = false;
        private bool isRight = false;
        private bool isBackward = false;
        private bool isForward = false;

        private float pitch = 0.0f;
        private float yaw = 0.0f;

        private bool willJump = false;
        private bool willCrouch = false;

        private bool willPrimaryDown = false;
        private bool willPrimaryHold = false;
        private bool willPrimaryUp = false;
        private bool willSecondaryDown = false;

        private bool willAbilityFire = false;
        private bool willAim = false;
        private bool willHoldInteract = false;

        private bool willSelect = false;
        private bool quickSwitch = false;
        private int activeSlot = 0;
        private int activeIndex = 0;
        private int prevSlot = 0;

        public bool WillKnockBack { get; set; }

        public IPrimaryDown PrimaryDown { get; set; }
        public IPrimaryFireHold PrimaryHold { get; set; }
        public IPrimaryFireUp PrimaryUp { get; set; }
        public ISecondaryFire SecondaryDown { get; set; }
        public Ability Ability { get; set; }

        private List<int> ownedGadgets = new List<int>(10);

        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            playerAnimator = GetComponent<PlayerAnimator>();
            playerLoadout = GetComponent<PlayerLoadout>();
            playerInteract = GetComponentInChildren<PlayerInteract>(true);
        }

        private void Update()
        {
            ApplyInput();
        }

        public override void Attached()
        {
            state.AddCallback("OwnedGadgets[].ID", EditOwnedGadget);
        }

        private void EditOwnedGadget(IState state, string path, ArrayIndices indices)
        {
            var playerState = state as IPlayerState;
            int index = indices[0];

            if (!string.IsNullOrEmpty(playerState.OwnedGadgets[index].ID))
            {
                if (!ownedGadgets.Contains(index))
                {
                    ownedGadgets.Add(index);
                }
            }
            else
            {
                if (ownedGadgets.Contains(index))
                {
                    ownedGadgets.Remove(index);
                }
            }

            ownedGadgets.Sort();

            if (playerState.ActiveGadget.Slot < 0 && ownedGadgets.Count > 0)
            {
                playerState.ActiveGadget.Slot = ownedGadgets[0];
                activeIndex = 0;
            }
        }

        public override void Detached()
        {
            pitch = 0;
            yaw = 0;
            activeSlot = 0;
            prevSlot = 0;

            ResetInput();

            PrimaryDown = null;
            PrimaryHold = null;
            PrimaryUp = null;
            SecondaryDown = null;
        }

        public override void ControlGained()
        {
            var token = entity.ControlGainedToken as PlayerToken;

            yaw = token.Yaw;
        }

        public override void SimulateController()
        {
            ApplyFixedInput();
            ApplyCommand();
            ResetInput();
        }

        public override void ExecuteCommand(Command command, bool resetState)
        {
            var playerCommand = command as PlayerCommand;
            var input = playerCommand.Input;
            var result = playerCommand.Result;

            state.ServerFrame = playerCommand.ServerFrame;

            if (resetState)
            {
                playerController.SetMovement(result);

                return;
            }

            MovementObject movement = playerController.GetMovement(input);

            result.LocalPosition = movement.LocalPosition;
            result.Position = movement.Position;
            result.Velocity = movement.Velocity;
            result.JumpFrames = movement.JumpFrames;
            result.ForceFrames = movement.ForceFrames;
            result.IsGrounded = movement.IsGrounded;
            result.IsCrouching = movement.IsCrouching;
            result.StepOffset = movement.StepOffset;
            result.ColliderCenter = movement.ColliderCenter;
            result.ColliderHeight = movement.ColliderHeight;

            if (!playerCommand.IsFirstExecution)
            {
                return;
            }

            playerController.CheckFall();

            if (!entity.IsAttached)
            {
                return;
            }

            playerAnimator.Animate(input);

            if (state.isFrozen)
            {
                state.Pitch = 0;

                return;
            }

            state.Pitch = input.Pitch;
            state.ActiveGadget.IsAiming = input.Aim;

            if (input.Select && !string.IsNullOrEmpty(state.OwnedGadgets[input.ActiveSlot].ID))
            {
                prevSlot = state.ActiveGadget.Slot;
                state.ActiveGadget.Slot = input.ActiveSlot;
            }

            if (input.QuickSwitch && !string.IsNullOrEmpty(state.OwnedGadgets[prevSlot].ID))
            {
                var tempSlot = state.ActiveGadget.Slot;
                state.ActiveGadget.Slot = prevSlot;

                if (tempSlot > -1)
                {
                    prevSlot = tempSlot;
                }
            }

            if (PrimaryDown != null && input.PrimaryDown)
            {
                PrimaryDown.PrimaryDown();
            }

            if (PrimaryHold != null && input.PrimaryHold)
            {
                PrimaryHold.PrimaryHold();
            }

            if (PrimaryUp != null && input.PrimaryUp)
            {
                PrimaryUp.PrimaryUp();
            }

            if (SecondaryDown != null && input.SecondaryDown)
            {
                SecondaryDown.SecondaryDown();
            }

            if (Ability != null && entity.IsOwner && input.AbilityFire)
            {
                Ability.ApplyAbility();
            }

            if (input.HoldInteract)
            {
                playerInteract.HoldInteract();
            }
            else
            {
                playerInteract.ReleaseInteract();
            }
        }

        private void SetRotation(float pitch, float yaw)
        {
            this.pitch = state.isFrozen ? 0 : Mathf.Clamp(this.pitch - pitch, minimumPitch, maximumPitch);
            this.yaw = state.isFrozen ? 0 : ScramMath.Mod(this.yaw + yaw);
        }

        public void LerpRotation(float pitch, float yaw, float speed)
        {
            var time = BoltNetwork.FrameDeltaTime;

            this.pitch = state.isFrozen ? 0 : Mathf.Clamp(Mathf.Lerp(this.pitch, this.pitch - pitch, speed * time), minimumPitch, maximumPitch);
            this.yaw = state.isFrozen ? 0 : ScramMath.Mod(Mathf.Lerp(this.yaw, this.yaw + yaw, speed * time));
        }

        private void ApplyCommand()
        {
            IPlayerCommandInput input = PlayerCommand.Create();

            input.Left = isLeft;
            input.Right = isRight;
            input.Forward = isForward;
            input.Backward = isBackward;

            input.Pitch = pitch;
            input.Yaw = yaw;

            input.Jump = willJump;
            input.KnockBack = WillKnockBack;
            input.Crouch = willCrouch;
            input.PrimaryDown = willPrimaryDown;
            input.PrimaryHold = willPrimaryHold;
            input.PrimaryUp = willPrimaryUp;
            input.SecondaryDown = willSecondaryDown;
            input.AbilityFire = willAbilityFire;
            input.Aim = willAim;
            input.HoldInteract = willHoldInteract;

            input.Select = willSelect;
            input.ActiveSlot = activeSlot;
            input.QuickSwitch = quickSwitch;

            entity.QueueInput(input);
        }

        private void ApplyInput()
        {
            if (!entity.IsAttached || !entity.HasControl)
            {
                return;
            }

            if (ScramInput.IsPausedOrChatting)
            {
                ResetInput();

                return;
            }

            willJump |= (state.IsSpace) ? Input.GetKey(InputCode.Jump) : Input.GetKeyDown(InputCode.Jump);

            for (var i = 0; i < slots.Length; i++)
            {
                if (Input.GetKeyDown(slots[i]))
                {
                    willSelect = true;
                    activeSlot = i;
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                quickSwitch = true;
            }

            if (!ScramInput.CursorIsLocked)
            {
                return;
            }

            if (Input.GetAxisRaw("Mouse Wheel") < 0)
            {
                if (activeIndex < ownedGadgets.Count - 1)
                {
                    willSelect = true;
                    activeIndex++;
                }
                else
                {
                    willSelect = true;
                    activeIndex = 0;
                }

                if (activeIndex >= 0 && activeIndex < ownedGadgets.Count)
                {
                    activeSlot = ownedGadgets[activeIndex];
                }
            }
            else if (Input.GetAxisRaw("Mouse Wheel") > 0)
            {
                if (activeIndex > 0)
                {
                    willSelect = true;
                    activeIndex--;
                }
                else
                {
                    willSelect = true;
                    activeIndex = ownedGadgets.Count - 1;
                }

                if (activeIndex >= 0 && activeIndex < ownedGadgets.Count)
                {
                    activeSlot = ownedGadgets[activeIndex];
                }
            }

            float sensitivity = InputCode.MouseSensitivity;

            if (PrimaryDown != null && PrimaryDown.Data is HitscanGadgetData && state.ActiveGadget.IsAiming)
            {
                var data = PrimaryDown.Data as HitscanGadgetData;

                sensitivity /= data.AimSensitivityDivisor;
            }

            SetRotation(InputCode.MouseY * sensitivity, InputCode.MouseX * sensitivity);

            if (state.ActiveGadget.Slot > -1)
            {
                var ammo = entity.IsOwner ? state.OwnedGadgets[state.ActiveGadget.Slot].ActiveAmmo : state.OwnedGadgets[state.ActiveGadget.Slot].PredictedAmmo;

                willPrimaryDown |= ((PrimaryDown != null && !PrimaryDown.Data.IsFullAuto) || ammo <= 0) ? Input.GetKeyDown(InputCode.PrimaryFire) : false;
            }

            willPrimaryUp |= Input.GetKeyUp(InputCode.PrimaryFire);
            willSecondaryDown |= (PrimaryDown != null && PrimaryDown.Data.SecondaryFire != GadgetData.SecondaryFireState.Aim) ? Input.GetKeyDown(InputCode.SecondaryFire) : false;
            willAbilityFire |= Input.GetKeyDown(KeyCode.F);
        }

        private void ApplyFixedInput()
        {
            if (ScramInput.IsPausedOrChatting)
            {
                ResetInput();

                return;
            }

            isLeft = Input.GetKey(InputCode.Left);
            isRight = Input.GetKey(InputCode.Right);
            isBackward = Input.GetKey(InputCode.Backward);
            isForward = Input.GetKey(InputCode.Forward);

            willCrouch |= Input.GetKey(InputCode.Crouch);
            willAim |= (PrimaryDown != null && PrimaryDown.Data.SecondaryFire == GadgetData.SecondaryFireState.Aim) ? Input.GetKey(InputCode.SecondaryFire) : false;
            willHoldInteract |= Input.GetKey(InputCode.Interact);

            if (!ScramInput.CursorIsLocked)
            {
                return;
            }

            if (state.ActiveGadget.Slot > -1)
            {
                var ammo = entity.IsOwner ? state.OwnedGadgets[state.ActiveGadget.Slot].ActiveAmmo : state.OwnedGadgets[state.ActiveGadget.Slot].PredictedAmmo;

                willPrimaryDown |= (PrimaryDown != null && PrimaryDown.Data.IsFullAuto && ammo > 0) ? Input.GetKey(InputCode.PrimaryFire) : false;
            }

            willPrimaryHold |= Input.GetKey(InputCode.PrimaryFire);
        }

        private void ResetInput()
        {
            isLeft = false;
            isRight = false;
            isBackward = false;
            isForward = false;
            willJump = false;
            WillKnockBack = false;
            willCrouch = false;
            willPrimaryDown = false;
            willPrimaryHold = false;
            willPrimaryUp = false;
            willSecondaryDown = false;
            willAbilityFire = false;
            willAim = false;
            willHoldInteract = false;
            willSelect = false;
            quickSwitch = false;
            activeSlot = 0;
        }
    }
}