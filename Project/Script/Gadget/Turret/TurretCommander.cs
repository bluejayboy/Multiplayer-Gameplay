using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class TurretCommander : EntityBehaviour<ITurretState>
    {
        [SerializeField] private float minimumPitch = -90.0f;
        [SerializeField] private float maximumPitch = 90.0f;
        [SerializeField] private float minimumYaw = -90.0f;
        [SerializeField] private float maximumYaw = 90.0f;

        private Turret turret = null;
        private TurretHitscan hitscan = null;
        private Ability ability = null;

        private float pitch = 0.0f;
        private float yaw = 0.0f;
        private bool willFire = false;
        private bool willAbandon = false;

        private void Awake()
        {
            turret = GetComponent<Turret>();
            hitscan = GetComponent<TurretHitscan>();
        }

        private void Update()
        {
            ApplyInput();
        }

        public override void SimulateController()
        {
            ApplyCommand();
            ResetInput();
        }

        public override void ExecuteCommand(Command command, bool resetState)
        {
            var turretCommand = command as TurretCommand;
            var input = turretCommand.Input;

            if (resetState)
            {
                return;
            }

            if (!turretCommand.IsFirstExecution)
            {
                return;
            }

            turret.Rotate(input.Pitch, input.Yaw);

            if (input.Fire)
            {
                hitscan.Fire(turretCommand.ServerFrame);
            }

            if (input.Abandon)
            {
                turret.Abandon();
            }
        }

        private void SetRotation(float pitch, float yaw)
        {
            this.pitch = Mathf.Clamp(this.pitch - pitch, minimumPitch, maximumPitch);
            this.yaw = Mathf.Clamp(this.yaw + yaw, minimumYaw, maximumYaw);
        }

        public void LerpRotation(float pitch, float yaw, float speed)
        {
            var time = BoltNetwork.FrameDeltaTime;

            this.pitch = Mathf.Clamp(Mathf.Lerp(this.pitch, this.pitch - pitch, speed * time), minimumPitch, maximumPitch);
            this.yaw = Mathf.Clamp(Mathf.Lerp(this.yaw, this.yaw + yaw, speed * time), minimumYaw, maximumYaw);
        }

        private void ApplyCommand()
        {
            ITurretCommandInput input = TurretCommand.Create();

            input.Pitch = pitch;
            input.Yaw = yaw;
            input.Fire = willFire;
            input.Abandon = willAbandon;

            entity.QueueInput(input);
        }

        private void ApplyInput()
        {
            if (!entity.IsAttached || !entity.HasControl)
            {
                return;
            }

            if (ScramInput.IsPausedOrChatting || !ScramInput.CursorIsLocked)
            {
                ResetInput();

                return;
            }

            SetRotation(InputCode.MouseY * InputCode.MouseSensitivity, InputCode.MouseX * InputCode.MouseSensitivity);

            willFire |= Input.GetKey(InputCode.PrimaryFire);
            willAbandon |= Input.GetKeyDown(InputCode.Interact);
        }

        private void ResetInput()
        {
            willFire = false;
            willAbandon = false;
        }
    }
}