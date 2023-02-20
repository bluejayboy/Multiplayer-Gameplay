using UnityEngine;
using MEC;
using Photon.Bolt;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class EliminationGameMode : GameMode
    {
        [SerializeField] private Team defaultTeam = null;

        public bool spawnWithItem = false;
        public string gadget1;
        public string gadget2;

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
            if (spawnWithItem)
            {
                connection.PlayerInfo.state.Gadget1 = gadget1;
                connection.PlayerInfo.state.Gadget2 = gadget2;
            }

            defaultTeam.PlayerSpawner.Spawn(connection);
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