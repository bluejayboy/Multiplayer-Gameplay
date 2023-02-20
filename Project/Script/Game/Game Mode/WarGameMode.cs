using MEC;
using UnityEngine;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class WarGameMode : GameMode
    {
        [SerializeField] private Team naziTeam = null;
        [SerializeField] private Team communistTeam = null;

        [SerializeField] private ControlPoint[] controlPoints;

        protected override void Start()
        {
            base.Start();

            //controlPoints[0].Activate(true);

            if (BoltNetwork.IsClient)
            {
                return;
            }

            HostPlayerAction.Instance.OnDeath = OnPlayerDeath;
            Game.Instance.MatchStatus = Game.MatchState.MidRound;
        }

        public override void OnDisconnect(PlayerConnection connection)
        {
            if (BoltNetwork.IsClient)
            {
                return;
            }

            if (naziTeam.Players.Contains(connection))
            {
                naziTeam.Players.Remove(connection);
            }
            else if (communistTeam.Players.Contains(connection))
            {
                communistTeam.Players.Remove(connection);
            }
        }

        public override void ConnectedDuringPlay(PlayerConnection connection)
        {
            if (BoltNetwork.IsClient)
            {
                return;
            }

            if (naziTeam.Players.Count <= communistTeam.Players.Count)
            {
                naziTeam.Players.Add(connection);
                naziTeam.PlayerSpawner.Spawn(connection);

                connection.PlayerInfo.state.Color = naziTeam.Data.Color;
            }
            else
            {
                communistTeam.Players.Add(connection);
                communistTeam.PlayerSpawner.Spawn(connection);

                connection.PlayerInfo.state.Color = communistTeam.Data.Color;
            }
        }

        protected override void OnPlayerDeath(PlayerConnection connection, Vector3 position, float yaw)
        {
            if (BoltNetwork.IsClient)
            {
                return;
            }

            if (naziTeam.Players.Contains(connection) && naziTeam.Players.Count > communistTeam.Players.Count + 1)
            {
                naziTeam.Players.Remove(connection);
                communistTeam.Players.Add(connection);
                connection.PlayerInfo.state.Color = communistTeam.Data.Color;
            }
            else if (communistTeam.Players.Contains(connection) && communistTeam.Players.Count > naziTeam.Players.Count + 1)
            {
                communistTeam.Players.Remove(connection);
                naziTeam.Players.Add(connection);
                connection.PlayerInfo.state.Color = naziTeam.Data.Color;
            }

            if (naziTeam.Players.Contains(connection))
            {
                Timing.RunCoroutine(naziTeam.PlayerSpawner.SpawnWithDelay(connection), "Respawn");
            }
            else if (communistTeam.Players.Contains(connection))
            {
                Timing.RunCoroutine(communistTeam.PlayerSpawner.SpawnWithDelay(connection), "Respawn");
            }
        }

        public void TakeControlPoint(int index)
        {
            controlPoints[index].Activate(false);

            var nextPoint = ++index;

            if (nextPoint < controlPoints.Length)
            {
                controlPoints[nextPoint].Activate(true);
            }
            else
            {
                Debug.LogError("Game finished.");
            }
        }
    }
}