using Photon.Bolt;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class RescueVehicle : EntityBehaviour<IRescueVehicleState>
    {
        [SerializeField] private EvacuationGameMode evac = null;
        [SerializeField] private RescueData rescueData = null;
        [SerializeField] private TeamData teamData = null;

        [SerializeField] private Transform rotor = null;
        [SerializeField] private float rotorSpeed = -360.0f;

        public Transform start = null;
        public Transform middle = null;
        public Transform end = null;
        [SerializeField] private BoxCollider[] boundaries = null;
        [SerializeField] private Transform render = null;

        private Transform cachedTransform = null;
        private float destinationTimer;

        private void Awake()
        {
            cachedTransform = transform;
        }

        private void IgnoreBoundary(Player player, bool isTrue)
        {
            for (int i = 0; i < boundaries.Length; i++)
            {
                player.IgnoreCollider(boundaries[i], isTrue);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<Player>();

            if (player == null || !player.entity.IsAttached || !player.entity.IsControllerOrOwner)
            {
                return;
            }

            other.transform.SetParent(render, true);
            IgnoreBoundary(player, true);

            if (BoltNetwork.IsClient)
            {
                return;
            }

            if (player.entity.IsAttached && player.state.Team == teamData.ID && !evac.EscapedHumans.Contains(player.entity.Controller.GetPlayerConnection()))
            {
                evac.EscapedHumans.Add(player.entity.Controller.GetPlayerConnection());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var player = other.GetComponent<Player>();

            if (player == null || !player.entity.IsAttached || !player.entity.IsControllerOrOwner)
            {
                return;
            }

            other.transform.SetParent(null, true);
            IgnoreBoundary(player, false);

            if (BoltNetwork.IsClient)
            {
                return;
            }

            if (evac.EscapedHumans.Contains(player.entity.Controller.GetPlayerConnection()))
            {
                evac.EscapedHumans.Remove(player.entity.Controller.GetPlayerConnection());
            }
        }

        private void Start()
        {
            if (HostGameAction.Instance != null)
            {
                HostGameAction.Instance.OnRoundStart += ResetAll;
            }
        }

        private void FixedUpdate()
        {
            RotateRotor(Time.fixedDeltaTime);
        }

        private void Update()
        {
            if (!entity.IsAttached || middle == null)
            {
                return;
            }

            if (state.Position.y <= middle.localPosition.y + 0.1f && state.Position.y >= middle.localPosition.y - 0.1f)
            {
                state.SetTransforms(state.Transform, null, null);
                cachedTransform.localPosition = state.Position;
            }
            else
            {
                state.SetTransforms(state.Transform, cachedTransform, render);
            }
        }

        public override void Attached()
        {
            state.SetTransforms(state.Transform, cachedTransform, render);

            if (entity.IsOwner)
            {
                state.AddCallback("Destination", ApplyTimer);
            }
        }

        public override void SimulateOwner()
        {
            if (start == null || middle == null || end == null)
            {
                return;
            }

            var time = BoltNetwork.FrameDeltaTime;

            if (state.Destination == 1 && destinationTimer > 0.0f)
            {
                destinationTimer -= time;

                Vector3 distance = middle.localPosition - start.localPosition;

                cachedTransform.localPosition = start.localPosition + (distance / evac.ArrivalTime * (evac.ArrivalTime - destinationTimer));
            }
            else if (state.Destination == 2)
            {
                cachedTransform.localPosition = Vector3.Lerp(cachedTransform.localPosition, end.localPosition, 0.1f * time);
            }

            state.Position = cachedTransform.localPosition;
        }

        private void RotateRotor(float time)
        {
            var speed = rotorSpeed * time;

            rotor.Rotate(new Vector3(speed * Vector3.up.x, speed * Vector3.up.y, speed * Vector3.up.z));
        }

        private void ApplyTimer()
        {
            if (state.Destination == 1)
            {
                destinationTimer = evac.ArrivalTime;
            }
            else
            {
                destinationTimer = 0.0f;

                if (state.Destination == 0)
                {
                    cachedTransform.localPosition = start.localPosition;
                    state.Position = cachedTransform.localPosition;
                }
            }
        }

        private void ResetAll()
        {
            if (!entity.IsAttached)
            {
                return;
            }

            state.Destination = -1;
            state.Destination = 0;
        }
    }
}