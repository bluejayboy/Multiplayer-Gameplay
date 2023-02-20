using Photon.Bolt;
using MEC;
using UnityEngine;

namespace Scram
{
    [DisallowMultipleComponent]
    public sealed class Lounge : GlobalEventListener
    {
        public static Lounge Instance { get; private set; }

        [SerializeField] private GameModeData gameModeData = null;
        [SerializeField] private PlayerSpawner loungePlayerSpawner = null;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public override void Disconnected(BoltConnection connection)
        {
            if (BoltNetwork.IsServer && Game.Instance.MatchStatus == Game.MatchState.Intermission && HostPlayerRegistry.Instance.PlayerConnections.Count < gameModeData.MinimumPlayerCount)
            {
                WaitForPlayers();
            }
        }

        public void ConnectedDuringWaitingForPlayers()
        {
            if (HostPlayerRegistry.Instance.PlayerConnections.Count >= gameModeData.MinimumPlayerCount)
            {
                StartIntermission();
            }
            else
            {
                WaitForPlayers();
            }
        }

        public void ConnectedDuringIntermission(PlayerConnection connection)
        {
            loungePlayerSpawner.Spawn(connection);
            GameMode.Instance.SetNewPlayerTeam(connection);
        }

        private void WaitForPlayers()
        {
            HostGameAction.Instance.OnTimerEnd = null;
            HostPlayerAction.Instance.OnDeath = OnPlayerDeath;

            Game.Instance.MatchStatus = Game.MatchState.WaitingForPlayers;

            GameHud.Instance.state.Timer = -1;
            GameHud.Instance.state.Status = "Waiting for Players";

            ScramGame.ResetAllPlayerTeams();

            HostPlayerRegistry.Instance.DestroyAllPlayers();
            SpawnAllPlayers();

            BoltGlobalEvent.SendCue(string.Empty, Color.white);
        }

        public void StartIntermission()
        {
            if (HostPlayerRegistry.Instance.PlayerConnections.Count < gameModeData.MinimumPlayerCount)
            {
                WaitForPlayers();

                return;
            }

            if (Game.Instance.MatchStatus == Game.MatchState.Intermission)
            {
                return;
            }

            HostGameAction.Instance.OnTimerEnd = EndIntermission;
            HostPlayerAction.Instance.OnDeath = OnPlayerDeath;

            Game.Instance.MatchStatus = Game.MatchState.Intermission;
            GameHud.Instance.state.Status = "Intermission";

            GameTimer.Instance.StartTimer(gameModeData.IntermissionTime);

            HostPlayerRegistry.Instance.DestroyAllPlayers();
            SpawnAllPlayers();

            GameMode.Instance.ApplyAllPlayerTeams();
        }

        private void EndIntermission()
        {
            if (BoltNetwork.IsClient)
            {
                return;
            }

            HostGameAction.Instance.OnTimerEnd = null;
            HostPlayerAction.Instance.OnDeath = null;

            HostGameAction.Instance.OnIntermissionEnd.Invoke();

            GameHud.Instance.state.Timer = -1;

            HostPlayerRegistry.Instance.DestroyAllPlayers();

            if (GameMode.Instance.Data.HasPreRound)
            {
                GameMode.Instance.PreStartRound();
            }
            else
            {
                GameMode.Instance.StartRound();
            }

            BoltGlobalEvent.SendCue(string.Empty, Color.white);
        }

        private void SpawnAllPlayers()
        {
            loungePlayerSpawner.ResetSpawns();

            for (var i = 0; i < HostPlayerRegistry.Instance.PlayerConnections.Count; i++)
            {
                loungePlayerSpawner.Spawn(HostPlayerRegistry.Instance.PlayerConnections[i]);
            }
        }

        private void OnPlayerDeath(PlayerConnection connection, Vector3 position, float yaw)
        {
            if (BoltNetwork.IsServer)
            {
                Timing.RunCoroutine(loungePlayerSpawner.SpawnWithDelay(connection, default(Vector3), 0.0f), "Respawn");
            }
        }
    }
}