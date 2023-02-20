using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Turret : EntityBehaviour<ITurretState>, IInteractable
    {
        [SerializeField] private Transform gun;
        [SerializeField] private Transform render;
        [SerializeField] private Transform seat;
        [SerializeField] private GameObject cam;

        [SerializeField] private string display = string.Empty;
        public string Display { get { return (entity.IsAttached && !state.IsOccupied) ? display : string.Empty; } }

        public Player Shooter { get; private set; }

        private void Start()
        {
            if (BoltNetwork.IsServer)
            {
                HostPlayerAction.Instance.TurretDeath += CheckShooter;
            }
        }

        public override void Attached()
        {
            state.SetTransforms(state.Transform, gun, render);
        }

        public override void ControlGained()
        {
            cam.SetActive(true);

            var token = this.entity.ControlGainedToken as TurretToken;
            var entity = BoltNetwork.FindEntity(token.NetworkID);

            if (entity == null || !entity.IsAttached)
            {
                return;
            }

            Shooter = entity.GetComponent<Player>();

            Shooter.transform.SetPositionAndRotation(seat.transform.position, seat.transform.rotation);
            Shooter.ToggleTurretView(false);
        }

        public override void ControlLost()
        {
            cam.SetActive(false);

            var token = this.entity.ControlLostToken as TurretToken;
            var entity = BoltNetwork.FindEntity(token.NetworkID);

            if (entity == null || !entity.IsAttached)
            {
                return;
            }

            Shooter.GetComponent<Player>().ToggleTurretView(true);
        }

        public void Interact(Player player)
        {
            if (!entity.IsAttached || !player.entity.IsAttached || Shooter != null || state.IsOccupied)
            {
                return;
            }

            state.IsOccupied = true;
            Shooter = player;
            Shooter.state.isFrozen = true;
            Shooter.transform.SetPositionAndRotation(seat.transform.position, seat.transform.rotation);

            var token = new TurretToken
            {
                NetworkID = Shooter.entity.NetworkId
            };

            if (Shooter.entity.HasControl)
            {
                entity.TakeControl(token);
            }
            else
            {
                entity.AssignControl(Shooter.entity.Controller, token);
            }
        }

        public void Abandon()
        {
            if (!entity.IsOwner)
            {
                return;
            }

            var token = new TurretToken
            {
                NetworkID = Shooter.entity.NetworkId
            };

            if (Shooter.entity.HasControl)
            {
                entity.ReleaseControl(token);
            }
            else
            {
                entity.RevokeControl(token);
            }

            Shooter.state.isFrozen = false;
            Shooter = null;
            state.IsOccupied = false;
        }

        public void Rotate(float pitch, float yaw)
        {
            gun.localRotation = Quaternion.Euler(pitch, yaw, 0.0f);
        }

        public void CheckShooter(BoltEntity entity)
        {
            if (Shooter != null && entity == Shooter.entity)
            {
                Abandon();
            }
        }
    }
}