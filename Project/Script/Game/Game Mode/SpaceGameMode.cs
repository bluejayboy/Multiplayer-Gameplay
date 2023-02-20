using UnityEngine;
using MEC;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class SpaceGameMode : GameMode
    {
        [SerializeField] private Team defaultTeam = null;

        protected override void Start()
        {
            base.Start();

            if (BoltNetwork.IsServer)
            {
                HostPlayerAction.Instance.OnDeath = OnPlayerDeath;
                Game.Instance.MatchStatus = Game.MatchState.MidRound;
            }
        }

        public override void ConnectedDuringPlay(PlayerConnection connection)
        {
          var player =  defaultTeam.PlayerSpawner.Spawn(connection);

            player.state.IsSpace = true;
        }

        protected override void OnPlayerDeath(PlayerConnection connection, Vector3 position, float yaw)
        {
            if (BoltNetwork.IsClient)
            {
                return;
            }

            Timing.RunCoroutine(defaultTeam.PlayerSpawner.SpawnWithDelay(connection), "Respawn");
        }
    }
}