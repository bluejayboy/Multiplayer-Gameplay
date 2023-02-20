using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class VehicleCommander : EntityBehaviour<IVehicleState>
    {
        private Rigidbody myRigidbody;

        public override void Attached()
        {
            myRigidbody = GetComponent<Rigidbody>();
            state.SetTransforms(state.Transform, transform);
        }

        public override void SimulateController()
        {
            IVehicleCommandInput input = VehicleCommand.Create();
            input.Right = Input.GetKey(KeyCode.D);
            input.Left = Input.GetKey(KeyCode.A);
            input.Up = Input.GetKey(KeyCode.W);
            input.Down = Input.GetKey(KeyCode.S);

            entity.QueueInput(input);

        }

        public override void ExecuteCommand(Command command, bool resetState)
        {
            var cmd = (VehicleCommand)command;

            if (BoltNetwork.IsServer)
            {
                if (cmd.Input.Right)
                {
                    transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y + 1, transform.localRotation.eulerAngles.z);
                }
                else if (cmd.Input.Left)
                {
                    transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y - 1, transform.localRotation.eulerAngles.z);
                }

                if (cmd.Input.Up)
                {
                    transform.localPosition += transform.forward * 0.5f;
                }
                else if (cmd.Input.Down)
                {
                    transform.localPosition -= transform.forward * 0.5f;
                }
            }
        }
    }
}