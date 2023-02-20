using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class VehicleDriverSeat : EntityBehaviour<IVehicleState>, IInteractable
    {
        [SerializeField] private Transform seat;
        [SerializeField] private GameObject cam;

        [SerializeField] private string display = string.Empty;
        public string Display { get { return (entity.IsAttached && !state.IsOccupied) ? display : string.Empty; } }

        public Player Driver { get; private set; }

        private void Start()
        {
            if (BoltNetwork.IsServer)
            {
                HostPlayerAction.Instance.TurretDeath += CheckShooter;
            }
        }

        public override void ControlGained()
        {
            //cam.SetActive(true);

            var token = this.entity.ControlGainedToken as TurretToken;
            var entity = BoltNetwork.FindEntity(token.NetworkID);

            if (entity == null || !entity.IsAttached)
            {
                return;
            }

            Driver = entity.GetComponent<Player>();
            Driver.transform.SetPositionAndRotation(seat.transform.position, seat.transform.rotation);
            //Driver.ToggleTurretView(false);
        }

        public override void ControlLost()
        {
            //cam.SetActive(false);

            var token = this.entity.ControlLostToken as TurretToken;
            var entity = BoltNetwork.FindEntity(token.NetworkID);

            if (entity == null || !entity.IsAttached)
            {
                return;
            }

            //Driver.GetComponent<Player>().ToggleTurretView(true);
        }

        public void Interact(Player player)
        {
            if (!entity.IsAttached || !player.entity.IsAttached || Driver != null || state.IsOccupied)
            {
                return;
            }

            state.IsOccupied = true;
            Driver = player;
            Driver.state.IsImmobilized = true;
            Driver.transform.SetPositionAndRotation(seat.transform.position, seat.transform.rotation);

            var token = new TurretToken
            {
                NetworkID = Driver.entity.NetworkId
            };

            if (Driver.entity.HasControl)
            {
                entity.TakeControl(token);
            }
            else
            {
                entity.AssignControl(Driver.entity.Controller, token);
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
                NetworkID = Driver.entity.NetworkId
            };

            if (Driver.entity.HasControl)
            {
                entity.ReleaseControl(token);
            }
            else
            {
                entity.RevokeControl(token);
            }

            Driver.state.IsImmobilized = false;
            Driver = null;
            state.IsOccupied = false;
        }

        public void CheckShooter(BoltEntity entity)
        {
            if (Driver != null && entity == Driver.entity)
            {
                Abandon();
            }
        }
    }
}